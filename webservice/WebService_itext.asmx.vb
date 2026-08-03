Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Web.Http
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json
Public Class service_itex
    Public Property error_sistema As String
    Public Property url_salida As String
    Public Property url_path As String
    Public Property name_file As String
    Public Property error_log As String
    Public Property top As String
End Class
' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService_itext
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_solicita_datos_configuracion_itex_stamp_user(anotate) As IEnumerable(Of class_ra_itex_config_stamp)
        Dim resul_service = New List(Of class_ra_itex_config_stamp)()
        Dim item As New class_ra_itex_config_stamp
        Try
            Dim Result As String = ""
            Dim id_user_gestion As Integer = Session.Item("GA_IDUSUARIOGESTION")
            Dim Class_ra_config_itex_stamp As New Class_ra_config_itex_stamp
            Result = Class_ra_config_itex_stamp.Solicita_datos_configuracion_itex_stamp_user(id_user_gestion,
                                                                                             resul_service)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = ex.Message
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_update_insert_configuracion_itex_stamp_user(anotate) As IEnumerable(Of class_ra_itex_config_stamp)
        Dim resul_service = New List(Of class_ra_itex_config_stamp)()
        Dim item As New class_ra_itex_config_stamp
        Try
            Dim Result As String = ""
            Dim id_user_gestion As Integer = Session.Item("GA_IDUSUARIOGESTION")
            Dim Class_ra_config_itex_stamp As New Class_ra_config_itex_stamp
            Dim existencia As String = ""
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of class_ra_itex_config_stamp())(anotate)
            Result = Class_ra_config_itex_stamp.Solicita_existencia_datos_configuracion_itex_stamp_user(id_user_gestion,
                                                                                                        existencia)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                If existencia = "NO" Then
                    Result = Class_ra_config_itex_stamp.Insert_datos_configuracion_itex_stamp_user(id_user_gestion,
                                                                                                   deserialize_parameter(0))
                Else
                    Result = Class_ra_config_itex_stamp.Update_datos_configuracion_itex_stamp_user(id_user_gestion,
                                                                                                   deserialize_parameter(0))
                End If
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = ex.Message
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_anotate_pdf_image(ByVal anotate As Object) As IEnumerable(Of service_itex)
        Dim resul_service = New List(Of service_itex)()
        Dim item As New service_itex
        Try
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of Class_itext_anotate())(anotate)
            Dim Class_ItexShare As New Class_ItexShare
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Dim Result As String = ""
            Dim valor As String = ""
            Dim url_pagina As String = ""
            '////-------Solicita validación firma digital del documento-------//////
            Dim Class_ra_cert_registro_certificado_archivo As New Class_ra_cert_registro_certificado_archivo
            Dim id_certificado As Long = 0
            Result = Class_ra_cert_registro_certificado_archivo.Solicita_registro_certificado_archivo_imagen_gabinete(deserialize_parameter(0).anotate_id_imagen,
                                                                                                                      deserialize_parameter(0).anotate_cabinete_imagen,
                                                                                                                      id_certificado)
            If Result <> "YES" Then
                item.error_sistema = Result
                item.error_log = "YES"
                resul_service.Add(item)
                Return resul_service
            End If
            If id_certificado <> 0 Then
                item.error_sistema = "El documento está firmado digitalmente, agregar o modificar el contenido podría invalidar su efectividad legal"
                item.error_log = "YES"
                resul_service.Add(item)
                Return resul_service
            End If
            Result = Class_ItexShare.add_anotate_image_pdf(deserialize_parameter(0),
                                                           valor,
                                                           url_pagina)
            If Result <> "YES" Then
                item.error_sistema = Result
                item.error_log = "YES"
                resul_service.Add(item)
                Return resul_service

            End If
            Dim datos_add_grafo As String = "url grafo : " & deserialize_parameter(0).anotate_file_src & vbCrLf &
                " DIMENCION DEL BITMAN GRAFO" & vbCrLf &
                " TOP     : " & deserialize_parameter(0).anotate_y & vbCrLf &
                " LEFT    : " & deserialize_parameter(0).anotate_x & vbCrLf &
                " WITH    : " & deserialize_parameter(0).anotate_width & vbCrLf &
                " HEIGTH  : " & deserialize_parameter(0).anotate_heigth & vbCrLf &
                "DESCRIPCION DOCUMENTO" & vbCrLf &
                " URL DOC : " & deserialize_parameter(0).pdf_file_src & vbCrLf &
                " ESCALA : " & deserialize_parameter(0).anotate_scale & vbCrLf &
                " GABINETE : " & deserialize_parameter(0).anotate_cabinete_imagen & vbCrLf &
                " ID IMAGEN : " & deserialize_parameter(0).anotate_id_imagen & vbCrLf
            If deserialize_parameter(0).anotate_cabinete_imagen <> "" And deserialize_parameter(0).anotate_id_imagen Then
                Result = Class_logdocuarchi.Registra_log_procesing_image(deserialize_parameter(0).anotate_id_imagen,
                                                                        deserialize_parameter(0).anotate_cabinete_imagen,
                                                                        deserialize_parameter(0).anotate_desc_transacion,
                                                                        "AGREGA GRAFO PDF",
                                                                        deserialize_parameter(0).anotate_id_workflow,
                                                                        deserialize_parameter(0).anotate_radicado,
                                                                        datos_add_grafo)
                item.top = valor
                item.error_sistema = "YES"
                item.url_salida = url_pagina
                item.error_log = Result
                resul_service.Add(item)
                Return resul_service
            Else
                item.top = valor
                item.error_sistema = "YES"
                item.url_salida = url_pagina
                item.error_log = "YES"
                resul_service.Add(item)
                Return resul_service
            End If
        Catch ex As Exception
            item.error_sistema = ex.Message
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function service_download_pdf_visor_plus(ByVal anotate As Object,
                                                    ByVal parameter_log As Object,
                                                    ByVal operation As Object) As IEnumerable(Of service_itex)
        Dim resul_service = New List(Of service_itex)()
        Dim item As New service_itex
        Try
            Dim Class_ItexShare As New Class_ItexShare
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Dim deserialize_parameter = Nothing
            deserialize_parameter = JsonConvert.DeserializeObject(Of Class_itext_anotate())(parameter_log)
            Dim Result As String = ""
            Dim url_download_pdf As String = ""
            Dim url_path As String = ""
            Dim name_file As String = ""
            Result = Class_ItexShare.download_pdf_visor_plus(anotate, url_download_pdf, url_path, name_file)
            If Result <> "YES" Then
                item.error_sistema = Result
                resul_service.Add(item)
                Return resul_service
            Else
                If deserialize_parameter(0).anotate_cabinete_imagen <> "" And deserialize_parameter(0).anotate_id_imagen Then
                    Result = Class_logdocuarchi.Registra_log_procesing_image(deserialize_parameter(0).anotate_id_imagen,
                                                                             deserialize_parameter(0).anotate_cabinete_imagen,
                                                                             deserialize_parameter(0).anotate_desc_transacion,
                                                                             operation,
                                                                             deserialize_parameter(0).anotate_id_workflow,
                                                                             deserialize_parameter(0).anotate_radicado,
                                                                             "")

                    item.error_sistema = "YES"
                    item.url_salida = url_download_pdf
                    item.name_file = name_file
                    item.url_path = url_path
                    item.error_log = Result
                    resul_service.Add(item)
                    Return resul_service
                Else
                    item.error_sistema = "YES"
                    item.url_salida = url_download_pdf
                    item.name_file = name_file
                    item.url_path = url_path
                    item.error_log = "YES"
                    resul_service.Add(item)
                    Return resul_service
                End If

            End If
        Catch ex As Exception
            item.error_sistema = ex.Message
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function
End Class