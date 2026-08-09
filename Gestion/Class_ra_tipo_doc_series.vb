Imports System.IO
Imports System.Xml
Imports MySql.Data.MySqlClient

Public Class class_ra_tipo_documental_serie
    Property id_imagen As Integer
    Property Gabinete As String
    Property id_serie As Integer
    Property nombre_serie As String
    Property id_sub_serie As Integer
    Property nombre_sub_serie As String
    Property id_tipo_documental As Integer
    Property nombre_tipo_documental As String
    Property error_gestion As String
End Class
Public Class Class_ra_tipo_doc_series
    Function Actualiza_tipo_documental_migracion(Class_ra_tipo_documental_serie As class_ra_tipo_documental_serie,
                                                 ByRef valor_cambio As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Actualiza tipo documental desde gsbinete o modulo de migracion
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_ra_tipo_documental_serie      : Representa la estructura con los datos
        'de gestión del del tipo documental
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'valor_cambio  : Retorna el valor del nuevo tipo documental
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Dim Result As String = ""
        Dim Class_system1 As New Class_system1
        Dim option_trd As Integer = 0
        Dim option_inventario As Integer = 0
        Dim option_unidad As Integer = 0
        Result = Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Class_ra_tipo_documental_serie.Gabinete,
                                                                                                 option_inventario,
                                                                                                 option_trd,
                                                                                                 option_unidad)
        If Result <> "YES" Then
            Actualiza_tipo_documental_migracion = Result
            Exit Function
        End If
        If option_inventario = 0 Then
            Actualiza_tipo_documental_migracion = "Debe activar la opción aplicar inventario documental en el gabinete"
            Exit Function
        End If
        If option_inventario = 0 Then
            Actualiza_tipo_documental_migracion = "Debe activar la opción aplicar tabla de retención en el gabinete"
            Exit Function
        End If
        Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
        Dim id_registro_produccion As Long = 0
        Result = ClassGaProducionDocumental.Solicita_id_registro_producion_documental(Class_ra_tipo_documental_serie.id_imagen,
                                                                                      Class_ra_tipo_documental_serie.Gabinete,
                                                                                      id_registro_produccion)
        If Result <> "YES" Then
            Actualiza_tipo_documental_migracion = Result
            Exit Function
        End If
        Dim id_expediente As Integer = 0
        If id_registro_produccion <> 0 Then
            Result = ClassGaProducionDocumental.Solicita_id_expediente_registro_produccion(id_registro_produccion,
                                                                                           id_expediente)
            If Result <> "YES" Then
                Actualiza_tipo_documental_migracion = Result
                Exit Function
            End If
        End If
        Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
        Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
        Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
        Dim xmlArchivo As New XmlDocument
        Dim update_indice As String = ""
        Dim Ruta_archivo_xml As String = ""
        Dim id_cert_indice_expediente As Long = 0
        If id_expediente <> 0 Then
            Result = Class_ra_cert_indice_expediente.Solicita_existencia_indice_produccion(id_registro_produccion,
                                                                                           id_cert_indice_expediente)
            If Result <> "YES" Then
                Actualiza_tipo_documental_migracion = Result
                Exit Function
            End If
            If id_cert_indice_expediente <> 0 Then
                Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
                If Result <> "YES" Then
                    Actualiza_tipo_documental_migracion = Result
                    Exit Function
                End If
                Dim disco_carpeta_ As String = stru_ruta_expediente_.DISCO
                Dim class_zerro_fill_ As New Class_zero_fill
                Result = class_zerro_fill_.zero_fill(disco_carpeta_, 9, "0")
                If Result <> "YES" Then
                    Actualiza_tipo_documental_migracion = Result
                    Exit Function
                End If
                Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
                If Directory.Exists(Ruta_expediente) = False Then
                    Actualiza_tipo_documental_migracion = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
                    Exit Function
                End If
                Ruta_expediente = Ruta_expediente & disco_carpeta_
                If Directory.Exists(Ruta_expediente) = False Then
                    Directory.CreateDirectory(Ruta_expediente)
                End If
                Dim expediente_zero_fil As String = id_expediente.ToString
                Result = class_zerro_fill_.zero_fill(expediente_zero_fil, 9, "0")
                If Result <> "YES" Then
                    Actualiza_tipo_documental_migracion = Result
                    Exit Function
                End If
                Ruta_archivo_xml = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
                '----------------------------------------------------------------------------
                'Actualiza indice archivo expediente archivo
                '-----------------------------------------------------------------------------
                Dim classgaexpediente As New ClassGaExpediente
                Result = classgaexpediente.Actualiza_indice_tipo_documental_xml_expediente(Ruta_archivo_xml,
                                                                                           id_registro_produccion,
                                                                                           Class_ra_tipo_documental_serie.nombre_tipo_documental,
                                                                                           xmlArchivo)
                If Result <> "YES" Then
                    Actualiza_tipo_documental_migracion = Result
                    Exit Function
                End If
                update_indice = "update ra_cert_indice_expediente set Tipologia_documental='" & Class_ra_tipo_documental_serie.nombre_tipo_documental & "'" &
                                " where id_cert_indice_expediente=" & id_cert_indice_expediente
            End If
        End If
        Dim ClassDaGabinete As New ClassDaGabinete
        Result = ClassDaGabinete.RemoveDiacritics(Class_ra_tipo_documental_serie.nombre_tipo_documental,
                                                  Class_ra_tipo_documental_serie.nombre_tipo_documental)
        If Result <> "YES" Then
            Actualiza_tipo_documental_migracion = Result
            Exit Function
        End If
        Dim id_area As Integer = 0
        Dim ref_Class_series_documentales As New Class_series_documentales
        Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(Class_ra_tipo_documental_serie.id_serie,
                                                                                id_area)
        If Result <> "YES" Then
            Actualiza_tipo_documental_migracion = Result
            Exit Function
        End If
        Dim nombre_area As String = ""
        Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
        If id_area <> 0 Then
            Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
                                                                                       nombre_area)
            If Result <> "YES" Then
                Actualiza_tipo_documental_migracion = Result
                Exit Function
            End If
        End If
        Dim ref_descripcion_tipo_documento As String = "Null"
        If Class_ra_tipo_documental_serie.nombre_tipo_documental <> "" Then
            ref_descripcion_tipo_documento = "'" & Class_ra_tipo_documental_serie.nombre_tipo_documental & "'"
        End If
        Dim ref_id_tipo_documento As Object = "Null"
        If Class_ra_tipo_documental_serie.id_tipo_documental <> 0 Then
            ref_id_tipo_documento = Class_ra_tipo_documental_serie.id_tipo_documental
        End If
        Dim ref_id_area As Object = "Null"
        If id_area <> 0 Then
            ref_id_area = id_area
        End If
        Dim ref_id_serie As Object = "Null"
        If Class_ra_tipo_documental_serie.id_serie <> 0 Then
            ref_id_serie = Class_ra_tipo_documental_serie.id_serie
        End If
        Dim ref_id_sub_serie As Object = "Null"
        If Class_ra_tipo_documental_serie.id_sub_serie <> 0 Then
            ref_id_sub_serie = Class_ra_tipo_documental_serie.id_sub_serie
        End If
        Dim ref_nombre_area As String = "Null"
        If nombre_area <> "" Then
            ref_nombre_area = "'" & nombre_area & "'"
        End If
        Dim ref_nombre_serie As String = "Null"
        If Class_ra_tipo_documental_serie.nombre_serie <> "" Then
            ref_nombre_serie = "'" & Class_ra_tipo_documental_serie.nombre_serie & "'"
        End If
        Dim ref_nombre_sub_serie As String = "Null"
        If Class_ra_tipo_documental_serie.nombre_sub_serie <> "" Then
            ref_nombre_sub_serie = "'" & Class_ra_tipo_documental_serie.nombre_sub_serie & "'"
        End If
        Dim Update_gabinete As String = "update " & Class_ra_tipo_documental_serie.Gabinete & " set TIPODOCUMENTO=" & ref_descripcion_tipo_documento & "," &
           "ID_TIPODOCUMENTO=" & ref_id_tipo_documento & ",ID_AREA=" & ref_id_area & ",ID_SERIE=" & ref_id_serie & ",ID_SUB_SERIE=" & ref_id_sub_serie &
           ",NOMBRESERIE=" & ref_nombre_serie & ",NOMBRESUBSERIE=" & ref_nombre_sub_serie & " where ID=" & Class_ra_tipo_documental_serie.id_imagen
        Dim Update_producion As String = ""
        If id_registro_produccion <> 0 Then
            Update_producion = "update registro_producion_documental set ID_TIPO_DOCUMENTO=" & ref_id_tipo_documento & ",ID_AREA_DEPARTAMENTO=" & ref_id_area &
            ",ID_SERIE_DOCUMENTO=" & ref_id_serie & ",ID_SUBSERIE_DOCUMENTO=" & ref_id_sub_serie & ",SERIE_DOCUMENTO=" & ref_nombre_serie &
            ",SUBSERIE_DOCUMENTO=" & ref_nombre_sub_serie & ",NOMBRE_AREA_DEPARTAMENTO=" & ref_nombre_area & ",DESCRIPCION_TIPO_DOCUMENTO=" & ref_descripcion_tipo_documento &
            " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_produccion
        End If
        Dim datos_campo As String = ""
        Dim detalle_trans As String = ""
        Dim campos_trans As String = ""
        Dim hor2 As New System.DateTime
        hor2 = Date.Now
        Dim hora As String = hor2.Hour.ToString & ":" & hor2.Minute.ToString & ":" & hor2.Second.ToString
        detalle_trans = "CAMBIA CLASE DOCUMENTO"
        campos_trans = "CAMBIA A TIPO (" & Class_ra_tipo_documental_serie.nombre_tipo_documental & ")"
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        Dim isert_datos As String = ""
        If Result <> "YES" Then
            Actualiza_tipo_documental_migracion = Result
            Exit Function
        End If
        isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                     id_registro_produccion & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & "MIGRACION" & "','" & campos_trans & "')"
        Dim update_gestion As String = "INSERT INTO ra_log_inventario (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_REGISTRO_PRODUCCION" &
                                    ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                    isert_datos
        Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
        & "RUT_DOCU,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) VALUES ( "
        SqlTransac = SqlTransac & "'" & Class_ra_tipo_documental_serie.id_imagen & "',"
        SqlTransac = SqlTransac & "'" & "EditarIndice" & "',"
        SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "',"
        SqlTransac = SqlTransac & "'" & date1al & "',"
        SqlTransac = SqlTransac & "'" & "NONE" & "',"
        SqlTransac = SqlTransac & "'" & Class_ra_tipo_documental_serie.Gabinete & "',"
        SqlTransac = SqlTransac & "'" & datos_campo & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & "GESTOR DOCUMENTAL'" & ")"
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim Switc As Integer = 0
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '------------------------------------------
            'Actualiza gabinete
            '------------------------------------------
            If Update_gabinete <> "" Then
                myCommand2.CommandText = Update_gabinete
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documental_migracion = "Imposible actualizar la tabla gabinete cambios  : " & Update_gabinete
                    myConnection.Close()
                    Exit Function
                End If
            End If

            '------------------------------------------
            'Actualiza registro producción documental
            '------------------------------------------
            If Update_producion <> "" Then
                myCommand2.CommandText = Update_producion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documental_migracion = "Imposible actualizar la tabla produccion cambios  : " & Update_producion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If

            '--------------------------------------------
            'Actualiza log inventario
            '--------------------------------------------
            If update_gestion <> "" Then
                myCommand2.CommandText = update_gestion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documental_migracion = "Imposible actualizar la tabla log inventario cambios  : " & update_gestion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            'myTrans.Rollback()
            'myConnection.Close()
            'Actualiza_tipo_documental_migracion = update_gestion
            'Exit Function
            '--------------------------------------------
            'Actualiza indice log  docuarchi
            '--------------------------------------------
            myCommand2.CommandText = SqlTransac
            Switc = myCommand2.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_tipo_documental_migracion = "Imposible actualizar la tabla log docuarchi cambios  : " & SqlTransac
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '--------------------------------------------
            'Actualiza indice expdiente
            '--------------------------------------------
            If update_indice <> "" Then
                myCommand2.CommandText = update_indice
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_tipo_documental_migracion = "Imposible actualizar la tabla indice expediente  : " & update_indice
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                xmlArchivo.Save(Ruta_archivo_xml)
            End If
            myTrans.Commit()
            valor_cambio = Class_ra_tipo_documental_serie.nombre_tipo_documental
            Actualiza_tipo_documental_migracion = "YES"
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_tipo_documental_migracion = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_tipo_documental_migracion = "Error General " & e.Message
            Exit Function
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
        End Try
    End Function
    Function Solicita_lista_tipos_documentales_relacionados_id_sub_serie(ByVal id_sub_serie As Integer,
                                                                         ByRef control_drow_lista As List(Of control_drow_lista)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de tipos documentales relacionados a la sub serie
        '          para lista de documentos en interface
        '
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_serie               : Representa la identificación de la sub serie
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista tipos documentales
        '                     value: identificación del tipo documento
        '                      text: Nombre del tipo documento
        '
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-08-17
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim SQLconsulta = "select Id_Tipo_Doc_Series,Descripcion_Documento " &
            " from tipo_doc_series WHERE sub_serie_id_serie=" & id_sub_serie & " and  Estado_Tipo=1 or tipo_doc_trasversal=1 order by Descripcion_Documento"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            Dim item As control_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                Solicita_lista_tipos_documentales_relacionados_id_sub_serie = "YES"
                Exit Function
            Else
                Solicita_lista_tipos_documentales_relacionados_id_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_tipos_documentales_relacionados_id_sub_serie = "Inconsistencia general funcion Solicita_lista_tipos_documentales_relacionados_id_sub_serie " & ex.Message
        End Try

    End Function
    Function Solicita_lista_tipos_documentales_relacionados_id_serie(ByVal id_serie As Integer,
                                                                    ByRef control_drow_lista As List(Of control_drow_lista)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de tipos documentales relacionados a la serie
        '          para lista de documentos en interface
        '
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_serie               : Representa la identificación de la serie
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista tipo documentales
        '                     value: identificación del tipo documento
        '                      text: Nombre del tipo documento
        '
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-08-17
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim SQLconsulta = "select Id_Tipo_Doc_Series,Descripcion_Documento " &
            " from tipo_doc_series WHERE Series_Documentales_Id_Series=" & id_serie &
            " and  Estado_Tipo=1 and sub_serie_id_serie is null or  tipo_doc_trasversal=1 order by Descripcion_Documento"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            Dim item As control_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                Solicita_lista_tipos_documentales_relacionados_id_serie = "YES"
                Exit Function
            Else
                Solicita_lista_tipos_documentales_relacionados_id_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_tipos_documentales_relacionados_id_serie = "Inconsistencia general funcion Solicita_lista_tipos_documentales_relacionados_id_serie " & ex.Message
        End Try
    End Function
    Function Solicita_tipos_documentales_relacionados_a_la_series(ByVal id_serie As Integer,
                                                                 ByRef drop_lis_sub_series As DropDownList,
                                                                 ByRef ref_update As UpdatePanel,
                                                                 ByVal limpia_list As Integer) As String
        Try

            If limpia_list = 1 Then
                drop_lis_sub_series.Items.Clear()
            End If
            Dim Parametro_Consulta = "select Descripcion_Documento,Id_Tipo_Doc_Series " &
          " from tipo_doc_series WHERE Series_Documentales_Id_Series=" & id_serie &
          " and  Estado_Tipo=1 and sub_serie_id_serie is null or  tipo_doc_trasversal=1 order by Descripcion_Documento"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipos_documentales_relacionados_a_la_series = "Funcion  Solicita_documentales_relacionados_a_la_sub_series dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipos_documentales_relacionados_a_la_series = "YES"
                Exit Function
            Else
                If drop_lis_sub_series.Items.Count = 0 Then
                    drop_lis_sub_series.Items.Add("")
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim lis As New ListItem(Datset.Tables(0).Rows(i).Item(0), Datset.Tables(0).Rows(i).Item(1) & "|" & id_serie & "|0|SERIE|" & Datset.Tables(0).Rows(i).Item(0), True)
                    drop_lis_sub_series.Items.Add(lis)
                Next
                Solicita_tipos_documentales_relacionados_a_la_series = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipos_documentales_relacionados_a_la_series = "Inconsistencia general función Solicita_tipos_documentales_relacionados_a_la_series " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function Solicita_tipos_documentales_relacionados_a_la_sub_series(ByVal id_sub_serie As Integer,
                                                                      ByRef drop_lis_sub_series As DropDownList,
                                                                      ByRef ref_update As UpdatePanel,
                                                                      ByVal limpia_list As Integer,
                                                                      ByVal id_serie As Integer) As String
        Try
            If limpia_list = 1 Then
                drop_lis_sub_series.Items.Clear()
            End If
            Dim Parametro_Consulta = "select Descripcion_Documento,Id_Tipo_Doc_Series " &
            " from tipo_doc_series WHERE sub_serie_id_serie=" & id_sub_serie & " and  Estado_Tipo=1 or tipo_doc_trasversal=1 order by Descripcion_Documento"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipos_documentales_relacionados_a_la_sub_series = "Funcion  Solicita_tipos_documentales_relacionados_a_la_sub_series dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipos_documentales_relacionados_a_la_sub_series = "YES"
                Exit Function
            Else
                If drop_lis_sub_series.Items.Count = 0 Then
                    drop_lis_sub_series.Items.Add("")
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    'Dim lis As New ListItem(Datset.Tables(0).Rows(i).Item(0), Datset.Tables(0).Rows(i).Item(1) & "|" & id_sub_serie & "|SUBSERIE", True)
                    Dim lis As New ListItem(Datset.Tables(0).Rows(i).Item(0), Datset.Tables(0).Rows(i).Item(1) & "|" & id_serie & "|" & id_sub_serie & "|SUBSERIE|" & Datset.Tables(0).Rows(i).Item(0), True)
                    drop_lis_sub_series.Items.Add(lis)
                Next
                Solicita_tipos_documentales_relacionados_a_la_sub_series = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipos_documentales_relacionados_a_la_sub_series = "Inconsistencia general función Solicita_tipos_documentales_relacionados_a_la_sub_series " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function SolicitaTiposDocumentalesRelacionadosSubSerie(ByVal IdSubSerie As Integer,
                                                           ByRef Control_drow_lista As List(Of control_drow_lista)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la lista de los tipos documentales relacionado a la sub serie
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdSubSerie          : Representa la identificación de la sub serie
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Control_drow_lista  : Retorna la estructura del control lista
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta = "select Id_Tipo_Doc_Series,Descripcion_Documento " &
            " from tipo_doc_series WHERE sub_serie_id_serie=" & IdSubSerie & " and  Estado_Tipo=1 or tipo_doc_trasversal=1 order by Descripcion_Documento"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaTiposDocumentalesRelacionadosSubSerie = "Funcion  SolicitaTiposDocumentalesRelacionadosSubSerie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaTiposDocumentalesRelacionadosSubSerie = "YES"
                Exit Function
            Else
                Dim item As control_drow_lista
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    Control_drow_lista.Add(item)
                Next
                SolicitaTiposDocumentalesRelacionadosSubSerie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaTiposDocumentalesRelacionadosSubSerie = "Inconsistencia general funcion SolicitaTiposDocumentalesRelacionadosSubSerie " & ex.Message
        End Try
    End Function
    Function SolicitaTiposDocumentalesRelacionadosSerie(ByVal IdSerie As Integer,
                                                        ByRef Control_drow_lista As List(Of control_drow_lista)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la lista de los tipos documentales relacionado a la serie
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdSerie          : Representa la identificación de la serie
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Control_drow_lista  : Retorna la estructura del control lista
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select  Id_Tipo_Doc_Series,Descripcion_Documento " &
          " from tipo_doc_series WHERE Series_Documentales_Id_Series=" & IdSerie &
          " and  Estado_Tipo=1 and sub_serie_id_serie is null or  tipo_doc_trasversal=1 order by Descripcion_Documento"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaTiposDocumentalesRelacionadosSerie = "Funcion  SolicitaTiposDocumentalesRelacionadosSerie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaTiposDocumentalesRelacionadosSerie = "YES"
                Exit Function
            Else
                Dim item As control_drow_lista
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    Control_drow_lista.Add(item)
                Next
                SolicitaTiposDocumentalesRelacionadosSerie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaTiposDocumentalesRelacionadosSerie = "Inconsistencia general funcion SolicitaTiposDocumentalesRelacionadosSerie " & ex.Message
        End Try
    End Function
    Function Solicita_tipos_documentales_relacionados_a_la_sub_series_default(ByVal id_sub_serie As Integer,
                                                                              ByRef drop_lis_sub_series As DropDownList,
                                                                              ByRef ref_update As UpdatePanel,
                                                                              ByVal nombre_tipo_documental As String,
                                                                              ByVal limpia_list As Integer,
                                                                              ByVal id_tipo_documental As Integer,
                                                                              ByVal pertenencia As String,
                                                                              ByVal id_serie As Integer) As String
        Try

            If limpia_list = 1 Then
                drop_lis_sub_series.Items.Clear()
            End If
            Dim Parametro_Consulta = "select Descripcion_Documento,Id_Tipo_Doc_Series " &
            " from tipo_doc_series WHERE sub_serie_id_serie=" & id_sub_serie & " and  Estado_Tipo=1 or tipo_doc_trasversal=1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipos_documentales_relacionados_a_la_sub_series_default = "Funcion  Solicita_tipos_documentales_relacionados_a_la_sub_series_default dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipos_documentales_relacionados_a_la_sub_series_default = "YES"
                Exit Function
            Else
                If drop_lis_sub_series.Items.Count = 0 Then
                    drop_lis_sub_series.Items.Add("")
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim lis As New ListItem(Datset.Tables(0).Rows(i).Item(0), Datset.Tables(0).Rows(i).Item(1) & "|" & id_serie & "|" & id_sub_serie & "|SUBSERIE|" & Datset.Tables(0).Rows(i).Item(0), True)
                    drop_lis_sub_series.Items.Add(lis)
                Next
                If nombre_tipo_documental <> "" Then
                    For z As Integer = 0 To drop_lis_sub_series.Items.Count - 1
                        If drop_lis_sub_series.Items(z).Value = id_tipo_documental & "|" & id_serie & "|" & id_sub_serie & "|" & pertenencia & "|" & nombre_tipo_documental Then
                            drop_lis_sub_series.SelectedIndex = z
                            Exit For
                        End If
                    Next
                End If
                Solicita_tipos_documentales_relacionados_a_la_sub_series_default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipos_documentales_relacionados_a_la_sub_series_default = "Inconsistencia general función Solicita_tipos_documentales_relacionados_a_la_sub_series_default " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function

    Function Solicita_tipos_documentales_relacionados_a_la_series_defaul(ByVal id_serie As Integer,
                                                                         ByRef drop_lis_sub_series As DropDownList,
                                                                         ByRef ref_update As UpdatePanel,
                                                                         ByVal nombre_tipo_documental As String,
                                                                         ByVal limpia_list As Integer,
                                                                         ByVal id_tipo As Integer,
                                                                         ByVal pertenencia As String) As String
        Try

            If limpia_list = 1 Then
                drop_lis_sub_series.Items.Clear()
            End If
            Dim Parametro_Consulta = "select Descripcion_Documento,Id_Tipo_Doc_Series " &
            " from tipo_doc_series WHERE Series_Documentales_Id_Series=" & id_serie & " and  Estado_Tipo=1 or  tipo_doc_trasversal=1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("SERIES_DOCUMENTALES")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipos_documentales_relacionados_a_la_series_defaul = "Funcion  Solicita_tipos_documentales_relacionados_a_la_series_defaul dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipos_documentales_relacionados_a_la_series_defaul = "YES"
                Exit Function
            Else
                If drop_lis_sub_series.Items.Count = 0 Then
                    drop_lis_sub_series.Items.Add("")
                End If
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    Dim lis As New ListItem(Datset.Tables(0).Rows(i).Item(0), Datset.Tables(0).Rows(i).Item(1) & "|" & id_serie & "|0|SERIE|" & Datset.Tables(0).Rows(i).Item(0), True)
                    drop_lis_sub_series.Items.Add(lis)
                Next
                If nombre_tipo_documental <> "" Then
                    For z As Integer = 0 To drop_lis_sub_series.Items.Count - 1
                        If drop_lis_sub_series.Items(z).Value = id_tipo & "|" & id_serie & "|0|" & pertenencia & "|" & nombre_tipo_documental Then
                            drop_lis_sub_series.SelectedIndex = z
                            Exit For
                        End If
                    Next
                Else
                End If
                Solicita_tipos_documentales_relacionados_a_la_series_defaul = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipos_documentales_relacionados_a_la_series_defaul = "Inconsistencia general función Solicita_tipos_documentales_relacionados_a_la_series_defaul " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function
    Function SolicitaNombreTipoDocumentalSerieSubSerie(ByVal IdTipoDocumental As Integer,
                                                       ByRef NombreTipoDocumental As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita nombre del tipo documental de la serie o sub serie
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoDocumental           : Representa la identificación del tipo documental de serie o 
        '                             sub serie
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreTipoDocumental  : Retorna el nombre del tipo documental
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select  Descripcion_Documento " &
            " from tipo_doc_series where Id_Tipo_Doc_Series=" & IdTipoDocumental
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreTipoDocumentalSerieSubSerie = "Funcion  Solicita_nombre_tipo_documental_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                NombreTipoDocumental = ""
                SolicitaNombreTipoDocumentalSerieSubSerie = "YES"
                Exit Function
            Else
                NombreTipoDocumental = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreTipoDocumentalSerieSubSerie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreTipoDocumentalSerieSubSerie = "Inconsistencia general función Solicita_nombre_tipo_documental_serie " & ex.Message
        End Try
    End Function
    Function SolicitaNombreTipoDocumentalSerie(ByVal id_tipo_documental_serie As Integer,
                                               ByRef nombre_tipo_documental As String) As String
        Try
            Dim Parametro_Consulta = "select  Descripcion_Documento " &
            " from tipo_doc_series where Id_Tipo_Doc_Series=" & id_tipo_documental_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreTipoDocumentalSerie = "Funcion  Solicita_nombre_tipo_documental_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreTipoDocumentalSerie = "Imposible encontrar la identificación del tipo documental " & nombre_tipo_documental
                Exit Function
            Else
                nombre_tipo_documental = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreTipoDocumentalSerie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreTipoDocumentalSerie = "Inconsistencia general función SolicitaNombreTipoDocumentalSerie " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_tipo_documental_sub_serie(ByVal id_tipo_documental_sub_serie As Integer,
                                                       ByRef nombre_tipo_documental As String) As String
        Try
            Dim Parametro_Consulta = "select Descripcion_Documento " &
           " from tipo_doc_series where Id_Tipo_Doc_Series=" & id_tipo_documental_sub_serie
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_series")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_tipo_documental_sub_serie = "Funcion  Solicita_nombre_tipo_documental_sub_serie dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_tipo_documental_sub_serie = "Imposible encontrar el nombre del tipo documental " & id_tipo_documental_sub_serie
                Exit Function
            Else
                nombre_tipo_documental = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_tipo_documental_sub_serie = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_tipo_documental_sub_serie = "Inconsistencia general función Solicita_nombre_tipo_documental_sub_serie " & ex.Message
        End Try
    End Function
    Function Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred(ByVal id_serie As Integer,
                                                                          ByRef page1 As Page) As String
        Try
            Dim Parametro_Consulta = "select Id_Tipo_Doc_Series,Descripcion_Documento As TIPO_DOCUMENTAL " &
            " from tipo_doc_series WHERE Series_Documentales_Id_Series=" & id_serie & " and  Estado_Tipo=1 or tipo_doc_trasversal=1 order by Descripcion_Documento"
            Dim scripma As GridView = page1.FindControl("data_grid")
            Dim ref_UpdateGeneral_documentos As UpdatePanel = page1.FindControl("UpdateGeneral")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID")
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            'HttpContext.Current.Session.Item("DATA_SET_SESION") = Datset_consulta
            If Result <> "YES" Then
                Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred = "Error función Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred  " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                ref_UpdateGeneral_documentos.Update()
                Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred = "YES"
                Exit Function
            Else
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(0).Text.ToString())
                Next
                ref_UpdateGeneral_documentos.Update()
                Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred = "Inconsistencia general función Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred " & ex.Message
        End Try
    End Function
    Function Solicita_tipos_documentales_relacionados_a_la_sub_serie_tipo_gred(ByVal id_sub_serie As Integer,
                                                                               ByRef page1 As Page) As String
        Try
            Dim Parametro_Consulta = "select Id_Tipo_Doc_Series,Descripcion_Documento As TIPO_DOCUMENTAL" &
            " from tipo_doc_series WHERE sub_serie_id_serie=" & id_sub_serie & " and  Estado_Tipo=1 or tipo_doc_trasversal=1 order by Descripcion_Documento"
            Dim scripma As GridView = page1.FindControl("data_grid")
            Dim ref_UpdateGeneral_documentos As UpdatePanel = page1.FindControl("UpdateGeneral")
            Dim hideselecion As Object = page1.FindControl("hdnEmailID")
            Dim Datset_consulta As DataSet = New DataSet("ra_dig_tipos_docum_lista_chequeo")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            'HttpContext.Current.Session.Item("DATA_SET_SESION") = Datset_consulta
            If Result <> "YES" Then
                Solicita_tipos_documentales_relacionados_a_la_sub_serie_tipo_gred = "Error función Solicita_tipos_documentales_relacionados_a_la_serie_tipo_gred " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                ref_UpdateGeneral_documentos.Update()
                Solicita_tipos_documentales_relacionados_a_la_sub_serie_tipo_gred = "YES"
                Exit Function
            Else
                scripma.DataSource = Datset_consulta
                hideselecion.value = "-1"
                scripma.DataBind()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    scripma.Rows(i).Attributes.Add("id", scripma.Rows(i).Cells(0).Text.ToString())
                Next
                ref_UpdateGeneral_documentos.Update()
                Solicita_tipos_documentales_relacionados_a_la_sub_serie_tipo_gred = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipos_documentales_relacionados_a_la_sub_serie_tipo_gred = "Inconsistencia general función Solicita_tipos_documentales_relacionados_a_la_sub_serie_tipo_gred " & ex.Message
        End Try
    End Function
    Function Retorna_nombre_tipo_documental(ByVal id_serie As Integer,
                                           ByVal id_Sub_serie As Integer,
                                           ByVal id_tipo_documental As Integer,
                                           ByRef nombre_tipo_documento As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String
            Parametro_Consulta = " SELECT  Descripcion_Documento " &
             " from tipo_doc_series where  Id_Tipo_Doc_Series=" & id_tipo_documental

            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipos_doc_subseries")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_tipo_documental = "Función Retorna_nombre_tipo_documental dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_nombre_tipo_documental = "Imposible encontrar el nombre del tipo documental con la serie (" & id_serie & ") o la sub serie (" & id_Sub_serie & ")"
                Exit Function
            Else
                nombre_tipo_documento = Datset.Tables(0).Rows(0).Item(0)
                Retorna_nombre_tipo_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_tipo_documental = "Inconsistencia general función Retorna_nombre_tipo_documental " & ex.Message
        End Try
    End Function
End Class
