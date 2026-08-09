Imports GestionDocumental_Docuarchi.net.Class_config_general_service

Public Structure consultarRecibo
    Dim codigoerror As String
    Dim mensajeerror As String
    Dim recibo As String
    Dim fecha As String
    Dim hora As String
    Dim operacion As String
    Dim factura As String
    Dim radicado As String
    Dim rutasii As String
    Dim usuario As String
    Dim tipogasto As String
    Dim idclase As String
    Dim identificacion As String
    Dim nombre As String
    Dim direccion As String
    Dim municipio As String
    Dim telefono As String
    Dim email As String
    Dim tipotramite As String
    Dim valorneto As String
    Dim tipodoc As String
    Dim numerodoc As String
    Dim origendoc As String
    Dim fechadoc As String
    Dim municipiodoc As String
    Dim numerointernorue As String
    Dim numerounicorue As String
    Dim servicios() As servicios
    Dim imagenes() As imagenes
End Structure
Public Class Class_ConSultaRecibo_Service
    Property Error_gestion As String
    Property Class_config_general_service As IList(Of Class_config_general_service)
    Property Class_parram_consultarRadicado As Class_parram_consultarRadicado
    Property Class_parram_consultarRecibo As Class_parram_consultarRecibo
    Property Class_service_ilist_drowlist As IList(Of Class_service_ilist_drowlist)
    Property Class_service_ilist_drowlist_actividad_flujo As IList(Of Class_service_ilist_drowlist)
    Property Class_service_ilist_drowlist_flujos As IList(Of Class_service_ilist_drowlist)
    Property Class_service_ilist_drowlist_rutas As IList(Of Class_service_ilist_drowlist)
    Property Class_service_ilist_drowlist_actividad As IList(Of Class_service_ilist_drowlist)
    Property class_row_rue_sii As IList(Of class_row_rue_sii)
    Property class_row_virtual_sii As IList(Of class_row_virtual_sii)
    Property id_flujo As Object
    Property id_usuario_workflow As Object
    Property id_ruta As Object
    Property id_grupo_workflow As Object
    Property id_actividad_workflow As Object
    Property id_actividad_flujo As Object
    Property id_usuario_workflow_transacion As Object
    Property option_registra_log As Object
End Class

Public Class Class_ConSultaRecibo
    Function Solicita_clase_Recibo_Radicado_SII(ByVal numero_recibo As String,
                                                ByVal codigo_empresa As String,
                                                ByVal usuario_sii As String,
                                                ByVal clave_usuario_sii As String,
                                                ByVal UrlBase As String,
                                                ByRef class_parram_consultarRecibo As Class_parram_consultarRecibo,
                                                ByRef class_parram_consultarRadicado As Class_parram_consultarRadicado) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos desde el sistema SII y lo asigna a las clases recibo caja
        '          y radicado 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'numero_recibo       : Representa el numero de recibo SII
        'codigo_empresa      : Representa el código de camaras de identificación del SII
        'usuario_sii         : Representa el usuario RES API SII
        'clave_usuario_sii   : Representa la clave del usuario RES API SII
        'UrlBase             : Representa la url base del servicio web RES API SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_parram_consultarRecibo   : Retorna la estructura de la clase del recibo de caja SII
        'class_parram_consultarRadicado : Retorna la estructura de la clase del radicado SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-29
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim stru_token As SolicitaToken = Nothing
            Dim Class_ClassResfull As New Class_ClassResfull
            Dim Resul As String = ""
            Resul = Class_ClassResfull.Solicitar_token_general(codigo_empresa,
                                                               usuario_sii,
                                                               clave_usuario_sii,
                                                               UrlBase & "solicitarToken",
                                                               stru_token)
            If Resul <> "YES" Then
                Solicita_clase_Recibo_Radicado_SII = Resul
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                Solicita_clase_Recibo_Radicado_SII = stru_token.mensajeerror
                Exit Function
            End If
            Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros.Add("codigoempresa", codigo_empresa)
            Parametros.Add("usuariows", usuario_sii)
            Parametros.Add("token", stru_token.token)
            Parametros.Add("recibo", numero_recibo)
            Dim Result As String = ""
            Dim respuestaServidor As String = ""
            Result = Class_ClassResfull.GetResponse(UrlBase & "consultarRecibo",
                                                    Parametros,
                                                    "POST",
                                                    respuestaServidor)
            If Result <> "YES" Then
                Solicita_clase_Recibo_Radicado_SII = Result
                Exit Function
            End If
            Dim Class_Desserializacion As New Class_Desserializacion
            class_parram_consultarRecibo = New Class_parram_consultarRecibo
            class_parram_consultarRecibo = Class_Desserializacion.Deserialize(Of Class_parram_consultarRecibo)(respuestaServidor)
            If class_parram_consultarRecibo.mensajeerror <> "" Then
                Solicita_clase_Recibo_Radicado_SII = "La función ConsultaRecibo del SII genero el siguiente error :" & class_parram_consultarRecibo.mensajeerror & class_parram_consultarRecibo.codigoerror
                Exit Function
            End If
            If class_parram_consultarRecibo.tipotramite = "" Then
                Solicita_clase_Recibo_Radicado_SII = "No se ha generado el consecutivo (" & numero_recibo & ") de recibo en el sii "
                Exit Function
            End If
            Dim Parametros_ As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros_.Add("codigoempresa", codigo_empresa)
            Parametros_.Add("usuariows", usuario_sii)
            Parametros_.Add("token", stru_token.token)
            Parametros_.Add("radicado", class_parram_consultarRecibo.radicado)
            If class_parram_consultarRecibo.radicado <> "" Then
                Result = Class_ClassResfull.GetResponse(UrlBase & "consultarRadicado",
                                                        Parametros_,
                                                        "POST",
                                                        respuestaServidor)
                If Result <> "YES" Then
                    Solicita_clase_Recibo_Radicado_SII = Result
                    Exit Function
                End If
                class_parram_consultarRadicado = New Class_parram_consultarRadicado
                class_parram_consultarRadicado = Class_Desserializacion.Deserialize(Of Class_parram_consultarRadicado)(respuestaServidor)
                If class_parram_consultarRadicado.mensajeerror <> "" Then
                    Solicita_clase_Recibo_Radicado_SII = "La función ConSultarRadicado del SII genero el siguiente error :" & class_parram_consultarRadicado.mensajeerror & class_parram_consultarRadicado.codigoerror
                    Exit Function
                End If
                If class_parram_consultarRecibo.tipotramite = "inscripciondocumentos" Then
                    class_parram_consultarRecibo.tipotramite = class_parram_consultarRadicado.subtipotramite
                End If
                Dim Class_Integracion_SII As New Class_Integracion_SII
                Dim salida_formato_nombre As String = ""
                Class_Integracion_SII.Formato_campo_nombre_sii(class_parram_consultarRadicado.nombre,
                                                               salida_formato_nombre)
                class_parram_consultarRecibo.nombre = salida_formato_nombre
                class_parram_consultarRadicado.nombre = salida_formato_nombre
                If class_parram_consultarRadicado.matricula = "" Then
                    class_parram_consultarRadicado.matricula = "0"
                End If
                class_parram_consultarRecibo.identificacion = class_parram_consultarRadicado.identificacion
                Solicita_clase_Recibo_Radicado_SII = "YES"
                Exit Function
            Else
                Solicita_clase_Recibo_Radicado_SII = "El recibo (" & numero_recibo & ") del servicio  (" & class_parram_consultarRecibo.tipotramite & ")  no tiene relacionado un radicado o código de barras, imposile continuar con el registro de la tarea"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_clase_Recibo_Radicado_SII = "Inconsistencia general función CosnultaRecibo " & ex.Message
        End Try
    End Function
    Function ConSultaRecibo(ByRef stru_consulta_recibo As consultarRecibo,
                            ByRef stru_consulta_radicado As ConsultarRadicado_sii,
                            ByVal numero_recibo As String,
                            ByVal codigo_empresa As String,
                            ByVal usuario_sii As String,
                            ByVal clave_usuario_sii As String,
                            ByVal UrlBase As String) As String
        Try
            Dim stru_token As SolicitaToken = Nothing
            Dim Class_ClassResfull As New Class_ClassResfull
            Dim Resul As String = ""
            Resul = Class_ClassResfull.Solicitar_token_general(
                                                              codigo_empresa,
                                                              usuario_sii,
                                                              clave_usuario_sii,
                                                              UrlBase & "solicitarToken",
                                                              stru_token)
            If Resul <> "YES" Then
                ConSultaRecibo = Resul
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                ConSultaRecibo = stru_token.mensajeerror
                Exit Function
            End If
            Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros.Add("codigoempresa", codigo_empresa)
            Parametros.Add("usuariows", usuario_sii)
            Parametros.Add("token", stru_token.token)
            Parametros.Add("recibo", numero_recibo)
            Dim Result As String = ""
            Dim respuestaServidor As String = ""
            Result = Class_ClassResfull.GetResponse(UrlBase & "consultarRecibo",
                                                    Parametros,
                                                    "POST",
                                                    respuestaServidor)
            If Result <> "YES" Then
                ConSultaRecibo = Result
                Exit Function
            End If
            Dim Class_Desserializacion As New Class_Desserializacion
            Result = Class_Desserializacion.DesSerializacion_consultarRecibo(respuestaServidor,
                                                                             stru_consulta_recibo)
            If Result <> "YES" Then
                ConSultaRecibo = Result
                Exit Function
            End If
            If stru_consulta_recibo.mensajeerror <> "" Then
                ConSultaRecibo = "La función ConsultaRecibo del SII genero el siguiente error:" & stru_consulta_recibo.mensajeerror & stru_consulta_recibo.codigoerror
            End If
            If stru_consulta_recibo.tipotramite = "" Then
                ConSultaRecibo = "No se ha generado el consecutivo " & numero_recibo & " de recibo en el sii "
                Exit Function
            End If
            Dim Parametros_ As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros_.Add("codigoempresa", codigo_empresa)
            Parametros_.Add("usuariows", usuario_sii)
            Parametros_.Add("token", stru_token.token)
            Parametros_.Add("radicado", stru_consulta_recibo.radicado)
            If stru_consulta_recibo.radicado <> "" Then
                Result = Class_ClassResfull.GetResponse(UrlBase & "consultarRadicado",
                                                        Parametros_,
                                                        "POST",
                                                        respuestaServidor)
                If Result <> "YES" Then
                    ConSultaRecibo = Result
                    Exit Function
                End If
                Result = Class_Desserializacion.DesSerializacion_consultarRadicado(respuestaServidor,
                                                                                   stru_consulta_radicado)
                If Result <> "YES" Then
                    ConSultaRecibo = Result
                    Exit Function
                End If
                If stru_consulta_radicado.mensajeerror <> "" Then
                    ConSultaRecibo = "La función ConSultarRadicado del SII genero el siguiente error:" & stru_consulta_radicado.mensajeerror & stru_consulta_radicado.codigoerror
                    Exit Function
                End If
                If stru_consulta_recibo.tipotramite = "inscripciondocumentos" Then
                    stru_consulta_recibo.tipotramite = stru_consulta_radicado.subtipotramite
                End If
                stru_consulta_recibo.nombre = stru_consulta_radicado.nombre
                stru_consulta_recibo.identificacion = stru_consulta_radicado.identificacion
            End If
            ConSultaRecibo = "YES"
        Catch ex As Exception
            ConSultaRecibo = "Inconsistencia general función CosnultaRecibo " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII(ByVal recibo As String,
                                                                         ByRef class_parram_consultarRecibo As Class_parram_consultarRecibo,
                                                                         ByRef class_parram_consultarRadicado As Class_parram_consultarRadicado) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos desde el sistema SII y lo asigna a las clases recibo caja
        '          y radicado 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'numero_recibo       : Representa el numero de recibo SII
        '
        '
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_parram_consultarRecibo   : Retorna la estructura de la clase del recibo de caja SII
        'class_parram_consultarRadicado : Retorna la estructura de la clase del radicado SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-02
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim stru_consulta_recibo As consultarRecibo = Nothing
            Dim stru_consulta_radicado As ConsultarRadicado_sii = Nothing
            Result = Solicita_datos_estructura_recibo_radicado_SII(recibo,
                                                                   class_parram_consultarRecibo,
                                                                   class_parram_consultarRadicado)
            Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII = Result
            Exit Function
        Catch ex As Exception
            Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII = "Inconsistencia general funcion Solicita_datos_estructura_consulta_recibo_ruta_interfaz_SII" & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_recibo_radicado_SII(ByVal recibo As String,
                                                           ByRef class_parram_consultarRecibo As Class_parram_consultarRecibo,
                                                           ByRef class_parram_consultarRadicado As Class_parram_consultarRadicado) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos de estrucutura de una recibo  de caja del sistema SII Camara 
        '          de comercio de villavicencio y la estructura del radicado o codigo de barras
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'numero_recibo           : Representa el consecutivo del recibo de caja
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_consulta_recibo  : Retorna la estructura del recibo SII
        'stru_consulta_radicado: Retorna la estructura del radicado SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-29
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim codigo_empresa As String = ""
            Dim usuario_sii As String = ""
            Dim clave_usuario_sii As String = ""
            Dim UrlBase As String = ""
            Dim Class_ws_usuarioworkflowsii As New Class_ws_usuarioworkflowsii
            Result = Class_ws_usuarioworkflowsii.solicita_usuario_validacion_sii(codigo_empresa,
                                                                                 usuario_sii,
                                                                                 clave_usuario_sii)
            If Result <> "YES" Then
                Solicita_datos_estructura_recibo_radicado_SII = Result
                Exit Function
            End If
            Result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(UrlBase,
                                                                                     "solicitarToken")
            If Result <> "YES" Then
                Solicita_datos_estructura_recibo_radicado_SII = Result
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
                Solicita_datos_estructura_recibo_radicado_SII = Result
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                Solicita_datos_estructura_recibo_radicado_SII = stru_token.mensajeerror
                Exit Function
            End If
            Dim Class_ConSultaRecibo As New Class_ConSultaRecibo
            Result = Me.Solicita_clase_Recibo_Radicado_SII(recibo,
                                                           codigo_empresa,
                                                           usuario_sii,
                                                           clave_usuario_sii,
                                                           UrlBase,
                                                           class_parram_consultarRecibo,
                                                           class_parram_consultarRadicado)

            If Result <> "YES" Then
                Solicita_datos_estructura_recibo_radicado_SII = Result
                Exit Function
            End If
            If class_parram_consultarRecibo.codigoerror <> "0000" Then
                Solicita_datos_estructura_recibo_radicado_SII = "Imposible encontrar el recibo codigo error del sii " & class_parram_consultarRecibo.codigoerror
                Exit Function
            End If
            If class_parram_consultarRecibo.recibo = "" Then
                Solicita_datos_estructura_recibo_radicado_SII = "Imposible encontrar el recibo " & recibo
                Exit Function
            End If
            Solicita_datos_estructura_recibo_radicado_SII = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_datos_estructura_recibo_radicado_SII = "Inconsistencia general función Solicita_datos_estructura_recibo_radicado_SII " & ex.Message
        End Try
    End Function
End Class
