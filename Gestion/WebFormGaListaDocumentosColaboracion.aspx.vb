Imports System.IO

Public Class WebFormGaListaDocumentosColaboracion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        If Me.IsPostBack = False Then
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            '--------------------------------------------------------
            'Lista registro colaboración con radicado relacionado
            '--------------------------------------------------------
            If Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO A RADICADO" Then
                Dim Result As String = reflcas_respuesta.Lista_registros_de_colaboracion_radicado(HttpContext.Current.Session.Item("GA_STRU_RADICADO_COLABORACION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  Me.UpdatePanel_title, _
                                                                                                  1, _
                                                                                                  "")
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                End If
            End If
            '------------------------------------------------------------------------
            'Lista registro colaboración con id documento compartido relacionado
            '------------------------------------------------------------------------
            If Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO ID DOCUMENTO" Then
                Dim Result As String = reflcas_respuesta.Lista_registros_de_colaboracion_id_documento_compartido(HttpContext.Current.Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO_COLABORACION"), _
                                                                                                                 Me.data_grid_listado_solicitudes, _
                                                                                                                 Me.Label_titulo_listado_solicitudes, _
                                                                                                                 Me.hdnEmailID, _
                                                                                                                 UpdateGeneral, _
                                                                                                                 Me.UpdatePanel_title, _
                                                                                                                 1, _
                                                                                                                 "")
                If Result <> "YES" Then
                    Label_estado.Text = Label_estado.Text & Result
                End If
            End If

        End If
    End Sub
    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
        Catch ex As Exception

        End Try

    End Sub
    Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_solicitudes.PageIndex = e.NewPageIndex        
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            '--------------------------------------------------------
            'Lista registro colaboración con radicado relacionado
            '--------------------------------------------------------
            If Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO A RADICADO" Then
                Dim Result As String = reflcas_respuesta.Lista_registros_de_colaboracion_radicado(HttpContext.Current.Session.Item("GA_STRU_RADICADO_COLABORACION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  Me.UpdatePanel_title, _
                                                                                                  Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO"), _
                                                                                                  Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO"))
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
            '------------------------------------------------------------------------
            'Lista registro colaboración con id documento compartido relacionado
            '------------------------------------------------------------------------
            If Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO ID DOCUMENTO" Then
                Dim Result As String = reflcas_respuesta.Lista_registros_de_colaboracion_id_documento_compartido(HttpContext.Current.Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO_COLABORACION"), _
                                                                                                                 Me.data_grid_listado_solicitudes, _
                                                                                                                 Me.Label_titulo_listado_solicitudes, _
                                                                                                                 Me.hdnEmailID, _
                                                                                                                 UpdateGeneral, _
                                                                                                                 Me.UpdatePanel_title, _
                                                                                                                 Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO"), _
                                                                                                                 Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO"))
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub
    Private Sub ImageButton_buscar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_buscar.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim reflcas_respuesta As New ClassGaCompartirDocumento
            '--------------------------------------------------------
            'Lista registro colaboración con radicado relacionado
            '--------------------------------------------------------
            If Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO A RADICADO" Then
                Dim Result As String = reflcas_respuesta.Lista_registros_de_colaboracion_radicado(HttpContext.Current.Session.Item("GA_STRU_RADICADO_COLABORACION"), _
                                                                                                  Me.data_grid_listado_solicitudes, _
                                                                                                  Me.Label_titulo_listado_solicitudes, _
                                                                                                  Me.hdnEmailID, _
                                                                                                  UpdateGeneral, _
                                                                                                  Me.UpdatePanel_title, _
                                                                                                  2, _
                                                                                                  Me.TextBox_busqueda.Text)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
            '------------------------------------------------------------------------
            'Lista registro colaboración con id documento compartido relacionado
            '------------------------------------------------------------------------
            If Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "RELACIONADO ID DOCUMENTO" Then
                Dim Result As String = reflcas_respuesta.Lista_registros_de_colaboracion_id_documento_compartido(HttpContext.Current.Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO_COLABORACION"), _
                                                                                                                 Me.data_grid_listado_solicitudes, _
                                                                                                                 Me.Label_titulo_listado_solicitudes, _
                                                                                                                 Me.hdnEmailID, _
                                                                                                                 UpdateGeneral, _
                                                                                                                 Me.UpdatePanel_title, _
                                                                                                                 2, _
                                                                                                                 Me.TextBox_busqueda.Text)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
                End If
            End If
           
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, UpdatePanel_busqueda, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_ver_documentos_relacionados_Click(sender As Object, e As EventArgs) Handles Button_ver_documentos_relacionados.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "0" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para ver los documentos relacionados", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassGaCompartirDocumento
            Dim stru() As stru_documentos_colaboracion = Nothing
            Result = Refclas.Lista_documentos_colaboracion_usuario(Me.hdnEmailID.Value, _
                                                                   stru, _
                                                                   Me.Label_estado_doc_colaboracion, _
                                                                   Me.UpdatePanel_estado_doc_colaboracion)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Lista_documentos_colaboracion_interface(Me.Page, Me.hdnEmailID.Value)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            ModalPopupExtender_edition_lista_documentos_colaboracion.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_descarga_documento_Click(sender As Object, e As EventArgs) Handles Button_descarga_documento.Click
        Dim scrijava As New Classscrripjava
        Dim split() As String = Me.Hidden_documento_descarga.Value.Split("|")
        Dim Result As String = ""
        Try
            If split.Length > 1 Then
                Dim refclas_visualiza As New ClassVisualisaDocumento
                Dim matri_documento() As String = Nothing
                Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(split(0), split(1), matri_documento)
                If Result <> "YES" Then
                    scrijava.Showscripman_menu(Result, Me.UpdatePanel_descraga_documento, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim files As New FileInfo(matri_documento(1))
                Dim Ruttempo As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("GA_RUTA_TEMPO") & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Directory.Exists(Ruttempo) = False Then
                    Directory.CreateDirectory(Ruttempo)
                End If
                Ruttempo = Ruttempo & "\tempo_image_externa"
                If Directory.Exists(Ruttempo) = False Then
                    Directory.CreateDirectory(Ruttempo)
                End If
                If File.Exists(Ruttempo & "\" & "documento_descarga" & files.Extension) Then
                    Kill(Ruttempo & "\" & "documento_descarga" & files.Extension)
                    File.Copy(files.FullName, Ruttempo & "\" & "documento_descarga" & files.Extension)
                Else
                    File.Copy(files.FullName, Ruttempo & "\" & "documento_descarga" & files.Extension)
                End If
                Dim url_imagen As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPO") + HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString + "/tempo_image_externa/" & "documento_descarga" & files.Extension
                Hidden_ruta_archivo.Value = url_imagen
                ifmExcel_.Attributes.Add("src", url_imagen)
                updatapanel_iframe.Update()
            End If
            
        Catch ex As Exception
            scrijava.Showscripman_menu(Result, Me.UpdatePanel_descraga_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_ver_nota_colaboracion_Click(sender As Object, e As EventArgs) Handles Button_ver_nota_colaboracion.Click
        Dim Result As String = ""
        Dim scrijava As New Classscrripjava
        Try
            If Me.hdnEmailID.Value = "0" Then
                scrijava.Showscripman_menu("Debe seleccionar el registro para ver la nota de colaboración ", Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassGaCompartirDocumento
            Dim stru() As stru_documentos_colaboracion = Nothing
            Dim id_nota_colaboracion As Integer = 0
            Me.TextBox_nota_colaboracion.Text = ""
            Result = Refclas.Retorna_contenido_nota_id_documento_compartido(Me.hdnEmailID_VAL.Value, _
                                                                            Me.hdnEmailID.Value, _
                                                                            Me.TextBox_nota_colaboracion.Text, _
                                                                            id_nota_colaboracion)
            If Result <> "YES" Then
                scrijava.Showscripman_menu(Result, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.UpdatePanel_nota_solicitud_colaboracion.Update()
            ModalPopupExtender_edition_nota_solicitud_colaboracion.Show()
        Catch ex As Exception
            scrijava.Showscripman_menu(ex.Message, Me.update_botonoes_opciones_solicitud_general, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
End Class