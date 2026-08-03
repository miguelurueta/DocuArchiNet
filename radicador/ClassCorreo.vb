Imports System.Net.Mail
Imports System.IO
Imports iTextSharp.text.pdf
Imports iTextSharp.text
Imports System.Net.Mime
Imports MySql.Data.MySqlClient
Imports Ionic.Zip
Imports System.Collections.Generic
Imports iTextSharp.text.pdf.events.IndexEvents

Public Structure Config_Smtp
    Dim SERV_SMTP As String
    Dim PUERTO_SERV_SMTP As Integer
    Dim USUARIO_SMTP As String
    Dim PASW_SMTP As String
    Dim DOMINIO_SMTP As String
    Dim SMTP_TIEMPO As Integer
    Dim ESTADO_SSL As Integer
    Dim ESTADO_ENVIO As Integer
    Dim ESTADO_CREDENCIAL As Integer
    Dim ESTADO_BODY As Integer

End Structure
Public Class ClassCorreo

    Function Envio_Correo_recuperacion_pasword(ByVal Adic_mensaje() As String,
                                               ByVal Corre_dest As String,
                                               ByVal subyect As String) As String
        Try

            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_recuperacion_pasword = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_recuperacion_pasword = "YES"
                Exit Function
            End If
            Dim HTTEXT As String = ""
            'Dim Splite_Mensaje() As String = RefDatosConfig.BODY_CUERPO_MENSAJE.Split(vbCrLf)
            'For i As Integer = 0 To Splite_Mensaje.Length - 1
            '    HTTEXT = HTTEXT & "<p>" & Splite_Mensaje(i) & _
            '      "</p>"
            'Next
            'Erase Splite_Mensaje
            'Splite_Mensaje = Adic_mensaje.Split(vbCrLf)
            For i As Integer = 0 To Adic_mensaje.Length - 1
                HTTEXT = HTTEXT & "<p>" & Adic_mensaje(i) &
                  "</p>"
            Next
            Dim mensaj As String = "<html>" &
               "<head>" &
                  "<title>" &
                    "Recuperar contaseña de usuario " &
                  "</title>" &
               "</head>" &
               "<body>" &
                 "<p>" &
                  "<h1>" &
                     "<Font Color=Green>" &
                      " Sistema de gestión Docuarchi.net web " &
                    "</Font>" &
                  "</h1>" &
                 "</p>" &
                  HTTEXT &
                "<p>" &
                  "<h2>Servidor Smtp  </h2>" &
                 "</p>" &
                 "<p>" & "Mensaje enviado el " &
                        Now.ToShortDateString &
                       " a las " & Now.ToLongTimeString &
                 "</p>" &
               "</body>" &
            "</html>"
            '********************************
            'Envia correo electronico
            '********************************
            'Declaro la variable para enviar el correo
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            correo.Subject = subyect
            correo.To.Add(Corre_dest)
            correo.BodyEncoding = System.Text.Encoding.UTF8
            'correo.Body = RefDatosConfig.BODY_CUERPO_MENSAJE & mensaj
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
                'mensaj = RefDatosConfig.BODY_CUERPO_MENSAJ
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Envio_Correo_recuperacion_pasword = "YES"
        Catch ex As Exception
            Envio_Correo_recuperacion_pasword = "Inconsistencia general función Envio_Correo_recuperacion_pasword  " & ex.HelpLink & " " & ex.Source & " " & ex.InnerException.Message
        End Try
    End Function
    Function Envio_Correo_recuperacion_anualidad_pqr(ByVal Adic_mensaje() As String,
                                                     ByVal Corre_dest As String,
                                                     ByVal subyect As String) As String
        Try

            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_recuperacion_anualidad_pqr = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_recuperacion_anualidad_pqr = "YES"
                Exit Function
            End If
            Dim HTTEXT As String = ""
            'Dim Splite_Mensaje() As String = RefDatosConfig.BODY_CUERPO_MENSAJE.Split(vbCrLf)
            'For i As Integer = 0 To Splite_Mensaje.Length - 1
            '    HTTEXT = HTTEXT & "<p>" & Splite_Mensaje(i) & _
            '      "</p>"
            'Next
            'Erase Splite_Mensaje
            'Splite_Mensaje = Adic_mensaje.Split(vbCrLf)
            For i As Integer = 0 To Adic_mensaje.Length - 1
                HTTEXT = HTTEXT & "<p>" & Adic_mensaje(i) &
                  "</p>"
            Next
            Dim mensaj As String = "<html>" &
               "<head>" &
                  "<title>" &
                    "Notificación envío año de nacimiento registrado " &
                  "</title>" &
               "</head>" &
               "<body>" &
                 "<p>" &
                  "<h1>" &
                     "<Font Color=Green>" &
                      " Sistema PQRS " &
                    "</Font>" &
                  "</h1>" &
                 "</p>" &
                  HTTEXT &
                "<p>" &
                  "<h2>Servidor Smtp  </h2>" &
                 "</p>" &
                 "<p>" & "Mensaje enviado el " &
                        Now.ToShortDateString &
                       " a las " & Now.ToLongTimeString &
                 "</p>" &
               "</body>" &
            "</html>"
            '********************************
            'Envia correo electronico
            '********************************
            'Declaro la variable para enviar el correo
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            correo.Subject = subyect
            correo.To.Add(Corre_dest)
            correo.BodyEncoding = System.Text.Encoding.UTF8
            'correo.Body = RefDatosConfig.BODY_CUERPO_MENSAJE & mensaj
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
                'mensaj = RefDatosConfig.BODY_CUERPO_MENSAJ
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Envio_Correo_recuperacion_anualidad_pqr = "YES"
        Catch ex As Exception
            Envio_Correo_recuperacion_anualidad_pqr = "Inconsistencia general  " & ex.HelpLink & " " & ex.Source & " " & ex.InnerException.Message
        End Try
    End Function
    Function Envio_Correo_confirmacion_solicitud_aprobacion_respuesta(ByVal Adic_mensaje() As String,
                                                                      ByVal Corre_dest As String,
                                                                      ByVal subyect As String,
                                                                      ByVal matri_documentos() As String) As String
        Try

            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_confirmacion_solicitud_aprobacion_respuesta = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_confirmacion_solicitud_aprobacion_respuesta = "YES"
                Exit Function
            End If
            Dim HTTEXT As String = ""
            'Dim Splite_Mensaje() As String = RefDatosConfig.BODY_CUERPO_MENSAJE.Split(vbCrLf)
            'For i As Integer = 0 To Splite_Mensaje.Length - 1
            '    HTTEXT = HTTEXT & "<p>" & Splite_Mensaje(i) & _
            '      "</p>"
            'Next
            'Erase Splite_Mensaje
            'Splite_Mensaje = Adic_mensaje.Split(vbCrLf)
            Dim correo_usuario_solicitante As String = ""
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                               correo_usuario_solicitante)
            If Result <> "YES" Then
                Envio_Correo_confirmacion_solicitud_aprobacion_respuesta = Result
                Exit Function
            End If
            For i As Integer = 0 To Adic_mensaje.Length - 1
                HTTEXT = HTTEXT & "<p>" & Adic_mensaje(i) &
                  "</p>"
            Next
            HTTEXT = HTTEXT & "<p>" & "Cualquier inquietud responda al siguiete correo eletrónico" & correo_usuario_solicitante &
                  "</p>"
            Dim mensaj As String = "<html>" &
               "<head>" &
                  "<title>" &
                    "Notificación solicitud aprobación respuesta documento " &
                  "</title>" &
               "</head>" &
               "<body>" &
                 "<p>" &
                  "<h1>" &
                     "<Font Color=Green>" &
                      " Sistema Docuarchi.net Web gestión documental " &
                    "</Font>" &
                  "</h1>" &
                 "</p>" &
                  HTTEXT &
                "<p>" &
                  "<h2>Servidor Smtp  </h2>" &
                 "</p>" &
                 "<p>" & "Mensaje enviado el " &
                        Now.ToShortDateString &
                       " a las " & Now.ToLongTimeString &
                 "</p>" &
               "</body>" &
            "</html>"
            '********************************
            'Envia correo electronico
            '********************************
            'Declaro la variable para enviar el correo
            'Dim correo As New System.Net.Mail.MailMessage()
            'correo.From = New System.Net.Mail.MailAddress(Corre_dest)

            Dim splidest() As String = Nothing
            If InStr(Corre_dest, ",") > 0 Then
                splidest = Corre_dest.ToString.Split(",")
            End If
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            If splidest Is Nothing Then
                correo.To.Add(New System.Net.Mail.MailAddress(Corre_dest))
            Else
                For i As Integer = 0 To splidest.Length - 1
                    correo.To.Add(New System.Net.Mail.MailAddress(splidest(i)))
                Next
            End If
            correo.Subject = subyect
            If Not matri_documentos Is Nothing Then
                For i As Integer = 0 To matri_documentos.Length - 1
                    If Not matri_documentos(i) Is Nothing Then
                        Dim memStream As MemoryStream = New MemoryStream()
                        Dim fileStream As FileStream = File.OpenRead(matri_documentos(i))
                        memStream.SetLength(fileStream.Length)
                        fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                        fileStream.Close()
                        Dim file_inf As New FileInfo(matri_documentos(i))
                        correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos(i)), "application/" & Replace(file_inf.Extension, ".", "")))
                    End If

                Next
            End If

            'correo.To.Add(Corre_dest)
            'correo.To.Add(Corre_dest)
            'correo.BodyEncoding = System.Text.Encoding.UTF8
            ''correo.Body = RefDatosConfig.BODY_CUERPO_MENSAJE & mensaj
            'If RefDatosConfig.ESTADO_BODY = 0 Then
            '    correo.IsBodyHtml = False
            '    'mensaj = RefDatosConfig.BODY_CUERPO_MENSAJ
            'Else
            '    correo.IsBodyHtml = True
            'End If
            'correo.Body = mensaj
            ''Configuracion del servidor
            'Dim Servidor As New System.Net.Mail.SmtpClient
            'Servidor.Host = RefDatosConfig.SERV_SMTP
            'Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            'Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            'If RefDatosConfig.ESTADO_SSL = 0 Then
            '    Servidor.EnableSsl = False
            'Else
            '    Servidor.EnableSsl = True
            'End If

            'If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
            '    Servidor.UseDefaultCredentials = True
            'Else
            '    Servidor.UseDefaultCredentials = False
            '    Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP, _
            '     RefDatosConfig.PASW_SMTP)
            'End If
            'Servidor.Send(correo)
            correo.BodyEncoding = System.Text.Encoding.UTF8
            'correo.Body = RefDatosConfig.BODY_CUERPO_MENSAJE & mensaj
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
                'mensaj = RefDatosConfig.BODY_CUERPO_MENSAJE
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Envio_Correo_confirmacion_solicitud_aprobacion_respuesta = "YES"
        Catch ex As Exception
            Envio_Correo_confirmacion_solicitud_aprobacion_respuesta = "Inconsistencia general  " & ex.Message
        End Try
    End Function
    Function Envio_Correo_documento_compartido(ByVal Adic_mensaje() As String,
                                               ByVal Corre_dest As String,
                                               ByVal subyect As String) As String
        Try

            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_documento_compartido = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_documento_compartido = "YES"
                Exit Function
            End If
            Dim HTTEXT As String = ""
            For i As Integer = 0 To Adic_mensaje.Length - 1
                HTTEXT = HTTEXT & "<p>" & Adic_mensaje(i) &
                  "</p>"
            Next
            Dim mensaj As String = "<html>" &
               "<head>" &
                  "<title>" &
                    "Notificación documento compartido " &
                  "</title>" &
               "</head>" &
               "<body>" &
                 "<p>" &
                  "<h1>" &
                     "<Font Color=Green>" &
                      " Sistema Docuarchi.net Web gestión documental " &
                    "</Font>" &
                  "</h1>" &
                 "</p>" &
                  HTTEXT &
                "<p>" &
                  "<h2>Servidor Smtp  </h2>" &
                 "</p>" &
                 "<p>" & "Mensaje enviado el " &
                        Now.ToShortDateString &
                       " a las " & Now.ToLongTimeString &
                 "</p>" &
               "</body>" &
            "</html>"
            '********************************
            'Envia correo electronico
            '********************************
            'Declaro la variable para enviar el correo
            'Dim correo As New System.Net.Mail.MailMessage()
            'correo.From = New System.Net.Mail.MailAddress(Corre_dest)
            Dim splidest() As String = Nothing
            If InStr(Corre_dest, ",") > 0 Then
                splidest = Corre_dest.ToString.Split(",")
            End If
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            If splidest Is Nothing Then
                correo.To.Add(New System.Net.Mail.MailAddress(Corre_dest))
            Else
                For i As Integer = 0 To splidest.Length - 1
                    correo.To.Add(New System.Net.Mail.MailAddress(splidest(i)))
                Next
            End If
            correo.Subject = subyect
            correo.BodyEncoding = System.Text.Encoding.UTF8
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Envio_Correo_documento_compartido = "YES"
        Catch ex As Exception
            Envio_Correo_documento_compartido = "Inconsistencia general  " & ex.Message & " " & ex.HelpLink & " " & ex.Source & " " & ex.InnerException.Message
        End Try
    End Function
    Function Envio_Correo_confirmacion_recibido_solicitud(ByVal Radicado_respuesta As String,
                                                          ByVal Adic_mensaje() As String,
                                                          ByVal corre_dest As String,
                                                          ByVal matri_documentos() As String,
                                                          ByVal nombre_usuario_respuesta As String,
                                                          ByVal cargo_usuario_responde As String,
                                                          ByVal area_usuario_responde As String,
                                                          ByVal correo_responde As String,
                                                          ByVal estru As stru_envio,
                                                          ByVal Ruta_descarga As String,
                                                          ByRef matri_anexos() As String,
                                                          ByVal NotaConfirmacion As String) As String
        Try
            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim cantidad_archivo_anexo As Integer = 0
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            If Not matri_anexos Is Nothing Then
                cantidad_archivo_anexo = matri_anexos.Length
            End If
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_confirmacion_recibido_solicitud = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_confirmacion_recibido_solicitud = "YES"
                Exit Function
            End If
            Dim ruta_server As String = ""
            Dim tipo_notificacion As Integer = 1
            Dim correo_copia As String = ""
            Dim Refclas_ra_config As New Class_ra_config_notifica_correo
            Result = Refclas_ra_config.Solicita_estado_ruta_notificacion(ruta_server,
                                                                         tipo_notificacion,
                                                                         correo_copia)
            If Result <> "YES" Then
                Envio_Correo_confirmacion_recibido_solicitud = Result
                Exit Function
            End If
            Dim uri = ruta_server & "/workflow/Handler_image_scrip_wf.ashx?rut_image="
            Dim refclas As New ClassAdmonEmpresa
            Dim nombre_empresa As String = ""
            Result = refclas.Retorna_nombre_empresa_usuario_gestion(nombre_empresa,
                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Envio_Correo_confirmacion_recibido_solicitud = Result
                Exit Function
            End If
            Dim pat_confir As String = nombre_empresa & "|" & estru.ID_RESPUESTA_RADICADO & "|" & estru.RADICADO
            Dim pat_confir_encript As String = ""
            If tipo_notificacion = 2 Then
                Result = encriptacion.encript_md5(pat_confir,
                                            "7894561230!",
                                             pat_confir_encript)

                If Result <> "YES" Then
                    Envio_Correo_confirmacion_recibido_solicitud = Result
                    Exit Function
                End If
            End If
            Dim link_esta_distica As String = " a este correo <a href=mailto:" & correo_responde & "> Notificar a :  </a>" & correo_responde
            Dim uri_confir As String = ruta_server & "/Gestion/WebForm_gestion_confirma_recibido_usuario.aspx?path_confir=" & pat_confir_encript
            If tipo_notificacion = 2 Then
                link_esta_distica = "<a href=" & uri_confir & "> aquí </a>"
            End If
            Dim HTTEXT As String = "<p>" & nombre_empresa & " está confirmando el recibido de su radicado " & Radicado_respuesta & " de asunto : " & estru.ASUNTO & "</p>"
            HTTEXT = HTTEXT & "<p> <Font Color=Red> Para estadísticas es necesario que usted confirme el recibido " & link_esta_distica & "</Font> </p>"
            HTTEXT = HTTEXT & "<p style=font-size:12px>  Si no puede desplegar el link anterior copie en su navegador esta url :  </p> <p style=font-size:10px>" & uri_confir & " </p>"
            HTTEXT = HTTEXT & "<p>" & NotaConfirmacion &
                  "</p>"
            For i As Integer = 0 To Adic_mensaje.Length - 1
                HTTEXT = HTTEXT & "<p>" & Adic_mensaje(i) &
                  "</p>"
            Next
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue>" & "Respondio a su solicitud " & "</Font>" &
                 "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue>" & nombre_usuario_respuesta & "</Font>" &
                "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue> Cargo: " & cargo_usuario_responde & "</Font>" &
               "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue> Area: " & area_usuario_responde & "</Font>" &
               "</p>"
            HTTEXT = HTTEXT & "<p> " & "<h1> " &
            "<Font Color=Green>" &
            "Si usted tiene alguna duda o sugerencia con respecto a la confirmación, contacteme al siguiente correo electrónico " & correo_responde &
             "</Font>" &
              "</h1>" &
            "</p>"

            If Not matri_documentos Is Nothing Then
                HTTEXT = HTTEXT & "<p> Con este correo electronico se adjuntan " & matri_documentos.Length + cantidad_archivo_anexo & " archivo(s) " &
                             "</p>"
            End If
            HTTEXT = HTTEXT & "<p> Detalle solicitud radicado numero " & estru.RADICADO & "</p>" &
                    "<table>" &
                       "<tr>" &
                          "<td> Su petición se recepciono con este radicado : </td>" &
                          "<td>" & estru.RADICADO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Tipo tramite para envío : </td> " &
                            "<td>" & estru.TRAMITE_DOCUMENTO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Respuesta dirigida a : </td>" &
                            "<td>" & estru.DESTINATARIO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Se respondio con el radicado numero  : </td> " &
                            "<td> " & estru.RADICADO_RESPUESTA & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha Limite respuesta : </td> " &
                            "<td>" & estru.FECHA_VENCE & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha respuesta del usuario de la entidad : </td>" &
                             "<td>" & estru.FECHA_RESPUETA & "</td>" &
                       "</tr>" &
                       "<tr>" &
                           "<td> Otros datos : </td> " &
                            "<td>" & " Guía : " & estru.GUIA_ENVIO & "  Empresa " & estru.EMPRESA_ENVIO & "</td>" &
                       "</tr>" &
                    "</table>"
            If tipo_notificacion = 2 Then
                HTTEXT = HTTEXT & "<p> Documentos relacionados   </p>" &
                "<table>"
                If Not matri_documentos Is Nothing Then
                    For i As Integer = 0 To matri_documentos.Length - 1
                        Dim has_huella_md5_escript As String = ""
                        Result = encriptacion.encript_md5(matri_documentos(i),
                                                        "7894561230!",
                                                        has_huella_md5_escript)

                        If Result <> "YES" Then
                            Envio_Correo_confirmacion_recibido_solicitud = Result
                            Exit Function
                        End If
                        HTTEXT = HTTEXT &
                            "<tr>" &
                              "<td><a href=" & uri & has_huella_md5_escript & " > Descarga respuesta aquí  </td>" &
                              "<td>  <p " & " style=font-size:12px> o copie esta url en su navegador :  </p>" &
                              "<td>  <p " & " style=font-size:10px> " & uri & has_huella_md5_escript & "  </p>" &
                           "</tr>"
                    Next
                End If
                If Not matri_anexos Is Nothing Then
                    For i As Integer = 0 To matri_anexos.Length - 1
                        Dim has_huella_md5_escript As String = ""
                        Result = encriptacion.encript_md5(matri_anexos(i),
                                                        "7894561230!",
                                                        has_huella_md5_escript)

                        If Result <> "YES" Then
                            Envio_Correo_confirmacion_recibido_solicitud = Result
                            Exit Function
                        End If
                        HTTEXT = HTTEXT &
                          "<tr>" &
                              "<td><a href=" & uri & has_huella_md5_escript & " > Descarga anexo aquí  </td>" &
                              "<td>  <p " & " style=font-size:12px> o copie esta url en su navegador :  </p>" &
                              "<td>  <p " & " style=font-size:10px> " & uri & has_huella_md5_escript & "  </p>" &
                           "</tr>"
                    Next
                End If
                HTTEXT = HTTEXT & "</table>"
            End If
            Dim mensaj As String = "<html>" &
                  "<head>" &
                     "<title>" &
                       "Respuesta a petición " &
                     "</title>" &
                  "</head>" &
                  "<body>" &
                    "<p>" &
                     "<h1>" &
                        "<Font Color=Green>" &
                         " Sistema de gestión Docuarchi.net web" &
                       "</Font>" &
                     "</h1>" &
                    "</p>" &
                     HTTEXT &
                   "<p>" &
                     "<h2>Servidor Smtp  </h2>" &
                    "</p>" &
                    "<p>" & "Mensaje enviado el " &
                           Now.ToShortDateString &
                          " a las " & Now.ToLongTimeString &
                    " Este correo es automatico no renvie informnación a este correo</p>" &
                  "</body>" &
               "</html>"

            '********************************
            'Envia correo electrónico
            '********************************
            Dim splidest() As String = Nothing
            If InStr(corre_dest, ",") > 0 Then
                splidest = corre_dest.ToString.Split(",")
            End If
            Dim ref_trim As String = ""
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            If splidest Is Nothing Then
                correo.To.Add(New System.Net.Mail.MailAddress(corre_dest, ""))
            Else
                For i As Integer = 0 To splidest.Length - 1
                    If Trim(splidest(i)) <> "" And Trim(splidest(i)) <> " " Then
                        If InStr(Trim(splidest(i)), "|") > 0 Then
                            Dim split_correo() As String = Trim(splidest(i)).Split("|")
                            correo.To.Add(New System.Net.Mail.MailAddress(split_correo(0), split_correo(1)))
                        Else
                            correo.To.Add(New System.Net.Mail.MailAddress(Trim(splidest(i)), ""))
                        End If

                    End If
                Next

            End If
            correo.CC.Add(correo_responde)
            If correo_copia <> "" Then
                correo.CC.Add(correo_copia)
            End If
            If tipo_notificacion = 1 Then
                If Not matri_documentos Is Nothing Then
                    If estru.ID_TIPO_DOC_RESPUESTA = 1 Then
                        For i As Integer = 0 To matri_documentos.Length - 1
                            Dim file_infe As New FileInfo(matri_documentos(i))
                            If file_infe.Extension <> ".PDF" Then
                                Dim memStream As MemoryStream = New MemoryStream()
                                'Dim OB As New localhost.Service
                                'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
                                Dim fil_estrean As Object = Nothing
                                Dim archi_descarga As String = ""
                                'Result = OB.Descarga_doxc_formatos_archivo_web_application(matri_documentos(i),
                                '                                                           fil_estrean,
                                '                                                           "PDF", 1)
                                'If Result <> "YES" Then
                                '    Envio_Correo_confirmacion_recibido_solicitud = Result
                                '    Exit Function
                                'End If
                                Dim refclas_gest_resp As New ClassRaEnvioCorrespondencia
                                archi_descarga = Ruta_descarga & "corre_envi.pdf"
                                If File.Exists(archi_descarga) Then
                                    Kill(archi_descarga)
                                End If
                                Result = refclas_gest_resp.SaveFile(archi_descarga,
                                                                    fil_estrean)
                                If Result <> "YES" Then
                                    Envio_Correo_confirmacion_recibido_solicitud = Result
                                    Exit Function
                                End If

                                Dim fileStream As FileStream = File.OpenRead(archi_descarga)
                                memStream.SetLength(fileStream.Length)
                                fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                                fileStream.Close()
                                Dim file_inf As New FileInfo(archi_descarga)
                                correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(archi_descarga), "application/" & Replace(file_inf.Extension, ".", "")))
                            Else
                                Dim memStream As MemoryStream = New MemoryStream()
                                Dim fileStream As FileStream = File.OpenRead(matri_documentos(i))
                                memStream.SetLength(fileStream.Length)
                                fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                                fileStream.Close()
                                Dim file_inf As New FileInfo(matri_documentos(i))
                                correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos(i)), "application/" & Replace(file_inf.Extension, ".", "")))
                            End If

                        Next
                    Else
                        If matri_documentos.Length > 1 Then
                            Using zip As New ZipFile()
                                For i As Integer = 0 To matri_documentos.Length - 1
                                    If i = 0 Then
                                        zip.AddFile(matri_documentos(i), "FilesDocuarchi")
                                    Else
                                        zip.AddFile(matri_documentos(i), "FilesDocuarchi")
                                    End If
                                Next
                                Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                                Dim archivo_salida_zip As String = Ruta_descarga & zipName
                                If File.Exists(archivo_salida_zip) Then
                                    Kill(archivo_salida_zip)
                                End If
                                zip.Save(archivo_salida_zip)
                                Dim memStream As MemoryStream = New MemoryStream()
                                Dim fileStream As FileStream = File.OpenRead(archivo_salida_zip)
                                memStream.SetLength(fileStream.Length)
                                fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                                fileStream.Close()
                                correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(archivo_salida_zip), "application/zip"))
                            End Using
                        Else
                            Dim file_inf As New FileInfo(matri_documentos(0))
                            Dim memStream As MemoryStream = New MemoryStream()
                            Dim fileStream As FileStream = File.OpenRead(matri_documentos(0))
                            memStream.SetLength(fileStream.Length)
                            fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                            fileStream.Close()
                            correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos(0)), "application/" & Replace(file_inf.Extension, ".", "")))
                        End If

                    End If

                End If
                If Not matri_anexos Is Nothing Then
                    For i As Integer = 0 To matri_anexos.Length - 1
                        Dim memStream As MemoryStream = New MemoryStream()
                        Dim fileStream As FileStream = File.OpenRead(matri_anexos(i))
                        memStream.SetLength(fileStream.Length)
                        fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                        fileStream.Close()
                        Dim file_inf As New FileInfo(matri_anexos(i))
                        correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_anexos(i)), "application/" & Replace(file_inf.Extension, ".", "")))
                    Next
                End If
            End If
            correo.Subject = "Confirmación de recibido a petición " & estru.RADICADO & " Asunto " & estru.ASUNTO
            'correo.To.Add(corre_dest)
            correo.BodyEncoding = System.Text.Encoding.UTF8
            'correo.Body = RefDatosConfig.BODY_CUERPO_MENSAJE & mensaj
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
                'mensaj = RefDatosConfig.BODY_CUERPO_MENSAJE
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Result = Registra_log_respuesta_notificacion_radicado(estru, mensaj, corre_dest)
            If Result <> "YES" Then
                Envio_Correo_confirmacion_recibido_solicitud = "Se envio el correo pero no se pudo registrar el estado, " & Result
                Exit Function
            End If
            Envio_Correo_confirmacion_recibido_solicitud = "YES"
        Catch ex As Exception
            Dim obpklink As Object
            If Not ex.HelpLink Is Nothing Then
                obpklink = ex.HelpLink
            Else
                obpklink = ""
            End If
            Dim obInnerException As Object
            If Not ex.HelpLink Is Nothing Then
                obInnerException = ex.InnerException.Message
            Else
                obInnerException = ""
            End If
            Envio_Correo_confirmacion_recibido_solicitud = "Inconsistencia general funcion Envio_Correo_confirmacion_recibido_solicitud   " & ex.Message
        End Try
    End Function
    Function Envio_Correo_confirma_Archivado_tramite(ByVal Adic_mensaje() As String,
                                                     ByVal corre_dest As String,
                                                     ByVal radicado As String,
                                                     ByVal id_usuario_gestion As Integer,
                                                     ByVal id_respuesta As Integer,
                                                     ByVal estru As stru_envio) As String
        Try
            Dim Result As String = ""
            Dim matri_resp() As String = Nothing
            Dim matri_anexos() As String = Nothing
            Dim matri_documentos() As String = Nothing
            Dim z As Integer = 0
            Dim Classgestionrespuesta As New Classgestionrespuesta
            Dim matri_mensaje() As String = {""}
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Dim nombre_area As String = ""
            Dim cargo_usuario_gestion_responde As String = ""
            Dim nombre_usuario_gestion_responde As String = ""
            Dim correo_usuario_gestion_responde As String = ""
            Result = Reclas_remit_dest_interno.Retorna_datos_caracterizacion_usuario_gestion(id_usuario_gestion,
                                                                                             nombre_usuario_gestion_responde,
                                                                                             cargo_usuario_gestion_responde,
                                                                                             correo_usuario_gestion_responde)
            If Result <> "YES" Then
                Envio_Correo_confirma_Archivado_tramite = Result
                Exit Function
            End If
            Result = Reclas_remit_dest_interno.Solicita_id_area_nombre_area_destinatario(id_usuario_gestion,
                                                                                         0,
                                                                                         nombre_area)
            If Result <> "YES" Then
                Envio_Correo_confirma_Archivado_tramite = Result
                Exit Function
            End If
            Result = Classgestionrespuesta.Genera_zip_documento_anexo(id_respuesta,
                                                                      matri_anexos)
            If Result <> "YES" Then
                Envio_Correo_confirma_Archivado_tramite = Result
                Exit Function
            End If

            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim cantidad_archivo_anexo As Integer = 0
            Dim RefDatosConfig As New Config_Smtp
            If Not matri_anexos Is Nothing Then
                cantidad_archivo_anexo = matri_anexos.Length
            End If
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_confirma_Archivado_tramite = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_confirma_Archivado_tramite = "YES"
                Exit Function
            End If
            Dim refclas As New ClassAdmonEmpresa
            Dim nombre_empresa As String = ""
            Result = refclas.Retorna_nombre_empresa_usuario_gestion(nombre_empresa,
                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Envio_Correo_confirma_Archivado_tramite = Result
                Exit Function
            End If
            Dim HTTEXT As String = "<p>" & nombre_empresa & " Archivó su solicitud de radicación " & radicado & " de asunto : " & estru.ASUNTO & "</p>"
            HTTEXT = HTTEXT & "<p> <Font Color=Red> Para estadísticas es necesario que usted confirme el recibido a este correo  " & "<a href=mailto:" & correo_usuario_gestion_responde & "> Notificar a :  </a>" & correo_usuario_gestion_responde & "</Font> </p>"
            If Not Adic_mensaje Is Nothing Then
                For i As Integer = 0 To Adic_mensaje.Length - 1
                    HTTEXT = HTTEXT & "<p>" & Adic_mensaje(i) &
                      "</p>"
                Next
            End If
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue>" & "Archivo su solicitud " & "</Font>" &
                 "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue>" & nombre_usuario_gestion_responde & "</Font>" &
                "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue> Cargo: " & cargo_usuario_gestion_responde & "</Font>" &
               "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue> Area: " & nombre_area & "</Font>" &
               "</p>"
            HTTEXT = HTTEXT & "<p> " & "<h1> " &
            "<Font Color=Green>" &
            "Si usted tiene alguna duda o sugerencia con respecto a la confirmación, contacteme al siguiente correo electrónico " & correo_usuario_gestion_responde &
             "</Font>" &
              "</h1>" &
            "</p>"

            HTTEXT = HTTEXT & "<p> Detalle solicitud radicado numero " & estru.RADICADO & "</p>" &
                    "<table>" &
                       "<tr>" &
                          "<td> Su petición se recepcionó con este radicado : </td>" &
                          "<td>" & estru.RADICADO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Tipo tramite : </td> " &
                            "<td>" & estru.TRAMITE_DOCUMENTO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Respuesta dirigida a : </td>" &
                            "<td>" & estru.DESTINATARIO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha Limite respuesta : </td> " &
                            "<td>" & estru.FECHA_VENCE & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha de archivado del tramite : </td>" &
                             "<td>" & estru.FECHA_RESPUETA & "</td>" &
                       "</tr>" &
                    "</table>"
            Dim mensaj As String = "<html>" &
                  "<head>" &
                     "<title>" &
                       "Archivado de petición " &
                     "</title>" &
                  "</head>" &
                  "<body>" &
                    "<p>" &
                     "<h1>" &
                        "<Font Color=Green>" &
                         " Sistema de gestión Docuarchi.net web" &
                       "</Font>" &
                     "</h1>" &
                    "</p>" &
                     HTTEXT &
                   "<p>" &
                     "<h2>Servidor Smtp  </h2>" &
                    "</p>" &
                    "<p>" & "Mensaje enviado el " &
                           Now.ToShortDateString &
                          " a las " & Now.ToLongTimeString &
                    " Este correo es automatico no renvie informnación a este correo</p>" &
                  "</body>" &
               "</html>"

            '********************************
            'Envia correo electrónico
            '********************************
            Dim splidest() As String = Nothing
            If InStr(corre_dest, ",") > 0 Then
                splidest = corre_dest.ToString.Split(",")
            End If
            Dim ref_trim As String = ""
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            If splidest Is Nothing Then
                correo.To.Add(New System.Net.Mail.MailAddress(corre_dest, ""))
            Else
                For i As Integer = 0 To splidest.Length - 1
                    If Trim(splidest(i)) <> "" And Trim(splidest(i)) <> " " Then
                        If InStr(Trim(splidest(i)), "|") > 0 Then
                            Dim split_correo() As String = Trim(splidest(i)).Split("|")
                            correo.To.Add(New System.Net.Mail.MailAddress(split_correo(0), split_correo(1)))
                        Else
                            correo.To.Add(New System.Net.Mail.MailAddress(Trim(splidest(i)), ""))
                        End If

                    End If
                Next
                'correo.From = New System.Net.Mail.MailAddress(Trim(ref_trim))
            End If
            correo.CC.Add(correo_usuario_gestion_responde)
            If Not matri_documentos Is Nothing Then
                If estru.ID_TIPO_DOC_RESPUESTA = 1 Then
                    For i As Integer = 0 To matri_documentos.Length - 1
                        Dim file_infe As New FileInfo(matri_documentos(i))
                        If file_infe.Extension <> ".PDF" Then
                            Dim memStream As MemoryStream = New MemoryStream()
                            'Dim OB As New localhost.Service
                            'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
                            Dim fil_estrean As Object = Nothing
                            Dim archi_descarga As String = ""
                            'Result = OB.Descarga_doxc_formatos_archivo_web_application(matri_documentos(i), fil_estrean, "PDF", 1)
                            'If Result <> "YES" Then
                            '    Envio_Correo_confirma_Archivado_tramite = Result
                            '    Exit Function
                            'End If
                            Dim refclas_gest_resp As New ClassRaEnvioCorrespondencia
                            archi_descarga = "" & "corre_envi.pdf"
                            If File.Exists(archi_descarga) Then
                                Kill(archi_descarga)
                            End If
                            Result = refclas_gest_resp.SaveFile(archi_descarga, fil_estrean)
                            If Result <> "YES" Then
                                Envio_Correo_confirma_Archivado_tramite = Result
                                Exit Function
                            End If

                            Dim fileStream As FileStream = File.OpenRead(archi_descarga)
                            memStream.SetLength(fileStream.Length)
                            fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                            fileStream.Close()
                            Dim file_inf As New FileInfo(archi_descarga)
                            correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(archi_descarga), "application/" & Replace(file_inf.Extension, ".", "")))
                        Else
                            Dim memStream As MemoryStream = New MemoryStream()
                            Dim fileStream As FileStream = File.OpenRead(matri_documentos(i))
                            memStream.SetLength(fileStream.Length)
                            fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                            fileStream.Close()
                            Dim file_inf As New FileInfo(matri_documentos(i))
                            correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos(i)), "application/" & Replace(file_inf.Extension, ".", "")))
                        End If

                    Next
                Else
                    If matri_documentos.Length > 1 Then
                        Using zip As New ZipFile()
                            For i As Integer = 0 To matri_documentos.Length - 1
                                If i = 0 Then
                                    zip.AddFile(matri_documentos(i), "FilesDocuarchi")
                                Else
                                    zip.AddFile(matri_documentos(i), "FilesDocuarchi")
                                End If
                            Next
                            Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                            Dim archivo_salida_zip As String = "" & zipName
                            If File.Exists(archivo_salida_zip) Then
                                Kill(archivo_salida_zip)
                            End If
                            zip.Save(archivo_salida_zip)
                            Dim memStream As MemoryStream = New MemoryStream()
                            Dim fileStream As FileStream = File.OpenRead(archivo_salida_zip)
                            memStream.SetLength(fileStream.Length)
                            fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                            fileStream.Close()
                            correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(archivo_salida_zip), "application/zip"))
                        End Using
                    Else
                        Dim file_inf As New FileInfo(matri_documentos(0))
                        Dim memStream As MemoryStream = New MemoryStream()
                        Dim fileStream As FileStream = File.OpenRead(matri_documentos(0))
                        memStream.SetLength(fileStream.Length)
                        fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                        fileStream.Close()
                        correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos(0)), "application/" & Replace(file_inf.Extension, ".", "")))
                    End If

                End If

            End If
            If Not matri_anexos Is Nothing Then
                For i As Integer = 0 To matri_anexos.Length - 1
                    Dim memStream As MemoryStream = New MemoryStream()
                    Dim fileStream As FileStream = File.OpenRead(matri_anexos(i))
                    memStream.SetLength(fileStream.Length)
                    fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                    fileStream.Close()
                    Dim file_inf As New FileInfo(matri_anexos(i))
                    correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_anexos(i)), "application/" & Replace(file_inf.Extension, ".", "")))
                Next
            End If
            correo.Subject = "Confirmación de recibido a petición " & estru.RADICADO & " Asunto " & estru.ASUNTO
            correo.BodyEncoding = System.Text.Encoding.UTF8
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Result = Registra_log_respuesta_notificacion_radicado(estru, mensaj, corre_dest)
            If Result <> "YES" Then
                Envio_Correo_confirma_Archivado_tramite = "Se envio el correo pero no se pudo registrar el estado, " & Result
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim update As String = "Update ra_respuesta_radicado set estado_envio_correo=1 where ID_RESPUESTA_RADICADO=" & id_respuesta
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(update)
            Envio_Correo_confirma_Archivado_tramite = "YES"
        Catch ex As Exception
            Dim obpklink As Object
            If Not ex.HelpLink Is Nothing Then
                obpklink = ex.HelpLink
            Else
                obpklink = ""
            End If
            Dim obInnerException As Object
            If Not ex.HelpLink Is Nothing Then
                obInnerException = ex.InnerException.Message
            Else
                obInnerException = ""
            End If
            Envio_Correo_confirma_Archivado_tramite = "Inconsistencia general funcion Envio_Correo_confirmacion_recibido_solicitud   " & ex.Message
        End Try
    End Function
    Function Envio_Correo_confirma_traslado_tramite(ByVal Class_config_general_service_ As List(Of Class_config_general_service),
                                                    ByVal corre_dest As String,
                                                    ByVal radicado As String,
                                                    ByVal id_usuario_gestion As Integer,
                                                    ByVal id_respuesta As Integer,
                                                    ByVal estru As stru_envio,
                                                    ByVal nombre_entidad As String,
                                                    ByVal motivo As String,
                                                    ByVal stru_paramter_image() As stru_paramter_image,
                                                    ByVal ruta_server As String) As String
        Try
            Dim Result As String = ""
            Dim Classgestionrespuesta As New Classgestionrespuesta
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Dim nombre_area As String = ""
            Dim cargo_usuario_gestion_responde As String = ""
            Dim nombre_usuario_gestion_responde As String = ""
            Dim correo_usuario_gestion_responde As String = ""
            Result = Reclas_remit_dest_interno.Retorna_datos_caracterizacion_usuario_gestion(id_usuario_gestion,
                                                                                             nombre_usuario_gestion_responde,
                                                                                             cargo_usuario_gestion_responde,
                                                                                             correo_usuario_gestion_responde)
            If Result <> "YES" Then
                Envio_Correo_confirma_traslado_tramite = Result
                Exit Function
            End If
            Result = Reclas_remit_dest_interno.Solicita_id_area_nombre_area_destinatario(id_usuario_gestion,
                                                                                         0,
                                                                                         nombre_area)
            If Result <> "YES" Then
                Envio_Correo_confirma_traslado_tramite = Result
                Exit Function
            End If
            Dim HTTEXT_ As String = ""
            If Not stru_paramter_image Is Nothing Then
                If ruta_server <> "" Then
                    HTTEXT_ = HTTEXT_ & "<p> Documentos relacionados del traslado  </p>" &
                        "<table>"
                    For i As Integer = 0 To stru_paramter_image.Length - 1
                        HTTEXT_ = HTTEXT_ & "<tr>" & "<td><a href=" & stru_paramter_image(i).RUTA_IMAGEN_URL & "> Descarga documento aquí </a></td>" & "</tr>"
                    Next
                    HTTEXT_ = HTTEXT_ & "</table>"
                Else

                End If

            End If
            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim cantidad_archivo_anexo As Integer = 0
            Dim RefDatosConfig As New Config_Smtp

            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_confirma_traslado_tramite = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_confirma_traslado_tramite = "YES"
                Exit Function
            End If
            Dim refclas As New ClassAdmonEmpresa
            Dim nombre_empresa As String = ""
            Result = refclas.Retorna_nombre_empresa_usuario_gestion(nombre_empresa,
                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Envio_Correo_confirma_traslado_tramite = Result
                Exit Function
            End If
            Dim HTTEXT As String = "<p>" & nombre_empresa & " Traslado su solicitud de radicación " & radicado & " de asunto : " & estru.ASUNTO & " A la entidad " & nombre_entidad & "</p>"
            HTTEXT = HTTEXT & "<p> Su solicitud se traslado teniendo en cuenta lo siguiente :  " & motivo & " </p>"
            HTTEXT = HTTEXT & "<h1> Detalle del traslado   </h1>"
            HTTEXT = HTTEXT & "<table>"
            For i As Integer = 0 To Class_config_general_service_.Count - 1
                Dim tipo_tipo_error As String = ""
                '---------agrega nombre campo
                HTTEXT = HTTEXT & "<tr>"
                If Class_config_general_service_(i).aleas_campo = "" Then
                    HTTEXT = HTTEXT & "<td>" & Class_config_general_service_(i).name_campo & "</td>"
                Else
                    HTTEXT = HTTEXT & "<td>" & Class_config_general_service_(i).aleas_campo & "</td>"
                End If
                '------- Agrega valor
                If Class_config_general_service_(i).alow_tipo_value = 0 Then
                    HTTEXT = HTTEXT & "<td>" & Class_config_general_service_(i).texto_campo & "</td>"
                Else
                    HTTEXT = HTTEXT & "<td>" & Class_config_general_service_(i).value_campo & "</td>"
                End If

                HTTEXT = HTTEXT & "</tr>"
            Next
            HTTEXT = HTTEXT & "</table>"
            HTTEXT = HTTEXT & HTTEXT_
            HTTEXT = HTTEXT & "<h1> Funcionario que traslada  " & " </h1>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue>" & "Traslado su solicitud " & "</Font>" &
                 "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue>" & nombre_usuario_gestion_responde & "</Font>" &
                "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue> Cargo: " & cargo_usuario_gestion_responde & "</Font>" &
               "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue> Area: " & nombre_area & "</Font>" &
               "</p>"


            HTTEXT = HTTEXT & "<h1> Detalle solicitud radicado numero : " & estru.RADICADO & "</h1>" &
                    "<table>" &
                       "<tr>" &
                          "<td> Su petición se recepcionó con este radicado : </td>" &
                          "<td>" & estru.RADICADO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Tipo tramite : </td> " &
                            "<td>" & estru.TRAMITE_DOCUMENTO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Respuesta dirigida a : </td>" &
                            "<td>" & estru.DESTINATARIO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha Limite respuesta : </td> " &
                            "<td>" & estru.FECHA_VENCE & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha de traslado : </td>" &
                             "<td>" & estru.FECHA_RESPUETA & "</td>" &
                       "</tr>" &
                    "</table>"
            HTTEXT = HTTEXT & "<p> " & "<h1> " &
            "<Font Color=Green>" &
            "Si usted tiene alguna duda o sugerencia con respecto a la confirmación, contacteme al siguiente correo electrónico " & correo_usuario_gestion_responde &
             "</Font>" &
              "</h1>" &
            "</p>"
            Dim mensaj As String = "<html>" &
                  "<head>" &
                     "<title>" &
                       "Traslado de petición " &
                     "</title>" &
                  "</head>" &
                  "<body>" &
                    "<p>" &
                     "<h1>" &
                        "<Font Color=Green>" &
                         " Sistema de gestión Docuarchi.net web" &
                       "</Font>" &
                     "</h1>" &
                    "</p>" &
                     HTTEXT &
                   "<p>" &
                     "<h2>Servidor Smtp  </h2>" &
                    "</p>" &
                    "<p>" & "Mensaje enviado el " &
                           Now.ToShortDateString &
                          " a las " & Now.ToLongTimeString &
                    " Este correo es automatico no renvie informnación a este correo</p>" &
                  "</body>" &
               "</html>"

            '********************************
            'Envia correo electrónico
            '********************************

            Dim ref_trim As String = ""
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")

            correo.CC.Add(corre_dest)
            correo.Subject = "Traslado de solicitud " & estru.RADICADO & " Asunto " & estru.ASUNTO
            correo.BodyEncoding = System.Text.Encoding.UTF8
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Result = Registra_log_respuesta_notificacion_radicado(estru, mensaj, corre_dest)
            If Result <> "YES" Then
                Envio_Correo_confirma_traslado_tramite = "Se envio el correo pero no se pudo registrar el estado, " & Result
                Exit Function
            End If
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim update As String = "Update ra_respuesta_radicado set estado_envio_correo=1 where ID_RESPUESTA_RADICADO=" & id_respuesta
            Result = Ref_Car_Conec.SELECTION_INSERT_COMMAND(update)
            Envio_Correo_confirma_traslado_tramite = "YES"
        Catch ex As Exception
            Dim obpklink As Object
            If Not ex.HelpLink Is Nothing Then
                obpklink = ex.HelpLink
            Else
                obpklink = ""
            End If
            Dim obInnerException As Object
            If Not ex.HelpLink Is Nothing Then
                obInnerException = ex.InnerException.Message
            Else
                obInnerException = ""
            End If
            Envio_Correo_confirma_traslado_tramite = "Inconsistencia general funcion Envio_Correo_confirma_traslado_tramite   " & ex.Message
        End Try
    End Function
    Function Notificacion_error_registro_pqrf_administrador(ByVal Ra_log_error_pqr_publico() As ra_log_error_pqr_publico,
                                                            ByVal correo_usuario_admon As String) As String
        Try
            Dim HTTEXT As String = "<p> Inconsitencia PQRF  RADICADO " & Ra_log_error_pqr_publico(0).consecutivo_radicado & " de asunto : " & Ra_log_error_pqr_publico(0).asunto & "</p>"
            HTTEXT = HTTEXT & "<p> USUARIO PETICIONARIO " & Ra_log_error_pqr_publico(0).nombre_peticionario & "</p>"
            HTTEXT = HTTEXT & "<p> CORREO PETICIONARIO " & Ra_log_error_pqr_publico(0).correo_peticionario & "</p>"
            HTTEXT = HTTEXT & "<br>  </br>"
            HTTEXT = HTTEXT & "<br>  </br>"
            HTTEXT = HTTEXT & "<p> Detalle inconsistencias radicado numero " & Ra_log_error_pqr_publico(0).consecutivo_radicado & "</p>"
            HTTEXT = HTTEXT & "<table>"
            HTTEXT = HTTEXT & "<tr>"
            HTTEXT = HTTEXT & "<td>" & "TIPO ERROR" & "</td>"
            HTTEXT = HTTEXT & "<td>" & "DESCRIPCION ERROR" & "</td>"
            HTTEXT = HTTEXT & "<td>" & "FECHA ERROR" & "</td>"
            HTTEXT = HTTEXT & "</tr>"
            For i As Integer = 0 To Ra_log_error_pqr_publico.Length - 1
                Dim tipo_tipo_error As String = ""
                If Ra_log_error_pqr_publico(i).tipo_error = 1 Then
                    tipo_tipo_error = "GUARDANDO ARCHIVO  PQR"
                End If
                If Ra_log_error_pqr_publico(i).tipo_error = 2 Then
                    tipo_tipo_error = "GUARDANDO ARCHIVO ANEXO"
                End If
                If Ra_log_error_pqr_publico(i).tipo_error = 3 Then
                    tipo_tipo_error = "ASIGNANDO RADICADO"
                End If
                If Ra_log_error_pqr_publico(i).tipo_error = 4 Then
                    tipo_tipo_error = "NOTIFICACION DE CORREO A USUARIO PETICIONARIO"
                End If
                '---------agrega tipo error 
                HTTEXT = HTTEXT & "<tr>"
                HTTEXT = HTTEXT & "<td>" & tipo_tipo_error & "</td>"
                '------- Agrega el código del error
                HTTEXT = HTTEXT & "<td>" & Ra_log_error_pqr_publico(i).error_code & "</td>"
                '-------- Agrega fecha error 
                HTTEXT = HTTEXT & "<td>" & Ra_log_error_pqr_publico(i).date_registro & "</td>"
                HTTEXT = HTTEXT & "</tr>"
            Next
            HTTEXT = HTTEXT & "</table>"
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Notificacion_error_registro_pqrf_administrador = Result
                Exit Function
            End If
            Dim mensaj As String = "<html>" &
                  "<head>" &
                     "<title>" &
                       "Alerta error registro PQRF " &
                     "</title>" &
                  "</head>" &
                  "<body>" &
                    "<p>" &
                     "<h1>" &
                        "<Font Color=Green>" &
                         " Sistema de gestión Docuarchi.net web" &
                       "</Font>" &
                     "</h1>" &
                     "<div>" &
                     HTTEXT &
                     "</div>" &
                     "<h2>Servidor Smtp  </h2>" &
                    "</p>" &
                    "<p>" & "Mensaje enviado el " &
                           Now.ToShortDateString &
                          " a las " & Now.ToLongTimeString &
                    " Este correo es automatico no renvie informnación a este correo</p>" &
                  "</body>" &
               "</html>"
            Dim correo As New System.Net.Mail.MailMessage()
            correo.Subject = "INCONSISTENCIA PQRF  RADICADO " & Ra_log_error_pqr_publico(0).consecutivo_radicado
            correo.BodyEncoding = System.Text.Encoding.UTF8
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False

            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Dim ref_trim As String = ""
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            correo.To.Add(New System.Net.Mail.MailAddress(correo_usuario_admon, ""))
            Servidor.Send(correo)
            Notificacion_error_registro_pqrf_administrador = "YES"
        Catch ex As Exception
            Dim obpklink As Object
            If Not ex.HelpLink Is Nothing Then
                obpklink = ex.HelpLink
            Else
                obpklink = ""
            End If
            Dim obInnerException As Object
            If Not ex.HelpLink Is Nothing Then
                obInnerException = ex.InnerException.Message
            Else
                obInnerException = ""
            End If
            Notificacion_error_registro_pqrf_administrador = "Inconsistencia general funcion Envio_Correo_respuesta_documento  " & ex.Message

        End Try
    End Function
    Function Envio_Correo_respuesta_documento(ByVal Radicado_respuesta As String,
                                              ByVal Adic_mensaje() As String,
                                              ByVal corre_dest As String,
                                              ByVal matri_documentos_respuesta() As String,
                                              ByVal nombre_usuario_respuesta As String,
                                              ByVal cargo_usuario_responde As String,
                                              ByVal area_usuario_responde As String,
                                              ByVal correo_responde As String,
                                              ByVal estru As stru_envio,
                                              ByVal Ruta_descarga As String,
                                              ByRef matri_anexos() As String) As String
        Try
            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim cantidad_archivo_anexo As Integer = 0
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            If Not matri_anexos Is Nothing Then
                cantidad_archivo_anexo = matri_anexos.Length
            End If
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_respuesta_documento = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_respuesta_documento = "YES"
                Exit Function
            End If
            Dim ruta_server As String = ""
            Dim tipo_notificacion As Integer = 1
            Dim correo_copia As String = ""
            Dim Refclas_ra_config As New Class_ra_config_notifica_correo
            Result = Refclas_ra_config.Solicita_estado_ruta_notificacion(ruta_server,
                                                                         tipo_notificacion,
                                                                         correo_copia)
            If Result <> "YES" Then
                Envio_Correo_respuesta_documento = Result
                Exit Function
            End If
            Dim uri = ruta_server & "/workflow/Handler_image_scrip_wf.ashx?rut_image="
            Dim refclas As New ClassAdmonEmpresa
            Dim nombre_empresa As String = ""
            Result = refclas.Retorna_nombre_empresa_usuario_gestion(nombre_empresa,
                                                                    HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Envio_Correo_respuesta_documento = Result
                Exit Function
            End If
            Dim pat_confir As String = nombre_empresa & "|" & estru.ID_RESPUESTA_RADICADO & "|" & estru.RADICADO
            Dim pat_confir_encript As String = ""
            Result = encriptacion.encript_md5(pat_confir,
                                            "7894561230!",
                                             pat_confir_encript)

            If Result <> "YES" Then
                Envio_Correo_respuesta_documento = Result
                Exit Function
            End If
            Dim uri_confir As String = ruta_server & "/Gestion/WebForm_gestion_confirma_recibido_usuario.aspx?path_confir=" & pat_confir_encript
            Dim HTTEXT As String = "<p>" & nombre_empresa & " Esta respondiendo su petición con radicado " & estru.RADICADO & " de asunto : " & estru.ASUNTO & "</p>"
            HTTEXT = HTTEXT & "<p> <Font Color=Red> Por favor confirme el recibido" & "<a href=" & uri_confir & "> aquí </a>" & "</Font> </p>"
            HTTEXT = HTTEXT & "<p style=font-size:12px>  Si no puede desplegar el link anterior copie en su navegador esta url :  </p> <p style=font-size:10px>" & uri_confir & " </p>"
            For i As Integer = 0 To Adic_mensaje.Length - 1
                HTTEXT = HTTEXT & "<p>" & Adic_mensaje(i) &
                  "</p>"
            Next
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue>" & "Respondio a su petición " & "</Font>" &
                 "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue>" & nombre_usuario_respuesta & "</Font>" &
                "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue> Cargo: " & cargo_usuario_responde & "</Font>" &
               "</p>"
            HTTEXT = HTTEXT & "<p>" & "<Font Color=Blue> Area: " & area_usuario_responde & "</Font>" &
               "</p>"
            HTTEXT = HTTEXT & "<p> " &
            "<Font Color=Green>" &
            "Si usted tiene alguna duda o sugerencia con respecto a mi respuesta, contacteme al siguiente correo electrónico " & correo_responde &
             "</Font>" &
            "</p>"

            If Not matri_documentos_respuesta Is Nothing Then
                HTTEXT = HTTEXT & "<p> Con este correo electrónico se adjuntan " & matri_documentos_respuesta.Length + cantidad_archivo_anexo & " archivo(s) " &
                             "</p>"
            End If
            HTTEXT = HTTEXT & "<p> Detalle solicitud radicado numero " & estru.RADICADO & "</p>" &
                    "<table>" &
                       "<tr>" &
                          "<td> Su petición se recepciono con este radicado : </td>" &
                          "<td>" & estru.RADICADO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Tipo tramite para envío : </td> " &
                            "<td>" & estru.TRAMITE_DOCUMENTO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Respuesta dirigida a : </td>" &
                            "<td>" & estru.DESTINATARIO & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Se respondio con el radicado numero  : </td> " &
                            "<td> " & estru.RADICADO_RESPUESTA & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha Limite respuesta : </td> " &
                            "<td>" & estru.FECHA_VENCE & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha respuesta del usuario de la entidad : </td>" &
                             "<td>" & estru.FECHA_RESPUETA & "</td>" &
                       "</tr>" &
                       "<tr>" &
                           "<td> Otros datos : </td> " &
                            "<td>" & " Guía : " & estru.GUIA_ENVIO & "  Empresa " & estru.EMPRESA_ENVIO & "</td>" &
                       "</tr>" &
                    "</table>"
            If tipo_notificacion = 2 Then
                If Not matri_documentos_respuesta Is Nothing Then
                    HTTEXT = HTTEXT & "<p> Documentos relacionados   </p>" &
                        "<table>"
                    For i As Integer = 0 To matri_documentos_respuesta.Length - 1
                        Dim has_huella_md5_escript As String = ""
                        Result = encriptacion.encript_md5(matri_documentos_respuesta(i),
                                                          "7894561230!",
                                                          has_huella_md5_escript)

                        If Result <> "YES" Then
                            Envio_Correo_respuesta_documento = Result
                            Exit Function
                        End If
                        HTTEXT = HTTEXT &
                            "<tr>" &
                              "<td><a href=" & uri & has_huella_md5_escript & " > Descarga respuesta aquí  </td>" &
                              "<td>  <p " & " style=font-size:12px> o copie esta url en su navegador :  </p>" &
                              "<td>  <p " & " style=font-size:10px> " & uri & has_huella_md5_escript & "  </p>" &
                           "</tr>"
                    Next
                    If Not matri_anexos Is Nothing Then
                        For i As Integer = 0 To matri_anexos.Length - 1
                            Dim has_huella_md5_escript As String = ""
                            Result = encriptacion.encript_md5(matri_anexos(i),
                                                            "7894561230!",
                                                            has_huella_md5_escript)

                            If Result <> "YES" Then
                                Envio_Correo_respuesta_documento = Result
                                Exit Function
                            End If
                            HTTEXT = HTTEXT &
                               "<tr>" &
                              "<td><a href=" & uri & has_huella_md5_escript & " style=font-size:14px> Descarga anexo aquí  </td>" &
                              "<td>  <p " & " style=font-size:14px> o copie esta url en su navegador :  </p>" &
                              "<td>  <p " & " style=font-size:10px> " & uri & has_huella_md5_escript & "  </p>" &
                           "</tr>"
                        Next
                    End If

                    HTTEXT = HTTEXT & "</table>"
                End If
            End If
            Dim mensaj As String = "<html>" &
                  "<head>" &
                     "<title>" &
                       "Respuesta a petición " &
                     "</title>" &
                  "</head>" &
                  "<body>" &
                    "<p>" &
                     "<h1>" &
                        "<Font Color=Green>" &
                         " Sistema de gestión Docuarchi.net web" &
                       "</Font>" &
                     "</h1>" &
                    "</p>" &
                     HTTEXT &
                   "<p>" &
                     "<h2>Servidor Smtp  </h2>" &
                    "</p>" &
                    "<p>" & "Mensaje enviado el " &
                           Now.ToShortDateString &
                          " a las " & Now.ToLongTimeString &
                    " Este correo es automatico no renvie informnación a este correo</p>" &
                  "</body>" &
               "</html>"

            '********************************
            'Envia correo electrónico
            '********************************
            Dim splidest() As String = Nothing
            If InStr(corre_dest, ",") > 0 Then
                splidest = corre_dest.ToString.Split(",")
            End If
            Dim ref_trim As String = ""
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            If splidest Is Nothing Then
                correo.To.Add(New System.Net.Mail.MailAddress(corre_dest, ""))
            Else
                For i As Integer = 0 To splidest.Length - 1
                    If Trim(splidest(i)) <> "" And Trim(splidest(i)) <> " " Then
                        If InStr(Trim(splidest(i)), "|") > 0 Then
                            Dim split_correo() As String = Trim(splidest(i)).Split("|")
                            correo.To.Add(New System.Net.Mail.MailAddress(split_correo(0), split_correo(1)))
                        Else
                            correo.To.Add(New System.Net.Mail.MailAddress(Trim(splidest(i)), ""))
                        End If

                    End If
                Next

            End If
            correo.CC.Add(correo_responde)
            If correo_copia <> "" Then
                correo.CC.Add(correo_copia)
            End If
            If tipo_notificacion = 1 Then
                If Not matri_documentos_respuesta Is Nothing Then
                    If estru.ID_TIPO_DOC_RESPUESTA = 1 Then
                        For i As Integer = 0 To matri_documentos_respuesta.Length - 1
                            Dim file_infe As New FileInfo(matri_documentos_respuesta(i))
                            If file_infe.Extension <> ".PDF" Then
                                Dim memStream As MemoryStream = New MemoryStream()
                                'Dim OB As New localhost.Service
                                'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
                                Dim fil_estrean As Object = Nothing
                                Dim archi_descarga As String = ""
                                'Result = OB.Descarga_doxc_formatos_archivo_web_application(matri_documentos_respuesta(i), fil_estrean, "PDF", 1)
                                'If Result <> "YES" Then
                                '    Envio_Correo_respuesta_documento = Result
                                '    Exit Function
                                'End If
                                Dim refclas_gest_resp As New ClassRaEnvioCorrespondencia
                                archi_descarga = Ruta_descarga & "corre_envi.pdf"
                                If File.Exists(archi_descarga) Then
                                    Kill(archi_descarga)
                                End If
                                Result = refclas_gest_resp.SaveFile(archi_descarga, fil_estrean)
                                If Result <> "YES" Then
                                    Envio_Correo_respuesta_documento = Result
                                    Exit Function
                                End If
                                Dim fileStream As FileStream = File.OpenRead(archi_descarga)
                                memStream.SetLength(fileStream.Length)
                                fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                                fileStream.Close()
                                Dim file_inf As New FileInfo(archi_descarga)
                                correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(archi_descarga), "application/" & Replace(file_inf.Extension, ".", "")))
                            Else
                                Dim memStream As MemoryStream = New MemoryStream()
                                Dim fileStream As FileStream = File.OpenRead(matri_documentos_respuesta(i))
                                memStream.SetLength(fileStream.Length)
                                fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                                fileStream.Close()
                                Dim file_inf As New FileInfo(matri_documentos_respuesta(i))
                                correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos_respuesta(i)), "application/" & Replace(file_inf.Extension, ".", "")))
                            End If

                        Next
                    Else
                        If matri_documentos_respuesta.Length > 1 Then
                            Using zip As New ZipFile()
                                For i As Integer = 0 To matri_documentos_respuesta.Length - 1
                                    If i = 0 Then
                                        zip.AddFile(matri_documentos_respuesta(i), "FilesDocuarchi")
                                    Else
                                        zip.AddFile(matri_documentos_respuesta(i), "FilesDocuarchi")
                                    End If
                                Next
                                Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                                Dim archivo_salida_zip As String = Ruta_descarga & zipName
                                If File.Exists(archivo_salida_zip) Then
                                    Kill(archivo_salida_zip)
                                End If
                                zip.Save(archivo_salida_zip)
                                Dim memStream As MemoryStream = New MemoryStream()
                                Dim fileStream As FileStream = File.OpenRead(archivo_salida_zip)
                                memStream.SetLength(fileStream.Length)
                                fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                                fileStream.Close()
                                correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(archivo_salida_zip), "application/zip"))
                            End Using
                        Else
                            Dim file_inf As New FileInfo(matri_documentos_respuesta(0))
                            Dim memStream As MemoryStream = New MemoryStream()
                            Dim fileStream As FileStream = File.OpenRead(matri_documentos_respuesta(0))
                            memStream.SetLength(fileStream.Length)
                            fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                            fileStream.Close()
                            correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos_respuesta(0)), "application/" & Replace(file_inf.Extension, ".", "")))
                        End If

                    End If

                End If
                If Not matri_anexos Is Nothing Then
                    For i As Integer = 0 To matri_anexos.Length - 1
                        Dim memStream As MemoryStream = New MemoryStream()
                        Dim fileStream As FileStream = File.OpenRead(matri_anexos(i))
                        memStream.SetLength(fileStream.Length)
                        fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                        fileStream.Close()
                        Dim file_inf As New FileInfo(matri_anexos(i))
                        correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_anexos(i)), "application/" & Replace(file_inf.Extension, ".", "")))
                    Next
                End If
            End If
            correo.Subject = "Respuesta a petición " & estru.RADICADO & " Asunto " & estru.ASUNTO
            'correo.To.Add(corre_dest)
            correo.BodyEncoding = System.Text.Encoding.UTF8
            'correo.Body = RefDatosConfig.BODY_CUERPO_MENSAJE & mensaj
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
                'mensaj = RefDatosConfig.BODY_CUERPO_MENSAJE
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Result = Registra_log_respuesta_notificacion_radicado(estru, mensaj, corre_dest)
            If Result <> "YES" Then
                Envio_Correo_respuesta_documento = "Se envio el correo pero no se pudo registrar el estado, " & Result
                Exit Function
            End If
            Dim Ref_clas_respuesta As New Class_ra_respuesta_radicado
            Ref_clas_respuesta.Actualiza_estado_envio_correo_notificacion(estru.ID_RESPUESTA_RADICADO,
                                                                          corre_dest)
            If Result <> "YES" Then
                Envio_Correo_respuesta_documento = "Se envio el correo pero no se pudo registrar el estado, " & Result
                Exit Function
            End If
            Envio_Correo_respuesta_documento = "YES"
        Catch ex As Exception
            Dim obpklink As Object
            If Not ex.HelpLink Is Nothing Then
                obpklink = ex.HelpLink
            Else
                obpklink = ""
            End If
            Dim obInnerException As Object
            If Not ex.HelpLink Is Nothing Then
                obInnerException = ex.InnerException.Message
            Else
                obInnerException = ""
            End If
            Envio_Correo_respuesta_documento = "Inconsistencia general funcion Envio_Correo_respuesta_documento  " & ex.Message
        End Try
    End Function
    Function Registra_log_respuesta_notificacion_radicado(ByVal estru As stru_envio,
                                                          ByVal copia_correo As String,
                                                          ByVal correo_send As String) As String
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim sqlresultinsert As Integer = 0
        Try

            Dim date1al As String = Date.Now
            Dim hor As String = Now
            Dim SQL As String = "Update ra_respuesta_radicado Set estado_envio_correo=" & 1 &
          " where ID_RESPUESTA_RADICADO=" & estru.ID_RESPUESTA_RADICADO
            Dim refclas_gestion_fechas As New ClassGestionFechas
            Dim Result As String = refclas_gestion_fechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Registra_log_respuesta_notificacion_radicado = Result
                Exit Function
            End If
            Dim insert_datos_envio As String = "('" & "NOTIFICACION CORREO" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") &
                "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                      estru.ID_RESPUESTA_RADICADO & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WEB-WORKFLOW','" & copia_correo.Replace("\", "°") & "','" & correo_send & "')"
            Dim update_envio As String = "INSERT INTO ra_log_respuesta_radicado (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_RESPUESTA_RADICADO" &
                                                ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS,SEND_CORREO_ELECTRONICO) values " &
                                                insert_datos_envio
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = SQL
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_log_respuesta_notificacion_radicado = "Imposible actualizar estado envio de correo  "
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myCommand.CommandText = update_envio
            sqlresultinsert = myCommand.ExecuteNonQuery()
            If sqlresultinsert = 0 Then
                Registra_log_respuesta_notificacion_radicado = "Imposible actualizar registro log envio de correo  "
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Registra_log_respuesta_notificacion_radicado = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Registra_log_respuesta_notificacion_radicado = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registra_log_respuesta_notificacion_radicado = "Error General " & e.Message
            Exit Function
        End Try
    End Function
    Function Envio_Correo_notificacion_asignacion(ByVal Adic_mensaje() As String,
                                                   ByVal Corre_dest As String,
                                                   ByVal subyect As String) As String
        Try

            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Envio_Correo_notificacion_asignacion = Result
                Exit Function
            End If
            If RefDatosConfig.ESTADO_ENVIO = 0 Then
                Envio_Correo_notificacion_asignacion = "YES"
                Exit Function
            End If
            Dim HTTEXT As String = ""

            For i As Integer = 0 To Adic_mensaje.Length - 1
                HTTEXT = HTTEXT & "<p>" & Adic_mensaje(i) &
                  "</p>"
            Next
            Dim mensaj As String = "<html>" &
               "<head>" &
                  "<title>" &
                    "Notificación envío de correspondencia " &
                  "</title>" &
               "</head>" &
               "<body>" &
                 "<p>" &
                  "<h1>" &
                     "<Font Color=Green>" &
                      " Sistema de gestión Docuarchi.net web " &
                    "</Font>" &
                  "</h1>" &
                 "</p>" &
                  HTTEXT &
                "<p>" &
                  "<h2>Servidor Smtp  </h2>" &
                 "</p>" &
                 "<p>" & "Mensaje enviado el " &
                        Now.ToShortDateString &
                       " a las " & Now.ToLongTimeString &
                 "</p>" &
               "</body>" &
            "</html>"

            '********************************
            'Envia correo electronico
            '********************************
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            correo.Subject = subyect
            correo.To.Add(Corre_dest)
            correo.BodyEncoding = System.Text.Encoding.UTF8
            'correo.Body = RefDatosConfig.BODY_CUERPO_MENSAJE & mensaj
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
                'mensaj = RefDatosConfig.BODY_CUERPO_MENSAJ
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If

            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                 RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Envio_Correo_notificacion_asignacion = "YES"
        Catch ex As Exception
            Envio_Correo_notificacion_asignacion = "Inconsistencia general  " & ex.Message
        End Try
    End Function
    Function Lista_tipos_configuracion_correos(ByRef drow_list As DropDownList, ByVal nombre_configuracion As String) As String
        Try
            Dim sql_consulta As String = "Select NOMBRE_CONFIGURACION from config_smpt_side  "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            drow_list.Items.Clear()
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_tipos_configuracion_correos = "Función Lista_tipos_configuracion_correos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    drow_list.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                If drow_list.Items.Count > 0 Then
                    For i As Integer = 0 To drow_list.Items.Count - 1
                        If nombre_configuracion = drow_list.Items(i).Text Then
                            drow_list.Text = drow_list.Items(i).Text
                            Exit For
                        End If
                    Next
                End If
                Lista_tipos_configuracion_correos = "YES"
                Exit Function
            Else
                Lista_tipos_configuracion_correos = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_tipos_configuracion_correos = "Inconsistencia funcion Lista_tipos_configuracion_correos " & ex.Message
        End Try
    End Function
    Function Actualiza_configuracion_correo_remitente(ByVal id_tipo_cuenta As Integer, ByVal correo_smtp As String, ByVal pasw_smtp As String) As String
        Try
            Dim Result As String = ""
            Dim sql_insert As String = "update ra_config_smpt_side_usuario set PASW_SMTP='" & pasw_smtp & "', COFIG_ID_CONFIG=" & id_tipo_cuenta &
                 " where USUARIO_SMTP='" & correo_smtp & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Actualiza_configuracion_correo_remitente = Result
                Exit Function
            Else
                Actualiza_configuracion_correo_remitente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_configuracion_correo_remitente = "Inconsistencia funcion Actualiza_configuracion_correo_remitente " & ex.Message
        End Try
    End Function
    Function Registra_configuracion_correo_remitente(ByVal config_id As Integer, ByVal correo_smtp As String, ByVal pasw_smtp As String) As String
        Try
            Dim Result As String = ""
            Dim sql_insert As String = "Insert into ra_config_smpt_side_usuario (COFIG_ID_CONFIG,USUARIO_SMTP,PASW_SMTP) values (" &
                "'" & config_id & "','" & correo_smtp & "','" & pasw_smtp & "')"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Registra_configuracion_correo_remitente = Result
                Exit Function
            Else
                Registra_configuracion_correo_remitente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_configuracion_correo_remitente = "Inconsistencia funcion Registra_configuracion_correo_remitente " & ex.Message
        End Try
    End Function
    Function Retorna_tipo_configuracion_correos(ByVal nombre_configuracion As String, ByRef id_tipo_configuacion As Integer) As String
        Try
            Dim sql_consulta As String = "Select ID_CONFIG  from config_smpt_side where NOMBRE_CONFIGURACION='" & nombre_configuracion & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")

            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_tipo_configuracion_correos = "Función Lista_tipos_configuracion_correos dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_tipo_configuacion = Datset.Tables(0).Rows(0).Item(0)
                Retorna_tipo_configuracion_correos = "YES"
                Exit Function
            Else
                Retorna_tipo_configuracion_correos = "Imposble encontrar id configuración "
                Exit Function
            End If

        Catch ex As Exception
            Retorna_tipo_configuracion_correos = "Inconsistencia funcion Retorna_tipo_configuracion_correos " & ex.Message
        End Try
    End Function
    Function Retorna_datos_correo_usuario_remitente_radicacion(ByVal correo_usuario As String,
                                                               ByRef pasword_correo As String,
                                                               ByRef id_cuenta As String,
                                                               ByRef nombre_configuracion As String) As String
        Try
            Dim sql_consulta As String = "Select rcs.COFIG_ID_CONFIG,rcs.PASW_SMTP,css.NOMBRE_CONFIGURACION from ra_config_smpt_side_usuario as rcs " &
                " left outer join config_smpt_side as css on (css.ID_CONFIG=rcs.COFIG_ID_CONFIG) " &
                " where rcs.USUARIO_SMTP='" & correo_usuario & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Retorna_datos_correo_usuario_remitente_radicacion = "Función Retorna_datos_correo_usuario_remitente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_cuenta = Datset.Tables(0).Rows(0).Item(0)
                pasword_correo = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    nombre_configuracion = ""
                Else
                    nombre_configuracion = Datset.Tables(0).Rows(0).Item(2)
                End If
                Retorna_datos_correo_usuario_remitente_radicacion = "YES"
                Exit Function
            Else

                Retorna_datos_correo_usuario_remitente_radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_datos_correo_usuario_remitente_radicacion = "Inconsistencia funcion Retorna_datos_correo_usuario_remitente_radicacion " & ex.Message
        End Try
    End Function
    Function Obtener_Datos_ConfigSmtp(ByVal ID_CONFIG As Integer, ByRef Refconfig As Config_Smtp) As String
        Try
            Dim Parametro_Consulta As String = "select * from Config_Smpt_Side where ID_CONFIG =" & ID_CONFIG
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Obtener_Datos_ConfigSmtp = " Función Obtener_Datos_ConfigSmtp dice " & Result
                Return Obtener_Datos_ConfigSmtp
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Obtener_Datos_ConfigSmtp = "Imposible encontrar datos de configuración SMTP"
                Exit Function
            Else
                Refconfig.SERV_SMTP = Datset.Tables(0).Rows(0).Item(2).ToString
                Refconfig.PUERTO_SERV_SMTP = Datset.Tables(0).Rows(0).Item(3).ToString
                Refconfig.USUARIO_SMTP = Datset.Tables(0).Rows(0).Item(4).ToString
                Refconfig.PASW_SMTP = Datset.Tables(0).Rows(0).Item(5).ToString
                Refconfig.DOMINIO_SMTP = Datset.Tables(0).Rows(0).Item(6).ToString
                Refconfig.SMTP_TIEMPO = Datset.Tables(0).Rows(0).Item(7).ToString
                Refconfig.ESTADO_SSL = Datset.Tables(0).Rows(0).Item(8).ToString
                Refconfig.ESTADO_ENVIO = Datset.Tables(0).Rows(0).Item(9).ToString
                Refconfig.ESTADO_BODY = Datset.Tables(0).Rows(0).Item(10).ToString
                Refconfig.ESTADO_CREDENCIAL = Datset.Tables(0).Rows(0).Item(11).ToString
                Obtener_Datos_ConfigSmtp = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Obtener_Datos_ConfigSmtp = "Inconsistencia función Obtener_Datos_ConfigSmtp " & ex.Message

        End Try
    End Function
    Function Obtener_Datos_ConfigSmtp(ByRef Refconfig As Config_Smtp) As String
        Try
            Dim Parametro_Consulta As String = "select * from Config_Smpt_Side where ESTADO_ENVIO=1"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("Config_Smpt_Side")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Obtener_Datos_ConfigSmtp = " Función Obtener_Datos_ConfigSmtp dice " & Result
                Return Obtener_Datos_ConfigSmtp
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Obtener_Datos_ConfigSmtp = "Imposible encontrar datos de configuración SMTP"
                Exit Function
            Else
                Refconfig.SERV_SMTP = Datset.Tables(0).Rows(0).Item(2).ToString
                Refconfig.PUERTO_SERV_SMTP = Datset.Tables(0).Rows(0).Item(3).ToString
                Refconfig.USUARIO_SMTP = Datset.Tables(0).Rows(0).Item(4).ToString
                Refconfig.PASW_SMTP = Datset.Tables(0).Rows(0).Item(5).ToString
                Refconfig.DOMINIO_SMTP = Datset.Tables(0).Rows(0).Item(6).ToString
                Refconfig.SMTP_TIEMPO = Datset.Tables(0).Rows(0).Item(7).ToString
                Refconfig.ESTADO_SSL = Datset.Tables(0).Rows(0).Item(8).ToString
                Refconfig.ESTADO_ENVIO = Datset.Tables(0).Rows(0).Item(9).ToString
                Refconfig.ESTADO_BODY = Datset.Tables(0).Rows(0).Item(10).ToString
                Refconfig.ESTADO_CREDENCIAL = Datset.Tables(0).Rows(0).Item(11).ToString
                Obtener_Datos_ConfigSmtp = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Obtener_Datos_ConfigSmtp = "Inconsistencia función Obtener_Datos_ConfigSmtp " & ex.Message

        End Try
    End Function
    Function inicia_notificacion_correo_envio_correspondencia(ByRef hdnEmailID_VAL As Object,
                                                              ByRef TextBox_lista_correos As String,
                                                              ByRef TextBox_nota_noti_ficacion As TextBox,
                                                              ByRef Hidden_cuenta_correo_envio As Object,
                                                              ByVal tipo_notificacion As String, ByVal chekbox_adjunta As CheckBox,
                                                              ByVal chekbox_pdf As CheckBox,
                                                              ByVal chekbox_lectura As CheckBox,
                                                              ByVal chekbox_pasw As CheckBox,
                                                              ByVal id_plantilla As Integer,
                                                              ByVal ruta_tempo As String,
                                                              ByRef TextBox_asunto_notificacion As TextBox,
                                                              ByVal tipo_envio As Integer) As String
        Try
            Dim Result As String = ""
            If hdnEmailID_VAL.Value = "-1" Then
                inicia_notificacion_correo_envio_correspondencia = "No hay una seleccion de tramite para notificar"
                Exit Function
            End If
            If TextBox_lista_correos = "" Then
                inicia_notificacion_correo_envio_correspondencia = "Por favor informe el correo destinatario"
                Exit Function
            End If
            If TextBox_nota_noti_ficacion.Text = "" Then
                inicia_notificacion_correo_envio_correspondencia = "Por favor digite el texto de notificación"
                Exit Function
            End If
            If TextBox_asunto_notificacion.Text = "" Then
                inicia_notificacion_correo_envio_correspondencia = "Por favor digite el asunto notificación"
                Exit Function
            End If
            Dim nombre_configuracion As String = ""
            Dim smtp_pasword As String = ""
            Dim ID_CONFIG As Integer = 0
            'Result = Retorna_datos_correo_usuario_remitente_radicacion(Hidden_cuenta_correo_envio.Value, smtp_pasword, ID_CONFIG, nombre_configuracion)
            'If Result <> "YES" Then
            '    inicia_notificacion_correo_envio_correspondencia = Result
            '    Exit Function
            'End If
            Dim slplicopia() As String = TextBox_lista_correos.Split(",")
            If tipo_notificacion = "ENVIO CORESPONDENCIA" Then
                Result = Enviando_Correo_Smtp_notificacion(hdnEmailID_VAL.Value,
                                                           "Notificación tramite respuesta",
                                                           slplicopia(0), ID_CONFIG,
                                                           TextBox_lista_correos,
                                                           TextBox_nota_noti_ficacion.Text,
                                                           Hidden_cuenta_correo_envio.Value,
                                                           smtp_pasword)
                If Result <> "YES" Then
                    inicia_notificacion_correo_envio_correspondencia = Result
                    Exit Function
                End If
            End If
            If tipo_notificacion = "NOTIFICA CORESPONDENCIA" Then
                Dim imagen_adjunta As Integer = 0
                Dim lectura As Integer = 0
                Dim pdf_convierte As Integer = 0
                Dim pasw_pdf As Integer = 0
                If chekbox_adjunta.Checked = True Then
                    imagen_adjunta = 1
                Else
                    imagen_adjunta = 0
                End If
                If chekbox_pdf.Checked = True Then
                    pdf_convierte = 1
                Else
                    pdf_convierte = 0
                End If
                If chekbox_lectura.Checked = True Then
                    lectura = 1
                Else
                    lectura = 0
                End If
                If chekbox_pdf.Checked = True Then
                    pasw_pdf = 1
                Else
                    pasw_pdf = 0
                End If
                Dim correo_remite As String = Hidden_cuenta_correo_envio.Value
                Dim radicado As String = hdnEmailID_VAL.Value
                Result = Enviando_Correo_Smtp_radicado(radicado,
                                                       id_plantilla,
                                                       TextBox_asunto_notificacion.Text,
                                                       slplicopia(0).ToString,
                                                       ID_CONFIG,
                                                       TextBox_lista_correos,
                                                       TextBox_nota_noti_ficacion.Text,
                                                       correo_remite,
                                                       smtp_pasword,
                                                       ruta_tempo,
                                                       imagen_adjunta,
                                                       pdf_convierte,
                                                       lectura,
                                                       pasw_pdf,
                                                       tipo_envio)
                If Result <> "YES" Then
                    inicia_notificacion_correo_envio_correspondencia = Result
                    Exit Function
                End If
            End If
            If tipo_notificacion = "ENVIO CORREO PRODUCCION" Then
                Dim imagen_adjunta As Integer = 0
                Dim lectura As Integer = 0
                Dim pdf_convierte As Integer = 0
                Dim pasw_pdf As Integer = 0
                If chekbox_adjunta.Checked = True Then
                    imagen_adjunta = 1
                Else
                    imagen_adjunta = 0
                End If
                If chekbox_pdf.Checked = True Then
                    pdf_convierte = 1
                Else
                    pdf_convierte = 0
                End If
                If chekbox_lectura.Checked = True Then
                    lectura = 1
                Else
                    lectura = 0
                End If
                If chekbox_pdf.Checked = True Then
                    pasw_pdf = 1
                Else
                    pasw_pdf = 0
                End If
                Dim correo_remite As String = Hidden_cuenta_correo_envio.Value
                Dim radicado As String = hdnEmailID_VAL.Value
                Dim matri_id_selecion() As Long
                Dim matri_tempo_seleccion() As String = Nothing
                If InStr(HttpContext.Current.Session.Item("PG_SELECCION_ID_ARCHIVO"), "|") > 0 Then
                    matri_tempo_seleccion = HttpContext.Current.Session.Item("PG_SELECCION_ID_ARCHIVO").ToString.Split("|")
                    For i As Integer = 0 To matri_tempo_seleccion.Length - 1
                        ReDim Preserve matri_id_selecion(i)
                        matri_id_selecion(i) = Val(matri_tempo_seleccion(i))
                    Next
                Else
                    ReDim Preserve matri_id_selecion(0)
                    matri_id_selecion(0) = HttpContext.Current.Session.Item("PG_SELECCION_ID_ARCHIVO")
                End If
                Result = Me.Enviando_correo_electronico_archivo_de_produccion_documental(matri_id_selecion,
                                                                                         TextBox_asunto_notificacion.Text, "",
                                                                                         ID_CONFIG,
                                                                                         TextBox_lista_correos,
                                                                                         TextBox_nota_noti_ficacion.Text,
                                                                                         correo_remite,
                                                                                         smtp_pasword,
                                                                                         ruta_tempo,
                                                                                         imagen_adjunta,
                                                                                         pdf_convierte,
                                                                                         lectura,
                                                                                         pasw_pdf,
                                                                                         tipo_envio)
                If Result <> "YES" Then
                    inicia_notificacion_correo_envio_correspondencia = Result
                    Exit Function
                Else
                    inicia_notificacion_correo_envio_correspondencia = "YES"
                    Exit Function
                End If
            End If
            If tipo_notificacion = "ENVIO CORREO WORKFLOW" Then
                Dim imagen_adjunta As Integer = 0
                Dim lectura As Integer = 0
                Dim pdf_convierte As Integer = 0
                Dim pasw_pdf As Integer = 0
                If chekbox_adjunta.Checked = True Then
                    imagen_adjunta = 1
                Else
                    imagen_adjunta = 0
                End If
                If chekbox_pdf.Checked = True Then
                    pdf_convierte = 1
                Else
                    pdf_convierte = 0
                End If
                If chekbox_lectura.Checked = True Then
                    lectura = 1
                Else
                    lectura = 0
                End If
                If chekbox_pdf.Checked = True Then
                    pasw_pdf = 1
                Else
                    pasw_pdf = 0
                End If
                Dim correo_remite As String = Hidden_cuenta_correo_envio.Value
                Dim radicado As String = hdnEmailID_VAL.Value
                Dim stru_documento_compartido() As stru_documentos_compartidos = Nothing
                stru_documento_compartido = HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_COMPARTIDO")
                If stru_documento_compartido Is Nothing Then
                    inicia_notificacion_correo_envio_correspondencia = "No hay documentos disponibles para compartir al correo eletrónico"
                    Exit Function
                End If
                Result = Me.Enviando_correo_electronico_documetos_tarea_workflow(stru_documento_compartido,
                                                                                 TextBox_asunto_notificacion.Text,
                                                                                 "",
                                                                                 ID_CONFIG, TextBox_lista_correos,
                                                                                 TextBox_nota_noti_ficacion.Text,
                                                                                 correo_remite,
                                                                                 smtp_pasword,
                                                                                 ruta_tempo,
                                                                                 imagen_adjunta,
                                                                                 pdf_convierte,
                                                                                 lectura,
                                                                                 pasw_pdf,
                                                                                 tipo_envio)
                If Result <> "YES" Then
                    inicia_notificacion_correo_envio_correspondencia = Result
                    Exit Function
                Else
                    inicia_notificacion_correo_envio_correspondencia = "YES"
                    Exit Function
                End If
            End If
            inicia_notificacion_correo_envio_correspondencia = "YES"
            Exit Function
        Catch ex As Exception
            inicia_notificacion_correo_envio_correspondencia = "Inconsistencia función inicia_notificacion_correo_envio_correspondencia " & ex.Message
        End Try
    End Function
    Function Retorna_relacion_gabinete_busqueda_predeterminado(ByVal id_plantilla As Integer,
                                                               ByRef Gabinete As String,
                                                               ByRef campo_busqueda_gabinete As String) As String
        '********************************************************************
        'Función : Asigna el campo de gabinete y el gabiente para la busqeda 
        'del radicado
        'Fecha : 2015-05-01
        'Ingeniero : Miguel Angel Urueta Miranda
        '*********************************************************************
        Try
            Dim Parametro_Consulta As String = "select NOMBRE,CAMPO_BUSQUEDA from ra_relacion_plantilla_radicado_gabinete where id_plantilla =" & id_plantilla &
                " AND PREDETERMINADO=1"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_relacion_gabinete_busqueda_predeterminado = " Función Retorna_relacion_gabinete_busqueda_predeterminado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_relacion_gabinete_busqueda_predeterminado = "Imposible encontrar gabinete relacionado para la busqueda del documento "
                Exit Function
            Else
                Gabinete = Datset.Tables(0).Rows(0).Item(0)
                campo_busqueda_gabinete = Datset.Tables(0).Rows(0).Item(1)
                Retorna_relacion_gabinete_busqueda_predeterminado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_relacion_gabinete_busqueda_predeterminado = "Inconsistencia función Retorna_relacion_gabinete_busqueda_predeterminado " & ex.Message
        End Try
    End Function
    Function SolicitaDatosConfigSmtp(ByRef Refconfig As Config_Smtp) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Función que retorna los datos de configuración del servicio de envio de correos 
        '          electronicos para la conexión docuarchi
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------

        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Refconfig           : Retorna estructura con los datos de configuración
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-09-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select * from Config_Smpt_Side where ESTADO_ENVIO=1"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("Config_Smpt_Side")
            Result = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosConfigSmtp = " Función SolicitaDatosConfigSmtp dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaDatosConfigSmtp = "Imposible encontrar datos de configuración SMTP"
                Exit Function
            Else
                Refconfig.SERV_SMTP = Datset.Tables(0).Rows(0).Item(2).ToString
                Refconfig.PUERTO_SERV_SMTP = Datset.Tables(0).Rows(0).Item(3).ToString
                Refconfig.USUARIO_SMTP = Datset.Tables(0).Rows(0).Item(4).ToString
                Refconfig.PASW_SMTP = Datset.Tables(0).Rows(0).Item(5).ToString
                Refconfig.DOMINIO_SMTP = Datset.Tables(0).Rows(0).Item(6).ToString
                Refconfig.SMTP_TIEMPO = Datset.Tables(0).Rows(0).Item(7).ToString
                Refconfig.ESTADO_SSL = Datset.Tables(0).Rows(0).Item(8).ToString
                Refconfig.ESTADO_ENVIO = Datset.Tables(0).Rows(0).Item(9).ToString
                Refconfig.ESTADO_BODY = Datset.Tables(0).Rows(0).Item(10).ToString
                Refconfig.ESTADO_CREDENCIAL = Datset.Tables(0).Rows(0).Item(11).ToString
                SolicitaDatosConfigSmtp = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosConfigSmtp = "Inconsistencia función SolicitaDatosConfigSmtp " & ex.Message

        End Try
    End Function
    Function EnviaCorreoNotificacionResultadoConsultaRue(ByVal CdRues As CdRues,
                                                         ByVal DescripcionError As String,
                                                         ByVal MensajeSistema As String,
                                                         ByVal CorreDestino As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Función que envia email para notificación mensajes de inconsistencia RUE
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'CdRues              : Representa la estructura del usuario rues de consulta
        'DescripcionError    : Representa el mensaje esapto del erroe
        'MensajeSistema      : Representa el mensaje presentado al usuario
        'CorreDestino        : Representa los correos de notificación
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-09-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim correo As New System.Net.Mail.MailMessage()
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Dim Dest_Smtp As String = ""
            Result = SolicitaDatosConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                EnviaCorreoNotificacionResultadoConsultaRue = Result
                Exit Function
            End If
            Dim HTTEXT As String = "<p> Se ha detectado una inconsistencia en la solicitud de consulta de documentos desde el sistema RUES para la matrícula/expediente " & CdRues.expediente & ", Tipo de registro : " & CdRues.tipoRegistro & ", posible causa de la inconsistencia " & DescripcionError & " </p>"
            HTTEXT = HTTEXT & "<p> La aplicación presentó el siguiente mensaje al usuario de consulta :  " & MensajeSistema & ". </p>"
            HTTEXT = HTTEXT & "<p> App  :  " & "Consulta RUE DocuArchi" & " </p>"
            HTTEXT = HTTEXT & "<h1> Recomendaciones adicionales  </h1>"
            HTTEXT = HTTEXT & "<p> 1. Verificación de datos: Asegúrese de que la matrícula/expediente (" & CdRues.expediente & ") del registro público (" & CdRues.tipoRegistro & ") se ecuentre en el gestor documental DocuArchi. </p>"
            HTTEXT = HTTEXT & "<p> 2. Consulta en el sistema RUES: Puede verificar la información directamente en el sitio oficial del Registro Único Empresarial y Social (RUES). </p>"
            HTTEXT = HTTEXT & "<p> 3. Contacto con el RUE: Si la inconsistencia persiste, le recomiendo contactar al RUES para obtener asistencia personalizada. </p>"
            HTTEXT = HTTEXT & "<p> 4. Contacto con la entidad que consulta: Si la inconsistencia se supera, le recomiendo contactar a la entidad y confirmar que se superó la novedad con los datos a continuación. </p>"
            HTTEXT = HTTEXT & "<h1> Contacto de la entidad que consulta  </h1>" &
                    "<table>" &
                       "<tr>" &
                          "<td> Nombre entidad : </td>" &
                          "<td>" & CdRues.nombreEntidad & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Nit Entidad : </td> " &
                            "<td>" & CdRues.nitEntidad & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Email usuario : </td>" &
                            "<td>" & CdRues.emailUsuario & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Usuario : </td> " &
                            "<td>" & CdRues.nombreUsuario & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Nit usuario : </td>" &
                             "<td>" & CdRues.identificacionusuario & "</td>" &
                       "</tr>" &
                    "</table>"
            HTTEXT = HTTEXT & "<p> " & "<h1> "
            Dim mensaj As String = "<html>" &
                  "<head>" &
                     "<title>" &
                       "Solicitud de consulta de expedientes y documentos desde el sistema RUE  " &
                     "</title>" &
                  "</head>" &
                  "<body>" &
                    "<p>" &
                     "<h1>" &
                        "<Font Color=Green>" &
                         " Sistema de gestión DocuArchi.net web" &
                       "</Font>" &
                     "</h1>" &
                    "</p>" &
                     HTTEXT &
                   "<p>" &
                     "<h2>Servidor Smtp  </h2>" &
                    "</p>" &
                    "<p>" & "Mensaje enviado el " &
                           Now.ToShortDateString &
                          " a las " & Now.ToLongTimeString &
                    " Este correo es automatico no renvie informnación a este correo</p>" &
                  "</body>" &
               "</html>"
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            correo.CC.Add(CorreDestino)
            correo.Subject = "RUE alerta consulta Expediente " & CdRues.expediente & " Registro " & CdRues.tipoRegistro
            correo.BodyEncoding = System.Text.Encoding.UTF8
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = mensaj
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If
            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                                                                        RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            EnviaCorreoNotificacionResultadoConsultaRue = "YES"
            Exit Function
        Catch ex As Exception
            EnviaCorreoNotificacionResultadoConsultaRue = "Inconsistencia general funcion EnviaCorreoNotificacionResultadoConsultaRue " & ex.Message
        End Try
    End Function
    Function Enviando_correo_electronico_documetos_tarea_workflow(ByVal stru_documentos_compartidos() As stru_documentos_compartidos,
                                                                  ByVal Asunto As String,
                                                                  ByVal correo_destinatario As String,
                                                                  ByVal ID_CONFIG As Integer,
                                                                  ByVal copia_destinatario As String,
                                                                  ByVal contenido_destinatario As String,
                                                                  ByVal Usuario_Smtp As String,
                                                                  ByVal pasword_smtp As String,
                                                                  ByVal ruta_tempo As String,
                                                                  ByVal chekbox_adjunta As Integer,
                                                                  ByVal chekbox_pdf As Integer,
                                                                  ByVal chekbox_lectura As Integer,
                                                                  ByVal chekbox_pasw As Integer,
                                                                  ByVal tipo_envio As Integer) As String
        Try
            '********************************
            'Solicita datos de configuración
            '********************************
            Dim correo As New System.Net.Mail.MailMessage()
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Dim Dest_Smtp As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Enviando_correo_electronico_documetos_tarea_workflow = Result
                Exit Function
            End If
            Dim ruta_server As String = ""
            Dim tipo_notificacion As Integer = 1
            Dim correo_copia As String = ""
            Dim Refclas_ra_config As New Class_ra_config_notifica_correo
            If tipo_envio = 1 Then
                Result = Refclas_ra_config.Solicita_estado_ruta_notificacion(ruta_server,
                                                                             tipo_notificacion,
                                                                             correo_copia)
                If Result <> "YES" Then
                    Enviando_correo_electronico_documetos_tarea_workflow = Result
                    Exit Function
                End If
            End If
            Dim uri = ruta_server & "/workflow/Handler_image_scrip_wf.ashx?rut_image="

            '********************************
            'Consulta datos cuerpo mensaje
            '********************************
            Dim Refclas_respuesta As New Classgestionrespuesta
            Dim correo_usuario_gestion As String = ""
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              correo_usuario_gestion)
            If Result <> "YES" Then
                Enviando_correo_electronico_documetos_tarea_workflow = Result
                Exit Function
            End If
            '*****************************************
            'Solcita el cuerpo del correo electrónico
            '*****************************************
            Dim ConTenido As String = ""
            Result = Me.Solicita_contenido_cuerpo_mensaje_envio_dcoumento_correo_electronico(ConTenido,
                                                                                             contenido_destinatario,
                                                                                             correo_usuario_gestion)
            If Result <> "YES" Then
                Enviando_correo_electronico_documetos_tarea_workflow = Result
                Exit Function
            End If
            '*****************************************
            'Genera matriz de documentos a adjuntar
            '*****************************************
            Dim matri_documentos() As String
            Erase matri_documentos
            Dim Matri_tempo_doc_copia() As String = Nothing
            Dim matri_id_imagen() As Integer
            Erase matri_id_imagen
            Result = Me.Solicita_matris_documentos_workflow_envio_correo(stru_documentos_compartidos,
                                                                         matri_documentos,
                                                                         ruta_tempo,
                                                                         Matri_tempo_doc_copia)
            If Result <> "YES" Then
                Enviando_correo_electronico_documetos_tarea_workflow = Result
                Exit Function
            End If

            '-----------------------------------------------
            'Agrega los documentos para correo electrónico
            '-----------------------------------------------
            If Not matri_documentos Is Nothing Then
                If tipo_envio <> 1 Then
                    For i As Integer = 0 To matri_documentos.Length - 1
                        Dim file_inf As New FileInfo(matri_documentos(i))
                        Dim extension As String = file_inf.Extension.Replace(".", "/")
                        Dim fileStream As FileStream = File.OpenRead(matri_documentos(i))
                        Dim memStream As MemoryStream = New MemoryStream()
                        memStream.SetLength(fileStream.Length)
                        fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                        fileStream.Close()
                        correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos(i)), "application" & extension))
                    Next
                Else
                    Dim HTTEXT As String = ""
                    HTTEXT = "<Font Color=Red> " & "<p>" & " Los link a continuación representan el acceso a un archivo compartido en nuestros servidores que " &
                     " estará disponible   " &
                      " es una lapso de tiempo no mayor de cinco (5) días a partir de este envío, por favor descargar y conservar si es necesario. " &
                     "</p> </Font> <br>"
                    HTTEXT = HTTEXT + "<table>"
                    For i As Integer = 0 To matri_documentos.Length - 1
                        Dim has_huella_md5_escript As String = ""
                        Result = encriptacion.encript_md5(matri_documentos(i),
                                                        "7894561230!",
                                                        has_huella_md5_escript)

                        If Result <> "YES" Then
                            Enviando_correo_electronico_documetos_tarea_workflow = Result
                            Exit Function
                        End If

                        HTTEXT = HTTEXT &
                            "<tr>" &
                               "<td><a href=" & uri & has_huella_md5_escript & "> Descarga archivo relacionado aquí </a></td>" &
                           "</tr>"
                    Next
                    If Not Matri_tempo_doc_copia Is Nothing Then
                        For i As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                            Dim has_huella_md5_escript As String = ""
                            Result = encriptacion.encript_md5(Matri_tempo_doc_copia(i),
                                                            "7894561230!",
                                                            has_huella_md5_escript)

                            If Result <> "YES" Then
                                Enviando_correo_electronico_documetos_tarea_workflow = Result
                                Exit Function
                            End If

                            HTTEXT = HTTEXT &
                                "<tr>" &
                                   "<td><a href=" & uri & has_huella_md5_escript & "> Descarga archivo relacionado aquí </a></td>" &
                               "</tr>"
                        Next
                    End If

                    HTTEXT = HTTEXT & "</table>"
                    ConTenido = HTTEXT & ConTenido
                End If

            Else
                Enviando_correo_electronico_documetos_tarea_workflow = "Imposible encontrar documentos para relacionar al correo eletrónico "
                Exit Function
            End If
            '------------------------------
            'Verifica si tiene correo
            '------------------------------
            Dest_Smtp = correo_destinatario
            '--------------------------------
            'Envia correo electrónico
            '--------------------------------
            'Declaro la variable para enviar el correo
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            correo.Subject = Asunto
            Me.Agrega_destinatarios_formato_estructurado(Dest_Smtp,
                                                         correo)
            If Result <> "YES" Then
                Enviando_correo_electronico_documetos_tarea_workflow = Result
                Exit Function
            End If
            Me.Agrega_destinatarios_formato_estructurado_copia(copia_destinatario,
                                                               correo)
            If Result <> "YES" Then
                Enviando_correo_electronico_documetos_tarea_workflow = Result
                Exit Function
            End If
            correo.BodyEncoding = System.Text.Encoding.UTF8
            '*********************************
            'Configuracion de cuerpo de correo
            '*********************************
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = ConTenido
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If
            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP, RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            '**********************************
            'Elimina los archivos
            '**********************************
            If chekbox_pdf = 1 And chekbox_adjunta = 1 Then
                If Not matri_documentos Is Nothing Then
                    If tipo_envio <> 1 Then
                        For i As Integer = 0 To matri_documentos.Length - 1
                            If File.Exists(matri_documentos(i)) = True Then
                                File.Delete(matri_documentos(i))
                            End If
                        Next
                    End If
                End If
            End If
            If Not Matri_tempo_doc_copia Is Nothing Then
                If tipo_envio <> 1 Then
                    For i As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                        If File.Exists(Matri_tempo_doc_copia(i)) = True Then
                            File.Delete(Matri_tempo_doc_copia(i))
                        End If
                    Next
                End If
            End If
            Enviando_correo_electronico_documetos_tarea_workflow = "YES"
        Catch ex As Exception
            Enviando_correo_electronico_documetos_tarea_workflow = "Inconsistencia general función Enviando_correo_electronico_documetos_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Enviando_correo_electronico_archivo_de_produccion_documental(ByVal matri_id_registro_producion() As Long,
                                                                          ByVal Asunto As String,
                                                                          ByVal correo_destinatario As String,
                                                                          ByVal ID_CONFIG As Integer,
                                                                          ByVal copia_destinatario As String,
                                                                          ByVal contenido_destinatario As String,
                                                                          ByVal Usuario_Smtp As String,
                                                                          ByVal pasword_smtp As String,
                                                                          ByVal ruta_tempo As String,
                                                                          ByVal chekbox_adjunta As Integer,
                                                                          ByVal chekbox_pdf As Integer,
                                                                          ByVal chekbox_lectura As Integer,
                                                                          ByVal chekbox_pasw As Integer,
                                                                          ByVal tipo_envio As Integer) As String
        Try
            '********************************
            'Solicita datos de configuración
            '********************************
            Dim correo As New System.Net.Mail.MailMessage()
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Dim Dest_Smtp As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Enviando_correo_electronico_archivo_de_produccion_documental = Result
                Exit Function
            End If
            Dim ruta_server As String = ""
            Dim tipo_notificacion As Integer = 1
            Dim correo_copia As String = ""
            Dim Refclas_ra_config As New Class_ra_config_notifica_correo
            If tipo_envio = 1 Then
                Result = Refclas_ra_config.Solicita_estado_ruta_notificacion(ruta_server,
                                                                             tipo_notificacion,
                                                                             correo_copia)
                If Result <> "YES" Then
                    Enviando_correo_electronico_archivo_de_produccion_documental = Result
                    Exit Function
                End If
            End If
            Dim uri = ruta_server & "/workflow/Handler_image_scrip_wf.ashx?rut_image="

            '********************************
            'Consulta datos cuerpo mensaje
            '********************************
            Dim Refclas_respuesta As New Classgestionrespuesta
            Dim correo_usuario_gestion As String = ""
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              correo_usuario_gestion)
            If Result <> "YES" Then
                Enviando_correo_electronico_archivo_de_produccion_documental = Result
                Exit Function
            End If
            '*****************************************
            'Solcita el cuerpo del correo electrónico
            '*****************************************
            Dim ConTenido As String = ""
            Result = Me.Solicita_contenido_cuerpo_mensaje_envio_dcoumento_correo_electronico(ConTenido,
                                                                                             contenido_destinatario,
                                                                                             correo_usuario_gestion)
            If Result <> "YES" Then
                Enviando_correo_electronico_archivo_de_produccion_documental = Result
                Exit Function
            End If
            '*****************************************
            'Genera matriz de documentos a adjuntar
            '*****************************************
            Dim matri_documentos() As String
            Erase matri_documentos
            Dim Matri_tempo_doc_copia() As String = Nothing
            Dim matri_tempo_doc_link() As String = Nothing
            Dim Refclas_registro_producion As New ClassGaProducionDocumental
            Result = Refclas_registro_producion.Solicita_matris_documentos_producion_documental(matri_id_registro_producion,
                                                                                                matri_documentos,
                                                                                                ruta_tempo,
                                                                                                Matri_tempo_doc_copia)
            If Result <> "YES" Then
                Enviando_correo_electronico_archivo_de_produccion_documental = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Agrega los documentos para correo electrónico
            '-----------------------------------------------
            If tipo_envio <> 1 Then
                If Not matri_documentos Is Nothing Then
                    For i As Integer = 0 To matri_documentos.Length - 1
                        Dim file_inf As New FileInfo(matri_documentos(i))
                        Dim extension As String = file_inf.Extension.Replace(".", "/")
                        Dim fileStream As FileStream = File.OpenRead(matri_documentos(i))
                        Dim memStream As MemoryStream = New MemoryStream()
                        memStream.SetLength(fileStream.Length)
                        fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                        fileStream.Close()
                        correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos(i)), "application" & extension))
                    Next
                Else
                    Enviando_correo_electronico_archivo_de_produccion_documental = "Imposible encontrar documentos para relacionar al correo eletrónico "
                    Exit Function
                End If
            Else
                Dim HTTEXT As String = ""
                If Not matri_documentos Is Nothing Then
                    HTTEXT = "<Font Color=Red> " & "<p>" & " Los link a continuación representan el acceso a un archivo compartido en nuestros servidores que estará disponible   " &
                      " es una lapso de tiempo no mayor de cinco (5) días a partir de este envío, por favor descargar y conservar si es necesario. " &
                     "</p> </Font> <br>"
                    HTTEXT = HTTEXT + "<table>"
                    For i As Integer = 0 To matri_documentos.Length - 1
                        Dim has_huella_md5_escript As String = ""
                        Result = encriptacion.encript_md5(matri_documentos(i),
                                                        "7894561230!",
                                                        has_huella_md5_escript)

                        If Result <> "YES" Then
                            Enviando_correo_electronico_archivo_de_produccion_documental = Result
                            Exit Function
                        End If

                        HTTEXT = HTTEXT &
                            "<tr>" &
                               "<td><a href=" & uri & has_huella_md5_escript & "> Descarga archivo relacionado aquí </a></td>" &
                           "</tr>"
                    Next
                    If Not Matri_tempo_doc_copia Is Nothing Then
                        For i As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                            Dim has_huella_md5_escript As String = ""
                            Result = encriptacion.encript_md5(Matri_tempo_doc_copia(i),
                                                            "7894561230!",
                                                            has_huella_md5_escript)

                            If Result <> "YES" Then
                                Enviando_correo_electronico_archivo_de_produccion_documental = Result
                                Exit Function
                            End If

                            HTTEXT = HTTEXT &
                                "<tr>" &
                                   "<td><a href=" & uri & has_huella_md5_escript & "> Descarga archivo relacionado aquí </a></td>" &
                               "</tr>"
                        Next
                    End If
                    HTTEXT = HTTEXT & "</table>"
                    ConTenido = HTTEXT & ConTenido
                End If
            End If

            '------------------------------
            'Verifica si tiene correo
            '------------------------------
            Dest_Smtp = correo_destinatario
            '--------------------------------
            'Envia correo electrónico
            '--------------------------------
            'Declaro la variable para enviar el correo
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP,
                                                          "Sistema de gestión Docuarchi.net web")
            correo.Subject = Asunto
            Me.Agrega_destinatarios_formato_estructurado(Dest_Smtp,
                                                         correo)
            If Result <> "YES" Then
                Enviando_correo_electronico_archivo_de_produccion_documental = Result
                Exit Function
            End If
            Me.Agrega_destinatarios_formato_estructurado_copia(copia_destinatario,
                                                               correo)
            If Result <> "YES" Then
                Enviando_correo_electronico_archivo_de_produccion_documental = Result
                Exit Function
            End If
            correo.BodyEncoding = System.Text.Encoding.UTF8
            '*********************************
            'Configuracion de cuerpo de correo
            '*********************************
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = ConTenido

            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If
            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP,
                                                                        RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            '**********************************
            'Elimina los archivos
            '**********************************
            If chekbox_pdf = 1 And chekbox_adjunta = 1 Then
                If tipo_envio <> 1 Then
                    If Not matri_documentos Is Nothing Then
                        For i As Integer = 0 To matri_documentos.Length - 1
                            If File.Exists(matri_documentos(i)) = True Then
                                File.Delete(matri_documentos(i))
                            End If
                        Next
                    End If
                End If

            End If
            If Not Matri_tempo_doc_copia Is Nothing Then
                If tipo_envio <> 1 Then
                    For i As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                        If File.Exists(Matri_tempo_doc_copia(i)) = True Then
                            File.Delete(Matri_tempo_doc_copia(i))
                        End If
                    Next
                End If
            End If
            Enviando_correo_electronico_archivo_de_produccion_documental = "YES"
        Catch ex As Exception
            Enviando_correo_electronico_archivo_de_produccion_documental = "Inconsistencia general función Enviando_correo_electronico_archivo_de_produccion_documental " & ex.Message
        End Try
    End Function
    Function Enviando_Correo_Smtp_radicado(ByVal Radicado As String,
                                           ByVal id_plantilla As Integer,
                                           ByVal Asunto As String,
                                           ByVal correo_destinatario As String,
                                           ByVal ID_CONFIG As Integer,
                                           ByVal copia_destinatario As String,
                                           ByVal contenido_destinatario As String,
                                           ByVal Usuario_Smtp As String,
                                           ByVal pasword_smtp As String,
                                           ByVal ruta_tempo As String,
                                           ByVal chekbox_adjunta As Integer,
                                           ByVal chekbox_pdf As Integer,
                                           ByVal chekbox_lectura As Integer,
                                           ByVal chekbox_pasw As Integer,
                                           ByVal tipo_envio As Integer) As String
        Try
            Dim ConTenido As String = ""
            Dim Nombre_Plantilla As String = ""
            Dim Refclas As New ClassRadicador
            Dim Dest_Smtp As String = ""
            '********************************
            'Solicita datos de configuración
            '********************************
            Dim correo As New System.Net.Mail.MailMessage()
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_radicado = Result
                Exit Function
            End If
            Dim ruta_server As String = ""
            Dim tipo_notificacion As Integer = 1
            Dim correo_copia As String = ""
            Dim Refclas_ra_config As New Class_ra_config_notifica_correo
            If tipo_envio = 1 Then
                Result = Refclas_ra_config.Solicita_estado_ruta_notificacion(ruta_server,
                                                                             tipo_notificacion,
                                                                             correo_copia)
                If Result <> "YES" Then
                    Enviando_Correo_Smtp_radicado = Result
                    Exit Function
                End If
            End If
            Dim uri = ruta_server & "/workflow/Handler_image_scrip_wf.ashx?rut_image="
            Result = ""
            Dim Ref_Clas_sytem_plantilla As New Class_system_plantilla_radicado
            Result = Ref_Clas_sytem_plantilla.Solicita_nombre_plantilla_radicado(id_plantilla,
                                                                                 Nombre_Plantilla)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_radicado = Result
                Exit Function
            End If
            '********************************
            'Retrorna asunto
            '********************************
            Dim ref_asunto As String = ""
            Result = Retorna_Asunto_Radicado_Notificacion(id_plantilla,
                                                          Nombre_Plantilla,
                                                          ref_asunto,
                                                          Radicado)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_radicado = Result
                Exit Function
            End If
            Asunto = Asunto & " " & ref_asunto
            '********************************
            'Consulta datos cuerpo mensaje
            '********************************
            Dim Refclas_respuesta As New Classgestionrespuesta
            Dim correo_usuario_gestion As String = ""
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_correo_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              correo_usuario_gestion)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_radicado = Result
                Exit Function
            End If
            Result = ""
            Result = Consulta_Datos_Cuerpo_Mensaje(Nombre_Plantilla,
                                                   ConTenido,
                                                   Radicado,
                                                   Dest_Smtp,
                                                   contenido_destinatario,
                                                   correo_usuario_gestion)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_radicado = Result
                Exit Function
            End If
            '********************************
            'Retorna campo gabinete busqueda
            '********************************
            Dim gabinete As String = ""
            Dim campo_busqueda As String = ""
            Dim matri_documentos() As String
            Erase matri_documentos
            Dim Matri_tempo_doc_copia() As String = Nothing
            If chekbox_adjunta = 1 Then
                Result = Me.Retorna_relacion_gabinete_busqueda_predeterminado(id_plantilla,
                                                                              gabinete,
                                                                              campo_busqueda)
                If Result <> "YES" Then
                    Enviando_Correo_Smtp_radicado = Result
                    Exit Function
                End If
                '*******************************************
                'Busca concordancia radicado
                '*******************************************
                Dim matri_id_imagen() As Integer
                Erase matri_id_imagen
                Result = Retorna_id_imagen_docuarchi(gabinete,
                                                     campo_busqueda,
                                                     Radicado.ToString,
                                                     matri_id_imagen,
                                                     0)
                If Result <> "YES" Then
                    Enviando_Correo_Smtp_radicado = Result
                    Exit Function
                End If
                '*******************************************
                'Carga los archivos desde el servidor
                '*******************************************   
                If Not matri_id_imagen Is Nothing Then
                    Result = Enviando_Documento_Correo_Gabinete(gabinete,
                                                                matri_id_imagen,
                                                                Radicado,
                                                                matri_documentos,
                                                                ruta_tempo,
                                                                Matri_tempo_doc_copia)
                    If Result <> "YES" Then
                        Enviando_Correo_Smtp_radicado = Result
                        Exit Function
                    End If
                End If
                '*********************************************
                'Agrega los documentos para correo eletrónico
                '*********************************************
                If Not matri_documentos Is Nothing Then
                    If tipo_envio <> 1 Then
                        For i As Integer = 0 To matri_documentos.Length - 1
                            Dim file_inf As New FileInfo(matri_documentos(i))
                            Dim extension As String = file_inf.Extension.Replace(".", "/")
                            Dim fileStream As FileStream = File.OpenRead(matri_documentos(i))
                            Dim memStream As MemoryStream = New MemoryStream()
                            memStream.SetLength(fileStream.Length)
                            fileStream.Read(memStream.GetBuffer(), 0, fileStream.Length)
                            fileStream.Close()
                            correo.Attachments.Add(New Attachment(memStream, Path.GetFileName(matri_documentos(i)), "application" & extension))
                        Next
                    Else
                        Dim HTTEXT As String = ""

                        HTTEXT = "<Font Color=Red> " & "<p>" & " Los link a continuación representan el acceso a un archivo compartido en nuestros servidores que " &
                       "estará disponible   " &
                      " es una lapso de tiempo no mayor de cinco (5) días a partir de este envío, por favor descargar y conservar si es necesario. " &
                     "</p> </Font> <br>"
                        HTTEXT = HTTEXT + "<table>"
                        For i As Integer = 0 To matri_documentos.Length - 1
                            Dim has_huella_md5_escript As String = ""
                            Result = encriptacion.encript_md5(matri_documentos(i),
                                                            "7894561230!",
                                                            has_huella_md5_escript)

                            If Result <> "YES" Then
                                Enviando_Correo_Smtp_radicado = Result
                                Exit Function
                            End If

                            HTTEXT = HTTEXT &
                                "<tr>" &
                                   "<td><a href=" & uri & has_huella_md5_escript & "> Descarga archivo relacionado aquí </a></td>" &
                               "</tr>"
                        Next
                        HTTEXT = HTTEXT & "</table>"
                        ConTenido = HTTEXT & ConTenido
                    End If
                End If

            End If
            '******************************
            'Verifica si tiene correo
            '******************************
            Dest_Smtp = correo_destinatario
            '********************************
            'Envia correo electronico
            '********************************
            'Declaro la variable para enviar el correo
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP,
                                                          "Sistema de gestión Docuarchi.net web")
            correo.Subject = Asunto
            Me.Agrega_destinatarios_formato_estructurado(Dest_Smtp, correo)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_radicado = Result
                Exit Function
            End If
            Me.Agrega_destinatarios_formato_estructurado_copia(copia_destinatario,
                                                               correo)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_radicado = Result
                Exit Function
            End If
            correo.BodyEncoding = System.Text.Encoding.UTF8
            '*********************************
            'Configuracion de cuerpo de correo
            '*********************************
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = ConTenido
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If
            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP, RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            '**********************************
            'Elimina los archivos
            '**********************************
            If chekbox_pdf = 1 And chekbox_adjunta = 1 Then
                If Not matri_documentos Is Nothing Then
                    If tipo_envio <> 1 Then
                        For i As Integer = 0 To matri_documentos.Length - 1
                            If File.Exists(matri_documentos(i)) = True Then
                                File.Delete(matri_documentos(i))
                            End If
                        Next
                    End If
                End If
            End If
            If Not Matri_tempo_doc_copia Is Nothing Then
                For i As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                    If File.Exists(Matri_tempo_doc_copia(i)) = True Then
                        File.Delete(Matri_tempo_doc_copia(i))
                    End If
                Next
            End If
            Enviando_Correo_Smtp_radicado = "YES"
        Catch ex As Exception
            Enviando_Correo_Smtp_radicado = "Inconsistencia general  función Enviando_Correo_Smtp_radicado  " & ex.Message
        End Try
    End Function
    Function Solicita_matris_documentos_workflow_envio_correo(ByVal Matri_Id_Doc() As stru_documentos_compartidos,
                                                              ByRef Matri_Temp_Doc() As String,
                                                              ByVal ruta_tempo As String,
                                                              ByRef Matri_tempo_doc_copia() As String) As String
        Try
            Dim Refclasvis As New ClassVisualisaDocumento
            Dim Refclas As New ClassWorkflowReportes
            Dim IcontMatriPrinc As Integer = 0
            Dim Matricopia() As String = Nothing
            Matri_tempo_doc_copia = Nothing
            Dim Result As String = ""
            Dim i_contador As Integer = 0
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim nombre_tipo_documental As String = ""
            Dim Refclas_produccion As New ClassGaProducionDocumental
            Dim Refclas_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim nombre_gabinete As String = ""
            For i As Integer = 0 To Matri_Id_Doc.Length - 1
                nombre_gabinete = Matri_Id_Doc(i).nombre_gabinete
                Erase Matricopia
                Dim Ref_Class_system1 As New Class_system1
                Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete,
                                                                                                              inventario_documental,
                                                                                                              aplica_trd,
                                                                                                              asigna_unidad)
                If Result <> "YES" Then
                    Solicita_matris_documentos_workflow_envio_correo = Result
                    Exit Function
                End If
                '-----------------------------------------------------------------------------------------------
                'Busca el nombre del documento si el gabinete tiene la opción aplicar tabla de retención activa
                '-----------------------------------------------------------------------------------------------
                If aplica_trd <> 0 Then
                    Refclas_workflow_digitalizacion.Retorna_tipo_documento_gabinete(nombre_gabinete,
                                                                                    Matri_Id_Doc(i).id_imagen,
                                                                                    nombre_tipo_documental)
                    If Result <> "YES" Then
                        Solicita_matris_documentos_workflow_envio_correo = Result
                        Exit Function
                    End If
                    If nombre_tipo_documental <> "" Then
                        Result = Me.Normaliza_nombre_archivo(nombre_tipo_documental)
                        If Result <> "YES" Then
                            Solicita_matris_documentos_workflow_envio_correo = Result
                            Exit Function
                        End If

                    End If
                End If
                Result = Refclasvis.Genera_Matris_Documentos_Almacenados(Matri_Id_Doc(i).id_imagen,
                                                                         nombre_gabinete,
                                                                         Matricopia)
                If Result <> "YES" Then
                    Solicita_matris_documentos_workflow_envio_correo = Result
                    Exit Function
                End If

                If nombre_tipo_documental <> "" Then
                    Result = Me.Normaliza_nombre_archivo(nombre_tipo_documental)
                    If Result <> "YES" Then
                        Solicita_matris_documentos_workflow_envio_correo = Result
                        Exit Function
                    End If
                End If
                Dim date_string = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(":", "-")
                '-------------------------------------------------------------------
                'Convierte en tif los documentos de formato tiff, jpg y bmp
                '-------------------------------------------------------------------
                Dim file_inf As New FileInfo(Matricopia(1))
                If UCase(file_inf.Extension) = ".TIF" Or UCase(file_inf.Extension) = ".TIFF" _
                    Or UCase(file_inf.Extension) = ".JPG" Or UCase(file_inf.Extension) = ".BMP" Then
                    Dim Ruta_Exportacion As String = ruta_tempo
                    Dim ref_matri_tempo() As String = Nothing
                    Dim it As Integer = 0
                    For i2 As Integer = 1 To Matricopia.Length - 1
                        ReDim Preserve ref_matri_tempo(it)
                        ref_matri_tempo(it) = Matricopia(i2)
                        it = it + 1
                    Next
                    Result = Refclas.Convertir_tif_pdf_correo(ref_matri_tempo,
                                                              Ruta_Exportacion,
                                                              "YES",
                                                              0,
                                                              "",
                                                              i)
                    If Result <> "YES" Then
                        Solicita_matris_documentos_workflow_envio_correo = Result
                        Exit Function
                    Else
                        Dim file_inf_copia As New FileInfo(Ruta_Exportacion)
                        Dim tempo_documento As String = ""
                        If nombre_tipo_documental <> "" Then
                            tempo_documento = ruta_tempo & date_string & "-" & nombre_gabinete & "-" & "" & file_inf_copia.Extension
                            If File.Exists(tempo_documento) Then
                                Kill(tempo_documento)
                            End If
                            File.Move(Ruta_Exportacion, tempo_documento)
                        Else
                            tempo_documento = ruta_tempo & date_string & "-" & nombre_gabinete & "-" & file_inf_copia.Name
                            If File.Exists(tempo_documento) Then
                                Kill(tempo_documento)
                            End If
                            File.Move(Ruta_Exportacion, tempo_documento)
                        End If
                        ReDim Preserve Matri_tempo_doc_copia(i_contador)
                        Matri_tempo_doc_copia(i_contador) = tempo_documento
                    End If
                Else
                    Dim file_inf_copia As New FileInfo(Matricopia(1))
                    Dim tempo_documento As String = ""
                    If nombre_tipo_documental <> "" Then
                        tempo_documento = ruta_tempo & date_string & "-" & nombre_gabinete & "-" & "" & file_inf_copia.Extension
                        If File.Exists(tempo_documento) Then
                            Kill(tempo_documento)
                        End If
                        File.Copy(Matricopia(1), tempo_documento)
                    Else
                        tempo_documento = ruta_tempo & date_string & "-" & nombre_gabinete & "-" & file_inf_copia.Name
                        If File.Exists(tempo_documento) Then
                            Kill(tempo_documento)
                        End If
                        File.Copy(Matricopia(1), tempo_documento)
                    End If
                    ReDim Preserve Matri_tempo_doc_copia(i_contador)
                    Matri_tempo_doc_copia(i_contador) = tempo_documento
                End If
                i_contador = i_contador + 1
            Next
            '------------------------------------------------------------------------------------------
            'Agrega en zip los documentos relacionados cuando sobre pasa el máximo número de adjuntos
            '------------------------------------------------------------------------------------------
            If Not Matri_tempo_doc_copia Is Nothing Then
                If Matri_tempo_doc_copia.Length > 20 Then
                    Dim zip As New ZipFile()
                    For i As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                        Dim temp_archivo As String = Matri_tempo_doc_copia(i)
                        If System.IO.File.Exists(temp_archivo) = True Then
                            zip.AddFile(Matri_tempo_doc_copia(i), "FilesAdjuntos")
                        End If
                    Next
                    Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                    zip.Save(ruta_tempo & zipName)
                    ReDim Preserve Matri_Temp_Doc(0)
                    Matri_Temp_Doc(0) = ruta_tempo & zipName
                    Solicita_matris_documentos_workflow_envio_correo = "YES"
                    Exit Function
                Else
                    If Not Matri_tempo_doc_copia Is Nothing Then
                        For i2 As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                            ReDim Preserve Matri_Temp_Doc(i2)
                            Matri_Temp_Doc(i2) = Matri_tempo_doc_copia(i2)
                        Next
                        Erase Matri_tempo_doc_copia
                    End If
                    Solicita_matris_documentos_workflow_envio_correo = "YES"
                    Exit Function
                End If
            Else
                Solicita_matris_documentos_workflow_envio_correo = "No se encontraron documentos relacionados para adjuntar, función Solicita_matris_documentos_workflow_envio_correo"
                Exit Function
            End If
            Solicita_matris_documentos_workflow_envio_correo = "YES"
        Catch ex As Exception
            Solicita_matris_documentos_workflow_envio_correo = "Inconsistencia general función Solicita_matris_documentos_workflow_envio_correo " & ex.Message
        End Try
    End Function

    Function Enviando_Documento_Correo_Gabinete(ByVal nombre_gabinete As String,
                                                ByVal Matri_Id_Doc() As Integer,
                                                ByVal radicado As String,
                                                ByRef Matri_Temp_Doc() As String,
                                                ByVal ruta_tempo As String,
                                                ByRef Matri_tempo_doc_copia() As String) As String
        Try
            Dim Refclasvis As New ClassVisualisaDocumento
            Dim Refclas As New ClassWorkflowReportes
            Dim IcontMatriPrinc As Integer = 0
            Dim Matricopia() As String = Nothing
            Matri_tempo_doc_copia = Nothing
            Dim Result As String = ""
            Dim i_contador As Integer = 0
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim nombre_tipo_documental As String = ""
            Dim Refclas_produccion As New ClassGaProducionDocumental
            Dim Refclas_workflow_digitalizacion As New ClassWorkflowDigitalizacion
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete,
                                                                                                          inventario_documental,
                                                                                                          aplica_trd,
                                                                                                          asigna_unidad)
            If Result <> "YES" Then
                Enviando_Documento_Correo_Gabinete = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'Recorre la matriz de id de los documentos relacioandos
            '--------------------------------------------------------
            For i As Integer = 0 To Matri_Id_Doc.Length - 1
                Erase Matricopia
                nombre_tipo_documental = ""
                Result = Refclasvis.Genera_Matris_Documentos_Almacenados(Matri_Id_Doc(i),
                                                                         nombre_gabinete,
                                                                         Matricopia)
                If Result <> "YES" Then
                    Enviando_Documento_Correo_Gabinete = Result
                    Exit Function
                End If
                '-----------------------------------------------------------------------------------------------
                'Busca el nombre del documento si el gabinete tiene la opción aplicar tabla de retención activa
                '-----------------------------------------------------------------------------------------------
                If aplica_trd <> 0 Then
                    Refclas_workflow_digitalizacion.Retorna_tipo_documento_gabinete(nombre_gabinete,
                                                                                    Matri_Id_Doc(i),
                                                                                    nombre_tipo_documental)
                    If Result <> "YES" Then
                        Enviando_Documento_Correo_Gabinete = Result
                        Exit Function
                    End If
                    If nombre_tipo_documental <> "" Then
                        Result = Me.Normaliza_nombre_archivo(nombre_tipo_documental)
                        If Result <> "YES" Then
                            Enviando_Documento_Correo_Gabinete = Result
                            Exit Function
                        End If

                    End If
                End If
                '-------------------------------------------------------------------
                'Convierte en tif los documentos de formato tiff, jpg y bmp
                '-------------------------------------------------------------------
                Dim file_inf As New FileInfo(Matricopia(1))
                If UCase(file_inf.Extension) = ".TIF" Or UCase(file_inf.Extension) = ".TIFF" _
                    Or UCase(file_inf.Extension) = ".JPG" Or UCase(file_inf.Extension) = ".BMP" Then
                    Dim Ruta_Exportacion As String = ruta_tempo
                    Dim ref_matri_tempo() As String = Nothing
                    Dim it As Integer = 0
                    For i2 As Integer = 1 To Matricopia.Length - 1
                        ReDim Preserve ref_matri_tempo(it)
                        ref_matri_tempo(it) = Matricopia(i2)
                        it = it + 1
                    Next
                    Result = Refclas.Convertir_tif_pdf_correo(ref_matri_tempo,
                                                              Ruta_Exportacion,
                                                              "YES",
                                                              0,
                                                              "",
                                                              i)
                    If Result <> "YES" Then
                        Enviando_Documento_Correo_Gabinete = Result
                        Exit Function
                    Else
                        Dim file_inf_copia As New FileInfo(Ruta_Exportacion)
                        Dim tempo_documento As String = ""
                        If nombre_tipo_documental <> "" Then
                            tempo_documento = ruta_tempo & i & "-" & nombre_tipo_documental & file_inf_copia.Extension
                            If File.Exists(tempo_documento) Then
                                Kill(tempo_documento)
                            End If
                            File.Move(Ruta_Exportacion, tempo_documento)
                        Else
                            tempo_documento = ruta_tempo & i & "-" & file_inf_copia.Name
                            If File.Exists(tempo_documento) Then
                                Kill(tempo_documento)
                            End If
                            File.Move(Ruta_Exportacion,
                                      tempo_documento)
                        End If
                        ReDim Preserve Matri_tempo_doc_copia(i_contador)
                        Matri_tempo_doc_copia(i_contador) = tempo_documento
                    End If
                Else
                    Dim file_inf_copia As New FileInfo(Matricopia(1))
                    Dim tempo_documento As String = ""
                    If nombre_tipo_documental <> "" Then
                        tempo_documento = ruta_tempo & i & "-" & nombre_tipo_documental & file_inf_copia.Extension
                        If File.Exists(tempo_documento) Then
                            Kill(tempo_documento)
                        End If
                        File.Copy(Matricopia(1), tempo_documento)
                    Else
                        tempo_documento = ruta_tempo & i & "-" & file_inf_copia.Name
                        If File.Exists(tempo_documento) Then
                            Kill(tempo_documento)
                        End If
                        File.Copy(Matricopia(1), tempo_documento)
                    End If
                    ReDim Preserve Matri_tempo_doc_copia(i_contador)
                    Matri_tempo_doc_copia(i_contador) = tempo_documento
                End If
                i_contador = i_contador + 1
            Next
            '------------------------------------------------------------------------------------------
            'Agrega en zip los documentos relacionados cuando sobre pasa el máximo número de adjuntos
            '------------------------------------------------------------------------------------------
            If Not Matri_tempo_doc_copia Is Nothing Then
                If Matri_tempo_doc_copia.Length > 20 Then
                    Dim zip As New ZipFile()
                    For i As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                        Dim temp_archivo As String = Matri_tempo_doc_copia(i)
                        If System.IO.File.Exists(temp_archivo) = True Then
                            zip.AddFile(Matri_tempo_doc_copia(i), "FilesAdjuntos")
                        End If
                    Next
                    Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                    zip.Save(ruta_tempo & zipName)
                    ReDim Preserve Matri_Temp_Doc(0)
                    Matri_Temp_Doc(0) = ruta_tempo & zipName
                    Enviando_Documento_Correo_Gabinete = "YES"
                    Exit Function
                Else
                    If Not Matri_tempo_doc_copia Is Nothing Then
                        For i2 As Integer = 0 To Matri_tempo_doc_copia.Length - 1
                            ReDim Preserve Matri_Temp_Doc(i2)
                            Matri_Temp_Doc(i2) = Matri_tempo_doc_copia(i2)
                        Next
                        Erase Matri_tempo_doc_copia
                    End If
                    Enviando_Documento_Correo_Gabinete = "YES"
                    Exit Function
                End If
            Else
                Enviando_Documento_Correo_Gabinete = "No se encontraron documentos relacionados para adjuntar, función Enviando_Documento_Correo_Gabinete"
                Exit Function
            End If
            Enviando_Documento_Correo_Gabinete = "YES"
        Catch ex As Exception
            Enviando_Documento_Correo_Gabinete = "Inconsistencia general función Solicita_matris_documentos_producion_documental  " & ex.Message
        End Try
    End Function
    Function Normaliza_nombre_archivo(ByRef nombre_archivo As String) As String
        Try
            nombre_archivo = nombre_archivo.Replace("/", "")
            nombre_archivo = nombre_archivo.Replace("\", "")
            nombre_archivo = nombre_archivo.Replace(":", "")
            nombre_archivo = nombre_archivo.Replace("*", "")
            nombre_archivo = nombre_archivo.Replace("?", "")
            nombre_archivo = nombre_archivo.Replace("""", "")
            nombre_archivo = nombre_archivo.Replace("<", "")
            nombre_archivo = nombre_archivo.Replace(">", "")
            nombre_archivo = nombre_archivo.Replace("|", "")
            Normaliza_nombre_archivo = "YES"
        Catch ex As Exception
            Normaliza_nombre_archivo = "Inconsistencia general función Normaliza_nombre_archivo " & ex.Message
        End Try
    End Function
    Function Enviando_Documento_Correo_Gabinete_pdf(
                                                   ByVal Tipo_Adjunto As Integer,
                                                   ByRef Mtri_Principal() As String,
                                                   ByVal ruta_tempo As String,
                                                   ByVal nombre_gabinete As String,
                                                   ByVal Matri_Id_Doc() As Integer,
                                                   ByVal radicado As String,
                                                   ByVal lectura As Integer,
                                                   ByVal pasword As String, ByVal p As Integer) As String
        Try
            Dim Result As String = ""
            Dim Refclasvis As New ClassVisualisaDocumento
            Dim IcontMatriPrinc As Integer = 0
            Dim Matri_Temp_Doc() As String
            Erase Mtri_Principal
            For i As Integer = 0 To UBound(Matri_Id_Doc)
                Erase Matri_Temp_Doc
                Result = Refclasvis.Genera_Matris_Documentos_Almacenados(Matri_Id_Doc(i),
                            nombre_gabinete, Matri_Temp_Doc)
                If Result <> "YES" Then
                    Enviando_Documento_Correo_Gabinete_pdf = Result
                    Exit Function
                End If
                '******************************
                'Crea archivo pdf
                '******************************
                Dim Refclas As New ClassWorkflowReportes
                Dim Resiz As String = "YES"
                Result = ""
                Dim Ruta_Exportacion As String = ruta_tempo
                Dim ref_matri_tempo() As String = Nothing
                Dim it As Integer = 0
                For i2 As Integer = 1 To Matri_Temp_Doc.Length - 1
                    ReDim Preserve ref_matri_tempo(it)
                    ref_matri_tempo(it) = Matri_Temp_Doc(i2)
                    it = it + 1
                Next
                Result = Refclas.Convertir_tif_pdf(ref_matri_tempo, Ruta_Exportacion, Resiz, lectura, pasword)
                If Result <> "YES" Then
                    Enviando_Documento_Correo_Gabinete_pdf = Result
                    Exit Function
                End If
                'Elimina_Temporal(Ruta_Exportacion)
                ReDim Preserve Mtri_Principal(i)
                Mtri_Principal(i) = Ruta_Exportacion
            Next
            If Mtri_Principal.Length > 20 Then
                Enviando_Documento_Correo_Gabinete_pdf = "Sobrepaso el número de item adjuntos (20)"
                Exit Function
            End If
            Enviando_Documento_Correo_Gabinete_pdf = "YES"
        Catch ex As Exception
            Enviando_Documento_Correo_Gabinete_pdf = "Inconsistencia general  " & ex.HelpLink & " " & ex.Source & " " & ex.InnerException.Message
        End Try
    End Function
    Public Function Elimina_Temporal(ByVal Rutatemp As String) As String
        Try
            'Dim Rutatemp As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\"
            'Dim Rutatemp As String = System.IO.Path.GetTempPath()
            'Rutatemp = Rutatemp & "TEMP1MP" & UCase(Login_Usuario) & "\"
            If Directory.Exists(Rutatemp) = True Then
                Dim counter = My.Computer.FileSystem.GetFiles(Rutatemp)
                If counter.Count > 0 Then
                    Kill(Rutatemp & "*.*")
                End If

            End If
            Elimina_Temporal = "YES"
        Catch ex As Exception
            Elimina_Temporal = "YES"
        End Try
    End Function
    Function Retorna_id_imagen_docuarchi(ByVal nombre_gabinete As String,
        ByVal campo_consulta As String, ByVal valor_consulta As String,
        ByRef id_imagen() As Integer, ByVal confirma_existencia As Integer) As String
        Try

            Dim Parametro_Consulta As String = " select id from " & nombre_gabinete & " where " & campo_consulta & "='" & valor_consulta & "' or enlase='" & valor_consulta & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_imagen_docuarchi = " Función Verifica_Existencia_Campo_tabla dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If confirma_existencia = 1 Then
                    Retorna_id_imagen_docuarchi = "Imposible encontrar datos para el radicado " & valor_consulta & " en el gabinete " & nombre_gabinete
                    Exit Function
                Else
                    Retorna_id_imagen_docuarchi = "YES"
                    Exit Function
                End If

            Else

                Dim iconta As Integer = 0
                'While Dat_reader.Read()
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve id_imagen(iconta)
                    id_imagen(iconta) = Datset.Tables(0).Rows(i).Item(0)
                    iconta = iconta + 1
                Next
                Retorna_id_imagen_docuarchi = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_imagen_docuarchi = "Inconsistencia general funcion Retorna_id_imagen_docuarchi " & ex.Message
        End Try
    End Function
    Function Verifica_Existencia_Campo_tabla(ByVal nombre_tabla As String,
                                             ByVal nombre_campo As String,
                                             ByRef existencia_campo As String)
        '********************************************
        'Funcion : Verifica existencia campo en la
        'tabla informada
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-12-26
        '********************************************
        Try

            Dim Parametro_Consulta As String = "SHOW FIELDS from " & nombre_tabla & " where field='" & nombre_campo & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_Existencia_Campo_tabla = " Función Verifica_Existencia_Campo_tabla dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_campo = "NO"
                Verifica_Existencia_Campo_tabla = "YES"
                Exit Function
            Else
                existencia_campo = "YES"
                Verifica_Existencia_Campo_tabla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_Existencia_Campo_tabla = "Inconsistencia general funcion  Verifica_Existencia_Campo_tabla  " & ex.Message
        End Try
    End Function
    Function Solicita_contenido_cuerpo_mensaje_envio_dcoumento_correo_electronico(ByRef ConTenido As String,
                                                                                  ByVal datos_adicional_mensaje As String,
                                                                                  ByVal correo_electronico_respuesta As String) As String
        Try
            Dim HTTEXT As String = ""
            HTTEXT = "<Font Color=Red>" & "<p>" & "Importante!!  Si usted necesita responder a este correo electrónico, por favor remitase a la siguiente dirección correo  : " & correo_electronico_respuesta & "</p>" & "</Font>" &
            "<br>" &
            "<p>" & datos_adicional_mensaje & "</p>" &
            "<br>" &
            "<p> Detalle del Tramite : " & "</p>"
            Dim mensaj As String = "<html>" &
           "<head>" &
          "<title>" &
            "Gestor Documental " &
          "</title>" &
          "</head>" &
          "<body>" &
          HTTEXT &
           "<p>" &
           "<h2>Servidor Smtp  </h2>" &
           "</p>" &
          "<p>" & "Mensaje enviado el " &
                Now.ToShortDateString &
               " a las " & Now.ToLongTimeString & " , por favor abstenerse a responder a este correo electrónico cualquier tipo de duda o inconveniente" &
         "</p>" &
           "</body>" &
          "</html>"
            ConTenido = mensaj
            Solicita_contenido_cuerpo_mensaje_envio_dcoumento_correo_electronico = "YES"
        Catch ex As Exception
            Solicita_contenido_cuerpo_mensaje_envio_dcoumento_correo_electronico = "Inconsistencia general función Solicita_contenido_cuerpo_mensaje_envio_dcoumento_correo_electronico " & ex.Message
        End Try
    End Function

    Function Consulta_Datos_Cuerpo_Mensaje(ByVal Nombre_Plantilla As String,
                                           ByRef ConTenido As String,
                                           ByVal Consec_Radicado As String,
                                           ByRef Dest_Smtp As String,
                                           ByVal datos_adicional_mensaje As String,
                                           ByVal correo_electronico_respuesta As String) As String
        Try
            Dim Refclas As New ClassRadicador
            Dim Result As String = ""
            Dim tipo_plantilla As String = ""
            Dim Ref_Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Result = Ref_Class_system_plantilla_radicado.Retorna_Tipo_Plantilla_nombre(Nombre_Plantilla,
                                                                                       tipo_plantilla)
            If Result <> "YES" Then
                Consulta_Datos_Cuerpo_Mensaje = Result
                Exit Function
            End If
            Dim Parametro_Consulta As String = ""
            '----------------------------------------------
            'Verifica existencia campos
            '----------------------------------------------
            Dim Existencia_campo As String = "NO"
            Dim Campo_exist As String = ""
            Dim codigo_proceso As String = ""
            Result = Verifica_Existencia_Campo_tabla(Nombre_Plantilla, "CODIGOPROCESO", Existencia_campo)
            If Result <> "YES" Then
                Consulta_Datos_Cuerpo_Mensaje = Result
                Exit Function
            End If
            If Existencia_campo = "YES" Then
                Campo_exist = ",pr.CODIGOPROCESO"
            Else
                Campo_exist = ""
            End If

            If tipo_plantilla = "RADICACION ENTRANTE" Then
                Parametro_Consulta = " select pr.Consecutivo_Rad,pr.Consecutivo_CodBarra,pr.Fecha_Radicado,pr.Descripcion_Documento," &
            "pr.Numero_Folios,pr.Destinatario_Cor,pr.Remitente_Cor,rdi.correo_electronico" & Campo_exist & " from " & Nombre_Plantilla & " pr " &
            "inner join remit_dest_interno rdi on (rdi.id_remit_dest_int=pr.Destinatario_Externo_id_Dest_Ext)" &
            " where Consecutivo_Rad='" & Consec_Radicado & "'"
            Else
                Parametro_Consulta = " select pr.Consecutivo_Rad,pr.Consecutivo_CodBarra,pr.Fecha_Radicado,pr.Descripcion_Documento," &
                "pr.Numero_Folios,pr.Destinatario_Cor,pr.Remitente_Cor,rdi.correo_electronico" & Campo_exist & " from " & Nombre_Plantilla & " pr " &
                "inner join remit_dest_interno rdi on (rdi.id_remit_dest_int=pr.Remit_Dest_Interno_id_Remit_Dest_Int)" &
                " where Consecutivo_Rad='" & Consec_Radicado & "'"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Consulta_Datos_Cuerpo_Mensaje = " Función Consulta_Datos_Cuerpo_Mensaje dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_Datos_Cuerpo_Mensaje = "Imposible encontrar datos para el cuerpo de Correo "
                Exit Function
            Else
                If tipo_plantilla = "RADICACION ENTRANTE" Then
                    If Existencia_campo = "YES" Then
                        If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                            codigo_proceso = "-/-"
                        Else
                            codigo_proceso = Datset.Tables(0).Rows(0).Item(8).ToString & " - "
                        End If
                    End If
                    Dim HTTEXT As String = ""
                    HTTEXT = "<Font Color=Red>" & "<p>" & "Importante!!  Si usted necesita responder a este correo electrónico, por favor remitase a la siguiente dirección correo  : " & correo_electronico_respuesta & "</p>" & "</Font>" &
                    "<br>" &
                    "<p>" & datos_adicional_mensaje & "</p>" &
                    "<br>" &
                    "<p> Detalle del Tramite : " & "</p>" &
                    "<table>" &
                       "<tr>" &
                          "<td> Consecutivo Radicación : </td>" &
                          "<td>" & codigo_proceso & " REE- " & Datset.Tables(0).Rows(0).Item(0).ToString & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Consecutivo Cod_Barra : </td> " &
                            "<td>" & Datset.Tables(0).Rows(0).Item(1).ToString & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Fecha Radicación : </td>" &
                            "<td>" & Datset.Tables(0).Rows(0).Item(2).ToString & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Descripcion Radicación : </td> " &
                            "<td> " & Datset.Tables(0).Rows(0).Item(3).ToString & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Numero Folios : </td> " &
                            "<td>" & Datset.Tables(0).Rows(0).Item(4).ToString & "</td>" &
                       "</tr>" &
                       "<tr>" &
                            "<td> Destinatario Radicación : </td>" &
                             "<td>" & Datset.Tables(0).Rows(0).Item(5).ToString & "</td>" &
                       "</tr>" &
                       "<tr>" &
                           "<td> Remitente Radicación : </td> " &
                            "<td>" & Datset.Tables(0).Rows(0).Item(6).ToString & "</td>" &
                       "</tr>" &
                    "</table>" &
                    " <br> </br>" &
                    "<Font Color=Green>" &
                     "</Font>"
                    Dest_Smtp = Datset.Tables(0).Rows(0).Item(7).ToString
                    Dim mensaj As String = "<html>" &
                   "<head>" &
                  "<title>" &
                    "Gestor Documental " &
                  "</title>" &
                  "</head>" &
                  "<body>" &
                  HTTEXT &
                   "<p>" &
                   "<h2>Servidor Smtp  </h2>" &
                   "</p>" &
                  "<p>" & "Mensaje enviado el " &
                        Now.ToShortDateString &
                       " a las " & Now.ToLongTimeString & " , por favor abstenerse a responder a este correo electrónico cualquier tipo de duda o inconveniente" &
                 "</p>" &
                   "</body>" &
                  "</html>"
                    ConTenido = mensaj
                End If
                If tipo_plantilla = "RADICACION SALIENTE" Then
                    If Existencia_campo = "YES" Then
                        If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                            codigo_proceso = "-/-"
                        Else
                            codigo_proceso = Datset.Tables(0).Rows(0).Item(8).ToString & " - "
                        End If

                    End If

                    Dim HTTEXT = "<Font Color=Red>" & "<p>" & "Importante!!  Si usted necesita responder a este correo electrónico, por favor remitase a la siguiente dirección correo  : " & correo_electronico_respuesta & "</p>" & "</Font>" &
                    "<br>" &
                    "<p>" & datos_adicional_mensaje & "</p>" &
                    "<br>" &
                    "<p> Detalle del Tramite : " & "</p>" &
                    "<table>" &
                      "<tr>" &
                         "<td> Consecutivo de Radicación : </td>" &
                        "<td>" & codigo_proceso & " RIS- " & Datset.Tables(0).Rows(0).Item(0).ToString & "</td>" &
                      "</tr>" &
                      "<tr>" &
                         "<td> Consecutivo Código de Barra : </td>" &
                         "<td>" & Datset.Tables(0).Rows(0).Item(1).ToString & "</td>" &
                      "</tr>" &
                      "<tr>" &
                         "<td> Fecha Radicación : </td>" &
                         "<td>" & Datset.Tables(0).Rows(0).Item(2).ToString & "</td>" &
                      "</tr>" &
                      "<tr>" &
                          "<td> Descripción Radicación: </td>" &
                          "<td>" & Datset.Tables(0).Rows(0).Item(3).ToString & "</td>" &
                      "</tr>" &
                      "<tr>" &
                          "<td> Numero de Folios: </td> " &
                          "<td>" & Datset.Tables(0).Rows(0).Item(4).ToString & "</td>" &
                      "</tr>" &
                      "<tr>" &
                           "<td> Destinatario Radicación : </td> " &
                            "<td>" & Datset.Tables(0).Rows(0).Item(5).ToString & "</td>" &
                      "</tr>" &
                    "</table>" &
                    "<br></br> "
                    Dest_Smtp = Datset.Tables(0).Rows(0).Item(7).ToString
                    Dim mensaj As String = "<html>" &
                   "<head>" &
                  "<title>" &
                    "Gestor Documental " &
                  "</title>" &
                  "</head>" &
                  "<body>" &
                  "<p>" &
                  "<h1>" &
                     "<Font Color=Green>" &
                      "  Radicación sistema de gestión documental" &
                    "</Font>" &
                   "</h1>" &
                   "</p>" &
                  HTTEXT &
                   "<p>" &
                   "<h2>Servidor Smtp  </h2>" &
                   "</p>" &
                  "<p>" & "Mensaje enviado el " &
                        Now.ToShortDateString &
                       " a las " & Now.ToLongTimeString &
                 "</p>" &
                  "</body>" &
                "</html>"
                    ConTenido = mensaj
                End If
                Consulta_Datos_Cuerpo_Mensaje = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_Datos_Cuerpo_Mensaje = "Inconsistencia general  " & ex.HelpLink & " " & ex.Source & " " & ex.InnerException.Message
        End Try
    End Function
    Function Retorna_Asunto_Radicado_Notificacion(ByVal codigo_plantilla As Integer, ByVal nombre_plantilla As String, ByRef asunto As String, ByVal consecutivo_rad As String) As String
        Try
            Dim Result As String = ""
            Dim confirmacion_asunto As String = "NO"
            Dim confirmacion_fechalimite As String = "NO"
            Result = Verifica_asunto_radicado(codigo_plantilla, confirmacion_asunto, confirmacion_fechalimite)
            If confirmacion_asunto = "NO" And confirmacion_fechalimite = "NO" Then
                Retorna_Asunto_Radicado_Notificacion = "YES"
                Exit Function
            End If
            Dim campos_select As String = "Select "
            If confirmacion_asunto = "YES" Then
                campos_select = campos_select & " ASUNTO"
            End If
            If confirmacion_fechalimite = "YES" Then
                If campos_select = "Select " Then
                    campos_select = campos_select & " FECHALIMITERESPUESTA"
                Else
                    campos_select = campos_select & ",FECHALIMITERESPUESTA"
                End If
            End If

            Dim Parametro_Consulta As String = campos_select & " from " & nombre_plantilla & " where consecutivo_rad='" & consecutivo_rad & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Asunto_Radicado_Notificacion = " Función Retorna_Asunto_Radicado_Notificacion dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Asunto_Radicado_Notificacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If confirmacion_asunto = "YES" Then
                        If Datset.Tables(0).Rows(i).IsNull("ASUNTO") = True Then

                        Else
                            asunto = asunto & " Asunto " & Datset.Tables(0).Rows(i).Item("ASUNTO")
                        End If
                    End If
                    If confirmacion_fechalimite = "YES" Then
                        If Datset.Tables(0).Rows(i).IsNull("FECHALIMITERESPUESTA") = True Then

                        Else
                            asunto = asunto & " Fecha Limite respueta " & Datset.Tables(0).Rows(i).Item("FECHALIMITERESPUESTA")
                        End If
                    End If
                Next
                Retorna_Asunto_Radicado_Notificacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Asunto_Radicado_Notificacion = "Inconsistencia funcion Retorna_Asunto_Radicado_Notificacion " & ex.Message
        End Try
    End Function
    Function Verifica_asunto_radicado(ByVal codigo_plantilla As Integer, ByRef confirmacion_asunto As String, ByRef confirmacion_fechalimite As String) As String

        Try
            Dim Parametro_Consulta As String = "select Campo_Plantilla from  detalle_plantilla_radicado where System_Plantilla_Radicado_id_Plantilla=" & codigo_plantilla
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_asunto_radicado = " Función Verifica_asunto_radicado dice   " & Result
                Exit Function
            End If
            confirmacion_asunto = "NO"
            confirmacion_fechalimite = "NO"

            If Datset.Tables(0).Rows.Count = 0 Then

                Verifica_asunto_radicado = " Error consultando campos asunto   " & Parametro_Consulta
                Return Verifica_asunto_radicado
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Verifica_asunto_radicado = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).Item(0).ToString = "ASUNTO" Then
                        confirmacion_asunto = "YES"
                    End If
                    If Datset.Tables(0).Rows(i).Item(0).ToString = "FECHALIMITERESPUESTA" Then
                        confirmacion_fechalimite = "YES"
                    End If
                Next
                Verifica_asunto_radicado = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Verifica_asunto_radicado = "Inconsistencia función Verifica_asunto_radicado " & ex.Message
            Exit Function
        End Try

    End Function
    Function Enviando_Correo_Smtp_notificacion(ByVal id_tramite As Integer,
    ByVal Asunto As String, ByVal correo_destinatario As String, ByVal ID_CONFIG As Integer,
    Optional ByVal copia_destinatario As String = "", Optional ByVal contenido_destinatario As String = "",
    Optional ByVal Usuario_Smtp As String = "", Optional pasword_smtp As String = "") As String
        Try
            Dim ConTenido As String = ""
            Dim Nombre_Plantilla As String = ""
            Dim Refclas As New ClassRadicador
            Dim Dest_Smtp As String = ""
            '********************************
            'Solicita datos de configuracion
            '********************************
            Dim RefDatosConfig As New Config_Smtp
            Dim Result As String = ""
            Result = Obtener_Datos_ConfigSmtp(RefDatosConfig)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_notificacion = Result
                Exit Function
            End If
            'If Usuario_Smtp <> "" Then
            '    RefDatosConfig.USUARIO_SMTP = Usuario_Smtp
            '    RefDatosConfig.PASW_SMTP = pasword_smtp

            'End If
            Result = ""
            Consulta_Datos_cuerpo_mensaje_notificacion_respuesta(id_tramite, ConTenido, "")
            If contenido_destinatario <> "" Then
                ConTenido = contenido_destinatario & ConTenido
            End If
            '******************************
            'Verifica si tiene correo
            '******************************
            Dest_Smtp = correo_destinatario
            '********************************
            'Envia correo electronico
            '********************************
            'Declaro la variable para enviar el correo
            Dim correo As New System.Net.Mail.MailMessage()
            correo.From = New System.Net.Mail.MailAddress(RefDatosConfig.USUARIO_SMTP, "Sistema de gestión Docuarchi.net web")
            correo.Subject = Asunto & " Consecutivo " & id_tramite
            Result = Me.Agrega_destinatarios_formato_estructurado(Dest_Smtp, correo)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_notificacion = Result
                Exit Function
            End If
            Result = Me.Agrega_destinatarios_formato_estructurado_copia(copia_destinatario, correo)
            If Result <> "YES" Then
                Enviando_Correo_Smtp_notificacion = Result
                Exit Function
            End If

            correo.BodyEncoding = System.Text.Encoding.UTF8
            '*********************************
            'Configuracion de cuerpo de correo
            '*********************************
            If RefDatosConfig.ESTADO_BODY = 0 Then
                correo.IsBodyHtml = False
            Else
                correo.IsBodyHtml = True
            End If
            correo.Body = ConTenido
            'Configuracion del servidor
            Dim Servidor As New System.Net.Mail.SmtpClient
            Servidor.Host = RefDatosConfig.SERV_SMTP
            Servidor.Port = RefDatosConfig.PUERTO_SERV_SMTP
            Servidor.Timeout = RefDatosConfig.SMTP_TIEMPO * 100000
            If RefDatosConfig.ESTADO_SSL = 0 Then
                Servidor.EnableSsl = False
            Else
                Servidor.EnableSsl = True
            End If
            If RefDatosConfig.ESTADO_CREDENCIAL = 0 Then
                Servidor.UseDefaultCredentials = True
            Else
                Servidor.UseDefaultCredentials = False
                Servidor.Credentials = New System.Net.NetworkCredential(RefDatosConfig.USUARIO_SMTP, RefDatosConfig.PASW_SMTP)
            End If
            Servidor.Send(correo)
            Enviando_Correo_Smtp_notificacion = "YES"
        Catch ex As Exception
            Enviando_Correo_Smtp_notificacion = "Inconsistencia general función Enviando_Correo_Smtp_notificacion  " & ex.Message
        End Try
    End Function

    Function Consulta_Datos_cuerpo_mensaje_notificacion_respuesta(ByVal id_tramite_respuesta As Integer,
                                                                  ByRef ConTenido As String,
                                                                  ByVal correo_responde As String) As String
        Try
            Dim Result As String = ""
            Dim estru As stru_envio = Nothing
            Dim Class_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Result = Class_ra_respuesta_radicado.SolicitaEstructuraRespuestaRadicado(id_tramite_respuesta,
                                                                                     estru)
            If Result <> "YES" Then
                Consulta_Datos_cuerpo_mensaje_notificacion_respuesta = Result
                Exit Function
            End If
            Dim HTTEXT As String = ""
            HTTEXT =
            "<p> Detalle solicitud numero " & estru.ID_RESPUESTA_RADICADO & "</p>" &
            "<table>" &
               "<tr>" &
                  "<td> Consecutivo tramite de envío : </td>" &
                  "<td>" & estru.ID_RESPUESTA_RADICADO & "</td>" &
               "</tr>" &
               "<tr>" &
                    "<td> Tipo tramite para envío : </td> " &
                    "<td>" & estru.TRAMITE_DOCUMENTO & "</td>" &
               "</tr>" &
               "<tr>" &
                    "<td> Respuesta dirigida a : </td>" &
                    "<td>" & estru.DESTINATARIO & "</td>" &
               "</tr>" &
               "<tr>" &
                    "<td> Respuesta a Radicado numero  : </td> " &
                    "<td> " & estru.RADICADO & "</td>" &
               "</tr>" &
               "<tr>" &
                    "<td> Fecha Limite respuesta : </td> " &
                    "<td>" & estru.FECHA_VENCE & "</td>" &
               "</tr>" &
               "<tr>" &
                    "<td> Fecha respuesta del usuario  gestión : </td>" &
                     "<td>" & estru.FECHA_RESPUETA & "</td>" &
               "</tr>" &
               "<tr>" &
                   "<td> Otros datos : </td> " &
                    "<td>" & " Guía : " & estru.GUIA_ENVIO & "  Empresa " & estru.EMPRESA_ENVIO & "</td>" &
               "</tr>" &
            "</table>" &
            " <br> </br>" &
            "<Font Color=Green>" &
            "<p> Por favor responda al correo adjunto aqui:  " & correo_responde & "</p>" &
             "</Font>"
            ' Dest_Smtp = Dat_reader.Item(7).ToString
            Dim mensaj As String = "<html>" &
           "<head>" &
          "<title>" &
            "Gestor Documental " &
          "</title>" &
          "</head>" &
          "<body>" &
          "<p>" &
          "<h1>" &
             "<Font Color=Green>" &
              "  Radicación SIDE" &
            "</Font>" &
           "</h1>" &
           "</p>" &
          HTTEXT &
           "<p>" &
           "<h2>Servidor Smtp  </h2>" &
           "</p>" &
          "<p>" & "Mensaje enviado el " &
                Now.ToShortDateString &
               " a las " & Now.ToLongTimeString &
         "</p>" &
           "</body>" &
          "</html>"
            ConTenido = mensaj
            Consulta_Datos_cuerpo_mensaje_notificacion_respuesta = "YES"
        Catch ex As Exception
            Consulta_Datos_cuerpo_mensaje_notificacion_respuesta = "Inconsistencia general  " & ex.HelpLink & " " & ex.Source & " " & ex.InnerException.Message
        End Try
    End Function

    Function Agrega_destinatarios_formato_estructurado(ByVal corre_dest As String, ByRef correo As System.Net.Mail.MailMessage) As String
        Try
            If corre_dest = "" Then
                Agrega_destinatarios_formato_estructurado = "YES"
                Exit Function
            End If
            Dim splidest() As String = Nothing
            If InStr(corre_dest, ",") > 0 Then
                splidest = corre_dest.ToString.Split(",")
            End If
            Dim ref_trim As String = ""
            If splidest Is Nothing Then
                If InStr(Trim(corre_dest), "|") > 0 Then
                    Dim split_correo() As String = Trim(corre_dest).Split("|")
                    correo.To.Add(New System.Net.Mail.MailAddress(split_correo(0), split_correo(1)))
                Else
                    correo.To.Add(New System.Net.Mail.MailAddress(corre_dest, ""))
                End If
                Agrega_destinatarios_formato_estructurado = "YES"
                Exit Function
            Else
                For i As Integer = 0 To splidest.Length - 1
                    If Trim(splidest(i)) <> "" And Trim(splidest(i)) <> " " Then
                        If InStr(Trim(splidest(i)), "|") > 0 Then
                            Dim split_correo() As String = Trim(splidest(i)).Split("|")
                            correo.To.Add(New System.Net.Mail.MailAddress(split_correo(0), split_correo(1)))
                        Else
                            correo.To.Add(New System.Net.Mail.MailAddress(Trim(splidest(i)), ""))
                        End If

                    End If
                Next
                Agrega_destinatarios_formato_estructurado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Agrega_destinatarios_formato_estructurado = "Inconsistencia general función Agrega_destinatarios_formato_estructurado " & ex.Message
        End Try
    End Function
    Function Agrega_destinatarios_formato_estructurado_copia(ByVal corre_dest As String, ByRef correo As System.Net.Mail.MailMessage) As String
        Try
            If corre_dest = "" Then
                Agrega_destinatarios_formato_estructurado_copia = "YES"
                Exit Function
            End If
            Dim splidest() As String = Nothing
            If InStr(corre_dest, ",") > 0 Then
                splidest = corre_dest.ToString.Split(",")
            End If
            Dim ref_trim As String = ""
            If splidest Is Nothing Then
                If InStr(Trim(corre_dest), "|") > 0 Then
                    Dim split_correo() As String = Trim(corre_dest).Split("|")
                    correo.CC.Add(New System.Net.Mail.MailAddress(split_correo(0), split_correo(1)))
                Else
                    correo.CC.Add(New System.Net.Mail.MailAddress(corre_dest, ""))
                End If
                Agrega_destinatarios_formato_estructurado_copia = "YES"
                Exit Function
            Else
                For i As Integer = 0 To splidest.Length - 1
                    If Trim(splidest(i)) <> "" And Trim(splidest(i)) <> " " Then
                        If InStr(Trim(splidest(i)), "|") > 0 Then
                            Dim split_correo() As String = Trim(splidest(i)).Split("|")
                            correo.CC.Add(New System.Net.Mail.MailAddress(split_correo(0), split_correo(1)))
                        Else
                            correo.CC.Add(New System.Net.Mail.MailAddress(Trim(splidest(i)), ""))
                        End If

                    End If
                Next
                Agrega_destinatarios_formato_estructurado_copia = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Agrega_destinatarios_formato_estructurado_copia = "Inconsistencia general función Agrega_destinatarios_formato_estructurado_copia " & ex.Message
        End Try
    End Function
End Class
