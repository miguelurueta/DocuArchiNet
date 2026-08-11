Imports System.Web.SessionState
Imports GestionDocumental_Docuarchi.net.ClassGaCompartirDocumento
Imports System.Web.Http
Imports System.Web.Http.WebHost
Imports System.Web.Routing
Imports System.Web.Mvc
Imports System.Web.Optimization
Public Class Global_asax
    Inherits System.Web.HttpApplication

    Private Const ClaveIdSolicitudDiagnostico As String = "WF_REQUEST_ID"
    Private Const ClaveSesionDiagnostico As String = "WF_SESSION_ID"

    Private Function EsSolicitudDinamica() As Boolean
        Dim contexto As HttpContext = HttpContext.Current
        If contexto Is Nothing OrElse contexto.Request Is Nothing Then Return False
        Dim ruta As String = contexto.Request.Path
        Return ruta.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) OrElse
               ruta.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase) OrElse
               ruta.EndsWith(".asmx", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Sub RegistraSolicitudSesion(ByVal etapa As String)
        If Not EsSolicitudDinamica() Then Exit Sub
        Dim contexto As HttpContext = HttpContext.Current
        If contexto.Items(ClaveIdSolicitudDiagnostico) Is Nothing Then
            contexto.Items(ClaveIdSolicitudDiagnostico) = Guid.NewGuid().ToString("N").Substring(0, 8)
        End If
        Dim idSolicitud As String = contexto.Items(ClaveIdSolicitudDiagnostico).ToString()
        Dim idSesion As String = Convert.ToString(contexto.Items(ClaveSesionDiagnostico))
        Try
            If contexto.Session IsNot Nothing Then
                idSesion = contexto.Session.SessionID
                contexto.Items(ClaveSesionDiagnostico) = idSesion
            End If
        Catch ex As HttpException
            'La sesión todavía no está disponible en las primeras etapas del pipeline.
        End Try
        Dim transcurrido As Long = CLng((DateTime.Now - contexto.Timestamp).TotalMilliseconds)
        System.Diagnostics.Debug.WriteLine("WF_SESSION|" & etapa &
                                           "|Req=" & idSolicitud &
                                           "|Session=" & idSesion &
                                           "|" & contexto.Request.HttpMethod &
                                           "|" & contexto.Request.Path &
                                           "|" & transcurrido & " ms")
    End Sub

    Private Function EsSolicitudWebWorkflow() As Boolean
        Dim contexto As HttpContext = HttpContext.Current
        Return contexto IsNot Nothing AndAlso
               contexto.Request IsNot Nothing AndAlso
               contexto.Request.AppRelativeCurrentExecutionFilePath.EndsWith("/workflow/Webworkflow.aspx", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Sub RegistraEtapaPipeline(ByVal etapa As String)
        If Not EsSolicitudWebWorkflow() Then Exit Sub
        Dim transcurrido As Long = CLng((DateTime.Now - HttpContext.Current.Timestamp).TotalMilliseconds)
        System.Diagnostics.Debug.WriteLine("WF_PIPELINE|" & etapa & "|" & transcurrido & " ms desde inicio request")
    End Sub

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        Dim k
        Try
            AreaRegistration.RegisterAllAreas()
            RouteConfig.RegisterRoutes(RouteTable.Routes)
            Application("numero_visitas") += 1
        Catch ex As Exception
            k = ex.Message
        End Try
        'RouteTable.Routes.MapHttpRoute("DefaultApi",
        '                              "api/{controller}/{id}",
        '                              defaults:=New With {.id = System.Web.Http.RouteParameter.Optional})
        'RouteTable.Routes.MapHttpRoute("SolicitarDocumentoController",
        '                              "api/SolicitarDocumento/{id}",
        '                              defaults:=New With {.controller = "SolicitarDocumentoController", .id = System.Web.Http.RouteParameter.Optional})
        'RouteTable.Routes.MapHttpRoute("Controller_ccv",
        '                              "api/GetResponse_POST_service/{url}",
        '                              defaults:=New With {.controller = "Controller_ccv", .id = System.Web.Http.RouteParameter.Optional})

        ' Se desencadena al iniciar la aplicación
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Se desencadena al iniciar la sesión

        Session.Item("EXTENSION_ARCHIVO_ADJUNTA") = ""
        Session.Item("RUTA_TEMPORAL_ARCHIVO_ADJUNTA") = ""
        Session.Item("TIPO_ADJUNTA_STATE") = 1
        Session.Item("SESION_STATE") = ""
        Session.Item("DATA_SET_SESION") = vbObject
        Session.Item("DATA_SET_SESION_TRAZA_RAD") = vbObject
        Session.Add("wcp", "started")
        Session.Item("ctrl") = vbObject
        Session.Item("DETALLE_SESION") = ""
        Session.Item("ID_MODULO") = ""
        Session.Item("ID_EMPRESA") = ""
        Session.Item("NOMBRE_MODULO") = ""
        Session.Item("IP_SERVER_MODULO") = ""
        Session.Item("DB_NAME_MODULO") = ""
        Session.Item("USER_DBMS_MODULO") = ""
        Session.Item("PASW_DBMS_MODULO") = ""
        Session.Item("TYPE_DBMS_MODULO") = ""
        Session.Item("NUMERO_DBMS_CONEX") = ""
        Session.Item("ACTIVA_POOL_DBMS") = ""
        Session.Item("ENCRIPT_PASW") = "0"
        Session.Item("TIPOMODULO") = ""
        Session.Item("ACTIVA_WEB_SERVICE") = 0
        Session.Item("URL_WEB_SERVICE") = ""
        Session.Item("USER_WEB_SERVICE") = ""
        Session.Item("PASW_WEB_SERVICE") = ""
        Session.Item("SELECCIONTEMPORAL") = ""
        Session.Item("Id_Usuario_Workflow") = 0
        Session.Item("Id_actividad_Workflow") = 0
        Session.Item("Id_Ruta_Workflow") = 0
        Session.Item("Id_Grupo_Workflow") = 0
        Session.Item("Seleccion_Manual") = 0
        Session.Item("Seleccion_Automatico") = 0
        Session.Item("Actualizar_Imagen") = 0
        Session.Item("Datos_Externos") = 0
        Session.Item("Interactuar_Aplicaciones") = 0
        Session.Item("Interactuar_Mensageria") = 0
        Session.Item("Interactuar_Alertas") = 0
        Session.Item("Editar_Indice_Imagen") = 0
        Session.Item("Cambio_Ruta") = 0
        Session.Item("Interactuar_Anotaciones") = 0
        Session.Item("Interactuar_Pendiente") = 0
        Session.Item("CAMBIO_USUARIO") = 0
        Session.Item("RECUPERAR_TAREA") = 0
        Session.Item("UNIR_TAREA") = 0
        Session.Item("DUPLICAR_DOCUMENTO") = 0
        Session.Item("AGREGAR_DOCUMENTO_LIBRE") = 0
        Session.Item("AGREGAR_DOCUMENTO_TRD") = 0
        Session.Item("EDITAR_INDICE_WORKFLOW") = 0
        Session.Item("FIRMA_DIGITAL_DOCUMENTO_WF") = 0
        Session.Item("ELIMINA_FIRMA_DIGITAL_DOCUMENTO_WF") = 0
        Session.Item("AGREGAR_FIRMA") = 0
        Session.Item("AGREGAR_STAMP") = 0
        Session.Item("ADJUNTAR_IMAGENES_USUARIO") = 0
        Session.Item("ADJUNTAR_IMAGENES_PREDETERMINADA") = 0
        Session.Item("ADJUNTAR_SELLO") = 0
        Session.Item("IMPRIMIR_IMAGENES") = 0
        Session.Item("UTIL_SAVE_DOCUMENT") = 0
        Session.Item("UTIL_VISOR_EXPRESS") = 0             'Determina el visor express para workflow permiso usuario workflow
        Session.Item("UTIL_VISOR_EXPRESS_PRODUCION") = 0   'Determina el visor express para produción documental permiso usuario gestion
        Session.Item("UTIL_VISOR_EXPRESS_APROBACION") = 0  'Determina el visor express para aprobación documental permiso usuario gestion
        Session.Item("UTIL_VISOR_EXPRESS_EXPEDIENTE") = 0  'Determina el visor express para visor de expediente permiso usuario gestion
        Session.Item("UTIL_VISOR_EXPRESS_CONSULTAS") = 0   'Determina el visor express para visor de consultas permiso usuario gestion
        Session.Item("UTIL_VISOR_EXPRESS_DOCUARCHI") = 0   'Determina el visor express para visor de docuarchi permiso usuario docuarchi
        Session.Item("VALIDA_VISOR_EXPRES") = 0
        Session.Item("UTIL_GESTION_REASING_USER") = 0
        Session.Item("EJECUTAR_CODIGO_DEFAULT") = 0
        Session.Item("AGREGAR_DOCUMENTO_LIBRE") = 0
        Session.Item("AGREGAR_DOCUMENTO_TRD") = 0
        Session.Item("SELECIONA_ACTIVIDAD_AREA_WORKFLOW") = 0
        Session.Item("SELECIONA_ACTIVIDAD_USUARIO_WORKFLOW") = 0
        Session.Item("REASIGNA_TAREA_WORKFLOW") = 0
        Session.Item("DEVOLVER_TAREA_WORKFLOW") = 0
        Session.Item("EXPORTA_GABINETE_WORKFLOW") = 0
        Session.Item("MASTER_ELIMINA_GABINETE_WORKFLOW") = 0
        Session.Item("REASIGNA_TAREA_WORKFLOW_SII") = 0
        Session.Item("RESPUESTA_LIBRE") = 0
        Session.Item("COMPARTE_USUARIO_INTERNO") = 0
        Session.Item("COMPARTE_CORREO_ELECTRONICO") = 0
        Session.Item("ESTADO_PENDIENTE_APROBACION") = 0
        Session.Item("LISTA_ESTADO_PENDIENTE_APROBACION") = 0
        Session.Item("RESPUESTA_TRAMITE") = 0
        Session.Item("REASIGNA_RESPUESTA_TRAMITE") = 0
        Session.Item("CAMBIA_FLUJO_TRABAJO") = 0
        Session.Item("GESTION_FLUJOS_TRABAJO") = 0
        Session.Item("REVERSA_RESPUESTA") = 0
        Session.Item("UTIL_PAGINACION") = 1
        Session.Item("UTIL_ITER_PENDIENTE") = 1
        Session.Item("COPIA_ESTRUCTURA_PRODUCION") = 0
        Session.Item("COPIA_DOCUMENTO_EXPEDIENTE") = 0
        Session.Item("RELACIONA_EXPEDIENTE") = 0
        Session.Item("Id_Log_Usuario_Workfow") = 0
        Session.Item("Login_Usuario_Workfow") = ""
        Session.Item("ID_TAREA_SELECCIONDA") = "0"
        Session.Item("ID_TAREA_SELECCIONDA_ENLACE") = "0"
        Session.Item("Parametro_Intervalo_workflow") = -1
        Session.Item("Parametro_Intervalo_Alarma") = -1
        Session.Item("PAGINACION") = 1
        Session.Item("NUMEROACTIVIDADES") = 0
        Session.Item("NUMEROACTIVDADESUSUARIO") = 0
        Session.Item("PREACTUALIZAR") = ""
        Session.Item("ACTUALIZAR") = ""
        Session.Item("FINALIZAR") = ""
        Session.Item("INICIO") = ""
        Session.Item("PREINICIO") = ""
        Session.Item("TOMARTAREA") = ""
        Session.Item("ENLASE") = ""
        Session.Item("TIPOACTIVIDADWF") = ""
        Session.Item("PRETERMINARACTIVIAD") = ""
        Session.Item("TERMINARACTIVIDAD") = ""
        Session.Item("PEDIENTE") = ""
        Session.Item("ADJUNTOS") = ""
        Session.Item("PENDIENTE") = ""
        Session.Item("ADJUNTARIMAGENES") = ""
        Session.Item("CREARIMAGENES") = ""
        Session.Item("DEFAULTSCRIPT") = ""
        Session.Item("SESIONCOMPILAR") = ""
        Session.Item("PARAMETERCOMPILER") = ""
        Session.Item("CAMPOSELECCION") = ""
        Session.Item("ESTADOFILESERVER") = 1
        'Variables workflow configuracion
        Dim ob As Object
        'variables workflow docuarhi.visor
        Session.Item("WF_INTER_SELECION_DOCUMENTO") = ""
        Session.Item("WF_DETALLES_SESION") = ""
        Session.Item("WF_TAGSELECCION") = ""
        Session.Item("WF_ID_DOCUMENTO_SELECCIONADO") = 0
        Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_LISTA_RESPUESTA") = 0
        Session.Item("WF_GABINETE_SELECCIONADO") = ""
        Session.Item("WF_GABINETE_SELECCIONADO_LISTA_RESPUESTA") = ""
        Session.Item("WF_ID_GABINETE_SELECCIONADO") = ""
        Session.Item("WF_GABINETE_SELECCIONADO_CHAECHE") = ""
        Session.Item("WF_GABINETE_SELECCIONADO_CAMPOS_CHAECHE") = ""
        Session.Item("WF_ESTADO_ACTUALIZA_INDICE") = 0
        Session.Item("WF_ACTUALIZA_INDICE_BATCH_WF") = 0
        Session.Item("WF_RUTADOCUMENTO") = ""
        Session.Item("WF_RUTAWORKFLOW") = ""
        Session.Item("WF_IMAGE_HEIHG") = "30"
        Session.Item("WF_IMAGE_WITH") = "30"
        Session.Item("WF_IMAGE_HEIHG_SIZE") = "0"
        Session.Item("WF_IMAGE_WITH_SIZE") = "0"
        Session.Item("WF_MATRI_IMAGE") = ""
        Session.Item("WF_PAGINASELECCION") = ""
        Session.Item("WF_IMAGE_TEMPORAL") = ""
        Session.Item("WF_DOC_ACTUAL") = "1"
        Session.Item("WF_RUTA_FIRMA") = "../tempfirma/"
        Session.Item("WF_RUTA_TEMPO_WF") = "../Temp_Image/"
        Session.Item("WF_RUTA_TEMPO_FINAL") = ""
        Session.Item("WF_RUTA_TEMPO_RUTA_DESCARGA") = ""
        Session.Item("WF_RUTA_FIRMA_FINAL") = ""
        Session.Item("WF_RUTA_TEMPO_ESCANER") = ""
        Session.Item("WF_RUTA_TEMPO_ESCANER_FILE") = ""
        Session.Item("WF_RUTA_ERROR_ESCANER_FILE") = ""
        Session.Item("WF_RUTA_LINK") = ""
        Session.Item("WF_RUTA_DOCUMENTO_SELECCIONADO") = ""
        Session.Item("WF_NUMERO_TAREAS_SELECCIONADAS_W") = 0
        Session.Item("WF_MATRI_COPIA_ESTRUCTURA") = ob
        Session.Item("WF_MATRI_VINCULA_ESTRUCTURA") = ob
        Session.Item("WF_RADICADO_COPIA_ESTRUCTURA") = ""
        Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 0
        'Variables visor emergente workflow
        Session.Item("WF_TAGSELECCION_EMERGENTE") = ""
        Session.Item("WF_PAGINASELECCION_EMERGENTE") = ""
        Session.Item("WF_ID_DOCUMENTO_SELECCIONADO_EMERGENTE") = ""
        Session.Item("WF_DOC_ACTUAL_EMERGENTE") = "1"
        Session.Item("WF_IMAGE_HEIHG_EMERGENTE") = "30"
        Session.Item("WF_IMAGE_WITH_EMERGENTE") = "30"
        Session.Item("WF_IMAGE_HEIHG_SIZE_EMERGENTE") = "0"
        Session.Item("WF_IMAGE_WITH_SIZE_EMERGENTE") = "0"
        Session.Item("WF_MATRI_IMAGE_EMERGENTE") = ""
        Session.Item("WF_CHACHE_TABLA") = ""
        Session.Item("WF_URL_SELECCION") = ""
        Session.Item("WF_ERROR_RESPUESTA") = ""
        Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
        Session.Item("WF_TIPO_ADJUNTA") = ""
        'VARIABLES PERFIL DIAGRAMADOR 
        Session.Item("WF_IMPORTADOR_RUTA") = 0
        Session.Item("WF_CREA_FLUJO_TRABAJO") = 0
        Session.Item("WF_AGREGA_ACTIVIDAD") = 0
        Session.Item("WF_CONECTA_ACTIVIDAD") = 0
        Session.Item("WF_ELIMINA_ACTIVIDAD") = 0
        Session.Item("WF_ELIMINA_CONECTOR") = 0
        Session.Item("WF_DIAGRAMADOR") = 0
        Session.Item("WF_MIGRACION") = 0
        'VARIABLES GESTION TRAMITES
        Session.Item("WF_CAMPOS_LISTA_TRAMITE") = ""
        Session.Item("WF_CAMPOS_RADICADO_LISTA_TRAMITE") = ""
        Session.Item("WF_CAMPOS_BENEFICIARIO_LISTA_TRAMITE") = ""
        Session.Item("WF_CAMPOS_TRAMITE_LISTA_TRAMITE") = ""
        Session.Item("WF_ID_ACTIVIDAD") = 0
        Session.Item("WF_NUMERO_TRAMITE_ASIGNADO") = 0
        Session.Item("WF_ESTADO_TRAMITE") = ""
        Session.Item("WF_ESTADO_FLUJO_RUTA") = ""
        Session.Item("WF_ESTADO_RESPUESTA_TRAMITE_USUARIO") = ""
        Session.Item("SortExpression_wf_tramite") = ""
        Session.Item("SortDirection_wf_tramite") = "DESC"
        Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_HI") = ""
        Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_HI") = ""
        Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI") = ""
        Session.Item("Sort_matri_colum_compartido_hi") = ""
        Session.Item("SortExpression_compartido_hi") = ""
        Session.Item("SortDirection_compartido_hi") = "DESC"
        'variables modulo workflow actualizacion
        Session.Item("SortExpression_wf_tramite_WF") = ""
        Session.Item("SortDirection_wf_tramite_WF") = "DESC"
        Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO_HI_WF") = ""
        Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO_HI_WF") = ""
        Session.Item("WF_CAMPOS_LISTA_TRAMITE_HI_WF") = ""
        Session.Item("WF_ESTADO_EVALUA_SCRIPT_INICIO") = 0
        Dim matri_ob() As Object
        Session.Item("WF_MPARANT_LISTA_TRAMITE") = matri_ob
        Session.Item("WF_CACHE_CONSULTA_SCRIPT_INICIO") = ""
        Session.Item("WF_DATA_LISTA_CACHE_WF") = vbObject
        Session.Item("WF_DATA_LISTA_CACHE_WF_RESTORE") = vbObject
        Session.Item("WF_FILTRA_USUARIO_GRUPO_HI_WF") = "Todas"
        Session.Item("WF_CAMPOS_LISTA_TRAMITE_SCRIPT_HI_WF") = ""
        Session.Item("WF_TIPO_LISTA_TRAMITE_HI_WF") = ""
        Session.Item("WF_CAMPOS_LISTA_TAREA_PENDIENTE_HI_WF") = ""
        Session.Item("WF_MTRI_CAMPOS_LISTA_TAREA_PENDIENTE_HI_WF") = ""
        Session.Item("Sort_matri_colum_compartido_hi_WF") = ""
        Session.Item("CAHCE_DATA_SET_WF") = ob
        Session.Item("CAHCE_CONSULTA_INERT_WF") = ""
        Session.Item("SortExpression_compartido_hi_WF") = ""
        Session.Item("SortDirection_compartido_hi_WF") = "DESC"
        'MODULO DE REPORTES Dato_Sql_Consulta
        Session.Item("Dato_Sql_Consulta") = ""
        Session.Item("Dato_Sql_Consulta_ra") = ""
        Session.Item("Dato_Sql") = ""
        Session.Item("Dato_Sql_ra") = ""

        Session.Item("dat_gred_cahce_hi") = ob
        Session.Item("dat_gred_cahce") = vbObject
        Session.Item("dat_gred_cahce_restore") = vbObject
        'Varaibles de servidor general
        Session.Item("DA_GABINETE_IMPRESION") = ""
        Session.Item("DA_ID_IMAGEN_IMPRESION") = "0"
        Session.Item("ip_host_name") = ""
        Session.Item("id_registro_sesion_log") = "0"
        Session.Item("id_registro_sesion_log_wf") = "0"
        Session.Item("id_registro_sesion_log_ra") = "0"
        Session.Item("id_registro_sesion_log_gd") = "0"
        'Variables de docuarchi
        Session.Item("DA_ID_MODULO") = ""
        Session.Item("DA_ID_EMPRESA") = ""
        Session.Item("DA_NOMBRE_MODULO") = ""
        Session.Item("DA_IP_SERVER_MODULO") = ""
        Session.Item("DA_DB_NAME_MODULO") = ""
        Session.Item("DA_USER_DBMS_MODULO") = ""
        Session.Item("DA_PASW_DBMS_MODULO") = ""
        Session.Item("DA_TYPE_DBMS_MODULO") = ""
        Session.Item("DA_NUMERO_DBMS_CONEX") = ""
        Session.Item("DA_ACTIVA_POOL_DBMS") = ""
        Session.Item("DA_ENCRIPT_PASW") = "0"
        Session.Item("ESTADOFILESERVER") = "1"
        Session.Item("ID_USUARIO_DOCUARCHI") = "0"
        Session.Item("DA_GABINETE_CONSULTA") = ""
        Session.Item("DA_IMAGEN") = 0 'almacena el permiso de editar imagenes
        'Variables visor emergente workflow
        Session.Item("DA_TAGSELECCION_EMERGENTE") = ""
        Session.Item("DA_PAGINASELECCION_EMERGENTE") = ""
        Session.Item("DA_ID_DOCUMENTO_SELECCIONADO_EMERGENTE") = ""
        Session.Item("DA_DOC_ACTUAL_EMERGENTE") = "1"
        Session.Item("DA_IMAGE_HEIHG_EMERGENTE") = "30"
        Session.Item("DA_IMAGE_WITH_EMERGENTE") = "30"
        Session.Item("DA_IMAGE_HEIHG_SIZE_EMERGENTE") = "0"
        Session.Item("DA_IMAGE_WITH_SIZE_EMERGENTE") = "0"
        Session.Item("DA_MATRI_IMAGE_EMERGENTE") = ""
        Session.Item("DA_MATRI_IMAGE_EMERGENTE") = ""
        Session.Item("DA_CHACHE_TABLA") = ""
        Session.Item("DA_TEMPO_MIGRACION") = "../Temp_Radicacion/migracion/"
        'VARIABLES DE USUARIOS Y GRUPOS
        'Session.Item("Idusuario_Logueado As Integer") = 0
        Session.Item("DA_gruposusu") = 0 'guarda el usuario relacionado
        'Session.Item("DA_usuariologi") = ""  'almacena el usuario logeado
        Session.Item("DA_Login_Usuario") = ""
        Session.Item("DA_ESTADO_AUDITORIA_VISOR") = 0
        Session.Item("ID_DOCUMENTO_VISUALIZA") = -1 ' Variable que guarda el id para auditoria de impresion
        'VARIABLES DE PERMISOS LOCALES
        Session.Item("DA_Consuarchi1") = 0 'almacena el permiso de consulta
        Session.Item("DA_Almarchi") = 0 'almacena el permiso de almacenamiento
        Session.Item("DA_Importex1") = 0 'almacena el permiso de importacion de archivo
        Session.Item("DA_Actimp1") = 0 'almacena el permiso de importador
        Session.Item("DA_Expordata1") = 0 'almacena el permiso de exportar documentos desde la base de datos
        Session.Item("DA_eliminar1") = 0 'almacena el permiso de eliminar documetos
        Session.Item("DA_Imprimir1") = 0 'almacena el permiso de imprimir documentos
        Session.Item("DA_Guardar1") = 0 'almacena el permiso de guardar lo documentos
        Session.Item("DA_Edtitareg1") = 0 'almacena el permiso de editar registros
        Session.Item("DA_Editarimag1") = 0 'almacena el permiso de editar imagenes
        Session.Item("FIRMA_DIGITAL_DOCUMENTO_DA") = 0
        Session.Item("ELIMINA_FIRMA_DIGITAL_DOCUMENTO_DA") = 0
        Dim mtri_colum() As String = Nothing
        'VARIABLES CONSULTA
        Session.Item("GA_TIPO_CONSULTA_SOLICITUD_DA_CONSULTA") = 1
        Session.Item("GA_DATO_CONSULTA_SOLICITUD_PRODUCCION") = ""
        Session.Item("Sort_matri_colum_colaboracion") = mtri_colum
        Session.Item("SortExpression_da_consulta") = ""
        Session.Item("SortDirection_produccion_da_consulta") = ""
        Session.Item("GA_TIPO_CONSULTA_DOC_PRODUCCION") = 1
        Session.Item("GA_DATO_CONSULTA_DOC_PRODUCCION") = ""
        'VARIABLES PERMISOS GABINETES
        Session.Item("DA_consuarchi2") = 0
        Session.Item("DA_almarchi2") = 0
        Session.Item("DA_expordata2") = 0
        Session.Item("DA_eliminar2") = 0
        Session.Item("DA_imprimir2") = 0
        Session.Item("DA_guardar2") = 0
        Session.Item("DA_edtitareg2") = 0
        Session.Item("DA_edtitareg2") = 0
        Session.Item("DA_DESCARGA_EXTERNO") = 0
        Session.Item("DA_DESCARGA") = ""
        Session.Item("CONSULTA_IMAGEN") = 0
        Session.Item("ALMACENA_IMAGEN") = 0
        Session.Item("PREINDEX_IMAGEN") = 0
        Session.Item("EXPORT_IMAGE_GABINETE") = 0
        Session.Item("EXPORT_IMAGE_FYLESYSTEM") = 0
        Session.Item("EXDPORT_IMAGE_CARPETA") = 0
        Session.Item("EXPOR_IMAGE_CORREO") = 0
        Session.Item("ELIMINAR_REGISTRO") = 0
        Session.Item("MASTER_ELIMINAR_REGISTRO") = 0
        Session.Item("ADD_IMAGEN_REGISTRO") = 0
        Session.Item("EDITAR_REGISTRO") = 0
        Session.Item("EXPORTAR_LISTA_REGISTRO") = 0
        Session.Item("ACTUALIZA_BATCH_REGISTRO") = 0
        Session.Item("EDICION_IMAGEN") = 0
        Session.Item("IMPRI_IMAGEN") = 0
        Session.Item("GUARDAR_IMAGEN") = 0
        Session.Item("CROP_IMAGEN") = 0
        Session.Item("ADD_SELLO_IMAGEN") = 0
        Session.Item("ADD_FIRMA_DIGTIAL_IMAGEN") = 0
        Session.Item("ADD_ESTAMP_CRONOLOGICO_IMAGEN") = 0
        Session.Item("ADD_COPIA_ANOTACION_IMAGEN") = 0
        Session.Item("ADD_CAPO_WF_IMAGEN") = 0
        Session.Item("ADD_STAMP_RADICADO_IMAGEN") = 0
        Session.Item("ADD_BIPMAN_IMAGE") = 0
        Session.Item("ADD_OCR_IMAGE") = 0
        Session.Item("ADD_TRANSFORM_IMAGE") = 0
        Session.Item("ADD_DESKIEW_IMAGE") = 0
        Session.Item("DA_TIPOMODULO") = ""
        Session.Item("DA_ACTIVA_WEB_SERVICE") = 0
        Session.Item("DA_URL_WEB_SERVICE") = ""
        Session.Item("DA_USER_WEB_SERVICE") = ""
        Session.Item("DA_PASW_WEB_SERVICE") = ""
        'Varaibles de Radicacion
        Session.Item("RA_ID_MODULO") = ""
        Session.Item("RA_ID_EMPRESA") = ""
        Session.Item("RA_ID_EMPRESA_CONSULTA") = 0
        Session.Item("RA_ID_ORGANIGRAMA") = ""
        Session.Item("RA_NOMBRE_MODULO") = ""
        Session.Item("RA_IP_SERVER_MODULO") = ""
        Session.Item("RA_DB_NAME_MODULO") = ""
        Session.Item("RA_USER_DBMS_MODULO") = ""
        Session.Item("RA_PASW_DBMS_MODULO") = ""
        Session.Item("RA_TYPE_DBMS_MODULO") = ""
        Session.Item("RA_NUMERO_DBMS_CONEX") = ""
        Session.Item("RA_ACTIVA_POOL_DBMS") = ""
        Session.Item("RA_ENCRIPT_PASW") = "0"
        Session.Item("RA_ID_USUARIO") = 0
        Session.Item("RA_ID_PLANTILLA_RADICADO_SELECCIONADO") = 0
        Session.Item("RA_TIPO_PLANTILLA_RADICADO_SELECCIONADO") = 0
        Session.Item("RA_LOGIN_USER") = ""
        Session.Item("RA_PERMISO_RADICADO") = 0
        Session.Item("RA_PERMISO_ADICIONAR_DEST_INTERNO") = 0
        Session.Item("RA_PERMISO_CONSULTA") = 0
        Session.Item("RA_PERMISO_EDITA_RADICADO") = 0
        Session.Item("RA_PERMISO_ELIMINA_RADICADO") = 0
        Session.Item("RA_PERMISO_GENERAR_GUIA") = 0
        Session.Item("RA_PERMISO_IMPRIMIR_GUIA") = 0
        Session.Item("RA_PERMISO_ELIMINAR_GUIA") = 0
        Session.Item("RA_PERMISO_EDITAR_GUIA") = 0
        Session.Item("RA_PERMISO_GESTION_RESPUESTA") = 0
        Session.Item("RA_PERMISO_GESTION_CORRESPONDENCIA") = 0
        Session.Item("RA_PERMISO_REMISION_CORRESPONDENCIA_INTERNA") = 0
        Session.Item("RA_PERMISO_GESTION_CORRESPONDENCIA_SIMPLE") = 0
        Session.Item("RA_PERMISO_GESTION_REPORTES") = 0
        Session.Item("RA_MODULO_SELECCIONADO") = ""
        Session.Item("RA_RUTA_TEMPO") = "../Temp_Radicacion/"
        Session.Item("RA_RUTA_TEMPO_IMPRESION") = ""
        Session.Item("RA_RUTA_TEMPO_IMPRESION_DESCARGA_ROTULO") = ""
        Session.Item("RA_RUTA_IMPRESION_FINAL") = "OJO"
        Session.Item("RA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = ""
        Session.Item("RA_RUTA_IMPRESION_EXTENSION") = ""
        Session.Item("RA_RUTA_TEMPO_DESCARGA") = ""
        Session.Item("RA_RUTA_TEMPO_ESCANER") = ""
        Session.Item("RA_TIPO_IMPRESION") = "-1"
        Session.Item("RA_DATO_IMPRESION") = ""
        Session.Item("RA_DATO_VALIDACION") = ""
        Session.Item("RA_DATO_CONSULTA") = ""
        Session.Item("RA_DATO_CONSULTA_DATA_SET_CAHE") = Nothing
        Session.Item("RA_DATO_CONSULTA_RADICADO") = ""
        Session.Item("RA_VALIDACION_AGREGAR") = "0"
        Session.Item("RA_VALIDACION_ELIMINAR") = "0"
        Session.Item("RA_VALIDACION_EDITAR") = "0"
        Session.Item("RA_TIPOMODULO") = ""
        Session.Item("RA_ACTIVA_WEB_SERVICE") = 0
        Session.Item("RA_URL_WEB_SERVICE") = ""
        Session.Item("RA_USER_WEB_SERVICE") = ""
        Session.Item("RA_PASW_WEB_SERVICE") = ""
        Session.Item("RA_ID_DEST_EXTERNO") = "-1"
        Session.Item("RA_ID_RESPUESTA_SELECCIONADA") = "-1"
        Session.Item("RA_ID_RESPUESTA_SELECCIONADA_SOLICITUD_APROBACION") = "-1"
        Session.Item("RA_TIPO_CONSULTA_RADICADO") = "NORMAL"
        Session.Item("RA_CACHE_COSNSULTA_RADICADO") = ""
        Session.Item("RA_TIPO_CONSULTA_INTERNO_REMIT") = 1
        Session.Item("RA_DATO_CONSULTA_INTERNO_REMIT") = ""
        Session.Item("RA_TIPO_MODULO_GESTION_ENVIO_RADICADO") = 0
        Session.Item("RA_ID_REGISTRO_RADICADO") = 0
        Session.Item("RA_RADICADO_REGISTRO") = ""
        Session.Item("RA_RADICADO_CONSULTA_RESPUESTA_TODAS") = 0
        Session.Item("SortExpression_interno_remit") = ""
        Session.Item("SortDirection_interno_remit") = ""
        Session.Item("Sort_matri_colum_remit_interno") = ""
        Session.Item("RA_RADICADO_CONSULTA") = ""
        Session.Item("RA_PLANTILLA_CONSULTA") = ""
        Session.Item("RA_TIPO_PLANTILLA_CONSULTA") = ""
        Dim RA_ADJUNTO_RADIC_INERNOS() As String = Nothing
        Session.Item("RA_ADJUNTOS_RADICADO_INTERNO") = RA_ADJUNTO_RADIC_INERNOS
        Session.Item("ProdSelection") = ""
        'Variable Mensaje global
        Session.Item("OPCIONSELECION") = ""
        Session.Item("OPCIONSELECIONPENDIENTE") = ""
        Session.Item("SESIONITERCAMBIO") = ""
        Session.Item("SESIONITERCAMBIOVISOR") = ""
        Session.Item("SESIONITERCAMBIOEXPEDIENTE") = ""
        Session.Item("SESIONITERCAMBIOEXPEDIENTE_ASIG") = ""
        Session.Item("SESIONITERCAMBIOPLANTILLAVALIDACION") = ""
        Session.Item("SesionActiva") = "YES"
        'Variables gestión documental 
        Session.Item("GA_SQL_CACHE_CONSULTA_UNIDAD") = ""
        Session.Item("GA_OPCIONGESTION") = ""
        Session.Item("GA_IDUSUARIOGESTION") = 0
        Session.Item("GA_IDEMPRESA") = 0
        Session.Item("GA_LOGINUSUARIOGESTION") = ""
        Session.Item("GA_Manager_Produccion") = 0
        Session.Item("GA_Generar_Documento") = 0
        Session.Item("GA_Anular_documento") = 0
        Session.Item("GA_Eliminar_documento") = 0
        Session.Item("GA_Almacenar_Documento") = 0
        Session.Item("GA_Radicar_enviar_documento") = 0
        Session.Item("Radicar_enviar_documento_master_interno") = 0
        Session.Item("GA_MANAGER_CONFIGURACION") = 0
        Session.Item("GA_MANAGER_GESTION") = 0
        Session.Item("GESTION_FISICA") = 0
        Session.Item("GA_REGISTRA_UNIDAD_CONSERVACION") = 0
        Session.Item("GA_EDITA_UNIDAD_CONSERVACION") = 0
        Session.Item("GA_ELIMINA_UNIDAD_CONSERVACION") = 0
        Session.Item("GA_ARCHIVA_UNIDAD_CONSERVACION") = 0
        Session.Item("GA_APLICATRD_UNIDAD_CONSERVACION") = 0
        Session.Item("GA_TRANSLADO_UNIDAD_CONSERVACION") = 0
        Session.Item("GESTION_EXPEDIENTE") = 0
        Session.Item("GESTION_UNIDAD_CONSERVACION") = 0
        Session.Item("CONSULTA_EXPEDIENTE") = 0
        Session.Item("GA_REGISTRA_EXPEDIENTES") = 0
        Session.Item("GA_EDITA_EXPEDIENTES") = 0
        Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = ""
        Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_CONDICION") = ""
        Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_ADD_PAGINACION") = ""
        Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_AGREGADOS") = ""
        Session.Item("GA_ELIMINA_EXPEDIENTES") = 0
        Session.Item("GA_ARCHIVA_EXPEDIENTES") = 0
        Session.Item("GA_APLICATRD_EXPEDIENTES") = 0
        Session.Item("GA_TRANSLADO_EXPEDIENTES") = 0
        Session.Item("GA_REGISTRA_DOCUMENTOS") = 0
        Session.Item("GA_EDITA_DOCUMENTOS") = 0
        Session.Item("GA_ELIMINA_DOCUMENTOS") = 0
        Session.Item("GA_ARCHIVA_DOCUMENTOS") = 0
        Session.Item("GA_APLICATRD_DOCUMENTOS") = 0
        Session.Item("GA_TRANSLADO_DOCUMENTOS") = 0
        Session.Item("GA_PRESTAMO_ARCHIVO") = 0
        Session.Item("GA_CLASIFICA_DOCUMENTOS") = 0
        Session.Item("GA_ASIGNA_UNIDAD_CONSERVACION_DOCUMENTOS") = 0
        Session.Item("GA_ASIGNA_EXPEDIENTE_DOCUMENTOS") = 0
        Session.Item("GA_ESTADO_ARCHIVA_DOCUMENTO") = 0
        Session.Item("GA_SELECCIONA_CLASE_DOCUMENTOS") = 0
        Session.Item("GA_CLASIFICA_UNIDAD_CONSERVACION") = 0
        Session.Item("GA_CLASIFICA_EXPEDIENTES") = 0
        Session.Item("GA_ADMINISTRACION_ORGANICA") = 0
        Session.Item("GA_ADMINISTRACION_INSTRUMENTO") = 0
        Session.Item("GA_CONSULTA_TABLA_RETENCION") = 0
        Session.Item("GA_CONSULTA_CUADRO_CLASIFICACION") = 0
        Session.Item("FIRMA_DIGITAL_DOCUMENTO_GD") = 0
        Session.Item("ELIMINA_FIRMA_DIGITAL_DOCUMENTO_GD") = 0
        Session.Item("GA_ADMINISTRACION_TRD") = 0
        Session.Item("GA_ADMINISTRACION_TVD") = 0
        Session.Item("GA_ADMINISTRACION_CCD") = 0
        Session.Item("GA_ADMINISTRACION_ESTRUCTURA_ARCHIVO") = 0
        Session.Item("PRODUCCION_MANAGER") = 0
        Session.Item("GA_TIPOMODULO") = ""
        Session.Item("GA_ACTIVA_WEB_SERVICE") = 0
        Session.Item("GA_URL_WEB_SERVICE") = ""
        Session.Item("GA_USER_WEB_SERVICE") = ""
        Session.Item("GA_PASW_WEB_SERVICE") = ""
        Session.Item("GA_RUTA_TEMPO") = "../Temp_Gestion/"
        Session.Item("GA_RUTA_FIRMA_GESTION") = "../Temp_Gestion/"
        Session.Item("GA_RUTA_TEMP_GESTION") = ""
        Session.Item("GA_RUTA_TEMP_GESTION_URL") = "/Temp_Gestion/"
        Session.Item("GA_RUTA_TEMPO_IMPRESION") = ""
        Session.Item("GA_RUTA_IMPRESION_FINAL") = ""
        Session.Item("GA_RUTA_IMPRESION_FINAL_DOC_ACTUAL") = ""
        Session.Item("GA_RUTA_IMPRESION_EXTENSION") = ""
        Session.Item("GA_RUTA_TEMPO_DESCARGA") = ""
        Session.Item("GA_RUTA_TEMPO_ESCANER") = ""
        Session.Item("GA_INTERCAMBIO_NOTA_APROBACION") = ""
        Session.Item("GA_INTERCAMBIO_TIPO_NOTA_APROBACION") = ""
        Session.Item("PU_TRAZABILIDAD") = ""
        Session.Item("GA_CODIGO_UNIDAD_CONTENEDORA") = ""
        Session.Item("GA_ID_UNIDAD_CONTENEDORA") = 0
        Session.Item("GA_TIPO_MODULO_RESPUESTA") = 1
        Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_APROBAR_USUARIO") = 0
        Session.Item("GA_NUMERO_SOLICITUDES_PENDIENTES_POR_APROBACION") = 0
        'ESTRUCTURA DOCUMENTOS COMPARTIDOS
        Dim stru_documento_compartido() As stru_documentos_compartidos = Nothing
        Session.Item("GA_STRU_DOCUMENTO_COMPARTIDO") = stru_documento_compartido
        Session.Item("GA_STRU_DOCUMENTO_RADICADO") = ""
        Session.Item("GA_STRU_DOCUMENTO_TIPO_COMPARTIDO") = ""
        Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO") = 0
        Session.Item("SortDirection_compartido") = ""
        Session.Item("SortExpression_compartido") = ""

        Session.Item("Sort_matri_colum_compartido") = mtri_colum
        Session.Item("GA_STRU_DOCUMENTO_NUMERO_COMPARTIDO") = 0
        Session.Item("GA_STRU_DOCUMENTO_PENDIENTE_REVISION") = 0
        Session.Item("GA_STRU_ID_DOCUMENTO_COMPARTIDO_COLABORACION") = 0 'Guarda el id general del documento compartido para listado de colaboración
        Session.Item("GA_STRU_RADICADO_COLABORACION") = "" 'Guarda del radicado seleccionado para listar los documentos de colaboración
        Session.Item("GA_STRU_TIPO_LISTADO_DOC_COLABORACION") = "" 'Gurda el tipo de listado de documentos de colaboracion (RELACIONADO A RADICADO-RELACIONADO ID DOCUMENTO)
        Session.Item("GA_TIPO_CONSULTA_SOLICITUD_COLABORACION") = 1
        Session.Item("GA_DATO_CONSULTA_SOLICITUD_COLABORACION") = ""
        Session.Item("Sort_matri_colum_colaboracion") = mtri_colum
        Session.Item("SortExpression_colaboracion") = ""
        Session.Item("SortDirection_colaboracion") = ""
        Session.Item("GA_TIPO_CONSULTA_DOC_COMPARTIDO") = 1
        Session.Item("GA_DATO_CONSULTA_DOC_COMPARTIDO") = ""
        'ESTRUCTURA SOLICITUDES DE APROBACION
        Session.Item("GA_STRU_ESTADO_NUEVA_SOLICITUD_APROBACION") = ""
        Session.Item("GA_TIPO_CONSULTA_SOLICITUD_APROBACION") = 1
        Session.Item("GA_DATO_CONSULTA_SOLICITUD_APROBACION") = ""
        Session.Item("Sort_matri_colum_aprobacion") = ""
        Session.Item("SortExpression_aprobacion") = ""
        Session.Item("SortDirection_aprobacion") = ""
        'PARAMETROS CERTIFICACION ELECTRONICA
        Session.Item("RUTA_ARCHIVO_CERTIFICACION") = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("GA_RUTA_TEMPO")) & "certdigital\GemBoxSampleExplorer.pfx"
        Session.Item("CLAVE_ARCHIVO_CERTIFICACION") = "GemBoxPassword"
        Session.Item("EMPRESA_GESTION") = ""
        Session.Item("TIPO_VISOR_PDF") = ""
        Session.Item("PQRS_CODIGO_PLANTILLA_RADICADO") = 0
        Session.Item("PQRS_NOMBRE_PLANTILLA_RADICADO") = ""
        Session.Item("PQRS_CODIGO_PLANTILLA_VALIDACION") = 0
        Session.Item("PQRS_NOMBRE_PLANTILLA_VALIDACION") = ""
        Session.Item("PQRS_CODIGO_SCRIPT_PLANTILLA") = 0
        Session.Item("PQRS_CAMPO_NIT_PLANTILLA") = ""
        Session.Item("PQRS_CAMPO_ANUALIDAD_PLANTILLA") = ""
        Session.Item("PQRS_CAMPO_IDEXT_PLANTILLA") = ""
        Session.Item("PQRS_TIPO_PQRS") = ""
        Session.Item("PQRS_ID_USUARIO_PQRS") = 0
        Session.Item("CC_DOC_ACTUAL_EMERGENTE") = "1"
        Session.Item("CC_IMAGE_HEIHG_EMERGENTE") = "30"
        Session.Item("CC_IMAGE_WITH_EMERGENTE") = "30"
        Session.Item("CC_IMAGE_HEIHG_SIZE_EMERGENTE") = "0"
        Session.Item("CC_IMAGE_WITH_SIZE_EMERGENTE") = "0"
        Session.Item("CC_MATRI_IMAGE_EMERGENTE") = ""
        Session.Item("CC_MATRI_IMAGE_EMERGENTE") = ""
        Session.Item("CC_SESIONITERCAMBIOVISOR") = ""
        '--------------------------------------------
        'Variables diagramador
        '--------------------------------------------
        Session.Item("DR_RUTASELECCION_ID_CONECTOR") = 0
        Session.Item("DR_RUTASELECCION_DIAGRAMA") = ""
        Session.Item("DR_RUTASELECCION_FLUJO") = ""
        Session.Item("DR_ID_RUTA_SELECION_FLUJO") = 0
        Session.Item("DR_ID_FLUJO_SELECCIONADO") = 0
        Session.Item("DR_FLUJO_SELECCIONADO") = ""
        Session.Item("DR_ID_FLUJO_SELECCIONADO_TEMPORAL") = 0
        Session.Item("DR_FLUJO_SELECCIONADO_TEMPORAL") = ""
        Session.Item("DR_TIPO_ACTIVIDAD_AGREGAR") = ""
        Session.Item("DR_ID_FLUJO_TRABAJO") = 0
        Session.Item("DR_ID_ACTIVIDAD_FLUJO_TRABAJO") = 0
        Session.Item("DR_ID_USUARIO_WORKFLOW_FLUJO_TRABAJO") = 0
        Session.Item("DR_ID_TAREA_FLUJO_TRABAJO") = 0
        Session.Item("DR_RADICADO_FLUJO_TRABAJO") = ""
        Session.Item("RU_ID_TAREA_RUTA_TRABAJO") = 0
        Session.Item("RU_RADICADO_RUTA_TRABAJO") = ""
        Session.Item("DR_ID_FLUJO_TRABAJO_SELECCION") = 0
        Dim MatriScriptUusario() As String = {"INICIO", "PREINICIO", "TOMARTAREA", "PRETERMINARACTIVIAD", _
        "TERMINARACTIVIDAD", "PENDIENTE", "ADJUNTOS", "ADJUNTARIMAGENES", "CREARIMAGENES", "DEFAULTSCRIPT"}
        Session.Item("MatriScriptUusario") = MatriScriptUusario
        Dim MatriScriptEnlace() As String = {"INICIO", "PREINICIO", "TOMARTAREA", "ENLASE", "PRETERMINARACTIVIAD", _
        "TERMINARACTIVIDAD", "PENDIENTE", "ADJUNTOS", "ADJUNTARIMAGENES", "CREARIMAGENES", "DEFAULTSCRIPT"}
        Session.Item("MatriScriptEnlace") = MatriScriptEnlace
        Dim MatriScriptUusarioWEB() As String = {"INICIO-WEB", "PREINICIO-WEB", "TOMARTAREA-WEB", "PRETERMINARACTIVIAD-WEB", _
        "TERMINARACTIVIDAD-WEB", "PENDIENTE-WEB", "ADJUNTOS-WEB", "ADJUNTARIMAGENES-WEB", "CREARIMAGENES-WEB", "DEFAULTSCRIPT-WEB"}
        Session.Item("MatriScriptUusarioWEB") = MatriScriptUusarioWEB
        Dim MatriScriptEnlaseWEB() As String = {"INICIO-WEB", "PREINICIO-WEB", "TOMARTAREA-WEB", "ENLASE-WEB", "PRETERMINARACTIVIAD-WEB", _
        "TERMINARACTIVIDAD-WEB", "PENDIENTE-WEB", "ADJUNTOS-WEB", "ADJUNTARIMAGENES-WEB", "CREARIMAGENES-WEB", "DEFAULTSCRIPT-WEB"}
        Session.Item("MatriScriptEnlaseWEB") = MatriScriptEnlaseWEB
        Dim MatriScriptSistema() As String = {"PREACTUALIZAR", "ACTUALIZAR", "FINALIZAR", "ADJUNTARIMAGENES_SISTEMA", "CREARIMAGENES_SISTEMA", "DEFAULTSCRIPT_SISTEMA"}
        Session.Item("MatriScriptSistema") = MatriScriptSistema
        '---------------------------------------------
        'VARIABLES DIGITALIZACIÓN
        '---------------------------------------------
        Session.Item("DG_TIPODIGITALIZACION") = ""
        Session.Item("DG_ID_TRAMITE") = 0
        Session.Item("DG_TIPO_TRAMITE") = ""
        Session.Item("DG_NOMBRE_TRAMITE") = ""
        Session.Item("DG_ID_GABINETE") = 0
        Session.Item("DG_NOMBRE_GABINETE") = ""
        Session.Item("DG_RADICADO") = ""
        Session.Item("DG_LISTA_CHEQUEO") = -1
        Session.Item("DG_RIPO_DOCUMENTAL_LISTA_CHEQUEO") = ""
        Session.Item("DG_ID_CONFIG_DIGITALIZACION") = -1
        Session.Item("DG_SELECION_TREE") = ""
        Session.Item("DG_ID_EXPEDIENTE") = 0
        Session.Item("DG_SELECCION_TIPODOCUMENTO_EXPEDIENTE") = ""
        Session.Item("DG_NOMBRE_DOCUMENTO") = ""
        Session.Item("DG_ID_RUTA") = 0
        Session.Item("DG_ID_TAREA") = 0
        Session.Item("DG_ESTADO_VENTA") = 0
        Session.Item("DG_TRAMITE_DIGITAIZACION") = -1
        '--------------------------------------------
        'VARIABLES MANEJO DE CARACTERES 
        '--------------------------------------------
        Session.Item("DG_CDCARACTERES") = New CDcarateres
        '--------------------------------------------
        'VARIABLES PRODUCCIÓN DOCUMENTAL
        '--------------------------------------------
        Session.Item("PG_SELECCION_ID_ARCHIVO") = ""
        Session.Item("PG_SELECCION_ID_EXPEIDENTE") = 0
        Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = ""
        Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION_TEXT") = ""
        Session.Item("PG_SELECCION_ID_NIVEL") = 0
        Dim stru_file_system() As stru_file_system = Nothing
        Session.Item("PG_STRU_IMAGENES_DOWNLOAD_PRODUCION") = stru_file_system
        Dim tre As TreeNode
        Session.Item("PG_CORTAR_PEGAR") = True
        Session.Item("PG_CORTAR_PEGAR_ARCHIVO") = ""
        Session.Item("GA_TIPO_CONSULTA_SOLICITUD_PRODUCCION") = 1
        Session.Item("GA_DATO_CONSULTA_SOLICITUD_PRODUCCION") = ""
        Session.Item("Sort_matri_colum_colaboracion") = mtri_colum
        Session.Item("SortExpression_produccion") = ""
        Session.Item("SortDirection_produccion") = ""
        Session.Item("GA_TIPO_CONSULTA_DOC_PRODUCCION") = 1
        Session.Item("GA_DATO_CONSULTA_DOC_PRODUCCION") = ""
        Session.Item("GA_CONSECUTVO_DOC_PRODUCCION") = 0
        Session.Item("GA_ROMULARIO_WEB") = ob
        '--------------------------------------------
        'VARIABLES ORGANIGRAMA TRD
        '--------------------------------------------
        Session.Item("ORG_ID_ORGANIGRAMA_ACTIVO") = 0
        Session.Item("ORG_ID_AREA_ACTIVA") = 0
        '--------------------------------------------
        'VARIABLES TRD
        '--------------------------------------------
        Session.Item("TRD_CONTADOR") = -1
        Session.Item("TRD_APLICA_ID_SERIE") = -1
        Session.Item("TRD_APLICA_ID_SUB_SERIE") = -1
        Session.Item("TRD_APLICA_EXPEDIENTE") = ""
        Dim stru_serie_() As Serie_documental = Nothing
        Session.Item("TR_SERIES_CACHE") = stru_serie_
        '--------------------------------------------
        'PUBLICO
        '--------------------------------------------
        Session.Item("Sort_matri_colum_publico") = mtri_colum
        Session.Item("SortExpression_publico") = ""
        Session.Item("SortDirection_publico") = ""
        Session.Item("GA_TIPO_CONSULTA_PUBLICO") = ""
        Session.Item("GA_DATO_CONSULTA_PUBLICO") = ""
        Session.Item("GA_DATO_CONSULTA_PUBLICO_BJEC") = ob
        Session.Item("Sort_matri_colum_solicitudes_apro") = mtri_colum
        Session.Item("SortExpression_solicitudes_apro") = ""
        Session.Item("SortDirection_solicitudes_apro") = ""
        Session.Item("Tipo_consulta_solicitudes_apro") = ""
        Session.Item("Tipo_dato_solicitudes_apro") = ""
        '----------------------------------------------
        'VISOR
        '----------------------------------------------
        Session.Item("TIPOVISOR_INDICE_EXPEDIENTE") = ""
        Session.Item("ZOON_VISOR_WEB_TIF") = "0"
        '---------------------------------------------
        'VARIABLES CUADRO CLASFICACION
        '---------------------------------------------
        Session.Item("Sort_matri_colum_expe_clasificacion") = mtri_colum
        Session.Item("SortExpression_expe_clasificacion") = ""
        Session.Item("SortDirection_expe_clasificacion") = ""
        Session.Item("GA_TIPO_CONSULTA_expe_clasificacion") = 0
        Session.Item("GA_DATO_CONSULTA_expe_clasificacion") = ""
        Session.Item("GA_LIMIT_CONSULTA_expe_clasificacion") = ""
        Session.Item("nivel_expe_clasificacion") = ""
        Session.Item("serie_expe_clasificacion") = 0
        Session.Item("Sort_matri_colum_doc_clasificacion") = mtri_colum
        Session.Item("SortExpression_doc_clasificacion") = ""
        Session.Item("SortDirection_doc_clasificacion") = ""
        Session.Item("GA_TIPO_CONSULTA_doc_clasificacion") = ""
        Session.Item("GA_DATO_CONSULTA_doc_clasificacion") = ""
        Session.Item("GA_DATO_CONSULTA_doc_id_unidad_clasificacion") = 0
        '------------------------------------------------
        'VARIABLES AUTORIZACION
        '------------------------------------------------
        'VARIABLES CONSULTA
        Session.Item("GA_TIPO_CONSULTA_LISTA_AUTORIZA") = 1
        Session.Item("GA_DATO_CONSULTA_LISTA_AUTORIZA") = ""
        Session.Item("Sort_matri_colum_lista_autoriza") = mtri_colum
        Session.Item("SortExpression_lista_autoriza") = ""
        Session.Item("SortDirection_produccion_lista_autoriza") = ""
        '--------------------------------------------------
        'VARIABLES DE CONFIRMACIÓN DE CORREO ELECTRÓNICO
        '--------------------------------------------------
        Session.Item("GA_ID_RESPUESTA_CONFIRMACION") = ""
        Session.Item("GA_RUTA_TEMPORAL_DESCARGA_ARCHIVO_CORREO") = ""

        '---------------------------------------------------
        'VARIABLES INDICE ELECTRONICO
        '---------------------------------------------------
        Session.Item("CERT_ID_EXPEDIENTE_INDICE") = 0
        Session.Item("CERT_TIPO_CONSULTA_EXPEDIENTE_INDICE") = 0
        Session.Item("SortExpression_expediente_indice") = ""
        Session.Item("SortDirection_expediente_indice") = ""
        Session.Item("dat_gred_cahce_CERT") = ob
        '---------------------------------------------------
        'VARIABLES SISTEMA META DATOS
        '----------------------------------------------------
        Session.Item("ID_SISTEMA_META_DATOS") = 0
        Session.Item("ID_IMAGEN_PRODUCCION_SISTEMA_META_DATOS") = 0
        Session.Item("GABINETE_SISTEMA_META_DATOS") = ""
        Session.Item("NOMBRE_SISTEMA_META_DATOS") = ""
        '---------------------------------------------------
        'VARIABLES INTEGRACION SII
        '---------------------------------------------------
        Session.Item("UTIL_SII_REGISTRO_TAREA_RUTA") = 0   'Permiso para registrar tarea ruta integracion SII
        Session.Item("UTIL_SII_REGISTRO_TAREA_FLUJO") = 0  'Permiso para registrar tarea flujo integracion SII
        Session.Item("UTIL_SII_GESTION_TAREA_RUE") = 0     'Permiso para gestionar tarea rue integracion SII
        Session.Item("UTIL_SII_GESTION_TAREA_VIRTUAL") = 0 'Permiso para gestionar tarea rue integracion SII
        Session.Item("SII_RECIBO") = ""
        Session.Item("SII_COD_BARRAS") = ""
        '---------------------------------------------------
        'VARIABLES PERMISOS MIGRACION DOCUMENTOS
        '---------------------------------------------------
        Session.Item("UTIL_MODULO_CONSULTA_MIGRA_FORMATO_ARCHIVO") = 0
        Session.Item("UTIL_MODULO_MIGRA_FORMATO_ARCHIVO") = 0
        Session.Item("UTIL_MIGRA_FORMATO_ARCHIVO") = 0
        Session.Item("UTIL_MIGRA_LOAD_FORMATO_ARCHIVO") = 0
        Session.Item("UTIL_MIGRA_REMPLAZA_VERSION_DOCUMENTO") = 0
        '---------------------------------------------------------------------
        'VARIABLES PERMISOS  RESTAURA DOCUMENTO EN GABINETE
        'DESDE LA INTERFACE DE VERSION DE DOCUMENTOS
        '---------------------------------------------------------------------
        Session.Item("UTIL_VER_MIG_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0  'Migracion -1
        Session.Item("UTIL_VER_WF_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0   'Workflow  -2  OOJOOO
        Session.Item("UTIL_VER_PR_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0   'Producion documental -3
        Session.Item("UTIL_VER_DA_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0   'Docuarchi -4
        Session.Item("UTIL_VER_RA_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0   'radicacion -5
        Session.Item("UTIL_VER_COR_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0  'gestión correspodencia -6
        Session.Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = 0  'gestión correspodencia -6

        '---------------------------------------------------------------------
        'VARIABLES PERMISOS  ELIMINA VERSION DOCUMENTO DESDE
        'LA INTERFACE VERSION DE DOCUMENTOS
        '---------------------------------------------------------------------
        Session.Item("UTIL_VER_MIG_ELIMINA_VERSION_DOCUMENTO") = 0      'Migracion -1
        Session.Item("UTIL_VER_WF_ELIMINA_VERSION_DOCUMENTO") = 0       'Workflow  -2  
        Session.Item("UTIL_VER_PR_ELIMINA_VERSION_DOCUMENTO") = 0       'Producion documental -3
        Session.Item("UTIL_VER_DA_ELIMINA_VERSION_DOCUMENTO") = 0       'Docuarchi -4
        Session.Item("UTIL_VER_RA_ELIMINA_VERSION_DOCUMENTO") = 0       'radicacion -5
        Session.Item("UTIL_VER_COR_ELIMINA_VERSION_DOCUMENTO") = 0      'gestión correspodencia -6
        Session.Item("UTIL_VER_CON_MIGRA_ELIMINA_VERSION_DOCUMENTO") = 0 'consulta migración correspodencia -6
        Session.Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = 0   'Master elimina versión sin importar si el el propietario

        '--------------------------------------------------------------------
        'VARIABLES PERMISOS  REMPLAZA VERSION DOCUMENTO DOCUMENTO FUERA DE LA 
        'INTERFACE DE VERSIONES PARA LOS COMPONENTES QUE UTILICEN  LA FUNCION
        'Adjunta_version_documento con la opcion de remplazar documento
        '--------------------------------------------------------------------
        Session.Item("UTIL_VER_MIG_REMPLAZA_VERSION_DOCUMENTO") = 0        'Migracion -1
        Session.Item("UTIL_VER_WF_REMPLAZA_VERSION_DOCUMENTO") = 0         'Workflow  -2  OJO
        Session.Item("UTIL_VER_WF_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0  'Master elimina versión sin importar si el el propietario modulo workflow
        Session.Item("UTIL_VER_PR_REMPLAZA_VERSION_DOCUMENTO") = 0         'Producion documental -3
        Session.Item("UTIL_VER_DA_REMPLAZA_VERSION_DOCUMENTO") = 0         'Docuarchi -4
        Session.Item("UTIL_VER_RA_REMPLAZA_VERSION_DOCUMENTO") = 0         'radicacion -5
        Session.Item("UTIL_VER_COR_REMPLAZA_VERSION_DOCUMENTO") = 0        'radicacion -5
        Session.Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = 0     'Master elimina versión sin importar si el el propietario

        '-------------------------------------------------------------------
        'VARIABLE ELIMINACIÓN DE DOCUMENTOS  FIRMADOS Y LEVANTAMIENTO DE 
        'REGISTRO DE FIRMA
        '-------------------------------------------------------------------
        Session.Item("UTIL_FIR_MASTER_ELIMINA_DOCUMENTO") = 0
        Session.Item("UTIL_FIR_MASTER_ELIMINA_REGISTRO_FIRMA") = 0

        '-------------------------------------------------------------------
        'VARIABLES PERMISO DE AUTOVINCULACION DE DOCUMENTOS A EXEPDEINTES
        'DESDE EL MODULO DE MIGRACION
        '-------------------------------------------------------------------
        Session.Item("UTIL_MIG_AUTO_VINCULA_DOC_EXPEDIENTE") = 0
        '------------------------------------------------------------------
        '------------------------------------------------------
        'VARIABLES DE CAMBIO DE TIPOLOGIA DOCUMENTAL MIGRACION
        '------------------------------------------------------
        Session.Item("UTIL_MIGRA_UPDATE_TIPOLOGIA") = 0
        '------------------------------------------------------
        'VARIABLE ACTUALIZACION DE INDICE BATCH PARA MIGRACION
        '------------------------------------------------------
        Session.Item("UTIL_MIGRA_UPDATE_INDICE_BATCH") = 0
        '---------------------------------------------------
        'VARIABLES VISOR VERSION TIF-BMP
        '---------------------------------------------------
        Session.Item("VER_IMAGE_TEMPORAL_EMERGENTE") = ""
        Session.Item("VER_MATRI_IMAGE_EMERGENTE") = ""
        Session.Item("VER_DOC_ACTUAL_EMERGENTE") = "1"
        Session.Item("VER_GABINETE_CONSULTA") = ""
        Session.Item("VER_IMAGEN") = 0
        Session.Item("VER_ID_REGISTRO_VERSION") = 0
        Session.Item("VER_MATRI_IMAGE_EMERGENTE") = ""
        Session.Item("VER_IMAGE_HEIHG_EMERGENTE") = vbObject
        Session.Item("VER_IMAGE_WITH_EMERGENTE") = vbObject
        Session.Item("VER_IMAGE_HEIHG_SIZE_EMERGENTE") = vbObject
        Session.Item("VER_IMAGE_WITH_SIZE_EMERGENTE") = vbObject
        Session.Item("VER_ZOON_VISOR_WEB_TIF") = "0"

        '-----------------------------------------------------------
        'VARIABLES GESTION CORRESPONDNCIA
        '----------------------------------------------------------
        Session.Item("UTILGCOROptionHCarchivaTramite") = 0
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Se desencadena al comienzo de cada solicitud
        RegistraSolicitudSesion("START")
        RegistraEtapaPipeline("BeginRequest")
        System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("es-MX")
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Se desencadena al intentar autenticar el uso
        RegistraEtapaPipeline("AuthenticateRequest")
    End Sub

    Sub Application_AuthorizeRequest(ByVal sender As Object, ByVal e As EventArgs)
        RegistraEtapaPipeline("AuthorizeRequest")
    End Sub

    Sub Application_AcquireRequestState(ByVal sender As Object, ByVal e As EventArgs)
        RegistraEtapaPipeline("AcquireRequestState")
    End Sub

    Sub Application_PostAcquireRequestState(ByVal sender As Object, ByVal e As EventArgs)
        RegistraSolicitudSesion("ACQUIRED")
        RegistraEtapaPipeline("PostAcquireRequestState")
    End Sub

    Sub Application_ReleaseRequestState(ByVal sender As Object, ByVal e As EventArgs)
        RegistraSolicitudSesion("RELEASING")
    End Sub

    Sub Application_PreRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)
        RegistraEtapaPipeline("PreRequestHandlerExecute")
    End Sub

    Sub Application_PostRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)
        RegistraEtapaPipeline("PostRequestHandlerExecute")
    End Sub

    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)
        RegistraEtapaPipeline("EndRequest")
        RegistraSolicitudSesion("END")
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Se desencadena cuando se produce un error
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Se desencadena cuando finaliza la sesión
        'System.Web.Security.FormsAuthentication.SignOut()
        'System.Web.Security.FormsAuthentication.RedirectToLoginPage()
        Session.Item("SesionActiva") = ""
        'Session.Remove(Session.SessionID)

    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Se desencadena cuando finaliza la aplicación
    End Sub
End Class
