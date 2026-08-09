Imports System.IO
Imports Newtonsoft.Json

Public Class class_stru_list_vew_migra_documento
    Public id_registro_migracion As Long
    Public ruta_documento As String
    Public url_ruta_documento As String
    Public Extension_doc_migrado As String
    Public Error_result As String
End Class
Public Structure stru_registro_migracion
    Dim id_registro_migracion As Long
    Dim system1_id_gabinete As Integer
    Dim fecha_registro As String
    Dim user_loguin As String
    Dim nombre_gabinete As String
    Dim id_imagen As Integer
    Dim aplica_ocr As Integer
    Dim aplica_compresion As Integer
    Dim version_pdf As String
    Dim ruta_documento As String
    Dim url_ruta_documento As String
    Dim id_registro_version_nuevo As Long
    Dim id_registro_version_anterior As Long
    Dim tipo_migracion As Integer
    Dim valor_campo_gabinete As String
    Dim nombre_archivo As String
    Dim estado_eliminado As Integer
    Dim fecha_registro_eliminado As String
    Dim id_usuario_gestion_elimina As Integer
    Dim full_index As String
    Dim Extension_doc_migrado As String
    Dim num_page_anterior As Integer
    Dim num_page_nuevo As Integer
    Dim leng_file As String
    Dim ESTADO_FIRMA_DIGITAL As Integer
    Dim fecha_registro_elimina_doc_fuente As String
    Dim id_usuario_gestion_elimina_doc_fuente As Integer
    Dim user_loguin_elimina_doc_fuente As String
End Structure
Public Class class_stru_registro_migracion
    Property id_registro_migracion As Long
    Property system1_id_gabinete As Integer
    Property fecha_registro As String
    Property user_loguin As String
    Property nombre_gabinete As String
    Property id_imagen As Integer
    Property aplica_ocr As Integer
    Property aplica_compresion As Integer
    Property version_pdf As String
    Property ruta_documento As String
    Property url_ruta_documento As String
    Property id_registro_version_nuevo As Long
    Property id_registro_version_anterior As Long
    Property tipo_migracion As Integer
    Property valor_campo_gabinete As String
    Property nombre_archivo As String
    Property estado_eliminado As Integer
    Property fecha_registro_eliminado As String
    Property id_usuario_gestion_elimina As Integer
    Property full_index As String
    Property Extension_doc_migrado As String
    Property num_page_anterior As Integer
    Property num_page_nuevo As Integer
    Property leng_file As String
    Property ESTADO_FIRMA_DIGITAL As Integer
    Property Error_result As String
    Property fecha_registro_elimina_doc_fuente As String
    Property id_usuario_gestion_elimina_doc_fuente As Integer
    Property user_loguin_elimina_doc_fuente As String
End Class
Public Class Class_ra_mig_registro_migracion
    Function Solicita_datos_auto_complete_registro_migracion(ByVal name_dbs_auto As String,
                                                             ByVal value_auto As String,
                                                             ByRef country As List(Of String)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura con los registro de auto 
        '          de auto complete de un registro de migración
        '         
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'name_dbs_auto        : Representa el nombre del dbs de conexion a basde de datos             
        'value                : Representa el valor de consulta sobre la tabla
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'country              : Retorna la estructura con los registros
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-08-31
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim ref As Object
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            country = New List(Of String)()
            Dim class_campos_table_bostra_table As List(Of class_campos_table_bostra_table) = Nothing
            Result = Me.Solicita_campos_lista_consulta_documentos_migracion(class_campos_table_bostra_table)
            If Result <> "YES" Then
                Solicita_datos_auto_complete_registro_migracion = Result
                Exit Function
            End If
            Dim seleccampos As String = "Select "
            Dim campo_clase_documento As String = ""
            Dim campo_expediente As String = ""
            For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                If class_campos_table_bostra_table.Item(i).visible_like_sql = 1 Then
                    If seleccampos = "Select " Then
                        seleccampos = seleccampos & class_campos_table_bostra_table(i).field
                    Else
                        seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
                    End If
                End If
            Next
            Dim condicionsql As String = " where "
            Dim likeigual As String = " Like"
            For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                If class_campos_table_bostra_table.Item(i).visible_like_sql = 1 Then
                    If condicionsql = " where " Then
                        condicionsql = condicionsql & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & value_auto & "%'"
                    Else
                        condicionsql = condicionsql & " or " & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & value_auto & "%'"
                    End If
                End If
            Next

            If name_dbs_auto = "WF" Then
                ref = New conect.Dbase_Conction_Mysql
            Else
                ref = New conect.Dbase_Conction_Mysql_RA
            End If
            Dim order_colum As String = "DESC"
            Dim colum_order_name As String = "id_registro_migracion"
            Dim sqlfrom As String = " From ra_mig_registro_migracion "
            Sql_consulta = seleccampos & " " & sqlfrom & " " & condicionsql & " AND id_registro_version_nuevo <> 0 Order by " & colum_order_name & " " & order_colum & " LIMIT 50"
            Dim Datset As DataSet = New DataSet("ra_mig_registro_migracion")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_auto_complete_registro_migracion = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                country = Nothing
                Solicita_datos_auto_complete_registro_migracion = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                            If Datset.Tables(0).Rows(i).IsNull(z) = False Then
                                Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(z).GetType.ToString
                                Dim estado_exit As String = "NO"
                                If obsgetipe = "System.DateTime" Then
                                    Dim subtrin As String = Datset.Tables(0).Rows(i).Item(z).ToString()
                                    Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                                    Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
                                    ClassDaGabinete.Veri_existe_regitro(country,
                                                                        tempo_fecha,
                                                                        estado_exit)
                                    If estado_exit = "NO" Then
                                        country.Add(tempo_fecha)
                                    End If
                                Else
                                    ClassDaGabinete.Veri_existe_regitro(country,
                                                                        Datset.Tables(0).Rows(i).Item(z).ToString(),
                                                                        estado_exit)
                                    If estado_exit = "NO" Then
                                        country.Add(Datset.Tables(0).Rows(i).Item(z).ToString())
                                    End If

                                End If
                            End If
                        Next
                    Next
                End If
                Solicita_datos_auto_complete_registro_migracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_auto_complete_registro_migracion = "Inconsistencia general funcion Solicita_datos_auto_complete_registro_migracion " & ex.Message
        End Try
    End Function
    Function Elimina_documento_migrado(ByVal id_registro_migracion As Long) As String
        '---------------------------------------------------------------------------
        'Funcion : Elimina la versión del documento fuente de migrado
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_migracion : Representa la identiifcación del registro de migración
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Stru_registro_migracion As stru_registro_migracion = Nothing
            Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
            Result = Me.Solicita_estructura_registro_migracion_documento(id_registro_migracion,
                                                                         Stru_registro_migracion)
            If Result <> "YES" Then
                Elimina_documento_migrado = Result
                Exit Function
            End If
            If Stru_registro_migracion.id_registro_version_anterior = 0 Then
                Elimina_documento_migrado = "El registro de migración no contiene imagen fuente de migración"
                Exit Function
            End If
            Result = Class_ra_ver_version_documento.Elimina_version_documento(Stru_registro_migracion.id_registro_version_anterior,
                                                                              7,
                                                                              HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                              HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION"),
                                                                              1,
                                                                              id_registro_migracion,
                                                                              0)
            If Result <> "YES" Then
                Elimina_documento_migrado = Result
                Exit Function
            End If
            Elimina_documento_migrado = "YES"
            Exit Function
        Catch ex As Exception
            Elimina_documento_migrado = "Inconsistencia general funcion Elimina_documento_migrado " & ex.Message
        End Try
    End Function
    Function Consulta_registro_migracion(ByVal tipo_consulta As Object,
                                         ByVal valor_consulta As String,
                                         ByVal Class_config_general_service As List(Of Class_config_general_service),
                                         ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita consulta de registros de migración
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
        ' y los registros de la consulta de migracion
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim Result As String = ""
            ' --------- /// Solicita la estructura de los campos
            Result = Solicita_campos_lista_consulta_documentos_migracion(class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Consulta_registro_migracion = Result
                Exit Function
            End If
            Dim sql_consulta As String = ""
            ' --------- /// Solicita el comando SQL para realizar la consulta
            Result = Solicita_Sql_Consulta_registro_migracion(Class_config_general_service,
                                                              tipo_consulta,
                                                              valor_consulta,
                                                             "ra_mig_registro_migracion",
                                                              class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic,
                                                              sql_consulta)
            If Result <> "YES" Then
                Consulta_registro_migracion = Result
                Exit Function
            End If
            Result = Solicita_structura_consulta_documentos_migrados(sql_consulta,
                                                                     class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Consulta_registro_migracion = Result
                Exit Function
            End If
            Consulta_registro_migracion = "YES"
            Exit Function
        Catch ex As Exception
            Consulta_registro_migracion = "Inconsistencia general funcion Consulta_registro_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_campos_lista_consulta_documentos_migracion(ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          resultados de la consulta de registro de migración
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.field = "operate"
            item.title = "OPTION OPERATION"
            item.checkbox = False
            item.visible = True
            item.viisble_sql = 0
            item.clickToSelect = False
            item.visible_like_sql = 0
            item.align = "center"
            item.events = "window.operateEvents"
            item.formatter = "operateFormattertablebootmig"
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "id_registro_migracion"
            item.field = "id_registro_migracion"
            item.visible = False
            item.viisble_sql = 1
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "FECHA REGISTRO"
            item.field = "FECHA_REGISTRO"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "GABINETE"
            item.field = "nombre_gabinete"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "VERSION ANTERIOR"
            item.field = "id_registro_version_anterior"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)

            item = New class_campos_table_bostra_table
            item.title = "VERSION NUEVA"
            item.field = "id_registro_version_nuevo"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)

            item = New class_campos_table_bostra_table
            item.title = "IMAGEN"
            item.field = "id_imagen"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "REGISTRO"
            item.field = "valor_campo_gabinete"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.title = "USUARIO"
            item.field = "user_loguin"
            item.visible = True
            item.viisble_sql = 1
            item.visible_like_sql = 1
            class_campos_table_bostra_table.Add(item)
            Solicita_campos_lista_consulta_documentos_migracion = "YES"
        Catch ex As Exception
            Solicita_campos_lista_consulta_documentos_migracion = "inconsistencia general funcion Solicita_campos_lista_consulta_documentos_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_Sql_Consulta_registro_migracion(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                      ByVal tipo_consulta As Integer,
                                                      ByVal valor_consulta As String,
                                                      ByVal table As String,
                                                      ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                                      ByRef consulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta REGISTRO de migracion
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
        'consulta  : Retorna comando sql de consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim condicionsql As String = " where "
            Dim seleccampos As String = "Select "
            Dim campo_clase_documento As String = ""
            Dim campo_expediente As String = ""
            For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                If class_campos_table_bostra_table(i).viisble_sql = "1" Then
                    If seleccampos = "Select " Then
                        seleccampos = seleccampos & class_campos_table_bostra_table(i).field
                    Else
                        seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
                    End If
                End If
            Next
            If tipo_consulta = 2 Then
                Dim likeigual As String = " Like"
                For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                    If class_campos_table_bostra_table.Item(i).visible_like_sql = 1 Then
                        If condicionsql = " where " Then
                            condicionsql = condicionsql & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & valor_consulta & "%'"
                        Else
                            condicionsql = condicionsql & " or " & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & valor_consulta & "%'"
                        End If
                    End If
                Next
            End If
            If tipo_consulta = 1 Then
                For i As Integer = 0 To Class_config_general_service.Count - 1
                    Dim campo_plantilla As String = "da." & Class_config_general_service.Item(i).name_campo
                    If Class_config_general_service.Item(i).tipo_campo = "DATE" Or Class_config_general_service.Item(i).tipo_campo = "INT" Then
                        'caso between
                        If Class_config_general_service.Item(i).value_campo <> "" And Class_config_general_service.Item(i).value_campo_beetwen <> "" Then
                            If Class_config_general_service.Item(i).tipo_campo = "DATE" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & " CAST(" & campo_plantilla & " AS DATE) " & " between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                Else
                                    condicionsql = condicionsql & " and " & " CAST(" & campo_plantilla & " AS DATE) " & "  between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                End If
                            Else
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & " between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "  between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                End If
                            End If
                        Else
                            'Caso primer campo
                            If Class_config_general_service.Item(i).value_campo <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo & "'"
                                End If
                            End If
                            'Caso segundo campo
                            If Class_config_general_service.Item(i).value_campo_beetwen <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                                End If
                            End If
                        End If
                    Else
                        'Caso primer campo
                        If Class_config_general_service.Item(i).value_campo <> "" Then
                            If condicionsql = " where " Then
                                condicionsql = condicionsql & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo & "'"
                            Else
                                condicionsql = condicionsql & " and " & campo_plantilla & "='" & Class_config_general_service.Item(i).value_campo & "'"
                            End If
                        End If
                    End If
                Next
            End If
            Dim order_colum As String = "ASC"
            Dim colum_order_name As String = "id_registro_migracion"
            Dim sqlfrom As String = " From " & table & " as da"
            consulta = seleccampos & " " & sqlfrom & " " & condicionsql & " and (id_registro_version_nuevo is not  null)  " & " ORDER BY " & colum_order_name & " " & order_colum & " LIMIT 5000"
            Solicita_Sql_Consulta_registro_migracion = "YES"
        Catch ex As Exception
            Solicita_Sql_Consulta_registro_migracion = "Inconsistencia general funcion Solicita_Sql_Consulta_registro_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_structura_consulta_documentos_migrados(ByVal consulta As String,
                                                             ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         de registros migrados
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'consulta               : Representa la consulta en comando SQL
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'stru_row_gabinete_generic  : Retorna la estructura de datos de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("ra_mig_registro_migracion")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                Solicita_structura_consulta_documentos_migrados = "Funcion  Solicita_structura_consulta_documentos_migrados " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_structura_consulta_documentos_migrados = "YES"
        Catch ex As Exception
            Solicita_structura_consulta_documentos_migrados = "Inconsistencia general fucnion Solicita_structura_consulta_documentos_migrados " & ex.Message
        End Try
    End Function
    Function Solicita_campos_consulta_documentos_migracion(ByVal name_espace_form_control As String,
                                                           ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de los campos de consulta de los registros
        '          de migracion de documentos
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'name_espace_form_control : Representa el nombre del esppacio de nombres
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Retorna la estructura de los campos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
            parameter_gestion.aleas_campo = "IDENTIFICADOR DE REGISTRO"
            parameter_gestion.name_campo = "id_registro_migracion"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "INT"
            parameter_gestion.max_leng_campo = 10
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = -1
            parameter_gestion.tbl_control = "ra_mig_registro_migracion"
            parameter_gestion.clas_service_control = "WebService_control_general.asmx"
            parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)

            parameter_gestion = New Class_config_general_service()
            parameter_gestion.aleas_campo = "FECHA REGISTRO"
            parameter_gestion.name_campo = "fecha_registro"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 0
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "DATE"
            parameter_gestion.max_leng_campo = 12
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = -1
            parameter_gestion.tbl_control = "ra_mig_registro_migracion"
            parameter_gestion.clas_service_control = "WebService_control_general.asmx"
            parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
            parameter_gestion.error_gestion = "YES"
            parameter_gestion.disable_campo = 1
            Class_config_general_service.Add(parameter_gestion)

            parameter_gestion = New Class_config_general_service()
            parameter_gestion.aleas_campo = "VALOR REGISTRO"
            parameter_gestion.name_campo = "valor_campo_gabinete"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 300
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = -1
            parameter_gestion.tbl_control = "ra_mig_registro_migracion"
            parameter_gestion.clas_service_control = "WebService_control_general.asmx"
            parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)


            parameter_gestion = New Class_config_general_service()
            parameter_gestion.aleas_campo = "IMAGEN"
            parameter_gestion.name_campo = "id_imagen"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "INT"
            parameter_gestion.max_leng_campo = 10
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = -1
            parameter_gestion.tbl_control = "ra_mig_registro_migracion"
            parameter_gestion.clas_service_control = "WebService_control_general.asmx"
            parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)


            parameter_gestion = New Class_config_general_service()
            parameter_gestion.aleas_campo = "GABINETE"
            parameter_gestion.name_campo = "nombre_gabinete"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 300
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = -1
            parameter_gestion.tbl_control = "ra_mig_registro_migracion"
            parameter_gestion.clas_service_control = "WebService_control_general.asmx"
            parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)


            parameter_gestion = New Class_config_general_service()
            parameter_gestion.aleas_campo = "USUARIO"
            parameter_gestion.name_campo = "user_loguin"
            parameter_gestion.alow_null = 0
            parameter_gestion.alow_tipo_value = 1
            parameter_gestion.campo_tip = 1
            parameter_gestion.value_campo = ""
            parameter_gestion.disable_campo = 1
            parameter_gestion.control_tip_correo = 0
            parameter_gestion.tipo_campo = "VARCHAR"
            parameter_gestion.max_leng_campo = 300
            parameter_gestion.name_space_campo = name_espace_form_control
            parameter_gestion.dbms_control = "DA"
            parameter_gestion.dms_id_registro = -1
            parameter_gestion.tbl_control = "ra_mig_registro_migracion"
            parameter_gestion.clas_service_control = "WebService_control_general.asmx"
            parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
            parameter_gestion.error_gestion = "YES"
            Class_config_general_service.Add(parameter_gestion)
            Solicita_campos_consulta_documentos_migracion = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_campos_consulta_documentos_migracion = "Inconsistencia general funcion Solicita_campos_consulta_documentos_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_registro_migracion_documento(ByVal id_registro_migracion As Long,
                                                              ByRef Stru_registro_migracion As stru_registro_migracion) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura del registro del documento migrado con la
        '          identificacion del registro de migración
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_migracion : Representa la identificación del registro de migra
        '                        ción
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Stru_registro_migracion : Retorna la estructura del registro del documento
        '                                      migrado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-02
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Sql_consulta = "SELECT id_registro_migracion,system1_id_gabinete,fecha_registro,user_loguin," &
                "nombre_gabinete,id_imagen,aplica_ocr,aplica_compresion,version_pdf,ruta_documento,url_ruta_documento," &
                "id_registro_version_nuevo,id_registro_version_anterior,tipo_migracion,valor_campo_gabinete," &
                "nombre_archivo,estado_eliminado,fecha_registro_eliminado,id_usuario_gestion_elimina,full_index,Extension_doc_migrado," &
                "num_page_anterior,num_page_nuevo,leng_file,ESTADO_FIRMA_DIGITAL," &
                "fecha_registro_elimina_doc_fuente,id_usuario_gestion_elimina_doc_fuente,user_loguin_elimina_doc_fuente" &
               " from ra_mig_registro_migracion where id_registro_migracion=" & id_registro_migracion
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_mig_registro_migracion")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_registro_migracion_documento = "Error de conexión funcion Solicita_estructura_registro_migracion_documento " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_registro_migracion_documento = "Imposible encontar el registro de migracion del indetificador (" & id_registro_migracion & ")"
                Exit Function
            Else
                Stru_registro_migracion.id_registro_migracion = Datset.Tables(0).Rows(0).Item(0)
                Stru_registro_migracion.system1_id_gabinete = Datset.Tables(0).Rows(0).Item(1)
                Stru_registro_migracion.fecha_registro = Datset.Tables(0).Rows(0).Item(2)
                Stru_registro_migracion.user_loguin = Datset.Tables(0).Rows(0).Item(3)
                Stru_registro_migracion.nombre_gabinete = Datset.Tables(0).Rows(0).Item(4)
                Stru_registro_migracion.id_imagen = Datset.Tables(0).Rows(0).Item(5)
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    Stru_registro_migracion.aplica_ocr = 0
                Else
                    Stru_registro_migracion.aplica_ocr = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    Stru_registro_migracion.aplica_compresion = 0
                Else
                    Stru_registro_migracion.aplica_compresion = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    Stru_registro_migracion.version_pdf = ""
                Else
                    Stru_registro_migracion.version_pdf = Datset.Tables(0).Rows(0).Item(8)
                End If
                Stru_registro_migracion.ruta_documento = Datset.Tables(0).Rows(0).Item(9)
                Stru_registro_migracion.url_ruta_documento = Datset.Tables(0).Rows(0).Item(10)
                If Datset.Tables(0).Rows(0).IsNull(11) Then
                    Stru_registro_migracion.id_registro_version_nuevo = 0
                Else
                    Stru_registro_migracion.id_registro_version_nuevo = Datset.Tables(0).Rows(0).Item(11)
                End If
                If Datset.Tables(0).Rows(0).IsNull(12) Then
                    Stru_registro_migracion.id_registro_version_anterior = 0
                Else
                    Stru_registro_migracion.id_registro_version_anterior = Datset.Tables(0).Rows(0).Item(12)
                End If
                Stru_registro_migracion.tipo_migracion = Datset.Tables(0).Rows(0).Item(13)
                If Datset.Tables(0).Rows(0).IsNull(14) Then
                    Stru_registro_migracion.valor_campo_gabinete = ""
                Else
                    Stru_registro_migracion.valor_campo_gabinete = Datset.Tables(0).Rows(0).Item(14)
                End If
                Stru_registro_migracion.nombre_archivo = Datset.Tables(0).Rows(0).Item(15)
                Stru_registro_migracion.estado_eliminado = Datset.Tables(0).Rows(0).Item(16)
                If Datset.Tables(0).Rows(0).IsNull(17) Then
                    Stru_registro_migracion.fecha_registro_eliminado = ""
                Else
                    Stru_registro_migracion.fecha_registro_eliminado = Datset.Tables(0).Rows(0).Item(17)
                End If
                Stru_registro_migracion.id_usuario_gestion_elimina = Datset.Tables(0).Rows(0).Item(18)
                If Datset.Tables(0).Rows(0).IsNull(19) Then
                    Stru_registro_migracion.full_index = ""
                Else
                    Stru_registro_migracion.full_index = Datset.Tables(0).Rows(0).Item(19)
                End If
                Stru_registro_migracion.Extension_doc_migrado = Datset.Tables(0).Rows(0).Item(20)
                Stru_registro_migracion.num_page_anterior = Datset.Tables(0).Rows(0).Item(21)
                Stru_registro_migracion.num_page_nuevo = Datset.Tables(0).Rows(0).Item(22)
                Stru_registro_migracion.leng_file = Datset.Tables(0).Rows(0).Item(23)
                Stru_registro_migracion.ESTADO_FIRMA_DIGITAL = Datset.Tables(0).Rows(0).Item(24)
                If Datset.Tables(0).Rows(0).IsNull(25) Then
                    Stru_registro_migracion.fecha_registro_elimina_doc_fuente = ""
                Else
                    Stru_registro_migracion.fecha_registro_elimina_doc_fuente = Datset.Tables(0).Rows(0).Item(25)
                End If
                If Datset.Tables(0).Rows(0).IsNull(26) Then
                    Stru_registro_migracion.id_usuario_gestion_elimina_doc_fuente = 0
                Else
                    Stru_registro_migracion.id_usuario_gestion_elimina_doc_fuente = Datset.Tables(0).Rows(0).Item(26)
                End If
                If Datset.Tables(0).Rows(0).IsNull(27) Then
                    Stru_registro_migracion.user_loguin_elimina_doc_fuente = ""
                Else
                    Stru_registro_migracion.user_loguin_elimina_doc_fuente = Datset.Tables(0).Rows(0).Item(27)
                End If
                Solicita_estructura_registro_migracion_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_registro_migracion_documento = "Inconsistencia general funcion Solicita_estructura_registro_migracion_documento " & ex.Message
        End Try
    End Function
    Function Solicita_clase_datos_registro_migracion_documento(ByVal id_registro_migracion As Long,
                                                               ByRef Stru_registro_migracion As class_stru_registro_migracion) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura del registro del documento migrado con la
        '          identificacion del registro de migración
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_registro_migracion : Representa la identificación del registro de migra
        '                        ción
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Stru_registro_migracion : Retorna la estructura del registro del documento
        '                                      migrado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-02
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Sql_consulta = "SELECT id_registro_migracion,system1_id_gabinete,fecha_registro,user_loguin," &
                "nombre_gabinete,id_imagen,aplica_ocr,aplica_compresion,version_pdf,ruta_documento,url_ruta_documento," &
                "id_registro_version_nuevo,id_registro_version_anterior,tipo_migracion,valor_campo_gabinete," &
                "nombre_archivo,estado_eliminado,fecha_registro_eliminado,id_usuario_gestion_elimina,full_index,Extension_doc_migrado," &
                "num_page_anterior,num_page_nuevo,leng_file,ESTADO_FIRMA_DIGITAL," &
                "fecha_registro_elimina_doc_fuente,id_usuario_gestion_elimina_doc_fuente,user_loguin_elimina_doc_fuente" &
               " from ra_mig_registro_migracion where id_registro_migracion=" & id_registro_migracion
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_mig_registro_migracion")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_clase_datos_registro_migracion_documento = "Error de conexión funcion Solicita_clase_datos_registro_migracion_documento " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_clase_datos_registro_migracion_documento = "Imposible encontar el registro de migracion del indetificador (" & id_registro_migracion & ")"
                Exit Function
            Else
                Stru_registro_migracion.id_registro_migracion = Datset.Tables(0).Rows(0).Item(0)
                Stru_registro_migracion.system1_id_gabinete = Datset.Tables(0).Rows(0).Item(1)
                Stru_registro_migracion.fecha_registro = Datset.Tables(0).Rows(0).Item(2)
                Stru_registro_migracion.user_loguin = Datset.Tables(0).Rows(0).Item(3)
                Stru_registro_migracion.nombre_gabinete = Datset.Tables(0).Rows(0).Item(4)
                Stru_registro_migracion.id_imagen = Datset.Tables(0).Rows(0).Item(5)
                If Datset.Tables(0).Rows(0).IsNull(6) Then
                    Stru_registro_migracion.aplica_ocr = 0
                Else
                    Stru_registro_migracion.aplica_ocr = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) Then
                    Stru_registro_migracion.aplica_compresion = 0
                Else
                    Stru_registro_migracion.aplica_compresion = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) Then
                    Stru_registro_migracion.version_pdf = ""
                Else
                    Stru_registro_migracion.version_pdf = Datset.Tables(0).Rows(0).Item(8)
                End If
                Stru_registro_migracion.ruta_documento = Datset.Tables(0).Rows(0).Item(9)
                Stru_registro_migracion.url_ruta_documento = Datset.Tables(0).Rows(0).Item(10)
                If Datset.Tables(0).Rows(0).IsNull(11) Then
                    Stru_registro_migracion.id_registro_version_nuevo = 0
                Else
                    Stru_registro_migracion.id_registro_version_nuevo = Datset.Tables(0).Rows(0).Item(11)
                End If
                If Datset.Tables(0).Rows(0).IsNull(12) Then
                    Stru_registro_migracion.id_registro_version_anterior = 0
                Else
                    Stru_registro_migracion.id_registro_version_anterior = Datset.Tables(0).Rows(0).Item(12)
                End If
                Stru_registro_migracion.tipo_migracion = Datset.Tables(0).Rows(0).Item(13)
                If Datset.Tables(0).Rows(0).IsNull(14) Then
                    Stru_registro_migracion.valor_campo_gabinete = ""
                Else
                    Stru_registro_migracion.valor_campo_gabinete = Datset.Tables(0).Rows(0).Item(14)
                End If
                Stru_registro_migracion.nombre_archivo = Datset.Tables(0).Rows(0).Item(15)
                Stru_registro_migracion.estado_eliminado = Datset.Tables(0).Rows(0).Item(16)
                If Datset.Tables(0).Rows(0).IsNull(17) Then
                    Stru_registro_migracion.fecha_registro_eliminado = ""
                Else
                    Stru_registro_migracion.fecha_registro_eliminado = Datset.Tables(0).Rows(0).Item(17)
                End If
                Stru_registro_migracion.id_usuario_gestion_elimina = Datset.Tables(0).Rows(0).Item(18)
                If Datset.Tables(0).Rows(0).IsNull(19) Then
                    Stru_registro_migracion.full_index = ""
                Else
                    Stru_registro_migracion.full_index = Datset.Tables(0).Rows(0).Item(19)
                End If
                Stru_registro_migracion.Extension_doc_migrado = Datset.Tables(0).Rows(0).Item(20)
                Stru_registro_migracion.num_page_anterior = Datset.Tables(0).Rows(0).Item(21)
                Stru_registro_migracion.num_page_nuevo = Datset.Tables(0).Rows(0).Item(22)
                Stru_registro_migracion.leng_file = Datset.Tables(0).Rows(0).Item(23)
                Stru_registro_migracion.ESTADO_FIRMA_DIGITAL = Datset.Tables(0).Rows(0).Item(24)
                If Datset.Tables(0).Rows(0).IsNull(25) Then
                    Stru_registro_migracion.fecha_registro_elimina_doc_fuente = ""
                Else
                    Stru_registro_migracion.fecha_registro_elimina_doc_fuente = Datset.Tables(0).Rows(0).Item(25)
                End If
                If Datset.Tables(0).Rows(0).IsNull(26) Then
                    Stru_registro_migracion.id_usuario_gestion_elimina_doc_fuente = 0
                Else
                    Stru_registro_migracion.id_usuario_gestion_elimina_doc_fuente = Datset.Tables(0).Rows(0).Item(26)
                End If
                If Datset.Tables(0).Rows(0).IsNull(27) Then
                    Stru_registro_migracion.user_loguin_elimina_doc_fuente = ""
                Else
                    Stru_registro_migracion.user_loguin_elimina_doc_fuente = Datset.Tables(0).Rows(0).Item(27)
                End If
                Solicita_clase_datos_registro_migracion_documento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_clase_datos_registro_migracion_documento = "Inconsistencia general funcion Solicita_clase_datos_registro_migracion_documento " & ex.Message
        End Try
    End Function
    Function Solicita_id_registro_migracion_imagen(ByVal id_imagen As Integer,
                                                   ByVal gabinete As String,
                                                   ByRef id_registro_migracion As Long) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el registro de migracion de un documento
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_registro_migracion : Retorna la estructura del registro de migracion
        '                                      
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-14
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Sql_consulta = "SELECT id_registro_migracion from ra_mig_registro_migracion " &
            " where id_imagen=" & id_imagen & " and nombre_gabinete='" & gabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_mig_registro_migracion")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_registro_migracion_imagen = "Error de conexión Solicita_id_registro_migracion_imagen " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_registro_migracion_imagen = "Documento sin registro de migración"
                Exit Function
            Else
                id_registro_migracion = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_registro_migracion_imagen = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_registro_migracion_imagen = "Inconsistencia general funcion Solicita_id_registro_migracion_imagen " & ex.Message
        End Try
    End Function
    Function Solicita_estructura_lista_documento_migrado(ByVal id_imagen As Integer,
                                                         ByVal gabinete As String,
                                                         ByRef class_stru_list_vew_migra_documento As class_stru_list_vew_migra_documento) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de visualización del documento migrado 
        '          de formato
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : Retorna la estructura del documento
        '                                      migrado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Sql_consulta = "SELECT id_registro_migracion,ruta_documento,url_ruta_documento,Extension_doc_migrado from ra_mig_registro_migracion " &
            " where id_imagen=" & id_imagen & " and nombre_gabinete='" & gabinete & "' and estado_eliminado=0"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_mig_registro_migracion")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_lista_documento_migrado = "Error de conexión funcion Solicita_estructura_lista_documento_migrado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                class_stru_list_vew_migra_documento.id_registro_migracion = -1
                class_stru_list_vew_migra_documento.url_ruta_documento = ""
                class_stru_list_vew_migra_documento.ruta_documento = ""
                class_stru_list_vew_migra_documento.Extension_doc_migrado = ""
                Solicita_estructura_lista_documento_migrado = "YES"
                Exit Function
            Else
                class_stru_list_vew_migra_documento.id_registro_migracion = Datset.Tables(0).Rows(0).Item(0)
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    class_stru_list_vew_migra_documento.ruta_documento = ""
                Else
                    class_stru_list_vew_migra_documento.ruta_documento = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    class_stru_list_vew_migra_documento.url_ruta_documento = ""
                Else
                    class_stru_list_vew_migra_documento.url_ruta_documento = "../workflow/Handler_image_wf.ashx?rut_image=" & Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    class_stru_list_vew_migra_documento.Extension_doc_migrado = ""
                Else
                    class_stru_list_vew_migra_documento.Extension_doc_migrado = Datset.Tables(0).Rows(0).Item(2)
                End If
                Solicita_estructura_lista_documento_migrado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_lista_documento_migrado = "Inconsistencia general funcion Solicita_estructura_lista_documento_migrado " & ex.Message
        End Try
    End Function
    Function Migra_formato_documento(ByVal id_imagen As Integer,
                                     ByVal gabinete As String,
                                     ByVal id_usuario_gestion As Integer,
                                     ByVal loguin_usuario_gestion As String,
                                     ByRef class_stru_list_vew_migra_documento As class_stru_list_vew_migra_documento) As String
        '---------------------------------------------------------------------------
        'Funcion : Migra formato de documento con la identificación del documento y
        'el nombre del gabinete
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : Retorna la estructura del documento
        '                                      migrado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-22
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim matri_documentos() As String = Nothing
            Dim Class_ra_mig_config_migracion As New Class_ra_mig_config_migracion
            Dim stru_ra_mig_config_migracion As stru_ra_mig_config_migracion = Nothing
            Result = Class_ra_mig_config_migracion.Solicita_estructura_parametro_migracion(stru_ra_mig_config_migracion)
            If Result <> "YES" Then
                Migra_formato_documento = Result
                Exit Function
            End If
            Dim id_tipo_imagen As Integer = 0
            Result = ClassDaGabinete.SolicitaIdTipoImagen(id_imagen,
                                                            gabinete,
                                                            id_tipo_imagen)
            If Result <> "YES" Then
                Migra_formato_documento = Result
                Exit Function
            End If
            Dim tipo_file As String = ""
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen,
                                                              tipo_file)
            If Result <> "YES" Then
                Migra_formato_documento = Result
                Exit Function
            End If
            If InStr(UCase(tipo_file), UCase(stru_ra_mig_config_migracion.formato_archivo_permitido)) <= 0 Then
                Migra_formato_documento = "El formato (" & UCase(tipo_file) & ") no esta permitodo para migración"
                Exit Function
            End If
            Dim Ruta_documento_migrado As String = ""
            Dim Ruta_url_documento_migrado As String = ""
            Dim ruta_temporal_migracion As String = stru_ra_mig_config_migracion.ruta_temporal_migracion.Replace("/", "\")
            Result = Solicita_ruta_temporal_migracion(id_imagen,
                                                      gabinete,
                                                      ruta_temporal_migracion,
                                                      stru_ra_mig_config_migracion.formato_permitido_migracion,
                                                      Ruta_documento_migrado,
                                                      Ruta_url_documento_migrado)
            If Result <> "YES" Then
                Migra_formato_documento = Result
                Exit Function
            End If
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     gabinete,
                                                                                     matri_documentos)
            If Result <> "YES" Then
                Migra_formato_documento = Result
                Exit Function
            End If
            Dim ref_matri_documento() As String = Nothing
            Dim Icont As Integer = 0
            For i As Integer = 1 To matri_documentos.Length - 1
                ReDim Preserve ref_matri_documento(Icont)
                ref_matri_documento(Icont) = matri_documentos(i)
                Icont = Icont + 1
            Next
            Dim leng_file As String = ""
            Dim Class_fyle_system As New Class_fyle_system
            Result = Class_fyle_system.Solicita_peso_matriz_documentos(ref_matri_documento,
                                                                       leng_file)
            If Result <> "YES" Then
                Migra_formato_documento = Result
                Exit Function
            End If
            Dim Class_ItexShare As New Class_ItexShare
            Dim num_page As Integer = 0
            Result = Class_ItexShare.Migra_formato_a_pdf(ref_matri_documento,
                                                        Ruta_documento_migrado,
                                                        "NO",
                                                        stru_ra_mig_config_migracion.version_formato_migracion_pdf,
                                                        num_page)
            If Result <> "YES" Then
                Migra_formato_documento = Result
                Exit Function
            End If
            Dim id_registro_migracion As Long = 0
            Result = Solicita_exitencia_registro_migracion(id_imagen,
                                                           gabinete,
                                                           id_registro_migracion)
            If Result <> "YES" Then
                Migra_formato_documento = Result
                Exit Function
            End If
            Dim file_inf As New FileInfo(Ruta_documento_migrado)
            If id_registro_migracion = 0 Then
                Result = Guarda_registro_documento_migrado(id_imagen,
                                                           gabinete,
                                                           Ruta_documento_migrado,
                                                           Ruta_url_documento_migrado,
                                                           stru_ra_mig_config_migracion,
                                                           1,
                                                           tipo_file,
                                                           id_usuario_gestion,
                                                           loguin_usuario_gestion,
                                                           num_page,
                                                           num_page,
                                                           file_inf.Name,
                                                           leng_file,
                                                           id_registro_migracion)
                If Result <> "YES" Then
                    Migra_formato_documento = Result
                    Exit Function
                End If
            Else
                Result = Actualiza_registro_documento_migrado(id_registro_migracion,
                                                              Ruta_documento_migrado,
                                                              Ruta_url_documento_migrado,
                                                              stru_ra_mig_config_migracion,
                                                              1,
                                                              tipo_file,
                                                              id_usuario_gestion,
                                                              loguin_usuario_gestion,
                                                              num_page,
                                                              num_page,
                                                              file_inf.Name,
                                                              leng_file)
                If Result <> "YES" Then
                    Migra_formato_documento = Result
                    Exit Function
                End If
            End If
            class_stru_list_vew_migra_documento.id_registro_migracion = id_registro_migracion
            class_stru_list_vew_migra_documento.ruta_documento = Ruta_documento_migrado
            class_stru_list_vew_migra_documento.url_ruta_documento = Ruta_url_documento_migrado
            Migra_formato_documento = "YES"
            Exit Function
        Catch ex As Exception
            Migra_formato_documento = "Inconsistencia general funcion Migra_formato_documento " & ex.Message
        End Try
    End Function
    Function Adjunta_documento_migracion(ByVal id_imagen As Long,
                                         ByVal gabinete As String,
                                         ByVal archivo_load As String,
                                         ByVal id_usuario_gestion As Integer,
                                         ByVal loguin_usuario_gestion As String,
                                         ByRef class_stru_list_vew_migra_documento As class_stru_list_vew_migra_documento) As String
        '---------------------------------------------------------------------------
        'Funcion : Adjunta documento para migración  desde dispositivo
        '
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_list_vew_migra_documento : Retorna la estructura del documento
        '                                      migrado
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-30
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim matri_documentos() As String = Nothing
            Dim Class_ra_mig_config_migracion As New Class_ra_mig_config_migracion
            Dim stru_ra_mig_config_migracion As stru_ra_mig_config_migracion = Nothing
            If HttpContext.Current.Session.Item("UTIL_MIGRA_LOAD_FORMATO_ARCHIVO") = 0 Then
                Adjunta_documento_migracion = "El usuario no tiene permiso para guardar documento de remplazo para migración"
                Exit Function
            End If
            Result = Class_ra_mig_config_migracion.Solicita_estructura_parametro_migracion(stru_ra_mig_config_migracion)
            If Result <> "YES" Then
                Adjunta_documento_migracion = Result
                Exit Function
            End If
            Dim file_ As New FileInfo(archivo_load)
            Dim tipo_file = UCase(file_.Extension)
            If InStr(UCase(file_.Extension), UCase(stru_ra_mig_config_migracion.formato_permitido_migracion)) <= 0 Then
                Adjunta_documento_migracion = "El formato (" & UCase(file_.Extension) & ") no esta permitodo para migración"
                Exit Function
            End If
            Dim Ruta_documento_migrado As String = ""
            Dim Ruta_url_documento_migrado As String = ""
            Dim ruta_temporal_migracion As String = stru_ra_mig_config_migracion.ruta_temporal_migracion.Replace("/", "\")
            Result = Solicita_ruta_temporal_migracion(id_imagen,
                                                      gabinete,
                                                      ruta_temporal_migracion,
                                                      stru_ra_mig_config_migracion.formato_permitido_migracion,
                                                      Ruta_documento_migrado,
                                                      Ruta_url_documento_migrado)
            If Result <> "YES" Then
                Adjunta_documento_migracion = Result
                Exit Function
            End If
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     gabinete,
                                                                                     matri_documentos)
            If Result <> "YES" Then
                Adjunta_documento_migracion = Result
                Exit Function
            End If
            Dim num_page As Integer = matri_documentos.Length - 1
            Dim Class_ItexShare As New Class_ItexShare
            Dim numero_paginas_pdf As Integer = 0
            If UCase(file_.Extension) = ".PDF" Then
                Result = Class_ItexShare.Retorna_numero_paginas_documentos_unificados(archivo_load,
                                                                                      numero_paginas_pdf)
                If Result <> "YES" Then
                    Adjunta_documento_migracion = Result
                    Exit Function
                End If
            End If
            If numero_paginas_pdf <> 0 Then
                If (num_page) <> numero_paginas_pdf Then
                    Adjunta_documento_migracion = "El numero de paginas del archivo para remplazo es de (" & numero_paginas_pdf & ") paginas el cual no corresponde al numero de paginas a remplazar (" & matri_documentos.Length - 1 & ")"
                    Exit Function
                End If
            End If
            Dim leng_file As String = ""
            Dim Class_fyle_system As New Class_fyle_system
            Dim ref_matri_documento() As String = Nothing
            ReDim Preserve ref_matri_documento(0)
            ref_matri_documento(0) = archivo_load
            Result = Class_fyle_system.Solicita_peso_matriz_documentos(ref_matri_documento,
                                                                       leng_file)
            If Result <> "YES" Then
                Adjunta_documento_migracion = Result
                Exit Function
            End If
            File.Copy(archivo_load, Ruta_documento_migrado)
            Dim id_registro_migracion As Long = 0
            Result = Solicita_exitencia_registro_migracion(id_imagen,
                                                           gabinete,
                                                           id_registro_migracion)
            If Result <> "YES" Then
                Adjunta_documento_migracion = Result
                Exit Function
            End If
            Dim file_inf As New FileInfo(Ruta_documento_migrado)
            If id_registro_migracion = 0 Then
                Result = Guarda_registro_documento_migrado(id_imagen,
                                                           gabinete,
                                                           Ruta_documento_migrado,
                                                           Ruta_url_documento_migrado,
                                                           stru_ra_mig_config_migracion,
                                                           2,
                                                           tipo_file,
                                                           id_usuario_gestion,
                                                           loguin_usuario_gestion,
                                                           num_page,
                                                           num_page,
                                                           file_inf.Name,
                                                           leng_file,
                                                           id_registro_migracion)
                If Result <> "YES" Then
                    Adjunta_documento_migracion = Result
                    Exit Function
                End If
            Else
                Result = Actualiza_registro_documento_migrado(id_registro_migracion,
                                                              Ruta_documento_migrado,
                                                              Ruta_url_documento_migrado,
                                                              stru_ra_mig_config_migracion,
                                                              2,
                                                              tipo_file,
                                                              id_usuario_gestion,
                                                              loguin_usuario_gestion,
                                                              num_page,
                                                              num_page,
                                                              file_inf.Name,
                                                              leng_file)
                If Result <> "YES" Then
                    Adjunta_documento_migracion = Result
                    Exit Function
                End If
            End If
            class_stru_list_vew_migra_documento = New class_stru_list_vew_migra_documento
            class_stru_list_vew_migra_documento.id_registro_migracion = id_registro_migracion
            class_stru_list_vew_migra_documento.ruta_documento = Ruta_documento_migrado
            class_stru_list_vew_migra_documento.url_ruta_documento = Ruta_url_documento_migrado
            Adjunta_documento_migracion = "YES"
            Exit Function
        Catch ex As Exception
            Adjunta_documento_migracion = "Inconsistencia general funcion Adjunta_documento_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_ruta_temporal_migracion(ByVal id_imagen As Integer,
                                              ByVal gabinete As String,
                                              ByVal ruta_carpeta_temporal_migracion As String,
                                              ByVal formato_archivo As String,
                                              ByRef ruta_almacenamiento_archivo_migracion As String,
                                              ByRef ruta_url_archivo_migracion As String) As String
        '---------------------------------------------------------------------------------------
        'Funcion : Solicita la ruta temporal de migracion de documentos
        '          
        '---------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------------------
        'id_imagen                             : Representa la identificación de la imagen dentro del
        '                                        gabinete
        'gabinete                              : Representa el nombre del gabinete
        'ruta_carpeta_temporal_migracion       : Representa la ruta temporal de migracion
        'formato_archivo                       : Representa el formato de archivo
        '
        '---------------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------------
        'ruta_almacenamiento_archivo_migracion : Representa ruta archivo migrado
        'ruta_url_archivo_migracion            : Representa url del archivo migrado                          
        '---------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------------
        'Fecha                 : 2024-06-25
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            If Directory.Exists(ruta_carpeta_temporal_migracion) = False Then
                Solicita_ruta_temporal_migracion = "Imposible encontrar la ruta temporal de migracion (" & ruta_carpeta_temporal_migracion & ")"
                Exit Function
            End If
            Dim Ruta_carpeta_temporal_gabinete As String = ruta_carpeta_temporal_migracion & gabinete
            If Directory.Exists(Ruta_carpeta_temporal_gabinete) = False Then
                Directory.CreateDirectory(Ruta_carpeta_temporal_gabinete)
            End If
            Ruta_carpeta_temporal_gabinete = Ruta_carpeta_temporal_gabinete & "\"
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim zero_fill As String = ""
            Result = ClassAlmacenamiento.Ceros_Imagen_Almacenada(zero_fill,
                                                                 id_imagen)
            If Result <> "YES" Then
                Solicita_ruta_temporal_migracion = Result
                Exit Function
            End If
            Dim file_error As String = ""
            ruta_almacenamiento_archivo_migracion = Ruta_carpeta_temporal_gabinete & "MIG" & zero_fill & id_imagen & file_error & formato_archivo
            If File.Exists(ruta_almacenamiento_archivo_migracion) = True Then
                For i As Integer = 0 To 50
                    Try
                        File.Delete(ruta_almacenamiento_archivo_migracion)
                        Exit For
                    Catch ex As Exception
                        file_error = "_" & i
                        ruta_almacenamiento_archivo_migracion = Ruta_carpeta_temporal_gabinete & "MIG" & zero_fill & id_imagen & file_error & formato_archivo
                    End Try
                Next
            End If
            ruta_url_archivo_migracion = "../workflow/Handler_image_wf.ashx?rut_image=" & ruta_almacenamiento_archivo_migracion
            Solicita_ruta_temporal_migracion = "YES"
        Catch ex As Exception
            Solicita_ruta_temporal_migracion = "Inconsistencia general funcion Solicita_ruta_temporal_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_exitencia_registro_migracion(ByVal id_imagen As Integer,
                                                   ByVal gabinete As String,
                                                   ByRef id_registro_migracion As Long) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la exitencia de registro de migración
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_registro_migracion : Retorna el registro de migracion
        '                                      
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-26
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Sql_consulta = "SELECT id_registro_migracion from ra_mig_registro_migracion " &
            " where id_imagen=" & id_imagen & " and nombre_gabinete='" & gabinete & "' and estado_eliminado=0"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_mig_registro_migracion")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_exitencia_registro_migracion = "Error de conexión funcion Solicita_exitencia_registro_migracion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_migracion = 0
                Solicita_exitencia_registro_migracion = "YES"
                Exit Function
            Else
                id_registro_migracion = Datset.Tables(0).Rows(0).Item(0)
                Solicita_exitencia_registro_migracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_exitencia_registro_migracion = "Incosistencia general funcion Solicita_exitencia_registro_migracion " & ex.Message
        End Try
    End Function
    Function Guarda_registro_documento_migrado(ByVal id_imagen As Integer,
                                               ByVal gabinete As String,
                                               ByVal ruta_documento As String,
                                               ByVal url_documento As String,
                                               ByVal stru_ra_mig_config_migracion As stru_ra_mig_config_migracion,
                                               ByVal tipo_migracion As Integer,
                                               ByVal Extension_doc_migrado As String,
                                               ByVal id_usuario_gestion As Integer,
                                               ByVal loguin_usuario_gestion As String,
                                               ByVal num_page_anterior As Integer,
                                               ByVal num_page_nuevo As Integer,
                                               ByVal nombre_archivo As String,
                                               ByVal leng_file As String,
                                               ByRef id_restro_migracion As Long) As String
        '------------------------------------------------------------------------------
        'Funcion : Gauarda el registro de migración de documentos migrados
        '          
        '------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen dentro del
        '                        gabinete
        'gabinete              : Representa el nombre del gabinete
        'ruta_documento        : Representa la ruta del documento migrado
        'url_documento         : Representa la url del documento migrado
        'stru_ra_mig_config_migracion : Representa la estructura del documento migrado
        'tipo_migracion        : Representa el tipo de migracin de documento
        'Extension_doc_migrado : Representa la extension del documento migrado
        'id_usuario_gestion    : Representa la identiifcación del documento migrado
        'loguin_usuario_gestion : Representa el loguin del usuario migrado
        'num_page_anterior     : Representa el numero de paginas del documento a migrar
        'num_page_nuevo        : Representa el numero de paginas del documento migrado
        'nombre_archivo        : Representa el nombre del archivo del documento migrado
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------
        '
        '                                      
        '-------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------
        'Fecha                 : 2024-06-27
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_system1 As New Class_system1
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim time1al As String = Date.Now.ToString
            ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
            Dim id_gabinete As Integer = 0
            Result = Class_system1.SolicitaIdGabineteDocuarchi(gabinete,
                                                       id_gabinete)
            If Result <> "YES" Then
                Guarda_registro_documento_migrado = Result
                Exit Function
            End If
            Dim ref_ruta_documento = ""
            If ruta_documento <> "" Then
                ref_ruta_documento = ruta_documento.Replace("\", "/")
            End If
            Dim sql_insert As String = "insert into ra_mig_registro_migracion (system1_id_gabinete,fecha_registro,id_usuario_gestion_registro," &
                "user_loguin,nombre_gabinete,id_imagen,aplica_ocr,aplica_compresion,version_pdf,ruta_documento,url_ruta_documento,tipo_migracion," &
                "Extension_doc_migrado,num_page_anterior,num_page_nuevo,nombre_archivo,leng_file) values (" & id_gabinete & ",'" & time1al & "'," & id_usuario_gestion & ",'" &
                loguin_usuario_gestion & "','" & gabinete & "','" & id_imagen & "','" & stru_ra_mig_config_migracion.aplica_ocr & "','" &
                stru_ra_mig_config_migracion.aplica_comprencion & "','" & stru_ra_mig_config_migracion.version_formato_migracion_pdf & "','" &
                ref_ruta_documento & "','" & url_documento & "','" & tipo_migracion & "','" & Extension_doc_migrado & "','" & num_page_anterior &
                "','" & num_page_nuevo & "','" & nombre_archivo & "','" & leng_file & "')"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_LAST_INSERT_COMMAND(sql_insert, id_restro_migracion)
            If Result <> "YES" Then
                Guarda_registro_documento_migrado = Result
            Else
                Guarda_registro_documento_migrado = "YES"
            End If
        Catch ex As Exception
            Guarda_registro_documento_migrado = "Incosistencia general funcion Guarda_registro_documento_migrado " & ex.Message
        End Try
    End Function
    Function Actualiza_registro_documento_migrado(ByVal id_registro_migracion As Long,
                                                  ByVal ruta_documento As String,
                                                  ByVal url_documento As String,
                                                  ByVal stru_ra_mig_config_migracion As stru_ra_mig_config_migracion,
                                                  ByVal tipo_migracion As Integer,
                                                  ByVal Extension_doc_migrado As String,
                                                  ByVal id_usuario_gestion As Integer,
                                                  ByVal loguin_usuario_gestion As String,
                                                  ByVal num_page_anterior As Integer,
                                                  ByVal num_page_nuevo As Integer,
                                                  ByVal nombre_archivo As String,
                                                  ByVal leng_file As String) As String
        '------------------------------------------------------------------------------
        'Funcion : Actualiza el registro de migración de documentos migrados
        '          
        '------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------
        'id_registro_migracion : Representa la identificacion del registro de migracion
        '                        
        'ruta_documento        : Representa la ruta del documento migrado
        'url_documento         : Representa la url del documento migrado
        'stru_ra_mig_config_migracion : Representa la estructura del documento migrado
        'tipo_migracion        : Representa el tipo de migracin de documento
        'Extension_doc_migrado : Representa la extension del documento migrado
        'id_usuario_gestion    : Representa la identiifcación del documento migrado
        'loguin_usuario_gestion : Representa el loguin del usuario migrado
        'num_page_anterior     : Representa el numero de paginas del documento a migrar
        'num_page_nuevo        : Representa el numero de paginas del documento migrado
        'nombre_archivo        : Representa el nombre del archivo del documento migrado
        '-------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------
        '
        '                                      
        '-------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------
        'Fecha                 : 2024-06-27
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_system1 As New Class_system1
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim time1al As String = Date.Now.ToString
            ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(time1al)
            Dim ref_ruta_documento = ""
            If ruta_documento <> "" Then
                ref_ruta_documento = ruta_documento.Replace("\", "/")
            End If
            Dim sql_update As String = "update ra_mig_registro_migracion set fecha_registro='" & time1al & "'," &
                "id_usuario_gestion_registro='" & id_usuario_gestion & "',user_loguin='" & loguin_usuario_gestion &
                "',aplica_ocr='" & stru_ra_mig_config_migracion.aplica_ocr & "',aplica_compresion='" & stru_ra_mig_config_migracion.aplica_comprencion &
             "',version_pdf='" & stru_ra_mig_config_migracion.version_formato_migracion_pdf & "',ruta_documento='" & ref_ruta_documento &
             "',url_ruta_documento='" & url_documento & "',tipo_migracion='" & tipo_migracion & "',Extension_doc_migrado='" & Extension_doc_migrado &
             "',num_page_anterior=" & num_page_anterior & ",num_page_nuevo=" & num_page_nuevo & ",nombre_archivo='" & nombre_archivo &
             "',leng_file='" & leng_file & "'" &
             " where id_registro_migracion=" & id_registro_migracion
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Actualiza_registro_documento_migrado = Result
            Else
                Actualiza_registro_documento_migrado = "YES"
            End If
        Catch ex As Exception
            Actualiza_registro_documento_migrado = "Inconsistencia general funcion Actualiza_registro_documento_migrado " & ex.Message
        End Try
    End Function

End Class
