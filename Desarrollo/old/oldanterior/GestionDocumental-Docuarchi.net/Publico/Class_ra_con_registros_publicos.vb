Public Class class_parameter_consulta_documentos_acto
    Property libro As Integer
    Property inscripcion As Integer
    Property enlace As String
End Class
Public Class class_parameter_visualiza_documento
    Property id_imagen As Integer
    Property id_registro_publico As Integer
    Property id_usuario_registro_publico As Integer
    Property matricula As String
End Class
Public Class Class_ra_con_registros_publicos
    Public Structure ra_con_registros_publicos
        Dim id_registro_publico As Integer
        Dim system1_id_gabinete As Integer
        Dim nombre_registro As String
        Dim estado_registro As Integer
        Dim gabinete As String
    End Structure
    Function Consulta_lista_documentos_relacionados_matriculado(ByVal tipo_consulta As Object,
                                                                ByVal valor_consulta As String,
                                                                ByVal id_registro_publico As Integer,
                                                                ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                                ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita consulta de los documentos relacionados a un  
        '          matriculado
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic  : Retorna la estructura con los campos 
        ' y los registros de la consulta del matriculado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-10
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim Result As String = ""
            '----------/// Solicita estructura consunta registro publico 
            Dim Ra_con_registros_publicos As ra_con_registros_publicos = Nothing
            Result = Solicita_estructura_consulta_registro_publico(id_registro_publico,
                                                                   Ra_con_registros_publicos)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_matriculado = Result
                Exit Function
            End If
            ' --------- /// Solicita nombre gabinete consulta publica
            Dim Class_system1 As New Class_system1
            Dim gabinete As String = ""
            Result = Class_system1.SolicitaNombreGabinetePorId(Ra_con_registros_publicos.system1_id_gabinete,
                                                            gabinete)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_matriculado = Result
                Exit Function
            End If
            Dim Class_ra_con_campos_consulta_publica As New Class_ra_con_campos_consulta_publica
            ' --------- /// Solicita la estructura de los campos
            Result = Class_ra_con_campos_consulta_publica.Solicita_campos_lista_documentos_matricualdo(id_registro_publico,
                                                                                                       class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_matriculado = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim sql_consulta As String = ""
            ' --------- /// Solicita el comando SQL para realizar la consulta
            Result = ClassDaGabinete.Solicita_Sql_Consulta_lista_documentos_matricualdo(Class_config_general_service,
                                                                                        tipo_consulta,
                                                                                        valor_consulta,
                                                                                        gabinete,
                                                                                        class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic,
                                                                                        sql_consulta)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_matriculado = Result
                Exit Function
            End If
            Result = ClassDaGabinete.Solicita_row_documentos_matriculado_table_boot(sql_consulta,
                                                                                    class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_matriculado = Result
                Exit Function
            End If
            Consulta_lista_documentos_relacionados_matriculado = "YES"
            Exit Function
        Catch ex As Exception
            Consulta_lista_documentos_relacionados_matriculado = "Inconsistencia general funcion Consulta_lista_documentos_relacionados_matriculado " & ex.Message
        End Try
    End Function
    Function Load_visor_tiff_consulta_publica(ByVal id_imagen As Integer,
                                              ByVal gabinete As String,
                                              ByVal pag As Page,
                                              ByRef DropDownList_zom As DropDownList,
                                              ByRef UpdatePanelButon As UpdatePanel,
                                              ByRef Doc_actual As Integer,
                                              ByRef Matriz_documentos() As String) As String
        '--------------------------------------------------------------------------------------
        'Funcion : Carga el visor de documentos para archivos tif, bmp y jpg
        '         
        '--------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'gabinete                     : Representa el nombre del gabinete al que pertence la imagen
        '                               
        '                             : 
        '                             : 
        '-------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'Matriz_documentos : Retorna la matriz de documentos de visualizacion
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-10
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            HttpContext.Current.Session.Item("VER_IMAGE_TEMPORAL_EMERGENTE") = ""
            HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") = ""
            HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE") = "0"
            Dim Matri_dat_gabi() As String
            Erase Matri_dat_gabi
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Matri_Img_Temp() As String = Nothing
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     gabinete,
                                                                                     Matri_Img_Temp)
            If Result <> "YES" Then
                Load_visor_tiff_consulta_publica = Result
                Exit Function
            End If
            Dim i_conta As Integer = 0
            Matriz_documentos = Nothing
            For i As Integer = 1 To Matri_Img_Temp.Length - 1
                ReDim Preserve Matriz_documentos(i_conta)
                Matriz_documentos(i_conta) = Matri_Img_Temp(i)
                i_conta = i_conta + 1
            Next
            For i As Integer = 0 To Matriz_documentos.Length - 1
                If HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") = "" Then
                    HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") = Matriz_documentos(i)
                Else
                    HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") = HttpContext.Current.Session.Item("VER_MATRI_IMAGE_EMERGENTE") & "|" & Matriz_documentos(i)
                End If
            Next
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            Result = Class_ra_ver_version_documento.Show_visor_tif_version_documento(Matriz_documentos,
                                                                                     Doc_actual,
                                                                                     "inicio",
                                                                                     0,
                                                                                     pag,
                                                                                     HttpContext.Current.Session.Item("VER_DOC_ACTUAL_EMERGENTE"),
                                                                                     HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_EMERGENTE"),
                                                                                     HttpContext.Current.Session.Item("DA_IMAGE_WITH_EMERGENTE"),
                                                                                     HttpContext.Current.Session.Item("VER_IMAGE_HEIHG_SIZE_EMERGENTE"),
                                                                                     HttpContext.Current.Session.Item("VER_IMAGE_WITH_SIZE_EMERGENTE"),
                                                                                     DropDownList_zom,
                                                                                     UpdatePanelButon)
            If Result <> "YES" Then
                Load_visor_tiff_consulta_publica = Result
                Exit Function
            Else
                Load_visor_tiff_consulta_publica = "YES"
            End If
        Catch ex As Exception
            Load_visor_tiff_consulta_publica = "Inconsistencia general funcion Load_visor_tiff_consulta_publica " & ex.Message
        End Try
    End Function
    Function Lista_documento_consulta_publica_expediente(ByVal id_imagen As Integer,
                                                         ByVal id_registro_publico As Integer,
                                                         ByVal id_usuario_registro_pubico As Integer,
                                                         ByVal matricula As String,
                                                         ByRef class_stru_visor_migracion As class_stru_visor_migracion) As String
        '--------------------------------------------------------------------------------------
        'Funcion : Solicita el tipo de archivo a visualuizar y retorna la url de visualización
        '         
        '--------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'id_registro_publico          : Representa la identificación del registro publico
        '                               
        '                             : 
        '                             : 
        '-------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_visor_migracion : Retorna la estructura con los datos de visualizacion
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            '----------/// Solicita estructura consunta registro publico 
            Dim Ra_con_registros_publicos As ra_con_registros_publicos = Nothing
            Dim Result As String = Solicita_estructura_consulta_registro_publico(id_registro_publico,
                                                                                 Ra_con_registros_publicos)
            If Result <> "YES" Then
                Lista_documento_consulta_publica_expediente = Result
                Exit Function
            End If
            ' --------- /// Solicita nombre gabinete consulta publica
            Dim Class_system1 As New Class_system1
            Dim gabinete As String = ""
            Result = Class_system1.SolicitaNombreGabinetePorId(Ra_con_registros_publicos.system1_id_gabinete,
                                                            gabinete)
            If Result <> "YES" Then
                Lista_documento_consulta_publica_expediente = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim id_tipo_imagen As Integer = 0

            Result = ClassDaGabinete.SolicitaIdTipoImagen(id_imagen,
                                                            gabinete,
                                                            id_tipo_imagen)
            If Result <> "YES" Then
                Lista_documento_consulta_publica_expediente = Result
                Exit Function
            End If
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen,
                                                              class_stru_visor_migracion.tipo_file)
            If Result <> "YES" Then
                Lista_documento_consulta_publica_expediente = Result
                Exit Function
            End If
            If class_stru_visor_migracion.tipo_file = ".TIF" Or class_stru_visor_migracion.tipo_file = ".JPG" Or class_stru_visor_migracion.tipo_file = ".BMP" Then
                class_stru_visor_migracion.url_iframe = "../Docuarchi/WebFormDaVisorVersionPublico.aspx"
            Else
                class_stru_visor_migracion.url_iframe = "../Docuarchi/WebFormDaVisorExternoPublico.aspx"
            End If
            Dim Ra_con_log_consulta_publica As ra_con_log_consulta_publica = Nothing
            Ra_con_log_consulta_publica.id_imagen = id_imagen
            Ra_con_log_consulta_publica.ra_con_usuario_consulta_publica_id_registro_usuario = id_usuario_registro_pubico
            Ra_con_log_consulta_publica.matricula = matricula
            Ra_con_log_consulta_publica.ip_host = HttpContext.Current.Session.Item("ip_host_name")
            Ra_con_log_consulta_publica.operacion = "VISUALIZA"
            Ra_con_log_consulta_publica.gabinete = gabinete
            Dim Class_ra_con_log_consulta_publica As New Class_ra_con_log_consulta_publica
            Dim id_registro_log As Integer = 0
            Result = Class_ra_con_log_consulta_publica.Registro_log_consulta_publica_expediente(Ra_con_log_consulta_publica,
                                                                                                id_registro_log)
            If Result <> "YES" Then
                Lista_documento_consulta_publica_expediente = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DA_IMAGEN") = id_imagen
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = gabinete
            Lista_documento_consulta_publica_expediente = "YES"
        Catch ex As Exception
            Lista_documento_consulta_publica_expediente = "Inconsistencia general funcion Lista_documento_consulta_publica_expediente " & ex.Message
        End Try
    End Function
    Function Consulta_lista_documentos_relacionados_actos(ByVal tipo_consulta As Object,
                                                          ByVal valor_consulta As String,
                                                          ByVal libro As Integer,
                                                          ByVal inscripcion As Integer,
                                                          ByVal enlace As String,
                                                          ByVal id_registro_publico As Integer,
                                                          ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                          ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita consulta de los documentos relacionados a un acto del 
        '          matriculado
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic  : Retorna la estructura con los campos 
        ' y los registros de la consulta del matriculado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim Result As String = ""
            '----------/// Solicita estructura consunta registro publico 
            Dim Ra_con_registros_publicos As ra_con_registros_publicos = Nothing
            Result = Solicita_estructura_consulta_registro_publico(id_registro_publico,
                                                                   Ra_con_registros_publicos)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_actos = Result
                Exit Function
            End If
            ' --------- /// Solicita nombre gabinete consulta publica
            Dim Class_system1 As New Class_system1
            Dim gabinete As String = ""
            Result = Class_system1.SolicitaNombreGabinetePorId(Ra_con_registros_publicos.system1_id_gabinete,
                                                            gabinete)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_actos = Result
                Exit Function
            End If
            Dim Class_ra_con_campos_consulta_publica As New Class_ra_con_campos_consulta_publica
            ' --------- /// Solicita la estructura de los campos
            Result = Class_ra_con_campos_consulta_publica.Solicita_campos_lista_documentos_relacionados_actos(id_registro_publico,
                                                                                                              class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_actos = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim sql_consulta As String = ""
            ' --------- /// Solicita el comando SQL para realizar la consulta
            Result = ClassDaGabinete.Solicita_Sql_Consulta_lista_documentos_relacinados_actos(Class_config_general_service,
                                                                                              tipo_consulta,
                                                                                              valor_consulta,
                                                                                              libro,
                                                                                              inscripcion,
                                                                                              enlace,
                                                                                              gabinete,
                                                                                              class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic,
                                                                                              sql_consulta)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_actos = Result
                Exit Function
            End If
            Result = ClassDaGabinete.Solicita_row_table_boot_consulta_publica_actos_matriculado(sql_consulta,
                                                                                                class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Consulta_lista_documentos_relacionados_actos = Result
                Exit Function
            End If
            Consulta_lista_documentos_relacionados_actos = "YES"
            Exit Function
        Catch ex As Exception
            Consulta_lista_documentos_relacionados_actos = "Inconsistencia general funcion Consulta_lista_documentos_relacionados_actos " & ex.Message
        End Try
    End Function
    Function Consulta_lista_actos_expediente(ByVal tipo_consulta As Object,
                                             ByVal valor_consulta As String,
                                             ByVal id_registro_publico As Integer,
                                             ByVal Class_config_general_service As List(Of Class_config_general_service),
                                             ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita consulta de registros actos del matriculado
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic  : Retorna la estructura con los campos 
        ' y los registros de la consulta del matriculado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-08
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim Result As String = ""
            '----------/// Solicita estructura consunta registro publico 
            Dim Ra_con_registros_publicos As ra_con_registros_publicos = Nothing
            Result = Solicita_estructura_consulta_registro_publico(id_registro_publico,
                                                                   Ra_con_registros_publicos)
            If Result <> "YES" Then
                Consulta_lista_actos_expediente = Result
                Exit Function
            End If
            ' --------- /// Solicita nombre gabinete consulta publica
            Dim Class_system1 As New Class_system1
            Dim gabinete As String = ""
            Result = Class_system1.SolicitaNombreGabinetePorId(Ra_con_registros_publicos.system1_id_gabinete,
                                                            gabinete)
            If Result <> "YES" Then
                Consulta_lista_actos_expediente = Result
                Exit Function
            End If
            Dim Class_ra_con_campos_consulta_publica As New Class_ra_con_campos_consulta_publica
            ' --------- /// Solicita la estructura de los campos
            Result = Class_ra_con_campos_consulta_publica.Solicita_campos_lista_consulta_actos_matriculado_lista_bot(id_registro_publico,
                                                                                                                    class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Consulta_lista_actos_expediente = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim sql_consulta As String = ""
            ' --------- /// Solicita el comando SQL para realizar la consulta
            Result = ClassDaGabinete.Solicita_Sql_Consulta_actos_matriculado_gabinete(Class_config_general_service,
                                                                                      tipo_consulta,
                                                                                      valor_consulta,
                                                                                      gabinete,
                                                                                      class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic,
                                                                                      sql_consulta)
            If Result <> "YES" Then
                Consulta_lista_actos_expediente = Result
                Exit Function
            End If
            Result = ClassDaGabinete.Solicita_row_table_boot_consulta_publica_actos_matriculado(sql_consulta,
                                                                                                class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Consulta_lista_actos_expediente = Result
                Exit Function
            End If
            Consulta_lista_actos_expediente = "YES"
            Exit Function
        Catch ex As Exception
            Consulta_lista_actos_expediente = "Inconsistencia general funcion Consulta_lista_actos_expediente " & ex.Message
        End Try
    End Function
    Function Consulta_publica_matriculado_gabinete(ByVal tipo_consulta As Object,
                                                   ByVal valor_consulta As String,
                                                   ByVal id_registro_publico As Integer,
                                                   ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                   ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita consulta de registros del matriculado
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic  : Retorna la estructura con los campos 
        ' y los registros de la consulta del matriculado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim Result As String = ""
            '----------/// Solicita estructura consunta registro publico 
            Dim Ra_con_registros_publicos As ra_con_registros_publicos = Nothing
            Result = Solicita_estructura_consulta_registro_publico(id_registro_publico,
                                                                   Ra_con_registros_publicos)
            If Result <> "YES" Then
                Consulta_publica_matriculado_gabinete = Result
                Exit Function
            End If
            ' --------- /// Solicita nombre gabinete consulta publica
            Dim Class_system1 As New Class_system1
            Dim gabinete As String = ""
            Result = Class_system1.SolicitaNombreGabinetePorId(Ra_con_registros_publicos.system1_id_gabinete,
                                                            gabinete)
            If Result <> "YES" Then
                Consulta_publica_matriculado_gabinete = Result
                Exit Function
            End If
            Dim Class_ra_con_campos_consulta_publica As New Class_ra_con_campos_consulta_publica
            ' --------- /// Solicita la estructura de los campos
            Result = Class_ra_con_campos_consulta_publica.Solicita_campos_lista_consulta_matriculado_lista_bot(id_registro_publico,
                                                                                                               class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Consulta_publica_matriculado_gabinete = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim sql_consulta As String = ""
            ' --------- /// Solicita el comando SQL para realizar la consulta
            Result = ClassDaGabinete.Solicita_Sql_Consulta_publica_matriculado_gabinete(Class_config_general_service,
                                                                                        tipo_consulta,
                                                                                        valor_consulta,
                                                                                        gabinete,
                                                                                        class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic,
                                                                                        sql_consulta)
            If Result <> "YES" Then
                Consulta_publica_matriculado_gabinete = Result
                Exit Function
            End If
            Result = ClassDaGabinete.Solicita_row_table_boot_consulta_publica_matriculado(sql_consulta,
                                                                                          class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Consulta_publica_matriculado_gabinete = Result
                Exit Function
            End If
            Consulta_publica_matriculado_gabinete = "YES"
            Exit Function
        Catch ex As Exception
            Consulta_publica_matriculado_gabinete = "Inconsistencia general funcion Consulta_publica_matriculado_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_lista_tipo_consulta_publica(ByRef control_drow_lista As List(Of control_drow_lista)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita los tipos de consulta publica para expedientes
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '                             : 
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista tipos 
        '                     value: identificación del registro
        '                      text: Nombre del gabinete  
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  id_registro_publico,nombre_registro  from  ra_con_registros_publicos where estado_registro=1 order by nombre_registro"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_con_registros_publicos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_tipo_consulta_publica = " Función Solicita_lista_tipo_consulta_publica dice " & Result
                Exit Function
            End If
            Dim item As control_drow_lista
            item = New control_drow_lista
            item.value = "-1"
            item.text = "Seleccione el registro de consulta pública"
            control_drow_lista.Add(item)
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                Solicita_lista_tipo_consulta_publica = "YES"
                Exit Function
            Else
                Solicita_lista_tipo_consulta_publica = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_tipo_consulta_publica = "Inconsistencia general función Solicita_lista_tipo_consulta_publica " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_consulta_registro_publico(ByVal id_registro_publico As Integer,
                                                           ByRef Ra_con_registros_publicos As ra_con_registros_publicos) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita estructura del registro publico de consulta de expediente
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_publico   : Representa la identificación de registro piblico
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Ra_con_registros_publicos : Retorna la estructura de registro
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "select rcr.id_registro_publico,rcr.system1_id_gabinete,rcr.nombre_registro,rcr.estado_registro," &
                "sy.NOMBRE " &
                " from  ra_con_registros_publicos as rcr " &
                " inner join system1 as sy on (sy.id_gabinete=rcr.system1_id_gabinete) " &
                " where id_registro_publico=" & id_registro_publico
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_con_registros_publicos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_consulta_registro_publico = " Función Solicita_estructura_consulta_registro_publico dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_consulta_registro_publico = "Imposible encontrar los datos de caracterización del registro (" & id_registro_publico & ")"
                Exit Function
            Else
                Ra_con_registros_publicos.id_registro_publico = Datset.Tables(0).Rows(0).Item("id_registro_publico")
                Ra_con_registros_publicos.system1_id_gabinete = Datset.Tables(0).Rows(0).Item("system1_id_gabinete")
                Ra_con_registros_publicos.nombre_registro = Datset.Tables(0).Rows(0).Item("nombre_registro")
                Ra_con_registros_publicos.estado_registro = Datset.Tables(0).Rows(0).Item("estado_registro")
                Ra_con_registros_publicos.gabinete = Datset.Tables(0).Rows(0).Item("NOMBRE")
                Solicita_estructura_consulta_registro_publico = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_consulta_registro_publico = "Inconsistencia general funcion Solicita_estructura_consulta_registro_publico " & ex.Message
        End Try
    End Function
End Class
