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
            Dim deserialize_parameter As Class_itext_anotate() =
                JsonConvert.DeserializeObject(Of Class_itext_anotate())(Convert.ToString(anotate))
            If deserialize_parameter Is Nothing OrElse deserialize_parameter.Length <> 1 OrElse
               deserialize_parameter(0) Is Nothing OrElse
               Not NormalizeGraphAnnotation(deserialize_parameter(0)) Then
                item.error_sistema = "Los datos de la anotación no son válidos"
                item.error_log = "YES"
                resul_service.Add(item)
                Return resul_service
            End If

            Dim Class_ItexShare As New Class_ItexShare
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Dim Result As String = ""
            Dim valor As String = ""
            Dim url_pagina As String = ""
            Dim loadingMode As String = EstrategiaCargaPdf.ModoStreamingCompleto
            '////-------Solicita validación firma digital del documento-------//////
            Dim Class_ra_cert_registro_certificado_archivo As New Class_ra_cert_registro_certificado_archivo
            Dim id_certificado As Long = 0
            Result = Class_ra_cert_registro_certificado_archivo.Solicita_registro_certificado_archivo_imagen_gabinete(deserialize_parameter(0).anotate_id_imagen,
                                                                                                                      deserialize_parameter(0).anotate_cabinete_imagen,
                                                                                                                      id_certificado)
            If Result <> "YES" Then
                item.error_sistema = "No fue posible validar el estado del documento"
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
                                                           url_pagina,
                                                           loadingMode)
            If Result <> "YES" Then
                item.error_sistema = Result
                item.error_log = "YES"
                resul_service.Add(item)
                Return resul_service
            End If
            Dim datos_add_grafo As String = "PAGINA : " & deserialize_parameter(0).pdf_file_page & vbCrLf &
                "OPERACION : AGREGA GRAFO PDF" & vbCrLf &
                "RESULTADO : CONFIRMADO" & vbCrLf &
                "MODO CARGA : " & loadingMode & vbCrLf
            Dim cabinet As String = Convert.ToString(deserialize_parameter(0).anotate_cabinete_imagen)
            Dim imageId As Long = Convert.ToInt64(deserialize_parameter(0).anotate_id_imagen)
            If Not String.IsNullOrWhiteSpace(cabinet) AndAlso imageId > 0 Then
                Try
                    Result = Class_logdocuarchi.Registra_log_procesing_image(deserialize_parameter(0).anotate_id_imagen,
                                                                            deserialize_parameter(0).anotate_cabinete_imagen,
                                                                            deserialize_parameter(0).anotate_desc_transacion,
                                                                            "AGREGA GRAFO PDF",
                                                                            deserialize_parameter(0).anotate_id_workflow,
                                                                            deserialize_parameter(0).anotate_radicado,
                                                                            datos_add_grafo)
                Catch
                    Result = "No fue posible registrar la auditoría"
                End Try
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
            item.error_sistema = "No fue posible procesar la anotación"
            item.error_log = "YES"
            resul_service.Add(item)
            Return resul_service
        End Try
    End Function

    Private Function NormalizeGraphAnnotation(ByVal annotation As Class_itext_anotate) As Boolean
        If String.IsNullOrWhiteSpace(Convert.ToString(annotation.pdf_file_src)) Then Return False

        Dim pageNumber As Integer
        Dim rotation As Integer
        Dim applyTransparency As Integer
        Dim tolerance As Integer
        Dim canvasWidth As Double
        Dim canvasHeight As Double
        Dim annotationWidth As Double
        Dim annotationHeight As Double
        Dim annotationX As Double
        Dim annotationY As Double
        Dim annotationScale As Double
        Dim imageId As Long

        If Not TryInteger(annotation.pdf_file_page, pageNumber) OrElse pageNumber < 1 Then Return False
        If Not TryInteger(annotation.rotation, rotation) OrElse
           (rotation <> 0 AndAlso rotation <> 90 AndAlso rotation <> 180 AndAlso rotation <> 270) Then Return False
        If Not TryInteger(annotation.aplica_transparente, applyTransparency) OrElse
           (applyTransparency <> 0 AndAlso applyTransparency <> 1) Then Return False
        If Not TryInteger(annotation.num_tranaparent, tolerance) OrElse tolerance < 0 OrElse tolerance > 255 Then Return False
        If Not TryPositiveFinite(annotation.pdf_file_width, canvasWidth) Then Return False
        If Not TryPositiveFinite(annotation.pdf_file_heigth, canvasHeight) Then Return False
        If Not TryPositiveFinite(annotation.anotate_width, annotationWidth) Then Return False
        If Not TryPositiveFinite(annotation.anotate_heigth, annotationHeight) Then Return False
        If Not TryNonNegativeFinite(annotation.anotate_x, annotationX) Then Return False
        If Not TryNonNegativeFinite(annotation.anotate_y, annotationY) Then Return False
        If Not TryPositiveFinite(annotation.anotate_scale, annotationScale) Then Return False
        If Not Long.TryParse(Convert.ToString(annotation.anotate_id_imagen, Globalization.CultureInfo.InvariantCulture),
                             Globalization.NumberStyles.Integer,
                             Globalization.CultureInfo.InvariantCulture,
                             imageId) OrElse imageId < 0 Then Return False

        Dim hasFile As Boolean = Not String.IsNullOrWhiteSpace(Convert.ToString(annotation.anotate_file_src))
        Dim hasBase64 As Boolean = Not String.IsNullOrWhiteSpace(Convert.ToString(annotation.anotate_file_bit))
        If hasFile = hasBase64 Then Return False

        annotation.pdf_file_page = pageNumber
        annotation.rotation = rotation.ToString(Globalization.CultureInfo.InvariantCulture)
        annotation.aplica_transparente = applyTransparency.ToString(Globalization.CultureInfo.InvariantCulture)
        annotation.num_tranaparent = tolerance.ToString(Globalization.CultureInfo.InvariantCulture)
        annotation.pdf_file_width = canvasWidth
        annotation.pdf_file_heigth = canvasHeight
        annotation.anotate_width = annotationWidth
        annotation.anotate_heigth = annotationHeight
        annotation.anotate_x = annotationX
        annotation.anotate_y = annotationY
        annotation.anotate_scale = annotationScale
        annotation.anotate_id_imagen = imageId
        Return True
    End Function

    Private Function TryInteger(ByVal value As Object, ByRef result As Integer) As Boolean
        Return Integer.TryParse(Convert.ToString(value, Globalization.CultureInfo.InvariantCulture),
                                Globalization.NumberStyles.Integer,
                                Globalization.CultureInfo.InvariantCulture,
                                result)
    End Function

    Private Function TryPositiveFinite(ByVal value As Object, ByRef result As Double) As Boolean
        Return TryFinite(value, result) AndAlso result > 0
    End Function

    Private Function TryNonNegativeFinite(ByVal value As Object, ByRef result As Double) As Boolean
        Return TryFinite(value, result) AndAlso result >= 0
    End Function

    Private Function TryFinite(ByVal value As Object, ByRef result As Double) As Boolean
        Return Double.TryParse(Convert.ToString(value, Globalization.CultureInfo.InvariantCulture),
                               Globalization.NumberStyles.Float,
                               Globalization.CultureInfo.InvariantCulture,
                               result) AndAlso Not Double.IsNaN(result) AndAlso Not Double.IsInfinity(result)
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
