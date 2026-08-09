Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports AjaxControlToolkit
Imports System.Drawing

Public Structure campos_docuarchi
    Dim nombre_campo As String
    Dim tipo_campo As String
End Structure
Public Structure stru_campos_docuarchi
    Dim nombre_campo As String
    Dim tipo_campo As String
    Dim valor_campo As String
End Structure
Public Class ClassWorkflowIndiceDA
    Private Sub comman_clik(ByVal sender As  _
    System.Object, ByVal e As System.EventArgs)

       
    End Sub
    Function Solicita_valor_campo_estructura_docuarchi(ByVal stru_campos_docuarchi() As stru_campos_docuarchi,
                                                       ByVal nombre_campo As String,
                                                       ByRef valor As String) As String
        Try
            valor = ""
            Dim suitch As Integer = 0
            If Not stru_campos_docuarchi Is Nothing Then
                For i As Integer = 0 To stru_campos_docuarchi.Length - 1
                    If stru_campos_docuarchi(i).nombre_campo = nombre_campo Then
                        valor = stru_campos_docuarchi(i).valor_campo
                        suitch = 1
                        Exit For
                    End If
                Next
                If suitch = 0 Then
                    Solicita_valor_campo_estructura_docuarchi = "Imposible encontrar el campo (" & nombre_campo & ")"
                    Exit Function
                Else
                    Solicita_valor_campo_estructura_docuarchi = "YES"
                    Exit Function
                End If
            Else
                Solicita_valor_campo_estructura_docuarchi = "Estrucura de campos vacia en la función Solicita_valor_campo_estructura_docuarchi"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_valor_campo_estructura_docuarchi = "Inconsistencia general función Solicita_valor_campo_estructura_docuarchi " & ex.Message
        End Try
    End Function

    Function Asigna_datos_gestion_estructura(ByVal stru_campos_docuarchi() As stru_campos_docuarchi, _
                                             ByRef matri_gestion As estructure_gestion, _
                                             ByVal nombre_gabinete As String) As String
        '***************************************************
        'Funcion : Asigna datos gestion documental de la 
        'interface de almacenamiento a la estructura
        'Fecha : 2014-12-28, modificado para web 2015-06-24
        'Ing : Miguel Angel Urueta Miranda
        '***************************************************
        Try
            
            '********************************************
            'Consulta opcion aplica trd
            '*******************************************
            Dim refclastrd As New ClassTrdDocumental
            Dim Result As String = ""
            Dim opt_tabla_retencion As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(opt_tabla_retencion, _
                                                                               nombre_gabinete)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            If opt_tabla_retencion = 0 Then
                Asigna_datos_gestion_estructura = "YES"
                Exit Function
            End If
            Dim valor_campo As String = ""
            Result = Me.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                  "Hidden_id_serie", _
                                                                  valor_campo)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_SERIE = valor_campo
            Result = Me.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                 "Hidden_id_sub_serie", _
                                                                 valor_campo)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_SUB_SERIE = valor_campo
            Result = Me.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                 "Hidden_id_documento", _
                                                                 valor_campo)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_TIPODOCUMENTO = valor_campo
            Result = Me.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                  "Hidden_id_area", _
                                                                  valor_campo)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_AREA = valor_campo
            Result = Me.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                 "NOMBRESERIE", _
                                                                  valor_campo)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            matri_gestion.NOMBRE_SERIE = valor_campo
            Result = Me.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                 "NOMBRESUBSERIE", _
                                                                  valor_campo)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            matri_gestion.NOMBRE_SUB_SERIE = valor_campo
            Result = Me.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi, _
                                                                "TIPODOCUMENTO", _
                                                                 valor_campo)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            matri_gestion.TIPODOCUMENTO = valor_campo
            
            Asigna_datos_gestion_estructura = "YES"
        Catch ex As Exception
            Asigna_datos_gestion_estructura = "Inconsistencia general funcion Asigna_datos_gestion_estructura " & ex.Message
        End Try
    End Function
    Function Asigna_datos_gestion_estructura(ByVal page1 As Page,
                                             ByRef matri_gestion As estructure_gestion,
                                             ByVal nombre_gabinete As String) As String
        '***************************************************
        'Funcion : Asigna datos gestion documental de la 
        'interface de almacenamiento a la estructura
        'Fecha : 2014-12-28, modificado para web 2015-06-24
        'Ing : Miguel Angel Urueta Miranda
        '***************************************************
        Try
            '***********************************
            'Asigna datos trd estructura
            '***********************************
            '********************************************
            'Consulta opcion aplica trd
            '*******************************************
            Dim refclastrd As New ClassTrdDocumental
            Dim Result As String = ""
            Dim opt_tabla_retencion As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(opt_tabla_retencion,
                                                                               nombre_gabinete)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura = Result
                Exit Function
            End If
            If opt_tabla_retencion = 0 Then
                Asigna_datos_gestion_estructura = "YES"
                Exit Function
            End If
            Dim contro As Object = Nothing
            Dim Hidden_id_serie As Object = Nothing
            Dim Hidden_id_sub_serie As Object = Nothing
            Dim Hidden_id_documento As Object = Nothing
            Dim Hidden_id_area As Object = Nothing
            Dim NOMBRESERIE As Object = Nothing
            Dim NOMBRESUBSERIE As Object = Nothing
            Dim TIPODOCUMENTO As Object = Nothing
            Hidden_id_serie = page1.FindControl("Hidden_id_serie")
            If Hidden_id_serie Is Nothing Then
                Asigna_datos_gestion_estructura = "Función Asigna_datos_gestion_estructura dice : imposible encontrar el control Hidden_id_serie"
                Exit Function
            End If
            Hidden_id_sub_serie = page1.FindControl("Hidden_id_sub_serie")
            If Hidden_id_sub_serie Is Nothing Then
                Asigna_datos_gestion_estructura = "Función Asigna_datos_gestion_estructura dice : imposible encontrar el control Hidden_id_sub_serie"
                Exit Function
            End If
            Hidden_id_documento = page1.FindControl("Hidden_id_documento")
            If Hidden_id_documento Is Nothing Then
                Asigna_datos_gestion_estructura = "Función Asigna_datos_gestion_estructura dice : imposible encontrar el control Hidden_id_documento"
                Exit Function
            End If
            Hidden_id_area = page1.FindControl("Hidden_id_area")
            If Hidden_id_area Is Nothing Then
                Asigna_datos_gestion_estructura = "Función Asigna_datos_gestion_estructura dice : imposible encontrar el control Hidden_id_area"
                Exit Function
            End If
            NOMBRESERIE = page1.FindControl("NOMBRESERIE")
            If NOMBRESERIE Is Nothing Then
                Asigna_datos_gestion_estructura = "Función Asigna_datos_gestion_estructura dice : imposible encontrar el control NOMBRESERIE"
                Exit Function
            End If
            NOMBRESUBSERIE = page1.FindControl("NOMBRESUBSERIE")
            If NOMBRESUBSERIE Is Nothing Then
                Asigna_datos_gestion_estructura = "Función Asigna_datos_gestion_estructura dice : imposible encontrar el control NOMBRESUBSERIE"
                Exit Function
            End If
            TIPODOCUMENTO = page1.FindControl("TIPODOCUMENTO")
            If TIPODOCUMENTO Is Nothing Then
                Asigna_datos_gestion_estructura = "Función Asigna_datos_gestion_estructura dice : imposible encontrar el control TIPODOCUMENTO"
                Exit Function
            End If
            matri_gestion.ID_SERIE = Hidden_id_serie.value
            matri_gestion.ID_SUB_SERIE = Hidden_id_sub_serie.value
            matri_gestion.ID_TIPODOCUMENTO = Trim(Hidden_id_documento.value)
            matri_gestion.ID_AREA = Hidden_id_area.value
            matri_gestion.NOMBRE_SERIE = Trim(NOMBRESERIE.text)
            matri_gestion.NOMBRE_SUB_SERIE = Trim(NOMBRESUBSERIE.text)
            matri_gestion.TIPODOCUMENTO = Trim(TIPODOCUMENTO.text)
            Asigna_datos_gestion_estructura = "YES"
        Catch ex As Exception
            Asigna_datos_gestion_estructura = "Inconsistencia general funcion Asigna_datos_gestion_estructura " & ex.Message
        End Try
    End Function

    Function Actualiza_Indice_Imagen_service(ByVal id_Imagen As String,
                                             ByVal Nombre_Gabinete As String,
                                             ByVal stru_campos_docuarchi() As stru_campos_docuarchi,
                                             ByRef tipo_documento As String,
                                             ByVal id_tarea_wf As Long,
                                             ByVal radicado As String) As String
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim result As String = ""
        Dim SqlUpdate As String = "UPDATE " & Nombre_Gabinete & " SET "
        Dim Elimina As String = ""
        Dim starindex As Integer = 0
        Dim pagi As Integer = 0
        Dim actualiza_fultex As String = ""
        Dim option_inventario As Integer = 0
        Dim id_inventario As Long = 0
        Dim suit As Integer = 0
        tipo_documento = ""
        Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
        Dim stru_campo_detalle() As stru_campo_detalle = Nothing
        result = Ref_Class_DETALLE_GABIENETE.SolicitaDetalleCamposGabinete(Nombre_Gabinete,
                                                                              stru_campo_detalle)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        Dim datos_campo As String = ""
        Dim valor_campo As String = ""
        For i As Integer = 0 To stru_campo_detalle.Count - 1
            result = Me.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi,
                                                                  stru_campo_detalle(i).nombre_campo,
                                                                  valor_campo)
            If result <> "YES" Then
                Actualiza_Indice_Imagen_service = result
                Exit Function
            End If
            suit = 1
            If stru_campo_detalle(i).tipo_campo = "INT" Then
                If valor_campo = "" Then
                    'actualiza_fultex = actualiza_fultex & "" & vbCrLf
                    datos_campo = datos_campo & stru_campo_detalle(i).nombre_campo & "=" & "NULL"
                    SqlUpdate = SqlUpdate & stru_campo_detalle(i).nombre_campo & "=" & "NULL,"
                Else
                    actualiza_fultex = actualiza_fultex & Replace(valor_campo, "'", "") & vbCrLf
                    datos_campo = datos_campo & stru_campo_detalle(i).nombre_campo & "=" & Replace(valor_campo, "'", "")
                    SqlUpdate = SqlUpdate & stru_campo_detalle(i).nombre_campo & "=" & Replace(valor_campo, "'", "") & ","
                End If
            End If
            '------------------------------
            'Verifica formato string
            '------------------------------
            If stru_campo_detalle(i).tipo_campo <> "INT" And stru_campo_detalle(i).tipo_campo <> "DATE" Then
                If valor_campo <> "" Then
                    actualiza_fultex = actualiza_fultex & Replace(valor_campo, "'", "") & vbCrLf
                    datos_campo = datos_campo & stru_campo_detalle(i).nombre_campo & "=" & Replace(valor_campo, "'", "")
                    SqlUpdate = SqlUpdate & stru_campo_detalle(i).nombre_campo & "='" & Replace(valor_campo, "'", "") & "',"
                Else
                    datos_campo = datos_campo & stru_campo_detalle(i).nombre_campo & "=" & "NULL"
                    SqlUpdate = SqlUpdate & stru_campo_detalle(i).nombre_campo & "=NULL,"
                End If
            End If
            '-----------------------------
            'Verifica el formato fecha
            '-----------------------------
            Dim Result_Formato_fecha As String = ""
            Dim Matriz_Error() As String
            If stru_campo_detalle(i).tipo_campo = "DATE" Then
                If valor_campo <> "" Then
                    Result_Formato_fecha = ClassGestionFechas.Verifi_campo_fecha_Form6(valor_campo)
                    Erase Matriz_Error
                    Matriz_Error = Split(Result_Formato_fecha, "_")
                    'Verifica el formato general de la fecha
                    If Matriz_Error(0) = "CI" Then
                        Actualiza_Indice_Imagen_service = "Error Formato fecha " & Matriz_Error(1)
                        Exit Function
                    End If
                    'Verifica el formato general del dia
                    If Matriz_Error(0) = "ED" Then
                        Actualiza_Indice_Imagen_service = "Error Formato fecha " & Matriz_Error(1)
                        Exit Function
                    End If
                    'Verifica el formato general del mes
                    If Matriz_Error(0) = "EM" Then
                        Actualiza_Indice_Imagen_service = "Error Formato fecha " & Matriz_Error(1)
                        Exit Function
                    End If
                    actualiza_fultex = actualiza_fultex & Replace(valor_campo, "'", "") & vbCrLf
                    datos_campo = datos_campo & "=" & Replace(valor_campo, "'", "")
                    SqlUpdate = SqlUpdate & stru_campo_detalle(i).nombre_campo & "='" & Replace(valor_campo, "'", "") & "',"
                Else
                    'actualiza_fultex = actualiza_fultex & "" & vbCrLf
                    datos_campo = datos_campo & stru_campo_detalle(i).nombre_campo & "=" & "NULL"
                    SqlUpdate = SqlUpdate & stru_campo_detalle(i).nombre_campo & "=" & "NULL,"
                End If

            End If

        Next
        starindex = SqlUpdate.Length - 1
        Elimina = SqlUpdate.ToString.Substring(starindex)
        If Elimina = "," Then
            SqlUpdate = Left(SqlUpdate,
                             starindex)
        End If
        SqlUpdate = SqlUpdate & " WHERE ID=" & id_Imagen
        '----------------------------------------------------
        'Verifica que por lo menos un campo esta en uno
        '----------------------------------------------------
        If suit = 0 Then
            Actualiza_Indice_Imagen_service = "El sistema ha detectado que el indice puede quedar sin identificación, " & vbCrLf &
            " debe contener por lo menos un campo digitado"
            Exit Function
        End If
        '----------------------------------------------------
        'Cuenta el numero de documentos 
        '----------------------------------------------------
        Dim refclasvisualiza As New ClassVisualisaDocumento
        Dim matri_documentos() As String
        Erase matri_documentos
        result = refclasvisualiza.Genera_Matris_Documentos_Almacenados(id_Imagen,
                                                                       Nombre_Gabinete,
                                                                       matri_documentos)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        If matri_documentos Is Nothing Then
            Actualiza_Indice_Imagen_service = "La matriz de documentos es nothing imposible continuar"
            Exit Function
        End If
        pagi = matri_documentos.Length
        '----------------------------------------------------
        'Verfica si esta activado invnetario documental
        '-----------------------------------------------------
        Dim refclasalmacen As New ClassTrdDocumental
        Dim ref_Class_system1 As New Class_system1
        result = ref_Class_system1.VerificaOpcionAplicarInventarioDocumental(option_inventario,
                                                                                 Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        If option_inventario = 1 Then
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Actualiza_Indice_Imagen_service = "El usuario workflow debe estar asociado a un usuario de gestión  "
                Exit Function
            End If
            '-----------------------------------------------------
            'Retorna el id del inventario del documento
            '----------------------------------------------------
            result = verifica_exitencia_valor_invnetario_gabinete(Nombre_Gabinete,
                                                                  id_Imagen,
                                                                  id_inventario)
            If result <> "YES" Then
                Actualiza_Indice_Imagen_service = result
                Exit Function
            End If
            'Hidden_id_inventario.value = id_inventario
        End If
        Dim Refclasradic As New ClassAlmacenamiento
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        '-------------------------------------------
        'Asigna datos gestion
        '-------------------------------------------
        Dim matri_gestion As estructure_gestion = Nothing
        matri_gestion.CLASE_DOCUMENTO = ""
        matri_gestion.EXPEDIENTE = ""
        matri_gestion.ID_AREA = 0
        matri_gestion.ID_CLASE_DOCUMENTO = 0
        matri_gestion.ID_EXPEDIENTE = 0
        matri_gestion.ID_SERIE = 0
        matri_gestion.ID_SUB_SERIE = 0
        matri_gestion.ID_TIPO_EXPEDIENTE = 0
        matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
        matri_gestion.ID_TIPODOCUMENTO = 0
        matri_gestion.ID_UNIDAD_CONSERVACION = 0
        matri_gestion.ID_USUARIO_GESTION = 0
        matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
        matri_gestion.UNIDAD_CONSERVACION = ""
        matri_gestion.FECHA_ELABORACION = ""
        result = Me.Asigna_datos_gestion_estructura(stru_campos_docuarchi,
                                                   matri_gestion,
                                                   Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = "2|" & result
            Exit Function
        End If
        '---------------------------------------------------------------
        'Asigna datos tipo documento
        '---------------------------------------------------------------
        Dim refcclastipo As New ClassGaTipoDocumental
        result = refcclastipo.Asigna_datos_tipo_documental_estructura(stru_campos_docuarchi,
                                                                      matri_gestion,
                                                                      Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = "2|" & result
            Exit Function
        End If
        '------------------------------------------------------------------
        'Asigna datos desde la interface del expediente a la estrucutura
        '------------------------------------------------------------------
        Dim Refclasexpediente As New ClassGaExpediente
        result = ""
        result = Refclasexpediente.Asigna_datos_expediente_estructura(stru_campos_docuarchi,
                                                                      matri_gestion,
                                                                      Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = "2|" & result
            Exit Function
        End If
        '------------------------------------------------
        'Asigna datos unidad de conservación
        '------------------------------------------------
        Dim Refclasunidad As New ClassUnidadConservacion
        result = ""
        result = Refclasunidad.Asigna_datos_unidad_conservacion_estructura(stru_campos_docuarchi,
                                                                           matri_gestion,
                                                                           Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = "2|" & result
            Exit Function
        End If
        Dim nombre_area As String = ""
        Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
        If matri_gestion.ID_AREA <> 0 Then
            result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(matri_gestion.ID_AREA,
                                                                                       nombre_area)
            If result <> "YES" Then
                Actualiza_Indice_Imagen_service = result
                Exit Function
            End If
        End If
        '---------------------------------------------
        'Construye registro de inventario documental
        '---------------------------------------------
        Dim ref_expediente As String = "null"
        Dim ref_nombre_serie As String = "null"
        Dim ref_nombre_sub_serie As String = "null"
        Dim ref_tipo_documento As String = "null"
        Dim ref_unidad_conserva As String = "null"
        Dim ref_clase_documento As String = "null"
        Dim ref_fecha_elaboracion As String = "null"
        Dim ref_id_expediente As String = "null"
        Dim ref_id_unidad_conservacion As String = "null"
        Dim ref_id_area As String = "null"
        Dim ref_id_serie As String = "null"
        Dim ref_id_tipo_unidad_conservacion As String = "null"
        Dim ref_id_clase_documento As String = "null"
        Dim ref_nombre_area As String = "null"
        Dim ref_id_sub_serie As String = "null"
        Dim ref_id_tipo_documento As String = "null"
        Dim ref_id_tipo_expediente As String = "null"
        Dim ref_id_tipo_unidad_documental As String = "null"
        If matri_gestion.ID_EXPEDIENTE <> 0 Then
            ref_id_tipo_unidad_documental = 2
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 2
        End If
        If matri_gestion.ID_UNIDAD_CONSERVACION <> 0 Then
            ref_id_tipo_unidad_documental = 1
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 1
        End If
        If matri_gestion.ID_TIPO_EXPEDIENTE <> 0 Then
            ref_id_tipo_expediente = matri_gestion.ID_TIPO_EXPEDIENTE
        End If
        If matri_gestion.ID_TIPODOCUMENTO <> 0 Then
            ref_id_tipo_documento = matri_gestion.ID_TIPODOCUMENTO
        End If
        If matri_gestion.ID_SUB_SERIE <> 0 Then
            ref_id_sub_serie = matri_gestion.ID_SUB_SERIE
        End If
        If nombre_area <> "" Then
            ref_nombre_area = "'" & nombre_area & "'"
        End If
        If matri_gestion.ID_CLASE_DOCUMENTO <> 0 Then
            ref_id_clase_documento = matri_gestion.ID_CLASE_DOCUMENTO
        End If
        If matri_gestion.ID_TIPO_UNIDAD_CONSERVACION <> 0 Then
            ref_id_tipo_unidad_conservacion = matri_gestion.ID_TIPO_UNIDAD_CONSERVACION
        End If
        If matri_gestion.ID_SERIE <> 0 Then
            ref_id_serie = matri_gestion.ID_SERIE
        End If
        If matri_gestion.ID_AREA <> 0 Then
            ref_id_area = matri_gestion.ID_AREA
        End If
        If matri_gestion.ID_EXPEDIENTE <> 0 Then
            ref_id_expediente = matri_gestion.ID_EXPEDIENTE
        End If
        If matri_gestion.ID_UNIDAD_CONSERVACION <> 0 Then
            ref_id_unidad_conservacion = matri_gestion.ID_UNIDAD_CONSERVACION
        End If
        If matri_gestion.EXPEDIENTE <> "" Then
            ref_expediente = "'" & matri_gestion.EXPEDIENTE & "'"
        End If
        If matri_gestion.NOMBRE_SERIE <> "" Then
            ref_nombre_serie = "'" & matri_gestion.NOMBRE_SERIE & "'"
        End If
        If matri_gestion.NOMBRE_SUB_SERIE <> "" Then
            ref_nombre_sub_serie = "'" & matri_gestion.NOMBRE_SUB_SERIE & "'"
        End If
        If matri_gestion.TIPODOCUMENTO <> "" Then
            ref_tipo_documento = "'" & matri_gestion.TIPODOCUMENTO & "'"
        End If
        If matri_gestion.UNIDAD_CONSERVACION <> "" Then
            ref_unidad_conserva = "'" & matri_gestion.UNIDAD_CONSERVACION & "'"
        End If
        If matri_gestion.CLASE_DOCUMENTO <> "" Then
            ref_clase_documento = "'" & matri_gestion.CLASE_DOCUMENTO & "'"
        End If
        If matri_gestion.FECHA_ELABORACION <> "" Then
            ref_fecha_elaboracion = "'" & matri_gestion.FECHA_ELABORACION & "'"
        End If
        Dim estado_archivo As Integer = 1
        If matri_gestion.ID_EXPEDIENTE <> 0 Or matri_gestion.ID_UNIDAD_CONSERVACION <> 0 Then
            estado_archivo = 0
        End If
        Dim up_modificacion As String = ""
        If estado_archivo = 0 Then
            up_modificacion = " ESTADO_DOCUMENTO_ARCHIVO=" & estado_archivo & ","
        End If

        Dim datos_insert_inventario As String = ""
        Dim sqlinventario As String = ""
        If option_inventario = 1 Then
            sqlinventario = "Update registro_producion_documental " &
            " set ID_AREA_DEPARTAMENTO=" & ref_id_area & "," &
            " ID_SERIE_DOCUMENTO=" & ref_id_serie & "," & up_modificacion &
            " SERIE_DOCUMENTO=" & ref_nombre_serie & "," &
            " ID_SUBSERIE_DOCUMENTO=" & ref_id_sub_serie & "," &
            " SUBSERIE_DOCUMENTO=" & ref_nombre_sub_serie & "," &
            " ID_TIPO_DOCUMENTO=" & ref_id_tipo_documento & "," &
            " DESCRIPCION_TIPO_DOCUMENTO=" & ref_tipo_documento & "," &
            " FULTEXT_DOCUMENTO='" & actualiza_fultex & "'," &
            " EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" & ref_id_expediente & "," &
            " EXPEDIENTE=" & ref_expediente & "," &
            " ID_TIPO_EXPEDIENTE=" & ref_id_tipo_expediente & "," &
            " ID_TIPO_UNIDAD_CONSERVACION=" & ref_id_tipo_unidad_conservacion & "," &
            " ID_UNIDAD_CONSERVACION=" & ref_id_unidad_conservacion & "," &
            " ID_CLASE_DOCUMENTO=" & ref_id_clase_documento & "," &
            " CLASEDOCUMENTO=" & ref_clase_documento & "," &
            " FECHA_ELABORACION=" & ref_fecha_elaboracion & "," &
            " UNIDADCONSERVA=" & ref_unidad_conserva & "," &
            " NOMBRE_AREA_DEPARTAMENTO=" & ref_nombre_area & "," &
            " ID_TIPO_UNIDAD_DOCUMENTAL=" & ref_id_tipo_unidad_documental &
            " where ID_DOCUMENTO_DOCUARCHI_ALMACEN=" & id_Imagen &
            " and NOMBRE_GABINETE='" & Nombre_Gabinete & "'"

        End If
        Dim update_gestion As String = ""
        Dim op_selecion_unidad As Integer = 0
        result = ref_Class_system1.Verfica_opcion_seleccion_unidad(op_selecion_unidad,
                                                                   Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        If op_selecion_unidad <> 0 Then
            update_gestion = "Update " & Nombre_Gabinete & " set ID_EXPEDIENTE=" & ref_id_expediente &
            ", ID_TIPO_EXPEDIENTE=" & ref_id_tipo_expediente & "," &
            " ID_TIPO_UNIDAD_CONSERVACION=" & ref_id_tipo_unidad_conservacion & "," &
            " ID_UNIDAD_CONSERVACION=" & ref_id_unidad_conservacion & "," &
            " ID_CLASE_DOCUMENTO=" & ref_id_clase_documento & "," &
            " ID_TIPO_UNIDAD_DOCUMENTAL=" & ref_id_tipo_unidad_documental

        End If
        Dim op_tabla_retension As Integer = 0
        result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(op_tabla_retension,
                                                                           Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        If op_tabla_retension <> 0 Then
            If update_gestion = "" Then
                update_gestion = "Update " & Nombre_Gabinete & " set ID_AREA=" & ref_id_area & "," &
                " ID_SERIE=" & ref_id_serie & "," &
                " ID_SUB_SERIE=" & ref_id_sub_serie & "," &
                " ID_TIPODOCUMENTO=" & ref_id_tipo_documento
            Else
                update_gestion = update_gestion & ", ID_AREA=" & ref_id_area & "," &
                                " ID_SERIE=" & ref_id_serie & "," &
                                " ID_SUB_SERIE=" & ref_id_sub_serie & "," &
                                " ID_TIPODOCUMENTO=" & ref_id_tipo_documento
            End If
        End If
        If update_gestion <> "" Then
            update_gestion = update_gestion & " where id=" & id_Imagen
        End If
        '-----------------------------------------------------------
        'Detectar cambio unidada conservacion o expediente
        '-----------------------------------------------------------
        Dim matri_gestion_antigua As estructure_gestion = Nothing
        matri_gestion_antigua.CLASE_DOCUMENTO = ""
        matri_gestion_antigua.EXPEDIENTE = ""
        matri_gestion_antigua.ID_AREA = 0
        matri_gestion_antigua.ID_CLASE_DOCUMENTO = 0
        matri_gestion_antigua.ID_EXPEDIENTE = 0
        matri_gestion_antigua.ID_SERIE = 0
        matri_gestion_antigua.ID_SUB_SERIE = 0
        matri_gestion_antigua.ID_TIPO_EXPEDIENTE = 0
        matri_gestion_antigua.ID_TIPO_UNIDAD_CONSERVACION = 0
        matri_gestion_antigua.ID_TIPODOCUMENTO = 0
        matri_gestion_antigua.ID_UNIDAD_CONSERVACION = 0
        matri_gestion_antigua.ID_USUARIO_GESTION = 0
        matri_gestion_antigua.TIPO_UNIDAD_DOCUMENTAL = 0
        matri_gestion_antigua.UNIDAD_CONSERVACION = ""
        matri_gestion_antigua.FECHA_ELABORACION = ""
        '-------------------------------------------------------------
        'Asigna datos a la estructura desde la base de datos
        '-------------------------------------------------------------
        Dim refclas2 As New ClassAlmacenamiento
        Dim ClassDaGabinete As New ClassDaGabinete
        If op_selecion_unidad <> 0 Then
            result = refclas2.Solicita_datos_unidad_conservacion_estructura_base_datos(matri_gestion_antigua,
                                                                                       Nombre_Gabinete,
                                                                                       id_Imagen)
            If result <> "YES" Then
                Actualiza_Indice_Imagen_service = result
                Exit Function
            End If
        End If
        If op_selecion_unidad <> 0 Then
            result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(id_Imagen,
                                                                                 Nombre_Gabinete,
                                                                                 matri_gestion_antigua)
            If result <> "YES" Then
                Actualiza_Indice_Imagen_service = result
                Exit Function
            End If
        End If
        If op_selecion_unidad <> 0 Then
            result = refclas2.Solicita_datos_tipo_documental_estructura_base_datos(matri_gestion_antigua,
                                                                                   Nombre_Gabinete,
                                                                                   id_Imagen)
            If result <> "YES" Then
                Actualiza_Indice_Imagen_service = result
                Exit Function
            End If
        End If
        If op_tabla_retension <> 0 Then
            result = refclas2.Solicita_datos_gestion_estructura_base_datos(matri_gestion_antigua,
                                                                           Nombre_Gabinete,
                                                                           id_Imagen)
            If result <> "YES" Then
                Actualiza_Indice_Imagen_service = result
                Exit Function
            End If
        End If
        tipo_documento = matri_gestion.TIPODOCUMENTO
        '----------------------------------------------------
        'Solicita ruta del documento
        '----------------------------------------------------
        Dim Route_cabinet As String = ""
        Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
        result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Route_cabinet,
                                                                   Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If

        Dim Route_document As String = ""
        result = ClassDaGabinete.Solicita_ruta_achivo_gabinete(id_Imagen,
                                                                   Nombre_Gabinete,
                                                                   Route_cabinet,
                                                                   Route_document)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        Route_document = Route_document.Replace("\", "/")
        '------------------------------------------------
        'Solicita datos del documento
        '------------------------------------------------
        Dim Class_system1 As New Class_system1
        Dim inventario_documental As Integer = 0
        Dim aplica_trd As Integer = 0
        Dim asigna_unidad As Integer = 0
        result = Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Nombre_Gabinete,
                                                                                                     inventario_documental,
                                                                                                     aplica_trd,
                                                                                                     asigna_unidad)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        Dim stru_paramter_image As stru_paramter_image = Nothing
        result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(Nombre_Gabinete,
                                                                 id_Imagen,
                                                                 stru_paramter_image,
                                                                 aplica_trd,
                                                                 1)
        If result <> "YES" Then
            Actualiza_Indice_Imagen_service = result
            Exit Function
        End If
        Dim ref_user As String = "null"
        If stru_paramter_image.USER <> "" Then
            ref_user = "'" & stru_paramter_image.USER & "'"
        End If
        Dim ref_Tipologia As String = "null"
        If stru_paramter_image.TIPODOCUMENTO <> "" Then
            ref_Tipologia = "'" & stru_paramter_image.TIPODOCUMENTO & "'"
        End If
        Dim update_exp_aterior As String = ""
        Dim update_exp_nuevo As String = ""
        Dim update_unidad_anterior As String = ""
        Dim update_unidad_nuevo As String = ""
        Dim caso_unidad As Integer = 0
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim mySqldatReader2 As MySqlDataReader
        Dim Switc As Integer = 0
        Try
            Dim refclas As New ClassAlmacenamiento
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            If op_selecion_unidad <> 0 Then
                '----------------------------------------------------------------------------------------------------
                'Caso (1) cambia expediente Decrementa exp antiguo incrementa exp nuevo
                '----------------------------------------------------------------------------------------------------
                If matri_gestion_antigua.ID_EXPEDIENTE <> matri_gestion.ID_EXPEDIENTE And matri_gestion.ID_EXPEDIENTE > 0 _
                 And matri_gestion_antigua.ID_EXPEDIENTE > 0 And caso_unidad = 0 Then
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If
                    Dim unidad_conserva_tipo_nuevo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If
                    update_exp_aterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                        " FROM expediente_archivo where ID_EXPEDIENTE = " _
                       & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"

                    update_exp_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                   & "'" & matri_gestion.ID_EXPEDIENTE & "' " & "for update"
                    '----------------------------------------------
                    'Decrementar expediente antiguo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    myCommand2.CommandText = update_exp_aterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación del expediente por conexión caso 1 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro del expediente caso 1 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 1
                        End If
                    End If
                    '----------------------------------------------
                    'Incrementa expediente nuevo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_exp_nuevo
                    mySqldatReader2 = myCommand2.ExecuteReader()
                    If mySqldatReader2 Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación del expediente por conexión caso 1 Incrementa expediente nuevo"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader2.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro del expediente caso 1 Incrementa expediente nuevo"
                        mySqldatReader2.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader2.Read()
                        Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                        Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                        mySqldatReader2.Close()
                    End If
                    update_sql = ""
                    If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                        update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                    End If
                    If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                        Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                        update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                    End If
                    myCommand2.CommandText = update_sql
                    Switc = myCommand2.ExecuteNonQuery()
                    If Switc = 0 Then
                        Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                        'mySqldatReader2.Close()
                        'mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        caso_unidad = 1
                        'mySqldatReader2.Close()
                        'mySqldatReader.Close()
                    End If

                End If

                '----------------------------------------------------------------------------------------------------
                'Caso (2) cambia unidad conservación decrementa unidad antigua incrementa unidad nueva
                '----------------------------------------------------------------------------------------------------
                If matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> matri_gestion.ID_UNIDAD_CONSERVACION And
                 matri_gestion.ID_UNIDAD_CONSERVACION > 0 And matri_gestion_antigua.ID_UNIDAD_CONSERVACION > 0 _
                 And caso_unidad = 0 Then
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If
                    Dim unidad_conserva_tipo_nuevo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If
                    update_unidad_anterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                       " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                      & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"


                    update_unidad_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                     " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                    & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' " & "for update"

                    '----------------------------------------------
                    'Decrementar unidad de conservacion antigua
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    myCommand2.CommandText = update_unidad_anterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 2 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro de la unidad de conservacion caso 2 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios de la unidad de conservación "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 2
                        End If

                    End If
                    '----------------------------------------------
                    'Incrementa unidad conservación nuevo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_unidad_nuevo
                    mySqldatReader2 = myCommand2.ExecuteReader()
                    If mySqldatReader2 Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación de la unidad  conexión caso 2 Incrementa unidad"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader2.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro de la unidad caso 2 Incrementa Incrementa unidad"
                        mySqldatReader2.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader2.Read()
                        Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                        Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                        mySqldatReader2.Close()
                    End If
                    update_sql = ""
                    If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                        Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                            mySqldatReader2.Close()
                            mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 2
                        End If
                    End If
                End If
                If matri_gestion_antigua.ID_EXPEDIENTE <> matri_gestion.ID_EXPEDIENTE And
                matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> matri_gestion.ID_UNIDAD_CONSERVACION And caso_unidad = 0 Then
                    '------------------------------------------------------------------------------------------------------------------
                    'Caso (3) cambia expediente a unidad de conservación decrementa exp antiguo e incrementa unidad conservacion nueva
                    '------------------------------------------------------------------------------------------------------------------
                    If matri_gestion.ID_UNIDAD_CONSERVACION > 0 And matri_gestion_antigua.ID_EXPEDIENTE > 0 Then
                        update_unidad_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                             " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                            & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' " & "for update"

                        update_exp_aterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                       " FROM expediente_archivo where ID_EXPEDIENTE = " _
                      & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"
                        Dim refclastrd As New ClassTrdDocumental
                        Dim unidad_conserva_tipo_antiguo As String = ""
                        result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                        If result <> "YES" Then
                            Actualiza_Indice_Imagen_service = result
                            Exit Function
                        End If
                        Dim unidad_conserva_tipo_nuevo As String = ""
                        result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                        If result <> "YES" Then
                            Actualiza_Indice_Imagen_service = result
                            Exit Function
                        End If
                        '----------------------------------------------
                        'Decrementar expediente antiguo caso 3
                        '---------------------------------------------
                        Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                        Dim Numero_Electronico_contenido_antiguo As Integer = 0
                        myCommand2.CommandText = update_exp_aterior
                        mySqldatReader = myCommand2.ExecuteReader()
                        If mySqldatReader Is Nothing Then
                            Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación del expediente por conexión caso 3 decrementar"
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader.HasRows = False Then
                            Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro del expediente caso 3 decrementar"
                            mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader.Read()
                            Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                            Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                            mySqldatReader.Close()
                        End If
                        Dim update_sql As String = ""
                        If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                            Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                            update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                            " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                        End If
                        If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                            Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                            update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                            " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                        End If
                        If update_sql <> "" Then
                            myCommand2.CommandText = update_sql
                            Switc = myCommand2.ExecuteNonQuery()
                            If Switc = 0 Then
                                Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            Else
                                caso_unidad = 3
                            End If
                        End If
                        '----------------------------------------------
                        'Incrementa unidad conservación nuevo caso 3
                        '---------------------------------------------
                        Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                        Dim Numero_Electronico_contenido_nuevo As Integer = 0
                        myCommand2.CommandText = update_unidad_nuevo
                        mySqldatReader2 = myCommand2.ExecuteReader()
                        If mySqldatReader2 Is Nothing Then
                            Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación de la unidad  conexión caso 3 Incrementa unidad"
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader2.HasRows = False Then
                            Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro de la unidad caso 3 Incrementa Incrementa unidad"
                            mySqldatReader2.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader2.Read()
                            Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                            Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                            mySqldatReader2.Close()
                        End If
                        update_sql = ""
                        If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                            Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                            update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                            " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                        End If
                        If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                            Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                            update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                            " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                        End If
                        If update_sql <> "" Then
                            myCommand2.CommandText = update_sql
                            Switc = myCommand2.ExecuteNonQuery()
                            If Switc = 0 Then
                                Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                                mySqldatReader2.Close()
                                mySqldatReader.Close()
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            Else
                                caso_unidad = 3
                            End If
                        End If
                    End If
                    '-----------------------------------------------------------------------------------------------------------
                    'Caso (4) cambia unidad de conservación a expediente ( incrementa exp nuevo, decrmenta unidad antigua
                    '-----------------------------------------------------------------------------------------------------------
                    If matri_gestion.ID_EXPEDIENTE > 0 And matri_gestion_antigua.ID_UNIDAD_CONSERVACION > 0 And caso_unidad = 0 Then
                        update_unidad_anterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                               " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                              & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"

                        update_exp_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                       " FROM expediente_archivo where ID_EXPEDIENTE = " _
                                      & "'" & matri_gestion.ID_EXPEDIENTE & "' " & "for update"
                        '---------------------------------------------------
                        'Retorna unidad de tipo documento
                        '---------------------------------------------------
                        Dim refclastrd As New ClassTrdDocumental
                        Dim unidad_conserva_tipo_antiguo As String = ""
                        result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                        If result <> "YES" Then
                            Actualiza_Indice_Imagen_service = result
                            Exit Function
                        End If
                        Dim unidad_conserva_tipo_nuevo As String = ""
                        result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                        If result <> "YES" Then
                            Actualiza_Indice_Imagen_service = result
                            Exit Function
                        End If
                        '----------------------------------------------
                        'Decrementar unidad de conservacion antigua
                        '----------------------------------------------
                        Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                        Dim Numero_Electronico_contenido_antiguo As Integer = 0
                        myCommand2.CommandText = update_unidad_anterior
                        mySqldatReader = myCommand2.ExecuteReader()
                        If mySqldatReader Is Nothing Then
                            Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 4 decrementar"
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader.HasRows = False Then
                            Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro de la unidad de conservacion caso 4 decrementar"
                            mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader.Read()
                            Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                            Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                            mySqldatReader.Close()
                        End If
                        Dim update_sql As String = ""
                        If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                            Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                            update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                            " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                        End If
                        If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                            Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                            update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                            " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                        End If
                        If update_sql <> "" Then
                            myCommand2.CommandText = update_sql
                            Switc = myCommand2.ExecuteNonQuery()
                            If Switc = 0 Then
                                Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios de la unidad de conservación "
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            Else
                                caso_unidad = 4
                            End If

                        End If
                        '----------------------------------------------
                        'Incrementa expediente nuevo
                        '---------------------------------------------
                        Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                        Dim Numero_Electronico_contenido_nuevo As Integer = 0
                        myCommand2.CommandText = update_exp_nuevo
                        mySqldatReader2 = myCommand2.ExecuteReader()
                        If mySqldatReader2 Is Nothing Then
                            Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación del expediente por conexión caso 4 Incrementa expediente nuevo"
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader2.HasRows = False Then
                            Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro del expediente caso 4 Incrementa expediente nuevo"
                            mySqldatReader2.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader2.Read()
                            Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                            Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                            mySqldatReader2.Close()
                        End If
                        update_sql = ""
                        If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                            Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                            update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                            " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                        End If
                        If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                            Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                            update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                            " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                        End If
                        If update_sql <> "" Then
                            myCommand2.CommandText = update_sql
                            Switc = myCommand2.ExecuteNonQuery()
                            If Switc = 0 Then
                                Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                                'mySqldatReader2.Close()
                                'mySqldatReader.Close()
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            Else
                                caso_unidad = 4
                                'mySqldatReader2.Close()
                                'mySqldatReader.Close()
                            End If
                        End If
                    End If

                End If

                '---------------------------------------------------
                'Caso (5) limpia expediente decrementa exp antiguo
                '---------------------------------------------------
                If matri_gestion_antigua.ID_EXPEDIENTE <> matri_gestion.ID_EXPEDIENTE And
                matri_gestion.ID_EXPEDIENTE = 0 And matri_gestion.ID_UNIDAD_CONSERVACION = 0 And caso_unidad = 0 Then
                    update_exp_aterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                        " FROM expediente_archivo where ID_EXPEDIENTE = " _
                       & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO,
                                                                               unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If

                    '----------------------------------------------
                    'Decrementar expediente antiguo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    myCommand2.CommandText = update_exp_aterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación del expediente por conexión caso 1 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro del expediente caso 5 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 1
                        End If
                    End If


                End If

                '--------------------------------------------------------------
                'Caso (6) limpia unidad conservación decrementa unidad antigua
                '--------------------------------------------------------------
                If matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> matri_gestion.ID_UNIDAD_CONSERVACION And
                matri_gestion.ID_UNIDAD_CONSERVACION = 0 And matri_gestion.ID_EXPEDIENTE = 0 And caso_unidad = 0 Then
                    update_unidad_anterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                                              " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                                             & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO,
                                                                               unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If

                    '----------------------------------------------
                    'Decrementar unidad de conservacion antigua
                    '----------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    myCommand2.CommandText = update_unidad_anterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 6 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro de la unidad de conservacion caso 6 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios de la unidad de conservación caso (6) "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 6
                        End If

                    End If
                End If
                '--------------------------------------------------------
                'Caso (7) asigna expediente incremeta nuevo expediente
                '--------------------------------------------------------
                If matri_gestion_antigua.ID_EXPEDIENTE <> matri_gestion.ID_EXPEDIENTE And matri_gestion.ID_EXPEDIENTE > 0 _
                 And matri_gestion_antigua.ID_EXPEDIENTE = 0 And matri_gestion_antigua.ID_UNIDAD_CONSERVACION = 0 And caso_unidad = 0 Then
                    update_exp_aterior = ""
                    update_exp_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                   & "'" & matri_gestion.ID_EXPEDIENTE & "' " & "for update"

                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_nuevo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO,
                                                                               unidad_conserva_tipo_nuevo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If

                    '----------------------------------------------
                    'Incrementa expediente nuevo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_exp_nuevo
                    mySqldatReader2 = myCommand2.ExecuteReader()
                    If mySqldatReader2 Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación del expediente por conexión caso 1 Incrementa expediente nuevo"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader2.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro del expediente caso 1 Incrementa expediente nuevo"
                        mySqldatReader2.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader2.Read()
                        Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                        Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                        mySqldatReader2.Close()
                    End If
                    Dim update_sql = ""
                    If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                        update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                    End If
                    If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                        Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                        update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                            mySqldatReader2.Close()
                            'mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 7
                            'mySqldatReader2.Close()
                            'mySqldatReader.Close()
                        End If
                    End If

                End If
                '----------------------------------------------------------------------------
                'Caso (8) asigna unidad conservación incrementa nueva unidad de conservación
                '----------------------------------------------------------------------------
                If matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> matri_gestion.ID_UNIDAD_CONSERVACION And
                 matri_gestion.ID_UNIDAD_CONSERVACION > 0 And matri_gestion_antigua.ID_UNIDAD_CONSERVACION = 0 _
                 And matri_gestion_antigua.ID_EXPEDIENTE = 0 And caso_unidad = 0 Then
                    update_unidad_anterior = ""
                    update_unidad_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                     " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                    & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' " & "for update"
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_nuevo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If

                    '----------------------------------------------
                    'Incrementa unidad conservación nuevo caso 8
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_unidad_nuevo
                    mySqldatReader2 = myCommand2.ExecuteReader()
                    If mySqldatReader2 Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación de la unidad  conexión caso 8 Incrementa unidad"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader2.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro de la unidad caso 8 Incrementa Incrementa unidad"
                        mySqldatReader2.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader2.Read()
                        Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                        Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                        mySqldatReader2.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                        Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios del expediente "
                            mySqldatReader2.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 8

                        End If
                    End If
                End If

                '----------------------------------------------------------------------------------
                'Caso 9 cambia solo calse de documento unidad conservacion seleccionada
                '----------------------------------------------------------------------------------
                If matri_gestion_antigua.ID_UNIDAD_CONSERVACION = matri_gestion.ID_UNIDAD_CONSERVACION And
                matri_gestion.ID_EXPEDIENTE = matri_gestion_antigua.ID_EXPEDIENTE _
                And matri_gestion_antigua.ID_CLASE_DOCUMENTO <> matri_gestion.ID_CLASE_DOCUMENTO _
                And matri_gestion_antigua.ID_UNIDAD_CONSERVACION > 0 And caso_unidad = 0 Then

                    update_unidad_anterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                                                                 " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                                                                & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If
                    Dim unidad_conserva_tipo_nueva As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nueva)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen_service = result
                        Exit Function
                    End If
                    '--------------------------------------------------------------------
                    'Incrementa o decrementa numero de electrnicos o digitalizados
                    'de unidad de conservación
                    '-----------------------------------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_unidad_anterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 9 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro de la unidad de conservacion caso 9 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And unidad_conserva_tipo_nueva = "ELECTRONICO" Then
                        If pagi <= Numero_Digitalizado_contenido_antiguo Then
                            Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        End If
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        ",NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And unidad_conserva_tipo_nueva = "DIGITALIZADO" Then
                        If pagi <= Numero_Electronico_contenido_antiguo Then
                            Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        End If
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        ",NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios de la unidad de conservación caso (6) "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 9
                        End If

                    End If

                End If
            End If
            '----------------------------------------------------------------------------------
            'Caso 10 cambia solo calse de documento expediente seleccionado
            '----------------------------------------------------------------------------------
            If matri_gestion_antigua.ID_UNIDAD_CONSERVACION = matri_gestion.ID_UNIDAD_CONSERVACION And
            matri_gestion.ID_EXPEDIENTE = matri_gestion_antigua.ID_EXPEDIENTE _
            And matri_gestion_antigua.ID_CLASE_DOCUMENTO <> matri_gestion.ID_CLASE_DOCUMENTO _
            And matri_gestion_antigua.ID_EXPEDIENTE > 0 And caso_unidad = 0 Then
                update_exp_aterior = ""
                update_exp_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                " FROM expediente_archivo where ID_EXPEDIENTE = " _
               & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"
                '---------------------------------------------------
                'Retorna unidad de tipo documento
                '---------------------------------------------------
                Dim refclastrd As New ClassTrdDocumental
                Dim unidad_conserva_tipo_antiguo As String = ""
                result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                If result <> "YES" Then
                    Actualiza_Indice_Imagen_service = result
                    Exit Function
                End If
                Dim unidad_conserva_tipo_nueva As String = ""
                result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nueva)
                If result <> "YES" Then
                    Actualiza_Indice_Imagen_service = result
                    Exit Function
                End If
                '--------------------------------------------------------------------
                'Incrementa o decrementa numero de electrnicos o digitalizados
                'de expedientes
                '-----------------------------------------------------------------------
                Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                Dim Numero_Electronico_contenido_antiguo As Integer = 0
                Dim Numero_Electronico_contenido_nuevo As Integer = 0
                myCommand2.CommandText = update_exp_nuevo
                mySqldatReader = myCommand2.ExecuteReader()
                If mySqldatReader Is Nothing Then
                    Actualiza_Indice_Imagen_service = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 10 decrementar"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If mySqldatReader.HasRows = False Then
                    Actualiza_Indice_Imagen_service = "Imposible Encontrar el registro de la unidad de conservacion caso 10 decrementar"
                    mySqldatReader.Close()
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    mySqldatReader.Read()
                    Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                    Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                    mySqldatReader.Close()
                End If
                Dim update_sql As String = ""
                If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And unidad_conserva_tipo_nueva = "ELECTRONICO" Then
                    If pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                    End If
                    Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo + pagi
                    update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                    ",NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                    " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                End If
                If unidad_conserva_tipo_antiguo = "ELECTRONICO" And unidad_conserva_tipo_nueva = "DIGITALIZADO" Then
                    If pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                    End If
                    Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo + pagi
                    update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                    ",NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                    " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                End If
                If update_sql <> "" Then
                    myCommand2.CommandText = update_sql
                    Switc = myCommand2.ExecuteNonQuery()
                    If Switc = 0 Then
                        Actualiza_Indice_Imagen_service = "Imposible Actualizar numero de folios de la unidad de conservación caso (10) "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        caso_unidad = 10
                    End If

                End If
            End If
            '------------------------------------------------------------
            'Verifica cambios gestion
            '-----------------------------------------------------------
            Dim cambio_gestion As Integer = 0
            If matri_gestion.CLASE_DOCUMENTO <> matri_gestion_antigua.CLASE_DOCUMENTO Then cambio_gestion = 1
            If matri_gestion.EXPEDIENTE <> matri_gestion_antigua.EXPEDIENTE Then cambio_gestion = 1
            If matri_gestion.FECHA_ELABORACION <> matri_gestion_antigua.FECHA_ELABORACION Then cambio_gestion = 1
            If matri_gestion.ID_AREA <> matri_gestion_antigua.ID_AREA Then cambio_gestion = 1
            If matri_gestion.ID_CLASE_DOCUMENTO <> matri_gestion_antigua.ID_CLASE_DOCUMENTO Then cambio_gestion = 1
            If matri_gestion.ID_EXPEDIENTE <> matri_gestion_antigua.ID_EXPEDIENTE Then cambio_gestion = 1
            If matri_gestion.ID_SERIE <> matri_gestion_antigua.ID_SERIE Then cambio_gestion = 1
            If matri_gestion.ID_SUB_SERIE <> matri_gestion_antigua.ID_SUB_SERIE Then cambio_gestion = 1
            If matri_gestion.ID_TIPO_EXPEDIENTE <> matri_gestion_antigua.ID_TIPO_EXPEDIENTE Then cambio_gestion = 1
            If matri_gestion.ID_TIPO_UNIDAD_CONSERVACION <> matri_gestion_antigua.ID_TIPO_UNIDAD_CONSERVACION Then cambio_gestion = 1
            If matri_gestion.ID_TIPODOCUMENTO <> matri_gestion_antigua.ID_TIPODOCUMENTO Then cambio_gestion = 1
            If matri_gestion.UNIDAD_CONSERVACION <> matri_gestion_antigua.UNIDAD_CONSERVACION Then cambio_gestion = 1
            '------------------------------------------------------------
            'Actualiza campos docuarchi plantilla y fulltex inventario
            '-------------------------------------------------------------
            If SqlUpdate <> "UPDATE " & Nombre_Gabinete & " SET " Then
                If option_inventario = 1 And id_inventario <> 0 Then
                    Dim sqlinventario_fultex = "Update registro_producion_documental " &
                    " set FULTEXT_DOCUMENTO='" & actualiza_fultex & "'" &
                    " where ID_DOCUMENTO_DOCUARCHI_ALMACEN=" & id_Imagen &
                    " and NOMBRE_GABINETE='" & Nombre_Gabinete & "'"
                    myCommand2.CommandText = sqlinventario_fultex
                    Switc = myCommand2.ExecuteNonQuery()
                    If Switc = 0 Then
                        Actualiza_Indice_Imagen_service = "Imposible actualizar fultex invnetario  : " & sqlinventario_fultex
                        'myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        caso_unidad = 30
                    End If
                End If
                'Update = Update & " Where id=" & _Data_Grid.Rows(IndexRow).Cells(0).Value
                myCommand2.CommandText = SqlUpdate
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_Indice_Imagen_service = "Imposible actualizar la tabla docuarchi cambios  : " & SqlUpdate
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    caso_unidad = 30
                End If
                Dim hor2 As New System.DateTime
                hor2 = Date.Now
                Dim hora As String = hor2.Hour.ToString & ":" & hor2.Minute.ToString & ":" & hor2.Second.ToString
                Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
                & "RUT_DOCU,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,RADICADO,ID_TAREA_WF,ID_RUTA_WF,USER_PROPIETARIO,TIPOLOGIA_DOCUMENTAL) VALUES ( "
                SqlTransac = SqlTransac & "'" & id_Imagen & "',"
                SqlTransac = SqlTransac & "'" & "EditarIndice" & "',"
                SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("DA_Login_Usuario") & "',"
                SqlTransac = SqlTransac & "'" & date1al & "',"
                SqlTransac = SqlTransac & "'" & Route_document & "',"
                SqlTransac = SqlTransac & "'" & Nombre_Gabinete & "',"
                SqlTransac = SqlTransac & "'" & datos_campo & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & "WORKFLOW'" & ",'" &
                    radicado & "'," & id_tarea_wf & "," & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & "," & ref_user & "," & ref_Tipologia & ")"
                myCommand2.CommandText = SqlTransac
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_Indice_Imagen_service = "Imposible actualizar la tabla docuarchi cambios  : " & SqlUpdate
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    caso_unidad = 30
                End If
            End If
            '******************************************
            'Actualiza indice de invnetario documental
            '******************************************
            If option_inventario = 1 And id_inventario <> 0 And cambio_gestion <> 0 Then
                myCommand2.CommandText = sqlinventario
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_Indice_Imagen_service = "Imposible actualizar la tabla System  : " & sqlinventario
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    caso_unidad = 20
                End If
            End If
            '************************************************
            'Actualiza datos gestión documental  en gabinete  ojo cambio 
            'If update_gestion <> "" And cambio_gestion <> 0 Then
            '************************************************
            If update_gestion <> "" Then
                myCommand2.CommandText = update_gestion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_Indice_Imagen_service = "Imposible actualizar datos gestion gabinete  : " & update_gestion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    caso_unidad = 15
                End If
            End If
            Dim hor As String = Now
            Dim detalle_trans As String = ""
            Dim campos_trans As String = ""
            Dim isert_datos As String = ""
            If cambio_gestion <> 0 Then
                '********************************************************
                'Registra auditoria inventario
                '********************************************************
                If option_inventario <> 0 And id_inventario <> 0 Then
                    If matri_gestion.CLASE_DOCUMENTO <> matri_gestion_antigua.CLASE_DOCUMENTO Then
                        detalle_trans = "CAMBIA CLASE DOCUMENTO"
                        campos_trans = "CAMBIA CLASE (" & matri_gestion_antigua.CLASE_DOCUMENTO &
                        ") A CLASE (" & matri_gestion.CLASE_DOCUMENTO & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_EXPEDIENTE <> matri_gestion_antigua.ID_EXPEDIENTE Then
                        detalle_trans = "CAMBIA EXPEDIENTE"
                        campos_trans = "CAMBIA EXPEDIENTE (" & matri_gestion_antigua.EXPEDIENTE &
                       ") A EXPEDIENTE (" & matri_gestion.EXPEDIENTE & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.FECHA_ELABORACION <> matri_gestion_antigua.FECHA_ELABORACION Then
                        detalle_trans = "CAMBIA FECHA ELABORACION"
                        campos_trans = "CAMBIA FECHA ELABORACION (" & matri_gestion_antigua.FECHA_ELABORACION &
                      ") POR (" & matri_gestion.FECHA_ELABORACION & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_AREA <> matri_gestion_antigua.ID_AREA Then
                        detalle_trans = "CAMBIA AREA DOCUMENTO"
                        campos_trans = "CAMBIA AREA DOCUMENTO (" & matri_gestion_antigua.ID_AREA &
                      ") POR (" & matri_gestion.ID_AREA & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_CLASE_DOCUMENTO <> matri_gestion_antigua.ID_CLASE_DOCUMENTO Then
                        detalle_trans = "CAMBIA ID CLASE DOCUMENTO"
                        campos_trans = "CAMBIA ID CLASE DOCUMENTO (" & matri_gestion_antigua.ID_CLASE_DOCUMENTO &
                      ") POR (" & matri_gestion.ID_CLASE_DOCUMENTO & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_SERIE <> matri_gestion_antigua.ID_SERIE Then
                        detalle_trans = "CAMBIA SERIE DOCUMENTO"
                        campos_trans = "CAMBIA SERIE DOCUMENTO (" & matri_gestion_antigua.ID_SERIE &
                      ") POR (" & matri_gestion.ID_SERIE & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_SUB_SERIE <> matri_gestion_antigua.ID_SUB_SERIE Then
                        detalle_trans = "CAMBIA SUB SERIE DOCUMENTO"
                        campos_trans = "CAMBIA SUB SERIE DOCUMENTO (" & matri_gestion_antigua.ID_SUB_SERIE &
                      ") POR (" & matri_gestion.ID_SUB_SERIE & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_TIPODOCUMENTO <> matri_gestion_antigua.ID_TIPODOCUMENTO Then
                        detalle_trans = "CAMBIA TIPO DOCUMENTO"
                        campos_trans = "CAMBIA TIPO DOCUMENTO (" & matri_gestion_antigua.ID_TIPODOCUMENTO &
                      ") POR (" & matri_gestion.ID_TIPODOCUMENTO & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_TIPO_UNIDAD_CONSERVACION <> matri_gestion_antigua.ID_TIPO_UNIDAD_CONSERVACION Then
                        detalle_trans = "CAMBIA UNIDAD CONSERVACION"
                        campos_trans = "CAMBIA UNIDAD CONSERVACION (" & matri_gestion_antigua.UNIDAD_CONSERVACION &
                     ") POR (" & matri_gestion.UNIDAD_CONSERVACION & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"
                        End If

                    End If
                    update_gestion = "INSERT INTO ra_log_inventario (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_REGISTRO_PRODUCCION" &
                                     ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                     isert_datos
                    If isert_datos <> "" Then
                        myCommand2.CommandText = update_gestion
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen_service = "Imposible registrar log inventario  : " & update_gestion
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 50
                        End If
                    End If
                End If

            End If
            If caso_unidad <> 0 Then
                myTrans.Commit()
            End If
            Actualiza_Indice_Imagen_service = "YES"
            Exit Function
        Catch e As Exception
            Try

                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_Indice_Imagen_service = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_Indice_Imagen_service = "Error General " & e.Message
            Exit Function
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
        End Try

    End Function
    Function Actualiza_Indice_Imagen(ByVal id_Imagen As String,
                                     ByVal Nombre_Gabinete As String,
                                     ByVal Conection_Dat As String,
                                     ByVal radicado As String,
                                     ByVal id_tarea_wf As Long,
                                     ByVal id_ruta_wf As Integer,
                                     ByRef PAGE1 As Page,
                                     ByRef tipo_documento As String) As String
        Dim ClassGestionFechas As New ClassGestionFechas
        Dim result As String = ""
        Dim SqlUpdate As String = "UPDATE " & Nombre_Gabinete & " SET "
        Dim Elimina As String = ""
        Dim starindex As Integer = 0
        Dim Objet As New Object
        Dim pagi As Integer = 0
        Dim actualiza_fultex As String = ""
        Dim option_inventario As Integer = 0
        Dim id_inventario As Long = 0
        Dim Hidden_id_inventario As Object = PAGE1.FindControl("Hidden_id_inventario")
        Dim suit As Integer = 0
        tipo_documento = ""
        Dim Sql_consulta = "SELECT CAMPO,TIPO FROM " &
             "DETALLE_GABIENETE " &
             "WHERE GABINETE='" & Nombre_Gabinete & "' AND VISIBLE=1 ORDER BY IDENTI"
        Dim ref2 As New conect.Dbase_Conction_Mysql_DA
        Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
        Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
        If Resulta <> "YES" Then
            Actualiza_Indice_Imagen = "Funcion  Visualiza_Idice_Documento WF-01 Mensaje DBMS " & Resulta
            Exit Function
        End If
        If Datset.Tables(0).Rows.Count = 0 Then
            Actualiza_Indice_Imagen = "Imposible encontrar los campos para gabinete : " & Nombre_Gabinete
            Exit Function
        End If
        Dim Matri_campo_nombre As String = ""
        Dim Matri_Campos_Gabinete() As String
        Erase Matri_Campos_Gabinete
        For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
            'ReDim Preserve Datos_Imagen(I)
            ReDim Preserve Matri_Campos_Gabinete(y)
            Matri_Campos_Gabinete(y) = Datset.Tables(0).Rows(y).Item(0).ToString & "|" & Datset.Tables(0).Rows(y).Item(1).ToString
            If y = 0 Then
                Matri_campo_nombre = Datset.Tables(0).Rows(y).Item(0).ToString
            Else
                Matri_campo_nombre = Matri_campo_nombre & "," & Datset.Tables(0).Rows(y).Item(0).ToString
            End If
        Next
        Dim datos_campo As String = ""
        For i As Integer = 0 To Matri_Campos_Gabinete.Count - 1
            Dim splitdate() As String
            splitdate = Split(Matri_Campos_Gabinete(i).ToString, "|")
            Objet = Nothing
            Objet = PAGE1.Form.FindControl(splitdate(0))
            If Not Objet Is Nothing Then
                If Objet.text <> "" Then
                    suit = 1
                End If
                'verifica formato enteros
                If splitdate(1) = "INT" Then
                    If Objet.text = "" Then
                        'actualiza_fultex = actualiza_fultex & "" & vbCrLf
                        datos_campo = datos_campo & splitdate(0) & "=" & "NULL"
                        SqlUpdate = SqlUpdate & splitdate(0) & "=" & "NULL,"
                    Else
                        actualiza_fultex = actualiza_fultex & Replace(Objet.text, "'", "") & vbCrLf
                        datos_campo = datos_campo & splitdate(0) & "=" & Replace(Objet.text, "'", "")
                        SqlUpdate = SqlUpdate & splitdate(0) & "=" & Replace(Objet.text, "'", "") & ","
                    End If
                End If
                '------------------------------
                'Verifica formato string
                '------------------------------
                If splitdate(1) <> "INT" And splitdate(1) <> "DATE" Then
                    If Objet.text <> "" Then
                        actualiza_fultex = actualiza_fultex & Replace(Objet.text, "'", "") & vbCrLf
                        datos_campo = datos_campo & splitdate(0) & "=" & Replace(Objet.text, "'", "")
                        SqlUpdate = SqlUpdate & splitdate(0) & "='" & Replace(Objet.text, "'", "") & "',"
                    Else
                        'actualiza_fultex = actualiza_fultex & "" & vbCrLf
                        datos_campo = datos_campo & splitdate(0) & "=" & "NULL"
                        SqlUpdate = SqlUpdate & splitdate(0) & "=NULL,"
                    End If
                End If
                '-----------------------------
                'Verifica el formato fecha
                '-----------------------------
                Dim Result_Formato_fecha As String = ""
                Dim Matriz_Error() As String
                If splitdate(1) = "DATE" Then
                    If Objet.text <> "" Then
                        Result_Formato_fecha = ClassGestionFechas.Verifi_campo_fecha_Form6(Objet.text)
                        Erase Matriz_Error
                        Matriz_Error = Split(Result_Formato_fecha, "_")
                        'Verifica el formato general de la fecha
                        If Matriz_Error(0) = "CI" Then
                            Actualiza_Indice_Imagen = "Error Formato fecha " & Matriz_Error(1)
                            Exit Function
                        End If
                        'Verifica el formato general del dia
                        If Matriz_Error(0) = "ED" Then
                            Actualiza_Indice_Imagen = "Error Formato fecha " & Matriz_Error(1)
                            Exit Function
                        End If
                        'Verifica el formato general del mes
                        If Matriz_Error(0) = "EM" Then
                            Actualiza_Indice_Imagen = "Error Formato fecha " & Matriz_Error(1)
                            Exit Function
                        End If
                        actualiza_fultex = actualiza_fultex & Replace(Objet.text, "'", "") & vbCrLf
                        datos_campo = datos_campo & "=" & Replace(Objet.text, "'", "")
                        SqlUpdate = SqlUpdate & splitdate(0) & "='" & Replace(Objet.text, "'", "") & "',"
                    Else
                        'actualiza_fultex = actualiza_fultex & "" & vbCrLf
                        datos_campo = datos_campo & splitdate(0) & "=" & "NULL"
                        SqlUpdate = SqlUpdate & splitdate(0) & "=" & "NULL,"
                    End If

                End If
            End If
        Next
        starindex = SqlUpdate.Length - 1
        Elimina = SqlUpdate.ToString.Substring(starindex)
        If Elimina = "," Then
            SqlUpdate = Left(SqlUpdate,
                             starindex)
        End If
        SqlUpdate = SqlUpdate & " WHERE ID=" & id_Imagen
        '----------------------------------------------------
        'Verifica que por lo menos un campo esta en uno
        '----------------------------------------------------
        If suit = 0 Then
            Actualiza_Indice_Imagen = "El sistema ha detectado que el indice puede quedar sin identificación, " & vbCrLf &
            " debe contener por lo menos un campo digitado"
            Exit Function
        End If
        '----------------------------------------------------
        'Cuenta el numero de documentos 
        '----------------------------------------------------
        Dim refclasvisualiza As New ClassVisualisaDocumento
        Dim matri_documentos() As String
        Erase matri_documentos
        result = refclasvisualiza.Genera_Matris_Documentos_Almacenados(id_Imagen,
                                                                       Nombre_Gabinete,
                                                                       matri_documentos)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If
        If matri_documentos Is Nothing Then
            Actualiza_Indice_Imagen = "La matriz de documentos es nothing imposible continuar"
            Exit Function
        End If
        pagi = matri_documentos.Length
        '----------------------------------------------------
        'Verfica si esta activado invnetario documental
        '-----------------------------------------------------
        Dim refclasalmacen As New ClassTrdDocumental
        Dim ref_Class_system1 As New Class_system1
        result = ref_Class_system1.VerificaOpcionAplicarInventarioDocumental(option_inventario,
                                                                                 Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If
        If option_inventario = 1 Then
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Actualiza_Indice_Imagen = "El usuario workflow debe estar asociado a un usuario de gestión  "
                Exit Function
            End If
            '-----------------------------------------------------
            'Retorna el id del inventario del documento
            '----------------------------------------------------
            result = verifica_exitencia_valor_invnetario_gabinete(Nombre_Gabinete,
                                                                  id_Imagen,
                                                                  id_inventario)
            If result <> "YES" Then
                Actualiza_Indice_Imagen = result
                Exit Function
            End If
            Hidden_id_inventario.value = id_inventario
        End If
        Dim Refclasradic As New ClassAlmacenamiento
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If
        '-------------------------------------------
        'Asigna datos gestion
        '-------------------------------------------
        Dim matri_gestion As estructure_gestion = Nothing
        'matri_gestion = Nothing
        matri_gestion.CLASE_DOCUMENTO = ""
        matri_gestion.EXPEDIENTE = ""
        matri_gestion.ID_AREA = 0
        matri_gestion.ID_CLASE_DOCUMENTO = 0
        matri_gestion.ID_EXPEDIENTE = 0
        matri_gestion.ID_SERIE = 0
        matri_gestion.ID_SUB_SERIE = 0
        matri_gestion.ID_TIPO_EXPEDIENTE = 0
        matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
        matri_gestion.ID_TIPODOCUMENTO = 0
        matri_gestion.ID_UNIDAD_CONSERVACION = 0
        matri_gestion.ID_USUARIO_GESTION = 0
        matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
        matri_gestion.UNIDAD_CONSERVACION = ""
        matri_gestion.FECHA_ELABORACION = ""
        result = Me.Asigna_datos_gestion_estructura(PAGE1,
                                                   matri_gestion,
                                                   Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = "2|" & result
            Exit Function
        End If
        '---------------------------------------------------------------
        'Asigna datos tipo documento
        '---------------------------------------------------------------
        Dim refcclastipo As New ClassGaTipoDocumental
        result = refcclastipo.Asigna_datos_tipo_documental_estructura(PAGE1,
                                                                      matri_gestion,
                                                                      Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = "2|" & result
            Exit Function
        End If
        '------------------------------------------------------------------
        'Asigna datos desde la interface del expediente a la estrucutura
        '------------------------------------------------------------------
        Dim Refclasexpediente As New ClassGaExpediente
        result = ""
        result = Refclasexpediente.Asigna_datos_expediente_estructura(PAGE1,
        matri_gestion, Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = "2|" & result
            Exit Function
        End If
        '------------------------------------------------
        'Asigna datos unidad de conservación
        '------------------------------------------------
        Dim Refclasunidad As New ClassUnidadConservacion
        result = ""
        result = Refclasunidad.Asigna_datos_unidad_conservacion_estructura(PAGE1,
        matri_gestion, Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = "2|" & result
            Exit Function
        End If
        Dim nombre_area As String = ""
        Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
        If matri_gestion.ID_AREA <> 0 Then
            result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(matri_gestion.ID_AREA,
                                                                                       nombre_area)
            If result <> "YES" Then
                Actualiza_Indice_Imagen = result
                Exit Function
            End If
        End If
        '---------------------------------------------
        'Construye registro de inventario documental
        '---------------------------------------------
        Dim ref_expediente As String = "null"
        Dim ref_nombre_serie As String = "null"
        Dim ref_nombre_sub_serie As String = "null"
        Dim ref_tipo_documento As String = "null"
        Dim ref_unidad_conserva As String = "null"
        Dim ref_clase_documento As String = "null"
        Dim ref_fecha_elaboracion As String = "null"
        Dim ref_id_expediente As String = "null"
        Dim ref_id_unidad_conservacion As String = "null"
        Dim ref_id_area As String = "null"
        Dim ref_id_serie As String = "null"
        Dim ref_id_tipo_unidad_conservacion As String = "null"
        Dim ref_id_clase_documento As String = "null"
        Dim ref_nombre_area As String = "null"
        Dim ref_id_sub_serie As String = "null"
        Dim ref_id_tipo_documento As String = "null"
        Dim ref_id_tipo_expediente As String = "null"
        Dim ref_id_tipo_unidad_documental As String = "null"
        If matri_gestion.ID_EXPEDIENTE <> 0 Then
            ref_id_tipo_unidad_documental = 2
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 2
        End If
        If matri_gestion.ID_UNIDAD_CONSERVACION <> 0 Then
            ref_id_tipo_unidad_documental = 1
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 1
        End If

        If matri_gestion.ID_TIPO_EXPEDIENTE <> 0 Then
            ref_id_tipo_expediente = matri_gestion.ID_TIPO_EXPEDIENTE
        End If
        If matri_gestion.ID_TIPODOCUMENTO <> 0 Then
            ref_id_tipo_documento = matri_gestion.ID_TIPODOCUMENTO
        End If
        If matri_gestion.ID_SUB_SERIE <> 0 Then
            ref_id_sub_serie = matri_gestion.ID_SUB_SERIE
        End If
        If nombre_area <> "" Then
            ref_nombre_area = "'" & nombre_area & "'"
        End If
        If matri_gestion.ID_CLASE_DOCUMENTO <> 0 Then
            ref_id_clase_documento = matri_gestion.ID_CLASE_DOCUMENTO
        End If
        If matri_gestion.ID_TIPO_UNIDAD_CONSERVACION <> 0 Then
            ref_id_tipo_unidad_conservacion = matri_gestion.ID_TIPO_UNIDAD_CONSERVACION
        End If
        If matri_gestion.ID_SERIE <> 0 Then
            ref_id_serie = matri_gestion.ID_SERIE
        End If
        If matri_gestion.ID_AREA <> 0 Then
            ref_id_area = matri_gestion.ID_AREA
        End If
        If matri_gestion.ID_EXPEDIENTE <> 0 Then
            ref_id_expediente = matri_gestion.ID_EXPEDIENTE
        End If
        If matri_gestion.ID_UNIDAD_CONSERVACION <> 0 Then
            ref_id_unidad_conservacion = matri_gestion.ID_UNIDAD_CONSERVACION
        End If
        If matri_gestion.EXPEDIENTE <> "" Then
            ref_expediente = "'" & matri_gestion.EXPEDIENTE & "'"
        End If
        If matri_gestion.NOMBRE_SERIE <> "" Then
            ref_nombre_serie = "'" & matri_gestion.NOMBRE_SERIE & "'"
        End If
        If matri_gestion.NOMBRE_SUB_SERIE <> "" Then
            ref_nombre_sub_serie = "'" & matri_gestion.NOMBRE_SUB_SERIE & "'"
        End If
        If matri_gestion.TIPODOCUMENTO <> "" Then
            ref_tipo_documento = "'" & matri_gestion.TIPODOCUMENTO & "'"
        End If
        If matri_gestion.UNIDAD_CONSERVACION <> "" Then
            ref_unidad_conserva = "'" & matri_gestion.UNIDAD_CONSERVACION & "'"
        End If
        If matri_gestion.CLASE_DOCUMENTO <> "" Then
            ref_clase_documento = "'" & matri_gestion.CLASE_DOCUMENTO & "'"
        End If
        If matri_gestion.FECHA_ELABORACION <> "" Then
            ref_fecha_elaboracion = "'" & matri_gestion.FECHA_ELABORACION & "'"
        End If
        Dim estado_archivo As Integer = 1
        If matri_gestion.ID_EXPEDIENTE <> 0 Or matri_gestion.ID_UNIDAD_CONSERVACION <> 0 Then
            estado_archivo = 0
        End If
        Dim up_modificacion As String = ""
        If estado_archivo = 0 Then
            up_modificacion = " ESTADO_DOCUMENTO_ARCHIVO=" & estado_archivo & ","
        End If
        Dim ref_hidden_selecion_actualiza_treview As Object = PAGE1.FindControl("hidden_selecion_actualiza_treview")
        If Not ref_hidden_selecion_actualiza_treview Is Nothing Then
            ref_hidden_selecion_actualiza_treview.value = matri_gestion.TIPODOCUMENTO
        End If
        Dim datos_insert_inventario As String = ""
        Dim sqlinventario As String = ""
        If option_inventario = 1 Then
            sqlinventario = "Update registro_producion_documental " &
            " set ID_AREA_DEPARTAMENTO=" & ref_id_area & "," &
            " ID_SERIE_DOCUMENTO=" & ref_id_serie & "," & up_modificacion &
            " SERIE_DOCUMENTO=" & ref_nombre_serie & "," &
            " ID_SUBSERIE_DOCUMENTO=" & ref_id_sub_serie & "," &
            " SUBSERIE_DOCUMENTO=" & ref_nombre_sub_serie & "," &
            " ID_TIPO_DOCUMENTO=" & ref_id_tipo_documento & "," &
            " DESCRIPCION_TIPO_DOCUMENTO=" & ref_tipo_documento & "," &
            " FULTEXT_DOCUMENTO='" & actualiza_fultex & "'," &
            " EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" & ref_id_expediente & "," &
            " EXPEDIENTE=" & ref_expediente & "," &
            " ID_TIPO_EXPEDIENTE=" & ref_id_tipo_expediente & "," &
            " ID_TIPO_UNIDAD_CONSERVACION=" & ref_id_tipo_unidad_conservacion & "," &
            " ID_UNIDAD_CONSERVACION=" & ref_id_unidad_conservacion & "," &
            " ID_CLASE_DOCUMENTO=" & ref_id_clase_documento & "," &
            " CLASEDOCUMENTO=" & ref_clase_documento & "," &
            " FECHA_ELABORACION=" & ref_fecha_elaboracion & "," &
            " UNIDADCONSERVA=" & ref_unidad_conserva & "," &
            " NOMBRE_AREA_DEPARTAMENTO=" & ref_nombre_area & "," &
            " ID_TIPO_UNIDAD_DOCUMENTAL=" & ref_id_tipo_unidad_documental &
            " where ID_DOCUMENTO_DOCUARCHI_ALMACEN=" & id_Imagen &
            " and NOMBRE_GABINETE='" & Nombre_Gabinete & "'"

        End If
        Dim update_gestion As String = ""
        Dim op_selecion_unidad As Integer = 0
        result = ref_Class_system1.Verfica_opcion_seleccion_unidad(op_selecion_unidad,
                                                                   Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If
        If op_selecion_unidad <> 0 Then
            update_gestion = "Update " & Nombre_Gabinete & " set ID_EXPEDIENTE=" & ref_id_expediente &
            ", ID_TIPO_EXPEDIENTE=" & ref_id_tipo_expediente & "," &
            " ID_TIPO_UNIDAD_CONSERVACION=" & ref_id_tipo_unidad_conservacion & "," &
            " ID_UNIDAD_CONSERVACION=" & ref_id_unidad_conservacion & "," &
            " ID_CLASE_DOCUMENTO=" & ref_id_clase_documento & "," &
            " ID_TIPO_UNIDAD_DOCUMENTAL=" & ref_id_tipo_unidad_documental

        End If
        Dim op_tabla_retension As Integer = 0
        result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(op_tabla_retension,
                                                                           Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If
        If op_tabla_retension <> 0 Then
            If update_gestion = "" Then
                update_gestion = "Update " & Nombre_Gabinete & " set ID_AREA=" & ref_id_area & "," &
                " ID_SERIE=" & ref_id_serie & "," &
                " ID_SUB_SERIE=" & ref_id_sub_serie & "," &
                " ID_TIPODOCUMENTO=" & ref_id_tipo_documento
            Else
                update_gestion = update_gestion & ", ID_AREA=" & ref_id_area & "," &
                                " ID_SERIE=" & ref_id_serie & "," &
                                " ID_SUB_SERIE=" & ref_id_sub_serie & "," &
                                " ID_TIPODOCUMENTO=" & ref_id_tipo_documento
            End If
        End If
        If update_gestion <> "" Then
            update_gestion = update_gestion & " where id=" & id_Imagen
        End If
        '-----------------------------------------------------------
        'Detectar cambio unidada conservacion o expediente
        '-----------------------------------------------------------
        Dim matri_gestion_antigua As estructure_gestion = Nothing
        'matri_gestion = Nothing
        matri_gestion_antigua.CLASE_DOCUMENTO = ""
        matri_gestion_antigua.EXPEDIENTE = ""
        matri_gestion_antigua.ID_AREA = 0
        matri_gestion_antigua.ID_CLASE_DOCUMENTO = 0
        matri_gestion_antigua.ID_EXPEDIENTE = 0
        matri_gestion_antigua.ID_SERIE = 0
        matri_gestion_antigua.ID_SUB_SERIE = 0
        matri_gestion_antigua.ID_TIPO_EXPEDIENTE = 0
        matri_gestion_antigua.ID_TIPO_UNIDAD_CONSERVACION = 0
        matri_gestion_antigua.ID_TIPODOCUMENTO = 0
        matri_gestion_antigua.ID_UNIDAD_CONSERVACION = 0
        matri_gestion_antigua.ID_USUARIO_GESTION = 0
        matri_gestion_antigua.TIPO_UNIDAD_DOCUMENTAL = 0
        matri_gestion_antigua.UNIDAD_CONSERVACION = ""
        matri_gestion_antigua.FECHA_ELABORACION = ""
        '-------------------------------------------------------------
        'Asigna datos a la estructura desde la base de datos
        '-------------------------------------------------------------
        Dim refclas2 As New ClassAlmacenamiento
        Dim ClassDaGabinete As New ClassDaGabinete
        If op_selecion_unidad <> 0 Then
            result = refclas2.Solicita_datos_unidad_conservacion_estructura_base_datos(matri_gestion_antigua, Nombre_Gabinete, id_Imagen)
            If result <> "YES" Then
                Actualiza_Indice_Imagen = result
                Exit Function
            End If
        End If
        If op_selecion_unidad <> 0 Then
            result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(id_Imagen, Nombre_Gabinete, matri_gestion_antigua)
            If result <> "YES" Then
                Actualiza_Indice_Imagen = result
                Exit Function
            End If
        End If
        If op_selecion_unidad <> 0 Then
            result = refclas2.Solicita_datos_tipo_documental_estructura_base_datos(matri_gestion_antigua, Nombre_Gabinete, id_Imagen)
            If result <> "YES" Then
                Actualiza_Indice_Imagen = result
                Exit Function
            End If
        End If
        If op_tabla_retension <> 0 Then
            result = refclas2.Solicita_datos_gestion_estructura_base_datos(matri_gestion_antigua, Nombre_Gabinete, id_Imagen)
            If result <> "YES" Then
                Actualiza_Indice_Imagen = result
                Exit Function
            End If
        End If
        tipo_documento = matri_gestion.TIPODOCUMENTO
        '----------------------------------------------------
        'Solicita ruta del documento
        '----------------------------------------------------
        Dim Route_cabinet As String = ""
        Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
        result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Route_cabinet,
                                                                   Nombre_Gabinete)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If

        Dim Route_document As String = ""
        result = ClassDaGabinete.Solicita_ruta_achivo_gabinete(id_Imagen,
                                                                   Nombre_Gabinete,
                                                                   Route_cabinet,
                                                                   Route_document)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If
        Route_document = Route_document.Replace("\", "/")
        '------------------------------------------------
        'Solicita datos del documento
        '------------------------------------------------
        Dim Class_system1 As New Class_system1
        Dim inventario_documental As Integer = 0
        Dim aplica_trd As Integer = 0
        Dim asigna_unidad As Integer = 0
        result = Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Nombre_Gabinete,
                                                                                                     inventario_documental,
                                                                                                     aplica_trd,
                                                                                                     asigna_unidad)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If
        Dim stru_paramter_image As stru_paramter_image = Nothing
        result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(Nombre_Gabinete,
                                                                 id_Imagen,
                                                                 stru_paramter_image,
                                                                 aplica_trd,
                                                                 1)
        If result <> "YES" Then
            Actualiza_Indice_Imagen = result
            Exit Function
        End If
        Dim ref_user As String = "null"
        If stru_paramter_image.USER <> "" Then
            ref_user = "'" & stru_paramter_image.USER & "'"
        End If
        Dim ref_Tipologia As String = "null"
        If stru_paramter_image.TIPODOCUMENTO <> "" Then
            ref_Tipologia = "'" & stru_paramter_image.TIPODOCUMENTO & "'"
        End If
        Dim update_exp_aterior As String = ""
        Dim update_exp_nuevo As String = ""
        Dim update_unidad_anterior As String = ""
        Dim update_unidad_nuevo As String = ""
        Dim caso_unidad As Integer = 0
        Dim myConnection As New MySqlConnection
        Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
        myConnection_da.Returna_Conexion_Mysql(myConnection)
        Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim mySqldatReader2 As MySqlDataReader
        Dim Switc As Integer = 0
        Try
            Dim refclas As New ClassAlmacenamiento
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            If op_selecion_unidad <> 0 Then

                '----------------------------------------------------------------------------------------------------
                'Caso (1) cambia expediente Decrementa exp antiguo incrementa exp nuevo
                '----------------------------------------------------------------------------------------------------
                If matri_gestion_antigua.ID_EXPEDIENTE <> matri_gestion.ID_EXPEDIENTE And matri_gestion.ID_EXPEDIENTE > 0 _
                 And matri_gestion_antigua.ID_EXPEDIENTE > 0 And caso_unidad = 0 Then
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If
                    Dim unidad_conserva_tipo_nuevo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If
                    update_exp_aterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                        " FROM expediente_archivo where ID_EXPEDIENTE = " _
                       & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"

                    update_exp_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                   & "'" & matri_gestion.ID_EXPEDIENTE & "' " & "for update"
                    '----------------------------------------------
                    'Decrementar expediente antiguo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    myCommand2.CommandText = update_exp_aterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación del expediente por conexión caso 1 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro del expediente caso 1 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 1
                        End If
                    End If
                    '----------------------------------------------
                    'Incrementa expediente nuevo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_exp_nuevo
                    mySqldatReader2 = myCommand2.ExecuteReader()
                    If mySqldatReader2 Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación del expediente por conexión caso 1 Incrementa expediente nuevo"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader2.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro del expediente caso 1 Incrementa expediente nuevo"
                        mySqldatReader2.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader2.Read()
                        Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                        Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                        mySqldatReader2.Close()
                    End If
                    update_sql = ""
                    If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                        update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                    End If
                    If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                        Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                        update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                    End If
                    myCommand2.CommandText = update_sql
                    Switc = myCommand2.ExecuteNonQuery()
                    If Switc = 0 Then
                        Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                        'mySqldatReader2.Close()
                        'mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        caso_unidad = 1
                        'mySqldatReader2.Close()
                        'mySqldatReader.Close()
                    End If

                End If

                '----------------------------------------------------------------------------------------------------
                'Caso (2) cambia unidad conservación decrementa unidad antigua incrementa unidad nueva
                '----------------------------------------------------------------------------------------------------
                If matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> matri_gestion.ID_UNIDAD_CONSERVACION And
                 matri_gestion.ID_UNIDAD_CONSERVACION > 0 And matri_gestion_antigua.ID_UNIDAD_CONSERVACION > 0 _
                 And caso_unidad = 0 Then
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If
                    Dim unidad_conserva_tipo_nuevo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If
                    update_unidad_anterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                       " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                      & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"


                    update_unidad_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                     " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                    & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' " & "for update"

                    '----------------------------------------------
                    'Decrementar unidad de conservacion antigua
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    myCommand2.CommandText = update_unidad_anterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 2 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro de la unidad de conservacion caso 2 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios de la unidad de conservación "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 2
                        End If

                    End If
                    '----------------------------------------------
                    'Incrementa unidad conservación nuevo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_unidad_nuevo
                    mySqldatReader2 = myCommand2.ExecuteReader()
                    If mySqldatReader2 Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación de la unidad  conexión caso 2 Incrementa unidad"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader2.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro de la unidad caso 2 Incrementa Incrementa unidad"
                        mySqldatReader2.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader2.Read()
                        Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                        Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                        mySqldatReader2.Close()
                    End If
                    update_sql = ""
                    If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                        Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                            mySqldatReader2.Close()
                            mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 2

                        End If
                    End If
                End If

                If matri_gestion_antigua.ID_EXPEDIENTE <> matri_gestion.ID_EXPEDIENTE And
                matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> matri_gestion.ID_UNIDAD_CONSERVACION And caso_unidad = 0 Then
                    '------------------------------------------------------------------------------------------------------------------
                    'Caso (3) cambia expediente a unidad de conservación decrementa exp antiguo e incrementa unidad conservacion nueva
                    '------------------------------------------------------------------------------------------------------------------
                    If matri_gestion.ID_UNIDAD_CONSERVACION > 0 And matri_gestion_antigua.ID_EXPEDIENTE > 0 Then
                        update_unidad_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                             " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                            & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' " & "for update"

                        update_exp_aterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                       " FROM expediente_archivo where ID_EXPEDIENTE = " _
                      & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"
                        Dim refclastrd As New ClassTrdDocumental
                        Dim unidad_conserva_tipo_antiguo As String = ""
                        result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                        If result <> "YES" Then
                            Actualiza_Indice_Imagen = result
                            Exit Function
                        End If
                        Dim unidad_conserva_tipo_nuevo As String = ""
                        result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                        If result <> "YES" Then
                            Actualiza_Indice_Imagen = result
                            Exit Function
                        End If
                        '----------------------------------------------
                        'Decrementar expediente antiguo caso 3
                        '---------------------------------------------
                        Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                        Dim Numero_Electronico_contenido_antiguo As Integer = 0
                        myCommand2.CommandText = update_exp_aterior
                        mySqldatReader = myCommand2.ExecuteReader()
                        If mySqldatReader Is Nothing Then
                            Actualiza_Indice_Imagen = "Imposible encontrar la identificación del expediente por conexión caso 3 decrementar"
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader.HasRows = False Then
                            Actualiza_Indice_Imagen = "Imposible Encontrar el registro del expediente caso 3 decrementar"
                            mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader.Read()
                            Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                            Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                            mySqldatReader.Close()
                        End If
                        Dim update_sql As String = ""
                        If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                            Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                            update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                            " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                        End If
                        If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                            Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                            update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                            " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                        End If
                        If update_sql <> "" Then
                            myCommand2.CommandText = update_sql
                            Switc = myCommand2.ExecuteNonQuery()
                            If Switc = 0 Then
                                Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            Else
                                caso_unidad = 3
                            End If
                        End If
                        '----------------------------------------------
                        'Incrementa unidad conservación nuevo caso 3
                        '---------------------------------------------
                        Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                        Dim Numero_Electronico_contenido_nuevo As Integer = 0
                        myCommand2.CommandText = update_unidad_nuevo
                        mySqldatReader2 = myCommand2.ExecuteReader()
                        If mySqldatReader2 Is Nothing Then
                            Actualiza_Indice_Imagen = "Imposible encontrar la identificación de la unidad  conexión caso 3 Incrementa unidad"
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader2.HasRows = False Then
                            Actualiza_Indice_Imagen = "Imposible Encontrar el registro de la unidad caso 3 Incrementa Incrementa unidad"
                            mySqldatReader2.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader2.Read()
                            Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                            Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                            mySqldatReader2.Close()
                        End If
                        update_sql = ""
                        If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                            Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                            update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                            " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                        End If
                        If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                            Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                            update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                            " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                        End If
                        If update_sql <> "" Then
                            myCommand2.CommandText = update_sql
                            Switc = myCommand2.ExecuteNonQuery()
                            If Switc = 0 Then
                                Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                                mySqldatReader2.Close()
                                mySqldatReader.Close()
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            Else
                                caso_unidad = 3

                            End If
                        End If


                    End If
                    '-----------------------------------------------------------------------------------------------------------
                    'Caso (4) cambia unidad de conservación a expediente ( incrementa exp nuevo, decrmenta unidad antigua
                    '-----------------------------------------------------------------------------------------------------------
                    If matri_gestion.ID_EXPEDIENTE > 0 And matri_gestion_antigua.ID_UNIDAD_CONSERVACION > 0 And caso_unidad = 0 Then
                        update_unidad_anterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                               " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                              & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"

                        update_exp_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                       " FROM expediente_archivo where ID_EXPEDIENTE = " _
                                      & "'" & matri_gestion.ID_EXPEDIENTE & "' " & "for update"
                        '---------------------------------------------------
                        'Retorna unidad de tipo documento
                        '---------------------------------------------------
                        Dim refclastrd As New ClassTrdDocumental
                        Dim unidad_conserva_tipo_antiguo As String = ""
                        result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                        If result <> "YES" Then
                            Actualiza_Indice_Imagen = result
                            Exit Function
                        End If
                        Dim unidad_conserva_tipo_nuevo As String = ""
                        result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                        If result <> "YES" Then
                            Actualiza_Indice_Imagen = result
                            Exit Function
                        End If
                        '----------------------------------------------
                        'Decrementar unidad de conservacion antigua
                        '----------------------------------------------
                        Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                        Dim Numero_Electronico_contenido_antiguo As Integer = 0
                        myCommand2.CommandText = update_unidad_anterior
                        mySqldatReader = myCommand2.ExecuteReader()
                        If mySqldatReader Is Nothing Then
                            Actualiza_Indice_Imagen = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 4 decrementar"
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader.HasRows = False Then
                            Actualiza_Indice_Imagen = "Imposible Encontrar el registro de la unidad de conservacion caso 4 decrementar"
                            mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader.Read()
                            Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                            Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                            mySqldatReader.Close()
                        End If
                        Dim update_sql As String = ""
                        If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                            Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                            update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                            " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                        End If
                        If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                            Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                            update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                            " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                        End If
                        If update_sql <> "" Then
                            myCommand2.CommandText = update_sql
                            Switc = myCommand2.ExecuteNonQuery()
                            If Switc = 0 Then
                                Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios de la unidad de conservación "
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            Else
                                caso_unidad = 4
                            End If

                        End If
                        '----------------------------------------------
                        'Incrementa expediente nuevo
                        '---------------------------------------------
                        Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                        Dim Numero_Electronico_contenido_nuevo As Integer = 0
                        myCommand2.CommandText = update_exp_nuevo
                        mySqldatReader2 = myCommand2.ExecuteReader()
                        If mySqldatReader2 Is Nothing Then
                            Actualiza_Indice_Imagen = "Imposible encontrar la identificación del expediente por conexión caso 4 Incrementa expediente nuevo"
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader2.HasRows = False Then
                            Actualiza_Indice_Imagen = "Imposible Encontrar el registro del expediente caso 4 Incrementa expediente nuevo"
                            mySqldatReader2.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader2.Read()
                            Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                            Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                            mySqldatReader2.Close()
                        End If
                        update_sql = ""
                        If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                            Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                            update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                            " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                        End If
                        If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                            Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                            update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                            " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                        End If
                        If update_sql <> "" Then
                            myCommand2.CommandText = update_sql
                            Switc = myCommand2.ExecuteNonQuery()
                            If Switc = 0 Then
                                Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                                'mySqldatReader2.Close()
                                'mySqldatReader.Close()
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            Else
                                caso_unidad = 4
                                'mySqldatReader2.Close()
                                'mySqldatReader.Close()
                            End If
                        End If
                    End If

                End If

                '---------------------------------------------------
                'Caso (5) limpia expediente decrementa exp antiguo
                '---------------------------------------------------
                If matri_gestion_antigua.ID_EXPEDIENTE <> matri_gestion.ID_EXPEDIENTE And
                matri_gestion.ID_EXPEDIENTE = 0 And matri_gestion.ID_UNIDAD_CONSERVACION = 0 And caso_unidad = 0 Then
                    update_exp_aterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                        " FROM expediente_archivo where ID_EXPEDIENTE = " _
                       & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If

                    '----------------------------------------------
                    'Decrementar expediente antiguo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    myCommand2.CommandText = update_exp_aterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación del expediente por conexión caso 1 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro del expediente caso 5 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 1
                        End If
                    End If


                End If

                '--------------------------------------------------------------
                'Caso (6) limpia unidad conservación decrementa unidad antigua
                '--------------------------------------------------------------
                If matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> matri_gestion.ID_UNIDAD_CONSERVACION And
                matri_gestion.ID_UNIDAD_CONSERVACION = 0 And matri_gestion.ID_EXPEDIENTE = 0 And caso_unidad = 0 Then
                    update_unidad_anterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                                              " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                                             & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If

                    '----------------------------------------------
                    'Decrementar unidad de conservacion antigua
                    '----------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    myCommand2.CommandText = update_unidad_anterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 6 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro de la unidad de conservacion caso 6 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios de la unidad de conservación caso (6) "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 6
                        End If

                    End If
                End If
                '--------------------------------------------------------
                'Caso (7) asigna expediente incremeta nuevo expediente
                '--------------------------------------------------------
                If matri_gestion_antigua.ID_EXPEDIENTE <> matri_gestion.ID_EXPEDIENTE And matri_gestion.ID_EXPEDIENTE > 0 _
                 And matri_gestion_antigua.ID_EXPEDIENTE = 0 And matri_gestion_antigua.ID_UNIDAD_CONSERVACION = 0 And caso_unidad = 0 Then

                    update_exp_aterior = ""
                    update_exp_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                   & "'" & matri_gestion.ID_EXPEDIENTE & "' " & "for update"

                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_nuevo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If

                    '----------------------------------------------
                    'Incrementa expediente nuevo
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_exp_nuevo
                    mySqldatReader2 = myCommand2.ExecuteReader()
                    If mySqldatReader2 Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación del expediente por conexión caso 1 Incrementa expediente nuevo"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader2.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro del expediente caso 1 Incrementa expediente nuevo"
                        mySqldatReader2.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader2.Read()
                        Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                        Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                        mySqldatReader2.Close()
                    End If
                    Dim update_sql = ""
                    If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                        update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                    End If
                    If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                        Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                        update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                        " where ID_EXPEDIENTE = " & "'" & matri_gestion.ID_EXPEDIENTE & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                            mySqldatReader2.Close()
                            'mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 7
                            'mySqldatReader2.Close()
                            'mySqldatReader.Close()
                        End If
                    End If

                End If
                '----------------------------------------------------------------------------
                'Caso (8) asigna unidad conservación incrementa nueva unidad de conservación
                '----------------------------------------------------------------------------
                If matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> matri_gestion.ID_UNIDAD_CONSERVACION And
                 matri_gestion.ID_UNIDAD_CONSERVACION > 0 And matri_gestion_antigua.ID_UNIDAD_CONSERVACION = 0 _
                 And matri_gestion_antigua.ID_EXPEDIENTE = 0 And caso_unidad = 0 Then
                    update_unidad_anterior = ""
                    update_unidad_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                     " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                    & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' " & "for update"
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_nuevo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nuevo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If

                    '----------------------------------------------
                    'Incrementa unidad conservación nuevo caso 8
                    '---------------------------------------------
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_unidad_nuevo
                    mySqldatReader2 = myCommand2.ExecuteReader()
                    If mySqldatReader2 Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación de la unidad  conexión caso 8 Incrementa unidad"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader2.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro de la unidad caso 8 Incrementa Incrementa unidad"
                        mySqldatReader2.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader2.Read()
                        Numero_Digitalizado_contenido_nuevo = mySqldatReader2.Item(0)
                        Numero_Electronico_contenido_nuevo = mySqldatReader2.Item(1)
                        mySqldatReader2.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_nuevo = "DIGITALIZADO" Then
                        Numero_Digitalizado_contenido_nuevo = Numero_Digitalizado_contenido_nuevo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_nuevo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_nuevo = "ELECTRONICO" Then
                        Numero_Electronico_contenido_nuevo = Numero_Electronico_contenido_nuevo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_nuevo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios del expediente "
                            mySqldatReader2.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 8

                        End If
                    End If
                End If

                '----------------------------------------------------------------------------------
                'Caso 9 cambia solo calse de documento unidad conservacion seleccionada
                '----------------------------------------------------------------------------------
                If matri_gestion_antigua.ID_UNIDAD_CONSERVACION = matri_gestion.ID_UNIDAD_CONSERVACION And
                matri_gestion.ID_EXPEDIENTE = matri_gestion_antigua.ID_EXPEDIENTE _
                And matri_gestion_antigua.ID_CLASE_DOCUMENTO <> matri_gestion.ID_CLASE_DOCUMENTO _
                And matri_gestion_antigua.ID_UNIDAD_CONSERVACION > 0 And caso_unidad = 0 Then

                    update_unidad_anterior = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                                                                 " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                                                                & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"
                    '---------------------------------------------------
                    'Retorna unidad de tipo documento
                    '---------------------------------------------------
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If
                    Dim unidad_conserva_tipo_nueva As String = ""
                    result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nueva)
                    If result <> "YES" Then
                        Actualiza_Indice_Imagen = result
                        Exit Function
                    End If
                    '--------------------------------------------------------------------
                    'Incrementa o decrementa numero de electrnicos o digitalizados
                    'de unidad de conservación
                    '-----------------------------------------------------------------------
                    Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                    Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                    Dim Numero_Electronico_contenido_antiguo As Integer = 0
                    Dim Numero_Electronico_contenido_nuevo As Integer = 0
                    myCommand2.CommandText = update_unidad_anterior
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        Actualiza_Indice_Imagen = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 9 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        Actualiza_Indice_Imagen = "Imposible Encontrar el registro de la unidad de conservacion caso 9 decrementar"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                        Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And unidad_conserva_tipo_nueva = "ELECTRONICO" Then
                        If pagi <= Numero_Digitalizado_contenido_antiguo Then
                            Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                        End If
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        ",NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If unidad_conserva_tipo_antiguo = "ELECTRONICO" And unidad_conserva_tipo_nueva = "DIGITALIZADO" Then
                        If pagi <= Numero_Electronico_contenido_antiguo Then
                            Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                        End If
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo + pagi
                        update_sql = "update unidad_conservacion Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                        ",NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                        " where ID_UNIDAD_CONSERVACION = " & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' "
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios de la unidad de conservación caso (6) "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 9
                        End If

                    End If

                End If
            End If
            '----------------------------------------------------------------------------------
            'Caso 10 cambia solo calse de documento expediente seleccionado
            '----------------------------------------------------------------------------------
            If matri_gestion_antigua.ID_UNIDAD_CONSERVACION = matri_gestion.ID_UNIDAD_CONSERVACION And
            matri_gestion.ID_EXPEDIENTE = matri_gestion_antigua.ID_EXPEDIENTE _
            And matri_gestion_antigua.ID_CLASE_DOCUMENTO <> matri_gestion.ID_CLASE_DOCUMENTO _
            And matri_gestion_antigua.ID_EXPEDIENTE > 0 And caso_unidad = 0 Then
                update_exp_aterior = ""
                update_exp_nuevo = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                " FROM expediente_archivo where ID_EXPEDIENTE = " _
               & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"
                '---------------------------------------------------
                'Retorna unidad de tipo documento
                '---------------------------------------------------
                Dim refclastrd As New ClassTrdDocumental
                Dim unidad_conserva_tipo_antiguo As String = ""
                result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_antiguo)
                If result <> "YES" Then
                    Actualiza_Indice_Imagen = result
                    Exit Function
                End If
                Dim unidad_conserva_tipo_nueva As String = ""
                result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion.ID_CLASE_DOCUMENTO, unidad_conserva_tipo_nueva)
                If result <> "YES" Then
                    Actualiza_Indice_Imagen = result
                    Exit Function
                End If
                '--------------------------------------------------------------------
                'Incrementa o decrementa numero de electrnicos o digitalizados
                'de expedientes
                '-----------------------------------------------------------------------
                Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
                Dim Numero_Digitalizado_contenido_nuevo As Integer = 0
                Dim Numero_Electronico_contenido_antiguo As Integer = 0
                Dim Numero_Electronico_contenido_nuevo As Integer = 0
                myCommand2.CommandText = update_exp_nuevo
                mySqldatReader = myCommand2.ExecuteReader()
                If mySqldatReader Is Nothing Then
                    Actualiza_Indice_Imagen = "Imposible encontrar la identificación la unidad de conservacion por conexión caso 10 decrementar"
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If mySqldatReader.HasRows = False Then
                    Actualiza_Indice_Imagen = "Imposible Encontrar el registro de la unidad de conservacion caso 10 decrementar"
                    mySqldatReader.Close()
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    mySqldatReader.Read()
                    Numero_Digitalizado_contenido_antiguo = mySqldatReader.Item(0)
                    Numero_Electronico_contenido_antiguo = mySqldatReader.Item(1)
                    mySqldatReader.Close()
                End If
                Dim update_sql As String = ""
                If unidad_conserva_tipo_antiguo = "DIGITALIZADO" And unidad_conserva_tipo_nueva = "ELECTRONICO" Then
                    If pagi <= Numero_Digitalizado_contenido_antiguo Then
                        Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo - pagi
                    End If
                    Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo + pagi
                    update_sql = "update expediente_archivo Set NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                    ",NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                    " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                End If
                If unidad_conserva_tipo_antiguo = "ELECTRONICO" And unidad_conserva_tipo_nueva = "DIGITALIZADO" Then
                    If pagi <= Numero_Electronico_contenido_antiguo Then
                        Numero_Electronico_contenido_antiguo = Numero_Electronico_contenido_antiguo - pagi
                    End If
                    Numero_Digitalizado_contenido_antiguo = Numero_Digitalizado_contenido_antiguo + pagi
                    update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido_antiguo &
                    ",NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Digitalizado_contenido_antiguo &
                    " where ID_EXPEDIENTE = " & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' "
                End If
                If update_sql <> "" Then
                    myCommand2.CommandText = update_sql
                    Switc = myCommand2.ExecuteNonQuery()
                    If Switc = 0 Then
                        Actualiza_Indice_Imagen = "Imposible Actualizar numero de folios de la unidad de conservación caso (10) "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        caso_unidad = 10
                    End If

                End If
            End If
            '------------------------------------------------------------
            'Verifica cambios gestion
            '-----------------------------------------------------------
            Dim cambio_gestion As Integer = 0
            If matri_gestion.CLASE_DOCUMENTO <> matri_gestion_antigua.CLASE_DOCUMENTO Then cambio_gestion = 1
            If matri_gestion.EXPEDIENTE <> matri_gestion_antigua.EXPEDIENTE Then cambio_gestion = 1
            If matri_gestion.FECHA_ELABORACION <> matri_gestion_antigua.FECHA_ELABORACION Then cambio_gestion = 1
            If matri_gestion.ID_AREA <> matri_gestion_antigua.ID_AREA Then cambio_gestion = 1
            If matri_gestion.ID_CLASE_DOCUMENTO <> matri_gestion_antigua.ID_CLASE_DOCUMENTO Then cambio_gestion = 1
            If matri_gestion.ID_EXPEDIENTE <> matri_gestion_antigua.ID_EXPEDIENTE Then cambio_gestion = 1
            If matri_gestion.ID_SERIE <> matri_gestion_antigua.ID_SERIE Then cambio_gestion = 1
            If matri_gestion.ID_SUB_SERIE <> matri_gestion_antigua.ID_SUB_SERIE Then cambio_gestion = 1
            If matri_gestion.ID_TIPO_EXPEDIENTE <> matri_gestion_antigua.ID_TIPO_EXPEDIENTE Then cambio_gestion = 1
            If matri_gestion.ID_TIPO_UNIDAD_CONSERVACION <> matri_gestion_antigua.ID_TIPO_UNIDAD_CONSERVACION Then cambio_gestion = 1
            If matri_gestion.ID_TIPODOCUMENTO <> matri_gestion_antigua.ID_TIPODOCUMENTO Then cambio_gestion = 1
            If matri_gestion.UNIDAD_CONSERVACION <> matri_gestion_antigua.UNIDAD_CONSERVACION Then cambio_gestion = 1

            'SqlUpdate As String = "UPDATE " & Nombre_Gabinete & " SET "
            '------------------------------------------------------------
            'Actualiza campos docuarchi plantilla y fulltex inventario
            '-------------------------------------------------------------
            If SqlUpdate <> "UPDATE " & Nombre_Gabinete & " SET " Then
                If option_inventario = 1 And id_inventario <> 0 Then
                    Dim sqlinventario_fultex = "Update registro_producion_documental " &
                    " set FULTEXT_DOCUMENTO='" & actualiza_fultex & "'" &
                    " where ID_DOCUMENTO_DOCUARCHI_ALMACEN=" & id_Imagen &
                    " and NOMBRE_GABINETE='" & Nombre_Gabinete & "'"
                    myCommand2.CommandText = sqlinventario_fultex
                    Switc = myCommand2.ExecuteNonQuery()
                    If Switc = 0 Then
                        Actualiza_Indice_Imagen = "Imposible actualizar fultex invnetario  : " & sqlinventario_fultex
                        'myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        caso_unidad = 30
                    End If
                End If
                myCommand2.CommandText = SqlUpdate
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_Indice_Imagen = "Imposible actualizar la tabla docuarchi cambios  : " & SqlUpdate
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    caso_unidad = 30
                End If
                Dim hor2 As New System.DateTime
                hor2 = Date.Now
                Dim hora As String = hor2.Hour.ToString & ":" & hor2.Minute.ToString & ":" & hor2.Second.ToString
                Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
                & "RUT_DOCU,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,RADICADO,ID_TAREA_WF,ID_RUTA_WF,USER_PROPIETARIO,TIPOLOGIA_DOCUMENTAL) VALUES ( "
                SqlTransac = SqlTransac & "'" & id_Imagen & "',"
                SqlTransac = SqlTransac & "'" & "EditarIndice" & "',"
                SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("DA_Login_Usuario") & "',"
                SqlTransac = SqlTransac & "'" & date1al & "',"
                SqlTransac = SqlTransac & "'" & Route_document & "',"
                SqlTransac = SqlTransac & "'" & Nombre_Gabinete & "',"
                SqlTransac = SqlTransac & "'" & datos_campo & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & "WORKFLOW'" &
                    ",'" & radicado & "'," & id_tarea_wf & "," & id_ruta_wf & "," & ref_user & "," & ref_Tipologia & ")"
                myCommand2.CommandText = SqlTransac
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_Indice_Imagen = "Imposible actualizar la tabla docuarchi cambios  : " & SqlUpdate
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    caso_unidad = 30
                End If
            End If
            '******************************************
            'Actualiza indice de invnetario documental
            '******************************************
            If option_inventario = 1 And id_inventario <> 0 And cambio_gestion <> 0 Then
                myCommand2.CommandText = sqlinventario
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_Indice_Imagen = "Imposible actualizar la tabla System  : " & sqlinventario
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    caso_unidad = 20
                End If
            End If
            '************************************************
            'Actualiza datos gestión documental  en gabinete  ojo cambio 
            'If update_gestion <> "" And cambio_gestion <> 0 Then
            '************************************************
            If update_gestion <> "" Then
                myCommand2.CommandText = update_gestion
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    Actualiza_Indice_Imagen = "Imposible actualizar datos gestion gabinete  : " & update_gestion
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    caso_unidad = 15
                End If
            End If
            Dim hor As String = Now
            Dim detalle_trans As String = ""
            Dim campos_trans As String = ""
            Dim isert_datos As String = ""
            If cambio_gestion <> 0 Then
                '********************************************************
                'Registra auditoria inventario
                '********************************************************
                If option_inventario <> 0 And id_inventario <> 0 Then
                    If matri_gestion.CLASE_DOCUMENTO <> matri_gestion_antigua.CLASE_DOCUMENTO Then
                        detalle_trans = "CAMBIA CLASE DOCUMENTO"
                        campos_trans = "CAMBIA CLASE (" & matri_gestion_antigua.CLASE_DOCUMENTO &
                        ") A CLASE (" & matri_gestion.CLASE_DOCUMENTO & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_EXPEDIENTE <> matri_gestion_antigua.ID_EXPEDIENTE Then
                        detalle_trans = "CAMBIA EXPEDIENTE"
                        campos_trans = "CAMBIA EXPEDIENTE (" & matri_gestion_antigua.EXPEDIENTE &
                       ") A EXPEDIENTE (" & matri_gestion.EXPEDIENTE & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.FECHA_ELABORACION <> matri_gestion_antigua.FECHA_ELABORACION Then
                        detalle_trans = "CAMBIA FECHA ELABORACION"
                        campos_trans = "CAMBIA FECHA ELABORACION (" & matri_gestion_antigua.FECHA_ELABORACION &
                      ") POR (" & matri_gestion.FECHA_ELABORACION & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_AREA <> matri_gestion_antigua.ID_AREA Then
                        detalle_trans = "CAMBIA AREA DOCUMENTO"
                        campos_trans = "CAMBIA AREA DOCUMENTO (" & matri_gestion_antigua.ID_AREA &
                      ") POR (" & matri_gestion.ID_AREA & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_CLASE_DOCUMENTO <> matri_gestion_antigua.ID_CLASE_DOCUMENTO Then
                        detalle_trans = "CAMBIA ID CLASE DOCUMENTO"
                        campos_trans = "CAMBIA ID CLASE DOCUMENTO (" & matri_gestion_antigua.ID_CLASE_DOCUMENTO &
                      ") POR (" & matri_gestion.ID_CLASE_DOCUMENTO & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_SERIE <> matri_gestion_antigua.ID_SERIE Then
                        detalle_trans = "CAMBIA SERIE DOCUMENTO"
                        campos_trans = "CAMBIA SERIE DOCUMENTO (" & matri_gestion_antigua.ID_SERIE &
                      ") POR (" & matri_gestion.ID_SERIE & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_SUB_SERIE <> matri_gestion_antigua.ID_SUB_SERIE Then
                        detalle_trans = "CAMBIA SUB SERIE DOCUMENTO"
                        campos_trans = "CAMBIA SUB SERIE DOCUMENTO (" & matri_gestion_antigua.ID_SUB_SERIE &
                      ") POR (" & matri_gestion.ID_SUB_SERIE & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_TIPODOCUMENTO <> matri_gestion_antigua.ID_TIPODOCUMENTO Then
                        detalle_trans = "CAMBIA TIPO DOCUMENTO"
                        campos_trans = "CAMBIA TIPO DOCUMENTO (" & matri_gestion_antigua.ID_TIPODOCUMENTO &
                      ") POR (" & matri_gestion.ID_TIPODOCUMENTO & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"
                        End If
                    End If
                    If matri_gestion.ID_TIPO_UNIDAD_CONSERVACION <> matri_gestion_antigua.ID_TIPO_UNIDAD_CONSERVACION Then
                        detalle_trans = "CAMBIA UNIDAD CONSERVACION"
                        campos_trans = "CAMBIA UNIDAD CONSERVACION (" & matri_gestion_antigua.UNIDAD_CONSERVACION &
                     ") POR (" & matri_gestion.UNIDAD_CONSERVACION & ")"
                        If isert_datos = "" Then
                            isert_datos = isert_datos & "('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"
                        Else
                            isert_datos = isert_datos & ", ('" & detalle_trans & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                                                     id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','WORKFLOW-WEB','" & campos_trans & "')"
                        End If

                    End If
                    update_gestion = "INSERT INTO ra_log_inventario (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_REGISTRO_PRODUCCION" &
                                     ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                     isert_datos
                    If isert_datos <> "" Then
                        myCommand2.CommandText = update_gestion
                        Switc = myCommand2.ExecuteNonQuery()
                        If Switc = 0 Then
                            Actualiza_Indice_Imagen = "Imposible registrar log inventario  : " & update_gestion
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            caso_unidad = 50
                        End If
                    End If
                End If
                '-----------------------------------------------------------------
                'Registra log expediente
                '-----------------------------------------------------------------
            End If
            If caso_unidad <> 0 Then
                myTrans.Commit()
            End If

            Actualiza_Indice_Imagen = "YES"

        Catch e As Exception
            Try

                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    Actualiza_Indice_Imagen = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_Indice_Imagen = "Error General " & e.Message
            Exit Function
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
        End Try

    End Function
    Function Asignar_unidad_conservacion_estructura_interface_indice_imagen(ByRef PAGE1 As Page,
                                                                            ByVal estructura As estructure_gestion) As String
        '********************************************************************
        'Funcion : Asigna datos la unidad de conservacion codigo unidad, tipo
        'unidad a la interface indice imagen desde el gabinete
        'Fecha : 2015-01-17
        '*********************************************************************
        Try
            Dim Result As String = ""
            Dim Hidden_id_unidad_conservacion As Object = Nothing
            Dim Hidden_id_tipo_unidad_conservacion As Object = Nothing
            Hidden_id_unidad_conservacion = PAGE1.FindControl("Hidden_id_unidad_conservacion")
            If Hidden_id_unidad_conservacion Is Nothing Then
                Asignar_unidad_conservacion_estructura_interface_indice_imagen = "Función Asignar_unidad_conservacion_estructura_interface_indice_imagen dice : Imposible encontrar el control Hidden_id_unidad_conservacion"
                Exit Function
            End If
            Hidden_id_tipo_unidad_conservacion = PAGE1.FindControl("Hidden_id_tipo_unidad_conservacion")
            If Hidden_id_tipo_unidad_conservacion Is Nothing Then
                Asignar_unidad_conservacion_estructura_interface_indice_imagen = "Función Asignar_unidad_conservacion_estructura_interface_indice_imagen dice : Imposible encontrar el control Hidden_id_tipo_unidad_conservacion"
                Exit Function
            End If
            Hidden_id_unidad_conservacion.value = estructura.ID_UNIDAD_CONSERVACION
            Hidden_id_tipo_unidad_conservacion.value = estructura.ID_TIPO_UNIDAD_CONSERVACION
            Asignar_unidad_conservacion_estructura_interface_indice_imagen = "YES"
        Catch ex As Exception
            Asignar_unidad_conservacion_estructura_interface_indice_imagen = "Inconsistencia Función Asignar_unidad_conservacion_estructura_interface_indice_imagen " & ex.Message
        End Try
    End Function
    Public Function Actualiza_Indice_Imagen(ByVal id_Imagen As String,
                                            ByVal Nombre_Gabinete As String,
                                            ByRef PAGE1 As Page) As String
        Try
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim SqlUpdate As String = "UPDATE " & Nombre_Gabinete & " SET "
            Dim Sql_consulta = "SELECT CAMPO,TIPO FROM " &
               "DETALLE_GABIENETE " &
               "WHERE GABINETE='" & Nombre_Gabinete & "' AND VISIBLE=1 ORDER BY IDENTI"

            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Actualiza_Indice_Imagen = "Funcion  Visualiza_Idice_Documento WF-01 Mensaje DBMS" & Resulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Actualiza_Indice_Imagen = "Imposible encontrar los campos para gabinete : " & Nombre_Gabinete
                Exit Function
            End If
            Dim Matri_campo_nombre As String = ""
            Dim Matri_Campos_Gabinete() As String
            Erase Matri_Campos_Gabinete
            For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                'ReDim Preserve Datos_Imagen(I)
                ReDim Preserve Matri_Campos_Gabinete(y)

                Matri_Campos_Gabinete(y) = Datset.Tables(0).Rows(y).Item(0).ToString & "|" & Datset.Tables(0).Rows(y).Item(1).ToString
                If y = 0 Then
                    Matri_campo_nombre = Datset.Tables(0).Rows(y).Item(0).ToString
                Else
                    Matri_campo_nombre = Matri_campo_nombre & "," & Datset.Tables(0).Rows(y).Item(0).ToString
                End If
            Next


            'Me.ScriptManager1.AsyncPostBackSourceElementID()
            Dim Elimina As String = ""
            Dim starindex As Integer = 0

            For i As Integer = 0 To Matri_Campos_Gabinete.Count - 1
                Dim splitdate() As String
                splitdate = Split(Matri_Campos_Gabinete(i).ToString, "|")
                Dim Objet As Object = PAGE1.Form.FindControl(splitdate(0))
                'For Each Objet In tabler.Controls
                'For Each Objet1 In Objet.Controls
                If Not Objet Is Nothing Then

                    If splitdate(1) = "INT" Then
                        If Objet.text = "" Then
                            SqlUpdate = SqlUpdate & splitdate(0) & "=" & "NULL,"
                        Else
                            SqlUpdate = SqlUpdate & splitdate(0) & "=" & Replace(Objet.text, "'", "") & ","
                        End If


                    End If
                    '------------------------------
                    'Verifica formato string
                    '------------------------------
                    If splitdate(1) <> "INT" And splitdate(1) <> "DATE" Then

                        If Objet.text <> "" Then
                            SqlUpdate = SqlUpdate & splitdate(0) & "='" & Replace(Objet.text, "'", "") & "',"
                        Else
                            SqlUpdate = SqlUpdate & splitdate(0) & "=NULL,"
                        End If
                    End If
                    '-----------------------------
                    'Verifica el formato fecha
                    '-----------------------------
                    Dim Result_Formato_fecha As String = ""
                    Dim Matriz_Error() As String
                    If splitdate(1) = "DATE" Then
                        If Objet.text <> "" Then
                            Result_Formato_fecha = ClassGestionFechas.Verifi_campo_fecha_Form6(Objet.text)
                            Erase Matriz_Error

                            Matriz_Error = Split(Result_Formato_fecha, "_")
                            'Verifica el formato general de la fecha
                            If Matriz_Error(0) = "CI" Then
                                Actualiza_Indice_Imagen = "Error Formato fecha " & Matriz_Error(1)
                                Exit Function
                            End If
                            'Verifica el formato general del dia
                            If Matriz_Error(0) = "ED" Then
                                Actualiza_Indice_Imagen = "Error Formato fecha " & Matriz_Error(1)
                                Exit Function
                            End If
                            'Verifica el formato general del mes
                            If Matriz_Error(0) = "EM" Then
                                Actualiza_Indice_Imagen = "Error Formato fecha " & Matriz_Error(1)
                                Exit Function
                            End If
                            SqlUpdate = SqlUpdate & splitdate(0) & "='" & Replace(Objet.text, "'", "") & "',"
                        Else
                            SqlUpdate = SqlUpdate & splitdate(0) & "=" & "NULL,"
                        End If

                    End If
                End If
                'Next
                'Next
            Next
            starindex = SqlUpdate.Length - 1
            Elimina = SqlUpdate.ToString.Substring(starindex)
            If Elimina = "," Then
                SqlUpdate = Left(SqlUpdate, starindex)
            End If
            SqlUpdate = SqlUpdate & " WHERE ID=" & id_Imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim result = ref.SELECTION_INSERT_COMMAND(SqlUpdate)
            If result <> "YES" Then
                Actualiza_Indice_Imagen = " Error Actualizando datos tarea   " & SqlUpdate
                Return Actualiza_Indice_Imagen
                'Exit Function
            Else

            End If
            Actualiza_Indice_Imagen = "YES"
        Catch ex As Exception
            Actualiza_Indice_Imagen = "Inconsistencia general funcion Actualiza_Indice_Imagen " & ex.Message
        End Try
    End Function

    Function Asigna_datos_gestion_estructura_de_gabinete(ByVal id_imagen As Integer,
                                                         ByRef estru_gestion As estructure_gestion,
                                                         ByVal Nombre_Gabinete As String) As String
        '**********************************************************
        'Función : Asigna los datos de gestion a la estructura
        'desde el gabinete
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2015-01-17 Modifiicado para web 2015-06-25
        '**********************************************************
        Try
            Dim Parametro_Consulta As String = "Select " &
            "ID_EXPEDIENTE,ID_TIPO_EXPEDIENTE,ID_UNIDAD_CONSERVACION,ID_TIPO_UNIDAD_CONSERVACION,ID_TIPO_UNIDAD_DOCUMENTAL," &
            "ID_CLASE_DOCUMENTO from " & Nombre_Gabinete & " Where ID=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(Nombre_Gabinete)
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Asigna_datos_gestion_estructura_de_gabinete = "Funcion Asigna_datos_gestion_estructura_de_gabinete dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_datos_gestion_estructura_de_gabinete = "Imposible Encontrar en el gabienete el id de la imagen " & id_imagen
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estru_gestion.ID_EXPEDIENTE = 0
                Else
                    estru_gestion.ID_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    estru_gestion.ID_TIPO_EXPEDIENTE = 0
                Else
                    estru_gestion.ID_TIPO_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    estru_gestion.ID_UNIDAD_CONSERVACION = 0
                Else
                    estru_gestion.ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    estru_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
                Else
                    estru_gestion.ID_TIPO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    estru_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
                Else
                    estru_gestion.ID_TIPODOCUMENTO = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    estru_gestion.ID_CLASE_DOCUMENTO = 0
                Else
                    estru_gestion.ID_CLASE_DOCUMENTO = Datset.Tables(0).Rows(0).Item(5)
                End If
                Asigna_datos_gestion_estructura_de_gabinete = "YES"
                Exit Function

            End If
        Catch ex As Exception
            Asigna_datos_gestion_estructura_de_gabinete = "Inconsistencia función Asigna_datos_gestion_estructura_de_gabinete " & ex.Message
        End Try
    End Function
    Function Asignar_tipo_documental_interface_indice(ByRef page1 As Page,
                                                      ByVal id_documento As Integer) As String
        '**********************************************************************
        'Funcion : asigna datos tipo documental interface de indice
        'fecha : 2015-01-05
        'Ing :Miguel Angel Urueta
        '**********************************************************************
        Try
            Dim Result As String = ""
            Dim Hidden_id_tipo As Object = Nothing
            Hidden_id_tipo = page1.FindControl("Hidden_id_tipo")
            If Hidden_id_tipo Is Nothing Then
                Asignar_tipo_documental_interface_indice = "Función Asignar_tipo_documental_interface_indice dice : imposible encontrar el control Hidden_id_tipo "
                Exit Function
            End If
            Hidden_id_tipo.value = id_documento
            Asignar_tipo_documental_interface_indice = "YES"
        Catch ex As Exception
            Asignar_tipo_documental_interface_indice = "Inconsistencia funcion Asignar_tipo_documental_interface_indice " & ex.Message
        End Try
    End Function
    Function Genera_interface_indice_documento(ByVal id_iamgen As String,
                                               ByVal NombreGabi As String,
                                               ByRef Page1 As Page,
                                               ByRef nombre As String,
                                               ByRef pane As Panel,
                                               ByRef Update As UpdatePanel,
                                               ByVal activa_botones_gestion As Integer,
                                               Optional ByVal evalua_editar_indice_wf As Integer = 0) As String
        Try
            Dim ref As New ClassListandoTareas
            '-------------------------------------------
            'Verifica permisos para editar imagen
            '-------------------------------------------
            If evalua_editar_indice_wf = 0 Then
                If HttpContext.Current.Session("Editar_Indice_Imagen") = 0 Then
                    Genera_interface_indice_documento = "YES"
                    Exit Function
                End If
            End If

            Dim Result As String = ""
            Dim I2 As Integer = 0
            Dim Sql_consulta = "SELECT CAMPO,TIPO,SISTEMA,VISIBLE,ESTADO FROM " &
                "DETALLE_GABIENETE " &
                "WHERE GABINETE='" & NombreGabi & "' AND VISIBLE=1 ORDER BY IDENTI"
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            Result = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Genera_interface_indice_documento = "Funcion  Visualiza_Idice_Documento WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Genera_interface_indice_documento = "Imposible encontrar los campos para gabinete : " & NombreGabi
                Exit Function
            End If
            Dim Matri_campo_nombre As String = ""
            Dim Matri_Campos_Gabinete() As String
            Erase Matri_Campos_Gabinete
            For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                ReDim Preserve Matri_Campos_Gabinete(y)
                Matri_Campos_Gabinete(y) = Datset.Tables(0).Rows(y).Item(0).ToString
                If Datset.Tables(0).Rows(y).IsNull(1) = False Then
                    Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & Datset.Tables(0).Rows(y).Item(1).ToString
                Else
                    Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & ""
                End If
                If Datset.Tables(0).Rows(y).IsNull(2) = False Then
                    Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & Datset.Tables(0).Rows(y).Item(2).ToString
                Else
                    Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & ""
                End If
                If Datset.Tables(0).Rows(y).IsNull(3) = False Then
                    Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & Datset.Tables(0).Rows(y).Item(2).ToString
                Else
                    Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & ""
                End If
                If Datset.Tables(0).Rows(y).IsNull(4) = False Then
                    Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & Datset.Tables(0).Rows(y).Item(4).ToString
                Else
                    Matri_Campos_Gabinete(y) = Matri_Campos_Gabinete(y) & "|" & ""
                End If
                If y = 0 Then
                    Matri_campo_nombre = Datset.Tables(0).Rows(y).Item(0).ToString
                Else
                    Matri_campo_nombre = Matri_campo_nombre & "," & Datset.Tables(0).Rows(y).Item(0).ToString
                End If
            Next
            '********************************************
            'Consulta opcion aplica trd
            '*******************************************
            Dim estru_gestion As estructure_gestion
            estru_gestion = Nothing
            'Dim refclasinterfacealmacen As New ClassInterfaceAlmacenamiento
            Dim refclastrd As New ClassTrdDocumental
            Dim opt_tabla_retencion As Integer = 0
            Dim option_inventario As Integer = 0
            Dim id_inventario As Integer = 0
            Dim opt_seleccion_unidad As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") <> 0 Then
                Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(opt_tabla_retencion,
                                                                               NombreGabi)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If
                '************************************************
                'Consulta opcion selecciona unidad documental
                '************************************************
                Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(opt_seleccion_unidad,
                                                                               NombreGabi)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If
                '----------------------------------------------------
                'Verfica si esta activado invnetario documental
                '-----------------------------------------------------       
                Dim refclasalmacen As New ClassTrdDocumental
                Result = ref_Class_system1.VerificaOpcionAplicarInventarioDocumental(option_inventario,
                                                                                     NombreGabi)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If
                If option_inventario = 1 Then
                    '-----------------------------------------------------
                    'Retorna el id del inventario del documento
                    '----------------------------------------------------
                    Result = verifica_exitencia_valor_invnetario_gabinete(NombreGabi,
                                                                          id_iamgen,
                                                                          id_inventario)
                    If Result <> "YES" Then
                        Genera_interface_indice_documento = Result
                        Exit Function
                    End If

                End If
            End If
            '***********************************************
            'Consulta datos de la imagen
            '***********************************************
            Sql_consulta = "SELECT " & Matri_campo_nombre & " FROM " &
              NombreGabi &
              " WHERE ID=" & id_iamgen
            Result = ref2.SELECTION_SELECT_FIELDA(Sql_consulta,
                                                  Datset)
            If Result <> "YES" Then
                Genera_interface_indice_documento = "Funcion  Visualiza_Idice_Documento WF-02 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Genera_interface_indice_documento = "Imposible encontrar datos para la imagen : " & id_iamgen
                Exit Function
            End If
            For y As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                For zi As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    Dim spl() As String = Matri_Campos_Gabinete(zi).Split("|")
                    Dim valortemporal As String = Datset.Tables(0).Rows(y).Item(zi).ToString
                    Dim ValorFinal As String = ""
                    '----------------------------------
                    'formateo de la fecha a mysql
                    '----------------------------------
                    If Trim(spl(1)) = "DATE" Then
                        If valortemporal.ToString.Length < 10 Then
                            ValorFinal = valortemporal
                        Else
                            ValorFinal = Left(valortemporal, 10)
                            Dim splitcampo() As String = Split(ValorFinal, "/")
                            ValorFinal = splitcampo(2) & "-" & splitcampo(1) & "-" & splitcampo(0)
                        End If
                    Else
                        ValorFinal = valortemporal
                    End If
                    Matri_Campos_Gabinete(zi) = Matri_Campos_Gabinete(zi) & "|" & ValorFinal
                Next
            Next

            '------------------------------------------------------------
            'Asigna datos id tabla de retención opt_tabla_retencion 
            '------------------------------------------------------------
            Dim DaGabinete As New ClassDaGabinete
            If opt_tabla_retencion <> 0 Then
                Result = DaGabinete.Asigna_datos_trd_estructura_de_gabinete(id_iamgen,
                                                                            estru_gestion,
                                                                            NombreGabi)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If
                'If Page1.IsPostBack = False Then
                Result = Me.Asignar_trd_estructura_interface_indice_imagen(Page1,
                                                                          estru_gestion)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If

            End If
            '************************************************
            'Consulta datos de gestión desde el gabinete
            '************************************************
            If opt_seleccion_unidad <> 0 Then
                Result = Me.Asigna_datos_gestion_estructura_de_gabinete(id_iamgen,
                                                                        estru_gestion,
                                                                        NombreGabi)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If

                Result = Me.Asignar_expediente_estructura_interface_indice_imagen(Page1,
                                                                                  estru_gestion)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If

                Result = Me.Asignar_unidad_conservacion_estructura_interface_indice_imagen(Page1,
                                                                                           estru_gestion)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If
                Result = Me.Asignar_tipo_documental_interface_indice(Page1,
                                                                     estru_gestion.ID_CLASE_DOCUMENTO)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If
            End If
            'Genera_interface_indice_documento = "YES"
            'Exit Function
            Dim Table As New Table
            Table.ID = "raw_some_table"
            Dim objRow As TableRow
            Dim objCell As TableCell
            Dim m_TextBoxes() As TextBox = {}
            Dim LabelBox() As Label = {}
            Dim Hidden_image_gabinete As HtmlInputHidden = Page1.FindControl("Hidden_image_gabinete")
            objRow = New TableRow
            Hidden_image_gabinete.Value = id_iamgen & "|" & NombreGabi
            objCell = New TableCell
            Dim Icontr As Integer = 0
            Dim z As Integer = 0
            '***********************************
            'Creaccion label gabinete
            '***********************************
            ReDim Preserve LabelBox(z)
            LabelBox(z) = New Label
            LabelBox(z).ID = "labelGABI"
            LabelBox(z).Text = NombreGabi
            LabelBox(z).CssClass = "p6"
            '***********************************
            'Agrega boton y label
            '***********************************
            objCell.Controls.Add(LabelBox(z))
            objRow.Cells.Add(objCell)
            Table.Rows.Add(objRow)
            Dim Matri_CampoES() As String
            Erase Matri_CampoES
            For z = 0 To UBound(Matri_Campos_Gabinete)
                ReDim Preserve m_TextBoxes(z)
                ReDim Preserve LabelBox(z + 1)
                LabelBox(z + 1) = New Label
                LabelBox(z).CssClass = "h6 font-weight-light"
                m_TextBoxes(z) = New TextBox
                m_TextBoxes(z).CssClass = "form control"
                Erase Matri_CampoES
                Matri_CampoES = Matri_Campos_Gabinete(z).Split("|")
                If Matri_CampoES(0) = "" Then
                    LabelBox(z + 1).Text = "SIN CAMPO"
                    m_TextBoxes(z).Text = "SIN CAMPO"
                    m_TextBoxes(z).ID = "SIN CAMPO-Z"
                Else
                    If Matri_CampoES(0) = "ENLASE" Then
                        LabelBox(z + 1).Text = "ENLACE"
                    Else
                        LabelBox(z + 1).Text = Matri_CampoES(0)
                    End If
                    'LabelBox(z + 1).Text = Matri_CampoES(0)
                    m_TextBoxes(z).Text = Matri_CampoES(5)
                    m_TextBoxes(z).ID = Matri_CampoES(0).ToString
                End If
                Result = agregar_auto_complete_docuarchi(m_TextBoxes(z).ID, pane, "GetPosiblesDatosGabinete", NombreGabi, Matri_CampoES(0).ToString)
                If Result <> "YES" Then
                    Genera_interface_indice_documento = Result
                    Exit Function
                End If
                If Matri_CampoES(4) = 1 Then
                    m_TextBoxes(z).Enabled = False
                End If
                If Matri_CampoES(0) = "ENLASE" Then
                    m_TextBoxes(z).Enabled = False
                End If
                If Matri_CampoES(0) = "NUMERORADICA" Then
                    m_TextBoxes(z).Enabled = False
                End If
                '------------------------------------------------------------------------
                objRow = New TableRow()
                objCell = New TableCell
                objCell.CssClass = "pt-2"
                objCell.Controls.Add(LabelBox(z + 1))
                objRow.Cells.Add(objCell)
                Table.Rows.Add(objRow)
                objRow = New TableRow()
                objCell = New TableCell
                objCell.Controls.Add(m_TextBoxes(z))
                objRow.Cells.Add(objCell)
                '-------------------------------------------------------------------------
                Dim boton_trd As New Button
                boton_trd.ID = "boton_trd"
                boton_trd.Text = "T"
                boton_trd.ToolTip = "Selecciona tabla retención documental "
                boton_trd.Attributes.Add("onclick", "seter_size_hiden();")
                Dim boton_trd_restore As New Button
                boton_trd_restore.ID = "boton_trd_restore"
                boton_trd_restore.Text = "R"
                boton_trd_restore.ToolTip = "Restaura tabla retención documental "
                If opt_tabla_retencion = 1 Then
                    If m_TextBoxes(z).ID = "NOMBRESERIE" And activa_botones_gestion = 1 Then
                        m_TextBoxes(z).BackColor = Color.PaleGoldenrod
                        'boton_trd.BackColor = Color.PaleGoldenrod
                        'boton_trd_restore.BackColor = Color.PaleGoldenrod
                        'objCell.Controls.Add(boton_trd)
                        'objCell.Controls.Add(boton_trd_restore)
                        'AddHandler boton_trd.Click, AddressOf _
                        'comman_trd_clik
                        'AddHandler boton_trd_restore.Click, AddressOf _
                        'comman_trd_restore_clik
                    End If
                End If
                If m_TextBoxes(z).ID = "NOMBRESUBSERIE" Then
                    m_TextBoxes(z).BackColor = Color.PaleGoldenrod
                End If
                If m_TextBoxes(z).ID = "TIPODOCUMENTO" Then
                    m_TextBoxes(z).BackColor = Color.PaleGoldenrod
                End If
                Dim boton_clase_documento As New Button
                boton_clase_documento.ID = "boton_clase_documento"
                boton_clase_documento.ToolTip = "Selecciona tipo documento, unidad documental simple"
                boton_clase_documento.Text = "C"
                Dim boton_clase_documento_restore As New Button
                boton_clase_documento_restore.ID = "boton_clase_documento_restore"
                boton_clase_documento_restore.ToolTip = "Restaura tipo documento, unidad documental simple"
                boton_clase_documento_restore.Text = "R"
                '-----expdiente
                Dim boton_expediente As New Button
                boton_expediente.ID = "boton_expediente"
                boton_expediente.Text = "E"
                boton_expediente.Attributes.Add("onclick", "seter_size_hiden();")
                boton_expediente.ToolTip = "Selecciona unidad compleja, compuesta (Expediente)"
                Dim boton_expediente_restore As New Button
                boton_expediente_restore.ID = "boton_expediente_restore"
                boton_expediente_restore.Text = "R"
                boton_expediente_restore.ToolTip = "Restaura unidad compleja, compuesta (Expediente)"
                '----unidad conservacion
                Dim boton_unidad_conserva As New Button
                boton_unidad_conserva.ID = "boton_unidad_conserva"
                boton_unidad_conserva.Text = "U"
                boton_unidad_conserva.ToolTip = "Selecciona unidad conservación (Carpeta, legajo, tómo, etc...)"
                Dim boton_fecha_elaboracion As New Button
                boton_fecha_elaboracion.ID = "boton_fecha_elaboracion"
                boton_fecha_elaboracion.Text = "F"
                boton_fecha_elaboracion.ToolTip = "Selecciona fecha elaboración del documento "
                Dim boton_fecha_elaboracion_restore As New Button
                boton_fecha_elaboracion_restore.ID = "boton_fecha_elaboracion_restore"
                boton_fecha_elaboracion_restore.Text = "R"
                boton_fecha_elaboracion_restore.ToolTip = "Restaura fecha elaboración del documento "
                Dim refclas_radicado As New ClassRadicador
                If opt_seleccion_unidad = 1 Then
                    If m_TextBoxes(z).ID = "EXPEDIENTE" And activa_botones_gestion = 1 Then
                        m_TextBoxes(z).BackColor = Color.Pink
                        ' boton_expediente.BackColor = Color.Pink
                        ' boton_expediente_restore.BackColor = Color.Pink
                        ' objCell.Controls.Add(boton_expediente)
                        ' objCell.Controls.Add(boton_expediente_restore)
                        ' AddHandler boton_expediente.Click, AddressOf _
                        ' comman_expediente_clik
                        ' AddHandler boton_expediente_restore.Click, AddressOf _
                        'comman_expediente_restore_clik
                    End If
                    If m_TextBoxes(z).ID = "CLASEDOCUMENTO" And activa_botones_gestion = 1 Then
                        m_TextBoxes(z).BackColor = Color.GreenYellow
                        ' boton_clase_documento.BackColor = Color.GreenYellow
                        ' boton_clase_documento_restore.BackColor = Color.GreenYellow
                        ' objCell.Controls.Add(boton_clase_documento)
                        ' objCell.Controls.Add(boton_clase_documento_restore)
                        ' AddHandler boton_clase_documento.Click, AddressOf _
                        ' comman_tipo_documemnto_clik
                        ' AddHandler boton_clase_documento_restore.Click, AddressOf _
                        'comman_tipo_documemnto_restore_clik
                    End If
                    If m_TextBoxes(z).ID = "UNIDADCONSERVA" And activa_botones_gestion = 1 Then
                        m_TextBoxes(z).BackColor = Color.PaleGreen
                        'objCell.Controls.Add(boton_unidad_conserva)
                    End If
                    If m_TextBoxes(z).ID = "FECHAELABORACION" Then
                        boton_fecha_elaboracion.Text = "#"
                        objCell.Controls.Add(boton_fecha_elaboracion)
                        boton_fecha_elaboracion.ID = "Fecha_ela_" & Matri_CampoES(0)
                        boton_fecha_elaboracion.ToolTip = "Selecciona fecha de " & Matri_CampoES(0)
                        boton_fecha_elaboracion.Attributes.Add("class", "ml-1 btn btn-success border-0")
                        boton_fecha_elaboracion.Attributes.Add("font-size", "10px")
                        boton_fecha_elaboracion.Attributes.Add("title", "formato aaaa mm dd")
                        Result = refclas_radicado.Agregar_Calendar(boton_fecha_elaboracion.ID, m_TextBoxes(z).ID, pane)
                        m_TextBoxes(z).Enabled = True
                        m_TextBoxes(z).CssClass = "date_indice"
                        m_TextBoxes(z).Attributes.Add("onkeypress", "GetChar (event);")
                        m_TextBoxes(z).Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                        m_TextBoxes(z).Attributes.Add("placeholder", "yyyy mm dd")
                        objCell.Controls.Add(boton_fecha_elaboracion)
                    End If
                End If
                If Matri_CampoES(1) = "DATE" And m_TextBoxes(z).ID <> "FECHAELABORACION" Then
                    boton_fecha_elaboracion.Text = "#"
                    objCell.Controls.Add(boton_fecha_elaboracion)
                    boton_fecha_elaboracion.ID = "Fecha_ela_" & Matri_CampoES(0)
                    boton_fecha_elaboracion.ToolTip = "Selecciona fecha de " & Matri_CampoES(0)
                    boton_fecha_elaboracion.Attributes.Add("class", "ml-1 btn btn-success border-0")
                    boton_fecha_elaboracion.Attributes.Add("font-size", "10px")
                    boton_fecha_elaboracion.Attributes.Add("title", "formato aaaa mm dd")
                    Result = refclas_radicado.Agregar_Calendar(boton_fecha_elaboracion.ID, m_TextBoxes(z).ID, pane)
                    m_TextBoxes(z).CssClass = "date_indice"
                    m_TextBoxes(z).Attributes.Add("onkeypress", "GetChar (event);")
                    m_TextBoxes(z).Attributes.Add("onkeypress", "return validate_fecha(event,this);")
                    m_TextBoxes(z).Attributes.Add("placeholder", "yyyy mm dd")
                End If
                If Matri_CampoES(1) = "INT" Then
                    m_TextBoxes(z).CssClass = "date_indice"
                    m_TextBoxes(z).Attributes.Add("onkeypress", "GetChar (event);")
                    m_TextBoxes(z).Attributes.Add("onkeypress", "return validate_numero(event,this);")
                End If
                Table.Rows.Add(objRow)
            Next
            nombre = m_TextBoxes(0).ID
            pane.Controls.Add(Table)
            Dim tribOTON1 As New AsyncPostBackTrigger()
            tribOTON1.ControlID = "Button_actualiza_hiden_Expediente"
            Update.Triggers.Add(tribOTON1)
            Dim hiden As HtmlInputHidden = Page1.FindControl("Hiddenheih")
            pane.Height = Val(hiden.Value)
            Update.Update()
            Genera_interface_indice_documento = "YES"
        Catch ex As Exception
            Genera_interface_indice_documento = "Inconsistencia general funcion Genera_interface_indice_documento " & ex.Message
        End Try
    End Function
    Function Asignar_expediente_estructura_interface_indice_imagen(ByRef page1 As Page,
                                                                   ByVal estructura As estructure_gestion) As String
        '***************************************************************
        'Funcion : Asigna datos del expediente codigo expediente, tipo
        'expediente a la interface desde el gabinete
        'Fecha : 2015-01-17
        '***************************************************************
        Try

            Dim Result As String = ""
            Dim Hidden_id_tipo_expediente As Object = Nothing
            Dim Hidden_id_expediente As Object = Nothing
            Hidden_id_tipo_expediente = page1.FindControl("Hidden_id_tipo_expediente")
            If Hidden_id_tipo_expediente Is Nothing Then
                Asignar_expediente_estructura_interface_indice_imagen = "Función Asigna_datos_expediente_estructura dice : imposible encontrar el control Hidden_id_tipo_expediente"
                Exit Function
            End If
            Hidden_id_expediente = page1.FindControl("Hidden_id_expediente")
            If Hidden_id_expediente Is Nothing Then
                Asignar_expediente_estructura_interface_indice_imagen = "Función Asignar_expediente_estructura_interface_indice_imagen dice : imposible encontrar el control Hidden_id_expediente"
                Exit Function
            End If
            Hidden_id_tipo_expediente.value = estructura.ID_TIPO_EXPEDIENTE
            Hidden_id_expediente.value = estructura.ID_EXPEDIENTE
            Asignar_expediente_estructura_interface_indice_imagen = "YES"
        Catch ex As Exception
            Asignar_expediente_estructura_interface_indice_imagen = "Inconsistencia Función " & vbCrLf &
            " Asignar_expediente_estructura_interface_indice_imagen " & vbCrLf & ex.Message
        End Try
    End Function
    Function Asignar_trd_estructura_interface_indice_imagen(ByRef page1 As Page,
    ByVal matri_gestion As estructure_gestion) As String
        '**********************************************************
        'Funcion : asigna datos trd interface indice imagen desde
        'la estrucutura del gabinete
        'fecha : 2015-01-18
        'Ing :Miguel Angel Urueta
        '**********************************************************
        Try
            Dim contro As Object = Nothing
            Dim Hidden_id_serie As Object = Nothing
            Dim Hidden_id_sub_serie As Object = Nothing
            Dim Hidden_id_documento As Object = Nothing
            Dim Hidden_id_area As Object = Nothing
            Hidden_id_serie = page1.FindControl("Hidden_id_serie")
            If Hidden_id_serie Is Nothing Then
                Asignar_trd_estructura_interface_indice_imagen = "Función Asignar_trd_estructura_interface_indice_imagen dice : imposible encontrar el control Hidden_id_serie"
                Exit Function
            End If
            Hidden_id_sub_serie = page1.FindControl("Hidden_id_sub_serie")
            If Hidden_id_sub_serie Is Nothing Then
                Asignar_trd_estructura_interface_indice_imagen = "Función Asignar_trd_estructura_interface_indice_imagen dice : imposible encontrar el control Hidden_id_sub_serie"
                Exit Function
            End If
            Hidden_id_documento = page1.FindControl("Hidden_id_documento")
            If Hidden_id_documento Is Nothing Then
                Asignar_trd_estructura_interface_indice_imagen = "Función Asignar_trd_estructura_interface_indice_imagen dice : imposible encontrar el control Hidden_id_documento"
                Exit Function
            End If
            Hidden_id_area = page1.FindControl("Hidden_id_area")
            If Hidden_id_area Is Nothing Then
                Asignar_trd_estructura_interface_indice_imagen = "Función Asignar_trd_estructura_interface_indice_imagen dice : imposible encontrar el control Hidden_id_area"
                Exit Function
            End If
            Hidden_id_serie.value = matri_gestion.ID_SERIE
            Hidden_id_sub_serie.value = matri_gestion.ID_SUB_SERIE
            Hidden_id_documento.value = matri_gestion.ID_TIPODOCUMENTO
            Hidden_id_area.value = matri_gestion.ID_AREA
            Asignar_trd_estructura_interface_indice_imagen = "YES"
        Catch ex As Exception
            Asignar_trd_estructura_interface_indice_imagen = "Inconsistencia función Asignar_trd_estructura_interface_indice_imagen " & ex.Message
        End Try
    End Function
    Private Sub comman_tipo_documemnto_restore_clik(ByVal sender As _
      System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim update As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")
        Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
        Try
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Mens.Showscripman("El usuario docuarchi no tiene usuario de gestión relacionado", update)
                ref_Hidden_resultado.value = ""
                ref_Updatepanel_actualiza.Update()
                'sender.focus()
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                If HttpContext.Current.Session.Item("GA_SELECCIONA_CLASE_DOCUMENTOS") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para restaurar la clase de documento", update)
                    ref_Hidden_resultado.value = ""
                    ref_Updatepanel_actualiza.Update()
                    'sender.focus()
                    Exit Sub
                End If
                Dim resulta As String = ""
                Dim refclas_inventario As New ClassGaGestionInventario
                If Hidden_id_inventario.Value <> 0 Then
                    resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If resulta <> "YES" Then
                        ref_Hidden_resultado.value = ""
                        ref_Updatepanel_actualiza.Update()
                        Mens.Showscripman(resulta, update)
                        'sender.focus()
                        Exit Sub
                    End If
                End If
            End If
            ref_Hidden_resultado.value = "YES"
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally

        End Try
    End Sub
    Private Sub comman_tipo_documemnto_clik(ByVal sender As _
      System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim update As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")
        Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
        Try
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Mens.Showscripman("El usuario docuarchi no tiene usuario de gestión relacionado", update)
                ref_Hidden_resultado.value = ""
                ref_Updatepanel_actualiza.Update()
                'sender.focus()
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                If HttpContext.Current.Session.Item("GA_SELECCIONA_CLASE_DOCUMENTOS") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para seleccionar la clase de documento", update)
                    ref_Hidden_resultado.value = ""
                    ref_Updatepanel_actualiza.Update()
                    'sender.focus()
                    Exit Sub
                End If
                Dim resulta As String = ""
                Dim refclas_inventario As New ClassGaGestionInventario
                If Hidden_id_inventario.Value <> 0 Then
                    resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If resulta <> "YES" Then
                        ref_Hidden_resultado.value = ""
                        ref_Updatepanel_actualiza.Update()
                        Mens.Showscripman(resulta, update)
                        'sender.focus()
                        Exit Sub
                    End If
                End If
            End If
            ref_Hidden_resultado.value = "YES"
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally

        End Try
    End Sub
    Private Sub comman_trd_clik(ByVal sender As _
       System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim Hidden_image_gabinete As HtmlInputHidden = sender.page.findcontrol("Hidden_image_gabinete")
        Dim update As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")
        Dim estru_gestion As estructure_gestion = Nothing
        Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
        Dim nombre_gabinete As String = ""
        Dim id_imagen As Long = 0
        Try
            If InStr(Hidden_image_gabinete.Value, "|") = 0 Then
                Mens.Showscripman("Imposible encontrar el gabinete seleccionado", update)
                Exit Sub
            End If
            Dim spli() As String = Hidden_image_gabinete.Value.ToString.Split("|")
            id_imagen = spli(0)
            nombre_gabinete = spli(1)
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                MsgBox("El usuario docuarchi no tiene usuario de gestión relacionado", MsgBoxStyle.Information)
                'sender.focus()
                Exit Sub
            End If
            Dim resulta As String = ""
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                If HttpContext.Current.Session.Item("GA_APLICATRD_DOCUMENTOS") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para aplicar trd al documento", update)
                    Exit Sub
                End If

                Dim refclas_inventario As New ClassGaGestionInventario
                If Hidden_id_inventario.Value <> 0 Then
                    resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value,
                                                                                      HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If resulta <> "YES" Then
                        Mens.Showscripman(resulta, update)
                        'sender.focus()
                        Exit Sub
                    End If
                End If
            End If
            HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE") = -1
            HttpContext.Current.Session.Item("TRD_APLICA_ID_SUB_SERIE") = -1
            resulta = Me.Asigna_datos_gestion_estructura_de_gabinete(id_imagen,
                                                                        estru_gestion,
                                                                        nombre_gabinete)
            If resulta <> "YES" Then
                Mens.Showscripman(resulta, update)
                Exit Sub
            End If
            Dim Refclass_expediente As New ClassGaExpediente
            If estru_gestion.ID_EXPEDIENTE <> 0 Then
                resulta = Refclass_expediente.SolicitaDatosEstructuraExpediente(estru_gestion.ID_EXPEDIENTE,
                                                                               estru_unidad_conservacion)
                If resulta <> "YES" Then
                    Mens.Showscripman(resulta, update)
                    Exit Sub
                End If
                If estru_unidad_conservacion(0).CODIGO_SERIE = "" Or estru_unidad_conservacion(0).CODIGO_SERIE = "0" Then
                    HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE") = -1
                Else
                    HttpContext.Current.Session.Item("TRD_APLICA_ID_SERIE") = Val(estru_unidad_conservacion(0).CODIGO_SERIE)
                End If
                If estru_unidad_conservacion(0).CODIGO_SUBSERIE = "" Or estru_unidad_conservacion(0).CODIGO_SUBSERIE = "0" Then
                    HttpContext.Current.Session.Item("TRD_APLICA_ID_SUB_SERIE") = -1
                Else
                    HttpContext.Current.Session.Item("TRD_APLICA_ID_SUB_SERIE") = Val(estru_unidad_conservacion(0).CODIGO_SUBSERIE)
                End If
                HttpContext.Current.Session.Item("TRD_APLICA_EXPEDIENTE") = estru_unidad_conservacion(0).ALEAS_EXPEDIENTE
            Else
                'Mens.Showscripman("Para clasificar el documento debe agregarlo a un expediente", update)
                'Exit Sub
            End If

            'Dim ref_Aplicar tabla de retención
            Dim ref_Label_trd_popup As Label = sender.page.findcontrol("Label_trd_popup")
            ref_Label_trd_popup.Text = "Aplicar tabla de retención"
            Dim ref_Iframe_trd_popup = sender.page.findcontrol("Iframe_trd_popup_")
            ref_Iframe_trd_popup.Attributes.Add("src", "../gestion/WebFormGaAplicarTrd.aspx")
            Dim ref_UpdatePanel_trd_popup = sender.page.findcontrol("UpdatePanel_trd_popup")
            ref_UpdatePanel_trd_popup.Update()
            Dim ref_ModalPopupExtende_trd_popup = sender.page.findcontrol("ModalPopupExtende_trd_popup")
            ref_ModalPopupExtende_trd_popup.Show()
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona tabla retención documental")
        End Try
    End Sub
    Private Sub comman_trd_restore_clik(ByVal sender As _
                                        System.Object,
                                        ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim update As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                MsgBox("El usuario docuarchi no tiene usuario de gestión relacionado", MsgBoxStyle.Information)
                'sender.focus()
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                If HttpContext.Current.Session.Item("GA_APLICATRD_DOCUMENTOS") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para retaurar trd al documento", update)
                    'sender.focus()
                    Exit Sub
                End If
                Dim resulta As String = ""
                Dim refclas_inventario As New ClassGaGestionInventario
                If Hidden_id_inventario.Value <> 0 Then
                    resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If resulta <> "YES" Then
                        Mens.Showscripman(resulta, update)
                        'sender.focus()
                        Exit Sub
                    End If
                End If
            End If
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona tabla retención documental")
        End Try
    End Sub
    Private Sub comman_trd_fecha_restore_clik(ByVal sender As _
                                              System.Object,
                                              ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim update As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                MsgBox("El usuario docuarchi no tiene usuario de gestión relacionado", MsgBoxStyle.Information)
                'sender.focus()
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                If HttpContext.Current.Session.Item("GA_APLICATRD_DOCUMENTOS") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para retaurar la fecha trd al documento", update)
                    'sender.focus()
                    Exit Sub
                End If
                Dim resulta As String = ""
                Dim refclas_inventario As New ClassGaGestionInventario
                If Hidden_id_inventario.Value <> 0 Then
                    resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If resulta <> "YES" Then
                        Mens.Showscripman(resulta, update)
                        'sender.focus()
                        Exit Sub
                    End If
                End If
            End If
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona tabla retención documental")
        End Try
    End Sub
    Private Sub comman_trd_fecha_clik(ByVal sender As _
      System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Dim update As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                MsgBox("El usuario docuarchi no tiene usuario de gestión relacionado", MsgBoxStyle.Information)
                'sender.focus()
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                If HttpContext.Current.Session.Item("GA_APLICATRD_DOCUMENTOS") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para cambiar la fecha trd al documento", update)
                    'sender.focus()
                    Exit Sub
                End If
                Dim resulta As String = ""
                Dim refclas_inventario As New ClassGaGestionInventario
                If Hidden_id_inventario.Value <> 0 Then
                    resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If resulta <> "YES" Then
                        Mens.Showscripman(resulta, update)
                        'sender.focus()
                        Exit Sub
                    End If
                End If
            End If
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona tabla retención documental")
        End Try
    End Sub
    Private Sub comman_expediente_restore_clik(ByVal sender As _
       System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim update As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Mens.Showscripman("El usuario docuarchi no tiene usuario de gestión relacionado", update)
                'sender.focus()
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                If HttpContext.Current.Session.Item("GA_ASIGNA_EXPEDIENTE_DOCUMENTOS") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para restaurar expediente al documento", update)
                    'sender.focus()
                    Exit Sub
                End If
                Dim resulta As String = ""
                Dim refclas_inventario As New ClassGaGestionInventario
                If Hidden_id_inventario.Value <> "0" Then
                    resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If resulta <> "YES" Then
                        Mens.Showscripman(resulta, update)
                        'sender.focus()
                        Exit Sub
                    End If
                End If
            End If
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona unidad compleja, compuesta (Expediente)")
        End Try
    End Sub
    Private Sub comman_expediente_clik(ByVal sender As _
       System.Object, ByVal e As System.EventArgs)
        Dim Mens As New Classscrripjava
        Dim update As UpdatePanel = sender.page.findcontrol("ActualizaindiceImage")
        Dim Hidden_id_inventario As HtmlInputHidden = sender.page.findcontrol("Hidden_id_inventario")
        Try
            If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                Mens.Showscripman("El usuario docuarchi no tiene usuario de gestión relacionado", update)
                Exit Sub
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                If HttpContext.Current.Session.Item("GA_ASIGNA_EXPEDIENTE_DOCUMENTOS") = 0 Then
                    Mens.Showscripman("El usuario no tiene permisos para asignar expediente al documento", update)
                    'sender.focus()
                    Exit Sub
                End If
                Dim resulta As String = ""
                Dim refclas_inventario As New ClassGaGestionInventario
                If Hidden_id_inventario.Value <> "0" Then
                    resulta = refclas_inventario.Verifica_propiedad_usuario_documento(Hidden_id_inventario.Value, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                    If resulta <> "YES" Then
                        Mens.Showscripman(resulta, update)
                        'sender.focus()
                        Exit Sub
                    End If
                End If
            End If
            Dim Refclas As New ClassAdmonEmpresa
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 1 Then
                'Result = Refclas.Listar_Empresa_de_Gestion_Activa(FormGaGestionExpediente.ComboBoxEntidadEmpresa, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                'If Result <> "YES" Then
                '    Mens.Showscripman(Result, update)
                '    Exit Sub
                'End If

                'Dim clasadmonempresa As New ClassAdmonEmpresa
                'Dim empresa_usuario_gestion As String = ""
                'Result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                'If Result <> "YES" Then
                '    Mens.Showscripman(Result, update)
                '    sender.focus()
                '    Exit Sub
                'End If
                'If FormGaGestionExpediente.ComboBoxEntidadEmpresa.Items.Count > 0 Then
                '    FormGaGestionExpediente.ComboBoxEntidadEmpresa.Text = empresa_usuario_gestion
                'End If
            Else
                'Result = Refclas.Listar_Empresa_de_Gestion_Activa(FormGaGestionExpediente.ComboBoxEntidadEmpresa, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                'If Result <> "YES" Then
                '    MsgBox(Result, MsgBoxStyle.Information)
                '    Exit Sub
                'End If
                'Dim clasadmonempresa As New ClassAdmonEmpresa
                'Dim empresa_usuario_gestion As String = ""
                'Result = clasadmonempresa.Retorna_nombre_empresa_usuario_gestion(empresa_usuario_gestion, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                'If Result <> "YES" Then
                '    Mens.Showscripman(Result, update)
                '    sender.focus()
                '    Exit Sub
                'End If
                'If FormGaGestionExpediente.ComboBoxEntidadEmpresa.Items.Count > 0 Then
                '    FormGaGestionExpediente.ComboBoxEntidadEmpresa.Text = empresa_usuario_gestion
                'End If
            End If
            Dim ref_Iframe_expdiente_popup = sender.page.findcontrol("Iframe_expdiente_popup_")
            ref_Iframe_expdiente_popup.Attributes.Add("src", "../gestion/WebFormGaGestionExpediente.aspx")
            Dim ref_UpdatePanel_expdiente_popup = sender.page.findcontrol("UpdatePanel_expdiente_popup")
            ref_UpdatePanel_expdiente_popup.Update()
            Dim ref_ModalPopupExtende_expdiente_popup = sender.page.findcontrol("ModalPopupExtende_expdiente_popup")
            ref_ModalPopupExtende_expdiente_popup.Show()
            Dim ref_Hidden_resultado = sender.page.findcontrol("Hidden_resultado")
            ref_Hidden_resultado.value = "YES"
            Dim ref_Updatepanel_actualiza As UpdatePanel = sender.page.findcontrol("Updatepanel_actualiza")
            ref_Updatepanel_actualiza.Update()
        Catch ex As Exception
            Mens.Showscripman(ex.Message, update)
        Finally
            'toltip.SetToolTip(sender, "Selecciona unidad compleja, compuesta (Expediente)")
        End Try
    End Sub
    Function verifica_exitencia_valor_invnetario_gabinete(ByVal nombre_gabinete As String, ByVal id_imagen As Integer,
   ByRef id_invnetario As Long) As String
        '*********************************************************
        'Retorna valor inventario en el gabinete
        'fecha : 2015-02-15
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try

            Dim Result As String = ""
            Dim Parametro_Consulta As String = "Select ID_INVENTARIO_DOCUMENTAL from " & nombre_gabinete & " where id='" & id_imagen & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("AREAS_DEPART_RADICACION")
            Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                verifica_exitencia_valor_invnetario_gabinete = "Funcion  verifica_exitencia_valor_invnetario_gabinete dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                verifica_exitencia_valor_invnetario_gabinete = "Imposible encontrar id de la imagen "
                Exit Function
            Else

                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    id_invnetario = Datset.Tables(0).Rows(0).Item(0)
                Else
                    id_invnetario = 0
                End If
                verifica_exitencia_valor_invnetario_gabinete = "YES"
                Exit Function
            End If

        Catch ex As Exception
            verifica_exitencia_valor_invnetario_gabinete = "Inconsistencia función  verifica_exitencia_valor_invnetario_gabinete " & ex.Message
        End Try
    End Function
    Function agregar_auto_complete_docuarchi(ByVal id_tex As String, ByRef pnae As Panel, ByVal ruta_webservice As String,
                            ByVal tabla As String, ByVal campo As String) As String
        '***************************************************************
        'Funcion : Agrega control autocomplete, debe agregar funcion
        'java onDataShown para navegador chrome
        'Fecha : 2014-08-21
        'Ingeniero : Miguel Angel Urueta Miranda
        '***************************************************************
        Try
            Dim Auto As New AutoCompleteExtender
            Auto.TargetControlID = id_tex
            Auto.MinimumPrefixLength = 2
            Auto.EnableCaching = True
            Auto.CompletionSetCount = 10
            Auto.CompletionInterval = 10
            Auto.ServiceMethod = ruta_webservice
            Auto.ServicePath = "../webservice/WebServiceDocuarchi.asmx"
            Auto.ContextKey = campo & "|" & tabla
            Auto.UseContextKey = True
            Auto.CompletionSetCount = 7
            Auto.OnClientShown = "onDataShown"
            'Auto.CompletionListElementID = id_tex
            Auto.CompletionListCssClass = "completionList"
            Auto.CompletionListHighlightedItemCssClass = "itemHighlighted"
            Auto.CompletionListItemCssClass = "listItem"
            pnae.Controls.Add(Auto)
            agregar_auto_complete_docuarchi = "YES"
        Catch ex As Exception
            agregar_auto_complete_docuarchi = "Inconsistencia fucnion agregar_auto_complete_docuarchi " & ex.Message
        End Try
    End Function


End Class
