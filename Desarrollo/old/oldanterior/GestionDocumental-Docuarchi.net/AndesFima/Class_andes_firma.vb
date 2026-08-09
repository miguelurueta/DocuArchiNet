
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Security.Cryptography.X509Certificates
Imports System.IO

Public Class class_paramter_andes_firma
    Property Login_service As String    'Representa el loguin del usuario de la plataforma andes
    Property password_service As String 'Representa el pasword del servicio de la plataforma andes
    Property documento As String        'Representa la cedual del documento de identificación del firmante
    Property pinFirma As String         'Representa el ping del certificado andes
    Property tipodocumento As String    'Represental el tipo de documento  default 1
    Property formatoEntrada As Integer  'Representa el formato de entrada de archivo default 1
    Property formatoSalida As Integer   'Representa el formato de salida de archivo default 1
    Property file_entrada As String     'Archivo de entrada
    Property file_salida As String      'Archivo firmado de salida
    Property tsa As String              'Activa si activa estampa "false"
    Property tsaUser As String          'Usuario para el estampado digital
    Property tsaPass As String          'Pasword para el estampado digital
End Class
Public Class class_service_parameter_firma
    Property Error_gestion As String
    Property Class_paramter_andes_firma As List(Of class_paramter_andes_firma)
    Property class_parameter_firma_sistema As class_parameter_firma_sistema
End Class
Public Class class_parameter_firma_sistema
    Property url_service As String
    Property id_certficado As Integer
    Property esta_formato_permitido As String
    Property id_registro_producion As Object
    Property id_imagen As Integer
    Property gabinete As String
    Property archivo As String
    Property estado_registra_firma_producion As Integer
End Class

Public Class Class_andes_firma
    Function Solicita_estrutura_andes_firma(ByVal id_imagen As Integer,
                                            ByVal gabinete As String,
                                            ByVal archivo As String,
                                            ByVal id_registro_produccion As Long,
                                            ByVal valida_existencia_firma As Integer,
                                            ByRef class_service_parameter_firma As class_service_parameter_firma) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de firmado para la api local de firmado de andes
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_imagen                : Representa la identificación la imagen a firmar
        'gabinete                 : Representa el nombre del gabinete
        'archivo                  : Representa el archivo a firmar
        'id_registro_produccion   : Representa el registro de producción documental
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_service_parameter_firma   : Retorna la estructura de servicio de consumo de api andes
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            class_service_parameter_firma.Class_paramter_andes_firma = New List(Of class_paramter_andes_firma)
            class_service_parameter_firma.class_parameter_firma_sistema = New class_parameter_firma_sistema
            Dim Class_ra_cert_certificado_has_remit_dest_interno As New Class_ra_cert_certificado_has_remit_dest_interno
            Dim Class_ra_cert_certificado As New Class_ra_cert_certificado
            Dim Result As String = ""
            Result = Class_ra_cert_certificado_has_remit_dest_interno.Solicita_identificacion_certificado_usuario(HttpContext.Current.Session("GA_IDUSUARIOGESTION"),
                                                                                                                  class_service_parameter_firma.class_parameter_firma_sistema.id_certficado)
            If Result <> "YES" Then
                Solicita_estrutura_andes_firma = Result
                Exit Function
            End If
            If class_service_parameter_firma.class_parameter_firma_sistema.id_certficado = 0 Then
                Result = Class_ra_cert_certificado.Solicita_identificacion_cert_default(class_service_parameter_firma.class_parameter_firma_sistema.id_certficado)
                If Result <> "YES" Then
                    Solicita_estrutura_andes_firma = Result
                    Exit Function
                End If
            End If
            If class_service_parameter_firma.class_parameter_firma_sistema.id_certficado = 0 Then
                Solicita_estrutura_andes_firma = "No hay una firma registrada para firmar el documento digitalmente"
                Exit Function
            End If
            '---//Solicita el registro de producción documental si viene vacio
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            If id_registro_produccion = 0 Then
                Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(gabinete,
                                                                         id_imagen,
                                                                         stru_paramter_image,
                                                                         1,
                                                                         1,
                                                                         1,
                                                                         1,
                                                                         1)
                If Result <> "YES" Then
                    Solicita_estrutura_andes_firma = Result
                    Exit Function
                End If
                id_registro_produccion = stru_paramter_image.ID_PRODUCCION
            End If
            Dim Stru_ra_cert_certificado As Stru_ra_cert_certificado = Nothing
            Result = Class_ra_cert_certificado.Solicita_estructura_certificado(class_service_parameter_firma.class_parameter_firma_sistema.id_certficado,
                                                                               Stru_ra_cert_certificado)
            If Result <> "YES" Then
                Solicita_estrutura_andes_firma = Result
                Exit Function
            End If
            If Stru_ra_cert_certificado.estado_revocado = 1 Then
                Solicita_estrutura_andes_firma = "El certificado se encuentra revocado, imposible firmar el documento"
                Exit Function
            End If
            '---///Valida la firma del documento---////
            Dim id_certificado_archivo_registro As Long = 0
            Dim Class_ra_cert_registro_certificado_archivo As New Class_ra_cert_registro_certificado_archivo
            If valida_existencia_firma = 1 And id_registro_produccion <> 0 Then
                Result = Class_ra_cert_registro_certificado_archivo.Solicita_registro_certificado_archivo(id_registro_produccion,
                                                                                                          stru_paramter_image.ID_REGISTRO_VERSION,
                                                                                                          id_certificado_archivo_registro)
                If Result <> "YES" Then
                    Solicita_estrutura_andes_firma = Result
                    Exit Function
                End If
                If id_certificado_archivo_registro <> 0 Then
                    Solicita_estrutura_andes_firma = "El archivo ya ha sido firmado digitalmente. No es posible realizar más modificaciones o firmar nuevamente."
                    Exit Function
                End If
            End If
            '---///Valida la vigencia del certificado de documentos
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim date_fecha As String = Now
            Dim fecha_valida_firma As String = Stru_ra_cert_certificado.valido_hasta
            Result = ClassGestionFechas.Formatea_Fecha_Almacenamiento_guion(date_fecha)
            If Result <> "YES" Then
                Solicita_estrutura_andes_firma = Result
                Exit Function
            End If
            Result = ClassGestionFechas.formato_fecha_estructura(fecha_valida_firma)
            If Result <> "YES" Then
                Solicita_estrutura_andes_firma = Result
                Exit Function
            End If
            If fecha_valida_firma < date_fecha Then
                Solicita_estrutura_andes_firma = "El certificado se encuentra vencido, valido hasta " & fecha_valida_firma
                Exit Function
            End If
            '---///Solicitamos la estructura del ente certificador
            Dim Class_ra_cert_ente_certificador As New Class_ra_cert_ente_certificador
            Dim Stru_ra_cert_ente_certificador As Stru_ra_cert_ente_certificador = Nothing
            Result = Class_ra_cert_ente_certificador.Solicita_estructura_ente_certificador(Stru_ra_cert_certificado.ra_cert_ente_certificador_id_ente_certificador,
                                                                                           Stru_ra_cert_ente_certificador)
            If Result <> "YES" Then
                Solicita_estrutura_andes_firma = Result
                Exit Function
            End If
            '---///Solicitamos la estructura del servicio relacionado
            Dim Class_ra_cert_servicio_certificado As New Class_ra_cert_servicio_certificado
            Dim Stru_ra_cert_servicio_certificado As stru_ra_cert_servicio_certificado = Nothing
            Result = Class_ra_cert_servicio_certificado.Solicita_estructura_servicio_firma_certificado(Stru_ra_cert_certificado.ra_cert_servicio_certificado_id_cert_sevcio_certificado,
                                                                                                       Stru_ra_cert_servicio_certificado)
            If Result <> "YES" Then
                Solicita_estrutura_andes_firma = Result
                Exit Function
            End If
            class_service_parameter_firma.class_parameter_firma_sistema.url_service = Stru_ra_cert_servicio_certificado.url_servicio
            '---///Solicitamos la estructura de la extensines permitidas
            Dim Class_cert_file_extension_servicio_certificado As New Class_cert_file_extension_servicio_certificado
            Dim stru_file_extensiones() As String = Nothing
            Result = Class_cert_file_extension_servicio_certificado.Solicita_extensiones_archivo_servicio_certificado(Stru_ra_cert_certificado.ra_cert_servicio_certificado_id_cert_sevcio_certificado,
                                                                                                                      stru_file_extensiones)
            If Result <> "YES" Then
                Solicita_estrutura_andes_firma = Result
                Exit Function
            End If
            Dim class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Dim Ruta_almacenamiento As String = ""
            Dim ruta_archivo_gabinete As String = ""
            '----//Solicita la ruta del archivo si la función no trae un archivo
            If archivo = "" Then
                Result = class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruta_almacenamiento,
                                                                       gabinete)
                If Result <> "YES" Then
                    Solicita_estrutura_andes_firma = Result
                    Exit Function
                End If

                Result = ClassDaGabinete.Solicita_ruta_achivo_gabinete(id_imagen,
                                                                       gabinete,
                                                                       Ruta_almacenamiento,
                                                                       ruta_archivo_gabinete)
                If Result <> "YES" Then
                    Solicita_estrutura_andes_firma = Result
                    Exit Function
                End If
                archivo = ruta_archivo_gabinete
            End If
            '-----///----Determina las extensiones permitidas----////
            Dim estado_firma_extension As String = ""
            Dim fil As New FileInfo(archivo)
            Dim format_repalce As String = fil.Extension.Replace(".", "")
            class_service_parameter_firma.class_parameter_firma_sistema.esta_formato_permitido = format_repalce
            For i As Integer = 0 To stru_file_extensiones.Length - 1
                If UCase(format_repalce) = UCase(stru_file_extensiones(i)) Then
                    estado_firma_extension = "YES"
                End If
            Next
            If estado_firma_extension <> "YES" Then
                Solicita_estrutura_andes_firma = "Formato no valido para firma digital (" & format_repalce & ")"
                Exit Function
            End If
            Dim Class_paramter_andes_firma As New class_paramter_andes_firma
            Class_paramter_andes_firma.Login_service = Stru_ra_cert_ente_certificador.Login_service
            Class_paramter_andes_firma.password_service = Stru_ra_cert_ente_certificador.password_service
            Class_paramter_andes_firma.tsaUser = Stru_ra_cert_ente_certificador.Login_tsa_service
            Class_paramter_andes_firma.tsaPass = Stru_ra_cert_ente_certificador.password_tsa_service
            Class_paramter_andes_firma.documento = Stru_ra_cert_certificado.numero_identificacion_suscriptor
            Class_paramter_andes_firma.pinFirma = Stru_ra_cert_certificado.numero_serial
            Class_paramter_andes_firma.tipodocumento = "1"
            Class_paramter_andes_firma.formatoEntrada = 1
            Class_paramter_andes_firma.formatoSalida = 1
            Class_paramter_andes_firma.file_entrada = archivo
            Class_paramter_andes_firma.file_salida = archivo
            If Stru_ra_cert_certificado.util_tsa_certificado = 1 Then
                Class_paramter_andes_firma.tsa = "true"
            Else
                Class_paramter_andes_firma.tsa = "false"
            End If
            class_service_parameter_firma.Class_paramter_andes_firma.Add(Class_paramter_andes_firma)
            class_service_parameter_firma.class_parameter_firma_sistema.id_registro_producion = id_registro_produccion
            class_service_parameter_firma.class_parameter_firma_sistema.id_imagen = id_imagen
            class_service_parameter_firma.class_parameter_firma_sistema.gabinete = gabinete
            class_service_parameter_firma.class_parameter_firma_sistema.archivo = archivo.Replace("\", "/")
            class_service_parameter_firma.class_parameter_firma_sistema.estado_registra_firma_producion = 1
            Solicita_estrutura_andes_firma = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_estrutura_andes_firma = "Inconsistencia general funcion Solicita_estrutura_andes_firma " & ex.Message
        End Try
    End Function

End Class
