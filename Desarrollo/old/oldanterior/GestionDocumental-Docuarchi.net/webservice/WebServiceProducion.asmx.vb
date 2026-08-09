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
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceProducion
    Inherits System.Web.Services.WebService
    Public Class paramter_nodo_expediente
        Public result_ As String
        Public valor_ As String

    End Class
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaCargarDocumentoExpediente(ByVal parameter As Object)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la solcitud de carga de documento y evlalua si el usuario 
        '          tiene permisos para adjuntar documentos y retorna la identiicación del expediente
        '          y el nivel seleccionado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDproduccion  : Retorna la estructtura del expediente y el resultado del permiso de cargar
        '                documetos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-31
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim ListCDproduccion = New List(Of CDproduccion)()
        Dim ItemCDproduccion As New CDproduccion
        Try
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim CDexpedienteSeleccionado As New CDexpedienteSeleccionado
            ItemCDproduccion.AppError = ClassGaProducionDocumental.SolicitaCargarDocumentoExpediente(CDexpedienteSeleccionado)
            ItemCDproduccion.CDexpedienteSeleccionado = New List(Of CDexpedienteSeleccionado)
            ItemCDproduccion.CDexpedienteSeleccionado.Add(CDexpedienteSeleccionado)
            ListCDproduccion.Add(ItemCDproduccion)
            Return ListCDproduccion
        Catch ex As Exception
            ItemCDproduccion.AppError = "Inconsistencia general funcion  ServiceSolicitaCargarDocumentoExpediente " & ex.Message
            ListCDproduccion.Add(ItemCDproduccion)
            Return ListCDproduccion
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ServiceSolicitaListaTipologiasExpediente(ByVal id As Object) As IEnumerable(Of control_general_drow_lista)
        '-----------------------------------------------------------------------------------------------
        'Funcion : Servicio que expone la solcitud de las tipologias documentales relacionadas a 
        '          un expediente
        '         
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'control_general_drow_lista  : Retorna la estructtura con el control lista de las tipologías
        '                
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-31
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Dim resul_service = New List(Of control_general_drow_lista)()
        Dim item As New control_general_drow_lista
        Dim lista_item_drow As New List(Of control_drow_lista)
        Try
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            item.error_sistema = ClassGaProducionDocumental.SolicitaListaTipologiasExpediente(id,
                                                                                              lista_item_drow)
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        Catch ex As Exception
            item.error_sistema = "Inconsistencia general funcion  ServiceSolicitaListaTipologiasExpediente " & ex.Message
            item.item_sistema = lista_item_drow
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_descarga_documentos_expediente_produccion(ByVal id_producion As Object,
                                                                      ByVal totalCount As Object,
                                                                      ByVal id_Cont As Object) As IEnumerable(Of class_dowload_expediente_producion)
        '---------------------------------------------------------------------------
        'Funcion : Servicio que genera un item de una estructura para la descarga
        '          de un documento de un expediente
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_producion          : Representa la identificador del documento en la
        '                        produccion documental
        'totalCount            : Representa el total de los documentos a descargar
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'PG_STRU_IMAGENES_DOWNLOAD_PRODUCION  : Retorna la estructura de documentos
        'que se van a descargar
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-12-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Dim resultList = New List(Of class_dowload_expediente_producion)()
        Dim parameter_upload As class_dowload_expediente_producion = New class_dowload_expediente_producion()
        Try
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim Result As String = ""
            Dim state_property As Long = 1
            If id_Cont = 1 Then
                Session.Item("PG_STRU_IMAGENES_DOWNLOAD_PRODUCION") = Nothing
            End If
            Dim out_source_file_zip As String = ""
            Dim url_file_zip As String = ""
            Dim name_document As String = ""
            Result = ClassGaProducionDocumental.Descarga_documentos_expediente_produccion(id_producion,
                                                                                          totalCount,
                                                                                          id_Cont,
                                                                                          HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                          state_property,
                                                                                          Session.Item("PG_STRU_IMAGENES_DOWNLOAD_PRODUCION"), out_source_file_zip,
                                                                                          url_file_zip,
                                                                                          name_document)

            If Result <> "YES" Then
                parameter_upload.result = Result
                resultList.Add(parameter_upload)
                Return resultList
            Else
                parameter_upload.result = Result
                parameter_upload.id_Cont = id_Cont
                parameter_upload.id_produccion = id_producion
                parameter_upload.state_propietary = state_property
                parameter_upload.out_source_file_zip = out_source_file_zip
                parameter_upload.url_file_zip = url_file_zip
                parameter_upload.name_document = name_document
                resultList.Add(parameter_upload)
                Return resultList
            End If
        Catch ex As Exception
            parameter_upload.result = ex.Message
            resultList.Add(parameter_upload)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_parameter_upload(ByVal parameter As Object) As IEnumerable(Of class_parameter_upload)
        Dim resultList = New List(Of class_parameter_upload)()
        Dim parameter_upload As class_parameter_upload = New class_parameter_upload()
        Try
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim Class_ra_config_upload_gestion As New Class_ra_config_upload_gestion
            Dim Class_wf_config_upload_gestion As New Class_wf_config_upload_gestion
            Dim Result As String = ""
            Select Case parameter
                Case "PQR"
                    Dim ra_config_upload_gestion As Class_ra_config_upload_gestion.ra_config_upload_gestion = Nothing
                    Result = Class_ra_config_upload_gestion.Solicita_parameter_upload_gestion("PQR",
                                                                                              ra_config_upload_gestion)
                    If Result <> "YES" Then
                        parameter_upload.error_result = Result
                        resultList.Add(parameter_upload)
                        Return resultList
                    Else
                        parameter_upload.error_result = "YES"
                        parameter_upload.ExtensionPermitida = ra_config_upload_gestion.EXTENSION_UPLOAD
                        parameter_upload.Maximo_tamano_archivo_byte = ra_config_upload_gestion.LENG_UPLOAD
                        resultList.Add(parameter_upload)
                        Return resultList
                    End If
                Case "PRODUCCION"
                    Dim ra_config_upload_gestion As Class_ra_config_upload_gestion.ra_config_upload_gestion = Nothing
                    Result = Class_ra_config_upload_gestion.Solicita_parameter_upload_gestion("PRODUCCION",
                                                                                              ra_config_upload_gestion)
                    If Result <> "YES" Then
                        parameter_upload.error_result = Result
                        resultList.Add(parameter_upload)
                        Return resultList
                    Else
                        parameter_upload.error_result = "YES"
                        parameter_upload.ExtensionPermitida = ra_config_upload_gestion.EXTENSION_UPLOAD
                        parameter_upload.Maximo_tamano_archivo_byte = ra_config_upload_gestion.LENG_UPLOAD
                        resultList.Add(parameter_upload)
                        Return resultList
                    End If
                Case "WORKFLOW"
                    Dim wf_config_upload_gestion As Class_wf_config_upload_gestion.wf_config_upload_gestion = Nothing
                    Result = Class_wf_config_upload_gestion.Solicita_parameter_upload_gestion_wf("WORKFLOW",
                                                                                                 wf_config_upload_gestion)
                    If Result <> "YES" Then
                        parameter_upload.error_result = Result
                        resultList.Add(parameter_upload)
                        Return resultList
                    Else
                        parameter_upload.error_result = "YES"
                        parameter_upload.ExtensionPermitida = wf_config_upload_gestion.EXTENSION_UPLOAD
                        parameter_upload.Maximo_tamano_archivo_byte = wf_config_upload_gestion.LENG_UPLOAD
                        resultList.Add(parameter_upload)
                        Return resultList
                    End If
                Case "CORRESPO"
                    Dim ra_config_upload_gestion As Class_ra_config_upload_gestion.ra_config_upload_gestion = Nothing
                    Result = Class_ra_config_upload_gestion.Solicita_parameter_upload_gestion("CORRESPO",
                                                                                              ra_config_upload_gestion)
                    If Result <> "YES" Then
                        parameter_upload.error_result = Result
                        resultList.Add(parameter_upload)
                        Return resultList
                    Else
                        parameter_upload.error_result = "YES"
                        parameter_upload.ExtensionPermitida = ra_config_upload_gestion.EXTENSION_UPLOAD
                        parameter_upload.Maximo_tamano_archivo_byte = ra_config_upload_gestion.LENG_UPLOAD
                        resultList.Add(parameter_upload)
                        Return resultList
                    End If
                Case "INTRUESII"
                    Dim ra_config_upload_gestion As Class_ra_config_upload_gestion.ra_config_upload_gestion = Nothing
                    parameter_upload.error_result = "YES"
                    parameter_upload.ExtensionPermitida = ".xlsx"
                    parameter_upload.Maximo_tamano_archivo_byte = 1000000000
                    resultList.Add(parameter_upload)
                    Return resultList
                Case "INTVIRTUALSII"
                    Dim ra_config_upload_gestion As Class_ra_config_upload_gestion.ra_config_upload_gestion = Nothing
                    parameter_upload.error_result = "YES"
                    parameter_upload.ExtensionPermitida = ".xlsx"
                    parameter_upload.Maximo_tamano_archivo_byte = 1000000000
                    resultList.Add(parameter_upload)
                    Return resultList
                Case "MIGRACION"
                    Dim Class_ra_mig_config_migracion As New Class_ra_mig_config_migracion
                    Dim ra_config_upload_migracion As mig_config_upload_gestion = Nothing
                    Result = Class_ra_mig_config_migracion.Solicita_parameter_upload_migracion(ra_config_upload_migracion)
                    If Result <> "YES" Then
                        parameter_upload.error_result = Result
                        resultList.Add(parameter_upload)
                        Return resultList
                    Else
                        parameter_upload.error_result = "YES"
                        parameter_upload.ExtensionPermitida = ra_config_upload_migracion.EXTENSION_UPLOAD
                        parameter_upload.Maximo_tamano_archivo_byte = ra_config_upload_migracion.LENG_UPLOAD
                        resultList.Add(parameter_upload)
                        Return resultList
                    End If
                Case Else
                    Dim ExtensionPermitida As String = ""
                    Result = ClassGaProducionDocumental.Solicita_listado_extension_de_archivos_permitidas(ExtensionPermitida, ".")
                    If Result <> "YES" Then
                        parameter_upload.error_result = Result
                        resultList.Add(parameter_upload)
                        Return resultList
                    Else
                        parameter_upload.error_result = "YES"
                        parameter_upload.ExtensionPermitida = ExtensionPermitida
                        parameter_upload.Maximo_tamano_archivo_byte = 10000000
                        resultList.Add(parameter_upload)
                        Return resultList
                    End If
            End Select
        Catch ex As Exception
            parameter_upload.error_result = ex.Message
            resultList.Add(parameter_upload)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_cortar_expediente(ByVal item_user As Object)
        Try
            Session.Item("PG_CORTAR_PEGAR") = Nothing
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                Return "Por favor seleccione un expediente para cortar y mover"
                Exit Function
            End If
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") = 0 Then
                Return "Los niveles no se pueden cortar para mover"
                Exit Function
            End If
            Dim split() As String = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION").Split("|")
            Dim Result As String = ""
            Dim Ref_class As New Class_ra_pro_niveles
            Dim Estado_propietario As String = ""
            Result = Ref_class.Solicita_estado_nivel_propietario(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                 Val(split(1)),
                                                                 Estado_propietario)
            If Result <> "YES" Then
                Return Result
                Exit Function
            End If
            If Estado_propietario = "NO" Then
                Return "Usted no es propietario del nivel, no pueden mover expedientes anidados en este nivel"
                Exit Function
            End If
            Session.Item("PG_CORTAR_PEGAR") = Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION")
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general servicio Get_cortar_expediente " & ex.Message
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Get_pegar_expediente(ByVal item_user As Object)
        Dim serializer = New JavaScriptSerializer()
        Dim paramter_nodo_expediente_ As New paramter_nodo_expediente
        Dim deserialize_user = Nothing
        Try

            If Session.Item("PG_CORTAR_PEGAR") Is Nothing Then
                paramter_nodo_expediente_.result_ = "Por favor seleccione un expediente para mover"
                paramter_nodo_expediente_.valor_ = ""
                deserialize_user = serializer.Serialize(paramter_nodo_expediente_)
                Return deserialize_user
                Exit Function
            End If
            If Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION") = "" Then
                paramter_nodo_expediente_.result_ = "Por favor seleccione un nivel para mover el expediente"
                paramter_nodo_expediente_.valor_ = ""
                deserialize_user = serializer.Serialize(paramter_nodo_expediente_)
                Return deserialize_user
                Exit Function
            End If
            If InStr(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION"), "|") > 0 Then
                paramter_nodo_expediente_.result_ = "Por favor seleccione un nivel para mover el expediente"
                paramter_nodo_expediente_.valor_ = ""
                deserialize_user = serializer.Serialize(paramter_nodo_expediente_)
                Return deserialize_user
                Exit Function
            End If
            Dim Result As String = ""
            Dim Refclas As New Class_niveles_organizacion
            Dim spli_() As String = Session.Item("PG_CORTAR_PEGAR").ToString.Split("|")
            Dim node_agrege_text As String = ""
            Result = Refclas.Traslado_carpeta_nivel(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                   Val(spli_(1)),
                                                   Val(Session.Item("PG_SELECCION_TREVIEEW_PRODUCCION")),
                                                   Val(spli_(2)),
                                                   Val(spli_(0)),
                                                   node_agrege_text)
            If Result <> "YES" Then
                paramter_nodo_expediente_.result_ = Result
                paramter_nodo_expediente_.valor_ = ""
                deserialize_user = serializer.Serialize(paramter_nodo_expediente_)
                Return deserialize_user
                'Return Result & "|"
            Else
                paramter_nodo_expediente_.result_ = "YES"
                paramter_nodo_expediente_.valor_ = node_agrege_text
                deserialize_user = serializer.Serialize(paramter_nodo_expediente_)
                Return deserialize_user
                'Return "YES"
            End If
        Catch ex As Exception
            paramter_nodo_expediente_.result_ = ex.Message
            paramter_nodo_expediente_.valor_ = ""
            deserialize_user = serializer.Serialize(paramter_nodo_expediente_)
            Return deserialize_user

        End Try
    End Function
End Class
Public Class class_parameter_upload
    Public Property ExtensionPermitida As String
    Public Property Maximo_tamano_archivo_byte As Long
    Public Property error_result As String
End Class