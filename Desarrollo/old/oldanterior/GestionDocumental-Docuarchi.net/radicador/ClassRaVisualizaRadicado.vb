Public Structure stru_gabinetes_busqueda_radicado
    Dim nombre_gabinete As String
    Dim campo_busqueda As String
End Structure
Public Class ClassRaVisualizaRadicado
    Function visualiza_documentos_relacionados_respuesta(ByVal radicado As String, _
                                                         ByVal incializa_treview As Integer, _
                                                         ByRef Tre_v2 As TreeView) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New Classgestionrespuesta
            Dim nombre_plantilla As String = ""
            Dim id_plantilla As Integer = 0
            Result = Refclas.Retorna_plantilla_default_respuesta(nombre_plantilla, _
                                                                 id_plantilla)
            If Result <> "YES" Then
                visualiza_documentos_relacionados_respuesta = Result
                Exit Function
            End If
            If nombre_plantilla = "" Then
                visualiza_documentos_relacionados_respuesta = "YES"
                Exit Function
            End If
            Dim refclasselecion As New Classselecciotarea
            Dim gabinete() As stru_gabinetes_busqueda_radicado
            '----------------------------------------------
            'Busca el gabinete predeterminado del radicado
            '----------------------------------------------
            Erase gabinete
            Result = Retorna_relacion_gabinete_busqueda(id_plantilla, _
                                                        gabinete)
            If Result <> "YES" Then
                visualiza_documentos_relacionados_respuesta = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna los radicados relacionados a la respuesta
            '---------------------------------------------------
            Dim matri_radicados() As String
            Erase matri_radicados
            Result = Refclas.Retorna_radicados_relacionados_respuesta(radicado, _
                                                                      matri_radicados)
            If Result <> "YES" Then
                visualiza_documentos_relacionados_respuesta = Result
                Exit Function
            End If
            If matri_radicados Is Nothing Then
                visualiza_documentos_relacionados_respuesta = "YES"
                Exit Function
            End If
            Dim option_aplica_trd As Integer = 0
            Dim Refclas_trd As New ClassTrdDocumental
            Dim Matri_Documentos_Tareas() As String
            Erase Matri_Documentos_Tareas
            Dim Conta_Doc As Integer = 0
            If Not gabinete Is Nothing Then
                For k As Integer = 0 To matri_radicados.Length - 1
                    For z As Integer = 0 To gabinete.Length - 1
                        Dim ref_Class_system1 As New Class_system1
                        Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd, _
                                                                                           gabinete(z).nombre_gabinete)
                        If Result <> "YES" Then
                            visualiza_documentos_relacionados_respuesta = Result
                            Exit Function
                        End If
                        '****************************************
                        'Consulta ruta busqueda de documentos
                        '****************************************
                        Dim Ruta_Busqueda As String = ""
                        Dim Refclasalamacen As New ClassAlmacenamiento
                        Dim ref_Class_SYSTEM1RUT As New Class_SYSTEM1RUT
                        Result = ""
                        Result = ref_Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruta_Busqueda, _
                                                                                   gabinete(z).nombre_gabinete)
                        If Result <> "YES" Then
                            visualiza_documentos_relacionados_respuesta = Result
                            Exit Function
                        End If
                        '**************************************************************
                        'Busca concordancia radicado y retorna imagenes del radicado
                        '**************************************************************
                        Dim matri_id_imagen() As Integer
                        Erase matri_id_imagen
                        Dim Refclascorreeo As New ClassCorreo
                        Result = Refclascorreeo.Retorna_id_imagen_docuarchi(gabinete(z).nombre_gabinete, _
                                                                            gabinete(z).campo_busqueda, _
                                                                            matri_radicados(k).ToString, _
                                                                            matri_id_imagen, _
                                                                            1)
                        If Result <> "YES" And Result <> "Imposible encontrar datos para el radicado " & radicado & " en el gabinete " & gabinete(z).nombre_gabinete Then
                            visualiza_documentos_relacionados_respuesta = Result
                            Exit Function
                        End If
                        '---------------------------------------------------------------
                        'Recorre las matriz de documentos relacionados
                        '---------------------------------------------------------------
                        If Not matri_id_imagen Is Nothing Then
                            For i As Integer = 0 To matri_id_imagen.Length - 1
                                Dim Datos_Imagen As String = ""
                                Result = ""
                                Dim ref_ClassDaGabinete As New ClassDaGabinete
                                Dim stru_paramter_image As stru_paramter_image = Nothing
                                Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(gabinete(z).nombre_gabinete,
                                                                                             Val(matri_id_imagen(i)),
                                                                                             stru_paramter_image,
                                                                                             option_aplica_trd)
                                If Result <> "YES" Then
                                    visualiza_documentos_relacionados_respuesta = Result
                                    Exit Function
                                End If
                                '***********************************************
                                'Determina la extension de la imagen
                                '***********************************************
                                Dim Cod_Visor As String = ""
                                Dim Extension As String = ""
                                Dim Estado_Documento As String = ""
                                Dim ref_Class_da_extension As New Class_da_extension
                                Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN, _
                                                                                              Cod_Visor, _
                                                                                              Extension, _
                                                                                              Estado_Documento)
                                If Result <> "YES" Then
                                    visualiza_documentos_relacionados_respuesta = "36 RADICADO Imposible determinar el tipo del id=" + matri_id_imagen(i) + Result
                                    Exit Function
                                End If
                                If Estado_Documento = "LINK" Then
                                    'Dim Tempo As String = Datos_Imagen_Matri(1).Replace("-", "")
                                    'Result = ""
                                    'Result = ref_ClassDaGabinete.Obtener_Datos_Imagen("", _
                                    '                                                  gabinete(z).nombre_gabinete, _
                                    '                                                  matri_id_imagen(i), _
                                    '                                                  Datos_Imagen, _
                                    '                                                  option_aplica_trd)
                                    'If Result <> "YES" Then
                                    '    visualiza_documentos_relacionados_respuesta = "37  Obteniendo datos de la imagen tipo lik=" + matri_id_imagen(i) + " " + Result
                                    '    Exit Function
                                    'End If
                                    'Erase Datos_Imagen_Matri
                                    'Datos_Imagen_Matri = Split(Datos_Imagen, "|")
                                    'matri_id_imagen(i) = Datos_Imagen_Matri(0)
                                End If
                                Result = ""
                                Dim Valor_Ceros_Imagen As String = "DIG"
                                Dim Valor_Ceros_Carpeta_Imagen As String = ""
                                Dim Valor_Disco_Imagen As String = ""
                                Dim Ruta_Imagen As String = ""
                                '----------------------------------------------
                                'Obteniendo la identidad de la imagen 
                                '----------------------------------------------
                                Result = Obtener_Ceros_Imagen(matri_id_imagen(i), _
                                                              Valor_Ceros_Imagen)
                                If Result <> "YES" Then
                                    visualiza_documentos_relacionados_respuesta = "38 RADICADO Error En la funcion Obtener ceros para la imagen=" + matri_id_imagen(i) + Result
                                    Exit Function
                                End If
                                Valor_Ceros_Imagen = Valor_Ceros_Imagen & Extension
                                Result = ""
                                '--------------------------------------------------
                                'obtener la identidad de la carpeta
                                '--------------------------------------------------
                                Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image.IDEX, _
                                                                      Valor_Ceros_Carpeta_Imagen)
                                If Result <> "YES" Then
                                    visualiza_documentos_relacionados_respuesta = "#39 SELECCIONA-WF Error en la funcion obtener ceros de la carpeta =" + matri_id_imagen(i) + Result
                                    Exit Function
                                End If
                                '---------------------------------------------------
                                'Obtener carpeta contenedora imagen enlace
                                '---------------------------------------------------
                                Valor_Disco_Imagen = gabinete(z).nombre_gabinete & stru_paramter_image.DISC
                                '---------------------------------------------------
                                'Obtener ruta completa de iamgen enlace
                                'asignarla a la matris general
                                '---------------------------------------------------
                                Dim nombre_trd_documento As String = ""
                                If option_aplica_trd <> 0 Then
                                    nombre_trd_documento = stru_paramter_image.TIPODOCUMENTO
                                End If
                                Ruta_Imagen = Ruta_Busqueda & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen
                                ReDim Preserve Matri_Documentos_Tareas(Conta_Doc)
                                Matri_Documentos_Tareas(Conta_Doc) = Ruta_Imagen & "|" & stru_paramter_image.PAG & "|" & stru_paramter_image.ID & "|" & Extension & "|" & Estado_Documento & "|" & gabinete(z).nombre_gabinete & "|" & nombre_trd_documento
                                Conta_Doc = Conta_Doc + 1
                            Next

                        End If
                    Next
                Next
            End If
            If incializa_treview = 1 Then
                Tre_v2.Nodes.Clear()
            End If
            Dim Tre_v As New TreeNode
            Tre_v.Text = "Documentos relacionados"
            Tre_v.ExpandAll()
            If Not Matri_Documentos_Tareas Is Nothing Then
                For i As Integer = 0 To Matri_Documentos_Tareas.Length - 1
                    Dim spli() As String = Matri_Documentos_Tareas(i).Split("|")
                    Dim attrNodeGru1 As New TreeNode
                    attrNodeGru1.Text = "Documento(" & i & ")" & spli(5)
                    attrNodeGru1.Value = Matri_Documentos_Tareas(i)
                    If option_aplica_trd <> 0 Then
                        If spli(6) <> "" And spli(6) <> " " Then
                            attrNodeGru1.Text = spli(6)
                        Else
                            attrNodeGru1.Text = "Documento(" & i & ") " & spli(5)
                        End If
                    Else
                        attrNodeGru1.Text = "Documento(" & i & ") " & spli(5)
                    End If
                    Dim ref_clas_seleccion As New Classselecciotarea
                    Dim spli_archivo() As String = Matri_Documentos_Tareas(i).Split("|")
                    Result = ref_clas_seleccion.Agrega_icono_image_tre_view(spli_archivo(0), _
                                                                          attrNodeGru1)
                    Tre_v.ChildNodes.Add(attrNodeGru1)
                Next
            End If
            Tre_v2.EnableViewState = True
            Tre_v2.Nodes.Add(Tre_v)
            Tre_v2.ExpandAll()
            visualiza_documentos_relacionados_respuesta = "YES"
        Catch ex As Exception
            visualiza_documentos_relacionados_respuesta = "Inconsistencia general funcion visualiza_documentos_relacionados_respuesta " & ex.Message
        End Try
    End Function
    Function visualiza_documentos_radicado(ByVal radicado As String, _
                                           ByVal codigo_plantilla_radicado As Integer, _
                                           ByRef Tre_v2 As TreeView, _
                                           ByRef value_res As String) As String
        Try
            value_res = ""
            Dim Result As String = ""
            Dim Refclas As New ClassCorreo
            Dim refclasselecion As New Classselecciotarea
            Dim gabinete() As stru_gabinetes_busqueda_radicado
            '----------------------------------------------
            'Busca el gabinete predeterminado del radicado
            '----------------------------------------------
            Erase gabinete
            Result = Retorna_relacion_gabinete_busqueda(codigo_plantilla_radicado, _
                                                        gabinete)
            If Result <> "YES" Then
                visualiza_documentos_radicado = Result
                Exit Function
            End If
            Dim Refclas_trd As New ClassTrdDocumental
            Dim option_aplica_trd As Integer = 0
            Dim Matri_Documentos_Tareas() As String
            Erase Matri_Documentos_Tareas
            Dim Conta_Doc As Integer = 0
            Dim matri_relacion_tipos() As String
            If Not gabinete Is Nothing Then
                For z As Integer = 0 To gabinete.Length - 1
                    Dim ref_Class_system1 As New Class_system1
                    Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd, _
                                                                                       gabinete(z).nombre_gabinete)
                    If Result <> "YES" Then
                        visualiza_documentos_radicado = Result
                        Exit Function
                    End If
                    '****************************************
                    'Consulta ruta busqueda de documentos
                    '****************************************
                    Dim Ruta_Busqueda As String = ""
                    Dim ref_Class_SYSTEM1RUT As New Class_SYSTEM1RUT
                    Result = ""
                    Result = ref_Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruta_Busqueda, _
                                                                               gabinete(z).nombre_gabinete)
                    If Result <> "YES" Then
                        visualiza_documentos_radicado = Result
                        Exit Function
                    End If
                    '**************************************************************
                    'Busca concordancia radicado y retorna imagenes del radicado
                    '**************************************************************
                    Dim matri_id_imagen() As Integer
                    Erase matri_id_imagen
                    Result = Refclas.Retorna_id_imagen_docuarchi(gabinete(z).nombre_gabinete, _
                                                                 gabinete(z).campo_busqueda, _
                                                                 radicado.ToString, _
                                                                 matri_id_imagen, _
                                                                 1)
                    If Result <> "YES" And Result <> "Imposible encontrar datos para el radicado " & radicado & " en el gabinete " & gabinete(z).nombre_gabinete Then
                        visualiza_documentos_radicado = Result
                        Exit Function
                    End If
                    '---------------------------------------------------------------
                    'Recorre las matriz de documentos relacionados
                    '---------------------------------------------------------------
                    If Not matri_id_imagen Is Nothing Then
                        For i As Integer = 0 To matri_id_imagen.Length - 1
                            Result = ""
                            Dim ref_ClassDaGabinete As New ClassDaGabinete
                            Dim stru_paramter_image As stru_paramter_image = Nothing
                            Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(gabinete(z).nombre_gabinete,
                                                                                         Val(matri_id_imagen(i)),
                                                                                         stru_paramter_image,
                                                                                         option_aplica_trd)
                            If Result <> "YES" Then
                                visualiza_documentos_radicado = Result
                                Exit Function
                            End If
                            'Result = ref_ClassDaGabinete.Obtener_Datos_Imagen("", gabinete(z).nombre_gabinete, _
                            '                                                  matri_id_imagen(i), _
                            '                                                  Datos_Imagen, _
                            '                                                  option_aplica_trd)
                            'If Result <> "YES" Then
                            '    visualiza_documentos_radicado = "31 visualiza documento radicado error Obteniendo datos de la  " & matri_id_imagen(i) & " en el gabinete" + gabinete(z).nombre_gabinete + " Por favor verifique la existencia del documento en el gabinete"
                            '    Exit Function
                            'End If
                            'Erase matri_relacion_tipos
                            'Dim Datos_Imagen_Matri() As String
                            'Erase Datos_Imagen_Matri
                            'Datos_Imagen_Matri = Split(Datos_Imagen, "|")
                            'If Datos_Imagen_Matri Is Nothing Then
                            '    visualiza_documentos_radicado = "Imposible Encontrar Datos en el gabinete " & _
                            '     gabinete(z).nombre_gabinete + " de la imagen =" + matri_id_imagen(i)
                            '    Exit Function
                            'End If
                            '***********************************************
                            'Determina la extension de la imagen
                            '***********************************************
                            Dim Cod_Visor As String = ""
                            Dim Extension As String = ""
                            Dim Estado_Documento As String = ""
                            Dim ref_Class_da_extension As New Class_da_extension
                            Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN, _
                                                                                          Cod_Visor, _
                                                                                          Extension, _
                                                                                          Estado_Documento)
                            If Result <> "YES" Then
                                visualiza_documentos_radicado = "36 RADICADO Imposible determinar el tipo del id=" + matri_id_imagen(i) + Result
                                Exit Function
                            End If
                            If Estado_Documento = "LINK" Then
                                'Dim Tempo As String = Datos_Imagen_Matri(1).Replace("-", "")
                                'Result = ""

                                'Result = ref_ClassDaGabinete.Obtener_Datos_Imagen("", _
                                '                                                 gabinete(z).nombre_gabinete, _
                                '                                                 matri_id_imagen(i), _
                                '                                                 Datos_Imagen, _
                                '                                                 option_aplica_trd)
                                'If Result <> "YES" Then
                                '    visualiza_documentos_radicado = "37  Obteniendo datos de la imagen tipo lik=" + matri_id_imagen(i) + " " + Result
                                '    Exit Function
                                'End If
                                'Erase Datos_Imagen_Matri
                                'Datos_Imagen_Matri = Split(Datos_Imagen, "|")
                                'matri_id_imagen(i) = Datos_Imagen_Matri(0)
                            End If
                            Result = ""
                            Dim Valor_Ceros_Imagen As String = "DIG"
                            Dim Valor_Ceros_Carpeta_Imagen As String = ""
                            Dim Valor_Disco_Imagen As String = ""
                            Dim Ruta_Imagen As String = ""
                            '----------------------------------------------
                            'Obteniendo la identidad de la imagen 
                            '----------------------------------------------
                            Result = Obtener_Ceros_Imagen(matri_id_imagen(i), Valor_Ceros_Imagen)
                            If Result <> "YES" Then
                                visualiza_documentos_radicado = "38 RADICADO Error En la funcion Obtener ceros para la imagen=" + matri_id_imagen(i) + Result
                                Exit Function
                            End If
                            Valor_Ceros_Imagen = Valor_Ceros_Imagen & Extension
                            Result = ""
                            '--------------------------------------------------
                            'obtener la identidad de la carpeta
                            '--------------------------------------------------
                            Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image.IDEX, _
                                                                  Valor_Ceros_Carpeta_Imagen)
                            If Result <> "YES" Then
                                visualiza_documentos_radicado = "#39 SELECCIONA-WF Error en la funcion obtener ceros de la carpeta =" + matri_id_imagen(i) + Result
                                Exit Function
                            End If
                            '---------------------------------------------------
                            'Obtener carpeta contenedora imagen enlace
                            '---------------------------------------------------
                            Valor_Disco_Imagen = gabinete(z).nombre_gabinete & stru_paramter_image.DISC
                            '---------------------------------------------------
                            'Obtener ruta completa de iamgen enlace
                            'asignarla a la matris general
                            '---------------------------------------------------
                            Dim nombre_trd_documento As String = ""
                            If option_aplica_trd <> 0 Then
                                nombre_trd_documento = stru_paramter_image.TIPODOCUMENTO
                            End If
                            Ruta_Imagen = Ruta_Busqueda & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen
                            ReDim Preserve Matri_Documentos_Tareas(Conta_Doc)
                            Matri_Documentos_Tareas(Conta_Doc) = Ruta_Imagen & "|" & stru_paramter_image.PAG & "|" & stru_paramter_image.ID & "|" & Extension & "|" & Estado_Documento & "|" & gabinete(z).nombre_gabinete & "|" & nombre_trd_documento
                            If value_res = "" Then
                                value_res = Matri_Documentos_Tareas(Conta_Doc)
                            End If
                            Conta_Doc = Conta_Doc + 1
                        Next
                    End If
                Next
            End If
            Tre_v2.Nodes.Clear()
            Dim Tre_v As New TreeNode
            Tre_v.Text = ""
            Tre_v.ExpandAll()
            If Not Matri_Documentos_Tareas Is Nothing Then
                For i As Integer = 0 To Matri_Documentos_Tareas.Length - 1
                    Dim spli() As String = Matri_Documentos_Tareas(i).Split("|")
                    Dim attrNodeGru1 As New TreeNode
                    attrNodeGru1.Text = "Documento(" & i & ")" & spli(5)
                    attrNodeGru1.Value = Matri_Documentos_Tareas(i)
                    If option_aplica_trd <> 0 Then
                        If spli(6) <> "" And spli(6) <> " " Then
                            attrNodeGru1.Text = spli(6)
                        Else
                            attrNodeGru1.Text = "Documento(" & i & ") " & spli(5)
                        End If
                    Else
                        attrNodeGru1.Text = "Documento(" & i & ") " & spli(5)
                    End If
                    Dim ref_clas_seleccion As New Classselecciotarea
                    Dim spli_archivo() As String = Matri_Documentos_Tareas(i).Split("|")
                    Result = ref_clas_seleccion.Agrega_icono_image_tre_view(spli_archivo(0), _
                                                                          attrNodeGru1)
                    Tre_v.ChildNodes.Add(attrNodeGru1)
                Next
            End If
            Tre_v2.EnableViewState = True
            Tre_v2.Nodes.Add(Tre_v)
            Tre_v2.ExpandAll()
            visualiza_documentos_radicado = "YES"
        Catch ex As Exception
            visualiza_documentos_radicado = "Inconcistencia función visualiza_documentos_radicado " & ex.Message
        End Try
    End Function

    Function Retorna_relacion_gabinete_busqueda(ByVal id_plantilla As Integer, ByRef Gabinete() As stru_gabinetes_busqueda_radicado) As String
        '********************************************************************
        'Función : Asigna el campo de gabinete y el gabiente para la busqeda 
        'del radicado
        'Fecha : 2015-05-01
        'Ingeniero : Miguel Angel Urueta Miranda
        '*********************************************************************
        Try
            Erase Gabinete
            Dim Parametro_Consulta As String = "select NOMBRE,CAMPO_BUSQUEDA from ra_relacion_plantilla_radicado_gabinete where id_plantilla =" & id_plantilla
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("desxt")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_relacion_gabinete_busqueda = " Función Retorna_relacion_gabinete_busqueda_predeterminado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_relacion_gabinete_busqueda = "Imposible encontrar gabinete relacionado para la busqueda del documento "
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Gabinete(i)
                    Gabinete(i).nombre_gabinete = Datset.Tables(0).Rows(i).Item(0)
                    Gabinete(i).campo_busqueda = Datset.Tables(0).Rows(i).Item(1)
                Next
                Retorna_relacion_gabinete_busqueda = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_relacion_gabinete_busqueda = "Inconsistencia función Gabinete(i) " & ex.Message
        End Try
    End Function
End Class
