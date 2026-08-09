Imports System.IO
Public Structure stru_ra_cert_servicio_certificado
    Dim id_cert_sevcio_certificado As Integer
    Dim nombre_servicio As String
    Dim url_servicio As String
    Dim url_archivo_send As String
    Dim url_archivo_load As String
    Dim tipo_servicio As String
    Dim tipo_send As String
    Dim tipo_load As String
End Structure

Public Class Class_ra_cert_servicio_certificado
    Function Solicita_estructura_servicio_firma_certificado(ByVal id_servicio As Integer,
                                                            ByRef Stru_ra_cert_servicio_certificado As stru_ra_cert_servicio_certificado) As String
        '----------------------------------------------------
        'Funcion : Solicita la estructura del servicio
        'de formado digital de documentos de un certficado
        'Fecha : 2022-03-12
        'Ing. Miguel Angel Urueta Miranda
        '----------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " SELECT  id_cert_sevcio_certificado,nombre_servicio,url_servicio, " &
            "url_archivo_send,url_archivo_load,tipo_servicio,tipo_send,tipo_load " &
            " from ra_cert_servicio_certificado where id_cert_sevcio_certificado=" & id_servicio
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cert_servicio_certificado")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_servicio_firma_certificado = "Función Solicita_estructura_servicio_firma_certificado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then

                Solicita_estructura_servicio_firma_certificado = "Imposible encontrar el servicio de firmado digital para el certificado"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    Stru_ra_cert_servicio_certificado.id_cert_sevcio_certificado = 0
                Else
                    Stru_ra_cert_servicio_certificado.id_cert_sevcio_certificado = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) Then
                    Stru_ra_cert_servicio_certificado.nombre_servicio = ""
                Else
                    Stru_ra_cert_servicio_certificado.nombre_servicio = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) Then
                    Stru_ra_cert_servicio_certificado.url_servicio = ""
                Else
                    Stru_ra_cert_servicio_certificado.url_servicio = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) Then
                    Stru_ra_cert_servicio_certificado.url_archivo_send = ""
                Else
                    Stru_ra_cert_servicio_certificado.url_archivo_send = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) Then
                    Stru_ra_cert_servicio_certificado.url_archivo_load = ""
                Else
                    Stru_ra_cert_servicio_certificado.url_archivo_load = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) Then
                    Stru_ra_cert_servicio_certificado.tipo_servicio = ""
                Else
                    Stru_ra_cert_servicio_certificado.tipo_servicio = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    Stru_ra_cert_servicio_certificado.tipo_send = ""
                Else
                    Stru_ra_cert_servicio_certificado.tipo_send = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    Stru_ra_cert_servicio_certificado.tipo_load = ""
                Else
                    Stru_ra_cert_servicio_certificado.tipo_load = Datset.Tables(0).Rows(0).Item(7)
                End If
                Solicita_estructura_servicio_firma_certificado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_servicio_firma_certificado = "Inconsistencia general funcion Solicita_estructura_servicio_firma_certificado " & ex.Message
        End Try
    End Function
    Function Consume_service_firma_archivo(ByVal url_file_send_firma As String,
                                           ByVal Stru_ra_cert_certificado As Stru_ra_cert_certificado,
                                           ByVal Stru_ra_cert_servicio_certificado As stru_ra_cert_servicio_certificado,
                                           ByRef url_file_load_firma As String) As String
        Try
            Dim Result As String = ""
            If Stru_ra_cert_servicio_certificado.nombre_servicio = "SII" Then
                Result = Me.Service_firma_digital_archivo_sii(url_file_send_firma,
                                                              Stru_ra_cert_certificado,
                                                              url_file_load_firma)
                If Result <> "YES" Then
                    Consume_service_firma_archivo = Result
                    Exit Function
                End If
            End If
            Consume_service_firma_archivo = "YES"
        Catch ex As Exception
            Consume_service_firma_archivo = "Inconsistencia general funcion Consume_service_firma_archivo " & ex.Message
        End Try
    End Function
    Function Service_firma_digital_archivo_sii(ByVal url_file_send_firma As String,
                                               ByVal Stru_ra_cert_certificado As Stru_ra_cert_certificado,
                                               ByRef url_file_load_firma As String) As String
        Try
            Dim Result As String = ""
            Dim usuario_sii As String = ""
            Dim clave_usuario_sii As String = ""
            Dim UrlBase As String = ""
            Dim codigo_empresa As String = ""
            Dim Class_ws_usuarioworkflowsii As New Class_ws_usuarioworkflowsii
            Result = Class_ws_usuarioworkflowsii.solicita_usuario_validacion_sii(codigo_empresa,
                                                                                 usuario_sii,
                                                                                 clave_usuario_sii)
            If Result <> "YES" Then
                Service_firma_digital_archivo_sii = Result
                Exit Function
            End If
            Result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(UrlBase,
                                                                                     "solicitarToken")
            If Result <> "YES" Then
                Service_firma_digital_archivo_sii = Result
                Exit Function
            End If
            Dim stru_token As SolicitaToken = Nothing
            Dim Class_ClassResfull As New Class_ClassResfull
            Result = Class_ClassResfull.Solicitar_token_general(codigo_empresa,
                                                               usuario_sii,
                                                               clave_usuario_sii,
                                                               UrlBase & "solicitarToken",
                                                               stru_token)
            If Result <> "YES" Then
                Service_firma_digital_archivo_sii = Result
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                Service_firma_digital_archivo_sii = stru_token.mensajeerror
                Exit Function
            End If
            Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros.Add("codigoempresa", codigo_empresa)
            Parametros.Add("usuariows", usuario_sii)
            Parametros.Add("token", stru_token.token)
            Parametros.Add("url", url_file_send_firma)
            'Parametros.Add("url", "http://190.242.62.210/gestion_new/Temp_Radicacion/migracion/DIG00005004.PDF")
            Dim Class_Desserializacion As New Class_Desserializacion
            Dim Stru_firma_digital_sii As stru_firma_digital_sii = Nothing
            Dim respuestaServidor As String = ""
            Result = Class_ClassResfull.GetResponse(UrlBase & "firmarDigitalmenteDocumento",
                                                    Parametros,
                                                    "POST",
                                                    respuestaServidor)
            If Result <> "YES" Then
                Service_firma_digital_archivo_sii = Result
                Exit Function
            End If
            Result = Class_Desserializacion.DesSerializacion_firma_digital(respuestaServidor,
                                                                           Stru_firma_digital_sii)
            If Result <> "YES" Then
                Service_firma_digital_archivo_sii = Result
                Exit Function
            End If
            If Stru_firma_digital_sii.mensajeerror <> "" Then
                Service_firma_digital_archivo_sii = "La funcion firmarDigitalmenteDocumento del SII genero el siguiente error : " & Stru_firma_digital_sii.mensajeerror & Stru_firma_digital_sii.codigoerror
                Exit Function
            End If
            url_file_load_firma = Stru_firma_digital_sii.url
            Service_firma_digital_archivo_sii = "YES"
        Catch ex As Exception
            Service_firma_digital_archivo_sii = "Inconsistencia general funcion Service_firma_digital_archivo_sii " & ex.Message
        End Try
    End Function
End Class
