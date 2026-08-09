Imports GemBox.Document
Imports System.IO
Imports GemBox.Document.Tables
Imports System.Xml

Public Class ClassGaGembox
    Function Solicita_Documento_plantilla_radicado(ByVal Pie_usuario() As String, _
                                                   ByVal Radicado As String, _
                                                   ByVal id_respuesta As String, _
                                                   ByVal Usuario As String, _
                                                   ByVal Ruta_plantilla As String, _
                                                   ByRef Docx As Object, _
                                                   ByVal Ruta_Firma As String, _
                                                   ByVal radicado_respuesta As String, _
                                                   ByVal id_usuario_gestion As Integer, _
                                                   ByVal id_usuario_firma As Integer) As String
        '***************************************************************
        'Funcion : crea el documento plantilla para el software 
        'de servicio web
        'Fecha 2016-06-15
        'Ing .Miguel Angel Urueta Miranda
        '****************************************************************
        Dim Ruta_Docx_Plantilla As String = ""
        Dim Result As String = ""
        Dim streamd As Stream = Nothing
        Dim matri_user_permitido() As String
        Erase matri_user_permitido
        Dim ruta_archivo As String = ""
        'Result = lee_archivo_auxiliar_villavivenda(matri_user_permitido)
        'If Result <> "YES" Then
        '    Crea_Documento_plantilla_servicio_web_radicado = "Funcion Crea_Documento_plantilla_servicio_web_radicado : " & Result
        '    Exit Function
        'End If
        'Dim existencia_plantilla As String = ""
        'If Not matri_user_permitido Is Nothing Then
        '    For z As Integer = 0 To matri_user_permitido.Length - 1
        '        If Not Pie_usuario Is Nothing Then
        '            If UCase(Pie_usuario(0)) = UCase(matri_user_permitido(z)) Then
        '                existencia_plantilla = "YES"
        '                Exit For
        '            End If
        '        End If
        '    Next
        'End If
        'If existencia_plantilla = "YES" Then
        '    Result = Me.Crea_Documento_auxiliar_villavivienda(Radicado, ruta_archivo, Pie_usuario(0))
        '    If Result <> "YES" Then
        '        Crea_Documento_plantilla_servicio_web_radicado = Result
        '        Exit Function
        '    Else
        '        If File.Exists(ruta_archivo) = True Then
        '            Docx = ReadFile(ruta_archivo)
        '        End If
        '        Crea_Documento_plantilla_servicio_web_radicado = "YES"
        '        Exit Function
        '    End If
        '    Crea_Documento_plantilla_servicio_web_radicado = "YES"
        '    Exit Function
        'End If
        '-------------------------------------------------------
        'Crear plantilla temporal y retorna la ruta temporal
        '-------------------------------------------------------
        Result = Solicita_ruta_documento_plantilla_temporal(Ruta_plantilla, _
                                                            Ruta_Docx_Plantilla, _
                                                            id_respuesta)
        If Result <> "YES" Then
            Solicita_Documento_plantilla_radicado = "Funcion Crea_Documento_Libre_Docx : " & Result
            Exit Function
        End If
        Try
            Dim nombre_destinatario As String = ""
            Dim direccion_destinatario As String = ""
            Dim tel_destinatario As String = ""
            Dim id_destinatario As Integer = 0
            Dim radicado_respeuesta As String = ""
            Dim asunto As String = ""
            Dim Refclas As New ClassTrdDocumental
            Dim Refclas_ra_respuesta_radicado As New Class_ra_respuesta_radicado
            Dim Refclas_destinario_externo As New Class_destinatario_externo
            Dim Refclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Refclas_ra_respuesta_radicado.Solicita_id_destinatario_externo_plantilla(id_respuesta, _
                                                                                              id_destinatario)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = "Funcion Crea_Documento_plantilla_servicio_web : " & Result
                Exit Function
            End If
            Dim nombre_area As String = ""
            Dim id_area As Integer = 0
            Result = Refclas_remit_dest_interno.Solicita_id_area_departamento_usuario_gestion(id_usuario_gestion, _
                                                                                              id_area)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = "Funcion Crea_Documento_plantilla_servicio_web : " & Result
                Exit Function
            End If
            Dim Refclas_area_depart_radicacion As New Class_areas_depart_radicacion
            Result = Refclas_area_depart_radicacion.Solicita_nombre_area_departamento(id_area, nombre_area)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = "Funcion Crea_Documento_plantilla_servicio_web : " & Result
                Exit Function
            End If
            'Result = Refclas_ra_respuesta_radicado.Solicta_nombre_area_responsable_respuesta(id_respuesta, _
            '                                                                                 nombre_area)
            'If Result <> "YES" Then
            '    Solicita_Documento_plantilla_radicado = "Funcion Crea_Documento_plantilla_servicio_web : " & Result
            '    Exit Function
            'End If
            Dim Nombre_plantilla_radicado As String = ""
            Dim ref_Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Result = ref_Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                              Nombre_plantilla_radicado)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = "Funcion Crea_Documento_plantilla_servicio_web : " & Result
                Exit Function
            End If
            If id_destinatario <> 0 Then
                Result = Refclas_destinario_externo.Solicita_datos_caracterizacion_solicitante_respuesta_externo(id_destinatario, _
                                                                                                                 Nombre_plantilla_radicado,
                                                                                                                 nombre_destinatario, _
                                                                                                                 direccion_destinatario, _
                                                                                                                 tel_destinatario)
                If Result <> "YES" Then
                    Solicita_Documento_plantilla_radicado = "Funcion Crea_Documento_plantilla_servicio_web : " & Result
                    Exit Function
                End If

            End If
            Result = Refclas_ra_respuesta_radicado.Solicta_asunto_solicitud_respuesta(id_respuesta, _
                                                                                      asunto)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = "Funcion Crea_Documento_plantilla_servicio_web : " & Result
                Exit Function
            End If
            Dim ciudad As String = ""
            Result = Refclas_remit_dest_interno.Solicita_ciudad_sede_usuario_gestion(id_usuario_gestion, _
                                                                                     ciudad)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = "Función Crea_Documento_plantilla_servicio_web_radicado : " & Result
                Exit Function
            End If
            Dim mes As New Date
            Dim numero_mes As Integer = 0
            Dim nombre_mes As String = MonthName(Now.Month, False)
            Dim numero_dia As Integer = Now.Day
            Dim numero_ano As Integer = Now.Year
            ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
            Dim document As DocumentModel = DocumentModel.Load(Ruta_Docx_Plantilla)
            For Each item As ContentRange In document.Content.Find("Cod_unico_ver.1.1")
                item.LoadText("Código interno " & id_respuesta)
            Next
            Dim ref_dia As String = ""
            Dim ceros_dia As String = ""
            Result = Me.Ceros_dia_ext(numero_dia, ceros_dia)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = "Función Solicita_Documento_plantilla_radicado : " & Result
                Exit Function
            End If
            ref_dia = ceros_dia & numero_dia.ToString
            For Each item As ContentRange In document.Content.Find("Fecha_ver.1.1")
                item.LoadText(ciudad & ",  " & nombre_mes & " " & ref_dia & " de " & numero_ano)
            Next
            For Each item As ContentRange In document.Content.Find("Destinatario_ver.1.1")
                item.LoadText(UCase(nombre_destinatario))
            Next
            For Each item As ContentRange In document.Content.Find("Direccion_ver.1.1")
                item.LoadText("Dir. " & direccion_destinatario)
            Next
            For Each item As ContentRange In document.Content.Find("Telefono_ver.1.1")
                item.LoadText("Tel." & tel_destinatario)
            Next
            For Each item As ContentRange In document.Content.Find("Asunto_ver.1.1")
                item.LoadText("Asunto: " & asunto & ".")
            Next
            'For Each item As ContentRange In document.Content.Find("Aresponder_ver.1.1")
            '    item.LoadText("Al responder cite este radicado" & radicado_respeuesta)
            'Next
            For Each item As ContentRange In document.Content.Find("Usuario_ver.1.1")
                item.LoadText(Pie_usuario(0))
            Next
            For Each item As ContentRange In document.Content.Find("Cargo_ver.1.1")
                item.LoadText(Pie_usuario(1))
            Next
            For Each item As ContentRange In document.Content.Find("Area_ver.1.1")
                item.LoadText(nombre_area)
            Next
            For Each item As ContentRange In document.Content.Find("Area_ver_producion.1.1")
                item.LoadText("DEP : " & nombre_area)
            Next
            For Each item As ContentRange In document.Content.Find("Respuesta_radicado_ver.1.1")
                item.LoadText("Respuesta al radicado " & Radicado)
            Next
            '-----------------------------------------------------
            'Guarda el radicado de respuesta
            '-----------------------------------------------------
            For Each item As ContentRange In document.Content.Find("Radicado_ver.1.1")
                item.LoadText("Radicado respuesta " & radicado_respuesta)
            Next
            For Each item As ContentRange In document.Content.Find("Asunto_ver.1.1")
                item.LoadText("Asunto: respuesta al radicado REE" & Radicado & ".")
            Next
            For Each item As ContentRange In document.Content.Find("Aresponder_ver.1.1")
                item.LoadText("Al responder cite este radicado" & Radicado)
            Next
            For Each item As ContentRange In document.Content.Find("Cod_unico_ver.1.1")
                item.LoadText("Código interno " & id_respuesta)
            Next
            If Ruta_Firma <> "" Then
                If File.Exists(Ruta_Firma) = True Then
                    Dim picture = New GemBox.Document.Picture(document, Ruta_Firma)
                    For Each item As ContentRange In document.Content.Find("Frima_ver.1.1")
                        item.Set(picture.Content)
                    Next
                End If
            End If          
            document.Save(Ruta_Docx_Plantilla)
            Dim dat As Date = Now
            Dim fecha_registro_descarga_plantilla As String = ""
            Dim Refclas_gestion_fecha As New ClassGestionFechas
            Result = Refclas_gestion_fecha.Formatea_fecha_time_framework(dat, _
                                                                       fecha_registro_descarga_plantilla)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = Result
                Exit Function
            End If
            Dim Ref_registro As New Class_ra_ra_registro_down_formato
            Result = Ref_registro.Registra_descarga_formato_respuesta_solicitud(id_usuario_gestion, _
                                                                               id_respuesta, _
                                                                                id_usuario_firma, _
                                                                              fecha_registro_descarga_plantilla)
            If Result <> "YES" Then
                Solicita_Documento_plantilla_radicado = Result
                Exit Function
            End If
            Docx = ReadFile(Ruta_Docx_Plantilla)
            Solicita_Documento_plantilla_radicado = "YES"
        Catch ex As Exception
            Solicita_Documento_plantilla_radicado = "Inconsistencia funcion Solicita_Documento_plantilla_radicado " & ex.Message
        End Try

    End Function

    Function Solicita_ruta_documento_plantilla_temporal(ByVal Ruta_Plantilla As String, _
                                                        ByRef Ruta_Docx_Plantilla As String, _
                                                        ByVal Radicado As String) As String
        '*********************************************************************
        'Funcion : La funcion copia la plantilla a una plantilla temporal
        'con la referencia del radicado, la fecha y un numero aleatorio
        'regresa el nombre de la plantilla
        'Fecha : 2013-04-19
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************************
        Try
            'Verifica la existencia de la ruta de plantilla
            If File.Exists(Ruta_Plantilla) = False Then
                Solicita_ruta_documento_plantilla_temporal = "El archivo plantilla no existe " & Ruta_Plantilla
                Exit Function
            End If
            Dim fileinf As New FileInfo(Ruta_Plantilla)
            Dim Dat As String = Date.Now
            'Crea el nombre del archivo plantilla
            Dim rand As New Random
            Dim randsalida As Integer = rand.Next()
            Dim Nombre_Plantilla As String = Radicado & "-" & Dat & "-" & randsalida.ToString
            Nombre_Plantilla = Nombre_Plantilla.Replace("\", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("/", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace(":", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("*", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("?", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("""", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("<>", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("<", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace(">", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("|", "")
            Nombre_Plantilla = Nombre_Plantilla & fileinf.Extension
            '\ / : * ? " <> |
            'Crea la ruta de la plantilla
            Dim Ruta_plantilla_vista As String = fileinf.DirectoryName
            Ruta_plantilla_vista = Ruta_plantilla_vista & "\DIRECTROY\"
            If Directory.Exists(Ruta_plantilla_vista) = False Then
                Directory.CreateDirectory(Ruta_plantilla_vista)
            End If
            Ruta_plantilla_vista = Ruta_plantilla_vista & Nombre_Plantilla
            If File.Exists(Ruta_plantilla_vista) = True Then
                Kill(Ruta_plantilla_vista)
            End If
            'copia el archivo plantilla
            File.Copy(Ruta_Plantilla, Ruta_plantilla_vista)
            Ruta_Docx_Plantilla = Ruta_plantilla_vista
            Solicita_ruta_documento_plantilla_temporal = "YES"
        Catch ex As Exception
            Solicita_ruta_documento_plantilla_temporal = "Funcion Solicita_ruta_documento_plantilla_temporal " & ex.Message
        End Try
    End Function
    Function ReadFile(ByVal FilePath As String) As Byte()
        Dim fs As FileStream
        Try
            ' Read file and return contents
            fs = File.Open(FilePath, FileMode.Open, FileAccess.Read)
            Dim lngLen As Long = fs.Length
            Dim abytBuffer(CInt(lngLen - 1)) As Byte
            fs.Read(abytBuffer, 0, CInt(lngLen))
            Return abytBuffer
        Catch exp As Exception
            Return Nothing
        Finally
            If Not fs Is Nothing Then
                fs.Close()
            End If
        End Try
    End Function
    Function Firma_documento_formato_respuesta(ByVal documento_respuesta_servidor As String,
                                               ByVal opcion_firma_digital As String,
                                               ByVal pasword_firma_digital As String,
                                               ByVal file_digital_archivo As String,
                                               ByVal id_usuario_firma As Integer,
                                               ByVal id_usuario_gestion As Integer,
                                               ByVal estado_firma As Integer,
                                               ByVal formato_documento As String,
                                               ByRef file_salida As String) As String
        Try
            Dim Refclas_remit_dest_int As New Class_remit_dest_interno
            Dim Refclas_usuario_workflow As New ClassWorkflowUsuario
            Dim nombre_cargo_usuario_firma_respuesta As String = ""
            Dim nombre_area_usuario_firma_respuesta As String = ""
            Dim nombre_usuario_firma_respuesta As String = ""
            Dim id_area_usuario_firma_respuesta As Integer = 0
            Dim id_usuario_wf_firma_respuesta As Integer = 0
            Dim ruta_archivo_firma As String = ""
            Dim nombre_cargo_usuario_remplaza As String = ""
            Dim nombre_area_usuario_remplaza As String = ""
            Dim nombre_usuario_remplaza As String = ""
            Dim id_area_usuario_remplaza As Integer = 0
            Dim Result As String = ""
            If opcion_firma_digital = 1 Then
                If pasword_firma_digital = "" Then
                    Firma_documento_formato_respuesta = "El sistema requiere de su  clave para la firma digital"
                    Exit Function
                End If
                If file_digital_archivo = "" Then
                    Firma_documento_formato_respuesta = "El sistema requiere de su archivo de firma digital"
                    Exit Function
                End If
            End If
            '--------------------------------------------------------------------
            'Verifica firma autorizada
            '-------------------------------------------------------------------
            If id_usuario_firma <> id_usuario_gestion Then
                Result = Refclas_remit_dest_int.Retorna_nombre_cargo_destinatario_interno(id_usuario_firma,
                                                                                          nombre_usuario_firma_respuesta,
                                                                                          nombre_cargo_usuario_firma_respuesta)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
                Result = Refclas_remit_dest_int.Solicita_id_usuario_workflow_relacionado(id_usuario_firma,
                                                                                         id_usuario_wf_firma_respuesta)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
                Result = Refclas_remit_dest_int.Solicita_id_area_nombre_area_destinatario(id_usuario_firma,
                                                                                          id_area_usuario_firma_respuesta,
                                                                                          nombre_area_usuario_firma_respuesta)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
                Result = Refclas_usuario_workflow.Solicita_firma_usuario_workflow(id_usuario_wf_firma_respuesta,
                                                                                  ruta_archivo_firma)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
                '----------------------------------------------------------------------
                'Solicita caracterización usuario remplaza datos de caracterización
                '---------------------------------------------------------------------
                Result = Refclas_remit_dest_int.Retorna_nombre_cargo_destinatario_interno(id_usuario_gestion,
                                                                                         nombre_usuario_remplaza,
                                                                                         nombre_cargo_usuario_remplaza)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If

                Result = Refclas_remit_dest_int.Solicita_id_area_nombre_area_destinatario(id_usuario_gestion,
                                                                                          id_area_usuario_remplaza,
                                                                                          nombre_area_usuario_remplaza)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
            Else
                Result = Refclas_remit_dest_int.Retorna_nombre_cargo_destinatario_interno(id_usuario_gestion,
                                                                                          nombre_usuario_firma_respuesta,
                                                                                          nombre_cargo_usuario_firma_respuesta)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
                Result = Refclas_remit_dest_int.Solicita_id_usuario_workflow_relacionado(id_usuario_gestion,
                                                                                         id_usuario_wf_firma_respuesta)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
                Result = Refclas_remit_dest_int.Solicita_id_area_nombre_area_destinatario(id_usuario_gestion,
                                                                                          id_area_usuario_firma_respuesta,
                                                                                          nombre_area_usuario_firma_respuesta)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
                Result = Refclas_usuario_workflow.Solicita_firma_usuario_workflow(id_usuario_wf_firma_respuesta,
                                                                                  ruta_archivo_firma)
                If Result <> "YES" Then
                    Firma_documento_formato_respuesta = Result
                    Exit Function
                End If
            End If
            ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
            Dim document As DocumentModel = DocumentModel.Load(documento_respuesta_servidor)
            Dim file_inf As New FileInfo(documento_respuesta_servidor)
            file_salida = documento_respuesta_servidor.Replace(file_inf.Extension, "." & formato_documento)
            If File.Exists(file_salida) Then
                Kill(file_salida)
            End If
            If ruta_archivo_firma <> "" Then
                If File.Exists(ruta_archivo_firma) = True Then
                    Dim picture = New GemBox.Document.Picture(document, ruta_archivo_firma)
                    If estado_firma = 1 And formato_documento <> "DOCX" Then
                        For Each item As ContentRange In document.Content.Find("Frima_ver.1.1")
                            item.Set(picture.Content)
                        Next
                    End If
                    If id_usuario_firma <> id_usuario_gestion Then
                        For Each item As ContentRange In document.Content.Find(nombre_usuario_remplaza)
                            item.LoadText(nombre_usuario_firma_respuesta)
                        Next
                        For Each item As ContentRange In document.Content.Find(nombre_cargo_usuario_remplaza)
                            item.LoadText(nombre_cargo_usuario_firma_respuesta)
                        Next
                        For Each item As ContentRange In document.Content.Find(nombre_area_usuario_remplaza)
                            item.LoadText(nombre_area_usuario_firma_respuesta)
                        Next
                    End If
                    If opcion_firma_digital = 1 Then
                        document.Save(file_salida)
                        Firma_documento_formato_respuesta = "YES"
                        Exit Function
                    Else
                        document.Save(file_salida)
                        Firma_documento_formato_respuesta = "YES"
                        Exit Function
                    End If
                Else
                    Firma_documento_formato_respuesta = "Función Firma_documento_formato_respuesta dice Imposible encontrar archivo firma " & ruta_archivo_firma
                    Exit Function
                End If
            Else
                document.Save(file_salida)
                Firma_documento_formato_respuesta = "YES"
                Exit Function
            End If
            Firma_documento_formato_respuesta = "YES"
        Catch ex As Exception
            Firma_documento_formato_respuesta = "Inconsistencia función Firma_documento_formato_respuesta " & ex.Message
        End Try
    End Function
    Function Verifica_auntentificacion_doc_respuesta_web(ByVal ruta_documento_temporal As String, ByVal id_respuesta As Integer) As String
        Try
            'Dim ruta_documento_temporal As String = ruta_temporal & id_respuesta & ".docx"
            'If File.Exists(ruta_documento_temporal) = True Then
            '    Kill(ruta_documento_temporal)
            'End If
            Dim result As String = ""
            'Dim refclas As New Classconvert
            'result = refclas.descarga_a_disco(ruta_documento_temporal, documento_resp_docx)
            'If result <> "YES" Then
            '    Verifica_auntentificacion_doc_respuesta_web = "Function Verifica_auntentificacion_doc_respuesta_web 22 dice " & result
            '    Exit Function
            'End If
            ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
            Dim document As DocumentModel = DocumentModel.Load(ruta_documento_temporal)
            If document.Content.Find("Radicado_ver.1.1").Count = 0 Then
                Verifica_auntentificacion_doc_respuesta_web = "El documento de respuesta carece del segmento (Radicado_ver.1.1) "
                Exit Function
            End If
            If document.Content.Find("Aresponder_ver.1.1").Count = 0 Then
                Verifica_auntentificacion_doc_respuesta_web = "El documento de respuesta carece del segmento (Aresponder_ver.1.1) "
                Exit Function
            End If
            If document.Content.Find("Código interno " & id_respuesta).Count = 0 Then
                Verifica_auntentificacion_doc_respuesta_web = "El documento de respuesta carece del Código interno (" & id_respuesta & ")"
                Exit Function
            End If

            Verifica_auntentificacion_doc_respuesta_web = "YES"
        Catch ex As Exception
            Verifica_auntentificacion_doc_respuesta_web = "Inconsistencia función Verifica_auntentificacion_doc_respuesta_web " & ex.Message
        End Try
    End Function
    Function Verifica_auntentificacion_doc_respuesta_web_radicado(ByVal ruta_documento_temporal As String, _
                                                                  ByVal id_respuesta As Integer, _
                                                                  ByRef estado_verficacion As String) As String
        Try
            'Dim ruta_documento_temporal As String = ruta_temporal & id_respuesta & ".docx"
            'If File.Exists(ruta_documento_temporal) = True Then
            '    Kill(ruta_documento_temporal)
            'End If
            Dim result As String = ""
            'Dim refclas As New Classconvert
            'result = refclas.descarga_a_disco(ruta_documento_temporal, documento_resp_docx)
            'If result <> "YES" Then
            '    Verifica_auntentificacion_doc_respuesta_web = "Function Verifica_auntentificacion_doc_respuesta_web 22 dice " & result
            '    Exit Function
            'End If
            ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
            Dim document As DocumentModel = DocumentModel.Load(ruta_documento_temporal)
            'If document.Content.Find("Radicado_ver.1.1").Count = 0 Then
            '    Verifica_auntentificacion_doc_respuesta_web_radicado = "El documento de respuesta carece del segmento (Radicado_ver.1.1) "
            '    Exit Function
            'End If
            'If document.Content.Find("Aresponder_ver.1.1").Count = 0 Then
            '    Verifica_auntentificacion_doc_respuesta_web_radicado = "El documento de respuesta carece del segmento (Aresponder_ver.1.1) "
            '    Exit Function
            'End If
            estado_verficacion = "YES"
            If document.Content.Find("Código interno " & id_respuesta).Count = 0 Then
                estado_verficacion = "El documento de respuesta carece del Código interno (" & id_respuesta & "), es posible que el formato de respuesta se haya modificado, vuelva y descargue el formato y copie el contenido de su respuesta"
                Verifica_auntentificacion_doc_respuesta_web_radicado = "YES"
                Exit Function
            End If

            Verifica_auntentificacion_doc_respuesta_web_radicado = "YES"
        Catch ex As Exception
            Verifica_auntentificacion_doc_respuesta_web_radicado = "Inconsistencia función Verifica_auntentificacion_doc_respuesta_web_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_formato_respuesta_con_Footers(ByVal id_respuesta As Integer, _
                                                     ByVal ruta_documento As String, _
                                                     ByRef ruta_documento_salida As String) As String
        Dim document_contenido As New GemBox.Document.DocumentModel
        Dim document_contenido_foter As New GemBox.Document.DocumentModel
        Try
            '-----------------------------------------
            'Lee el documento fuente que tiene el con
            'tenido del documento
            '---------------------------------------
            document_contenido = GemBox.Document.DocumentModel.Load(ruta_documento, _
                                                                    GemBox.Document.LoadOptions.DocxDefault)
            'Limpia los metadatos para que no salgan el chkeditor
            document_contenido.DocumentProperties.Custom.Clear()
            document_contenido.DocumentProperties.BuiltIn.Clear()
            'Elimina los Footers para que no salga en el chkeditor
            document_contenido.Sections(0).HeadersFooters.Clear()
            'Elimina el posicionamiento de las imagenes
            For Each picture As GemBox.Document.Picture In document_contenido.GetChildElements(True, GemBox.Document.ElementType.Picture)
                picture.Layout = GemBox.Document.Layout.Inline(picture.Layout.Size)
            Next
            'Elimina el posicionamiento de las tablas
            For Each ob2 As GemBox.Document.Tables.Table In document_contenido.GetChildElements(True, GemBox.Document.ElementType.Table).Cast(Of Table)()
                'ob.TableFormat.Positioning.ClearPositioning()
                'ob.TableFormat.Alignment = GemBox.Document.HorizontalAlignment.Right
                ob2.TableFormat.Alignment = GemBox.Document.HorizontalAlignment.Left

            Next
            '-----------------------------------------------------------
            'Descarga el documento con la plantilla con el futeers
            '-----------------------------------------------------------
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
            'Dim conten As Object = Nothing
            'Dim OB As New localhost.Service
            'Dim Result As String = ""
            'OB.Url = HttpContext.Current.Session.Item("URL_WEB_SERVICE")
            'Result = OB.Donwload_archivo_plantilla_Footers(conten)
            'If Result <> "YES" Then
            '    Retorna_documento_respuesta_con_Footers = Result
            '    Exit Function
            'End If
            'Dim Refclas_sav_file As New ClassRaEnvioCorrespondencia
            'Result = Refclas_sav_file.SaveFile(ruta_fisica & "template_rut" & ".docx", conten)
            'If Result <> "YES" Then
            '    Retorna_documento_respuesta_con_Footers = Result
            '    Exit Function
            'End If
            Dim ruta_virtual_repositorio As String = "../repositorio/"
            Dim ruta_fisica_repositorio As String = HttpContext.Current.Server.MapPath(ruta_virtual_repositorio) & "plantillalibre_web_contendor_Footers.docx"
            If File.Exists(ruta_fisica_repositorio) = False Then
                Solicita_formato_respuesta_con_Footers = "Falta el documento plantillalibre_web_contendor_Footers.docx en el repositorio"
                Exit Function
            Else
                If File.Exists(ruta_fisica & "template_rut" & ".docx") = True Then
                    Kill(ruta_fisica & "template_rut" & ".docx")
                End If
                File.Copy(ruta_fisica_repositorio, ruta_fisica & "template_rut" & ".docx")
            End If
            document_contenido_foter = GemBox.Document.DocumentModel.Load(ruta_fisica & "template_rut" & ".docx", GemBox.Document.LoadOptions.DocxDefault)
            Dim settings As XmlWriterSettings = New XmlWriterSettings()
            settings.OmitXmlDeclaration = True
            settings.ConformanceLevel = ConformanceLevel.Fragment
            settings.CloseOutput = False
            Dim str As String = ""
            Using sw = New StringWriter()
                Using xw = XmlWriter.Create(sw, settings)
                    document_contenido.Save(xw, New GemBox.Document.HtmlSaveOptions() With {.EmbedImages = True, .UseSemanticElements = False, .HtmlType = GemBox.Document.HtmlType.HtmlInline})
                    str = sw.ToString().Replace("<title></title>", "")
                    str = str.Replace("margin-left:-27pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;mso-pagination:lines-together;" & """" & ">", "")
                    str = str.Replace("<span>&#xa0;</span></p>", "")
                    'str = str.Replace("<p style=" & """" & "<p style=" & """" & "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;" & """" & ">", "")
                    'str = str.Replace("<p style=" & """" & "margin-left:0pt;margin-right:0pt;margin-top:0pt;margin-bottom:0pt;padding: 1pt 4pt;" & """" & ">", "")
                    str = str.Replace("<span style=" & """" & "font-family:Bell MT;font-size:7pt;" & """" & "> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</span></p>", "")
                    'htmlEditor.Text = str
                End Using
            End Using
            For Each item As GemBox.Document.ContentRange In document_contenido_foter.Content.Find("Resp_conte_web")
                item.LoadText(str, GemBox.Document.LoadOptions.HtmlDefault)
            Next
            document_contenido_foter.Save(ruta_fisica & "template_rut" & ".docx", New GemBox.Document.DocxSaveOptions() With {.Format = GemBox.Document.DocxFormat.Docx})
            ruta_documento_salida = ruta_fisica & "template_rut" & ".docx"
            Solicita_formato_respuesta_con_Footers = "YES"
        Catch ex As Exception
            Solicita_formato_respuesta_con_Footers = "Inconsistencia función Solicita_formato_respuesta_con_Footers " & ex.Message
        Finally
            document_contenido_foter = Nothing
            document_contenido = Nothing
        End Try
    End Function
    Function Solicita_descargar_documento_plantilla(ByVal id_tipo_plantilla As Integer, _
                                                    ByVal id_usuario_gestion As Integer, _
                                                    ByRef Hidden_ruta_archivo As Object, _
                                                    ByRef iframe As Object, _
                                                    ByRef ref_update_panel As UpdatePanel, _
                                                    ByRef UpdatePanel_botones_unidad As UpdatePanel) As String
        Try
            '-----------------------------------------------------
            'Solicita datos usuario de caracterización
            '-----------------------------------------------------
            Dim stru As STRU_USUARIO_GESTION = Nothing
            Dim Result As String = ""
            Dim Refclas As New ClassTrdDocumental
            Result = Me.Solicita_datos_de_caracrerizacion_usuario_gestion(id_usuario_gestion, _
                                                                          stru)
            If Result <> "YES" Then
                Solicita_descargar_documento_plantilla = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Bajar firma usuario worflow
            '-----------------------------------------------------
            Dim ref_ruta_firma As String = ""
            Dim ref_class_workflow As New ClassWorkflowUsuario
            ref_class_workflow.Solicita_firma_usuario_workflow(HttpContext.Current.Session("Id_Usuario_Workflow"), _
                                                               ref_ruta_firma)
            If Result <> "YES" Then
                Solicita_descargar_documento_plantilla = Result
                Exit Function
            End If
            'Dim Parametro_Consulta As String = "select FIRMA_USUARIO from USUARIO_WORKFLOW where IDU_SUARIO ='" & HttpContext.Current.Session("Id_Usuario_Workflow") & "'"
            'Dim Ref_Clas_Inicio As New ClassNeodynamic
            'If HttpContext.Current.Session("WF_RUTA_FIRMA") = "../tempfirma/" Then
            '    Dim Resultado_Firma As String = Ref_Clas_Inicio.Bajar_Firma_Plantilla_Wf(Parametro_Consulta, _
            '                                                                             "bmp", _
            '                                                                             " Firma ")
            '    If Resultado_Firma = "YES" Then
            '        ref_ruta_firma = HttpContext.Current.Server.MapPath(HttpContext.Current.Session("WF_RUTA_FIRMA")) & HttpContext.Current.Session("Id_Usuario_Workflow") & ".bmp"
            '    End If
            'Else
            '    ref_ruta_firma = HttpContext.Current.Server.MapPath(HttpContext.Current.Session("WF_RUTA_FIRMA"))
            'End If
            '-----------------------------------------------------
            'Descarga archivo plantilla
            '-----------------------------------------------------
            Dim Ruta_plantilla As String = ""
            Result = Me.Crea_documento_copia_plantilla(id_tipo_plantilla, _
                                                       Ruta_plantilla, _
                                                       id_usuario_gestion)
            If Result <> "YES" Then
                Solicita_descargar_documento_plantilla = Result
                Exit Function
            End If
            Dim ciudad As String = ""
            Dim Reclas_remit_dest_interno As New Class_remit_dest_interno
            Result = Reclas_remit_dest_interno.Solicita_ciudad_sede_usuario_gestion(id_usuario_gestion, ciudad)
            If Result <> "YES" Then
                Solicita_descargar_documento_plantilla = "Función Solicita_descargar_documento_plantilla : " & Result
                Exit Function
            End If
            Dim mes As New Date
            Dim numero_mes As Integer = 0
            Dim nombre_mes As String = MonthName(Now.Month, False)
            Dim numero_dia As Integer = Now.Day
            Dim numero_ano As Integer = Now.Year
            ComponentInfo.SetLicense("DTFX-JTBY-6RJK-Y101")
            Dim document As DocumentModel = DocumentModel.Load(Ruta_plantilla)
            Dim ref_dia As String = ""
            Dim ceros_dia As String = ""
            Result = Me.Ceros_dia_ext(numero_dia, ceros_dia)
            If Result <> "YES" Then
                Solicita_descargar_documento_plantilla = "Función Solicita_descargar_documento_plantilla : " & Result
                Exit Function
            End If
            ref_dia = ceros_dia & numero_dia.ToString
            For Each item As ContentRange In document.Content.Find("Fecha_ver.1.1")
                item.LoadText(ciudad & ",  " & nombre_mes & " " & ref_dia & " de " & numero_ano)
            Next
            For Each item As ContentRange In document.Content.Find("Ciudad_ver.1.1")
                item.LoadText(ciudad)
            Next
            For Each item As ContentRange In document.Content.Find("Usuario_ver.1.1")
                item.LoadText(stru.Nombre_usuario)
            Next
            For Each item As ContentRange In document.Content.Find("Cargo_ver.1.1")
                item.LoadText(stru.Cargo_usuario)
            Next
            For Each item As ContentRange In document.Content.Find("Area_ver.1.1")
                item.LoadText(stru.Area_usuario)
            Next
            For Each item As ContentRange In document.Content.Find("Area_ver_producion.1.1")
                item.LoadText("DEP : " & stru.Area_usuario)
            Next

            If ref_ruta_firma <> "" Then
                If File.Exists(ref_ruta_firma) = True Then
                    Dim picture = New GemBox.Document.Picture(document, ref_ruta_firma)
                    'document.Bookmarks("Frima_ver.1.1").GetContent(False).Set(picture.Content)
                    For Each item As ContentRange In document.Content.Find("Frima_ver.1.1")
                        item.Set(picture.Content)
                    Next

                End If
            End If
            document.Save(Ruta_plantilla)
            Dim file_inf As New FileInfo(Ruta_plantilla)
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
            File.Copy(Ruta_plantilla, ruta_fisica & file_inf.Name)
            Hidden_ruta_archivo.value = ruta_fisica & file_inf.Name
            iframe.Attributes.Add("src", "..\Docuarchi\WebFormDaDescarga.aspx")
            ref_update_panel.Update()
            UpdatePanel_botones_unidad.Update()
            Solicita_descargar_documento_plantilla = "YES"
        Catch ex As Exception
            Solicita_descargar_documento_plantilla = "Inconsistencia general función Solicita_descargar_documento_plantilla " & ex.Message
        End Try
    End Function
    Function Ceros_dia_ext(ByVal Val_Ext As Integer, ByRef Ceros_Ext As String) As String
        Ceros_Ext = ""
        Try
            Select Case Len(Val_Ext.ToString)
                Case "1"
                    Ceros_Ext = Ceros_Ext & "0"

            End Select
            Ceros_dia_ext = "YES"
        Catch ex As Exception
            Ceros_dia_ext = ex.ToString
        End Try
    End Function
    Function Solicita_datos_de_caracrerizacion_usuario_gestion(ByVal id_usuario_gestion As Integer, _
                                                               ByRef stru As STRU_USUARIO_GESTION) As String

        '******************************************************************
        'Function : Retorna datos de caracterizacion usuario gestión 
        'respuesta
        'Fecha 2018-01-31
        'Ing : Miguel Angel Urueta Miranda
        '******************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " Select Nombre_Remitente,Cargo_Remite,Correo_Electronico,adr.Nombre_Area " & _
               " FROM  remit_dest_interno as rdi  " & _
               " left outer join areas_depart_radicacion as adr on (adr.Codigo_Area=rdi.Areas_Dep_Radicacion_id_Areas_Dep)" & _
               " where id_Remit_Dest_Int=" & id_usuario_gestion
            Dim Datset As DataSet = New DataSet("remit_dest_interno")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_de_caracrerizacion_usuario_gestion = "Función retorna_datos_respuesta_usuario_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_de_caracrerizacion_usuario_gestion = "Imposible encontrar datos de Caracterización usuario gestión"
                Exit Function
            Else
                stru.Nombre_usuario = Datset.Tables(0).Rows(0).Item(0)
                stru.Cargo_usuario = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru.Correo_Electronico = ""
                Else
                    stru.Correo_Electronico = Datset.Tables(0).Rows(0).Item(2)
                End If
                stru.Area_usuario = Datset.Tables(0).Rows(0).Item(3)
                Solicita_datos_de_caracrerizacion_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_de_caracrerizacion_usuario_gestion = "Inconsistencia general función Solicita_datos_de_caracrerizacion_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Crea_documento_copia_plantilla(ByVal id_tipo_plantilla As Integer, _
                                            ByRef Ruta_Docx_Plantilla As String, _
                                            ByVal id_usuario_gestion As Integer) As String
        Try
            Dim Ruta_tempo_directorio As String = ""
            If id_tipo_plantilla = 1 Then
                Ruta_tempo_directorio = HttpContext.Current.Server.MapPath("../repositorio/plantilla_documento_oficio_footers_web.docx")
            End If
            If id_tipo_plantilla = 2 Then
                Ruta_tempo_directorio = HttpContext.Current.Server.MapPath("../repositorio/plantilla_documento_oficio_sin_footers_web.docx")
            End If
            If File.Exists(Ruta_tempo_directorio) = False Then
                Crea_documento_copia_plantilla = "El archivo plantilla no existe " & Ruta_tempo_directorio
                Exit Function
            End If
            Dim fileinf As New FileInfo(Ruta_tempo_directorio)
            Dim Dat As String = Date.Now
            'Crea el nombre del archivo plantilla
            Dim rand As New Random
            Dim randsalida As Integer = rand.Next()
            Dim Nombre_Plantilla As String = id_usuario_gestion & "-" & randsalida.ToString
            Nombre_Plantilla = Nombre_Plantilla.Replace("\", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("/", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace(":", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("*", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("?", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("""", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("<>", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("<", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace(">", "")
            Nombre_Plantilla = Nombre_Plantilla.Replace("|", "")
            Nombre_Plantilla = Nombre_Plantilla & fileinf.Extension
            '\ / : * ? " <> |
            'Crea la ruta de la plantilla
            Dim Ruta_plantilla_vista As String = fileinf.DirectoryName
            Ruta_plantilla_vista = Ruta_plantilla_vista & "\DIRECTROY\"
            If Directory.Exists(Ruta_plantilla_vista) = False Then
                Directory.CreateDirectory(Ruta_plantilla_vista)
            End If
            Ruta_plantilla_vista = Ruta_plantilla_vista & Nombre_Plantilla
            If File.Exists(Ruta_plantilla_vista) = True Then
                Kill(Ruta_plantilla_vista)
            End If
            'copia el archivo plantilla
            File.Copy(Ruta_tempo_directorio, Ruta_plantilla_vista)
            Ruta_Docx_Plantilla = Ruta_plantilla_vista
            Crea_documento_copia_plantilla = "YES"
        Catch ex As Exception
            Crea_documento_copia_plantilla = "Inconsistencia general función Crea_documento_copia_plantilla " & ex.Message
        End Try
    End Function


End Class
