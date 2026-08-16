Imports System.Drawing
Imports System.Web.Services
Imports System.IO
Imports Neodynamic.WebControls.ImageDraw
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Diagnostics
Imports System.Configuration

Public Class Webworkflow
    Inherits RefreshArticle.BasePage
    Private Const WorkflowCentroTrabajoModernEnabledKey As String = "WorkflowCentroTrabajoModernEnabled"
    Private Const WorkflowCentroTrabajoModernPilotProfilesKey As String = "WorkflowCentroTrabajoModernPilotProfiles"
    Private Const WorkflowCentroTrabajoModernLayersKey As String = "WorkflowCentroTrabajoModernLayers"
    Public Matri_Doc_Visual() As String
    Public Doc_actual As String = ""
    Public ruta_documento As String = ""
    Dim datos() As String
    Dim CantRegi As Integer
    Dim Matri_Read() As String
    Dim Matri_Record() As String
    Dim Matri_Celd() As String
    Dim Rango_Ini As Long
    Dim Rango_Final As Long
    Dim Nomb_Colum() As String
    Dim ini As Long
    Dim Popupenlace As Object
    Dim bolindice As Boolean = False
    Private _workflowTransitionModernActive As Nullable(Of Boolean)

    Public ReadOnly Property WorkflowCentroTrabajoModernActive As Boolean
        Get
            If Not IsConfigurationEnabled(ReadConfigurationValue(WorkflowCentroTrabajoModernEnabledKey, "false")) Then
                Return False
            End If

            Return CurrentWorkflowPilotIsEnabled()
        End Get
    End Property

    Public ReadOnly Property WorkflowCentroTrabajoModernCssAttribute As String
        Get
            If Not WorkflowCentroTrabajoModernActive Then
                Return String.Empty
            End If

            Return " class=""workflow-centro-trabajo-moderno " & EnabledWorkflowCentroTrabajoLayers() & """"
        End Get
    End Property

    Public ReadOnly Property WorkflowCentroTrabajoSelectedDocumentAvailable As Boolean
        Get
            Dim selectedDocument As String = WorkflowCentroTrabajoSelectedDocumentRaw()
            Return Not String.IsNullOrWhiteSpace(selectedDocument) AndAlso selectedDocument <> "-1"
        End Get
    End Property

    Public ReadOnly Property WorkflowCentroTrabajoSelectedDocumentTitle As String
        Get
            Dim values() As String = WorkflowCentroTrabajoSelectedDocumentRaw().Split("|"c)
            Dim title As String = If(values.Length > 4, values(4), String.Empty)

            Return EncodeWorkflowCentroTrabajoContextText(title, "Documento seleccionado")
        End Get
    End Property

    Public ReadOnly Property WorkflowCentroTrabajoSelectedDocumentFormat As String
        Get
            Dim selectionTag As String = String.Empty
            Dim values() As String
            Dim extension As String

            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
                selectionTag = Convert.ToString(HttpContext.Current.Session.Item("WF_TAGSELECCION"))
            End If

            values = selectionTag.Split("|"c)
            extension = If(values.Length > 3, values(3), String.Empty).Trim().TrimStart("."c)
            If Not String.IsNullOrWhiteSpace(extension) Then
                Return System.Web.HttpUtility.HtmlEncode(extension.ToUpperInvariant())
            End If

            Return If(WorkflowCentroTrabajoSelectedDocumentAvailable, "Documento", "")
        End Get
    End Property

    Public ReadOnly Property WorkflowCentroTrabajoSelectedDocumentMetadataAvailable As Boolean
        Get
            Return WorkflowCentroTrabajoSelectedDocumentAvailable AndAlso
                   Panel_tolbar_pdf IsNot Nothing AndAlso
                   Panel_tolbar_pdf.Visible
        End Get
    End Property

    Public ReadOnly Property WorkflowCentroTrabajoSelectedDocumentId As String
        Get
            Dim selectedDocumentId As String = String.Empty
            Dim values() As String

            If hiden_seleccion_documento_id_wf IsNot Nothing Then
                selectedDocumentId = Convert.ToString(hiden_seleccion_documento_id_wf.Value).Trim()
            End If

            If String.IsNullOrWhiteSpace(selectedDocumentId) Then
                values = WorkflowCentroTrabajoSelectedDocumentRaw().Split("|"c)
                selectedDocumentId = If(values.Length > 1, values(1), String.Empty)
            End If

            Return System.Web.HttpUtility.HtmlAttributeEncode(selectedDocumentId)
        End Get
    End Property

    Public ReadOnly Property WorkflowCentroTrabajoSelectedDocumentReference As String
        Get
            Return System.Web.HttpUtility.HtmlAttributeEncode(WorkflowCentroTrabajoSelectedDocumentRaw())
        End Get
    End Property

    Public Function WorkflowCentroTrabajoSelectedDocumentActionExists(ByVal actionName As String) As Boolean
        Return WorkflowCentroTrabajoSelectedDocumentAction(actionName) IsNot Nothing
    End Function

    Public Function WorkflowCentroTrabajoSelectedDocumentActionAttribute(ByVal actionName As String, ByVal attributeName As String) As String
        Dim action As Global.System.Web.UI.HtmlControls.HtmlGenericControl = WorkflowCentroTrabajoSelectedDocumentAction(actionName)
        If action Is Nothing OrElse String.IsNullOrWhiteSpace(attributeName) Then
            Return String.Empty
        End If

        Return System.Web.HttpUtility.HtmlAttributeEncode(Convert.ToString(action.Attributes(attributeName)))
    End Function

    Private Function WorkflowCentroTrabajoSelectedDocumentRaw() As String
        If hiden_seleccion_documento_wf Is Nothing Then
            Return String.Empty
        End If

        Return Convert.ToString(hiden_seleccion_documento_wf.Value).Trim()
    End Function

    Private Function WorkflowCentroTrabajoSelectedDocumentAction(ByVal actionName As String) As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Dim selectedDocumentId As String = String.Empty
        Dim row As System.Web.UI.WebControls.GridViewRow

        If String.IsNullOrWhiteSpace(actionName) OrElse Not WorkflowCentroTrabajoSelectedDocumentAvailable OrElse GridView_list_documento_relacion_wf Is Nothing Then
            Return Nothing
        End If

        If hiden_seleccion_documento_id_wf IsNot Nothing Then
            selectedDocumentId = Convert.ToString(hiden_seleccion_documento_id_wf.Value).Trim()
        End If

        For Each row In GridView_list_documento_relacion_wf.Rows
            If String.Equals(Convert.ToString(row.Attributes("id_wf")), selectedDocumentId, StringComparison.Ordinal) Then
                Return FindWorkflowCentroTrabajoAction(row, actionName)
            End If
        Next

        Return Nothing
    End Function

    Private Shared Function FindWorkflowCentroTrabajoAction(ByVal parent As Global.System.Web.UI.Control, ByVal actionName As String) As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Dim child As Global.System.Web.UI.Control
        Dim htmlControl As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Dim nestedAction As Global.System.Web.UI.HtmlControls.HtmlGenericControl

        For Each child In parent.Controls
            htmlControl = TryCast(child, Global.System.Web.UI.HtmlControls.HtmlGenericControl)
            If htmlControl IsNot Nothing AndAlso String.Equals(Convert.ToString(htmlControl.Attributes("tip_event")), actionName, StringComparison.Ordinal) Then
                Return htmlControl
            End If

            nestedAction = FindWorkflowCentroTrabajoAction(child, actionName)
            If nestedAction IsNot Nothing Then
                Return nestedAction
            End If
        Next

        Return Nothing
    End Function

    Private Shared Function EncodeWorkflowCentroTrabajoContextText(ByVal value As String, ByVal fallback As String) As String
        Dim text As String = If(String.IsNullOrWhiteSpace(value), fallback, value.Trim().TrimStart("-"c).Trim())
        Return System.Web.HttpUtility.HtmlEncode(text)
    End Function

    Private Shared Function ReadConfigurationValue(ByVal key As String, ByVal fallback As String) As String
        Dim configuredValue As String = ConfigurationManager.AppSettings(key)
        If String.IsNullOrWhiteSpace(configuredValue) Then
            Return fallback
        End If

        Return configuredValue.Trim()
    End Function

    Private Shared Function IsConfigurationEnabled(ByVal value As String) As Boolean
        Return String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) OrElse
               String.Equals(value, "yes", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function CurrentWorkflowPilotIsEnabled() As Boolean
        If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
            Return False
        End If

        'El perfil piloto es el login de gestión creado por el servidor durante el inicio de sesión.
        Dim currentProfile As String = Convert.ToString(HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION")).Trim()
        Dim configuredProfiles As String = ReadConfigurationValue(WorkflowCentroTrabajoModernPilotProfilesKey, String.Empty)
        Dim profiles() As String
        Dim profile As String

        If String.IsNullOrWhiteSpace(currentProfile) OrElse String.IsNullOrWhiteSpace(configuredProfiles) Then
            Return False
        End If

        profiles = configuredProfiles.Split(New Char() {","c, ";"c, ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
        For Each profile In profiles
            If String.Equals(profile.Trim(), currentProfile, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function EnabledWorkflowCentroTrabajoLayers() As String
        Dim configuredLayers As String = ReadConfigurationValue(WorkflowCentroTrabajoModernLayersKey, "layout,actions,documents,a11y")
        Dim layoutEnabled As Boolean = IsWorkflowCentroTrabajoLayerEnabled(configuredLayers, "layout")
        Dim classes As String = String.Empty

        'Todas las subcapas dependen del layout: sin él la capa queda inerte y es reversible por configuración.
        If Not layoutEnabled Then
            Return classes
        End If

        classes = "ctw-layer-layout"
        If IsWorkflowCentroTrabajoLayerEnabled(configuredLayers, "actions") Then
            classes &= " ctw-layer-actions"
        End If
        If IsWorkflowCentroTrabajoLayerEnabled(configuredLayers, "documents") Then
            classes &= " ctw-layer-documents"
        End If
        If IsWorkflowCentroTrabajoLayerEnabled(configuredLayers, "a11y") Then
            classes &= " ctw-layer-a11y"
        End If

        Return classes
    End Function

    Private Shared Function IsWorkflowCentroTrabajoLayerEnabled(ByVal configuredLayers As String, ByVal expectedLayer As String) As Boolean
        Dim layers() As String = configuredLayers.Split(New Char() {","c, ";"c, ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries)
        Dim layer As String

        For Each layer In layers
            If String.Equals(layer.Trim(), expectedLayer, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Sub ConfigureWorkflowCentroTrabajoViewport()
        If workflowCentroTrabajoModernViewport IsNot Nothing Then
            workflowCentroTrabajoModernViewport.Visible = WorkflowCentroTrabajoModernActive
        End If
    End Sub

    Private ReadOnly Property WorkflowTransitionModernActive As Boolean
        Get
            If Not _workflowTransitionModernActive.HasValue Then
                _workflowTransitionModernActive = WorkflowModernPresentationBootstrap.EstaActivaParaSolicitudActual()
            End If

            Return _workflowTransitionModernActive.Value
        End Get
    End Property

    Private Sub ConfigureWorkflowTransitionModernPresentation()
        If Not WorkflowTransitionModernActive Then
            Return
        End If

        RegisterWorkflowTransitionModernStyle()
        RegisterWorkflowTransitionModernScript()
        RegisterWorkflowTransitionModernBootstrap()
    End Sub

    Private Sub RegisterWorkflowTransitionModernStyle()
        If Page.Header Is Nothing OrElse Page.Header.FindControl("workflowTransitionModernStyle") IsNot Nothing Then
            Return
        End If

        Dim style As New Global.System.Web.UI.HtmlControls.HtmlLink()
        style.ID = "workflowTransitionModernStyle"
        style.Href = "../Styles/workflow-transition-modern.css?v=20260816-doc12qa5"
        style.Attributes("rel") = "stylesheet"
        style.Attributes("type") = "text/css"
        Page.Header.Controls.Add(style)
    End Sub

    Private Sub RegisterWorkflowTransitionModernScript()
        If Page.Header Is Nothing OrElse Page.Header.FindControl("workflowTransitionModernScript") IsNot Nothing Then
            Return
        End If

        Dim script As New Global.System.Web.UI.HtmlControls.HtmlGenericControl("script")
        script.ID = "workflowTransitionModernScript"
        script.Attributes("src") = "../js/workflow/workflow-transition-ui.js?v=20260816-doc12qa4"
        script.Attributes("type") = "text/javascript"
        Page.Header.Controls.Add(script)
    End Sub

    Private Sub RegisterWorkflowTransitionModernBootstrap()
        Dim taskInputClientId As String = System.Web.HttpUtility.JavaScriptStringEncode(Hidden_id_tarea_sel.ClientID)
        Dim currentTaskInputClientId As String = System.Web.HttpUtility.JavaScriptStringEncode(Hidden_id_tarea_selecionada.ClientID)
        Dim startupScript As String = "(function(){var trigger=document.getElementById('workflow-transition-trigger');if(!trigger){return;}trigger.setAttribute('data-workflow-modern-active','true');trigger.setAttribute('data-workflow-current-task-input-id','" & currentTaskInputClientId & "');trigger.setAttribute('data-workflow-task-input-id','" & taskInputClientId & "');if(window.WorkflowTransitionUi&&typeof window.WorkflowTransitionUi.inicializar==='function'){window.WorkflowTransitionUi.inicializar();}}());"

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "workflowTransitionModernBootstrap", startupScript, True)
    End Sub

    Private Function MilisegundosDesdeInicioRequest() As Long
        Return CLng((DateTime.Now - HttpContext.Current.Timestamp).TotalMilliseconds)
    End Function

    Private Sub Webworkflow_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        System.Diagnostics.Debug.WriteLine("WF_LIFECYCLE|Webworkflow.PreInit|" & MilisegundosDesdeInicioRequest() & " ms desde inicio request")
    End Sub

    Private Sub Webworkflow_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        System.Diagnostics.Debug.WriteLine("WF_LIFECYCLE|Webworkflow.Unload|" & MilisegundosDesdeInicioRequest() & " ms desde inicio request")
    End Sub

    <WebMethod()>
    Public Shared Function Inicializa() As String
        Inicializa = "YES"
    End Function
    Private Sub Webworkflow_Init(sender As Object, e As EventArgs) Handles Me.Init
        System.Diagnostics.Debug.WriteLine("WF_LIFECYCLE|Webworkflow.Init entrada|" & MilisegundosDesdeInicioRequest() & " ms desde inicio request")
        ImageDraw.LicenseOwner = "Miguel Angel Urueta Miranda-Developer License"
        ImageDraw.LicenseKey = "28Q48MH26VEUUW84A4FH9YV8Q33LJ7PC6WF84EZF3AMC93SVP2FQ"
        System.Diagnostics.Debug.WriteLine("WF_LIFECYCLE|Webworkflow.Init salida|" & MilisegundosDesdeInicioRequest() & " ms desde inicio request")
    End Sub
    Public mEval As New ClassEdtiScript

    Private Sub RegistraTiempoLoadWorkflow(ByVal nombreFuncion As String,
                                           ByVal cronometro As Stopwatch,
                                           Optional ByVal resultado As String = "")
        Dim detalleResultado As String = cronometro.ElapsedMilliseconds & " ms"
        If resultado <> "" Then
            detalleResultado &= " - Resultado=" & resultado.Replace("|", "/").Replace("¬", "/")
        End If
        Dim mensaje As String = nombreFuncion & "|" & detalleResultado
        System.Diagnostics.Debug.WriteLine("WF_LOAD|" & mensaje)
        HttpContext.Current.Trace.Warn("WF_LOAD", mensaje)
        HttpContext.Current.Session.Item("DETALLE_SESION") =
            Convert.ToString(HttpContext.Current.Session.Item("DETALLE_SESION")) &
            "WF_LOAD " & mensaje & "¬"
    End Sub

    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        System.Diagnostics.Debug.WriteLine("WF_LIFECYCLE|Webworkflow.Page_Load entrada|" & MilisegundosDesdeInicioRequest() & " ms desde inicio request")
        ConfigureWorkflowCentroTrabajoViewport()
        ConfigureWorkflowTransitionModernPresentation()
        Dim cronometroTotal As Stopwatch = Stopwatch.StartNew()
        Try
            Dim cs As ClientScriptManager = Page.ClientScript
            Dim scr As [String] = "$(document).ready(function () {$().clired_user();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr, True)
            End If
            Dim scr2 As [String] = "$(document).ready(function () {$().auto_postback();});"
            If (Not cs.IsClientScriptBlockRegistered(MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""))) Then
                ScriptManager.RegisterClientScriptBlock(MyBase.Page, MyBase.Page.[GetType](), [String].Format("jQuery_{0}", ""), scr2, True)
            End If
            Dim Ref As New ClassListandoTareas
            Dim RefClasele As New Classselecciotarea
            Dim Result As String = ""
            Dim Tablecolum As New Table
            Dim OBE As Object
            Dim Mens As New Classscrripjava
            Page.MaintainScrollPositionOnPostBack = True
            If HttpContext.Current.Session("SesionActiva") = "" Then
                FormsAuthentication.RedirectFromLoginPage("../gestor.aspx", False)
            End If
            HttpContext.Current.Session("SesionActiva") = "YES"
            If Me.IsPostBack = False And AjaxFileUpload_dowload.IsInFileUploadPostBack = False Then

                '----------------------------------------------------
                'Retorna id ruta
                '----------------------------------------------------
                Dim Ref_clas_rutas As New Class_worflow_rutas
                Dim cronometroFuncion As Stopwatch = Stopwatch.StartNew()
                Result = Ref_clas_rutas.Retorna_nombre_ruta_por_id_ruta(Session.Item("Id_Ruta_Workflow").ToString,
                                                                        Session.Item("WF_RUTAWORKFLOW"))
                cronometroFuncion.Stop()
                RegistraTiempoLoadWorkflow("Retorna_nombre_ruta_por_id_ruta", cronometroFuncion, Result)
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Error consultado ruta|" & Result & "||"
                    Me.LabelEspera.Text = Result
                End If
                If HttpContext.Current.Session.Item("DEVOLVER_TAREA_WORKFLOW") = 0 Then
                    Panel_EnviarUsuario.Visible = False
                Else
                    Panel_EnviarUsuario.Visible = True
                End If
                '************************************
                'Lista actividades de envio
                '************************************
                Result = ""
                Dim Actividad_Seleccion As Integer = 0
                Dim id_actividad As Integer = 0
                Dim Refclas_ As New Classselecciotarea
                Result = ""
                Dim TipoActividad As String = ""
                cronometroFuncion.Restart()
                Result = Refclas_.Determina_Tipo_Actividad_Usuario(id_actividad,
                                                                   TipoActividad)
                cronometroFuncion.Stop()
                RegistraTiempoLoadWorkflow("Determina_Tipo_Actividad_Usuario", cronometroFuncion, Result)
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "DETERMINA TIPO ACTIVIDAD | " & Result & "||"

                End If
                HttpContext.Current.Session.Item("Id_actividad_Workflow") = id_actividad
                HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD") = id_actividad
                HttpContext.Current.Session.Item("TIPOACTIVIDADWF") = TipoActividad
                '**********************************
                'Compilando script wf
                '**********************************

                If HttpContext.Current.Session.Item("UTIL_PAGINACION") = 1 Then
                    Me.GridView2.AllowPaging = True
                Else
                    Me.GridView2.AllowPaging = False
                End If
                Me.Hidden_00020_4001.Value = HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE")
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0"
                HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
                HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
                HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
                DropDownListseleccionfiltro.Items.Clear()
                DropDownListseleccionfiltro.Items.Add("tareas (todas)")
                DropDownListseleccionfiltro.Items.Add("tareas de grupo")
                DropDownListseleccionfiltro.Items.Add("tareas de usuario")
                If HttpContext.Current.Session.Item("UTIL_ITER_PENDIENTE") = 0 Then
                    DropDownListseleccionfiltro.Items.Add("tareas en proceso")
                    DropDownListseleccionfiltro.Items.Add("tareas en espera")
                End If
                Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF") = "Todas"
                Session.Item("SortExpression_compartido_WF") = "etw.fecha_inicio"
                Session.Item("SortDirection_compartido_WF") = "DESC"
                If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") <> 0 Then
                    cronometroFuncion.Restart()
                    Result = Ref.Inicializar_la_lista_de_tareas_workflow(Me.Page,
                                                                         OBE,
                                                                         Me.GridView2,
                                                                         1,
                                                                         1,
                                                                         "",
                                                                         Session.Item("SortExpression_compartido_WF"),
                                                                         Session.Item("SortDirection_compartido_WF"),
                                                                         0,
                                                                         Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                    cronometroFuncion.Stop()
                    RegistraTiempoLoadWorkflow("Inicializar_la_lista_de_tareas_workflow", cronometroFuncion, Result)
                    If Result <> "YES" Then
                        HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Actividades de Grupo|" & Result & "||"
                        Me.LabelEspera.Text = Result
                    End If
                Else
                    LabelEspera.Text = "No tiene permiso para listar tareas"
                End If
                If Session.Item("Parametro_Intervalo_workflow") <> -1 Then
                    'TimerActualiza.Enabled = True
                    'TimerActualiza.Interval = Session.Item("Parametro_Intervalo_workflow")
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Parametro de Actualizacion|" & Session.Item("Parametro_Intervalo_workflow") & "||"
                End If
                Hidden_intervalo_search.Value = Session.Item("Parametro_Intervalo_workflow")

                '**************************************
                'Verifica tarea seleccionada
                '**************************************
                cronometroFuncion.Restart()
                Result = RefClasele.Verifica_Tarea_Seleccionada_Uusario_Inicio(Me.Page)
                cronometroFuncion.Stop()
                RegistraTiempoLoadWorkflow("Verifica_Tarea_Seleccionada_Uusario_Inicio", cronometroFuncion, Result)
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Parametro de Actualizacion|" & Result & "||"
                    'RefClasele.Agrega_treview_error_seleccion(Me.TreeViewseleccion,
                    '                                          Me.Page)
                End If
                '---------------------------------------------
                'Captura el intervalo de alarma de workflow
                '--------------------------------------------
                Dim RefclasUsuario As New ClassWorkflowUsuario
                cronometroFuncion.Restart()
                Result = RefclasUsuario.Intervalo_Alarma_Usuario()
                cronometroFuncion.Stop()
                RegistraTiempoLoadWorkflow("Intervalo_Alarma_Usuario", cronometroFuncion, Result)
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "Error buscando paramnetro de alarma|" & Result & "||"
                End If
                '**********************************************
                'Crea directorio temporal workflow
                '**********************************************
                Dim Refclas As New InicioWorkflow
                HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "ESTADO WEB SERVICE WF ESTADO |" & Session.Item("ACTIVA_WEB_SERVICE") & " " &
                Session.Item("URL_WEB_SERVICE") & "||"
                HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "ESTADO WEB SERVICE DA ESTADO :" & Session.Item("DA_ACTIVA_WEB_SERVICE") & " " &
                Session.Item("DA_URL_WEB_SERVICE") & "||"
                HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "ESTADO WEB SERVICE RA ESTADO |" & Session.Item("RA_ACTIVA_WEB_SERVICE") & " " &
                Session.Item("RA_URL_WEB_SERVICE") & "||"
                HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "ESTADO WEB SERVICE GD ESTADO |" & Session.Item("GA_ACTIVA_WEB_SERVICE") & " " &
                 Session.Item("GA_URL_WEB_SERVICE") & "||"
                'Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
                'Result = Class_estados_tarea_workflow.SolicitaNumeroActividadesSelecionadasUsuario(id_actividad,
                '                                                                                   HttpContext.Current.Session("Id_Usuario_Workflow"),
                '                                                                                   Actividad_Seleccion)
                'If Result <> "YES" Then
                '    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "NUMERO TAREAS SELECCIONADAS | " & Result & "||"
                'End If
                cronometroFuncion.Restart()
                Result = Refclas.Inicializa_firma_usuario_workflow()
                cronometroFuncion.Stop()
                RegistraTiempoLoadWorkflow("Inicializa_firma_usuario_workflow", cronometroFuncion, Result)
                If Result <> "YES" Then
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "RUTA FIRMA ERROR| " & HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL") & "||"
                Else
                    HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL") = HttpContext.Current.Session("WF_RUTA_FIRMA") & HttpContext.Current.Session("Id_Usuario_Workflow") & ".bmp"
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & "RUTA FIRMA  | " & Result & "||"
                End If
                HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION").ToString.Replace("||", "¬")
                Hiddenintercambio2.Value = HttpContext.Current.Session("WF_RUTA_FIRMA_FINAL")
                Dim refclas2 As New ClassNeodynamic
                cronometroFuncion.Restart()
                Dim resultadoFirmaTransparente As String = refclas2.Firma_transparente()
                cronometroFuncion.Stop()
                RegistraTiempoLoadWorkflow("Firma_transparente", cronometroFuncion, resultadoFirmaTransparente)
            Else
                Dim Matri_Temp() As String
                Erase Matri_Temp
                If Session.Item("WF_MATRI_IMAGE") <> "" Then
                    Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
                    If Not Matri_Temp Is Nothing Then
                        For i As Integer = 0 To Matri_Temp.Length - 2
                            ReDim Preserve Matri_Doc_Visual(i)
                            Matri_Doc_Visual(i) = Matri_Temp(i)
                        Next
                    End If
                End If
                Me.Hidden_seccion.Value = "YES"
            End If
            Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("WF_LOAD|EXCEPCION|" & ex.ToString())
            HttpContext.Current.Trace.Warn("WF_LOAD", "EXCEPCION|" & ex.ToString())
            HttpContext.Current.Session.Item("DETALLE_SESION") =
                Convert.ToString(HttpContext.Current.Session.Item("DETALLE_SESION")) &
                "WF_LOAD EXCEPCION|" & ex.Message.Replace("|", "/").Replace("¬", "/") & "¬"
        Finally
            cronometroTotal.Stop()
            RegistraTiempoLoadWorkflow("TOTAL_PAGE_LOAD", cronometroTotal)
        End Try

    End Sub

    '--------Proced actualiza gredview actividades de grupo por medio de funcion java script 
    Private Sub Buttonactividad_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Buttonactividad.Click

        Dim OBE As Object = Nothing
        Dim Ref As New ClassListandoTareas
        Dim RefMesaje As New Classscrripjava
        Try
            Me.HiddenSeleccion.Value = "-1"
            If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0 Then
                LabelEspera.Text = "El usuario no tiene permiso para listar tareas"
                Me.UpdatePanelnumeroespera.Update()
                Exit Sub
            End If
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 1 Then
                Result = Ref.Pre_Listar_tareas_workflow_Script(Me.Page,
                                                               OBE,
                                                               Me.GridView2,
                                                               1,
                                                               HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                               HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                               HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                               HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                               Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    RefMesaje.Show(Result)
                    Exit Sub
                End If
            End If
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 0 Then
                Result = Ref.Pre_Listar_tareas_workflow(Me.Page,
                                                        OBE,
                                                        Me.GridView2,
                                                        1,
                                                        HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                        HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                        HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                        HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                        Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    RefMesaje.Show(Result)
                    Exit Sub
                End If
            End If
            'If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") <> 0 Then
            '    Dim Result As String = ""
            '    Result = Ref.Inicializar_la_lista_de_tareas_workflow(Me.Page,
            '                                                         OBE,
            '                                                         Me.GridView2,
            '                                                         1,
            '                                                         HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
            '                                                         HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
            '                                                         HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
            '                                                         HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
            '                                                         Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
            '    If Result <> "YES" Then
            '        RefMesaje.Show(Result)
            '        Exit Sub
            '    Else
            '        Exit Sub
            '    End If

            '    'End If
            'Else
            '    LabelEspera.Text = "No tiene permiso para listar tareas"
            '    Me.UpdatePanelnumeroespera.Update()
            '    Exit Sub
            'End If

        Catch ex As Exception
            RefMesaje.Show(ex.Message)
        End Try
    End Sub

    '-----sub para actaulizar imagenes relacionadas del popup enlace
    Protected Sub Buttonactualizar_Click(ByVal sender As Object,
                                         ByVal e As EventArgs) Handles Buttonactualizar.Click
        Dim Refclas As New Classselecciotarea
        Dim Mens As New Classscrripjava
        Dim SplitParan() As String
        Dim Result As String = ""
        Erase SplitParan
        '0-id_tarea
        '1-id_actividad
        '2-Index
        '3-TipoActividad
        SplitParan = Split(Session.Item("SELECCIONTEMPORAL"), "|")
        If SplitParan Is Nothing Then
            Mens.Showscripman("Lo paramentros id_tarea, id activdad index son nulos", Me.UpdateDatos)
            Me.UpdatePanelnumeroespera.Update()
            Exit Sub
        End If
        '---------Ejecuta el escript enlace
        Dim ClassEdtiScript As New ClassEdtiScript
        Result = ClassEdtiScript.EjecutaEventoEnlaceDocumentosWorkflow(SplitParan(0),
                                                                      SplitParan(1),
                                                                      HttpContext.Current.Session("ENLASE"))
        If Result <> "YES" Then
            Mens.Showscripman(Result, Me.UpdateDatos)
            Me.UpdatePanelnumeroespera.Update()
            Exit Sub
        End If
        Me.UpdateDatos.Update()
        If Result <> "YES" Then
            Mens.Showscripman(Result, Me.UpdateDatos)
            Me.UpdatePanelnumeroespera.Update()
            Exit Sub
        Else
            Me.UpdatePanelnumeroespera.Update()
        End If
    End Sub

    Private Sub ImageButtonautoterminar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonautoterminar.Click
        Dim showmensaje As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim id_actividad As Integer = 0
            Dim id_usuario_workflow As Integer = 0
            Dim mensaje As String = ""
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Result = Refclas_f.Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                           HttpContext.Current.Session.Item("Id_Usuario_Workflow"))
            If Result <> "YES" Then
                showmensaje.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Auto_terminar_flujo_documental(id_usuario_workflow,
                                                            id_actividad,
                                                            mensaje)
            If Result <> "YES" Then
                showmensaje.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.LabelMensaje_autoterminar.Text = mensaje
            Me.Hidden_id_actividad.Value = id_actividad
            Me.Hidden_id_usuario.Value = id_usuario_workflow
            Me.updatepanel_mensaje_extender_autoterminar.Update()
            Me.updatepanel_mensaje_extender_autoterminar.Update()
            Me.ModalPopupExtendermensaje_autoterminar.Show()
        Catch ex As Exception
            showmensaje.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub
    '-------Desencadena evento aceptar del popup mensaje
    Private Sub btnokay_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOkay.Click
        Dim refclas As New ClassWorkflow
        Dim mens As New Classscrripjava
        Try
            Dim result As String = ""
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If result <> "YES" Then
                mens.Showscripman(result, Me.updatepanel_mensaje_extender)
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                                         Estado_solicitud_aprobacion,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If result <> "YES" Then
                mens.Showscripman(result, Me.updatepanel_mensaje_extender)
                Exit Sub
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                mens.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.updatepanel_mensaje_extender)
                Exit Sub
            End If
            Dim Resultado_envalua_terminar As String = ""
            result = refclas.Terminar_Tarea_Workflow("",
                                                     "",
                                                     HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                     "",
                                                     Me.Page,
                                                     Resultado_envalua_terminar)
            If result <> "YES" Then
                mens.Showscripman(result, updatepanel_mensaje_extender)
                Exit Sub
            Else
                Me.ModalPopupExtendermensaje.Hide()
                Me.UpdatePanelseleccion.Update()
                Me.Hidden_id_tarea_selecionada.Value = "0"
                UpdatePanel_general_variable.Update()
                Dim refcla As New ClassWorflowVisor
                Dim Resutl As String = ""
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    mens.Showscripman(Resutl, updatepanel_mensaje_extender)
                End If
                If Resultado_envalua_terminar <> "YES" Then
                    mens.Showscripman(Resultado_envalua_terminar, Me.updatepanel_mensaje_extender)
                End If
            End If
        Catch ex As Exception
            mens.Showscripman(ex.Message, updatepanel_mensaje_extender)
        End Try
    End Sub

    Private Sub GridView_envia_actividades_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_envia_actividades.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ImageButtonanotacion__Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonanotacion_.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim Result As String = ""
            If HiddenSeleccion.Value = "-1" Or HiddenSeleccion.Value = "" Then Exit Sub
            If HttpContext.Current.Session("Interactuar_Anotaciones") = "0" Then
                refclsjava.Showscripman("El usuario no tiene permiso para interactuar con anotaciones ", UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim refclas As New Class_anotacion_tarea
            Result = refclas.Listar_Anotaciones_tarea_workflow(Me.GridView_lista_notas,
                                                               Val(HiddenSeleccion.Value))
            If Result <> "YES" Then
                refclsjava.Showscripman(Result, UpdatePanel_tool_menu)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_content_anotacion.Show()
                Exit Sub
            End If

        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_tool_menu)
        End Try
    End Sub
    '-------Anotacion sobre la actividad
    Private Sub ImageButtonanotacion_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonanotacion.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim Result As String = ""
            If Session.Item("ID_TAREA_SELECCIONDA") = "0" Then Exit Sub
            If HttpContext.Current.Session("Interactuar_Anotaciones") = "0" Then
                refclsjava.Showscripman("El usuario no tiene permiso para interactuar con anotaciones ", UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim refclas As New Class_anotacion_tarea
            Result = refclas.Listar_Anotaciones_tarea_workflow(Me.GridView_lista_notas,
                                                               HttpContext.Current.Session("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                refclsjava.Showscripman(Result, UpdatePanel_tool_menu)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_content_anotacion.Show()
                Exit Sub
            End If

        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_tool_menu)
        End Try
    End Sub



    '----- boton para cerrar el popup
    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.ModalPopupExtendermensaje.Hide()
        'Me.UpdatePanelseleccion.Update()
    End Sub

    '----boton para asignar tareas de pendiente
    Private Sub ButtonAsignar_Aprobacion_Click(sender As Object, e As EventArgs) Handles ButtonAsignar_Aprobacion.Click
        Dim Mens As New Classscrripjava
        Try
            If Session.Item("OPCIONSELECION") = "PENDIENTE" Then
                Dim refclas As New Class_tarea_pendiente
                Dim Result As String = ""
                If Me.hdnEmailID.Value = "0" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                Else
                    Session.Item("SESIONITERCAMBIO") = Me.hdnEmailID.Value
                End If
                If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") <> "0" Then
                    Mens.Showscripman("Usuario con tarea seleccionada imposible asignar el documento", Me.UpdatePanelpedieteboton)
                    Exit Sub
                End If
                If Session.Item("SESIONITERCAMBIO") = "" Then
                    Mens.Showscripman("Imposible asignar tarea por favor seleccione una tarea", Me.UpdatePanelpedieteboton)
                    Exit Sub
                End If
                Dim Split() As String = Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                Result = refclas.Sacar_Tarea_Pendiente(Split(0),
                                                       Split(1),
                                                       Me.TreeViewseleccion,
                                                       Me.Page)
                If Result <> "YES" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Me.hdnEmailID.Value = ""
                    Mens.Showscripman(Result, Me.UpdatePanelpedieteboton)
                    Me.Iframependiente_.Attributes("SRC") = ""
                    Me.ModalPopupExtenderpendiente.Hide()
                    Exit Sub
                Else
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Me.hdnEmailID.Value = ""
                    HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Split(1)
                    Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UpdatePanel_general_variable.Update()
                    Dim refclasvisor As New ClassWorflowVisor
                    refclasvisor.Limpia_Visor_Workflow(Me.Page, "")
                    Me.ModalPopupExtenderpendiente.Hide()
                    Me.UpdatePanelintercambio.Update()
                    Me.UpdatePanelseleccion.Update()

                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanelpedieteboton)
            Exit Sub
        End Try
    End Sub

    Private Sub Button_tool_menucab_Click(sender As Object, e As EventArgs) Handles Button_tool_menucab.Click
        Dim Refclasjava As New Classscrripjava
        Dim Result As String = ""
        Try

            Dim Valselect As String = Me.Hidden_menucab.Value
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            If Valselect = "G-TTN" Then
                If Session.Item("RESPUESTA_TRAMITE") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario no tiene usuario permiso para responder el trámite", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") <> "0" Then
                    If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = "0" Then
                        Refclasjava.Showscripman_menu("El usuario workflow no tiene usuario de gestión relacionado", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                    Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                         Radicado)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result,
                                                      Me.UpdatePanel_tool_menucab,
                                                      "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    If Radicado = "" Then
                        Refclasjava.Showscripman_menu("La tarea seleccionada no tiene radicado relacionado ",
                                                      Me.UpdatePanel_tool_menucab,
                                                      "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim refclas_resp As New Classgestionrespuesta

                    Dim id_respuesta As Integer = 0
                    Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                                       HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                       id_respuesta)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result,
                                                      Me.UpdatePanel_tool_menucab,
                                                      "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    If id_respuesta = 0 Then
                        Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado, id_respuesta)
                        If Result <> "YES" Then
                            Refclasjava.Showscripman_menu(Result,
                                                          Me.UpdatePanel_tool_menucab,
                                                          "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        End If
                        If id_respuesta = 0 Then
                            Refclasjava.Showscripman_menu("El radicado actual no tiene una respuesta relacionada",
                                                          Me.UpdatePanel_tool_menucab,
                                                          "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        Else
                            Refclasjava.Showscripman_menu("El usuario no tiene asiganda la tarea para gestionar la respuesta",
                                                          Me.UpdatePanel_tool_menucab,
                                                          "ModalPopupExtender_mensaje_personalizado")

                            Exit Sub
                        End If
                    Else

                        Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                             Radicado)
                        If Result <> "YES" Then
                            Refclasjava.Showscripman_menu(Result,
                                                          Me.UpdatePanel_tool_menucab,
                                                          "ModalPopupExtender_mensaje_personalizado")
                            Exit Sub
                        End If
                        Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                        Hidden_radic_select.Value = Radicado
                        UpdatePanel_general_variable.Update()
                    End If

                End If
            End If
            '-----------------------------------------------
            'Envia tarea pendiente por aprobación
            '-----------------------------------------------
            If Valselect = "E-ETPA" Then
                If Session.Item("ESTADO_PENDIENTE_APROBACION") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario no tiene permiso para enviar tarea a estado de aprobación ",
                                                  Me.UpdatePanel_tool_menucab,
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Session.Item("ID_TAREA_SELECCIONDA") = 0 Or Session.Item("ID_TAREA_SELECCIONDA") = -1 Then
                    Refclasjava.Showscripman_menu("El sistema no detecto tareas seleccionada ",
                                                  Me.UpdatePanel_tool_menucab,
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    'Me.Label_tex_envia_documento_pendiente.Text = "Desea enviar la tarea a pendientes por aprobación ? "
                    'Me.TextBox_texto_pendiente_aprobacion.Text = "Pendiente por aprobación"
                    UpdatePanel_envia_documento_pendiente_apro.Update()
                    Me.ModalPopupExtender_edition_envia_documento_pendiente_apro.Show()
                End If
            End If
            '-----------------------------------------------
            'Envia tarea pendiente 
            '-----------------------------------------------
            If Valselect = "E-ETP" Then
                If Session.Item("ID_TAREA_SELECCIONDA") = 0 Or Session.Item("ID_TAREA_SELECCIONDA") = -1 Then
                    Refclasjava.Showscripman_menu("El sistema no detecto tareas seleccionada ",
                                                  Me.UpdatePanel_tool_menucab,
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    'Me.Label_tex_envia_documento_pendiente.Text = "Desea enviar la tarea a pendientes ?"
                    Me.TextBox_texto_pendiente_aprobacion.Text = ""
                    UpdatePanel_envia_documento_pendiente_apro.Update()
                    Me.ModalPopupExtender_edition_envia_documento_pendiente_apro.Show()
                End If
            End If
            '------------------------------------------------
            'Lista tareas pendientes
            '------------------------------------------------
            If Valselect = "E-LTEP" Then
                Dim refclsjava As New Classscrripjava

                If HttpContext.Current.Session("Interactuar_Pendiente") = "0" Then
                    refclsjava.Showscripman_menu("El usuario no tiene permiso para interactuar con tareas pendientes",
                                                 Me.UpdatePanel_tool_menucab,
                                                 "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.hdnEmailID.Value = "0"
                Me.Hidden1.Value = "PENDIENTE"
                Me.UpdatePanelintercambio.Update()
                Session.Item("OPCIONSELECION") = "PENDIENTE"
                Session.Item("SESIONITERCAMBIO") = ""
                Me.Labelpendiente.Text = "Tareas pendiente"
                Me.Iframependiente_.Attributes("SRC") = "../workflow/WebFormPendientes.aspx"
                Me.UpdatePanelpendiente.Update()
                Me.ModalPopupExtenderpendiente.Show()
                Me.Hidden_estado_tareas_pendiente.Value = "NO"
                updatemenu.Update()

            End If
            '-------------------------------------------------
            'Lista tareas aprobadas o desaporbadas
            '-------------------------------------------------
            If Valselect = "E-LTEPA" Then
                If Session.Item("LISTA_ESTADO_PENDIENTE_APROBACION") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario no tiene permiso para interactuar con la lista de tareas en estado de aprobación",
                                                  Me.UpdatePanel_tool_menucab,
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.hdnEmailID.Value = "0"
                Me.Hidden1.Value = "PENDIENTE"
                Me.UpdatePanelintercambio.Update()
                Session.Item("OPCIONSELECION") = "PENDIENTE"
                Session.Item("SESIONITERCAMBIO") = ""
                Me.Labelpendiente.Text = "Solicitudes de aprobación"
                Me.Iframependiente_.Attributes("SRC") = "../radicador/WebFormListaSolicitudesAprobacion.aspx"
                Me.UpdatePanelpendiente.Update()
                Me.ModalPopupExtenderpendiente.Show()
                Me.Hidden_estado_pendiente_aprobacion.Value = "NO"
                updatemenu.Update()
            End If
            '-----------------------------------------------
            'Comparte el documento seleccionado
            '-----------------------------------------------
            If Valselect = "D-CDW" Then
                If Session.Item("COMPARTE_USUARIO_INTERNO") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario no tiene permiso para compartir documentos",
                                                  Me.UpdatePanel_tool_menucab,
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Session.Item("ID_TAREA_SELECCIONDA") = 0 Or Session.Item("ID_TAREA_SELECCIONDA") = -1 Then
                    Refclasjava.Showscripman_menu("El sistema no detecto tareas seleccionada para compartir documentos",
                                                  Me.UpdatePanel_tool_menucab,
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Dim Refclas As New ClassWorkflow
                    Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                    Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                         Session.Item("GA_STRU_DOCUMENTO_RADICADO"))
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result,
                                                      Me.UpdatePanel_tool_menucab,
                                                      "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim ref_ClassDaGabinete As New ClassDaGabinete
                    Result = ref_ClassDaGabinete.Retorna_Matriz_imagenes_relacionadas_a_tarea(Session.Item("ID_TAREA_SELECCIONDA"))
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result,
                                                      Me.UpdatePanel_tool_menucab,
                                                      "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Session.Item("GA_STRU_DOCUMENTO_TIPO_COMPARTIDO") = "COMPARTIR WORKFLOW"
                    Me.Iframe_compartir_documento_.Attributes("SRC") = "../gestion/WebFormGaCompartirDocumento.aspx"
                    Me.UpdatePanel_autoriza_compartir_documento.Update()
                    Me.ModalPopupExtender_edition_autoriza_compartir_documento.Show()
                End If

            End If
            '-------------------------------------------------------
            'Envío de documentos de la tarea por correo electrónico
            '-------------------------------------------------------
            If Valselect = "D-CEDTS" Then
                If Session.Item("COMPARTE_CORREO_ELECTRONICO") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario no tiene permiso para compartir el documento a correos electrónicos",
                                                  Me.UpdatePanel_tool_menucab,
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Session.Item("ID_TAREA_SELECCIONDA") = 0 Or Session.Item("ID_TAREA_SELECCIONDA") = -1 Then
                    Refclasjava.Showscripman_menu("El sistema no detecto tareas seleccionada para compartir documentos",
                                                  Me.UpdatePanel_tool_menucab,
                                                  "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Dim Refclas As New ClassWorkflow
                    Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                    Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                          Session.Item("GA_STRU_DOCUMENTO_RADICADO"))
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result,
                                                      Me.UpdatePanel_tool_menucab,
                                                      "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim ref_ClassDaGabinete As New ClassDaGabinete
                    Result = ref_ClassDaGabinete.Retorna_Matriz_imagenes_relacionadas_a_tarea(Session.Item("ID_TAREA_SELECCIONDA"))
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result,
                                                      Me.UpdatePanel_tool_menucab,
                                                      "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Me.Hidden_ruta_tempo.Value = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\"
                    Me.Hidden_cuenta_correo_envio.Value = ""
                    Me.Hidden_correo_envio_default.Value = ""
                    Me.Iframe_comparte_coreo.Attributes.Add("src", "../radicador/WebFormNotificar.aspx")
                    Me.UpdatePanel_iframenotifica.Update()
                    ModalPopupExtender_notifica_gestion.Show()
                End If
            End If
            'Recuperar tarea
            If Valselect = "T-RTW" Then
                Me.hdnEmailID.Value = "0"
                Me.UpdatePanelintercambio.Update()
                Me.Hidden1.Value = "RECUPERARTAREA"
                Session.Item("OPCIONSELECION") = "RECUPERARTAREA"
                Session.Item("SESIONITERCAMBIO") = ""
                Me.IframeRecuperar_.Attributes("SRC") = "../workflow/WebFormRecuperarTarea.aspx"
                Me.UpdatePanelRecuperar.Update()
                Me.ModalPopupExtenderRecuperar.Show()
            End If
            If Valselect = "S-CPT" Then
                If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                    Exit Sub
                End If
                Me.hdnEmailID.Value = "0"
                Me.UpdatePanelintercambio.Update()
                Me.Hidden1.Value = "PRIORIDAD"
                Session.Item("OPCIONSELECION") = "PRIORIDAD"
                Session.Item("SESIONITERCAMBIO") = ""
                Me.Labeladver.Text = "Prioridades"
                Me.Iframe_auxiliares_.Attributes("SRC") = "../workflow/WebFormCambiarPrioridad.aspx"
                Me.UpdatePanel_auxiliares.Update()
                Me.ModalPopupExtender_auxiliares.Show()
            End If
            If Valselect = "U-CCU" Then
                Me.hdnEmailID.Value = "0"
                Me.UpdatePanelintercambio.Update()
                Me.Hidden1.Value = "PASWORD"
                Session.Item("OPCIONSELECION") = "PASWORD"
                Session.Item("SESIONITERCAMBIO") = ""
                Me.Labeladver.Text = "Cambiar Contraseña"
                Me.Iframe_auxiliares_.Attributes("SRC") = "../workflow/WebWorkflowCambiarPasword.aspx"
                Me.UpdatePanel_auxiliares.Update()
                Me.ModalPopupExtender_auxiliares.Show()
            End If
            '"U-CIA"
            If Valselect = "U-CIA" Then
                Me.hdnEmailID.Value = "0"
                Me.UpdatePanelintercambio.Update()
                Me.Hidden1.Value = "ALARMA"
                Session.Item("OPCIONSELECION") = "ALARMA"
                Session.Item("SESIONITERCAMBIO") = ""
                Me.Labeladver.Text = "Actualizar Intervalo de Alarma"
                Me.Iframe_auxiliares_.Attributes("SRC") = "../workflow/WebWorkflowIntervaloAlarma.aspx"
                Me.UpdatePanel_auxiliares.Update()
                Me.ModalPopupExtender_auxiliares.Show()
            End If
            If Valselect = "U-CAA" Then
                Me.hdnEmailID.Value = "0"
                Me.UpdatePanelintercambio.Update()
                Me.Hidden1.Value = "ACTUALIZACION"
                Session.Item("OPCIONSELECION") = "ACTUALIZACION"
                Session.Item("SESIONITERCAMBIO") = ""
                Me.Labeladver.Text = "Actualizar Intervalo de Actualización"
                Me.Iframe_auxiliares_.Attributes("SRC") = "../workflow/WebWorkflowIntervaloActualizacion.aspx"
                Me.UpdatePanel_auxiliares.Update()
                Me.ModalPopupExtender_auxiliares.Show()
            End If
            'S-DDS
            If Valselect = "S-DDS" Then
                Dim Ref_class_inicio As New InicioWorkflow
                Result = Ref_class_inicio.Genera_detalle_sesion_workflow(HttpContext.Current.Session.Item("DETALLE_SESION"),
                                                                         Me.Table_detalle_session,
                                                                         Me.UpdatePanel_detalle_sesion)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.ModalPopupExtender_edition_detalle_sesion.Show()
            End If
            If Valselect = "S-DTS" Then
                Dim Refclas As New Class_DAT_ADIC_TAR
                If Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                    Refclasjava.Showscripman_menu("No hay tarea seleccionada imposible mostrar detalle", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim structure_table_detalle_flujo() As structure_table_detalle_flujo = Nothing
                Result = Refclas.Listar_datos_tarea_workflow(structure_table_detalle_flujo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas.Genera_interface_detalle_tarea_workflow(structure_table_detalle_flujo,
                                                                         Me.Table_detalle_flujo,
                                                                         Me.UpdatePanel_detalle_flujo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.ModalPopupExtender_edition_detalle_flujo.Show()
            End If
            '--------------------------------------------------
            'Activa la respuesta al documento 
            '--------------------------------------------------
            If Valselect = "G-TTA" Then
                If Session.Item("RESPUESTA_TRAMITE") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario no tiene permiso para responder el trámite", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                    Refclasjava.Showscripman_menu("Debe seleccionar el tramite a responder", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = "0" Then
                    Refclasjava.Showscripman_menu("El usuario workflow no tiene usuario de gestión relacionado", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Session.Item("RESPUESTA_TRAMITE") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario workflow no tiene permiso para tramitar respuesta", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Radicado)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Radicado = "" Then
                    Refclasjava.Showscripman_menu("La tarea seleccionada no tiene radicado relacionado ", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

                'Dim refclas_resp As New Classgestionrespuesta
                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                   id_respuesta)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If id_respuesta = 0 Then
                    Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado, id_respuesta)
                    If Result <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    If id_respuesta = 0 Then

                        Refclasjava.Showscripman_menu("El radicado actual no tiene una respuesta relacionada", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Refclasjava.Showscripman_menu("El usuario no tiene asiganda la tarea para gestionar la respuesta", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                End If
                Me.Hidden_radicado.Value = Radicado
                Me.Hidden_id_respuesta.Value = id_respuesta
                Me.Iframe_respuesta_radicado_.Attributes.Add("src", "../radicador/WebFormRespuestaRadicado.aspx")
                UpdatePanel_respuesta_radicado.Update()
                ModalPopup_respuesta_radicado.Show()
            End If
            '----------------------------------------------
            'Muestra detalle de respuesta
            '----------------------------------------------
            If Valselect = "G-DRR" Then
                If Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                    Refclasjava.Showscripman_menu("No hay tarea seleccionada imposible mostrar detalle", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Radicado As String = ""
                Dim Refclas As New ClassWorkflow
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Radicado)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim estru As stru_envio = Nothing

                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                   id_respuesta)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If id_respuesta = 0 Then
                    Dim Resulta = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                                   id_respuesta)
                    If Resulta <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    If id_respuesta = 0 Then
                        Refclasjava.Showscripman_menu("El radicado actual no tiene una respuesta relacionada", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
                    Resulta = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta,
                                                                                                 estru)
                    If Resulta <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Session.Item("PU_TRAZABILIDAD") = estru.RADICADO
                        Me.Iframe_visor_externo_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
                        Me.UpdatePanel_detalle_respuesta.Update()
                        Me.ModalPopupExtender_detalle_respuesta.Show()
                    End If
                Else
                    Session.Item("PU_TRAZABILIDAD") = Radicado
                    Me.Iframe_visor_externo_.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
                    Me.UpdatePanel_detalle_respuesta.Update()
                    Me.ModalPopupExtender_detalle_respuesta.Show()
                End If

            End If
            '------------------------------------------------------------
            'Muestra log de la respuesta del radicado
            '-------------------------------------------------------------
            If Valselect = "G-TDR" Then
                If Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                    Refclasjava.Showscripman_menu("No hay tarea seleccionada imposible mostrar detalle", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Radicado)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

                Dim estru As stru_envio = Nothing
                'Dim refclas_resp As New Classgestionrespuesta
                Dim id_respuesta As Integer = 0
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                                   HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                   id_respuesta)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If id_respuesta = 0 Then
                    Dim Resulta = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                                   id_respuesta)
                    If Resulta <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    If id_respuesta = 0 Then
                        Refclasjava.Showscripman_menu("El radicado actual no tiene una respuesta relacionada", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    End If
                    Dim ref_ra_resp_radic As New Class_ra_respuesta_radicado
                    Resulta = ref_ra_resp_radic.Solicita_datos_estructura_envio_por_id_respuesta(id_respuesta, estru)
                    If Resulta <> "YES" Then
                        Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                        Exit Sub
                    Else
                        Session.Item("PU_TRAZABILIDAD") = id_respuesta
                        Me.Iframe_visor_externo_.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                        Me.UpdatePanel_detalle_respuesta.Update()
                        Me.ModalPopupExtender_detalle_respuesta.Show()
                    End If
                Else
                    Session.Item("PU_TRAZABILIDAD") = id_respuesta
                    Me.Iframe_transacciones_.Attributes("SRC") = "../Gestion/WebFormLogRespuestaRadicado.aspx"
                    Me.UpdatePanel_transacciones.Update()
                    Me.ModalPopupExtender_transacciones.Show()
                End If
            End If
            '-------------------------------------------------
            'Muestra la trazabilidad del documento workflow
            '-------------------------------------------------
            If Valselect = "G-TDW" Then
                If Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                    Refclasjava.Showscripman_menu("No hay tarea seleccionada imposible mostrar detalle ", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Refclas As New ClassWorkflow
                Dim Radicado As String = ""
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Radicado)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                'Me.Label12.Text = "Trazabilidad radicado"
                Session.Item("PU_TRAZABILIDAD") = Radicado
                Me.Iframe_trazabilidad_.Attributes("SRC") = "../workflow/WebFormTrazabilidadWorkflow.aspx"
                Me.UpdatePanel_trazabilidad.Update()
                Me.ModalPopupExtender_trazabilidad.Show()
            End If
            If Valselect = "R-T-D" Then
                If Session.Item("REASIGNA_RESPUESTA_TRAMITE") = 0 Then
                    Refclasjava.Showscripman_menu("El usuario no tiene permiso para reasignar el trámite", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                    Me.TextBox_login_autoriza_reasigna.Text = ""
                    Me.TextBox_pasw_autoriza_reasigna.Text = ""
                    Me.UpdatePanel_contenido_reasigna_responsable_tramite.Update()
                    Me.ModalPopupExtender_edition_reasigna_responsable_tramite.Show()
                    Exit Sub
                Else
                    ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Show()
                    Exit Sub
                End If
            End If
            '-------------------------------------------------------
            'Muestra la trazabilidad en modo grafico G-TDWG
            '-------------------------------------------------------
            If Valselect = "G-TDWG" Then
                If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                    Exit Sub
                End If
                Dim Refclas_w As New ClassWorkflow
                Dim Refclas_f As New Class_flujo_trabajo_workflow
                Dim Radicado As String = ""
                Dim Id_flujo_trabajo As Integer = 0
                Dim id_actividad_flujo_trabajo As Integer = 0
                Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
                Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
                Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Radicado)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas_f.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(Radicado,
                                    id_actividad_flujo_trabajo, Id_flujo_trabajo, id_usuario_workflow_actividad_flujo_trabajo,
                                    HttpContext.Current.Session.Item("Id_Usuario_Workflow"), HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Id_flujo_trabajo <> 0 Then
                    Session.Item("DR_ID_FLUJO_TRABAJO") = Id_flujo_trabajo
                    Session.Item("DR_ID_ACTIVIDAD_FLUJO_TRABAJO") = id_actividad_flujo_trabajo
                    Session.Item("DR_ID_USUARIO_WORKFLOW_FLUJO_TRABAJO") = id_usuario_workflow_actividad_flujo_trabajo
                    Session.Item("DR_ID_TAREA_FLUJO_TRABAJO") = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Session.Item("DR_RADICADO_FLUJO_TRABAJO") = Radicado
                    Me.Iframetraza_grafica_.Attributes.Add("SRC", "../workflow/WebFormDiagramaEstadoFlujoTrabajo.aspx")
                    Me.UpdatePaneltraza_grafica.Update()
                    Me.ModalPopupExtendertraza_grafica.Show()
                Else
                    Session.Item("RU_ID_TAREA_RUTA_TRABAJO") = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Session.Item("RU_RADICADO_RUTA_TRABAJO") = Radicado
                    Me.Iframetraza_grafica_.Attributes.Add("SRC", "../workflow/WebFormDiagramadorEstadoRutaWorkflow.aspx")
                    Me.UpdatePaneltraza_grafica.Update()
                    Me.ModalPopupExtendertraza_grafica.Show()
                End If
            End If
            '--------------------------------------------------------------------------
            'Muestra la interfacece de digitalización para agregar un nuevo documento
            '--------------------------------------------------------------------------
            If Valselect = "D-DNDT" Then
                If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                    Refclasjava.Showscripman_menu("Debe haber una tarea seleccionada para desplegar la interface", Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
                Dim Refclas_w As New ClassWorkflow
                Result = Refclas_w.Activa_adjuntar_documento_digitalizado_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                             Me.Page)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            '-----------------------------------------------------
            'Ver grupo o actividad a la que pertenece el usuario 
            '-----------------------------------------------------
            Dim Refclas_workflow_usuario As New ClassWorkflowUsuario
            Dim Nombre_grupo As String = ""
            Dim nombre_actividad As String = ""
            Dim mensaje As String = ""
            If Valselect = "S-GAU" Then
                Result = Refclas_workflow_usuario.Solicita_nombre_grupo_actividad_usuario_workflow(HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                   Nombre_grupo,
                                                                                                   nombre_actividad)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    mensaje = "Grupo  : " & Nombre_grupo & "<br \>" & " Actividad  : " & nombre_actividad
                    Refclasjava.Showscripman_menu(mensaje, Me.UpdatePanel_tool_menucab, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_tool_menucab)
        End Try
    End Sub

    '----Boton image anotacion
    Private Sub btnCancelpagina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelpagina.Click
        Me.ModalPopupExtendermesjpagina.Hide()
    End Sub
    '---------Visualiza documento listview
    Private Sub Buttoncabcel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Buttoncabcel.Click
        Me.ModalPopupExtenderLibre.Hide()
    End Sub
    'activa respuesta documento
    Private Sub Button_activa_respuesta_radicado_Click(sender As Object, e As EventArgs) Handles Button_activa_respuesta_radicado.Click
        Dim Mens As New Classscrripjava
        Try

            If Session.Item("RESPUESTA_TRAMITE") = 0 Then
                Mens.Showscripman_menu("El usuario no tiene permiso para responder el trámite", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If _
             HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Mens.Showscripman_menu("Debe seleccionar el tramite a responder", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = "0" Then
                Mens.Showscripman_menu("El usuario workflow no tiene usuario de gestión relacionado", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Radicado As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                 Radicado)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            If Radicado = "" Then
                Mens.Showscripman_menu("La tarea seleccionada no tiene radicado relacionado", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim id_respuesta As Integer = 0
            Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado(Radicado,
                                                                               HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                               id_respuesta)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                Exit Sub
            End If
            If id_respuesta = 0 Then
                Result = Class_ra_respuesta_radicado.Retorna_id_respuesta_radicado_usuario_no_propietario(Radicado,
                                                                                                          id_respuesta)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_respuesta_radicado)
                    Exit Sub
                End If
                If id_respuesta = 0 Then
                    Mens.Showscripman_menu("El radicado actual no tiene una respuesta relacionada", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Mens.Showscripman_menu("El usuario no tiene asiganda la tarea para gestionar la respuesta", Me.UpdatePanel_respuesta_radicado, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Me.Hidden_radicado.Value = Radicado
            Me.Hidden_id_respuesta.Value = id_respuesta
            Me.Iframe_respuesta_radicado_.Attributes.Add("src", "../radicador/WebFormRespuestaRadicado.aspx")
            ModalPopup_respuesta_radicado.Show()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_respuesta_radicado)
            Exit Sub
        End Try
    End Sub
    'visualiza documento desde el treview seleccion
    Protected Sub TreeViewseleccion_SelectedNodeChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TreeViewseleccion.SelectedNodeChanged
        Dim Mens As New Classscrripjava
        Try
            sender.selectedvalue()
            Dim Valor_Tab_Selccion As String = sender.selectedvalue()
            Dim Valor_Tab_documeto() As String
            Erase Valor_Tab_documeto
            If Valor_Tab_Selccion = "Documentos workflow" Then
                Exit Sub
            End If
            If Valor_Tab_Selccion = "" Then
                Mens.Showscripman("Tarea sin documentos relacionados", Me.UpdatePanelseleccion)
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassVisualisaDocumento
            'Result = Refclas.Visualiza_documento_workflow_visor(Valor_Tab_Selccion, _
            '                                                    Me.ifrm_visor, _
            '                                                    Me.Panel_indice, _
            '                                                    Me.UpdatePanelindice, _
            '                                                    Me.UpdatePanelVisor, _
            '                                                    1, _
            '                                                    Me.HiddenHeigth, _
            '                                                    Me.Page)
            'If Result <> "YES" Then
            '    Mens.Showscripman(Result, Me.UpdatePanelseleccion)
            '    Exit Sub
            'End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanelseleccion)
        End Try

    End Sub
    Private Sub Button_tool_visualiza_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_visualiza_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_result_boton_tool.Value = ""
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Prevent_visualiza_documento_seleccion_envio_radicado(hiden_seleccion_documento.Value,
                                                                                           HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                                           Me.IframeVisor_,
                                                                                           Me.UpdatePanelIframevisor)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Me.Hidden_result_boton_tool.Value = ""
            Else
                Me.Hidden_result_boton_tool.Value = "YES"
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_selecion_treview_documento_Click(sender As Object, e As EventArgs) Handles Button_selecion_treview_documento.Click
        Dim Mens As New Classscrripjava
        Try


            Dim Valor_Tab_Selccion As String = Me.hiden_seleccion_documento_wf.Value
            Dim Result As String = ""
            Dim Refclas As New ClassVisualisaDocumento
            Result = Refclas.Visualiza_documento_workflow_visor(Valor_Tab_Selccion,
                                                                Me.ifrm_visor_,
                                                                Me.Panel_indice,
                                                                Me.UpdatePanelindice,
                                                                Me.UpdatePanelVisor,
                                                                1,
                                                                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                0,
                                                                Me.HiddenHeigth,
                                                                Me.Page,
                                                                Matri_Doc_Visual,
                                                                Doc_actual)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_seleccion_treview)
                Exit Sub
            End If

            If WorkflowCentroTrabajoModernActive Then
                Me.UpdatePanel_panel_toll.Update()
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_seleccion_treview)
        End Try
    End Sub
    Private Sub ButtonTreeviewSeleccion_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonTreeviewSeleccion.Click
        Dim ob As Object = sender
        Dim a As Object = e
        'Dim at As Object = Request.Params.Get("__EVENTTARGET")
        Dim Valor_Tab_Selccion As String = Request.Params.Get("__EVENTARGUMENT")
        Dim valor_tab_documeto() As String
        Erase valor_tab_documeto
        valor_tab_documeto = Split(Valor_Tab_Selccion, "|")
        Dim mens As New Classscrripjava
        If valor_tab_documeto Is Nothing Then
            mens.Showscripman("documento sin datos adjuntos", Me.UpdatePanelseleccion)
            Exit Sub
        End If

        Session.Item("wf_matri_image") = ""
        Session.Item("wf_tagseleccion") = Valor_Tab_Selccion
        If valor_tab_documeto(13) = ".TIF" Or valor_tab_documeto(13) = ".JPG" Or valor_tab_documeto(13) = ".BMP" Then
            Me.ifrm_visor_.Attributes("src") = "../workflow/webformfreeimage.aspx"
            UpdatePanelVisor.Update()
            'updatepanelintercambio.update()
            'me.updatepanelseleccion.update()


        Else
            mens.Showscripman("para el documento " & valor_tab_documeto(3) & " no esta implementado visor", Me.UpdatePanelseleccion)
            Exit Sub
        End If
    End Sub
    'Eliminar archivo wokflow desde JAVA
    Private Sub ButtonEliminarArchivos_Click(sender As Object, e As EventArgs) Handles ButtonEliminarArchivos.Click
        Dim Mens As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString() <> "" Then
                Dim counter = My.Computer.FileSystem.GetFiles(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString() & "\")
                If counter.Count > 1 Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER").ToString() & "\*.*")
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdateDatos)
        End Try
    End Sub

    'Actualiza los documentos alamcenados relacionados con la actividad
    Private Sub ImagebutonActualizarA_Click(sender As Object, e As ImageClickEventArgs) Handles ImagebutonActualizarA.Click
        Dim Result As String = ""
        Dim Refclas As New ClassWorkflowDigitalizacion
        Dim Refscritpjava As New Classscrripjava

        Try
            HttpContext.Current.Session.Item("DG_SELECION_TREE") = ""
            Dim id_tarea As Long = Me.Hidden_id_tarea_sel.Value
        Catch ex As Exception
            Refscritpjava.Showscripman(ex.Message, Me.UpdateDatos)
        End Try
    End Sub
    Private Sub ImageButtonVisibleEscaner_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonVisibleEscaner.Click
        Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
    End Sub
    'Actualiza a documento principal el documento seleccionado
    Private Sub ButtonPrincipa_Click(sender As Object, e As ImageClickEventArgs) Handles ButtonPrincipa.Click
        'Dim Refclasjavamensaje As New Classscrripjava
        'Try

        '    Dim Refclasworkflodigitalizacion As New ClassWorkflowDigitalizacion
        '    Dim Result As String = ""
        '    If Me.HiddenPROMP.Value = "1" Then
        '        Exit Sub
        '    End If
        '    If Me.TreeViewseleccion_digitalizado.SelectedNode Is Nothing Then
        '        Refclasjavamensaje.Showscripman("Seleccione el documento de la lista ", Me.UpdatePanelBotones)
        '        Exit Sub
        '    End If
        '    Dim Splipositvol() As String = Me.TreeViewseleccion_digitalizado.SelectedNode.Value.Split("|")
        '    Dim slplit() As String = HttpContext.Current.Session.Item("SELECCIONTEMPORAL").ToString.Split("|")
        '    Dim Gabinete As String = Splipositvol(0)
        '    Dim Radicado As String = Splipositvol(2)
        '    Dim id_tarea As Long = slplit(0)
        '    Result = Refclasworkflodigitalizacion.Actualiza_a_Documento_Principal_treview(Me.UpdateDatos, _
        '                                                                                  Me.TreeViewseleccion_digitalizado, _
        '                                                                                  Gabinete, _
        '                                                                                  Radicado, _
        '                                                                                  HttpContext.Current.Session.Item("Id_Ruta_Workflow"), _
        '                                                                                  id_tarea)
        '    If Result <> "YES" Then
        '        Refclasjavamensaje.Showscripman(Result, Me.UpdatePanelBotones)
        '        Exit Sub
        '    End If
        'Catch ex As Exception
        '    Refclasjavamensaje.Showscripman(ex.Message, Me.UpdatePanelBotones)
        'End Try
    End Sub

    Private Sub ButtonActivarBusqueda_Click(sender As Object, e As EventArgs) Handles ButtonActivarBusqueda.Click
        Me.ModalPopupExtenderbusqueda.Show()

    End Sub

    Private Sub Button_sube_pediente_Click(sender As Object, e As EventArgs) Handles Button_sube_pediente.Click
        Dim scripjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Me.TreeViewseleccion.Nodes.Clear()
                Label_docu_relacionado_wf.Text = "Documentos (" & "0" & ")"
                Me.UpdatePanel_label_seleccion.Update()
                'Tre_v.ChildNodes.Add(attrNodeGru)
                Me.UpdatePanelseleccion.Update()
                Dim Refcriptman As New Classscrripjava
                Dim refcla As New ClassWorflowVisor
                Dim Resutl As String = ""
                Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Me.Label_estado_selecion.Text = ""
                Me.Panel_autoriza.Visible = False
                Me.updatemenu.Update()
                UpdatePanel_general_variable.Update()
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    Refcriptman.Show(Resutl)
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanelpedieteboton)
        End Try
    End Sub

    Private Sub btnCancel_autoterminar_Click(sender As Object, e As EventArgs) Handles btnCancel_autoterminar.Click
        Me.ModalPopupExtendermensaje_autoterminar.Hide()
    End Sub




    Private Sub Button_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_detalle_radicado.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.Hidden_radicado_seleccion.Value = "-1" Then
                clasjava.Showscripman("Debe seleccionar el radicado", UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim id_respuesta_radicado As Integer = -1
            Dim Refclas As New Classgestionrespuesta
            Dim Result As String = ""
            Result = Refclas.Reorna_id_respuesta_radicado(Me.Hidden_radicado_seleccion.Value, id_respuesta_radicado)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, UpdatePanel_boton_tool)
                Exit Sub
            End If
            If id_respuesta_radicado = -1 Then
                clasjava.Showscripman("El tipo de radicado no requiere de una respuesta, no hay detalles para mostrar", UpdatePanel_boton_tool)
                Exit Sub
            End If

            Session.Item("PU_TRAZABILIDAD") = Me.Hidden_radicado_seleccion.Value
            Me.Iframe_visor_externo__.Attributes("SRC") = "../Gestion/WebFormDetalleRadicado.aspx"
            Me.UpdatePanel_visor_externo.Update()
            Me.ModalPopupExtender_visor_externo.Show()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
            Exit Sub
        End Try
    End Sub
    Private Sub Button_autoriza_reasignacion_Click(sender As Object, e As EventArgs) Handles Button_autoriza_reasignacion.Click
        Dim refclas_gestion As New Classgestionrespuesta
        Dim id_usuario_autoriza As Integer = -1
        Dim Mens As New Classscrripjava
        Dim Result As String = ""
        Try
            Result = refclas_gestion.Valida_usuario_administrador_general(Me.TextBox_login_autoriza_reasignacion_tarea.Text, Me.TextBox_pasw_autoriza_reasignacion_tarea.Text, id_usuario_autoriza, "reasigna_documento")
            If Result <> "YES" Then
                Mens.Showscripman(Result, UpdatePanel_autoriza_reasignacion_tarea)
                Exit Sub
            End If

            If Session.Item("OPCIONSELECION") = "ENVIARUSUARIO" Then

                Dim refclas As New ClassWorkflow
                If Me.hdnEmailID.Value = "0" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                Else
                    Session.Item("SESIONITERCAMBIO") = Me.hdnEmailID.Value
                End If

                If Session.Item("SESIONITERCAMBIO") = "" Then
                    Mens.Showscripman("Imposible enviar tarea usuario no seleccionado", Me.UpdatePanel_autoriza_reasignacion_tarea)
                    Exit Sub
                End If
                '-----------------------------------------------
                'Verifica estado solicitudes de aprobación sin
                'desición
                '-----------------------------------------------
                Dim Estado_solicitud_aprobacion As String = ""
                Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
                Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")), Estado_solicitud_aprobacion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_autoriza_reasignacion_tarea)
                    Exit Sub
                End If
                If Estado_solicitud_aprobacion = "YES" Then
                    Mens.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.UpdatePanel_autoriza_reasignacion_tarea)
                    Exit Sub
                End If
                Dim Split() As String = Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                Dim resultado As String = ""
                Dim refclas_gestino_resp As New Classgestionrespuesta
                Dim Resultado_evalua_terminar As String = ""
                Result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                     HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Split(0),
                                                                                     Split(1),
                                                                                     Split(0),
                                                                                     Me.TreeViewseleccion,
                                                                                     resultado,
                                                                                     Me.TextBox_login_autoriza_reasignacion_tarea.Text,
                                                                                     id_usuario_autoriza,
                                                                                     Me.Page,
                                                                                     0,
                                                                                     0,
                                                                                     0,
                                                                                     Resultado_evalua_terminar)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_autoriza_reasignacion_tarea)
                    Exit Sub
                Else
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UpdatePanel_general_variable.Update()
                    Me.hdnEmailID.Value = ""
                    ModalPopupExtender_edition_autoriza_reasignacion_tarea.Hide()
                    Me.ModalPopupExtendermesjpagina.Hide()
                    Me.UpdatePanelintercambio.Update()
                    Me.UpdatePanelseleccion.Update()
                    Dim refcla As New ClassWorflowVisor
                    Dim Resutl As String = ""
                    Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                    If Resutl <> "YES" Then
                        Mens.Showscripman(Resutl, Me.UpdatePanel_autoriza_reasignacion_tarea)
                    End If
                    If resultado <> "" Then
                        Mens.Showscripman(resultado, Me.UpdatePanel_autoriza_reasignacion_tarea)
                    End If
                    If Resultado_evalua_terminar <> "YES" Then
                        Mens.Showscripman(Resultado_evalua_terminar, Me.UpdatePanel_autoriza_reasignacion_tarea)
                    End If
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_autoriza_reasignacion_tarea)
        End Try
    End Sub


    'Actualiza el treeview de seleccion
    Protected Sub Button_actualiza_trevie_seleccion_Click(sender As Object, e As EventArgs) Handles Button_actualiza_trevie_seleccion.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classselecciotarea
            If Session.Item("ID_TAREA_SELECCIONDA") = 0 Then Exit Sub
            Dim Actividad_Seleccion As Integer = 0
            Result = ""
            Dim id_actividad As Integer = 0
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Result = Class_estados_tarea_workflow.SolicitaNumeroActividadesSelecionadasUsuario(id_actividad,
                                                                                               HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                              Actividad_Seleccion)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanelseleccion)
                Exit Sub
            End If
            Result = Refclas.Asigna_tarea(Session.Item("ID_TAREA_SELECCIONDA"),
                                          -1,
                                          id_actividad,
                                          -1,
                                          Me.Page)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanelseleccion)
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanelseleccion)
        End Try
    End Sub
    'Elimina documento adjunto
    Private Sub Button_eliminar_documento_adjunto_Click(sender As Object, e As EventArgs) Handles Button_eliminar_documento_adjunto.Click
        Dim Mens As New Classscrripjava
        Try

            Dim Valor_Tab_Selccion As String = hiden_seleccion_documento_wf.Value
            Dim slplitlist() As String = Me.hiden_seleccion_documento_wf.Value.ToString.Split("|")
            Dim Result As String = ""
            Dim Refclas As New ClassAñadirDocumento
            Result = Refclas.Elimina_Documentos_Adjuntos(Val(slplitlist(1)))
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_tool_tareas_pedientes)
                Exit Sub
            End If
            Dim Refclas_ As New ClassVisualisaDocumento
            Result = Refclas_.Visualiza_documento_workflow_visor(Valor_Tab_Selccion,
                                                                 Me.ifrm_visor_,
                                                                 Me.Panel_indice,
                                                                 Me.UpdatePanelindice,
                                                                 Me.UpdatePanelVisor,
                                                                 0, HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                 0,
                                                                 Me.HiddenHeigth,
                                                                 Me.Page, Matri_Doc_Visual,
                                                                                   Doc_actual)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_tool_tareas_pedientes)
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_tool_tareas_pedientes)
        End Try
    End Sub
    'Elimina documento lista doumentos workflow
    Private Sub Button_eliminar_documento_Click(sender As Object, e As EventArgs) Handles Button_eliminar_documento.Click
        'Dim Mens As New Classscrripjava
        'Dim Result As String = ""
        'Dim RefclasEliminadoc As New ClassEliminarDocListResult
        'Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
        'Dim Class_estados_modulo_radicacion As New Class_estados_modulo_radicacion
        'Try
        '    Hidden_confir_elimina.Value = ""
        '    If Me.Hidden_selccion_documento_eliminar_split_wf.Value = "" Or Me.Hidden_selccion_documento_eliminar_split_wf.Value = "-1" Then
        '        Mens.Showscripman_menu("Seleccione el documento a eliminar", Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    End If
        '    Dim split_documento() As String = Me.Hidden_selccion_documento_eliminar_split_wf.Value.Split("|")
        '    Dim Gabinete As String = split_documento(0)
        '    Dim id_imagen As Integer = Val(split_documento(1))
        '    Dim id_dex As Integer = Val(split_documento(3))
        '    Dim id_tarea As Long = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
        '    Result = Class_estados_modulo_radicacion.Eliminar_documento_relcionado_workflow(Gabinete,
        '                                                                                    id_imagen,
        '                                                                                    id_dex,
        '                                                                                    1,
        '                                                                                    Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW"),
        '                                                                                    id_tarea)
        '    If Result <> "YES" Then
        '        Mens.Showscripman_menu(Result, Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    Else
        '        Hidden_confir_elimina.Value = "YES"
        '        If Hidden_selccion_documento_eliminar_wf.Value = hiden_seleccion_documento_id_wf.Value Then
        '            Dim refcla As New ClassWorflowVisor
        '            Dim Resutl As String = ""
        '            Resutl = refcla.Limpia_Visor_Workflow(Me,
        '                                                  "PRINCIPAL",
        '                                                  0)
        '            If Resutl <> "YES" Then
        '                Mens.Showscripman_menu(Result, Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
        '            End If
        '        End If
        '    End If
        'Catch ex As Exception
        '    Mens.Showscripman(ex.Message, Me.UpdatePanel_tool_tareas_pedientes)
        'End Try
    End Sub
    'Elimina documento enlace
    Private Sub Button_tool_elimina_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_elimina_documento.Click
        'Dim clasjava As New Classscrripjava
        'Try
        '    Dim Result As String = ""
        '    Me.Hidden_result_boton_tool.Value = ""
        '    Dim Refclass As New Class_estados_modulo_radicacion
        '    If Me.HiddenPROMP.Value = "1" Then
        '        Exit Sub
        '    End If
        '    If Me.Hidden_selccion_documento_eliminar_split_rad.Value = "" Then
        '        clasjava.Showscripman_menu("Por favor seleccione el documento", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        '        Exit Sub
        '    End If
        '    Dim estado_actualizacion_imagen_ruta As String = "YES"
        '    Result = Refclass.Eliminar_documento_relacionado_radicado(Me.Hidden_selccion_documento_eliminar_split_rad.Value,
        '                                                              HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE"),
        '                                                              1,
        '                                                              Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW"),
        '                                                              estado_actualizacion_imagen_ruta)
        '    If Result <> "YES" Then
        '        Me.Hidden_result_boton_tool.Value = ""
        '        clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        '    Else
        '        Me.Hidden_result_boton_tool.Value = "YES"
        '        If estado_actualizacion_imagen_ruta <> "YES" Then
        '            clasjava.Showscripman_menu("Se elimino el documento, pero no se pudo actualizar la imagen en la ruta worklfow error : " &
        '                                       estado_actualizacion_imagen_ruta, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        '        End If
        '    End If
        'Catch ex As Exception
        '    clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        'End Try
    End Sub
    Private Sub ImageButton_pendiente_aprobacion_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_pendiente_aprobacion.Click
        Dim refclsjava As New Classscrripjava
        Try
            If HttpContext.Current.Session("LISTA_ESTADO_PENDIENTE_APROBACION") = "0" Then
                refclsjava.Showscripman_menu("El usuario no tiene permiso para interactuar con tareas pendientes por aprobación", Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.hdnEmailID.Value = "0"
            Me.Hidden1.Value = "PENDIENTE"
            Me.UpdatePanelintercambio.Update()
            Session.Item("OPCIONSELECION") = "PENDIENTE"
            Session.Item("SESIONITERCAMBIO") = ""
            Me.Labelpendiente.Text = "Solicitudes de aprobación"
            Me.Iframependiente_.Attributes("SRC") = "../radicador/WebFormListaSolicitudesAprobacion.aspx"
            Me.UpdatePanelpendiente.Update()
            Me.ModalPopupExtenderpendiente.Show()
            Me.Hidden_estado_pendiente_aprobacion.Value = "NO"
            updatemenu.Update()
        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_tool_menu)
        End Try
    End Sub


    Protected Sub ImageButton_adjunt_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_adjunt.Click

        Dim refclas_java As New Classscrripjava
        Try
            Dim Refclas As New ra_dig_tipos_docum_lista_chequeo
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
            End If
            Dim estado_resultado As String = ""
            If Result = "YES" Then
                Result = Refclas.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta(Session.Item("DG_ID_TRAMITE"),
                                                                                                               Session.Item("DG_TIPO_TRAMITE"),
                                                                                                               Me.Page,
                                                                                                              estado_resultado)
            End If
            Hidden_0001.Value = "-1"
            If Result <> "YES" Then
                Hidden_0002.Value = "0"
                Label_estado_lista_chequeo.Text = Result
                UpdateGeneral.Update()
            End If
            If estado_resultado = "YES" Then
                Hidden_0002.Value = "1"
                Me.ModalPopupExtender_edition_lista_chequeo_tramite.Show()
                Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, UpdatePanelBotones)
        Finally
            Me.UpdatePanel_lista_chequeo_estado.Update()
        End Try
    End Sub
    Protected Sub Button_Actualizar_Lista_chequeo_Click(sender As Object, e As EventArgs) Handles Button_Actualizar_Lista_chequeo.Click
        Dim refclas_java As New Classscrripjava
        Try
            Dim Refclas As New ra_dig_tipos_docum_lista_chequeo
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
            End If
            Dim estado_resultado As String = ""
            If Result = "YES" Then
                Result = Refclas.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta(Session.Item("DG_ID_TRAMITE"),
                                                                                                               Session.Item("DG_TIPO_TRAMITE"), Me.Page,
                                                                                                               estado_resultado)
            End If
            Hidden_0001.Value = "-1"
            If Result <> "YES" Then
                Me.data_grid_chequeo.DataSource = Nothing
                Me.data_grid_chequeo.DataBind()
                Hidden_0002.Value = "0"
                Label_estado_lista_chequeo.Text = Result
                UpdateGeneral.Update()
            Else
                Hidden_0002.Value = "1"
            End If
        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, UpdatePanel_lista_chequeo)
        Finally
            Me.UpdatePanel_lista_chequeo_estado.Update()
        End Try
    End Sub

    Private Sub data_grid_chequeo_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_chequeo.RowCreated
        e.Row.Cells(0).Visible = False
    End Sub
    Private Sub data_grid_chequeo_actualiza_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_chequeo_actualiza.RowCreated
        e.Row.Cells(0).Visible = False
    End Sub


    Private Sub Button_buton_actualiza_seleccion_Click(sender As Object, e As EventArgs) Handles Button_buton_actualiza_seleccion.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classselecciotarea
            If Session.Item("ID_TAREA_SELECCIONDA") = 0 Then Exit Sub
            Dim Actividad_Seleccion As Integer = 0
            Result = ""
            Dim id_actividad As Integer = 0
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Result = Class_estados_tarea_workflow.SolicitaNumeroActividadesSelecionadasUsuario(id_actividad,
                                                                                          HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                          Actividad_Seleccion)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanelseleccion)
                Exit Sub
            End If
            Result = Refclas.Asigna_tarea(Session.Item("ID_TAREA_SELECCIONDA"),
                                                     -1,
                                                     id_actividad,
                                                     -1,
                                                     Me.Page)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_seleccion_treview)
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_seleccion_treview)
        End Try
    End Sub
    Protected Sub Button_cancela_confirma_reasigna_Click(sender As Object, e As EventArgs) Handles Button_cancela_confirma_reasigna.Click
        Me.ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Hide()
    End Sub

    Protected Sub Button_autoriza_reasigna_Click(sender As Object, e As EventArgs) Handles Button_autoriza_reasigna.Click

        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim id_usuario_autoriza As Integer = 0
            If Session.Item("ID_TAREA_SELECCIONDA") = "0" Or Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                clasjava.Showscripman_menu("Por favor seleccione una tarea para reasignar la respuesta", Me.UpdatePanel_contenido_reasigna_responsable_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas.Valida_usuario_administrador_general(Me.TextBox_login_autoriza_reasigna.Text, Me.TextBox_pasw_autoriza_reasigna.Text, id_usuario_autoriza, "reasigna_documento")
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_contenido_reasigna_responsable_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim Resultado_correo As String = ""
            Result = Refclas.Reasigna_respuesta_tarea_manual(Session.Item("ID_TAREA_SELECCIONDA"), Resultado_correo, Me.Page, Me.TextBox_login_autoriza_reasigna.Text, id_usuario_autoriza)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_contenido_reasigna_responsable_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_reasigna_responsable_tramite.Hide()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_contenido_reasigna_responsable_tramite, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub


    Protected Sub Button_autoriza_confirma_reasigna_Click(sender As Object, e As EventArgs) Handles Button_autoriza_confirma_reasigna.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim id_usuario_autoriza As Integer = 0
            If Session.Item("ID_TAREA_SELECCIONDA") = "0" Or Session.Item("ID_TAREA_SELECCIONDA") = "-1" Then
                Me.ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Hide()
                clasjava.Showscripman_menu("Por favor seleccione una tarea para reasignar la respuesta", Me.UpdatePanel_contenido_confirma_reasigna_responsable_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Resultado_correo As String = ""
            Result = Refclas.Reasigna_respuesta_tarea_manual(Session.Item("ID_TAREA_SELECCIONDA"),
                                                             Resultado_correo,
                                                             Me.Page,
                                                             Me.TextBox_login_autoriza_reasigna.Text, id_usuario_autoriza)
            If Result <> "YES" Then
                Me.ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Hide()
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_contenido_confirma_reasigna_responsable_tramite, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.ModalPopupExtender_edition_confirma_reasigna_responsable_tramite.Hide()
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_contenido_confirma_reasigna_responsable_tramite, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_valida_Cerrar_respuesta_radicado_Click(sender As Object, e As EventArgs) Handles Button_valida_Cerrar_respuesta_radicado.Click
        Try
            If Session.Item("GA_STRU_ESTADO_NUEVA_SOLICITUD_APROBACION") = "YES" Then
                'Me.Label_tex_envia_documento_pendiente.Text = "El sistema detecto una solicitud de aprobación del documentos de respuesta, desea que el sistema envíe la tarea a pendientes por aprobación ?"
                Me.TextBox_texto_pendiente_aprobacion.Text = "Pendiente por aprobación"
                UpdatePanel_envia_documento_pendiente_apro.Update()
                Me.ModalPopupExtender_edition_envia_documento_pendiente_apro.Show()
            End If
            ModalPopup_respuesta_radicado.Hide()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Button_cancelar_envia_documento_pendiente_apro_Click(sender As Object, e As EventArgs) Handles Button_cancelar_envia_documento_pendiente_apro.Click
        Me.ModalPopupExtender_edition_envia_documento_pendiente_apro.Hide()
    End Sub



    Protected Sub Button_cancelar_confirmacion_Click(sender As Object, e As EventArgs) Handles Button_cancelar_confirmacion.Click
        ModalPopupExtender_edition_envia_actividad_flujo_trabjo.Hide()
    End Sub
    Private Sub Button_detalle_enviar_actividad_flujo_trabajo_Click(sender As Object, e As EventArgs) Handles Button_detalle_enviar_actividad_flujo_trabajo.Click
        Dim clasjava As New Classscrripjava
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            Dim Result As String = ""
            Dim nombre_usuario As String = ""
            Dim cargo_usuario As String = ""
            Dim correo_electronico As String = ""
            Dim grupo As String = ""
            If Me.Hidden_id_usuario_workflow.Value <> "&nbsp;" And Me.Hidden_id_usuario_workflow.Value <> "0" Then
                Result = Refclas.Lista_detalle_usuario_workflow(Me.Hidden_id_usuario_workflow.Value,
                                                                nombre_usuario,
                                                                cargo_usuario,
                                                                correo_electronico,
                                                                grupo)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Label_nombre_usuario.Text = nombre_usuario
                    Me.Label_cargo.Text = cargo_usuario
                    Me.Label_correo.Text = correo_electronico
                    Me.Label_nombre_grupo.Text = grupo
                    Me.UpdatePanel_detalle_actividad_flujo_user.Update()
                    Me.ModalPopupExtender_edition_detalle_actividad_flujo_user.Show()
                    Exit Sub
                End If
            End If
            Dim nombre_actividad As String = ""
            Dim descripcion_actividad As String = ""
            Dim tipo_actividad As String = ""
            Dim datos_usuario_relacion_usuario As String = ""
            Dim matri_usuarios_relacionados() As String = Nothing
            If Me.Hidden_id_actividad_destino.Value <> "0" And Me.Hidden_id_actividad_destino.Value <> "&nbsp;" Then
                Result = Refclas.Lista_detalle_actividad_workflow(Me.Hidden_id_actividad_destino.Value,
                                                                  nombre_actividad,
                                                                  descripcion_actividad,
                                                                  tipo_actividad)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.Label_nombre_actividad.Text = nombre_actividad
                    Me.Label_descripcion.Text = descripcion_actividad
                    Me.Label_tipo_actividad.Text = tipo_actividad
                End If
                Result = Refclas.Lista_usuarios_relacionados_id_actividad(Me.Hidden_id_actividad_destino.Value,
                                                                          matri_usuarios_relacionados)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    If matri_usuarios_relacionados Is Nothing Then
                        Me.UpdatePanel_detalle_actividad_flujo.Update()
                        Me.ModalPopupExtender_edition_detalle_actividad_flujo.Show()
                        Exit Sub
                    Else
                        For i As Integer = 0 To matri_usuarios_relacionados.Length - 1
                            If i = 0 Then
                                datos_usuario_relacion_usuario = matri_usuarios_relacionados(i)
                            Else
                                datos_usuario_relacion_usuario = datos_usuario_relacion_usuario & "-" & matri_usuarios_relacionados(i)
                            End If
                        Next
                        Me.Label_usuario_relacionados.Text = datos_usuario_relacion_usuario
                        Me.UpdatePanel_detalle_actividad_flujo.Update()
                        Me.ModalPopupExtender_edition_detalle_actividad_flujo.Show()
                        Exit Sub
                    End If
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub


    '-----Sub para desencadenar popup enviar actividad a usuario 
    Private Sub ImageButtonEnviarUsuario_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonEnviarUsuario.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim Class_usuario_workflow As New Class_usuario_workflow
            Dim Result As String = ""
            Result = Class_usuario_workflow.Valida_lista_usuarios_workflow_para_envio_tarea(Me.Page)
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub
    Private Sub GridView_envia_usuario_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_envia_usuario.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_tool_busqueda_enviar_usuario_Click(sender As Object, e As EventArgs) Handles Button_tool_busqueda_enviar_usuario.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_usuario_workflow As New Class_usuario_workflow
            Result = Ref_class_usuario_workflow.Solicita_listado_usuarios_workflow_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                        2,
                                                                                        Me.TextBox_buequeda_general_lista_usuarios.Text,
                                                                                        Me.GridView_envia_usuario,
                                                                                        Me.titulo_label_lista_usuario_ruta,
                                                                                        Me.Hidden_sel_actividad,
                                                                                        Me.UpdateGeneral_lista_usuarios_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_tool_restore_busqueda_enviar_usuario_Click(sender As Object, e As EventArgs) Handles Button_tool_restore_busqueda_enviar_usuario.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_usuario_workflow As New Class_usuario_workflow
            Result = Ref_class_usuario_workflow.Solicita_listado_usuarios_workflow_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                        1,
                                                                                        "",
                                                                                        Me.GridView_envia_usuario,
                                                                                        Me.titulo_label_lista_usuario_ruta,
                                                                                        Me.Hidden_sel_actividad,
                                                                                        Me.UpdateGeneral_lista_usuarios_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    'Envia la actividad a usuario especifico
    Private Sub Button_tool_enviar_usuario_Click(sender As Object, e As EventArgs) Handles Button_tool_enviar_usuario.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim refclas As New ClassWorkflow
            Dim ref_classsWorkflowUsuario As New ClassWorkflowUsuario
            Dim estado_envio_correo As Integer = 0
            Dim Result As String = ""
            Dim resul_correo As String = ""
            Me.Hidden_result_boton_tool.Value = ""
            Result = ref_classsWorkflowUsuario.Solicita_estado_envio_correo_usuario_workflow(Me.Hidden_id_usuario_envio.Value,
                                                                                            estado_envio_correo)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim Resultado_evalua_terminar As String = ""
            Result = refclas.After_envio_usuario_workflow(Session.Item("WF_ESTADO_RESPUESTA_TRAMITE_USUARIO"),
                                                          estado_envio_correo,
                                                          Me.Hidden_id_usuario_envio.Value,
                                                          Me.Hidden_id_actividad_envio.Value,
                                                          HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                          Me.Page,
                                                          resul_correo,
                                                          Resultado_evalua_terminar)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            Else
                Dim Resutl_ As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Resutl_ = ref_ClassDaGabinete.Inicializa_documentos_seleccion_workflow(Me.Page)
                If Resutl_ <> "YES" Then
                    clasjava.Showscripman_menu(Resutl_, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                End If
                Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Me.Hidden_result_boton_tool.Value = "YES"
                UpdatePanel_general_variable.Update()
                Me.UpdatePanelintercambio.Update()
                Me.UpdatePanelseleccion.Update()
                Me.ModalPopupExtender_edition_lista_usuarios_ruta.Hide()
                Dim refcla As New ClassWorflowVisor
                Dim Resutl As String = ""
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                End If
                If Resultado_evalua_terminar <> "YES" Then
                    clasjava.Showscripman(Resultado_evalua_terminar, Me.UpdatePanel_boton_tool)
                End If
                If resul_correo <> "" And resul_correo <> "YES" Then
                    clasjava.Showscripman(resul_correo, Me.UpdatePanel_boton_tool)
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_devolver_a_actividades_anterior_Click(sender As Object, e As EventArgs) Handles Button_tool_devolver_a_actividades_anterior.Click
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = 0 Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = -1 Then
                clasjava.Showscripman("Debe seleccionar una tarea para devolver", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim Result As String = ""
            Dim ClassWorkflow As New ClassWorkflow
            Result = ClassWorkflow.Activa_devolver_actividades_anteriores(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                          Me.Page)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_devolver_a_usuario_Click(sender As Object, e As EventArgs) Handles Button_tool_devolver_a_usuario.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim refclas As New ClassWorkflow
            Dim ref_classsWorkflowUsuario As New ClassWorkflowUsuario
            Dim estado_envio_correo As Integer = 0
            Dim Result As String = ""
            Dim resul_correo As String = ""
            Me.Hidden_result_boton_tool.Value = ""
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = 0 Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = -1 Then
                clasjava.Showscripman("Debe seleccionar una tarea para devolver", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim Resultado_evalua_terminar As String = ""
            Dim lista_actividades As Integer = 0
            Result = refclas.Devolver_tarea_workflow_usuario_anterior(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                      HttpContext.Current.Session.Item("Id_actividad_Workflow"),
                                                                      Session.Item("Id_Ruta_Workflow"),
                                                                      Me.Page,
                                                                      Resultado_evalua_terminar, lista_actividades)

            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            Else
                If lista_actividades = 1 Then
                    Exit Sub
                End If
                Dim Resutl_ As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Resutl_ = ref_ClassDaGabinete.Inicializa_documentos_seleccion_workflow(Me.Page)
                If Resutl_ <> "YES" Then
                    clasjava.Showscripman_menu(Resutl_, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                End If
                Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Me.Hidden_result_boton_tool.Value = "YES"
                UpdatePanel_general_variable.Update()
                Me.UpdatePanelintercambio.Update()
                Me.UpdatePanelseleccion.Update()
                Dim refcla As New ClassWorflowVisor
                Dim Resutl As String = ""
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                End If
                If Resultado_evalua_terminar <> "YES" Then
                    clasjava.Showscripman(Resultado_evalua_terminar, Me.UpdatePanel_boton_tool)
                End If
                If resul_correo <> "" And resul_correo <> "YES" Then
                    clasjava.Showscripman(resul_correo, Me.UpdatePanel_boton_tool)
                End If
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub ButtonReasignarTerminar_Click(sender As Object, e As EventArgs) Handles ButtonReasignarTerminar.Click
        Dim Refclasjava As New Classscrripjava
        '***********************************************
        'Ejecuta accion enviar actividad a usuario
        '***********************************************
        Try
            If Me.HiddenPROMP.Value = "1" Then
                Exit Sub
            End If
            Dim Result As String = ""
            If Session.Item("OPCIONSELECION") = "ENVIARUSUARIO" Then
                If HttpContext.Current.Session.Item("REASIGNA_TAREA_WORKFLOW") = 0 Then
                    Me.TextBox_login_autoriza_reasignacion_tarea.Text = ""
                    Me.TextBox_pasw_autoriza_reasignacion_tarea.Text = ""
                    Me.UpdatePanel_autoriza_reasignacion_tarea.Update()
                    ModalPopupExtender_edition_autoriza_reasignacion_tarea.Show()
                    Exit Sub
                End If
                Dim refclas As New ClassWorkflow
                If Me.hdnEmailID.Value = "0" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                Else
                    Session.Item("SESIONITERCAMBIO") = Me.hdnEmailID.Value
                End If

                If Session.Item("SESIONITERCAMBIO") = "" Then
                    Refclasjava.Showscripman_menu("Imposible enviar tarea usuario no seleccionado", Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
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
                    Refclasjava.Showscripman_menu(Result, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                If Estado_solicitud_aprobacion = "YES" Then
                    Refclasjava.Showscripman_menu("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim Split() As String = Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                Dim resultado As String = ""
                Dim refclas_gestino_resp As New Classgestionrespuesta
                Dim Resultado_evalua_terminar As String = ""
                Result = refclas_gestino_resp.Reasigna_respuesta_envia_tarea_usuario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                     HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Split(0),
                                                                                     Split(1),
                                                                                     Split(0),
                                                                                     Me.TreeViewseleccion,
                                                                                     resultado,
                                                                                     "sin autorizacion usuario permitido",
                                                                                     0,
                                                                                     Me.Page,
                                                                                     0,
                                                                                     0,
                                                                                     0,
                                                                                     Resultado_evalua_terminar)
                If Result <> "YES" Then
                    Refclasjava.Showscripman_menu(Result, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UpdatePanel_general_variable.Update()
                    Me.hdnEmailID.Value = ""
                    Me.ModalPopupExtendermesjpagina.Hide()
                    Me.UpdatePanelintercambio.Update()
                    Me.UpdatePanelseleccion.Update()
                    Dim refcla As New ClassWorflowVisor
                    Dim Resutl As String = ""
                    Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                    If Resutl <> "YES" Then
                        Refclasjava.Showscripman_menu(Resutl, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                    End If
                    If resultado <> "YES" Then
                        Refclasjava.Showscripman_menu(resultado, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                    End If
                    If Resultado_evalua_terminar <> "YES" Then
                        Refclasjava.Showscripman_menu(Resultado_evalua_terminar, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
                    End If
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman_menu(ex.Message, Me.Updatecondiciona, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    '----ENVIAR TAREA A ACTIVIDAD
    '----Bunton activa ventana envio actividad 
    Private Sub ImageButtonEnviaActividad_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonEnviaActividad.Click
        Dim refclsjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            If HttpContext.Current.Session("Cambio_Ruta") = "0" Then
                refclsjava.Showscripman("El usuario no tiene permiso enviar tarea a otra actividad", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Result = Refclas_f.Verifica_existencia_flujo_trabajo_Actividad_avierto_cerrado(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                           HttpContext.Current.Session.Item("Id_Usuario_Workflow"))
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica la existencia de ruta abierta cerrada
            '-----------------------------------------------
            Dim estado_tramite_ruta As Integer = 0
            Dim tramite As String = ""
            Dim Refclas_workflow_rutas As New Class_worflow_rutas
            Result = Refclas_workflow_rutas.Solicita_etado_abierto_cerrado_ruta_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                      Session.Item("Id_Ruta_Workflow"),
                                                                                      estado_tramite_ruta,
                                                                                      tramite)
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_tramite_ruta = 1 Then
                refclsjava.Showscripman_menu("La tarea pertenece al tipo trámite (" & tramite & ") de ruta cerrada. Imposible enviar tarea a grupos", Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Ref_class_listado_actividades As New Class_Listado_Actividades_workflow
            Result = Ref_class_listado_actividades.Solicita_listado_actividades_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                     1,
                                                                                     "",
                                                                                      Me.GridView_envia_actividades,
                                                                                      Me.titulo_label_lista_actividad_ruta,
                                                                                      Me.Hidden_sel_actividad,
                                                                                      Me.UpdateGeneral_lista_actividades_ruta)
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_lista_actividades_ruta.Show()
            End If

        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub

    Private Sub Button_detalle_enviar_actividad_ruta_Click(sender As Object, e As EventArgs) Handles Button_detalle_enviar_actividad_ruta.Click
        Dim clasjava As New Classscrripjava
        Dim Refclas As New Class_flujo_trabajo_workflow
        Try
            Dim Result As String = ""
            Dim nombre_actividad As String = ""
            Dim descripcion_actividad As String = ""
            Dim tipo_actividad As String = ""
            Dim datos_usuario_relacion_usuario As String = ""
            Dim matri_usuarios_relacionados() As String = Nothing
            'If Me.Hidden_id_actividad_ruta.Value <> "0" And Me.Hidden_id_actividad_ruta.Value <> "&nbsp;" Then
            'Result = Refclas.Lista_detalle_actividad_workflow(Me.Hidden_id_actividad_ruta.Value, nombre_actividad, descripcion_actividad, tipo_actividad)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_lista_actividades_ruta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.Label_nombre_actividad.Text = nombre_actividad
                Me.Label_descripcion.Text = descripcion_actividad
                Me.Label_tipo_actividad.Text = tipo_actividad

            End If
            'Result = Refclas.Lista_usuarios_relacionados_id_actividad(Me.Hidden_id_actividad_ruta.Value, matri_usuarios_relacionados)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_lista_actividades_ruta, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If matri_usuarios_relacionados Is Nothing Then
                    Me.UpdatePanel_detalle_actividad_flujo.Update()
                    Me.ModalPopupExtender_edition_detalle_actividad_flujo.Show()
                    Exit Sub
                Else
                    For i As Integer = 0 To matri_usuarios_relacionados.Length - 1
                        If i = 0 Then
                            datos_usuario_relacion_usuario = matri_usuarios_relacionados(i)
                        Else
                            datos_usuario_relacion_usuario = datos_usuario_relacion_usuario & "-" & matri_usuarios_relacionados(i)
                        End If
                    Next
                    Me.Label_usuario_relacionados.Text = datos_usuario_relacion_usuario
                    Me.UpdatePanel_detalle_actividad_flujo.Update()
                    Me.ModalPopupExtender_edition_detalle_actividad_flujo.Show()
                    Exit Sub
                End If
            End If
            'End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_lista_actividades_ruta, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    '----Bunton activa envio de actividad a ruta
    Private Sub Button_tool_enviar_actividad_Click(sender As Object, e As EventArgs) Handles Button_tool_enviar_actividad.Click
        Dim Refclasjava As New Classscrripjava
        Try
            Dim ref_ClassWorkflow As New ClassWorkflow
            Dim ref_class_listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            Dim Result As String = ""
            Dim resul_correo As String = ""
            Dim nombre_actividad As String = ""
            Dim Estado_solicitud_aprobacion As String = ""
            Me.Hidden_result_boton_tool.Value = ""
            Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                                         Estado_solicitud_aprobacion,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Refclasjava.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Result = ref_class_listado_Actividades_workflow.Retorna_Nombre_Actividad_id_actividad(Val(Hidden_id_tarea.Value),
                                                                                                  nombre_actividad)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim estado_envio_correo As Integer = 0
            Result = ref_class_listado_Actividades_workflow.Solicita_estado_envio_correo_actividad(Val(Hidden_id_tarea.Value),
                                                                                                   estado_envio_correo)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim Resultado_evalua_terminar As String = ""
            Result = ref_ClassWorkflow.Terminar_Tarea_Workflow("",
                                                              Val(Hidden_id_tarea.Value),
                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                              nombre_actividad,
                                                              Me.Page,
                                                              Resultado_evalua_terminar,
                                                              0,
                                                              resul_correo,
                                                              0,
                                                              0,
                                                              0,
                                                              estado_envio_correo)
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            Else
                Dim Resutl_ As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Resutl_ = ref_ClassDaGabinete.Inicializa_documentos_seleccion_workflow(Me.Page)
                If Resutl_ <> "YES" Then
                    Refclasjava.Showscripman_menu(Resutl_, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                End If
                Me.ModalPopupExtender_edition_lista_actividades_ruta.Hide()
                Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Me.Hidden_result_boton_tool.Value = "YES"
                UpdatePanel_general_variable.Update()
                Me.UpdatePanelintercambio.Update()
                Me.UpdatePanelseleccion.Update()
                Dim refcla As New ClassWorflowVisor
                Dim Resutl As String = ""
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.UpdatePanel_boton_tool)
                End If
                If Resultado_evalua_terminar <> "YES" Then
                    Refclasjava.Showscripman(Resultado_evalua_terminar, Me.UpdatePanel_boton_tool)
                End If
                If resul_correo <> "" Then
                    Refclasjava.Showscripman(resul_correo, Me.UpdatePanel_boton_tool)
                End If
            End If
        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_tool)
        End Try
    End Sub

    Private Sub Button_tool_busqueda_enviar_actividad_Click(sender As Object, e As EventArgs) Handles Button_tool_busqueda_enviar_actividad.Click

        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Ref_class_listado_actividades As New Class_Listado_Actividades_workflow
            Result = Ref_class_listado_actividades.Solicita_listado_actividades_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                     2,
                                                                                     TextBox_buequeda_general_lista_actividades.Text,
                                                                                      Me.GridView_envia_actividades,
                                                                                      Me.titulo_label_lista_actividad_ruta,
                                                                                      Me.Hidden_sel_actividad,
                                                                                      Me.UpdateGeneral_lista_actividades_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try

    End Sub

    Private Sub Button_tool_restore_busqueda_enviar_actividad_Click(sender As Object, e As EventArgs) Handles Button_tool_restore_busqueda_enviar_actividad.Click
        Dim clasjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Ref_class_listado_actividades As New Class_Listado_Actividades_workflow
            Result = Ref_class_listado_actividades.Solicita_listado_actividades_ruta(Val(HttpContext.Current.Session.Item("Id_Ruta_Workflow")),
                                                                                     1,
                                                                                     "",
                                                                                      Me.GridView_envia_actividades,
                                                                                      Me.titulo_label_lista_actividad_ruta,
                                                                                      Me.Hidden_sel_actividad,
                                                                                      Me.UpdateGeneral_lista_actividades_ruta)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    'Envia la actividad por medio de ruta de trabajo
    Private Sub Button_activa_enviar_actividad_ruta_Click(sender As Object, e As EventArgs) Handles Button_activa_enviar_actividad_ruta.Click
        Dim refclas As New ClassWorkflow
        Dim mens As New Classscrripjava
        Try
            Dim result As String = ""
            Dim nombre_actividad As String = ""
            Dim Ref_Class_Listado_Actvidades_workflow As New Class_Listado_Actividades_workflow
            Dim refcla As New ClassWorflowVisor
            Dim Resutl As String = ""
            Me.Hidden_result_actividad_ruta.Value = ""
            result = Ref_Class_Listado_Actvidades_workflow.Retorna_Nombre_Actividad_id_actividad(Val(Me.Hidden_id_actividad_envio.Value),
                                                                                                 nombre_actividad)
            If result <> "YES" Then
                mens.Showscripman(result, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If result <> "YES" Then
                mens.Showscripman(result,
                                  Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                                         Estado_solicitud_aprobacion,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If result <> "YES" Then
                mens.Showscripman(result, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                mens.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            Dim Refclas_actividades_disp_envio As New Class_actividades_disponibles_envio
            Dim estado_envio_correo As Integer = 0
            Dim resultado_correo As String = ""
            result = Refclas_actividades_disp_envio.Solicita_estado_envio_correo_conector_ruta(Val(Hidden_id_actividad_disp_envio.Value),
                                                                                               estado_envio_correo)
            If result <> "YES" Then
                mens.Showscripman(result, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            End If
            Dim Resultado_evalua_terminar As String = ""
            result = refclas.Terminar_Tarea_Workflow("",
                                                     "",
                                                     HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                     nombre_actividad,
                                                     Me.Page,
                                                     Resultado_evalua_terminar,
                                                     0,
                                                     resultado_correo,
                                                     0,
                                                     0,
                                                     0,
                                                     estado_envio_correo,
                                                     Val(Me.Hidden_id_actividad_disp_envio.Value),
                                                     HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                     HttpContext.Current.Session("Id_actividad_Workflow"))
            If result <> "YES" Then
                mens.Showscripman(result, Me.UpdatePanel_lista_actividades_ruta)
                Exit Sub
            Else
                Dim Resutl_ As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Resutl_ = ref_ClassDaGabinete.Inicializa_documentos_seleccion_workflow(Me.Page)
                If Resutl_ <> "YES" Then
                    mens.Showscripman_menu(Resutl_, Me.UpdatePanel_lista_actividades_ruta, "ModalPopupExtender_mensaje_personalizado")
                End If
                Me.Hidden_result_actividad_ruta.Value = "YES"
                Me.ModalPopupExtender_edition_lista_actividades_worflow_ruta.Hide()
                Me.UpdatePanelseleccion.Update()
                Me.Hidden_id_tarea_selecionada.Value = "0"
                Me.UpdatePanel_general_variable.Update()
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    mens.Showscripman(result, Me.UpdatePanel_lista_actividades_ruta)
                End If
                If Resultado_evalua_terminar <> "YES" Then
                    mens.Showscripman(Resultado_evalua_terminar, Me.UpdatePanel_lista_actividades_ruta)
                End If
                If resultado_correo <> "" Then
                    mens.Showscripman(resultado_correo, Me.UpdatePanel_lista_actividades_ruta)
                End If
            End If
        Catch ex As Exception
            mens.Showscripman(ex.Message, Me.UpdatePanel_lista_actividades_ruta)
        End Try
    End Sub
    'Envia la actividad por medio de flujo de trabajo
    Private Sub Button_activa_enviar_actividad_flujo_trabajo_Click(sender As Object, e As EventArgs) Handles Button_activa_enviar_actividad_flujo_trabajo.Click
        Dim refclas As New Class_flujo_trabajo_workflow
        Dim mens As New Classscrripjava
        Try
            Dim result As String = ""
            Dim resultado_correo As String = ""
            Me.Hidden_resultado_enviar_activdad_flujo.Value = ""
            Dim Resultado_evalua_terminar As String = ""
            result = refclas.Enviar_actividad_por_conector_flujo_de_trabajo(Me.Page,
                                                                            Session.Item("ID_TAREA_SELECCIONDA"),
                                                                            Me.TreeViewseleccion,
                                                                            resultado_correo,
                                                                            HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                            HttpContext.Current.Session("Id_actividad_Workflow"),
                                                                            Val(Hidden_id_actividad_flujo.Value),
                                                                            Resultado_evalua_terminar)
            If result <> "YES" Then
                mens.Showscripman(result, UpdatePanel_enviar_actividad)
                Exit Sub
            Else
                Dim Resutl_ As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Dim refcla As New ClassWorflowVisor
                Resutl_ = ref_ClassDaGabinete.Inicializa_documentos_seleccion_workflow(Me.Page)
                If Resutl_ <> "YES" Then
                    mens.Showscripman_menu(Resutl_, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                End If
                Me.UpdatePanelseleccion.Update()
                Me.Hidden_resultado_enviar_activdad_flujo.Value = "YES"
                Me.Hidden_id_tarea_selecionada.Value = "0"
                UpdatePanel_general_variable.Update()
                Dim Resutl As String = ""
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    mens.Showscripman(Resutl, UpdatePanel_enviar_actividad)
                End If
                If resultado_correo <> "" Then
                    mens.Showscripman(resultado_correo, UpdatePanel_enviar_actividad)
                End If
                If Resultado_evalua_terminar <> "YES" Then
                    mens.Showscripman(Resultado_evalua_terminar, UpdatePanel_enviar_actividad)
                End If
            End If
        Catch ex As Exception
            mens.Showscripman(ex.Message, UpdatePanel_enviar_actividad)
        End Try
    End Sub
    Private Sub Button_activa_enviar_actividad_flujo_trabajo_anterior_Click(sender As Object, e As EventArgs) Handles Button_activa_enviar_actividad_flujo_trabajo_anterior.Click
        Dim refclas As New Class_flujo_trabajo_workflow
        Dim mens As New Classscrripjava
        Try
            Dim result As String = ""
            Dim resultado_correo As String = ""
            Me.Hidden_resultado_enviar_activdad_flujo.Value = ""
            Dim Resultado_evalua_terminar As String = ""
            result = refclas.Enviar_actividad_por_conector_flujo_de_trabajo_anterior(Me.Page,
                                                                                     Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Me.TreeViewseleccion,
                                                                                     resultado_correo,
                                                                                     HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                     HttpContext.Current.Session("Id_actividad_Workflow"),
                                                                                     Val(Hidden_id_actividad_flujo.Value),
                                                                                     Resultado_evalua_terminar)
            If result <> "YES" Then
                mens.Showscripman(result, UpdatePanel_enviar_actividad)
                Exit Sub
            Else
                Dim Resutl_ As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Dim refcla As New ClassWorflowVisor
                Resutl_ = ref_ClassDaGabinete.Inicializa_documentos_seleccion_workflow(Me.Page)
                If Resutl_ <> "YES" Then
                    mens.Showscripman_menu(Resutl_, Me.UpdatePanel_enviar_actividad, "ModalPopupExtender_mensaje_personalizado")
                End If
                Me.UpdatePanelseleccion.Update()
                Me.Hidden_resultado_enviar_activdad_flujo.Value = "YES"
                Me.Hidden_id_tarea_selecionada.Value = "0"
                UpdatePanel_general_variable.Update()
                Dim Resutl As String = ""
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    mens.Showscripman(Resutl, UpdatePanel_enviar_actividad)
                End If
                If resultado_correo <> "" Then
                    mens.Showscripman(resultado_correo, UpdatePanel_enviar_actividad)
                End If
                If Resultado_evalua_terminar <> "YES" Then
                    mens.Showscripman(Resultado_evalua_terminar, UpdatePanel_enviar_actividad)
                End If
            End If
        Catch ex As Exception
            mens.Showscripman(ex.Message, UpdatePanel_enviar_actividad)
        End Try
    End Sub
    Protected Sub Button_autoriza_reasignacion_flujo_Click(sender As Object, e As EventArgs) Handles Button_autoriza_reasignacion_flujo.Click
        '--------------------------------------------------------
        'Autoriza reasignación mediante flujo de trabajo
        '--------------------------------------------------------
        Dim refclas_gestion As New Classgestionrespuesta
        Dim id_usuario_autoriza As Integer = -1
        Dim Refcriptman As New Classscrripjava
        Dim Result As String = ""
        Try
            Result = refclas_gestion.Valida_usuario_administrador_general(Me.TextBox_login_lista_actividades_ruta_flujo.Text,
                                                                          Me.TextBox_pasw_lista_actividades_ruta_flujo.Text,
                                                                          id_usuario_autoriza, "reasigna_documento")
            If Result <> "YES" Then
                Refcriptman.Showscripman(Result, Me.UpdatePanel_lista_actividades_ruta_flujo)
                Exit Sub
            End If
            Dim Refclas_wf As New ClassWorkflow
            Hidden_resp_envio_flujo.Value = ""
            Dim Resultado_evalua_terminar As String = ""
            Result = Refclas_wf.Reasigna_actividad_con_autorizacion_flujo_trabajo(Me.Page,
                                                                                  Me.TextBox_login_lista_actividades_ruta_flujo.Text,
                                                                                  0,
                                                                                  Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                  Me.TreeViewseleccion,
                                                                                  Resultado_evalua_terminar)
            If Result <> "YES" Then
                'Hidden_resp_envio_flujo.Value = ""
                Refcriptman.Showscripman(Result, Me.UpdatePanel_lista_actividades_ruta_flujo)
                Exit Sub
            Else
                Me.UpdatePanelseleccion.Update()
                Me.Hidden_id_tarea_selecionada.Value = "0"
                UpdatePanel_general_variable.Update()
                Dim refcla As New ClassWorflowVisor
                Dim Resutl As String = ""
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    Refcriptman.Showscripman(Resutl, Me.UpdatePanel_lista_actividades_ruta_flujo)
                End If
                If Resultado_evalua_terminar <> "YES" Then
                    Refcriptman.Showscripman(Resultado_evalua_terminar, Me.UpdatePanel_lista_actividades_ruta_flujo)
                End If
                ModalPopupExtender_edition_lista_actividades_ruta_flujo.Hide()
            End If
        Catch ex As Exception
            Refcriptman.Showscripman_menu(ex.Message, Me.UpdatePanel_lista_actividades_ruta_flujo, "ModalPopupExtender_mensaje_personalizado")

        End Try

    End Sub
    'Auto envia documento a usuario
    Private Sub btnOkay_autoterminar_Click(sender As Object, e As EventArgs) Handles btnOkay_autoterminar.Click

        Dim refclas As New ClassWorkflow
        Dim mens As New Classscrripjava
        Try
            Dim result As String = ""
            Me.Hidden_result_auto_termnar.Value = ""
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If result <> "YES" Then
                mens.Showscripman(result, Me.updatepanel_mensaje_extender)
                Exit Sub
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'desición
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                                         Estado_solicitud_aprobacion,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If result <> "YES" Then
                mens.Showscripman(result, Me.updatepanel_mensaje_extender)
                Exit Sub
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                mens.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.updatepanel_mensaje_extender)
                Exit Sub
            End If
            Dim resultadocorreo As String = "YES"
            Dim Resultado_evalua_terminar As String = ""
            result = refclas.Terminar_Tarea_Workflow(Me.Hidden_id_usuario.Value.ToString,
                                                     Me.Hidden_id_actividad.Value.ToString, 0,
                                                     HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                     Me.Page,
                                                     Resultado_evalua_terminar,
                                                     1,
                                                     resultadocorreo)
            If result <> "YES" Then
                mens.Showscripman(result, updatepanel_mensaje_extender_autoterminar)
                Exit Sub
            Else
                Dim Resutl_ As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Resutl_ = ref_ClassDaGabinete.Inicializa_documentos_seleccion_workflow(Me.Page)
                If Resutl_ <> "YES" Then
                    mens.Showscripman_menu(Resutl_, Me.updatepanel_mensaje_extender_autoterminar, "ModalPopupExtender_mensaje_personalizado")
                End If
                Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Me.Hidden_result_auto_termnar.Value = "YES"
                UpdatePanel_general_variable.Update()
                Me.ModalPopupExtendermensaje_autoterminar.Hide()
                Me.Hidden_id_actividad.Value = "0"
                Me.Hidden_id_usuario.Value = "0"
                Me.updatepanel_mensaje_extender_autoterminar.Update()
                Me.UpdatePanelseleccion.Update()
                Dim refcla As New ClassWorflowVisor
                Dim Resutl As String = ""
                Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                If Resutl <> "YES" Then
                    mens.Showscripman(Resutl, updatepanel_mensaje_extender_autoterminar)
                End If
                If Resultado_evalua_terminar <> "YES" Then
                    mens.Showscripman(Resultado_evalua_terminar, updatepanel_mensaje_extender_autoterminar)
                End If
                If resultadocorreo <> "YES" Then
                    mens.Showscripman(resultadocorreo, updatepanel_mensaje_extender_autoterminar)
                End If
            End If
        Catch ex As Exception
            mens.Showscripman(ex.Message, updatepanel_mensaje_extender)
        End Try
    End Sub
    Private Sub ImageButtonestadograficotrazabilida_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonestadograficotrazabilida.Click
        Dim showmensaje As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Exit Sub
            End If
            Dim Refclas_w As New ClassWorkflow
            Dim Refclas_f As New Class_flujo_trabajo_workflow
            Dim Result As String = ""
            Dim Radicado As String = ""
            Dim Id_flujo_trabajo As Integer = 0
            Dim id_actividad_flujo_trabajo As Integer = 0
            Dim id_usuario_workflow_actividad_flujo_trabajo As Integer = 0
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    Radicado)
            If Result <> "YES" Then
                showmensaje.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Refclas_f.Solicita_id_actividad_flujo_trabajo_id_flujo_trabajo_id_usuario_wf_flujo_trabajo(Radicado,
                                                                                                                id_actividad_flujo_trabajo,
                                                                                                                Id_flujo_trabajo,
                                                                                                                id_usuario_workflow_actividad_flujo_trabajo,
                                                                                                                HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                                                                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                showmensaje.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Id_flujo_trabajo <> 0 Then
                Session.Item("DR_ID_FLUJO_TRABAJO") = Id_flujo_trabajo
                Session.Item("DR_ID_ACTIVIDAD_FLUJO_TRABAJO") = id_actividad_flujo_trabajo
                Session.Item("DR_ID_USUARIO_WORKFLOW_FLUJO_TRABAJO") = id_usuario_workflow_actividad_flujo_trabajo
                Session.Item("DR_ID_TAREA_FLUJO_TRABAJO") = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Session.Item("DR_RADICADO_FLUJO_TRABAJO") = Radicado
                Me.Iframelibre_.Attributes.Add("SRC", "../workflow/WebFormDiagramaEstadoFlujoTrabajo.aspx")
                Me.UpdatePanelLibre.Update()
                Me.ModalPopupExtenderLibre.Show()
            Else
                Session.Item("RU_ID_TAREA_RUTA_TRABAJO") = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Session.Item("RU_RADICADO_RUTA_TRABAJO") = Radicado
                Me.Iframelibre_.Attributes.Add("SRC", "../workflow/WebFormDiagramadorEstadoRutaWorkflow.aspx")
                Me.UpdatePanelLibre.Update()
                Me.ModalPopupExtenderLibre.Show()
            End If
        Catch ex As Exception
            showmensaje.Showscripman_menu(ex.Message, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub ImagenAdjuntaAutomatico_Click(sender As Object, e As ImageClickEventArgs) Handles ImagenAdjuntaAutomatico.Click
        Me.ModalPopupExtender_adjunta_autamatico_documento.Show()
    End Sub



    Private Sub Button_Actualizar_seleccion_indice_wf_Click(sender As Object, e As EventArgs) Handles Button_Actualizar_seleccion_indice_wf.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_seleccion As New Classselecciotarea
            Dim Result As String = ""
            If Me.TreeViewseleccion.SelectedNode Is Nothing Then Exit Sub
            Result = Refclas_seleccion.Actualiza_seleccion_workflow_indice(Session.Item("WF_INTER_SELECION_DOCUMENTO"),
                                                                           Me.TreeViewseleccion.SelectedNode,
                                                                           Me.hidden_selecion_actualiza_treview,
                                                                           Me.UpdatePanelseleccion)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_seleccion_treview)
            End If

        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_seleccion_treview)
        End Try
    End Sub


    Private Sub Button_export_lista_event_Click(sender As Object, e As EventArgs) Handles Button_export_lista_event.Click
        Dim Result As String = ""
        Dim Refclasreposte As New ClassReportesRadicado
        Dim scripjava As New Classscrripjava
        Try
            Dim dat As Date
            dat = Now
            If Me.Hidden_colum_header.Value = "" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman("No hay resultados para exportar", Me.updatapanel_iframe)
                Exit Sub
            End If
            Dim value As Integer = CInt(Int((100 * Rnd()) + 1))
            Dim ruta_create As String = Server.MapPath(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/")
            If Directory.Exists(ruta_create) = False Then
                Directory.CreateDirectory(ruta_create)
            End If
            Dim Ref As New ClassReportesRadicado
            Dim Ref_greview As GridView = GridView2
            Dim nombre_reporte As String = "LISTA TAREAS PENDIENTES"
            Dim ruta_archivo As String = ruta_create + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + value.ToString + "test.xls"
            Result = Refclasreposte.genera_xls_paginacion(Ref_greview, ".xls",
                                                          ruta_archivo,
                                                          Hidden_colum_header.Value,
                                                          nombre_reporte,
                                                          Session.Item("GA_LOGINUSUARIOGESTION"),
                                                          HttpContext.Current.Session.Item("WF_DATA_LISTA_CACHE_WF"))
            If Result <> "YES" Then
                Hidden_ruta_archivo.Value = ""
                scripjava.Showscripman(Result, Me.updatapanel_iframe)
                Exit Sub
            Else
                If File.Exists(ruta_archivo) = True Then
                    Hidden_ruta_archivo.Value = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_WF") + "/reportes/" + HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString + "/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & value.ToString + "test.xls"
                    Me.ifmExcel_reporte_.Attributes.Add("src", "../radicador/WebFormDescargaRadicado.aspx")
                    updatapanel_iframe.Update()
                End If
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.updatapanel_iframe)
        End Try
    End Sub

    Private Sub ImageButton_autorizar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_autorizar.Click
        Dim scripjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                scripjava.Showscripman("Para autorizar debe haber una tarea seleccionada", Me.UpdatePanel_tool_menu)
            Else
                Me.ModalPopupExtender_edition_confirma_autoriza_tarea.Show()
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub

    Protected Sub Button_autoriza_tarea_Click(sender As Object, e As EventArgs) Handles Button_autoriza_tarea.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Autoriza_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                  HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                  HttpContext.Current.Session.Item("Id_Usuario_Workflow"),
                                                                  HttpContext.Current.Session.Item("Id_actividad_Workflow"))
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_autoriza_tarea)
            Else
                Me.updatemenu.Update()
                Me.ModalPopupExtender_edition_confirma_autoriza_tarea.Hide()
            End If

        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_autoriza_tarea)
        End Try
    End Sub
    Private Sub ImageButton_desautoriza_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_desautoriza.Click
        Dim scripjava As New Classscrripjava
        Try
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                scripjava.Showscripman("Para desautorizar debe haber una tarea seleccionada", Me.UpdatePanel_tool_menu)
            Else
                Me.ModalPopupExtender_edition_anula_autoriza_tarea.Show()
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub
    Private Sub Button_anula_autorizacion_tarea_Click(sender As Object, e As EventArgs) Handles Button_anula_autorizacion_tarea.Click
        Dim scripjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Anular_autorizacion_tarea(HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                             HttpContext.Current.Session.Item("Id_actividad_Workflow"),
                                                                             HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                scripjava.Showscripman(Result, Me.UpdatePanel_desautoriza)
            Else

                Me.Panel_autoriza.Visible = True
                Me.updatemenu.Update()
                Me.ModalPopupExtender_edition_anula_autoriza_tarea.Hide()
            End If
        Catch ex As Exception
            scripjava.Showscripman(ex.Message, Me.UpdatePanel_desautoriza)
        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_listado_solicitudes.RowCreated
        Try
            e.Row.Cells(1).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub data_grid_listado_solicitudes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles data_grid_listado_solicitudes.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            data_grid_listado_solicitudes.PageIndex = e.NewPageIndex

            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim Result As String = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_LISTA_AUTORIZA"),
                                                                                            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"),
                                                                                            HttpContext.Current.Session.Item("SortExpression_lista_autoriza"),
                                                                                            HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"),
                                                                                            Me.Label_title_listado_autorizaciones,
                                                                                            Me.data_grid_listado_solicitudes,
                                                                                            Me.UpdatePanel_contenido_lista_autorizacion)

            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_contenido_lista_autorizacion)

            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_contenido_lista_autorizacion)
        End Try

    End Sub
    Private Sub data_grid_listado_solicitudes_Sorting(sender As Object, e As GridViewSortEventArgs) Handles data_grid_listado_solicitudes.Sorting
        Dim clasjava As New Classscrripjava
        Try

            Dim Result As String = ""
            Session.Item("SortExpression_lista_autoriza") = e.SortExpression
            If Session.Item("SortDirection_produccion_lista_autoriza") = "DESC" Then
                Session.Item("SortDirection_produccion_lista_autoriza") = "ASC"
            Else
                Session.Item("SortDirection_produccion_lista_autoriza") = "DESC"
            End If
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                              HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_LISTA_AUTORIZA"),
                                                                              HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"),
                                                                              HttpContext.Current.Session.Item("SortExpression_lista_autoriza"),
                                                                              HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"),
                                                                              Me.Label_title_listado_autorizaciones,
                                                                              Me.data_grid_listado_solicitudes,
                                                                              Me.UpdatePanel_contenido_lista_autorizacion)

            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_contenido_lista_autorizacion)

            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdateGeneral, "ModalPopupExtender_mensaje_personalizado")
        End Try


    End Sub
    Private Sub data_grid_listado_solicitudes_DataBound(sender As Object, e As EventArgs) Handles data_grid_listado_solicitudes.DataBound
        Try
            'Select Case sender.SortDirection
            '    Case SortDirection.Ascending
            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black

            '    Case SortDirection.Descending
            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black

            '        sender.HeaderRow.ForeColor = System.Drawing.Color.Black
            '        sender.FooterRow.ForeColor = System.Drawing.Color.Black
            'End Select
        Catch ex As Exception
        End Try

    End Sub
    Private Sub ImageButton_ista_autorizacio__Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_ista_autorizacio_.Click
        Dim clasjava As New Classscrripjava
        Try
            If HiddenSeleccion.Value = "-1" Or HiddenSeleccion.Value = "" Then
                clasjava.Showscripman("Por favor seleccione el registro para ver la lista de uatorizaciones", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim Result As String = ""
            Session.Item("SortExpression_lista_autoriza") = "Id_autorizacion"
            Session.Item("SortDirection_produccion_lista_autoriza") = "DESC"
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(Val(HiddenSeleccion.Value),
                                                                              1,
                                                                              HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"),
                                                                              HttpContext.Current.Session.Item("SortExpression_lista_autoriza"),
                                                                              HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"),
                                                                              Me.Label_title_listado_autorizaciones,
                                                                              Me.data_grid_listado_solicitudes,
                                                                              Me.UpdatePanel_contenido_lista_autorizacion)

            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_tool_menu)
            Else
                Me.ModalPopupExtender_edition_lista_autorizacion.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub
    Private Sub ImageButton_ista_autorizacio_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_ista_autorizacio.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Session.Item("SortExpression_lista_autoriza") = "Id_autorizacion"
            Session.Item("SortDirection_produccion_lista_autoriza") = "DESC"
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Lista_autorizaciones_tarea(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                              1,
                                                                              HttpContext.Current.Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA"),
                                                                              HttpContext.Current.Session.Item("SortExpression_lista_autoriza"),
                                                                              HttpContext.Current.Session.Item("SortDirection_produccion_lista_autoriza"),
                                                                              Me.Label_title_listado_autorizaciones,
                                                                              Me.data_grid_listado_solicitudes,
                                                                              Me.UpdatePanel_contenido_lista_autorizacion)

            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_tool_menu)
            Else
                Me.ModalPopupExtender_edition_lista_autorizacion.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub

    Private Sub Button_dowload_xml_Click(sender As Object, e As EventArgs) Handles Button_dowload_xml.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Dim Result As String = ""
            If Me.Hidden_selec_list.Value = -1 Then
                clasjava.Showscripman("Debe seleccionar el item ", Me.UpdatePanel_boton_lista)
                Exit Sub
            End If
            Result = Class_autoriza_tarea_worklfow.Descarga_archivo_xml(Val(Me.Hidden_selec_list.Value),
                                                                        Me.ifmExcel_xml_autoriza,
                                                                        Me.Hidden_ruta_archivo,
                                                                        Me.updatapanel_iframe_xml_autoriza)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.UpdatePanel_boton_lista)
                Exit Sub
            Else
                Me.updatapanel_iframe.Update()
            End If

        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.UpdatePanel_boton_lista)
        End Try
    End Sub

    Private Sub GridView_list_documento_relacion_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_list_documento_relacion.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception
        End Try
    End Sub



    Private Sub Button_tool_activa_cambia_tipologia_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_cambia_tipologia.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                             Session.Item("DG_TIPO_TRAMITE"),
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"), 0)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                           Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                           "",
                                                                                                                           Me.DropDownList_tipologia_documental,
                                                                                                                           Me.Update_actualiza_tipologia_documental,
                                                                                                                           estado_resultado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_actualiza_tipologia_documental.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Protected Sub Button_actualiza_tipologia_documental_Click(sender As Object, e As EventArgs) Handles Button_actualiza_tipologia_documental.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_resulta_botno_tipologia_documental.Value = ""
            If Me.Hidden_selccion_documento_cambia_tipo_split_rad.Value = "" Then
                clasjava.Showscripman_menu("Debe seleccionar el documento", Me.UpdatePanel_boton_tipologia_documental, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim split() As String = Me.Hidden_selccion_documento_cambia_tipo_split_rad.Value.ToString.Split("|")
            Dim valor_campo As String = ""
            Dim value_tipologia As Integer = -1
            Dim value_text As String = ""
            If Me.DropDownList_tipologia_documental.Items.Count > 0 Then
                value_tipologia = Val(Me.DropDownList_tipologia_documental.SelectedValue)
                value_text = Me.DropDownList_tipologia_documental.SelectedItem.Text
            End If
            Result = Refclas.Actualiza_tipo_documento_lista_chequeo(split(1),
                                                                    value_tipologia,
                                                                    split(0),
                                                                    value_text,
                                                                    Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                    split(2),
                                                                    valor_campo)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tipologia_documental, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If valor_campo = "" Then
                    valor_campo = "Documento"
                End If
                Me.Hidden_resulta_botno_tipologia_documental.Value = valor_campo
                Me.ModalPopupExtender_edition_actualiza_tipologia_documental.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tipologia_documental, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    'Activa clasificar documento seleción workflow
    Private Sub Button_clasficar_documento_Click(sender As Object, e As EventArgs) Handles Button_clasficar_documento.Click
        Dim refclas_java As New Classscrripjava
        Try
            If Me.Hidden_selccion_documento_cambia_tipo_split_wf.Value = "" Or Me.Hidden_selccion_documento_cambia_tipo_split_wf.Value = "-1" Then
                refclas_java.Showscripman_menu("Seleccione el registro para cambiar el tipo documento", Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim Result As String = ""
            Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE_ADJUNTOWORKFLOW"
            Dim Refclas_digitalizacion As New ClassWorkflowDigitalizacion
            '-----Retorna el tipo de flujo interno o externo
            Dim refclas_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim id_tipo_flujo As Integer = 0
            Dim refclas_dat_adit As New Class_DAT_ADIC_TAR
            Result = refclas_dat_adit.SolicitaIdTipoFlujoTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                            Session.Item("WF_RUTAWORKFLOW"),
                                                            id_tipo_flujo)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If id_tipo_flujo = 1 Then
                Result = Refclas_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowInterna(Session.Item("Id_Ruta_Workflow"),
                                                                                 HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                 HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_TRAMITE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                 HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                 HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                 HttpContext.Current.Session.Item("DG_RADICADO"),
                                                                                 HttpContext.Current.Session("DG_NOMBRE_TRAMITE"))
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            Else
                Result = refclas_workflow_digitalizacion.SolicitaParametrosParaListartiposDocumentalesTareaWorkflowExterna(Session.Item("Id_Ruta_Workflow"),
                                                                                                  HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                  HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                  HttpContext.Current.Session.Item("DG_TIPO_TRAMITE"),
                                                                                                  HttpContext.Current.Session.Item("DG_ID_GABINETE"),
                                                                                                  HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                  HttpContext.Current.Session.Item("DG_RADICADO"),
                                                                                                  HttpContext.Current.Session.Item("DG_NOMBRE_TRAMITE"))
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Dim ref_class_tipo_doc_entrante As New Class_tipo_doc_entrante
                Result = ref_class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(Session.Item("DG_NOMBRE_TRAMITE"),
                                                                                       Session.Item("DG_ID_TRAMITE"))
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                             Session.Item("DG_TIPO_TRAMITE"),
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
            If Result = "YES" Then
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If

            End If

            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                           Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                           "",
                                                                                                                           Me.DropDownList_tipologia_documental_workflow,
                                                                                                                           Me.Update_actualiza_tipologia_documental_workflow,
                                                                                                                           estado_resultado)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_tool_tareas_pedientes, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_actualiza_tipologia_documental_workflow.Show()
            End If
        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, Me.UpdatePanel_tool_tareas_pedientes)
        Finally
            Me.UpdatePanel_lista_chequeo_estado_actualiza.Update()
        End Try
    End Sub
    'Actualiza tipologia documental documemnto seleccionado en workflow
    Private Sub Button_actualiza_tipologia_documental_workflow_Click(sender As Object, e As EventArgs) Handles Button_actualiza_tipologia_documental_workflow.Click
        Dim refclas_java As New Classscrripjava
        Try
            Me.Hidden_resulta_botno_tipologia_documental_workflow.Value = ""
            If Me.Hidden_selccion_documento_cambia_tipo_split_wf.Value = "" Or Me.Hidden_selccion_documento_cambia_tipo_split_wf.Value = "-1" Then
                refclas_java.Showscripman_menu("Debe seleccionar un item de documentos", Me.UpdatePanel_boton_tipologia_documental_workflow, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflowDigitalizacion
            Dim split() As String = Me.Hidden_selccion_documento_cambia_tipo_split_wf.Value.ToString.Split("|")
            Dim valor_campo As String = ""
            Dim value_tipologia As Integer = -1
            Dim value_text As String = ""
            If Me.DropDownList_tipologia_documental_workflow.Items.Count > 0 Then
                value_tipologia = Val(Me.DropDownList_tipologia_documental_workflow.SelectedValue)
                value_text = Me.DropDownList_tipologia_documental_workflow.SelectedItem.Text
            End If
            Result = Refclas.Actualiza_tipo_documento_lista_chequeo(split(1),
                                                                    value_tipologia,
                                                                    split(0),
                                                                    value_text,
                                                                    Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                    split(2),
                                                                    valor_campo)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tipologia_documental_workflow, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If valor_campo = "" Then
                    valor_campo = "Documento"
                End If
                Me.Hidden_resulta_botno_tipologia_documental_workflow.Value = valor_campo
                Me.ModalPopupExtender_edition_actualiza_tipologia_documental_workflow.Hide()
            End If

        Catch ex As Exception
            refclas_java.Showscripman(ex.Message, UpdatePanel_boton_tipologia_documental_workflow)
        End Try
    End Sub
    Private Sub Button_tool_activa_sube_documento_automatico_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento_automatico.Click
        Dim clasjava As New Classscrripjava
        Try
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim Result As String = ""
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                            Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownList_adjunta_documento_automatico.Items.Clear()
            Me.UpdatePane_adjunta_autamatico_documento.Update()
            If HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = "" Then
                HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = structure_gabinete_workflow.NOMBRE_GABINETE
            End If
            If HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0 Then
                HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = structure_datos_tarea_workflow.ID_IMAGEN
            End If
            '----------------------------------------
            'Solicita extensión del archivo
            '----------------------------------------
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                          HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                                          stru_paramter_image)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Class_da_extension As New Class_da_extension
            Dim extension As String = ""
            Result = Class_da_extension.RetornaExtensionTipoDocumento(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                      extension)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If extension = ".TIF" Or extension = ".BMP" Or extension = ".JPG" Then
                Me.Check_anexo_radicado_adj.Visible = True
                Me.Check_anexo_radicado_adj.Enabled = True
                Me.CheckBox_relacionado_radicado_adj.Visible = True
                Me.CheckBox_relacionado_radicado_adj.Enabled = True
                Me.h_adjunto_adjunto_doc_visor.Visible = True
                Me.h_adjunto_adjunto_doc_visor.Attributes.Add("display", "block")
                'Me.Update_actualiza_adjunta_documento.Update()
            Else
                Me.Check_anexo_radicado_adj.Visible = False
                Me.Check_anexo_radicado_adj.Enabled = False
                Me.CheckBox_relacionado_radicado_adj.Visible = True
                Me.CheckBox_relacionado_radicado_adj.Enabled = False
                Me.CheckBox_relacionado_radicado_adj.Checked = True
                Me.h_adjunto_adjunto_doc_visor.Visible = False
                'Me.h_adjunto_adjunto_doc_visor.Attributes.Add("display", "none")
                Me.Update_actualiza_adjunta_documento.Update()
            End If
            'If extension = ".TIF" Or extension = ".BMP" Or extension = ".JPG" Then
            '    Me.Check_anexo_radicado_automatico.Visible = True
            '    Me.Check_anexo_radicado_automatico.Enabled = True
            '    Me.CheckBox_relacionado_radicado_automatico.Visible = True
            '    Me.CheckBox_relacionado_radicado_automatico.Enabled = True
            '    Me.UpdatePanel_chek_adjunta_documento_automatico.Update()
            'Else
            '    Me.Check_anexo_radicado_automatico.Visible = False
            '    Me.Check_anexo_radicado_automatico.Enabled = False
            '    Me.CheckBox_relacionado_radicado_automatico.Visible = True
            '    Me.CheckBox_relacionado_radicado_automatico.Enabled = True
            '    Me.UpdatePanel_chek_adjunta_documento_automatico.Update()
            'End If
            '------------------------------------------
            'Verfica lista existencia lista de chequeo
            '------------------------------------------
            Dim estado_lista As String = ""
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               estado_lista)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            If estado_lista = "YES" Then
                If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                    Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                     Session.Item("DG_TIPO_TRAMITE"),
                                                                                     Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
                End If
                Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
                Dim estado_resultado As String = ""
                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                                Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                                "",
                                                                                                                                Me.DropDownList_adjunta_documento_automatico,
                                                                                                                                Me.UpdatePanel_actualiza_adjunta_documento_automatico,
                                                                                                                                estado_resultado)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Me.ModalPopupExtender_adjunta_autamatico_documento.Show()
                End If
            Else
                Me.ModalPopupExtender_adjunta_autamatico_documento.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    'Activa ventana para subir documentos
    Private Sub Button_tool_activa_sube_documento_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Me.Hidden_tip_adjunt.Value = "wf"
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                            Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")",
                                           Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownList_adjunta_documento.Items.Clear()
            Me.UpdatePane_adjunta_autamatico_documento.Update()
            Dim nombre_gabinete As String = ""
            Dim id_imagen As Integer = 0
            If HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = "" Then
                nombre_gabinete = structure_gabinete_workflow.NOMBRE_GABINETE
            Else
                nombre_gabinete = HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO")
            End If
            If HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0 Then
                id_imagen = structure_datos_tarea_workflow.ID_IMAGEN
            Else
                id_imagen = HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO")
            End If
            Dim Ref_class As New ClassGaProducionDocumental
            Dim Extension_permitida As String = ""
            Result = Ref_class.Solicita_listado_extension_de_archivos_permitidas(Extension_permitida)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Extension_permitida = "" Then
                clasjava.Showscripman_menu("El sistema no registra extensiones permitidas", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            AjaxFileUpload_dowload.AllowedFileTypes = Extension_permitida & ",TIFF"
            '----------------------------------------
            'Solicita extensión del archivo
            '----------------------------------------
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(nombre_gabinete,
                                                                                          id_imagen,
                                                                                          stru_paramter_image)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Class_da_extension As New Class_da_extension
            Dim extension As String = ""
            Result = Class_da_extension.RetornaExtensionTipoDocumento(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                      extension)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If extension = ".TIF" Or extension = ".BMP" Or extension = ".JPG" Then
                Me.Check_anexo_radicado_adj.Visible = True
                Me.Check_anexo_radicado_adj.Enabled = True
                Me.CheckBox_relacionado_radicado_adj.Visible = True
                Me.CheckBox_relacionado_radicado_adj.Enabled = True
                Me.h_adjunto_adjunto_doc_visor.Visible = True
                Me.Update_actualiza_adjunta_documento.Update()
            Else
                Me.Check_anexo_radicado_adj.Visible = False
                Me.Check_anexo_radicado_adj.Enabled = False
                Me.CheckBox_relacionado_radicado_adj.Visible = True
                Me.CheckBox_relacionado_radicado_adj.Enabled = False
                Me.CheckBox_relacionado_radicado_adj.Checked = True
                Me.h_adjunto_adjunto_doc_visor.Visible = False
                Me.Update_actualiza_adjunta_documento.Update()
            End If
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_lista As String = ""
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               estado_lista)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            If estado_lista = "YES" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                 0)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                           Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                           "",
                                                                                                                           Me.DropDownList_adjunta_documento,
                                                                                                                           Me.Update_actualiza_adjunta_documento,
                                                                                                                           estado_resultado)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
                Session.Item("DG_LISTA_CHEQUEO") = 1
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    'Activa la ventana para subir documento a un enlace
    Private Sub Button_tool_activa_sube_documento_enlace_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento_enlace.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim stru_config As Stru_config_digitalizacion = Nothing
            Me.HiddenField_sube_documento_adjunto.Value = 0
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            If HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") = -1 Or HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                       HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                                       structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                              structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownList_adjunta_documento.Items.Clear()
            Me.UpdatePane_adjunta_autamatico_documento.Update()
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = structure_gabinete_workflow.NOMBRE_GABINETE
            Dim Ref_class As New ClassGaProducionDocumental
            Dim Extension_permitida As String = ""
            Result = Ref_class.Solicita_listado_extension_de_archivos_permitidas(Extension_permitida)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Extension_permitida = "" Then
                clasjava.Showscripman_menu("El sistema no registra extensiones permitidas", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            AjaxFileUpload_dowload.AllowedFileTypes = Extension_permitida & ",TIFF"
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                             Session.Item("DG_TIPO_TRAMITE"),
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                             0)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Class_ra_dig_config_digitalizacion.SolicitaDatosConfiguracionDigitalizacionPorTramite(Session.Item("DG_ID_TRAMITE"), stru_config)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.HiddenField_sube_documento_adjunto.Value = stru_config.OBLIGA_LISTA_CHEQUEO
            Me.Check_anexo_radicado_adj.Visible = False
            Me.Check_anexo_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Visible = True
            Me.CheckBox_relacionado_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Checked = True
            Me.h_adjunto_adjunto_doc_visor.Visible = False
            Me.Update_actualiza_adjunta_documento.Update()
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                            Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                            "",
                                                                                                                            Me.DropDownList_adjunta_documento,
                                                                                                                            Me.Update_actualiza_adjunta_documento,
                                                                                                                            estado_resultado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Session.Item("WF_TIPO_ADJUNTA") = "ENLACE"
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub Button_tool_activa_sube_documento_lista_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento_lista.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim stru_config As Stru_config_digitalizacion = Nothing
            Me.HiddenField_sube_documento_adjunto.Value = 0
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                       Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                       structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")",
                                           Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                              structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownList_adjunta_documento.Items.Clear()
            Dim nombre_gabinete As String = ""
            Dim id_imagen As Integer = 0
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_LISTA_RESPUESTA") = structure_gabinete_workflow.NOMBRE_GABINETE
            HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_LISTA_RESPUESTA") = structure_datos_tarea_workflow.ID_IMAGEN
            Dim Ref_class As New ClassGaProducionDocumental
            Me.Check_anexo_radicado_adj.Visible = False
            Me.Check_anexo_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Visible = True
            Me.CheckBox_relacionado_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Checked = True
            Me.h_adjunto_adjunto_doc_visor.Visible = False
            Me.Update_actualiza_adjunta_documento.Update()
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_lista As String = ""
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               estado_lista)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If estado_lista = "YES" Then
                Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                 Session.Item("DG_TIPO_TRAMITE"),
                                                                                 Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                                 0)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                                Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                                "",
                                                                                                                                Me.DropDownList_adjunta_documento,
                                                                                                                                Me.Update_actualiza_adjunta_documento,
                                                                                                                                estado_resultado)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Result = Class_ra_dig_config_digitalizacion.SolicitaDatosConfiguracionDigitalizacionPorTramite(Session.Item("DG_ID_TRAMITE"),
                                                                                                                    stru_config)
                If Result <> "YES" Then
                    clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.HiddenField_sube_documento_adjunto.Value = stru_config.OBLIGA_LISTA_CHEQUEO
                Session.Item("WF_TIPO_ADJUNTA") = "LISTA"
                Session.Item("DG_LISTA_CHEQUEO") = 1
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Session.Item("WF_TIPO_ADJUNTA") = "LISTA"
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    Private Sub AjaxFileUpload_dowload_UploadComplete(sender As Object, e As AjaxControlToolkit.AjaxFileUploadEventArgs) Handles AjaxFileUpload_dowload.UploadComplete
        Try
            Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""

            If Session.Item("WF_TIPO_ADJUNTA") = "ESCANER" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "VISOR" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
                If IO.File.Exists(archivo_donwload) Then
                    Kill(archivo_donwload)
                End If
                Me.AjaxFileUpload_dowload.SaveAs(archivo_donwload)
                Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "ENLACE" Then
                Session.Item("WF_ERROR_RESPUESTA") = ""
                Dim Result As String = ""
                Dim Refclas As New Classgestionrespuesta
                Dim scrijava As New Classscrripjava
                Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
                If Directory.Exists(Server.MapPath(ruta_virtual)) = False Then
                    Directory.CreateDirectory(Server.MapPath(ruta_virtual))
                End If
                Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
                Dim exte = Path.GetTempPath() & "\" & e.FileName
                Dim archivo_donwload As String = Server.MapPath(ruta_virtual) & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_" & e.FileName
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
    Private Sub Button_guardar_desicion_Click(sender As Object, e As EventArgs) Handles Button_guardar_desicion.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAlmacenamiento
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            Me.Hidden_result_load.Value = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            If Session.Item("WF_ERROR_RESPUESTA") <> "" Then
                CLAS.Showscripman(Session.Item("WF_ERROR_RESPUESTA"), Me.UpdatePanel_descarga)
                Exit Sub
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "ESCANER" Then
                Me.Hidden_result_load.Value = ""
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                If Me.DropDownList_adjunta_documento.SelectedValue = "" Then
                    Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = Me.DropDownList_adjunta_documento.SelectedValue
                End If
                Dim ID_DOCUMENTO As Integer = 0
                Dim TIPO_DOCUMENTO As Integer = 0
                Dim datos_image As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Dim ref_ClassAlmacenamiento As New ClassAlmacenamiento
                Dim ob As Object = Nothing
                Result = ref_ClassAlmacenamiento.Almacenamiento_Documentos_Digitalizados("",
                                                                                        ID_DOCUMENTO,
                                                                                        TIPO_DOCUMENTO,
                                                                                        ob,
                                                                                        stru_datos_image_lista,
                                                                                        Session.Item("DG_TIPODIGITALIZACION"),
                                                                                        1)
                If Result <> "YES" Then
                    CLAS.Showscripman_menu(Result, Me.UpdatePanel_descarga, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                End If
                Me.Hidden_result_load.Value = "YES"
                Me.Hidden_date_row.Value = datos_image
                Me.ModalPopupExtender_sube_documento_adjunto.Hide()
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "VISOR" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                If Me.Check_anexo_radicado_adj.Checked = True Then
                    Result = Refclas.Adjunta_documento_parte_documento(Me.Page)
                    If Result <> "YES" Then
                        CLAS.Showscripman(Result, UpdatePanel_descarga)
                        Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        Exit Sub
                    Else
                        Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                    End If
                End If
                If Me.CheckBox_relacionado_radicado_adj.Checked = True Then
                    Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                        radicado)
                    Dim id_imagen As Integer = 0
                    If Val(Me.DropDownList_adjunta_documento.SelectedValue) = -1 Or Val(Me.DropDownList_adjunta_documento.SelectedValue) = 0 Then
                        Result = Refclas.Adjunta_donumento_relacionado(Me.Page,
                                                                       id_imagen,
                                                                       HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                       HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                       Val(Me.DropDownList_adjunta_documento.SelectedValue),
                                                                       Session.Item("ID_TAREA_SELECCIONDA"),
                                                                       radicado,
                                                                       stru_datos_image_lista,
                                                                       0)
                        If Result <> "YES" Then
                            CLAS.Showscripman(Result, UpdatePanel_descarga)
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                            Exit Sub
                        Else
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        End If
                        Me.Hidden_result_load.Value = "YES"
                        If stru_datos_image_lista.notipodocumento = "" Then
                            Dim nun_doc As Object = Val(Hidden_numero_doc_rel_wf.Value) + 1
                            stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                        End If
                        Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                       "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & Session.Item("ID_TAREA_SELECCIONDA")
                    Else
                        Result = Refclas.Adjunta_donumento_relacionado(Me.Page,
                                                                       id_imagen,
                                                                       HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO"),
                                                                       HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"),
                                                                       Val(Me.DropDownList_adjunta_documento.SelectedValue),
                                                                       Session.Item("ID_TAREA_SELECCIONDA"),
                                                                       radicado,
                                                                       stru_datos_image_lista,
                                                                       0)
                        If Result <> "YES" Then
                            CLAS.Showscripman(Result, UpdatePanel_descarga)
                            ModalPopupExtender_sube_documento_adjunto.Hide()
                            Session.Item("DG_LISTA_CHEQUEO") = -1
                            Exit Sub
                        Else
                            Session.Item("DG_LISTA_CHEQUEO") = -1
                            Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                        End If
                        Me.Hidden_result_load.Value = "YES"
                        If stru_datos_image_lista.notipodocumento = "" Then

                            stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                        End If
                        Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                       "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & Session.Item("ID_TAREA_SELECCIONDA")
                    End If
                End If
            End If
            If Session.Item("WF_TIPO_ADJUNTA") = "ENLACE" Then
                If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                    Exit Sub
                End If
                If Me.DropDownList_adjunta_documento.SelectedValue = "" Then
                    Session.Item("DG_LISTA_CHEQUEO") = "-1"
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = Me.DropDownList_adjunta_documento.SelectedValue
                End If
                Dim id_imagen_almacenada As Integer = 0

                Result = Refclas.Almacenamiento_documentos_load_enlace("",
                                                                       1,
                                                                       HttpContext.Current.Session("WF_RUTA_TEMPO_ADJUNTA"),
                                                                       HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                       1,
                                                                       id_imagen_almacenada,
                                                                       stru_datos_image_lista)
                If Result <> "YES" Then
                    CLAS.Showscripman(Result, UpdatePanel_descarga)
                    ModalPopupExtender_sube_documento_adjunto.Hide()
                    Session.Item("DG_LISTA_CHEQUEO") = -1
                    Exit Sub
                Else
                    Me.Hidden_result_load.Value = "YES"
                    If stru_datos_image_lista.notipodocumento = "" Then
                        Dim nun_doc As Object = Val(Hidden_numero_doc_rel_wf.Value) + 1
                        stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                    End If
                    Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                   "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE")
                    Session.Item("DG_LISTA_CHEQUEO") = -1
                    Me.ModalPopupExtender_sube_documento_adjunto.Hide()
                End If
            End If
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_descarga)
        End Try
    End Sub
    'Protected Sub Button_guardar_automatico_Click(sender As Object, e As EventArgs) Handles Button_guardar_automatico.Click
    '    '-----------------------------------------------------------------
    '    'Selcciona Adjuntar documento automatico desde el evento
    '    'Adjuntar
    '    '----------------------------------------------------------------
    '    Dim clasjava As New Classscrripjava
    '    Try
    '        Dim Result As String = ""
    '        Dim estado_lista As String = ""
    '        Dim Refclas As New ClassWorkflow
    '        Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
    '        Dim option_sel As Integer = 1
    '        'Me.HiddenField_estado_guarda.Value = ""
    '        If Me.Check_anexo_radicado_automatico.Checked = False And _
    '            Me.CheckBox_relacionado_radicado_automatico.Checked = False Then
    '            clasjava.Showscripman("Debe seleccionar una opción de la lista", Me.UpdatePane_adjunta_autamatico_documento)
    '            Exit Sub
    '        End If
    '        If Me.Check_anexo_radicado_automatico.Checked = True Then
    '            Result = Refclas.Adjunta_imagen_default(option_sel, Me.Page, 0)
    '            If Result <> "YES" Then
    '                clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
    '                Exit Sub
    '            Else
    '                Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
    '            End If
    '        End If
    '        If Me.CheckBox_relacionado_radicado_automatico.Checked = True Then
    '            '------------------------------------------
    '            'Verfica lista existencia lista de chequeo
    '            '------------------------------------------
    '            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"), estado_lista)
    '            If Result <> "YES" Then
    '                clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
    '                Exit Sub
    '            End If
    '            Dim estado_resultado As String = ""
    '            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
    '            If estado_lista = "YES" Then
    '                If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
    '                    Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"), _
    '                                                                                     Session.Item("DG_TIPO_TRAMITE"), _
    '                                                                                     Session.Item("DG_ID_CONFIG_DIGITALIZACION"))
    '                End If
    '                If Result = "YES" Then
    '                    Result = Refclas_digitalizacion.Solicita_listar_tipos_documentales_relacionados_al_tipo_tramite_lista_adjunta(Session.Item("DG_ID_TRAMITE"), _
    '                                                                                                                                  Session.Item("DG_TIPO_TRAMITE"), _
    '                                                                                                                                  Me.Page, _
    '                                                                                                                                  estado_resultado)
    '                End If

    '                If Result <> "YES" Then
    '                    Session.Item("DG_TIPODIGITALIZACION") = "PREDETERMINADO"
    '                    Me.Hidden_0001.Value = "-1"
    '                    Me.data_grid_chequeo.DataSource = Nothing
    '                    Me.data_grid_chequeo.DataBind()
    '                    Me.Hidden_0002.Value = "0"
    '                    Me.Label_estado_lista_chequeo.Text = Result
    '                    Me.ModalPopupExtender_edition_lista_chequeo_tramite.Show()
    '                    Me.UpdateGeneral.Update()
    '                Else
    '                    If estado_resultado = "YES" Then
    '                        Session.Item("DG_TIPODIGITALIZACION") = "PREDETERMINADO"
    '                        Me.Hidden_0001.Value = "-1"
    '                        Me.ModalPopupExtender_edition_lista_chequeo_tramite.Show()
    '                        Me.Hidden_0002.Value = "1"
    '                        Me.UpdateGeneral.Update()
    '                    Else
    '                        Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
    '                        Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
    '                        Result = Refclas.Adjunta_imagen_default(2, Me.Page, 0)
    '                        If Result <> "YES" Then
    '                            clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
    '                            Exit Sub
    '                        Else
    '                            Me.Hidden_list_cheo_acepta.Value = "YES"
    '                            'Me.HiddenField_estado_guarda.Value = "YES"
    '                            'Me.UpdatePane_seleccion_tipo_adjunto.Update()
    '                            Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()

    '                        End If
    '                    End If

    '                End If
    '            Else
    '                Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
    '                Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
    '                Result = Refclas.Adjunta_imagen_default(2, Me.Page, 0)
    '                If Result <> "YES" Then
    '                    clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
    '                    Exit Sub
    '                Else
    '                    Me.Hidden_list_cheo_acepta.Value = "YES"
    '                    'Me.HiddenField_estado_guarda.Value = "YES"
    '                    'Me.UpdatePane_seleccion_tipo_adjunto.Update()
    '                    Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()

    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        clasjava.Showscripman_menu(ex.Message, Me.UpdatePane_adjunta_autamatico_documento, "ModalPopupExtender_mensaje_personalizado")
    '    End Try
    'End Sub
    Protected Sub Button_guardar_automatico_Click(sender As Object, e As EventArgs) Handles Button_guardar_automatico.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim estado_lista As String = ""
            Dim Ref_classAlmacenamiento As New ClassAlmacenamiento
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim option_sel As Integer = 1
            Me.HiddenField_estado_guarda_automatico.Value = ""
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            If Me.Check_anexo_radicado_automatico.Checked = True Then
                Result = Ref_classAlmacenamiento.Adjunta_imagen_default(option_sel,
                                                                        Me.Page,
                                                                        0,
                                                                        Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue),
                                                                        0,
                                                                        HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"), stru_datos_image_lista)
                If Result <> "YES" Then
                    clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
                End If
            End If
            If Me.CheckBox_relacionado_radicado_automatico.Checked = True Then
                Dim estado_resultado As String = ""
                Dim Refclas_config As New Class_ra_dig_config_digitalizacion
                If Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue) <> -1 And Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue) <> 0 Then
                    Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
                    Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
                    Dim Refclas_workflow As New ClassWorkflow
                    Result = Ref_classAlmacenamiento.Adjunta_imagen_default(2,
                                                                            Me.Page,
                                                                            0,
                                                                            Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue),
                                                                            0,
                                                                            HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"), stru_datos_image_lista)
                    If Result <> "YES" Then
                        clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
                        Exit Sub
                    Else
                        Me.HiddenField_estado_guarda_automatico.Value = "YES"
                        Me.Hidden_tip_adjunt_auto.Value = "wf"
                        If stru_datos_image_lista.notipodocumento = "" Then
                            Dim nun_doc As Object = Val(Hidden_numero_doc_rel_wf.Value) + 1
                            stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                        End If
                        Me.Hidden_date_row_auto.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                   "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|0"
                        Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
                    End If
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = Me.Hidden_0001.Value
                    Session.Item("WF_TIPO_ADJUNTA") = "ESCANER"
                    Result = Ref_classAlmacenamiento.Adjunta_imagen_default(2,
                                                                           Me.Page,
                                                                           0,
                                                                           Val(Me.DropDownList_adjunta_documento_automatico.SelectedValue),
                                                                           0,
                                                                           HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO"), stru_datos_image_lista)
                    If Result <> "YES" Then
                        clasjava.Showscripman(Result, Me.UpdatePane_adjunta_autamatico_documento)
                        Exit Sub
                    Else
                        Me.HiddenField_estado_guarda_automatico.Value = "YES"
                        Me.Hidden_tip_adjunt_auto.Value = "wf"
                        If stru_datos_image_lista.notipodocumento = "" Then
                            Dim nun_doc As Object = Val(Hidden_numero_doc_rel_wf.Value) + 1
                            stru_datos_image_lista.notipodocumento = "D-" & stru_datos_image_lista.id_imagen
                        End If
                        Me.Hidden_date_row_auto.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
                   "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|0"
                        Me.ModalPopupExtender_adjunta_autamatico_documento.Hide()
                    End If
                End If
            End If

        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePane_adjunta_autamatico_documento, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub
    'Activa para subir documento relacionado
    Private Sub Button_tool_adjunta_documento_relacionado_Click(sender As Object, e As EventArgs) Handles Button_tool_adjunta_documento_relacionado.Click
        Dim refclas_java As New Classscrripjava
        Try
            If Session.Item("ID_TAREA_SELECCIONDA") = -1 Or Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_USUARIO") = 0 Then
                refclas_java.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", UpdatePanel_boton_tool)
                Exit Sub
            End If

            Dim Result As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                            Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                refclas_java.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                refclas_java.Showscripman_menu("Imposible econtrar la imagen relacionada a la tarea (" & Session.Item("ID_TAREA_SELECCIONDA") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = structure_gabinete_workflow.NOMBRE_GABINETE
            If HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0 Then
                HttpContext.Current.Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = structure_datos_tarea_workflow.ID_IMAGEN
            End If
            Me.Check_anexo_radicado_adj.Visible = False
            Me.Check_anexo_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Visible = True
            Me.CheckBox_relacionado_radicado_adj.Enabled = False
            Me.CheckBox_relacionado_radicado_adj.Checked = True
            Me.Update_actualiza_adjunta_documento.Update()
            Dim Refclas_digitalizacion As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_lista As String = ""
            Me.Buttonaceptar.Enabled = True
            Session.Item("WF_TIPO_ADJUNTA") = "VISOR"
            AjaxFileUpload_dowload.OnClientUploadComplete = "activa_boton_dowload"
            Dim Ref_class As New ClassGaProducionDocumental
            Dim Extension_permitida As String = ""
            Result = Ref_class.Solicita_listado_extension_de_archivos_permitidas(Extension_permitida)
            If Result <> "YES" Then
                refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Extension_permitida = "" Then
                refclas_java.Showscripman_menu("El sistema no registra extensiones permitidas", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            AjaxFileUpload_dowload.AllowedFileTypes = Extension_permitida & ",TIFF"
            '------------------------------------------
            'Verfica lista existencia lista de chequeo
            '------------------------------------------
            Result = Refclas_digitalizacion.Asigna_datos_lista_chequeo_adjunta(Session.Item("ID_TAREA_SELECCIONDA"),
                                                                               estado_lista)
            If Result <> "YES" Then
                refclas_java.Showscripman(Result, UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            If estado_lista = "YES" Then
                If Session.Item("DG_TIPODIGITALIZACION") = "TRAMITE" Then
                    Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                                     Session.Item("DG_TIPO_TRAMITE"),
                                                                                     Session.Item("DG_ID_CONFIG_DIGITALIZACION"), 0)
                    If Result <> "YES" Then
                        refclas_java.Showscripman(Result, UpdatePanel_boton_tool)
                        Exit Sub
                    End If
                End If
                Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
                Dim estado_resultado As String = ""

                Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                                Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                                "",
                                                                                                                                Me.DropDownList_adjunta_documento,
                                                                                                                                Me.Update_actualiza_adjunta_documento,
                                                                                                                                estado_resultado)
                If Result <> "YES" Then
                    refclas_java.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                    Exit Sub
                Else
                    Session.Item("DG_LISTA_CHEQUEO") = 1
                    Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                    Me.ModalPopupExtender_sube_documento_adjunto.Show()
                End If
            Else
                Session.Item("WF_ERROR_RESPUESTA") = "adjunto"
                AjaxFileUpload_dowload.MaximumNumberOfFiles = 1
                UpdatePanel_descarga.Update()
                Me.ModalPopupExtender_sube_documento_adjunto.Show()
            End If
        Catch ex As Exception
            refclas_java.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    '-------almacena los documentos digitalizados
    Private Sub ButtonAlmacenar_Click(sender As Object, e As EventArgs) Handles ButtonAlmacenar.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim StruDatosIImageLista As stru_datos_image_lista = Nothing
            Dim IdImagenAlmacenada As Integer = 0
            Result = ClassAlmacenamiento.UploadSaveFileScan(Session.Item("DG_TIPODIGITALIZACION"),
                                                            StruDatosIImageLista)
            If Result <> "YES" Then
                Mens.Showscripman(Result, UpdatePanel_boton_tool)
                Exit Sub
            End If
            Me.Hidden_result_load_.Value = "YES"
            Me.Hidden_date_row_.Value = StruDatosIImageLista.nombre_gabinete & "|" & StruDatosIImageLista.id_imagen & "|" & StruDatosIImageLista.radicado &
                    "|" & StruDatosIImageLista.tipodocumental & "|" & StruDatosIImageLista.notipodocumento & "|" & StruDatosIImageLista.id_tarea_workflow &
                    "|" & StruDatosIImageLista.estado_firma_digital & "|" & StruDatosIImageLista.icono_icono_awe_some

        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdateDatos)
        End Try
    End Sub

    '----------Seleccionar tarea
    Private Sub ButtonSeleccionGrupo_Click(sender As Object, e As EventArgs) Handles ButtonSeleccionGrupo.Click
        '************************************************
        'Procedimiento: Seleciona tareas de cada grupo
        'de cada grupo
        '************************************************
        Dim Mens As New Classscrripjava
        Dim Result As String = ""
        Dim Resultado As String = ""
        Try
            Me.Hidden_resultado_selecion.Value = "NO"
            Dim Ref_selecciontarea As New Classselecciotarea
            Dim Ref_Class_grupos_workflow As New Class_grupos_workflow
            Dim id_actividad_usuario_logueado As Integer = 0
            Dim stru_campo_tarea_() As stru_campo_tarea = Nothing
            Result = Ref_Class_grupos_workflow.Solicita_id_actividad_usuario_workflow(id_actividad_usuario_logueado,
                                                                                      HttpContext.Current.Session("Id_Grupo_Workflow"))
            If Result <> "YES" Then
                Mens.Showscripman_menu(Left(Result, 160), Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Result = Ref_selecciontarea.Seleccion_tarea_workflow(Me.GridView2,
                                                                 "Tareas de Grupo=",
                                                                  Resultado,
                                                                  "1",
                                                                  mEval,
                                                                  Page,
                                                                  0,
                                                                  "",
                                                                  0,
                                                                  1,
                                                                  0,
                                                                  id_actividad_usuario_logueado,
                                                                  Hidden_id_tarea_sel.Value,
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  stru_campo_tarea_)
            If Result <> "YES" And Result <> "ENLACE" Then
                Mens.Showscripman_menu(Left(Result, 160), Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                If Result <> "ENLACE" Then
                    Me.Hidden_resultado_selecion.Value = "YES"
                    Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Me.Hidden_id_tarea_sel.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Me.UpdatePanel_general_variable.Update()
                Else
                    'Me.Hidden_resultado_selecion.Value = "NO"
                    Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Me.Hidden_id_tarea_sel.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Me.UpdatePanel_general_variable.Update()
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(Left(ex.Message, 40), Me.UpdatePanel_boton_tool)
        End Try
    End Sub
    '-------Selecciona tarea recuperada
    Private Sub ButtonRecuperar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonRecuperar.Click
        Dim Mens As New Classscrripjava
        Try
            Me.Hidden_resultado_selecion.Value = "NO"
            If Session.Item("OPCIONSELECION") = "RECUPERARTAREA" Then
                Dim refclas As New ClassWorkflow
                Dim Result As String = ""
                If Me.hdnEmailID.Value = "0" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                Else
                    Session.Item("SESIONITERCAMBIO") = Me.hdnEmailID.Value
                End If
                If Session.Item("SESIONITERCAMBIO") = "" Then
                    Mens.Showscripman("Imposible asignar tarea por favor seleccione una tarea", Me.UpdatePanel_boton_tool)
                    Exit Sub
                End If
                'Valores split
                '0-id_tarea
                '1-id_actividad
                Me.Hidden_00021_row.Value = ""
                Dim Split() As String = Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                Dim Resultado As String = ""
                Dim Resultado_correo As String = ""
                Dim Ref_selecciontarea As New Classselecciotarea
                Dim stru_campo_tarea_() As stru_campo_tarea = Nothing
                Result = Ref_selecciontarea.Seleccion_tarea_workflow(Me.GridView2,
                                                                     "Tareas de grupo=",
                                                                      Resultado,
                                                                      "-1",
                                                                      mEval,
                                                                      Page,
                                                                      1,
                                                                      Resultado_correo,
                                                                      1,
                                                                      1,
                                                                      1,
                                                                      Val(Split(1)),
                                                                      Val(Split(0)),
                                                                      2,
                                                                      1,
                                                                      0,
                                                                      0,
                                                                      1,
                                                                      stru_campo_tarea_)
                If Result <> "YES" And Result <> "AUTORIZA RECUPERA" And Result <> "ENLACE" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Me.hdnEmailID.Value = ""
                    Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
                    Me.ModalPopupExtenderRecuperar.Hide()
                    Exit Sub
                Else
                    If Result = "ENLACE" Then
                        Hidden_tip_adjunt.Value = "rad"
                        Me.ModalPopupExtenderRecuperar.Hide()
                        Exit Sub
                    End If
                    If Result = "AUTORIZA RECUPERA" Then
                        Me.ModalPopupExtenderRecuperar.Hide()
                        Exit Sub
                    End If
                    If Result = "YES" Then
                        For i As Integer = 5 To stru_campo_tarea_.Length - 1
                            If i = 5 Then
                                If stru_campo_tarea_(i).valor_campo.ToString <> "" Then
                                    Me.Hidden_00021_row.Value = stru_campo_tarea_(i).valor_campo.ToString.Replace("|", "")
                                Else
                                    Me.Hidden_00021_row.Value = stru_campo_tarea_(i).valor_campo.ToString
                                End If
                            Else
                                If stru_campo_tarea_(i).valor_campo.ToString <> "" Then
                                    Me.Hidden_00021_row.Value = Me.Hidden_00021_row.Value & "|" & stru_campo_tarea_(i).valor_campo.ToString.Replace("|", "")
                                Else
                                    Me.Hidden_00021_row.Value = Me.Hidden_00021_row.Value & "|" & stru_campo_tarea_(i).valor_campo.ToString
                                End If
                            End If
                        Next
                        Session.Item("SESIONITERCAMBIO") = ""
                        Session.Item("OPCIONSELECION") = ""
                        Me.Hidden_resultado_selecion_enlace.Value = "YES"
                        Me.hdnEmailID.Value = "0"
                        Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                        Me.Hidden_id_tarea_sel.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                        Me.UpdatePanel_general_variable.Update()
                        Me.ModalPopupExtenderRecuperar.Hide()
                        Me.UpdatePanelintercambio.Update()
                        Me.UpdatePanelseleccion.Update()
                    End If
                End If
            Else
                Session.Item("SESIONITERCAMBIO") = ""
                Session.Item("OPCIONSELECION") = ""
                Me.hdnEmailID.Value = "0"
                Mens.Showscripman("La opción de selección global no coincide con la de RECUPERARTAREA se recomienda cerrar y iniciar sesión nuevamente", Me.UpdatePanel_boton_tool)
                Me.ModalPopupExtenderRecuperar.Hide()
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman("Excepcion General " & ex.Message, Me.UpdatePanel_boton_tool)
            Me.ModalPopupExtenderRecuperar.Hide()
            Exit Sub
        End Try
    End Sub
    '----Asigna la tarea desde la ventana enlace
    Protected Sub Buttonaceptar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Buttonaceptar.Click
        Dim Mens As New Classscrripjava
        Dim Refclas As New Classselecciotarea
        Dim Resultado As String = ""
        Dim Resultado_correo As String = ""
        Dim Refclas_wf As New ClassWorkflow
        Dim refclas_workflow_digitalizacion As New ClassWorkflowDigitalizacion
        Dim Refclas_seleccion As New Classselecciotarea
        Dim Result As String = ""
        Try
            Me.Hidden_resultado_selecion_enlace.Value = "NO"
            Dim SplitParan() As String
            Erase SplitParan
            '0-id_tarea
            '1-id_actividad
            '2-Index
            '3-TipoActividad
            Me.Hidden_00022_row.Value = ""
            SplitParan = Split(Session.Item("SELECCIONTEMPORAL"), "|")
            If SplitParan Is Nothing Then
                Mens.Showscripman("Los paramentros id_tarea, id actividad index son nulos", Me.UpdateDatos)
                Exit Sub
            End If
            If SplitParan.Length = 0 Then
                Mens.Showscripman("No hay selección temporal contacte a su administrador", Me.UpdateDatos)
                Exit Sub
            End If
            If Session.Item("DG_ID_TRAMITE") <> 0 And Session.Item("DG_NOMBRE_GABINETE") <> "" And Session.Item("DG_RADICADO") <> "" Then
                Result = refclas_workflow_digitalizacion.Verfica_existencia_tipo_documental_obligatorio_digitalizado(
                                                                                                                     Session.Item("DG_RADICADO"),
                                                                                                                     Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                                     Session.Item("DG_ID_TRAMITE"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdateDatos)
                    Exit Sub
                End If
            End If
            '----Selecciona la tarea cuando es recuperada desde la opcion recuperar
            Dim ref_Classselecciotarea As New Classselecciotarea
            Dim obj As Object = Nothing
            Dim stru_campo_tarea_() As stru_campo_tarea = Nothing
            If SplitParan(2) = "-1" Then
                If HttpContext.Current.Session.Item("RECUPERAR_TAREA") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para recuperar la tarea", Me.UpdateDatos)
                    Exit Sub
                End If
                '------------------------------------------------------
                'Valida asignación de la tarea antes de recuperar
                '------------------------------------------------------
                Dim class_estados_tarea As New Class_estados_tarea_workflow
                Dim estado_asignacion As String = ""
                Dim nombre_usuario As String = ""
                Dim cargo_usuario As String = ""
                Dim login_usuario As String = ""
                Result = class_estados_tarea.Solicita_estado_asignacion_tarea_workflow(Val(SplitParan(0)),
                                                                                       estado_asignacion,
                                                                                       nombre_usuario,
                                                                                       cargo_usuario,
                                                                                       login_usuario)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdateDatos)
                    Exit Sub
                End If
                If estado_asignacion = "YES" Then
                    Mens.Showscripman("La tarea esta asignada al usuario (" & nombre_usuario & ") cargo (" & cargo_usuario & ") logueo (" & login_usuario & ") no es posible asignar la tarea", Me.UpdateDatos)
                    Exit Sub
                End If
                Result = ref_Classselecciotarea.Seleccion_tarea_workflow(obj,
                                                                        "Tareas de Grupo=",
                                                                         Resultado,
                                                                         Me.HiddenSeleccion.Value,
                                                                         mEval,
                                                                         Page,
                                                                         1,
                                                                         "",
                                                                         1,
                                                                         0,
                                                                         0,
                                                                         Val(SplitParan(1)),
                                                                         Val(SplitParan(0)),
                                                                         2,
                                                                         1,
                                                                         1,
                                                                         0,
                                                                         1,
                                                                         stru_campo_tarea_)
                If Result <> "YES" And Result <> "AUTORIZA RECUPERA" Then
                    Mens.Showscripman(Result, Me.UpdateDatos)
                    Exit Sub
                End If
                If Result = "AUTORIZA RECUPERA" Then
                    Exit Sub
                End If
                If Result = "YES" Then
                    For i As Integer = 5 To stru_campo_tarea_.Length - 1
                        If i = 5 Then
                            If stru_campo_tarea_(i).valor_campo.ToString <> "" Then
                                Me.Hidden_00022_row.Value = stru_campo_tarea_(i).valor_campo.ToString.Replace("|", "")
                            Else
                                Me.Hidden_00022_row.Value = stru_campo_tarea_(i).valor_campo.ToString
                            End If
                        Else
                            If stru_campo_tarea_(i).valor_campo.ToString <> "" Then
                                Me.Hidden_00022_row.Value = Me.Hidden_00022_row.Value & "|" & stru_campo_tarea_(i).valor_campo.ToString.Replace("|", "")
                            Else
                                Me.Hidden_00022_row.Value = Me.Hidden_00022_row.Value & "|" & stru_campo_tarea_(i).valor_campo.ToString
                            End If
                        End If
                    Next
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Session.Item("SELECCIONTEMPORAL") = ""
                    Me.Hidden_resultado_selecion_enlace.Value = "YES"
                    Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Me.Hidden_id_tarea_sel.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Me.hdnEmailID.Value = "0"
                    Me.UpdatePanel_general_variable.Update()
                    Me.UpdatePanelintercambio.Update()
                    Me.UpdatePanelseleccion.Update()
                    Me.ModalPopupExtender_edition_admon_documentos.Hide()
                    Exit Sub
                End If
            Else
                '----Selecciona la tarea cuando es recuperada con curso normal
                Result = ref_Classselecciotarea.Seleccion_tarea_workflow(obj,
                                                                       "Tareas de Grupo=",
                                                                        Resultado,
                                                                        Me.HiddenSeleccion.Value,
                                                                        mEval,
                                                                        Page,
                                                                        0,
                                                                        "",
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        Val(SplitParan(1)),
                                                                        Val(SplitParan(0)),
                                                                        0,
                                                                        0,
                                                                        1,
                                                                        0, 0, stru_campo_tarea_)
                If Result = "YES" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Session.Item("SELECCIONTEMPORAL") = ""
                    Me.Hidden_resultado_selecion_enlace.Value = "YES"
                    Me.hdnEmailID.Value = "0"
                    Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Me.Hidden_id_tarea_sel.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    Me.UpdatePanel_general_variable.Update()
                    Me.UpdatePanelintercambio.Update()
                    Me.UpdatePanelseleccion.Update()
                    Me.ModalPopupExtender_edition_admon_documentos.Hide()
                    Exit Sub
                Else
                    Mens.Showscripman(Result, Me.UpdateDatos)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdateDatos)
        End Try
    End Sub
    Private Sub Button_actualiza_enlace_Click(sender As Object, e As EventArgs) Handles Button_actualiza_enlace.Click
        Dim Mens As New Classscrripjava
        Dim Refclas As New Classselecciotarea
        Dim Resultado As String = ""
        Dim Resultado_correo As String = ""
        Dim Refclas_wf As New ClassWorkflow
        Dim refclas_workflow_digitalizacion As New ClassWorkflowDigitalizacion
        Dim Result As String = ""
        Try
            Dim SplitParan() As String
            Erase SplitParan
            '0-id_tarea
            '1-id_actividad
            '2-Index
            '3-TipoActividad
            SplitParan = Split(Session.Item("SELECCIONTEMPORAL"), "|")
            If SplitParan Is Nothing Then
                Mens.Showscripman("Los paramentros id_tarea, id actividad index son nulos", Me.UpdateDatos)
                Exit Sub
            End If
            If SplitParan.Length = 0 Then
                Mens.Showscripman("No hay selección temporal contacte a su administrador", Me.UpdateDatos)
                Exit Sub
            End If

            '----Selecciona la tarea cuando es recuperada con curso normal
            Dim ref_Classselecciotarea As New Classselecciotarea
            Dim obj As Object = Nothing
            Dim stru_campo_tarea_() As stru_campo_tarea = Nothing
            Result = ref_Classselecciotarea.Seleccion_tarea_workflow(obj,
                                                                      "Tareas de Grupo=",
                                                                       Resultado,
                                                                       Me.HiddenSeleccion.Value,
                                                                       mEval,
                                                                       Page,
                                                                       0,
                                                                       "",
                                                                       0,
                                                                       0,
                                                                       0,
                                                                       Val(SplitParan(1)),
                                                                       Val(SplitParan(0)),
                                                                       0,
                                                                       0,
                                                                       1,
                                                                       1,
                                                                       0,
                                                                       stru_campo_tarea_)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdateDatos)
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdateDatos)
        End Try
    End Sub
    '------Sub para confirmar y enlazar documentos
    Private Sub Button_aceptar_reasignacion_tarea_recuperada_enlazada_Click(sender As Object, e As EventArgs) Handles Button_aceptar_reasignacion_tarea_recuperada_enlazada.Click
        Dim Mens As New Classscrripjava
        Dim Refclas As New Classselecciotarea
        Dim Result As String = ""
        Try
            Me.Hidden_resp_reasignacion_tarea_recuperada_enlazada.Value = ""
            Dim SplitParan() As String
            Erase SplitParan
            '0-id_tarea
            '1-id_actividad
            '2-Index
            '3-TipoActividad
            SplitParan = Split(Session.Item("SELECCIONTEMPORAL"), "|")
            If SplitParan Is Nothing Then
                Mens.Showscripman("Los paramentros id_tarea, id actividad index son nulos", Me.UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada)
                Exit Sub
            End If
            If SplitParan.Length = 0 Then
                Mens.Showscripman("No hay seleccion temporal contacte a su administrador", Me.UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada)
                Exit Sub
            End If

            Dim refclas_gestion As New Classgestionrespuesta
            Dim id_usuario_autoriza As Integer = 0
            Result = refclas_gestion.Valida_usuario_administrador_general(Me.TextBox_login_autoriza_reasignacion_tarea_recuperada_enlazada.Text,
                                                                          Me.TextBox_pasw_autoriza_reasignacion_tarea_recuperada_enlazada.Text,
                                                                          id_usuario_autoriza,
                                                                          "reasigna_documento")
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada)
                Exit Sub
            End If
            Dim Resultado As String = ""
            Dim Resultado_correo As String = ""
            Dim Refclas_wf As New ClassWorkflow
            Dim ref_Classselecciotarea As New Classselecciotarea
            Dim obj As Object = Nothing
            Dim stru_campo_tarea_() As stru_campo_tarea = Nothing
            Result = ref_Classselecciotarea.Seleccion_tarea_workflow(obj,
                                                                     "Tareas de Grupo=",
                                                                     Resultado,
                                                                     Me.HiddenSeleccion.Value,
                                                                     mEval,
                                                                     Page,
                                                                     1,
                                                                     "",
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     Val(SplitParan(1)),
                                                                     Val(SplitParan(0)),
                                                                     2,
                                                                     0,
                                                                     1,
                                                                     0,
                                                                     0,
                                                                     stru_campo_tarea_)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada)
                Exit Sub
            Else
                Session.Item("SESIONITERCAMBIO") = ""
                Session.Item("OPCIONSELECION") = ""
                Session.Item("SELECCIONTEMPORAL") = ""
                Me.hdnEmailID.Value = "0"
                Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Me.Hidden_id_tarea_sel.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Me.Hidden_resp_reasignacion_tarea_recuperada_enlazada.Value = "YES"
                Me.UpdatePanel_general_variable.Update()
                Me.UpdatePanelintercambio.Update()
                Me.UpdatePanelseleccion.Update()
                Me.ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada_enlazada.Hide()
                Me.ModalPopupExtender_edition_admon_documentos.Hide()
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_autoriza_reasignacion_tarea_recuperada_enlazada)
        End Try
    End Sub
    'boton que asigna y reasigna tarea recuperada con autorización
    Private Sub Button_aceptar_reasignacion_tarea_recuperada_Click(sender As Object, e As EventArgs) Handles Button_aceptar_reasignacion_tarea_recuperada.Click
        Dim Mens As New Classscrripjava
        Try
            If Session.Item("OPCIONSELECION") = "RECUPERARTAREA" Then
                Dim refclas As New ClassWorkflow
                Dim Result As String = ""
                Dim refclas_gestion As New Classgestionrespuesta
                Dim id_usuario_autoriza As Integer = 0
                Result = refclas_gestion.Valida_usuario_administrador_general(Me.TextBox_login_autoriza_reasignacion_tarea_recuperada.Text,
                                                                              Me.TextBox_pasw_autoriza_reasignacion_tarea_recuperada.Text,
                                                                              id_usuario_autoriza,
                                                                              "reasigna_documento")
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_autoriza_reasignacion_tarea_recuperada)
                    Exit Sub
                End If
                'Valores split
                '0-id_tarea
                '1-id_actividad
                Dim Split() As String = Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                Dim Resultado As String = ""
                Dim Resultado_correo As String = ""
                Dim Ref_selecciontarea As New Classselecciotarea
                Dim ob As Object = Nothing
                Dim stru_campo_tarea_() As stru_campo_tarea = Nothing
                Result = Ref_selecciontarea.Seleccion_tarea_workflow(ob,
                                                                     "Tareas de usuario=",
                                                                      Resultado,
                                                                      "-1",
                                                                      mEval,
                                                                      Page,
                                                                      1,
                                                                      Resultado_correo,
                                                                      0,
                                                                      0,
                                                                      0,
                                                                      Val(Split(1)),
                                                                      Val(Split(0)),
                                                                      2,
                                                                      0,
                                                                      0,
                                                                      0,
                                                                      0,
                                                                      stru_campo_tarea_)
                If Result <> "YES" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Me.hdnEmailID.Value = "0"
                    Mens.Showscripman(Result, Me.UpdatePanel_autoriza_reasignacion_tarea_recuperada)
                    Me.ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada.Hide()
                    Exit Sub
                Else
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Me.hdnEmailID.Value = "0"
                    Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UpdatePanel_general_variable.Update()
                    Me.UpdatePanelintercambio.Update()
                    Me.UpdatePanelseleccion.Update()
                    Me.ModalPopupExtender_edition_autoriza_reasignacion_tarea_recuperada.Hide()
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman("Excepcion General " & ex.Message, Me.UpdatePanel_autoriza_reasignacion_tarea_recuperada)
        End Try
    End Sub

    Private Sub GridView2_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView2.PageIndexChanging
        Dim Mens As New Classscrripjava
        Try
            GridView2.PageIndex = e.NewPageIndex
            Dim Ref As New ClassListandoTareas
            'Dim OBE As Object
            Dim Result As String = ""
            'Result = Ref.Inicializar_la_lista_de_tareas_workflow(Me.Page,
            '                                                     OBE,
            '                                                     Me.GridView2,
            '                                                     1,
            '                                                     HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
            '                                                     HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
            '                                                     Session.Item("SortExpression_compartido_WF"),
            '                                                     Session.Item("SortDirection_compartido_WF"), 0,
            '                                                     Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
            'If Result <> "YES" Then
            '    Mens.Showscripman(Result, Me.UpdatePanel1)
            'End If
            Result = Ref.Cahche_pagin_sorting_lista_tareas_workflow(Me.GridView2,
                                                      UpdatePanel1,
                                                      0,
                                                      Session.Item("SortExpression_compartido_WF"),
                                                      Session.Item("SortDirection_compartido_WF"))
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel1)
            End If

        Catch ex As Exception
            Mens.Showscripman(Left(ex.Message, 40), Me.UpdatePanel1)
        End Try
    End Sub

    Private Sub GridView2_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView2.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
            e.Row.Cells(4).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception
        End Try
    End Sub
    '------Poceb seleciona tareas de grupo gredview2
    Private Sub GridView2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView2.SelectedIndexChanged
        ''*******************************************
        ''Procedimiento: Seleciona tareas pendientes
        ''de cada grupo
        ''*******************************************
        'Dim Mens As New Classscrripjava
        'Dim Result As String = ""
        'Dim Resultado As String = ""
        'Try
        '    Dim Ref_selecciontarea As New Classselecciotarea
        '    Result = Ref_selecciontarea.Seleccion_tarea_workflow(Me.GridView2, _
        '                                                         "Tareas de Grupo=", _
        '                                                         Resultado, _
        '                                                         "-1", _
        '                                                         mEval, _
        '                                                         Page, _
        '                                                         0, _
        '                                                         "", _
        '                                                         0, _
        '                                                         1, _
        '                                                         0, _
        '                                                         0, _
        '                                                         Hidden_id_tarea_sel.Value, _
        '                                                         0, _
        '                                                         0, _
        '                                                         0, _
        '                                                         0)
        '    If Result <> "YES" Then
        '        Mens.Showscripman(Left(Result, 160), Me.UpdatePanel1)
        '        Exit Sub
        '    Else
        '        'Me.TextBoxDatos.Text = Resultado
        '        'Me.UpdatePanelnumeroespera.Update()
        '        'Me.UpdateDatos.Update()
        '    End If
        'Catch ex As Exception
        '    Mens.Showscripman(Left(ex.Message, 40), Me.UpdatePanel1)
        'End Try
    End Sub
    Private Sub ImageButtonseleccionar_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonseleccionar.Click
        Dim scriptjava As New Classscrripjava
        Try

            '------------------------------------
            'Selecionar grupo
            '------------------------------------
            Dim Result As String = ""
            Dim Resultado As String = ""
            If HiddenGredview.Value = "Gredview2" Then
                If Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0 Then
                    scriptjava.Showscripman("El usuario no tiene permisos para seleccionar tareas de grupo", Me.UpdatePanel_tool_menu)
                    Exit Sub
                End If
                If Session.Item("Seleccion_Automatico") = 1 Then
                    'Exit Sub
                End If
                If Session.Item("Seleccion_Manual") = 1 Then
                    If Me.Hidden_id_tarea_sel.Value = "-1" Then
                        scriptjava.Showscripman("Por favor seleccione la tarea ", Me.UpdatePanel_tool_menu)
                        Exit Sub
                    End If
                    Me.Hidden_resultado_selecion.Value = "NO"
                    Dim Ref_selecciontarea As New Classselecciotarea
                    Dim stru_campo_tarea_() As stru_campo_tarea = Nothing
                    Result = Ref_selecciontarea.Seleccion_tarea_workflow(Me.GridView2,
                                                                         "Tareas de Grupo=",
                                                                         Resultado,
                                                                         Me.HiddenSeleccion.Value,
                                                                         mEval,
                                                                         Page,
                                                                         0,
                                                                         "",
                                                                         0,
                                                                         1,
                                                                         0,
                                                                         0,
                                                                         Hidden_id_tarea_sel.Value,
                                                                         0,
                                                                         0,
                                                                         0,
                                                                         0,
                                                                         0,
                                                                         stru_campo_tarea_)
                    If Result <> "YES" Then
                        scriptjava.Showscripman(Left(Result, 160), Me.UpdatePanel_tool_menu)
                        Exit Sub
                    Else
                        Me.UpdatePanel_boton_tool.Update()
                        Me.TextBoxDatos.Text = Resultado
                        Me.UpdatePanelnumeroespera.Update()
                        Me.UpdateDatos.Update()
                    End If
                End If
            End If
        Catch ex As Exception
            scriptjava.Showscripman(Left(ex.Message, 160), Me.UpdatePanel_tool_menu)
            Exit Sub
        End Try
    End Sub

    Private Sub ButtonRecuperarReasignar_Click(sender As Object, e As EventArgs) Handles ButtonRecuperarReasignar.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Refclas As New Classselecciotarea
            Dim Result As String = ""
            Result = Refclas.Activa_recupera_asigna_tarea(Me.Page,
                                                          Me.hdnEmailID,
                                                          mEval,
                                                          Me.Hidden_id_tarea_selecionada,
                                                          Me.ModalPopupExtenderRecuperar,
                                                          Me.UpdatePanelintercambio,
                                                          Me.UpdatePanelseleccion,
                                                          Me.UpdatePanel_general_variable)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
        Catch ex As Exception
            Mens.Showscripman("Excepcion General " & ex.Message, Me.UpdatePanel_boton_tool)
        End Try
    End Sub

    Private Sub GridView2_Sorting(sender As Object, e As GridViewSortEventArgs) Handles GridView2.Sorting
        Dim Mens As New Classscrripjava
        Try
            Dim Ref As New ClassListandoTareas
            Dim Result As String = ""
            Session.Item("SortExpression_compartido_WF") = e.SortExpression
            If Session.Item("SortDirection_compartido_WF") = "DESC" Then
                Session.Item("SortDirection_compartido_WF") = "ASC"
            Else
                Session.Item("SortDirection_compartido_WF") = "DESC"
            End If
            Result = Ref.Cahche_pagin_sorting_lista_tareas_workflow(Me.GridView2,
                                                                    UpdatePanel1,
                                                                     1,
                                                                    Session.Item("SortExpression_compartido_WF"),
                                                                    Session.Item("SortDirection_compartido_WF"))
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel1)
            End If

        Catch ex As Exception
            Mens.Showscripman(Left(ex.Message, 40), Me.UpdatePanel1)
        End Try
    End Sub

    Private Sub Button_tool_search_lista_tareas_Click(sender As Object, e As EventArgs) Handles Button_tool_search_lista_tareas.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Ref As New ClassListandoTareas
            Dim Result As String = ""
            Result = Ref.Cahche_Search_lista_tareas_workflow(Me.GridView2,
                                                             UpdatePanel1,
                                                             1,
                                                             Session.Item("SortExpression_compartido_WF"),
                                                             Session.Item("SortDirection_compartido_WF"),
                                                             Me.auto_complex.Text,
                                                             LabelEspera,
                                                             UpdatePanelnumeroespera)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
            End If
        Catch ex As Exception
            Mens.Showscripman(Left(ex.Message, 40), Me.UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_searh_new_task_Click(sender As Object, e As EventArgs) Handles Button_tool_searh_new_task.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Ref As New ClassListandoTareas
            Dim Result As String = ""
            Dim OBE As Object = Nothing
            If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0 Then
                Mens.Showscripman("El usuario no tiene permiso para listar tareas ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF") = 1
            HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF") = ""
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 1 Then
                Result = Ref.Pre_Listar_tareas_workflow_Script(Me.Page,
                                                               OBE,
                                                               Me.GridView2,
                                                               1,
                                                               HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                               HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                               HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                               HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                               HttpContext.Current.Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
                    Exit Sub
                End If
            End If
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 0 Then
                Result = Ref.Pre_Listar_tareas_workflow(Me.Page,
                                                        OBE,
                                                        Me.GridView2,
                                                        1,
                                                        HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                        HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                        HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                        HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                        HttpContext.Current.Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(Left(ex.Message, 40), Me.UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_search_especial_Click(sender As Object, e As EventArgs) Handles Button_tool_search_especial.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Ref As New ClassListandoTareas
            Dim Result As String = ""
            Dim OBE As Object = Nothing
            If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0 Then
                Mens.Showscripman("El usuario no tiene permiso para listar tareas ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If

            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 1 Then
                Result = Ref.Pre_Listar_tareas_workflow_Script(Me.Page,
                                                               OBE,
                                                               Me.GridView2,
                                                               1,
                                                               2,
                                                               Me.Hidden_value_search_especial.Value.ToString,
                                                               HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                               HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                               HttpContext.Current.Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_consulta_avanzada_ruta_workflow.Hide()
                End If
            End If
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 0 Then
                Result = Ref.Pre_Listar_tareas_workflow(Me.Page,
                                                        OBE,
                                                        Me.GridView2,
                                                        1,
                                                        2,
                                                        Me.Hidden_value_search_especial.Value.ToString,
                                                        HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                        HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                        HttpContext.Current.Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
                    Exit Sub
                Else
                    Me.ModalPopupExtender_edition_consulta_avanzada_ruta_workflow.Hide()
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(Left(ex.Message, 40), Me.UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_restore_lista_tareas_Click(sender As Object, e As EventArgs) Handles Button_tool_restore_lista_tareas.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Ref As New ClassListandoTareas
            Dim Result As String = ""
            Result = Ref.Cahche_lista_tareas_workflow(Me.GridView2,
                                                      UpdatePanel1,
                                                      0,
                                                      Session.Item("SortExpression_compartido_WF"),
                                                      Session.Item("SortDirection_compartido_WF"),
                                                      LabelEspera,
                                                      UpdatePanelnumeroespera)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
            End If
        Catch ex As Exception
            Mens.Showscripman(Left(ex.Message, 40), Me.UpdatePanel_boton_tool)
        End Try
    End Sub

    '--------Sub actualizacion de listado tareas
    Private Sub ImageButtonactualizar_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonactualizar.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim OBE As Object = Nothing
            Dim Ref As New ClassListandoTareas
            Me.HiddenSeleccion.Value = "-1"
            'HttpContext.Current.Session.Item("SortExpression_compartido_WF") = ""
            'HttpContext.Current.Session.Item("SortDirection_compartido_WF") = ""
            If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0 Then
                Mens.Showscripman("El usuario no tiene permiso para listar tareas ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 1 Then
                Result = Ref.Pre_Listar_tareas_workflow_Script(Me.Page,
                                                               OBE,
                                                               Me.GridView2,
                                                               1,
                                                               HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                               HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                               HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                               HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                               Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
                    Exit Sub
                End If
            End If
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 0 Then
                Result = Ref.Pre_Listar_tareas_workflow(Me.Page,
                                                        OBE,
                                                        Me.GridView2,
                                                        1,
                                                        HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                        HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                        HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                        HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                        Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanel_boton_tool)
                    Exit Sub
                End If
            End If
            'If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") <> 0 Then
            '    Result = Ref.Inicializar_la_lista_de_tareas_workflow(Me.Page,
            '                                                         OBE,
            '                                                         Me.GridView2,
            '                                                         1,
            '                                                         1,
            '                                                         "",
            '                                                         HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
            '                                                         HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
            '                                                         Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
            '    If Result <> "YES" Then
            '        Mens.Show(Result)
            '    End If
            'Else
            '    LabelEspera.Text = "No tiene permiso para listar tareas"
            '    Me.UpdatePanelnumeroespera.Update()
            'End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_boton_tool)
        End Try
    End Sub


    Private Sub Button_tool_activa_detalle_radicado_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_detalle_radicado.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_estados_modulo_radicacion
            If Me.Hidden_id_tarea_sel.Value = "-1" Or Me.Hidden_id_tarea_sel.Value = "" Then
                Mens.Showscripman_menu("Debe seleccionar la tarea", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(Me.Hidden_id_tarea_sel.Value,
                                                                                    radicado)
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_class_detalle_plantilla_rad As New Class_ra_detalle_plantilla_radicado
            Result = ref_class_detalle_plantilla_rad.Genera_interface_detalle_radicado(radicado,
                                                                                       Me.Page)
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_detalle_radicado.Show()
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub Button_tool_activa_detalle_radicado_seleccion_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_detalle_radicado_seleccion.Click
        Dim Mens As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_estados_modulo_radicacion
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Mens.Showscripman_menu("Debe seleccionar la tarea para ver la información", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    radicado)
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_class_detalle_plantilla_rad As New Class_ra_detalle_plantilla_radicado
            Result = ref_class_detalle_plantilla_rad.Genera_interface_detalle_radicado(radicado,
                                                                                       Me.Page)
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_detalle_radicado.Show()
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_boton_tool)
        End Try
    End Sub
    '-------sub para desencadenar popup confirmacion terminar tarea y enviar a actividad
    Private Sub ImageButtonterminar_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonterminar.Click
        Dim showmensaje As New Classscrripjava
        Try
            Dim ClassWorkflow As New ClassWorkflow
            Dim Result As String = ""
            Result = ClassWorkflow.Validar_enviar_actividad_por_conector_flujo_o_ruta(Me.Page)
            If Result <> "YES" Then
                showmensaje.Showscripman_menu(Result, Me.UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

        Catch ex As Exception
            showmensaje.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try

    End Sub
    Private Sub GridView_envia_flujo_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_envia_flujo.RowCreated
        Try
            If HttpContext.Current.Session.Item("WF_ESTADO_FLUJO_RUTA") = "FLUJO" Then
                e.Row.Cells(1).Visible = False
                e.Row.Cells(2).Visible = False
                e.Row.Cells(3).Visible = False
                e.Row.Cells(4).Visible = False
                e.Row.Cells(6).Visible = False
            Else
                e.Row.Cells(1).Visible = False
                e.Row.Cells(2).Visible = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    '----sub que activa el evento de popup de paginas externas
    Private Sub btnOkpagina_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOkpagina.Click
        Dim Refclasjava As New Classscrripjava
        Try
            If Me.HiddenPROMP.Value = "1" Then
                Exit Sub
            End If
            Dim Result As String = ""
            '---------------------------------
            'Verifica respuesta radicado
            '---------------------------------
            Dim refclasgestion As New Classgestionrespuesta
            Result = refclasgestion.Verifica_respuesta_radicado_sin_respuesta(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"))
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.Updatecondiciona)
                Exit Sub
            End If
            '------------------------------------
            'Eestado envió correo electrónico
            '------------------------------------
            Dim estado_envio_correo As Integer = 0
            Dim resul_correo As String = ""
            If CheckBox_noti_envio.Checked = True Then
                estado_envio_correo = 1
            Else
                estado_envio_correo = 0
            End If
            '-----------------------------------------------
            'Verifica estado solicitudes de aprobación sin
            'decisión
            '-----------------------------------------------
            Dim Estado_solicitud_aprobacion As String = ""
            Dim ref_class_solicitud As New ClassRaSolicitudesAprobacion
            Result = ref_class_solicitud.Verifica_solicitudes_de_aprobacion_sin_desicion(Val(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")),
                                                                                         Estado_solicitud_aprobacion,
                                                                                         HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Refclasjava.Showscripman(Result, Me.Updatecondiciona)
                Exit Sub
            End If
            If Estado_solicitud_aprobacion = "YES" Then
                Refclasjava.Showscripman("Imposible continuar con la operación, el sistema detecto solicitudes de aprobación pendientes por confirmar", Me.Updatecondiciona)
                Exit Sub
            End If
            If Session.Item("OPCIONSELECION") = "ENVIARUSUARIO" Then
                Dim refclas As New ClassWorkflow
                If Me.hdnEmailID.Value = "0" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                Else
                    Session.Item("SESIONITERCAMBIO") = Me.hdnEmailID.Value
                End If

                If Session.Item("SESIONITERCAMBIO") = "" Then
                    Refclasjava.Showscripman("Imposible enviar tarea usuario no seleccionado", Me.Updatecondiciona)
                    Exit Sub
                End If
                Dim Split() As String = Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                Dim Resultado_evalua_terminar As String = ""
                Result = refclas.Terminar_Tarea_Workflow(Split(0),
                                                         Split(1),
                                                         HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                         "",
                                                         Me.Page,
                                                         Resultado_evalua_terminar,
                                                         0,
                                                         resul_correo,
                                                         0,
                                                         0,
                                                         0,
                                                         estado_envio_correo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.Updatecondiciona)
                    Exit Sub
                Else
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UpdatePanel_general_variable.Update()
                    Me.hdnEmailID.Value = ""
                    Me.ModalPopupExtendermesjpagina.Hide()
                    Me.UpdatePanelintercambio.Update()
                    Me.UpdatePanelseleccion.Update()
                    Dim refcla As New ClassWorflowVisor
                    Dim Resutl As String = ""
                    Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                    If Resutl <> "YES" Then
                        Refclasjava.Showscripman(Result, Me.Updatecondiciona)
                    End If
                    If Resultado_evalua_terminar <> "YES" Then
                        Refclasjava.Showscripman(Resultado_evalua_terminar, Updatecondiciona)
                    End If
                    If resul_correo <> "" Then
                        Refclasjava.Showscripman(resul_correo, Me.Updatecondiciona)
                    End If
                End If
            End If
            '***********************************************
            'Ejecuta accion enviar actividad 
            '***********************************************
            If Session.Item("OPCIONSELECION") = "ENVIARACTIVIDAD" Then
                Dim refclas As New ClassWorkflow
                If Me.hdnEmailID.Value = "0" Then
                    Session.Item("SESIONITERCAMBIO") = ""
                Else
                    Session.Item("SESIONITERCAMBIO") = Me.hdnEmailID.Value
                End If

                If Session.Item("SESIONITERCAMBIO") = "" Then
                    Refclasjava.Showscripman("Imposible enviar tarea a Actividad", Me.Updatecondiciona)
                    Exit Sub
                End If
                Dim Split() As String = Session.Item("SESIONITERCAMBIO").ToString.Split("-")
                Dim Resultado_evalua_terminar As String = ""
                Result = refclas.Terminar_Tarea_Workflow("",
                                                         Split(0),
                                                         HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                         Split(1),
                                                         Me.Page,
                                                         Resultado_evalua_terminar,
                                                         0,
                                                         resul_correo,
                                                         0,
                                                         0,
                                                         0,
                                                         estado_envio_correo)
                If Result <> "YES" Then
                    Refclasjava.Showscripman(Result, Me.Updatecondiciona)
                    Exit Sub
                Else
                    Session.Item("SESIONITERCAMBIO") = ""
                    Session.Item("OPCIONSELECION") = ""
                    Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                    UpdatePanel_general_variable.Update()
                    Me.hdnEmailID.Value = ""
                    Me.ModalPopupExtendermesjpagina.Hide()
                    Me.UpdatePanelintercambio.Update()
                    Me.UpdatePanelseleccion.Update()
                    Dim refcla As New ClassWorflowVisor
                    Dim Resutl As String = ""
                    Resutl = refcla.Limpia_Visor_Workflow(Me, "PRINCIPAL")
                    If Resutl <> "YES" Then
                        Refclasjava.Showscripman(Result, Me.Updatecondiciona)
                    End If
                    If Resultado_evalua_terminar <> "YES" Then
                        Refclasjava.Showscripman(Resultado_evalua_terminar, Me.Updatecondiciona)
                    End If
                    If resul_correo <> "" Then
                        Refclasjava.Showscripman(resul_correo, Me.Updatecondiciona)
                    End If
                End If
            End If

        Catch ex As Exception
            Refclasjava.Showscripman(ex.Message, Me.Updatecondiciona)
        End Try
    End Sub

    Private Sub Button_descarga_consolidado_aprobacion_Click(sender As Object, e As EventArgs) Handles Button_descarga_consolidado_aprobacion.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Class_autoriza_tarea_worklfow As New Class_autoriza_tarea_worklfow
            Result = Class_autoriza_tarea_worklfow.Solicita_consolidado_autorizacion(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     Me.ifmExcel_xml_autoriza,
                                                                                     Me.Hidden_ruta_archivo,
                                                                                     Me.updatapanel_iframe_xml_autoriza)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.updatemenu_lista_autorizacion)
                Exit Sub
            Else
                Me.updatapanel_iframe.Update()
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.updatemenu_lista_autorizacion)
        End Try
    End Sub
    Private Sub GridView_lista_notas_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView_lista_notas.RowCreated
        Try
            e.Row.Cells(2).Visible = False
            e.Row.Cells(1).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception

        End Try
    End Sub


    Private Sub data_grid_lista_pendientes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles data_grid_lista_pendientes.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    '-----Boton para activar la ventana de pendientes
    Private Sub ImageButtonpendiente_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonpendiente.Click
        Dim refclsjava As New Classscrripjava
        Try
            If HttpContext.Current.Session("Interactuar_Pendiente") = "0" Then
                refclsjava.Showscripman("El usuario no tiene permiso para interactuar con tareas pendientes", UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim ref_Class_tarea_pendiente As New Class_tarea_pendiente
            Dim Result As String = ""
            Dim tipo_consulta As Integer = 1
            Dim valor_consulta As String = ""
            Dim colum_order_name As String = ""
            Dim order_colum As String = ""
            Result = ref_Class_tarea_pendiente.Lista_tareas_pendientes_workflow(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                                                HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                Me.data_grid_lista_pendientes,
                                                                                HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                Me.titulo_label_tareas_pendientes,
                                                                                Me.Hidden_id_list_pent,
                                                                                Me.UpdatePanel_tareas_pendientes,
                                                                                Me.UpdatePanel_title_tarea_pendiente,
                                                                                tipo_consulta,
                                                                                valor_consulta,
                                                                                colum_order_name,
                                                                                order_colum,
                                                                                Me.Hidden_count_reg)
            If Result <> "YES" Then
                refclsjava.Showscripman(Result, UpdatePanel_tool_menu)
                Exit Sub
            Else
                Me.ModalPopupExtender_edition_tareas_pendientes.Show()
            End If

        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_tool_menu)
        End Try

    End Sub

    Private Sub Button_tool_consulta_lista_tareas_pendiente_Click(sender As Object, e As EventArgs) Handles Button_tool_consulta_lista_tareas_pendiente.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim ref_Class_tarea_pendiente As New Class_tarea_pendiente
            Dim Result As String = ""
            Dim tipo_consulta As Integer = 2
            Dim valor_consulta As String = Me.busqueda_lista_pendiente.Text
            Dim colum_order_name As String = ""
            Dim order_colum As String = ""
            Result = ref_Class_tarea_pendiente.Lista_tareas_pendientes_workflow(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                                                HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                Me.data_grid_lista_pendientes,
                                                                                HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                Me.titulo_label_tareas_pendientes,
                                                                                Me.Hidden_id_list_pent,
                                                                                Me.UpdatePanel_tareas_pendientes,
                                                                                Me.UpdatePanel_title_tarea_pendiente,
                                                                                tipo_consulta,
                                                                                valor_consulta,
                                                                                colum_order_name,
                                                                                order_colum,
                                                                                Me.Hidden_count_reg)
            If Result <> "YES" Then
                refclsjava.Showscripman(Result, UpdatePanel_tool_tareas_pedientes)
                Exit Sub
            End If
        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_tool_tareas_pedientes)
        End Try
    End Sub

    Private Sub Button_tool_restore_lista_tareas_pendiente_Click(sender As Object, e As EventArgs) Handles Button_tool_restore_lista_tareas_pendiente.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim ref_Class_tarea_pendiente As New Class_tarea_pendiente
            Dim Result As String = ""
            Dim tipo_consulta As Integer = 1
            Dim valor_consulta As String = Me.busqueda_lista_pendiente.Text
            Dim colum_order_name As String = ""
            Dim order_colum As String = ""
            Result = ref_Class_tarea_pendiente.Lista_tareas_pendientes_workflow(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                HttpContext.Current.Session("Id_Grupo_Workflow"),
                                                                                HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                Me.data_grid_lista_pendientes,
                                                                                HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                Me.titulo_label_tareas_pendientes,
                                                                                Me.Hidden_id_list_pent,
                                                                                Me.UpdatePanel_tareas_pendientes,
                                                                                Me.UpdatePanel_title_tarea_pendiente,
                                                                                tipo_consulta,
                                                                                valor_consulta,
                                                                                colum_order_name,
                                                                                order_colum,
                                                                                Me.Hidden_count_reg)
            If Result <> "YES" Then
                refclsjava.Showscripman(Result, UpdatePanel_tool_tareas_pedientes)
                Exit Sub
            End If
        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_tool_tareas_pedientes)
        End Try
    End Sub

    Private Sub Button_tool_visor_emergente_tareas_pendiente_Click(sender As Object, e As EventArgs) Handles Button_tool_visor_emergente_tareas_pendiente.Click
        Dim refclsjava As New Classscrripjava
        Try
            Session.Item("SESIONITERCAMBIOVISOR") = "VISOR WORKFLOW|" & Me.Hidden_id_list_id_task.Value
            Me.Iframe_visor_tareas_pendiente_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_visor_tareas_pendiente.Update()
            Me.ModalPopupExtender_visor_tareas_pendiente.Show()
        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_tool_tareas_pedientes)
        End Try
    End Sub
    Private Sub Button_visor_emergente_Click(sender As Object, e As EventArgs) Handles Button_visor_emergente.Click
        Try
            Session.Item("SESIONITERCAMBIOVISOR") = Me.Hidden_tipo_visor.Value + "|" & Me.Hidden_id_tarea_sel.Value
            Me.Iframe_visor_tareas_pendiente_.Attributes("SRC") = "../workflow/WebFormVisorExterno.aspx"
            Me.UpdatePanel_visor_tareas_pendiente.Update()
            Me.ModalPopupExtender_visor_tareas_pendiente.Show()
        Catch ex As Exception

        End Try

    End Sub
    '----boton para asignar tareas de pendiente
    Private Sub ButtonAsignar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAsignar.Click
        Dim Mens As New Classscrripjava
        Try
            Me.hidden_000_aceptacion.Value = ""
            Dim refclas As New Class_tarea_pendiente
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") <> "0" Then
                Mens.Showscripman("Usuario con tarea seleccionada imposible asignar el documento", Me.UpdatePanelpedieteboton)
                Exit Sub
            End If
            Result = refclas.Sacar_Tarea_Pendiente(Me.Hidden_id_list_pent.Value,
                                                   Me.Hidden_id_list_id_task.Value,
                                                   Me.TreeViewseleccion,
                                                   Me.Page)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanelpedieteboton)
                Exit Sub
            Else
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = Me.Hidden_id_list_id_task.Value
                Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")

                UpdatePanel_general_variable.Update()
                Dim refclasvisor As New ClassWorflowVisor
                refclasvisor.Limpia_Visor_Workflow(Me.Page, "")

                Me.ModalPopupExtenderpendiente.Hide()
                Me.UpdatePanelintercambio.Update()
                Me.UpdatePanelseleccion.Update()
                Me.ModalPopupExtender_edition_tareas_pendientes.Hide()
                Dim Refclas_seleccion As New Classselecciotarea
                Result = Refclas_seleccion.Actualiza_interface_estado_flujo_ruta(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                 HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                 HttpContext.Current.Session("Id_Usuario_Workflow"),
                                                                                 HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD"),
                                                                                 Me.Page)
                If Result <> "YES" Then
                    Mens.Showscripman(Result, Me.UpdatePanelpedieteboton)
                    Exit Sub
                Else
                    Me.hidden_000_aceptacion.Value = "YES"
                End If
            End If
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanelpedieteboton)
        End Try
    End Sub
    'Enviar tarea a estado pendiente
    Protected Sub Button_aceptar_envia_documento_pendiente_apro_Click(sender As Object, e As EventArgs) Handles Button_aceptar_envia_documento_pendiente_apro.Click
        Dim clasjava As New Classscrripjava
        Try
            Me.Hidden_000_estado.Value = ""
            Dim Result As String = ""
            Dim Ref_class As New Class_tarea_pendiente
            Me.Hidden_0001_estado.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            Me.Hidden_id_tarea_sel.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            Result = Ref_class.Subir_Tarea_Pendiente(Me.TextBox_texto_pendiente_aprobacion.Text,
                                                     Page)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_buton_envia_documento_pendiente_apro, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0"
                HttpContext.Current.Session.Item("DG_ID_TRAMITE") = 0
                HttpContext.Current.Session.Item("DG_TIPO_TRAMITE") = ""
                HttpContext.Current.Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
                Dim Resutl_ As String = ""
                Dim ref_ClassDaGabinete As New ClassDaGabinete
                Dim refcla As New ClassWorflowVisor
                Resutl_ = ref_ClassDaGabinete.Inicializa_documentos_seleccion_workflow(Me.Page)
                If Resutl_ <> "YES" Then
                    clasjava.Showscripman_menu(Resutl_, Me.UpdatePanel_buton_envia_documento_pendiente_apro, "ModalPopupExtender_mensaje_personalizado")
                End If
                If Session.Item("UTIL_ITER_PENDIENTE") = 1 Then
                    Me.Hidden_0001_estado.Value = 1
                Else
                    Me.Hidden_0001_estado.Value = 0
                End If
                Me.Hidden_id_tarea_selecionada.Value = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
                Me.UpdatePanel_general_variable.Update()
                Me.Label_estado_selecion.Text = ""
                Me.updatemenu.Update()
                Resutl_ = refcla.Limpia_Visor_Workflow(Me,
                                                      "PRINCIPAL")
                If Resutl_ <> "YES" Then
                    clasjava.Showscripman_menu(Resutl_, Me.UpdatePanel_buton_envia_documento_pendiente_apro, "ModalPopupExtender_mensaje_personalizado")
                End If
                Me.Hidden_000_estado.Value = "YES"
                Me.ModalPopupExtender_edition_envia_documento_pendiente_apro.Hide()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_buton_envia_documento_pendiente_apro, "ModalPopupExtender_mensaje_personalizado")
        Finally
            Session.Item("GA_STRU_ESTADO_NUEVA_SOLICITUD_APROBACION") = ""
        End Try
    End Sub
    Private Sub Button_actualiza_indice_imagen_Click(sender As Object, e As EventArgs) Handles Button_actualiza_indice_imagen.Click
        Dim Matri_Sender() As String
        Dim Result As String = ""
        Erase Matri_Sender
        Dim Mens As New Classscrripjava
        Dim Refclasindice As New ClassWorkflowIndiceDA
        Try
            Matri_Sender = Split(Hidden_image_gabinete.Value, "|")
            Result = Refclasindice.Actualiza_Indice_Imagen(Matri_Sender(0),
                                                           Matri_Sender(1),
                                                           "",
                                                           "",
                                                           HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                           HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                           Me.Page,
                                                           "")
            If Result <> "YES" Then
                Mens.Showscripman("Imposible actualizar indice " & Result, Updatepanel_actualiza)
            End If

        Catch ex As Exception
            Mens.Showscripman("Error general actualizando indice " & ex.Message, Updatepanel_actualiza)
        End Try
    End Sub


    'ZONA SUBIR DOCUMENTOS WEB SERVICE CCV SELLOS
    'Boton activa ventana listado de inscripciones de sellos disponibles
    Private Sub Button_tool_activa_sube_imagen_inscripcion_web_service_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_imagen_inscripcion_web_service.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_estados_modulo_radicacion
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "-1" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                refclsjava.Showscripman_menu("Debe seleccionar una tarea para adjuntar un documento desde los servicios web.", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                refclsjava.Showscripman("El usuario no tiene permisos para adjuntar documentos desde los servicios de integración. ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaCodigoBarrasIdTareaWorflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                         HttpContext.Current.Session.Item("SII_COD_BARRAS"))
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    HttpContext.Current.Session.Item("SII_RECIBO"))
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If

            Dim Class_lista_imagenes_sii As New Class_consultarInformacionSello
            Session.Item("SortExpression_publico") = "id_estado_radicado"
            Session.Item("SortDirection_publico") = "DESC"
            Result = Class_lista_imagenes_sii.Lista_inscripciones_radicado_sii(HttpContext.Current.Session.Item("SII_COD_BARRAS"),
                                                                               HttpContext.Current.Session.Item("SII_RECIBO"),
                                                                               1,
                                                                               "",
                                                                               HttpContext.Current.Session.Item("SortExpression_publico"),
                                                                               HttpContext.Current.Session.Item("SortDirection_publico"),
                                                                               Me.titulo_label_list_inscripciones_sii,
                                                                               Me.GridView_list_inscripciones_sii,
                                                                               Me.Hidden_list_inscripciones_sii,
                                                                               Me.Update_list_inscripciones_sii)
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_list_inscripciones_sii.Show()
            End If
        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub

    Private Sub GridView_list_inscripciones_sii_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_list_inscripciones_sii.RowCreated
        Try
            e.Row.Cells(6).Visible = False
            e.Row.Cells(7).Visible = False
            e.Row.Cells(8).Visible = False
            e.Row.Cells(9).Visible = False
        Catch ex As Exception

        End Try
    End Sub
    Private Sub GridView_list_inscripciones_sii_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_list_inscripciones_sii.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            GridView_list_inscripciones_sii.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim Class_lista_imagenes_sii As New Class_consultarInformacionSello
            Session.Item("SortExpression_publico") = "id_estado_radicado"
            Session.Item("SortDirection_publico") = "DESC"
            Result = Class_lista_imagenes_sii.Lista_inscripciones_radicado_sii(HttpContext.Current.Session.Item("SII_COD_BARRAS"),
                                                                               HttpContext.Current.Session.Item("SII_RECIBO"),
                                                                               2,
                                                                               "",
                                                                               HttpContext.Current.Session.Item("SortExpression_publico"),
                                                                               HttpContext.Current.Session.Item("SortDirection_publico"),
                                                                               Me.titulo_label_list_inscripciones_sii,
                                                                               Me.GridView_list_inscripciones_sii,
                                                                               Me.Hidden_list_inscripciones_sii,
                                                                               Me.Update_list_inscripciones_sii)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")

            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.Update_list_inscripciones_sii, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    'ZONA SUBE DOCMENTOS ENLACE WEB CCV
    Private Sub Button_tool_activa_sube_documento_web_service_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento_web_service.Click
        Dim refclsjava As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Ref_class_estados_modulo_rad As New Class_estados_modulo_radicacion
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE") = "-1" Or HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE") = "0" Then
                refclsjava.Showscripman_menu("Debe seleccionar la tarea", UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                refclsjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            Result = ref_Class_DAT_ADIC_TAR.SolicitaCodigoBarrasIdTareaWorflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                               radicado)
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim class_lista_imagenes_sii As New Class_lista_imagenes_sii.imagenes_sii_lista
            Session.Item("sortexpression_publico") = "id_estado_radicado"
            Session.Item("sortdirection_publico") = "DESC"
            Result = class_lista_imagenes_sii.Lista_imagenes_sii(radicado,
                                                                 1,
                                                                 "",
                                                                 HttpContext.Current.Session.Item("sortexpression_publico"),
                                                                 HttpContext.Current.Session.Item("sortdirection_publico"),
                                                                 Me.titulo_label_list_imagenes_sii,
                                                                 Me.GridView_list_imagenes_sii,
                                                                 Me.Hidden_list_imagenes_sii,
                                                                 Me.Update_list_imagenes_sii)
            If Result <> "YES" Then
                refclsjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "modalpopupextender_mensaje_personalizado")
            Else
                Me.ModalPopupExtender_edition_list_imagenes_sii.Show()
            End If
        Catch ex As Exception
            refclsjava.Showscripman(ex.Message, UpdatePanel_boton_tool)
        End Try
    End Sub
    Private Sub GridView_list_imagenes_sii_DataBound(sender As Object, e As EventArgs) Handles GridView_list_imagenes_sii.DataBound
        Try
            Select Case sender.SortDirection
                Case SortDirection.Ascending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                Case SortDirection.Descending
                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black

                    sender.HeaderRow.ForeColor = System.Drawing.Color.Black
                    sender.FooterRow.ForeColor = System.Drawing.Color.Black
            End Select
        Catch ex As Exception
        End Try
    End Sub

    Private Sub GridView_list_imagenes_sii_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView_list_imagenes_sii.PageIndexChanging
        Dim clasjava As New Classscrripjava
        Try
            GridView_list_imagenes_sii.PageIndex = e.NewPageIndex
            Dim Result As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim radicado As String = ""
            Dim Class_lista_imagenes_sii As New Class_lista_imagenes_sii.imagenes_sii_lista
            Session.Item("SortExpression_publico") = "id_estado_radicado"
            Session.Item("SortDirection_publico") = "DESC"
            Result = Class_lista_imagenes_sii.Lista_imagenes_sii(radicado,
                                                                 2,
                                                                 "",
                                                                 HttpContext.Current.Session.Item("SortExpression_publico"),
                                                                 HttpContext.Current.Session.Item("SortDirection_publico"),
                                                                 Me.titulo_label_list_imagenes_sii,
                                                                 Me.GridView_list_imagenes_sii,
                                                                 Me.Hidden_list_imagenes_sii,
                                                                 Me.Update_list_imagenes_sii)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")

            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.Update_list_imagenes_sii, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub GridView_list_imagenes_sii_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_list_imagenes_sii.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
        Catch ex As Exception

        End Try
    End Sub

    'Activa para adjuntar documentos del servicio web   Deprecate
    Private Sub Button_tool_activa_sube_documento_enlace_integra_sii_Click(sender As Object, e As EventArgs) Handles Button_tool_activa_sube_documento_enlace_integra_sii.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas_config As New Class_ra_dig_config_digitalizacion
            Dim Result As String = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            If HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") = -1 Or HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") = 0 Then
                Exit Sub
            End If
            If Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0 Then
                clasjava.Showscripman("El usuario no tiene permisos para adjuntar imagenes ", Me.UpdatePanel_boton_tool)
                Exit Sub
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Session.Item("WF_RUTAWORKFLOW"),
                                                                                            HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE"),
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                clasjava.Showscripman_menu("Imposible econtrar el id del gabinete de la tarea (" & HttpContext.Current.Session("ID_TAREA_SELECCIONDA_ENLACE") & ")", Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                              structure_gabinete_workflow)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.DropDownList_documento_integra_sii.Items.Clear()
            Me.UpdatePanel_drowp_sube_documento_integra_sii.Update()
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO") = structure_gabinete_workflow.NOMBRE_GABINETE
            Result = Refclas_config.Solicita_id_configuracion_digitalizacion(Session.Item("DG_ID_TRAMITE"),
                                                                             Session.Item("DG_TIPO_TRAMITE"),
                                                                             Session.Item("DG_ID_CONFIG_DIGITALIZACION"),
                                                                             0)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Dim Refclas_list_cheg As New ra_dig_tipos_docum_lista_chequeo
            Dim estado_resultado As String = ""
            Result = Refclas_list_cheg.Solicita_listar_tipos_documentales_relacionados_edita_tramite_lista_adjunta_drowlist(Session.Item("DG_ID_TRAMITE"),
                                                                                                                            Session.Item("DG_TIPO_TRAMITE"),
                                                                                                                            "",
                                                                                                                            Me.DropDownList_documento_integra_sii,
                                                                                                                            Me.UpdatePanel_drowp_sube_documento_integra_sii,
                                                                                                                            estado_resultado)
            If Result <> "YES" Then
                clasjava.Showscripman_menu(Result, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            Else
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Me.ModalPopupExtender_sube_documento_integra_sii.Show()
            End If
        Catch ex As Exception
            clasjava.Showscripman_menu(ex.Message, Me.UpdatePanel_boton_tool, "ModalPopupExtender_mensaje_personalizado")
        End Try
    End Sub

    Private Sub Button_acepta_sube_documento_integra_sii_Click(sender As Object, e As EventArgs) Handles Button_acepta_sube_documento_integra_sii.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAlmacenamiento
            Dim Class_ra_dig_config_digitalizacion As New Class_ra_dig_config_digitalizacion
            Dim stru_config As Stru_config_digitalizacion = Nothing
            Result = Class_ra_dig_config_digitalizacion.SolicitaDatosConfiguracionDigitalizacionPorTramite(Session.Item("DG_ID_TRAMITE"), stru_config)
            If Result <> "YES" Then
                CLAS.Showscripman(Result, UpdatePanel_boton_sube_documento_integra_sii)
                Exit Sub
            End If
            If stru_config.OBLIGA_LISTA_CHEQUEO = 1 And Me.DropDownList_documento_integra_sii.SelectedValue = "" Then
                CLAS.Showscripman("Debe seleccionar la tipología documental para guardar el documento", UpdatePanel_boton_sube_documento_integra_sii)
                Exit Sub
            End If
            Dim stru_datos_image_lista As stru_datos_image_lista = Nothing
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "/DONWLOAD/"
            Dim ruta_fisica As String = Server.MapPath(ruta_virtual)
            If Directory.Exists(ruta_fisica) = False Then
                Directory.CreateDirectory(ruta_fisica)
            End If
            Dim ob1 = Me.Hidden_extension.Value
            Dim archivo As String = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION").ToString & "_doc_adjunto_." & Me.Hidden_extension.Value
            Dim archivo_donwload As String = ruta_fisica & archivo
            If IO.File.Exists(archivo_donwload) Then
                Kill(archivo_donwload)
            End If
            Dim ob As Object
            Dim Class_file_byte As New Class_file_byte
            Result = Class_file_byte.DownloadFileViaRestAPI(Hidden_url.Value,
                                                            ob,
                                                            "MyDocumentLib",
                                                            archivo,
                                                            ruta_fisica)
            If Result <> "YES" Then
                CLAS.Showscripman(Result, UpdatePanel_boton_sube_documento_integra_sii)
                Exit Sub
            End If
            Session.Item("WF_RUTA_TEMPO_ADJUNTA") = archivo_donwload
            If Session.Item("WF_RUTA_TEMPO_ADJUNTA") = "" Then
                Exit Sub
            End If
            If Me.DropDownList_documento_integra_sii.SelectedValue = "" Then
                Session.Item("DG_LISTA_CHEQUEO") = "-1"
            Else
                Session.Item("DG_LISTA_CHEQUEO") = Me.DropDownList_documento_integra_sii.SelectedValue
            End If
            Dim id_tipo_documento As Integer = Val(Me.DropDownList_documento_integra_sii.SelectedValue)
            Dim id_imagen_almacenada As Integer = 0
            Dim id_tarea_workflow As Long = 0
            Dim contador As Integer = 0
            HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = "ENLACE"
            Result = Refclas.UploadSaveFile(0,
                                              id_tipo_documento,
                                              "",
                                              0,
                                              0,
                                              0,
                                              "",
                                              stru_datos_image_lista,
                                              id_tarea_workflow,
                                              contador)
            If Result <> "YES" Then
                CLAS.Showscripman(Result, UpdatePanel_boton_sube_documento_integra_sii)
                ModalPopupExtender_sube_documento_integra_sii.Hide()
                HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = ""
                Session.Item("DG_LISTA_CHEQUEO") = -1
                Exit Sub
            Else
                Me.UpdatePanel_descarga.Update()
                Me.Hidden_result_load.Value = "YES"
                Dim file_icon_some = "fa-file"
                If stru_datos_image_lista.icono_icono_awe_some <> "" Then
                    Dim espacio = " "
                    Dim spli_some = stru_datos_image_lista.icono_icono_awe_some.Split(espacio)
                    file_icon_some = spli_some(1)
                End If
                Me.Hidden_tip_adjunt.Value = "rad"
                Me.Hidden_date_row.Value = stru_datos_image_lista.nombre_gabinete & "|" & stru_datos_image_lista.id_imagen & "|" & stru_datos_image_lista.radicado &
               "|" & stru_datos_image_lista.tipodocumental & "|" & stru_datos_image_lista.notipodocumento & "|" & stru_datos_image_lista.id_tarea_workflow & "|" &
               stru_datos_image_lista.estado_firma_digital & "|" & file_icon_some
                Session.Item("DG_LISTA_CHEQUEO") = -1
                HttpContext.Current.Session.Item("WF_TIPO_ADJUNTA") = ""
                Me.ModalPopupExtender_sube_documento_integra_sii.Hide()
            End If
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, UpdatePanel_boton_sube_documento_integra_sii)
        End Try
    End Sub

    Private Sub Button_activa_estado_paginacion_Click(sender As Object, e As EventArgs) Handles Button_activa_estado_paginacion.Click
        Try
            If HttpContext.Current.Session.Item("UTIL_PAGINACION") = 1 Then
                Me.CheckBox_estado_paginacion.Checked = True
            Else
                Me.CheckBox_estado_paginacion.Checked = False
            End If
            Me.UpdatePanel_estado_paginacion_chek.Update()
            Me.ModalPopupExtender_edition_estado_paginacion.Show()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_cambiar_estado_paginacion_Click(sender As Object, e As EventArgs) Handles Button_cambiar_estado_paginacion.Click
        Dim CLAS As New Classscrripjava
        Try
            Dim ref_ClassWorkflowUsuario As New ClassWorkflowUsuario
            Dim Result As String = ""
            Dim estado_paginacion As Integer = 0
            Dim OBE As Object
            If Me.CheckBox_estado_paginacion.Checked = True Then
                estado_paginacion = 1
            Else
                estado_paginacion = 0
            End If
            Result = ref_ClassWorkflowUsuario.Cambia_estado_pagiancion_usuario(estado_paginacion,
                                                                             Val(Session.Item("Id_Usuario_Workflow")))
            If Result <> "YES" Then
                CLAS.Showscripman(Result, UpdatePanel_estado_paginacion)
                Exit Sub
            Else
                Dim Ref As New ClassListandoTareas
                HttpContext.Current.Session.Item("UTIL_PAGINACION") = estado_paginacion
                If HttpContext.Current.Session.Item("UTIL_PAGINACION") = 1 Then
                    Me.GridView2.AllowPaging = True
                Else
                    Me.GridView2.AllowPaging = False
                End If
                If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0 Then
                    CLAS.Showscripman("El usuario no tiene permiso para listar tareas ", Me.UpdatePanel_estado_paginacion)
                    Exit Sub
                End If
                If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 1 Then
                    Result = Ref.Pre_Listar_tareas_workflow_Script(Me.Page,
                                                               OBE,
                                                               Me.GridView2,
                                                               1,
                                                               HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                               HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                               HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                               HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                               Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                    If Result <> "YES" Then
                        CLAS.Showscripman(Result, Me.UpdatePanel_estado_paginacion)
                        Exit Sub
                    End If
                End If
                If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 0 Then
                    Result = Ref.Pre_Listar_tareas_workflow(Me.Page,
                                                        OBE,
                                                        Me.GridView2,
                                                        1,
                                                        HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                        HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                        HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                        HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                        Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                    If Result <> "YES" Then
                        CLAS.Showscripman(Result, Me.UpdatePanel_estado_paginacion)
                        Exit Sub
                    End If
                End If

                Me.ModalPopupExtender_edition_estado_paginacion.Hide()
            End If
        Catch ex As Exception
            CLAS.Showscripman(ex.Message, Me.UpdatePanel_estado_paginacion)
        End Try
    End Sub

    Private Sub DropDownListseleccionfiltro_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListseleccionfiltro.SelectedIndexChanged
        Dim CLAS As New Classscrripjava
        Try
            Dim Ref As New ClassListandoTareas
            Dim Result As String = ""
            'Dim OBE As Object
            If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0 Then
                CLAS.Showscripman("El usuario no tiene permiso para listar tareas ", Me.UpdatePanelseleccionfiltro)
                Exit Sub
            End If
            Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF") = DropDownListseleccionfiltro.SelectedValue
            Result = Ref.Cache_filtra_lista_tareas_workflow(Me.GridView2,
                                                            UpdatePanel1,
                                                            0,
                                                            "",
                                                            "",
                                                            Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"),
                                                            Me.LabelEspera,
                                                            Me.UpdatePanelnumeroespera)
            If Result <> "YES" Then
                CLAS.Showscripman(Result, Me.UpdatePanelseleccionfiltro)
                Exit Sub
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button_activa_search_Click(sender As Object, e As EventArgs) Handles Button_activa_search.Click
        Try
            Dim Ref As New ClassListandoTareas
            Dim Result As String = ""
            Dim OBE As Object
            Dim Mens As New Classscrripjava
            Me.HiddenSeleccion.Value = "-1"
            If HttpContext.Current.Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0 Then
                'Mens.Showscripman("El usuario no tiene permiso para listar tareas ", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 1 Then
                Result = Ref.Pre_Listar_tareas_workflow_Script(Me.Page,
                                                           OBE,
                                                           Me.GridView2,
                                                           1,
                                                           HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                           HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                           HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                           HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                           HttpContext.Current.Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    'Mens.Showscripman(Result, Me.UpdatePanel_tool_menu)
                    Exit Sub
                End If
            End If
            If HttpContext.Current.Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 0 Then
                Result = Ref.Pre_Listar_tareas_workflow(Me.Page,
                                                    OBE,
                                                    Me.GridView2,
                                                    1,
                                                    HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                    HttpContext.Current.Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_WF"),
                                                    HttpContext.Current.Session.Item("SortExpression_compartido_WF"),
                                                    HttpContext.Current.Session.Item("SortDirection_compartido_WF"), 1,
                                                    HttpContext.Current.Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF"))
                If Result <> "YES" Then
                    'Mens.Showscripman(Result, Me.UpdatePanel_tool_menu)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button_activa_copiar_estructura_Click(sender As Object, e As EventArgs) Handles Button_activa_copiar_estructura.Click
        Dim Mens As New Classscrripjava
        Try
            Dim refclas As New Classselecciotarea
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("COPIA_ESTRUCTURA_PRODUCION") = 0 Then
                Mens.Showscripman("Usuario sin permmisos para copiar documentos a estructutura de produción documental", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Mens.Showscripman("Usuario sin tarea seleccionada imposible copiar documentos ", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim stru_paramter_image_final As stru_paramter_image() = Nothing
            Result = refclas.Solicita_lista_id_producion_relacionados_tarea_workflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     stru_paramter_image_final)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            If stru_paramter_image_final Is Nothing Then
                Mens.Showscripman("No hay documentos para copiar", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            For i As Integer = 0 To stru_paramter_image_final.Length - 1
                ReDim Preserve HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i)
                HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i) = stru_paramter_image_final(i).ID_PRODUCCION
            Next
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            Me.Iframe_copiar_estructura_.Attributes("SRC") = "../Gestion/WebFormProducionDocumental.aspx"
            Me.UpdatePanel_copiar_estructura.Update()
            Me.ModalPopupExtender_copiar_estructura.Show()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub

    Private Sub Button_activa_copiar_expediente_Click(sender As Object, e As EventArgs) Handles Button_activa_copiar_expediente.Click
        Dim Mens As New Classscrripjava
        Try
            Dim refclas As New Classselecciotarea
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("COPIA_DOCUMENTO_EXPEDIENTE") = 0 Then
                Mens.Showscripman("Usuario sin permmisos para copiar documentos a expediente", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Mens.Showscripman("Usuario sin tarea seleccionada imposible copiar documentos ", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim stru_paramter_image_final As stru_paramter_image() = Nothing
            Result = refclas.Solicita_lista_id_producion_relacionados_tarea_workflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                     stru_paramter_image_final)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            If stru_paramter_image_final Is Nothing Then
                Mens.Showscripman("No hay documentos para copiar", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            For i As Integer = 0 To stru_paramter_image_final.Length - 1
                ReDim Preserve HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i)
                HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i) = stru_paramter_image_final(i).ID_PRODUCCION
            Next
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 1
            Me.Iframe_copiar_estructura_.Attributes("SRC") = "../Gestion/WebFormGaGestionExpediente.aspx"
            Me.UpdatePanel_copiar_estructura.Update()
            Me.ModalPopupExtender_copiar_estructura.Show()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub

    Private Sub Button_activa_incorpora_expediente_Click(sender As Object, e As EventArgs) Handles Button_activa_incorpora_expediente.Click
        Dim Mens As New Classscrripjava
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("RELACIONA_EXPEDIENTE") = 0 Then
                Mens.Showscripman("Usuario sin permmisos para vincular archivos a expediente", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Mens.Showscripman("Usuario sin tarea seleccionada imposible vincular documentos ", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            Dim stru_paramter_image_final As stru_imagen_gabinete_workflow() = Nothing
            Result = ClassDaGabinete.SolicitaListaImagensGabineteRelacionTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                       stru_paramter_image_final)
            If Result <> "YES" Then
                Mens.Showscripman(Result, Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            If stru_paramter_image_final Is Nothing Then
                Mens.Showscripman("No hay documentos para vincular", Me.UpdatePanel_tool_menu)
                Exit Sub
            End If
            For i As Integer = 0 To stru_paramter_image_final.Length - 1
                ReDim Preserve HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA")(i)
                HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA")(i) = stru_paramter_image_final(i).id_image & "|" & stru_paramter_image_final(i).gabinete
            Next
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Mens.Showscripman_menu(Result, UpdatePanel_tool_menu, "ModalPopupExtender_mensaje_personalizado")
                Exit Sub
            End If
            HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 2
            Me.Iframe_copiar_estructura_.Attributes("SRC") = "../Gestion/WebFormGaGestionExpediente.aspx"
            Me.UpdatePanel_copiar_estructura.Update()
            Me.ModalPopupExtender_copiar_estructura.Show()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, Me.UpdatePanel_tool_menu)
        End Try
    End Sub

    Private Sub GridView_list_documento_relacion_wf_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_list_documento_relacion_wf.RowCreated
        Try
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
            e.Row.Cells(5).Visible = False
        Catch ex As Exception
        End Try
    End Sub

    'ZONA VISOR NEODIAMIC
    Private Sub ImageButtonInicio_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonInicio.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "inicio",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   Me.DropDownList_zom,
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub ImageButtonFinal_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonFinal.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "final",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   DropDownList_zom,
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageButtonAnterior_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonAnterior.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "-1",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   DropDownList_zom,
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageButtonSiguiente_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonSiguiente.Click
        Try
            Dim clasjava As New Classscrripjava
            Me.ImageButtonSiguiente.Enabled = False
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual,
                                                                   Doc_actual,
                                                                   "+1",
                                                                   0,
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   Me.DropDownList_zom,
                                                                   Me.UpdatePanel_conte_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
        Finally
            Me.ImageButtonSiguiente.Enabled = True
        End Try
    End Sub
    Private Sub ImageButton_ir_pagina_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButton_ir_pagina.Click
        Dim clasjava As New Classscrripjava
        Try
            If Me.LabelConteo.Text = "" Then Exit Sub
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor(Matri_Doc_Visual, Doc_actual,
                                                                   "seleccion",
                                                                   Val(Me.LabelConteo.Text),
                                                                   Me,
                                                                   HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_HEIHG_SIZE"),
                                                                   HttpContext.Current.Session.Item("WF_IMAGE_WITH_SIZE"),
                                                                   Me.DropDownList_zom,
                                                                   Me.Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
            Me.UpdatePanel_conte_bot.Update()
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Protected Sub ImageMenos_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMenos.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual, "-",
                                                                          Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                          DropDownList_zom,
                                                                          Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageMas_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageMas.Click
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale(Matri_Doc_Visual,
                                                                          "+",
                                                                          Me,
                                                                          HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                          HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                          DropDownList_zom,
                                                                          Updatepanel_boton_content)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)

            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub DropDownList_zom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList_zom.SelectedIndexChanged
        Dim Refclas As New ClassWorflowVisor
        Dim clasjava As New Classscrripjava
        Try
            Dim Result As String = Refclas.Muestra_Documento_Visor_Escale_zom(Matri_Doc_Visual, DropDownList_zom.SelectedValue,
                                                                              Me,
                                                                              HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
                                                                              HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"),
                                                                              HttpContext.Current.Session.Item("WF_IMAGE_WITH"),
                                                                              DropDownList_zom,
                                                                              Me.UpdatePanel_drows_bot)
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)

            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub ImageRotate45_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate45.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 90, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageRotate180_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate180.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 180, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub

    Private Sub ImageRotate270_Click(sender As Object, e As ImageClickEventArgs) Handles ImageRotate270.Click
        Dim clasjava As New Classscrripjava
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = Refclas.Muestra_Documento_Visor_Rotate(Matri_Doc_Visual, 270, Me, HttpContext.Current.Session.Item("WF_DOC_ACTUAL"),
            HttpContext.Current.Session.Item("WF_IMAGE_HEIHG"), HttpContext.Current.Session.Item("WF_IMAGE_WITH"))
            If Result <> "YES" Then
                clasjava.Showscripman(Result, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            clasjava.Showscripman(ex.Message, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Private Sub ImageButtonguardar_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButtonguardar.Click
        Dim Mens As New Classscrripjava
        Try
            Dim refclas As New ClassNeodynamic
            Dim Result As String = ""
            Dim fx, fy, fwith, fhigth, topcontenido, scrootop, tamimag, heigimageor, witimageor As String
            Dim matrival() As String = Split(Me.Hiddenintercambio.Value, "-")
            fy = matrival(0)
            fx = matrival(1)
            fhigth = matrival(2)
            fwith = matrival(3)
            topcontenido = matrival(4)
            scrootop = matrival(5)
            tamimag = matrival(6)
            heigimageor = matrival(7)
            witimageor = matrival(8)
            Dim ruta As String = Matri_Doc_Visual(HttpContext.Current.Session.Item("WF_DOC_ACTUAL"))
            Result = refclas.Shape_Firma(ruta,
                                         fx,
                                         fy,
                                         fwith,
                                         fhigth,
                                         Matri_Doc_Visual,
                                         Me,
                                         topcontenido,
                                         scrootop,
                                         tamimag,
                                         heigimageor,
                                         witimageor)
            If Result <> "YES" Then
                Mens.Showscripman("Grabar error " & Result & " " & Me.Hiddenintercambio.Value, Me.Updatepanel_boton_content)
            End If
        Catch ex As Exception
            Mens.Showscripman("Grabar error " & ex.ToString & " " & Me.Hiddenintercambio.Value, Me.Updatepanel_boton_content)
        End Try
    End Sub
    Protected Sub ImageButtonguardardocumento_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonguardardocumento.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            If HttpContext.Current.Session.Item("WF_TAGSELECCION") = "" Then
                scri.Showscripman("Debe seleccionar un documento ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Tag_Seleccion() As String = HttpContext.Current.Session.Item("WF_TAGSELECCION").ToString.Split("|")
            Dim gabinete_consulta As String = Tag_Seleccion(5)
            Dim id_imagen As Integer = Tag_Seleccion(2)
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(gabinete_consulta,
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman("Imposible guardar " & Result, Me.Updatepanel_boton_content)
                Exit Sub
            End If
            If stru_permiso.GUARDAR_IMAGEN = 0 Then
                scri.Showscripman("El usuario no tiene permisos para guardar imagenes desde gabinete ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            Session.Item("RA_RUTA_IMPRESION_FINAL") = ""
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    'ReDim Preserve Matri_Doc_Visual(i)
                    If i = 1 Then
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Matri_Temp(i)
                    Else
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Session.Item("RA_RUTA_IMPRESION_FINAL") & "," & Matri_Temp(i)
                    End If
                Next
            End If
            If Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
                scri.Showscripman("Imposible consultar descargar la matriz de documentos esta vacia ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If

            Session.Item("DA_GABINETE_IMPRESION") = gabinete_consulta
            Session.Item("DA_ID_IMAGEN_IMPRESION") = id_imagen
            Iframe_guardar.Attributes.Add("src", "../Docuarchi/WebFormDaExportArchivo.aspx")
            Me.ModalPopupExtender_guardar.Show()
            UpdatePane_guardar.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, Updatepanel_boton_content)
        End Try

    End Sub
    Private Sub ImageButtonimprimir_Click(sender As Object, e As ImageClickEventArgs) Handles ImageButtonimprimir.Click
        Dim scri As New Classscrripjava
        Dim refgabinete As New ClassDaGabinete
        Dim stru_permiso As stru_permiso_gabinete = Nothing
        Dim Result As String = ""
        Try
            If HttpContext.Current.Session.Item("WF_TAGSELECCION") = "" Then
                scri.Showscripman("Debe seleccionar un documento ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Tag_Seleccion() As String = HttpContext.Current.Session.Item("WF_TAGSELECCION").ToString.Split("|")
            Dim gabinete_consulta As String = Tag_Seleccion(5)
            Dim id_imagen As Integer = Tag_Seleccion(2)
            Result = refgabinete.SolicitaPermisosGeneralesGabinete(gabinete_consulta,
                                                                   HttpContext.Current.Session.Item("ID_USUARIO_DOCUARCHI"),
                                                                   HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                   stru_permiso)
            If Result <> "YES" Then
                scri.Showscripman("Imposible imprimir " & Result, Me.Updatepanel_boton_content)
                Exit Sub
            End If
            If stru_permiso.IMPRI_IMAGEN = 0 Then
                scri.Showscripman("El usuario no tiene permisos para imprimir en el gabinete ", Me.Updatepanel_boton_content)
                Exit Sub
            End If
            Dim Matri_Temp() As String
            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            Session.Item("RA_RUTA_IMPRESION_FINAL") = ""
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    'ReDim Preserve Matri_Doc_Visual(i)
                    If i = 1 Then
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Matri_Temp(i)
                    Else
                        Session.Item("RA_RUTA_IMPRESION_FINAL") = Session.Item("RA_RUTA_IMPRESION_FINAL") & "," & Matri_Temp(i)
                    End If

                Next
            End If

            If Session.Item("RA_RUTA_IMPRESION_FINAL") = "" Then
                scri.Showscripman("Imposible consultar imprimir la matriz de documentos esta vacia ", Me.Updatepanel_boton_content)
                Exit Sub
            End If

            Erase Matri_Temp
            Matri_Temp = Split(Session.Item("WF_MATRI_IMAGE"), "|")
            If Not Matri_Temp Is Nothing Then
                For i As Integer = 0 To Matri_Temp.Length - 2
                    ReDim Preserve Matri_Doc_Visual(i)
                    Matri_Doc_Visual(i) = Matri_Temp(i)
                Next
            End If
            Session.Item("DA_GABINETE_IMPRESION") = gabinete_consulta
            Session.Item("DA_ID_IMAGEN_IMPRESION") = id_imagen
            Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = Matri_Doc_Visual(HttpContext.Current.Session.Item("WF_DOC_ACTUAL"))
            Me.ModalPopupExtenderimpre_post.Show()
            UpdatePaneliframe_post.Update()
        Catch ex As Exception
            scri.Showscripman(ex.Message, Updatepanel_boton_content)
        End Try
    End Sub


End Class
