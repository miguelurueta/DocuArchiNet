Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports GestionDocumental_Docuarchi.net.conect
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceGaExpediente
    Inherits System.Web.Services.WebService
    Public Class str_expediente_service
        Public ID_EXPEDIENTE As String
        Public CODIGO_UNICO As String
        Public ESTADO_CODIGO_UNICO As String
        Public ID_EMPRESA_GESTION As String
        Public FECHA_INICIAL_EXPEDICION As String
        Public FECHA_FINAL_TERMINACION As String
        Public RANGO_EXTREMO_INICIAL As String
        Public RANGO_EXTREMO_FINAL As String
        Public TEMA As String
        Public REGISTRO_ORGANIGRAMA As String
        Public NOMBRE_AREA As String
        Public NOMBRE_SERIE As String
        Public NOMBRE_SUBSERIE As String
        Public TIPO_EXPEDIENTE As String
        Public FOLIO_DIGITALIZADO As String
        Public FOLIO_FISICO As String
        Public FOLIO_ELECTRONICO As String
        Public ASUNTO As String
        Public TIPO_UNIDAD_DOCUMENTAL As String
        Public OBSERVACION_EXPEDIENTE As String
        Public NOMBRE_SUB_AREA As String
        Public NOMBRE_CICLO_ARCHIVO As String
        Public NOMBRE_FONDO As String
        Public NOMBRE_SOLICITANTE As String
        Public IDENTIFICACION_SOLICITANTE As String
        Public RESPONSABLE_EXPEDIENTE As String
        Public IDENFICACION_RESPONSABLE As String
        Public ALEAS_EXPEDIENTE As String
        Public EXPEDIENTE_PADRE As String
        Public ID_INSTRUMENTO As String
        Public GABINETE_PRODUCION As String
        Public ID_NIVEL_PADRE As String
        Public ID_REGISTRO_RELACION As String
        Public TIPO_UNIDAD_CONSERVACION As String
    End Class
    Public Class stru_result_expediente
        Private m_CONSECUTIVO_EXPEDIENTE_2 As String
        Public Property CONSECUTIVO_EXPEDIENTE_2() As String
            Get
                Return m_CONSECUTIVO_EXPEDIENTE_2
            End Get
            Set(value As String)
                m_CONSECUTIVO_EXPEDIENTE_2 = value
            End Set
        End Property
        Private m_VOLUMEN_EXPEDIENTE As String
        Public Property VOLUMEN_EXPEDIENTE() As String
            Get
                Return m_VOLUMEN_EXPEDIENTE
            End Get
            Set(value As String)
                m_VOLUMEN_EXPEDIENTE = value
            End Set
        End Property
        Private m_EXPEDIENTE_PADRE As String
        Public Property EXPEDIENTE_PADRE() As String
            Get
                Return m_EXPEDIENTE_PADRE
            End Get
            Set(value As String)
                m_EXPEDIENTE_PADRE = value
            End Set
        End Property
        Private m_ID_EXPEDIENTE As String
        Public Property ID_EXPEDIENTE() As String
            Get
                Return m_ID_EXPEDIENTE
            End Get
            Set(value As String)
                m_ID_EXPEDIENTE = value
            End Set
        End Property
        Private m_CODIGO_UNICO As String
        Public Property CODIGO_UNICO() As String
            Get
                Return m_CODIGO_UNICO
            End Get
            Set(value As String)
                m_CODIGO_UNICO = value
            End Set
        End Property
        Private m_NOMBRE_SERIE_TRD As String
        Public Property NOMBRE_SERIE_TRD() As String
            Get
                Return m_NOMBRE_SERIE_TRD
            End Get
            Set(value As String)
                m_NOMBRE_SERIE_TRD = value
            End Set
        End Property
        Private m_NOMBRE_SUBSERIE_TRD As String
        Public Property NOMBRE_SUBSERIE_TRD() As String
            Get
                Return m_NOMBRE_SUBSERIE_TRD
            End Get
            Set(value As String)
                m_NOMBRE_SUBSERIE_TRD = value
            End Set
        End Property
        Private m_TEMA_EXPEDIENTE As String
        Public Property TEMA() As String
            Get
                Return m_TEMA_EXPEDIENTE
            End Get
            Set(value As String)
                m_TEMA_EXPEDIENTE = value
            End Set
        End Property
        Private m_ASUNTO_EXPEDIENTE As String
        Public Property ASUNTO() As String
            Get
                Return m_ASUNTO_EXPEDIENTE
            End Get
            Set(value As String)
                m_ASUNTO_EXPEDIENTE = value
            End Set
        End Property
        Private m_FECHA_CREACION As String
        Public Property FECHA_CREACION() As String
            Get
                Return m_FECHA_CREACION
            End Get
            Set(value As String)
                m_FECHA_CREACION = value
            End Set
        End Property
        Private m_CODIGO_AREA_TRD As String
        Public Property CODIGO_AREA_TRD() As String
            Get
                Return m_CODIGO_AREA_TRD
            End Get
            Set(value As String)
                m_CODIGO_AREA_TRD = value
            End Set
        End Property
        Private m_NOMBRE_AREA_TRD As String
        Public Property NOMBRE_AREA_TRD() As String
            Get
                Return m_NOMBRE_AREA_TRD
            End Get
            Set(value As String)
                m_NOMBRE_AREA_TRD = value
            End Set
        End Property
        Private m_CODIGO_SERIE_TRD As String
        Public Property CODIGO_SERIE_TRD() As String
            Get
                Return m_CODIGO_SERIE_TRD
            End Get
            Set(value As String)
                m_CODIGO_SERIE_TRD = value
            End Set
        End Property
        Private m_CODIGO_SUB_SERIE_TRD As String
        Public Property CODIGO_SUB_SERIE_TRD() As String
            Get
                Return m_CODIGO_SUB_SERIE_TRD
            End Get
            Set(value As String)
                m_CODIGO_SUB_SERIE_TRD = value
            End Set
        End Property
        Private m_NOMBRE_TIPO_UNIDAD_DOCUMENTAL As String
        Public Property TIPO_UNIDAD_DOCUMENTAL() As String
            Get
                Return m_NOMBRE_TIPO_UNIDAD_DOCUMENTAL
            End Get
            Set(value As String)
                m_NOMBRE_TIPO_UNIDAD_DOCUMENTAL = value
            End Set
        End Property
        Private m_TIPO_UNIDAD_CONSERVACION As String
        Public Property TIPO_UNIDAD_CONSERVACION() As String
            Get
                Return m_TIPO_UNIDAD_CONSERVACION
            End Get
            Set(value As String)
                m_TIPO_UNIDAD_CONSERVACION = value
            End Set
        End Property
        Private m_COMPOSICION_EXPEDIENTE As String
        Public Property COMPOSICION_EXPEDIENTE() As String
            Get
                Return m_COMPOSICION_EXPEDIENTE
            End Get
            Set(value As String)
                m_COMPOSICION_EXPEDIENTE = value
            End Set
        End Property
        Private m_FECHA_EXTREMA_INICIAL As String
        Public Property FECHA_INICIAL_EXPEDICION() As String
            Get
                Return m_FECHA_EXTREMA_INICIAL
            End Get
            Set(value As String)
                m_FECHA_EXTREMA_INICIAL = value
            End Set
        End Property
        Private m_FECHA_EXTREMA_FINAL As String
        Public Property FECHA_FINAL_TERMINACION() As String
            Get
                Return m_FECHA_EXTREMA_FINAL
            End Get
            Set(value As String)
                m_FECHA_EXTREMA_FINAL = value
            End Set
        End Property
        Private m_RANGO_EXTREMO_INICIAL As String
        Public Property RANGO_EXTREMO_INICIAL() As String
            Get
                Return m_RANGO_EXTREMO_INICIAL
            End Get
            Set(value As String)
                m_RANGO_EXTREMO_INICIAL = value
            End Set
        End Property
        Private m_RANGO_EXTREMO_FINAL As String
        Public Property RANGO_EXTREMO_FINAL() As String
            Get
                Return m_RANGO_EXTREMO_FINAL
            End Get
            Set(value As String)
                m_RANGO_EXTREMO_FINAL = value
            End Set
        End Property
        Private m_ESTADO_EXPEDIENTE As String
        Public Property ESTADO_EXPEDIENTE() As String
            Get
                Return m_ESTADO_EXPEDIENTE
            End Get
            Set(value As String)
                m_ESTADO_EXPEDIENTE = value
            End Set
        End Property
        Private m_NOMBRE_PERSONA_EXPEDIENTE As String
        Public Property NOMBRE_SOLICITANTE() As String
            Get
                Return m_NOMBRE_PERSONA_EXPEDIENTE
            End Get
            Set(value As String)
                m_NOMBRE_PERSONA_EXPEDIENTE = value
            End Set
        End Property
        Private m_IDENTIFICACION_PERSONA_EXPEDIENTE As String
        Public Property IDENTIFICACION_SOLICITANTE() As String
            Get
                Return m_IDENTIFICACION_PERSONA_EXPEDIENTE
            End Get
            Set(value As String)
                m_IDENTIFICACION_PERSONA_EXPEDIENTE = value
            End Set
        End Property
        Private m_NOMBRE_RESPONSABLE_EXPEDIENTE As String
        Public Property RESPONSABLE_EXPEDIENTE() As String
            Get
                Return m_NOMBRE_RESPONSABLE_EXPEDIENTE
            End Get
            Set(value As String)
                m_NOMBRE_RESPONSABLE_EXPEDIENTE = value
            End Set
        End Property
        Private m_IDENFICACION_RESPONSABLE_EXPEDIENTE As String
        Public Property IDENFICACION_RESPONSABLE() As String
            Get
                Return m_IDENFICACION_RESPONSABLE_EXPEDIENTE
            End Get
            Set(value As String)
                m_IDENFICACION_RESPONSABLE_EXPEDIENTE = value
            End Set
        End Property
        Private m_NOMBRE_FONDO As String
        Public Property NOMBRE_FONDO() As String
            Get
                Return m_NOMBRE_FONDO
            End Get
            Set(value As String)
                m_NOMBRE_FONDO = value
            End Set
        End Property
        Private m_NOMBRE_CICLO_ARCHIVO As String
        Public Property NOMBRE_CICLO_ARCHIVO() As String
            Get
                Return m_NOMBRE_CICLO_ARCHIVO
            End Get
            Set(value As String)
                m_NOMBRE_CICLO_ARCHIVO = value
            End Set
        End Property
        Private m_NUMERO_FOLIOS_CONTENIDOS As String
        Public Property FOLIO_FISICO() As String
            Get
                Return m_NUMERO_FOLIOS_CONTENIDOS
            End Get
            Set(value As String)
                m_NUMERO_FOLIOS_CONTENIDOS = value
            End Set
        End Property
        Private m_NUMERO_ELECTRONICO_CONTENIDO As String
        Public Property FOLIO_ELECTRONICO() As String
            Get
                Return m_NUMERO_ELECTRONICO_CONTENIDO
            End Get
            Set(value As String)
                m_NUMERO_ELECTRONICO_CONTENIDO = value
            End Set
        End Property
        Private m_NUMERO_DIGITALIZADO_CONTENIDO As String
        Public Property FOLIO_DIGITALIZADO() As String
            Get
                Return m_NUMERO_DIGITALIZADO_CONTENIDO
            End Get
            Set(value As String)
                m_NUMERO_DIGITALIZADO_CONTENIDO = value
            End Set
        End Property
        Private m_ERROR_SERVICE As String
        Public Property ERROR_SERVICE() As String
            Get
                Return m_ERROR_SERVICE
            End Get
            Set(value As String)
                m_ERROR_SERVICE = value
            End Set
        End Property
        Private m_ARCHIVO_SERVICE As String
        Public Property ARCHIVO_SERVICE() As String
            Get
                Return m_ARCHIVO_SERVICE
            End Get
            Set(value As String)
                m_ARCHIVO_SERVICE = value
            End Set
        End Property
        Private m_ESTADO_GESTION_EXPEDIENTE As String
        Public Property ESTADO_GESTION_EXPEDIENTE() As String
            Get
                Return m_ESTADO_GESTION_EXPEDIENTE
            End Get
            Set(value As String)
                m_ESTADO_GESTION_EXPEDIENTE = value
            End Set
        End Property
    End Class
    Public Class stru_result_elimina_rel_volumen
        Private m_ERROR_SERVICE As String
        Public Property ERROR_SERVICE() As String
            Get
                Return m_ERROR_SERVICE
            End Get
            Set(value As String)
                m_ERROR_SERVICE = value
            End Set
        End Property
        Private m_NUMERO_REL_VOLUMEN As String
        Public Property NUMERO_REL_VOLUMEN() As String
            Get
                Return m_NUMERO_REL_VOLUMEN
            End Get
            Set(value As String)
                m_NUMERO_REL_VOLUMEN = value
            End Set
        End Property
        Private m_ID_EXPEDIENTE_PADRE As String
        Public Property ID_EXPEDIENTE_PADRE() As String
            Get
                Return m_ID_EXPEDIENTE_PADRE
            End Get
            Set(value As String)
                m_ID_EXPEDIENTE_PADRE = value
            End Set
        End Property
    End Class


    Dim stru_result_expediente_ As stru_result_expediente() = New stru_result_expediente() {}
    Dim stru_result_elimina_rel_volumen_ As stru_result_elimina_rel_volumen() = New stru_result_elimina_rel_volumen() {}

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Auto_registra_expediente_tramite(ByVal id_tipo_doc_entrante As Object,
                                                             ByVal radicado As Object,
                                                             ByVal id_tarea_workflow As Object)
        Dim resultList = New List(Of ClassExpedienteVincula)()
        Dim parameter_gestion As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim id_expediente As Integer = 0
            Dim nombre_expediente As String = ""
            Result = ClassGaExpediente.AutoRegistraExpedienteTramite(id_tipo_doc_entrante,
                                                                        radicado,
                                                                        id_tarea_workflow,
                                                                        0,
                                                                        id_expediente,
                                                                        nombre_expediente)
            If Result <> "YES" Then
                parameter_gestion.nombre_expediente = nombre_expediente
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = 0
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.nombre_expediente = nombre_expediente
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = id_expediente
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.nombre_expediente = ""
            parameter_gestion.error_gestion = "Inconsistencia general funcion Service_Auto_registra_expediente_tramite " & ex.Message
            parameter_gestion.id_expediente = 0
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_activa_auto_vincula_documentos_seleccionados_a_expediente(ByVal parameter As Object)
        Dim resultList = New List(Of ClassExpedienteVincula)()
        Dim parameter_gestion As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of List(Of class_item_element))(parameter)
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.Auto_vincula_documentos_seleccionado_a_expediente(deserialize_parameter,
                                                                                         parameter_gestion)
            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.nombre_expediente = ""
            parameter_gestion.error_gestion = "Inconsistencia general funcion Service_activa_auto_vincula_documentos_seleccionados_a_expediente " & ex.Message
            parameter_gestion.id_expediente = 0
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceCreaExpedienteIntegracionSII(ByVal IdTramite As Object, ByVal CIncripcionSII As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio crea expedente integración SII 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTramite           : Representa la identificación del tramite SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'parameter_gestion   : Retorna el resultado  del expediente registrado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-08
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim _ClassExpedienteVincula = New List(Of ClassExpedienteVincula)()
        Dim ClassExpedienteVincula As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim Result As String = ""
            Dim SCIncripcionSII As New List(Of CIncripcionSII)
            SCIncripcionSII = JsonConvert.DeserializeObject(Of List(Of CIncripcionSII))(CIncripcionSII)
            ClassExpedienteVincula.error_gestion = ClassGaExpediente.CreaExpedienteIntegracionSII(IdTramite,
                                                                                                  HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                                  HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                                  HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                  SCIncripcionSII,
                                                                                                  ClassExpedienteVincula)
            _ClassExpedienteVincula.Add(ClassExpedienteVincula)
            Return _ClassExpedienteVincula
        Catch ex As Exception
            ClassExpedienteVincula.nombre_expediente_rlacionado = ""
            ClassExpedienteVincula.error_gestion = "inconsistencia general funcion ServiceCreaExpedienteIntegracionSII " & ex.Message
            ClassExpedienteVincula.id_expediente = 0
            ClassExpedienteVincula.id_imagen_copia = 0
            ClassExpedienteVincula.valor_campos = ""
            _ClassExpedienteVincula.Add(ClassExpedienteVincula)
            Return _ClassExpedienteVincula
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaDocumentosVinculacionUnicoExpedienteSII(ByVal ReciboSII As Object,
                                                                           ByVal IdTramite As Object,
                                                                           ByVal CodBarras As Object,
                                                                           ByVal IdExpediente As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura de los documentos relacionados a una tarea para
        '          la vinculación de documentos a gabinete para un único expeidiente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTramite           : Representa la identificación del tramite SII
        'ReciboSII           : Representa el consecutivo radicado SII
        'CodBarras           : Rpresenta el consecutivo de codigo de barras SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'parameter_gestion   : Retorna el resultado  del expediente registrado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-08
        'Elabora               : Miguel Angel Urueta Miranda 
        '------------------------------------------------------------------------------------------------
        Dim _ClassExpedienteVincula = New List(Of ClassExpedienteVincula)()
        Dim ClassExpedienteVincula As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Result As String = ""
            Dim SCIncripcionSII As New List(Of CIncripcionSII)

            ClassExpedienteVincula.error_gestion = ClassDaGabinete.SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII(ReciboSII,
                                                                                                                                 CodBarras,
                                                                                                                                 IdExpediente,
                                                                                                                                 HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                                                 IdTramite,
                                                                                                                                 ClassExpedienteVincula)
            _ClassExpedienteVincula.Add(ClassExpedienteVincula)
            Return _ClassExpedienteVincula
        Catch ex As Exception
            ClassExpedienteVincula.nombre_expediente_rlacionado = ""
            ClassExpedienteVincula.error_gestion = "inconsistencia general funcion ServiceSolicitaDocumentosRelacionadosTareaWorkflowVinculacion " & ex.Message
            ClassExpedienteVincula.id_expediente = 0
            ClassExpedienteVincula.id_imagen_copia = 0
            ClassExpedienteVincula.valor_campos = ""
            _ClassExpedienteVincula.Add(ClassExpedienteVincula)
            Return _ClassExpedienteVincula
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII(ByVal ReciboSII As Object,
                                                                                            ByVal IdTramite As Object,
                                                                                            ByVal CIncripcionSII As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la estructura de los documentos relacionados a una tarea para
        '          la vinculación de documentos a gabinete para multiplex expeidientes
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTramite           : Representa la identificación del tramite SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'parameter_gestion   : Retorna el resultado  del expediente registrado
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-08
        'Elabora               : Miguel Angel Urueta Miranda 
        '------------------------------------------------------------------------------------------------
        Dim _ClassExpedienteVincula = New List(Of ClassExpedienteVincula)()
        Dim ClassExpedienteVincula As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Result As String = ""
            Dim SCIncripcionSII As New List(Of CIncripcionSII)
            SCIncripcionSII = JsonConvert.DeserializeObject(Of List(Of CIncripcionSII))(CIncripcionSII)
            ClassExpedienteVincula.error_gestion = ClassDaGabinete.SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII(ReciboSII,
                                                                                                                                    HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                                                                    IdTramite,
                                                                                                                                    SCIncripcionSII,
                                                                                                                                    ClassExpedienteVincula)
            _ClassExpedienteVincula.Add(ClassExpedienteVincula)
            Return _ClassExpedienteVincula
        Catch ex As Exception
            ClassExpedienteVincula.nombre_expediente_rlacionado = ""
            ClassExpedienteVincula.error_gestion = "inconsistencia general funcion ServiceSolicitaDocumentosRelacionadosTareaWorkflowVinculacion " & ex.Message
            ClassExpedienteVincula.id_expediente = 0
            ClassExpedienteVincula.id_imagen_copia = 0
            ClassExpedienteVincula.valor_campos = ""
            _ClassExpedienteVincula.Add(ClassExpedienteVincula)
            Return _ClassExpedienteVincula
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceVinculaDocumentoExpediente(ByVal ClsssStructureVinculaDocumento As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio de vinculación de documentos a través de web service
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'parameter_gestion   : Retorna el resultado  
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-08
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resultList = New List(Of ClassExpedienteVincula)()
        Dim parameter_gestion As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim Result As String = ""
            Dim valor_campos As String = ""
            Dim nombre_exediente_relacion As String = ""
            Dim _ClsssStructureVinculaDocumento As New List(Of ClsssStructureVinculaDocumento)
            _ClsssStructureVinculaDocumento = JsonConvert.DeserializeObject(Of List(Of ClsssStructureVinculaDocumento))(ClsssStructureVinculaDocumento)
            Result = ClassGaExpediente.VinculaDocumentoExpediente(_ClsssStructureVinculaDocumento.Item(0).IdExpedienteWeb,
                                                                  _ClsssStructureVinculaDocumento.Item(0).IdImagen,
                                                                  _ClsssStructureVinculaDocumento.Item(0).Gabinete,
                                                                  _ClsssStructureVinculaDocumento.Item(0).Radicado,
                                                                  _ClsssStructureVinculaDocumento.Item(0).IdFlujoTarea,
                                                                  valor_campos,
                                                                  nombre_exediente_relacion)
            If Result <> "YES" Then
                parameter_gestion.nombre_expediente_rlacionado = nombre_exediente_relacion
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = 0
                parameter_gestion.id_imagen_copia = 0
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.nombre_expediente_rlacionado = nombre_exediente_relacion
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = 0
                parameter_gestion.id_imagen_copia = 1
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.nombre_expediente_rlacionado = ""
            parameter_gestion.error_gestion = "inconsistencia general función Get_vincula_archivo_expediente_service " & ex.Message
            parameter_gestion.id_expediente = 0
            parameter_gestion.id_imagen_copia = 0
            parameter_gestion.valor_campos = ""
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_auto_vincula_documentos_a_expediente(ByVal dna As String)
        Dim resultList = New List(Of ClassExpedienteVincula)()
        Dim parameter_gestion As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.Auto_vincula_documentos_a_expediente(parameter_gestion)
            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList

        Catch ex As Exception
            parameter_gestion.nombre_expediente = ""
            parameter_gestion.error_gestion = "Inconsistencia general funcion Service_auto_vincula_documentos_a_expediente " & ex.Message
            parameter_gestion.id_expediente = 0
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_auto_vincula_documentos_a_expediente_estructura(ByVal id_tarea_seleccionada As Object)
        Dim resultList = New List(Of ClassExpedienteVincula)()
        Dim parameter_gestion As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.Auto_vincula_documentos_a_expediente_estructura(id_tarea_seleccionada, parameter_gestion)
            parameter_gestion.error_gestion = Result
            resultList.Add(parameter_gestion)
            Return resultList
        Catch ex As Exception
            parameter_gestion.nombre_expediente = ""
            parameter_gestion.error_gestion = "Inconsistencia general funcion Service_auto_vincula_documentos_a_expediente_estructura " & ex.Message
            parameter_gestion.id_expediente = 0
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function Get_copia_archivo_expediente_produccion_service(ByVal id_imagen As String,
                                                                    ByVal tipo_copia As String,
                                                                    ByVal id_flujo_wf As String,
                                                                    ByVal radicado As String) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Copia el archivo a la produción documental  desde workflow
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_imagen                  : Representa la imagen a copiar en el expediente
        'tipo_copia                 : Representa si el sistema valida si el documento hace
        '                             parte de un radicado interno y evita la copia
        'id_flujo_wf                : Representa la dientificación del flujo de trabajo
        'radicado                   : Representa el radicado del tramite
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        '
        '                             
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2023-06-06
        'Elabora               : Miguel Angel Urueta Miranda  Session.Item("WF_RUTAWORKFLOW")  
        '-----------------------------------------------------------------------------------
        Try
            Dim Refclass As New ClassGaProducionDocumental
            Dim Result As String = ""
            Dim valor_campos As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim obliga_relacion_expediente_gabinete As Integer = 0
            Dim nombre_campo_tramite As String = ""
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoTramiteRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                     nombre_campo_tramite)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim Valor_campo_tramite As String = ""
            Result = Class_DAT_ADIC_TAR.Solicita_valor_campo_dinamico_ruta(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                           nombre_campo_tramite,
                                                                           Val(id_flujo_wf),
                                                                           Valor_campo_tramite)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim wf_copia_doc_expediente_actualiza_exped_gabinete As Integer = 0
            Dim wf_auto_vincula_doc_expediente_actualiza_exped_gabinete As Integer = 0
            Dim wf_copia_doc_expediente_produc_actualiza_exped_gabinete As Integer = 0
            Dim obliga_actualiza_indice_expediente_gabinete As Integer = 0
            Result = Class_tipo_doc_entrante.Solicita_permiso_actualiza_expediente_indice_gabinete(Valor_campo_tramite,
                                                                                                   wf_copia_doc_expediente_actualiza_exped_gabinete,
                                                                                                   wf_auto_vincula_doc_expediente_actualiza_exped_gabinete,
                                                                                                   wf_copia_doc_expediente_produc_actualiza_exped_gabinete)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            If wf_copia_doc_expediente_produc_actualiza_exped_gabinete = 1 Then
                obliga_actualiza_indice_expediente_gabinete = 1
            End If
            Result = Refclass.activa_copia_service(id_imagen,
                                                   valor_campos,
                                                   Val(tipo_copia),
                                                   Val(id_flujo_wf),
                                                   radicado,
                                                   obliga_actualiza_indice_expediente_gabinete,
                                                   wf_copia_doc_expediente_produc_actualiza_exped_gabinete)
            If Result <> "YES" Then
                Return Result
                Exit Function
            Else
                If valor_campos = "" Then
                    Return "YES|" & valor_campos
                Else
                    Return "YES|" & valor_campos
                End If

                Exit Function
            End If
        Catch ex As Exception
            Return "inconsistencia general función Get_copia_archivo_expediente_produccion_service " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function Get_copia_archivo_producion_service(ByVal id_imagen As String,
                                                        ByVal tipo_copia As String,
                                                        ByVal id_flujo_wf As String,
                                                        ByVal radicado As String) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Copia el archivo a la produción documental 
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_imagen                  : Representa la imagen a copiar en el expediente
        'tipo_copia                 : Representa si el sistema valida si el documento hace
        '                             parte de un radicado interno y evita la copia
        'id_flujo_wf                : Representa la dientificación del flujo de trabajo
        'radicado                   : Representa el radicado del tramite
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        '
        '                             
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2023-06-06
        'Elabora               : Miguel Angel Urueta Miranda  
        '-----------------------------------------------------------------------------------
        Try
            Dim Refclass As New ClassGaProducionDocumental
            Dim Result As String = ""
            Dim valor_campos As String = ""
            Result = Refclass.activa_copia_service(id_imagen,
                                                   valor_campos,
                                                   Val(tipo_copia),
                                                   Val(id_flujo_wf),
                                                   radicado,
                                                   0,
                                                   0)
            If Result <> "YES" Then
                Return Result
                Exit Function
            Else
                If valor_campos = "" Then
                    Return "YES|" & valor_campos
                Else
                    Return "YES|" & valor_campos
                End If

                Exit Function
            End If
        Catch ex As Exception
            Return "inconsistencia general función Get_copia_archivo_producion_service " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function Get_copia_archivo_expediente_service(ByVal id_imagen As String,
                                                         ByVal tipo_copia As String,
                                                         ByVal id_flujo_wf As String,
                                                         ByVal radicado As String,
                                                         ByVal id_expediente_web As String) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Copia el archivo a un expediente especifico desde workflow
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_imagen                  : Representa la imagen a copiar en el expediente
        'tipo_copia                 : Representa si el sistema valida si el documento hace
        '                             parte de un radicado interno y evita la copia
        'id_flujo_wf                : Representa la dientificación del flujo de trabajo
        'radicado                   : Representa el radicado del tramite
        'id_expediente_web          : Representa la identificacion del expediente a copiar
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        '
        '                             
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2023-06-06
        'Elabora               : Miguel Angel Urueta Miranda 
        '-----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim valor_campos As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Dim obliga_relacion_expediente_gabinete As Integer = 0
            Dim nombre_campo_tramite As String = ""
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoTramiteRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                     nombre_campo_tramite)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim Valor_campo_tramite As String = ""
            Result = Class_DAT_ADIC_TAR.Solicita_valor_campo_dinamico_ruta(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                           nombre_campo_tramite,
                                                                           Val(id_flujo_wf),
                                                                           Valor_campo_tramite)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim wf_copia_doc_expediente_actualiza_exped_gabinete As Integer = 0
            Dim wf_auto_vincula_doc_expediente_actualiza_exped_gabinete As Integer = 0
            Dim wf_copia_doc_expediente_produc_actualiza_exped_gabinete As Integer = 0
            Dim obliga_actualiza_indice_expediente_gabinete As Integer = 0
            Result = Class_tipo_doc_entrante.Solicita_permiso_actualiza_expediente_indice_gabinete(Valor_campo_tramite,
                                                                                                   wf_copia_doc_expediente_actualiza_exped_gabinete,
                                                                                                   wf_auto_vincula_doc_expediente_actualiza_exped_gabinete,
                                                                                                   wf_copia_doc_expediente_produc_actualiza_exped_gabinete)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            If wf_copia_doc_expediente_actualiza_exped_gabinete = 1 Then
                obliga_actualiza_indice_expediente_gabinete = 1
            End If
            Result = ClassGaExpediente.Copia_documento_expediente(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                  id_imagen,
                                                                  Val(id_expediente_web),
                                                                  valor_campos,
                                                                  tipo_copia,
                                                                  id_flujo_wf,
                                                                  radicado,
                                                                  obliga_actualiza_indice_expediente_gabinete,
                                                                  wf_copia_doc_expediente_actualiza_exped_gabinete)
            If Result <> "YES" Then
                Return Result
                Exit Function
            Else
                If valor_campos = "" Then
                    Return "YES|" & valor_campos
                Else
                    Return "YES|" & valor_campos
                End If
                Exit Function
            End If
        Catch ex As Exception
            Return "inconsistencia general función Get_copia_archivo_expediente_service " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_auto_vincula_documento_gabinete_expediente(ByVal id_imagen As Object,
                                                                       ByVal id_expediente As Object,
                                                                       ByVal gabinete As Object)
        '--------------------------------------------------------------------------------
        'Funcion : Servicio que espone la vinculación de un documento a un expediente
        '          desde un gabinete
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen en un gabinete
        'id_expediente         : Representa la identificación de un expediente
        'gabinete              : Representa la identificación del gabinete al que
        '                        pertenece el documento
        '----------------------------------------------------------------------------------
        '                           RETORNO
        '----------------------------------------------------------------------------------
        'estado_expediente_service  : Retorna la estructura con los datos del expediente
        '----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-08-16
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------

        Dim resultList = New List(Of ClassExpedienteVincula)()
        Dim parameter_gestion As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim Result As String = ""
            Dim valor_campos As String = ""
            Dim nombre_exediente_relacion As String = ""
            If Session.Item("UTIL_MIG_AUTO_VINCULA_DOC_EXPEDIENTE") = 0 Then
                parameter_gestion.nombre_expediente_rlacionado = ""
                parameter_gestion.error_gestion = "El usuario no tiene permisos para auto vincular documentos al expediente desde módulo de migración"
                parameter_gestion.id_expediente = 0
                parameter_gestion.id_imagen_copia = 0
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Result = ClassGaExpediente.Vincula_documento_gabinete_expediente_migracion(id_expediente,
                                                                                       id_imagen,
                                                                                       gabinete,
                                                                                       valor_campos,
                                                                                       nombre_exediente_relacion)
            If Result <> "YES" Then
                parameter_gestion.nombre_expediente_rlacionado = nombre_exediente_relacion
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = 0
                parameter_gestion.id_imagen_copia = 0
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.nombre_expediente_rlacionado = nombre_exediente_relacion
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = 0
                parameter_gestion.id_imagen_copia = 1
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.nombre_expediente_rlacionado = ""
            parameter_gestion.error_gestion = "inconsistencia general función Service_auto_vincula_documento_gabinete_expediente " & ex.Message
            parameter_gestion.id_expediente = 0
            parameter_gestion.id_imagen_copia = 0
            parameter_gestion.valor_campos = ""
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_auto_registra_gabinete_expediente(ByVal id_gabinete As Object,
                                                              ByVal id_imagen As Object,
                                                              ByVal id_auto_registro As Object,
                                                              ByVal gabinete As Object)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que expone la funcion de Auto registro de expediente
        'con datos del gabinete 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_gabinete           : Representa la identificación del gabinete
        'nombre_gabinete       : Representa el consecutivo de radicado
        'id_imagen             : Representa la identificación de la imagen
        'gabinete              : Representa el nombre del  gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_expediente        : Retorna la idnetificación del expediente
        'nombre_expediente    : Retorna el nombre del expediente
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-13
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of ClassExpedienteVincula)()
        Dim parameter_gestion As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim valor_campos As String = ""
            Dim nombre_exediente_relacion As String = ""
            Dim id_expediente As Integer = 0
            If Session.Item("UTIL_MIG_AUTO_VINCULA_DOC_EXPEDIENTE") = 0 Then
                parameter_gestion.nombre_expediente_rlacionado = ""
                parameter_gestion.error_gestion = "El usuario no tiene permisos para auto vincular documentos al expediente desde módulo de migración"
                parameter_gestion.id_expediente = 0
                parameter_gestion.id_imagen_copia = 0
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            End If
            Result = ClassGaExpediente.Auto_registra_gabinete_expediente(id_gabinete,
                                                                         gabinete,
                                                                         id_auto_registro,
                                                                         id_imagen,
                                                                         id_expediente,
                                                                         nombre_exediente_relacion)
            If Result <> "YES" Then
                parameter_gestion.nombre_expediente_rlacionado = nombre_exediente_relacion
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = id_expediente
                parameter_gestion.id_imagen_copia = 0
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.nombre_expediente_rlacionado = nombre_exediente_relacion
                parameter_gestion.id_expediente = id_expediente
                parameter_gestion.error_gestion = Result
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.nombre_expediente_rlacionado = ""
            parameter_gestion.error_gestion = "inconsistencia general función Service_auto_registra_gabinete_expediente " & ex.Message
            parameter_gestion.id_expediente = 0
            parameter_gestion.id_imagen_copia = 0
            parameter_gestion.valor_campos = ""
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_vincula_archivo_expediente_service(ByVal id_imagen As String,
                                                           ByVal gabinete As String,
                                                           ByVal id_flujo_wf As String,
                                                           ByVal radicado As String,
                                                           ByVal id_expediente_web As String)
        Dim resultList = New List(Of ClassExpedienteVincula)()
        Dim parameter_gestion As ClassExpedienteVincula = New ClassExpedienteVincula()
        Try
            Dim Refclass As New ClassGaExpediente
            Dim Result As String = ""
            Dim valor_campos As String = ""
            Dim nombre_exediente_relacion As String = ""
            Result = Refclass.VinculaDocumentoExpediente(Val(id_expediente_web),
                                                               Val(id_imagen),
                                                               gabinete,
                                                               radicado,
                                                               Val(id_flujo_wf),
                                                               valor_campos,
                                                               nombre_exediente_relacion)
            If Result <> "YES" Then
                parameter_gestion.nombre_expediente_rlacionado = nombre_exediente_relacion
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = 0
                parameter_gestion.id_imagen_copia = 0
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            Else
                parameter_gestion.nombre_expediente_rlacionado = nombre_exediente_relacion
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = 0
                parameter_gestion.id_imagen_copia = 1
                parameter_gestion.valor_campos = valor_campos
                resultList.Add(parameter_gestion)
                Return resultList
            End If
        Catch ex As Exception
            parameter_gestion.nombre_expediente_rlacionado = ""
            parameter_gestion.error_gestion = "inconsistencia general función Get_vincula_archivo_expediente_service " & ex.Message
            parameter_gestion.id_expediente = 0
            parameter_gestion.id_imagen_copia = 0
            parameter_gestion.valor_campos = ""
            resultList.Add(parameter_gestion)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_campos_expedientes_gestion(ByVal DName As String, ByVal CAmpo As String)
        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim sqlconsult As String = " Select " & CAmpo & " from expediente_archivo where " &
                  CAmpo & "  Like '%" & DName & "%' LIMIT 50"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLista_expedientes_gestion(ByVal DName As String)

        Dim response As String = ""
        Dim country As List(Of String) = New List(Of String)()
        Try
            Dim refcconect As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim sqlconsult As String = "SELECT DISTINCT CONSECUTIVO_EXPEDIENTE_2,VOLUMEN_EXPEDIENTE,EXPEDIENTE_PADRE,ID_EXPEDIENTE AS CODIGO_UNICO," &
                "CODIGO_UNICO AS CONSECUTIVO,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD,TEMA_EXPEDIENTE AS TEMA, " &
                "ASUNTO_EXPEDIENTE AS ASUNTO,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD," &
                "CODIGO_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS TIPO_UNIDAD,COMPOSICION_EXPEDIENTE," &
                "FECHA_EXTREMA_INICIAL AS FECHA_INICIAL_EXPEDICION,FECHA_EXTREMA_FINAL AS FECHA_FINAL_TERMINACION," &
                "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,ESTADO_EXPEDIENTE,NOMBRE_PERSONA_EXPEDIENTE" &
                " AS NOMBRE_SOLICITANTE,IDENTIFICACION_PERSONA_EXPEDIENTE AS IDENTIFICACION_SOLICITANTE," _
                & "NOMBRE_RESPONSABLE_EXPEDIENTE AS RESPONSABLE_EXPEDIENTE,IDENFICACION_RESPONSABLE_EXPEDIENTE" &
                " AS IDENFICACION_RESPONSABLE,NOMBRE_FONDO,NOMBRE_CICLO_ARCHIVO,NUMERO_FOLIOS_CONTENIDOS as FOLIO_FISICO," &
                "NUMERO_ELECTRONICO_CONTENIDO AS FOLIO_ELECTRONICO,NUMERO_DIGITALIZADO_CONTENIDO AS FOLIO_DIGITALIZADO from expediente_archivo where (" &
                    "  CONSECUTIVO_EXPEDIENTE_2 like '%" & DName & "%'" &
                    " or VOLUMEN_EXPEDIENTE like '%" & DName & "%'" &
                    " or EXPEDIENTE_PADRE like '%" & DName & "%'" &
                    " or ID_EXPEDIENTE like '%" & DName & "%'" &
                    " or CODIGO_UNICO like '%" & DName & "%'" &
                    " or NOMBRE_SERIE_TRD like '%" & DName & "%'" &
                    " or NOMBRE_SUBSERIE_TRD like '%" & DName & "%'" &
                    " or TEMA_EXPEDIENTE like '%" & DName & "%'" &
                    " or ASUNTO_EXPEDIENTE like '%" & DName & "%'" &
                    " or FECHA_CREACION like '%" & DName & "%'" &
                    " or CODIGO_AREA_TRD like '%" & DName & "%'" &
                    " or NOMBRE_AREA_TRD like '%" & DName & "%'" &
                    " or CODIGO_SERIE_TRD like '%" & DName & "%'" &
                    " or CODIGO_SUB_SERIE_TRD like '%" & DName & "%'" &
                    " or NOMBRE_TIPO_UNIDAD_DOCUMENTAL like '%" & DName & "%'" &
                    " or COMPOSICION_EXPEDIENTE like '%" & DName & "%'" &
                    " or FECHA_EXTREMA_INICIAL like '%" & DName & "%'" &
                    " or FECHA_EXTREMA_FINAL like '%" & DName & "%'" &
                    " or RANGO_EXTREMO_INICIAL like '%" & DName & "%'" &
                    " or RANGO_EXTREMO_FINAL like '%" & DName & "%'" &
                    " or ESTADO_EXPEDIENTE like '%" & DName & "%'" &
                    " or NOMBRE_PERSONA_EXPEDIENTE like '%" & DName & "%'" &
                    " or IDENTIFICACION_PERSONA_EXPEDIENTE like '%" & DName & "%'" &
                    " or NOMBRE_RESPONSABLE_EXPEDIENTE like '%" & DName & "%'" &
                    " or IDENFICACION_RESPONSABLE_EXPEDIENTE like '%" & DName & "%'" &
                    " or NOMBRE_FONDO like '%" & DName & "%'" &
                    " or NOMBRE_CICLO_ARCHIVO like '%" & DName & "%'" &
                    " or NUMERO_FOLIOS_CONTENIDOS like '%" & DName & "%'" &
                    " or NUMERO_ELECTRONICO_CONTENIDO like '%" & DName & "%'" &
                    " or NUMERO_DIGITALIZADO_CONTENIDO like '%" & DName & "%'" &
                    "  ) " & "LIMIT 50"
            response = SELECTION_SELECT_FIELD(sqlconsult, datset)
            If response <> "YES" Then
                Return country
                Exit Function
            End If
            If datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To datset.Tables(0).Rows.Count - 1
                    For z As Integer = 0 To datset.Tables(0).Columns.Count - 1
                        If datset.Tables(0).Rows(i).IsNull(z) = False Then
                            Dim obsgetipe As Object = datset.Tables(0).Rows(i).Item(z).GetType.ToString
                            Dim estado_exit As String = "NO"
                            If obsgetipe = "System.DateTime" Then
                                Dim subtrin As String = datset.Tables(0).Rows(i).Item(z).ToString()
                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                Veri_existe_regitro(country,
                                          tempo_fecha,
                                          estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(tempo_fecha)
                                End If
                            Else
                                Veri_existe_regitro(country,
                                         datset.Tables(0).Rows(i).Item(z).ToString(),
                                         estado_exit)
                                If estado_exit = "NO" Then
                                    country.Add(datset.Tables(0).Rows(i).Item(z).ToString())
                                End If

                            End If
                        End If
                    Next
                Next
                Return country
            Else
                Return country
            End If
        Catch ex As Exception
            Return country
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registra_expediente(ByVal parameter As Object) As IEnumerable(Of stru_result_expediente)
        Dim stru_result_list = stru_result_expediente_.ToList
        Dim stru_result As New stru_result_expediente
        Try
            Dim deserialize_parameter = Nothing
            Dim serializer = New JavaScriptSerializer()
            deserialize_parameter = serializer.Deserialize(Of List(Of str_expediente_service))(parameter)
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""
            Dim id_expediente As Integer = 0
            Dim option_obliga_archivo_unidad As Integer = 0
            Dim id_unidad_contenedora As Integer = 0
            If deserialize_parameter Is Nothing Then
                stru_result.ERROR_SERVICE = "Imposible deserializar"
                stru_result_list.Add(stru_result)
                Return stru_result_list
            Else
                Dim id_tipo_expediente As Integer = 0
                Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
                Result = ref_Class_ra_tipo_expediente.Retorna_tipo_id_expediente(id_tipo_expediente,
                                                                                 deserialize_parameter(0).TIPO_EXPEDIENTE)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
                Dim requiere_unida_conservacion_fisica As Integer = 0
                Result = ref_Class_ra_tipo_expediente.Retorna_tipo_expediente_requiere_unidad_conservacion(id_tipo_expediente,
                                                                                                           requiere_unida_conservacion_fisica)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
                Result = Refclas.Registrar_Expediente_Conservacion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                   deserialize_parameter(0).CODIGO_UNICO,
                                                                   Val(deserialize_parameter(0).ESTADO_CODIGO_UNICO),
                                                                   Val(deserialize_parameter(0).ID_EMPRESA_GESTION),
                                                                   deserialize_parameter(0).FECHA_INICIAL_EXPEDICION,
                                                                   deserialize_parameter(0).FECHA_FINAL_TERMINACION,
                                                                   deserialize_parameter(0).RANGO_EXTREMO_INICIAL,
                                                                   deserialize_parameter(0).RANGO_EXTREMO_FINAL,
                                                                   deserialize_parameter(0).TEMA,
                                                                   deserialize_parameter(0).REGISTRO_ORGANIGRAMA,
                                                                   deserialize_parameter(0).NOMBRE_AREA,
                                                                   deserialize_parameter(0).NOMBRE_SERIE,
                                                                   deserialize_parameter(0).NOMBRE_SUBSERIE,
                                                                   id_tipo_expediente,
                                                                   Val(deserialize_parameter(0).FOLIO_DIGITALIZADO),
                                                                   Val(deserialize_parameter(0).FOLIO_FISICO),
                                                                   Val(deserialize_parameter(0).FOLIO_ELECTRONICO),
                                                                   deserialize_parameter(0).ASUNTO, 1,
                                                                   deserialize_parameter(0).TIPO_UNIDAD_CONSERVACION,
                                                                   id_expediente,
                                                                   deserialize_parameter(0).OBSERVACION_EXPEDIENTE,
                                                                   deserialize_parameter(0).TIPO_UNIDAD_DOCUMENTAL,
                                                                   deserialize_parameter(0).NOMBRE_SUB_AREA,
                                                                   option_obliga_archivo_unidad,
                                                                   "",
                                                                   id_unidad_contenedora,
                                                                   requiere_unida_conservacion_fisica,
                                                                   deserialize_parameter(0).NOMBRE_CICLO_ARCHIVO,
                                                                   deserialize_parameter(0).NOMBRE_FONDO,
                                                                   deserialize_parameter(0).NOMBRE_SOLICITANTE,
                                                                   deserialize_parameter(0).IDENTIFICACION_SOLICITANTE,
                                                                   deserialize_parameter(0).RESPONSABLE_EXPEDIENTE,
                                                                   deserialize_parameter(0).IDENFICACION_RESPONSABLE,
                                                                   deserialize_parameter(0).ALEAS_EXPEDIENTE,
                                                                   deserialize_parameter(0).EXPEDIENTE_PADRE,
                                                                   1,
                                                                   Val(deserialize_parameter(0).ID_INSTRUMENTO),
                                                                   deserialize_parameter(0).GABINETE_PRODUCION,
                                                                   0,
                                                                   0, 0)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
                Result = Refclas.Solicita_datos_expediente_service_web(id_expediente,
                                                                       stru_result)
                If Result <> "YES" Then
                    'stru_result.ESTADO_GESTION_EXPEDIENTE = HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE")
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                Else
                    HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = "CODIGO_UNICO"
                    HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = "DESC"
                    If HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_ADD_PAGINACION") = "" Then
                        HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_ADD_PAGINACION") = " OR ID_EXPEDIENTE=" & id_expediente & " "
                    Else
                        HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_ADD_PAGINACION") = HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_ADD_PAGINACION") & " OR ID_EXPEDIENTE=" & id_expediente & " "
                    End If
                    stru_result.ESTADO_GESTION_EXPEDIENTE = HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE")
                End If
                '------------------------------------------------------
                'Retorna plantilla impresion usuario gestión
                '------------------------------------------------------
                Dim Refclasexp As New ClassGaExpediente
                Dim nombre_plantilla_impresion As String = ""
                Dim id_configuracion_plantilla_rotulo As Integer = 0
                Result = Refclasexp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                     id_configuracion_plantilla_rotulo,
                                                                                                     nombre_plantilla_impresion)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = "Se registro el expediente, pero no pudo configurar el rotulo por el siguiente error " & Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list

                End If
                If nombre_plantilla_impresion = "" Then
                    nombre_plantilla_impresion = "DEFAULT"
                End If
                Dim ruta_archivo As String = ""
                Result = Refclas.Genera_rotulo_Eexpediente_pdf(id_expediente,
                                                               Session.Item("GA_IDEMPRESA"),
                                                               nombre_plantilla_impresion,
                                                               ruta_archivo)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = "Se registro el expediente, pero no se generó el rotulo " & Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                Else
                    Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                    stru_result.ERROR_SERVICE = "YES"
                    stru_result.ARCHIVO_SERVICE = ruta_archivo
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
            End If
        Catch ex As Exception
            stru_result.ERROR_SERVICE = ex.Message
            stru_result_list.Add(stru_result)
            Return stru_result_list
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_registra_expediente_volumen(ByVal parameter As Object) As IEnumerable(Of stru_result_expediente)
        Dim stru_result_list = stru_result_expediente_.ToList
        Dim stru_result As New stru_result_expediente
        Try
            Dim deserialize_parameter = Nothing
            Dim serializer = New JavaScriptSerializer()
            deserialize_parameter = serializer.Deserialize(Of List(Of str_expediente_service))(parameter)
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""
            Dim id_expediente As Integer = 0
            Dim option_obliga_archivo_unidad As Integer = 0
            Dim id_unidad_contenedora As Integer = 0
            If deserialize_parameter Is Nothing Then
                stru_result.ERROR_SERVICE = "Imposible deserializar"
                stru_result_list.Add(stru_result)
                Return stru_result_list
            Else
                Dim id_tipo_expediente As Integer = 0
                Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
                Result = ref_Class_ra_tipo_expediente.Retorna_tipo_id_expediente(id_tipo_expediente,
                                                                                 deserialize_parameter(0).TIPO_EXPEDIENTE)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
                Dim requiere_unida_conservacion_fisica As Integer = 0
                Result = ref_Class_ra_tipo_expediente.Retorna_tipo_expediente_requiere_unidad_conservacion(id_tipo_expediente,
                                                                                                           requiere_unida_conservacion_fisica)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
                Dim estru_expediente() As expediente_conservacion = Nothing
                Result = Refclas.SolicitaDatosEstructuraExpediente(Val(deserialize_parameter(0).ID_EXPEDIENTE),
                                                                                 estru_expediente)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
                Dim refclas_unidad_conservacion_ As New ClassUnidadConservacion
                Dim tipo_unidad_archivamento_expediente As String = ""
                Dim id_unidad_archivamento_expediente As Integer = 0
                If estru_expediente(0).ESTADO_ARCHIVO_INIDAD <> 0 Then
                    If estru_expediente(0).ENTRE_PAÑO_ID_ENTREPAÑO <> 0 Then
                        tipo_unidad_archivamento_expediente = "Entrepaño"
                        id_unidad_archivamento_expediente = estru_expediente(0).ENTRE_PAÑO_ID_ENTREPAÑO
                    End If
                    If estru_expediente(0).ID_UNIDAD_CONSERVACION <> 0 Then
                        tipo_unidad_archivamento_expediente = "UNIDAD CONTENEDORA EXPEDIENTE"
                        id_unidad_archivamento_expediente = estru_expediente(0).ID_UNIDAD_CONSERVACION
                    End If
                End If
                Result = Refclas.Registrar_Expediente_Volumen(Session.Item("GA_IDUSUARIOGESTION"),
                                                                   deserialize_parameter(0).CODIGO_UNICO,
                                                                   Val(deserialize_parameter(0).ESTADO_CODIGO_UNICO),
                                                                   Val(deserialize_parameter(0).ID_EMPRESA_GESTION),
                                                                   deserialize_parameter(0).FECHA_INICIAL_EXPEDICION,
                                                                   deserialize_parameter(0).FECHA_FINAL_TERMINACION,
                                                                   deserialize_parameter(0).RANGO_EXTREMO_INICIAL,
                                                                   deserialize_parameter(0).RANGO_EXTREMO_FINAL,
                                                                   deserialize_parameter(0).TEMA,
                                                                   deserialize_parameter(0).REGISTRO_ORGANIGRAMA,
                                                                   deserialize_parameter(0).NOMBRE_AREA,
                                                                   deserialize_parameter(0).NOMBRE_SERIE,
                                                                   deserialize_parameter(0).NOMBRE_SUBSERIE,
                                                                   id_tipo_expediente,
                                                                   Val(deserialize_parameter(0).FOLIO_DIGITALIZADO),
                                                                   Val(deserialize_parameter(0).FOLIO_FISICO),
                                                                   Val(deserialize_parameter(0).FOLIO_ELECTRONICO),
                                                                   deserialize_parameter(0).ASUNTO, Val(deserialize_parameter(0).ID_EXPEDIENTE),
                                                                   estru_expediente(0).CODIGO_LARGO,
                                                                   deserialize_parameter(0).TIPO_UNIDAD_CONSERVACION,
                                                                   id_expediente,
                                                                   deserialize_parameter(0).OBSERVACION_EXPEDIENTE,
                                                                   deserialize_parameter(0).TIPO_UNIDAD_DOCUMENTAL,
                                                                   deserialize_parameter(0).NOMBRE_SUB_AREA,
                                                                   option_obliga_archivo_unidad,
                                                                   tipo_unidad_archivamento_expediente,
                                                                   id_unidad_archivamento_expediente,
                                                                   requiere_unida_conservacion_fisica,
                                                                   deserialize_parameter(0).NOMBRE_CICLO_ARCHIVO,
                                                                   deserialize_parameter(0).NOMBRE_FONDO,
                                                                   deserialize_parameter(0).NOMBRE_SOLICITANTE,
                                                                   deserialize_parameter(0).IDENTIFICACION_SOLICITANTE,
                                                                   deserialize_parameter(0).RESPONSABLE_EXPEDIENTE,
                                                                   deserialize_parameter(0).IDENFICACION_RESPONSABLE,
                                                                   Val(deserialize_parameter(0).ID_INSTRUMENTO))
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
                Result = Refclas.Solicita_datos_expediente_service_web(id_expediente,
                                                                       stru_result)
                If Result <> "YES" Then
                    stru_result.ESTADO_GESTION_EXPEDIENTE = HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE")
                    stru_result.ERROR_SERVICE = Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
                '------------------------------------------------------
                'Retorna plantilla impresion usuario gestión
                '------------------------------------------------------
                Dim Refclasexp As New ClassGaExpediente
                Dim nombre_plantilla_impresion As String = ""
                Dim id_configuracion_plantilla_rotulo As Integer = 0
                Result = Refclasexp.Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                     id_configuracion_plantilla_rotulo,
                                                                                                     nombre_plantilla_impresion)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = "Se registro el expediente, pero no pudo configurar el rotulo por el siguiente error " & Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list

                End If
                If nombre_plantilla_impresion = "" Then
                    nombre_plantilla_impresion = "DEFAULT"
                End If
                Dim ruta_archivo As String = ""
                Result = Refclas.Genera_rotulo_Eexpediente_pdf(id_expediente,
                                                               Session.Item("GA_IDEMPRESA"),
                                                               nombre_plantilla_impresion,
                                                               ruta_archivo)
                If Result <> "YES" Then
                    stru_result.ERROR_SERVICE = "Se registro el expediente, pero no se generó el rotulo " & Result
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                Else
                    Session.Item("RA_RUTA_IMPRESION_FINAL") = ruta_archivo
                    stru_result.ERROR_SERVICE = "YES"
                    stru_result.ARCHIVO_SERVICE = ruta_archivo
                    stru_result_list.Add(stru_result)
                    Return stru_result_list
                End If
            End If
        Catch ex As Exception
            stru_result.ERROR_SERVICE = ex.Message
            stru_result_list.Add(stru_result)
            Return stru_result_list
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_actualiza_expediente(ByVal parameter As Object, ByVal id_expediente As Object) As IEnumerable(Of stru_result_expediente)
        Dim stru_result_list = stru_result_expediente_.ToList
        Dim stru_result As New stru_result_expediente
        Try
            Dim deserialize_parameter = Nothing
            Dim serializer = New JavaScriptSerializer()
            deserialize_parameter = serializer.Deserialize(Of List(Of str_expediente_service))(parameter)
            Dim Refclas As New ClassGaExpediente
            Dim Result As String = ""
            Dim option_obliga_archivo_unidad As Integer = 0
            Dim id_unidad_contenedora As Integer = 0
            'System.Threading.Thread.Sleep(10000)
            'stru_result.ERROR_SERVICE = "Imposible deserializar"
            'stru_result_list.Add(stru_result)
            'Return stru_result_list
            If deserialize_parameter Is Nothing Then
                stru_result.ERROR_SERVICE = "Imposible deserializar"
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If
            Dim id_tipo_expediente As Integer = 0
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            Result = ref_Class_ra_tipo_expediente.Retorna_tipo_id_expediente(id_tipo_expediente,
                                                                              deserialize_parameter(0).TIPO_EXPEDIENTE)
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If
            Dim requiere_unida_conservacion_fisica As Integer = 0
            Result = ref_Class_ra_tipo_expediente.Retorna_tipo_expediente_requiere_unidad_conservacion(id_tipo_expediente,
                                                                                                       requiere_unida_conservacion_fisica)
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If
            Result = Refclas.Actualiza_Expediente_Conservacion(Session.Item("GA_IDUSUARIOGESTION"),
                                                                   deserialize_parameter(0).CODIGO_UNICO,
                                                                   Val(deserialize_parameter(0).ESTADO_CODIGO_UNICO),
                                                                   Val(deserialize_parameter(0).ID_EMPRESA_GESTION),
                                                                   deserialize_parameter(0).FECHA_INICIAL_EXPEDICION,
                                                                   deserialize_parameter(0).FECHA_FINAL_TERMINACION,
                                                                   deserialize_parameter(0).RANGO_EXTREMO_INICIAL,
                                                                   deserialize_parameter(0).RANGO_EXTREMO_FINAL,
                                                                   deserialize_parameter(0).TEMA,
                                                                   deserialize_parameter(0).REGISTRO_ORGANIGRAMA,
                                                                   deserialize_parameter(0).NOMBRE_AREA,
                                                                   deserialize_parameter(0).NOMBRE_SERIE,
                                                                   deserialize_parameter(0).NOMBRE_SUBSERIE,
                                                                   id_expediente,
                                                                   Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                   Session.Item("ip_host_name"),
                                                                   0,
                                                                   id_tipo_expediente,
                                                                   Val(deserialize_parameter(0).FOLIO_DIGITALIZADO),
                                                                   Val(deserialize_parameter(0).FOLIO_FISICO),
                                                                   Val(deserialize_parameter(0).FOLIO_ELECTRONICO),
                                                                   deserialize_parameter(0).ASUNTO, 1,
                                                                   deserialize_parameter(0).TIPO_UNIDAD_CONSERVACION,
                                                                   deserialize_parameter(0).TIPO_UNIDAD_CONSERVACION,
                                                                   deserialize_parameter(0).OBSERVACION_EXPEDIENTE,
                                                                   deserialize_parameter(0).TIPO_UNIDAD_DOCUMENTAL,
                                                                   deserialize_parameter(0).NOMBRE_SUB_AREA,
                                                                    deserialize_parameter(0).NOMBRE_CICLO_ARCHIVO,
                                                                   deserialize_parameter(0).NOMBRE_FONDO,
                                                                   deserialize_parameter(0).NOMBRE_SOLICITANTE,
                                                                   deserialize_parameter(0).IDENTIFICACION_SOLICITANTE,
                                                                   deserialize_parameter(0).RESPONSABLE_EXPEDIENTE,
                                                                   deserialize_parameter(0).IDENFICACION_RESPONSABLE,
                                                                   deserialize_parameter(0).ID_INSTRUMENTO,
                                                                   requiere_unida_conservacion_fisica,
                                                                   "")
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If
            Result = Refclas.Solicita_datos_expediente_service_web(id_expediente,
                                                                       stru_result)
            If Result <> "YES" Then
                stru_result.ESTADO_GESTION_EXPEDIENTE = HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE")
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            Else
                stru_result.ERROR_SERVICE = "YES"
                stru_result.ARCHIVO_SERVICE = ""
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If


        Catch ex As Exception
            stru_result.ERROR_SERVICE = ex.Message
            stru_result_list.Add(stru_result)
            Return stru_result_list
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_des_registrar_expediente_volumen(ByVal id_expediente As Object) As IEnumerable(Of stru_result_elimina_rel_volumen)
        Dim stru_result_list = stru_result_elimina_rel_volumen_.ToList
        Dim stru_result As New stru_result_elimina_rel_volumen
        Try
            Dim ref_Class_ra_relacion_expediente As New Class_ra_relacion_expediente
            Dim Result As String = ""
            Dim id_expediente_padre As Integer = 0
            Dim numero_volumen As Integer = 0
            Result = ref_Class_ra_relacion_expediente.Des_registrar_expediente_volumen(Val(id_expediente),
                                                                                       "",
                                                                                       id_expediente_padre)
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If
            Result = ref_Class_ra_relacion_expediente.Solicita_numero_volumen_expediente_padre(id_expediente_padre,
                                                                                               numero_volumen)
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If
            stru_result.ERROR_SERVICE = "YES"
            stru_result.ID_EXPEDIENTE_PADRE = id_expediente_padre
            stru_result.NUMERO_REL_VOLUMEN = numero_volumen
            stru_result_list.Add(stru_result)
            Return stru_result_list
        Catch ex As Exception
            stru_result.ERROR_SERVICE = ex.Message
            stru_result_list.Add(stru_result)
            Return stru_result_list
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_relacionar_como_expediente_volumen(ByVal id_expediente_padre As Object, ByVal id_expediente_volumen As Object) As IEnumerable(Of stru_result_elimina_rel_volumen)
        Dim stru_result_list = stru_result_elimina_rel_volumen_.ToList
        Dim stru_result As New stru_result_elimina_rel_volumen
        Try
            Dim ref_Class_ra_relacion_expediente As New Class_ra_relacion_expediente
            Dim Result As String = ""
            Dim numero_volumen As Integer = 0
            Result = ref_Class_ra_relacion_expediente.Relacionar_como_expediente_volumen(Val(id_expediente_padre),
                                                                                         Val(id_expediente_volumen),
                                                                                         "")
            If Result <> "YES" Then
                stru_result.ERROR_SERVICE = Result
                stru_result_list.Add(stru_result)
                Return stru_result_list
            End If
            stru_result.ERROR_SERVICE = "YES"
            stru_result.ID_EXPEDIENTE_PADRE = id_expediente_padre
            stru_result.NUMERO_REL_VOLUMEN = numero_volumen
            stru_result_list.Add(stru_result)
            Return stru_result_list
        Catch ex As Exception
            stru_result.ERROR_SERVICE = ex.Message
            stru_result_list.Add(stru_result)
            Return stru_result_list
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_inicializa_gestion_expediente(ByVal parameter As Object) As String

        Try
            HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 0
            HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA") = Nothing
            HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA") = Nothing
            Return "YES"

        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Function Veri_existe_regitro(ByVal country As Object,
                                 ByVal valor As String,
                                 ByRef estado_exist As String) As String
        Try
            Veri_existe_regitro = "NO"
            For i As Integer = 0 To country.Count - 1
                If Trim(country(i).ToString) = Trim(valor) Then
                    estado_exist = "YES"
                    Veri_existe_regitro = "YES"
                    Exit Function
                End If
            Next
            Veri_existe_regitro = "YES"
        Catch ex As Exception
            Veri_existe_regitro = "Inconcistencia general función Veri_existe_regitro " & ex.Message
        End Try
    End Function

    Private MYSQL_SELECT_COMMAND As MySqlCommand
    Private MYSQL_INSERT_COMMAND As MySqlCommand
    Private Function MYSQL_INSERT_COMMNAD(ByVal Sql_String As String) As String

        Dim Command_Base As New MySqlCommand(Sql_String)
        Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
        Dim Result As String = Returna_Conexion_Mysql(conectmyslq)
        If Result <> "YES" Then
            MYSQL_INSERT_COMMNAD = "Imposible conectar con la base de datos " & Result
        End If
        Me.MYSQL_INSERT_COMMAND = Command_Base
        Try
            Dim command As New MySqlCommand(Me.MYSQL_INSERT_COMMAND.CommandText, conectmyslq)
            If command.ExecuteNonQuery <> 0 Then
                MYSQL_INSERT_COMMNAD = "YES"
                Return MYSQL_INSERT_COMMNAD
            Else
                MYSQL_INSERT_COMMNAD = "NO"

                Return MYSQL_INSERT_COMMNAD
            End If
            MYSQL_INSERT_COMMNAD = "YES"
        Catch ex As MySqlException
            MYSQL_INSERT_COMMNAD = ex.Message
        Finally
            conectmyslq.Close()
        End Try
    End Function
    Private Function Returna_Conexion_Mysql(ByRef CconectionMysql As MySql.Data.MySqlClient.MySqlConnection) As String
        Dim poltrue As String = "False"
        If HttpContext.Current.Session.Item("RA_ACTIVA_POOL_DBMS") = "1" Then
            poltrue = "True"
        Else
            poltrue = "False"
        End If
        Dim Contenido_Config As String = "Persist Security Info=" _
          & True & ";database=" & HttpContext.Current.Session("RA_DB_NAME_MODULO").ToString _
          & ";server=" & HttpContext.Current.Session("RA_IP_SERVER_MODULO").ToString _
         & ";user id=" & HttpContext.Current.Session("RA_USER_DBMS_MODULO").ToString _
         & ";pwd=" & HttpContext.Current.Session("RA_PASW_DBMS_MODULO").ToString _
         & ";Pooling=" & poltrue & ";Min Pool Size=0;Max Pool Size=" &
         HttpContext.Current.Session.Item("RA_NUMERO_DBMS_CONEX")


        Try
            CconectionMysql = New MySql.Data.MySqlClient.MySqlConnection(Contenido_Config)
            If Not CconectionMysql Is Nothing Then
                CconectionMysql.Open()
            Else
                Returna_Conexion_Mysql = "Imposible conectar en la base de datos"
                Exit Function
            End If
            Returna_Conexion_Mysql = "YES"
        Catch ex As MySqlException
            Returna_Conexion_Mysql = ex.Message
        Finally
            'CconectionMysql = Nothing
        End Try
    End Function
    Public Function SELECTION_SELECT_FIELD(ByVal Sql_String As String, ByRef objet As Object) As String
        Dim Result As String = ""
        SELECTION_SELECT_FIELD = "SELECTION_SELECT_FIELD NO RECONOCE EL DBMS"
        If HttpContext.Current.Session("RA_TYPE_DBMS_MODULO").ToString = "mysql" Then
            Result = MYSQL_SELECT_FIELD(Sql_String, objet)
            If Result <> "YES" Then
                SELECTION_SELECT_FIELD = "Inconsistencia en la funcion SELECTION_SELEC_FIELD LLAMANDO A MYSQL FIELD " & Result
                Exit Function
            Else
                SELECTION_SELECT_FIELD = "YES"
                Exit Function
            End If

        End If

    End Function
    Private Function MYSQL_SELECT_FIELD(ByVal Sql_String As String, ByRef Mysqldatacet As System.Data.DataSet) As String
        Dim Result As String = ""
        MYSQL_SELECT_FIELD = "YES"
        Mysqldatacet = New DataSet
        Dim conectmyslq As New MySql.Data.MySqlClient.MySqlConnection
        Result = Returna_Conexion_Mysql(conectmyslq)
        If Result <> "YES" Then
            MYSQL_SELECT_FIELD = "Imposible conectar con la base de datos " & Result
            Exit Function
        End If
        MYSQL_SELECT_COMMAND = New MySqlCommand(Sql_String)
        Dim DatMysqlAdpter As MySql.Data.MySqlClient.MySqlDataAdapter =
            New MySql.Data.MySqlClient.MySqlDataAdapter(MYSQL_SELECT_COMMAND.CommandText, conectmyslq)
        Try
            DatMysqlAdpter.Fill(Mysqldatacet)
        Catch ex As MySqlException
            MYSQL_SELECT_FIELD = ex.Message
        Finally
            conectmyslq.Close()
        End Try


    End Function
End Class