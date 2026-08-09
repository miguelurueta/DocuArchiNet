Imports System.Drawing
Imports System.Web.Script.Serialization
Imports MindFusion.Diagramming
Imports MindFusion.Diagramming.WebForms
Imports MindFusion.Diagramming.Import.VisioImporter
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports Graphics = System.Drawing.Graphics

Public Class WebWorkflowDigramaRuta
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim clasjava As New Classscrripjava
        Dim scr As [String] = "$(document).ready(function () {$().inicio();});"
        If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", scr))) Then
            ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
        End If
        Dim Result As String = ""
        If IsPostBack = False Then
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Result = Ref_clas_rutas.lista_zon_interface(Me.DropDownZonFactor, Me.updatemenu)
            If Result <> "YES" Then
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 1 " & Result
            End If
            Dim nombres_rutas() As String = Nothing
            Result = Ref_clas_rutas.Solicita_nombres_rutas_workflow(nombres_rutas)
            If Result = "YES" Then
                Me.DropDownList_rutas_disponibles_workflow.Items.Add("")
                Result = Ref_clas_rutas.Lista_rutas_interface_importacion(nombres_rutas, _
                                                                          Me.DropDownList_rutas_disponibles_workflow, _
                                                                          Me.updatemenu, 1)
                If Result <> "YES" Then
                    Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 2 " & Result
                End If
            Else
                Me.Label_Estado_documento.Text = Me.Label_Estado_documento.Text & " 3 " & Result
            End If
        End If
        diagramView.LicenseKey = "AQAAAEQAAAAoAAAAAQAAFx8IvJCRi56MkJmL35yQko+ekYbfjNGe0YyylpGbuYqMlpCR0buWnpiNnpKSlpGY0aianbmQjZKM//8kUb3nKfdw6tdlsocDxSo9XQSlcFbsP0LRQx1Gv9GwV+gLRASirGRQYiL2I50e"
        diagramView.Diagram.LinkCustomDraw = CustomDraw.None
        diagramView.DelKeyAction = WebForms.DelKeyAction.None
        diagramView.ModificationStart = ModificationStart.SelectedOnly
        diagramView.Diagram.AllowSplitLinks = True
        diagramView.Diagram.LinkShape = LinkShape.Polyline
        diagramView.Diagram.AllowUnanchoredLinks = False
        diagramView.Behavior = Behavior.DrawLinks
        diagramView.Diagram.LinkEndsMovable = False
        diagramView.Diagram.UndoManager.UndoEnabled = True
        If CheckBox_Grid_alineamiento.Checked = True Then
            diagramView.Diagram.ShowGrid = True
        Else
            diagramView.Diagram.ShowGrid = False
        End If
        diagramView.LinkModifyingScript = "diagranview_bloqued(sender, args);"
        Dim Refclas As New InicioWorkflow
        'Result = Refclas.Crea_Dir_Temporal_wf()
        'If Result <> "YES" Then

        'End If
    End Sub
    Protected Sub Button_me_active_men_dive_Click(sender As Object, e As EventArgs) Handles Button_me_active_men_dive.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.Hidden_menu_var_event_dive.Value = "" Then
                Exit Sub
            End If
            Dim ref_clas_workflow_rutas As New Class_worflow_rutas
            Dim Result As String = ""
            Result = ref_clas_workflow_rutas.Seleccion_menu_pricipal(Me.Hidden_menu_var_event_dive.Value, _
                                                                     Me.Page)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, _
                                              Me.UpdatePanel_menu_var_event, _
                                              "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, UpdatePanel_menu_var_event)
        End Try
    End Sub
    Protected Sub Button_seleccion_archivo_Click(sender As Object, e As EventArgs) Handles Button_seleccion_archivo.Click

        Session.Item("WF_TIPO_ADJUNTA") = "IMPORTA_RUTA"
        AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
        AjaxFileUpload_dowload.AllowedFileTypes = "vdx"
        Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
        AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
        UpdatePanel_descarga.Update()
        Me.ModalPopupExtender_sube_documento_adjunto.Show()
    End Sub

    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
           
            If Session.Item("WF_TIPO_ADJUNTA") = "IMPORTA_RUTA" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If IO.Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    IO.Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = IO.Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
        Catch ex As Exception
            Session.Item("WF_ERROR_RESPUESTA") = ex.Message

        End Try
    End Sub
    '-------Boton que guarda el archivo
    Private Sub Button_guardar_desicion_Click(sender As Object, e As EventArgs) Handles Button_guardar_desicion.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAlmacenamiento
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                CLAS.Showscripman(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga)
                Exit Sub
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "IMPORTA_RUTA" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                Me.TextBox_archivo_import.Text = Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                UpdatePanel_adunta_archivo_ruta.Update()
                Me.ModalPopupExtender_sube_documento_adjunto.Hide()
            End If
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_descarga)
        End Try
    End Sub

    Protected Sub Button_importa_ruta_archivo_Click(sender As Object, e As EventArgs) Handles Button_importa_ruta_archivo.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Dim Archivo_salida As String = ""
        Result = Refclas.Importar_ruta_workflow_general(Me.TextBox_archivo_import.Text, Archivo_salida, Me.diagramView, _
                                                        Me.DropDownList_rutas_disponibles.Text, Me.UpdatePanel_diagran_view, _
                                                        Me.DropDownList_rutas_disponibles_workflow, Me.updatemenu)
        If Result <> "YES" Then
            Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_importa_ruta, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        Else
            Me.ModalPopupExtender_edition_importa_ruta.Hide()
        End If
    End Sub

    Protected Sub DropDownZonFactor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownZonFactor.SelectedIndexChanged
        Dim Ref_clas_rutas As New Class_worflow_rutas
        Dim Result As String = ""
        Result = Ref_clas_rutas.Aplica_zon_factor_diagranview(Me.diagramView, Me.DropDownZonFactor.Text, Me.UpdatePanel_diagran_view)
    End Sub

    Private Sub Button_abrir_ruta_Click(sender As Object, e As EventArgs) Handles Button_abrir_ruta.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Result = Refclas.Abre_ruta_workflow(Me.DropDownListrutasdisponibles.Text, Me.diagramView, Me.UpdatePanel_diagran_view, Me.CheckBox_Grid_alineamiento)
        If Result <> "YES" Then
            Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_abrir_rutas_disponibles, "ModalPopupExtender_mensaje_personalizado")
            Exit Sub
        Else
            Me.ModalPopupExtender_edition_abrir_rutas_disponibles.Hide()
        End If
    End Sub

    Private Sub DropDownList_Nombre_Gabinete_Agrega_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_Nombre_Gabinete_Agrega.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New ClassDaGabinete
        Try
            Result = Refclas.Lista_datos_configuracion_gabinete_seleccionado(Me.DropDownList_Nombre_Gabinete_Agrega.Text, Me.TextBox_ruta_fisica_gab_agrega.Text, _
                    Me.TextBox_ruta_busqueda_gab_agrega.Text, Me.TextBox_ruta_almacena_gab_agrega.Text, Me.DropDownList_base_datos_gabinete_agrega, _
                    Me.DropDownList_dbms_gabinete_agrega, Me.UpdatePanel_parametros_gabinete_agrega)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_nombre_gabinete_agrega, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_nombre_gabinete_agrega, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_agrega_gabinete_Click(sender As Object, e As EventArgs) Handles Button_agrega_gabinete.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            Result = Refclas.Agregar_nuevo_gabinete_configuracion_workflow(Me.DropDownList_Nombre_Gabinete_Agrega.Text, Me.TextBox_ruta_fisica_gab_agrega.Text, _
            Me.TextBox_ruta_busqueda_gab_agrega.Text, Me.TextBox_ruta_almacena_gab_agrega.Text, Me.DropDownList_base_datos_gabinete_agrega.Text, _
            Me.DropDownList_dbms_gabinete_agrega.Text, Me.TextBox_unc_gabinete_agrega.Text, Me.TextBox_usuario_db_gabinete_agrega.Text, _
             Me.TextBox_pasword_db_gabinete_agrega.Text)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_opciones_agrega_gabinete, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                ModalPopupExtender_agrega_nuevo_gabinete.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_opciones_agrega_gabinete, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub DropDownList_Nombre_Gabinete_edita_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_Nombre_Gabinete_edita.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            Result = Refclas.Lista_datos_configuracion_gabinete_seleccionado(Me.DropDownList_Nombre_Gabinete_edita.Text, Me.TextBox_ruta_fisica_gab_edita.Text, _
                    Me.TextBox_ruta_busqueda_gab_edita.Text, Me.TextBox_ruta_almacena_gab_edita.Text, Me.DropDownList_base_datos_gabinete_edita, _
                    Me.DropDownList_dbms_gabinete_edita, Me.TextBox_unc_gabinete_edita.Text, Me.TextBox_usuario_db_gabinete_edita.Text, Me.TextBox_pasword_db_gabinete_edita.Text, Me.UpdatePanel_parametros_gabinete_edita)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_nombre_gabinete_edita, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_nombre_gabinete_edita, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_edita_gabinete_Click(sender As Object, e As EventArgs) Handles Button_edita_gabinete.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            Result = Refclas.Actualiza_gabinete_configuracion_workflow(Me.DropDownList_Nombre_Gabinete_edita.Text, Me.TextBox_ruta_fisica_gab_edita.Text, _
            Me.TextBox_ruta_busqueda_gab_edita.Text, Me.TextBox_ruta_almacena_gab_edita.Text, Me.DropDownList_base_datos_gabinete_edita.Text, _
            Me.DropDownList_dbms_gabinete_edita.Text, Me.TextBox_unc_gabinete_edita.Text, Me.TextBox_usuario_db_gabinete_edita.Text, _
             Me.TextBox_pasword_db_gabinete_edita.Text)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_opciones_edita_gabinete, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                ModalPopupExtender_edita_configuracion_gabinete.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_opciones_edita_gabinete, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_compilar_evento_escript_Click(sender As Object, e As EventArgs) Handles Button_compilar_evento_escript.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            Result = Refclas.Compila_evento_escript(Me)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_botones_contenido_edita_escrip_evento)
                Exit Sub
            Else
                Refclasjava.Showscripman("Compilación correcta", Me.UpdatePanel_botones_contenido_edita_escrip_evento)
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_contenido_edita_escrip_evento)
        End Try
    End Sub

    Private Sub Button_actualiza_evento_escript_Click(sender As Object, e As EventArgs) Handles Button_actualiza_evento_escript.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            Result = Refclas.Compila_evento_escript(Me)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_botones_contenido_edita_escrip_evento)
                Exit Sub
            End If
            Result = Refclas.Actualiza_escript_actividad_seleccionada(Me)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_botones_contenido_edita_escrip_evento)
                Exit Sub
            
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_botones_contenido_edita_escrip_evento)
        End Try
    End Sub

    Private Sub DropDownList_rutas_disponibles_workflow_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_rutas_disponibles_workflow.SelectedIndexChanged
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            If Me.DropDownList_rutas_disponibles_workflow.Text = "" Then Exit Sub
            Result = Refclas.Abre_ruta_workflow(Me.DropDownList_rutas_disponibles_workflow.Text, Me.diagramView, Me.UpdatePanel_diagran_view, Me.CheckBox_Grid_alineamiento)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
       
    End Sub

    Protected Sub Button_crear_actividad_workflow_confirmar_Click(sender As Object, e As EventArgs) Handles Button_crear_actividad_workflow_confirmar.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            Dim estado_relaciona_grupo As Integer = 0
            If CheckBox_option_crea_grupo_workflow.Checked = True Then
                estado_relaciona_grupo = 1
            Else
                estado_relaciona_grupo = 0
            End If
            If HiddenField_tipo_actividad_seleccion.Value = "SISTEMA" Then
                estado_relaciona_grupo = 0
            End If
            If DropDownList_tipo_actividad.Text = "" Then
                Refclasjava.Showscripman_menu("Debe seleccionar el tipo de actividad", Me.UpdatePanel_buton_crear_actividad_workflow, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Result = Refclas.Crear_actividad_script_ruta(DropDownList_tipo_actividad.SelectedValue,
                                                         UCase(Me.TextBox_nombre_actvidad_crear_actividad.Text),
                                                         Session.Item("DR_RUTASELECCION_DIAGRAMA"),
                                                         Me.TextBox_descripcion_crear_actividad.Text,
                                                         Me.diagramView,
                                                         Me.UpdatePanel_diagran_view,
                                                         estado_relaciona_grupo)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_crear_actividad_workflow, "ModalPopupExtender_mensaje_personalizado")
                ModalPopupExtender_edition_crear_actividad_workflow.Hide()
                Exit Sub
            Else
                ModalPopupExtender_edition_crear_actividad_workflow.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_crear_actividad_workflow, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButton_Crear_Actividad_usuario_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_Crear_Actividad_usuario.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            If Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then Exit Sub
            'ModalPopupExtender_edition_crear_actividad_workflow.Show()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ImageButtonCrearGrupoActividadUsuario_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonCrearGrupoActividadUsuario.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            Dim Class_actividades_generales_workflow As New Class_actividades_generales_workflow
            If Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then Exit Sub
            Result = Class_actividades_generales_workflow.Solicita_tipos_actividades_grupo_drowlist(2, DropDownList_tipo_actividad)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HiddenField_tipo_actividad_seleccion.Value = "USUARIO"
            UpdatePanel_crear_actividad_workflow.Update()
            ModalPopupExtender_edition_crear_actividad_workflow.Show()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ImageButtonGuardar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonGuardar.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            If Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then Exit Sub
            Result = Refclas.Guardar_ruta_workflow(Me.diagramView, Session.Item("DR_RUTASELECCION_DIAGRAMA"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImageButtonEliminarActividades_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonEliminarActividades.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            If Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then Exit Sub
            Me.ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama.Show()
           
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_aceptar_confirmacion_eliminar_elmento_diagrama_Click(sender As Object, e As EventArgs) Handles Button_aceptar_confirmacion_eliminar_elmento_diagrama.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            Result = Refclas.Eliminar_elemento_diagrama_web(Me.diagramView, _
                                                            Me.UpdatePanel_diagran_view, _
                                                            Session.Item("DR_RUTASELECCION_DIAGRAMA"))
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_confirma, "ModalPopupExtender_mensaje_personalizado")
                Me.ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama.Hide()
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_confirma, "ModalPopupExtender_mensaje_personalizado")
        End Try
       
    End Sub

    Private Sub Button_cancelar_confirmacion_eliminar_elmento_diagrama_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion_eliminar_elmento_diagrama.Click
        ModalPopupExtender_edition_confirma_eliminar_elmento_diagrama.Hide()
    End Sub

    Private Sub ImageButtonCrearActividadEnlaceDocumento_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonCrearActividadEnlaceDocumento.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Class_actividades_generales_workflow As New Class_actividades_generales_workflow
            Dim Result As String = ""
            If Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then Exit Sub
            Result = Class_actividades_generales_workflow.Solicita_tipos_actividades_grupo_drowlist(3, DropDownList_tipo_actividad)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HiddenField_tipo_actividad_seleccion.Value = "ENLASE"
            UpdatePanel_crear_actividad_workflow.Update()
            ModalPopupExtender_edition_crear_actividad_workflow.Show()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ImageButton_Crear_Actividad_Sistema_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_Crear_Actividad_Sistema.Click
        If Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then Exit Sub
        HiddenField_tipo_actividad_seleccion.Value = "SISTEMA"
        UpdatePanel_crear_actividad_workflow.Update()
        ModalPopupExtender_edition_crear_actividad_workflow.Show()
    End Sub

    Private Sub ImageButton_conectar_actividades_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_conectar_actividades.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            If Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then Exit Sub
            Result = Refclas.Crear_conexion_actividades_workflow(Session.Item("DR_RUTASELECCION_DIAGRAMA"), Me.diagramView, Me.UpdatePanel_diagran_view)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.updatemenu, "ModalPopupExtender_mensaje_personalizado")
        End Try


    End Sub

    Protected Sub Button_asigna_grupo_workflow_Click(sender As Object, e As EventArgs) Handles Button_asigna_grupo_workflow.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            If Session.Item("DR_RUTASELECCION_DIAGRAMA") = "" Then Exit Sub
            If Me.DropDownList_grupos_disponibles_asignacion.Text = "" Then
                Refclasjava.Showscripman_menu("Por favor seleccione el grupo", Me.UpdatePanel_buton_grupos_disponibles_asignacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Actualiza_relacion_grupo_workflow_actividad(Me.Page, Session.Item("DR_RUTASELECCION_DIAGRAMA"), Me.DropDownList_grupos_disponibles_asignacion.Text)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_grupos_disponibles_asignacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                ModalPopupExtender_edition_grupos_disponibles_asignacion.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_grupos_disponibles_asignacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Protected Sub Button_elimina_relacion_grupo_workflow_Click(sender As Object, e As EventArgs) Handles Button_elimina_relacion_grupo_workflow.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Dim Refclas As New Class_worflow_rutas
        Try
            If Me.DropDownList_lista_grupo_workflow_relacion.Text = "" Then
                Refclasjava.Showscripman_menu("Por favor seleccione el grupo", Me.UpdatePanel_buton_lista_grupo_workflow_relacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Elimina_relacion_grupo_workflow_actividad(Page, Session.Item("DR_RUTASELECCION_DIAGRAMA"), Me.DropDownList_lista_grupo_workflow_relacion.Text)
            If Result <> "YES" Then
                Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_lista_grupo_workflow_relacion, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                ModalPopupExtender_edition_lista_grupo_workflow_relacion.Hide()
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_lista_grupo_workflow_relacion, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub CheckBox_Grid_alineamiento_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_Grid_alineamiento.CheckedChanged
        Try
            If CheckBox_Grid_alineamiento.Checked = True Then
                diagramView.Diagram.ShowGrid = True
            Else
                diagramView.Diagram.ShowGrid = False
            End If
            UpdatePanel_diagran_view.Update()
        Catch ex As Exception

        End Try
        
    End Sub

    Protected Sub Button_config_correo_conector_Click(sender As Object, e As EventArgs) Handles Button_config_correo_conector.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim estado_envio_correo As Integer = 0
            Dim Refclas As New Class_actividades_disponibles_envio
            Dim stru_config_conector_ruta As stru_config_conector_ruta = Nothing
            If Me.CheckBox_estado_correo_conector.Checked = True Then
                stru_config_conector_ruta.Estado_evia_correo = 1
            Else
                stru_config_conector_ruta.Estado_evia_correo = 0
            End If
            If Me.CheckBox_autoriza_tarea.Checked = True Then
                stru_config_conector_ruta.Estado_soicita_autorizacion = 1
            Else
                stru_config_conector_ruta.Estado_soicita_autorizacion = 0
            End If
            If Me.CheckBox_autoriza_tarea_firma_digital.Checked = True Then
                stru_config_conector_ruta.Estado_soicita_autorizacion_firma_digital = 1
            Else
                stru_config_conector_ruta.Estado_soicita_autorizacion_firma_digital = 0
            End If
            If Me.CheckBox_estado_copia_estructura.Checked = True Then
                stru_config_conector_ruta.Estado_copia_documento_estructura = 1
            Else
                stru_config_conector_ruta.Estado_copia_documento_estructura = 0
            End If
            If Me.CheckBox_Estado_asigna_expediente.Checked = True Then
                stru_config_conector_ruta.Estado_asigna_expediente = 1
            Else
                stru_config_conector_ruta.Estado_asigna_expediente = 0
            End If
            If Me.CheckBox_estado_firma_digital.Checked = True Then
                stru_config_conector_ruta.Estado_firma_digital = 1
            Else
                stru_config_conector_ruta.Estado_firma_digital = 0
            End If
            If Me.CheckBox_estado_valida_balanceo.Checked = True Then
                stru_config_conector_ruta.estado_valida_balanceo = 1
            Else
                stru_config_conector_ruta.estado_valida_balanceo = 0
            End If
            Result = Refclas.Actualiza_configuracion_conector_ruta(HttpContext.Current.Session.Item("DR_RUTASELECCION_ID_CONECTOR"), _
                                                                   stru_config_conector_ruta)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_buton_configura_envi_correo_conector)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_configura_envi_correo_conector.Hide()
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_buton_configura_envi_correo_conector)
        End Try
    End Sub
End Class