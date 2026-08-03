Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Xml
Imports Ionic.Zip
Imports System.Collections.Generic
Imports GemBox.Document.Tables
Imports iTextSharp.text.pdf.events.IndexEvents
Imports Dynamsoft.DotNet.TWAIN.Barcode


Public Structure perfil_administrador
    Dim adm_docuarchi As Integer
    Dim adm_workflow As Integer
    Dim adm_radicacion As Integer
    Dim adm_gestor As Integer
    Dim anula_radicado As Integer
    Dim reasigna_documento As Integer
    Dim cambia_tipo_tramite As Integer
    Dim reversa_respuesta As Integer
    Dim actualiza_peticionario As Integer
End Structure
Public Class CdGestionrespuesta
    Property AppError As String

End Class
Public Class Classgestionrespuesta
    Function Descarga_plantilla_radicada_respuesta(ByVal id_respuesta As Integer,
                                                   ByRef radicado As String,
                                                   ByRef pag As Page,
                                                   ByRef ruta_virtual_salida As String,
                                                   ByVal id_usuario_gestion_respuesta As Integer) As String
        '------------------------------------------------------------------
        'Fucnion : Radica y descarga plantilla de protocolo de respuesta
        'Fecha : 2016-04-10
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim stru As stru_envio = Nothing
            Dim refclas_radicado As New ClassRadicador
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                        stru,
                                                                                        0)
            If Result <> "YES" Then
                Descarga_plantilla_radicada_respuesta = Result
                Exit Function
            End If
            radicado = stru.RADICADO
            Dim Refclas As New ClassRaEnvioCorrespondencia
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_virtual_tempo As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA"
            Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
            Dim ruta_fisica_dos As String = HttpContext.Current.Server.MapPath(ruta_virtual_tempo)
            If Directory.Exists(ruta_fisica_dos) = False Then
                Directory.CreateDirectory(ruta_fisica_dos)
            End If
            Dim refclas_gestion As New ClassGestion
            Dim conten As Object = Nothing
            Dim text_destinatario As TextBox = pag.FindControl("RE_REMITENTE_COR_REMITENTE_COR_VARCHAR_RA")
            If text_destinatario Is Nothing Then
                Descarga_plantilla_radicada_respuesta = "Imposible encontrar el control RE_REMITENTE_COR_REMITENTE_COR_VARCHAR "
                Exit Function
            End If
            Dim drowlist As DropDownList = pag.FindControl("RE_Descripcion_Documento_ra")
            If drowlist Is Nothing Then
                Descarga_plantilla_radicada_respuesta = "Imposible encontrar el control RE_Descripcion_Documento"
                Exit Function
            End If
            If stru.ESTADO_RESPUESTA = 1 Then
                Descarga_plantilla_radicada_respuesta = "El trámite tiene una respuesta formal, imposible descargar el formato de protocolo de respuesta para este trámite"
                Exit Function
            End If
            If stru.ESTADO_RESPUESTA = 5 Then
                Descarga_plantilla_radicada_respuesta = "El trámite tiene una confirmación, imposible descargar el formato de protocolo de respuesta para este trámite"
                Exit Function
            End If
            If stru.ESTADO_RESPUESTA = 6 Then
                Descarga_plantilla_radicada_respuesta = "El trámite se encuentra archivado, imposible descargar el formato de protocolo de respuesta para este trámite"
                Exit Function
            End If
            If stru.ESTADO_APROBACION = 1 Then
                Descarga_plantilla_radicada_respuesta = "El trámite tiene una respuesta aprobada, imposible descargar el formato de protocolo de respuesta para este trámite"
                Exit Function
            End If
            If stru.RADICADO_RESPUESTA = "" Then
                Dim consecutivo_radicado As String = ""
                Result = refclas_radicado.Registra_Radicado_plantilla_respuesta("RADICACIONSALIENTE",
                                                                                consecutivo_radicado,
                                                                                stru,
                                                                                id_respuesta,
                                                                                HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                text_destinatario.Text,
                                                                                drowlist.Text, 1)
                If Result <> "YES" Then
                    Descarga_plantilla_radicada_respuesta = Result
                    Exit Function
                End If
                Result = refclas_gestion.Descarga_documento_plantilla_respuesta_radicado(ruta_fisica,
                                                                                         ruta_virtual,
                                                                                         id_respuesta,
                                                                                         conten,
                                                                                         stru.RADICADO,
                                                                                         consecutivo_radicado,
                                                                                         id_usuario_gestion_respuesta)
                If Result <> "YES" Then
                    Descarga_plantilla_radicada_respuesta = Result
                    Exit Function
                End If
                Dim nombre_usuario As String = stru.DESTINATARIO.Replace("/", "-")
                nombre_usuario = nombre_usuario.Replace("\", "-")
                Result = Refclas.SaveFile(ruta_fisica & id_respuesta & "-" & nombre_usuario & ".docx", conten)
                If Result <> "YES" Then
                    Descarga_plantilla_radicada_respuesta = Result
                    Exit Function
                End If
                ruta_virtual_salida = ruta_virtual & id_respuesta & "-" & nombre_usuario & ".docx"
                Dim Ref_clas As New Class_DAT_ADIC_TAR
                If stru.ESTADO_APROBACION = 0 Then
                    Result = Ref_clas.Actualiza_estado_tramite_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                              "En tramite")
                    If Result <> "YES" Then
                        Descarga_plantilla_radicada_respuesta = Result
                        Exit Function
                    End If
                End If
                If Not HttpContext.Current.Session.Item("dat_gred_cahce") Is Nothing Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item(0) = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") Then
                            HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item("ESTADO") = "En tramite"
                            HttpContext.Current.Session.Item("dat_gred_cahce").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
                If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                    For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows.Count - 1
                        If HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item(0) = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") Then
                            HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item("ESTADO") = "En tramite"
                            HttpContext.Current.Session.Item("dat_gred_cahce_restore").AcceptChanges()
                            Exit For
                        End If
                    Next
                End If
                Descarga_plantilla_radicada_respuesta = "YES"
            Else
                '---------------------------------------------
                'Si no existe la plantilla la descarga
                '---------------------------------------------
                Result = refclas_gestion.Descarga_documento_plantilla_respuesta_radicado(ruta_fisica,
                                                                                         ruta_virtual,
                                                                                         id_respuesta,
                                                                                         conten,
                                                                                         stru.RADICADO,
                                                                                         stru.RADICADO_RESPUESTA,
                                                                                         id_usuario_gestion_respuesta)
                If Result <> "YES" Then
                    Descarga_plantilla_radicada_respuesta = Result
                    Exit Function
                End If
                Dim nombre_usuario As String = stru.DESTINATARIO.Replace("/", "-")
                nombre_usuario = nombre_usuario.Replace("\", "-")
                Result = Refclas.SaveFile(ruta_fisica & id_respuesta & "-" & nombre_usuario & ".docx", conten)
                If Result <> "YES" Then
                    Descarga_plantilla_radicada_respuesta = Result
                    Exit Function
                End If
                ruta_virtual_salida = ruta_virtual & id_respuesta & "-" & nombre_usuario & ".docx"
                Dim Ref_clas As New Class_DAT_ADIC_TAR
                If stru.ESTADO_APROBACION = 0 Then
                    Result = Ref_clas.Actualiza_estado_tramite_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                              "En tramite")
                    If Result <> "YES" Then
                        Descarga_plantilla_radicada_respuesta = Result
                        Exit Function
                    End If
                End If
                Descarga_plantilla_radicada_respuesta = "YES"
            End If
        Catch ex As Exception
            Descarga_plantilla_radicada_respuesta = "Inconsistencia general Función Descarga_plantilla_radicada_respuesta " & ex.Message
        End Try
    End Function
    Function Limpiar_campos_consulta_respuestas(
    ByVal FECHA_REGISTRO_INI As TextBox, ByVal FECHA_REGISTRO_FIN As TextBox, ByVal FECHA_VENCE_INI As TextBox,
    ByVal FECHA_VENCE_FIN As TextBox, ByVal FECHA_RESPUETA_INI As TextBox, ByVal FECHA_RESPUETA_FIN As TextBox,
    ByVal FECHA_ENVIO_INI As TextBox, ByVal FECHA_ENVIO_FIN As TextBox, ByVal DESTINATARIO As TextBox,
    ByVal ASUNTO As TextBox, ByVal RADICADO As TextBox, ByVal estadorespuesta As DropDownList,
    ByVal tiporespuesta As DropDownList,
    ByRef page1 As Page,
    ByVal RADICADO_RESPUESTA As TextBox, ByVal USUARIO_RESPONSABLE As TextBox, ByVal TRAMITE_DOCUMENTO As DropDownList, ByVal AREA_RESPONSABLE As DropDownList) As String
        Try
            Dim update_panel As UpdatePanel = page1.FindControl("UpdatePanelContenido_val_radicacion")
            If update_panel Is Nothing Then
                Limpiar_campos_consulta_respuestas = "Imposible encontrar el control UpdatePanelContenido_val_radicacion "
                Exit Function
            End If
            FECHA_REGISTRO_INI.Text = ""
            FECHA_REGISTRO_FIN.Text = ""
            FECHA_VENCE_INI.Text = ""
            FECHA_VENCE_FIN.Text = ""
            FECHA_RESPUETA_INI.Text = ""
            FECHA_RESPUETA_FIN.Text = ""
            FECHA_ENVIO_INI.Text = ""
            FECHA_ENVIO_FIN.Text = ""
            DESTINATARIO.Text = ""
            ASUNTO.Text = ""
            RADICADO.Text = ""
            RADICADO.Text = ""
            estadorespuesta.Text = "TODOS"
            tiporespuesta.Text = "TODOS"
            RADICADO_RESPUESTA.Text = ""
            USUARIO_RESPONSABLE.Text = ""
            TRAMITE_DOCUMENTO.Text = ""
            AREA_RESPONSABLE.Text = ""
            update_panel.Update()
            Limpiar_campos_consulta_respuestas = "YES"
        Catch ex As Exception
            Limpiar_campos_consulta_respuestas = "Inconsistencia general función Limpiar_campos_consulta_respuestas " & ex.Message
        End Try
    End Function
    Function Consulta_historial_respuestas(ByVal id_respuesta_inicial As String,
                                           ByVal id_respuesta_final As String,
                                           ByVal FECHA_REGISTRO_INI As String,
                                           ByVal FECHA_REGISTRO_FIN As String,
                                           ByVal FECHA_VENCE_INI As String,
                                           ByVal FECHA_VENCE_FIN As String,
                                           ByVal FECHA_RESPUETA_INI As String,
                                           ByVal FECHA_RESPUETA_FIN As String,
                                           ByVal FECHA_ENVIO_INI As String,
                                           ByVal FECHA_ENVIO_FIN As String,
                                           ByVal DESTINATARIO As String,
                                           ByVal ASUNTO As String,
                                           ByVal RADICADO As String,
                                           ByVal estadorespuesta As String,
                                           ByVal tiporespuesta As String,
                                           ByVal opcion_todos_usuarios As Integer,
                                           ByVal id_usuario_gestion As Integer,
                                           ByRef page1 As Page,
                                           ByVal RADICADO_RESPUESTA As String,
                                           ByVal USUARIO_RESPONSABLE As String,
                                           ByVal TRAMITE_DOCUMENTO As String,
                                           ByVal AREA_RESPONSABLE As String,
                                           ByVal top_limite As Integer,
                                           ByVal tipo_consulta As Integer,
                                           ByVal valor_consulta As String) As String
        Try
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            If scripma Is Nothing Then
                Consulta_historial_respuestas = "Imposible encontrar datagrid GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Consulta_historial_respuestas = "Imposible encontrar el control titulo_label"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Consulta_historial_respuestas = "Imposible encontrar el control  UpdatePanelabel_validacion"
                Exit Function
            End If
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            If updatelabel Is Nothing Then
                Consulta_historial_respuestas = "Imposible encontrar el control  UpdatePanel_conenido_grid_val_radicacion"
                Exit Function
            End If
            Dim activaand As Integer = -1
            Dim sql_condicion As String = ""
            If tipo_consulta = 1 Then
                If opcion_todos_usuarios = 0 Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ID_REMIT_DEST_INT=" & id_usuario_gestion
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " and ID_REMIT_DEST_INT=" & id_usuario_gestion
                        activaand = 1
                    End If
                End If
                If estadorespuesta <> "" And estadorespuesta <> "TODOS" Then
                    If activaand = -1 Then
                        If estadorespuesta = "ENVIADOS AL SOLICITANTE O PETICIONARIO" Then
                            sql_condicion = sql_condicion & " where ESTADO_RESPUESTA=2 "
                            activaand = 1
                        End If
                        If estadorespuesta = "TRAMITES CON RESPUESTA" Then
                            sql_condicion = sql_condicion & " where fecha_respueta is not null "
                            activaand = 1
                        End If
                        If estadorespuesta = "PENDIENTES POR ENVIAR AL SOLICITANTE O PETICIONARIO" Then
                            sql_condicion = sql_condicion & " where ESTADO_RESPUESTA=1 "
                            activaand = 1
                        End If
                        If estadorespuesta = "PENDIENTES POR RESPONDER" Then
                            sql_condicion = sql_condicion & " where fecha_respueta is null "
                            activaand = 1
                        End If
                    Else
                        If estadorespuesta = "ENVIADOS AL SOLICITANTE O PETICIONARIO" Then
                            sql_condicion = sql_condicion & " and ESTADO_RESPUESTA=2 "
                            activaand = 1
                        End If
                        If estadorespuesta = "TRAMITES CON RESPUESTA" Then
                            sql_condicion = sql_condicion & " and fecha_respueta is not null "
                            activaand = 1
                        End If
                        If estadorespuesta = "PENDIENTES POR ENVIAR AL SOLICITANTE O PETICIONARIO" Then
                            sql_condicion = sql_condicion & " and ESTADO_RESPUESTA=1 "
                            activaand = 1
                        End If
                        If estadorespuesta = "PENDIENTES POR RESPONDER" Then
                            sql_condicion = sql_condicion & " and fecha_respueta is null "
                            activaand = 1
                        End If
                    End If
                End If
                If tiporespuesta <> "" And tiporespuesta <> "TODOS" Then
                    If activaand = -1 Then
                        If tiporespuesta = "RESPUESTA CON RADICADO" Then
                            sql_condicion = sql_condicion & " where RADICADO_RESPUESTA IS NOT NULL "
                            activaand = 1
                        End If
                        If tiporespuesta = "SOLO CONFIRMACION" Then
                            sql_condicion = sql_condicion & " where RADICADO_RESPUESTA IS NULL AND ESTADO_RESPUESTA <> 0"
                            activaand = 1
                        End If

                    Else
                        If tiporespuesta = "RESPUESTA CON RADICADO" Then
                            sql_condicion = sql_condicion & " AND RADICADO_RESPUESTA IS NOT  NULL "
                            activaand = 1
                        End If
                        If tiporespuesta = "SOLO CONFIRMACION" Then
                            sql_condicion = sql_condicion & " AND RADICADO_RESPUESTA IS NULL AND ESTADO_RESPUESTA <> 0"
                            activaand = 1
                        End If
                    End If

                End If
                If DESTINATARIO <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where DESTINATARIO='" & DESTINATARIO & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND DESTINATARIO='" & DESTINATARIO & "'"
                    End If

                End If
                If ASUNTO <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ASUNTO='" & ASUNTO & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ASUNTO='" & ASUNTO & "'"
                    End If

                End If
                If RADICADO <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where RADICADO='" & RADICADO & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND RADICADO='" & RADICADO & "'"
                    End If

                End If
                If RADICADO_RESPUESTA <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where RADICADO_RESPUESTA='" & RADICADO_RESPUESTA & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND RADICADO_RESPUESTA='" & RADICADO_RESPUESTA & "'"
                    End If
                End If
                If USUARIO_RESPONSABLE <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where USUARIO_RESPONSABLE='" & USUARIO_RESPONSABLE & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND USUARIO_RESPONSABLE='" & USUARIO_RESPONSABLE & "'"
                    End If
                End If
                If TRAMITE_DOCUMENTO <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where TRAMITE_DOCUMENTO='" & TRAMITE_DOCUMENTO & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND TRAMITE_DOCUMENTO='" & TRAMITE_DOCUMENTO & "'"
                    End If
                End If
                If AREA_RESPONSABLE <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where AREA_RESPONSABLE='" & AREA_RESPONSABLE & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND AREA_RESPONSABLE='" & AREA_RESPONSABLE & "'"
                    End If
                End If
                If id_respuesta_inicial <> "" And id_respuesta_final <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ID_RESPUESTA_RADICADO BETWEEN '" & id_respuesta_inicial & "' AND '" &
                        id_respuesta_final & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ID_RESPUESTA_RADICADO BETWEEN '" & id_respuesta_inicial & "' AND '" &
                       id_respuesta_final & "'"
                    End If
                Else
                    If id_respuesta_inicial <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where ID_RESPUESTA_RADICADO='" & id_respuesta_inicial & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND ID_RESPUESTA_RADICADO='" & id_respuesta_inicial & "'"
                        End If
                    End If
                    If id_respuesta_final <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where ID_RESPUESTA_RADICADO='" & id_respuesta_final & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND ID_RESPUESTA_RADICADO='" & id_respuesta_final & "'"
                        End If
                    End If
                End If
                If FECHA_REGISTRO_INI <> "" And FECHA_REGISTRO_FIN <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where CAST(FECHA_REGISTRO AS DATE) BETWEEN '" & FECHA_REGISTRO_INI & "' AND '" &
                        FECHA_REGISTRO_FIN & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND CAST(FECHA_REGISTRO AS DATE) BETWEEN '" & FECHA_REGISTRO_INI & "' AND '" &
                       FECHA_REGISTRO_FIN & "'"
                    End If
                Else
                    If FECHA_REGISTRO_INI <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where CAST(FECHA_REGISTRO AS DATE)='" & FECHA_REGISTRO_INI & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND CAST(FECHA_REGISTRO AS DATE)='" & FECHA_REGISTRO_INI & "'"
                        End If
                    End If
                    If FECHA_REGISTRO_FIN <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where CAST(FECHA_REGISTRO AS DATE)='" & FECHA_REGISTRO_FIN & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND CAST(FECHA_REGISTRO AS DATE)='" & FECHA_REGISTRO_FIN & "'"
                        End If
                    End If
                End If
                If FECHA_VENCE_INI <> "" And FECHA_VENCE_FIN <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where CAST(FECHA_VENCE BETWEEN AS DATE) '" & FECHA_VENCE_INI & "' AND '" &
                        FECHA_VENCE_FIN & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND CAST(FECHA_VENCE BETWEEN AS DATE) '" & FECHA_VENCE_INI & "' AND '" &
                       FECHA_VENCE_FIN & "'"
                    End If
                Else
                    If FECHA_VENCE_INI <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where CAST(FECHA_VENCE AS DATE)='" & FECHA_VENCE_INI & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND CAST(FECHA_VENCE AS DATE)='" & FECHA_VENCE_INI & "'"
                        End If
                    End If
                    If FECHA_VENCE_FIN <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where CAST(FECHA_VENCE AS DATE)='" & FECHA_VENCE_FIN & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND CAST(FECHA_VENCE AS DATE)='" & FECHA_VENCE_FIN & "'"
                        End If
                    End If
                End If

                If FECHA_RESPUETA_INI <> "" And FECHA_RESPUETA_FIN <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where CAST(FECHA_RESPUETA AS DATE) BETWEEN '" & FECHA_RESPUETA_INI & "' AND '" &
                        FECHA_RESPUETA_FIN & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND CAST(FECHA_RESPUETA AS DATE) BETWEEN '" & FECHA_RESPUETA_INI & "' AND '" &
                       FECHA_RESPUETA_FIN & "'"
                    End If
                Else
                    If FECHA_RESPUETA_INI <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where CAST(FECHA_RESPUETA AS DATE)='" & FECHA_RESPUETA_INI & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND CAST(FECHA_RESPUETA AS DATE)='" & FECHA_RESPUETA_INI & "'"
                        End If
                    End If
                    If FECHA_RESPUETA_FIN <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where CAST(FECHA_RESPUETA AS DATE)='" & FECHA_RESPUETA_FIN & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND CAST(FECHA_RESPUETA AS DATE)='" & FECHA_RESPUETA_FIN & "'"
                        End If
                    End If
                End If

                If FECHA_ENVIO_INI <> "" And FECHA_ENVIO_FIN <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where CAST(FECHA_ENVIO AS DATE) BETWEEN '" & FECHA_ENVIO_INI & "' AND '" &
                        FECHA_ENVIO_FIN & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND CAST(FECHA_ENVIO AS DATE) BETWEEN '" & FECHA_RESPUETA_INI & "' AND '" &
                       FECHA_RESPUETA_FIN & "'"
                    End If
                Else
                    If FECHA_ENVIO_INI <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where CAST(FECHA_ENVIO AS DATE)='" & FECHA_ENVIO_INI & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND CAST(FECHA_ENVIO AS DATE)='" & FECHA_ENVIO_INI & "'"
                        End If
                    End If
                    If FECHA_ENVIO_FIN <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where CAST(FECHA_ENVIO AS DATE)='" & FECHA_ENVIO_FIN & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND CAST(FECHA_ENVIO AS DATE)='" & FECHA_ENVIO_FIN & "'"
                        End If
                    End If
                End If
            End If
            If tipo_consulta = 2 Then
                Dim Sql_condicion_ As String = ""
                If opcion_todos_usuarios = 0 Then
                    Sql_condicion_ = "ID_REMIT_DEST_INT = " & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & " and  "
                End If
                sql_condicion = " where " & Sql_condicion_ &
                   " ( DESTINATARIO like '%" & valor_consulta & "%'" &
                   " or ASUNTO like '%" & valor_consulta & "%'" &
                   " or RADICADO like '%" & valor_consulta & "%'" &
                   " or RADICADO_RESPUESTA like '%" & valor_consulta & "%'" &
                   " or USUARIO_RESPONSABLE like '%" & valor_consulta & "%'" &
                   " or TRAMITE_DOCUMENTO like '%" & valor_consulta & "%'" &
                   " or AREA_RESPONSABLE like '%" & valor_consulta & "%'" &
                   " or ID_RESPUESTA_RADICADO like '%" & valor_consulta & "%'" &
                   " or FECHA_REGISTRO like '%" & valor_consulta & "%'" &
                   " or FECHA_VENCE like '%" & valor_consulta & "%'" &
                   " or FECHA_RESPUETA like '%" & valor_consulta & "%'" &
                   " or FECHA_ENVIO like '%" & valor_consulta & "%')"
            End If
            Dim sql_consulta As String = "SELECT ID_RESPUESTA_RADICADO as ID,TRAMITE_DOCUMENTO,RADICADO,RADICADO_RESPUESTA,FECHA_REGISTRO AS FECHA_RADICACION,FECHA_VENCE," &
                 "FECHA_RESPUETA,DESTINATARIO,USUARIO_RESPONSABLE,ASUNTO " &
                 " FROM ra_respuesta_radicado " &
                  sql_condicion & " limit " & top_limite
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_historial_respuestas = "Imposible Encontrar datos para listar " & Result
                labetitle.Text = "Se encontraron (" & "0" & ") registro(s) de respuesta (s) "
                scripma.DataSource = Nothing
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontraron (" & Datset.Tables(0).Rows.Count & ") registro(s) de respuesta (s) "
                scripma.DataSource = Nothing
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Consulta_historial_respuestas = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("RA_DATO_CONSULTA") = sql_consulta
                labetitle.Text = "Se encontraron (" & Datset.Tables(0).Rows.Count & ") registro(s) de respuesta (s) "
                scripma.DataSource = Datset
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-file-image")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver imagen del radicado")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "V-I-H-R")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-table")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn btn-info btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Detalle respuesta radicado")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "D-D-R-R")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-list-ol")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Transacciones de la respuesta")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "D-V-D-T")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-project-diagram")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Estados del radicado")
                    ahtml.Attributes.Add("idd", scripma.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "D-E-D-R")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)


                    divhtml.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                updat.Update()
                updatelabel.Update()
                Consulta_historial_respuestas = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_historial_respuestas = "Inconsistencia Consulta_historial_respuestas " & ex.Message
        End Try
    End Function
    Function Actualiza_guarda_documento_respuesta_chkeditor(ByVal id_respuesta As Integer, ByVal texto_documento As String, ByRef id_imagen_guardada As Integer) As String
        Try
            Dim Result As String = ""
            Dim stru As stru_envio = Nothing
            id_imagen_guardada = 0
            Dim refclas_gestion As New Classgestionrespuesta
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, stru, 0)
            If Result <> "YES" Then
                Actualiza_guarda_documento_respuesta_chkeditor = Result
                Exit Function
            End If

            If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                Actualiza_guarda_documento_respuesta_chkeditor = "Por favor informe la url web service para workflow"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("URL_WEB_SERVICE") = "" Then
                Actualiza_guarda_documento_respuesta_chkeditor = "Por favor informe la url web service para workflow"
                Exit Function
            End If
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                 Radicado)
            If Result <> "YES" Then
                Actualiza_guarda_documento_respuesta_chkeditor = Result
                Exit Function
            End If
            If Radicado = "" Then
                Actualiza_guarda_documento_respuesta_chkeditor = "La tarea seleccionada no tiene radicado relacionado "
                Exit Function
            End If
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
            Dim conten As Object = Nothing
            'Dim OB As New localhost.Service
            'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
            'Result = OB.Donwload_archivo_plantilla(conten)
            'If Result <> "YES" Then
            '    Actualiza_guarda_documento_respuesta_chkeditor = Result
            '    Exit Function
            'End If
            Dim Refclas_sav_file As New ClassRaEnvioCorrespondencia
            Result = Refclas_sav_file.SaveFile(ruta_fisica & "template_rut" & ".docx", conten)
            If Result <> "YES" Then
                Actualiza_guarda_documento_respuesta_chkeditor = Result
                Exit Function
            End If
            Dim document As GemBox.Document.DocumentModel
            Using stream = File.OpenRead(ruta_fisica & "template_rut" & ".docx")
                document = GemBox.Document.DocumentModel.Load(stream, GemBox.Document.LoadOptions.DocxDefault)
            End Using
            'Limpia los metadatos para que no salgan el chkeditor
            document.DocumentProperties.Custom.Clear()
            document.DocumentProperties.BuiltIn.Clear()
            For Each picture As GemBox.Document.Picture In document.GetChildElements(True, GemBox.Document.ElementType.Picture)
                picture.Layout = GemBox.Document.Layout.Inline(picture.Layout.Size)
            Next
            Dim str As String = texto_documento.ToString().Replace("<title></title>", "")
            Dim g = "<meta content=" & """" & "text/html; charset=utf-8" & """" & "http-equiv=" & """" & "content-type" & """" & "/>"
            str = str.Replace("<meta content=" & """" & "text/html; charset=utf-8" & """" & "http-equiv=" & """" & "content-type" & """" & "/>", "")
            str = str.Replace("margin: 0pt", "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt")
            'str = str.Replace("<p style=" & """" & "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;" & """" & ">", "")
            For Each item As GemBox.Document.ContentRange In document.Content.Find("Resp_conte_web")
                item.LoadText(str, GemBox.Document.LoadOptions.HtmlDefault)
            Next
            If stru.ID_IMAGEN <> 0 Then
                Dim refclasvisor As New ClassVisualisaDocumento
                Dim matri_documentos() As String = Nothing
                refclasvisor.Genera_Matris_Documentos_Almacenados(stru.ID_IMAGEN, "IMP03GESTIONTMP", matri_documentos)
                If Result <> "YES" Then
                    Actualiza_guarda_documento_respuesta_chkeditor = Result
                    Exit Function
                End If

                document.Save(matri_documentos(1), New GemBox.Document.DocxSaveOptions() With {.Format = GemBox.Document.DocxFormat.Docx})
            Else
                Dim ruta_virtual_rut As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DONWLOAD/"
                Dim archivo_donwload As String = HttpContext.Current.Server.MapPath(ruta_virtual_rut) & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & ".docx"
                document.Save(archivo_donwload, GemBox.Document.DocxSaveOptions.DocxDefault)
                Result = Me.Guardar_Documento_Respuesta(id_imagen_guardada,
                                                        "IMP03GESTIONTMP",
                                                        id_respuesta,
                                                        archivo_donwload,
                                                        1)
                If Result <> "YES" Then
                    Actualiza_guarda_documento_respuesta_chkeditor = Result
                    Exit Function
                Else
                    If File.Exists(archivo_donwload) Then
                        Kill(archivo_donwload)
                    End If
                End If
            End If

            Actualiza_guarda_documento_respuesta_chkeditor = "YES"
        Catch ex As Exception
            Actualiza_guarda_documento_respuesta_chkeditor = "Inconsistencia general funcion Actualiza_guarda_documento_respuesta_chkeditor " & ex.Message
        End Try
    End Function

    Function Asigna_respuesta_inicial_chkeditor(ByRef htmlEditor As Object, ByRef pag As Page, ByRef id_resp As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                 Radicado)
            If Result <> "YES" Then
                Asigna_respuesta_inicial_chkeditor = Result
                Exit Function
            End If
            If Radicado = "" Then
                Asigna_respuesta_inicial_chkeditor = "La tarea seleccionada no tiene radicado relacionado "
                Exit Function
            End If
            Dim refclas_resp As New Classgestionrespuesta
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim id_respuesta As Integer = 0
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                               id_respuesta)
            If Result <> "YES" Then
                Asigna_respuesta_inicial_chkeditor = Result
                Exit Function
            End If
            id_resp = id_respuesta
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, stru, 1)
            If Result <> "YES" Then
                Asigna_respuesta_inicial_chkeditor = Result
                Exit Function
            End If
            If stru.ID_IMAGEN <> 0 Then
                Dim refclas_visualiza As New ClassVisualisaDocumento
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
                Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
                Dim matri_documento() As String = Nothing
                Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(stru.ID_IMAGEN, "IMP03GESTIONTMP", matri_documento)
                If Result <> "YES" Then
                    Asigna_respuesta_inicial_chkeditor = Result
                    Exit Function
                End If
                Dim document = GemBox.Document.DocumentModel.Load(matri_documento(1), GemBox.Document.LoadOptions.DocxDefault)
                'Limpia los metadatos para que no salgan el chkeditor
                document.DocumentProperties.Custom.Clear()
                document.DocumentProperties.BuiltIn.Clear()
                'Elimina los Footers para que no salga en el chkeditor
                document.Sections(0).HeadersFooters.Clear()
                'Elimina el posicionamiento de las imagenes
                For Each picture As GemBox.Document.Picture In document.GetChildElements(True, GemBox.Document.ElementType.Picture)
                    picture.Layout = GemBox.Document.Layout.Inline(picture.Layout.Size)
                Next
                'Elimina el posicionamiento de las tablas
                For Each ob As GemBox.Document.Tables.Table In document.GetChildElements(True, GemBox.Document.ElementType.Table).Cast(Of Table)()
                    'ob.TableFormat.Positioning.ClearPositioning()
                    'ob.TableFormat.Alignment = GemBox.Document.HorizontalAlignment.Right
                    ob.TableFormat.Alignment = GemBox.Document.HorizontalAlignment.Left

                Next
                Dim settings As XmlWriterSettings = New XmlWriterSettings()
                settings.OmitXmlDeclaration = True
                settings.ConformanceLevel = ConformanceLevel.Fragment
                settings.CloseOutput = False
                Using sw = New StringWriter()
                    Using xw = XmlWriter.Create(sw, settings)
                        document.Save(xw, New GemBox.Document.HtmlSaveOptions() With {.EmbedImages = True, .UseSemanticElements = False, .HtmlType = GemBox.Document.HtmlType.HtmlInline})
                        Dim str As String = sw.ToString().Replace("<title></title>", "")
                        'str = str.Replace("margin-left:-27pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;mso-pagination:lines-together;" & """" & ">", "")
                        str = str.Replace("<span>&#xa0;</span></p>", "")
                        'str = str.Replace("<p style=" & """" & "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;" & """" & ">", "") 
                        'str = str.Replace("<p style=" & """" & "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;" & """" & ">", "")
                        str = str.Replace("<span style=" & """" & "font-family:Bell MT;font-size:7pt;" & """" & "> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</span></p>", "")
                        htmlEditor.Text = str

                    End Using
                End Using
            Else

                If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                    Asigna_respuesta_inicial_chkeditor = "Por favor active web service para workflow"
                    Exit Function
                End If
                If HttpContext.Current.Session.Item("URL_WEB_SERVICE") = "" Then
                    Asigna_respuesta_inicial_chkeditor = "Por favor active web service para workflow"
                    Exit Function
                End If
                Dim refclas_gestion As New ClassGestion
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
                Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
                Dim conten As Object = Nothing
                If HttpContext.Current.Session.Item("GA_TIPO_MODULO_RESPUESTA") = 0 Then
                    Result = refclas_gestion.Descarga_documento_plantilla_respuesta(pag, ruta_fisica, ruta_virtual, id_respuesta, conten, Radicado)
                    If Result <> "YES" Then
                        Asigna_respuesta_inicial_chkeditor = Result
                        Exit Function
                    End If
                    Dim Refclas_sav_file As New ClassRaEnvioCorrespondencia
                    Result = Refclas_sav_file.SaveFile(ruta_fisica & "template_rut" & ".docx", conten)
                    If Result <> "YES" Then
                        Asigna_respuesta_inicial_chkeditor = Result
                        Exit Function
                    End If
                End If
                If HttpContext.Current.Session.Item("GA_TIPO_MODULO_RESPUESTA") = 1 Then
                    Dim ruta_virtual_rep As String = ""
                    Dim radicado_resp As String = ""
                    Dim refclas_gestion_resp As New ClassGestion
                    Result = refclas_gestion_resp.Descarga_documento_plantilla_respuesta_radicado(ruta_fisica, ruta_virtual, id_respuesta, conten, Radicado, stru.RADICADO_RESPUESTA, 1)
                    If Result <> "YES" Then
                        Asigna_respuesta_inicial_chkeditor = Result
                        Exit Function
                    End If
                    Dim Refclas_sav_file As New ClassRaEnvioCorrespondencia
                    Result = Refclas_sav_file.SaveFile(ruta_fisica & "template_rut" & ".docx", conten)
                    If Result <> "YES" Then
                        Asigna_respuesta_inicial_chkeditor = Result
                        Exit Function
                    End If
                End If

                Dim document = GemBox.Document.DocumentModel.Load(ruta_fisica & "template_rut" & ".docx", GemBox.Document.LoadOptions.DocxDefault)
                'Limpia los metadatos para que no salgan el chkeditor
                document.DocumentProperties.Custom.Clear()
                document.DocumentProperties.BuiltIn.Clear()
                'Elimina los Footers para que no salga en el chkeditor
                document.Sections(0).HeadersFooters.Clear()
                'Elimina el posicionamiento de las imagenes
                For Each picture As GemBox.Document.Picture In document.GetChildElements(True, GemBox.Document.ElementType.Picture)
                    picture.Layout = GemBox.Document.Layout.Inline(picture.Layout.Size)
                Next
                'Elimina el posicionamiento de las tablas
                For Each ob As GemBox.Document.Tables.Table In document.GetChildElements(True, GemBox.Document.ElementType.Table).Cast(Of Table)()
                    'ob.TableFormat.Positioning.ClearPositioning()
                    ob.TableFormat.Alignment = GemBox.Document.HorizontalAlignment.Left
                Next
                Dim settings As XmlWriterSettings = New XmlWriterSettings()
                settings.OmitXmlDeclaration = True
                settings.ConformanceLevel = ConformanceLevel.Fragment
                settings.CloseOutput = False
                Using sw = New StringWriter()
                    Using xw = XmlWriter.Create(sw, settings)
                        document.Save(xw, New GemBox.Document.HtmlSaveOptions() With {.EmbedImages = True, .UseSemanticElements = False, .HtmlType = GemBox.Document.HtmlType.HtmlInline})
                        Dim str As String = sw.ToString().Replace("<title></title>", "")
                        str = str.Replace("margin-left:-27pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;mso-pagination:lines-together;" & """" & ">", "")
                        str = str.Replace("<span>&#xa0;</span></p>", "")
                        'str = str.Replace("<p style=" & """" & "<p style=" & """" & "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;" & """" & ">", "")
                        'str = str.Replace("<p style=" & """" & "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;" & """" & ">", "")
                        str = str.Replace("<span style=" & """" & "font-family:Bell MT;font-size:7pt;" & """" & "> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</span></p>", "")
                        htmlEditor.Text = str

                    End Using
                End Using
            End If
            Asigna_respuesta_inicial_chkeditor = "YES"
        Catch ex As Exception
            Asigna_respuesta_inicial_chkeditor = "Inconsistencia genera función " & ex.Message
        End Try
    End Function
    Function Actualiza_nota_respuesta(ByVal id_respuesta_radicado As Integer,
                                      ByVal nota_respuesta_radicado As String) As String
        Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
        Dim Result As String = ""
        Dim ref_nota_respuesta As String = ""
        If nota_respuesta_radicado = "" Then
            ref_nota_respuesta = "null"
        Else
            ref_nota_respuesta = "'" & nota_respuesta_radicado & "'"
        End If
        Dim hor As String = Now
        Dim date1al As String = Date.Now
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Actualiza_nota_respuesta = Result
            Exit Function
        End If

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            Dim insert_datos_envio As String = "('" & "ACTUALIZA NOTA RESPUESTA" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") &
                "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                        id_respuesta_radicado & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WEB-WORKFLOW','" & "" & "')"
            Dim update_log As String = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                                insert_datos_envio
            Dim update_nota_respuesta As String = "UPDATE ra_respuesta_radicado " &
                  " set NOTA_RESPUESTA=" & ref_nota_respuesta &
                 " where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_nota_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_nota_respuesta = "Imposible actualizar la nota de la respuesta  "
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_log
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Actualiza_nota_respuesta = "Imposible registrar log de actualización de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_nota_respuesta = "YES"
        Catch ex As MySqlException
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Actualiza_nota_respuesta = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Actualiza_nota_respuesta = "Error General " & ex.Message
            Exit Function
        End Try

    End Function
    Function Elimina_documento_respuesta(ByVal id_respuesta_radicado As Integer,
                                         ByRef image_url As String) As String
        Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
        Dim Refclass_ra_respuesta As New Class_ra_respuesta_radicado
        Dim ref_clas_solicitudes As New ClassRaSolicitudesAprobacion
        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
        Dim Refclas_ga_gabinete As New ClassDaGabinete
        Dim Refclas_gestion_fecha As New ClassGestionFechas
        Dim Refclas_system As New Class_system_plantilla_radicado
        Dim refclas_tip_doc_entrante As New Class_tipo_doc_entrante
        Dim ref_class_gabinete As New ClassEliminarDocListResult
        Dim Result As String = ""
        Dim date1al As String = ""
        Dim hor As String = Now
        Dim stru_envi As stru_envio = Nothing
        Dim estado As String = ""
        Dim id_imagen_plantilla As Integer = 0
        Dim radicado_respuesta As Integer = 0
        Dim fecha_respuesta As Integer = 0
        Dim id_imagen_respuesta As Integer = 0
        Dim estado_envio_respuesta As Integer = 0
        Dim existencia_imagen As String = ""
        Dim nombre_plantilla_radicado As String = ""
        Dim id_tipo_tramite As Integer = 0
        Dim id_tipo_envio_respuesta As Integer = 0
        Result = Refclass_ra_respuesta.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                       stru_envi)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = ref_clas_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta_radicado,
                                                                                                 0,
                                                                                                 estado,
                                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        If estado = "YES" Then
            Elimina_documento_respuesta = "El sistema ha detectado solicitudes de aprobación pendientes por decisión, imposible eliminar la plantilla. "
            Exit Function
        End If
        estado = ""
        Result = ref_clas_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta_radicado,
                                                                                                 1,
                                                                                                 estado,
                                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        If estado = "YES" Then
            Elimina_documento_respuesta = "El sistema ha detectado una solicitud donde se aprueba el documento de respuesta, imposible eliminar la plantilla"
            Exit Function
        End If
        Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado,
                                                                            id_imagen_plantilla,
                                                                            radicado_respuesta,
                                                                            fecha_respuesta,
                                                                            id_imagen_respuesta,
                                                                            estado_envio_respuesta)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        If fecha_respuesta <> 0 Then
            Elimina_documento_respuesta = "El tramite ya tiene una respuesta publicada, no puede eliminar el documento"
            Exit Function
        End If
        If id_imagen_plantilla = 0 Then
            Elimina_documento_respuesta = "No hay documento para eliminar"
            Exit Function
        End If
        Result = Refclas_ga_gabinete.Solicita_existencia_imagen_gabinete(stru_envi.ID_IMAGEN,
                                                                         "IMP03GESTIONTMP",
                                                                         existencia_imagen)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(Date.Now,
                                                                     date1al)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = Refclas_system.Solicita_nombre_plantilla_radicado(stru_envi.system_plantilla_radicado_id_plantilla,
                                                                   nombre_plantilla_radicado)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = Refclas_system.Solicita_id_tipo_tramite_plantilla_radicado(stru_envi.RADICADO,
                                                                            nombre_plantilla_radicado,
                                                                            id_tipo_tramite)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = refclas_tip_doc_entrante.Solicita_tipo_envio_respuesta(id_tipo_tramite,
                                                                        id_tipo_envio_respuesta)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            Dim insert_datos_envio As String = "('" & "ELIMINA PLANTILLA RESPUESTA" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" _
                                               & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                               id_respuesta_radicado & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor &
                                               "','WEB-WORKFLOW','" & "" & "')"
            Dim update_log As String = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                                insert_datos_envio
            Dim update_nota_respuesta As String = "UPDATE ra_respuesta_radicado " &
                  " set ID_IMAGEN=" & "null" &
                 " where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_nota_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_documento_respuesta = "Imposible actualizar la nota de la respuesta  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_log
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_documento_respuesta = "Imposible registrar log de actualización de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If existencia_imagen = "YES" Then
                Result = ref_class_gabinete.EliminarDocumentosGabinete(stru_envi.ID_IMAGEN,
                                                                             0,
                                                                             "IMP03GESTIONTMP",
                                                                             0,
                                                                             0,
                                                                             1,
                                                                            -1,
                                                                            "GESTIONCORRESPONDENCIA")
                If Result <> "YES" Then
                    Elimina_documento_respuesta = Result
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            If Result <> "YES" Then
                Elimina_documento_respuesta = Result
                Exit Function
            End If
            If id_tipo_envio_respuesta = 0 Then
                Result = Refclass_ra_respuesta.Solicita_estados_semaforo_respuesta(id_respuesta_radicado, image_url)
                If Result <> "YES" Then
                    Elimina_documento_respuesta = Result
                    Exit Function
                End If
            Else
                Result = Refclass_ra_respuesta.Solicita_estados_semaforo_respuesta_electronica(id_respuesta_radicado, image_url)
                If Result <> "YES" Then
                    Elimina_documento_respuesta = Result
                    Exit Function
                End If
            End If
            Elimina_documento_respuesta = "YES"
        Catch ex As MySqlException
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Elimina_documento_respuesta = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Elimina_documento_respuesta = "Error General " & ex.Message
            Exit Function
        End Try
    End Function
    Function Elimina_documento_respuesta(ByVal id_respuesta_radicado As Integer,
                                         ByRef Image As System.Web.UI.WebControls.Image) As String
        Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
        Dim Refclass_ra_respuesta As New Class_ra_respuesta_radicado
        Dim ref_clas_solicitudes As New ClassRaSolicitudesAprobacion
        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
        Dim Refclas_ga_gabinete As New ClassDaGabinete
        Dim Refclas_gestion_fecha As New ClassGestionFechas
        Dim Refclas_system As New Class_system_plantilla_radicado
        Dim refclas_tip_doc_entrante As New Class_tipo_doc_entrante
        Dim ref_class_gabinete As New ClassEliminarDocListResult
        Dim Result As String = ""
        Dim date1al As String = ""
        Dim hor As String = Now
        Dim stru_envi As stru_envio = Nothing
        Dim estado As String = ""
        Dim id_imagen_plantilla As Integer = 0
        Dim radicado_respuesta As Integer = 0
        Dim fecha_respuesta As Integer = 0
        Dim id_imagen_respuesta As Integer = 0
        Dim estado_envio_respuesta As Integer = 0
        Dim existencia_imagen As String = ""
        Dim nombre_plantilla_radicado As String = ""
        Dim id_tipo_tramite As Integer = 0
        Dim id_tipo_envio_respuesta As Integer = 0
        Result = Refclass_ra_respuesta.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                       stru_envi)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = ref_clas_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta_radicado,
                                                                                                 0,
                                                                                                 estado,
                                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        If estado = "YES" Then
            Elimina_documento_respuesta = "El sistema ha detectado solicitudes de aprobación pendientes por decisión, imposible eliminar la plantilla. "
            Exit Function
        End If
        estado = ""
        Result = ref_clas_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta_radicado,
                                                                                                 1,
                                                                                                 estado,
                                                                                                 HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        If estado = "YES" Then
            Elimina_documento_respuesta = "El sistema ha detectado una solicitud donde se aprueba el documento de respuesta, imposible eliminar la plantilla"
            Exit Function
        End If
        Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado,
                                                                            id_imagen_plantilla,
                                                                            radicado_respuesta,
                                                                            fecha_respuesta,
                                                                            id_imagen_respuesta,
                                                                            estado_envio_respuesta)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        If fecha_respuesta <> 0 Then
            Elimina_documento_respuesta = "El tramite ya tiene una respuesta publicada, no puede eliminar el documento"
            Exit Function
        End If
        If id_imagen_plantilla = 0 Then
            Elimina_documento_respuesta = "No hay documento para eliminar"
            Exit Function
        End If
        Result = Refclas_ga_gabinete.Solicita_existencia_imagen_gabinete(stru_envi.ID_IMAGEN,
                                                                         "IMP03GESTIONTMP",
                                                                         existencia_imagen)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(Date.Now,
                                                                     date1al)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = Refclas_system.Solicita_nombre_plantilla_radicado(stru_envi.system_plantilla_radicado_id_plantilla,
                                                                   nombre_plantilla_radicado)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = Refclas_system.Solicita_id_tipo_tramite_plantilla_radicado(stru_envi.RADICADO,
                                                                            nombre_plantilla_radicado,
                                                                            id_tipo_tramite)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Result = refclas_tip_doc_entrante.Solicita_tipo_envio_respuesta(id_tipo_tramite,
                                                                        id_tipo_envio_respuesta)
        If Result <> "YES" Then
            Elimina_documento_respuesta = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction = Nothing
        Dim sqlresultinsert As Integer = 0
        Try
            Dim insert_datos_envio As String = "('" & "ELIMINA PLANTILLA RESPUESTA" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" _
                                               & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                               id_respuesta_radicado & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor &
                                               "','WEB-WORKFLOW','" & "" & "')"
            Dim update_log As String = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                                insert_datos_envio
            Dim update_nota_respuesta As String = "UPDATE ra_respuesta_radicado " &
                  " set ID_IMAGEN=" & "null" &
                 " where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = update_nota_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_documento_respuesta = "Imposible actualizar la nota de la respuesta  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_log
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Elimina_documento_respuesta = "Imposible registrar log de actualización de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If existencia_imagen = "YES" Then
                Result = ref_class_gabinete.EliminarDocumentosGabinete(stru_envi.ID_IMAGEN,
                                                                            0,
                                                                           "IMP03GESTIONTMP",
                                                                            0,
                                                                            1,
                                                                            0,
                                                                            -1,
                                                                            "GESTIONCORRESPONDENCIA")
                If Result <> "YES" Then
                    Elimina_documento_respuesta = Result
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            If Result <> "YES" Then
                Elimina_documento_respuesta = Result
                Exit Function
            End If
            If id_tipo_envio_respuesta = 0 Then
                Result = Refclass_ra_respuesta.Solicita_estados_semaforo_respuesta(id_respuesta_radicado, Image)
                If Result <> "YES" Then
                    Elimina_documento_respuesta = Result
                    Exit Function
                End If
            Else
                Result = Refclass_ra_respuesta.Solicita_estados_semaforo_respuesta_electronica(id_respuesta_radicado, Image)
                If Result <> "YES" Then
                    Elimina_documento_respuesta = Result
                    Exit Function
                End If
            End If
            Elimina_documento_respuesta = "YES"
        Catch ex As MySqlException
            myTrans.Rollback()
            If Not myTrans.Connection Is Nothing Then
                Elimina_documento_respuesta = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
            If myTrans.Connection.State = ConnectionState.Open Then
                myConnection.Close()
            End If
            Elimina_documento_respuesta = "Error General " & ex.Message
            Exit Function
        End Try
    End Function
    Function Genera_archivo_detalle_respuesta(ByVal id_respuesta_radicado As Integer,
                                              ByRef archivo As String) As String
        Dim Result As String = ""
        Dim refclastrd As New ClassRaConsultaRadicados
        '--------------------------------------
        'Retorna nombre entidad- empresa
        '--------------------------------------
        Dim _Plantilla_Impre() As String
        Erase _Plantilla_Impre
        Dim nombre_empresa As String = ""
        Dim nit_empresa As String = ""
        Result = refclastrd.Lista_Nombre_Entidad(_Plantilla_Impre)
        If Result <> "YES" Then
            Genera_archivo_detalle_respuesta = Result
            Exit Function
        End If
        nombre_empresa = _Plantilla_Impre(0)
        nit_empresa = _Plantilla_Impre(1)
        Dim struc_envio As stru_envio = Nothing
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                    struc_envio)
        If Result <> "YES" Then
            Genera_archivo_detalle_respuesta = Result
            Exit Function
        End If
        Dim stiempo As String = ""
        Dim refclasradicado As New ClassRadicador
        Dim refclas_gestion_fechas As New ClassGestionFechas
        If struc_envio.FECHA_RESPUETA <> "" Then
            Dim stiempo_ As Object = Nothing
            Dim minuno As Object = Nothing
            Dim hora As Object = Nothing
            Dim dias_calendario As Object = Nothing
            Dim dias_no_habiles As Object = Nothing
            Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(struc_envio.FECHA_REGISTRO,
                                                                             stiempo_,
                                                                             hora,
                                                                             minuno,
                                                                             dias_calendario,
                                                                             dias_no_habiles,
                                                                             struc_envio.FECHA_RESPUETA)
            If Result <> "YES" Then
                Genera_archivo_detalle_respuesta = Result
                Exit Function
            End If
            stiempo = stiempo_ & " días, "
            stiempo = stiempo & hora.ToString & " horas, "
            stiempo = stiempo & minuno & " minutos. "
            stiempo = stiempo & "Formula de calculo tiempo de respuesta : dias calendario transcurridos " & dias_calendario & ", menos días no habiles " & dias_no_habiles
        Else
            Dim stiempo_ As Object = Nothing
            Dim minuno As Object = Nothing
            Dim hora As Object = Nothing
            Dim dias_calendario As Object = Nothing
            Dim dias_no_habiles As Object = Nothing
            Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(struc_envio.FECHA_REGISTRO,
                                                                             stiempo_,
                                                                             hora,
                                                                             minuno,
                                                                             dias_calendario,
                                                                             dias_no_habiles)
            If Result <> "YES" Then
                Genera_archivo_detalle_respuesta = Result
                Exit Function
            End If
            stiempo = "Para dar respuesta a su solicitud a transcurrido " & stiempo_ & " días, "
            stiempo = stiempo & hora.ToString & " horas, "
            stiempo = stiempo & minuno.ToString & " minutos, pero aún no tiene una respuesta para su solicitud "
            stiempo = stiempo & "(Formula de calculo tiempo de respuesta : dias calendario transcurridos " & dias_calendario & ", menos días no habiles " & dias_no_habiles & ")"
        End If
        Dim Usuario_gestion As String = ""
        Dim Cargo_usuario_gestion As String = ""
        '--------------------------------
        'Retorna datos usuario de gestión
        '---------------------------------
        Dim Ref_clas_remit_dest As New Class_remit_dest_interno
        Dim correo_electrnico As String = ""
        Result = Ref_clas_remit_dest.Retorna_datos_caracterizacion_usuario_gestion(struc_envio.ID_REMIT_DEST_INT,
                                                                                   Usuario_gestion,
                                                                                   Cargo_usuario_gestion,
                                                                                   correo_electrnico)
        If Result <> "YES" Then
            Genera_archivo_detalle_respuesta = Result
            Exit Function
        End If
        Dim tipo_respuesta_tramite As Integer = 0
        Result = Me.Retorna_estado_envio_por_id_respuesta(struc_envio.ID_RESPUESTA_RADICADO, tipo_respuesta_tramite)
        If Result <> "YES" Then
            Genera_archivo_detalle_respuesta = Result
            Exit Function
        End If
        Dim estado_obliga As Integer = -1
        Dim nombre_plantilla As String = ""
        Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
        Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(struc_envio.system_plantilla_radicado_id_plantilla,
                                                                             nombre_plantilla)
        If Result <> "YES" Then
            Genera_archivo_detalle_respuesta = Result
            Exit Function
        End If
        '----------------------------------------------------
        'Retorna el tipo de tramite obligatorio de respuesta
        '----------------------------------------------------
        Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
        Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
        Dim id_tipo_tramite As Integer = 0
        Dim descripcion_tramite As String = ""
        Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla,
                                                                                             struc_envio.RADICADO,
                                                                                             id_tipo_tramite,
                                                                                             descripcion_tramite)
        If Result <> "YES" Then
            Genera_archivo_detalle_respuesta = Result
            Exit Function
        End If
        Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                         estado_obliga)
        If Result <> "YES" Then
            Genera_archivo_detalle_respuesta = Result
            Exit Function
        End If
        'Me.Retorna_estado_respuesta_radicado_obligatorio(nombre_plantilla, struc_envio.RADICADO, estado_obliga)
        '-----------------------------------------------------------------
        'Asigna el tipo trámite del radicado a al tipo de respuesta si no
        'hay una respuesta elaborada
        '-----------------------------------------------------------------
        If struc_envio.FECHA_RESPUETA = "" Then
            struc_envio.TIPO_RESPUESTA_ELAB_USUARIO = estado_obliga
        End If
        Dim doc As iTextSharp.text.Document
        Dim writer As iTextSharp.text.pdf.PdfWriter
        Try
            Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/PUBLIC/DESCARGA/")
            If Directory.Exists(ruta_local) = False Then
                Directory.CreateDirectory(ruta_local)
            End If
            Dim archivo_pdf As String = ruta_local & "temp_" & "Da" & HttpContext.Current.Session.SessionID & ".pdf"
            If File.Exists(archivo_pdf) = True Then
                Kill(archivo_pdf)
            End If
            archivo = archivo_pdf
            doc = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER)
            doc.SetPageSize(iTextSharp.text.PageSize.LETTER.Rotate())
            writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc,
                              New FileStream(archivo_pdf, FileMode.Create))
            writer.AddViewerPreference(iTextSharp.text.pdf.PdfName.PICKTRAYBYPDFSIZE, iTextSharp.text.pdf.PdfBoolean.PDFTRUE)
            doc.Open()
            Dim ruta_image As String = HttpContext.Current.Server.MapPath("../imagera/logo_trd.png")
            Dim imagen As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ruta_image)
            imagen.BorderWidth = 0
            imagen.Alignment = iTextSharp.text.Element.ALIGN_LEFT
            Dim percentage As Object = 0.0F
            percentage = 100 / imagen.Width
            imagen.ScalePercent(percentage * 80)
            'Insertamos la imagen en el documento
            doc.Add(imagen)
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
                   12, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)
            Dim _standardFont_datos_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
            10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)
            Dim Dat As Date = Now
            Dim paragraf As New iTextSharp.text.Paragraph
            paragraf = New iTextSharp.text.Paragraph("DETALLE RESPUESTA RADICADO", _standardFont)
            paragraf.Alignment = iTextSharp.text.Element.ALIGN_CENTER
            doc.Add(paragraf)
            paragraf = New iTextSharp.text.Paragraph(nombre_empresa, _standardFont)
            paragraf.Alignment = iTextSharp.text.Element.ALIGN_RIGHT
            doc.Add(paragraf)
            paragraf = New iTextSharp.text.Paragraph(nit_empresa, _standardFont)
            paragraf.Alignment = iTextSharp.text.Element.ALIGN_RIGHT
            doc.Add(paragraf)
            _standardFont.Size = 10
            paragraf = New iTextSharp.text.Paragraph("Fecha reporte " & Dat, _standardFont)
            paragraf.Alignment = iTextSharp.text.Element.ALIGN_LEFT
            doc.Add(paragraf)
            doc.Add(iTextSharp.text.Chunk.NEWLINE)
            Dim tblrdatos As iTextSharp.text.pdf.PdfPTable = New iTextSharp.text.pdf.PdfPTable(2)
            tblrdatos.WidthPercentage = 100
            Dim cltitem As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("ITEM", _standardFont))
            cltitem.BorderWidth = 1
            Dim cltdetalle As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("DETALLE", _standardFont))
            cltdetalle.BorderWidth = 1
            tblrdatos.AddCell(cltitem)
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'RADICADO PETICIONARIO
            '----------------------------------------
            Dim cltitem_peticionario As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("RADICADO PETICIONARIO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem_peticionario)
            Dim cltdetalle_peticionario As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.RADICADO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle_peticionario)
            '-----------------------------------------
            'PETICIONARIO
            '-----------------------------------------
            Dim cltitem_peticionario_cliente As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("PETICIONARIO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem_peticionario_cliente)
            Dim cltdetalle_peticionario_cliente As iTextSharp.text.pdf.PdfPCell = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.DESTINATARIO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle_peticionario_cliente)
            '----------------------------------------
            'TIPO TRÁMITE RADICADO
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("TIPO TRÁMITE RADICADO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.TRAMITE_DOCUMENTO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'FECHA DEL RADICADO
            '----------------------------------------

            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("FECHA DEL RADICADO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.FECHA_REGISTRO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'FECHA DE RESPUESTA
            '----------------------------------------
            Dim fECHA As String
            Dim ESTADO_FECHA As String
            If struc_envio.ESTADO_RESPUESTA = 1 Then
                fECHA = "FECHA EN QUE SE LE DIO RESPUESTA AL TRAMITE"
            End If
            If struc_envio.ESTADO_RESPUESTA = 5 Then
                fECHA = "FECHA EN QUE SE CONFIRMO EL TRAMITE"
            End If
            If struc_envio.ESTADO_RESPUESTA = 6 Then
                fECHA = "FECHA EN QUE SE ARCHIVO EL TRAMITE"
            End If
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(fECHA, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.FECHA_RESPUETA, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'FECHA LÍMITE DE RESPUESTA DEL RADICADO
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("FECHA LÍMITE DE RESPUESTA DEL RADICADO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.FECHA_VENCE, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'TIEMPO ESTIMADO DE RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("TIEMPO ESTIMADO DE RESPUESTA", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(stiempo, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'CONSECUTIVO RADICADO RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("CONSECUTIVO RADICADO RESPUESTA", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.RADICADO_RESPUESTA, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'USUARIO QUE ELABORO LA RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("USUARIO QUE ELABORA(O) LA RESPUESTA", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(Usuario_gestion, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'ESTADO DE LA RESPIESTA
            '----------------------------------------
            Dim Estado As String = ""
            If struc_envio.ESTADO_RESPUESTA = 1 Then
                Estado = "Respuesta formal"
            End If
            If struc_envio.ESTADO_RESPUESTA = 6 Then
                Estado = "Archivado"
            End If
            If struc_envio.ESTADO_RESPUESTA = 5 Then
                Estado = "Confirmación"
            End If
            If struc_envio.ESTADO_RESPUESTA = 4 Then
                Estado = "En tramite"
            End If
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("ESTADO DEL TRAMITE", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(Estado, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'CARGO USUARIO QUE ELABORO LA RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("CARGO USUARIO QUE ELABORA(O) LA RESPUESTA", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(Cargo_usuario_gestion, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'MEDIO DE ENVÍO DE LA RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("TIPO ENVIO PREVISTO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.MEDIO_ENVIO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'CURRIER DE ENVÍO DE LA RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("CURRIER DE ENVÍO PREVISTO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.EMPRESA_ENVIO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'CONSECUTIVO GUÍA DE ENVÍO DE LA RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("CONSECUTIVO GUÍA DE ENVÍO DE LA RESPUESTA PREVISTO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.GUIA_ENVIO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'FECHA DE ENVÍO DE LA RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("FECHA DE ENVÍO PREVISTO DE LA RESPUESTA", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.FECHA_ENVIO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'ASUNTO
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("ASUNTO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.ASUNTO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'TIPO ENVIO DE LA RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("TIPO ENVIO PREVISTO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            Dim descrip_tipo_envio As String = ""
            If tipo_respuesta_tramite = 0 Then
                descrip_tipo_envio = "Envío respuesta a correo físico o convencional"
            Else
                descrip_tipo_envio = "Envío respuesta a correo Electronico"
            End If
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(descrip_tipo_envio, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'TIPO RESPUESTA ELABORADA POR EL USUARIO
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("TIPO DE RESPUESTA PREVISTO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            Dim descrip_tipo_resp As String = ""
            If struc_envio.FECHA_RESPUETA <> "" Then
                If struc_envio.ESTADO_RESPUESTA = 1 Then
                    descrip_tipo_resp = "Respuesta formal"
                End If
                If struc_envio.ESTADO_RESPUESTA = 6 Then
                    descrip_tipo_resp = "Archivado"
                End If
                If struc_envio.ESTADO_RESPUESTA = 5 Then
                    descrip_tipo_resp = "Confirmación"
                End If
                If struc_envio.ESTADO_RESPUESTA = 4 Then
                    descrip_tipo_resp = "En tramite"
                End If
            End If
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(descrip_tipo_resp, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'NOTA RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("NOTA RESPUESTA", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.NOTA_RESPUESTA, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'CODIGO UNICO RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("CODIGO UNICO RESPUESTA", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.ID_RESPUESTA_RADICADO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'FECHA ENVIO CORREO RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("FECHA ENVÍO CORREO ELECTRÓNICO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.FECHA_REGISTRO_EVIO_CORREO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'CORREO NOTIFICADO DE RESPUESTA
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("CORREO ELECTRÓNICO NOTIFICADO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.CORREO_NOTIFICACION, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'FECHA CONFIRMACIÓN RECIBIDO
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("FECHA DE CONFIRMACIÓN DE RECIBIDO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.FECHA_CONFIRMACION_CORREO_RECIBIDO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'IP CONFIRMACIÓN RECIBIDO
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("HOST DE CONFIRMACIÓN DE RECIBIDO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.IP_CONFIRMACION_CORREO_RECIBIDO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            '----------------------------------------
            'HUELLA CONFIRMACIÓN RECIBIDO
            '----------------------------------------
            cltitem = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase("HUELLA DE CONFIRMACIÓN DE RECIBIDO", _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltitem)
            cltdetalle = New iTextSharp.text.pdf.PdfPCell(New iTextSharp.text.Phrase(struc_envio.HUELLA_CORREO_RECIBIDO, _standardFont_datos_unidad_conservacion))
            tblrdatos.AddCell(cltdetalle)
            doc.Add(tblrdatos)
            doc.Close()
            writer.Close()
            Genera_archivo_detalle_respuesta = "YES"
        Catch ex As Exception
            Genera_archivo_detalle_respuesta = "Inconsistencia general función  Genera_archivo_detalle_respuesta " & ex.Message
        End Try
    End Function
    Function Prepara_interface_solo_confirma_respuesta(ByVal id_respuesta As Integer,
                                                       ByRef pag As Page) As String
        Try
            Dim id_plantilla_salida As Integer = 0
            Dim nombre_plantilla_salida As String = ""
            Dim refclasradicado As New ClassRadicador
            Dim Result As String = ""
            Dim CheckBox_envio_correo_solo_confirmar As CheckBox = pag.FindControl("CheckBox_envio_correo_solo_confirmar")
            If CheckBox_envio_correo_solo_confirmar Is Nothing Then
                Prepara_interface_solo_confirma_respuesta = "Imposible encontrar el control CheckBox_envio_correo_solo_confirmar"
                Exit Function
            End If
            Dim TextBox_correo_solo_confirmar As TextBox = pag.FindControl("TextBox_correo_solo_confirmar")
            If TextBox_correo_solo_confirmar Is Nothing Then
                Prepara_interface_solo_confirma_respuesta = "Imposible encontrar el control TextBox_correo_solo_confirmar"
                Exit Function
            End If
            Dim UpdatePanel_solo_confirmar As UpdatePanel = pag.FindControl("UpdatePanel_solo_confirmar")
            If UpdatePanel_solo_confirmar Is Nothing Then
                Prepara_interface_solo_confirma_respuesta = "Imposible encontrar el control UpdatePanel_solo_confirmar"
                Exit Function
            End If
            'retorna plantilla radicado defaul respuesta
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla_salida, nombre_plantilla_salida)
            If Result <> "YES" Then
                Prepara_interface_solo_confirma_respuesta = Result
                Exit Function
            End If
            Dim stru_envio As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, stru_envio)
            If Result <> "YES" Then
                Prepara_interface_solo_confirma_respuesta = Result
                Exit Function
            End If
            Dim nombre_plantilla_radicado As String = ""
            Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
            Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(stru_envio.system_plantilla_radicado_id_plantilla,
                                                                                 nombre_plantilla_radicado)
            If Result <> "YES" Then
                Prepara_interface_solo_confirma_respuesta = Result
                Exit Function
            End If
            Dim estado_tipo_respuesta As Integer = 0
            Result = Retorna_estado_envio_respuesta(nombre_plantilla_radicado, stru_envio.RADICADO, estado_tipo_respuesta)
            If Result <> "YES" Then
                Prepara_interface_solo_confirma_respuesta = Result
                Exit Function
            End If
            If estado_tipo_respuesta = 0 Then
                'CheckBox_envio_correo_solo_confirmar.Enabled = True
                CheckBox_envio_correo_solo_confirmar.Checked = False
            Else
                'CheckBox_envio_correo_solo_confirmar.Enabled = False
                CheckBox_envio_correo_solo_confirmar.Checked = True
            End If
            Dim correo_electronico As String = ""
            Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(stru_envio.codigo_dest_externo, correo_electronico, stru_envio.system_plantilla_radicado_id_plantilla)
            If Result <> "YES" Then
                Prepara_interface_solo_confirma_respuesta = Result
                Exit Function
            End If
            TextBox_correo_solo_confirmar.Text = correo_electronico
            UpdatePanel_solo_confirmar.Update()
            Prepara_interface_solo_confirma_respuesta = "YES"
        Catch ex As Exception
            Prepara_interface_solo_confirma_respuesta = "Inconsistencia general función  Prepara_interface_solo_confirma_respuesta " & ex.Message
        End Try
    End Function
    Function Prepara_interface_radica_confirma_respuesta(ByVal id_respuesta As Integer,
                                                         ByVal radicado_responder As String,
                                                         ByRef pag As Page,
                                                         ByVal estado_tipo_respuesta As Integer,
                                                         Optional ByVal tipo_interface As Integer = 0) As String
        '*********************************************************
        'Función : Alista interface radicado respuesta
        'Fecha : 2016-07-06
        'Ing : Miguel Angel Urueta
        '*********************************************************
        Try
            Dim id_plantilla_salida As Integer = 0
            Dim nombre_plantilla_salida As String = ""
            Dim refclasradicado As New ClassRadicador
            Dim Result As String = ""
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla_salida, nombre_plantilla_salida)
            If Result <> "YES" Then
                Prepara_interface_radica_confirma_respuesta = Result
                Exit Function
            End If
            Dim drop_list As DropDownList = pag.FindControl("RE_Descripcion_Documento")
            If drop_list Is Nothing Then
                Prepara_interface_radica_confirma_respuesta = "Imposible encontrar el control DropDownList_tipo_tramite"
                Exit Function
            End If
            Dim UpdatePanel_contenido_radica_documento As UpdatePanel = Nothing
            If tipo_interface = 0 Then
                UpdatePanel_contenido_radica_documento = pag.FindControl("UpdatePanel_contenido_radica_documento")
            Else
                UpdatePanel_contenido_radica_documento = pag.FindControl("UpdatePanel_radic_documento_respuesta")
            End If
            If UpdatePanel_contenido_radica_documento Is Nothing Then
                Prepara_interface_radica_confirma_respuesta = "Imposible encontrar el control UpdatePanel_contenido_radica_documento"
                Exit Function
            End If
            Dim TextBox_destintario As TextBox = pag.FindControl("RE_REMITENTE_COR_REMITENTE_COR_VARCHAR")
            If TextBox_destintario Is Nothing Then
                Prepara_interface_radica_confirma_respuesta = "Imposible encontrar el control TextBox_destintario"
                Exit Function
            End If
            Dim Hidden_remitente_destinatario As HtmlInputHidden = pag.FindControl("Hidden_remitente_destinatario")
            If Hidden_remitente_destinatario Is Nothing Then
                Prepara_interface_radica_confirma_respuesta = "Imposible encontrar el control Hidden_remitente_destinatario"
                Exit Function
            End If
            'TextBox_correo_electronico_interf
            'Dim TextBox_correo_electronico As TextBox = Nothing
            'If tipo_interface = 0 Then
            '    TextBox_correo_electronico = pag.FindControl("TextBox_correo_electronico")
            'Else
            '    TextBox_correo_electronico = pag.FindControl("TextBox_correo_electronico_interf")
            'End If

            'If TextBox_correo_electronico Is Nothing Then
            '    Prepara_interface_radica_confirma_respuesta = "Imposible encontrar el control TextBox_correo_electronico"
            '    Exit Function
            'End If
            Dim CheckBox_envio_correo As CheckBox = pag.FindControl("CheckBox_envio_correo")
            If CheckBox_envio_correo Is Nothing Then
                Prepara_interface_radica_confirma_respuesta = "Imposible encontrar el control CheckBox_envio_correo"
                Exit Function
            End If
            Dim CheckBox_envia_ventanilla As CheckBox = pag.FindControl("CheckBox_envia_ventanilla")
            If CheckBox_envia_ventanilla Is Nothing Then
                Prepara_interface_radica_confirma_respuesta = "Imposible encontrar el control CheckBox_envia_ventanilla"
                Exit Function
            End If
            If estado_tipo_respuesta = 0 Then
                'CheckBox_envio_correo.Enabled = True
                CheckBox_envio_correo.Checked = False
                'CheckBox_envia_ventanilla.Enabled = False
                CheckBox_envia_ventanilla.Checked = True
            Else
                'CheckBox_envio_correo.Enabled = False
                CheckBox_envio_correo.Checked = True
                'CheckBox_envia_ventanilla.Enabled = True
                CheckBox_envia_ventanilla.Checked = False
            End If
            Dim stru_envio As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, stru_envio)
            If Result <> "YES" Then
                Prepara_interface_radica_confirma_respuesta = Result
                Exit Function
            End If
            'Lista tipo de tramites
            Result = refclasradicado.Listar_Tipos_Documentales(drop_list, id_plantilla_salida, stru_envio.TRAMITE_DOCUMENTO)
            If Result <> "YES" Then
                Prepara_interface_radica_confirma_respuesta = Result
                Exit Function
            End If
            Hidden_remitente_destinatario.Value = stru_envio.codigo_dest_externo
            TextBox_destintario.Text = stru_envio.DESTINATARIO
            'lista correo electronico destinatario
            'Dim correo_electronico As String = ""
            'Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(stru_envio.codigo_dest_externo, _
            '                                                                           correo_electronico, _
            '                                                                           stru_envio.system_plantilla_radicado_id_plantilla)
            'If Result <> "YES" Then
            '    Prepara_interface_radica_confirma_respuesta = Result
            '    Exit Function
            'End If
            'TextBox_correo_electronico.Text = correo_electronico
            UpdatePanel_contenido_radica_documento.Update()
            Prepara_interface_radica_confirma_respuesta = "YES"
        Catch ex As Exception
            Prepara_interface_radica_confirma_respuesta = "Inconsistencia general funcion prepara_interface_radica_confirma_respuesta " & ex.Message
        End Try
    End Function
    Function Generar_interface_confirmar_respuesta(ByVal id_respuesta As Integer,
                                                   ByVal id_usuario_propietario As Integer,
                                                   ByVal radicado_responder As String,
                                                   ByRef pag As Page,
                                                   ByRef drop_lista_firma As DropDownList,
                                                   ByVal estado_tipo_respuesta As Integer,
                                                   Optional ByVal tipo_interface As Integer = 0) As String
        '*********************************************************
        'Función : Alista interface confirma respuesta con radicado
        'Fecha : 2016-10-04
        'Ing : Miguel Angel Urueta
        '*********************************************************
        Try
            Dim refclasradicado As New ClassRadicador
            Dim ref_class_ra_cd_solici_aprob As New ClassRaSolicitudesAprobacion
            Dim ref_class_ra_cd_usu As New Class_ra_cd_usuarios_solicitudes_aprobacion
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Dim ref_class_rel_firmas As New Class_relacion_firmas_autorizadas
            Dim ref_clas_ra_dow As New Class_ra_ra_registro_down_formato
            Dim stru_envio As stru_envio = Nothing
            Dim Result As String = ""
            Dim id_solictud_aprobacion As Integer = 0
            Dim id_usuario_firma_default As Integer = 0
            Dim stru_firmas_solicitud_aprobada() As stru_usu_firmas_autorizadas = Nothing
            Dim stru_firma_autoriza() As stru_usu_firmas_autorizadas = Nothing
            Dim UpdatePanel_contenido_radica_documento As UpdatePanel = Nothing
            If tipo_interface = 0 Then
                UpdatePanel_contenido_radica_documento = pag.FindControl("UpdatePanel_confirma_envio_respuesta")
            Else
                UpdatePanel_contenido_radica_documento = pag.FindControl("UpdatePanel_confirma_envio_respuesta")
            End If
            If UpdatePanel_contenido_radica_documento Is Nothing Then
                Generar_interface_confirmar_respuesta = "Imposible encontrar el control UpdatePanel_contenido_radica_documento"
                Exit Function
            End If
            '--------------------------------------------
            'Verifica usuario propietario de la respuesta
            '--------------------------------------------
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") <> id_usuario_propietario Then
                Generar_interface_confirmar_respuesta = "El usuario de gestión no es el autorizado para gestionar la respuesta"
                Exit Function
            End If
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                        stru_envio)
            If Result <> "YES" Then
                Generar_interface_confirmar_respuesta = Result
                Exit Function
            End If
            '--------------------------------------------
            'Verifica existencia documento respuesta
            '--------------------------------------------
            If stru_envio.ID_IMAGEN = 0 Then
                Generar_interface_confirmar_respuesta = "La respuesta actual no tiene un documento de respuesta asociado, por favor cargue el documento"
                Exit Function
            End If
            Result = ref_class_ra_cd_solici_aprob.Solicita_solicitud_aprobacion_aprobada(id_respuesta,
                                                                                         id_solictud_aprobacion)
            If Result <> "YES" Then
                Generar_interface_confirmar_respuesta = Result
                Exit Function
            End If
            '---------------------------------------------
            'Lista firmas solicitudes de aprobacion
            '---------------------------------------------
            If id_solictud_aprobacion <> 0 Then
                Result = ref_class_ra_cd_usu.Solicita_lista_usuarios_firmas_solicitudes_aprobacion(id_solictud_aprobacion,
                                                                                                   stru_firmas_solicitud_aprobada)
                If Result <> "YES" Then
                    Generar_interface_confirmar_respuesta = Result
                    Exit Function
                End If
            End If
            Result = Me.Lista_usuarios_firmas_permitidas_iterface(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                      stru_firmas_solicitud_aprobada,
                                                                      0, drop_lista_firma,
                                                                      UpdatePanel_contenido_radica_documento)
            If Result <> "YES" Then
                Generar_interface_confirmar_respuesta = Result
                Exit Function
            End If

            Result = ref_class_rel_firmas.Solicita_lista_usuarios_permitidos_firma(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                 stru_firma_autoriza)
            If Result <> "YES" Then
                Generar_interface_confirmar_respuesta = Result
                Exit Function
            End If
            Result = ref_clas_ra_dow.Solicita_utltimo_usuario_firma_formato_descarga(id_respuesta,
                                                                                     id_usuario_firma_default)
            If Result <> "YES" Then
                Generar_interface_confirmar_respuesta = Result
                Exit Function
            End If
            Result = Me.Lista_usuarios_firmas_permitidas_iterface_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                            stru_firma_autoriza,
                                                                            id_usuario_firma_default, drop_lista_firma,
                                                                            UpdatePanel_contenido_radica_documento)
            If Result <> "YES" Then
                Generar_interface_confirmar_respuesta = Result
                Exit Function
            End If
            'Dim TextBox_correo_electronico As TextBox = Nothing
            'If tipo_interface = 0 Then
            '    TextBox_correo_electronico = pag.FindControl("TextBox_correo_electronico_ra")
            'Else
            '    TextBox_correo_electronico = pag.FindControl("TextBox_correo_electronico_interf_ra")
            'End If

            'If TextBox_correo_electronico Is Nothing Then
            '    Generar_interface_confirmar_respuesta = "Imposible encontrar el control TextBox_correo_electronico"
            '    Exit Function
            'End If
            Dim CheckBox_envio_correo As CheckBox = pag.FindControl("CheckBox_envio_correo_ra")
            If CheckBox_envio_correo Is Nothing Then
                Generar_interface_confirmar_respuesta = "Imposible encontrar el control CheckBox_envio_correo"
                Exit Function
            End If
            Dim CheckBox_envia_ventanilla As CheckBox = pag.FindControl("CheckBox_envia_ventanilla_ra")
            If CheckBox_envia_ventanilla Is Nothing Then
                Generar_interface_confirmar_respuesta = "Imposible encontrar el control CheckBox_envia_ventanilla"
                Exit Function
            End If
            If estado_tipo_respuesta = 0 Then
                CheckBox_envio_correo.Enabled = True
                CheckBox_envio_correo.Checked = False
                CheckBox_envia_ventanilla.Enabled = False
                CheckBox_envia_ventanilla.Checked = True
            Else
                CheckBox_envio_correo.Enabled = False
                CheckBox_envio_correo.Checked = True
                CheckBox_envia_ventanilla.Enabled = True
                CheckBox_envia_ventanilla.Checked = False
            End If

            'lista correo electronico destinatario
            'Dim correo_electronico As String = ""
            'Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(stru_envio.codigo_dest_externo, _
            '                                                                           correo_electronico, _
            '                                                                           stru_envio.system_plantilla_radicado_id_plantilla)
            'If Result <> "YES" Then
            '    Generar_interface_confirmar_respuesta = Result
            '    Exit Function
            'End If
            'TextBox_correo_electronico.Text = correo_electronico
            UpdatePanel_contenido_radica_documento.Update()
            Generar_interface_confirmar_respuesta = "YES"
        Catch ex As Exception
            Generar_interface_confirmar_respuesta = "Inconsistencia general funcion Generar_interface_confirmar_respuesta " & ex.Message
        End Try
    End Function
    'Function Confirma_respuesta_con_radicado(ByVal id_respuesta_radicado As Integer, _
    '                                         ByRef pag As Page, _
    '                                         ByVal tipo_interface As Integer, _
    '                                         ByRef Reuslttado_correo As String, ByVal k As Integer) As String
    '    Dim id_imagen_plantilla As Integer = 0
    '    Dim radicado_respuesta As Integer = 0
    '    Dim fecha_respuesta As Integer = 0
    '    Dim id_imagen_respuesta As Integer = 0
    '    Dim estado_envio_respuesta As Integer = 0
    '    Dim Result As String = ""
    '    Dim refclas As New ClassRadicador
    '    Try
    '        Dim CheckBox_envio_correo As CheckBox = pag.FindControl("CheckBox_envio_correo")
    '        If CheckBox_envio_correo Is Nothing Then
    '            Confirma_respuesta_con_radicado = "Imposible encontrar el control CheckBox_envio_correo"
    '            Exit Function
    '        End If
    '        Dim CheckBox_envia_ventanilla As CheckBox = pag.FindControl("CheckBox_envia_ventanilla")
    '        If CheckBox_envia_ventanilla Is Nothing Then
    '            Confirma_respuesta_con_radicado = "Imposible encontrar el control CheckBox_envia_ventanilla"
    '            Exit Function
    '        End If
    '        Dim CheckBox_firma_digital As CheckBox = pag.FindControl("CheckBox_firma_digital")
    '        If CheckBox_firma_digital Is Nothing Then
    '            Confirma_respuesta_con_radicado = "Imposible encontrar el control CheckBox_firma_digital"
    '            Exit Function
    '        End If
    '        Dim TextBox_correo_electronico As TextBox = Nothing
    '        If tipo_interface = 0 Then
    '            TextBox_correo_electronico = pag.FindControl("TextBox_correo_electronico")
    '        Else
    '            TextBox_correo_electronico = pag.FindControl("TextBox_correo_electronico_interf")
    '        End If
    '        If TextBox_correo_electronico Is Nothing Then
    '            Confirma_respuesta_con_radicado = "Imposible encontrar el control TextBox_correo_electronico"
    '            Exit Function
    '        End If
    '        Dim Hidden_radicado As HtmlInputHidden = pag.FindControl("Hidden_radicado")
    '        If Hidden_radicado Is Nothing Then
    '            Confirma_respuesta_con_radicado = "Imposible encontrar el control Hidden_radicado"
    '            Exit Function
    '        End If
    '        If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
    '            Confirma_respuesta_con_radicado = "Por favor active web service para workflow"
    '            Exit Function
    '        End If
    '        If HttpContext.Current.Session.Item("URL_WEB_SERVICE") = "" Then
    '            Confirma_respuesta_con_radicado = "Por favor informe la url web service para workflow"
    '            Exit Function
    '        End If
    '        If CheckBox_envio_correo.Checked = False And CheckBox_envia_ventanilla.Checked = False Then
    '            Confirma_respuesta_con_radicado = "Por favor informe la opción de enviar por correo,  o solicitar al centro envio que envie su respuesta"
    '            Exit Function
    '        End If
    '        Dim opcion_certificado_digital As Integer = 0
    '        If CheckBox_firma_digital.Checked = True Then
    '            opcion_certificado_digital = 1
    '        End If
    '        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
    '        Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado, _
    '                                                                            id_imagen_plantilla, _
    '                                                                            radicado_respuesta, _
    '                                                                            fecha_respuesta, _
    '                                                                            id_imagen_respuesta, _
    '                                                                            estado_envio_respuesta)
    '        If Result <> "YES" Then
    '            Confirma_respuesta_con_radicado = Result
    '            Exit Function
    '        End If
    '        Dim option_opcion As Integer = 0
    '        Dim codigo_radicado As String = ""
    '        If fecha_respuesta = 0 And radicado_respuesta = 0 Then
    '            Dim id_imagen As Integer = -1
    '            Dim gabinete_imagen As String = ""
    '            Dim refclas_gestion As New Classgestionrespuesta
    '            Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(id_respuesta_radicado, gabinete_imagen, id_imagen)
    '            If Result <> "YES" Then
    '                Confirma_respuesta_con_radicado = Result
    '                Exit Function
    '            End If
    '            If id_imagen = -1 Then
    '                Confirma_respuesta_con_radicado = "Por favor guarde el documento plantilla "
    '                Exit Function
    '            End If
    '            Dim refclas_visualiza As New ClassVisualisaDocumento
    '            Dim matri_documento() As String = Nothing
    '            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen, "IMP03GESTIONTMP", matri_documento)
    '            If Result <> "YES" Then
    '                Confirma_respuesta_con_radicado = Result
    '                Exit Function
    '            End If
    '            If CheckBox_envio_correo.Checked = True Then
    '                If TextBox_correo_electronico.Text = "" Then
    '                    Confirma_respuesta_con_radicado = "Debe informar el correo electrónico de respuesta "
    '                    Exit Function
    '                End If
    '            End If
    '            Dim nombre_plantilla As String = ""
    '            Dim id_plantilla As Integer = -1
    '            Dim ref_calss_system As New Class_system_plantilla_radicado
    '            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla)
    '            If Result <> "YES" Then
    '                Confirma_respuesta_con_radicado = Result
    '                Exit Function
    '            End If
    '            Dim estado_envio_correo As Integer = 0
    '            If CheckBox_envio_correo.Checked = True Then
    '                estado_envio_correo = 1
    '            End If
    '            If CheckBox_envia_ventanilla.Checked = True Then
    '                estado_envio_respuesta = 1
    '            Else
    '                estado_envio_respuesta = 5
    '            End If
    '            '-----------------------------------------------
    '            'Verifica estado solicitudes de aprobación sin
    '            'desición
    '            '-----------------------------------------------
    '            Dim Estado_solicitud_aprobacion As String = ""
    '            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
    '            Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), Estado_solicitud_aprobacion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
    '            If Result <> "YES" Then
    '                Confirma_respuesta_con_radicado = Result
    '                Exit Function
    '            End If
    '            If Estado_solicitud_aprobacion = "YES" Then
    '                Confirma_respuesta_con_radicado = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
    '                Exit Function
    '            End If
    '            Dim rut_firma As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA_FINAL"))
    '            Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
    '            Result = refclas.Registra_Radicacion_saliente_respuesta_radicado(id_plantilla, "RADICACIONSALIENTE", _
    '            pag, nombre_plantilla, codigo_radicado, id_respuesta_radicado, _
    '            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), HttpContext.Current.Session.Item("Id_Ruta_Workflow"), "", _
    '            "", 0, estado_envio_respuesta, matri_documento(1), Hidden_radicado.Value, estado_envio_correo, ruta_local, _
    '            matri_documento, TextBox_correo_electronico.Text, Reuslttado_correo, estado_envio_correo, rut_firma, _
    '            opcion_certificado_digital, HttpContext.Current.Session.Item("CLAVE_ARCHIVO_CERTIFICACION"), _
    '            HttpContext.Current.Session.Item("RUTA_ARCHIVO_CERTIFICACION"))
    '            If Result <> "YES" Then
    '                Confirma_respuesta_con_radicado = Result
    '                Exit Function
    '            End If
    '            Confirma_respuesta_con_radicado = "YES"
    '            Exit Function
    '        Else
    '            Confirma_respuesta_con_radicado = "El tramite se encuentra radicado, imposible radicar"
    '            Exit Function
    '        End If
    '    Catch ex As Exception
    '        Confirma_respuesta_con_radicado = "Inconsistencia general función Confirma_respuesta_con_radicado " & ex.Message
    '    End Try
    'End Function
    Function Descarga_documento_respuesta(ByVal id_respuesta As Integer,
                                          ByVal formato As String,
                                          ByVal estado_firma As Integer,
                                          ByRef ifmExcel As Object,
                                          ByRef updatapanel_iframe As UpdatePanel,
                                          ByRef Hidden_ruta_archivo As Object) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaEnvioCorrespondencia
            Dim Refclasvisual As New ClassVisualisaDocumento
            Dim Refclasgestion As New Classgestionrespuesta
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                        estru)
            If Result <> "YES" Then
                Descarga_documento_respuesta = Result
                Exit Function
            End If
            If estru.FECHA_RESPUETA = "" Then
                Descarga_documento_respuesta = "No hay una respuesta para el documento "
                Exit Function
            End If
            If estru.ID_IMAGEN = 0 Then
                Descarga_documento_respuesta = "La solicitud no registra documento de respuesta para descargar "
                Exit Function
            End If

            Dim Matri_documentos() As String
            Erase Matri_documentos
            Result = Refclasvisual.Genera_Matris_Documentos_Almacenados(estru.ID_IMAGEN,
                                                                        UCase("imp03gestiontmp"),
                                                                        Matri_documentos)
            If Result <> "YES" Then
                Descarga_documento_respuesta = Result
                Exit Function
            End If
            If Matri_documentos.Length >= 2 Then
                Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/PUBLIC/DESCARGA/")
                If Directory.Exists(ruta_local) = False Then
                    Directory.CreateDirectory(ruta_local)
                End If
                Dim fileinf As New FileInfo(Matri_documentos(1))
                If File.Exists(Matri_documentos(1)) Then
                    If estru.ID_TIPO_DOC_RESPUESTA = 1 Then
                        Dim filecopia As String = ruta_local & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & fileinf.Extension
                        If File.Exists(filecopia) = True Then
                            File.Delete(filecopia)
                        End If
                        File.Copy(Matri_documentos(1), filecopia)
                        If File.Exists(filecopia) = True Then
                            Dim ruta_final As String = ruta_local & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & "." & fileinf.Extension
                            Dim archivo_final As String = HttpContext.Current.Session.Item("RA_ID_USUARIO") & "file_temp." & fileinf.Extension
                            If File.Exists(ruta_final) = True Then
                                Kill(ruta_final)
                            End If
                            Dim refclas_gembox As New ClassGaGembox
                            Dim ref_class_resp As New Class_ra_ra_registro_firma_documento
                            Dim rut_firma As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA_FINAL"))
                            Dim file_salida As String = ""
                            Dim id_usuario_gestion As Integer = estru.ID_REMIT_DEST_INT
                            Dim id_usuario_firma As Integer = 0
                            Result = ref_class_resp.Solicita_ultimo_usuario_firma_documento(id_respuesta,
                                                                                            id_usuario_firma)
                            If Result <> "YES" Then
                                Descarga_documento_respuesta = Result
                                Exit Function
                            End If
                            If id_usuario_firma = 0 Then
                                id_usuario_firma = id_usuario_gestion
                            End If
                            Result = refclas_gembox.Firma_documento_formato_respuesta(filecopia,
                                                                                   0,
                                                                                   HttpContext.Current.Session.Item("CLAVE_ARCHIVO_CERTIFICACION"),
                                                                                   HttpContext.Current.Session.Item("RUTA_ARCHIVO_CERTIFICACION"),
                                                                                   id_usuario_firma,
                                                                                   id_usuario_gestion,
                                                                                   estado_firma,
                                                                                   formato,
                                                                                   file_salida)
                            If Result <> "YES" Then
                                Descarga_documento_respuesta = Result
                                Exit Function
                            End If
                            Hidden_ruta_archivo.Value = "../Temp_Image/PUBLIC/DESCARGA/" & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & "." & formato
                            ifmExcel.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                            updatapanel_iframe.Update()
                            Descarga_documento_respuesta = "YES"
                            Exit Function
                        End If
                    End If
                    If estru.ID_TIPO_DOC_RESPUESTA = 2 Then
                        Dim filecopia As String = ruta_local & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & fileinf.Extension
                        If File.Exists(filecopia) = True Then
                            File.Delete(filecopia)
                        End If
                        File.Copy(Matri_documentos(1), filecopia)
                        If File.Exists(filecopia) = True Then
                            Hidden_ruta_archivo.Value = "../Temp_Image/PUBLIC/DESCARGA/" & "temp_" & "Dr" & HttpContext.Current.Session.SessionID & fileinf.Extension
                            ifmExcel.Attributes.Add("src", "..\radicador\WebFormDescargaRadicado.aspx")
                            updatapanel_iframe.Update()
                            Descarga_documento_respuesta = "YES"
                            Exit Function
                        End If
                    End If
                End If
            Else
                Descarga_documento_respuesta = "YES"
                Exit Function
            End If
            Descarga_documento_respuesta = "YES"
        Catch ex As Exception
            Descarga_documento_respuesta = "Inconsistencia función Descarga_documento_respuesta " & ex.Message
        End Try
    End Function
    Function Descarga_documento_respuesta(ByVal id_respuesta As Integer,
                                          ByVal formato As String,
                                          ByVal estado_firma As Integer,
                                          ByRef Url_image As String,
                                          ByRef name_file As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaEnvioCorrespondencia
            Dim Refclasvisual As New ClassVisualisaDocumento
            Dim Refclasgestion As New Classgestionrespuesta
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                        estru)
            If Result <> "YES" Then
                Descarga_documento_respuesta = Result
                Exit Function
            End If
            If estru.FECHA_RESPUETA = "" Then
                Descarga_documento_respuesta = "No hay una respuesta para el documento "
                Exit Function
            End If
            If estru.ID_IMAGEN = 0 Then
                Descarga_documento_respuesta = "La solicitud no registra documento de respuesta para descargar "
                Exit Function
            End If

            Dim Matri_documentos() As String
            Erase Matri_documentos
            Result = Refclasvisual.Genera_Matris_Documentos_Almacenados(estru.ID_IMAGEN,
                                                                        UCase("imp03gestiontmp"),
                                                                        Matri_documentos)
            If Result <> "YES" Then
                Descarga_documento_respuesta = Result
                Exit Function
            End If
            If Matri_documentos.Length >= 2 Then
                Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/PUBLIC/DESCARGA/")
                If Directory.Exists(ruta_local) = False Then
                    Directory.CreateDirectory(ruta_local)
                End If
                Dim fileinf As New FileInfo(Matri_documentos(1))
                If File.Exists(Matri_documentos(1)) Then
                    If estru.ID_TIPO_DOC_RESPUESTA = 1 Then
                        Dim filecopia As String = ruta_local & id_respuesta & "-" & estru.RADICADO & fileinf.Extension
                        If File.Exists(filecopia) = True Then
                            File.Delete(filecopia)
                        End If
                        File.Copy(Matri_documentos(1), filecopia)
                        If File.Exists(filecopia) = True Then
                            Dim ruta_final As String = ruta_local & id_respuesta & "-" & estru.RADICADO & "." & fileinf.Extension
                            Dim archivo_final As String = HttpContext.Current.Session.Item("RA_ID_USUARIO") & "file_temp." & fileinf.Extension
                            If File.Exists(ruta_final) = True Then
                                Kill(ruta_final)
                            End If
                            Dim refclas_gembox As New ClassGaGembox
                            Dim ref_class_resp As New Class_ra_ra_registro_firma_documento
                            Dim rut_firma As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA_FINAL"))
                            Dim file_salida As String = ""
                            Dim id_usuario_gestion As Integer = estru.ID_REMIT_DEST_INT
                            Dim id_usuario_firma As Integer = 0
                            Result = ref_class_resp.Solicita_ultimo_usuario_firma_documento(id_respuesta,
                                                                                            id_usuario_firma)
                            If Result <> "YES" Then
                                Descarga_documento_respuesta = Result
                                Exit Function
                            End If
                            If id_usuario_firma = 0 Then
                                id_usuario_firma = id_usuario_gestion
                            End If
                            Result = refclas_gembox.Firma_documento_formato_respuesta(filecopia,
                                                                                   0,
                                                                                   HttpContext.Current.Session.Item("CLAVE_ARCHIVO_CERTIFICACION"),
                                                                                   HttpContext.Current.Session.Item("RUTA_ARCHIVO_CERTIFICACION"),
                                                                                   id_usuario_firma,
                                                                                   id_usuario_gestion,
                                                                                   estado_firma,
                                                                                   formato,
                                                                                   file_salida)
                            If Result <> "YES" Then
                                Descarga_documento_respuesta = Result
                                Exit Function
                            End If
                            Dim file_info As New FileInfo(file_salida)
                            Url_image = HttpContext.Current.Request.Url.Scheme & System.Uri.SchemeDelimiter & HttpContext.Current.Request.Url.Host & HttpContext.Current.Request.ApplicationPath & "/Temp_Image/PUBLIC/DESCARGA/" & file_info.Name
                            name_file = file_info.Name
                            Descarga_documento_respuesta = "YES"
                            Exit Function
                        End If
                    End If
                    If estru.ID_TIPO_DOC_RESPUESTA = 2 Then
                        Dim filecopia As String = ruta_local & id_respuesta & "-" & estru.RADICADO & fileinf.Extension
                        If File.Exists(filecopia) = True Then
                            File.Delete(filecopia)
                        End If
                        File.Copy(Matri_documentos(1), filecopia)
                        If File.Exists(filecopia) = True Then
                            Dim file_info As New FileInfo(filecopia)
                            name_file = file_info.Name
                            Url_image = HttpContext.Current.Request.Url.Scheme & System.Uri.SchemeDelimiter & HttpContext.Current.Request.Url.Host & HttpContext.Current.Request.ApplicationPath & "/Temp_Image/PUBLIC/DESCARGA/" & file_info.Name
                            Descarga_documento_respuesta = "YES"
                            Exit Function
                        End If
                    End If
                End If
            Else
                Descarga_documento_respuesta = "YES"
                Exit Function
            End If
            Descarga_documento_respuesta = "YES"
        Catch ex As Exception
            Descarga_documento_respuesta = "Inconsistencia función Descarga_documento_respuesta " & ex.Message
        End Try
    End Function

    Function Responder_a_la_solicitud(ByVal id_respuesta_radicado As Integer,
                                      ByVal estado_envia_ventanilla As Integer,
                                      ByVal estado_envia_correo_electronico As Integer,
                                      ByVal estado_firma_digital As Integer,
                                      ByVal id_usuario_gestion_firma As Integer,
                                      ByVal correo_electronico_envio As String,
                                      ByVal tipo_respuesta As String,
                                      ByRef Reuslttado_correo As String,
                                      ByRef url_image As String,
                                      ByRef url_image_electronica As String) As String
        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
        Dim Refclas_remit_dest_int As New Class_remit_dest_interno
        Dim Refclas_usuario_workflow As New ClassWorkflowUsuario
        Dim Refclas_visualiza_documentos As New ClassVisualisaDocumento
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim stru_envio As stru_envio = Nothing
        Dim id_imagen_plantilla As Integer = 0
        Dim radicado_respuesta As Integer = 0
        Dim id_imagen_respuesta As Integer = 0
        Dim estado_envio_respuesta As Integer = 0
        Dim nombre_cargo_usuario_firma_respuesta As String = ""
        Dim nombre_area_usuario_firma_respuesta As String = ""
        Dim id_area_usuario_firma_respuesta As Integer = 0
        Dim id_usuario_wf_firma_respuesta As Integer = 0
        Dim ruta_archivo_firma As String = ""
        Dim date1al As String = ""
        Dim Result As String = ""
        Reuslttado_correo = "YES"
        If estado_envia_ventanilla = 0 And estado_envia_correo_electronico = 0 Then
            Responder_a_la_solicitud = "Debe seleccionar el tipo de notificación entre correo electrónico o envio al centro de envio de correspondencia"
            Exit Function
        End If
        Result = Refclas_resp_radicado.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                        stru_envio)
        If Result <> "YES" Then
            Responder_a_la_solicitud = Result
            Exit Function
        End If
        '----------------------------------------------------
        'Verfica existencia de una respeusta a la solicitud
        '----------------------------------------------------
        If stru_envio.FECHA_RESPUETA <> "" Then
            Responder_a_la_solicitud = "La solicitud ya cuenta con respuesta, imposible continuar"
            Exit Function
        End If
        '--------------------------------------------------------------------
        'Verifica existencia documento respuesta cargado en el  sistema
        '--------------------------------------------------------------------
        If stru_envio.ID_IMAGEN = 0 Then
            Responder_a_la_solicitud = "Por favor cargue el documento respuesta "
            Exit Function
        End If
        If estado_envia_correo_electronico = 1 Then
            If correo_electronico_envio = "" Then
                Responder_a_la_solicitud = "Debe informar el correo electrónico para el envío del respuesta, de lo contrario inactive la opción (Confirmar respuesta al correo electrónico del peticionario) "
                Exit Function
            End If
        End If


        '-----------------------------------------------
        'Verifica estado solicitudes de aprobación sin
        'desición
        '-----------------------------------------------
        Dim Estado_solicitud_aprobacion As String = ""
        Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
        Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                                     Estado_solicitud_aprobacion,
                                                                                     HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Responder_a_la_solicitud = Result
            Exit Function
        End If
        If Estado_solicitud_aprobacion = "YES" Then
            Responder_a_la_solicitud = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
            Exit Function
        End If
        Result = ""
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now,
                                                                      date1al)
        If Result <> "YES" Then
            Responder_a_la_solicitud = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim stiempo As Object = Nothing
        Dim minuno As Object = Nothing
        Dim hora As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru_envio.FECHA_REGISTRO,
                                                                         stiempo,
                                                                         hora,
                                                                         minuno,
                                                                         dias_calendario,
                                                                         dias_no_habiles)
        If Result <> "YES" Then
            Responder_a_la_solicitud = Result
            Exit Function
        End If
        '-------------------------------------------------
        'Guarda documento respuesta
        '-------------------------------------------------
        Dim ClassAlmacenamiento As New ClassAlmacenamiento
        Dim matri_documentos_resp() As String = Nothing
        Result = ClassAlmacenamiento.Almacenar_documento_respuesta(id_respuesta_radicado,
                                                                   ruta_archivo_firma,
                                                                   estado_firma_digital,
                                                                   HttpContext.Current.Session.Item("CLAVE_ARCHIVO_CERTIFICACION"),
                                                                   HttpContext.Current.Session.Item("RUTA_ARCHIVO_CERTIFICACION"),
                                                                   stru_envio,
                                                                   id_usuario_gestion_firma,
                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                   matri_documentos_resp)
        If Result <> "YES" Then
            Responder_a_la_solicitud = "No se almaceno el documento de respuesta imposible confirmar el trámite error : " & Result
            Exit Function
        End If
        Dim Class_ra_registro_pqr As New Class_ra_registro_pqr
        Dim correo_electronico_usuario_pqr As String = ""
        Result = Class_ra_registro_pqr.Solicita_correo_electronico_usuario_pqr(stru_envio.RADICADO,
                                                                               correo_electronico_usuario_pqr)
        If Result <> "YES" Then
            Responder_a_la_solicitud = Result
            Exit Function
        End If
        Dim datehora As String = Date.Now.Hour
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '---------------------------------------------------------------------
            'Actualiza estado respuesta radicado
            '--------------------------------------------------------------------- 
            Dim sqlupdate_actualiza_estado_respuesta As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & estado_envio_respuesta & ", ID_RUTA_WF=" &
               HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",ID_TAREA_WF=" & HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") &
               ",FECHA_RESPUETA='" & date1al & "', HORA_RESPUESTA='" & datehora & "', " &
               "TIEMPO_RESPUESTA=" & Val(stiempo) &
               ",id_usuario_gestion_propietario=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
               ",TIPO_RESPUESTA_ELAB_USUARIO=" & "1" &
               ",TIPO_RESPUESTA='" & tipo_respuesta & "'" &
               "  where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado
            Dim hor As String = Now
            Dim detalle_trans As String = "RESPUESTA RADICADO"
            Dim campos_trans As String = ""
            Dim isert_datos As String = ""
            campos_trans = "RESPUESTA DE LA SOLICITUD INTERNA NUMERO (" & stru_envio.ID_RESPUESTA_RADICADO &
            ") DEL RADICADO " & stru_envio.RADICADO & " CON EL RADICADO SALIENTE : " & stru_envio.RADICADO_RESPUESTA & " Al peticionario " & stru_envio.DESTINATARIO &
            " el día " & date1al & " a las " & datehora
            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                         stru_envio.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW','" & campos_trans & "')"
            Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                                isert_datos
            Dim sql_registra_usuario_firma As String = "Insert into ra_ra_registro_firma_documento (ra_respuesta_radicado_ID_RESPUESTA_RADICADO," &
                                                       "remit_dest_interno_id_Remit_Dest_Int,id_usuario_firma,fecha_registro_firma) values (" &
                                                       id_respuesta_radicado & "," & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
                                                       "," & id_usuario_gestion_firma & ",'" & date1al & "')"
            myCommand.CommandText = sqlupdate_actualiza_estado_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Responder_a_la_solicitud = "Imposible actualizar respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Responder_a_la_solicitud = "Imposible actualizar log respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = sql_registra_usuario_firma
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Responder_a_la_solicitud = "Imposible registrar usuario firma  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            '-------------------------------------------------------------------
            'Actualiza el estado del tramite en workflow
            '-------------------------------------------------------------------
            Dim Refclas_adic As New Class_DAT_ADIC_TAR
            Result = Refclas_adic.Actualiza_estado_tramite_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                          Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                          "Tramitado")
            If Result <> "YES" Then
                Responder_a_la_solicitud = "Imposible actualizar el estado en workflow  " & Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            If Not HttpContext.Current.Session.Item("dat_gred_cahce") Is Nothing Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item(0) = Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")) Then
                        HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item("ESTADO") = "Tramitado"
                        HttpContext.Current.Session.Item("dat_gred_cahce").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item(0) = Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")) Then
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item("ESTADO") = "Tramitado"
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            '-------------------------------------------------------------------
            'Si la opción enviar a ventanilla no es la seleccionda el sistema
            'debe priorizar el envío del correo antes de registrar la respuesta
            '-------------------------------------------------------------------
            If estado_envia_correo_electronico = 1 Then
                Dim Refclas_correo As New ClassCorreo
                Dim matri_mensaje() As String = {""}
                If stru_envio.NOTA_RESPUESTA <> "" Then
                    Erase matri_mensaje
                    matri_mensaje = stru_envio.NOTA_RESPUESTA.ToString.Split(vbCrLf)
                End If
                If stru_envio.FECHA_RESPUETA = "" Then
                    stru_envio.FECHA_RESPUETA = date1al
                End If
                Dim correo_usuario_gestion As String = ""
                Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
                Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                  correo_usuario_gestion)
                If Result <> "YES" Then
                    Reuslttado_correo = Result

                End If
                Dim nombre_remitente As String = ""
                Dim cargo_remit As String = ""
                Dim ref_class_remit_interno As New Class_remit_dest_interno
                Result = ref_class_remit_interno.Retorna_nombre_cargo_destinatario_interno(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                          nombre_remitente,
                                                                                          cargo_remit)
                If Result <> "YES" Then
                    Reuslttado_correo = Result

                End If
                Dim nombre_area As String = ""
                Result = Refclas_remit_dest_int.Solicita_id_area_nombre_area_destinatario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                          0,
                                                                                          nombre_area)
                If Result <> "YES" Then
                    Reuslttado_correo = Result

                End If
                Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" &
                                                                              HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
                Dim matri_anexos() As String = Nothing
                Result = Me.Genera_zip_documento_anexo(id_respuesta_radicado,
                                                       matri_anexos)
                If Result <> "YES" Then
                    Reuslttado_correo = Result

                End If
                If correo_electronico_usuario_pqr <> "" Then
                    If correo_electronico_envio <> "" Then
                        correo_electronico_envio = correo_electronico_envio & "," & correo_electronico_usuario_pqr
                    Else
                        correo_electronico_envio = correo_electronico_usuario_pqr
                    End If
                End If
                Result = Refclas_correo.Envio_Correo_respuesta_documento(stru_envio.RADICADO_RESPUESTA,
                                                                         matri_mensaje,
                                                                         correo_electronico_envio,
                                                                         matri_documentos_resp,
                                                                         nombre_remitente,
                                                                         cargo_remit,
                                                                         nombre_area,
                                                                         correo_usuario_gestion,
                                                                         stru_envio,
                                                                         ruta_local,
                                                                         matri_anexos)
                If Result <> "YES" Then
                    Reuslttado_correo = "Se completo la respuesta, pero no se pudo notificar al correo del peticionario. Código error del sistema " & Result

                End If
            End If
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            If estado_envia_correo_electronico = 0 Then
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta(id_respuesta_radicado,
                                                                               url_image)
                If Result <> "YES" Then
                    Responder_a_la_solicitud = "Se completo la respuesta, pero no se pudo determinar el estado " & Result
                    Exit Function
                End If
            Else
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta_electronica(id_respuesta_radicado,
                                                                                           url_image)
                If Result <> "YES" Then
                    Responder_a_la_solicitud = "Se completo la respuesta, pero no se pudo determinar el estado " & Result
                    Exit Function
                End If
            End If
            If estado_envia_correo_electronico = 0 Then
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion(id_respuesta_radicado,
                                                                                     url_image_electronica)
                If Result <> "YES" Then
                    Responder_a_la_solicitud = "Se completo la respuesta, pero no se pudo determinar el estado " & Result
                    Exit Function

                End If
            Else
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(id_respuesta_radicado,
                                                                                             url_image_electronica)
                If Result <> "YES" Then
                    Responder_a_la_solicitud = "Se completo la respuesta, pero no se pudo determinar el estado " & Result
                    Exit Function

                End If
            End If
            Responder_a_la_solicitud = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Responder_a_la_solicitud = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Responder_a_la_solicitud = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Responder_a_la_solicitud(ByVal id_respuesta_radicado As Integer,
                                      ByRef pag As Page,
                                      ByVal tipo_interface As Integer,
                                      ByVal id_usuario_gestion_firma As Integer,
                                      ByRef Reuslttado_correo As String) As String
        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
        Dim Refclas_remit_dest_int As New Class_remit_dest_interno
        Dim Refclas_usuario_workflow As New ClassWorkflowUsuario
        Dim Refclas_visualiza_documentos As New ClassVisualisaDocumento
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim stru_envio As stru_envio = Nothing
        Dim id_imagen_plantilla As Integer = 0
        Dim radicado_respuesta As Integer = 0
        Dim id_imagen_respuesta As Integer = 0
        Dim estado_envio_respuesta As Integer = 0
        Dim nombre_cargo_usuario_firma_respuesta As String = ""
        Dim nombre_area_usuario_firma_respuesta As String = ""
        Dim id_area_usuario_firma_respuesta As Integer = 0
        Dim id_usuario_wf_firma_respuesta As Integer = 0
        Dim ruta_archivo_firma As String = ""
        'Dim matri_documento() As String = Nothing
        Dim date1al As String = ""
        Dim Result As String = ""
        Dim CheckBox_envio_correo As CheckBox = pag.FindControl("CheckBox_envio_correo_ra")
        If CheckBox_envio_correo Is Nothing Then
            Responder_a_la_solicitud = "Imposible encontrar el control CheckBox_envio_correo"
            Exit Function
        End If
        Dim CheckBox_envia_ventanilla As CheckBox = pag.FindControl("CheckBox_envia_ventanilla_ra")
        If CheckBox_envia_ventanilla Is Nothing Then
            Responder_a_la_solicitud = "Imposible encontrar el control CheckBox_envia_ventanilla"
            Exit Function
        End If
        Dim TextBox_correo_electronico As Object = Nothing
        If tipo_interface = 0 Then
            TextBox_correo_electronico = pag.FindControl("Hidden_text_user_correo")
        Else
            TextBox_correo_electronico = pag.FindControl("Hidden_text_user_correo")
        End If

        If TextBox_correo_electronico Is Nothing Then
            Responder_a_la_solicitud = "Imposible encontrar el control TextBox_correo_electronico"
            Exit Function
        End If
        Dim Hidden_radicado As HtmlInputHidden = pag.FindControl("Hidden_radicado")
        If Hidden_radicado Is Nothing Then
            Responder_a_la_solicitud = "Imposible encontrar el control Hidden_radicado"
            Exit Function
        End If
        If CheckBox_envio_correo.Checked = False And CheckBox_envia_ventanilla.Checked = False Then
            Responder_a_la_solicitud = "Por favor informe la opción de enviar por correo,  o solicitar al centro envio que envie su respuesta"
            Exit Function
        End If
        Dim CheckBox_firma_digital As CheckBox = pag.FindControl("CheckBox_firma_digital")
        If CheckBox_firma_digital Is Nothing Then
            Responder_a_la_solicitud = "Imposible encontrar el control CheckBox_firma_digital"
            Exit Function
        End If

        Result = Refclas_resp_radicado.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                        stru_envio)
        If Result <> "YES" Then
            Responder_a_la_solicitud = Result
            Exit Function
        End If
        '----------------------------------------------------
        'Verfica existencia de una respeusta a la solicitud
        '----------------------------------------------------
        If stru_envio.FECHA_RESPUETA <> "" Then
            Responder_a_la_solicitud = "La solicitud ya cuenta con respuesta, imposible continuar"
            Exit Function
        End If
        '--------------------------------------------------------------------
        'Verifica existencia documento respuesta cargado en el  sistema
        '--------------------------------------------------------------------
        If stru_envio.ID_IMAGEN = 0 Then
            Responder_a_la_solicitud = "Por favor cargue el documento respuesta "
            Exit Function
        End If
        If CheckBox_envio_correo.Checked = True Then
            If TextBox_correo_electronico.value = "" Then
                Responder_a_la_solicitud = "Debe informar el correo electrónico para el envío del respuesta, de lo contrario inactive la opción (Confirmar respuesta al correo electrónico del peticionario) "
                Exit Function
            End If
        End If

        Dim estado_envio_correo As Integer = 0
        If CheckBox_envio_correo.Checked = True Then
            estado_envio_correo = 1
        End If
        Dim option_envia_ventanilla As Integer = 0
        If CheckBox_envia_ventanilla.Checked = True Then
            estado_envio_respuesta = 1
            option_envia_ventanilla = 1
        Else
            estado_envio_respuesta = 1
            option_envia_ventanilla = 0
        End If
        Dim opcion_certificado_digital As Integer = 0
        If CheckBox_firma_digital.Checked = True Then
            opcion_certificado_digital = 1
        End If
        '-----------------------------------------------
        'Verifica estado solicitudes de aprobación sin
        'desición
        '-----------------------------------------------
        Dim Estado_solicitud_aprobacion As String = ""
        Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
        Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                                     Estado_solicitud_aprobacion,
                                                                                     HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Responder_a_la_solicitud = Result
            Exit Function
        End If
        If Estado_solicitud_aprobacion = "YES" Then
            Responder_a_la_solicitud = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
            Exit Function
        End If
        Result = ""
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now,
                                                                      date1al)
        If Result <> "YES" Then
            Responder_a_la_solicitud = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim stiempo As Object = Nothing
        Dim minuno As Object = Nothing
        Dim hora As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(stru_envio.FECHA_REGISTRO,
                                                                         stiempo,
                                                                         hora,
                                                                         minuno,
                                                                         dias_calendario,
                                                                         dias_no_habiles)
        If Result <> "YES" Then
            Responder_a_la_solicitud = Result
            Exit Function
        End If
        '-------------------------------------------------
        'Guarda documento respuesta
        '-------------------------------------------------
        Dim ClassAlmacenamiento As New ClassAlmacenamiento
        Dim matri_documentos_resp() As String = Nothing
        Result = ClassAlmacenamiento.Almacenar_documento_respuesta(id_respuesta_radicado,
                                                                   ruta_archivo_firma,
                                                                   opcion_certificado_digital,
                                                                   HttpContext.Current.Session.Item("CLAVE_ARCHIVO_CERTIFICACION"),
                                                                   HttpContext.Current.Session.Item("RUTA_ARCHIVO_CERTIFICACION"),
                                                                   stru_envio,
                                                                   id_usuario_gestion_firma,
                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                   matri_documentos_resp)
        If Result <> "YES" Then
            Responder_a_la_solicitud = "No se almaceno el documento de respuesta imposible confirmar el trámite error : " & Result
            Exit Function
        End If
        Dim datehora As String = Date.Now.Hour
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '---------------------------------------------------------------------
            'Actualiza estado respuesta radicado
            '--------------------------------------------------------------------- 
            Dim sqlupdate_actualiza_estado_respuesta As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & estado_envio_respuesta & ", ID_RUTA_WF=" &
               HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",ID_TAREA_WF=" & HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") &
               ",FECHA_RESPUETA='" & date1al & "', HORA_RESPUESTA='" & datehora & "', " &
               "TIEMPO_RESPUESTA=" & Val(stiempo) &
               ",id_usuario_gestion_propietario=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
               ",TIPO_RESPUESTA_ELAB_USUARIO=" & "1" &
               "  where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado
            Dim hor As String = Now
            Dim detalle_trans As String = "RESPUESTA RADICADO"
            Dim campos_trans As String = ""
            Dim isert_datos As String = ""
            campos_trans = "RESPUESTA DE LA SOLICITUD INTERNA NUMERO (" & stru_envio.ID_RESPUESTA_RADICADO &
            ") DEL RADICADO " & stru_envio.RADICADO & " CON EL RADICADO SALIENTE : " & stru_envio.RADICADO_RESPUESTA & " Al peticionario " & stru_envio.DESTINATARIO &
            " el día " & date1al & " a las " & datehora
            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                         stru_envio.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW','" & campos_trans & "')"
            Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                                isert_datos
            Dim sql_registra_usuario_firma As String = "Insert into ra_ra_registro_firma_documento (ra_respuesta_radicado_ID_RESPUESTA_RADICADO," &
                                                       "remit_dest_interno_id_Remit_Dest_Int,id_usuario_firma,fecha_registro_firma) values (" &
                                                       id_respuesta_radicado & "," & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
                                                       "," & id_usuario_gestion_firma & ",'" & date1al & "')"
            myCommand.CommandText = sqlupdate_actualiza_estado_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Responder_a_la_solicitud = "Imposible actualizar respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Responder_a_la_solicitud = "Imposible actualizar log respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = sql_registra_usuario_firma
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Responder_a_la_solicitud = "Imposible registrar usuario firma  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            '-------------------------------------------------------------------
            'Actualiza el estado del tramite en workflow
            '-------------------------------------------------------------------
            Dim Result_ As String = "YES"
            Dim Refclas_adic As New Class_DAT_ADIC_TAR
            Result_ = Refclas_adic.Actualiza_estado_tramite_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                           Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                           "Tramitado")
            '-------------------------------------------------------------------
            'Si la opción enviar a ventanilla no es la seleccionda el sistema
            'debe priorizar el envío del correo antes de registrar la respuesta
            '-------------------------------------------------------------------
            If estado_envio_correo = 1 Then
                Dim Refclas_correo As New ClassCorreo
                Dim matri_mensaje() As String = {""}
                If stru_envio.NOTA_RESPUESTA <> "" Then
                    Erase matri_mensaje
                    matri_mensaje = stru_envio.NOTA_RESPUESTA.ToString.Split(vbCrLf)
                End If
                If stru_envio.FECHA_RESPUETA = "" Then
                    stru_envio.FECHA_RESPUETA = date1al
                End If
                Dim correo_usuario_gestion As String = ""
                Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
                Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                  correo_usuario_gestion)
                If Result <> "YES" Then
                    Reuslttado_correo = Reuslttado_correo & Result
                    Responder_a_la_solicitud = "YES"
                    Exit Function
                End If
                Dim nombre_remitente As String = ""
                Dim cargo_remit As String = ""
                Dim ref_class_remit_interno As New Class_remit_dest_interno
                Result = ref_class_remit_interno.Retorna_nombre_cargo_destinatario_interno(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                          nombre_remitente,
                                                                                          cargo_remit)
                If Result <> "YES" Then
                    Reuslttado_correo = Reuslttado_correo & Result
                    Responder_a_la_solicitud = "YES"
                    Exit Function
                End If
                Dim nombre_area As String = ""
                Result = Refclas_remit_dest_int.Solicita_id_area_nombre_area_destinatario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                          0,
                                                                                          nombre_area)
                If Result <> "YES" Then
                    Reuslttado_correo = Reuslttado_correo & Result
                    Responder_a_la_solicitud = "YES"
                    Exit Function
                End If
                Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" &
                                                                              HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
                Dim matri_anexos() As String = Nothing
                Result = Me.Genera_zip_documento_anexo(id_respuesta_radicado,
                                                 matri_anexos)
                If Result <> "YES" Then
                    Reuslttado_correo = Reuslttado_correo & Result
                    Responder_a_la_solicitud = "YES"
                    Exit Function
                End If
                Result = Refclas_correo.Envio_Correo_respuesta_documento(stru_envio.RADICADO_RESPUESTA,
                                                                         matri_mensaje,
                                                                         TextBox_correo_electronico.value,
                                                                         matri_documentos_resp,
                                                                         nombre_remitente,
                                                                         cargo_remit,
                                                                         nombre_area,
                                                                         correo_usuario_gestion,
                                                                         stru_envio,
                                                                         ruta_local,
                                                                         matri_anexos)
                If Result <> "YES" Then
                    Reuslttado_correo = Reuslttado_correo & "Se completo la respuesta, pero no se pudo notificar al correo del peticionario. Código error del sistema " & Result
                    Responder_a_la_solicitud = "YES"
                    Exit Function
                End If
            End If
            If Result_ <> "YES" Then
                Responder_a_la_solicitud = "Se completo la respuesta, pero no se pudo cambia el estado del tramite en workflow " & Result_
                Exit Function
            End If
            Responder_a_la_solicitud = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Responder_a_la_solicitud = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Responder_a_la_solicitud = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    'Function Confirma_envio_respuesta_con_radicado(ByVal id_respuesta_radicado As Integer, _
    '                                               ByRef pag As Page, _
    '                                               ByVal tipo_interface As Integer, _
    '                                               ByRef Reuslttado_correo As String) As String
    '    Dim id_imagen_plantilla As Integer = 0
    '    Dim radicado_respuesta As Integer = 0
    '    Dim id_imagen_respuesta As Integer = 0
    '    Dim estado_envio_respuesta As Integer = 0
    '    Dim Result As String = ""
    '    Dim refclas As New ClassRadicador
    '    Try
    '        Dim CheckBox_envio_correo As CheckBox = pag.FindControl("CheckBox_envio_correo_ra")
    '        If CheckBox_envio_correo Is Nothing Then
    '            Confirma_envio_respuesta_con_radicado = "Imposible encontrar el control CheckBox_envio_correo"
    '            Exit Function
    '        End If
    '        Dim CheckBox_envia_ventanilla As CheckBox = pag.FindControl("CheckBox_envia_ventanilla_ra")
    '        If CheckBox_envia_ventanilla Is Nothing Then
    '            Confirma_envio_respuesta_con_radicado = "Imposible encontrar el control CheckBox_envia_ventanilla"
    '            Exit Function
    '        End If
    '        Dim TextBox_correo_electronico As TextBox = Nothing
    '        If tipo_interface = 0 Then
    '            TextBox_correo_electronico = pag.FindControl("TextBox_correo_electronico_interf_ra")
    '        Else
    '            TextBox_correo_electronico = pag.FindControl("TextBox_correo_electronico_interf_ra")
    '        End If

    '        If TextBox_correo_electronico Is Nothing Then
    '            Confirma_envio_respuesta_con_radicado = "Imposible encontrar el control TextBox_correo_electronico"
    '            Exit Function
    '        End If
    '        Dim Hidden_radicado As HtmlInputHidden = pag.FindControl("Hidden_radicado")
    '        If Hidden_radicado Is Nothing Then
    '            Confirma_envio_respuesta_con_radicado = "Imposible encontrar el control Hidden_radicado"
    '            Exit Function
    '        End If
    '        If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
    '            Confirma_envio_respuesta_con_radicado = "Por favor active web service para workflow"
    '            Exit Function
    '        End If
    '        If HttpContext.Current.Session.Item("URL_WEB_SERVICE") = "" Then
    '            Confirma_envio_respuesta_con_radicado = "Por favor informe la url web service para workflow"
    '            Exit Function
    '        End If
    '        If CheckBox_envio_correo.Checked = False And CheckBox_envia_ventanilla.Checked = False Then
    '            Confirma_envio_respuesta_con_radicado = "Por favor informe la opción de enviar por correo,  o solicitar al centro envio que envie su respuesta"
    '            Exit Function
    '        End If
    '        Dim CheckBox_firma_digital As CheckBox = pag.FindControl("CheckBox_firma_digital")
    '        If CheckBox_firma_digital Is Nothing Then
    '            Confirma_envio_respuesta_con_radicado = "Imposible encontrar el control CheckBox_firma_digital"
    '            Exit Function
    '        End If
    '        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
    '        Dim stru_envio As stru_envio = Nothing
    '        Result = Refclas_resp_radicado.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado, _
    '                                                                                        stru_envio)
    '        If Result <> "YES" Then
    '            Confirma_envio_respuesta_con_radicado = Result
    '            Exit Function
    '        End If
    '        'Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado, _
    '        '                                                                    id_imagen_plantilla, _
    '        '                                                                    radicado_respuesta, _
    '        '                                                                    fecha_respuesta, _
    '        '                                                                    id_imagen_respuesta, _
    '        '                                                                    estado_envio_respuesta)
    '        'If Result <> "YES" Then
    '        '    Confirma_envio_respuesta_con_radicado = Result
    '        '    Exit Function
    '        'End If
    '        'Dim codigo_radicado As String = ""
    '        If stru_envio.FECHA_RESPUETA = "" Then
    '            Confirma_envio_respuesta_con_radicado = "La solicitud ya cuenta con respuesta, imposible continuar"
    '            Exit Function
    '        End If
    '        'Dim id_imagen As Integer = -1
    '        'Dim gabinete_imagen As String = ""
    '        'Dim refclas_gestion As New Classgestionrespuesta
    '        'Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(id_respuesta_radicado, _
    '        '                                                                  gabinete_imagen, _
    '        '                                                                  id_imagen)
    '        'If Result <> "YES" Then
    '        '    Confirma_envio_respuesta_con_radicado = Result
    '        '    Exit Function
    '        'End If
    '        If stru_envio.ID_IMAGEN = 0 Then
    '            Confirma_envio_respuesta_con_radicado = "Por favor cargue el documento respuesta "
    '            Exit Function
    '        End If
    '        Dim refclas_visualiza As New ClassVisualisaDocumento
    '        Dim matri_documento() As String = Nothing
    '        Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(stru_envio.ID_IMAGEN, _
    '                                                                        "IMP03GESTIONTMP", _
    '                                                                        matri_documento)
    '        If Result <> "YES" Then
    '            Confirma_envio_respuesta_con_radicado = Result
    '            Exit Function
    '        End If
    '        If CheckBox_envio_correo.Checked = True Then
    '            If TextBox_correo_electronico.Text = "" Then
    '                Confirma_envio_respuesta_con_radicado = "Debe informar el correo electrónico de respuesta "
    '                Exit Function
    '            End If
    '        End If
    '        'Dim nombre_plantilla As String = ""
    '        'Dim id_plantilla As Integer = -1
    '        'Dim ref_calss_system As New Class_system_plantilla_radicado
    '        'Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla)
    '        'If Result <> "YES" Then
    '        '    Confirma_envio_respuesta_con_radicado = Result
    '        '    Exit Function
    '        'End If
    '        Dim estado_envio_correo As Integer = 0
    '        If CheckBox_envio_correo.Checked = True Then
    '            estado_envio_correo = 1
    '        End If
    '        Dim option_envia_ventanilla As Integer = 0
    '        If CheckBox_envia_ventanilla.Checked = True Then
    '            estado_envio_respuesta = 1
    '            option_envia_ventanilla = 1
    '        Else
    '            estado_envio_respuesta = 5
    '            option_envia_ventanilla = 0
    '        End If
    '        Dim opcion_certificado_digital As Integer = 0
    '        If CheckBox_firma_digital.Checked = True Then
    '            opcion_certificado_digital = 1
    '        End If
    '        '-----------------------------------------------
    '        'Verifica estado solicitudes de aprobación sin
    '        'desición
    '        '-----------------------------------------------
    '        Dim Estado_solicitud_aprobacion As String = ""
    '        Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
    '        Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")), _
    '                                                                                     Estado_solicitud_aprobacion, _
    '                                                                                     HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
    '        If Result <> "YES" Then
    '            Confirma_envio_respuesta_con_radicado = Result
    '            Exit Function
    '        End If
    '        If Estado_solicitud_aprobacion = "YES" Then
    '            Confirma_envio_respuesta_con_radicado = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
    '            Exit Function
    '        End If
    '        Dim rut_firma As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_FIRMA_FINAL"))
    '        Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
    '        Result = Me.Confirma_respuesta_tramite(id_respuesta_radicado, _
    '                                               HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"), _
    '                                               HttpContext.Current.Session.Item("Id_Ruta_Workflow"), _
    '                                               estado_envio_correo, _
    '                                                TextBox_correo_electronico.Text, _
    '                                                1, estado_envio_respuesta, 1, _
    '                                               matri_documento, Reuslttado_correo, _
    '                                                ruta_local, option_envia_ventanilla, _
    '                                               rut_firma, opcion_certificado_digital, _
    '                                               HttpContext.Current.Session.Item("CLAVE_ARCHIVO_CERTIFICACION"), _
    '                                               HttpContext.Current.Session.Item("RUTA_ARCHIVO_CERTIFICACION"))
    '        If Result <> "YES" Then
    '            Confirma_envio_respuesta_con_radicado = Result
    '            Exit Function
    '        End If
    '        Confirma_envio_respuesta_con_radicado = "YES"
    '        Exit Function

    '    Catch ex As Exception
    '        Confirma_envio_respuesta_con_radicado = "Inconsistencia general función Confirma_envio_respuesta_con_radicado " & ex.Message
    '    End Try
    'End Function
    'Function Confirma_respuesta_tramite(ByVal id_registro_respuesta As Integer, _
    '                                    ByVal id_tarea_wf As Integer, _
    '                                    ByVal id_ruta_wf As Integer, _
    '                                    ByVal opcion_envia_correo As Integer, _
    '                                    ByVal correos_envio As String, _
    '                                    ByVal estado_anexo As Integer, _
    '                                    ByVal estado_respuesta As Integer, _
    '                                    ByVal option_almacena_documento_respuesta As Integer, _
    '                                    ByVal matri_documentos() As String, _
    '                                    ByRef resultado_correo As String, _
    '                                    ByVal ruta_temporal_documento As String, _
    '                                    ByVal option_envia_ventanilla As Integer, _
    '                                    ByVal file_firma As String, _
    '                                    ByVal opcion_firma_digital As String, _
    '                                    ByVal pasword_firma_digital As String, _
    '                                    ByVal file_digital_archivo As String) As String
    '    Dim Result As String = ""
    '    Confirma_respuesta_tramite = ""
    '    Dim matri_anexos() As String = Nothing
    '    Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & id_registro_respuesta & "/"
    '    If opcion_envia_correo = 1 Then
    '        If correos_envio = "" Then
    '            Confirma_respuesta_tramite = "Debe informar el correo electrónico"
    '            Exit Function
    '        End If
    '        If estado_anexo = 1 Then
    '            Dim Refclas_gestion As New Classgestionrespuesta
    '            'CAMBIO POR METODO DE ANEXOS
    '            Result = Refclas_gestion.Lista_ruta_documentos_anexos_respuesta(HttpContext.Current.Server.MapPath(ruta_virtual), _
    '                                                                            matri_anexos)
    '            If Result <> "YES" Then
    '                Confirma_respuesta_tramite = Result
    '                Exit Function
    '            End If
    '        End If
    '    End If
    '    Dim Refclas_respuesta As New Classgestionrespuesta
    '    Dim id_imagen As Integer = 0
    '    Dim estru As stru_envio = Nothing
    '    Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
    '    Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_registro_respuesta, _
    '                                                                                estru)
    '    If Result <> "YES" Then
    '        Confirma_respuesta_tramite = Result
    '        Exit Function
    '    End If

    '    If estru.FECHA_RESPUETA <> "" Then
    '        Confirma_respuesta_tramite = "El radicado tiene una respuesta para esta solicitud, el sistema no permite volver a confirmar "
    '        Exit Function
    '    End If
    '    If estru.ID_IMAGEN = 0 Then
    '        Confirma_respuesta_tramite = "Debe guardar el documento respuesta para poder confirmar la respuesta"
    '        Exit Function
    '    End If
    '    Dim refclasalmacen As New ClassRadicador
    '    Dim refclas_gestion_fechas As New ClassGestionFechas
    '    Dim date1al As String = ""
    '    Result = ""
    '    Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now, _
    '                                                                  date1al)
    '    If Result <> "YES" Then
    '        Confirma_respuesta_tramite = "Imposible formatear fecha " & Result
    '        Exit Function
    '    End If
    '    Dim stiempo As Object = Nothing
    '    Dim minuno As Object = Nothing
    '    Dim hora As Object = Nothing
    '    Dim dias_calendario As Object = Nothing
    '    Dim dias_no_habiles As Object = Nothing
    '    Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(estru.FECHA_REGISTRO, _
    '                                                                     stiempo, _
    '                                                                     hora, _
    '                                                                     minuno, _
    '                                                                     dias_calendario, _
    '                                                                     dias_no_habiles)
    '    If Result <> "YES" Then
    '        Confirma_respuesta_tramite = Result
    '        Exit Function
    '    End If
    '    Dim nombre_remitente As String = ""
    '    Dim cargo_remit As String = ""
    '    Dim ref_class_remit_interno As New Class_remit_dest_interno
    '    Result = ref_class_remit_interno.Retorna_nombre_cargo_destinatario_interno(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
    '                                                                              nombre_remitente, _
    '                                                                              cargo_remit)
    '    If Result <> "YES" Then
    '        Confirma_respuesta_tramite = Result
    '        Exit Function
    '    End If
    '    Dim nombre_area As String = ""
    '    Dim id_area As Integer = -1
    '    Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
    '    Result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(estru.ID_REMIT_DEST_INT, _
    '                                                                 id_area, _
    '                                                                 nombre_area)
    '    If Result <> "YES" Then
    '        Confirma_respuesta_tramite = Result
    '        Exit Function
    '    End If
    '    '-------------------------------------------------
    '    'Guarda documento respuesta
    '    '-------------------------------------------------
    '    Dim Refclasgestion As New Classgestionrespuesta
    '    Dim matri_documentos_resp() As String = Nothing
    '    If option_almacena_documento_respuesta = 1 Then
    '        Result = Refclasgestion.Almacenar_documento_respuesta(id_registro_respuesta, _
    '                                                              file_firma, _
    '                                                              opcion_firma_digital, _
    '                                                              pasword_firma_digital, _
    '                                                              file_digital_archivo, _
    '                                                              estru, _
    '                                                              id_u
    '                                                              matri_documentos_resp)
    '        If Result <> "YES" Then
    '            Confirma_respuesta_tramite = "No se almaceno el documento de respuesta imposible confirmar el trámite error : " & Result
    '            Exit Function
    '        End If
    '    End If
    '    Dim datehora As String = Date.Now
    '    Dim myConnection As New MySqlConnection
    '    Dim ref As New conect.Dbase_Conction_Mysql_RA
    '    ref.Returna_Conexion_Mysql(myConnection)
    '    Dim myTrans As MySqlTransaction
    '    Dim sqlresultinsert As Integer = 0
    '    Try
    '        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
    '        myTrans = myConnection.BeginTransaction()
    '        myCommand.Connection = myConnection
    '        myCommand.Transaction = myTrans
    '        '---------------------------------------------------------------------
    '        'Actualiza estado respuesta radicado
    '        '--------------------------------------------------------------------- 
    '        Dim sqlupdate_actualiza_estado_respuesta As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & estado_respuesta & ", ID_RUTA_WF=" & _
    '           id_ruta_wf & ",ID_TAREA_WF=" & id_tarea_wf & ",FECHA_RESPUETA='" & date1al & "', HORA_RESPUESTA='" & datehora & "', " & _
    '           "TIEMPO_RESPUESTA=" & Val(stiempo) & _
    '           ",id_usuario_gestion_propietario=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & _
    '           ",TIPO_RESPUESTA_ELAB_USUARIO=" & "1" & _
    '           "  where ID_RESPUESTA_RADICADO=" & id_registro_respuesta
    '        Dim hor As String = Now
    '        Dim detalle_trans As String = "RESPUESTA RADICADO"
    '        Dim campos_trans As String = ""
    '        Dim isert_datos As String = ""
    '        campos_trans = "RESPUESTA DE LA SOLICITUD INTERNA NUMERO (" & estru.ID_RESPUESTA_RADICADO & _
    '        ") DEL RADICADO " & estru.RADICADO & " CON EL RADICADO SALIENTE : " & estru.RADICADO_RESPUESTA & " Al peticionario " & estru.DESTINATARIO & _
    '        " el día " & date1al & " a las " & datehora
    '        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," & _
    '                     estru.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW','" & campos_trans & "')"
    '        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" & _
    '                                            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & _
    '                                            isert_datos

    '        myCommand.CommandText = sqlupdate_actualiza_estado_respuesta
    '        sqlresultinsert = myCommand.ExecuteNonQuery()
    '        If sqlresultinsert = 0 Then
    '            Confirma_respuesta_tramite = "Imposible actualizar confirmacion plantila  "
    '            myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        myCommand.CommandText = update_gestion
    '        sqlresultinsert = myCommand.ExecuteNonQuery()
    '        If sqlresultinsert = 0 Then
    '            Confirma_respuesta_tramite = "Imposible actualizar el log de la plantilla  "
    '            myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        myTrans.Commit()
    '        myConnection.Close()
    '        '-------------------------------------------------------------------
    '        'Si la opción enviar a ventanilla no es la seleccionda el sistema
    '        'debe priorizar el envío del correo antes de registrar la respuesta
    '        '-------------------------------------------------------------------
    '        If opcion_envia_correo = 1 Then
    '            Dim Refclas_correo As New ClassCorreo
    '            Dim matri_mensaje() As String = {""}
    '            If estru.NOTA_RESPUESTA <> "" Then
    '                Erase matri_mensaje
    '                matri_mensaje = estru.NOTA_RESPUESTA.ToString.Split(vbCrLf)
    '            End If
    '            If estru.FECHA_RESPUETA = "" Then
    '                estru.FECHA_RESPUETA = date1al
    '            End If
    '            Dim correo_usuario_gestion As String = ""
    '            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
    '            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
    '                                                                              correo_usuario_gestion)
    '            If Result <> "YES" Then
    '                resultado_correo = resultado_correo & "Imposible registrar la respuesta y el medio de envío, no se encontro el correo electrónico del usuario de gestión, el sistema dice  " & Result
    '                Confirma_respuesta_tramite = "Imposible registrar la respuesta y el medio de envío, no se encontro el correo electrónico del usuario de gestión, el sistema dice  " & Result
    '                Exit Function
    '            End If
    '            Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" & _
    '                                                                          HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
    '            Result = Refclas_correo.Envio_Correo_respuesta_documento(estru.RADICADO_RESPUESTA, _
    '                                                                     matri_mensaje, _
    '                                                                     correos_envio, _
    '                                                                     matri_documentos_resp, _
    '                                                                     nombre_remitente, _
    '                                                                     cargo_remit, _
    '                                                                     nombre_area, _
    '                                                                     correo_usuario_gestion, _
    '                                                                     estru, _
    '                                                                     ruta_temporal_documento, _
    '                                                                     matri_anexos)
    '            If Result <> "YES" Then
    '                resultado_correo = resultado_correo & "Imposible registrar la respuesta y el medio de envío, por que no se pudo notificar al correo del peticionario. Código error del sistema " & Result
    '                Confirma_respuesta_tramite = "Imposible registrar la respuesta y el medio de envío, por que no se pudo notificar al correo del peticionario. Código error del sistema " & Result
    '                Exit Function
    '            End If
    '        End If

    '        If Confirma_respuesta_tramite <> "" Then
    '        Else
    '            Confirma_respuesta_tramite = "YES"
    '        End If

    '    Catch e As Exception
    '        Try
    '            myTrans.Rollback()
    '        Catch ex As MySqlException
    '            If Not myTrans.Connection Is Nothing Then
    '                Confirma_respuesta_tramite = "An exception of type " + ex.GetType().ToString() + _
    '                                  " was encountered while attempting to roll back the transaction."
    '                Exit Function
    '            End If
    '        End Try
    '        If Not myConnection Is Nothing Then
    '            myConnection.Close()
    '        End If
    '        Confirma_respuesta_tramite = "Error General " & e.Message
    '        Exit Function
    '    End Try
    'End Function

    'Function Retorna_parametros_almacenamiento_documento_permanente_almacenado(ByVal id_registro_respuesta As Integer, _
    '                                                                           ByRef matris_datos_almacen() As String, _
    '                                                                           ByRef stru_gestion As estructure_gestion, _
    '                                                                           ByRef matris_documentos() As String, _
    '                                                                           ByRef nombre_gabinete_respuesta As String, _
    '                                                                           ByVal ruta_tempo As String, _
    '                                                                           ByVal ruta_documento_alamcen As String, _
    '                                                                           ByVal id_imagen_seleccionada As Integer, _
    '                                                                           ByVal file_firma As String, _
    '                                                                           ByVal opcion_firma_digital As String, _
    '                                                                           ByVal pasword_firma_digital As String, _
    '                                                                           ByVal file_digital_archivo As String, _
    '                                                                           ByVal stru_envi As stru_envio) As String
    '    Try
    '        Dim Result As String = ""
    '        Dim refclas_workflow As New ClassWorkflow
    '        '-----------------------------------------------------------------------------
    '        'Retorna la matriz de documento plantilla a guardar en formto tif
    '        '-----------------------------------------------------------------------------
    '        Dim ref_almacenaminento As New ClassVisualisaDocumento
    '        Dim mtri_documento() As String = Nothing
    '        Result = ref_almacenaminento.Genera_Matris_Documentos_Almacenados(stru_envi.ID_IMAGEN, _
    '                                                                          "IMP03GESTIONTMP", _
    '                                                                          mtri_documento)
    '        If Result <> "YES" Then
    '            Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '            Exit Function
    '        End If
    '        If File.Exists(mtri_documento(1)) = False Then
    '            Retorna_parametros_almacenamiento_documento_permanente_almacenado = "Por favor adjunte el documento respuesta"
    '            Exit Function
    '        End If
    '        Dim Ruta_documento_respuesta As String = ""
    '        Dim Refgembox As New ClassGaGembox
    '        Dim ruta_descarga As String = ""
    '        Dim fil_estrean As Object = Nothing
    '        Erase matris_documentos
    '        Dim Matri_documento_ob() As Object = Nothing
    '        Dim rfclas_gembox As New ClassGaGembox
    '        Result = ""
    '        Dim file_salida As String = ""
    '        If stru_envi.ID_TIPO_DOC_RESPUESTA = 1 Then
    '            Result = Refgembox.Retorna_documento_respuesta_con_Footers(id_registro_respuesta, _
    '                                                                   mtri_documento(1), _
    '                                                                   Ruta_documento_respuesta)
    '            If Result <> "YES" Then
    '                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '                Exit Function
    '            End If

    '            '---------------------------------------------
    '            'Guarda el documento en formato PDF e inserta
    '            'la firma del usuario sin servicio web
    '            '--------------------------------------------
    '            Result = rfclas_gembox.Radica_documento_respuesta_web(Ruta_documento_respuesta, _
    '                                                                  file_firma, _
    '                                                                  opcion_firma_digital, _
    '                                                                  pasword_firma_digital, _
    '                                                                  file_digital_archivo, _
    '                                                                  file_salida)
    '            If Result <> "YES" Then
    '                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '                Exit Function
    '            End If
    '            ReDim Matri_documento_ob(0)
    '            Matri_documento_ob(0) = file_salida
    '            For i As Integer = 0 To Matri_documento_ob.Length - 1
    '                ReDim Preserve matris_documentos(i)
    '                matris_documentos(i) = Matri_documento_ob(i)
    '            Next
    '        Else

    '            For i As Integer = 1 To mtri_documento.Length - 1
    '                ReDim Preserve matris_documentos(i - 1)
    '                matris_documentos(i - 1) = mtri_documento(i)
    '            Next
    '        End If

    '        '--------------------------------------------------------------
    '        'Retorna datos del a estructura del gabinete respuesta
    '        '--------------------------------------------------------------
    '        Dim Refclasalmacenamiento As New ClassAlmacenamiento
    '        Dim estructura_gabinete() As estructura_gabinete = Nothing
    '        Result = Refclasalmacenamiento.SolicitaEstructuraCamposGabinete(nombre_gabinete_respuesta, _
    '                                                                          estructura_gabinete)
    '        If Result <> "YES" Then
    '            Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '            Exit Function
    '        End If
    '        For i As Integer = 0 To estructura_gabinete.Length - 1
    '            estructura_gabinete(i).VALORCAMPO = ""
    '        Next
    '        '---------------------------------------------------------------
    '        'Asigna datos radicado respuesta estructura del gabienete
    '        '--------------------------------------------------------------
    '        For i As Integer = 0 To estructura_gabinete.Length - 1
    '            If estructura_gabinete(i).CAMPO = "NUMERORADICA" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_envi.RADICADO_RESPUESTA
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ENLASE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_envi.RADICADO
    '            End If
    '            If estructura_gabinete(i).CAMPO = "DESTINATARIO" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_envi.DESTINATARIO
    '            End If
    '            If estructura_gabinete(i).CAMPO = "REMITENTE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_envi.USUARIO_RESPONSABLE
    '            End If
    '            If estructura_gabinete(i).CAMPO = "DESCRIPCIONDOCU" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_envi.TRAMITE_DOCUMENTO
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ASUNTO" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_envi.ASUNTO
    '            End If
    '            If estructura_gabinete(i).CAMPO = "TIPORADICADO" Then
    '                estructura_gabinete(i).VALORCAMPO = "SALIENTE"
    '            End If
    '        Next

    '        '-------------------------------------------
    '        'Asigna datos gestion estructura gabinete
    '        '-------------------------------------------
    '        stru_gestion.CLASE_DOCUMENTO = ""
    '        stru_gestion.EXPEDIENTE = ""
    '        stru_gestion.ID_AREA = 0
    '        stru_gestion.ID_CLASE_DOCUMENTO = 0
    '        stru_gestion.ID_EXPEDIENTE = 0
    '        stru_gestion.ID_SERIE = 0
    '        stru_gestion.ID_SUB_SERIE = 0
    '        stru_gestion.ID_TIPO_EXPEDIENTE = 0
    '        stru_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
    '        stru_gestion.ID_TIPODOCUMENTO = 0
    '        stru_gestion.ID_UNIDAD_CONSERVACION = 0
    '        stru_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
    '        stru_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
    '        stru_gestion.UNIDAD_CONSERVACION = ""
    '        stru_gestion.FECHA_ELABORACION = ""
    '        Dim reflclasalalma As New ClassAlmacenamiento
    '        If id_imagen_seleccionada <> 0 Then
    '            Result = reflclasalalma.Solicita_datos_expediente_estructura_base_datos(stru_gestion, _
    '                                                                                  nombre_gabinete_respuesta, _
    '                                                                                  id_imagen_seleccionada)
    '            If Result <> "YES" Then
    '                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '                Exit Function
    '            End If
    '            Result = reflclasalalma.Solicita_datos_gestion_estructura_base_datos(stru_gestion, _
    '                                                                               nombre_gabinete_respuesta, _
    '                                                                               id_imagen_seleccionada)
    '            If Result <> "YES" Then
    '                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '                Exit Function
    '            End If
    '            Result = reflclasalalma.Solicita_datos_tipo_documental_estructura_base_datos(stru_gestion, _
    '                                                                                       nombre_gabinete_respuesta, _
    '                                                                                       id_imagen_seleccionada)
    '            If Result <> "YES" Then
    '                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '                Exit Function
    '            End If
    '            Result = reflclasalalma.Solicita_datos_unidad_conservacion_estructura_base_datos(stru_gestion, _
    '                                                                                           nombre_gabinete_respuesta, _
    '                                                                                           id_imagen_seleccionada)
    '            If Result <> "YES" Then
    '                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '                Exit Function
    '            End If
    '        End If
    '        '------------------------------------------
    '        'Retorna el id tipo documento
    '        '------------------------------------------
    '        Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
    '        Dim id_clase_documento As Integer = 0
    '        Dim Class_ra_tipo_documento As New Class_ra_tipo_documento
    '        Result = Class_ra_tipo_documento.Solicita_id_clase_documento(clase_documento, _
    '                                                                     id_clase_documento)
    '        If Result <> "YES" Then
    '            Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
    '            Exit Function
    '        End If
    '        stru_gestion.CLASE_DOCUMENTO = clase_documento
    '        stru_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
    '        Dim date1al As String = Date.Today
    '        Dim reflclasalmacenamiento As New ClassAlmacenamiento
    '        Result = reflclasalmacenamiento.Formatea_Fecha_Almacenamiento(date1al)
    '        If Result <> "YES" Then
    '            Retorna_parametros_almacenamiento_documento_permanente_almacenado = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
    '            Exit Function
    '        End If
    '        stru_gestion.FECHA_ELABORACION = date1al
    '        '---------------------------------------------------------
    '        'Asigna los datos de gestion a la estructura del gabinete
    '        '---------------------------------------------------------
    '        For i As Integer = 0 To estructura_gabinete.Length - 1
    '            If estructura_gabinete(i).CAMPO = "FECHAELABORACION" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.FECHA_ELABORACION
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_AREA" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_AREA
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_SERIE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_SERIE
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_SUB_SERIE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_SUB_SERIE
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_TIPODOCUMENTO" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_TIPODOCUMENTO
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_USUARIO_GESTION" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_USUARIO_GESTION
    '            End If
    '            If estructura_gabinete(i).CAMPO = "NOMBRESERIE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.NOMBRE_SERIE
    '            End If
    '            If estructura_gabinete(i).CAMPO = "NOMBRESUBSERIE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.NOMBRE_SUB_SERIE
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_EXPEDIENTE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_EXPEDIENTE
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_TIPO_EXPEDIENTE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_EXPEDIENTE
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_UNIDAD_CONSERVACION" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_UNIDAD_CONSERVACION
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_CONSERVACION" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_UNIDAD_CONSERVACION
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_DOCUMENTAL" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_UNIDAD_CONSERVACION
    '            End If
    '            If estructura_gabinete(i).CAMPO = "ID_CLASE_DOCUMENTO" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.ID_CLASE_DOCUMENTO
    '            End If
    '            If estructura_gabinete(i).CAMPO = "EXPEDIENTE" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.EXPEDIENTE
    '            End If
    '            If estructura_gabinete(i).CAMPO = "UNIDADCONSERVA" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.UNIDAD_CONSERVACION
    '            End If
    '            If estructura_gabinete(i).CAMPO = "CLASEDOCUMENTO" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.CLASE_DOCUMENTO
    '            End If
    '            If estructura_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
    '                estructura_gabinete(i).VALORCAMPO = stru_gestion.TIPODOCUMENTO
    '            End If
    '        Next
    '        '----------------------------------------------------------------------------------------
    '        'Asigna datos a la matriz de almacenamiento generica
    '        '----------------------------------------------------------------------------------------
    '        Dim i2 As Integer = 0
    '        For i As Integer = 0 To estructura_gabinete.Length - 1
    '            If estructura_gabinete(i).VISIBLE = 1 Then
    '                ReDim Preserve matris_datos_almacen(i2)
    '                matris_datos_almacen(i2) = estructura_gabinete(i).VALORCAMPO
    '                i2 = i2 + 1
    '            End If
    '        Next
    '        Retorna_parametros_almacenamiento_documento_permanente_almacenado = "YES"
    '    Catch ex As Exception
    '        Retorna_parametros_almacenamiento_documento_permanente_almacenado = "Inconsistencia general función Retorna_parametros_almacenamiento_documento_permanente_almacenado " & ex.Message
    '    End Try
    'End Function
    'Function Guardar_Documento_respuesta_tif_borrador(ByVal nombre_gabinete As String, _
    '                                                  ByVal radicado As String, _
    '                                                  ByRef matri_datos_almacen() As String, _
    '                                                  ByVal matri_gestion As estructure_gestion, _
    '                                                  ByVal matri_documentos() As String, _
    '                                                  ByRef id_imagen As Integer, _
    '                                                  ByVal id_respuesta As Integer) As String
    '    Try

    '        Dim Tipo_Doc_int As Integer = -1
    '        Dim Filein As New FileInfo(matri_documentos(0))
    '        Dim cl As New Classactualizacionvisor
    '        Dim Refalmacena As New ClassAlmacenamiento
    '        Dim Class_da_extension As New Class_da_extension
    '        Dim Result As String = Class_da_extension.Solicita_Tipo_Documento_Extension(UCase(Filein.Extension), _
    '                                                                                    Tipo_Doc_int)
    '        If Result <> "YES" Then
    '            Guardar_Documento_respuesta_tif_borrador = "Imposible determinar el tipo de documento " & Result
    '            Exit Function
    '        End If
    '        id_imagen = Tipo_Doc_int
    '        Result = Refalmacena.Almacenamiento("", "", nombre_gabinete, 0, matri_datos_almacen, _
    '        2, matri_documentos.Length, Tipo_Doc_int, matri_documentos, 0, id_imagen, Tipo_Doc_int, HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE, _
    '        matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE, _
    '        matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION, _
    '        matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE, _
    '        matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION, _
    '        matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado)
    '        If Result <> "YES" Then
    '            Guardar_Documento_respuesta_tif_borrador = "Guardar_Documento_respuesta_tif_borrador  dice " & Result
    '            Exit Function
    '        Else

    '            For i As Integer = 0 To matri_documentos.Length - 1
    '                'Kill(matri_documentos(i))
    '            Next
    '            '----------------------------------------------------------
    '            'Actualiza el estado del codigo del documento docuarchi
    '            '----------------------------------------------------------
    '            Dim SQL As String = "Update ra_respuesta_radicado set ID_IMAGEN_RESPUESTA=" & id_imagen & _
    '            " where ID_RESPUESTA_RADICADO=" & id_respuesta
    '            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
    '            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(SQL)
    '            If Result <> "YES" Then
    '                Guardar_Documento_respuesta_tif_borrador = "Inconsistencia actualizando id documento " & Result
    '                Exit Function
    '            End If
    '            Guardar_Documento_respuesta_tif_borrador = "YES"
    '            Exit Function
    '        End If

    '    Catch ex As Exception
    '        Guardar_Documento_respuesta_tif_borrador = "Inconsistencia función Guardar_Documento_respuesta_tif_borrador " & ex.Message
    '    End Try
    'End Function
    Function Prepara_interface_descarga_plantilla_con_radicado(ByVal id_respuesta As Integer,
                                                               ByRef pag As Page,
                                                               Optional ByVal tipo_interface As Integer = 0) As String
        '*********************************************************
        'Función : Alista interface que permite radicar y dercargar
        'la plantilla con el radicado de respuesta
        'Fecha : 2016-10-04
        'Ing : Miguel Angel Urueta
        '*********************************************************
        Try
            Dim id_plantilla_salida As Integer = 0
            Dim nombre_plantilla_salida As String = ""
            Dim refclasradicado As New ClassRadicador
            Dim Result As String = ""
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla_salida,
                                                                           nombre_plantilla_salida)
            If Result <> "YES" Then
                Prepara_interface_descarga_plantilla_con_radicado = Result
                Exit Function
            End If
            Dim drop_list As DropDownList = pag.FindControl("RE_Descripcion_Documento_ra")
            If drop_list Is Nothing Then
                Prepara_interface_descarga_plantilla_con_radicado = "Imposible encontrar el control DropDownList_tipo_tramite"
                Exit Function
            End If
            Dim UpdatePanel_contenido_radica_documento As UpdatePanel = Nothing
            If tipo_interface = 0 Then
                UpdatePanel_contenido_radica_documento = pag.FindControl("UpdatePanel_descarga_plantilla_radicada")
            Else
                UpdatePanel_contenido_radica_documento = pag.FindControl("UpdatePanel_radic_documento_respuesta")
            End If
            If UpdatePanel_contenido_radica_documento Is Nothing Then
                Prepara_interface_descarga_plantilla_con_radicado = "Imposible encontrar el control UpdatePanel_contenido_radica_documento"
                Exit Function
            End If
            Dim TextBox_destintario As TextBox = pag.FindControl("RE_REMITENTE_COR_REMITENTE_COR_VARCHAR_RA")
            If TextBox_destintario Is Nothing Then
                Prepara_interface_descarga_plantilla_con_radicado = "Imposible encontrar el control TextBox_destintario"
                Exit Function
            End If
            Dim Hidden_remitente_destinatario As HtmlInputHidden = pag.FindControl("Hidden_remitente_destinatario")
            If Hidden_remitente_destinatario Is Nothing Then
                Prepara_interface_descarga_plantilla_con_radicado = "Imposible encontrar el control Hidden_remitente_destinatario"
                Exit Function
            End If
            Dim stru_envio As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                        stru_envio)
            If Result <> "YES" Then
                Prepara_interface_descarga_plantilla_con_radicado = Result
                Exit Function
            End If
            'Lista tipo de tramites
            Result = refclasradicado.Listar_Tipos_Documentales(drop_list,
                                                               id_plantilla_salida,
                                                               stru_envio.TRAMITE_DOCUMENTO)
            If Result <> "YES" Then
                Prepara_interface_descarga_plantilla_con_radicado = Result
                Exit Function
            End If
            Hidden_remitente_destinatario.Value = stru_envio.codigo_dest_externo
            TextBox_destintario.Text = stru_envio.DESTINATARIO
            UpdatePanel_contenido_radica_documento.Update()
            Prepara_interface_descarga_plantilla_con_radicado = "YES"
        Catch ex As Exception
            Prepara_interface_descarga_plantilla_con_radicado = "Inconsistencia general funcion Prepara_interface_descarga_plantilla_con_radicado " & ex.Message
        End Try
    End Function

    Function Retorna_id_imagen_gabinete_resp_radicado(ByVal id_respuesta As Integer, ByRef nombre_gabinete As String, ByRef id_imagen As Integer) As String
        '*************************************************************************
        'Función : Retorna detalles de respuesta, gabinete y la id de la imagen
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT GABINETE,ID_IMAGEN" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_imagen_gabinete_resp_radicado = "Función Retorna_id_imagen_gabinete_resp_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_imagen_gabinete_resp_radicado = "Imposible encontrar detalles de imagen  de la respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    nombre_gabinete = Datset.Tables(0).Rows(0).Item(0)
                Else
                    nombre_gabinete = ""
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    id_imagen = Datset.Tables(0).Rows(0).Item(1)
                Else
                    id_imagen = -1
                End If
                Retorna_id_imagen_gabinete_resp_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_imagen_gabinete_resp_radicado = "Inconsistencia general función Retorna_id_imagen_gabinete_resp_radicado " & ex.Message
        End Try
    End Function

    Function Verifica_Existencia_Tabla(ByVal Table As String) As String
        Try
            Dim Parametro_Consulta As String = "show tables  FROM " & HttpContext.Current.Session.Item("RA_DB_NAME_MODULO") & "  LIKE '" & Table & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Tabla = "Función Verifica_Existencia_Tabla dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_Existencia_Tabla = "Imposible encontrar la tabla " & Table
                Exit Function
            Else
                Verifica_Existencia_Tabla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_Tabla = "Inconsistencia general funcion Verifica_Existencia_Tabla " & ex.Message
        End Try
    End Function
    Function Retorna_plantilla_default_respuesta(ByRef nombre_plantilla As String, ByRef id_plantilla As Integer) As String
        '---------------------------------------------------------------------
        'Funcion : Retorna plantilla default respuesta radicado
        'Fecha : 2015-02-18
        'Ing . Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select id_Plantilla,Nombre_Plantilla_Radicado " &
               " FROM  system_plantilla_radicado  " &
               " where util_default_respuesta=1 " &
               " and Tipo_Plantilla='" & "RADICACION SALIENTE" & "'"
            Dim Datset As DataSet = New DataSet("system_plantilla_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_plantilla_default_respuesta = "Función Retorna_plantilla_default_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_plantilla_default_respuesta = "YES"
                Exit Function
            Else
                id_plantilla = Datset.Tables(0).Rows(0).Item(0)
                nombre_plantilla = Datset.Tables(0).Rows(0).Item(1)
                Retorna_plantilla_default_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_plantilla_default_respuesta = "Inconsistencia función Retorna_plantilla_default_respuesta " & ex.Message
        End Try
    End Function
    Function Retorna_radicados_relacionados_respuesta(ByVal radicado As String, ByRef matri_radicados() As String) As String
        '---------------------------------------------------------------------
        'Funcion : Retorna radicados relacionados de respuesta al radicado
        'Fecha : 2015-02-18
        'Ing . Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select RADICADO_RESPUESTA " &
               " FROM  ra_respuesta_radicado  " &
               " where RADICADO_RESPUESTA  is not null " &
               " and RADICADO='" & radicado & "'"
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_radicados_relacionados_respuesta = "Función Retorna_radicados_relacionados_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_radicados_relacionados_respuesta = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_radicados(i)
                    matri_radicados(i) = Datset.Tables(0).Rows(i).Item(0)
                Next
                Retorna_radicados_relacionados_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_radicados_relacionados_respuesta = "Inconsistencia función Retorna_radicados_relacionados_respuesta " & ex.Message
        End Try
    End Function
    Function Verfica_radicado_con_respuesta(ByVal id_usuario_gestion As Integer,
                                            ByVal radicado As String,
                                            ByRef estado_respuesta As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select * " &
               " FROM  ra_respuesta_radicado AS RRR " &
               " where FECHA_RESPUETA is null and ID_REMIT_DEST_INT=" & id_usuario_gestion &
               " and RADICADO='" & radicado & "'"
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_radicado_con_respuesta = "Función Verfica_radicado_con_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_respuesta = "NO"
                Verfica_radicado_con_respuesta = "YES"
            Else

                estado_respuesta = "YES"
                Verfica_radicado_con_respuesta = "YES"
            End If

        Catch ex As Exception
            Verfica_radicado_con_respuesta = "Inconsistencia general funcion Verfica_radicado_con_respuesta " & ex.Message
        End Try
    End Function
    Function Verfica_respuesta_con_fecha_respuesta(ByVal id_usuario_gestion As Integer,
    ByVal id_respuesta As Integer, ByRef estado_respuesta As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select FECHA_RESPUETA " &
               " FROM  ra_respuesta_radicado AS RRR " &
               " where  ID_REMIT_DEST_INT=" & id_usuario_gestion &
               " and ID_RESPUESTA_RADICADO='" & id_respuesta & "'"
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_respuesta_con_fecha_respuesta = "Función Verfica_radicado_con_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_respuesta = "YES"
                Verfica_respuesta_con_fecha_respuesta = "Imposible encontrar el registro de la respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estado_respuesta = "NO"
                Else
                    estado_respuesta = "YES"
                End If
                Verfica_respuesta_con_fecha_respuesta = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Verfica_respuesta_con_fecha_respuesta = "Inconsistencia general funcion Verfica_radicado_con_respuesta " & ex.Message
        End Try
    End Function
    Function Descarga_archivo_donwload(ByVal file_dow_load As String, ByVal id_respuesta As Integer) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaEnvioCorrespondencia
            If (FileIO.FileSystem.FileExists(file_dow_load)) Then

                '--------------------------------------------
                'Verifica integridad del documento respuesta
                '---------------------------------------------
                'If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                '    Descarga_archivo_donwload = "Por favor active web service para workflow"
                '    Exit Function
                'End If
                'If HttpContext.Current.Session.Item("URL_WEB_SERVICE") = "" Then
                '    Descarga_archivo_donwload = "Por favor informe la url web service para workflow"
                '    Exit Function
                'End If
                'Dim content As Object = Nothing
                'Result = Refclas.ReadFile(file_dow_load, content)
                'If Result <> "YES" Then
                '    Descarga_archivo_donwload = "Imposible leer el archivo " & Result
                '    Exit Function
                'End If
                'Dim OB As New localhost.Service
                'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
                'Result = OB.Verifica_documento_plantilla_web(content, id_respuesta)
                'If Result <> "YES" Then
                '    Descarga_archivo_donwload = Result
                '    Exit Function
                'End If
                Dim ref_gesbox As New ClassGaGembox
                Result = ref_gesbox.Verifica_auntentificacion_doc_respuesta_web(file_dow_load, id_respuesta)
                If Result <> "YES" Then
                    Descarga_archivo_donwload = Result
                    Exit Function
                End If
                Descarga_archivo_donwload = "YES"
                Exit Function
            Else
                Descarga_archivo_donwload = "No existe el archivo en la ruta " & file_dow_load
                Exit Function
            End If
        Catch ex As Exception
            Descarga_archivo_donwload = "Inconsistencia general función Descarga_archivo_donwload " & ex.Message
        End Try
    End Function
    Function Descarga_archivo_donwload_radicado(ByVal file_dow_load As String,
                                                ByVal id_respuesta As Integer) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassRaEnvioCorrespondencia
            If (FileIO.FileSystem.FileExists(file_dow_load)) Then

                '--------------------------------------------
                'Verifica integridad del documento respuesta
                '---------------------------------------------
                'If HttpContext.Current.Session.Item("ACTIVA_WEB_SERVICE") = 0 Then
                '    Descarga_archivo_donwload_radicado = "Por favor active web service para workflow"
                '    Exit Function
                'End If
                'If HttpContext.Current.Session.Item("URL_WEB_SERVICE") = "" Then
                '    Descarga_archivo_donwload_radicado = "Por favor informe la url web service para workflow"
                '    Exit Function
                'End If
                'Dim content As Object = Nothing
                'Result = Refclas.ReadFile(file_dow_load, content)
                'If Result <> "YES" Then
                '    Descarga_archivo_donwload_radicado = "Imposible leer el archivo " & Result
                '    Exit Function
                'End If
                'Dim OB As New localhost.Service
                'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
                'Result = OB.Verifica_documento_plantilla_web_con_radicado(content, id_respuesta)
                'If Result <> "YES" Then
                '    Descarga_archivo_donwload_radicado = Result
                '    Exit Function
                'End If
                Dim ref_gesbox As New ClassGaGembox
                Dim estado_verificacion As String = ""
                Result = ref_gesbox.Verifica_auntentificacion_doc_respuesta_web_radicado(file_dow_load,
                                                                                         id_respuesta,
                                                                                         estado_verificacion)
                If Result <> "YES" Then
                    Descarga_archivo_donwload_radicado = Result
                    Exit Function
                End If
                Descarga_archivo_donwload_radicado = "YES"
                Exit Function
            Else
                Descarga_archivo_donwload_radicado = "No existe el archivo en la ruta " & file_dow_load
                Exit Function
            End If
        Catch ex As Exception
            Descarga_archivo_donwload_radicado = "Inconsistencia general función Descarga_archivo_donwload " & ex.Message
        End Try
    End Function
    Function Subir_anexo_a_la_respuesta(ByVal id_registro_respuesta As Integer,
                                        ByVal archivo_adjuntar As String,
                                        ByVal ruta_temporal As String,
                                        ByRef drow_list As DropDownList,
                                        ByRef up_date As UpdatePanel,
                                        ByRef drop_lis_simple As DropDownList,
                                        ByRef update_simple As UpdatePanel) As String
        Try
            Dim ra_resp_rad As New Class_ra_respuesta_radicado
            Dim ref_class_wf_ruta As New Class_worflow_rutas
            Dim ref_class_config_ruta As New Class_configuracion_listado_ruta
            Dim ref_clas_dat_adit As New Class_DAT_ADIC_TAR
            Dim ref_clas_neo_dinamuyc As New ClassNeodynamic
            Dim ref_clas_anexo_resp As New Class_ra_anexos_respuesta
            Dim stru_envi As stru_envio = Nothing
            Dim stru_gestion As estructure_gestion = Nothing
            Dim mtriz_datos_almacenamiento() As String = Nothing
            Dim nombre_ruta As String = ""
            Dim nombre_campo_radicado As String = ""
            Dim nombre_gabinete_padre As String = ""
            Dim id_imagen_padre As Integer = 0
            Dim Result As String = ""
            Result = ra_resp_rad.Solicita_datos_estructura_envio_por_id_respuesta(id_registro_respuesta,
                                                                                 stru_envi)
            If Result <> "YES" Then
                Subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Result = ref_class_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session.Item("Id_Ruta_Workflow").ToString,
                                                                     nombre_ruta)
            If Result <> "YES" Then
                Subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Result = ref_class_config_ruta.SolicitaNombreCampoRadicadoRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                           nombre_campo_radicado)
            If Result <> "YES" Then
                Subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Result = ref_clas_dat_adit.Solicita_id_imagen_gabinete_seleccionada(stru_envi.RADICADO,
                                                                              HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                              nombre_campo_radicado,
                                                                              nombre_ruta,
                                                                              id_imagen_padre,
                                                                              nombre_gabinete_padre)
            If Result <> "YES" Then
                Subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Result = Me.Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta(id_imagen_padre,
                                                                                         nombre_gabinete_padre,
                                                                                         stru_envi,
                                                                                         "DCOUMENTO ANEXO",
                                                                                         stru_gestion,
                                                                                         mtriz_datos_almacenamiento)
            If Result <> "YES" Then
                Subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Dim Filein As New FileInfo(archivo_adjuntar)
            Dim nombre_archivo As String = Filein.Name
            Dim matriz_documentos_almacenados() As String = Nothing
            '-----------------------------
            'Estrae documento multi tif
            '-----------------------------
            If UCase(Filein.Extension) = ".TIF" Then
                Result = ref_clas_neo_dinamuyc.Extraer_Documento_de_Multitif_fisico(archivo_adjuntar,
                                                                                   matriz_documentos_almacenados,
                                                                                  ruta_temporal)
                If Result <> "YES" Then
                    Subir_anexo_a_la_respuesta = Result
                    Exit Function
                End If
            Else
                ReDim Preserve matriz_documentos_almacenados(0)
                matriz_documentos_almacenados(0) = archivo_adjuntar
            End If
            Dim Tipo_Doc_int As Integer = -1
            Filein = New FileInfo(matriz_documentos_almacenados(0))
            Dim Refalmacena As New ClassAlmacenamiento
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension),
                                                                          Tipo_Doc_int)
            If Result <> "YES" Then
                Subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Dim id_imagen_almacenada As Integer = 0
            Dim radicado As String = ""
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete_padre,
                                                0, mtriz_datos_almacenamiento,
                                                2, matriz_documentos_almacenados.Length,
                                                Tipo_Doc_int,
                                                matriz_documentos_almacenados,
                                                0,
                                                id_imagen_almacenada,
                                                Tipo_Doc_int,
                                                HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                stru_gestion.ID_AREA,
                                                stru_gestion.ID_SERIE,
                                                stru_gestion.ID_SUB_SERIE,
                                                stru_gestion.ID_TIPODOCUMENTO,
                                                stru_gestion.ID_EXPEDIENTE,
                                                stru_gestion.ID_TIPO_EXPEDIENTE,
                                                stru_gestion.ID_UNIDAD_CONSERVACION,
                                                stru_gestion.ID_TIPO_UNIDAD_CONSERVACION,
                                                stru_gestion.ID_CLASE_DOCUMENTO,
                                                stru_gestion.EXPEDIENTE,
                                                stru_gestion.NOMBRE_SERIE,
                                                stru_gestion.NOMBRE_SUB_SERIE,
                                                stru_gestion.TIPODOCUMENTO,
                                                stru_gestion.UNIDAD_CONSERVACION,
                                                stru_gestion.CLASE_DOCUMENTO,
                                                stru_gestion.FECHA_ELABORACION, radicado)
            If Result <> "YES" Then
                Subir_anexo_a_la_respuesta = Result
                Exit Function
            Else

                For i As Integer = 0 To matriz_documentos_almacenados.Length - 1
                    Kill(matriz_documentos_almacenados(i))
                Next
                Dim id_anexo As Object
                Result = ref_clas_anexo_resp.Registra_anexo_respuesta(id_registro_respuesta,
                                                                      id_imagen_almacenada,
                                                                      nombre_gabinete_padre,
                                                                      nombre_archivo,
                                                                      id_anexo)
                If Result <> "YES" Then
                    Subir_anexo_a_la_respuesta = Result
                    Exit Function
                End If
                Dim ilis As New ListItem
                ilis.Text = nombre_archivo
                ilis.Value = id_anexo
                drow_list.Items.Add(ilis)
                drop_lis_simple.Items.Add(ilis)
                up_date.Update()
                update_simple.Update()
                Subir_anexo_a_la_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Subir_anexo_a_la_respuesta = "Inconsistencia general función Subir_anexo_a_la_respuesta " & ex.Message
        End Try
    End Function
    Function upload_subir_anexo_a_la_respuesta(ByVal id_registro_respuesta As Integer,
                                               ByVal archivo_adjuntar As String,
                                               ByVal ruta_temporal As String,
                                               ByRef id_anexo As Object,
                                               ByRef nombre_anexo As String) As String
        Try
            Dim ra_resp_rad As New Class_ra_respuesta_radicado
            Dim ref_class_wf_ruta As New Class_worflow_rutas
            Dim ref_class_config_ruta As New Class_configuracion_listado_ruta
            Dim ref_clas_dat_adit As New Class_DAT_ADIC_TAR
            Dim ref_clas_neo_dinamuyc As New ClassNeodynamic
            Dim ref_clas_anexo_resp As New Class_ra_anexos_respuesta
            Dim stru_envi As stru_envio = Nothing
            Dim stru_gestion As estructure_gestion = Nothing
            Dim mtriz_datos_almacenamiento() As String = Nothing
            Dim nombre_ruta As String = ""
            Dim nombre_campo_radicado As String = ""
            Dim nombre_gabinete_padre As String = ""
            Dim id_imagen_padre As Integer = 0
            Dim Result As String = ""
            Result = ra_resp_rad.Solicita_datos_estructura_envio_por_id_respuesta(id_registro_respuesta,
                                                                                 stru_envi)
            If Result <> "YES" Then
                upload_subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Result = ref_class_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session.Item("Id_Ruta_Workflow").ToString,
                                                                     nombre_ruta)
            If Result <> "YES" Then
                upload_subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Result = ref_class_config_ruta.SolicitaNombreCampoRadicadoRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                           nombre_campo_radicado)
            If Result <> "YES" Then
                upload_subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Result = ref_clas_dat_adit.Solicita_id_imagen_gabinete_seleccionada(stru_envi.RADICADO,
                                                                              HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                              nombre_campo_radicado,
                                                                              nombre_ruta,
                                                                              id_imagen_padre,
                                                                              nombre_gabinete_padre)
            If Result <> "YES" Then
                upload_subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Result = Me.Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta(id_imagen_padre,
                                                                                         nombre_gabinete_padre,
                                                                                         stru_envi,
                                                                                         "DCOUMENTO ANEXO",
                                                                                         stru_gestion,
                                                                                         mtriz_datos_almacenamiento)
            If Result <> "YES" Then
                upload_subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Dim Filein As New FileInfo(archivo_adjuntar)
            Dim nombre_archivo As String = Filein.Name
            Dim matriz_documentos_almacenados() As String = Nothing
            '-----------------------------
            'Estrae documento multi tif
            '-----------------------------
            If UCase(Filein.Extension) = ".TIF" Then
                Result = ref_clas_neo_dinamuyc.Extraer_Documento_de_Multitif_fisico(archivo_adjuntar,
                                                                                   matriz_documentos_almacenados,
                                                                                   ruta_temporal)
                If Result <> "YES" Then
                    upload_subir_anexo_a_la_respuesta = Result
                    Exit Function
                End If
            Else
                ReDim Preserve matriz_documentos_almacenados(0)
                matriz_documentos_almacenados(0) = archivo_adjuntar
            End If
            Dim Tipo_Doc_int As Integer = -1
            Filein = New FileInfo(matriz_documentos_almacenados(0))
            Dim Refalmacena As New ClassAlmacenamiento
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension),
                                                                          Tipo_Doc_int)
            If Result <> "YES" Then
                upload_subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(stru_gestion,
                                                                                       nombre_gabinete_padre,
                                                                                       id_imagen_padre,
                                                                                       HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                       stru_envi.RADICADO,
                                                                                       HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                       HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                       "")
            If Result <> "YES" Then
                upload_subir_anexo_a_la_respuesta = Result
                Exit Function
            End If
            Dim id_imagen_almacenada As Integer = 0
            Dim radicado As String = ""
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete_padre,
                                                0, mtriz_datos_almacenamiento,
                                                2, matriz_documentos_almacenados.Length,
                                                Tipo_Doc_int,
                                                matriz_documentos_almacenados,
                                                0,
                                                id_imagen_almacenada,
                                                Tipo_Doc_int,
                                                HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                stru_gestion.ID_AREA,
                                                stru_gestion.ID_SERIE,
                                                stru_gestion.ID_SUB_SERIE,
                                                stru_gestion.ID_TIPODOCUMENTO,
                                                stru_gestion.ID_EXPEDIENTE,
                                                stru_gestion.ID_TIPO_EXPEDIENTE,
                                                stru_gestion.ID_UNIDAD_CONSERVACION,
                                                stru_gestion.ID_TIPO_UNIDAD_CONSERVACION,
                                                stru_gestion.ID_CLASE_DOCUMENTO,
                                                stru_gestion.EXPEDIENTE,
                                                stru_gestion.NOMBRE_SERIE,
                                                stru_gestion.NOMBRE_SUB_SERIE,
                                                stru_gestion.TIPODOCUMENTO,
                                                stru_gestion.UNIDAD_CONSERVACION,
                                                stru_gestion.CLASE_DOCUMENTO,
                                                stru_gestion.FECHA_ELABORACION, radicado)
            If Result <> "YES" Then
                upload_subir_anexo_a_la_respuesta = Result
                Exit Function
            Else

                For i As Integer = 0 To matriz_documentos_almacenados.Length - 1
                    Kill(matriz_documentos_almacenados(i))
                Next
                Result = ref_clas_anexo_resp.Registra_anexo_respuesta(id_registro_respuesta,
                                                                      id_imagen_almacenada,
                                                                      nombre_gabinete_padre,
                                                                      nombre_archivo,
                                                                      id_anexo)
                If Result <> "YES" Then
                    upload_subir_anexo_a_la_respuesta = Result
                    Exit Function
                End If
                nombre_anexo = nombre_archivo
                upload_subir_anexo_a_la_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            upload_subir_anexo_a_la_respuesta = "Inconsistencia general función upload_subir_anexo_a_la_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta(ByVal id_imagen_padre As Integer,
                                                                              ByVal nombre_gabinete_padre As String,
                                                                              ByVal stru_envi As stru_envio,
                                                                              ByVal tipo_documento As String,
                                                                              ByRef stru_gestion As estructure_gestion,
                                                                              ByRef matri_datos_almacenamiento() As String) As String
        Try
            Dim Refclas As New ClassAlmacenamiento
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim stru_gabinete() As estructura_gabinete = Nothing
            Dim Result As String = ""
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete_padre,
                                                                                 stru_gabinete)
            If Result <> "YES" Then
                Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = Result
                Exit Function
            End If
            '--------------------------------------------------
            'Inicializa campos estructura del gabinete
            '-------------------------------------------------
            For i As Integer = 0 To stru_gabinete.Length - 1
                stru_gabinete(i).VALORCAMPO = ""
            Next
            '---------------------------------------------------------
            'Asigna datos radicado respuesta estructura del gabienete
            '---------------------------------------------------------
            For i As Integer = 0 To stru_gabinete.Length - 1
                If stru_gabinete(i).CAMPO = "NUMERORADICA" Then
                    stru_gabinete(i).VALORCAMPO = stru_envi.RADICADO_RESPUESTA
                End If
                If stru_gabinete(i).CAMPO = "ENLASE" Then
                    stru_gabinete(i).VALORCAMPO = stru_envi.RADICADO
                End If
                If stru_gabinete(i).CAMPO = "DESTINATARIO" Then
                    stru_gabinete(i).VALORCAMPO = stru_envi.DESTINATARIO
                End If
                If stru_gabinete(i).CAMPO = "REMITENTE" Then
                    stru_gabinete(i).VALORCAMPO = stru_envi.USUARIO_RESPONSABLE
                End If
                If stru_gabinete(i).CAMPO = "DESCRIPCIONDOCU" Then
                    stru_gabinete(i).VALORCAMPO = tipo_documento
                End If
                If stru_gabinete(i).CAMPO = "ASUNTO" Then
                    stru_gabinete(i).VALORCAMPO = stru_envi.ASUNTO
                End If
                If stru_gabinete(i).CAMPO = "TIPORADICADO" Then
                    stru_gabinete(i).VALORCAMPO = "SALIENTE"
                End If
            Next
            '-----------------------------------------------------------
            'Asigna datos gestion a la estructura de gestion documental
            '-----------------------------------------------------------
            stru_gestion.CLASE_DOCUMENTO = ""
            stru_gestion.EXPEDIENTE = ""
            stru_gestion.ID_AREA = 0
            stru_gestion.ID_CLASE_DOCUMENTO = 0
            stru_gestion.ID_EXPEDIENTE = 0
            stru_gestion.ID_SERIE = 0
            stru_gestion.ID_SUB_SERIE = 0
            stru_gestion.ID_TIPO_EXPEDIENTE = 0
            stru_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            stru_gestion.ID_TIPODOCUMENTO = 0
            stru_gestion.ID_UNIDAD_CONSERVACION = 0
            stru_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            stru_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            stru_gestion.UNIDAD_CONSERVACION = ""
            stru_gestion.FECHA_ELABORACION = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            If id_imagen_padre <> 0 Then
                Result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(id_imagen_padre,
                                                                                     nombre_gabinete_padre,
                                                                                     stru_gestion)
                If Result <> "YES" Then
                    Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = Result
                    Exit Function
                End If
                Result = Refclas.Solicita_datos_gestion_estructura_base_datos(stru_gestion,
                                                                                   nombre_gabinete_padre,
                                                                                   id_imagen_padre)
                If Result <> "YES" Then
                    Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = Result
                    Exit Function
                End If
                Result = Refclas.Solicita_datos_tipo_documental_estructura_base_datos(stru_gestion,
                                                                                           nombre_gabinete_padre,
                                                                                           id_imagen_padre)
                If Result <> "YES" Then
                    Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = Result
                    Exit Function
                End If
                Result = Refclas.Solicita_datos_unidad_conservacion_estructura_base_datos(stru_gestion,
                                                                                               nombre_gabinete_padre,
                                                                                               id_imagen_padre)
                If Result <> "YES" Then
                    Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = Result
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Retorna el id tipo documento
            '------------------------------------------
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim id_clase_documento As Integer = 0
            Dim Class_ra_tipo_documento As New Class_ra_tipo_documento
            Result = Class_ra_tipo_documento.Solicita_id_clase_documento(clase_documento,
                                                                         id_clase_documento)
            If Result <> "YES" Then
                Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = Result
                Exit Function
            End If
            '----------------------------------------------
            'Asigna tipo documento estructura gestion
            '---------------------------------------------
            stru_gestion.CLASE_DOCUMENTO = clase_documento
            stru_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            '---------------------------------------------------------------------
            'Asigna los datos estructura de gestion a la estructura del gabinete
            '---------------------------------------------------------------------
            stru_gestion.FECHA_ELABORACION = date1al
            For i As Integer = 0 To stru_gabinete.Length - 1
                If stru_gabinete(i).CAMPO = "FECHAELABORACION" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.FECHA_ELABORACION
                End If
                If stru_gabinete(i).CAMPO = "ID_AREA" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_AREA
                End If
                If stru_gabinete(i).CAMPO = "ID_SERIE" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_SERIE
                End If
                If stru_gabinete(i).CAMPO = "ID_SUB_SERIE" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_SUB_SERIE
                End If
                If stru_gabinete(i).CAMPO = "ID_TIPODOCUMENTO" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_TIPODOCUMENTO
                End If
                If stru_gabinete(i).CAMPO = "ID_USUARIO_GESTION" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_USUARIO_GESTION
                End If
                If stru_gabinete(i).CAMPO = "NOMBRESERIE" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.NOMBRE_SERIE
                End If
                If stru_gabinete(i).CAMPO = "NOMBRESUBSERIE" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.NOMBRE_SUB_SERIE
                End If
                If stru_gabinete(i).CAMPO = "ID_EXPEDIENTE" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_EXPEDIENTE
                End If
                If stru_gabinete(i).CAMPO = "ID_TIPO_EXPEDIENTE" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_EXPEDIENTE
                End If
                If stru_gabinete(i).CAMPO = "ID_UNIDAD_CONSERVACION" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_UNIDAD_CONSERVACION
                End If
                If stru_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_CONSERVACION" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_UNIDAD_CONSERVACION
                End If
                If stru_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_DOCUMENTAL" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_UNIDAD_CONSERVACION
                End If
                If stru_gabinete(i).CAMPO = "ID_CLASE_DOCUMENTO" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.ID_CLASE_DOCUMENTO
                End If
                If stru_gabinete(i).CAMPO = "EXPEDIENTE" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.EXPEDIENTE
                End If
                If stru_gabinete(i).CAMPO = "UNIDADCONSERVA" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.UNIDAD_CONSERVACION
                End If
                If stru_gabinete(i).CAMPO = "CLASEDOCUMENTO" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.CLASE_DOCUMENTO
                End If
                If stru_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    stru_gabinete(i).VALORCAMPO = stru_gestion.TIPODOCUMENTO
                End If
            Next
            '----------------------------------------------------------------------------------------
            'Asigna datos a la matriz de almacenamiento generica
            '----------------------------------------------------------------------------------------
            Dim i2 As Integer = 0
            For i As Integer = 0 To stru_gabinete.Length - 1
                If stru_gabinete(i).VISIBLE = 1 Then
                    ReDim Preserve matri_datos_almacenamiento(i2)
                    matri_datos_almacenamiento(i2) = stru_gabinete(i).VALORCAMPO
                    i2 = i2 + 1
                End If
            Next
            Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = Result
            Exit Function
        Catch ex As Exception
            Solicita_parametros_almacenamiento_documento_enexo_rad_respuesta = "Inconsistencia general función Solicita_parametros_almacenamiento_rad_respuesta " & ex.Message
        End Try

    End Function

    Function Confirma_respuesta_al_correo_con_radicado(ByVal id_respuesta_radicado As Integer,
                                                       ByRef pag As Page,
                                                       ByVal direccion_correo_electrnico As String,
                                                       ByVal estado_anexos As Integer) As String
        Try
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Result As String = ""
            Dim matri_anexos() As String = Nothing
            Confirma_respuesta_al_correo_con_radicado = ""
            Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta_radicado & "/"
            If estado_anexos = 1 Then
                Result = Me.Lista_ruta_documentos_anexos_respuesta(HttpContext.Current.Server.MapPath(ruta_virtual), matri_anexos)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_radicado = Result
                    Exit Function
                End If
            End If
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado,
                                                                                id_imagen_plantilla,
                                                                                radicado_respuesta,
                                                                                fecha_respuesta,
                                                                                id_imagen_respuesta,
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            If fecha_respuesta = 0 Then
                Confirma_respuesta_al_correo_con_radicado = "No hay una respuesta confirmada para notificar al correo electrónico"
                Exit Function
            End If
            If direccion_correo_electrnico = "" Then
                Confirma_respuesta_al_correo_con_radicado = "Debe informar el correo electrónico de respuesta "
                Exit Function
            End If
            Dim refclasradicado As New ClassRadicador
            Dim nombre_remitente As String = ""
            Dim cargo_remit As String = ""
            Dim ref_class_remit_interno As New Class_remit_dest_interno
            Result = ref_class_remit_interno.Retorna_nombre_cargo_destinatario_interno(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       nombre_remitente,
                                                                                       cargo_remit)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            '*********************************************************************************
            'Retorna id usuario destino radicacion
            '*********************************************************************************
            Dim codigo_destinatario As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            '*********************************************************************************
            'Retorna nombre y id area destinatario
            '*********************************************************************************
            Dim nombre_area As String = ""
            Dim id_area As Integer = -1
            Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
            Result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(codigo_destinatario,
                                                                                        id_area,
                                                                                        nombre_area)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim Refclas_respuesta As New Classgestionrespuesta
            Dim id_imagen As Integer = 0
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado, estru)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim correo_usuario_gestion As String = ""
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                             correo_usuario_gestion)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim gabinete_imagen As String = ""
            Dim refclas_gestion As New Classgestionrespuesta
            If estru.ID_IMAGEN <> 0 Then
                Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(id_respuesta_radicado,
                                                                                  gabinete_imagen,
                                                                                  id_imagen)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_radicado = Result
                    Exit Function
                End If
            End If
            Dim refclas_visualiza As New ClassVisualisaDocumento
            Dim matri_documento() As String = Nothing
            If id_imagen <> 0 Then
                Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(id_imagen,
                                                                                "IMP03GESTIONTMP",
                                                                                matri_documento)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_radicado = Result & ", El sistema intento buscar un documento de respuesta, pero el tipo de respuesta es de sólo confirmación,  por ende carece de un documento respuesta por tal motivo el sistema notifico y envió el correo electrónico "
                    'Exit Function
                End If
            End If
            Dim matri_resp() As String = Nothing
            Dim z As Integer = 0
            If Not matri_documento Is Nothing Then
                For i As Integer = 1 To matri_documento.Length - 1
                    ReDim Preserve matri_resp(z)
                    matri_resp(z) = matri_documento(i)
                Next
            End If
            Result = Me.Genera_zip_documento_anexo(id_respuesta_radicado,
                                                   matri_anexos)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim Refclas_correo As New ClassCorreo
            Dim matri_mensaje() As String = {""}
            If estru.NOTA_RESPUESTA <> "" Then
                Erase matri_mensaje
                matri_mensaje = estru.NOTA_RESPUESTA.ToString.Split(vbCrLf)
            End If
            Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
            Result = Refclas_correo.Envio_Correo_respuesta_documento(radicado_respuesta,
                                                                     matri_mensaje,
                                                                     direccion_correo_electrnico,
                                                                     matri_resp, nombre_remitente,
                                                                     cargo_remit,
                                                                     nombre_area,
                                                                     correo_usuario_gestion,
                                                                     estru,
                                                                     ruta_local,
                                                                     matri_anexos)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If

            Confirma_respuesta_al_correo_con_radicado = Confirma_respuesta_al_correo_con_radicado & "YES"
        Catch ex As Exception
            Confirma_respuesta_al_correo_con_radicado = "Inconsistencia general función Confirma_respuesta_al_correo_con_radicado " & ex.Message
        End Try
    End Function
    Function Confirma_respuesta_al_correo_con_radicado(ByVal id_respuesta_radicado As Integer,
                                                       ByVal direccion_correo_electrnico As String,
                                                       ByVal estado_anexos As Integer,
                                                       ByRef url_image As String,
                                                       ByRef url_image_electronica As String) As String
        Try
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Result As String = ""
            Dim matri_anexos() As String = Nothing
            Confirma_respuesta_al_correo_con_radicado = ""
            Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta_radicado & "/"
            If estado_anexos = 1 Then
                Result = Me.Lista_ruta_documentos_anexos_respuesta(HttpContext.Current.Server.MapPath(ruta_virtual),
                                                                   matri_anexos)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_radicado = Result
                    Exit Function
                End If
            End If
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado,
                                                                                id_imagen_plantilla,
                                                                                radicado_respuesta,
                                                                                fecha_respuesta,
                                                                                id_imagen_respuesta,
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            If fecha_respuesta = 0 Then
                Confirma_respuesta_al_correo_con_radicado = "No hay una respuesta confirmada para notificar al correo electrónico"
                Exit Function
            End If
            If direccion_correo_electrnico = "" Then
                Confirma_respuesta_al_correo_con_radicado = "Debe informar el correo electrónico de respuesta "
                Exit Function
            End If
            Dim refclasradicado As New ClassRadicador
            Dim nombre_remitente As String = ""
            Dim cargo_remit As String = ""
            Dim ref_class_remit_interno As New Class_remit_dest_interno
            Result = ref_class_remit_interno.Retorna_nombre_cargo_destinatario_interno(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       nombre_remitente,
                                                                                       cargo_remit)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            '*********************************************************************************
            'Retorna id usuario destino radicacion
            '*********************************************************************************
            Dim codigo_destinatario As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            '*********************************************************************************
            'Retorna nombre y id area destinatario
            '*********************************************************************************
            Dim nombre_area As String = ""
            Dim id_area As Integer = -1
            Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
            Result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(codigo_destinatario,
                                                                                        id_area,
                                                                                        nombre_area)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim Refclas_respuesta As New Classgestionrespuesta
            Dim id_imagen As Integer = 0
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                        estru)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim correo_usuario_gestion As String = ""
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                             correo_usuario_gestion)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim gabinete_imagen As String = ""
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.Solicita_nombre_gabinete_tramite(estru.TRAMITE_DOCUMENTO,
                                                                              estru.system_plantilla_radicado_id_plantilla,
                                                                              gabinete_imagen)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim matri_resp() As String = Nothing
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Route_File As String = ""
            Result = ClassDaGabinete.Solicita_ruta_documento_gabinete(estru.ID_IMAGEN_RESPUESTA,
                                                                      gabinete_imagen,
                                                                      Route_File)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            ReDim Preserve matri_resp(0)
            matri_resp(0) = Route_File
            Result = Me.Genera_zip_documento_anexo(id_respuesta_radicado,
                                                   matri_anexos)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            Dim Refclas_correo As New ClassCorreo
            Dim matri_mensaje() As String = {""}
            If estru.NOTA_RESPUESTA <> "" Then
                Erase matri_mensaje
                matri_mensaje = estru.NOTA_RESPUESTA.ToString.Split(vbCrLf)
            End If
            Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
            Result = Refclas_correo.Envio_Correo_respuesta_documento(radicado_respuesta,
                                                                     matri_mensaje,
                                                                     direccion_correo_electrnico,
                                                                     matri_resp,
                                                                     nombre_remitente,
                                                                     cargo_remit,
                                                                     nombre_area,
                                                                     correo_usuario_gestion,
                                                                     estru,
                                                                     ruta_local,
                                                                     matri_anexos)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_radicado = Result
                Exit Function
            End If
            If estru.ESTADO_ENVIO = 0 Then
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta(id_respuesta_radicado,
                                                                               url_image)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_radicado = Result
                    Exit Function
                End If
            Else
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta_electronica(id_respuesta_radicado,
                                                                                           url_image)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_radicado = Result
                    Exit Function
                End If
            End If
            If estru.ESTADO_ENVIO = 0 Then
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion(id_respuesta_radicado,
                                                                                  url_image_electronica)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_radicado = Result
                    Exit Function

                End If
            Else
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(id_respuesta_radicado,
                                                                                             url_image_electronica)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_radicado = Result
                    Exit Function

                End If
            End If
            Confirma_respuesta_al_correo_con_radicado = Confirma_respuesta_al_correo_con_radicado & "YES"
        Catch ex As Exception
            Confirma_respuesta_al_correo_con_radicado = "Inconsistencia general función Confirma_respuesta_al_correo_con_radicado " & ex.Message
        End Try
    End Function
    Function Genera_zip_documento_anexo(ByVal id_respuesta_radicado As Integer,
                                        ByRef matri_anexos() As String) As String
        Try
            Dim stru_anexo() As stru_anexos = Nothing
            Dim class_ra_anexos As New Class_ra_anexos_respuesta
            Dim Result As String = class_ra_anexos.Solicita_lista_anexos_respuesta(id_respuesta_radicado,
                                                                                   stru_anexo)
            If Result <> "YES" Then
                Genera_zip_documento_anexo = "Se completo la respuesta, pero no se pudo notificar al correo del peticionario. Código error del sistema " & Result
                Exit Function
            End If
            Dim Ref_class_visualiza As New ClassVisualisaDocumento
            If Not stru_anexo Is Nothing Then
                For i As Integer = 0 To stru_anexo.Length - 1
                    Dim matri_anexo_tempo() As String = Nothing
                    Result = Ref_class_visualiza.Genera_Matris_Documentos_Almacenados(stru_anexo(i).id_imagen_gabinete,
                                                                                      stru_anexo(i).nombre_gabinete,
                                                                                      matri_anexo_tempo)
                    If Result <> "YES" Then
                        Genera_zip_documento_anexo = "Se completo la respuesta, pero no se pudo notificar al correo del peticionario. Código error del sistema " & Result
                        Exit Function
                    End If
                    ReDim Preserve matri_anexos(i)
                    matri_anexos(i) = matri_anexo_tempo(1)

                Next
                Using zip As New ZipFile()
                    For i As Integer = 0 To matri_anexos.Length - 1
                        If i = 0 Then
                            zip.AddFile(matri_anexos(i), "Filesanexo")
                        Else
                            zip.AddFile(matri_anexos(i), "Filesanexo")
                        End If
                    Next
                    Dim zipName As String = [String].Format("Zip_{0}.zip", HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "_" & id_respuesta_radicado)
                    If File.Exists(HttpContext.Current.Session.Item("GA_RUTA_TEMPO_DESCARGA") & "\" & zipName) Then
                        Kill(HttpContext.Current.Session.Item("GA_RUTA_TEMPO_DESCARGA") & "\" & zipName)
                    End If
                    zip.Save(HttpContext.Current.Session.Item("GA_RUTA_TEMPO_DESCARGA") & "\" & zipName)
                    ReDim Preserve matri_anexos(0)
                    matri_anexos(0) = HttpContext.Current.Session.Item("GA_RUTA_TEMPO_DESCARGA") & "\" & zipName
                End Using
                Genera_zip_documento_anexo = "YES"
                Exit Function
            Else
                Genera_zip_documento_anexo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Genera_zip_documento_anexo = "Inconsistencia general función Genera_zip_documento_anexo " & ex.Message
        End Try
    End Function
    Function Confirma_respuesta_al_correo_con_sin_radicado(ByVal id_respuesta_radicado As Integer,
                                                           ByVal direccion_correo_electrnico As String,
                                                           ByVal estado_anexo As Integer,
                                                           ByRef url_image As String,
                                                           ByRef url_image_electronica As String) As String
        Try
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Result As String = ""
            Dim matri_anexos() As String = Nothing
            Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta_radicado & "/"
            If estado_anexo = 1 Then
                If Directory.Exists(HttpContext.Current.Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(ruta_virtual))
                End If
                Result = Me.Lista_ruta_documentos_anexos_respuesta(HttpContext.Current.Server.MapPath(ruta_virtual), matri_anexos)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
            End If
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado,
                                                                                id_imagen_plantilla,
                                                                                radicado_respuesta,
                                                                                fecha_respuesta,
                                                                                id_imagen_respuesta,
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_sin_radicado = Result
                Exit Function
            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            If fecha_respuesta <> 0 Then
                If direccion_correo_electrnico = "" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = "Debe informar el correo electrónico de respuesta "
                    Exit Function
                End If
                Dim refclasradicado As New ClassRadicador
                Dim nombre_remitente As String = ""
                Dim cargo_remit As String = ""
                Dim ref_class_remit_interno As New Class_remit_dest_interno
                Result = ref_class_remit_interno.Retorna_nombre_cargo_destinatario_interno(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                           nombre_remitente,
                                                                                           cargo_remit)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                '*********************************************************************************
                'Retorna id usuario destino radicacion
                '*********************************************************************************
                Dim codigo_destinatario As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
                '*********************************************************************************
                'Retorna nombre y id area destinatario
                '*********************************************************************************
                Dim nombre_area As String = ""
                Dim id_area As Integer = -1
                Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
                Result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(codigo_destinatario,
                                                                             id_area,
                                                                             nombre_area)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim Refclas_respuesta As New Classgestionrespuesta
                Dim id_imagen As Integer = 0
                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                            estru)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim correo_usuario_gestion As String = ""
                Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
                Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                  correo_usuario_gestion)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim gabinete_imagen As String = ""
                Dim refclas_gestion As New Classgestionrespuesta
                Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(id_respuesta_radicado,
                                                                                  gabinete_imagen,
                                                                                  id_imagen)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim Refclasvisual As New ClassVisualisaDocumento
                Dim Matri_documentos() As String
                Erase Matri_documentos
                Dim matri_resp() As String = Nothing
                Dim Refclas_correo As New ClassCorreo
                Dim matri_mensaje() As String = {""}
                If estru.NOTA_RESPUESTA <> "" Then
                    Erase matri_mensaje
                    matri_mensaje = estru.NOTA_RESPUESTA.ToString.Split(vbCrLf)
                End If
                Result = Me.Genera_zip_documento_anexo(id_respuesta_radicado,
                                                     matri_anexos)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
                Result = Refclas_correo.Envio_Correo_confirmacion_recibido_solicitud(estru.RADICADO,
                                                                                     matri_mensaje,
                                                                                     direccion_correo_electrnico,
                                                                                     matri_resp,
                                                                                     nombre_remitente,
                                                                                     cargo_remit,
                                                                                     nombre_area,
                                                                                     correo_usuario_gestion,
                                                                                     estru,
                                                                                     ruta_local,
                                                                                     matri_anexos,
                                                                                     "")
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
            Else
                Confirma_respuesta_al_correo_con_sin_radicado = "No hay una respuesta confirmada para notificar al correo electrónico"
                Exit Function
            End If
            If estru.ESTADO_ENVIO = 0 Then
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta(id_respuesta_radicado,
                                                                               url_image)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
            Else
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta_electronica(id_respuesta_radicado,
                                                                                           url_image)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
            End If
            If estru.ESTADO_ENVIO = 0 Then
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion(id_respuesta_radicado,
                                                                                  url_image_electronica)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function

                End If
            Else
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(id_respuesta_radicado,
                                                                                             url_image_electronica)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function

                End If
            End If
            Confirma_respuesta_al_correo_con_sin_radicado = "YES"
        Catch ex As Exception
            Confirma_respuesta_al_correo_con_sin_radicado = "Inconsistencia general función Confirma_respuesta_al_correo_con_sin_radicado " & ex.Message
        End Try
    End Function
    Function Confirma_respuesta_al_correo_con_sin_radicado(ByVal id_respuesta_radicado As Integer,
                                                           ByVal Page As Page,
                                                           ByVal direccion_correo_electrnico As String,
                                                           ByVal estado_anexo As Integer) As String
        Try
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Result As String = ""
            Dim matri_anexos() As String = Nothing
            Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta_radicado & "/"
            If estado_anexo = 1 Then
                If Directory.Exists(HttpContext.Current.Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(ruta_virtual))
                End If
                Result = Me.Lista_ruta_documentos_anexos_respuesta(HttpContext.Current.Server.MapPath(ruta_virtual), matri_anexos)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
            End If
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado,
                                                                                id_imagen_plantilla,
                                                                                radicado_respuesta,
                                                                                fecha_respuesta,
                                                                                id_imagen_respuesta,
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                Confirma_respuesta_al_correo_con_sin_radicado = Result
                Exit Function
            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            If fecha_respuesta <> 0 Then
                If direccion_correo_electrnico = "" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = "Debe informar el correo electrónico de respuesta "
                    Exit Function
                End If
                Dim refclasradicado As New ClassRadicador
                Dim nombre_remitente As String = ""
                Dim cargo_remit As String = ""
                Dim ref_class_remit_interno As New Class_remit_dest_interno
                Result = ref_class_remit_interno.Retorna_nombre_cargo_destinatario_interno(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                           nombre_remitente,
                                                                                           cargo_remit)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                '*********************************************************************************
                'Retorna id usuario destino radicacion
                '*********************************************************************************
                Dim codigo_destinatario As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
                '*********************************************************************************
                'Retorna nombre y id area destinatario
                '*********************************************************************************
                Dim nombre_area As String = ""
                Dim id_area As Integer = -1
                Dim Ref_class_remit_dest_int As New Class_remit_dest_interno
                Result = Ref_class_remit_dest_int.Solicita_id_area_nombre_area_destinatario(codigo_destinatario,
                                                                             id_area,
                                                                             nombre_area)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim Refclas_respuesta As New Classgestionrespuesta
                Dim id_imagen As Integer = 0

                Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado, estru)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim correo_usuario_gestion As String = ""
                Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
                Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                  correo_usuario_gestion)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim gabinete_imagen As String = ""
                Dim refclas_gestion As New Classgestionrespuesta
                Result = refclas_gestion.Retorna_id_imagen_gabinete_resp_radicado(id_respuesta_radicado,
                                                                                  gabinete_imagen,
                                                                                  id_imagen)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim Refclasvisual As New ClassVisualisaDocumento
                Dim Matri_documentos() As String
                Erase Matri_documentos
                Dim matri_resp() As String = Nothing
                Dim Refclas_correo As New ClassCorreo
                Dim matri_mensaje() As String = {""}
                If estru.NOTA_RESPUESTA <> "" Then
                    Erase matri_mensaje
                    matri_mensaje = estru.NOTA_RESPUESTA.ToString.Split(vbCrLf)
                End If
                Result = Me.Genera_zip_documento_anexo(id_respuesta_radicado,
                                                     matri_anexos)
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
                Dim ruta_local As String = HttpContext.Current.Server.MapPath("../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/")
                Result = Refclas_correo.Envio_Correo_confirmacion_recibido_solicitud(estru.RADICADO,
                                                                                     matri_mensaje,
                                                                                     direccion_correo_electrnico,
                                                                                     matri_resp,
                                                                                     nombre_remitente,
                                                                                     cargo_remit,
                                                                                     nombre_area,
                                                                                     correo_usuario_gestion,
                                                                                     estru,
                                                                                     ruta_local,
                                                                                     matri_anexos,
                                                                                     "")
                If Result <> "YES" Then
                    Confirma_respuesta_al_correo_con_sin_radicado = Result
                    Exit Function
                End If
            Else
                Confirma_respuesta_al_correo_con_sin_radicado = "No hay una respuesta confirmada para notificar al correo electrónico"
                Exit Function
            End If
            Confirma_respuesta_al_correo_con_sin_radicado = "YES"
        Catch ex As Exception
            Confirma_respuesta_al_correo_con_sin_radicado = "Inconsistencia general función Confirma_respuesta_al_correo_con_sin_radicado " & ex.Message
        End Try
    End Function
    Function Confirma_recibido_de_la_solicitud(ByVal id_respuesta_radicado As Integer,
                                               ByVal estado_envia_correo_electronico As Integer,
                                               ByVal nota_confirma As String,
                                               ByVal correo_electronico_envio As String,
                                               ByVal tipo_respuesta As String,
                                               ByRef url_image As String,
                                               ByRef url_image_electronica As String,
                                               ByRef resultado_envio_correo As String,
                                               ByVal NotaRespuesta As String) As String
        Dim Ref_clas_remit_dest As New Class_remit_dest_interno
        Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
        Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
        Dim Result As String = ""
        Dim estado_obligatorio_respuesta As Integer = 0
        Dim id_tipo_tramite As Integer = 0
        Dim descripcion_tramite As String = ""
        resultado_envio_correo = "YES"
        Dim valor_nota As String = "Null"
        valor_nota = "'" & Trim(nota_confirma) & "'"
        '*********************************************************************************
        'Retorna id usuario destino radicacion
        '*********************************************************************************
        Dim id_usuario_gestion As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        '*********************************************************************************
        'Retorna nombre y id area destinatario
        '*********************************************************************************

        Dim matri_anexos() As String = Nothing
        Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta_radicado & "/"
        If estado_envia_correo_electronico = 1 Then
            If correo_electronico_envio = "" Then
                Confirma_recibido_de_la_solicitud = "Debe informar el correo electrónico para notificación "
                Exit Function
            End If
        End If
        Dim stru_envio As stru_envio = Nothing
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                    stru_envio)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        If stru_envio.FECHA_RESPUETA <> "" Then
            Confirma_recibido_de_la_solicitud = "La solicitud tiene una respuesta, el sistema no permite volver a confirmar"
            Exit Function
        End If
        If stru_envio.RADICADO_RESPUESTA <> "" Then
            Confirma_recibido_de_la_solicitud = "Usted inicio un proceso de respuesta formal " &
                ", en el menú (Opciones) de esta ventana seleccione la opción (Reversar gestion tramite) e intente confirmar nuevamente "
            Exit Function
        End If
        Dim nombre_plantilla_radicado As String = ""
        Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
        Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(stru_envio.system_plantilla_radicado_id_plantilla,
                                                                             nombre_plantilla_radicado)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla_radicado,
                                                                                             stru_envio.RADICADO,
                                                                                             id_tipo_tramite,
                                                                                             descripcion_tramite)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                         estado_obligatorio_respuesta)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        If estado_obligatorio_respuesta = 1 Then
            Confirma_recibido_de_la_solicitud = "El trámite requiere de un radicado de respuesta, el sistema no permite sólo confirmar"
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = ""
        Result = ""
        Result = refclas_gestion_fechas.Formatea_fecha_time_db(Date.Now,
                                                               date1al)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim datehora As String = Date.Now.Hour
        Dim stiempo_dias As Object = Nothing
        Dim minuno_respuesta As Object = Nothing
        Dim hora_respuesta As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Dim refclas_gestion_fecha As New ClassGestionFechas
        Result = refclas_gestion_fecha.Solicita_tiempo_respuesta_tramite(stru_envio.FECHA_REGISTRO,
                                                                         stiempo_dias,
                                                                         hora_respuesta,
                                                                         minuno_respuesta,
                                                                         dias_calendario,
                                                                         dias_no_habiles)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        Dim Class_ra_registro_pqr As New Class_ra_registro_pqr
        Dim Correo_usuario_pqr As String = ""
        Result = Class_ra_registro_pqr.Solicita_correo_electronico_usuario_pqr(stru_envio.RADICADO,
                                                                               Correo_usuario_pqr)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        '---------------------------------------------------------------------
        'Actualiza estado respuesta radicado 
        '--------------------------------------------------------------------- 
        Dim sqlupdate_actualiza_estado_respuesta As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & 5 & ", ID_RUTA_WF=" &
           HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",ID_TAREA_WF=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & ",FECHA_RESPUETA='" & date1al & "', HORA_RESPUESTA='" & datehora & "', " &
           "TIEMPO_RESPUESTA=" & Val(stiempo_dias) &
           ",id_usuario_gestion_propietario=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
           ",TIPO_RESPUESTA_ELAB_USUARIO=" & "2" &
           ",NOTA_RESPUESTA=" & valor_nota &
           ",TIPO_RESPUESTA='" & tipo_respuesta & "'" &
           "  where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado
        Dim hor As String = Now
        Dim detalle_trans As String = "RESPUESTA RADICADO"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "RESPUESTA DE LA SOLICITUD INTERNA NUMERO (" & stru_envio.ID_RESPUESTA_RADICADO &
        ") DEL RADICADO " & stru_envio.RADICADO & " SOLO SE CONFIRMA  Al peticionario " & stru_envio.DESTINATARIO &
        " el día " & date1al & " a las " & datehora
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     stru_envio.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WEB-WORKFLOW','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                            isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try

            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlupdate_actualiza_estado_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Confirma_recibido_de_la_solicitud = "Imposible actualizar el estado de la respuesta  "

                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Confirma_recibido_de_la_solicitud = "Imposible actualizar el log de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------------------------------------------------------------------
            'Actualiza el estado del tramite en workflow
            '-------------------------------------------------------------------
            Dim Refclas_adic As New Class_DAT_ADIC_TAR
            Result = Refclas_adic.Actualiza_estado_tramite_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                           Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                           "Tramitado")
            If Result <> "YES" Then
                Confirma_recibido_de_la_solicitud = "Imposible actualizar el estado del tramite en workflow  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If Not HttpContext.Current.Session.Item("dat_gred_cahce") Is Nothing Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item(0) = Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")) Then
                        HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item("ESTADO") = "Tramitado"
                        HttpContext.Current.Session.Item("dat_gred_cahce").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item(0) = Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")) Then
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item("ESTADO") = "Tramitado"
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            myTrans.Commit()
            myConnection.Close()
            If Correo_usuario_pqr <> "" Then
                If correo_electronico_envio <> "" Then
                    correo_electronico_envio = correo_electronico_envio & "," & correo_electronico_envio
                Else
                    correo_electronico_envio = Correo_usuario_pqr
                End If
            End If
            If estado_envia_correo_electronico = 1 Then
                Dim matri_resp() As String = Nothing
                Dim z As Integer = 0
                Dim Refclas_correo As New ClassCorreo
                Dim matri_mensaje() As String = {""}
                If stru_envio.NOTA_RESPUESTA <> "" Then
                    Erase matri_mensaje
                    matri_mensaje = stru_envio.NOTA_RESPUESTA.ToString.Split(vbCrLf)
                End If
                Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
                Dim nombre_area As String = ""
                Dim cargo_usuario_gestion_responde As String = ""
                Dim nombre_usuario_gestion_responde As String = ""
                Dim correo_usuario_gestion_responde As String = ""
                Result = Ref_clas_remit_dest.Retorna_datos_caracterizacion_usuario_gestion(id_usuario_gestion,
                                                                                           nombre_usuario_gestion_responde,
                                                                                           cargo_usuario_gestion_responde,
                                                                                           correo_usuario_gestion_responde)
                If Result <> "YES" Then
                    resultado_envio_correo = "Se confirmo la solicitud, pero no se pudo notificar al correo electrónico " & Result

                End If
                Result = Reclas_remit_dest_interno.Solicita_id_area_nombre_area_destinatario(id_usuario_gestion,
                                                                                           0,
                                                                                           nombre_area)
                If Result <> "YES" Then
                    resultado_envio_correo = "Se confirmo la solicitud, pero no se pudo notificar al correo electrónico " & Result

                End If
                Result = Me.Genera_zip_documento_anexo(id_respuesta_radicado,
                                                        matri_anexos)
                If Result <> "YES" Then
                    resultado_envio_correo = "Se confirmo la solicitud, pero no se pudo notificar al correo electrónico " & Result

                End If
                resultado_envio_correo = Refclas_correo.Envio_Correo_confirmacion_recibido_solicitud(stru_envio.RADICADO,
                                                                                     matri_mensaje,
                                                                                     correo_electronico_envio,
                                                                                     matri_resp,
                                                                                     nombre_usuario_gestion_responde,
                                                                                     cargo_usuario_gestion_responde,
                                                                                     nombre_area,
                                                                                     correo_usuario_gestion_responde,
                                                                                     stru_envio,
                                                                                     "",
                                                                                     matri_anexos,
                                                                                     NotaRespuesta)


                If resultado_envio_correo = "YES" Then
                    Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                    Dim update As String = "Update ra_respuesta_radicado set estado_envio_correo=1 where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado
                    Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(update)
                    If Result <> "YES" Then
                        Confirma_recibido_de_la_solicitud = "Se envió el correo electrónico con la confirmación, sin embargo no se cambio el estado de envio en la base de datos " & Result
                        Exit Function
                    End If
                End If
            End If
            If estado_envia_correo_electronico = 0 Then
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta(id_respuesta_radicado,
                                                                               url_image)
                If Result <> "YES" Then
                    Confirma_recibido_de_la_solicitud = "Se completo la notificación, pero no se pudo determinar el estado " & Result
                    Exit Function
                End If
            Else
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta_electronica(id_respuesta_radicado,
                                                                                           url_image)
                If Result <> "YES" Then
                    Confirma_recibido_de_la_solicitud = "Se completo la notificación, pero no se pudo determinar el estado " & Result
                    Exit Function
                End If
            End If
            If estado_envia_correo_electronico = 0 Then
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion(id_respuesta_radicado,
                                                                                 url_image_electronica)
                If Result <> "YES" Then
                    Confirma_recibido_de_la_solicitud = "Se completo la notificación, pero no se pudo determinar el estado " & Result
                    Exit Function

                End If
            Else
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(id_respuesta_radicado,
                                                                                                 url_image_electronica)
                If Result <> "YES" Then
                    Confirma_recibido_de_la_solicitud = "Se completo la notificación, pero no se pudo determinar el estado " & Result
                    Exit Function

                End If
            End If
            Confirma_recibido_de_la_solicitud = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
                Confirma_recibido_de_la_solicitud = "Error General función  Confirma_recibido_de_la_solicitud " & e.Message
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Confirma_recibido_de_la_solicitud = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Confirma_recibido_de_la_solicitud = "Error General función  Confirma_recibido_de_la_solicitud " & e.Message
            Exit Function
        End Try
    End Function
    Function Confirma_recibido_de_la_solicitud(ByVal id_respuesta As Integer,
                                               ByVal pag As Page) As String
        Dim Ref_clas_remit_dest As New Class_remit_dest_interno
        Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
        Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
        Dim Result As String = ""
        Dim estado_obligatorio_respuesta As Integer = 0
        Dim id_tipo_tramite As Integer = 0
        Dim descripcion_tramite As String = ""
        Confirma_recibido_de_la_solicitud = ""
        Dim CheckBox_envio_correo_solo_confirmar As CheckBox = pag.FindControl("CheckBox_envio_correo_solo_confirmar")
        If CheckBox_envio_correo_solo_confirmar Is Nothing Then
            Confirma_recibido_de_la_solicitud = "Imposible encontrar el control CheckBox_envio_correo_solo_confirmar "
            Exit Function
        End If
        Dim TextBox_correo_solo_confirmar As Object = pag.FindControl("Hidden_text_user_correo")
        If TextBox_correo_solo_confirmar Is Nothing Then
            Confirma_recibido_de_la_solicitud = "Imposible encontrar el control TextBox_correo_solo_confirmar "
            Exit Function
        End If
        Dim ref_TextBox_nota_confirma As TextBox = pag.FindControl("TextBox_nota_confirma")
        Dim valor_nota As String = "Null"
        If Not ref_TextBox_nota_confirma Is Nothing Then
            valor_nota = "'" & Trim(ref_TextBox_nota_confirma.Text) & "'"
        End If
        '*********************************************************************************
        'Retorna id usuario destino radicacion
        '*********************************************************************************
        Dim id_usuario_gestion As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        '*********************************************************************************
        'Retorna nombre y id area destinatario
        '*********************************************************************************

        Dim matri_anexos() As String = Nothing
        Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta & "/"
        If CheckBox_envio_correo_solo_confirmar.Checked = True Then
            If TextBox_correo_solo_confirmar.value = "" Then
                Confirma_recibido_de_la_solicitud = "Debe informar el correo electrónico para notificación "
                Exit Function
            End If

        End If
        Dim stru_envio As stru_envio = Nothing
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                    stru_envio)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        If stru_envio.FECHA_RESPUETA <> "" Then
            Confirma_recibido_de_la_solicitud = "La solicitud tiene una respuesta, el sistema no permite volver a confirmar"
            Exit Function
        End If
        If stru_envio.RADICADO_RESPUESTA <> "" Then
            Confirma_recibido_de_la_solicitud = "Usted inicio un proceso de respuesta formal " &
                ", en el menú (Opciones) de esta ventana seleccione la opción (Reversar gestion tramite) e intente confirmar nuevamente "
            Exit Function
        End If
        Dim nombre_plantilla_radicado As String = ""
        Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
        Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(stru_envio.system_plantilla_radicado_id_plantilla,
                                                                             nombre_plantilla_radicado)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If

        Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla_radicado,
                                                                                             stru_envio.RADICADO,
                                                                                             id_tipo_tramite,
                                                                                             descripcion_tramite)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                         estado_obligatorio_respuesta)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        If estado_obligatorio_respuesta = 1 Then
            Confirma_recibido_de_la_solicitud = "El trámite requiere de un radicado de respuesta, el sistema no permite sólo confirmar"
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = ""
        Result = ""
        Result = refclas_gestion_fechas.Formatea_fecha_time_db(Date.Now,
                                                               date1al)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim datehora As String = Date.Now.Hour
        Dim stiempo_dias As Object = Nothing
        Dim minuno_respuesta As Object = Nothing
        Dim hora_respuesta As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Dim refclas_gestion_fecha As New ClassGestionFechas
        Result = refclas_gestion_fecha.Solicita_tiempo_respuesta_tramite(stru_envio.FECHA_REGISTRO,
                                                                         stiempo_dias,
                                                                         hora_respuesta,
                                                                         minuno_respuesta,
                                                                         dias_calendario,
                                                                         dias_no_habiles)
        If Result <> "YES" Then
            Confirma_recibido_de_la_solicitud = Result
            Exit Function
        End If
        '---------------------------------------------------------------------
        'Actualiza estado respuesta radicado 
        '--------------------------------------------------------------------- 
        Dim sqlupdate_actualiza_estado_respuesta As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & 5 & ", ID_RUTA_WF=" &
           HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",ID_TAREA_WF=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & ",FECHA_RESPUETA='" & date1al & "', HORA_RESPUESTA='" & datehora & "', " &
           "TIEMPO_RESPUESTA=" & Val(stiempo_dias) &
           ",id_usuario_gestion_propietario=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
           ",TIPO_RESPUESTA_ELAB_USUARIO=" & "2" &
           " ,NOTA_RESPUESTA=" & valor_nota &
           "  where ID_RESPUESTA_RADICADO=" & id_respuesta
        Dim hor As String = Now
        Dim detalle_trans As String = "RESPUESTA RADICADO"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "RESPUESTA DE LA SOLICITUD INTERNA NUMERO (" & stru_envio.ID_RESPUESTA_RADICADO &
        ") DEL RADICADO " & stru_envio.RADICADO & " SOLO SE CONFIRMA  Al peticionario " & stru_envio.DESTINATARIO &
        " el día " & date1al & " a las " & datehora
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     stru_envio.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WEB-WORKFLOW','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                            isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlupdate_actualiza_estado_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Confirma_recibido_de_la_solicitud = "Imposible actualizar el estado de la respuesta  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Confirma_recibido_de_la_solicitud = "Imposible actualizar el log de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------------------------------------------------------------------
            'Actualiza el estado del tramite en workflow
            '-------------------------------------------------------------------
            Dim Refclas_adic As New Class_DAT_ADIC_TAR
            Result = Refclas_adic.Actualiza_estado_tramite_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                           Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                           "Tramitado")
            If Result <> "YES" Then
                Confirma_recibido_de_la_solicitud = "Imposible actualizar el estado del tramite en workflow  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If Not HttpContext.Current.Session.Item("dat_gred_cahce") Is Nothing Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item(0) = Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")) Then
                        HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item("ESTADO") = "Tramitado"
                        HttpContext.Current.Session.Item("dat_gred_cahce").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item(0) = Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")) Then
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item("ESTADO") = "Tramitado"
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            myTrans.Commit()
            myConnection.Close()
            If CheckBox_envio_correo_solo_confirmar.Checked = True Then
                Dim matri_resp() As String = Nothing
                Dim z As Integer = 0
                Dim Refclas_correo As New ClassCorreo
                Dim matri_mensaje() As String = {""}
                If stru_envio.NOTA_RESPUESTA <> "" Then
                    Erase matri_mensaje
                    matri_mensaje = stru_envio.NOTA_RESPUESTA.ToString.Split(vbCrLf)
                End If

                Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
                Dim nombre_area As String = ""
                Dim cargo_usuario_gestion_responde As String = ""
                Dim nombre_usuario_gestion_responde As String = ""
                Dim correo_usuario_gestion_responde As String = ""
                Result = Ref_clas_remit_dest.Retorna_datos_caracterizacion_usuario_gestion(id_usuario_gestion,
                                                                                           nombre_usuario_gestion_responde,
                                                                                           cargo_usuario_gestion_responde,
                                                                                           correo_usuario_gestion_responde)
                If Result <> "YES" Then
                    Confirma_recibido_de_la_solicitud = "Se confirmo la solicitud, pero no se pudo notificar al correo electrónico " & Result
                    Exit Function
                End If
                Result = Reclas_remit_dest_interno.Solicita_id_area_nombre_area_destinatario(id_usuario_gestion,
                                                                                           0,
                                                                                           nombre_area)
                If Result <> "YES" Then
                    Confirma_recibido_de_la_solicitud = "Se confirmo la solicitud, pero no se pudo notificar al correo electrónico " & Result
                    Exit Function
                End If
                Result = Me.Genera_zip_documento_anexo(id_respuesta,
                                                        matri_anexos)
                If Result <> "YES" Then
                    Confirma_recibido_de_la_solicitud = "Se confirmo la solicitud, pero no se pudo notificar al correo electrónico " & Result
                    Exit Function
                End If
                Result = Refclas_correo.Envio_Correo_confirmacion_recibido_solicitud(stru_envio.RADICADO,
                                                                                     matri_mensaje,
                                                                                     TextBox_correo_solo_confirmar.value,
                                                                                     matri_resp,
                                                                                     nombre_usuario_gestion_responde,
                                                                                     cargo_usuario_gestion_responde,
                                                                                     nombre_area,
                                                                                     correo_usuario_gestion_responde,
                                                                                     stru_envio,
                                                                                     "",
                                                                                     matri_anexos,
                                                                                     "")
                If Result <> "YES" Then
                    Confirma_recibido_de_la_solicitud = "Se confirmo la solicitud, pero no se pudo notificar al correo electrónico " & Result
                    Exit Function
                End If

                If Confirma_recibido_de_la_solicitud = "" Then
                    Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                    Dim update As String = "Update ra_respuesta_radicado set estado_envio_correo=1 where ID_RESPUESTA_RADICADO=" & id_respuesta
                    Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(update)
                    If Result <> "YES" Then
                        Confirma_recibido_de_la_solicitud = "Se envió el correo electrónico con la confirmación, sin embargo no se cambio el estado de envio en la base de datos " & Result
                        Exit Function
                    End If
                End If
            End If
            Confirma_recibido_de_la_solicitud = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Confirma_recibido_de_la_solicitud = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Confirma_recibido_de_la_solicitud = "Error General función  Confirma_recibido_de_la_solicitud " & e.Message
            Exit Function
        End Try
    End Function
    Function Finalizar_tramite(ByVal id_tarea As Long) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New Class_Listado_Actividades_workflow
            Dim id_actividad As Integer = 0
            Result = Refclas.Solicita_actividad_workflow_final(id_actividad)
            If Result <> "YES" Then
                Finalizar_tramite = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = id_tarea
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            Result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                Finalizar_tramite = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                                         Estado_solicitud_aprobacion,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Finalizar_tramite = Result
                Exit Function
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Finalizar_tramite = "Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar"
                Exit Function

            End If
            Dim Refclas_workflow As New ClassWorkflow
            Result = Refclas_workflow.Terminar_Tarea_Workflow_Bacth("",
                                                                    id_actividad.ToString,
                                                                    0,
                                                                    HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                    "",
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    "",
                                                                    0)
            If Result <> "YES" Then
                Finalizar_tramite = Result
                Exit Function
            Else
                Finalizar_tramite = Result
                Exit Function
            End If
        Catch ex As Exception
            Finalizar_tramite = "Inconsistencia general funcion Finalizar_tramite " & ex.Message
        End Try
    End Function
    Function ArchivaTramiteHistoricoRadicado(ByRef RadicadoTramite As String,
                                             ByRef IdRespuestaRadicado As Integer,
                                             ByVal NotaArchivadoTramite As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Archiva radicado desde el historico de radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'RadicadoTramite      : Representa el consecutivo del radicado 
        'NotaArchivadoTramite : Representa la nota del archivo del tramite
        'IdRespuestaRadicado  : Representa la identifcación de la respuesta
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-17
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("UTILGCOROptionHCarchivaTramite") = 0 Then
                ArchivaTramiteHistoricoRadicado = "El usuario no tiene permiso para archivar el trámite desde la opción Consulta de Histórico de Trámites."
                Exit Function
            End If
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            If IdRespuestaRadicado = 0 Then
                Result = Class_ra_respuesta_radicado.SolicitaIdRespuestaRadicado(RadicadoTramite,
                                                                                 IdRespuestaRadicado)
                If Result <> "YES" Then
                    ArchivaTramiteHistoricoRadicado = Result
                    Exit Function
                End If
            End If
            If IdRespuestaRadicado = 0 Then
                ArchivaTramiteHistoricoRadicado = "El radicado no cuenta con un registro de trámite de respuesta; por lo tanto, no es posible archivar el trámite."
                Exit Function
            End If
            Dim struc_envio As stru_envio = Nothing
            Result = Class_ra_respuesta_radicado.Solicita_datos_estructura_envio_por_id_respuesta(IdRespuestaRadicado,
                                                                                                  struc_envio,
                                                                                                  1)
            If Result <> "YES" Then
                ArchivaTramiteHistoricoRadicado = Result
                Exit Function
            End If
            If RadicadoTramite = "" Then
                RadicadoTramite = struc_envio.RADICADO
            End If
            If struc_envio.RADICADO = Nothing Then
                ArchivaTramiteHistoricoRadicado = "Imposible encontrar los datos del registro de respuesta relacionados al identificador de respuesta (" & IdRespuestaRadicado & ")"
                Exit Function
            End If
            If struc_envio.FECHA_RESPUETA <> "" Then
                ArchivaTramiteHistoricoRadicado = "El trámite ya cuenta con una respuesta, por lo que no es posible continuar con el proceso de archivado."
                Exit Function
            End If
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Dim NombreRutaWorklow As String = ""
            Result = Class_worflow_rutas.Retorna_nombre_ruta_workflow(NombreRutaWorklow)
            If Result <> "YES" Then
                ArchivaTramiteHistoricoRadicado = Result
                Exit Function
            End If
            Dim IdRutaWorkflow As Integer = 0
            Result = Class_worflow_rutas.Retorna_id_ruta_workflow(NombreRutaWorklow,
                                                                  IdRutaWorkflow)
            If Result <> "YES" Then
                ArchivaTramiteHistoricoRadicado = Result
                Exit Function
            End If
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim NombreCampoRadicadoRuta As String = ""
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(IdRutaWorkflow,
                                                                                      NombreCampoRadicadoRuta)
            If Result <> "YES" Then
                ArchivaTramiteHistoricoRadicado = Result
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim IdTareaWorkflow As Long = 0
            Result = Class_DAT_ADIC_TAR.SolicitaIdTareaRutaRadicado(NombreRutaWorklow,
                                                                    NombreCampoRadicadoRuta,
                                                                    RadicadoTramite,
                                                                    IdTareaWorkflow)
            If Result <> "YES" Then
                ArchivaTramiteHistoricoRadicado = Result
                Exit Function
            End If
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim FechaRegistro As String = ""
            Dim datehora As String = Date.Now.Hour
            Result = ClassGestionFechas.Formatea_fecha_time_db(Date.Now,
                                                              FechaRegistro)
            If Result <> "YES" Then
                ArchivaTramiteHistoricoRadicado = Result
                Exit Function
            End If
            Dim stiempo_dias As Object = Nothing
            Dim minuno_respuesta As Object = Nothing
            Dim hora_respuesta As Object = Nothing
            Dim dias_calendario As Object = Nothing
            Dim dias_no_habiles As Object = Nothing
            Result = ClassGestionFechas.Solicita_tiempo_respuesta_tramite(struc_envio.FECHA_REGISTRO,
                                                                          stiempo_dias,
                                                                          hora_respuesta,
                                                                          minuno_respuesta,
                                                                          dias_calendario,
                                                                          dias_no_habiles)
            If Result <> "YES" Then
                ArchivaTramiteHistoricoRadicado = Result
                Exit Function
            End If
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim IdActividadFinal As Integer = 0
            Result = Class_Listado_Actividades_workflow.Solicita_actividad_workflow_final(IdActividadFinal)
            If Result <> "YES" Then
                ArchivaTramiteHistoricoRadicado = Result
                Exit Function
            End If
            Dim sqlupdate_actualiza_estado_respuesta As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & 6 & ", ID_RUTA_WF=" &
            IdRutaWorkflow & ",ID_TAREA_WF=" & IdTareaWorkflow & ",FECHA_RESPUETA='" & FechaRegistro & "', HORA_RESPUESTA='" & datehora & "', " &
            "TIEMPO_RESPUESTA=" & Val(stiempo_dias) &
            ",id_usuario_gestion_propietario=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
            ",TIPO_RESPUESTA_ELAB_USUARIO=" & "2" &
            "  where ID_RESPUESTA_RADICADO=" & IdRespuestaRadicado
            Dim hor As String = Now
            Dim DetalleTrans As String = "ARCHIVA TRAMITE"
            Dim CamposTrans As String = ""
            Dim DatosInsert As String = ""
            CamposTrans = NotaArchivadoTramite
            DatosInsert = DatosInsert & "('" & DetalleTrans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" &
                          HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & FechaRegistro & "'," &
                          IdRespuestaRadicado & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WEB-WORKFLOW','" & CamposTrans & "')"
            Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " & DatosInsert

            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            ref.Returna_Conexion_Mysql(myConnection)
            Dim myTrans As MySqlTransaction
            Dim sqlresultinsert As Integer = 0
            Try
                Dim myCommand As MySqlCommand = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand.Connection = myConnection
                myCommand.Transaction = myTrans
                myCommand.CommandText = sqlupdate_actualiza_estado_respuesta
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    ArchivaTramiteHistoricoRadicado = "Imposible actualizar el estado de la respuesta  "
                    myConnection.Close()
                    Exit Function
                End If
                myCommand.CommandText = update_gestion
                sqlresultinsert = myCommand.ExecuteNonQuery()
                If sqlresultinsert = 0 Then
                    ArchivaTramiteHistoricoRadicado = "Imposible actualizar el log de respuesta  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                '-------------------------------------------------------------------
                'Actualiza el estado del tramite en workflow
                '-------------------------------------------------------------------
                Dim Refclas_adic As New Class_DAT_ADIC_TAR
                Result = Refclas_adic.Actualiza_estado_tramite_tarea_workflow(NombreRutaWorklow,
                                                                              IdTareaWorkflow,
                                                                              "Tramitado archivado")
                If Result <> "YES" Then
                    ArchivaTramiteHistoricoRadicado = "Imposible cambiar el estado de la respuesta  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                myTrans.Commit()
                myConnection.Close()
                Dim ClassWorkflow As New ClassWorkflow
                Result = ClassWorkflow.EnviaTareaFInalWorflowArchivaRespuesta(IdTareaWorkflow,
                                                                              IdRutaWorkflow,
                                                                              IdActividadFinal)
                If Result <> "YES" Then
                    ArchivaTramiteHistoricoRadicado = "El trámite fue archivado; sin embargo, no se pudo enviar a la actividad final en el módulo de flujo de trabajo. Inconsitencia relacionada (" & Result & ")."
                    Exit Function
                Else
                    ArchivaTramiteHistoricoRadicado = "YES"
                    Exit Function
                End If
            Catch e As Exception
                Try
                    myTrans.Rollback()
                Catch ex As MySqlException
                    If Not myTrans.Connection Is Nothing Then
                        ArchivaTramiteHistoricoRadicado = "An exception of type " + ex.GetType().ToString() +
                                          " was encountered while attempting to roll back the transaction."
                        Exit Function
                    End If
                End Try
                If Not myConnection Is Nothing Then
                    myConnection.Close()
                End If
                ArchivaTramiteHistoricoRadicado = "Error General función  ArchivaTramiteHistoricoRadicado " & e.Message
                Exit Function
            End Try

        Catch ex As Exception
            ArchivaTramiteHistoricoRadicado = "Inconsistencia general funcion ArchivaTramiteHistoricoRadicado " & ex.Message
        End Try
    End Function
    Function Archiva_tramite_de_la_solicitud(ByVal tex_corroe_tokenize As String,
                                             ByVal confirma_envio_correo As Integer,
                                             ByRef resultado_evio_correo As String,
                                             ByRef estado_terminar_tarea As String) As String
        Dim Ref_clas_remit_dest As New Class_remit_dest_interno
        Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
        Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
        Dim Result As String = ""
        Dim estado_obligatorio_respuesta As Integer = 0
        Dim id_tipo_tramite As Integer = 0
        Dim descripcion_tramite As String = ""
        Dim Radicado As String = ""
        resultado_evio_correo = "YES"
        Dim matri_anexos() As String = Nothing
        If confirma_envio_correo = 1 Then
            If tex_corroe_tokenize = "" Then
                Archiva_tramite_de_la_solicitud = "Debe informar el correo electrónico para notificación "
                Exit Function
            End If
        End If
        Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                Radicado)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = Result
            Exit Function
        End If
        If Radicado = "" Then
            Archiva_tramite_de_la_solicitud = "La tarea seleccionada no tiene radicado relacionado"
            Exit Function
        End If

        Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
        Dim id_respuesta As Integer = 0
        Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                           id_respuesta)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = Result
            Exit Function
        End If
        Dim ruta_virtual As String = "../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta & "/"
        Dim Refclas_ As New Class_Listado_Actividades_workflow
        Dim id_actividad As Integer = 0
        Result = Refclas_.Solicita_actividad_workflow_final(id_actividad)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = Result
            Exit Function
        End If
        Dim id_usuario_gestion As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim stru_envio As stru_envio = Nothing
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                    stru_envio)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = Result
            Exit Function
        End If
        If stru_envio.FECHA_RESPUETA <> "" Then
            Archiva_tramite_de_la_solicitud = "La solicitud tiene una respuesta, el sistema no permite archivar el tramite"
            Exit Function
        End If
        If stru_envio.RADICADO_RESPUESTA <> "" Then
            Archiva_tramite_de_la_solicitud = "Usted inicio un proceso de respuesta formal generando el radicado de respuesta número " & stru_envio.RADICADO_RESPUESTA & ", por lo tanto debe terminar el proceso de respuesta formal imposible archivar el tramite."
            Exit Function
        End If
        Dim nombre_plantilla_radicado As String = ""
        Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
        Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(stru_envio.system_plantilla_radicado_id_plantilla,
                                                                             nombre_plantilla_radicado)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = Result
            Exit Function
        End If
        Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla_radicado,
                                                                                             stru_envio.RADICADO,
                                                                                             id_tipo_tramite,
                                                                                             descripcion_tramite)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = Result
            Exit Function
        End If
        Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                         estado_obligatorio_respuesta)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = Result
            Exit Function
        End If
        If estado_obligatorio_respuesta = 1 Then
            Archiva_tramite_de_la_solicitud = "El trámite requiere de un radicado de respuesta, el sistema no permite archivar el tramite"
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = ""
        Result = ""
        Result = refclas_gestion_fechas.Formatea_fecha_time_db(Date.Now,
                                                               date1al)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim datehora As String = Date.Now.Hour
        Dim stiempo_dias As Object = Nothing
        Dim minuno_respuesta As Object = Nothing
        Dim hora_respuesta As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Dim refclas_gestion_fecha As New ClassGestionFechas
        Result = refclas_gestion_fecha.Solicita_tiempo_respuesta_tramite(stru_envio.FECHA_REGISTRO,
                                                                         stiempo_dias,
                                                                         hora_respuesta,
                                                                         minuno_respuesta,
                                                                         dias_calendario,
                                                                         dias_no_habiles)
        If Result <> "YES" Then
            Archiva_tramite_de_la_solicitud = Result
            Exit Function
        End If
        '---------------------------------------------------------------------
        'Actualiza estado respuesta radicado
        '--------------------------------------------------------------------- 
        Dim sqlupdate_actualiza_estado_respuesta As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & 6 & ", ID_RUTA_WF=" &
           HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",ID_TAREA_WF=" & HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") & ",FECHA_RESPUETA='" & date1al & "', HORA_RESPUESTA='" & datehora & "', " &
           "TIEMPO_RESPUESTA=" & Val(stiempo_dias) &
           ",id_usuario_gestion_propietario=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
           ",TIPO_RESPUESTA_ELAB_USUARIO=" & "2" &
           "  where ID_RESPUESTA_RADICADO=" & id_respuesta
        Dim hor As String = Now
        Dim detalle_trans As String = "ARCHIVA TRAMITE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "RESPUESTA DE LA SOLICITUD INTERNA NUMERO (" & stru_envio.ID_RESPUESTA_RADICADO &
        ") DEL RADICADO " & stru_envio.RADICADO & " SOLO SE ARCHIVA EL TRAMITE peticionario " & stru_envio.DESTINATARIO &
        " el día " & date1al & " a las " & datehora
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     stru_envio.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WEB-WORKFLOW','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                            isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlupdate_actualiza_estado_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Archiva_tramite_de_la_solicitud = "Imposible actualizar el estado de la respuesta  "
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Archiva_tramite_de_la_solicitud = "Imposible actualizar el log de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------------------------------------------------------------------
            'Actualiza el estado del tramite en workflow
            '-------------------------------------------------------------------
            Dim Refclas_adic As New Class_DAT_ADIC_TAR
            Result = Refclas_adic.Actualiza_estado_tramite_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                          Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                          "Tramitado archivado")
            If Result <> "YES" Then
                Archiva_tramite_de_la_solicitud = "Imposible cambiar el estado de la respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim ClassCorreo As New ClassCorreo
            Dim matri_mensaje() As String = Nothing
            If confirma_envio_correo = 1 Then
                resultado_evio_correo = ClassCorreo.Envio_Correo_confirma_Archivado_tramite(matri_mensaje,
                                                                                           tex_corroe_tokenize,
                                                                                           Radicado,
                                                                                           id_usuario_gestion,
                                                                                           id_respuesta,
                                                                                           stru_envio)
            End If
            Dim Refclas_workflow As New ClassWorkflow
            estado_terminar_tarea = Refclas_workflow.Terminar_Tarea_Workflow_Bacth("",
                                                                                  id_actividad.ToString,
                                                                                  0,
                                                                                  HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                  "",
                                                                                  0,
                                                                                  0,
                                                                                  0,
                                                                                  0,
                                                                                  "",
                                                                                  0)
            If estado_terminar_tarea = "YES" Then
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = -1
            End If

            Archiva_tramite_de_la_solicitud = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Archiva_tramite_de_la_solicitud = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Archiva_tramite_de_la_solicitud = "Error General función  Archiva_tramite_de_la_solicitud " & e.Message
            Exit Function
        End Try
    End Function

    Function Redirecciona_solicitud_entidad_externa(ByVal Class_config_general_service_ As List(Of Class_config_general_service),
                                                    ByRef estado_terminar_tarea As String,
                                                    ByRef resultado_evio_correo As String) As String
        Dim Ref_clas_remit_dest As New Class_remit_dest_interno
        Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
        Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
        Dim Result As String = ""
        Dim estado_obligatorio_respuesta As Integer = 0
        Dim id_tipo_tramite As Integer = 0
        Dim descripcion_tramite As String = ""
        Dim Radicado As String = ""
        Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                Radicado)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        If Radicado = "" Then
            Redirecciona_solicitud_entidad_externa = "La tarea seleccionada no tiene radicado relacionado"
            Exit Function
        End If
        Dim Class_config_general_service As New Class_config_general_service
        Dim index_campo As Integer = -1
        Result = Class_config_general_service.Solicita_index_campo_form_control("Nombre_externo",
                                                                                Class_config_general_service_,
                                                                                index_campo)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        If index_campo = -1 Then
            Redirecciona_solicitud_entidad_externa = "Imposible encontrar el index del campo (Nombre_externo)"
            Exit Function
        End If
        Dim Nombre_entidad_externa As String = Class_config_general_service_(index_campo).value_campo
        index_campo = -1
        Result = Class_config_general_service.Solicita_index_campo_form_control("correo_electronico",
                                                                                Class_config_general_service_,
                                                                                index_campo)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        If index_campo = -1 Then
            Redirecciona_solicitud_entidad_externa = "Imposible encontrar el index del campo (correo_electronico)"
            Exit Function
        End If
        Dim correo_electronico_entidad As String = Class_config_general_service_(index_campo).value_campo
        index_campo = -1
        Result = Class_config_general_service.Solicita_index_campo_form_control("nota_traslado",
                                                                                Class_config_general_service_,
                                                                                index_campo)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        If index_campo = -1 Then
            Redirecciona_solicitud_entidad_externa = "Imposible encontrar el index del campo (nota_traslado)"
            Exit Function
        End If
        Dim motivo As String = Class_config_general_service_(index_campo).value_campo
        Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
        Dim id_respuesta As Integer = 0
        Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                           HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                           id_respuesta)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If

        Dim Refclas_ As New Class_Listado_Actividades_workflow
        Dim id_actividad As Integer = 0
        Result = Refclas_.Solicita_actividad_workflow_final(id_actividad)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Dim id_usuario_gestion As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
        Dim stru_envio As stru_envio = Nothing
        Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
        Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                    stru_envio)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        If stru_envio.FECHA_RESPUETA <> "" Then
            Redirecciona_solicitud_entidad_externa = "La solicitud tiene una respuesta, el sistema no permite archivar el tramite"
            Exit Function
        End If
        If stru_envio.RADICADO_RESPUESTA <> "" Then
            Redirecciona_solicitud_entidad_externa = "Usted inicio un proceso de respuesta formal generando el radicado de respuesta número " & stru_envio.RADICADO_RESPUESTA & ", por lo tanto debe terminar el proceso de respuesta formal imposible archivar el tramite."
            Exit Function
        End If
        Dim nombre_plantilla_radicado As String = ""
        Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
        Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(stru_envio.system_plantilla_radicado_id_plantilla,
                                                                             nombre_plantilla_radicado)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla_radicado,
                                                                                             stru_envio.RADICADO,
                                                                                             id_tipo_tramite,
                                                                                             descripcion_tramite)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
        Dim id_gabinete As Integer = 0
        Dim nombre_gabinete As String = ""
        Result = Class_tipo_doc_entrante.Retorna_id_nombre_gabinete_tipo_tramite(stru_envio.system_plantilla_radicado_id_plantilla,
                                                                                 descripcion_tramite,
                                                                                 id_gabinete,
                                                                                 nombre_gabinete)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If

        Dim ruta_server As String = ""
        Dim Refclas_ra_config As New Class_ra_config_notifica_correo
        Dim correo_copia As String = ""
        Result = Refclas_ra_config.Solicita_estado_ruta_notificacion(ruta_server,
                                                                     0,
                                                                     correo_copia)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Dim ClassDaGabinete As New ClassDaGabinete
        Dim stru_paramter_image() As stru_paramter_image = Nothing
        Result = ClassDaGabinete.Solicita_lista_rutas_imagenes_enlazadas_gabinete(nombre_gabinete,
                                                                                  Radicado,
                                                                                  Radicado,
                                                                                  0,
                                                                                  1,
                                                                                  ruta_server,
                                                                                  stru_paramter_image)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Dim refclasradicado As New ClassRadicador
        Dim correo_electronico_remitente As String = ""
        Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(stru_envio.codigo_dest_externo,
                                                                                   correo_electronico_remitente,
                                                                                   stru_envio.system_plantilla_radicado_id_plantilla)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Dim Class_ra_registro_pqr As New Class_ra_registro_pqr
        Dim correo_electronico_usuario_pqr As String = ""
        Result = Class_ra_registro_pqr.Solicita_correo_electronico_usuario_pqr(Radicado,
                                                                               correo_electronico_usuario_pqr)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Dim date1al As String = ""
        Result = ""
        Result = refclas_gestion_fechas.Formatea_fecha_time_db(Date.Now,
                                                               date1al)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = "Imposible formatear fecha " & Result
            Exit Function
        End If
        Dim datehora As String = Date.Now.Hour
        Dim stiempo_dias As Object = Nothing
        Dim minuno_respuesta As Object = Nothing
        Dim hora_respuesta As Object = Nothing
        Dim dias_calendario As Object = Nothing
        Dim dias_no_habiles As Object = Nothing
        Dim refclas_gestion_fecha As New ClassGestionFechas
        Result = refclas_gestion_fecha.Solicita_tiempo_respuesta_tramite(stru_envio.FECHA_REGISTRO,
                                                                         stiempo_dias,
                                                                         hora_respuesta,
                                                                         minuno_respuesta,
                                                                         dias_calendario,
                                                                         dias_no_habiles)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Dim sql_insert As String = ""
        Result = Class_config_general_service.add_campo_form_control("ra_respuesta_radicado_ID_RESPUESTA_RADICADO", "Id_respuesta", "", id_respuesta, id_respuesta, 0, 1, "0", 0, 1, Class_config_general_service_)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Result = Class_config_general_service.add_campo_form_control("Fecha_registro", "Fecha traslado", "", date1al, date1al, 0, 1, "0", 0, 1, Class_config_general_service_)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        Result = Class_config_general_service.Create_insert_form_control("ra_respuesta_envia_entidades",
                                                                         Class_config_general_service_,
                                                                         sql_insert)
        If Result <> "YES" Then
            Redirecciona_solicitud_entidad_externa = Result
            Exit Function
        End If
        '---------------------------------------------------------------------
        'Actualiza estado respuesta radicado
        '--------------------------------------------------------------------- 
        Dim sqlupdate_actualiza_estado_respuesta As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & 7 & ", ID_RUTA_WF=" &
           HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",ID_TAREA_WF=" & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & ",FECHA_RESPUETA='" & date1al & "', HORA_RESPUESTA='" & datehora & "', " &
           "TIEMPO_RESPUESTA=" & Val(stiempo_dias) &
           ",id_usuario_gestion_propietario=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
           ",TIPO_RESPUESTA_ELAB_USUARIO=" & "2" &
           "  where ID_RESPUESTA_RADICADO=" & id_respuesta
        Dim hor As String = Now
        Dim detalle_trans As String = "TRASLADO TRAMITE"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        campos_trans = "TRASLADO DE LA SOLICITUD INTERNA NUMERO (" & stru_envio.ID_RESPUESTA_RADICADO &
        ") DEL RADICADO " & stru_envio.RADICADO & " A LA ENTIDAD " & Nombre_entidad_externa &
        " el día " & date1al & " a las " & datehora
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     stru_envio.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WEB-WORKFLOW','" & campos_trans & "')"
        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                            isert_datos

        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlupdate_actualiza_estado_respuesta
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Redirecciona_solicitud_entidad_externa = "Imposible actualizar el estado de la respuesta  "
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = sql_insert
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Redirecciona_solicitud_entidad_externa = "Imposible registrar el traslado  de la solicitud"
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Redirecciona_solicitud_entidad_externa = "Imposible actualizar el log de respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------------------------------------------------------------------
            'Actualiza el estado del tramite en workflow
            '-------------------------------------------------------------------
            Dim Refclas_adic As New Class_DAT_ADIC_TAR
            Result = Refclas_adic.Actualiza_estado_tramite_tarea_workflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                           Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                           "Traslado")
            If Result <> "YES" Then
                Redirecciona_solicitud_entidad_externa = "Imposible cambiar el estado de la respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim ClassCorreo As New ClassCorreo
            Dim matri_mensaje() As String = Nothing
            Dim correos As String = correo_electronico_entidad
            If correo_electronico_remitente <> "" Then
                correos = correos & "," & correo_electronico_remitente
            End If
            If correo_electronico_usuario_pqr <> "" Then
                correos = correos & "," & correo_electronico_usuario_pqr
            End If
            resultado_evio_correo = ClassCorreo.Envio_Correo_confirma_traslado_tramite(Class_config_general_service_,
                                                                                       correos,
                                                                                       Radicado,
                                                                                       id_usuario_gestion,
                                                                                       id_respuesta,
                                                                                       stru_envio,
                                                                                       Nombre_entidad_externa,
                                                                                       motivo,
                                                                                       stru_paramter_image,
                                                                                       ruta_server)


            Dim Refclas_workflow As New ClassWorkflow
            estado_terminar_tarea = Refclas_workflow.Terminar_Tarea_Workflow_Bacth("",
                                                                                  id_actividad.ToString,
                                                                                  0,
                                                                                  HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                  "",
                                                                                  0,
                                                                                  0,
                                                                                  0,
                                                                                  0,
                                                                                  "",
                                                                                  0)
            If estado_terminar_tarea = "YES" Then
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = -1
            End If
            Redirecciona_solicitud_entidad_externa = "YES"
            Exit Function
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Redirecciona_solicitud_entidad_externa = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Redirecciona_solicitud_entidad_externa = "Error General función  Redirecciona_solicitud_entidad_externa " & e.Message
            Exit Function
        End Try
    End Function
    Function Actualiza_estados_general_semaforo(ByVal id_respuesta As Integer,
                                                ByRef pag As Page) As String
        '--------------------------------------------------------------------
        'Función : actualiza los estados de los semaforos de las respuestas
        'de los tramites
        'Fecha : 2016-08-01
        'Ing Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------
        Try
            Dim Image_estado_resp As Image = pag.FindControl("Image_estado_resp")
            If Image_estado_resp Is Nothing Then
                Actualiza_estados_general_semaforo = "Imposible encontrar el control Image_estado_resp"
                Exit Function
            End If
            Dim Image_estado_resp_solo_confirm As Image = pag.FindControl("Image_estado_resp_solo_confirm")
            If Image_estado_resp_solo_confirm Is Nothing Then
                Actualiza_estados_general_semaforo = "Imposible encontrar el control Image_estado_resp_solo_confirm"
                Exit Function
            End If
            Dim UpdatePanel_image_semaforo As UpdatePanel = pag.FindControl("UpdatePanel_image_semaforo")
            If UpdatePanel_image_semaforo Is Nothing Then
                Actualiza_estados_general_semaforo = "Imposible encontrar el control UpdatePanel_image_semaforo"
                Exit Function
            End If
            Dim UpdatePanel_image_semaforo_resp As UpdatePanel = pag.FindControl("UpdatePanel_image_semaforo_resp")
            If UpdatePanel_image_semaforo_resp Is Nothing Then
                Actualiza_estados_general_semaforo = "Imposible encontrar el control UpdatePanel_image_semaforo_resp"
                Exit Function
            End If
            Dim Panel_respuesta_formal As Panel = pag.FindControl("Panel_respuesta_formal")
            If Panel_respuesta_formal Is Nothing Then
                Actualiza_estados_general_semaforo = "Imposible encontrar el control Panel_respuesta_formal"
                Exit Function
            End If
            Dim panel_respuesta_confirmar As Panel = pag.FindControl("panel_respuesta_confirmar")
            If panel_respuesta_confirmar Is Nothing Then
                Actualiza_estados_general_semaforo = "Imposible encontrar el control panel_respuesta_confirmar"
                Exit Function
            End If
            Dim tipo_respuesta_tramite As Integer = 0
            Dim Result As String = ""
            Result = Me.Retorna_estado_envio_por_id_respuesta(id_respuesta,
                                                              tipo_respuesta_tramite)
            If Result <> "YES" Then
                Actualiza_estados_general_semaforo = Result
                Exit Function
            End If
            Dim reflclas_resp_radicado As New Class_ra_respuesta_radicado
            If Panel_respuesta_formal.Visible = True Then
                If tipo_respuesta_tramite = 0 Then
                    Result = reflclas_resp_radicado.Solicita_estados_semaforo_respuesta(id_respuesta,
                                                                                       Image_estado_resp)
                    If Result <> "YES" Then
                        Actualiza_estados_general_semaforo = Result
                        Exit Function
                    End If
                Else
                    Dim ref_clas_resp As New Class_ra_respuesta_radicado
                    Result = ref_clas_resp.Solicita_estados_semaforo_respuesta_electronica(id_respuesta,
                                                                                           Image_estado_resp)
                    If Result <> "YES" Then
                        Actualiza_estados_general_semaforo = Result
                        Exit Function
                    End If
                End If
                UpdatePanel_image_semaforo.Update()
            End If
            If panel_respuesta_confirmar.Visible = True Then
                If tipo_respuesta_tramite = 0 Then
                    Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion(id_respuesta,
                                                                                     Image_estado_resp_solo_confirm)
                    If Result <> "YES" Then
                        Actualiza_estados_general_semaforo = Result
                        Exit Function

                    End If
                Else
                    Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(id_respuesta,
                                                                                                 Image_estado_resp_solo_confirm)
                    If Result <> "YES" Then
                        Actualiza_estados_general_semaforo = Result
                        Exit Function

                    End If
                End If
                UpdatePanel_image_semaforo_resp.Update()
            End If
            Actualiza_estados_general_semaforo = "YES"
        Catch ex As Exception
            Actualiza_estados_general_semaforo = "Inconsistencia general función Actualiza_estados_general_semaforo " & ex.Message
        End Try
    End Function
    Function Retorna_pasw_encriptado_usuario_administrador(ByVal login_usuario_autoriza As String,
                                                           ByRef pasword_autoriza As String,
                                                           ByRef id_user_admnistrador As Integer) As String
        Try
            Dim Parametro_Consulta As String = "SELECT Pasw,id_Administrador_Aplicacion " &
                        " FROM administrador_aplicacion " &
                         " where  Login='" & login_usuario_autoriza & "'"
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("administrador_aplicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_pasw_encriptado_usuario_administrador = "Función Retorna_pasw_encriptado_usuario_administrador dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_pasw_encriptado_usuario_administrador = "Usuario de autorización no valido"
                Exit Function
            Else
                pasword_autoriza = Datset.Tables(0).Rows(0).Item(0).ToString
                id_user_admnistrador = Datset.Tables(0).Rows(0).Item(1)
                Retorna_pasw_encriptado_usuario_administrador = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_pasw_encriptado_usuario_administrador = "Inconsistencia función Retorna_pasw_encriptado_usuario_administrador " & ex.Message
        End Try
    End Function
    Function Asigna_datos_perfil_administrador_base_datos_estructura(ByVal id_usuario_admin As Integer,
                                                                     ByRef perfil As perfil_administrador) As String
        Try

            Dim Parametro_Consulta As String = "SELECT adm_docuarchi,adm_workflow,adm_radicacion,adm_gestor,anula_radicado,reasigna_documento,cambia_tipo_tramite,reversa_respuesta,actualiza_peticionario" &
            " FROM  ra_perfil_administracion_aplicacion where adm_apli_id_Administrador_Aplicacion=" & id_usuario_admin
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("administrador_aplicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Asigna_datos_perfil_administrador_base_datos_estructura = "Función Asigna_datos_perfil_administrador_base_datos_estructura dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                perfil.adm_docuarchi = Datset.Tables(0).Rows(0).Item(0)
                perfil.adm_workflow = Datset.Tables(0).Rows(0).Item(1)
                perfil.adm_radicacion = Datset.Tables(0).Rows(0).Item(2)
                perfil.adm_gestor = Datset.Tables(0).Rows(0).Item(3)
                perfil.anula_radicado = Datset.Tables(0).Rows(0).Item(4)
                perfil.reasigna_documento = Datset.Tables(0).Rows(0).Item(5)
                perfil.cambia_tipo_tramite = Datset.Tables(0).Rows(0).Item(6)
                perfil.reversa_respuesta = Datset.Tables(0).Rows(0).Item(7)
                perfil.actualiza_peticionario = Datset.Tables(0).Rows(0).Item(8)
                Asigna_datos_perfil_administrador_base_datos_estructura = "YES"
                Exit Function
            Else
                Asigna_datos_perfil_administrador_base_datos_estructura = "Imposible encontrar perfil usuario administrador"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_datos_perfil_administrador_base_datos_estructura = "Inconsistencia general función Asigna_datos_perfil_administrador_base_datos_estructura " & ex.Message
        End Try
    End Function
    Function Valida_usuario_administrador_general(ByVal login_usuario_autoriza As String,
                                                  ByVal pasword_autoriza As String,
                                                  ByRef id_usuario_autoriza As String,
                                                  ByVal tipo_autorizacion As String) As String
        Try

            Dim Result As String = ""
            Dim ref_pasword_autoriza As String = ""
            Result = Me.Retorna_pasw_encriptado_usuario_administrador(login_usuario_autoriza,
                                                                      ref_pasword_autoriza,
                                                                      id_usuario_autoriza)
            If Result <> "YES" Then
                Valida_usuario_administrador_general = Result
                Exit Function
            End If
            '-------------------------------------------------------
            'desnncripta pasword
            '-------------------------------------------------------
            If ref_pasword_autoriza.ToString.Length > 50 Then
                Result = Desc_Encript_Value(ref_pasword_autoriza)
                If Result <> "YES" Then
                    Valida_usuario_administrador_general = Result
                    Exit Function
                End If
            End If


            If ref_pasword_autoriza <> pasword_autoriza Then
                Valida_usuario_administrador_general = "Contraseña no valida"
                Exit Function
            Else
                Dim perfil As perfil_administrador
                Result = Me.Asigna_datos_perfil_administrador_base_datos_estructura(id_usuario_autoriza,
                                                                                    perfil)
                If Result <> "YES" Then
                    Valida_usuario_administrador_general = Result
                    Exit Function
                Else
                    If tipo_autorizacion = "reversa_respuesta" Then
                        If perfil.reversa_respuesta = 0 Then
                            Valida_usuario_administrador_general = "Usuario no autorizado para reversar respuesta"
                            Exit Function
                        End If
                    End If
                    If tipo_autorizacion = "anula_radicado" Then
                        If perfil.anula_radicado = 0 Then
                            Valida_usuario_administrador_general = "Usuario no autorizado para anula radicado"
                            Exit Function
                        End If
                    End If
                    If tipo_autorizacion = "reasigna_documento" Then
                        If perfil.reasigna_documento = 0 Then
                            Valida_usuario_administrador_general = "Usuario no autorizado para reasignar documento"
                            Exit Function
                        End If
                    End If
                    If tipo_autorizacion = "cambia_tipo_tramite" Then
                        If perfil.cambia_tipo_tramite = 0 Then
                            Valida_usuario_administrador_general = "Usuario no autorizado para cambiar el tipo trámite"
                            Exit Function
                        End If
                    End If
                    If tipo_autorizacion = "actualiza_peticionario" Then
                        If perfil.actualiza_peticionario = 0 Then
                            Valida_usuario_administrador_general = "Usuario no autorizado para actualizar peticionario de la solicitud"
                            Exit Function
                        End If
                    End If
                End If
                Valida_usuario_administrador_general = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Valida_usuario_administrador_general = "Inconsistencia general función Valida_usuario_administrador_general " & ex.Message
        End Try
    End Function
    Function Reversa_respuesta_radicado(ByVal estru As stru_envio,
                                        ByVal login_usuario_autoriza As String,
                                        ByVal id_usuario_autoriza As Integer) As String
        Dim Result As String = ""
        If estru.RADICADO_RESPUESTA = "" And estru.TIPO_RESPUESTA_ELAB_USUARIO = 1 Then
            Reversa_respuesta_radicado = "El sistema no registra respuesta para reversar " & estru.GUIA_ENVIO
            Exit Function
        End If
        If estru.FECHA_RESPUETA = "" And estru.TIPO_RESPUESTA_ELAB_USUARIO = 2 Then
            Reversa_respuesta_radicado = "El sistema no registra notificación para reversar " & estru.GUIA_ENVIO
            Exit Function
        End If
        If estru.TIPO_RESPUESTA_ELAB_USUARIO = 0 And estru.RADICADO_RESPUESTA = "" And estru.FECHA_RESPUETA = "" Then
            Reversa_respuesta_radicado = "El sistema no registra notificación ni respuesta para reversar " & estru.GUIA_ENVIO
            Exit Function
        End If
        If estru.ESTADO_ENVIO > 0 Then
            Reversa_respuesta_radicado = "Imposible reversar la respuesta el sistema registra envío con la guía " & estru.GUIA_ENVIO
            Exit Function
        End If
        Dim Ref_clas_rc_solicitudes As New ClassRaSolicitudesAprobacion
        Dim estado As String = ""
        Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(estru.ID_RESPUESTA_RADICADO,
                                                                                              0,
                                                                                              estado,
                                                                                              HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        If estado = "YES" Then
            Reversa_respuesta_radicado = "El sistema ha detectado solicitudes de aprobación pendientes por decisión, no se puede reversar la respuesta "
            Exit Function
        End If
        estado = ""
        Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(estru.ID_RESPUESTA_RADICADO,
                                                                                                    1,
                                                                                                    estado,
                                                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        If estado = "YES" Then
            Reversa_respuesta_radicado = "El sistema ha detectado una solicitud donde se aprueba el documento de respuesta, no se puede reversar la respuesta por favor archive la solicitud de aprobación"
            Exit Function
        End If
        Dim Ref_class_wf_ruta As New Class_worflow_rutas
        Dim nombre_ruta As String = ""
        Result = Ref_class_wf_ruta.Retorna_nombre_ruta_workflow(nombre_ruta)
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        Dim id_ruta As Integer = 0
        Result = Ref_class_wf_ruta.Retorna_id_ruta_workflow(nombre_ruta,
                                                            id_ruta)
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        Dim Refclas_confing_lista As New Class_configuracion_listado_ruta
        Dim nombre_campo_radicado As String = ""
        Result = Refclas_confing_lista.SolicitaNombreCampoRadicadoRuta(id_ruta,
                                                                       nombre_campo_radicado)
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        Dim id_tarea As Long = 0
        Dim Refclas_dat As New Class_DAT_ADIC_TAR
        Result = Refclas_dat.Solicita_id_tarea_radicado(estru.RADICADO,
                                                      nombre_ruta,
                                                      nombre_campo_radicado,
                                                      id_tarea,
                                                      0)
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        Dim hor As String = Now.Hour
        Dim date1al As String = ""
        Dim refclas_gestion_fechas As New ClassGestionFechas
        Result = refclas_gestion_fechas.Formatea_fecha_time_framework(Date.Now,
                                                                      date1al)
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If

        '------------------------------------------------
        'Retorna ruta documento plantilla para reversión
        '------------------------------------------------
        Dim ruta_local_documento As String = ""
        Dim refclas_visualiza As New ClassVisualisaDocumento
        Dim matri_documento() As String = Nothing
        If estru.ID_IMAGEN <> 0 Then
            Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(estru.ID_IMAGEN, "IMP03GESTIONTMP", matri_documento)
            If Result <> "YES" Then
                Reversa_respuesta_radicado = Result
                Exit Function
            End If
        End If
        If Not matri_documento Is Nothing Then
            ruta_local_documento = matri_documento(1)
        End If

        '---------------------------------------------
        'Retorna nombre gabinete 
        '---------------------------------------------
        Dim nombre_plantilla_radicado As String = ""
        Result = Me.Retorna_nombre_plantilla_por_id_respuesta(estru.ID_RESPUESTA_RADICADO, nombre_plantilla_radicado, "")
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        Dim estado_obligatorio As Integer = 0
        Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
        Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
        Dim id_tipo_tramite As Integer = 0
        Dim descripcion_tramite As String = ""
        Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla_radicado,
                                                                                             estru.RADICADO,
                                                                                             id_tipo_tramite,
                                                                                             descripcion_tramite)
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                         estado_obligatorio)
        If Result <> "YES" Then
            Reversa_respuesta_radicado = Result
            Exit Function
        End If
        Dim Minuto_Dur As Integer = 0
        Dim Matri_Split() As String
        Dim Matri_Min_Seg() As String
        Erase Matri_Min_Seg
        Erase Matri_Split
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try
            Dim autoriza As String = "Autorizo = " & id_usuario_autoriza & "-" & login_usuario_autoriza
            Dim insert_datos_envio As String = "('" & "REVERSA RESPUESTA" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") &
                "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                        estru.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WEB-WORKFLOW','" & autoriza & "')"
            Dim update_envio As String = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                                insert_datos_envio
            Dim sqlupdate As String = "Update ra_respuesta_radicado set ESTADO_RESPUESTA=" & "0" & ", ID_RUTA_WF=" &
                    "null" & ",ID_TAREA_WF=" & "null" & ",FECHA_RESPUETA=" & "null" & ", HORA_RESPUESTA=" & "null" & ", " &
                    "DIRECCION_DESTINATARIO=" & "null" &
                    ",TIEMPO_RESPUESTA=" & "null" &
                    ",estado_envio_correo=0" &
                    ",ID_IMAGEN_RESPUESTA=" & "null" &
                    ",ID_IMAGEN=" & "null" &
                    ",RADICADO_RESPUESTA=" & "null" &
                    ",TIPO_RESPUESTA_ELAB_USUARIO=" & estado_obligatorio &
                    "  where ID_RESPUESTA_RADICADO=" & estru.ID_RESPUESTA_RADICADO
            Dim update_radicado As String = "Update " & nombre_plantilla_radicado & " set Estado_radicado=0 where Consecutivo_Rad='" & estru.RADICADO & "'"
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlupdate
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Reversa_respuesta_radicado = "Imposible reversar la respuesta  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_envio
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Reversa_respuesta_radicado = "Imposible actualizar registro log reversar respuesta  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_radicado
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Reversa_respuesta_radicado = "Imposible anular el radicado  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Result = Refclas_dat.Actualiza_estado_tramite_tarea_workflow(nombre_ruta,
                                                                         id_tarea,
                                                                         "Por tramitar")
            If Result <> "YES" Then
                Reversa_respuesta_radicado = "Se reverso la respuesta, pero no se pudo actualizar el estado del tramite en workflow " & Result
                Exit Function
            End If
            If Not HttpContext.Current.Session.Item("dat_gred_cahce") Is Nothing Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item(0) = id_tarea Then
                        HttpContext.Current.Session.Item("dat_gred_cahce").Tables(0).Rows(i).Item("ESTADO") = "Por tramitar"
                        HttpContext.Current.Session.Item("dat_gred_cahce").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            If HttpContext.Current.Session.Item("dat_gred_cahce_restore").GetType.ToString = "System.Data.DataSet" Then
                For i As Integer = 0 To HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows.Count - 1
                    If HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item(0) = id_tarea Then
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").Tables(0).Rows(i).Item("ESTADO") = "Por tramitar"
                        HttpContext.Current.Session.Item("dat_gred_cahce_restore").AcceptChanges()
                        Exit For
                    End If
                Next
            End If
            Reversa_respuesta_radicado = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Reversa_respuesta_radicado = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Reversa_respuesta_radicado = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Retorna_estados_semaforo_respuesta_solo_confirmacion(ByVal id_respuesta As Integer,
                                                                  ByRef url_image As String) As String
        '*************************************************************************
        'Función : Retorna estados_semaforo
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT FECHA_RESPUETA" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_semaforo_respuesta_solo_confirmacion = "Función Retorna_id_imagen_gabinete_resp_radicado dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0

            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estados_semaforo_respuesta_solo_confirmacion = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    estado_final = 2
                Else
                    estado_final = 0
                End If
                url_image = "../radicador/imagenes/resp_solo_elctronica_conf_estado_V" & estado_final & ".png"
                Retorna_estados_semaforo_respuesta_solo_confirmacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_semaforo_respuesta_solo_confirmacion = "Inconsistencia general función Retorna_estados_semaforo_respuesta_solo_confirmacion " & ex.Message
        End Try
    End Function

    Function Retorna_estados_semaforo_respuesta_solo_confirmacion(ByVal id_respuesta As Integer,
                                                                  ByRef imag As Image) As String
        '*************************************************************************
        'Función : Retorna estados_semaforo
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT FECHA_RESPUETA" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_semaforo_respuesta_solo_confirmacion = "Función Retorna_id_imagen_gabinete_resp_radicado dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0

            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estados_semaforo_respuesta_solo_confirmacion = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    estado_final = 2
                Else
                    estado_final = 0
                End If
                imag.ImageUrl = "../radicador/imagenes/resp_solo_elctronica_conf_estado_V" & estado_final & ".png"
                Retorna_estados_semaforo_respuesta_solo_confirmacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_semaforo_respuesta_solo_confirmacion = "Inconsistencia general función Retorna_estados_semaforo_respuesta_solo_confirmacion " & ex.Message
        End Try
    End Function
    Function Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(ByVal id_respuesta As Integer,
                                                                              ByRef url_image As String) As String
        '*************************************************************************
        'Función : Retorna estados_semaforo
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT FECHA_RESPUETA,estado_envio_correo" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica = "Función Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0

            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).Item(1) = 1 And Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    estado_final = 2
                End If
                If Datset.Tables(0).Rows(0).IsNull(0) = False And Datset.Tables(0).Rows(0).Item(1) <> 1 Then
                    estado_final = 1
                End If
                If Datset.Tables(0).Rows(0).IsNull(0) = True And Datset.Tables(0).Rows(0).Item(1) = 0 Then
                    estado_final = 0
                End If
                url_image = "../radicador/imagenes/resp_solo_elctronica_conf_estado_V" & estado_final & ".png"
                Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica = "Inconsistencia general función Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica " & ex.Message
        End Try
    End Function
    Function Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(ByVal id_respuesta As Integer,
                                                                              ByRef imag As Image) As String
        '*************************************************************************
        'Función : Retorna estados_semaforo
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT FECHA_RESPUETA,estado_envio_correo" &
               " FROM ra_respuesta_radicado " &
              " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica = "Función Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica dice " & Result
                Exit Function
            End If
            Dim estado_final As Integer = 0

            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).Item(1) = 1 And Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    estado_final = 2
                End If
                If Datset.Tables(0).Rows(0).IsNull(0) = False And Datset.Tables(0).Rows(0).Item(1) <> 1 Then
                    estado_final = 1
                End If
                If Datset.Tables(0).Rows(0).IsNull(0) = True And Datset.Tables(0).Rows(0).Item(1) = 0 Then
                    estado_final = 0
                End If
                imag.ImageUrl = "../radicador/imagenes/resp_solo_elctronica_conf_estado_V" & estado_final & ".png"
                Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica = "Inconsistencia general función Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica " & ex.Message
        End Try
    End Function
    Function Retorna_estados_respuesta(ByVal id_respuesta As Integer,
                                       ByRef estado_respuesta As Integer,
                                       ByRef estado_plantilla_respuesta As Integer,
                                       ByRef estado_radicado As Integer,
                                       ByRef estado_envio_correo As Integer,
                                       ByRef estado_doc_respuesta As Integer) As String
        '*************************************************************************
        'Función : Retorna 
        'con el paramentro id_respeusta
        'Ing : Miguel Angel Urueta Miranda
        '*************************************************************************
        Try
            Dim Parametro_Consulta As String = "SELECT FECHA_RESPUETA,ID_IMAGEN,RADICADO_RESPUESTA,estado_envio_correo,ID_IMAGEN_RESPUESTA" &
                   " FROM ra_respuesta_radicado " &
                  " where ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estados_respuesta = "Función Retorna_etados_respuesta_electronica dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estados_respuesta = "Imposible encontrar detalles del estado  de la respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    estado_respuesta = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    estado_respuesta = 0

                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                    estado_plantilla_respuesta = 0
                Else
                    estado_plantilla_respuesta = 1
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = False Then
                    estado_envio_correo = 0
                Else
                    estado_envio_correo = 1
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = False Then
                    estado_doc_respuesta = 0
                Else
                    estado_doc_respuesta = 1

                End If
                Retorna_estados_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estados_respuesta = "Inconsistencia general función Retorna_estados_respuesta " & ex.Message
        End Try
    End Function

    Function Retorna_estado_envio_por_id_respuesta(ByVal id_respuesta As Integer,
                                                   ByRef estado_envio_respuesta As Integer) As String
        Try
            Dim Result As String = ""
            Dim nombre_plantilla_radicado As String = ""
            Dim radicado As String = ""
            Result = Me.Retorna_nombre_plantilla_por_id_respuesta(id_respuesta, nombre_plantilla_radicado, radicado)
            If Result <> "YES" Then
                Retorna_estado_envio_por_id_respuesta = Result
                Exit Function
            End If
            Result = Me.Retorna_estado_envio_respuesta(nombre_plantilla_radicado,
                                                       radicado,
                                                       estado_envio_respuesta)
            If Result <> "YES" Then
                Retorna_estado_envio_por_id_respuesta = Result
                Exit Function
            End If
            Retorna_estado_envio_por_id_respuesta = "YES"
        Catch ex As Exception
            Retorna_estado_envio_por_id_respuesta = "Inconsistencia general función Retorna_estado_envio_por_id_respuesta " & ex.Message
        End Try
    End Function
    Function Retorna_estado_envio_respuesta(ByVal nombre_plantilla As String,
                                            ByVal radicado As String,
                                            ByRef estado_envio_respuesta As Integer) As String

        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "SELECT tde.resp_correo_fisico_electronico " &
                    " FROM " & nombre_plantilla & " as rrr " &
                    "inner join tipo_doc_entrante as tde on (tde.id_Tipo_Doc_Entrante=rrr.tipo_doc_entrante_id_tipo_doc_entrante) " &
                     " where  Consecutivo_Rad='" & radicado & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_plantilla)
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_envio_respuesta = "Función Retorna_estado_envio_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_estado_envio_respuesta = "Imposible econtrar el estado del tipo documento radicado  " & radicado & " de la plantilla " & nombre_plantilla
                Exit Function
            Else
                estado_envio_respuesta = Datset.Tables(0).Rows(0).Item(0)
                Retorna_estado_envio_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estado_envio_respuesta = "Inconsistencia general función estado_envio_respuesta " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_plantilla_por_id_respuesta(ByVal id_respuesta As Integer,
                                                       ByRef nombre_plantilla As String,
                                                       ByRef radicado As String) As String

        Try

            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_plantilla_radicado_gabinete")
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "SELECT spr.Nombre_Plantilla_Radicado,rrr.RADICADO" &
                    " FROM ra_respuesta_radicado as rrr " &
                    "inner join system_plantilla_radicado as spr on (spr.id_Plantilla=rrr.system_plantilla_radicado_id_plantilla) " &
                     " where  ID_RESPUESTA_RADICADO=" & id_respuesta
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_plantilla_por_id_respuesta = "Función Retorna_nombre_plantilla_por_id_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_plantilla_por_id_respuesta = "Imposible econtrar el nombre de la plantilla "
                Exit Function
            Else
                nombre_plantilla = Datset.Tables(0).Rows(0).Item(0)
                radicado = Datset.Tables(0).Rows(0).Item(1)
                Retorna_nombre_plantilla_por_id_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_plantilla_por_id_respuesta = "Inconsistencia general función Retorna_nombre_plantilla_por_id_respuesta " & ex.Message
        End Try
    End Function
    Function Retorna_gabinete_default_plantilla_radicado(ByVal id_plantilla_radicado As Integer,
                                                         ByRef nombre_gabinete As String,
                                                         ByVal nombre_campo_default As String) As String

        Try

            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_plantilla_radicado_gabinete")
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "SELECT NOMBRE,CAMPO_BUSQUEDA" &
                    " FROM ra_relacion_plantilla_radicado_gabinete " &
                     " where  id_plantilla='" & id_plantilla_radicado & "' and PREDETERMINADO=1"
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_gabinete_default_plantilla_radicado = "Función Retorna_gabinete_default_plantilla_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_gabinete_default_plantilla_radicado = "Imposible econtrar el gabiente predeterminado para guardar el documento"
                Exit Function
            Else
                nombre_gabinete = Datset.Tables(0).Rows(0).Item(0)
                nombre_campo_default = Datset.Tables(0).Rows(0).Item(1)
                Retorna_gabinete_default_plantilla_radicado = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_gabinete_default_plantilla_radicado = "Inconsistencia general función Retorna_gabinete_default_plantilla_radicado " & ex.Message
        End Try
    End Function
    Function Retorna_parametros_almacenamiento_documento_permanente_almacenado(
     ByVal id_registro_respuesta As Integer, ByRef matri_datos_almacen() As String,
     ByRef matri_gestion As estructure_gestion, ByRef matri_documentos() As String,
     ByRef nombre_gabinete As String, ByVal ruta_tempo As String, ByVal ruta_documento_alamcen As String) As String
        Try
            Dim Result As String = ""
            Dim ob As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_registro_respuesta, ob)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            Dim id_imagen_seleccionada As Integer = 0
            Dim refclas_workflow As New ClassWorkflow
            Result = refclas_workflow.Retorna_id_imagen_seleccionada(ob.ID_TAREA_WF, ob.ID_RUTA_WF, id_imagen_seleccionada)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            If ob.ID_TAREA_WF = 0 Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = "La respuesta no tiene id tarea workflow asignada"
                Exit Function
            End If
            '----------------------------------------------------------------
            'Retorna plantilla radicado default respuesta
            '----------------------------------------------------------------
            Dim id_plantilla As Integer = 0
            Dim nombre_plantilla As String = ""
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            '----------------------------------------------------------------------------
            'Retorna datos del gabinete default para almacenar el documento saliente
            '----------------------------------------------------------------------------
            Dim nombre_gabinete_respuesta As String = ""
            Dim campo_default_respuesta As String = ""
            Result = Retorna_gabinete_default_plantilla_radicado(id_plantilla, nombre_gabinete_respuesta, campo_default_respuesta)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            nombre_gabinete = nombre_gabinete_respuesta
            '-----------------------------------------------------------------------------
            'Retorna la matriz de documento plantilla a guardar en formto tif
            '-----------------------------------------------------------------------------
            Dim ref_almacenaminento As New ClassVisualisaDocumento
            Dim mtri_documento() As String = Nothing
            Result = ref_almacenaminento.Genera_Matris_Documentos_Almacenados(ob.ID_IMAGEN, "IMP03GESTIONTMP", mtri_documento)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            If File.Exists(mtri_documento(1)) = False Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = "Por favor genere el documento plantilla para el radicado "
                Exit Function
            End If
            Dim Ruta_documento_respuesta As String = ""
            Dim Reflclas_gestion As New ClassGaGembox
            Result = Reflclas_gestion.Solicita_formato_respuesta_con_Footers(id_registro_respuesta, mtri_documento(1), Ruta_documento_respuesta)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            Dim ruta_descarga As String = ""
            'Dim OBSER As New localhost.Service
            'OBSER.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
            Dim fil_estrean As Object = Nothing
            Erase matri_documentos
            Dim Matri_documento_ob() As Object = Nothing
            'Result = OBSER.Convertir_documento_ruta_docx_formatos_aplication_web_met(Ruta_documento_respuesta, Matri_documento_ob, "", "TIF", 1)
            'If Result <> "YES" Then
            '    Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
            '    Exit Function
            'End If
            For i As Integer = 0 To Matri_documento_ob.Length - 1
                ReDim Preserve matri_documentos(i)
                matri_documentos(i) = Matri_documento_ob(i)
            Next
            Dim refclas_gest_resp As New ClassRaEnvioCorrespondencia
            '----------------------------------------------------------------------------
            'Retorna datos del gabinete default para almacenar el documento saliente
            '----------------------------------------------------------------------------
            Dim nombre_gabinete_radicado As String = ""
            Dim campo_default_radicado As String = ""
            Result = Retorna_gabinete_default_plantilla_radicado(ob.system_plantilla_radicado_id_plantilla, nombre_gabinete_radicado, campo_default_radicado)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete respuesta
            '--------------------------------------------------------------
            Dim Refclasalmacenamiento As New ClassAlmacenamiento
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete_respuesta, estructura_gabinete)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            For i As Integer = 0 To estructura_gabinete.Length - 1
                estructura_gabinete(i).VALORCAMPO = ""
            Next
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "NUMERORADICA" Then
                    estructura_gabinete(i).VALORCAMPO = ob.RADICADO_RESPUESTA
                End If
                If estructura_gabinete(i).CAMPO = "ENLASE" Then
                    estructura_gabinete(i).VALORCAMPO = ob.RADICADO
                End If
                If estructura_gabinete(i).CAMPO = "DESTINATARIO" Then
                    estructura_gabinete(i).VALORCAMPO = ob.DESTINATARIO
                End If
                If estructura_gabinete(i).CAMPO = "REMITENTE" Then
                    estructura_gabinete(i).VALORCAMPO = ob.USUARIO_RESPONSABLE
                End If
                If estructura_gabinete(i).CAMPO = "DESCRIPCIONDOCU" Then
                    estructura_gabinete(i).VALORCAMPO = ob.TRAMITE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "ASUNTO" Then
                    estructura_gabinete(i).VALORCAMPO = ob.ASUNTO
                End If
                If estructura_gabinete(i).CAMPO = "TIPORADICADO" Then
                    estructura_gabinete(i).VALORCAMPO = "SALIENTE"
                End If
            Next
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            matri_gestion.CLASE_DOCUMENTO = ""
            matri_gestion.EXPEDIENTE = ""
            matri_gestion.ID_AREA = 0
            matri_gestion.ID_CLASE_DOCUMENTO = 0
            matri_gestion.ID_EXPEDIENTE = 0
            matri_gestion.ID_SERIE = 0
            matri_gestion.ID_SUB_SERIE = 0
            matri_gestion.ID_TIPO_EXPEDIENTE = 0
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = 0
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = ""
            Dim reflclasalalma As New ClassAlmacenamiento
            Dim ClassDaGabinete As New ClassDaGabinete
            If id_imagen_seleccionada <> 0 Then
                Result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(id_imagen_seleccionada,
                                                                                     nombre_gabinete_radicado,
                                                                                     matri_gestion)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_gestion_estructura_base_datos(matri_gestion, nombre_gabinete_radicado, id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_tipo_documental_estructura_base_datos(matri_gestion, nombre_gabinete_radicado, id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_unidad_conservacion_estructura_base_datos(matri_gestion, nombre_gabinete_radicado, id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Retorna el id tipo documento
            '------------------------------------------
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim id_clase_documento As Integer = 0
            Dim Class_ra_tipo_documento As New Class_ra_tipo_documento
            Result = Class_ra_tipo_documento.Solicita_id_clase_documento(clase_documento,
                                                                         id_clase_documento)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = Result
                Exit Function
            End If
            matri_gestion.CLASE_DOCUMENTO = clase_documento
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente_almacenado = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            matri_gestion.FECHA_ELABORACION = date1al
            '---------------------------------------------
            'Asigna los datos de gestion a la estructura
            '---------------------------------------------
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "FECHAELABORACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.FECHA_ELABORACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_AREA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_AREA
                End If
                If estructura_gabinete(i).CAMPO = "ID_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_SUB_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPODOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "ID_USUARIO_GESTION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_USUARIO_GESTION
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESUBSERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_DOCUMENTAL" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_CLASE_DOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "UNIDADCONSERVA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "CLASEDOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.TIPODOCUMENTO
                End If
            Next
            Dim i2 As Integer = 0
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).VISIBLE = 1 Then
                    ReDim Preserve matri_datos_almacen(i2)
                    matri_datos_almacen(i2) = estructura_gabinete(i).VALORCAMPO
                    i2 = i2 + 1
                End If
            Next
            Retorna_parametros_almacenamiento_documento_permanente_almacenado = "YES"
        Catch ex As Exception
            Retorna_parametros_almacenamiento_documento_permanente_almacenado = "Inconsistencia general función Retorna_parametros_almacenamiento_documento_permanente_almacenado " & ex.Message
        End Try
    End Function

    Function Retorna_parametros_almacenamiento_documento_permanente(
     ByVal id_registro_respuesta As Integer, ByRef matri_datos_almacen() As String,
     ByRef matri_gestion As estructure_gestion, ByRef matri_documentos() As String,
     ByRef nombre_gabinete As String, ByVal ruta_tempo As String, ByVal ruta_documento_alamcen As String) As String
        Try
            Dim Result As String = ""
            Dim ob As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_registro_respuesta, ob)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = Result
                Exit Function
            End If
            Dim Refeclasaladir As New ClassAñadirDocumento
            Dim MatriDatosAlmacen() As String
            Erase MatriDatosAlmacen
            Dim Ruta_doc As String = ""
            Ruta_doc = ruta_tempo & "DOC-LIBRE" & id_registro_respuesta & ".docx"
            If File.Exists(ruta_documento_alamcen) = False Then
                Retorna_parametros_almacenamiento_documento_permanente = "Por favor genere el documento de respuesta para el radicado "
                Exit Function
            End If
            Dim ruta_descarga As String = ""
            'Dim OBSER As New localhost.Service
            'OBSER.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
            Dim fil_estrean As Object = Nothing
            'Result = OBSER.Descarga_doxc_formatos_archivo_web_application(ruta_documento_alamcen, fil_estrean, "TIF", 1)
            'If Result <> "YES" Then
            '    Retorna_parametros_almacenamiento_documento_permanente = Result
            '    Exit Function
            'End If
            '--------------------------------------------
            'Detecta los documentos con multitif
            '--------------------------------------------
            Dim Refclas2 As New ClassNeodynamic
            Dim Mat_Incre As Integer = 0
            Dim ob_ref As Object = Nothing
            Erase matri_documentos
            Result = Refclas2.Extraer_Documento_de_Multitif(ob_ref, fil_estrean, matri_documentos, ruta_tempo)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = Result
                Exit Function
            End If
            '----------------------------------------------------------------
            'Retorna plantilla radicado default respuesta
            '----------------------------------------------------------------
            Dim id_plantilla As Integer = 0
            Dim nombre_plantilla As String = ""
            Dim ref_calss_system As New Class_system_plantilla_radicado
            Result = ref_calss_system.Solicita_plantilla_default_respuesta(id_plantilla, nombre_plantilla)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = Result
                Exit Function
            End If
            '----------------------------------------------------------------------------
            'Retorna datos del gabinete default para almacenar el documento saliente
            '----------------------------------------------------------------------------
            Dim nombre_gabinete_respuesta As String = ""
            Dim campo_default_respuesta As String = ""
            Result = Retorna_gabinete_default_plantilla_radicado(id_plantilla, nombre_gabinete_respuesta, campo_default_respuesta)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = Result
                Exit Function
            End If
            nombre_gabinete = nombre_gabinete_respuesta
            '----------------------------------------------------------------------------
            'Retorna datos del gabinete default para almacenar el documento saliente
            '----------------------------------------------------------------------------
            Dim nombre_gabinete_radicado As String = ""
            Dim campo_default_radicado As String = ""
            Result = Retorna_gabinete_default_plantilla_radicado(ob.system_plantilla_radicado_id_plantilla, nombre_gabinete_radicado, campo_default_radicado)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = Result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Retorna datos del a estructura del gabinete respuesta
            '--------------------------------------------------------------
            Dim Refclasalmacenamiento As New ClassAlmacenamiento
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete_respuesta,
                                                                                 estructura_gabinete)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = Result
                Exit Function
            End If
            For i As Integer = 0 To estructura_gabinete.Length - 1
                estructura_gabinete(i).VALORCAMPO = ""
            Next
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "NUMERORADICA" Then
                    estructura_gabinete(i).VALORCAMPO = ob.RADICADO_RESPUESTA
                End If
                If estructura_gabinete(i).CAMPO = "ENLASE" Then
                    estructura_gabinete(i).VALORCAMPO = ob.RADICADO
                End If
                If estructura_gabinete(i).CAMPO = "DESTINATARIO" Then
                    estructura_gabinete(i).VALORCAMPO = ob.DESTINATARIO
                End If
                If estructura_gabinete(i).CAMPO = "REMITENTE" Then
                    estructura_gabinete(i).VALORCAMPO = ob.USUARIO_RESPONSABLE
                End If
                If estructura_gabinete(i).CAMPO = "DESCRIPCIONDOCU" Then
                    estructura_gabinete(i).VALORCAMPO = ob.TRAMITE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "ASUNTO" Then
                    estructura_gabinete(i).VALORCAMPO = ob.ASUNTO
                End If
                If estructura_gabinete(i).CAMPO = "TIPORADICADO" Then
                    estructura_gabinete(i).VALORCAMPO = "SALIENTE"
                End If
            Next

            Dim id_imagen_seleccionada As Integer = 0
            Dim refclas_workflow As New ClassWorkflow
            Result = refclas_workflow.Retorna_id_imagen_seleccionada(ob.ID_TAREA_WF, ob.ID_RUTA_WF, id_imagen_seleccionada)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = Result
                Exit Function
            End If
            'If id_imagen_seleccionada = 0 Then
            '    Retorna_parametros_almacenamiento_documento_permanente = "Impsoible encontrar la imagen del documentos seleccionado, para copiar los datos de gestión "
            '    Exit Function
            'End If

            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            matri_gestion.CLASE_DOCUMENTO = ""
            matri_gestion.EXPEDIENTE = ""
            matri_gestion.ID_AREA = 0
            matri_gestion.ID_CLASE_DOCUMENTO = 0
            matri_gestion.ID_EXPEDIENTE = 0
            matri_gestion.ID_SERIE = 0
            matri_gestion.ID_SUB_SERIE = 0
            matri_gestion.ID_TIPO_EXPEDIENTE = 0
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = 0
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = ""
            Dim reflclasalalma As New ClassAlmacenamiento
            Dim ClassDaGabinete As New ClassDaGabinete
            If id_imagen_seleccionada <> 0 Then
                Result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(id_imagen_seleccionada, nombre_gabinete_radicado, matri_gestion)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_permanente = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_gestion_estructura_base_datos(matri_gestion, nombre_gabinete_radicado, id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_permanente = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_tipo_documental_estructura_base_datos(matri_gestion, nombre_gabinete_radicado, id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_permanente = Result
                    Exit Function
                End If
                Result = reflclasalalma.Solicita_datos_unidad_conservacion_estructura_base_datos(matri_gestion, nombre_gabinete_radicado, id_imagen_seleccionada)
                If Result <> "YES" Then
                    Retorna_parametros_almacenamiento_documento_permanente = Result
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Retorna el id tipo documento
            '------------------------------------------
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim id_clase_documento As Integer = 0
            Dim Class_ra_tipo_documento As New Class_ra_tipo_documento
            Result = Class_ra_tipo_documento.Solicita_id_clase_documento(clase_documento,
                                                                         id_clase_documento)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = Result
                Exit Function
            End If
            matri_gestion.CLASE_DOCUMENTO = clase_documento
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Retorna_parametros_almacenamiento_documento_permanente = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            matri_gestion.FECHA_ELABORACION = date1al
            '---------------------------------------------
            'Asigna los datos de gestion a la estructura
            '---------------------------------------------
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).CAMPO = "FECHAELABORACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.FECHA_ELABORACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_AREA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_AREA
                End If
                If estructura_gabinete(i).CAMPO = "ID_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_SUB_SERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPODOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "ID_USUARIO_GESTION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_USUARIO_GESTION
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "NOMBRESUBSERIE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.NOMBRE_SUB_SERIE
                End If
                If estructura_gabinete(i).CAMPO = "ID_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_TIPO_EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "ID_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_CONSERVACION" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_TIPO_UNIDAD_DOCUMENTAL" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "ID_CLASE_DOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.ID_CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "EXPEDIENTE" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.EXPEDIENTE
                End If
                If estructura_gabinete(i).CAMPO = "UNIDADCONSERVA" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.UNIDAD_CONSERVACION
                End If
                If estructura_gabinete(i).CAMPO = "CLASEDOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.CLASE_DOCUMENTO
                End If
                If estructura_gabinete(i).CAMPO = "TIPODOCUMENTO" Then
                    estructura_gabinete(i).VALORCAMPO = matri_gestion.TIPODOCUMENTO
                End If
            Next
            Dim i2 As Integer = 0
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If estructura_gabinete(i).VISIBLE = 1 Then
                    ReDim Preserve matri_datos_almacen(i2)
                    matri_datos_almacen(i2) = estructura_gabinete(i).VALORCAMPO
                    i2 = i2 + 1
                End If
            Next
            Retorna_parametros_almacenamiento_documento_permanente = "YES"
        Catch ex As Exception
            Retorna_parametros_almacenamiento_documento_permanente = "Inconsistencia general función Retorna_parametros_almacenamiento_documento_permanente " & ex.Message
        End Try
    End Function


    Function Lista_nombre_documentos_anexos_respuesta_droplist(ByVal ruta_documentos As String, ByRef drowplist As DropDownList) As String
        Try
            drowplist.Items.Clear()
            For Each Archivo As String In My.Computer.FileSystem.GetFiles(
                                       ruta_documentos,
                                        FileIO.SearchOption.SearchTopLevelOnly,
                                       "*.*")
                Dim fileas As New FileInfo(Archivo)
                drowplist.Items.Add(fileas.Name)
            Next
            Lista_nombre_documentos_anexos_respuesta_droplist = "YES"
        Catch ex As Exception
            Lista_nombre_documentos_anexos_respuesta_droplist = "Inconsistencia general función Lista_nombre_documentos_anexos_respuesta_droplist " & ex.Message
        End Try
    End Function
    Function Lista_ruta_documentos_anexos_respuesta(ByVal ruta_documentos As String,
                                                    ByRef matri_documentos() As String) As String
        Try
            Dim i As Integer = 0
            Erase matri_documentos
            For Each Archivo As String In My.Computer.FileSystem.GetFiles(
                                       ruta_documentos,
                                        FileIO.SearchOption.SearchTopLevelOnly,
                                       "*.*")
                Dim fileas As New FileInfo(Archivo)
                ReDim Preserve matri_documentos(i)
                matri_documentos(i) = fileas.FullName
                i = i + 1
            Next
            Lista_ruta_documentos_anexos_respuesta = "YES"
        Catch ex As Exception
            Lista_ruta_documentos_anexos_respuesta = "Inconsistencia general función Lista_nombre_documentos_anexos_respuesta_droplist " & ex.Message
        End Try
    End Function
    Function Guardar_Documento_Respuesta(ByRef Id_imagen As Integer,
                                         ByVal Nombre_Gabinete As String,
                                         ByVal id_registro_respuesta As Integer,
                                         ByVal ruta_document As String,
                                         ByVal id_tipo_respuesta As Integer) As String
        Try
            Dim Refeclasaladir As New ClassAñadirDocumento
            Dim MatriDatosAlmacen() As String
            Erase MatriDatosAlmacen
            Dim Result As String = ""
            Dim Refalmacena As New ClassAlmacenamiento
            Dim option_unidad_conservacion As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(option_unidad_conservacion,
                                                                       Nombre_Gabinete)
            If Result <> "YES" Then
                Guardar_Documento_Respuesta = "Inconsistencia verficando opcón asignación unidad y expediente codigo:  " & Result
                Exit Function
            End If
            Dim matri_datos() As Datos_Almacenamiento
            ReDim Preserve matri_datos(0)
            matri_datos(0).nombre_campo = "NUMERORADICA"
            matri_datos(0).valor_campo = ""
            ReDim Preserve matri_datos(1)
            matri_datos(1).nombre_campo = "ENLASE"
            matri_datos(1).valor_campo = ""
            ReDim Preserve matri_datos(2)
            matri_datos(2).nombre_campo = "IDREGISTRORESP"
            matri_datos(2).valor_campo = id_registro_respuesta
            If option_unidad_conservacion = 1 Then
                ReDim Preserve matri_datos(3)
                matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
                matri_datos(3).valor_campo = "DOCUMENTO DIGITALIZADO"
                Dim date1al As String = Date.Today
                Result = ""
                Dim ref_ClassGestionFechas As New ClassGestionFechas
                Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
                If Result <> "YES" Then
                    Guardar_Documento_Respuesta = "Error formatenado fecha alamcenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                    Exit Function
                End If
                ReDim Preserve matri_datos(3)
                matri_datos(3).nombre_campo = "CLASEDOCUMENTO"
                matri_datos(3).valor_campo = "DOCUMENTO DIGITALIZADO"
            End If
            Dim RefclasAñadir As New ClassAñadirDocumento
            Dim Refclaswfdigtializado As New ClassWorkflowDigitalizacion
            Dim Matri_Datos_Almacen() As String
            Erase Matri_Datos_Almacen
            Result = Refclaswfdigtializado.Obtiene_Valores_Campos_Documento_Enlazados(Matri_Datos_Almacen, Nombre_Gabinete, matri_datos)
            If Result <> "YES" Then
                Guardar_Documento_Respuesta = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacen Is Nothing Then
                Guardar_Documento_Respuesta = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            Dim Filein As New IO.FileInfo(ruta_document)
            Result = ""
            Dim Tipo_Doc_int As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension),
                                                                          Tipo_Doc_int)
            If Result <> "YES" Then
                Guardar_Documento_Respuesta = Result
                Exit Function
            End If
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            Dim matri_gestion As estructure_gestion = Nothing
            'matri_gestion = Nothing
            matri_gestion.CLASE_DOCUMENTO = ""
            matri_gestion.EXPEDIENTE = ""
            matri_gestion.ID_AREA = 0
            matri_gestion.ID_CLASE_DOCUMENTO = 0
            matri_gestion.ID_EXPEDIENTE = 0
            matri_gestion.ID_SERIE = 0
            matri_gestion.ID_SUB_SERIE = 0
            matri_gestion.ID_TIPO_EXPEDIENTE = 0
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = 0
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = 0
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = ""
            Id_imagen = Tipo_Doc_int
            Dim radicado As String = ""
            Dim matri_documento() As String = {ruta_document}
            Result = Refalmacena.Almacenamiento("", "", Nombre_Gabinete, 0, Matri_Datos_Almacen,
            2, 1, Tipo_Doc_int, matri_documento, 0, Id_imagen, Tipo_Doc_int, HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado)
            If Result <> "YES" Then
                Guardar_Documento_Respuesta = "Almacenando  dice " & Result
                Exit Function
            End If
            '----------------------------------------------------------
            'Actualiza el estado del codigo del documento docuarchi
            '----------------------------------------------------------
            Dim SQL As String = "Update ra_respuesta_radicado set ID_IMAGEN=" & Id_imagen &
                ",ID_TIPO_DOC_RESPUESTA=" & id_tipo_respuesta &
            " where ID_RESPUESTA_RADICADO=" & id_registro_respuesta
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(SQL)
            If Result <> "YES" Then
                Guardar_Documento_Respuesta = "Inconsistencia actualizando id documento" & Result
                Exit Function
            End If
            Guardar_Documento_Respuesta = "YES"
            Exit Function
        Catch ex As Exception
            Guardar_Documento_Respuesta = "Inconsistencia función Guardar_Documento_Respuesta " & ex.Message
        End Try
    End Function
    Function Lista_documento_respuesta_drowlis(ByVal id_respuesta As Integer, ByRef drowlis As DropDownList) As String
        '************************************************************
        'Function : Lista documento de respuesta
        'Fecha : 2016-06-29
        'Ing : Migeuel angel urueta Miranda
        '************************************************************
        Try
            drowlis.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select ID_IMAGEN " &
               " FROM  ra_respuesta_radicado  " &
               " where   ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_documento_respuesta_drowlis = "Función Lista_documento_respuesta_drowlis dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_documento_respuesta_drowlis = "Imposible encontrar el registro de respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    drowlis.Items.Clear()
                Else
                    drowlis.Items.Add(id_respuesta)
                End If
                Lista_documento_respuesta_drowlis = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_documento_respuesta_drowlis = "Inconsistencia general función Lista_documento_respuesta_drowlis " & ex.Message
        End Try
    End Function

    Function Verifica_existencia_documento_de_respuesta(ByVal id_respuesta As Integer,
                                                        ByRef estado_documento As String) As String
        '************************************************************
        'Function : Verifica existencia documento respuesta
        'Fecha : 2016-06-28
        'Ing : Migeuel angel urueta Miranda
        '************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select ID_IMAGEN " &
               " FROM  ra_respuesta_radicado  " &
               " where   ID_RESPUESTA_RADICADO=" & id_respuesta
            Dim Datset As DataSet = New DataSet("ra_respuesta_radicado")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_documento_de_respuesta = "Función Verifica_existencia_documento_de_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_existencia_documento_de_respuesta = "Imposible encontrar el registro de respuesta"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estado_documento = "NO"
                Else
                    estado_documento = "YES"
                End If
                Verifica_existencia_documento_de_respuesta = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Verifica_existencia_documento_de_respuesta = "Inconsistencia general función Verifica_existencia_documento_de_respuesta " & ex.Message
        End Try
    End Function
    Function Verifica_respuesta_radicado_sin_respuesta(ByVal id_usuario_gestion As Integer,
                                                       ByVal id_tarea_seleccionada As Integer) As String
        Try
            '----------------------------------
            'Si el usuario no tiene asociado
            'usuario de gestion permite seguir
            'trabajando
            '----------------------------------
            If id_usuario_gestion = 0 Then
                Verifica_respuesta_radicado_sin_respuesta = "YES"
                Exit Function
            End If
            '************************************
            'Retorna radicado tarea seleccionada
            '************************************
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_seleccionada,
                                                                                 Radicado)
            If Result <> "YES" Then
                Verifica_respuesta_radicado_sin_respuesta = Result
                Exit Function
            End If
            If Radicado = "" Then
                Verifica_respuesta_radicado_sin_respuesta = "La tarea seleccionada no tiene radicado seleccionado "
                Exit Function
            End If
            Dim Refclas_resp As New Classgestionrespuesta
            '-----------------------------------------------------
            'Verifica la existencia de la respuesta del radicado
            '-----------------------------------------------------
            'Dim estado_respuesta As String = "NO"
            'Result = Refclas_resp.Verfica_radicado_con_respuesta(id_usuario_gestion, Radicado, estado_respuesta)
            'If Result <> "YES" Then
            '    Verifica_respuesta_radicado_sin_respuesta = Result
            '    Exit Function
            'End If
            'If estado_respuesta = "YES" Then
            '    Verifica_respuesta_radicado_sin_respuesta = "Por favor confirme la respuesta del radicado " & Radicado
            '    Exit Function
            'End If
            '---------------------------------------------------
            'Retorna la respuesta relacionada al radicado
            '----------------------------------------------------
            Dim id_respuesta_radicado As Integer = 0
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                               id_usuario_gestion,
                                                                               id_respuesta_radicado)
            If Result <> "YES" Then
                Verifica_respuesta_radicado_sin_respuesta = Result
                Exit Function
            End If
            '-----------------------------------------------------------------------------------------------
            'El usuario no es el propietario de la respuesta, no evalua la obligatoriedad de la respuesta
            '-----------------------------------------------------------------------------------------------
            If id_respuesta_radicado = 0 Then
                Verifica_respuesta_radicado_sin_respuesta = "YES"
                Exit Function
            End If
            '-------------------------------------------------------
            'Retorna datos estructura respuesta
            '-------------------------------------------------------
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado, estru)
            If Result <> "YES" Then
                Verifica_respuesta_radicado_sin_respuesta = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Retorna nombre plantilla radicación
            '-----------------------------------------------------
            Dim nombre_plantilla As String = ""
            Result = Refclas_resp.Retorna_nombre_plantilla_por_id_respuesta(id_respuesta_radicado, nombre_plantilla, "")
            If Result <> "YES" Then
                Verifica_respuesta_radicado_sin_respuesta = Result
                Exit Function
            End If
            '----------------------------------------------------------------------------
            'Verfica tipo respuesta radicado solo confirmacion o respuesta obligatoria
            '----------------------------------------------------------------------------
            Dim estado_obligatorio As Integer = 0
            Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
            Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_tramite As Integer = 0
            Dim descripcion_tramite As String = ""
            Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla,
                                                                                                 Radicado,
                                                                                                 id_tipo_tramite,
                                                                                                 descripcion_tramite)
            If Result <> "YES" Then
                Verifica_respuesta_radicado_sin_respuesta = Result
                Exit Function
            End If
            Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                            estado_obligatorio)
            If Result <> "YES" Then
                Verifica_respuesta_radicado_sin_respuesta = Result
                Exit Function
            End If
            ''-------------------------------------------------------------------------
            ''Valida si el usuario tiene permitido la respuesta libre de confirmacón
            ''-------------------------------------------------------------------------
            'If HttpContext.Current.Session.Item("RESPUESTA_LIBRE") <> "0" Then
            '    '-----------------------------------------------------------------------
            '    'tiene un radicado sin fecha
            '    'de respuesta
            '    '-----------------------------------------------------------------------
            '    If estru.RADICADO_RESPUESTA <> "" And estru.FECHA_RESPUETA = "" Then
            '        Verifica_respuesta_radicado_sin_respuesta = "El trámite requiere de un radicado de respuesta"
            '        Exit Function
            '    End If
            '    If estru.FECHA_RESPUETA <> "" Then
            '        Verifica_respuesta_radicado_sin_respuesta = "YES"
            '        Exit Function
            '    Else
            '        Verifica_respuesta_radicado_sin_respuesta = "El trámite requiere de una confirmación de respuesta"
            '        Exit Function
            '    End If
            'End If
            '---------------------------------------------------------------------------
            'Tramite de respuesta con radicado saliente
            '---------------------------------------------------------------------------
            If estado_obligatorio = 1 Then
                If estru.RADICADO_RESPUESTA = "" Then
                    Verifica_respuesta_radicado_sin_respuesta = "El trámite requiere de un radicado de respuesta"
                    Exit Function
                End If
                '-----------------------------------------------------------------------
                'Verifica cuando el trámite es reversado
                '-----------------------------------------------------------------------
                If estru.RADICADO_RESPUESTA <> "" And estru.FECHA_RESPUETA = "" Then
                    Verifica_respuesta_radicado_sin_respuesta = "El trámite requiere de un radicado de respuesta"
                    Exit Function
                End If
                '--------------------------------------------------------------------------
                'Retorna estados de la respuesta
                '--------------------------------------------------------------------------
                'Dim estado_respuesta_doc As Integer = 0
                'Dim estado_plantilla_respuesta As Integer = 0
                'Dim estado_radicado As Integer = 0
                'Dim estado_envio_correo As Integer = 0
                'Dim estado_doc_respuesta As Integer = 0
                'Result = Me.Retorna_estados_respuesta(id_respuesta_radicado, estado_respuesta_doc, estado_plantilla_respuesta, estado_radicado, estado_envio_correo, estado_doc_respuesta)
                'If Result <> "YES" Then
                '    Verifica_respuesta_radicado_sin_respuesta = Result
                '    Exit Function
                'End If
                '------------------------------------------------------------------------------------------
                'Retorna estado tipo de envío para determinar si se cumplio todo el tramite de respuesta
                '------------------------------------------------------------------------------------------
                Dim tipo_respuesta_tramite As Integer = 0
                Result = Refclas_resp.Retorna_estado_envio_por_id_respuesta(id_respuesta_radicado, tipo_respuesta_tramite)
                If Result <> "YES" Then
                    Verifica_respuesta_radicado_sin_respuesta = Result
                    Exit Function
                End If
                '------------------------------------------------
                'Tipo envío fisico
                '------------------------------------------------
                If tipo_respuesta_tramite = 0 Then
                    If estru.FECHA_RESPUETA = "" And estru.RADICADO_RESPUESTA = "" Then
                        Verifica_respuesta_radicado_sin_respuesta = "El trámite no tiene una fecha de respuesta"
                        Exit Function
                    End If
                    If estru.ID_IMAGEN = 0 Then
                        Verifica_respuesta_radicado_sin_respuesta = "La respuesta carece del documento plantilla, debe guardar una plantilla en el menú respuesta documento"
                        Exit Function
                    End If
                    If estru.RADICADO_RESPUESTA = "" Then
                        Verifica_respuesta_radicado_sin_respuesta = "La respuesta carece de un radicado de respuesta, debe generar un radicado de respuesta en el menú respuesta documento"
                        Exit Function
                    End If

                End If
                '------------------------------------------
                'Tipo envío electrónico
                '------------------------------------------
                If tipo_respuesta_tramite = 1 Then
                    If estru.FECHA_RESPUETA = "" And estru.RADICADO_RESPUESTA = "" Then
                        Verifica_respuesta_radicado_sin_respuesta = "El trámite no tiene una fecha de respuesta"
                        Exit Function
                    End If
                    If estru.ID_IMAGEN = 0 Then
                        Verifica_respuesta_radicado_sin_respuesta = "La respuesta carece del documento plantilla, debe guardar una plantilla en el menú respuesta documento"
                        Exit Function
                    End If
                    If estru.RADICADO_RESPUESTA = "" Then
                        Verifica_respuesta_radicado_sin_respuesta = "La respuesta carece de un radicado de respuesta, debe generar un radicado de respuesta en el menú respuesta documento"
                        Exit Function
                    End If
                    If estru.estado_envio_correo = 0 Then
                        'Verifica_respuesta_radicado_sin_respuesta = "La respuesta carece de la confirmación al correo electrónico, debe notificar al correo desde el menú respuesta documento"
                        'Exit Function
                    End If
                    If estru.ID_IMAGEN_RESPUESTA = 0 And estru.RADICADO_RESPUESTA = "" Then
                        Verifica_respuesta_radicado_sin_respuesta = "La respuesta carece del documento respuesta electrónico, debe publicar el documento de respuesta"
                        Exit Function
                    End If
                End If
            End If
            '------------------------------------
            'Solo confirmacion
            '------------------------------------
            If estado_obligatorio = 0 Then
                If estru.FECHA_RESPUETA = "" Then
                    Verifica_respuesta_radicado_sin_respuesta = "El trámite requiere de una confirmación de respuesta"
                    Exit Function
                End If
                If estru.estado_envio_correo = 0 Then
                    'Verifica_respuesta_radicado_sin_respuesta = "La respuesta carece de la confirmación al correo electrónico, debe notificar al correo desde el menú respuesta documento"
                    'Exit Function
                End If
            End If
            Verifica_respuesta_radicado_sin_respuesta = "YES"
            Exit Function
        Catch ex As Exception
            Verifica_respuesta_radicado_sin_respuesta = "Inconsistencia general función Verifica_respuesta_radicado_sin_respuesta  " & ex.Message
        End Try
    End Function
    Function Retorna_id_usuario_gestion_plantilla_radicado(ByVal nombre_plantila As String,
                                                           ByVal id_radicado As Integer,
                                                           ByRef id_usuario_geston As Integer) As String
        '**********************************************************
        'Funcion : Retorna id usuario de gestión, con el paramentro 
        'identificador unico del radicado
        'Fecha 2015-07-27
        'Ing : Miguel Angel Urueta Miranda
        'Modificado para la web 2016-01-19
        '***********************************************************
        Try
            Dim Parametro_Consulta As String = "select Destinatario_Externo_id_Dest_Ext from   " & nombre_plantila &
            " where id_Radicado=" & id_radicado
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_registro_general_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_usuario_gestion_plantilla_radicado = "Función Retorna_id_usuario_gestion_plantilla_radicado dice error de conexión o consulta" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_usuario_gestion_plantilla_radicado = "Imposible encontrar en la plantilla " & nombre_plantila & " el siguiente id de radicado " & id_radicado
                Exit Function
            Else
                id_usuario_geston = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_usuario_gestion_plantilla_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_usuario_gestion_plantilla_radicado = "Inconsistencia función  Retorna_id_usuario_gestion_plantilla_radicado " & ex.Message
        End Try
    End Function
    Function Retorna_datos_general_radicado(ByVal radicado As String,
                                            ByRef nombre_plantilla As String,
                                            ByRef id_Radicado_plantilla As Integer) As String
        '**********************************************************
        'Funcion : Retorna datos general del radicados, extraido de 
        'la tabla de radicacion general
        'Fecha 2015-07-27
        'Ing : Miguel Angel Urueta Miranda
        '***********************************************************
        Try
            Dim Parametro_Consulta As String = "select id_Radicado_plantilla,Nombre_plantilla_radicado from  ra_registro_general_radicacion " &
            " where Consecutivo_Rad='" & radicado & "'"
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_registro_general_radicacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_general_radicado = "Función Retorna_datos_general_radicado dice error de conexión o consulta" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_datos_general_radicado = "Imposible encontrar el registro general del radicado " & radicado
                Exit Function
            Else
                id_Radicado_plantilla = Datset.Tables(0).Rows(0).Item(0)
                nombre_plantilla = Datset.Tables(0).Rows(0).Item(1)
                Retorna_datos_general_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_general_radicado = "Inconsistencia función  Retorna_datos_general_radicado " & ex.Message
        End Try
    End Function

    Function Retorna_log_radicado(ByVal page1 As Page,
                                  ByVal radicado As String) As String
        Try
            Dim Result As String = ""
            Dim Nombre_ruta As String = ""
            Dim scripma As GridView = page1.FindControl("GridView_val_radicacion")
            Dim labetitle As Label = page1.FindControl("titulo_label_val_radicacion")
            Dim updatelabel As UpdatePanel = page1.FindControl("UpdatePanelabel_val_radicacion")
            Dim updat As UpdatePanel = page1.Page.FindControl("UpdatePanel_conenido_grid_val_radicacion")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID_VAL")
            If scripma Is Nothing Then
                Retorna_log_radicado = "Imposible encontrar datagrid  " & "GridView_val_radicacion"
                Exit Function
            End If
            If labetitle Is Nothing Then
                Retorna_log_radicado = "Imposible encontrar el control  " & "titulo_label_val_radicacion"
                Exit Function
            End If
            If updatelabel Is Nothing Then
                Retorna_log_radicado = "Imposible encontrar el control  " & "UpdatePanelabel_val_radicacion"
                Exit Function
            End If

            Dim Sql_consulta As String = "Select desc_op as DESCRIPCION_OPERACION,USER_OPER AS USUARIO_TRANSACCION," &
                "HORA_REGISTRO,CAMPOS AS CAMPOS_TRANSACCION,IP_TRANS AS DIRECCION_TRANSACCION,MODULO_REGISTRO FROM ra_log_radicados " &
                " where CONSECUTIVO_RADICADO='" & radicado & "' ORDER BY id_tran"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("radicado")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_log_radicado = "Error listando datos funcion Retorna_log_respuesta_radicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de transacciones efectuados al radicado " & radicado
                scripma.DataSource = Datset
                hideselecion.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                Retorna_log_radicado = "YES"
                Exit Function
            Else
                labetitle.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) de transacciones efectuados al radicado " & radicado
                scripma.DataSource = Datset
                hideselecion.value = "-1"
                scripma.DataBind()
                updat.Update()
                updatelabel.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", i)
                    For z As Integer = 0 To scripma.Rows(i).Cells.Count - 1
                        scripma.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                        scripma.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                    Next
                Next
                Retorna_log_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_log_radicado = "Inconsistencia general función Retorna_log_radicado " & ex.Message
        End Try
    End Function
    Function Reorna_id_respuesta_radicado(ByVal radicado As String, ByRef id_respueta As Integer) As String
        Try
            id_respueta = -1
            Dim Parametro_Consulta As String = "Select ID_RESPUESTA_RADICADO" &
               " from ra_respuesta_radicado where RADICADO='" & radicado & "' limit 1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Reorna_id_respuesta_radicado = "Función Reorna_id_respuesta_radicado dice  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_respueta = Dat_reader.Tables(0).Rows(0).Item(0)
                Reorna_id_respuesta_radicado = "YES"
                Exit Function
            Else
                id_respueta = -1
                Reorna_id_respuesta_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Reorna_id_respuesta_radicado = "Inconsistencia general función Reorna_id_respuesta_radicado  " & ex.Message
        End Try
    End Function
    Function Actualiza_ruta_workflow_respuesta_radicado(ByVal id_ruta_workflow As Integer,
                                                        ByVal radicado As String) As String
        Try
            '-----------------------------------------------------------------------
            'Verifica existencia del id tarea y id ruta en el registro de respuesta
            '-----------------------------------------------------------------------
            Dim Result As String = ""
            Dim id_tarea_workflow_ra As Integer = -1
            Dim id_respuesta_radicado_ref As Integer = -1
            Result = Me.Reorna_id_tarea_workflow_radicado_respuesta(radicado, id_tarea_workflow_ra, id_respuesta_radicado_ref)
            If Result <> "YES" Then
                Actualiza_ruta_workflow_respuesta_radicado = Result
                Exit Function
            End If
            If id_tarea_workflow_ra = -1 Then
                '---------------------------------------
                'Busca id tarea relacionada al radicado
                '---------------------------------------
                Dim refclas As New Classselecciotarea
                Result = refclas.Retorna_id_tarea_seleccionada_radicado(radicado, id_ruta_workflow, id_tarea_workflow_ra)
                If Result <> "YES" Then
                    Actualiza_ruta_workflow_respuesta_radicado = Result
                    Exit Function
                End If
                Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
                Dim sql_update As String = "Update ra_respuesta_radicado set ID_RUTA_WF=" & id_ruta_workflow & ",ID_TAREA_WF=" & id_tarea_workflow_ra &
                    " where ID_RESPUESTA_RADICADO=" & id_respuesta_radicado_ref
                Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(sql_update)
                If Result <> "YES" Then
                    Actualiza_ruta_workflow_respuesta_radicado = Result
                    Exit Function
                Else
                    Actualiza_ruta_workflow_respuesta_radicado = "YES"
                    Exit Function
                End If

            End If
            Actualiza_ruta_workflow_respuesta_radicado = "YES"
            Exit Function
        Catch ex As Exception
            Actualiza_ruta_workflow_respuesta_radicado = "Inconsistencia general función Actualiza_ruta_workflow_respuesta_radicado " & ex.Message
        End Try
    End Function
    Function Reorna_id_tarea_workflow_radicado_respuesta(ByVal radicado_respuesta As String,
                                                         ByRef id_tarea_workflow As Integer, ByRef id_respuesta_radicado As Integer) As String
        Try
            id_tarea_workflow = -1
            Dim Parametro_Consulta As String = "Select ID_TAREA_WF,ID_RESPUESTA_RADICADO" &
               " from ra_respuesta_radicado where RADICADO='" & radicado_respuesta & "' limit 1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Reorna_id_tarea_workflow_radicado_respuesta = "Función Reorna_id_tarea_workflow_radicado_respuesta dice  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Reorna_id_tarea_workflow_radicado_respuesta = "Imposible encontrar el registro de respuesta del radicado"
                Exit Function
            Else
                If Dat_reader.Tables(0).Rows(0).IsNull(0) Then
                    id_tarea_workflow = -1
                    id_respuesta_radicado = Dat_reader.Tables(0).Rows(0).Item(1)
                Else
                    id_tarea_workflow = Dat_reader.Tables(0).Rows(0).Item(0)
                    id_respuesta_radicado = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                Reorna_id_tarea_workflow_radicado_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Reorna_id_tarea_workflow_radicado_respuesta = "Inconsistencia general función Reorna_id_respuesta_radicado_respuesta  " & ex.Message
        End Try
    End Function
    Function Reorna_id_respuesta_radicado_respuesta(ByVal radicado_respuesta As String, ByRef id_respueta As Integer) As String
        Try
            id_respueta = -1
            Dim Parametro_Consulta As String = "Select ID_RESPUESTA_RADICADO" &
               " from ra_respuesta_radicado where RADICADO_RESPUESTA='" & radicado_respuesta & "' limit 1"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Reorna_id_respuesta_radicado_respuesta = "Función Reorna_id_respuesta_radicado dice  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                id_respueta = Dat_reader.Tables(0).Rows(0).Item(0)
                Reorna_id_respuesta_radicado_respuesta = "YES"
                Exit Function
            Else
                id_respueta = -1
                Reorna_id_respuesta_radicado_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Reorna_id_respuesta_radicado_respuesta = "Inconsistencia general función Reorna_id_respuesta_radicado_respuesta  " & ex.Message
        End Try
    End Function
    Function Retorna_combo_respuesta_tramite(ByVal radicado As String, ByRef dropwlist As DropDownList) As String
        '----------------------------------------------------
        'Función : Retorna listado de respuestas asociadas
        'al al radicado
        'Fecha 2016-08-20
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT ID_RESPUESTA_RADICADO " &
                   " FROM ra_respuesta_radicado " &
                    " where  RADICADO='" & radicado & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Retorna_combo_respuesta_tramite = "Función Retorna_combo_respuesta_tramite " & Result
                Exit Function
            End If
            dropwlist.Items.Clear()
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Dat_reader.Tables(0).Rows.Count - 1
                    dropwlist.Items.Add(Dat_reader.Tables(0).Rows(i).Item(0))
                Next
                Retorna_combo_respuesta_tramite = "YES"
                Exit Function
            End If
            Retorna_combo_respuesta_tramite = "YES"
        Catch ex As Exception
            Retorna_combo_respuesta_tramite = "Inconsistencia general función Retorna_combo_respuesta_tramite " & ex.Message
        End Try
    End Function
    Function Retorna_detalle_respuesta_radicado(ByVal id_respuesta As Integer,
                                                ByRef pag As Page,
                                                ByVal radicado As String) As String
        Try
            Dim Result As String = ""
            Dim struc_envio As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                       struc_envio)
            If Result <> "YES" Then
                Retorna_detalle_respuesta_radicado = Result
                Exit Function
            End If
            If struc_envio.RADICADO Is Nothing Then
                Retorna_detalle_respuesta_radicado = "El radicado " & radicado & " no tiene una respuesta relacionada"
                Exit Function
            End If
            Dim Usuario_gestion As String = ""
            Dim Cargo_usuario_gestion As String = ""
            '--------------------------------
            'Retorna datos usuario de gestión
            '---------------------------------
            Dim Ref_clas_remit_dest As New Class_remit_dest_interno
            Dim correo_electrnico As String = ""
            Result = Ref_clas_remit_dest.Retorna_datos_caracterizacion_usuario_gestion(struc_envio.ID_REMIT_DEST_INT,
                                                                                       Usuario_gestion,
                                                                                       Cargo_usuario_gestion,
                                                                                       correo_electrnico)
            If Result <> "YES" Then
                Retorna_detalle_respuesta_radicado = Result
                Exit Function
            End If
            Dim obje As Object = pag.FindControl("Label_RADICADO_TRAMITE")
            obje.text = struc_envio.RADICADO
            obje = pag.FindControl("LabelDESTINATARIO")
            obje.text = struc_envio.DESTINATARIO
            obje = pag.FindControl("Label_TIPO_TRAMITE")
            obje.text = struc_envio.TRAMITE_DOCUMENTO
            obje = pag.FindControl("Label_FECHA_VENCE")
            obje.text = struc_envio.FECHA_VENCE
            obje = pag.FindControl("Label_FECHA_RESPUETA")
            obje.text = struc_envio.FECHA_RESPUETA
            'Dim date1al As String = ""
            obje = pag.FindControl("LabelFECHA_REGISTRO")
            obje.text = struc_envio.FECHA_REGISTRO
            'date1al = struc_envio.FECHA_REGISTRO
            Dim sTiempo As String = ""
            Dim date_fecha As String = Left(struc_envio.FECHA_RESPUETA, 10)
            Dim dias_nohabiles As Integer = 0
            Dim refclasradicado As New ClassRadicador
            Dim refclas_gestion_fechas As New ClassGestionFechas
            If struc_envio.FECHA_RESPUETA <> "" Then
                Dim stiempo_ As Object = Nothing
                Dim minuno As Object = Nothing
                Dim hora As Object = Nothing
                Dim dias_calendario As Object = Nothing
                Dim dias_no_habiles As Object = Nothing
                Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(struc_envio.FECHA_REGISTRO,
                                                                                 stiempo_,
                                                                                 hora,
                                                                                 minuno,
                                                                                 dias_calendario,
                                                                                 dias_no_habiles,
                                                                                 struc_envio.FECHA_RESPUETA)
                If Result <> "YES" Then
                    Retorna_detalle_respuesta_radicado = Result
                    Exit Function
                End If
                sTiempo = stiempo_ & " días, "
                sTiempo = sTiempo & hora.ToString & " horas, "
                sTiempo = sTiempo & minuno & " minutos. "
                sTiempo = sTiempo & "Formula de calculo tiempo de respuesta : dias calendario transcurridos " & dias_calendario & ", menos días no habiles " & dias_no_habiles
            Else
                Dim stiempo_ As Object = Nothing
                Dim minuno As Object = Nothing
                Dim hora As Object = Nothing
                Dim dias_calendario As Object = Nothing
                Dim dias_no_habiles As Object = Nothing
                Result = refclas_gestion_fechas.Solicita_tiempo_respuesta_tramite(struc_envio.FECHA_REGISTRO,
                                                                                 stiempo_,
                                                                                 hora,
                                                                                 minuno,
                                                                                 dias_calendario,
                                                                                 dias_no_habiles)
                If Result <> "YES" Then
                    Retorna_detalle_respuesta_radicado = Result
                    Exit Function
                End If

                sTiempo = "Para dar respuesta a su solicitud a transcurrido " & stiempo_ & " días, "
                sTiempo = sTiempo & hora.ToString & " horas, "
                sTiempo = sTiempo & minuno.ToString & " minutos, pero aún no tiene una respuesta para su solicitud "
                sTiempo = sTiempo & "(Formula de calculo tiempo de respuesta : dias calendario transcurridos " & dias_calendario & ", menos días no habiles " & dias_no_habiles & ")"
            End If
            obje = pag.FindControl("Label_TIEMPO_ESTIMADO_RESPUESTA")
            obje.text = sTiempo
            obje = pag.FindControl("Label_RADICADO_RELACIONADO_RESPUESTA")
            obje.text = struc_envio.RADICADO_RESPUESTA
            obje = pag.FindControl("Label_USUARIO_ELABORO_RESPUESTA")
            obje.text = Usuario_gestion
            obje = pag.FindControl("Label_CARGO_USUARIO_ELABORO_RESPUESTA")
            obje.text = Cargo_usuario_gestion
            obje = pag.FindControl("Label_MEDIO_ENVIO_RESPUESTA")
            obje.text = struc_envio.MEDIO_ENVIO
            obje = pag.FindControl("Label_CURRIER_ENVIO_RESPUESTA")
            obje.text = struc_envio.EMPRESA_ENVIO
            obje = pag.FindControl("Label_GUIA_CURRIER_RESPUESTA")
            obje.text = struc_envio.GUIA_ENVIO
            obje = pag.FindControl("LabelFECHA_ENVIO_RESPUESTA")
            obje.text = struc_envio.FECHA_ENVIO
            obje = pag.FindControl("LabelASUNTO")
            obje.text = struc_envio.ASUNTO
            obje = pag.FindControl("Label_FECHA_REGISTRO_EVIO_CORREO")
            obje.text = struc_envio.FECHA_REGISTRO_EVIO_CORREO
            obje = pag.FindControl("Label_CORREO_NOTIFICACION")
            obje.text = struc_envio.CORREO_NOTIFICACION
            obje = pag.FindControl("Label_FECHA_CONFIRMACION_CORREO_RECIBIDO")
            obje.text = struc_envio.FECHA_CONFIRMACION_CORREO_RECIBIDO
            obje = pag.FindControl("TextBox_NOTA_RESPUESTA")
            obje.text = struc_envio.NOTA_RESPUESTA
            obje = pag.FindControl("Image_estado_resp")
            Dim tipo_respuesta_tramite As Integer = 0
            Result = Me.Retorna_estado_envio_por_id_respuesta(struc_envio.ID_RESPUESTA_RADICADO, tipo_respuesta_tramite)
            If Result <> "YES" Then
                Retorna_detalle_respuesta_radicado = Result
                Exit Function
            End If
            Dim estado_obliga As Integer = -1
            Dim nombre_plantilla As String = ""
            Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
            Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(struc_envio.system_plantilla_radicado_id_plantilla,
                                                                                 nombre_plantilla)
            If Result <> "YES" Then
                Retorna_detalle_respuesta_radicado = Result
                Exit Function
            End If
            '----------------------------------------------------
            'Retorna el tipo de tramite obligatorio de respuesta
            '----------------------------------------------------
            Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
            Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
            Dim id_tipo_tramite As Integer = 0
            Dim descripcion_tramite As String = ""
            Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla,
                                                                                                 struc_envio.RADICADO,
                                                                                                 id_tipo_tramite,
                                                                                                 descripcion_tramite)
            If Result <> "YES" Then
                Retorna_detalle_respuesta_radicado = Result
                Exit Function
            End If
            Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                             estado_obliga)
            If Result <> "YES" Then
                Retorna_detalle_respuesta_radicado = Result
                Exit Function
            End If
            '-----------------------------------------------------------------
            'Asigna el tipo trámite del radicado a al tipo de respuesta si no
            'hay una respuesta elaborada
            '-----------------------------------------------------------------
            If struc_envio.FECHA_RESPUETA = "" Then
                struc_envio.TIPO_RESPUESTA_ELAB_USUARIO = estado_obliga
            End If
            If struc_envio.TIPO_RESPUESTA_ELAB_USUARIO = 0 Then
                ''----------------------------------------------
                ''Dibuja semáforo para sólo confirmación
                ''----------------------------------------------
                'If tipo_respuesta_tramite = 0 Then
                '    Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion(struc_envio.ID_RESPUESTA_RADICADO, obje)
                '    If Result <> "YES" Then
                '        Retorna_detalle_respuesta_radicado = Result
                '        Exit Function
                '    End If
                'Else
                '    Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(struc_envio.ID_RESPUESTA_RADICADO, obje)
                '    If Result <> "YES" Then
                '        Retorna_detalle_respuesta_radicado = Result
                '        Exit Function
                '    End If
                'End If
            Else
                '----------------------------------------------
                'Dibuja semáforo para respuesta obligatoria
                '----------------------------------------------
                'Dim reflclas_resp_radicado As New Class_ra_respuesta_radicado
                'If tipo_respuesta_tramite = 0 Then
                '    Result = reflclas_resp_radicado.Solicita_estados_semaforo_respuesta(struc_envio.ID_RESPUESTA_RADICADO, obje)
                '    If Result <> "YES" Then
                '        Retorna_detalle_respuesta_radicado = Result
                '        Exit Function
                '    End If
                'Else
                '    Dim ref_clas_resp As New Class_ra_respuesta_radicado
                '    Result = ref_clas_resp.Solicita_estados_semaforo_respuesta_electronica(struc_envio.ID_RESPUESTA_RADICADO, obje)
                '    If Result <> "YES" Then
                '        Retorna_detalle_respuesta_radicado = Result
                '        Exit Function
                '    End If
                'End If
            End If
            obje = pag.FindControl("Label_fecha_resp")
            If Not obje Is Nothing Then
                If struc_envio.ESTADO_RESPUESTA = 1 Then
                    obje.text = "FECHA EN QUE SE LE DIO RESPUESTA AL TRAMITE"
                End If
                If struc_envio.ESTADO_RESPUESTA = 5 Then
                    obje.text = "FECHA EN QUE SE CONFIRMO EL TRAMITE"
                End If
                If struc_envio.ESTADO_RESPUESTA = 6 Then
                    obje.text = "FECHA EN QUE SE ARCHIVO EL TRAMITE"
                End If
            End If
            obje = pag.FindControl("Label_tipo_respuesta_usuario")
            obje.text = ""
            If struc_envio.ESTADO_RESPUESTA = 1 Then
                obje.text = "Respuesta formal"
            End If
            If struc_envio.ESTADO_RESPUESTA = 6 Then
                obje.text = "Archivado"
            End If
            If struc_envio.ESTADO_RESPUESTA = 5 Then
                obje.text = "Confirmación"
            End If
            If struc_envio.ESTADO_RESPUESTA = 4 Then
                obje.text = "En tramite"
            End If
            obje = pag.FindControl("Label_tipo_respuesta")
            If struc_envio.ESTADO_RESPUESTA = 1 Or struc_envio.ESTADO_RESPUESTA = 5 Then
                If struc_envio.TIPO_RESPUESTA_ELAB_USUARIO = 0 Then
                    obje.text = "Respuesta solo confirmación"
                Else
                    obje.text = "Respuesta formal con radicado"
                End If
            End If
            obje = pag.FindControl("Label_tipo_envio_respuesta")
            If struc_envio.ESTADO_RESPUESTA = 1 Or struc_envio.ESTADO_RESPUESTA = 5 Then
                If tipo_respuesta_tramite = 0 Then
                    obje.text = "Envío respuesta a correo físico o convencional"
                Else
                    obje.text = "Envío respuesta a correo Electrónico"
                End If
            End If
            Retorna_detalle_respuesta_radicado = "YES"
        Catch ex As Exception
            Retorna_detalle_respuesta_radicado = "Inconsistencia general funcion Retorna_detalle_respuesta_radicado " & ex.Message
        End Try
    End Function


    Function Retorna_detalle_respuesta_radicado(ByVal radicado As String,
                                                ByRef tramite As String,
                                                ByRef Fecha_vence As String,
                                                ByRef fecha_registro As String,
                                                ByRef destinatario As String,
                                                ByRef asunto As String) As String
        '-------------------------------------------------------------------------
        'Funcion : Retorna detalles de la respuesta del radicado con el parametro
        'radicado
        'Fecha 2016-02-14
        'Ing Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "SELECT TRAMITE_DOCUMENTO,FECHA_VENCE,FECHA_REGISTRO," &
               "DESTINATARIO,ASUNTO " &
               " FROM ra_respuesta_radicado " &
              " where  RADICADO='" & radicado & "' order by  ID_RESPUESTA_RADICADO asc"
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Retorna_detalle_respuesta_radicado = "función Retorna_detalle_respuesta_radicado " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count <= 0 Then
                Retorna_detalle_respuesta_radicado = "Imposible econtrar detalle respuesta radicado " & radicado
                Exit Function
            Else
                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    tramite = "No informado"
                Else
                    tramite = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = True Then
                    Fecha_vence = "No informado"
                Else
                    Fecha_vence = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = True Then
                    fecha_registro = "No informado"
                Else
                    fecha_registro = Dat_reader.Tables(0).Rows(0).Item(2)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(3) = True Then
                    destinatario = "No informado"
                Else
                    destinatario = Dat_reader.Tables(0).Rows(0).Item(3)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(4) = True Then
                    asunto = "No informado"
                Else
                    asunto = Dat_reader.Tables(0).Rows(0).Item(4)
                End If
                Retorna_detalle_respuesta_radicado = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_detalle_respuesta_radicado = "Inconsistencia función Retorna_detalle_respuesta_radicado " & ex.Message
        End Try
    End Function

    Function Retorna_datos_asignacion_respuesta_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                               ByRef id_area As Integer,
                                                               ByRef nombre_area As String,
                                                               ByRef nombre_usuario_gestion As String,
                                                               ByRef cargo_usuario_gestion As String) As String
        Try
            Dim Parametro_Consulta As String = "select Areas_Dep_Radicacion_id_Areas_Dep,Nombre_Area,Nombre_Remitente,Cargo_Remite from  remit_dest_interno as rdi " &
            "left outer join   areas_depart_radicacion as adr on (adr.Codigo_Area=rdi.Areas_Dep_Radicacion_id_Areas_Dep) where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                Retorna_datos_asignacion_respuesta_usuario_gestion = "función Retorna_datos_asignacion_respuesta_usuario_gestion " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count <= 0 Then
                Retorna_datos_asignacion_respuesta_usuario_gestion = "Imposible encontrar datos usuario de gestión de asignación"
                Exit Function
            Else
                id_area = Dat_reader.Tables(0).Rows(0).Item(0)
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = True Then
                    nombre_area = ""
                Else
                    nombre_area = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                nombre_usuario_gestion = Dat_reader.Tables(0).Rows(0).Item(2)
                cargo_usuario_gestion = Dat_reader.Tables(0).Rows(0).Item(3)
                Retorna_datos_asignacion_respuesta_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_asignacion_respuesta_usuario_gestion = "Inconsistencia función  Retorna_datos_asignacion_respuesta_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Retorna_datos_estructura_envio_por_radicado(ByVal radicado As String, ByRef struc_envio As stru_envio, Optional ByVal op_confirma As Integer = 0) As String
        Dim Parametro_Consulta As String = "Select ID_RESPUESTA_RADICADO,ID_REMIT_DEST_INT,ID_AREA," &
            "system_plantilla_radicado_id_plantilla,RADICADO,ID_RUTA_WF,ID_TAREA_WF,FECHA_REGISTRO," &
            "FECHA_VENCE,FECHA_RESPUETA,HORA_RESPUESTA,TIEMPO_RESPUESTA,RADICADO_RESPUESTA,MEDIO_ENVIO," &
            "EMPRESA_ENVIO,GUIA_ENVIO,FECHA_ENVIO,HORA_ENVIO,ID_USUARIO_RADICADO,ID_IMAGEN,GABINETE," &
            "NOTA_RESPUESTA,FECHA_RECIBO_FISICO,HORA_RECIBO_FISICO,DESTINATARIO,DIRECCION_DESTINATARIO," &
            "TRAMITE_DOCUMENTO,ESTADO_ENVIO,ESTADO_RESPUESTA,id_usuario_gestion_propietario,ASUNTO,codigo_dest_externo,USUARIO_RESPONSABLE from ra_respuesta_radicado where RADICADO='" & radicado & "' limit 1"
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Retorna_datos_estructura_envio_por_radicado = "Función Retorna_datos_estructura_envio_por_radicado dice  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then

                If Dat_reader.Tables(0).Rows(0).IsNull(0) = True Then
                    struc_envio.ID_RESPUESTA_RADICADO = 0
                Else
                    struc_envio.ID_RESPUESTA_RADICADO = Dat_reader.Tables(0).Rows(0).Item(0)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = True Then
                    struc_envio.ID_REMIT_DEST_INT = 0
                Else
                    struc_envio.ID_REMIT_DEST_INT = Dat_reader.Tables(0).Rows(0).Item(1)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(2) = True Then
                    struc_envio.ID_AREA = 0
                Else
                    struc_envio.ID_AREA = Dat_reader.Tables(0).Rows(0).Item(2)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(3) = True Then
                    struc_envio.system_plantilla_radicado_id_plantilla = 0
                Else
                    struc_envio.system_plantilla_radicado_id_plantilla = Dat_reader.Tables(0).Rows(0).Item(3)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(4) = True Then
                    struc_envio.RADICADO = ""
                Else
                    struc_envio.RADICADO = Dat_reader.Tables(0).Rows(0).Item(4)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(5) = True Then
                    struc_envio.ID_RUTA_WF = 0
                Else
                    struc_envio.ID_RUTA_WF = Dat_reader.Tables(0).Rows(0).Item(5)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(6) = True Then
                    struc_envio.ID_TAREA_WF = 0
                Else
                    struc_envio.ID_TAREA_WF = Dat_reader.Tables(0).Rows(0).Item(6)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(7) = True Then
                    struc_envio.FECHA_REGISTRO = ""
                Else
                    struc_envio.FECHA_REGISTRO = Dat_reader.Tables(0).Rows(0).Item(7)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(8) = True Then
                    struc_envio.FECHA_VENCE = ""
                Else
                    struc_envio.FECHA_VENCE = Dat_reader.Tables(0).Rows(0).Item(8)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(9) = True Then
                    struc_envio.FECHA_RESPUETA = ""
                Else
                    struc_envio.FECHA_RESPUETA = Dat_reader.Tables(0).Rows(0).Item(9)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(10) = True Then
                    struc_envio.HORA_RESPUESTA = ""
                Else
                    struc_envio.HORA_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(10)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(11) = True Then
                    struc_envio.TIEMPO_RESPUESTA = 0
                Else
                    struc_envio.TIEMPO_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(11)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(12) = True Then
                    struc_envio.RADICADO_RESPUESTA = ""
                Else
                    struc_envio.RADICADO_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(12)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(13) = True Then
                    struc_envio.MEDIO_ENVIO = ""
                Else
                    struc_envio.MEDIO_ENVIO = Dat_reader.Tables(0).Rows(0).Item(13)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(14) = True Then
                    struc_envio.EMPRESA_ENVIO = ""
                Else
                    struc_envio.EMPRESA_ENVIO = Dat_reader.Tables(0).Rows(0).Item(14)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(15) = True Then
                    struc_envio.GUIA_ENVIO = ""
                Else
                    struc_envio.GUIA_ENVIO = Dat_reader.Tables(0).Rows(0).Item(15)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(16) = True Then
                    struc_envio.FECHA_ENVIO = ""
                Else
                    struc_envio.FECHA_ENVIO = Dat_reader.Tables(0).Rows(0).Item(16)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(17) = True Then
                    struc_envio.HORA_ENVIO = ""
                Else
                    struc_envio.HORA_ENVIO = Dat_reader.Tables(0).Rows(0).Item(17)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(18) = True Then
                    struc_envio.ID_USUARIO_RADICADO = 0
                Else
                    struc_envio.ID_USUARIO_RADICADO = Dat_reader.Tables(0).Rows(0).Item(18)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(19) = True Then
                    struc_envio.ID_IMAGEN = 0
                Else
                    struc_envio.ID_IMAGEN = Dat_reader.Tables(0).Rows(0).Item(19)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(20) = True Then
                    struc_envio.GABINETE = ""
                Else
                    struc_envio.GABINETE = Dat_reader.Tables(0).Rows(0).Item(20)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(21) = True Then
                    struc_envio.NOTA_RESPUESTA = ""
                Else
                    struc_envio.NOTA_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(21)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(22) = True Then
                    struc_envio.FECHA_RECIBO_FISICO = ""
                Else
                    struc_envio.FECHA_RECIBO_FISICO = Dat_reader.Tables(0).Rows(0).Item(22)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(23) = True Then
                    struc_envio.HORA_RECIBO_FISICO = ""
                Else
                    struc_envio.HORA_RECIBO_FISICO = Dat_reader.Tables(0).Rows(0).Item(23)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(24) = True Then
                    struc_envio.DESTINATARIO = ""
                Else
                    struc_envio.DESTINATARIO = Dat_reader.Tables(0).Rows(0).Item(24)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(25) = True Then
                    struc_envio.DIRECCION_DESTINATARIO = ""
                Else
                    struc_envio.DIRECCION_DESTINATARIO = Dat_reader.Tables(0).Rows(0).Item(25)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(26) = True Then
                    struc_envio.TRAMITE_DOCUMENTO = ""
                Else
                    struc_envio.TRAMITE_DOCUMENTO = Dat_reader.Tables(0).Rows(0).Item(26)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(27) = True Then
                    struc_envio.ESTADO_ENVIO = 0
                Else
                    struc_envio.ESTADO_ENVIO = Dat_reader.Tables(0).Rows(0).Item(27)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(28) = True Then
                    struc_envio.ESTADO_RESPUESTA = 0
                Else
                    struc_envio.ESTADO_RESPUESTA = Dat_reader.Tables(0).Rows(0).Item(28)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(29) = True Then
                    struc_envio.id_usuario_gestion_propietario = 0
                Else
                    struc_envio.id_usuario_gestion_propietario = Dat_reader.Tables(0).Rows(0).Item(29)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(30) = True Then
                    struc_envio.ASUNTO = 0
                Else
                    struc_envio.ASUNTO = Dat_reader.Tables(0).Rows(0).Item(30)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(31) = True Then
                    struc_envio.codigo_dest_externo = 0
                Else
                    struc_envio.codigo_dest_externo = Dat_reader.Tables(0).Rows(0).Item(31)
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(32) = True Then
                    struc_envio.USUARIO_RESPONSABLE = ""
                Else
                    struc_envio.USUARIO_RESPONSABLE = Dat_reader.Tables(0).Rows(0).Item(32)
                End If
                Retorna_datos_estructura_envio_por_radicado = "YES"
                Exit Function
            Else
                struc_envio.ID_RESPUESTA_RADICADO = 0
                If op_confirma = 0 Then
                    Retorna_datos_estructura_envio_por_radicado = "El radicado (" & radicado & ") no tiene respuesta relacionada"
                    Exit Function
                Else
                    Retorna_datos_estructura_envio_por_radicado = "YES"
                    Exit Function
                End If

            End If
        Catch ex As Exception
            Retorna_datos_estructura_envio_por_radicado = "Inconsistencia función Retorna_datos_estructura_envio_por_radicado " & ex.Message
        End Try
    End Function
    Function Valida_usuario_propietario_respuesta(ByVal id_respuesta As Integer,
                                                  ByVal id_usuario_gestion_logueado As Integer) As String
        '-----------------------------------------------------
        'Función Verfica propiedad de respuesta de aprobación
        'de los usuarios
        'Ing Miguel Angel Urueta Miranda
        'Fecha 2017-02-10
        '------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim struc_envio As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, struc_envio)
            If Result <> "YES" Then
                Valida_usuario_propietario_respuesta = Result
                Exit Function
            End If
            If struc_envio.ID_REMIT_DEST_INT <> id_usuario_gestion_logueado Then
                Valida_usuario_propietario_respuesta = "El usuario no es el propietario de la respuesta, imposible seguir con su solicitud de aprobación"
                Exit Function
            Else
                Valida_usuario_propietario_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Valida_usuario_propietario_respuesta = "Inconsistencia general función Valida_usuario_propietario_respuesta " & ex.Message
        End Try
    End Function
    Function Reasigna_respuesta_sistema_usuario(ByVal id_respuesta As Integer,
                                                ByVal id_usuario_gestion As Integer,
                                                ByVal radicado As String,
                                                ByVal id_usuario_gestion_afectado As Integer) As String

        Dim id_imagen_plantilla As Integer = 0
        Dim radicado_respuesta As Integer = 0
        Dim fecha_respuesta As Integer = 0
        Dim id_imagen_respuesta As Integer = 0
        Dim estado_envio_respuesta As Integer = 0
        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
        Dim Result As String = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta,
                                                                                              id_imagen_plantilla,
                                                                                              radicado_respuesta,
                                                                                              fecha_respuesta,
                                                                                              id_imagen_respuesta,
                                                                                              estado_envio_respuesta)
        If Result <> "YES" Then
            Reasigna_respuesta_sistema_usuario = Result
            Exit Function
        End If
        If fecha_respuesta <> 0 And radicado_respuesta <> 0 Then
            Reasigna_respuesta_sistema_usuario = "El tramite tiene una respuesta imposible reasignar y gestionar"
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario al que se le reaasigna la respuesta
        '--------------------------------------------------------------
        Dim id_area_transaccion As Integer = 0
        Dim nombre_area_transaccion As String = ""
        Dim nombre_usuario_gestion_transaccion As String = ""
        Dim cargo_usuario_gestion_transaccion As String = ""
        Result = Retorna_datos_asignacion_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                        id_area_transaccion,
                                                                        nombre_area_transaccion,
                                                                        nombre_usuario_gestion_transaccion,
                                                                        cargo_usuario_gestion_transaccion)
        If Result <> "YES" Then
            Reasigna_respuesta_sistema_usuario = Result
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario de gestion al que se le quita el tramite
        '--------------------------------------------------------------
        Dim nombre_usuario_gestion_afectado As String = ""
        Dim cargo_usuario_gestion_afectado As String = ""
        Dim id_area_afectado As Integer = 0
        Dim nombre_area_afectado As String = ""
        Result = Retorna_datos_asignacion_respuesta_usuario_gestion(id_usuario_gestion_afectado,
                                                                    id_area_afectado,
                                                                    nombre_area_afectado,
                                                                    nombre_usuario_gestion_afectado,
                                                                    cargo_usuario_gestion_afectado)
        If Result <> "YES" Then
            Reasigna_respuesta_sistema_usuario = Result
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna documentos de colaboración relacionados con el tramite
        '--------------------------------------------------------------
        Dim numero_documentos_compartidos_relacionados As Integer = 0
        Dim ref_clas_doc_comp As New ClassGaCompartirDocumento
        Dim sql_actualiza_solicitante_colaboracion As String = ""
        Result = ref_clas_doc_comp.Numero_documentos_compartidos_relacionados_a_un_radicado(radicado,
                                                                                                numero_documentos_compartidos_relacionados)
        If Result <> "YES" Then
            Reasigna_respuesta_sistema_usuario = Result
            Exit Function
        End If
        If numero_documentos_compartidos_relacionados > 0 Then
            sql_actualiza_solicitante_colaboracion = "UPDATE ra_cd_documentos_compartidos SET Remit_Dest_Interno_id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
                    " where RADICADO_RELACIONADO='" & radicado & "' and TIPO_REGISTRO_COMPARTIDO=3"
        End If
        Dim hor As String = Now
        Dim detalle_trans As String = "REASIGNA RESPUESTA"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Reasigna_respuesta_sistema_usuario = Result
            Exit Function
        End If

        campos_trans = "SE RELEVÓ DE LA RESPUESTA NUMERO (" & id_respuesta &
        ") DEL RADICADO " & radicado & " AL USUARIO DE GESTION ID : " & id_usuario_gestion_afectado & " nombre " & nombre_usuario_gestion_afectado &
        " cargo " & cargo_usuario_gestion_afectado & " (Con autorización de " & "sistema autonomo" & ") ( por el usuario " & nombre_usuario_gestion_transaccion & " Del cargo " & cargo_usuario_gestion_transaccion & ")"
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     id_respuesta & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "'," & id_usuario_gestion_afectado & ")"

        Dim update_respuesta As String = "Update ra_respuesta_radicado set ID_REMIT_DEST_INT=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
           ",ID_AREA=" & id_area_transaccion & ",AREA_RESPONSABLE='" & nombre_area_transaccion & "',USUARIO_RESPONSABLE='" & nombre_usuario_gestion_transaccion & "'" &
           ",CARGO_RESPONSABLE='" & cargo_usuario_gestion_transaccion & "'" & " where RADICADO='" & radicado & "'"
        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                 ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS,id_usuario_gestion_afectado) values " &
                                                 isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref_conect As New conect.Dbase_Conction_Mysql_RA
        ref_conect.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '--------------------------------------------------
            'Reasigna respuesta radicado
            '--------------------------------------------------
            myCommand.CommandText = update_respuesta
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_sistema_usuario = "Imposible reasignar respuesta "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_sistema_usuario = "Imposible registrar log "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-----------------------------------------------------------------------
            'Reasigna los documentos de colaboración relacionados con el trámite
            '-----------------------------------------------------------------------
            If sql_actualiza_solicitante_colaboracion <> "" Then
                myCommand.CommandText = sql_actualiza_solicitante_colaboracion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Reasigna_respuesta_sistema_usuario = "Imposible regisignar trámites de colaboración "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If

            myTrans.Commit()
            myConnection.Close()
            Reasigna_respuesta_sistema_usuario = "YES"
        Catch ex As Exception
            If Not myConnection Is Nothing Then
                If myConnection.State = ConnectionState.Open Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Reasigna_respuesta_sistema_usuario = "Inconistencia general función Reasigna_respuesta_sistema_usuario " & ex.Message
                    Exit Function
                End If
            Else
                Reasigna_respuesta_sistema_usuario = "Inconistencia general función Reasigna_respuesta_sistema_usuario " & ex.Message
                Exit Function
            End If
        End Try

    End Function
    Function Reasigna_respuesta_tarea_manual(
                                             ByVal id_tarea_selecionada As Integer,
                                             ByRef resultadocorreo As String,
                                             ByRef pag As Page,
                                             ByVal usuario_transac As String,
                                             ByVal id_usuario_administrador As Integer) As String
        '*******************************************************
        'Fución : Envia y reasigna tarea a usuario de gestion
        'Fecha : 2015-03-21
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************
        Dim Refclas As New ClassWorkflow
        Dim Result As String = ""
        '-------------------------------------------------------------
        'Verifica existencia relacion usuario de envio seleccionado
        '-------------------------------------------------------------
        Dim id_usuario_gestion_afectado As Integer = 0
        Dim nombre_usuario_gestion_transaccion As String = ""
        Dim cargo_usuario_gestion_transaccion As String = ""
        Dim id_area_transaccion As Integer = 0
        Dim nombre_area_transaccion As String = ""
        Dim nombre_usuario_gestion_afectado As String = ""
        Dim cargo_usuario_gestion_afectado As String = ""
        Dim id_area_afectado As Integer = 0
        Dim nombre_area_afectado As String = ""
        Dim Refclas_resp As New Classgestionrespuesta
        Dim Refclas_gestor As New ClassGestorDocumental
        Dim Radicado As String = ""
        Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecionada,
                                                                             Radicado)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_manual = Result
            Exit Function
        End If
        If Radicado = "" Then
            Reasigna_respuesta_tarea_manual = "La tarea seleccionada no tiene radicado relacionado para reasignar "
            Exit Function
        End If
        Dim stru_envi As stru_envio = Nothing
        Result = Refclas_resp.Retorna_datos_estructura_envio_por_radicado(Radicado, stru_envi)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_manual = Result
            Exit Function
        End If
        id_usuario_gestion_afectado = stru_envi.ID_REMIT_DEST_INT
        Dim id_imagen_plantilla As Integer = 0
        Dim radicado_respuesta As Integer = 0
        Dim fecha_respuesta As Integer = 0
        Dim id_imagen_respuesta As Integer = 0
        Dim estado_envio_respuesta As Integer = 0
        Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
        Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(stru_envi.ID_RESPUESTA_RADICADO,
                                                                            id_imagen_plantilla,
                                                                            radicado_respuesta,
                                                                            fecha_respuesta,
                                                                            id_imagen_respuesta,
                                                                            estado_envio_respuesta)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_manual = Result
            Exit Function
        End If
        If fecha_respuesta <> 0 And radicado_respuesta <> 0 Then
            Reasigna_respuesta_tarea_manual = "El tramite tiene una respuesta imposible reasignar"
            Exit Function
        End If

        If id_usuario_gestion_afectado = 0 Then
            Reasigna_respuesta_tarea_manual = "La respuesta no tiene  relacionado usuario de gestión "
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario de gestion que reaasigna
        '--------------------------------------------------------------
        Result = Retorna_datos_asignacion_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                    id_area_transaccion,
                                                                    nombre_area_transaccion,
                                                                    nombre_usuario_gestion_transaccion,
                                                                    cargo_usuario_gestion_transaccion)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_manual = Result
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario de gestion al que se le quita el tramite
        '--------------------------------------------------------------
        Result = Retorna_datos_asignacion_respuesta_usuario_gestion(id_usuario_gestion_afectado,
                                                                    id_area_afectado,
                                                                    nombre_area_afectado,
                                                                    nombre_usuario_gestion_afectado,
                                                                    cargo_usuario_gestion_afectado)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_manual = Result
            Exit Function
        End If
        '---------------------------------------------------------------
        'Verifica que el usuarios de gestión no sea el mismo
        '---------------------------------------------------------------
        'If id_usuario_gestion_afectado = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
        '    Reasigna_respuesta_tarea_manual = "Está tratando de reasignar a un mismo usuario la responsabilidad de la respuesta, imposible continuar"
        '    Exit Function
        'End If
        '--------------------------------------------------------------
        'Retorna documentos de colaboración relacionados con el tramite
        '--------------------------------------------------------------
        Dim numero_documentos_compartidos_relacionados As Integer = 0
        Dim ref_clas_doc_comp As New ClassGaCompartirDocumento
        Dim sql_actualiza_solicitante_colaboracion As String = ""
        Result = ref_clas_doc_comp.Numero_documentos_compartidos_relacionados_a_un_radicado(stru_envi.RADICADO,
                                                                                            numero_documentos_compartidos_relacionados)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_manual = Result
            Exit Function
        End If
        If numero_documentos_compartidos_relacionados > 0 Then
            sql_actualiza_solicitante_colaboracion = "UPDATE ra_cd_documentos_compartidos SET Remit_Dest_Interno_id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
                " where RADICADO_RELACIONADO='" & stru_envi.RADICADO & "' and TIPO_REGISTRO_COMPARTIDO=3"
        End If
        Dim hor As String = Now
        Dim detalle_trans As String = "REASIGNA RESPUESTA"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_manual = Result
            Exit Function
        End If
        campos_trans = "SE RELEVÓ DE LA RESPUESTA NUMERO (" & stru_envi.ID_RESPUESTA_RADICADO &
        ") DEL RADICADO " & Radicado & " AL USUARIO DE GESTION ID : " & id_usuario_gestion_afectado & " nombre " & nombre_usuario_gestion_afectado &
        " cargo " & cargo_usuario_gestion_afectado & " (Con autorización de " & usuario_transac & ") ( por el usuario " & nombre_usuario_gestion_transaccion & " Del cargo " & cargo_usuario_gestion_transaccion & ")"
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     stru_envi.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "'," & id_usuario_gestion_afectado & ")"

        Dim update_respuesta As String = "Update ra_respuesta_radicado set ID_REMIT_DEST_INT=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
       ",ID_AREA=" & id_area_transaccion & ",AREA_RESPONSABLE='" & nombre_area_transaccion & "',USUARIO_RESPONSABLE='" & nombre_usuario_gestion_transaccion & "'" &
       ",CARGO_RESPONSABLE='" & cargo_usuario_gestion_transaccion & "'" & " where RADICADO='" & Radicado & "'"
        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS,id_usuario_gestion_afectado) values " &
                                             isert_datos

        Dim myConnection As New MySqlConnection
        Dim ref_conect As New conect.Dbase_Conction_Mysql_RA
        ref_conect.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '--------------------------------------------------
            'Reasigna respuesta radicado
            '--------------------------------------------------
            myCommand.CommandText = update_respuesta
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_tarea_manual = "Imposible reasignar respuesta "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_tarea_manual = "Imposible registrar log "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-----------------------------------------------------------------------
            'Reasigna los documentos de colaboración relacionados con el trámite
            '-----------------------------------------------------------------------
            If sql_actualiza_solicitante_colaboracion <> "" Then
                myCommand.CommandText = sql_actualiza_solicitante_colaboracion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Reasigna_respuesta_tarea_manual = "Imposible regisignar trámites de colaboración "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If

            myTrans.Commit()
            myConnection.Close()
        Catch ex As Exception
            If Not myConnection Is Nothing Then
                If myConnection.State = ConnectionState.Open Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Reasigna_respuesta_tarea_manual = "Inconistencia general función Reasigna_respuesta_tarea_manual " & ex.Message
                    Exit Function
                End If
            Else
                Reasigna_respuesta_tarea_manual = "Inconistencia general función Reasigna_respuesta_tarea_manual " & ex.Message
                Exit Function
            End If
        End Try
        Dim Refclas_gestion_respuesta As New Classgestionrespuesta
        Dim correo_electronico As String = ""
        Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
        Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(id_usuario_gestion_afectado,
                                                                          correo_electronico)
        If Result <> "YES" Then
            resultadocorreo = "El trámite se envió pero no se notifico al correo electrónico por el siguiente error " & Result
            Reasigna_respuesta_tarea_manual = "YES"
            Exit Function
        End If
        Dim tramite As String = ""
        Dim Fecha_vence As String = ""
        Dim fecha_registro As String = ""
        Dim destinatario As String = ""
        Dim asunto As String = ""
        Result = Me.Retorna_detalle_respuesta_radicado(Radicado, tramite, Fecha_vence, fecha_registro, destinatario, asunto)
        If Result <> "YES" Then
            resultadocorreo = "El trámite se envió pero no se notifico al correo electrónico por el siguiente error " & Result
            Reasigna_respuesta_tarea_manual = "YES"
            Exit Function
        End If
        asunto = tramite & " Fecha vencimiento  " & Fecha_vence
        Dim asunto_final As String = "Relevo respuesta tramite, " & asunto
        Dim split_notificacion() As String = {"Usted fue relevado para dar respuesta al radicado : " & Radicado & " Tipo tramite " & tramite, "Fecha de radicación : " & fecha_registro _
                , "Fecha límite de respuesta : " & Fecha_vence, "Remite : " & destinatario, "Asunto : " & asunto, "Radicado : " & Radicado,
                "Por el usuario " & nombre_usuario_gestion_transaccion & " del cargo " & cargo_usuario_gestion_transaccion & " en la fecha " & date1al}
        Try
            Dim refclascorreo As New ClassCorreo
            Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion, correo_electronico, asunto_final)
            If Result <> "YES" Then
                resultadocorreo = "El trámite se envió pero no se notifico al correo electrónico por el siguiente error " & Result
                Reasigna_respuesta_tarea_manual = "YES"
                Exit Function
            End If
            Reasigna_respuesta_tarea_manual = "YES"
        Catch ex As Exception
            resultadocorreo = "El trámite se envió pero no se notifico al correo electrónico por el siguiente error " & ex.Message
            Reasigna_respuesta_tarea_manual = "YES"
        End Try
    End Function
    Function Reasigna_respuesta_tarea_recuperda(
                                                ByVal id_tarea_selecionada As Integer,
                                                ByRef resultadocorreo As String,
                                                ByRef pag As Page) As String
        '*******************************************************
        'Fución : Envia y reasigna tarea a usuario de gestion
        'Fecha : 2015-03-21
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************
        Dim Refclas As New ClassWorkflow
        Dim Result As String = ""
        Dim usuario_transac As String = ""
        Dim id_usuario_administrador As Integer = 0
        Dim Hidden_usuario_autoriza As Object = pag.FindControl("Hidden_usuario_autoriza")
        Dim Hidden_usuario_autoriza_id As Object = pag.FindControl("Hidden_usuario_autoriza_id")
        If Not Hidden_usuario_autoriza Is Nothing Then
            usuario_transac = Hidden_usuario_autoriza.value
        End If
        If Not Hidden_usuario_autoriza_id Is Nothing Then
            id_usuario_administrador = Hidden_usuario_autoriza_id.value
        End If
        '-------------------------------------------------------------
        'Verifica existencia relacion usuario de envio seleccionado
        '-------------------------------------------------------------
        Dim id_usuario_gestion_afectado As Integer = 0
        Dim nombre_usuario_gestion_transaccion As String = ""
        Dim cargo_usuario_gestion_transaccion As String = ""
        Dim id_area_transaccion As Integer = 0
        Dim nombre_area_transaccion As String = ""
        Dim nombre_usuario_gestion_afectado As String = ""
        Dim cargo_usuario_gestion_afectado As String = ""
        Dim id_area_afectado As Integer = 0
        Dim nombre_area_afectado As String = ""
        Dim Refclas_resp As New Classgestionrespuesta
        Dim Refclas_gestor As New ClassGestorDocumental
        Dim Radicado As String = ""
        Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecionada,
                                                                             Radicado)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_recuperda = Result
            Exit Function
        End If
        If Radicado = "" Then
            Reasigna_respuesta_tarea_recuperda = "La tarea seleccionada no tiene radicado relacionado para reasignar "
            Exit Function
        End If
        Dim stru_envi As stru_envio = Nothing
        Result = Refclas_resp.Retorna_datos_estructura_envio_por_radicado(Radicado,
                                                                          stru_envi)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_recuperda = Result
            Exit Function
        End If
        id_usuario_gestion_afectado = stru_envi.ID_REMIT_DEST_INT
        'Result = Refclas_gestor.SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow(id_usuario_workflow_afectado, id_usuario_gestion_afectado)
        'If Result <> "YES" Then
        '    Reasigna_respuesta_tarea_recuperda = "Imposible relacionar el usuario seleccionado con el usuario de gestión " & Result
        '    Exit Function
        'End If

        If id_usuario_gestion_afectado = 0 Then
            Reasigna_respuesta_tarea_recuperda = "La respuesta no tiene  relacionado usuario de gestión "
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario de gestion que reaasigna
        '--------------------------------------------------------------
        Result = Retorna_datos_asignacion_respuesta_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                    id_area_transaccion,
                                                                    nombre_area_transaccion,
                                                                    nombre_usuario_gestion_transaccion,
                                                                    cargo_usuario_gestion_transaccion)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_recuperda = Result
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario de gestion al que se le quita el tramite
        '--------------------------------------------------------------
        Result = Retorna_datos_asignacion_respuesta_usuario_gestion(id_usuario_gestion_afectado, id_area_afectado,
                                                                    nombre_area_afectado,
                                                                    nombre_usuario_gestion_afectado,
                                                                    cargo_usuario_gestion_afectado)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_recuperda = Result
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna documentos de colaboración relacionados con el tramite
        '--------------------------------------------------------------
        Dim numero_documentos_compartidos_relacionados As Integer = 0
        Dim ref_clas_doc_comp As New ClassGaCompartirDocumento
        Dim sql_actualiza_solicitante_colaboracion As String = ""
        Result = ref_clas_doc_comp.Numero_documentos_compartidos_relacionados_a_un_radicado(stru_envi.RADICADO,
                                                                                            numero_documentos_compartidos_relacionados)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_recuperda = Result
            Exit Function
        End If
        If numero_documentos_compartidos_relacionados > 0 Then
            sql_actualiza_solicitante_colaboracion = "UPDATE ra_cd_documentos_compartidos SET Remit_Dest_Interno_id_Remit_Dest_Int=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
                " where RADICADO_RELACIONADO='" & stru_envi.RADICADO & "' and TIPO_REGISTRO_COMPARTIDO=3"
        End If
        Dim hor As String = Now
        Dim detalle_trans As String = "RECUPERACION Y RELEVO"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Reasigna_respuesta_tarea_recuperda = Result
            Exit Function
        End If
        campos_trans = "SE RELEVÓ DE LA RESPUESTA NUMERO (" & stru_envi.ID_RESPUESTA_RADICADO &
        ") DEL RADICADO " & Radicado & " AL USUARIO DE GESTION ID : " & id_usuario_gestion_afectado & " nombre " & nombre_usuario_gestion_afectado &
        " cargo " & cargo_usuario_gestion_afectado & " (Con autorización de " & usuario_transac & ") ( por el usuario " & nombre_usuario_gestion_transaccion & " Del cargo " & cargo_usuario_gestion_transaccion & ")"
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     stru_envi.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "'," & id_usuario_gestion_afectado & ")"

        Dim update_respuesta As String = "Update ra_respuesta_radicado set ID_REMIT_DEST_INT=" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") &
       ",ID_AREA=" & id_area_transaccion & ",AREA_RESPONSABLE='" & nombre_area_transaccion & "',USUARIO_RESPONSABLE='" & nombre_usuario_gestion_transaccion & "'" &
       ",CARGO_RESPONSABLE='" & cargo_usuario_gestion_transaccion & "'" & " where RADICADO='" & Radicado & "'"
        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS,id_usuario_gestion_afectado) values " &
                                             isert_datos

        Dim myConnection As New MySqlConnection
        Dim ref_conect As New conect.Dbase_Conction_Mysql_RA
        ref_conect.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '--------------------------------------------------
            'Reasigna respuesta radicado
            '--------------------------------------------------
            myCommand.CommandText = update_respuesta
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_tarea_recuperda = "Imposible reasignar respuesta "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_tarea_recuperda = "Imposible registrar log "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-----------------------------------------------------------------------
            'Reasigna los documentos de colaboración relacionados con el trámite
            '-----------------------------------------------------------------------
            If sql_actualiza_solicitante_colaboracion <> "" Then
                myCommand.CommandText = sql_actualiza_solicitante_colaboracion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Reasigna_respuesta_tarea_recuperda = "Imposible regisignar trámites de colaboración "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
        Catch ex As Exception
            If Not myConnection Is Nothing Then
                If myConnection.State = ConnectionState.Open Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Reasigna_respuesta_tarea_recuperda = "Inconistencia general función Reasigna_respuesta_tarea_recuperda " & ex.Message
                    Exit Function
                End If
            Else
                Reasigna_respuesta_tarea_recuperda = "Inconistencia general función Reasigna_respuesta_tarea_recuperda " & ex.Message
                Exit Function
            End If
        End Try
        Dim Refclas_gestion_respuesta As New Classgestionrespuesta
        Dim correo_electronico As String = ""
        Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
        Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(id_usuario_gestion_afectado,
                                                                          correo_electronico)
        If Result <> "YES" Then
            resultadocorreo = "El trámite se recupero  pero no se notifico al correo electrónico por el siguiente error " & Result
            Reasigna_respuesta_tarea_recuperda = "YES"
            Exit Function
        End If
        Dim tramite As String = ""
        Dim Fecha_vence As String = ""
        Dim fecha_registro As String = ""
        Dim destinatario As String = ""
        Dim asunto As String = ""
        Result = Me.Retorna_detalle_respuesta_radicado(Radicado, tramite, Fecha_vence, fecha_registro, destinatario, asunto)
        If Result <> "YES" Then
            resultadocorreo = "El trámite se recupero  pero no se notifico al correo electrónico por el siguiente error " & Result
            Reasigna_respuesta_tarea_recuperda = "YES"
            Exit Function
        End If
        asunto = tramite & " Fecha vencimiento  " & Fecha_vence
        Dim asunto_final As String = "Relevo respuesta tramite, " & asunto
        Dim split_notificacion() As String = {"Usted fue relevado para dar respuesta al radicado : " & Radicado & " Tipo tramite " & tramite, "Fecha de radicación : " & fecha_registro _
                , "Fecha límite de respuesta : " & Fecha_vence, "Remite : " & destinatario, "Asunto : " & asunto, "Radicado : " & Radicado,
                "Por el usuario " & nombre_usuario_gestion_transaccion & " del cargo " & cargo_usuario_gestion_transaccion & " en la fecha " & date1al}
        Try
            Dim refclascorreo As New ClassCorreo
            Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion, correo_electronico, asunto_final)
            If Result <> "YES" Then
                resultadocorreo = "El trámite se recupero  pero no se notifico al correo electrónico por el siguiente error " & Result
                Reasigna_respuesta_tarea_recuperda = "YES"
                Exit Function
            End If
            Reasigna_respuesta_tarea_recuperda = "YES"
            Exit Function
        Catch ex As Exception
            resultadocorreo = "El trámite se recupero  pero no se notifico al correo electrónico por el siguiente error " & ex.Message
            Reasigna_respuesta_tarea_recuperda = "YES"
        End Try
    End Function
    Function Reasigna_respuesta_envia_tarea_usuario(ByVal id_usuario_gestion As Integer,
                                                    ByVal id_tarea_selecionada As Integer,
                                                    ByVal id_usuario_workflow_envio As Integer,
                                                    ByVal id_actividad As Integer,
                                                    ByVal id_usuario As Integer,
                                                    ByRef teview As TreeView,
                                                    ByRef resultadocorreo As String,
                                                    ByVal usuario_transac As String,
                                                    ByVal id_usuario_administrador As Integer,
                                                    ByRef pag As Page,
                                                    ByVal id_flujo_trabajo As Integer,
                                                    ByVal id_actividad_flujo_trabajo As Integer,
                                                    ByVal id_usuario_workflow_flujo_trabajo As Integer,
                                                    ByRef Resultado_evalua_terminar As String) As String
        resultadocorreo = "YES"
        '*******************************************************
        'Fución : Envia y reasigna tarea a usuario de gestion
        'Fecha : 2015-03-21
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************
        Dim Refclas As New ClassWorkflow
        Dim Result As String = ""
        Dim id_usuario_gestion_envio As Integer = 0
        '-------------------------------------------------------------
        'Verifica existencia relacion usuario de envio seleccionado
        '-------------------------------------------------------------
        Dim nombre_usuario_gestion_transaccion As String = ""
        Dim cargo_usuario_gestion_transaccion As String = ""
        Dim nombre_usuario_gestion As String = ""
        Dim cargo_usuario_gestion As String = ""
        Dim area_usuario_gestion As String = ""
        Dim id_area As Integer = 0
        Dim nombre_area As String = ""
        Dim Refclas_resp As New Classgestionrespuesta
        Dim Refclas_gestor As New ClassGestorDocumental
        Result = Refclas_gestor.SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow(id_usuario_workflow_envio,
                                                          id_usuario_gestion_envio)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario = "Imposible relacionar el usuario seleccionado con el usuario de gestión " & Result
            Exit Function
        End If

        If id_usuario_gestion_envio = 0 Then
            Reasigna_respuesta_envia_tarea_usuario = "El usuario seleccionado no tiene relacionado usuario de gestión "
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario de gestion que reaasigna
        '--------------------------------------------------------------
        Result = Refclas_gestor.Retorna_Caracterizacion_Usuario_Gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                        nombre_usuario_gestion_transaccion,
                                                                        cargo_usuario_gestion_transaccion,
                                                                        area_usuario_gestion)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario = Result
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario de gestion al que le reasigna el tramite
        '--------------------------------------------------------------
        Result = Retorna_datos_asignacion_respuesta_usuario_gestion(id_usuario_gestion_envio,
                                                                    id_area,
                                                                    nombre_area,
                                                                    nombre_usuario_gestion,
                                                                    cargo_usuario_gestion)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario = Result
            Exit Function
        End If
        Dim Radicado As String = ""
        Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecionada,
                                                                                Radicado)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario = Result
            Exit Function
        End If
        If Radicado = "" Then
            Reasigna_respuesta_envia_tarea_usuario = "La tarea seleccionada no tiene radicado relacionado para reasignar "
            Exit Function
        End If
        Dim stru_envi As stru_envio = Nothing
        Result = Refclas_resp.Retorna_datos_estructura_envio_por_radicado(Radicado,
                                                                          stru_envi)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario = Result
            Exit Function
        End If
        If stru_envi.ID_REMIT_DEST_INT <> id_usuario_gestion Then
            Reasigna_respuesta_envia_tarea_usuario = "El usuario de gestión no tiene asignada la respuesta para reasignar y enviar"
            Exit Function
        End If
        If stru_envi.FECHA_RESPUETA <> "" Then
            Reasigna_respuesta_envia_tarea_usuario = "No se puede reasignar por que el sistema detecto una una respuesta permanente"
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna documentos de colaboración relacionados con el tramite
        '--------------------------------------------------------------
        Dim numero_documentos_compartidos_relacionados As Integer = 0
        Dim ref_clas_doc_comp As New ClassGaCompartirDocumento
        Dim sql_actualiza_solicitante_colaboracion As String = ""
        Result = ref_clas_doc_comp.Numero_documentos_compartidos_relacionados_a_un_radicado(stru_envi.RADICADO,
                                                                                            numero_documentos_compartidos_relacionados)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario = Result
            Exit Function
        End If
        If numero_documentos_compartidos_relacionados > 0 Then
            sql_actualiza_solicitante_colaboracion = "UPDATE ra_cd_documentos_compartidos SET Remit_Dest_Interno_id_Remit_Dest_Int=" & id_usuario_gestion_envio &
                " where RADICADO_RELACIONADO='" & stru_envi.RADICADO & "' and TIPO_REGISTRO_COMPARTIDO=3"
        End If
        Dim hor As String = Now
        Dim detalle_trans As String = "REASIGNACION"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario = Result
            Exit Function
        End If
        campos_trans = "REASIGNA RESPUESTA NUMERO (" & stru_envi.ID_RESPUESTA_RADICADO &
        ") DEL RADICADO " & Radicado & " AL USUARIO DE GESTION ID : " & id_usuario_gestion_envio & " nombre " & nombre_usuario_gestion &
        " cargo " & cargo_usuario_gestion & " (Con autorización de " & usuario_transac & ") (Reasigno el usuario " & nombre_usuario_gestion & " Del cargo " & cargo_usuario_gestion & ")"
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & id_usuario_gestion & "','" & date1al & "'," &
                     stru_envi.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW WEB','" & campos_trans & "'," & id_usuario_gestion_envio & ")"
        Dim update_respuesta As String = "Update ra_respuesta_radicado set ID_REMIT_DEST_INT=" & id_usuario_gestion_envio &
       ",ID_AREA=" & id_area & ",AREA_RESPONSABLE='" & nombre_area & "',USUARIO_RESPONSABLE='" & nombre_usuario_gestion & "'" &
       ",CARGO_RESPONSABLE='" & cargo_usuario_gestion & "'" & " where RADICADO='" & Radicado & "'"

        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS,id_usuario_gestion_afectado) values " &
                                             isert_datos
        Dim myConnection As New MySqlConnection
        Dim ref_conect As New conect.Dbase_Conction_Mysql_RA
        ref_conect.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '--------------------------------------------------
            'Reasigna respuesta radicado
            '--------------------------------------------------
            myCommand.CommandText = update_respuesta
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_envia_tarea_usuario = "Imposible reasignar respuesta "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_envia_tarea_usuario = "Imposible registrar log "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Result = Refclas.Terminar_Tarea_Workflow(id_usuario,
                                                     id_actividad,
                                                     id_tarea_selecionada,
                                                     "",
                                                     pag,
                                                     Resultado_evalua_terminar,
                                                     0,
                                                     "",
                                                     id_flujo_trabajo,
                                                     id_actividad_flujo_trabajo,
                                                     id_usuario_workflow_flujo_trabajo)
            If Result <> "YES" Then
                myTrans.Rollback()
                myConnection.Close()
                Reasigna_respuesta_envia_tarea_usuario = "Imposible Terminar Tarea " + id_tarea_selecionada + " " + Result
                Exit Function
            End If

            '-----------------------------------------------------------------------
            'Reasigna los documentos de colaboración relacionados con el trámite
            '-----------------------------------------------------------------------
            If sql_actualiza_solicitante_colaboracion <> "" Then
                myCommand.CommandText = sql_actualiza_solicitante_colaboracion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Reasigna_respuesta_envia_tarea_usuario = "Imposible regisignar trámites de colaboración "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
        Catch ex As Exception
            If Not myConnection Is Nothing Then
                If myConnection.State = ConnectionState.Open Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Reasigna_respuesta_envia_tarea_usuario = "Inconistencia general función Reasigna_respuesta_envia_tarea_usuario " & ex.Message
                    Exit Function
                End If
            Else
                Reasigna_respuesta_envia_tarea_usuario = "Inconistencia general función Reasigna_respuesta_envia_tarea_usuario " & ex.Message
                Exit Function
            End If
        End Try
        Dim Refclas_gestion_respuesta As New Classgestionrespuesta
        Dim correo_electronico As String = ""
        Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
        Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(id_usuario_gestion_envio,
                                                                          correo_electronico)
        If Result <> "YES" Then
            resultadocorreo = "El trámite se envió pero no se notifico al correo electrónico por el siguiente error " & Result
            Reasigna_respuesta_envia_tarea_usuario = "YES"
            Exit Function
        End If
        Dim tramite As String = ""
        Dim Fecha_vence As String = ""
        Dim fecha_registro As String = ""
        Dim destinatario As String = ""
        Dim asunto As String = ""
        Result = Me.Retorna_detalle_respuesta_radicado(Radicado, tramite, Fecha_vence, fecha_registro, destinatario, asunto)
        If Result <> "YES" Then
            resultadocorreo = "El trámite se envió pero no se notifico al correo electrónico por el siguiente error " & Result
            Reasigna_respuesta_envia_tarea_usuario = "YES"
            Exit Function
        End If
        Dim split_notificacion() As String = {"Trámite Reasignado : " & Radicado & " Tipo tramite " & tramite, "Fecha de radicación : " & fecha_registro _
                , "Fecha límite de respuesta : " & Fecha_vence, "Remite : " & destinatario, "Asunto : " & asunto, "Radicado : " & Radicado,
                "Para tramitar este radicado por favor ingrese al cliente workflow.net ", "Este tramite lo reasigno el usuario " & nombre_usuario_gestion_transaccion & " Del cargo " & cargo_usuario_gestion_transaccion & " en la fecha " & date1al}
        asunto = "Reasignación de tramite " & tramite & " Fecha vencimiento  " & Fecha_vence
        Try
            Dim refclascorreo As New ClassCorreo
            Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion, correo_electronico, asunto)
            If Result <> "YES" Then
                resultadocorreo = "El trámite se envió pero no se notifico al correo electrónico por el siguiente error " & Result
                Reasigna_respuesta_envia_tarea_usuario = "YES"
                Exit Function
            End If
            Reasigna_respuesta_envia_tarea_usuario = "YES"
            Exit Function
        Catch ex As Exception
            resultadocorreo = "El trámite se envió pero no se notifico al correo electrónico por el siguiente error " & ex.Message
            Reasigna_respuesta_envia_tarea_usuario = "YES"
            Exit Function
        End Try
    End Function
    Function Reasigna_respuesta_envia_tarea_usuario_batch(ByVal id_usuario_gestion As Integer,
                                                          ByVal id_tarea_selecionada As Integer,
                                                          ByVal id_usuario_workflow_envio As Integer,
                                                          ByVal id_actividad As Integer,
                                                          ByVal id_usuario As Integer,
                                                          ByVal id_pendiente As Integer,
                                                          ByVal nombre_actividad As String,
                                                          ByRef resultadocorreo As String,
                                                          ByVal usuario_transac As String,
                                                          ByVal id_flujo_trabajo As Integer,
                                                          ByVal id_actividad_flujo_trabajo As Integer,
                                                          ByVal id_usuario_workflow_flujo_trabajo As Integer,
                                                          ByVal estado_actividad As Integer) As String
        '*******************************************************
        'Fución : Envia y reasigna tarea a usuario de gestion
        'Fecha : 2015-03-21
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************
        Dim Refclas As New ClassWorkflow
        Dim Result As String = ""
        Dim id_usuario_gestion_envio As Integer = 0
        '-------------------------------------------------------------
        'Verifica existencia relacion usuario de envio seleccionado
        '-------------------------------------------------------------
        Dim nombre_usuario_gestion As String = ""
        Dim cargo_usuario_gestion As String = ""
        Dim Refclas_resp As New Classgestionrespuesta
        Dim Refclas_gestor As New ClassGestorDocumental
        Result = Refclas_gestor.SolicitaIdUsuarioGestionRelacionadoUsuarioWorkflow(id_usuario_workflow_envio,
                                                          id_usuario_gestion_envio)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario_batch = "Imposible relacionar el usuario seleccionado con el usuario de gestión " & Result
            Exit Function
        End If
        If id_usuario_gestion_envio = 0 Then
            Reasigna_respuesta_envia_tarea_usuario_batch = "El usuario seleccionado no tiene relacionado usuario de gestión "
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna datos usuario de gestion al que le reasigna el tramite
        '--------------------------------------------------------------
        Dim nombre_area As String = ""
        Dim id_area As Integer = 0
        Result = Retorna_datos_asignacion_respuesta_usuario_gestion(id_usuario_gestion_envio,
                                                                    id_area,
                                                                    nombre_area,
                                                                    nombre_usuario_gestion,
                                                                    cargo_usuario_gestion)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario_batch = Result
            Exit Function
        End If

        Result = Refclas_resp.Verifica_Existencia_Tabla("ra_respuesta_radicado")
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario_batch = Result
            Exit Function
        End If
        Dim Radicado As String = ""
        Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea_selecionada,
                                                                                Radicado)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario_batch = Result
            Exit Function
        End If
        If Radicado = "" Then
            Reasigna_respuesta_envia_tarea_usuario_batch = "La tarea seleccionada no tiene radicado relacionado para reasignar "
            Exit Function
        End If
        Dim stru_envi As stru_envio = Nothing
        Result = Refclas_resp.Retorna_datos_estructura_envio_por_radicado(Radicado, stru_envi)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario_batch = Result
            Exit Function
        End If
        If stru_envi.ID_REMIT_DEST_INT <> id_usuario_gestion Then
            Reasigna_respuesta_envia_tarea_usuario_batch = "El usuario de gestión no tiene asignada la respuesta para reasignar y enviar"
            Exit Function
        End If
        '--------------------------------------------------------------
        'Retorna documentos de colaboración relacionados con el tramite
        '--------------------------------------------------------------
        Dim numero_documentos_compartidos_relacionados As Integer = 0
        Dim ref_clas_doc_comp As New ClassGaCompartirDocumento
        Dim sql_actualiza_solicitante_colaboracion As String = ""
        Result = ref_clas_doc_comp.Numero_documentos_compartidos_relacionados_a_un_radicado(stru_envi.RADICADO,
                                                                                            numero_documentos_compartidos_relacionados)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario_batch = Result
            Exit Function
        End If
        If numero_documentos_compartidos_relacionados > 0 Then
            sql_actualiza_solicitante_colaboracion = "UPDATE ra_cd_documentos_compartidos SET Remit_Dest_Interno_id_Remit_Dest_Int=" & id_usuario_gestion_envio &
                " where RADICADO_RELACIONADO='" & stru_envi.RADICADO & "' and TIPO_REGISTRO_COMPARTIDO=3"
        End If
        Dim hor As String = Now
        Dim detalle_trans As String = "REASIGNACION"
        Dim campos_trans As String = ""
        Dim isert_datos As String = ""
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Reasigna_respuesta_envia_tarea_usuario_batch = Result
            Exit Function
        End If
        campos_trans = UCase(HttpContext.Current.Session.Item("Login_Usuario_Workfow")) & " REASIGNA RESPUESTA NUMERO (" & stru_envi.ID_RESPUESTA_RADICADO &
        ") DEL RADICADO " & Radicado & " AL USUARIO DE GESTION ID : " & id_usuario_gestion_envio & " NOMBRE : " & nombre_usuario_gestion &
        " CARGO : " & cargo_usuario_gestion
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("Login_Usuario_Workfow") & "','" & id_usuario_gestion & "','" & date1al & "'," &
                     stru_envi.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','GESTION CORRESPODENCIA','" & campos_trans & "'," & id_usuario_gestion_envio & ")"

        Dim update_respuesta As String = "Update ra_respuesta_radicado set ID_REMIT_DEST_INT=" & id_usuario_gestion_envio &
      ",ID_AREA=" & id_area & ",AREA_RESPONSABLE='" & nombre_area & "',USUARIO_RESPONSABLE='" & nombre_usuario_gestion & "'" &
      ",CARGO_RESPONSABLE='" & cargo_usuario_gestion & "'" & " where RADICADO='" & Radicado & "'"

        Dim update_gestion = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                             ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS,id_usuario_gestion_afectado) values " &
                                             isert_datos

        Dim myConnection As New MySqlConnection
        Dim ref_conect As New conect.Dbase_Conction_Mysql_RA
        ref_conect.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Try
            'myConnection.Open()
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            '--------------------------------------------------
            'Reasigna respuesta radicado
            '--------------------------------------------------
            myCommand.CommandText = update_respuesta
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_envia_tarea_usuario_batch = "Imposible reasignar respuesta "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_gestion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Reasigna_respuesta_envia_tarea_usuario_batch = "Imposible registrar log de envio  de gestión "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Result = Refclas.Terminar_Tarea_Workflow_Bacth(id_usuario,
                                                           id_actividad,
                                                           id_pendiente,
                                                           id_tarea_selecionada,
                                                           nombre_actividad,
                                                           id_flujo_trabajo,
                                                           id_actividad_flujo_trabajo,
                                                           id_usuario_workflow_flujo_trabajo,
                                                           0,
                                                           "",
                                                           estado_actividad)
            If Result <> "YES" Then
                myTrans.Rollback()
                myConnection.Close()
                Reasigna_respuesta_envia_tarea_usuario_batch = "Imposible Terminar Tarea " + id_tarea_selecionada + " " + Result
                Exit Function
            End If
            '-----------------------------------------------------------------------
            'Reasigna los documentos de colaboración relacionados con el trámite
            '-----------------------------------------------------------------------
            If sql_actualiza_solicitante_colaboracion <> "" Then
                myCommand.CommandText = sql_actualiza_solicitante_colaboracion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Reasigna_respuesta_envia_tarea_usuario_batch = "Imposible regisignar trámites de colaboración "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Dim Refclas_gestion_respuesta As New Classgestionrespuesta
            Dim correo_electronico As String = ""
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(id_usuario_gestion_envio,
                                                                              correo_electronico)
            If Result <> "YES" Then
                resultadocorreo = "El trámite se envío pero no se notifico al correo " & Result
                Reasigna_respuesta_envia_tarea_usuario_batch = "YES"
                Exit Function
            End If
            Dim tramite As String = ""
            Dim Fecha_vence As String = ""
            Dim fecha_registro As String = ""
            Dim destinatario As String = ""
            Dim asunto As String = ""
            Result = Me.Retorna_detalle_respuesta_radicado(Radicado,
                                                           tramite,
                                                           Fecha_vence,
                                                           fecha_registro,
                                                           destinatario, asunto)
            If Result <> "YES" Then
                resultadocorreo = "El trámite se envío pero no se notifico al correo por " & Result
                Reasigna_respuesta_envia_tarea_usuario_batch = "YES"
                Exit Function
            End If
            Dim split_notificacion() As String = {"Trámite Reasignado : " & Radicado & " Tipo tramite " & tramite, "Fecha de radicación : " & fecha_registro _
                    , "Fecha límite de respuesta : " & Fecha_vence, "Remite : " & destinatario, "Asunto : " & asunto, "Radicado : " & Radicado,
                    "Para tramitar este radicado por favor ingrese al modulo Gestión de correspondencia ", "Este tramite lo reasigno el usuario " & nombre_usuario_gestion & " Del cargo " & cargo_usuario_gestion & " en la fecha " & date1al}
            asunto = "Reasignación de tramite " & tramite & " Fecha vencimiento  " & Fecha_vence
            Dim refclascorreo As New ClassCorreo
            Result = refclascorreo.Envio_Correo_notificacion_asignacion(split_notificacion,
                                                                        correo_electronico,
                                                                        asunto)
            If Result <> "YES" Then
                resultadocorreo = "El trámite se envío pero no se notifico al correo por " & Result
                Reasigna_respuesta_envia_tarea_usuario_batch = "YES"
                Exit Function
            End If
            Reasigna_respuesta_envia_tarea_usuario_batch = "YES"
        Catch ex As Exception
            myTrans.Rollback()
            myConnection.Close()
            Reasigna_respuesta_envia_tarea_usuario_batch = ex.Message
            'Exit Function
        End Try
    End Function
    Function Generar_interface_responder_solicitud_gestion_respuesta(ByVal id_respuesta As Integer,
                                                                     ByVal id_usuario_propietario As Integer,
                                                                     ByRef pag As Page) As String
        Try
            Dim Check_adjunta_formato As CheckBox = pag.FindControl("Check_adjunta_formato")
            Dim CheckBox_adjunta_documento_libre As CheckBox = pag.FindControl("CheckBox_adjunta_documento_libre")
            Dim DropDownList_lista_firmas_adjunto_archivo As DropDownList =
                                                               pag.FindControl("DropDownList_lista_firmas_adjunto_archivo")
            Dim Panel_sube_plantilla_respuesta As Panel = pag.FindControl("Panel_sube_plantilla_respuesta")
            Dim UpdatePanel_Panel_descarga_ajax As UpdatePanel = pag.FindControl("UpdatePanel_Panel_descarga_ajax")
            Dim Panel_descarga_externo As Panel = pag.FindControl("Panel_descarga_externo")
            Dim UpdatePanel_descarga_formato_adjunto_archivo As UpdatePanel = pag.FindControl("UpdatePanel_descarga_formato_adjunto_archivo")
            Dim UpdatePane_opcion_adjunta As UpdatePanel = pag.FindControl("UpdatePane_opcion_adjunta")
            Dim Hidden_id_respuesta As Object = pag.FindControl("Hidden_id_respuesta")
            Dim Label_estado_carga As Label = pag.FindControl("Label_estado_sube_plantilla_respuesta")
            Dim AjaxFileUpload_sube_plantilla_respuesta As AjaxControlToolkit.AjaxFileUpload = pag.FindControl("AjaxFileUpload_sube_plantilla_respuesta")
            Dim UpdatePanel_sube_plantilla_respuesta As UpdatePanel = pag.FindControl("UpdatePanel_sube_plantilla_respuesta")
            Dim Result As String = ""
            Dim refclas_ra_resp_radicado As New Class_ra_respuesta_radicado
            Dim refclas_ra_ra_registro_dwo_formato As New Class_ra_ra_registro_down_formato
            Dim reflas_ra_ra_rela_firm_auto As New Class_relacion_firmas_autorizadas
            Dim refclas_gestion As New Classgestionrespuesta
            Dim stru_respuesta As stru_envio = Nothing
            Dim estado As String = ""
            Dim Ref_clas_rc_solicitudes As New ClassRaSolicitudesAprobacion
            Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta,
                                                                                                  0,
                                                                                                  estado,
                                                                                                  HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = Result
                Exit Function
            End If
            If estado = "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = "El sistema ha detectado solicitudes de aprobación pendientes por decisión, no se puede carga un nuevo documento "
                Exit Function
            End If
            estado = ""
            Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(Hidden_id_respuesta.Value,
                                                                                                        1,
                                                                                                        estado,
                                                                                                        HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = Result
                Exit Function
            End If
            If estado = "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = "El sistema ha detectado una solicitud donde se aprueba el documento de respuesta, no se puede carga un nuevo documento"
                Exit Function
            End If
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Hidden_id_respuesta.Value,
                                                                                id_imagen_plantilla,
                                                                                radicado_respuesta,
                                                                                fecha_respuesta,
                                                                                id_imagen_respuesta,
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = Result
                Exit Function
            End If
            If fecha_respuesta <> 0 And radicado_respuesta <> 0 Then
                Generar_interface_responder_solicitud_gestion_respuesta = "El tramite ya tiene una respuesta asociada, imposible cargar documento"
                Exit Function
            End If

            Result = refclas_ra_resp_radicado.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                               stru_respuesta, 1)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = Result
                Exit Function
            End If
            Dim id_user_gestion As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            If stru_respuesta.ID_REMIT_DEST_INT <>
                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                Generar_interface_responder_solicitud_gestion_respuesta = "El usuario de gestión no es el autorizado para gestionar la respuesta"
                Exit Function
            End If
            Dim id_usuario_respuesta As Integer = 0
            Result = refclas_ra_ra_registro_dwo_formato.Solicita_utltimo_usuario_firma_formato_descarga(id_respuesta,
                                                                                                        id_usuario_respuesta)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = Result
                Exit Function
            End If
            Dim stru_firmas As stru_usu_firmas_autorizadas() = Nothing
            Result = reflas_ra_ra_rela_firm_auto.Solicita_lista_usuarios_permitidos_firma(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                         stru_firmas)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = Result
                Exit Function
            End If
            Result = refclas_gestion.Lista_usuarios_firmas_permitidas_iterface(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                    stru_firmas,
                                                                                    id_usuario_respuesta,
                                                                                    DropDownList_lista_firmas_adjunto_archivo,
                                                                                    UpdatePanel_descarga_formato_adjunto_archivo)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud_gestion_respuesta = Result
                Exit Function
            End If
            'If stru_respuesta.RADICADO_RESPUESTA = "" Then
            '    CheckBox_adjunta_documento_libre.Checked = True
            '    Check_adjunta_formato.Checked = False
            '    UpdatePane_opcion_adjunta.Update()
            'Else
            '    Check_adjunta_formato.Checked = True
            '    CheckBox_adjunta_documento_libre.Checked = False
            '    UpdatePane_opcion_adjunta.Update()
            'End If
            Dim extension As String = ""
            If HttpContext.Current.Session.Item("TIPO_ADJUNTA_STATE") = 1 Then
                Panel_sube_plantilla_respuesta.Visible = True
                Panel_descarga_externo.Visible = False
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                AjaxFileUpload_sube_plantilla_respuesta.MaximumNumberOfFiles = 1
                If CheckBox_adjunta_documento_libre.Checked = True Then
                    AjaxFileUpload_sube_plantilla_respuesta.OnClientUploadComplete = "activa_boton_dowload_sube_plantilla"
                    Result = Actualiza_mensaje_tipo_adjunta(2,
                                                            Label_estado_carga.Text,
                                                            AjaxFileUpload_sube_plantilla_respuesta,
                                                            UpdatePanel_Panel_descarga_ajax,
                                                            UpdatePanel_sube_plantilla_respuesta)
                    If Result <> "YES" Then
                        Generar_interface_responder_solicitud_gestion_respuesta = Result
                        Exit Function
                    End If
                Else

                    AjaxFileUpload_sube_plantilla_respuesta.OnClientUploadComplete = "activa_boton_dowload_sube_plantilla"
                    Result = Actualiza_mensaje_tipo_adjunta(1,
                                                            Label_estado_carga.Text,
                                                            AjaxFileUpload_sube_plantilla_respuesta,
                                                            UpdatePanel_Panel_descarga_ajax,
                                                            UpdatePanel_sube_plantilla_respuesta)
                    If Result <> "YES" Then
                        Generar_interface_responder_solicitud_gestion_respuesta = Result
                        Exit Function
                    End If
                End If
            Else
                Panel_descarga_externo.Visible = True
                Panel_sube_plantilla_respuesta.Visible = False
                UpdatePanel_Panel_descarga_ajax.Update()
            End If
            Generar_interface_responder_solicitud_gestion_respuesta = "YES"
            Exit Function
        Catch ex As Exception
            Generar_interface_responder_solicitud_gestion_respuesta = "Inconsistencia general función Generar_interface_responder_solicitud_gestion_respuesta " & ex.Message
        End Try
    End Function
    Function Generar_interface_responder_solicitud(ByVal id_respuesta As Integer,
                                                   ByVal id_usuario_propietario As Integer,
                                                   ByRef pag As Page) As String
        Try
            Dim Check_adjunta_formato As CheckBox = pag.FindControl("Check_adjunta_formato")
            Dim CheckBox_adjunta_documento_libre As CheckBox = pag.FindControl("CheckBox_adjunta_documento_libre")
            Dim DropDownList_lista_firmas_adjunto_archivo As DropDownList =
                                                               pag.FindControl("DropDownList_lista_firmas_adjunto_archivo")
            Dim Panel_descarga_ajax As Panel = pag.FindControl("Panel_descarga_ajax")
            Dim UpdatePanel_Panel_descarga_ajax As UpdatePanel = pag.FindControl("UpdatePanel_Panel_descarga_ajax")
            Dim Panel_descarga_externo As Panel = pag.FindControl("Panel_descarga_externo")
            Dim UpdatePanel_descarga_formato_adjunto_archivo As UpdatePanel = pag.FindControl("UpdatePanel_descarga_formato_adjunto_archivo")
            Dim UpdatePane_opcion_adjunta As UpdatePanel = pag.FindControl("UpdatePane_opcion_adjunta")
            Dim Hidden_id_respuesta As Object = pag.FindControl("Hidden_id_respuesta")
            Dim Label_estado_carga As Label = pag.FindControl("Label_estado_carga")
            Dim AjaxFileUpload_dowload As AjaxControlToolkit.AjaxFileUpload = pag.FindControl("AjaxFileUpload_dowload")
            Dim UpdatePanel_descarga As UpdatePanel = pag.FindControl("UpdatePanel_descarga")
            Dim Result As String = ""
            Dim refclas_ra_resp_radicado As New Class_ra_respuesta_radicado
            Dim refclas_ra_ra_registro_dwo_formato As New Class_ra_ra_registro_down_formato
            Dim reflas_ra_ra_rela_firm_auto As New Class_relacion_firmas_autorizadas
            Dim refclas_gestion As New Classgestionrespuesta
            Dim stru_respuesta As stru_envio = Nothing
            Dim estado As String = ""
            Dim Ref_clas_rc_solicitudes As New ClassRaSolicitudesAprobacion
            Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(id_respuesta,
                                                                                                  0,
                                                                                                  estado,
                                                                                                  HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Generar_interface_responder_solicitud = Result
                Exit Function
            End If
            If estado = "YES" Then
                Generar_interface_responder_solicitud = "El sistema ha detectado solicitudes de aprobación pendientes por decisión, no se puede carga un nuevo documento "
                Exit Function
            End If
            estado = ""
            Result = Ref_clas_rc_solicitudes.Retorna_estados_solicitudes_de_aprobacion_por_id_respuesta(Hidden_id_respuesta.Value,
                                                                                                        1,
                                                                                                        estado,
                                                                                                        HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Generar_interface_responder_solicitud = Result
                Exit Function
            End If
            If estado = "YES" Then
                Generar_interface_responder_solicitud = "El sistema ha detectado una solicitud donde se aprueba el documento de respuesta, no se puede carga un nuevo documento"
                Exit Function
            End If
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(Hidden_id_respuesta.Value,
                                                                                id_imagen_plantilla,
                                                                                radicado_respuesta,
                                                                                fecha_respuesta,
                                                                                id_imagen_respuesta,
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud = Result
                Exit Function
            End If
            If fecha_respuesta <> 0 And radicado_respuesta <> 0 Then
                Generar_interface_responder_solicitud = "El tramite ya tiene una respuesta asociada, imposible cargar documento"
                Exit Function
            End If

            Result = refclas_ra_resp_radicado.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                               stru_respuesta, 1)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud = Result
                Exit Function
            End If
            Dim id_user_gestion As Integer = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            If stru_respuesta.ID_REMIT_DEST_INT <>
                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                Generar_interface_responder_solicitud = "El usuario de gestión no es el autorizado para gestionar la respuesta"
                Exit Function
            End If
            Dim id_usuario_respuesta As Integer = 0
            Result = refclas_ra_ra_registro_dwo_formato.Solicita_utltimo_usuario_firma_formato_descarga(id_respuesta,
                                                                                                        id_usuario_respuesta)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud = Result
                Exit Function
            End If
            Dim stru_firmas As stru_usu_firmas_autorizadas() = Nothing
            Result = reflas_ra_ra_rela_firm_auto.Solicita_lista_usuarios_permitidos_firma(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                         stru_firmas)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud = Result
                Exit Function
            End If
            Result = refclas_gestion.Lista_usuarios_firmas_permitidas_iterface(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                    stru_firmas,
                                                                                    id_usuario_respuesta,
                                                                                    DropDownList_lista_firmas_adjunto_archivo,
                                                                                    UpdatePanel_descarga_formato_adjunto_archivo)
            If Result <> "YES" Then
                Generar_interface_responder_solicitud = Result
                Exit Function
            End If
            If stru_respuesta.RADICADO_RESPUESTA = "" Then
                CheckBox_adjunta_documento_libre.Checked = True
                Check_adjunta_formato.Checked = False
                UpdatePane_opcion_adjunta.Update()
            Else
                Check_adjunta_formato.Checked = True
                CheckBox_adjunta_documento_libre.Checked = False
                UpdatePane_opcion_adjunta.Update()
            End If
            Dim extension As String = ""
            If HttpContext.Current.Session.Item("TIPO_ADJUNTA_STATE") = 1 Then
                Panel_descarga_ajax.Visible = True
                Panel_descarga_externo.Visible = False
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
                If CheckBox_adjunta_documento_libre.Checked = True Then
                    AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
                    Result = Actualiza_mensaje_tipo_adjunta(2,
                                                   Label_estado_carga.Text,
                                                   AjaxFileUpload_dowload,
                                                   UpdatePanel_Panel_descarga_ajax,
                                                   UpdatePanel_descarga)
                    If Result <> "YES" Then
                        Generar_interface_responder_solicitud = Result
                        Exit Function
                    End If
                Else

                    AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
                    Result = Actualiza_mensaje_tipo_adjunta(1,
                                                   Label_estado_carga.Text,
                                                   AjaxFileUpload_dowload,
                                                   UpdatePanel_Panel_descarga_ajax,
                                                   UpdatePanel_descarga)
                    If Result <> "YES" Then
                        Generar_interface_responder_solicitud = Result
                        Exit Function
                    End If
                End If
            Else
                Panel_descarga_externo.Visible = True
                Panel_descarga_ajax.Visible = False
                UpdatePanel_Panel_descarga_ajax.Update()
            End If
            Generar_interface_responder_solicitud = "YES"
            Exit Function
        Catch ex As Exception
            Generar_interface_responder_solicitud = "Inconsistencia general función Generar_interface_responder_solicitud " & ex.Message
        End Try
    End Function
    Function Actualiza_mensaje_tipo_adjunta(ByVal id_tipo_ajunta As Integer,
                                           ByRef label_adjunta As String,
                                           ByRef AjaxFileUpload_dowload As AjaxControlToolkit.AjaxFileUpload,
                                           ByRef update_descarga_ajax As UpdatePanel,
                                           ByRef UpdatePanel As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim extension As String = ""
            Result = Refclas.Solicita_tipos_archivo_adjunta_respuesta(id_tipo_ajunta,
                                                                      AjaxFileUpload_dowload,
                                                                      extension)
            If Result <> "YES" Then
                Actualiza_mensaje_tipo_adjunta = Result
                Exit Function
            End If
            If id_tipo_ajunta = 1 Then
                label_adjunta = "Por favor cargue el archivo del formato de respuesta del tipo " & "." & extension
            Else
                label_adjunta = "Por favor cargue el archivo en los formatos " & extension
            End If
            Actualiza_mensaje_tipo_adjunta = "YES"
            Exit Function
        Catch ex As Exception
            Actualiza_mensaje_tipo_adjunta = "Inconsistencia general función Actualiza_mensaje_tipo_adjunta " & ex.Message
        Finally
            update_descarga_ajax.Update()
            UpdatePanel.Update()
        End Try
    End Function
    Function Solicita_tipos_archivo_adjunta_respuesta(ByVal tipo_adjunto_respuesta As Integer,
                                                      ByVal AjaxFileUpload_dowload As AjaxControlToolkit.AjaxFileUpload,
                                                      ByRef extension_permitida As String) As String
        Try
            If tipo_adjunto_respuesta = 1 Then
                extension_permitida = "docx"
                AjaxFileUpload_dowload.AllowedFileTypes = extension_permitida
            Else
                extension_permitida = "xls,xlsx,docx,doc,txt,pdf,jpg,ppt,pptx,bmp,tif,tiff,pdfa,pdf"
                AjaxFileUpload_dowload.AllowedFileTypes = extension_permitida
            End If
            Solicita_tipos_archivo_adjunta_respuesta = "YES"
        Catch ex As Exception
            Solicita_tipos_archivo_adjunta_respuesta = "Inconsistencia general función Solicita_tipos_archivo_adjunta_respuesta " & ex.Message
        End Try
    End Function
    Function Lista_usuarios_firmas_permitidas_iterface(ByVal id_usuario_autorizado As Integer,
                                              ByVal stru_usu_firmas_autoriza() As stru_usu_firmas_autorizadas,
                                              ByVal id_usuario_autorizado_default As Integer,
                                              ByRef drop_list As DropDownList,
                                              ByRef upda_date As UpdatePanel) As String
        Try
            drop_list.Items.Clear()
            Dim ilis_ As System.Web.UI.WebControls.ListItem
            ilis_ = New System.Web.UI.WebControls.ListItem
            ilis_.Text = "A nombre propio"
            ilis_.Value = id_usuario_autorizado
            drop_list.Items.Add(ilis_)
            If Not stru_usu_firmas_autoriza Is Nothing Then
                For i As Integer = 0 To stru_usu_firmas_autoriza.Length - 1
                    ilis_ = New System.Web.UI.WebControls.ListItem
                    ilis_.Text = stru_usu_firmas_autoriza(i).nombre_usuario_autorizado & " (" & stru_usu_firmas_autoriza(i).nombre_cargo_autorizado & ")"
                    ilis_.Value = stru_usu_firmas_autoriza(i).id_usuario_autorizado
                    drop_list.Items.Add(ilis_)
                Next
            End If
            For i As Integer = 0 To drop_list.Items.Count - 1
                If drop_list.Items(i).Value = id_usuario_autorizado_default Then
                    drop_list.Items(i).Selected = True
                    Exit For
                End If
            Next
            Lista_usuarios_firmas_permitidas_iterface = "YES"
        Catch ex As Exception
            Lista_usuarios_firmas_permitidas_iterface = "Inconsistencia función Lista_usuarios_firmas_permitidas_iterface " & ex.Message
        Finally
            upda_date.Update()
        End Try
    End Function
    Function Lista_usuarios_firmas_permitidas_iterface_respuesta(ByVal id_usuario_autorizado As Integer,
                                                                 ByVal stru_usu_firmas_autoriza() As stru_usu_firmas_autorizadas,
                                                                 ByVal id_usuario_autorizado_default As Integer,
                                                                 ByRef drop_list As DropDownList,
                                                                 ByRef upda_date As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim estado_coincide As String = ""
            Dim ilis_ As System.Web.UI.WebControls.ListItem
            If drop_list.Items.Count = 0 Then
                ilis_ = New System.Web.UI.WebControls.ListItem
                ilis_.Text = "A nombre propio"
                ilis_.Value = id_usuario_autorizado
                drop_list.Items.Add(ilis_)
            End If
            If Not stru_usu_firmas_autoriza Is Nothing Then
                For i As Integer = 0 To stru_usu_firmas_autoriza.Length - 1
                    Result = Me.Busca_coincidencia_usuario_drop_list(stru_usu_firmas_autoriza(i).id_usuario_autorizado,
                                                                   drop_list,
                                                                   estado_coincide)
                    If Result <> "YES" Then
                        Lista_usuarios_firmas_permitidas_iterface_respuesta = Result
                        Exit Function
                    End If
                    If estado_coincide = "NO" Then
                        ilis_ = New System.Web.UI.WebControls.ListItem
                        ilis_.Text = stru_usu_firmas_autoriza(i).nombre_usuario_autorizado & " (" & stru_usu_firmas_autoriza(i).nombre_cargo_autorizado & ")"
                        ilis_.Value = stru_usu_firmas_autoriza(i).id_usuario_autorizado
                        drop_list.Items.Add(ilis_)
                    End If
                Next
            End If
            For i As Integer = 0 To drop_list.Items.Count - 1
                If drop_list.Items(i).Value = id_usuario_autorizado_default Then
                    drop_list.Items(i).Selected = True
                    Exit For
                End If
            Next
            Lista_usuarios_firmas_permitidas_iterface_respuesta = "YES"
        Catch ex As Exception
            Lista_usuarios_firmas_permitidas_iterface_respuesta = "Inconsistencia función Lista_usuarios_firmas_permitidas_iterface_respuesta " & ex.Message
        Finally
            upda_date.Update()
        End Try
    End Function
    Function Busca_coincidencia_usuario_drop_list(ByVal id_usuario_autorizado As Integer,
                                                  ByVal drop_list As DropDownList,
                                                  ByRef estado_conside As String) As String
        Try
            estado_conside = "NO"
            For i As Integer = 0 To drop_list.Items.Count - 1
                If drop_list.Items(i).Value = id_usuario_autorizado Then
                    estado_conside = "YES"
                    Exit For
                End If
            Next
            Busca_coincidencia_usuario_drop_list = "YES"
        Catch ex As Exception
            Busca_coincidencia_usuario_drop_list = "Inconsistencia general función Busca_coincidencia_usuario_drop_list " & ex.Message
        End Try
    End Function
    Function Subir_respuesta_radicado(ByVal id_respuesta As Integer,
                                      ByVal ruta_documento As String,
                                      ByRef Consecutivo_radicado_entrante As String,
                                      ByRef id_imagen As Integer,
                                      ByRef id_tipo_envio_respuesta As Integer,
                                      ByRef image As Image,
                                      ByRef update As UpdatePanel) As String
        Try
            Dim Refclas_resp As New Class_ra_respuesta_radicado
            Dim Refclas_system As New Class_system_plantilla_radicado
            Dim refclas_tip_doc_entrante As New Class_tipo_doc_entrante
            Dim Reclas_radicador As New ClassRadicador
            Dim stru_envi As stru_envio
            Dim id_plantilla_default As Integer = 0
            Dim nombre_plantilla_default As String = ""
            Dim nombre_plantilla_radicado As String = ""
            Dim tramite_homologado_respuesta As String = ""
            Dim id_tipo_tramite As Integer = 0
            Dim Result As String = ""
            Result = Refclas_resp.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                   stru_envi)
            If Result <> "YES" Then
                Subir_respuesta_radicado = Result
                Exit Function
            End If
            '------------------------------------------
            'Radicado respuesta
            '------------------------------------------
            If stru_envi.RADICADO_RESPUESTA = "" Then
                Result = Refclas_system.Solicita_plantilla_default_respuesta(id_plantilla_default,
                                                                           nombre_plantilla_default)
                If Result <> "YES" Then
                    Subir_respuesta_radicado = Result
                    Exit Function
                End If
                Result = refclas_tip_doc_entrante.Solicita_tipo_tramite_respuesta(id_plantilla_default,
                                                                                stru_envi.TRAMITE_DOCUMENTO,
                                                                                tramite_homologado_respuesta)
                If Result <> "YES" Then
                    Subir_respuesta_radicado = Result
                    Exit Function
                End If
                Result = Reclas_radicador.Registra_Radicado_plantilla_respuesta("RADICACIONSALIENTE",
                                                                              Consecutivo_radicado_entrante,
                                                                              stru_envi,
                                                                              id_respuesta,
                                                                              HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                              stru_envi.DESTINATARIO,
                                                                               tramite_homologado_respuesta, 2)
                If Result <> "YES" Then
                    Subir_respuesta_radicado = Result
                    Exit Function
                End If
            End If
            Result = Me.Guardar_Documento_Respuesta(id_imagen,
                                                  "IMP03GESTIONTMP",
                                                  id_respuesta,
                                                  ruta_documento, 2)
            If Result <> "YES" Then
                Subir_respuesta_radicado = Result
                Exit Function
            End If
            Result = Refclas_system.Solicita_nombre_plantilla_radicado(stru_envi.system_plantilla_radicado_id_plantilla,
                                                                      nombre_plantilla_radicado)
            If Result <> "YES" Then
                Subir_respuesta_radicado = Result
                Exit Function
            End If
            Result = Refclas_system.Solicita_id_tipo_tramite_plantilla_radicado(stru_envi.RADICADO,
                                                                                nombre_plantilla_radicado,
                                                                                id_tipo_tramite)
            If Result <> "YES" Then
                Subir_respuesta_radicado = Result
                Exit Function
            End If
            Result = refclas_tip_doc_entrante.Solicita_tipo_envio_respuesta(id_tipo_tramite,
                                                                            id_tipo_envio_respuesta)
            If Result <> "YES" Then
                Subir_respuesta_radicado = Result
                Exit Function
            End If
            If id_tipo_envio_respuesta = 0 Then
                Result = Refclas_resp.Solicita_estados_semaforo_respuesta(id_respuesta, image)
                If Result <> "YES" Then
                    Subir_respuesta_radicado = Result
                    Exit Function
                End If
            Else
                Result = Refclas_resp.Solicita_estados_semaforo_respuesta_electronica(id_respuesta, image)
                If Result <> "YES" Then
                    Subir_respuesta_radicado = Result
                    Exit Function
                End If
            End If
            Subir_respuesta_radicado = "YES"
        Catch ex As Exception
            Subir_respuesta_radicado = "Inconsistencia general función Subir_respuesta_radicado " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function upload_subir_respuesta_radicado(ByVal id_respuesta As Integer,
                                             ByVal ruta_documento As String,
                                             ByRef Consecutivo_radicado_entrante As String,
                                             ByRef id_imagen As Integer,
                                             ByRef id_tipo_envio_respuesta As Integer,
                                             ByRef url_image As String) As String
        Try
            Dim Refclas_resp As New Class_ra_respuesta_radicado
            Dim Refclas_system As New Class_system_plantilla_radicado
            Dim refclas_tip_doc_entrante As New Class_tipo_doc_entrante
            Dim Reclas_radicador As New ClassRadicador
            Dim stru_envi As stru_envio = Nothing
            Dim id_plantilla_default As Integer = 0
            Dim nombre_plantilla_default As String = ""
            Dim nombre_plantilla_radicado As String = ""
            Dim tramite_homologado_respuesta As String = ""
            Dim id_tipo_tramite As Integer = 0
            Dim Result As String = ""
            Result = Refclas_resp.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                   stru_envi)
            If Result <> "YES" Then
                upload_subir_respuesta_radicado = Result
                Exit Function
            End If
            '------------------------------------------
            'Radicado respuesta
            '------------------------------------------
            If stru_envi.RADICADO_RESPUESTA = "" Then
                Result = Refclas_system.Solicita_plantilla_default_respuesta(id_plantilla_default,
                                                                           nombre_plantilla_default)
                If Result <> "YES" Then
                    upload_subir_respuesta_radicado = Result
                    Exit Function
                End If
                Result = refclas_tip_doc_entrante.Solicita_tipo_tramite_respuesta(id_plantilla_default,
                                                                                stru_envi.TRAMITE_DOCUMENTO,
                                                                                tramite_homologado_respuesta)
                If Result <> "YES" Then
                    upload_subir_respuesta_radicado = Result
                    Exit Function
                End If
                Result = Reclas_radicador.Registra_Radicado_plantilla_respuesta("RADICACIONSALIENTE",
                                                                              Consecutivo_radicado_entrante,
                                                                              stru_envi,
                                                                              id_respuesta,
                                                                              HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                              stru_envi.DESTINATARIO,
                                                                               tramite_homologado_respuesta, 2)
                If Result <> "YES" Then
                    upload_subir_respuesta_radicado = Result
                    Exit Function
                End If
            End If
            Result = Me.Guardar_Documento_Respuesta(id_imagen,
                                                  "IMP03GESTIONTMP",
                                                  id_respuesta,
                                                  ruta_documento, 2)
            If Result <> "YES" Then
                upload_subir_respuesta_radicado = Result
                Exit Function
            End If
            Result = Refclas_system.Solicita_nombre_plantilla_radicado(stru_envi.system_plantilla_radicado_id_plantilla,
                                                                      nombre_plantilla_radicado)
            If Result <> "YES" Then
                upload_subir_respuesta_radicado = Result
                Exit Function
            End If
            Result = Refclas_system.Solicita_id_tipo_tramite_plantilla_radicado(stru_envi.RADICADO,
                                                                                nombre_plantilla_radicado,
                                                                                id_tipo_tramite)
            If Result <> "YES" Then
                upload_subir_respuesta_radicado = Result
                Exit Function
            End If
            Result = refclas_tip_doc_entrante.Solicita_tipo_envio_respuesta(id_tipo_tramite,
                                                                            id_tipo_envio_respuesta)
            If Result <> "YES" Then
                upload_subir_respuesta_radicado = Result
                Exit Function
            End If
            If id_tipo_envio_respuesta = 0 Then
                Result = Refclas_resp.Solicita_estados_semaforo_respuesta(id_respuesta, url_image)
                If Result <> "YES" Then
                    upload_subir_respuesta_radicado = Result
                    Exit Function
                End If
            Else
                Result = Refclas_resp.Solicita_estados_semaforo_respuesta_electronica(id_respuesta, url_image)
                If Result <> "YES" Then
                    upload_subir_respuesta_radicado = Result
                    Exit Function
                End If
            End If
            upload_subir_respuesta_radicado = "YES"
        Catch ex As Exception
            upload_subir_respuesta_radicado = "Inconsistencia general función upload_subir_respuesta_radicado " & ex.Message

        End Try
    End Function
    Function upload_sube_formato_respuesta_radicado(ByVal id_respuesta As Integer,
                                                    ByVal ruta_documento As String,
                                                    ByRef Consecutivo_radicado_entrante As String,
                                                    ByRef id_imagen As Integer,
                                                    ByRef id_tipo_envio_respuesta As Integer,
                                                    ByRef image_url As String) As String
        Try
            Dim Refclas_resp As New Class_ra_respuesta_radicado
            Dim Refclas_system As New Class_system_plantilla_radicado
            Dim refclas_tip_doc_entrante As New Class_tipo_doc_entrante
            Dim Refclas_gebox As New ClassGaGembox
            Dim Result As String = ""
            Dim stru_envi As stru_envio = Nothing
            Dim nombre_plantilla_radicado As String = ""
            Dim id_tipo_tramite As Integer = 0
            Dim estado_verificacion As String = ""
            Result = Refclas_gebox.Verifica_auntentificacion_doc_respuesta_web_radicado(ruta_documento,
                                                                                        id_respuesta,
                                                                                        estado_verificacion)
            If Result <> "YES" Then
                upload_sube_formato_respuesta_radicado = Result
                Exit Function
            End If
            If estado_verificacion <> "YES" Then
                upload_sube_formato_respuesta_radicado = estado_verificacion
                Exit Function
            End If
            Result = Refclas_resp.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                   stru_envi)
            If Result <> "YES" Then
                upload_sube_formato_respuesta_radicado = Result
                Exit Function
            End If
            If stru_envi.RADICADO_RESPUESTA = "" Then
                upload_sube_formato_respuesta_radicado = "Debe descargar el formato de respuesta o seleccionar la opción (Adjunta documento formato libre)"
                Exit Function
            End If
            Result = Me.Guardar_Documento_Respuesta(id_imagen,
                                                  "IMP03GESTIONTMP",
                                                  id_respuesta,
                                                  ruta_documento, 1)
            If Result <> "YES" Then
                upload_sube_formato_respuesta_radicado = Result
                Exit Function
            End If

            Result = Refclas_system.Solicita_nombre_plantilla_radicado(stru_envi.system_plantilla_radicado_id_plantilla,
                                                                     nombre_plantilla_radicado)
            If Result <> "YES" Then
                upload_sube_formato_respuesta_radicado = Result
                Exit Function
            End If
            Result = Refclas_system.Solicita_id_tipo_tramite_plantilla_radicado(stru_envi.RADICADO,
                                                                                nombre_plantilla_radicado,
                                                                                id_tipo_tramite)
            If Result <> "YES" Then
                upload_sube_formato_respuesta_radicado = Result
                Exit Function
            End If
            Result = refclas_tip_doc_entrante.Solicita_tipo_envio_respuesta(id_tipo_tramite,
                                                                            id_tipo_envio_respuesta)
            If Result <> "YES" Then
                upload_sube_formato_respuesta_radicado = Result
                Exit Function
            End If
            If id_tipo_envio_respuesta = 0 Then
                Result = Refclas_resp.Solicita_estados_semaforo_respuesta(id_respuesta, image_url)
                If Result <> "YES" Then
                    upload_sube_formato_respuesta_radicado = Result
                    Exit Function
                End If
            Else
                Result = Refclas_resp.Solicita_estados_semaforo_respuesta_electronica(id_respuesta, image_url)
                If Result <> "YES" Then
                    upload_sube_formato_respuesta_radicado = Result
                    Exit Function
                End If
            End If
            upload_sube_formato_respuesta_radicado = "YES"
        Catch ex As Exception
            upload_sube_formato_respuesta_radicado = "Inconsistencia general función upload_sube_formato_respuesta_radicado " & ex.Message
        End Try
    End Function
    Function Subir_formato_respuesta_radicado(ByVal id_respuesta As Integer,
                                              ByVal ruta_documento As String,
                                              ByRef Consecutivo_radicado_entrante As String,
                                              ByRef id_imagen As Integer,
                                              ByRef id_tipo_envio_respuesta As Integer,
                                              ByRef image As Image,
                                              ByRef update As UpdatePanel) As String
        Try
            Dim Refclas_resp As New Class_ra_respuesta_radicado
            Dim Refclas_system As New Class_system_plantilla_radicado
            Dim refclas_tip_doc_entrante As New Class_tipo_doc_entrante
            Dim Refclas_gebox As New ClassGaGembox
            Dim Result As String = ""
            Dim stru_envi As stru_envio
            Dim id_plantilla_default As Integer = 0
            Dim nombre_plantilla_default As String = ""
            Dim nombre_plantilla_radicado As String = ""
            Dim tramite_homologado_respuesta As String = ""
            Dim id_tipo_tramite As Integer = 0
            Dim estado_verificacion As String = ""
            Result = Refclas_gebox.Verifica_auntentificacion_doc_respuesta_web_radicado(ruta_documento,
                                                                                        id_respuesta,
                                                                                        estado_verificacion)
            If Result <> "YES" Then
                Subir_formato_respuesta_radicado = Result
                Exit Function
            End If
            If estado_verificacion <> "YES" Then
                Subir_formato_respuesta_radicado = estado_verificacion
                Exit Function
            End If
            Result = Refclas_resp.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                   stru_envi)
            If Result <> "YES" Then
                Subir_formato_respuesta_radicado = Result
                Exit Function
            End If
            If stru_envi.RADICADO_RESPUESTA = "" Then
                Subir_formato_respuesta_radicado = "Debe descargar el formato de respuesta o seleccionar la opción (Adjunta documento formato libre)"
                Exit Function
            End If
            Result = Me.Guardar_Documento_Respuesta(id_imagen,
                                                  "IMP03GESTIONTMP",
                                                  id_respuesta,
                                                  ruta_documento, 1)
            If Result <> "YES" Then
                Subir_formato_respuesta_radicado = Result
                Exit Function
            End If

            Result = Refclas_system.Solicita_nombre_plantilla_radicado(stru_envi.system_plantilla_radicado_id_plantilla,
                                                                     nombre_plantilla_radicado)
            If Result <> "YES" Then
                Subir_formato_respuesta_radicado = Result
                Exit Function
            End If
            Result = Refclas_system.Solicita_id_tipo_tramite_plantilla_radicado(stru_envi.RADICADO,
                                                                                nombre_plantilla_radicado,
                                                                                id_tipo_tramite)
            If Result <> "YES" Then
                Subir_formato_respuesta_radicado = Result
                Exit Function
            End If
            Result = refclas_tip_doc_entrante.Solicita_tipo_envio_respuesta(id_tipo_tramite,
                                                                            id_tipo_envio_respuesta)
            If Result <> "YES" Then
                Subir_formato_respuesta_radicado = Result
                Exit Function
            End If
            If id_tipo_envio_respuesta = 0 Then
                Result = Refclas_resp.Solicita_estados_semaforo_respuesta(id_respuesta, image)
                If Result <> "YES" Then
                    Subir_formato_respuesta_radicado = Result
                    Exit Function
                End If
            Else
                Result = Refclas_resp.Solicita_estados_semaforo_respuesta_electronica(id_respuesta, image)
                If Result <> "YES" Then
                    Subir_formato_respuesta_radicado = Result
                    Exit Function
                End If
            End If
            Subir_formato_respuesta_radicado = "YES"
        Catch ex As Exception
            Subir_formato_respuesta_radicado = "Inconsistencia general función Subir_formato_respuesta_radicado " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Eliminar_anexo_documento_respuesta(ByVal id_anexo As Integer,
                                                ByVal id_respuesta As Integer) As String
        Try

            Dim Ref_class_resp_rad As New Class_ra_respuesta_radicado
            Dim Ref_class_anexo As New Class_ra_anexos_respuesta
            Dim Ref_clas_da_gab As New ClassDaGabinete
            Dim Ref_class_eliminar As New ClassEliminarDocListResult
            Dim Result As String = ""
            Dim stru_envi As stru_envio = Nothing
            Dim stru_anex As stru_anexos = Nothing
            Result = Ref_class_resp_rad.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                         stru_envi)
            If Result <> "YES" Then
                Eliminar_anexo_documento_respuesta = Result
                Exit Function
            End If
            If stru_envi.FECHA_RESPUETA <> "" Then
                Eliminar_anexo_documento_respuesta = "La solicitud tiene una respuesta final, imposible eliminar anexos de la solicitud"
                Exit Function
            End If
            Result = Ref_class_anexo.Solicita_datos_estructura_anexo(id_anexo,
                                                                     stru_anex)
            If Result <> "YES" Then
                Eliminar_anexo_documento_respuesta = Result
                Exit Function
            End If
            Dim existencia_imagen As String = ""
            Result = Ref_clas_da_gab.Solicita_existencia_imagen_gabinete(stru_anex.id_imagen_gabinete,
                                                                         stru_anex.nombre_gabinete,
                                                                         existencia_imagen)
            If Result <> "YES" Then
                Eliminar_anexo_documento_respuesta = Result
                Exit Function
            End If
            If existencia_imagen = "YES" Then
                Result = Ref_class_eliminar.EliminarDocumentosGabinete(stru_anex.id_imagen_gabinete,
                                                                             0,
                                                                             stru_anex.nombre_gabinete,
                                                                             0,
                                                                             0,
                                                                             0,
                                                                             -1,
                                                                             "GESTIONCORRESPONDENCIA")
                If Result <> "YES" Then
                    Eliminar_anexo_documento_respuesta = Result
                    Exit Function
                End If
            End If
            Result = Ref_class_anexo.Elimina_anexo_respuesta(id_anexo)
            If Result <> "YES" Then
                Eliminar_anexo_documento_respuesta = Result
                Exit Function
            End If

            Eliminar_anexo_documento_respuesta = "YES"
            Exit Function
        Catch ex As Exception
            Eliminar_anexo_documento_respuesta = "Inconsistencia general función Eliminar_anexo_documento_respuesta " & ex.Message
        End Try
    End Function
    Function Eliminar_anexo_documento_respuesta(ByVal id_anexo As Integer,
                                                ByVal id_respuesta As Integer,
                                                ByRef drop_lis As DropDownList,
                                                ByRef update As UpdatePanel,
                                                ByRef drop_lis_simple As DropDownList,
                                                ByRef update_simple As UpdatePanel) As String
        Try

            Dim Ref_class_resp_rad As New Class_ra_respuesta_radicado
            Dim Ref_class_anexo As New Class_ra_anexos_respuesta
            Dim Ref_clas_da_gab As New ClassDaGabinete
            Dim Ref_class_eliminar As New ClassEliminarDocListResult
            Dim Result As String = ""
            Dim stru_envi As stru_envio = Nothing
            Dim stru_anex As stru_anexos = Nothing
            Result = Ref_class_resp_rad.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                         stru_envi)
            If Result <> "YES" Then
                Eliminar_anexo_documento_respuesta = Result
                Exit Function
            End If
            If stru_envi.FECHA_RESPUETA <> "" Then
                Eliminar_anexo_documento_respuesta = "La solicitud tiene una respuesta final, imposible eliminar anexos de la solicitud"
                Exit Function
            End If
            Result = Ref_class_anexo.Solicita_datos_estructura_anexo(id_anexo,
                                                                     stru_anex)
            If Result <> "YES" Then
                Eliminar_anexo_documento_respuesta = Result
                Exit Function
            End If
            Dim existencia_imagen As String = ""
            Result = Ref_clas_da_gab.Solicita_existencia_imagen_gabinete(stru_anex.id_imagen_gabinete,
                                                                         stru_anex.nombre_gabinete,
                                                                         existencia_imagen)
            If Result <> "YES" Then
                Eliminar_anexo_documento_respuesta = Result
                Exit Function
            End If
            If existencia_imagen = "YES" Then
                Result = Ref_class_eliminar.EliminarDocumentosGabinete(stru_anex.id_imagen_gabinete,
                                                                             0,
                                                                             stru_anex.nombre_gabinete,
                                                                             0,
                                                                             0,
                                                                             0,
                                                                             -1,
                                                                             "GESTIONCORRESPONDENCIA")
                If Result <> "YES" Then
                    Eliminar_anexo_documento_respuesta = Result
                    Exit Function
                End If
            End If
            Result = Ref_class_anexo.Elimina_anexo_respuesta(id_anexo)
            If Result <> "YES" Then
                Eliminar_anexo_documento_respuesta = Result
                Exit Function
            End If
            If drop_lis.Items.Count > 0 Then
                For i As Integer = 0 To drop_lis.Items.Count - 1
                    If drop_lis.Items(i).Value = id_anexo Then
                        drop_lis.Items.Remove(drop_lis.Items(i))
                        Exit For
                    End If
                Next
            End If
            If drop_lis_simple.Items.Count > 0 Then
                For i As Integer = 0 To drop_lis_simple.Items.Count - 1
                    If drop_lis_simple.Items(i).Value = id_anexo Then
                        drop_lis_simple.Items.Remove(drop_lis_simple.Items(i))
                        Exit For
                    End If
                Next
            End If

            Eliminar_anexo_documento_respuesta = "YES"
            Exit Function
        Catch ex As Exception
            Eliminar_anexo_documento_respuesta = "Inconsistencia general función Eliminar_anexo_documento_respuesta " & ex.Message
        Finally
            update.Update()
            update_simple.Update()
        End Try
    End Function
    Function Descargar_enexo_dcoumento_respuesta(ByVal id_anexo As Integer,
                                                 ByRef iframe As Object,
                                                 ByRef modal As AjaxControlToolkit.ModalPopupExtender,
                                                 ByRef update As UpdatePanel) As String
        Try
            Dim Refclas_anexo As New Class_ra_anexos_respuesta
            Dim Ref_clas_gabinete As New ClassDaGabinete
            Dim Result As String = ""
            Dim stru_anex As stru_anexos = Nothing
            Result = Refclas_anexo.Solicita_datos_estructura_anexo(id_anexo,
                                                                   stru_anex)
            If Result <> "YES" Then
                Descargar_enexo_dcoumento_respuesta = Result
                Exit Function
            End If
            Result = Ref_clas_gabinete.Inicializa_interface_exporta_archivo_gabinete(stru_anex.id_imagen_gabinete,
                                                                                     stru_anex.nombre_gabinete, 0,
                                                                                     iframe,
                                                                                     modal,
                                                                                     update, 0)
            If Result <> "YES" Then
                Descargar_enexo_dcoumento_respuesta = Result
                Exit Function
            Else
                Descargar_enexo_dcoumento_respuesta = "YES"
            End If
        Catch ex As Exception
            Descargar_enexo_dcoumento_respuesta = "Inconsistencia general función Descargar_enexo_dcoumento_respuesta " & ex.Message
        End Try
    End Function
    Function inicio_gestion_correspondencia(ByVal id_tarea As Integer,
                                            ByRef Gestion_respuesta As Gestion_respuesta) As String
        Try
            Dim Result As String = ""
            Gestion_respuesta.resultado_label = ""
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = id_tarea
            If HttpContext.Current.Session.Item("RESPUESTA_TRAMITE") = 0 Then
                inicio_gestion_correspondencia = "El usuario no tiene permiso para responder el trámite"
                Exit Function
            End If
            If _
            HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                inicio_gestion_correspondencia = "Debe seleccionar el tramite a responder"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = "0" Then
                inicio_gestion_correspondencia = "El usuario workflow no tiene usuario de gestión relacionado"
                Exit Function
            End If

            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    Radicado)
            If Result <> "YES" Then
                inicio_gestion_correspondencia = Result
                Exit Function
            End If
            If Radicado = "" Then
                inicio_gestion_correspondencia = "La tarea seleccionada no tiene radicado relacionado"
                Exit Function
            End If
            '----------------------
            'Asigna radicado
            '----------------------
            Gestion_respuesta.radicado = Radicado
            Dim refclas_resp As New Classgestionrespuesta
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim id_respuesta As Integer = 0
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                               HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                               id_respuesta)
            If Result <> "YES" Then
                inicio_gestion_correspondencia = Result
                Exit Function
            End If
            '----------------------
            'Asigna id respuesta
            '----------------------
            Gestion_respuesta.id_respuesta = id_respuesta
            Result = refclas_resp.Actualiza_ruta_workflow_respuesta_radicado(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                             Radicado)
            If Result <> "YES" Then
                inicio_gestion_correspondencia = Result
                Exit Function
            End If
            If id_respuesta = 0 Then
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                          id_respuesta)
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function
                End If
                If id_respuesta = 0 Then
                    inicio_gestion_correspondencia = "El radicado actual no tiene una respuesta relacionada"
                    Exit Function
                Else
                    inicio_gestion_correspondencia = "El usuario no tiene asiganda la tarea para gestionar la respuesta"
                    Exit Function
                End If
            End If
            If id_respuesta = 0 Then
                inicio_gestion_correspondencia = "El radicado actual no tiene una respuesta relacionada"
                Exit Function

            End If
            Dim estru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                            estru)
            If Result <> "YES" Then
                inicio_gestion_correspondencia = Result
                Exit Function
            End If
            If estru.ID_REMIT_DEST_INT = 0 Then
                Result = refclas_resp.Reasigna_respuesta_sistema_usuario(id_respuesta,
                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                         Radicado,
                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function
                End If
                estru.ID_REMIT_DEST_INT = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            End If
            If estru.ID_REMIT_DEST_INT <> HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") Then
                Result = refclas_resp.Reasigna_respuesta_sistema_usuario(id_respuesta,
                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                         Radicado,
                                                                         estru.ID_REMIT_DEST_INT)
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function
                End If
            End If
            HttpContext.Current.Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION") = id_respuesta
            Gestion_respuesta.title = "Respuesta al radicado (" & Radicado & ") código (" & id_respuesta & ") Peticionario (" & estru.DESTINATARIO & ") Tramite (" & estru.TRAMITE_DOCUMENTO & ")"
            Gestion_respuesta.id_remitente_interno = estru.ID_REMIT_DEST_INT
            Dim nombre_plantilla As String = ""
            Dim estado_envio As Integer = -1
            Dim estado_obligatorio As Integer = 0
            Result = refclas_resp.Retorna_nombre_plantilla_por_id_respuesta(id_respuesta,
                                                                            nombre_plantilla,
                                                                            "")
            If Result <> "YES" Then
                Gestion_respuesta.resultado_label = Result

            Else

                Dim Refclas_plantillas_radic As New Class_plantillas_radicacion
                Dim Refclas_tipo_dco_entrante As New Class_tipo_doc_entrante
                Dim id_tipo_tramite As Integer = 0
                Dim descripcion_tramite As String = ""
                Result = Refclas_plantillas_radic.Solicita_id_nombre_tipo_tramite_plantilla_radicado(nombre_plantilla,
                                                                                                         Radicado,
                                                                                                         id_tipo_tramite,
                                                                                                         descripcion_tramite)
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function
                End If
                Result = Refclas_tipo_dco_entrante.Solicita_estado_obligatoria_respuesta_tramite(id_tipo_tramite,
                                                                                                 estado_obligatorio)
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function
                End If
                Gestion_respuesta.estado_obligatorio = estado_obligatorio

                Result = refclas_resp.Retorna_estado_envio_respuesta(nombre_plantilla,
                                                                     Radicado,
                                                                     estado_envio)
                If Result <> "YES" Then
                    Gestion_respuesta.resultado_label = Result
                Else
                    Dim descripcion_estado_obligatorio As String = ""
                    If estado_obligatorio = 1 Then
                        descripcion_estado_obligatorio = ", el trámite requiere de una respuesta"
                    Else
                        descripcion_estado_obligatorio = ", el trámite requiere solo una confirmación"
                    End If
                    Gestion_respuesta.estado_envio = estado_envio
                    If estado_envio = 1 Then
                        Gestion_respuesta.resultado_label = "Tipo de contestacíon correo electrónico" & descripcion_estado_obligatorio
                    Else
                        Gestion_respuesta.resultado_label = "Tipo de contestacíon correo físico" & descripcion_estado_obligatorio
                    End If
                End If
            End If
            If Directory.Exists(HttpContext.Current.Server.MapPath("../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta & "/")) = False Then
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("../Temp_Image/" & "/adjuntos_respuesta/" & id_respuesta & "/"))
            End If
            Gestion_respuesta.correo_electronico_envio = ""
            Dim refclasradicado As New ClassRadicador
            Result = refclasradicado.Solicta_Correo_Electronico_remitente_por_radicado(estru.codigo_dest_externo,
                                                                                       Gestion_respuesta.correo_electronico_envio,
                                                                                       estru.system_plantilla_radicado_id_plantilla)
            If Result <> "YES" Then
                inicio_gestion_correspondencia = Result
                Exit Function
            End If
            Dim Ref_clas_anexos As New Class_ra_anexos_respuesta
            Result = Ref_clas_anexos.Solicita_lista_documentos_anexos(id_respuesta,
                                                                      Gestion_respuesta.item_anexos)
            If Result <> "YES" Then
                inicio_gestion_correspondencia = Result
                Exit Function
            End If
            Gestion_respuesta.id_remitente_externo = estru.codigo_dest_externo
            If estado_envio = 0 Then
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta(id_respuesta,
                                                                               Gestion_respuesta.url_image)
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function
                End If
            Else

                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta_electronica(id_respuesta,
                                                                                           Gestion_respuesta.url_image)
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function
                End If
            End If
            If estado_envio = 0 Then
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion(id_respuesta,
                                                                                     Gestion_respuesta.url_image_electronica)
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function

                End If
            Else
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(id_respuesta,
                                                                                             Gestion_respuesta.url_image_electronica)
                If Result <> "YES" Then
                    inicio_gestion_correspondencia = Result
                    Exit Function

                End If
            End If
            inicio_gestion_correspondencia = "YES"
            Exit Function
        Catch ex As Exception
            inicio_gestion_correspondencia = "Inconsistencia general funcion inicio_gestion_correspondencia " & ex.Message
        End Try
    End Function
    Function Reversa_gestion_tramite_usuario_autorizado(ByVal id_respuesta_radicado As Integer,
                                                        ByVal login_usuario As String,
                                                        ByVal pasword_usuario As String,
                                                        ByVal valida_usuario As Integer,
                                                        ByRef url_image As String,
                                                        ByRef url_image_electronica As String) As String
        Try
            Dim Result As String = ""
            Dim id_imagen_plantilla As Integer = 0
            Dim radicado_respuesta As Integer = 0
            Dim fecha_respuesta As Integer = 0
            Dim id_imagen_respuesta As Integer = 0
            Dim estado_envio_respuesta As Integer = 0
            Dim Refclas As New Classgestionrespuesta
            Dim Refclas_resp_radicado As New Class_ra_respuesta_radicado
            Result = Refclas_resp_radicado.Retorna_estados_respeuesta_documento(id_respuesta_radicado,
                                                                                id_imagen_plantilla,
                                                                                radicado_respuesta,
                                                                                fecha_respuesta,
                                                                                id_imagen_respuesta,
                                                                                estado_envio_respuesta)
            If Result <> "YES" Then
                Reversa_gestion_tramite_usuario_autorizado = Result
                Exit Function
            End If
            'If fecha_respuesta = 0 And radicado_respuesta = 0 Then
            '    clasjava.Showscripman_menu("La respuesta a reversar aun no tiene respuesta ", Me.UpdatePanel_contenido_radica_documento, "ModalPopupExtender_mensaje_personalizado")
            '    Exit Sub
            'End If
            Dim id_usuario_autoriza As Integer = -1
            If valida_usuario = 1 Then
                Result = Refclas.Valida_usuario_administrador_general(login_usuario,
                                                                      pasword_usuario,
                                                                      id_usuario_autoriza,
                                                                      "reversa_respuesta")
                If Result <> "YES" Then
                    Reversa_gestion_tramite_usuario_autorizado = Result
                    Exit Function
                End If
            Else
                login_usuario = "NA"
                id_usuario_autoriza = -1
            End If
            Dim stru As stru_envio = Nothing
            Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
            Result = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta_radicado,
                                                                                        stru,
                                                                                        1)
            If Result <> "YES" Then
                Reversa_gestion_tramite_usuario_autorizado = Result
                Exit Function
            End If

            Result = Refclas.Reversa_respuesta_radicado(stru,
                                                        login_usuario,
                                                        id_usuario_autoriza)
            If Result <> "YES" Then
                Reversa_gestion_tramite_usuario_autorizado = Result
                Exit Function
            End If
            If stru.ESTADO_ENVIO = 0 Then
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta(id_respuesta_radicado,
                                                                               url_image)
                If Result <> "YES" Then
                    Reversa_gestion_tramite_usuario_autorizado = Result
                    Exit Function
                End If
            Else
                Result = ref_ra_resp_radic.Solicita_estados_semaforo_respuesta_electronica(id_respuesta_radicado,
                                                                                           url_image)
                If Result <> "YES" Then
                    Reversa_gestion_tramite_usuario_autorizado = Result
                    Exit Function
                End If
            End If
            If stru.ESTADO_ENVIO = 0 Then
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion(id_respuesta_radicado,
                                                                                  url_image_electronica)
                If Result <> "YES" Then
                    Reversa_gestion_tramite_usuario_autorizado = Result
                    Exit Function

                End If
            Else
                Result = Me.Retorna_estados_semaforo_respuesta_solo_confirmacion_electronica(id_respuesta_radicado,
                                                                                             url_image_electronica)
                If Result <> "YES" Then
                    Reversa_gestion_tramite_usuario_autorizado = Result
                    Exit Function

                End If
            End If
            Reversa_gestion_tramite_usuario_autorizado = "YES"
        Catch ex As Exception
            Reversa_gestion_tramite_usuario_autorizado = "Inconsistencia general funcion Reversa_gestion_tramite_usuario_autorizado " & ex.Message
        End Try
    End Function
End Class