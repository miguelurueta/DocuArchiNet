Imports System.IO

Public Class Class_ra_cert_indice_expediente
    Function Crear_indice_expediente(ByVal id_expediente As Integer, _
                                     ByVal estado_remplaza As Integer) As String
        Try
            Dim Result As String = ""
            Dim stru_produccion_indice() As stru_produccion_indice = Nothing
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Result = ClassGaProducionDocumental.Solicita_estructura_registro_relacion_expediente_indice(id_expediente, _
                                                                                                        stru_produccion_indice)
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
            Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
            Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Dim disco_carpeta As String = stru_ruta_expediente_.DISCO
            Dim class_zerro_fill As New Class_zero_fill
            Result = class_zerro_fill.zero_fill(disco_carpeta, 9, "0")
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
            If Directory.Exists(Ruta_expediente) = False Then
                Crear_indice_expediente = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
                Exit Function
            End If
            Ruta_expediente = Ruta_expediente & disco_carpeta
            If Directory.Exists(Ruta_expediente) = False Then
                Directory.CreateDirectory(Ruta_expediente)
            End If
            Dim existencia_indice_db As String = ""
            Result = Me.Solicita_existencia_indice_db(id_expediente, _
                                                     existencia_indice_db)
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Dim expediente_zero_fil As String = id_expediente.ToString
            Result = class_zerro_fill.zero_fill(expediente_zero_fil, 9, "0")
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Dim Ruta_archivo_xml As String = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
            If estado_remplaza = 1 Then
                If existencia_indice_db = "YES" Then
                    'elimina indice base de datos expediente
                    Result = Elimina_indice_db_expediente(id_expediente)
                    If Result <> "YES" Then
                        Crear_indice_expediente = Result
                        Exit Function
                    End If
                    If File.Exists(Ruta_archivo_xml) Then
                        Kill(Ruta_archivo_xml)
                    End If
                End If
            End If
            Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim Ruta_almacenamiento As String = ""
            Dim codigo_sql_indice_detalle As String = ""
            Dim fecha_incorporacion As String = ""
            Result = ClassGestionFechas.Formatea_Fecha_Almacenamiento_guion(fecha_incorporacion)
            fecha_incorporacion = Left(fecha_incorporacion, 10)
            Dim i_conta As Integer = 1
            Dim i_conta_final = 0
            Dim i As Integer = 0
            If Not stru_produccion_indice Is Nothing Then
                For i = 0 To stru_produccion_indice.Length - 1
                    Result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruta_almacenamiento, _
                                                                           stru_produccion_indice(i).NOMBRE_GABINETE)
                    If Result <> "YES" Then
                        Crear_indice_expediente = Result
                        Exit Function
                    End If
                    Dim ref_ClassDaGabinete As New ClassDaGabinete
                    Dim stru_paramter_image As stru_paramter_image = Nothing
                    Result = ref_ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(stru_produccion_indice(i).NOMBRE_GABINETE, _
                                                                                                      stru_produccion_indice(i).ID_DOCUMENTO_DOCUARCHI_ALMACEN, _
                                                                                                      stru_paramter_image, _
                                                                                                      1)
                    If Result <> "YES" Then
                        Crear_indice_expediente = Result
                        Exit Function
                    End If
                    Dim ref_Class_da_extension As New Class_da_extension
                    Dim Extension As String = ""
                    Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN, _
                                                                                  "", _
                                                                                  Extension, _
                                                                                  "")
                    If Result <> "YES" Then
                        Crear_indice_expediente = Result
                        Exit Function
                    End If
                    Dim Valor_Ceros_Imagen As String = "DIG"
                    Result = Obtener_Ceros_Imagen(stru_produccion_indice(i).ID_DOCUMENTO_DOCUARCHI_ALMACEN.ToString, _
                                                  Valor_Ceros_Imagen)
                    If Result <> "YES" Then
                        Crear_indice_expediente = Result
                        Exit Function
                    End If
                    Valor_Ceros_Imagen = Valor_Ceros_Imagen & Extension
                    Dim Valor_Ceros_Carpeta_Imagen As String = ""
                    Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image.IDEX, _
                                                          Valor_Ceros_Carpeta_Imagen)
                    If Result <> "YES" Then
                        Crear_indice_expediente = Result
                        Exit Function
                    End If
                    Dim Valor_Disco_Imagen As String = ""
                    Valor_Disco_Imagen = stru_produccion_indice(i).NOMBRE_GABINETE & stru_paramter_image.DISC
                    Dim Ruta_Imagen As String = ""
                    Ruta_Imagen = Ruta_almacenamiento.Replace("/", "\") & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen
                    Dim fi As New FileInfo(Ruta_Imagen)
                    If fi.Exists Then
                        If (fi.Length / 1024) > 1024 Then
                            stru_produccion_indice(i).TAMANO = Math.Round(((fi.Length / 1024) / 1024), 2).ToString() & " Mb"
                        Else
                            stru_produccion_indice(i).TAMANO = Math.Round((fi.Length / 1024), 2).ToString() & " Kb"
                        End If
                    End If
                    Dim valor_ingreso_hueya As String = stru_produccion_indice(i).ID_REGISTRO_PRODUCION_DOCUMENTAL
                    Dim huella As String = ""
                    encriptacion.encript_md5(valor_ingreso_hueya, _
                                                  "7894561230!", _
                                                   stru_produccion_indice(i).VALOR_HUELLA)
                    stru_produccion_indice(i).FUCION_RESUMEN = "MD5"
                    stru_produccion_indice(i).NOMBRE_DOCUARCHI = Valor_Ceros_Imagen
                    If stru_produccion_indice(i).SEGUNDO_NOMBRE_DOCUMENTO = "" Then
                        stru_produccion_indice(i).SEGUNDO_NOMBRE_DOCUMENTO = "NA"
                    End If
                    stru_produccion_indice(i).FECHA_ELABORACION = fecha_incorporacion
                    stru_produccion_indice(i).FORMATO = Extension
                    stru_produccion_indice(i).RUTA_ARCHIVO = Ruta_Imagen.Replace("\", "/")
                    If stru_produccion_indice(i).DESCRIPCION_TIPO_DOCUMENTO = "" Then
                        stru_produccion_indice(i).DESCRIPCION_TIPO_DOCUMENTO = "NA"
                    End If
                    If stru_produccion_indice(i).CLASEDOCUMENTO = "" Then
                        stru_produccion_indice(i).CLASEDOCUMENTO = "NA"
                    End If
                Next        
                For i = 0 To stru_produccion_indice.Length - 1
                    stru_produccion_indice(i).ORDEN_EN_EXPEDIENTE = i + 1
                    If i = 0 Then
                        i_conta = 1
                        i_conta_final = stru_produccion_indice(i).NUMERO_FOLIOS
                        stru_produccion_indice(i).PAGINA_INICIO = i_conta
                        stru_produccion_indice(i).PAGINA_FINAL = i_conta_final
                    Else
                        i_conta = i_conta_final + 1
                        i_conta_final = i_conta_final + stru_produccion_indice(i).NUMERO_FOLIOS
                        stru_produccion_indice(i).PAGINA_INICIO = i_conta
                        stru_produccion_indice(i).PAGINA_FINAL = i_conta_final
                    End If
                    Result = Crea_indice_db(id_expediente, _
                                            stru_produccion_indice(i), _
                                            i + 1, _
                                            i_conta, _
                                            i_conta_final, _
                                            stru_produccion_indice(i).ID_INDICE)
                    If Result <> "YES" Then
                        Crear_indice_expediente = Result
                        Exit Function
                    End If
                Next
            End If
            Dim expediente_conservacion() As expediente_conservacion = Nothing
            Dim classGaExpediente As New ClassGaExpediente
            Result = classGaExpediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                       expediente_conservacion)
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Dim Ref_class_remit_dest_interno As New Class_remit_dest_interno
            Dim cargo_usuario_gestion As String = ""
            Dim nombre_usuario_gestion As String = ""
            Dim correo_electronico As String = ""
            Result = Ref_class_remit_dest_interno.Retorna_datos_caracterizacion_usuario_gestion(expediente_conservacion(0).ID_USUARIO_GESTION, _
                                                                                                nombre_usuario_gestion, _
                                                                                                cargo_usuario_gestion, _
                                                                                                correo_electronico)
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Dim nombre_empresa As String = ""
            Dim Ref_class_empresa As New Class_empresa_gestion_documental
            Result = Ref_class_empresa.Solicita_nombre_identificacion_empresa("",
                                                                              nombre_empresa)
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Result = classGaExpediente.Registra_archivo_xml_indice_expediente(Ruta_archivo_xml,
                                                                              id_expediente,
                                                                              expediente_conservacion(0).FECHA_CREACION,
                                                                              nombre_usuario_gestion,
                                                                              nombre_empresa,
                                                                              expediente_conservacion(0).DESCRIPCION_UNIDAD_CONSERVACION,
                                                                              stru_produccion_indice)
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Result = classGaExpediente.Actualiza_estado_expediente_indice_electronico(id_expediente, _
                                                                                      i, _
                                                                                      i_conta_final)
            If Result <> "YES" Then
                Crear_indice_expediente = Result
                Exit Function
            End If
            Crear_indice_expediente = "YES"
        Catch ex As Exception
            Crear_indice_expediente = "Inconsistencia general funcion Crear_indice_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_indice_db(ByVal id_expediente As Integer, _
                                           ByRef existencia_indice_db As String) As String
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select  id_cert_indice_expediente from  ra_cert_indice_expediente " & _
                " where expediente_archivo_ID_EXPEDIENTE=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cert_indice_expediente")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_indice_db = "Error funcion  Solicita_existencia_indice_db " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_indice_db = "NO"
                Solicita_existencia_indice_db = "YES"
                Exit Function
            Else
                existencia_indice_db = "YES"
                Solicita_existencia_indice_db = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_indice_db = "Inconsistencia general funcion Solicita_existencia_indice_db " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_indice_produccion(ByVal id_registro_produccion As Long, _
                                                   ByRef id_registro_indice As Long) As String
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select  id_cert_indice_expediente from  ra_cert_indice_expediente " & _
                " where registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_produccion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cert_indice_expediente")
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_indice_produccion = "Error funcion  Solicita_existencia_indice_produccion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_indice = 0
                Solicita_existencia_indice_produccion = "YES"
                Exit Function
            Else
                id_registro_indice = Datset.Tables(0).Rows(0).Item(0)
                Solicita_existencia_indice_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_indice_produccion = "Inconsistencia general funcion Solicita_existencia_indice_produccion " & ex.Message
        End Try
    End Function
    Function Elimina_indice_db_expediente(ByVal id_expediente As Integer) As String
        Try
            Dim Result As String = ""
            Dim sql_delete As String = "delete from  ra_cert_indice_expediente " & _
                " where expediente_archivo_ID_EXPEDIENTE=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & sql_delete)
            Result = ref.SELECTION_INSERT_COMMAND(sql_delete)
            If Result <> "YES" Then
                Elimina_indice_db_expediente = "Error function  Elimina_indice_db_expediente " & Result
                Exit Function
            Else
                Elimina_indice_db_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Elimina_indice_db_expediente = "Inconsistencia general funcion Elimina_indice_db_expediente " & ex.Message
        End Try
    End Function
    Function Crea_indice_db(ByVal id_expediente As Integer, _
                            ByVal stru_produccion_indice As stru_produccion_indice, _
                            ByVal orden_documento_expediente As Integer, _
                            ByVal pagina_inicial As Integer, _
                            ByVal pagina_final As Integer, _
                            ByRef id_registro As Long) As String
        Try
            Dim Result As String = ""
            Dim sql_insert As String = "insert into  ra_cert_indice_expediente (registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL," & _
                "expediente_archivo_ID_EXPEDIENTE,Nombre_documento,Tipologia_documental,fecha_declaracion_documento,fecha_incorporacion_documento," & _
                "valor_huella,Funcion_resumen,orden_documento_expedicion,pagina_inicial,pagina_final,formato,dimension_kb,origen,ruta_documento,numero_folios, segundo_nombre) values (" & _
                stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL & "," & id_expediente & ",'" & stru_produccion_indice.NOMBRE_DOCUARCHI & "','" & _
                stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO & "','" & stru_produccion_indice.FECHA_DOCUMENTO & "','" & stru_produccion_indice.FECHA_ELABORACION & "','" & _
                stru_produccion_indice.VALOR_HUELLA & "','" & stru_produccion_indice.FUCION_RESUMEN & "'," & orden_documento_expediente & "," & pagina_inicial & _
                "," & pagina_final & ",'" & stru_produccion_indice.FORMATO & "','" & stru_produccion_indice.TAMANO & "','" & stru_produccion_indice.CLASEDOCUMENTO & "','" & _
                stru_produccion_indice.RUTA_ARCHIVO & "'," & stru_produccion_indice.NUMERO_FOLIOS & ",'" & stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO & "')"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_LAST_INSERT_COMMAND(sql_insert, id_registro)
            If Result <> "YES" Then
                Crea_indice_db = "Error function  Crea_indice_db registro produccion (" & stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL & ") " & Result
                Exit Function
            Else
                Crea_indice_db = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Crea_indice_db = "Inconsistencia general funcion Crea_indice_db error sobre el id produccion (" & stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL & ") " & ex.Message
        End Try
    End Function
    Function Consulta_indice_expediente(ByVal id_expediente As Integer, _
                                        ByRef grediview As GridView, _
                                        ByRef reflabel As Label, _
                                        ByRef hideselecion As Object, _
                                        ByRef update As UpdatePanel, _
                                        ByVal estado_tramite As String, _
                                        ByRef update_title As UpdatePanel, _
                                        ByVal tipo_consulta As Integer, _
                                        ByVal valor_consulta As String, _
                                        ByRef colum_order_name As String, _
                                        ByRef order_colum As String) As String
        Try
            Dim sql_consulta As String = ""
            If tipo_consulta = 1 Then
                sql_consulta = "Select id_cert_indice_expediente as IDENTIFICADOR,registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL AS ID_DOCUMENTO," & _
                               " Nombre_documento  AS NOMBRE_DOCUMENTO, Tipologia_documental as TIPOLOGIA_DOCUMENTAL, fecha_declaracion_documento AS FECHA_DECLARACION " & _
                               ",fecha_incorporacion_documento as FECHA_INCORPORACION, valor_huella AS HUELLA, Funcion_resumen AS FUNCION_RESUMEN," & _
                               "orden_documento_expedicion AS  ORDEN, pagina_inicial AS PAGINA_INICIAL, pagina_final AS PAGINA_FINAL, formato as FORMATO, " & _
                               "dimension_kb as TAMANO, origen as ORIGEN, ruta_documento AS RUTA_DOCUMENTO, numero_folios as FOLIOS, segundo_nombre as NOMBRE " & _
                               " FROM ra_cert_indice_expediente " & _
                               " where expediente_archivo_ID_EXPEDIENTE=" & id_expediente & _
                                  " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 2 Then
                Dim sql_consulta_texto As String = ""
                If valor_consulta <> "" Then
                    sql_consulta_texto = "(id_cert_indice_expediente Like '%" & valor_consulta & "%'"
                    sql_consulta_texto = sql_consulta_texto & " id_cert_indice_expediente Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " Nombre_documento Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " Tipologia_documental Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " fecha_declaracion_documento Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " fecha_incorporacion_documento Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " valor_huella Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " Funcion_resumen Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " orden_documento_expedicion Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " pagina_inicial Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " pagina_final Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " formato Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " dimension_kb Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " origen Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " ruta_documento Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " numero_folios Like '%" & valor_consulta & "%' or "
                    sql_consulta_texto = sql_consulta_texto & " segundo_nombre Like '%" & valor_consulta & "%' )"
                End If
                sql_consulta = "Select id_cert_indice_expediente as IDENTIFICADOR,registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL AS ID_DOCUMENTO," & _
                              " Nombre_documento  AS NOMBRE_DOCUMENTO, Tipologia_documental as TIPOLOGIA_DOCUMENTAL, fecha_declaracion_documento AS FECHA_DECLARACION " & _
                              ",fecha_incorporacion_documento as FECHA_INCORPORACION, valor_huella AS HUELLA, Funcion_resumen AS FUNCION_RESUMEN," & _
                              "orden_documento_expedicion AS  ORDEN, pagina_inicial AS PAGINA_INICIAL, pagina_final AS PAGINA_FINAL, formato as FORMATO, " & _
                              "dimension_kb as TAMANO, origen as ORIGEN, ruta_documento AS RUTA_DOCUMENTO, numero_folios as FOLIOS, segundo_nombre as NOMBRE " & _
                              " FROM ra_cert_indice_expediente " & _
                              " where " & sql_consulta_texto & " expediente_archivo_ID_EXPEDIENTE=" & id_expediente & _
                              " order by " & colum_order_name & " " & order_colum
            End If
            If tipo_consulta = 3 Then
                'sql_consulta = "Select etw.Inicio_Tareas_Workflow_id_Tarea as id_tarea,etw.fecha_inicio,etw.estado_prioridad as prioridad," & campo_lista_tramite & ",ESTADO_TRAMITE AS ESTADO,Fecha_Inicio as FECHAINICIOTRAMITE, Fecha_Fin AS FECHAFINALTRAMITE" & "  from " & _
                '             " estados_tarea_workflow etw " & _
                '             " inner join dat_adic_tar" & nombre_ruta & " as  DAT on " & _
                '             " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & filtro & " ) " & _
                '            " where (" & valor_consulta & ") " & _
                '             " and etw.id_actividad=" & Id_actividad & _
                '             " and etw.fecha_Seleccion is null and etw.fecha_fin is null and etw.id_usuario=" & Id_Usuario_Workflow & " and etw.estado_tarea=0 and estado_modulo_radicado = 1 " _
                '               & " order by " & colum_order_name & " " & order_colum
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_cert_indice_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_indice_expediente = "Error listando indice del expediente " & Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("dat_gred_cahce_CERT") = Datset
            If Datset.Tables(0).Rows.Count = 0 Then
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron 0 registro(s) "
                Else
                    reflabel.Text = "Se encontraron 0 registro(s) "
                End If
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Consulta_indice_expediente = "YES"
                Exit Function
            Else
                If tipo_consulta = 1 Then
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
                Else
                    reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & "  registro(s) "
                End If
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fal fa-folder-open fa-lg")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "documento_solic_tramite")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                Consulta_indice_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_indice_expediente = "Inconsistencia general función Consulta_indice_expediente " & ex.Message
        End Try
    End Function
End Class
