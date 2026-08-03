Public Structure ConsultarRadicado_sii
    Dim codigoerror As String
    Dim mensajeerror As String
    Dim radicado As String
    Dim tipotramite As String
    Dim operacion As String
    Dim recibo As String
    Dim fecharadicacion As String
    Dim matricula As String
    Dim proponente As String
    Dim idclase As String
    Dim identificacion As String
    Dim nombre As String
    Dim estadofinal As String
    Dim usuariofinal As String
    Dim fechaestadofinal As String
    Dim horaestadofinal As String
    Dim sucursalfinal As String
    Dim actoreparto As String
    Dim tipodoc As String
    Dim tipodocsirep As String
    Dim tipodocdigitalizacion As String
    Dim tipoingreso As String
    Dim numerodoc As String
    Dim origendoc As String
    Dim fechadoc As String
    Dim municipiodoc As String
    Dim numerointernorue As String
    Dim numerounicorue As String
    Dim tipogasto As String
    Dim subtipotramite As String
    Dim cumplorequisitosbenley1780 As String
    Dim mantengorequisitosbenley1780 As String
    Dim renunciobeneficiosley1780 As String
    Dim multadoponal As String
    Dim controlactividadaltoimpacto As String
    Dim servicios() As servicios
    Dim imagenes() As imagenes
    Dim estados() As estados
End Structure
Public Structure servicios
    Dim servicio As String
    Dim nservicio As String
    Dim matricula As String
    Dim proponente As String
    Dim identificacion1 As String
    Dim nombre1 As String
    Dim cantidad As String
    Dim valorbase As String
    Dim valorservicio As String
    Dim ano As String
End Structure
Public Structure imagenes
    Dim url As String
    Dim idanexo As String
    Dim tipo As String
    Dim tipoanexo As String
    Dim tiposirep As String
    Dim tipodigitalizacion As String
    Dim identificador As String
    Dim formato As String
    Dim identificacion As String
    Dim nombre As String
    Dim matricula As String
    Dim proponente As String
    Dim fechadocumento As String
    Dim origen As String
    Dim observaciones As String
End Structure
Public Structure estados
    Dim fecha As String
    Dim hora As String
    Dim estado As String
    Dim usuariofinal As String
End Structure
Public Class Class_ConsultarRadicado_sii_servcio
    Property Error_gestion As String
    Property recibo_sii As String
End Class

Public Class Class_ConsultarRadicado_sii
    Function Solicita_datos_estructura_radicado_recibo_SII(ByVal radicado As String,
                                                           ByRef class_parram_consultarRecibo As Class_parram_consultarRecibo,
                                                           ByRef class_parram_consultarRadicado As Class_parram_consultarRadicado) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos de estrucutura de un radicado  del sistema SII Camara 
        '          de comercio de villavicencio y la estructura del recibo
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'radicado           : Representa numero de radicado
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
        'Fecha                 : 2025-01-02
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
                Solicita_datos_estructura_radicado_recibo_SII = Result
                Exit Function
            End If
            Result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(UrlBase,
                                                                                     "solicitarToken")
            If Result <> "YES" Then
                Solicita_datos_estructura_radicado_recibo_SII = Result
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
                Solicita_datos_estructura_radicado_recibo_SII = Result
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                Solicita_datos_estructura_radicado_recibo_SII = stru_token.mensajeerror
                Exit Function
            End If
            Dim Class_ConSultaRecibo As New Class_ConSultaRecibo
            Result = Me.Solicita_clase_Radicado_Recibo_SII(radicado,
                                                           codigo_empresa,
                                                           usuario_sii,
                                                           clave_usuario_sii,
                                                           UrlBase,
                                                           class_parram_consultarRecibo,
                                                           class_parram_consultarRadicado)

            If Result <> "YES" Then
                Solicita_datos_estructura_radicado_recibo_SII = Result
                Exit Function
            End If
            If class_parram_consultarRecibo.codigoerror <> "0000" Then
                Solicita_datos_estructura_radicado_recibo_SII = "Imposible encontrar el recibo codigo error del sii " & class_parram_consultarRecibo.codigoerror
                Exit Function
            End If
            If class_parram_consultarRecibo.recibo = "" Then
                Solicita_datos_estructura_radicado_recibo_SII = "Imposible encontrar el radicado " & radicado
                Exit Function
            End If
            Solicita_datos_estructura_radicado_recibo_SII = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_datos_estructura_radicado_recibo_SII = "Inconsistencia general función Solicita_datos_estructura_radicado_recibo_SII " & ex.Message
        End Try
    End Function
    Function Solicita_clase_Radicado_Recibo_SII(ByVal numero_radicado As String,
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
        'numero_radicado     : Representa el numero de radicado SII
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
        'Fecha                 : 2025-01-03
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
                Solicita_clase_Radicado_Recibo_SII = Resul
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                Solicita_clase_Radicado_Recibo_SII = stru_token.mensajeerror
                Exit Function
            End If
            Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros.Add("codigoempresa", codigo_empresa)
            Parametros.Add("usuariows", usuario_sii)
            Parametros.Add("token", stru_token.token)
            Parametros.Add("radicado", numero_radicado)
            Dim Result As String = ""
            Dim respuestaServidor As String = ""
            Result = Class_ClassResfull.GetResponse(UrlBase & "consultarRadicado",
                                                    Parametros,
                                                    "POST",
                                                    respuestaServidor)
            If Result <> "YES" Then
                Solicita_clase_Radicado_Recibo_SII = Result
                Exit Function
            End If
            Dim Class_Desserializacion As New Class_Desserializacion
            class_parram_consultarRadicado = New Class_parram_consultarRadicado
            class_parram_consultarRadicado = Class_Desserializacion.Deserialize(Of Class_parram_consultarRadicado)(respuestaServidor)
            If class_parram_consultarRadicado.mensajeerror <> "" Then
                Solicita_clase_Radicado_Recibo_SII = "La función consultarRadicado del SII genero el siguiente error :" & class_parram_consultarRadicado.mensajeerror & class_parram_consultarRadicado.codigoerror
                Exit Function
            End If
            Dim Parametros_ As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros_.Add("codigoempresa", codigo_empresa)
            Parametros_.Add("usuariows", usuario_sii)
            Parametros_.Add("token", stru_token.token)
            Parametros_.Add("recibo", class_parram_consultarRadicado.recibo)
            If class_parram_consultarRadicado.recibo <> "" Then
                Result = Class_ClassResfull.GetResponse(UrlBase & "consultarRecibo",
                                                        Parametros_,
                                                        "POST",
                                                        respuestaServidor)
                If Result <> "YES" Then
                    Solicita_clase_Radicado_Recibo_SII = Result
                    Exit Function
                End If
                class_parram_consultarRecibo = New Class_parram_consultarRecibo
                class_parram_consultarRecibo = Class_Desserializacion.Deserialize(Of Class_parram_consultarRecibo)(respuestaServidor)
                If class_parram_consultarRecibo.mensajeerror <> "" Then
                    Solicita_clase_Radicado_Recibo_SII = "La función consultarRadicado del SII genero el siguiente error :" & class_parram_consultarRecibo.mensajeerror & class_parram_consultarRecibo.codigoerror
                    Exit Function
                End If
                If class_parram_consultarRecibo.tipotramite = "" Then
                    Solicita_clase_Radicado_Recibo_SII = "No se ha generado el consecutivo (" & class_parram_consultarRadicado.recibo & ") de recibo en el sii "
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
                class_parram_consultarRecibo.identificacion = class_parram_consultarRadicado.identificacion
                Solicita_clase_Radicado_Recibo_SII = "YES"
                Exit Function
            Else
                Solicita_clase_Radicado_Recibo_SII = "El radicado (" & numero_radicado & ") del servicio  (" & class_parram_consultarRadicado.tipotramite & ")  no tiene relacionado un recibo, imposile continuar con el registro de la tarea"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_clase_Radicado_Recibo_SII = "Inconsistencia general función Solicita_clase_Radicado_Recibo_SII " & ex.Message
        End Try
    End Function
    Function ConSultarRadicado(ByVal radicado As String,
                               ByRef ConsultarRadicado_sii As ConsultarRadicado_sii) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de un radicado desde el sistema SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'radicado           : Representa el consecutivo de un radicado desde el sstema SII
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_consulta_radicado  : Retorna la esctructura de los datos de un radicado SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

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
                ConSultarRadicado = Result
                Exit Function
            End If
            Result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(UrlBase,
                                                                                     "solicitarToken")
            If Result <> "YES" Then
                ConSultarRadicado = Result
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
                ConSultarRadicado = Result
                Exit Function
            End If
            If stru_token.mensajeerror <> "" Then
                ConSultarRadicado = stru_token.mensajeerror
                Exit Function
            End If
            Dim Parametros As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros.Add("codigoempresa", codigo_empresa)
            Parametros.Add("usuariows", usuario_sii)
            Parametros.Add("token", stru_token.token)
            Parametros.Add("radicado", radicado)
            Dim Class_Desserializacion As New Class_Desserializacion
            Dim respuestaServidor As String = ""
            Class_ClassResfull = New Class_ClassResfull
            Result = Class_ClassResfull.GetResponse(UrlBase & "consultarRadicado",
                                                    Parametros,
                                                    "POST",
                                                    respuestaServidor)
            If Result <> "YES" Then
                ConSultarRadicado = "Error de intergacion con el sistema SII (" & Result & ")"
                Exit Function
            End If
            Result = Class_Desserializacion.DesSerializacion_consultarRadicado(respuestaServidor,
                                                                               ConsultarRadicado_sii)
            If Result <> "YES" Then
                ConSultarRadicado = Result
                Exit Function
            End If
            If ConsultarRadicado_sii.mensajeerror <> "" Then
                ConSultarRadicado = "La funcion ConSultarRadicado del SII genero el siguiente error : " & ConsultarRadicado_sii.mensajeerror & ConsultarRadicado_sii.codigoerror
            Else
                ConSultarRadicado = "YES"
            End If
        Catch ex As Exception
            ConSultarRadicado = "Inconsistencia general función ConSultarRadicado " & ex.Message
        End Try
    End Function
End Class
