Public Class Class_CambiaEstadoSii
    Function SolicitaDatosCambioEstadoSII(ByVal IdUsuarioWorkflow As Integer,
                                          ByVal IdActividadWorkflow As Integer,
                                          ByVal IdTareaWorkflow As Long,
                                          ByRef EstadoSII As String,
                                          ByRef RadicadoSII As String,
                                          ByRef CodigoCortoSII As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Valida los parametros obligatorios para cambio de estado y 
        '          retorna los parmetros
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'IdUsuarioWorkflow         : Representa la identificación del usuario workflow
        'IdActividadWorkflow       : Representa la identificación de la actividad workflow
        'IdTareaWorkflow           : Representa la identificación de la tarea workflow
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'EstadoSII            : Retorna el estado del SII relacionado a la actividad
        '                     : workflow
        'RadicadoSII          : Retorna el radicado relacionado a la tarea
        'CodigoCortoSII       : Retorna  el codigo corto del usuario SII
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Class_ws_estado_ruta_documentos_sii As New Class_ws_estado_ruta_documentos_sii
            Dim result As String = Class_ws_estado_ruta_documentos_sii.Solicita_estado_sii_actividad_workflow(IdActividadWorkflow,
                                                                                                              EstadoSII)
            If result <> "YES" Then
                SolicitaDatosCambioEstadoSII = result
                Exit Function
            End If
            If EstadoSII = "-1" Then
                SolicitaDatosCambioEstadoSII = "La actividad (" & IdActividadWorkflow & ") no está configurada para comunicarse con el sistema SII. Por favor, contacte al administrador del sistema para validar la configuración de integración."
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_workflow(IdTareaWorkflow,
                                                                            RadicadoSII)
            If result <> "YES" Then
                SolicitaDatosCambioEstadoSII = result
                Exit Function
            End If
            Dim Class_relacion_sirep_workflow As New Class_relacion_sirep_workflow
            result = Class_relacion_sirep_workflow.Solicita_codigo_usuario_sii_operador(IdUsuarioWorkflow,
                                                                                        CodigoCortoSII)
            If result <> "YES" Then
                SolicitaDatosCambioEstadoSII = result
                Exit Function
            End If
            If CodigoCortoSII = "" Then
                SolicitaDatosCambioEstadoSII = "No se ha encontrado la relación entre el usuario operador del sistema SII y el código de asignación correspondiente (" & CodigoCortoSII & ") . Por favor, contacte al administrador para configurar correctamente esta asignación."
                Exit Function
            End If
            SolicitaDatosCambioEstadoSII = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaDatosCambioEstadoSII = "Inconsistencia general funcion SolicitaDatosCambioEstadoSII " & ex.Message
        End Try
    End Function
    Function Cambia_estado_Radicado(ByVal Estado As String,
                                    ByVal Numero_radicado As String,
                                    ByVal usuario_operador As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Cambia el estado de un radicado en el sistema SII
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'estado                : Representa el estado a cambiar en el SII
        'numero_radicado       : Representa el numero del radicao del SII
        'usuario_operador      : Representa el usuario del SII que se le asigna la
        '                        tarea
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim result As String = ""
            Dim result_ As String = ""
            Dim codigo_empresa As String = ""
            Dim usuariows As String = ""
            Dim clavews As String = ""
            Dim url As String = ""
            Dim nombre_funcion As String = "recibirCambioEstadoRadicado"
            Dim stru_token As SolicitaToken = Nothing
            Dim Class_ws_usuarioworkflowsii As New Class_ws_usuarioworkflowsii
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim fecha As String = ""
            Dim hora As String = ""
            result = ClassGestionFechas.Solicita_fecha_hora_sii(fecha,
                                                                hora)
            If result <> "YES" Then
                Cambia_estado_Radicado = result
                Exit Function
            End If
            result = Class_ws_usuarioworkflowsii.solicita_usuario_validacion_sii(codigo_empresa,
                                                                                 usuariows,
                                                                                 clavews)
            If result <> "YES" Then
                Cambia_estado_Radicado = result
                Exit Function
            End If
            result = Class_ws_usuarioworkflowsii.Solicita_url_nombrefuncion_restfull(url, nombre_funcion)
            If result <> "YES" Then
                Cambia_estado_Radicado = result
                Exit Function
            End If
            Dim Class_ClassResfull As New Class_ClassResfull
            result = Class_ClassResfull.Solicitar_token_general(codigo_empresa,
                                                                usuariows,
                                                                clavews,
                                                                url & "solicitarToken",
                                                                stru_token)
            If result <> "YES" Then
                Cambia_estado_Radicado = result
                Exit Function
            End If
            '----------------------------------------
            'Verifica código de error sii
            '----------------------------------------
            If stru_token.mensajeerror <> "" Then
                Cambia_estado_Radicado = " Error servicio restfull descripción " & stru_token.mensajeerror &
                    " código " & stru_token.codigoerror
                Exit Function
            End If
            Dim respuestaServidor As String = ""
            '----------------------------------------
            'Valida asignación de codigo sii
            '-----------------------------------------
            Dim Parametros_estado As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros_estado.Add("codigoempresa", "40")
            Parametros_estado.Add("usuariows", "gesdoc")
            Parametros_estado.Add("token", stru_token.token)
            Parametros_estado.Add("radicado", numero_radicado)
            result = Class_ClassResfull.GetResponse(url & "consultarEstadoRadicado", Parametros_estado, "POST", respuestaServidor)
            If result <> "YES" Then
                Cambia_estado_Radicado = result
                Exit Function
            End If
            Dim stru_ConsultarEstadoRadicado As recibirConsultarEstadoRadicado = Nothing
            Dim Class_Desserializacion As New Class_Desserializacion
            result = Class_Desserializacion.DesSerializacion_recibirConsultarEstadoRadicado(respuestaServidor,
                                                                                            stru_ConsultarEstadoRadicado)
            If result <> "YES" Then
                Cambia_estado_Radicado = result
                Exit Function
            End If
            If stru_ConsultarEstadoRadicado.mensajeerror <> "" Then
                Cambia_estado_Radicado = " La función RecibirCambioEstadoRadicado del SII genero el siguiente error: " _
                    & stru_ConsultarEstadoRadicado.mensajeerror
                Exit Function
            End If
            '-----------------------------------------
            'Valida si asigna el tramite en el sii
            '------------------------------------------
            If stru_ConsultarEstadoRadicado.codigoestado = Estado Then
                Cambia_estado_Radicado = "YES"
                Exit Function
            End If
            Dim Parametros2 As Dictionary(Of String, String) = New Dictionary(Of String, String)()
            Parametros2.Add("codigoempresa", "40")
            Parametros2.Add("usuariows", "gesdoc")
            Parametros2.Add("token", stru_token.token)
            Parametros2.Add("radicado", numero_radicado)
            Parametros2.Add("estado", Estado)
            Parametros2.Add("fecha", fecha)
            Parametros2.Add("hora", hora)
            Parametros2.Add("usuario", usuario_operador)
            Parametros2.Add("sede", "40")
            result = Class_ClassResfull.GetResponse(url & "recibirCambioEstadoRadicado", Parametros2, "POST", respuestaServidor)
            If result <> "YES" Then
                Cambia_estado_Radicado = result
                Exit Function
            End If
            Dim stru_recibirEstadoRadicado As recibirCambioEstadoRadicado = Nothing
            result = Class_Desserializacion.DesSerializacion_recibirCambioEstadoRadicado(respuestaServidor, stru_recibirEstadoRadicado)
            If result <> "YES" Then
                Cambia_estado_Radicado = result
                Exit Function
            End If
            If stru_recibirEstadoRadicado.mensajeerror <> "" Then
                Cambia_estado_Radicado = " La función RecibirCambioEstadoRadicado del SII genero el siguiente error: " _
                    & stru_recibirEstadoRadicado.mensajeerror & " Código error sii " & stru_recibirEstadoRadicado.codigoerror &
                    " Operador : " & usuario_operador & " Estado relacionado : " & Estado & " Radicado : " & numero_radicado &
                    " hora : " & hora & " fecha : " & fecha
                Exit Function
            End If
            Cambia_estado_Radicado = "YES"
            Exit Function
        Catch ex As Exception
            Cambia_estado_Radicado = "Inconcistencia general función Cambia_estado_Radicado " & ex.Message
        End Try
    End Function
End Class
