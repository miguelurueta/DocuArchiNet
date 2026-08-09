Public Class WebFormEscan
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim ref_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Dim Refclas_producion As New ClassGaProducionDocumental
            Dim Result As String = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                Me.Button_adjuntar.Visible = True
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                If Result = "YES" Then
                    Result = ref_ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite(Session.Item("DG_ID_TRAMITE"),
                                                                                                                                  Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                                  Me.Page)
                End If
                If Result <> "YES" Then
                    Label_estado_lista.Text = Result
                End If
                Session.Item("DG_ESTADO_VENTA") = 0
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                If Result = "YES" Then
                    Result = ref_ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite(Session.Item("DG_ID_TRAMITE"),
                                                                                                                                  Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                                  Me.Page)
                End If
                If Result <> "YES" Then
                    Label_estado_lista.Text = Result
                End If
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "PRODUCCION" Then
                Result = Refclas_producion.Activa_digitalizacion_documentos(Session.Item("DG_ID_EXPEDIENTE"),
                                                                            Me.Page)
                Me.Button_adjuntar.Visible = True
                If Result <> "YES" Then
                    Label_estado_lista.Text = Result
                End If
                Me.Hidden21.Value = "1"
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "MIGRACION" Then
                Me.Hidden21.Value = "3"
                Me.TextBox_nombre.Visible = False
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE SIMPLE" Then
                Me.Button_adjuntar.Visible = True
                Me.Hidden21.Value = "4"
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                If Result <> "YES" Then
                    Label_estado_lista.Text = Result
                End If
                'Result = ref_ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite(Session.Item("DG_ID_TRAMITE"),
                '                                                                                                              Session.Item("DG_TIPO_TRAMITE"),
                '                                                                                                              Me.Page)
                'If Result <> "YES" Then
                '    Label_estado_lista.Text = Label_estado_lista.Text & " - " & Result
                'End If
                Session.Item("DG_ESTADO_VENTA") = 0
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "REMPLAZAVERSION" Then
                Me.Hidden21.Value = "5"
            End If
        End If
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If

    End Sub
    '///Buton activa el show del popup
    Protected Sub Button_guardar_documento_Click(sender As Object, e As EventArgs) Handles Button_guardar_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Or Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW" Then
                Me.TextBox_nombre.Visible = False
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "PRODUCCION" Then
                Me.TextBox_nombre.Visible = True
            End If
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim ref_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Dim Refclas_producion As New ClassGaProducionDocumental
            Dim Result As String = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                Me.Button_adjuntar.Visible = True
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                If Result = "YES" Then
                    Result = ref_ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite(Session.Item("DG_ID_TRAMITE"),
                                                                                                                                  Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                                  Me.Page)
                    If Result <> "YES" Then
                        clasjava.Showscripman(Result, Me.UpdatePanel_guadar)
                        Exit Sub
                    End If
                End If
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE SIMPLE" Then
                Me.Button_adjuntar.Visible = True
                Me.Hidden21.Value = "4"
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_guadar)
                    Exit Sub
                End If
                Result = ref_ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite(Session.Item("DG_ID_TRAMITE"),
                                                                                                                              Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                              Me.Page)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_guadar)
                    Exit Sub
                End If
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
                                                                                 Session.Item("DG_TIPO_TRAMITE"), _
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                If Result = "YES" Then
                    Result = ref_ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite(Session.Item("DG_ID_TRAMITE"), _
                                                                                                                                  Session.Item("DG_TIPO_TRAMITE"), _
                                                                                                                                  Me.Page)
                    If Result <> "YES" Then
                        clasjava.Showscripman(Result, Me.UpdatePanel_guadar)
                        Exit Sub
                    End If
                End If
                
            End If

            If Session.Item("DG_TIPODIGITALIZACION") = "PRODUCCION" Then
                Result = Refclas_producion.Activa_digitalizacion_documentos(Session.Item("DG_ID_EXPEDIENTE"),
                                                                            Me.Page)
                Me.Hidden21.Value = "1"
                Me.Button_adjuntar.Visible = True
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_guadar)
                    Exit Sub
                End If
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "MIGRACION" Then
                Me.Hidden21.Value = "3"
                Me.TextBox_nombre.Visible = False
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "REMPLAZAVERSION" Then
                Me.Hidden21.Value = "5"
                Me.TextBox_nombre.Visible = False
            End If
            Me.UpdatePanelguarda_servidor.Update()
            Me.ModalPopupExtenderimpre_guarda_servidor.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_guadar)
        End Try

    End Sub

    Protected Sub Button_cancelar_popup_Click(sender As Object, e As EventArgs) Handles Button_cancelar_popup.Click
        ModalPopupExtenderimpre_guarda_servidor.Hide()
    End Sub
    Private Sub data_grid_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid.RowCreated
        e.Row.Cells(0).Visible = False
    End Sub

    Private Sub Button_guardar_popup_Click(sender As Object, e As EventArgs) Handles Button_guardar_popup.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim Refclas_produccion As New ClassGaProducionDocumental
            Dim Ref_class_ra_pro_consecutivo As New Class_ra_pro_consecutivo_documento_produccion
            Dim KP = Session.Item("DG_TIPODIGITALIZACION")
            Dim P = Session.Item("DG_ID_CONFIG_DIGITALIZACION")
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" And Session.Item("DG_ID_CONFIG_DIGITALIZACION") <> "-1" Then
                Result = Refclas.Activa_guardar_documento_digitalizado_relacionado_a_tramite(Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                             Session.Item("DG_NOMBRE_GABINETE"),
                                                                                             Me.hdnEmailID.Value,
                                                                                             Session.Item("DG_RADICADO"))
                If Result <> "YES" Then
                    Me.Hidden0001.Value = "-1"
                    clasjava.Showscripman(Result, Me.UpdatePanelguarda_servidor)
                    Exit Sub
                Else
                    Session.Item("DG_RIPO_DOCUMENTAL_LISTA_CHEQUEO") = Me.hdnTipoTramite.Value
                    Session.Item("DG_LISTA_CHEQUEO") = Me.hdnEmailID.Value
                    Me.Hidden21.Value = "1"
                    Me.Hidden0001.Value = "1"
                End If
            Else
                Me.Hidden21.Value = "1"
                Me.Hidden0001.Value = "1"
            End If

            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW" And Session.Item("DG_ID_CONFIG_DIGITALIZACION") <> "-1" Then
                Result = Refclas.Activa_guardar_documento_digitalizado_relacionado_a_tramite(Session.Item("DG_ID_CONFIG_DIGITALIZACION"), _
                                                                                             Session.Item("DG_NOMBRE_GABINETE"), _
                                                                                             Me.hdnEmailID.Value, _
                                                                                             Session.Item("DG_RADICADO"))
                If Result <> "YES" Then
                    Me.Hidden0001.Value = "-1"
                    clasjava.Showscripman(Result, Me.UpdatePanelguarda_servidor)
                    Exit Sub
                Else
                    Session.Item("DG_RIPO_DOCUMENTAL_LISTA_CHEQUEO") = Me.hdnTipoTramite.Value
                    Session.Item("DG_LISTA_CHEQUEO") = Me.hdnEmailID.Value
                    Me.Hidden21.Value = "1"
                    Me.Hidden0001.Value = "1"
                End If
            Else
                Me.Hidden21.Value = "1"
                Me.Hidden0001.Value = "1"
            End If
            Session.Item("DG_SELECCION_TIPODOCUMENTO_EXPEDIENTE") = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "PRODUCCION" Then
                Result = Refclas_produccion.Activa_guardar_documento_digitalizado(Session.Item("DG_ID_EXPEDIENTE"),
                                                                                  Me.hdnEmailID.Value,
                                                                                  Session.Item("DG_SELECCION_TIPODOCUMENTO_EXPEDIENTE"))
                If Result <> "YES" Then
                    Me.Hidden0001.Value = "-1"
                    clasjava.Showscripman(Result, Me.UpdatePanelguarda_servidor)
                    Exit Sub
                Else
                    If Session.Item("DG_SELECCION_TIPODOCUMENTO_EXPEDIENTE") = "" Then
                        Dim file_name As String = ""
                        Dim Zero_file As String = ""
                        If Me.TextBox_nombre.Text = "" Then
                            Result = Ref_class_ra_pro_consecutivo.Solicita_consecutivo_produccion(Session.Item("GA_CONSECUTVO_DOC_PRODUCCION"),
                                                                                                   Zero_file)
                            If Result <> "YES" Then
                                Me.Hidden0001.Value = "-1"
                                clasjava.Showscripman(Result, Me.UpdatePanelguarda_servidor)
                                Exit Sub
                            Else
                                Session.Item("DG_NOMBRE_DOCUMENTO") = "DOC" & Zero_file
                            End If
                        Else
                            Session.Item("DG_NOMBRE_DOCUMENTO") = Me.TextBox_nombre.Text
                            Me.Hidden21.Value = "1"
                            Me.Hidden0001.Value = "1"
                        End If
                    Else
                        Dim split() As String = Session.Item("DG_SELECCION_TIPODOCUMENTO_EXPEDIENTE").ToString.Split("|")
                        If Me.TextBox_nombre.Text = "" Then
                            Session.Item("DG_NOMBRE_DOCUMENTO") = split(4)
                        Else
                            Session.Item("DG_NOMBRE_DOCUMENTO") = Me.TextBox_nombre.Text
                        End If
                        Me.Hidden21.Value = "1"
                        Me.Hidden0001.Value = "1"
                    End If
                End If
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "MIGRACION" Then
                Me.Hidden21.Value = "3"
                Me.TextBox_nombre.Visible = False
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "REMPLAZAVERSION" Then
                Me.Hidden21.Value = "5"
                Me.TextBox_nombre.Visible = False
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE SIMPLE" And Session.Item("DG_ID_CONFIG_DIGITALIZACION") <> "-1" Then
                Me.TextBox_nombre.Visible = False
                Result = Refclas.Activa_guardar_documento_digitalizado_relacionado_a_tramite(Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                             Session.Item("DG_NOMBRE_GABINETE"),
                                                                                             Me.hdnEmailID.Value,
                                                                                             Session.Item("DG_RADICADO"))
                If Result <> "YES" Then
                    Me.Hidden0001.Value = "-1"
                    clasjava.Showscripman(Result, Me.UpdatePanelguarda_servidor)
                    Exit Sub
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = Me.hdnEmailID.Value
                    Me.Hidden21.Value = "4"
                    Me.Hidden0001.Value = "1"
                End If
            End If
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE SIMPLE" Then
                Me.Hidden21.Value = "4"
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanelguarda_servidor)
        Finally
            UpdatePanel_tipo_save.Update()
        End Try
    End Sub

    Protected Sub Button_Actualizar_Lista_chequeo_Click(sender As Object, e As EventArgs) Handles Button_Actualizar_Lista_chequeo.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim ref_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Dim Refclas_producion As New ClassGaProducionDocumental
            Dim Result As String = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Or Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
                                                                                 Session.Item("DG_TIPO_TRAMITE"), _
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                If Result = "YES" Then
                    Result = ref_ra_dig_tipos_docum_lista_chequeo.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite(Session.Item("DG_ID_TRAMITE"), _
                                                                                                                                  Session.Item("DG_TIPO_TRAMITE"), _
                                                                                                                                  Me.Page)
                End If
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdateGeneral)
                    Exit Sub
                End If
            End If
           
            If Session.Item("DG_TIPODIGITALIZACION") = "PRODUCCION" Then
                 Result = Refclas_producion.Activa_digitalizacion_documentos(Session.Item("DG_ID_EXPEDIENTE"), Me.Page)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdateGeneral)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdateGeneral)
        End Try
    End Sub

    Private Sub Button_adjuntar_Click(sender As Object, e As EventArgs) Handles Button_adjuntar.Click
        Me.ModalPopupExtenderimpre_adjunta_servidor.Show()
    End Sub

    Private Sub Button_añadir_popup_Click(sender As Object, e As EventArgs) Handles Button_añadir_popup.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflowDigitalizacion
            If Session.Item("DG_SELECION_TREE") = "" Then
                clasjava.Showscripman("Por favor seleccione el documento al cual quiere añadir el documeto digitalizado", Me.UpdatePaneladjunta_servidor)
                Exit Sub
            End If
            '0 Nombre gabinete
            '1 id imagen
            Dim split_seleccion() As String = Session.Item("DG_SELECION_TREE").ToString.Split("|")
            Result = Refclas.Valida_adjuntar_documento_digitalizado(split_seleccion(1), _
                                                                    split_seleccion(0), _
                                                                    Me.Hidden22.Value)
            If Result <> "YES" Then
                Me.Hidden00010.Value = "-1"
                clasjava.Showscripman(Result, Me.UpdatePaneladjunta_servidor)
                Exit Sub
            Else
                Me.Hidden21.Value = "2"
                Me.Hidden00010.Value = "1"
                Me.ModalPopupExtenderimpre_adjunta_servidor.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePaneladjunta_servidor)
        Finally
            UpdatePanel_tipo_save.Update()
        End Try

    End Sub

    Private Sub Button_añade_documento_Click(sender As Object, e As EventArgs) Handles Button_añade_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAñadirDocumento
            If Session.Item("DG_SELECION_TREE") = "" Then
                clasjava.Showscripman("Debe seleccionar el documento para añadir", Me.UpdatePaneladjunta_servidor)
                Exit Sub
            End If
            Dim split_seleccion() As String = Session.Item("DG_SELECION_TREE").ToString.Split("|")
            Result = Refclas.Añade_documento_digitalizado(split_seleccion(1), _
                                                          split_seleccion(0))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePaneladjunta_servidor)
                Exit Sub
            Else
                Me.ModalPopupExtenderimpre_adjunta_servidor.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePaneladjunta_servidor)
        Finally
            UpdatePanel_tipo_save.Update()
        End Try
    End Sub

    Private Sub Button_cancelar_popup_adjunta_Click(sender As Object, e As EventArgs) Handles Button_cancelar_popup_adjunta.Click
        ModalPopupExtenderimpre_adjunta_servidor.Hide()
    End Sub
End Class