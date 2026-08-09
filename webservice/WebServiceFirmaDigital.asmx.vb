Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebServiceFirmaDigital
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Solicita_estrutura_andes_firma(ByVal id_image As Object,
                                                           ByVal gabinete As Object,
                                                           ByVal modulo_funcion As Integer,
                                                           ByVal valida_exitencia_firma As Integer)

        '-----------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone la estructura de firmando de andes
        '    
        '          
        '         
        '-----------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------------------
        'id_image               : Representa la idneitifcación de la imagen
        'gabinete               : Represental el nombre del gabinete                       
        'modulo_funcion         : Represental el modulo que firma el documento
        'valida_exitencia_firma : Representa si el sistema valida un sistema de firmas                        
        '
        '------------------------------------------------------------------------------------------
        '                           RETORNO
        '------------------------------------------------------------------------------------------
        'class_ra_cert_registro_certificado_archivo_reponse : Retorna la estructura de datos 
        'del resutlado de la firma
        '------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_service_parameter_firma)
        Dim class_service_parameter_firma As class_service_parameter_firma = New class_service_parameter_firma
        Try
            ''////------------------------validar permisos------////
            ''------//Valida permiso firma workflow
            If modulo_funcion = 1 Then
                If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_WF") = 0 Then
                    class_service_parameter_firma.Error_gestion = "El usuario workflow no tiene permiso para agregar firma digital"
                    resultList.Add(class_service_parameter_firma)
                    Return resultList
                End If
            End If
            ''------//Valida permiso produccion
            If modulo_funcion = 2 Then
                If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_GD") = 0 Then
                    class_service_parameter_firma.Error_gestion = "El usuario de gestión no tiene permiso para agregar firma digital"
                    resultList.Add(class_service_parameter_firma)
                    Return resultList
                End If
            End If
            ''------//Valida permiso docuarchi   
            If modulo_funcion = 3 Then
                If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_DA") = 0 Then
                    class_service_parameter_firma.Error_gestion = "El usuario docuarchi no tiene permiso para agregar firma digital"
                    resultList.Add(class_service_parameter_firma)
                    Return resultList
                End If
            End If
            Dim Class_andes_firma As New Class_andes_firma
            Dim id_registro_produccion As Long = 0
            Dim archivo_firma As String = ""
            class_service_parameter_firma.Error_gestion = Class_andes_firma.Solicita_estrutura_andes_firma(id_image,
                                                                                                           gabinete,
                                                                                                           archivo_firma,
                                                                                                           id_registro_produccion,
                                                                                                           valida_exitencia_firma,
                                                                                                           class_service_parameter_firma)
            resultList.Add(class_service_parameter_firma)
            Return resultList
        Catch ex As Exception
            class_service_parameter_firma.Error_gestion = ex.Message
            resultList.Add(class_service_parameter_firma)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_Agrega_certificado_digital_a_documento(ByVal parameter As Object)

        '-----------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el registro del certificdo digital 
        '    
        '          
        '         
        '-----------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------------------
        'id_image               : Representa la idneitifcación de la imagen
        'gabinete               : Represental el nombre del gabinete                       
        'modulo_funcion         : Represental el modulo que firma el documento
        'valida_exitencia_firma : Representa si el sistema valida un sistema de firmas                        
        '
        '------------------------------------------------------------------------------------------
        '                           RETORNO
        '------------------------------------------------------------------------------------------
        'class_ra_cert_registro_certificado_archivo_reponse : Retorna la estructura de datos 
        'del resutlado del registro de la firma en el sistema
        '------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-12
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_ra_cert_registro_certificado_archivo_reponse)
        Dim class_ra_cert_registro_certificado_archivo_reponse As class_ra_cert_registro_certificado_archivo_reponse = New class_ra_cert_registro_certificado_archivo_reponse
        Try

            Dim Class_ra_cert_registro_certificado_archivo As New Class_ra_cert_registro_certificado_archivo
            Dim id_certificado As Integer = 0
            Dim id_registro_firma As Long = 0
            Dim deserialize_parameter As New class_parameter_firma_sistema
            deserialize_parameter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of class_parameter_firma_sistema)(parameter)
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            class_ra_cert_registro_certificado_archivo_reponse.Error_gestion = ClassDaGabinete.SolicitaEtructuraImagenGabinete(deserialize_parameter.gabinete,
                                                                                                                               deserialize_parameter.id_imagen,
                                                                                                                               stru_paramter_image,
                                                                                                                               1,
                                                                                                                               1,
                                                                                                                               1,
                                                                                                                               1)
            If class_ra_cert_registro_certificado_archivo_reponse.Error_gestion <> "YES" Then
                resultList.Add(class_ra_cert_registro_certificado_archivo_reponse)
                Return resultList
            End If
            class_ra_cert_registro_certificado_archivo_reponse.Error_gestion = Class_ra_cert_registro_certificado_archivo.Agrega_certificado_digital_a_documento(deserialize_parameter.id_certficado,
                                                                                                                                                                 deserialize_parameter.id_registro_producion,
                                                                                                                                                                 deserialize_parameter.id_imagen,
                                                                                                                                                                 deserialize_parameter.gabinete,
                                                                                                                                                                 deserialize_parameter.archivo,
                                                                                                                                                                 1,
                                                                                                                                                                 stru_paramter_image.ID_REGISTRO_VERSION,
                                                                                                                                                                 deserialize_parameter.id_certficado)

            class_ra_cert_registro_certificado_archivo_reponse.Icono_file = "far fa-lock-alt"
            resultList.Add(class_ra_cert_registro_certificado_archivo_reponse)
            Return resultList
        Catch ex As Exception
            class_ra_cert_registro_certificado_archivo_reponse.Error_gestion = ex.Message
            resultList.Add(class_ra_cert_registro_certificado_archivo_reponse)
            Return resultList
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Service_firma_digital_documento(ByVal id_image As Object,
                                                    ByVal gabinete As Object,
                                                    ByVal modulo_funcion As Integer,
                                                    ByVal valida_exitencia_firma As Integer)

        '-----------------------------------------------------------------------------------------
        'Funcion : Servicio web que expone el metodo de firmado digital para el gestor documental
        '    
        '          
        '         
        '-----------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------------------
        'id_image               : Representa la idneitifcación de la imagen
        'gabinete               : Represental el nombre del gabinete                       
        'modulo_funcion         : Represental el modulo que firma el documento
        'valida_exitencia_firma : Representa si el sistema valida un sistema de firmas                        
        '
        '------------------------------------------------------------------------------------------
        '                           RETORNO
        '------------------------------------------------------------------------------------------
        'class_ra_cert_registro_certificado_archivo_reponse : Retorna la estructura de datos 
        'del resutlado de la firma
        '------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------
        Dim resultList = New List(Of class_ra_cert_registro_certificado_archivo_reponse)
        Dim class_ra_cert_registro_certificado_archivo_reponse As class_ra_cert_registro_certificado_archivo_reponse = New class_ra_cert_registro_certificado_archivo_reponse
        Try
            '////------------------------validar permisos------////
            '------//Valida permiso firma workflow
            If modulo_funcion = 1 Then
                If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_WF") = 0 Then
                    class_ra_cert_registro_certificado_archivo_reponse.Error_gestion = "El usuario workflow no tiene permiso para agregar firma digital"
                    resultList.Add(class_ra_cert_registro_certificado_archivo_reponse)
                    Return resultList
                End If
            End If
            '------//Valida permiso produccion
            If modulo_funcion = 2 Then
                If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_GD") = 0 Then
                    class_ra_cert_registro_certificado_archivo_reponse.Error_gestion = "El usuario de gestión no tiene permiso para agregar firma digital"
                    resultList.Add(class_ra_cert_registro_certificado_archivo_reponse)
                    Return resultList
                End If
            End If
            '------//Valida permiso docuarchi   
            If modulo_funcion = 3 Then
                If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_DA") = 0 Then
                    class_ra_cert_registro_certificado_archivo_reponse.Error_gestion = "El usuario docuarchi no tiene permiso para agregar firma digital"
                    resultList.Add(class_ra_cert_registro_certificado_archivo_reponse)
                    Return resultList
                End If
            End If
            Dim Class_ra_cert_registro_certificado_archivo As New Class_ra_cert_registro_certificado_archivo
            Dim id_certificado As Integer = 0
            Dim id_registro_firma As Long = 0
            'class_ra_cert_registro_certificado_archivo_reponse.Error_gestion = Class_ra_cert_registro_certificado_archivo.firma_digital_documento(id_image,
            '                                                                                                                                      gabinete,
            '                                                                                                                                      "",
            '                                                                                                                                      0,
            '                                                                                                                                      1,
            '                                                                                                                                      1,
            '                                                                                                                                      "",
            '                                                                                                                                      id_certificado,
            '                                                                                                                                      id_registro_firma)
            class_ra_cert_registro_certificado_archivo_reponse.Icono_file = "far fa-lock-alt"
            resultList.Add(class_ra_cert_registro_certificado_archivo_reponse)
            Return resultList
        Catch ex As Exception
            class_ra_cert_registro_certificado_archivo_reponse.Error_gestion = ex.Message
            resultList.Add(class_ra_cert_registro_certificado_archivo_reponse)
            Return resultList
        End Try
    End Function

End Class