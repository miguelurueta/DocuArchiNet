Imports System.Globalization
Imports System.IO
Imports System.Net
Imports Ionic.Zip
Imports Newtonsoft.Json
Imports System.Threading.Tasks
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Public Class class_stru_visor_migracion
    Public Error_result As String
    Public url_iframe As String
    Public tipo_file As String
    Public name_file As String
End Class
Public Class class_image_gabinete
    Public id_imagen As Integer
    Public gabinete As String
    Public error_gestion As String
    Public limpia_visor As Integer
End Class
Public Class class_image_gabinete_visor_rad_simple
    Public id_imagen As Integer
    Public gabinete As String
    Public radicado As String
    Public id_tarea_workflow As Long
End Class

Public Class class_stru_Row_Gabinete_Generic
    Public Error_result As String
    Public Obj_ilist_row_generic As Object     'Seralizado DATA-SET
    Public Obj_ilist_fileds_generic As Object  'class_campos_table_bostra_table
    Public NameTabla As Object
End Class
Public Class class_stru_auto_complete_migracion
    Public Error_result As String
    Public country As List(Of String)
End Class
Public Structure stru_imagen_gabinete_workflow
    Dim id_image As Integer
    Dim gabinete As String
    Dim ID_TIPODOCUMENTO As Integer
End Structure
Public Class class_cambio_tipologia_gabinete
    Public iLIStTipoTramite As List(Of control_drow_lista)
    Public iLIStTipo As List(Of control_drow_lista)
    Public iLIStSerie As List(Of control_drow_lista)
    Public iLIStSubSerie As List(Of control_drow_lista)
    Public Error_result As String
End Class
Public Structure stru_permiso_gabinete
    Dim CONSULTA_IMAGEN
    Dim ALMACENA_IMAGEN
    Dim PREINDEX_IMAGEN
    Dim EXPORT_IMAGE_GABINETE
    Dim EXPORT_IMAGE_FYLESYSTEM
    Dim EXDPORT_IMAGE_CARPETA
    Dim EXPOR_IMAGE_CORREO
    Dim ELIMINAR_REGISTRO
    Dim ADD_IMAGEN_REGISTRO
    Dim EDITAR_REGISTRO
    Dim EXPORTAR_LISTA_REGISTRO
    Dim ACTUALIZA_BATCH_REGISTRO
    Dim EDICION_IMAGEN
    Dim IMPRI_IMAGEN
    Dim GUARDAR_IMAGEN
    Dim CROP_IMAGEN
    Dim ADD_SELLO_IMAGEN
    Dim ADD_FIRMA_DIGTIAL_IMAGEN
    Dim ADD_ESTAMP_CRONOLOGICO_IMAGEN
    Dim ADD_COPIA_ANOTACION_IMAGEN
    Dim ADD_CAPO_WF_IMAGEN
    Dim ADD_STAMP_RADICADO_IMAGEN
    Dim ADD_BIPMAN_IMAGE
    Dim ADD_OCR_IMAGE
    Dim ADD_TRANSFORM_IMAGE
    Dim ADD_DESKIEW_IMAGE
    Dim MASTER_ELIMINAR_REGISTRO
End Structure
Public Class CDpersmisosGabinete
    Property CONSULTA_IMAGEN As Integer
    Property ALMACENA_IMAGEN As Integer
    Property PREINDEX_IMAGEN As Integer
    Property EXPORT_IMAGE_GABINETE As Integer
    Property EXPORT_IMAGE_FYLESYSTEM As Integer
    Property EXDPORT_IMAGE_CARPETA As Integer
    Property EXPOR_IMAGE_CORREO As Integer
    Property ELIMINAR_REGISTRO As Integer
    Property ADD_IMAGEN_REGISTRO As Integer
    Property EDITAR_REGISTRO As Integer
    Property EXPORTAR_LISTA_REGISTRO As Integer
    Property ACTUALIZA_BATCH_REGISTRO As Integer
    Property EDICION_IMAGEN As Integer
    Property IMPRI_IMAGEN As Integer
    Property GUARDAR_IMAGEN As Integer
    Property CROP_IMAGEN As Integer
    Property ADD_SELLO_IMAGEN As Integer
    Property ADD_FIRMA_DIGTIAL_IMAGEN As Integer
    Property ADD_ESTAMP_CRONOLOGICO_IMAGEN As Integer
    Property ADD_COPIA_ANOTACION_IMAGEN As Integer
    Property ADD_CAPO_WF_IMAGEN As Integer
    Property ADD_STAMP_RADICADO_IMAGEN As Integer
    Property ADD_BIPMAN_IMAGE As Integer
    Property ADD_OCR_IMAGE As Integer
    Property ADD_TRANSFORM_IMAGE As Integer
    Property ADD_DESKIEW_IMAGE As Integer
    Property MASTER_ELIMINAR_REGISTRO As Integer
End Class
Public Structure stru_paramter_image
    Dim ID As Long
    Dim DISC As Integer
    Dim PAG As Integer
    Dim DBT_TIPO_IMAGEN As Integer
    Dim IDEX As Integer
    Dim TIPODOCUMENTO As String
    Dim ID_TIPODOCUMENTO As Integer
    Dim RADICADO As String
    Dim ENLACE As String
    Dim ID_PRODUCCION As Long
    Dim RUTA_IMAGEN_FISICA As String
    Dim RUTA_IMAGEN_URL As String
    Dim RUTA_UNC_ORIGINAL As String
    Dim USER As String
    Dim DA_MIG As Integer
    Dim CTRL_ACES As Integer
    Dim ID_PROP As Integer
    Dim ID_REGISTRO_VERSION As Long
    Dim ID_VERSION_DOC As Integer
    Dim ESTADO_FIRMA_DIGITAL As Integer
    Dim TIME1 As String
    Dim DATE1 As String
End Structure
Public Class CDParmeterValoresCamposGabinete
    Property IdTareaWorkflow As Long
    Property IdRutaWorkflow As Integer
    Property NombreRutaWorkflow As String
    Property Gabinete As String
End Class
Public Class CDGabinetesPermitidos
    Property IdGabinete As Integer
    Property NombreGabinete As String
End Class
Public Class CDaGabinete
    Property AppError As String
    Property CDGabinetesPermitidos As New CDGabinetesPermitidos
    Property CDpersmisosGabinete As New CDpersmisosGabinete
    Property CDParamenterGabinete As New CDParamenterGabinete
End Class
Public Class CDParamenterGabinete
    Property IdGabinete As Object
    Property IdImagen As Object
    Property NombreGabinete As Object
    Property ValorConsulta As Object
    Property TipoConsulta As Object
    Property NameEspaceControl As Object
    Property NombreModulo As String
    Property ClassConfigGeneralService As List(Of Class_config_general_service)
End Class
Public Class ClassDaGabinete
    Function ActualizaIndiceDocumentoGabinete(ByVal IdImagen As Integer,
                                              ByVal NombreGabinete As String,
                                              ByVal StruCamposDocuarchi() As stru_campos_docuarchi,
                                              ByVal ModuloActualiza As String,
                                              ByVal LoguinUsuario As String,
                                              ByRef TipoDocumento As String,
                                              ByVal IdTareaWorkflow As Long,
                                              ByVal radicado As String) As String
        Dim result As String = ""
        Dim myConnection As New MySqlConnection
        Dim myTrans As MySqlTransaction
        Dim Switc As Integer = 0
        Try
            '//---Valida persmisos por mdulo-----//
            Select Case ModuloActualiza
                Case "MIGRACION"
                    If (HttpContext.Current.Session.Item("UTIL_MIGRA_UPDATE_INDICE_BATCH") = 0) Then
                        Return "El usuario no tiene persmiso para actualizar indice en modo batch en el módulo de migración."
                    End If
                Case "WORKFLOW"
                    If (HttpContext.Current.Session.Item("WF_ACTUALIZA_INDICE_BATCH_WF") = 0) Then
                        Return "El usuario no tiene persmiso para actualizar indice en modo batch en el moódulo de workflow."
                    End If
                Case "DOCUARCHI"
                    If (HttpContext.Current.Session.Item("ACTUALIZA_BATCH_REGISTRO") = 0) Then
                        Return "El usuario no tiene persmiso para actualizar indice en modo batch en el gabinete (" & NombreGabinete & ")"
                    End If
            End Select
            Dim SqlUpdate As String = "UPDATE " & NombreGabinete & " SET "
            Dim Elimina As String = ""
            Dim starindex As Integer = 0
            Dim pagi As Integer = 0
            Dim actualiza_fultex As String = ""
            Dim option_inventario As Integer = 0
            Dim id_inventario As Long = 0
            Dim suit As Integer = 0
            TipoDocumento = ""
            Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim EstructuraGabineteOld() As estructura_gabinete = Nothing
            Dim ClassGestionFechas As New ClassGestionFechas
            result = Ref_Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(NombreGabinete,
                                                                                  EstructuraGabineteOld)
            If result <> "YES" Then
                ActualizaIndiceDocumentoGabinete = result
                Exit Function
            End If
            '---Asigna datos de la imagen
            Dim ClassDaGabinete As New ClassDaGabinete
            result = ClassDaGabinete.AsignaDatosIndiceDocumento(IdImagen,
                                                                NombreGabinete,
                                                                EstructuraGabineteOld)
            If result <> "YES" Then
                ActualizaIndiceDocumentoGabinete = result
                Exit Function
            End If
            Dim datos_campo As String = ""
            Dim valor_campo As String = ""
            '---------------------------------------------------
            'Verifica existencia inventario documental
            '---------------------------------------------------
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim id_registro_producion As Long = 0
            result = ClassGaProducionDocumental.Solicita_id_registro_producion_documental(IdImagen,
                                                                                          NombreGabinete,
                                                                                          id_registro_producion)
            If result <> "YES" Then
                ActualizaIndiceDocumentoGabinete = result
                Exit Function
            End If
            Dim fultex_origen_image As String = ""
            Dim stru_produccion_indice As stru_produccion_indice = Nothing
            If id_registro_producion <> 0 Then
                result = ClassGaProducionDocumental.Solicita_estructura_id_registro_produccion(id_registro_producion,
                                                                                               stru_produccion_indice)
                If result <> "YES" Then
                    ActualizaIndiceDocumentoGabinete = result
                    Exit Function
                Else
                    fultex_origen_image = stru_produccion_indice.FULTEXT_DOCUMENTO
                End If
            End If
            For i As Integer = 0 To StruCamposDocuarchi.Count - 1
                Dim Exitencia_cambio_valor As String = "NO"
                result = ClassDaGabinete.Solicita_valor_campo_indice(StruCamposDocuarchi(i).nombre_campo,
                                                                     EstructuraGabineteOld,
                                                                     valor_campo)
                If result <> "YES" Then
                    ActualizaIndiceDocumentoGabinete = result
                    Exit Function
                End If
                '---------Valida la exitencia de cambio en campos enteros
                If StruCamposDocuarchi(i).tipo_campo = "INT" Then
                    If StruCamposDocuarchi(i).valor_campo = "" Then
                        datos_campo = datos_campo & StruCamposDocuarchi(i).nombre_campo & "=" & "NULL"
                        SqlUpdate = SqlUpdate & StruCamposDocuarchi(i).nombre_campo & "=" & "NULL,"
                    Else
                        If valor_campo <> "" Then
                            fultex_origen_image = fultex_origen_image.Replace(valor_campo, StruCamposDocuarchi(i).valor_campo)
                        End If
                        datos_campo = datos_campo & StruCamposDocuarchi(i).nombre_campo & "=" & StruCamposDocuarchi(i).valor_campo
                        SqlUpdate = SqlUpdate & StruCamposDocuarchi(i).nombre_campo & "=" & StruCamposDocuarchi(i).valor_campo & ","
                    End If
                End If
                '------------------------------
                'Verifica formato string
                '------------------------------
                If StruCamposDocuarchi(i).tipo_campo <> "INT" And StruCamposDocuarchi(i).tipo_campo <> "DATE" Then
                    If StruCamposDocuarchi(i).valor_campo <> "" Then
                        'actualiza_fultex = actualiza_fultex & Replace(valor_campo, "'", "") & vbCrLf
                        'fultex_origen_image = fultex_origen_image.Replace(valor_campo, StruCamposDocuarchi(i).valor_campo)
                        If valor_campo <> "" Then
                            fultex_origen_image = fultex_origen_image.Replace(valor_campo, StruCamposDocuarchi(i).valor_campo)
                        End If
                        datos_campo = datos_campo & StruCamposDocuarchi(i).nombre_campo & "=" & StruCamposDocuarchi(i).valor_campo
                        SqlUpdate = SqlUpdate & StruCamposDocuarchi(i).nombre_campo & "='" & StruCamposDocuarchi(i).valor_campo & "',"
                    Else
                        datos_campo = datos_campo & StruCamposDocuarchi(i).nombre_campo & "=" & "NULL"
                        SqlUpdate = SqlUpdate & StruCamposDocuarchi(i).nombre_campo & "=NULL,"
                    End If
                End If
                '-----------------------------
                'Verifica el formato fecha
                '-----------------------------
                Dim Result_Formato_fecha As String = ""
                Dim Matriz_Error() As String
                If StruCamposDocuarchi(i).tipo_campo = "DATE" Then
                    If StruCamposDocuarchi(i).valor_campo <> "" Then
                        Result_Formato_fecha = ClassGestionFechas.Verifi_campo_fecha(StruCamposDocuarchi(i).valor_campo)
                        Erase Matriz_Error
                        Matriz_Error = Split(Result_Formato_fecha, "_")
                        'Verifica el formato general de la fecha
                        If Matriz_Error(0) = "CI" Then
                            ActualizaIndiceDocumentoGabinete = "Error Formato fecha " & Matriz_Error(1)
                            Exit Function
                        End If
                        'Verifica el formato general del dia
                        If Matriz_Error(0) = "ED" Then
                            ActualizaIndiceDocumentoGabinete = "Error Formato fecha " & Matriz_Error(1)
                            Exit Function
                        End If
                        'Verifica el formato general del mes
                        If Matriz_Error(0) = "EM" Then
                            ActualizaIndiceDocumentoGabinete = "Error Formato fecha " & Matriz_Error(1)
                            Exit Function
                        End If
                        'actualiza_fultex = actualiza_fultex & Replace(valor_campo, "'", "") & vbCrLf
                        'fultex_origen_image = fultex_origen_image.Replace(valor_campo, StruCamposDocuarchi(i).valor_campo)
                        If valor_campo <> "" Then
                            fultex_origen_image = fultex_origen_image.Replace(valor_campo, StruCamposDocuarchi(i).valor_campo)
                        End If
                        datos_campo = datos_campo & "=" & StruCamposDocuarchi(i).valor_campo
                        SqlUpdate = SqlUpdate & StruCamposDocuarchi(i).nombre_campo & "='" & StruCamposDocuarchi(i).valor_campo & "',"
                    Else
                        datos_campo = datos_campo & StruCamposDocuarchi(i).nombre_campo & "=" & "NULL"
                        SqlUpdate = SqlUpdate & StruCamposDocuarchi(i).nombre_campo & "=" & "NULL,"
                    End If
                End If
                '-----------------------------
                'Verfica entero
                '----------------------------
                If StruCamposDocuarchi(i).tipo_campo = "INT" Then
                    Dim numero As Integer
                    If StruCamposDocuarchi(i).valor_campo <> "" Then
                        If (Integer.TryParse(StruCamposDocuarchi(i).valor_campo, numero) = False) Then
                            ActualizaIndiceDocumentoGabinete = "El campo  (" & StruCamposDocuarchi(i).nombre_campo & ") Solo permite números enteros."
                            Exit Function
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
            SqlUpdate = SqlUpdate & " WHERE ID=" & IdImagen
            Dim Refclasradic As New ClassAlmacenamiento
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If result <> "YES" Then
                ActualizaIndiceDocumentoGabinete = result
                Exit Function
            End If
            '----------------------------------------------------
            'Solicita ruta del documento
            '----------------------------------------------------
            Dim Route_cabinet As String = ""
            Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Route_cabinet,
                                                                   NombreGabinete)
            If result <> "YES" Then
                ActualizaIndiceDocumentoGabinete = result
                Exit Function
            End If
            Dim Route_document As String = ""
            result = ClassDaGabinete.Solicita_ruta_achivo_gabinete(IdImagen,
                                                                   NombreGabinete,
                                                                   Route_cabinet,
                                                                   Route_document)
            If result <> "YES" Then
                ActualizaIndiceDocumentoGabinete = result
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
            result = Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(NombreGabinete,
                                                                                                     inventario_documental,
                                                                                                     aplica_trd,
                                                                                                     asigna_unidad)
            If result <> "YES" Then
                ActualizaIndiceDocumentoGabinete = result
                Exit Function
            End If
            Dim stru_paramter_image As stru_paramter_image = Nothing
            result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(NombreGabinete,
                                                                     IdImagen,
                                                                     stru_paramter_image,
                                                                     aplica_trd,
                                                                     1)
            If result <> "YES" Then
                ActualizaIndiceDocumentoGabinete = result
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
            Dim myCommand2 As MySqlCommand
            Dim myConnection_da As New conect.Dbase_Conction_Mysql_DA
            If SqlUpdate <> "UPDATE " & NombreGabinete & " SET " Then
                result = myConnection_da.Returna_Conexion_Mysql(myConnection)
                If result <> "YES" Then
                    ActualizaIndiceDocumentoGabinete = result
                    Exit Function
                End If
                myCommand2 = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand2.Connection = myConnection
                myCommand2.Transaction = myTrans
                If id_registro_producion <> 0 And fultex_origen_image <> "" Then
                    Dim sqlinventario_fultex = "Update registro_producion_documental " &
                    " set FULTEXT_DOCUMENTO='" & fultex_origen_image & "'" &
                    " where ID_DOCUMENTO_DOCUARCHI_ALMACEN=" & IdImagen &
                    " and NOMBRE_GABINETE='" & NombreGabinete & "'"
                    myCommand2.CommandText = sqlinventario_fultex
                    Switc = myCommand2.ExecuteNonQuery()
                    If Switc = 0 Then
                        ActualizaIndiceDocumentoGabinete = "Imposible actualizar fultex invnetario  : " & sqlinventario_fultex
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                myCommand2.CommandText = SqlUpdate
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    ActualizaIndiceDocumentoGabinete = "Imposible actualizar la tabla docuarchi cambios  : " & SqlUpdate
                    If id_registro_producion <> 0 Then
                        myTrans.Rollback()
                    End If
                    myConnection.Close()
                    Exit Function
                End If
                Dim hor2 As New System.DateTime
                hor2 = Date.Now
                Dim hora As String = hor2.Hour.ToString & ":" & hor2.Minute.ToString & ":" & hor2.Second.ToString
                Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
                & "RUT_DOCU,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,RADICADO,ID_TAREA_WF,ID_RUTA_WF,USER_PROPIETARIO,TIPOLOGIA_DOCUMENTAL) VALUES ( "
                SqlTransac = SqlTransac & "'" & IdImagen & "',"
                SqlTransac = SqlTransac & "'" & "EditarIndice" & "',"
                SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("DA_Login_Usuario") & "',"
                SqlTransac = SqlTransac & "'" & date1al & "',"
                SqlTransac = SqlTransac & "'" & Route_document & "',"
                SqlTransac = SqlTransac & "'" & NombreGabinete & "',"
                SqlTransac = SqlTransac & "'" & datos_campo & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & ModuloActualiza & "','" &
                    radicado & "'," & IdTareaWorkflow & "," & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & "," & ref_user & "," & ref_Tipologia & ")"
                myCommand2.CommandText = SqlTransac
                Switc = myCommand2.ExecuteNonQuery()
                If Switc = 0 Then
                    ActualizaIndiceDocumentoGabinete = "Imposible actualizar la tabla docuarchi cambios  : " & SqlUpdate
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                myTrans.Commit()
                ActualizaIndiceDocumentoGabinete = "YES"
                Exit Function
            Else
                ActualizaIndiceDocumentoGabinete = "No se dectaron modificaciones en el indice"
                Exit Function
            End If
        Catch e As Exception
            Try
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    ActualizaIndiceDocumentoGabinete = "An exception of type " + ex.GetType().ToString() +
                                              " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            ActualizaIndiceDocumentoGabinete = "Error General ActualizaIndiceDocumentoGabinete " & e.Message
            Exit Function
        Finally
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
        End Try
    End Function
    Function SolicitaPermisosSessionGabinete(ByVal NombreGabinete As String,
                                             ByVal IdGrupoDocuArchi As Integer,
                                             ByVal Idusuario_Logueado As Integer,
                                             ByRef CDpersmisosGabinete As CDpersmisosGabinete) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los persmisos de sesión de una gabinete respecto al grupo y al usuario
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete      : Representa el nombre del gabinete
        'IdusuarioLogueado   : Representa la identifcación del usuario docuarchi logueado
        'IdGrupoDocuaerchi   : Representa la identificació del grupo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CDpersmisosGabinete  : Retorna la estructura con los respectivos permisos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha Modifica        : 2025-08-25
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim stru_permiso As stru_permiso_gabinete = Nothing
            Dim Result As String = ""
            Result = Me.SolicitaPermisosGeneralesGabinete(NombreGabinete,
                                                          Idusuario_Logueado,
                                                          IdGrupoDocuArchi,
                                                          stru_permiso)
            If Result <> "YES" Then
                SolicitaPermisosSessionGabinete = Result
                Exit Function
            End If
            If stru_permiso.CONSULTA_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("CONSULTA_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("CONSULTA_IMAGEN") = stru_permiso.CONSULTA_IMAGEN
            End If
            If stru_permiso.ALMACENA_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("ALMACENA_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("ALMACENA_IMAGEN") = stru_permiso.ALMACENA_IMAGEN
            End If
            If stru_permiso.PREINDEX_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("PREINDEX_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("PREINDEX_IMAGEN") = stru_permiso.PREINDEX_IMAGEN
            End If
            If stru_permiso.EXPORT_IMAGE_GABINETE = Nothing Then
                HttpContext.Current.Session.Item("EXPORT_IMAGE_GABINETE") = 0
            Else
                HttpContext.Current.Session.Item("EXPORT_IMAGE_GABINETE") = stru_permiso.EXPORT_IMAGE_GABINETE
            End If
            If stru_permiso.EXPORT_IMAGE_FYLESYSTEM = Nothing Then
                HttpContext.Current.Session.Item("EXPORT_IMAGE_FYLESYSTEM") = 0
            Else
                HttpContext.Current.Session.Item("EXPORT_IMAGE_FYLESYSTEM") = stru_permiso.EXPORT_IMAGE_FYLESYSTEM
            End If
            If stru_permiso.EXDPORT_IMAGE_CARPETA = Nothing Then
                HttpContext.Current.Session.Item("EXDPORT_IMAGE_CARPETA") = 0
            Else
                HttpContext.Current.Session.Item("EXDPORT_IMAGE_CARPETA") = stru_permiso.EXDPORT_IMAGE_CARPETA
            End If
            If stru_permiso.EXPOR_IMAGE_CORREO = Nothing Then
                HttpContext.Current.Session.Item("EXPOR_IMAGE_CORREO") = 0
            Else
                HttpContext.Current.Session.Item("EXPOR_IMAGE_CORREO") = stru_permiso.EXPOR_IMAGE_CORREO
            End If
            If stru_permiso.ELIMINAR_REGISTRO = Nothing Then
                HttpContext.Current.Session.Item("ELIMINAR_REGISTRO") = 0
            Else
                HttpContext.Current.Session.Item("ELIMINAR_REGISTRO") = stru_permiso.ELIMINAR_REGISTRO
            End If
            If stru_permiso.MASTER_ELIMINAR_REGISTRO = Nothing Then
                HttpContext.Current.Session.Item("MASTER_ELIMINAR_REGISTRO") = 0
            Else
                HttpContext.Current.Session.Item("MASTER_ELIMINAR_REGISTRO") = stru_permiso.MASTER_ELIMINAR_REGISTRO
            End If
            If stru_permiso.ADD_IMAGEN_REGISTRO = Nothing Then
                HttpContext.Current.Session.Item("ADD_IMAGEN_REGISTRO") = 0
            Else
                HttpContext.Current.Session.Item("ADD_IMAGEN_REGISTRO") = stru_permiso.ADD_IMAGEN_REGISTRO
            End If
            If stru_permiso.EDITAR_REGISTRO = Nothing Then
                HttpContext.Current.Session.Item("EDITAR_REGISTRO") = 0
            Else
                HttpContext.Current.Session.Item("EDITAR_REGISTRO") = stru_permiso.EDITAR_REGISTRO
            End If
            If stru_permiso.EXPORTAR_LISTA_REGISTRO = Nothing Then
                HttpContext.Current.Session.Item("EXPORTAR_LISTA_REGISTRO") = 0
            Else
                HttpContext.Current.Session.Item("EXPORTAR_LISTA_REGISTRO") = stru_permiso.EXPORTAR_LISTA_REGISTRO
            End If
            If stru_permiso.ACTUALIZA_BATCH_REGISTRO = Nothing Then
                HttpContext.Current.Session.Item("ACTUALIZA_BATCH_REGISTRO") = 0
            Else
                HttpContext.Current.Session.Item("ACTUALIZA_BATCH_REGISTRO") = stru_permiso.ACTUALIZA_BATCH_REGISTRO
            End If
            If stru_permiso.EDICION_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("EDICION_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("EDICION_IMAGEN") = stru_permiso.EDICION_IMAGEN
            End If
            If stru_permiso.IMPRI_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("IMPRI_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("IMPRI_IMAGEN") = stru_permiso.IMPRI_IMAGEN
            End If
            If stru_permiso.GUARDAR_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("GUARDAR_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("GUARDAR_IMAGEN") = stru_permiso.GUARDAR_IMAGEN
            End If
            If stru_permiso.CROP_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("CROP_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("CROP_IMAGEN") = stru_permiso.CROP_IMAGEN
            End If
            If stru_permiso.ADD_SELLO_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("ADD_SELLO_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("ADD_SELLO_IMAGEN") = stru_permiso.ADD_SELLO_IMAGEN
            End If
            If stru_permiso.ADD_FIRMA_DIGTIAL_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("ADD_FIRMA_DIGTIAL_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("ADD_FIRMA_DIGTIAL_IMAGEN") = stru_permiso.ADD_FIRMA_DIGTIAL_IMAGEN
            End If
            If stru_permiso.ADD_ESTAMP_CRONOLOGICO_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("ADD_ESTAMP_CRONOLOGICO_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("ADD_ESTAMP_CRONOLOGICO_IMAGEN") = stru_permiso.ADD_ESTAMP_CRONOLOGICO_IMAGEN
            End If
            If stru_permiso.ADD_COPIA_ANOTACION_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("ADD_COPIA_ANOTACION_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("ADD_COPIA_ANOTACION_IMAGEN") = stru_permiso.ADD_COPIA_ANOTACION_IMAGEN
            End If
            If stru_permiso.ADD_CAPO_WF_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("ADD_CAPO_WF_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("ADD_CAPO_WF_IMAGEN") = stru_permiso.ADD_CAPO_WF_IMAGEN
            End If
            If stru_permiso.ADD_STAMP_RADICADO_IMAGEN = Nothing Then
                HttpContext.Current.Session.Item("ADD_STAMP_RADICADO_IMAGEN") = 0
            Else
                HttpContext.Current.Session.Item("ADD_STAMP_RADICADO_IMAGEN") = stru_permiso.ADD_STAMP_RADICADO_IMAGEN
            End If
            If stru_permiso.ADD_BIPMAN_IMAGE = Nothing Then
                HttpContext.Current.Session.Item("ADD_BIPMAN_IMAGE") = 0
            Else
                HttpContext.Current.Session.Item("ADD_BIPMAN_IMAGE") = stru_permiso.ADD_BIPMAN_IMAGE
            End If
            If stru_permiso.ADD_OCR_IMAGE = Nothing Then
                HttpContext.Current.Session.Item("ADD_OCR_IMAGE") = 0
            Else
                HttpContext.Current.Session.Item("ADD_OCR_IMAGE") = stru_permiso.ADD_OCR_IMAGE
            End If
            If stru_permiso.ADD_TRANSFORM_IMAGE = Nothing Then
                HttpContext.Current.Session.Item("ADD_TRANSFORM_IMAGE") = 0
            Else
                HttpContext.Current.Session.Item("ADD_TRANSFORM_IMAGE") = stru_permiso.ADD_TRANSFORM_IMAGE
            End If
            If stru_permiso.ADD_DESKIEW_IMAGE = Nothing Then
                HttpContext.Current.Session.Item("ADD_DESKIEW_IMAGE") = 0
            Else
                HttpContext.Current.Session.Item("ADD_DESKIEW_IMAGE") = stru_permiso.ADD_DESKIEW_IMAGE
            End If
            Dim ClassGestonClase As New ClassGestonClase
            CDpersmisosGabinete = ClassGestonClase.StructToClass(Of stru_permiso_gabinete, CDpersmisosGabinete)(stru_permiso)
            SolicitaPermisosSessionGabinete = "YES"
        Catch ex As Exception
            SolicitaPermisosSessionGabinete = "Inconsistencia general función SolicitaPermisosSessionGabinete "
        End Try
    End Function
    Function Asigna_datos_trd_estructura_de_gabinete(ByVal id_imagen As Integer,
                                                     ByRef estru_gestion As estructure_gestion,
                                                     ByVal Nombre_Gabinete As String) As String
        '******************************************************************
        'Función : Asigna los datos de tablas de retención a la estructura
        'desde el gabinete
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2015-01-17
        '******************************************************************
        Try
            Dim Parametro_Consulta As String = "Select " &
            "ID_AREA,ID_SERIE,ID_SUB_SERIE,ID_TIPODOCUMENTO" &
            " from " & Nombre_Gabinete & " Where ID=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(Nombre_Gabinete)
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Asigna_datos_trd_estructura_de_gabinete = "Funcion Asigna_datos_trd_estructura_de_gabinete dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_datos_trd_estructura_de_gabinete = "Imposible Encontrar en el gabienete el id de la imagen " & id_imagen
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estru_gestion.ID_AREA = 0
                Else
                    estru_gestion.ID_AREA = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    estru_gestion.ID_SERIE = 0
                Else
                    estru_gestion.ID_SERIE = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    estru_gestion.ID_SUB_SERIE = 0
                Else
                    estru_gestion.ID_SUB_SERIE = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    estru_gestion.ID_TIPODOCUMENTO = 0
                Else
                    estru_gestion.ID_TIPODOCUMENTO = Datset.Tables(0).Rows(0).Item(3)
                End If
                Asigna_datos_trd_estructura_de_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_datos_trd_estructura_de_gabinete = "Inconsistencia función Asigna_datos_trd_estructura_de_gabinete " & ex.Message
        End Try
    End Function
    Function SolicitaDatosCamposIndiceGabinete(CDParmeterValoresCamposGabinete As CDParmeterValoresCamposGabinete,
                                               ByRef Radicado As String,
                                               ByRef CDcamposAsignaAlmacenamiento As List(Of CDcamposAsignaAlmacenamiento)) As String
        '--------------------------------------------------------------------------------------------------
        'Funcion : Asgina campos y datos de alamacenamiento para indice de gabinete de la relación gabinete
        '          plantilla de radicación
        '          
        '--------------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------------------
        'CDParmeterValoresCamposGabinete  : Representa la estructura con los parmetros para estructura
        'de campos y datos de gabinete  IdTareaWorkflow-> Representa la identificacón de la tarea workflow
        'IdRutaWorkflow -> Representa la identificación de la ruta  NombreRutaWorkflow-> Representa la 
        'nombre de la ruta workflow
        '--------------------------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------------------------
        'Radicado                     : Retorna el consecutivo de recibo del sistema SII
        'CDcamposAsignaAlmacenamiento : Retorna de los valores y los campos de almacenamiento
        '--------------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------------------------
        'Fecha                 : 2025-07-18
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim ClassConfiguracionListadoRuta As New Class_configuracion_listado_ruta
            Dim NombreCampoRadicado As String = ""
            Result = ClassConfiguracionListadoRuta.SolicitaNombreCampoRadicadoRuta(CDParmeterValoresCamposGabinete.IdRutaWorkflow,
                                                                                   NombreCampoRadicado)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabinete = Result
                Exit Function
            End If
            Result = Class_DAT_ADIC_TAR.SolicitaRadicadoTareaWorkflow(NombreCampoRadicado,
                                                                      CDParmeterValoresCamposGabinete.NombreRutaWorkflow,
                                                                      CDParmeterValoresCamposGabinete.IdTareaWorkflow,
                                                                      Radicado)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabinete = Result
                Exit Function
            End If
            Dim Class_system1 As New Class_system1
            Dim IdGabineteDocuarchi As Integer = 0
            Result = Class_system1.SolicitaIdGabineteDocuarchi(CDParmeterValoresCamposGabinete.Gabinete,
                                                               IdGabineteDocuarchi)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabinete = Result
                Exit Function
            End If
            Dim Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Dim NombrePlantillaRadicado As String = ""
            Result = Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                          NombrePlantillaRadicado)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabinete = Result
                Exit Function
            End If
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim IdPlantillaRadicado As Integer = 0
            Result = Class_system_plantilla_radicado.SolicitaIdPlantillaRadicado(IdPlantillaRadicado,
                                                                                 NombrePlantillaRadicado)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabinete = Result
                Exit Function
            End If
            Dim Class_ra_relacion_plantilla_gabinete As New Class_ra_relacion_plantilla_gabinete
            Dim stru_campos_plantilla_gabinete() As csfc_structure_relacion_campos_plantilla_ruta = Nothing
            Result = Class_ra_relacion_plantilla_gabinete.SolicitaCamposRelacionPlantillaGabinete(IdPlantillaRadicado,
                                                                                                  IdGabineteDocuarchi,
                                                                                                  stru_campos_plantilla_gabinete)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabinete = Result
                Exit Function
            End If
            Dim Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = Class_plantillas_radicacion.AsignaDatosCamposPlantillaRadicadoGabinete(stru_campos_plantilla_gabinete,
                                                                                            Radicado,
                                                                                            NombrePlantillaRadicado)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabinete = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'Formatea campos tipo date  y date time
            '--------------------------------------------------------
            Dim ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To stru_campos_plantilla_gabinete.Length - 1
                If stru_campos_plantilla_gabinete(i).tipo_campo_plantilla = "DATE" Then
                    If Not stru_campos_plantilla_gabinete(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_gabinete(i).dato_campo_plantilla <> "" Then
                        Result = ClassGestionFechas.csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio(stru_campos_plantilla_gabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            SolicitaDatosCamposIndiceGabinete = Result
                            Exit Function
                        End If
                    End If

                End If
                If stru_campos_plantilla_gabinete(i).tipo_campo_plantilla = "DATETIME" Then
                    If Not stru_campos_plantilla_gabinete(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_gabinete(i).dato_campo_plantilla <> "" Then
                        Result = ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(stru_campos_plantilla_gabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            SolicitaDatosCamposIndiceGabinete = Result
                            Exit Function
                        End If
                    End If
                End If
            Next
            Dim IlistCDcamposAsignaAlmacenamiento As New CDcamposAsignaAlmacenamiento
            If Not stru_campos_plantilla_gabinete Is Nothing Then
                For i As Integer = 0 To stru_campos_plantilla_gabinete.Length - 1
                    IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                    IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = stru_campos_plantilla_gabinete(i).nombre_campo_ruta
                    IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = stru_campos_plantilla_gabinete(i).dato_campo_plantilla
                    CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
                Next
            End If
            SolicitaDatosCamposIndiceGabinete = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaDatosCamposIndiceGabinete = "Inconsistencia general funcion SolicitaDatosCamposIndiceGabinete " & ex.Message
        End Try
    End Function
    Function ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow(ByVal IdTareaWorkflow As Long,
                                                                       ByVal NombreRutaWorkflow As String,
                                                                       ByVal Radicado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza los indices de los documentos en el gaibinete relacionados a una tarea
        'workflow 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'NombreRutaWorkflow  : Representa el nombre de la ruta workflow
        'Radicado            : Representa el consecutivo de radicación
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-31
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim IdGabineteWorkflow As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaIdGabineteWorkflowRuta(NombreRutaWorkflow,
                                                                       IdTareaWorkflow,
                                                                       IdGabineteWorkflow)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim NombreGabineteWorkflow As String = ""
            Class_configuracion_gabinete.SolicitanombreGabineteWorkflow(IdGabineteWorkflow,
                                                                        NombreGabineteWorkflow)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            Dim Class_system1 As New Class_system1
            Dim IdGabineteDocuarchi As Integer = 0
            Result = Class_system1.SolicitaIdGabineteDocuarchi(NombreGabineteWorkflow,
                                                               IdGabineteDocuarchi)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            Dim Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Dim NombrePlantillaRadicado As String = ""
            Result = Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                          NombrePlantillaRadicado)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            Dim Class_system_plantilla_radicado As New Class_system_plantilla_radicado
            Dim IdPlantillaRadicado As Integer = 0
            Result = Class_system_plantilla_radicado.SolicitaIdPlantillaRadicado(IdPlantillaRadicado,
                                                                                 NombrePlantillaRadicado)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            Dim Class_ra_relacion_plantilla_gabinete As New Class_ra_relacion_plantilla_gabinete
            Dim stru_campos_plantilla_gabinete() As csfc_structure_relacion_campos_plantilla_ruta = Nothing
            Result = Class_ra_relacion_plantilla_gabinete.SolicitaCamposRelacionPlantillaGabinete(IdPlantillaRadicado,
                                                                                                  IdGabineteDocuarchi,
                                                                                                  stru_campos_plantilla_gabinete)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            Dim Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = Class_plantillas_radicacion.AsignaDatosCamposPlantillaRadicadoGabinete(stru_campos_plantilla_gabinete,
                                                                                            Radicado,
                                                                                            NombrePlantillaRadicado)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'Formatea campos tipo date  y date time
            '--------------------------------------------------------
            Dim refclas_ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To stru_campos_plantilla_gabinete.Length - 1
                If stru_campos_plantilla_gabinete(i).tipo_campo_plantilla = "DATE" Then
                    If Not stru_campos_plantilla_gabinete(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_gabinete(i).dato_campo_plantilla <> "" Then
                        Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio(stru_campos_plantilla_gabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                            Exit Function
                        End If
                    End If

                End If
                If stru_campos_plantilla_gabinete(i).tipo_campo_plantilla = "DATETIME" Then
                    If Not stru_campos_plantilla_gabinete(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_gabinete(i).dato_campo_plantilla <> "" Then
                        Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(stru_campos_plantilla_gabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                            Exit Function
                        End If
                    End If
                End If
            Next
            '-------------------------------------------------------------
            'Actualiza datos en el gabinete workflow
            '-------------------------------------------------------------
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.ActualizaDatosGabinetePorRadicado(Radicado,
                                                                       NombreGabineteWorkflow,
                                                                       stru_campos_plantilla_gabinete)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Solicita id imagen relacionda a workflow
            '--------------------------------------------------------------
            Dim IdImagenDocuarchi As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaidimagenrelacionadaTareaworkflowRuta(NombreRutaWorkflow,
                                                                                    IdTareaWorkflow,
                                                                                    IdImagenDocuarchi)
            If Result <> "YES" Then
                ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                Exit Function
            End If
            '--------------------------------------------------------------
            'Solicita id imagen en el gabinete
            '--------------------------------------------------------------
            If IdImagenDocuarchi = 0 Then
                Result = ClassDaGabinete.SolicitaIdImagenGabinetePorRadicado(Radicado,
                                                                             NombreGabineteWorkflow,
                                                                             IdImagenDocuarchi)
                If Result <> "YES" Then
                    ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                    Exit Function
                End If
                '---------------------------------------------------------
                'Actualiza la imagen en workflow
                '--------------------------------------------------------
                Result = Class_DAT_ADIC_TAR.AcualizaIdImagenTareaWorkflow(IdTareaWorkflow,
                                                                          IdGabineteWorkflow,
                                                                          NombreRutaWorkflow,
                                                                          IdImagenDocuarchi)
                If Result <> "YES" Then
                    ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = Result
                    Exit Function
                End If
            End If
            ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = "YES"
        Catch ex As Exception
            ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow = "Incosistencia general funcion ActualizaIndiceDocumentosGabineteRleacionadoTareaWorkflow( "
        End Try
    End Function
    Function ActualizaDatosGabinetePorRadicado(ByVal Radicado As String,
                                               ByVal NombreGabinete As String,
                                               ByVal stru_campos_plantilla_gabinete() As csfc_structure_relacion_campos_plantilla_ruta) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza el indice de los documentos relacionados un radicado de enlace en el gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Radicado                       : Representa la identificación del radicado
        'NombreGabinete                 : Representa el nombre del gabinete
        'stru_campos_plantilla_gabinete : Representa la estrucutura con los datos de actualizacion
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la idnetificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim campos_update As String = ""
            For i As Integer = 0 To stru_campos_plantilla_gabinete.Length - 1
                If i = 0 Then
                    If stru_campos_plantilla_gabinete(i).dato_campo_plantilla = "" Or stru_campos_plantilla_gabinete Is Nothing Then
                        campos_update = "set " & stru_campos_plantilla_gabinete(i).nombre_campo_ruta & "=null"
                    Else
                        campos_update = "set " & stru_campos_plantilla_gabinete(i).nombre_campo_ruta & "='" & stru_campos_plantilla_gabinete(i).dato_campo_plantilla & "'"
                    End If
                Else
                    If stru_campos_plantilla_gabinete(i).dato_campo_plantilla = "" Or stru_campos_plantilla_gabinete Is Nothing Then
                        campos_update = campos_update & "," & stru_campos_plantilla_gabinete(i).nombre_campo_ruta & "=null"
                    Else
                        campos_update = campos_update & "," & stru_campos_plantilla_gabinete(i).nombre_campo_ruta & "='" & stru_campos_plantilla_gabinete(i).dato_campo_plantilla & "'"
                    End If
                End If
            Next
            Dim SqlActualiza As String = "Update " & NombreGabinete & " " & campos_update & " where enlase='" & Radicado & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Result As String = ""
            Result = ref.SELECTION_INSERT_COMMAND(SqlActualiza)
            If Result <> "YES" Then
                ActualizaDatosGabinetePorRadicado = Result
                Exit Function
            Else
                ActualizaDatosGabinetePorRadicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            ActualizaDatosGabinetePorRadicado = "Inconsistencia general función  ActualizaDatosGabinetePorRadicado " & ex.Message
        End Try
    End Function
    Function SolicitaIdImagenGabinetePorRadicado(ByVal Radicado As String,
                                                 ByVal NombreGabinete As String,
                                                 ByRef IdImagenGabinete As Long) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificación de la imagen del gabinete con el consecutivo del radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Radicado            : Representa el consecutivo del radicado
        'NombreGabinete      : Representa el nombre del gabinete
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenGabinete    : Retorna la identificacion del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim SqlConsulta As String = ""
            SqlConsulta = "select ID from " & NombreGabinete & "  where ENLASE = '" & Radicado & "'" &
             " order by ID "
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(NombreGabinete)
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(SqlConsulta, Datset)
            If Result <> "YES" Then
                SolicitaIdImagenGabinetePorRadicado = "Error función SoliitaIdImagenGabinetePorRadicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdImagenGabinetePorRadicado = "Imposible encontrar el enlace del radicado (" & Radicado & ") en el gabinete (" & NombreGabinete & ")"
                Exit Function
            Else
                IdImagenGabinete = Datset.Tables(0).Rows(0).Item(0)
                SolicitaIdImagenGabinetePorRadicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdImagenGabinetePorRadicado = "Inconistencia general función SoliitaIdImagenGabinetePorRadicado " & ex.Message
        End Try
    End Function
    Function SolicitaSqlConsultaRue(ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                    ByVal NombreCampoCondicion As String,
                                    ByVal ValueCampo As String,
                                    ByVal Table As String,
                                    ByRef Consulta As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el comando SQL para realizar la consulta de documentos en el expediente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'class_campos_table_bostra_table  : Representa la estructura de campos botstraf
        'NombreCampoCondicion             : Representa el nombre del campo de condición
        'ValueCampo                       : Representa el valor de consulta
        'Table                            : Representa la tabla de consulta
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'Consulta  : Retorna el comando sql consulta 
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim condicionsql As String = " where "
            Dim seleccampos As String = "Select "
            Dim campo_clase_documento As String = ""
            Dim campo_expediente As String = ""
            For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                If class_campos_table_bostra_table(i).viisble_sql = "1" Then
                    If seleccampos = "Select " Then
                        seleccampos = seleccampos & "  " & class_campos_table_bostra_table(i).field
                    Else
                        seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
                    End If
                End If
            Next
            condicionsql = condicionsql & NombreCampoCondicion & "='" & ValueCampo & "'"
            Dim order_colum As String = "ASC"
            Dim colum_order_name As String = "ENLASE,ID"
            Dim sqlfrom As String = " From " & Table & " as da"
            Consulta = seleccampos & " " & sqlfrom & " " & condicionsql & "  " & " LIMIT 5000"
            SolicitaSqlConsultaRue = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaSqlConsultaRue = "Inconsistencia general funcion SolicitaSqlConsultaRue " & ex.Message
        End Try
    End Function
    Function SolicitaRowTableConsultaRue(ByVal consulta As String,
                                         ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         de una maricula para servicio rue
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
        'Fecha                 : 2024-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                SolicitaRowTableConsultaRue = "Funcion  SolicitaRowTableConsultaRue " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            SolicitaRowTableConsultaRue = "YES"
        Catch ex As Exception
            SolicitaRowTableConsultaRue = "Inconsistencia general funcion SolicitaRowTableConsultaRue " & ex.Message
        End Try
    End Function
    Function SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII(ByVal ReciboSII As String,
                                                                           ByVal CodBarras As String,
                                                                           ByVal IdExpediente As Integer,
                                                                           ByVal IdTareaWorkflow As Long,
                                                                           ByVal IdTramite As Integer,
                                                                           ByRef ClassExpedienteVincula As ClassExpedienteVincula) As String
        Try
            Dim Result As String = ""
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim CTipoDocEntrante As New CTipoDocEntrante
            Result = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTramite,
                                                                       CTipoDocEntrante)
            If Result <> "YES" Then
                SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII = Result
                Exit Function
            End If
            Dim NombreGabinete As String = CTipoDocEntrante.nombre_gabinete_workflow
            Dim ClasDaGabinete As New ClassDaGabinete
            Dim StruImageGabineteWorfkflow As stru_imagen_gabinete_workflow() = Nothing
            Result = Me.SolicitaListaImagenesGabineteEnlace(NombreGabinete,
                                                            ReciboSII,
                                                            StruImageGabineteWorfkflow)
            If Result <> "YES" Then
                SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII = Result
                Exit Function
            End If
            If StruImageGabineteWorfkflow Is Nothing Then
                SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII = "Imposible encontrar documentos para relacionar al expediente de la tarea (" &
                    IdTareaWorkflow & ") con el consecutivo radicado SII (" & ReciboSII & ")."
                Exit Function
            End If
            ClassExpedienteVincula.id_flujo = IdTareaWorkflow
            For z As Integer = 0 To StruImageGabineteWorfkflow.Length - 1
                Dim ClsssStructureVinculaDocumento As New ClsssStructureVinculaDocumento
                ClsssStructureVinculaDocumento.Gabinete = NombreGabinete
                ClsssStructureVinculaDocumento.IdExpedienteWeb = IdExpediente
                ClsssStructureVinculaDocumento.IdImagen = StruImageGabineteWorfkflow(z).id_image
                ClsssStructureVinculaDocumento.IdFlujoTarea = IdTareaWorkflow
                ClsssStructureVinculaDocumento.Radicado = CodBarras
                ClassExpedienteVincula.ClsssStructureVinculaDocumento.Add(ClsssStructureVinculaDocumento)
            Next
            SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII = "Inconsistencia general funcion SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII " & ex.Message
        End Try
    End Function
    Function SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII(ByVal ReciboSII As String,
                                                                              ByVal IdTareaWorkflow As Long,
                                                                              ByVal IdTramite As Integer,
                                                                              ByVal CIncripcionSII As List(Of CIncripcionSII),
                                                                              ByRef ClassExpedienteVincula As ClassExpedienteVincula) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita estructura documentos relacionados a una tarea workflow con multiplex
        'expedientes caso matricula de perona matural con establecimiento
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS   
        '-----------------------------------------------------------------------------------------------
        'IdExpediente        : Representa la identificación del expediente vinculanete
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'ClassExpedienteVincula  : Retorna la estructura de documentos para vinculación
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-21
        'Elabora               : Miguel Angel Urueta Miranda  
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            Dim CStruSiiCahcheExpediente As New List(Of CStruSiiCahcheExpediente)
            Dim ValidaExitenciaRegistro As Integer = 1
            Result = ClassRaSIiCacheExpediente.SolicitaCacheCreacionExepdienteSiiRadicado(ReciboSII,
                                                                                          ValidaExitenciaRegistro,
                                                                                          CStruSiiCahcheExpediente)
            If Result <> "YES" Then
                SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII = Result
                Exit Function
            End If
            Dim ClasDaGabinete As New ClassDaGabinete
            Dim StruImageGabineteWorfkflow As stru_imagen_gabinete_workflow() = Nothing
            Result = Me.SolicitaListaImagenesGabineteEnlace(CStruSiiCahcheExpediente(0).NombreGabinete,
                                                           ReciboSII,
                                                           StruImageGabineteWorfkflow)
            If Result <> "YES" Then
                SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII = Result
                Exit Function
            End If
            If StruImageGabineteWorfkflow Is Nothing Then
                SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII = "Imposible encontrar documentos para relacionar al expediente de la tarea (" &
                    IdTareaWorkflow & ") con el consecutivo radicado SII (" & ReciboSII & ")."
                Exit Function
            End If
            ClassExpedienteVincula.id_flujo = IdTareaWorkflow
            '//--------Solicita tipologias de expedientes segundarios------//
            Dim ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Dim StruTiposExpedienteSegundarioSII() As StruTiposExpedienteSegundarioSII = Nothing
            Result = ra_dig_tipos_docum_lista_chequeo.SolicitaListaTiposExpedienteSegundarioSii(IdTramite,
                                                                                                StruTiposExpedienteSegundarioSII)
            If Result <> "YES" Then
                SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII = Result
                Exit Function
            End If
            If CStruSiiCahcheExpediente.Count > 1 Then
                If CIncripcionSII.Count = 1 Then
                    SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII = "Dado que la inscripción afecta múltiples expedientes, se recomienda utilizar la opción ‘Guardar todas las inscripciones’."
                    Exit Function
                End If
            End If
            ClassExpedienteVincula.ClsssStructureVinculaDocumento = New List(Of ClsssStructureVinculaDocumento)
            For i As Integer = 0 To CStruSiiCahcheExpediente.Count - 1
                '//-------------Agrega la estructura de documentos a vincular del expediente primario---------////
                If CStruSiiCahcheExpediente.Item(i).EstadoPadre = 1 Then
                    ClassExpedienteVincula.radicado = CStruSiiCahcheExpediente.Item(i).CodBarras
                    ClassExpedienteVincula.gabinete = CStruSiiCahcheExpediente.Item(i).NombreGabinete
                    ClassExpedienteVincula.id_expediente = CStruSiiCahcheExpediente.Item(i).IdExpediente
                    ClassExpedienteVincula.nombre_expediente = CStruSiiCahcheExpediente.Item(i).Matricula
                    ClassExpedienteVincula.Matricula = CStruSiiCahcheExpediente.Item(i).Matricula
                    If StruTiposExpedienteSegundarioSII IsNot Nothing Then
                        For z As Integer = 0 To StruImageGabineteWorfkflow.Length - 1
                            Dim Testigo As Integer = 0
                            For k As Integer = 0 To StruTiposExpedienteSegundarioSII.Length - 1
                                If StruImageGabineteWorfkflow(z).ID_TIPODOCUMENTO = StruTiposExpedienteSegundarioSII(k).IdTipo Then
                                    Testigo = 1
                                End If
                            Next
                            If Testigo = 0 Then
                                Dim ClsssStructureVinculaDocumento As New ClsssStructureVinculaDocumento
                                ClsssStructureVinculaDocumento.Gabinete = CStruSiiCahcheExpediente(i).NombreGabinete
                                ClsssStructureVinculaDocumento.IdExpedienteWeb = CStruSiiCahcheExpediente(i).IdExpediente
                                ClsssStructureVinculaDocumento.IdImagen = StruImageGabineteWorfkflow(z).id_image
                                ClsssStructureVinculaDocumento.IdFlujoTarea = IdTareaWorkflow
                                ClsssStructureVinculaDocumento.Radicado = CStruSiiCahcheExpediente(i).CodBarras
                                ClassExpedienteVincula.ClsssStructureVinculaDocumento.Add(ClsssStructureVinculaDocumento)
                            End If
                        Next
                    Else
                        For z As Integer = 0 To StruImageGabineteWorfkflow.Length - 1
                            Dim ClsssStructureVinculaDocumento As New ClsssStructureVinculaDocumento
                            ClsssStructureVinculaDocumento.Gabinete = CStruSiiCahcheExpediente(i).NombreGabinete
                            ClsssStructureVinculaDocumento.IdExpedienteWeb = CStruSiiCahcheExpediente(i).IdExpediente
                            ClsssStructureVinculaDocumento.IdImagen = StruImageGabineteWorfkflow(z).id_image
                            ClsssStructureVinculaDocumento.IdFlujoTarea = IdTareaWorkflow
                            ClsssStructureVinculaDocumento.Radicado = CStruSiiCahcheExpediente(i).CodBarras
                            ClassExpedienteVincula.ClsssStructureVinculaDocumento.Add(ClsssStructureVinculaDocumento)
                        Next
                    End If
                End If
                '//-------------Agrega la estructura de documentos a vincular del expediente segundario-------////
                If CStruSiiCahcheExpediente.Item(i).EstadoPadre = 2 Then
                    If StruTiposExpedienteSegundarioSII IsNot Nothing Then
                        For z As Integer = 0 To StruImageGabineteWorfkflow.Length - 1
                            For k As Integer = 0 To StruTiposExpedienteSegundarioSII.Length - 1
                                If StruImageGabineteWorfkflow(z).ID_TIPODOCUMENTO = StruTiposExpedienteSegundarioSII(k).IdTipo Then
                                    Dim ClsssStructureVinculaDocumento As New ClsssStructureVinculaDocumento
                                    ClsssStructureVinculaDocumento.Gabinete = CStruSiiCahcheExpediente(i).NombreGabinete
                                    ClsssStructureVinculaDocumento.IdExpedienteWeb = CStruSiiCahcheExpediente(i).IdExpediente
                                    ClsssStructureVinculaDocumento.IdImagen = StruImageGabineteWorfkflow(z).id_image
                                    ClsssStructureVinculaDocumento.IdFlujoTarea = IdTareaWorkflow
                                    ClsssStructureVinculaDocumento.Radicado = CStruSiiCahcheExpediente(i).CodBarras
                                    ClassExpedienteVincula.nombre_expediente_rlacionado = CStruSiiCahcheExpediente(i).Matricula
                                    ClassExpedienteVincula.ClsssStructureVinculaDocumento.Add(ClsssStructureVinculaDocumento)
                                End If
                            Next
                        Next
                    End If
                End If
            Next
            SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII = "Inconsistencia geeneral funcion SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII " & ex.Message
        End Try
    End Function

    Function ActualizaIndiceDocumentoCacheExpediente(ByVal NombreGabinete As String,
                                                     ByVal CStruSiiCahcheExpediente As CStruSiiCahcheExpediente) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza indice de documento en los gabinetes con la condición del expediente 
        '          relacionado en el cache de expediente.
        '          
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete             : Representa el nombre del gabinete
        'CStruSiiCahcheExpediente   : Representa la estructura del cache de expediente
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-25
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim NitIdentificacion As Object
            If CStruSiiCahcheExpediente.NitIdentificacion <> "" Then
                NitIdentificacion = "'" & CStruSiiCahcheExpediente.NitIdentificacion & "'"
            Else
                NitIdentificacion = "Null"
            End If
            Dim Rsocial As String = ""
            If CStruSiiCahcheExpediente.Rsocial <> "" Then
                Rsocial = Left(CStruSiiCahcheExpediente.Rsocial, 40)
                Rsocial = Rsocial.Replace("'", "")
                Rsocial = Rsocial.Replace("´", "")
            End If
            Dim Matricula As String = ""
            If UCase(NombreGabinete) = "ESAL" Then
                Matricula = CStruSiiCahcheExpediente.Matricula.Replace("S0", "")
            Else
                Matricula = CStruSiiCahcheExpediente.Matricula
            End If
            Dim SqlActualiza As String = ""
            If NombreGabinete = "MERCANTIL" Then
                SqlActualiza = "Update MERCANTIL set NITCEDULA=" & NitIdentificacion & ", RAZONSOCIAL='" & Rsocial & "'" &
                 ",MATRICULA='" & Matricula & "'" &
                 " WHERE ID_EXPEDIENTE=" & CStruSiiCahcheExpediente.IdExpediente
            End If
            If NombreGabinete = "ESAL" Then
                SqlActualiza = "Update ESAL set NITCEDULA=" & NitIdentificacion & ", RAZONSOCIAL='" & Rsocial & "'" &
                     ",MATRICULA='" & Matricula & "'" &
                     " WHERE ID_EXPEDIENTE=" & CStruSiiCahcheExpediente.IdExpediente
            End If
            If NombreGabinete = "RUP" Then
                SqlActualiza = "Update RUP set NITCEDULA=" & NitIdentificacion & ", RAZONSOCIAL='" & Rsocial & "'" &
                     ",MATRICULA='" & Matricula & "'" &
                    " WHERE ID_EXPEDIENTE=" & CStruSiiCahcheExpediente.IdExpediente
            End If
            If SqlActualiza = "" Then
                ActualizaIndiceDocumentoCacheExpediente = "El gabinete (" & NombreGabinete & ") no está homologado para la actualización de índices. "
                Exit Function
            End If
            Dim Result As String = ""
            Dim ConexionDB As New conect.Dbase_Conction_Mysql_DA
            Result = ConexionDB.UPDATE_COMMAND(SqlActualiza)
            If Result <> "YES" Then
                ActualizaIndiceDocumentoCacheExpediente = "Error función ActualizaIndiceDocumentoCacheExpediente  (" & Result & ")"
                Exit Function
            Else
                ActualizaIndiceDocumentoCacheExpediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            ActualizaIndiceDocumentoCacheExpediente = "Inconsistencia general funcion ActualizaIndiceDocumentoCacheExpediente " & ex.Message
        End Try
    End Function
    Function ActualizaIndiceDocumentoIntegracionSII(ByVal NombreGabinete As String,
                                                    ByVal StruSiiCahcheInscripcion As StruSiiCahcheInscripcion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza indice de documento en los gabinetes con la condición del recibo de caja
        '          del sistema SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete             : Representa el nombre del gabinete
        'StruSiiCahcheInscripcion   : Representa la estructura del registro de inscripción
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-03-31
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim NitIdentificacion As Object
            If StruSiiCahcheInscripcion.NitIdentificacion <> "" Then
                NitIdentificacion = "'" & StruSiiCahcheInscripcion.NitIdentificacion & "'"
            Else
                NitIdentificacion = "Null"
            End If
            Dim Rsocial As String = ""
            If StruSiiCahcheInscripcion.Rsocial <> "" Then
                Rsocial = Left(StruSiiCahcheInscripcion.Rsocial, 40)
                Rsocial = Rsocial.Replace("'", "")
                Rsocial = Rsocial.Replace("´", "")
            End If
            Dim Matricula As String = ""
            If UCase(NombreGabinete) = "ESAL" Then
                Matricula = StruSiiCahcheInscripcion.Matricula.Replace("S0", "")
            Else
                Matricula = StruSiiCahcheInscripcion.Matricula
            End If
            Dim SqlActualiza As String = ""
            If NombreGabinete = "MERCANTIL" Then
                SqlActualiza = "Update MERCANTIL set NITCEDULA=" & NitIdentificacion & ", RAZONSOCIAL='" & Rsocial & "'" &
                 ",MATRICULA='" & Matricula & "'" &
                 " WHERE ENLASE='" & StruSiiCahcheInscripcion.RadicadoSII & "'"
            End If
            If NombreGabinete = "ESAL" Then
                SqlActualiza = "Update ESAL set NITCEDULA=" & NitIdentificacion & ", RAZONSOCIAL='" & Rsocial & "'" &
                     ",MATRICULA='" & Matricula & "'" &
                     " WHERE ENLASE='" & StruSiiCahcheInscripcion.RadicadoSII & "'"
            End If
            If NombreGabinete = "RUP" Then
                SqlActualiza = "Update RUP set NITCEDULA=" & NitIdentificacion & ", RAZONSOCIAL='" & Rsocial & "'" &
                     ",MATRICULA='" & Matricula & "'" &
                     " WHERE ENLASE='" & StruSiiCahcheInscripcion.RadicadoSII & "'"
            End If
            If SqlActualiza = "" Then
                ActualizaIndiceDocumentoIntegracionSII = "El gabinete (" & NombreGabinete & ") no está homologado para la actualización de índices. "
                Exit Function
            End If
            Dim Result As String = ""
            Dim ConexionDB As New conect.Dbase_Conction_Mysql_DA
            Result = ConexionDB.UPDATE_COMMAND(SqlActualiza)
            If Result <> "YES" Then
                ActualizaIndiceDocumentoIntegracionSII = "Error función ActualizaIndiceDocumentoIntegracionSII  (" & Result & ")"
                Exit Function
            Else
                ActualizaIndiceDocumentoIntegracionSII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            ActualizaIndiceDocumentoIntegracionSII = "Inconsistencia general funcion ActualizaIndiceDocumentoIntegracionSII " & ex.Message
        End Try
    End Function
    Function Solicita_Sql_Consulta_lista_documentos_matricualdo(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                                ByVal tipo_consulta As Integer,
                                                                ByVal valor_consulta As String,
                                                                ByVal table As String,
                                                                ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                                                ByRef consulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta REGISTRO de documentos del matricualdo
        '         
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        'table                         : Representa el nombre de la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'consulta  : Retorna comando sql de consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-10
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
                        seleccampos = seleccampos & "  " & class_campos_table_bostra_table(i).field
                    Else
                        seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
                    End If
                End If
            Next
            If tipo_consulta = 3 Then
                condicionsql = condicionsql & " matricula=" & valor_consulta & " AND DBT <=1 "
            End If
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
                    Dim campo_plantilla As String = " da." & Class_config_general_service.Item(i).name_campo
                    If Class_config_general_service.Item(i).tipo_campo = " DATE" Or Class_config_general_service.Item(i).tipo_campo = " INT" Then
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
            Dim colum_order_name As String = "ID"
            Dim sqlfrom As String = " From " & table & " as da"
            consulta = seleccampos & " " & sqlfrom & " " & condicionsql & "  " & " LIMIT 5000" '& " ORDER BY " & colum_order_name & " " & order_colum & " LIMIT 5000"
            Solicita_Sql_Consulta_lista_documentos_matricualdo = "YES"
        Catch ex As Exception
            Solicita_Sql_Consulta_lista_documentos_matricualdo = "Inconsistencia general funcion Solicita_Sql_Consulta_lista_documentos_matricualdo " & ex.Message
        End Try
    End Function
    Function Solicita_row_documentos_matriculado_table_boot(ByVal consulta As String,
                                                            ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         de registros de documentos de un matriculado
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
        'Fecha                 : 2024-09-10
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                Solicita_row_documentos_matriculado_table_boot = "Funcion  Solicita_row_documentos_matriculado_table_boot " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_row_documentos_matriculado_table_boot = "YES"
        Catch ex As Exception
            Solicita_row_documentos_matriculado_table_boot = "Inconsistencia general fucnion Solicita_row_documentos_matriculado_table_boot " & ex.Message
        End Try
    End Function
    Function Solicita_row_documentos_relacionados_actos_table_boot(ByVal consulta As String,
                                                                  ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         de registros de documentos de un acto de registro de un matriculado
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
        'Fecha                 : 2024-09-09
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                Solicita_row_documentos_relacionados_actos_table_boot = "Funcion  Solicita_row_table_boot_consulta_publica_matriculado " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_row_documentos_relacionados_actos_table_boot = "YES"
        Catch ex As Exception
            Solicita_row_documentos_relacionados_actos_table_boot = "Inconsistencia general fucnion Solicita_row_documentos_relacionados_actos_table_boot " & ex.Message
        End Try
    End Function
    Function Solicita_Sql_Consulta_lista_documentos_relacinados_actos(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                                      ByVal tipo_consulta As Integer,
                                                                      ByVal valor_consulta As String,
                                                                      ByVal libro As Integer,
                                                                      ByVal inscripcion As Integer,
                                                                      ByVal enlace As String,
                                                                      ByVal table As String,
                                                                      ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                                                      ByRef consulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta REGISTRO de documentos del matricualdo
        '          relacionados a un acto
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        'table                         : Representa el nombre de la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'consulta  : Retorna comando sql de consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-09
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
                        seleccampos = seleccampos & "  " & class_campos_table_bostra_table(i).field
                    Else
                        seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
                    End If
                End If
            Next
            If tipo_consulta = 3 Then
                condicionsql = condicionsql & " (libro=" & libro & " and inscripcion=" & inscripcion & " AND DBT <=1 ) or enlase='" & enlace & "'"
            End If
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
            Dim colum_order_name As String = "ID"
            Dim sqlfrom As String = " From " & table & " as da"
            consulta = seleccampos & " " & sqlfrom & " " & condicionsql & "  " & " LIMIT 5000"
            Solicita_Sql_Consulta_lista_documentos_relacinados_actos = "YES"
        Catch ex As Exception
            Solicita_Sql_Consulta_lista_documentos_relacinados_actos = "Inconsistencia general funcion Solicita_Sql_Consulta_lista_documentos_relacinados_actos " & ex.Message
        End Try
    End Function
    Function Solicita_row_table_boot_consulta_publica_actos_matriculado(ByVal consulta As String,
                                                                        ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         de registros del matriculado
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
        'Fecha                 : 2024-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                Solicita_row_table_boot_consulta_publica_actos_matriculado = "Funcion  Solicita_row_table_boot_consulta_publica_actos_matriculado " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_row_table_boot_consulta_publica_actos_matriculado = "YES"
        Catch ex As Exception
            Solicita_row_table_boot_consulta_publica_actos_matriculado = "Inconsistencia general fucnion Solicita_row_table_boot_consulta_publica_actos_matriculado " & ex.Message
        End Try
    End Function
    Function Solicita_Sql_Consulta_actos_matriculado_gabinete(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                              ByVal tipo_consulta As Integer,
                                                              ByVal valor_consulta As String,
                                                              ByVal table As String,
                                                              ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                                              ByRef consulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta REGISTRO de actos del matricualdo para
        '         la  consulta publica de expedientes
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        'table                         : Representa el nombre de la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'consulta  : Retorna comando sql de consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-09-08
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
                        seleccampos = seleccampos & " DISTINCT " & class_campos_table_bostra_table(i).field
                    Else
                        seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
                    End If
                End If
            Next
            If tipo_consulta = 3 Then
                condicionsql = condicionsql & " matricula='" & valor_consulta & "' and libro is not null and inscripcion is not null"
            End If
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
            Dim colum_order_name As String = "ID"
            Dim sqlfrom As String = " From " & table & " as da"
            consulta = seleccampos & " " & sqlfrom & " " & condicionsql & " LIMIT 5000" '& " ORDER BY " & colum_order_name & " " & order_colum & " LIMIT 5000"
            Solicita_Sql_Consulta_actos_matriculado_gabinete = "YES"
        Catch ex As Exception
            Solicita_Sql_Consulta_actos_matriculado_gabinete = "Inconsistencia general funcion Solicita_Sql_Consulta_actos_matriculado_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_row_table_boot_consulta_publica_matriculado(ByVal consulta As String,
                                                                  ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         de registros del matriculado
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
        'Fecha                 : 2024-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                Solicita_row_table_boot_consulta_publica_matriculado = "Funcion  Solicita_row_table_boot_consulta_publica_matriculado " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_row_table_boot_consulta_publica_matriculado = "YES"
        Catch ex As Exception
            Solicita_row_table_boot_consulta_publica_matriculado = "Inconsistencia general fucnion Solicita_row_table_boot_consulta_publica_matriculado " & ex.Message
        End Try
    End Function
    Function Solicita_Sql_Consulta_publica_matriculado_gabinete(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                                ByVal tipo_consulta As Integer,
                                                                ByVal valor_consulta As String,
                                                                ByVal table As String,
                                                                ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                                                ByRef consulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta REGISTRO de matricualdo para la con
        '          consulta publica de expedientes
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Class_config_general_service  : Representa la clase generica con los campoa
        'tipo_consulta                 : Representa el tipo de consulta
        'valor_consulta                : Representa el valor para la consulta tipo like
        'table                         : Representa el nombre de la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'consulta  : Retorna comando sql de consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-07-09
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim condicionsql As String = ""
            Dim seleccampos As String = "Select "
            Dim campo_clase_documento As String = ""
            Dim campo_expediente As String = ""
            For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                If class_campos_table_bostra_table(i).viisble_sql = "1" Then
                    If seleccampos = "Select " Then
                        seleccampos = seleccampos & "  " & class_campos_table_bostra_table(i).field
                    Else
                        seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
                    End If
                End If
            Next
            If tipo_consulta = 2 Then
                Dim likeigual As String = " Like"
                For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
                    If class_campos_table_bostra_table.Item(i).visible_like_sql = 1 Then
                        If condicionsql = "" Then
                            condicionsql = condicionsql & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & valor_consulta & "%'"
                        Else
                            condicionsql = condicionsql & " or " & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & valor_consulta & "%'"
                        End If
                    End If
                Next
                condicionsql = " where (MATRICULA IS NOT NULL and  MATRICULA <>  0 )  and (" & condicionsql & ") "
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
            Dim colum_order_name As String = "ID"
            Dim sqlfrom As String = " From " & table & " as da"
            consulta = seleccampos & " " & sqlfrom & " " & condicionsql & " group by MATRICULA LIMIT 5000" '& " ORDER BY " & colum_order_name & " " & order_colum & " LIMIT 5000"
            Solicita_Sql_Consulta_publica_matriculado_gabinete = "YES"
        Catch ex As Exception
            Solicita_Sql_Consulta_publica_matriculado_gabinete = "Inconsistencia general funcion Solicita_Sql_Consulta_publica_matriculado_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_gestion_autoregistro_gabinete(ByVal id_auto_registro As Integer,
                                                    ByRef Class_cambio_tipologia_gabinete As class_cambio_tipologia_gabinete) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de gestión para el cambio de tipologia 
        'documental
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_auto_registro               : Representa la identificación del auto 
        '                                 registro
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'Class_cambio_tipologia_gabinete : Retorna la estructura de gestión
        '                     
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-08-19
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim Class_ra_auto_campos_gestion_expediente As New Class_ra_auto_campos_gestion_expediente
            Dim Class_series_documentales As New Class_series_documentales
            Dim Class_subseries_documentales As New Class_subseries_documentales
            Dim Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            Dim id_expediente As Integer = 0
            Dim Result As String = ""
            Dim id_fondo As Integer = 0
            Dim id_instrumento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            '---------------------------------------------------------------
            '--------Solicita los datos de auto gestión del auto registro
            '---------------------------------------------------------------
            Result = Class_ra_auto_campos_gestion_expediente.SolicitaDatosGestionCamposAutoRegistro(id_auto_registro,
                                                                                                         id_fondo,
                                                                                                         id_instrumento,
                                                                                                         id_area,
                                                                                                         id_serie,
                                                                                                         id_sub_serie)
            If Result <> "YES" Then
                Solicita_gestion_autoregistro_gabinete = Result
                Exit Function
            End If
            If id_serie <> 0 Then
                '------------------------------------------------------------
                '-----------Solicita lista de series de auto registro
                '------------------------------------------------------------
                Class_cambio_tipologia_gabinete.iLIStSerie = New List(Of control_drow_lista)
                Result = Class_series_documentales.Solicita_lista_serie_id_serie(id_serie,
                                                                                 Class_cambio_tipologia_gabinete.iLIStSerie)
                If Result <> "YES" Then
                    Solicita_gestion_autoregistro_gabinete = Result
                    Exit Function
                End If
            End If
            If id_sub_serie = 0 Then
                '-----------------------------------------------------------------
                '---------Solicita tipos documentales sub series de auto registro
                '-----------------------------------------------------------------
                Class_cambio_tipologia_gabinete.iLIStTipo = New List(Of control_drow_lista)
                Result = Class_ra_tipo_doc_series.Solicita_lista_tipos_documentales_relacionados_id_serie(id_serie,
                                                                                                          Class_cambio_tipologia_gabinete.iLIStTipo)
                If Result <> "YES" Then
                    Solicita_gestion_autoregistro_gabinete = Result
                    Exit Function
                End If
                Solicita_gestion_autoregistro_gabinete = "YES"
                Exit Function
            Else
                '----------------------------------------------------------
                '------Solicita sub series documentales
                '---------------------------------------------------------
                Class_cambio_tipologia_gabinete.iLIStSubSerie = New List(Of control_drow_lista)
                Result = Class_subseries_documentales.Solicita_lista_series_sub_documentales_id_serie(id_serie,
                                                                                                      Class_cambio_tipologia_gabinete.iLIStSubSerie)
                If Result <> "YES" Then
                    Solicita_gestion_autoregistro_gabinete = Result
                    Exit Function
                End If
                '-------------------------------------------------------------
                '-------Lista tipos documentales relacionados a la sub serie
                '-------------------------------------------------------------
                Class_cambio_tipologia_gabinete.iLIStTipo = New List(Of control_drow_lista)
                If Class_cambio_tipologia_gabinete.iLIStSubSerie.Count > 0 Then
                    Result = Class_ra_tipo_doc_series.Solicita_lista_tipos_documentales_relacionados_id_sub_serie(Val(Class_cambio_tipologia_gabinete.iLIStSubSerie.Item(0).value),
                                                                                                                  Class_cambio_tipologia_gabinete.iLIStTipo)
                    If Result <> "YES" Then
                        Solicita_gestion_autoregistro_gabinete = Result
                        Exit Function
                    End If
                End If
                Solicita_gestion_autoregistro_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_gestion_autoregistro_gabinete = "Inconsistencia general funcion Solicita_gestion_autoregistro_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_lista_series_relacionadas_gabinete_migracion(ByVal id_imagen As Integer,
                                                                   ByVal id_gabinete As Integer,
                                                                   ByVal gabinete As String,
                                                                   ByRef Class_cambio_tipologia_gabinete As class_cambio_tipologia_gabinete) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de gestión para el cambio de tipologia 
        'documental
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen               : Representa la identificación de la imagen
        'id_gabinete             : Representa la identficación del gabinete
        'gabinete                : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'Class_cambio_tipologia_gabinete : Retorna la estructura de gestión
        '                     
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-08-19
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim Class_ra_auto_campos_gestion_expediente As New Class_ra_auto_campos_gestion_expediente
            Dim Class_series_documentales As New Class_series_documentales
            Dim Class_subseries_documentales As New Class_subseries_documentales
            Dim Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
            Dim id_auto_registro As Integer = 0
            Dim id_expediente As Integer = 0
            Dim Result As String = ""
            Dim id_fondo As Integer = 0
            Dim id_instrumento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim expediente_conservacion() As expediente_conservacion = Nothing
            Result = Me.Solicita_id_expediente_imagen_gabinete(id_imagen,
                                                               gabinete,
                                                               id_expediente)
            If Result <> "YES" Then
                Solicita_lista_series_relacionadas_gabinete_migracion = Result
                Exit Function
            End If
            If id_expediente = 0 Then
                '----------------------------------------------------------------
                '-------Solicita las lista de tramites permitidos para gabinete
                '-------para realizar operaciones de vinculación y cambio de tipo
                '-------logias documentales
                '----------------------------------------------------------------
                Class_cambio_tipologia_gabinete.iLIStTipoTramite = New List(Of control_drow_lista)
                Result = Class_tipo_doc_entrante.Solicita_lista_tramite_auto_vinculacion_gabinete(id_gabinete,
                                                                                                  Class_cambio_tipologia_gabinete.iLIStTipoTramite)
                If Result <> "YES" Then
                    Solicita_lista_series_relacionadas_gabinete_migracion = Result
                    Exit Function
                End If
                If Class_cambio_tipologia_gabinete.iLIStTipoTramite.Count > 0 Then
                    '--------------------------------------------------------------
                    '-------Solicita la identificación del primer tramite
                    '--------------------------------------------------------------
                    id_auto_registro = Val(Class_cambio_tipologia_gabinete.iLIStTipoTramite.Item(0).value)
                    Result = Solicita_gestion_autoregistro_gabinete(id_auto_registro,
                                                                    Class_cambio_tipologia_gabinete)
                    If Result <> "YES" Then
                        Solicita_lista_series_relacionadas_gabinete_migracion = Result
                        Exit Function
                    End If

                End If
                Solicita_lista_series_relacionadas_gabinete_migracion = "YES"
                Exit Function
            Else
                '------------------------------------------------------------
                '-----------Solicita estructura expediente para extraer los 
                '-----------los datos de gestión documental
                '------------------------------------------------------------
                Result = ClassGaExpediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                           expediente_conservacion)
                If Result <> "YES" Then
                    Solicita_lista_series_relacionadas_gabinete_migracion = Result
                    Exit Function
                End If
                If expediente_conservacion(0).CODIGO_SERIE = 0 Then
                    Solicita_lista_series_relacionadas_gabinete_migracion = "Debe vincular el expediente (" & expediente_conservacion(0).CODIGO_UNICO & ") a una serie documental"
                    Exit Function
                End If
                '------------------------------------------------------------
                '-----------Solicita lista de series
                '------------------------------------------------------------
                Class_cambio_tipologia_gabinete.iLIStSerie = New List(Of control_drow_lista)
                Result = Class_series_documentales.Solicita_lista_serie_id_serie(expediente_conservacion(0).CODIGO_SERIE,
                                                                                 Class_cambio_tipologia_gabinete.iLIStSerie)
                If Result <> "YES" Then
                    Solicita_lista_series_relacionadas_gabinete_migracion = Result
                    Exit Function
                End If
                '-------------------------------------------------------------
                '---------Solicita tipos documentales series
                '-------------------------------------------------------------
                Class_cambio_tipologia_gabinete.iLIStTipo = New List(Of control_drow_lista)
                Result = Class_ra_tipo_doc_series.Solicita_lista_tipos_documentales_relacionados_id_serie(expediente_conservacion(0).CODIGO_SERIE,
                                                                                                          Class_cambio_tipologia_gabinete.iLIStTipo)
                If Result <> "YES" Then
                    Solicita_lista_series_relacionadas_gabinete_migracion = Result
                    Exit Function
                End If
                If Class_cambio_tipologia_gabinete.iLIStTipo.Count > 0 Then
                    Solicita_lista_series_relacionadas_gabinete_migracion = "YES"
                    Exit Function
                Else
                    '----------------------------------------------------------
                    '------Solicita sub series documentales
                    '---------------------------------------------------------
                    Class_cambio_tipologia_gabinete.iLIStSubSerie = New List(Of control_drow_lista)
                    Result = Class_subseries_documentales.Solicita_lista_series_sub_documentales_id_serie(expediente_conservacion(0).CODIGO_SERIE,
                                                                                                          Class_cambio_tipologia_gabinete.iLIStSubSerie)
                    If Result <> "YES" Then
                        Solicita_lista_series_relacionadas_gabinete_migracion = Result
                        Exit Function
                    End If
                    '-------------------------------------------------------------
                    '-------Lista tipos documentales relacionados a la sub serie
                    '-------------------------------------------------------------
                    If Class_cambio_tipologia_gabinete.iLIStSubSerie.Count > 0 Then
                        Class_cambio_tipologia_gabinete.iLIStTipo = New List(Of control_drow_lista)
                        Result = Class_ra_tipo_doc_series.Solicita_lista_tipos_documentales_relacionados_id_sub_serie(Val(Class_cambio_tipologia_gabinete.iLIStSubSerie.Item(0).value),
                                                                                                                      Class_cambio_tipologia_gabinete.iLIStTipo)
                        If Result <> "YES" Then
                            Solicita_lista_series_relacionadas_gabinete_migracion = Result
                            Exit Function
                        End If
                    End If
                End If
                Solicita_lista_series_relacionadas_gabinete_migracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_series_relacionadas_gabinete_migracion = "Inconsistencia general funcion Solicita_lista_series_relacionadas_gabinete_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_id_expediente_imagen_gabinete(ByVal id_imagen As Integer,
                                                    ByVal gabinete As String,
                                                    ByRef id_expediente As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la identiifcación del expediente relacionado al documento
        '          en el gabinete, con el nombre del gabinete y la identificación
        '          de la imagen dentro del gabinete
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen      : Representa la identificación de la imagen en el gabinete
        'gabinete       : Representa el nombre del gabinete al que pertence la image
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_expediente  : Retorna la identiifcación del expediente
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-17
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SQLconsulta As String = "Select ID_EXPEDIENTE from " & gabinete & " where ID=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(gabinete)
            Result = ref.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            If Result <> "YES" Then
                Solicita_id_expediente_imagen_gabinete = "Error funcion Solicita_id_expediente_imagen_gabinete " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_expediente_imagen_gabinete = "Imposible encontrar la identificación del documento (" & id_imagen & ") en el gabinete (" & gabinete & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_expediente = 0
                Else
                    id_expediente = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_expediente_imagen_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_expediente_imagen_gabinete = "Inconsistencia general funcion Solicita_id_expediente_imagen_gabinete " & ex.Message
        End Try
    End Function
    Function Asigna_valores_campos_gabinete_auto_relacion_gabinete_expediente(ByVal id_imagen As Integer,
                                                                              ByVal gabinete As String,
                                                                              ByVal Ra_auto_rel_campos_gabinete_expediente() As ra_auto_rel_campos_gabinete_expediente) As String
        '---------------------------------------------------------------------------
        'Funcion : Asigna valores campos gabinete expediente para el auto
        '          registro de expedientes
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen      : Representa la identificación de la imagen en el gabinete
        'gabinete       : Representa el nombre del gabinete al que pertence la image
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Ra_auto_rel_campos_gabinete_expediente  : Retorna la estructura de relacion
        'con los valores de los campos 
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-08-13
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SQLconsulta As String = ""
            Dim Sql_campos As String = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To Ra_auto_rel_campos_gabinete_expediente.Length - 1
                If i = 0 Then
                    Sql_campos = "Select " & Ra_auto_rel_campos_gabinete_expediente(i).CAMPO
                Else
                    Sql_campos = Sql_campos & "," & Ra_auto_rel_campos_gabinete_expediente(i).CAMPO
                End If
            Next
            SQLconsulta = Sql_campos & " from " & gabinete & " where ID=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(gabinete)
            Result = ref.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            If Result <> "YES" Then
                Asigna_valores_campos_gabinete_auto_relacion_gabinete_expediente = "Error funcion Asigna_valores_campos_gabinete_auto_relacion_gabinete_expediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_valores_campos_gabinete_auto_relacion_gabinete_expediente = "Imposible encontrar los datos de registro del documento (" & id_imagen & ") en el " &
                    " gabinete (" & gabinete & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    For z As Integer = 0 To Ra_auto_rel_campos_gabinete_expediente.Length - 1
                        If UCase(Datset.Tables(0).Columns(i).ColumnName) = UCase(Ra_auto_rel_campos_gabinete_expediente(z).CAMPO) Then
                            If Datset.Tables(0).Rows(0).IsNull(Datset.Tables(0).Columns(i).ColumnName) Then
                                Ra_auto_rel_campos_gabinete_expediente(z).value_campo_expediente = ""
                                Ra_auto_rel_campos_gabinete_expediente(z).value_campo_gabinete = ""
                            Else
                                Ra_auto_rel_campos_gabinete_expediente(z).value_campo_expediente = Left(Datset.Tables(0).Rows(0).Item(Datset.Tables(0).Columns(i).ColumnName),
                                                                                                        Ra_auto_rel_campos_gabinete_expediente(z).longitud_campo)
                                Ra_auto_rel_campos_gabinete_expediente(z).value_campo_gabinete = Datset.Tables(0).Rows(0).Item(Datset.Tables(0).Columns(i).ColumnName)
                                If Ra_auto_rel_campos_gabinete_expediente(z).TIPO = "DATE" Then
                                    ClassGestionFechas.Formatea_fecha_time_base_mysql(Ra_auto_rel_campos_gabinete_expediente(z).value_campo_expediente,
                                                                                      Ra_auto_rel_campos_gabinete_expediente(z).value_campo_expediente)
                                End If

                            End If
                        End If
                    Next
                Next
                Asigna_valores_campos_gabinete_auto_relacion_gabinete_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_valores_campos_gabinete_auto_relacion_gabinete_expediente = "Inconsistencia general función Asigna_valores_campos_gabinete_auto_relacion_gabinete_expediente " & ex.Message
        End Try
    End Function
    Function Lista_documentos_visor_a_migrar(ByVal id_imagen As Integer,
                                             ByVal gabinete As String,
                                             ByRef class_stru_visor_migracion As class_stru_visor_migracion) As String
        '--------------------------------------------------------------------------------------
        'Funcion : Solicita el tipo de archivo a visualuizar y retorna la url de visualización
        '         
        '--------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------
        'id_imagen                    : Representa la identiifcación de la imagen
        '                               
        'gabinete                     : Representa el nombre del gabinete
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
        'Fecha                 : 2024-06-19
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim id_tipo_imagen As Integer = 0
            Dim Result As String = ClassDaGabinete.SolicitaIdTipoImagen(id_imagen,
                                                                          gabinete,
                                                                          id_tipo_imagen)
            If Result <> "YES" Then
                Lista_documentos_visor_a_migrar = Result
                Exit Function
            End If
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen,
                                                                                 class_stru_visor_migracion.tipo_file)
            If Result <> "YES" Then
                Lista_documentos_visor_a_migrar = Result
                Exit Function
            End If
            If class_stru_visor_migracion.tipo_file = ".TIF" Or class_stru_visor_migracion.tipo_file = ".JPG" Or class_stru_visor_migracion.tipo_file = ".BMP" Then
                class_stru_visor_migracion.url_iframe = "../Docuarchi/WebFormDaVisorVersionPublico.aspx"
            Else
                class_stru_visor_migracion.url_iframe = "../Docuarchi/WebFormDaVisorExterno.aspx"
            End If
            HttpContext.Current.Session.Item("DA_IMAGEN") = id_imagen
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = gabinete
            Lista_documentos_visor_a_migrar = "YES"
        Catch ex As Exception
            Lista_documentos_visor_a_migrar = "Inconsistencia general funcion Lista_documentos_visor_a_migrar " & ex.Message
        End Try
    End Function
    Function SolicitaUrlVisorConsulta(ByVal IdImagenGabinete As Integer,
                                      ByVal Gabinete As String,
                                      ByRef ClassStruVisorMigracion As class_stru_visor_migracion) As String
        '--------------------------------------------------------------------------------------
        'Funcion : Solicita la url para visualizar un tipo de archivo
        '         
        '--------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------
        'IdImagenGabinete             : Representa la identiifcación de la imagen
        '                               
        'Gabinete                     : Representa el nombre del gabinete
        '                               
        '                             
        '                            
        '-------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'ClassStruVisorMigracion     : Retorna la estructura con los datos de visualizacion
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2025-08-28
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim IdTipoImagen As Integer = 0
            Dim Result As String = ClassDaGabinete.SolicitaIdTipoImagen(IdImagenGabinete,
                                                                        Gabinete,
                                                                        IdTipoImagen)
            If Result <> "YES" Then
                Return Result
            End If
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(IdTipoImagen,
                                                                                 ClassStruVisorMigracion.tipo_file)
            If Result <> "YES" Then
                Return Result
            End If
            If ClassStruVisorMigracion.tipo_file = ".TIF" Or ClassStruVisorMigracion.tipo_file = ".JPG" Or ClassStruVisorMigracion.tipo_file = ".BMP" Then
                ClassStruVisorMigracion.url_iframe = "../Docuarchi/WebFormDaVisorVersionPublico.aspx"
            Else
                ClassStruVisorMigracion.url_iframe = "../Docuarchi/WebFormDaVisorExterno.aspx"
            End If
            HttpContext.Current.Session.Item("DA_IMAGEN") = IdImagenGabinete
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = Gabinete
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitaUrlVisorConsulta " & ex.Message
        End Try
    End Function
    Function ConsultaGabinete(ByVal TipoConsulta As Object,
                              ByVal ValorConsulta As String,
                              ByVal IdGabinete As Object,
                              ByVal ClassConfigGeneralService As List(Of Class_config_general_service),
                              ByRef ClassStruRowGabineteGeneric As class_stru_Row_Gabinete_Generic) As String
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web Solicita la consulta sobre gabinetes
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'ClassConfigGeneralService    : Representa la estructura del indice extraidos
        '                               de la interface
        'TipoConsulta                 : Tipo de consulta de gabinete  1 - Consulta
        '                               Campos  2- Tipo de consulta general todos los campos
        'ValorConsulta                : Valor de consulta para tipo de consulta 2
        'IdGabinete                   : Representa identificacion del gabinete
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'ClassStruRowGabineteGeneric : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2025-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim EstructuraCamposGabinete() As estructura_gabinete = Nothing
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinetePorId(IdGabinete,
                                                                                   EstructuraCamposGabinete)
            If Result <> "YES" Then
                Return Result
            End If
            Dim ClassCamposTableBostraTable As New List(Of class_campos_table_bostra_table)
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposConsultaGabineteBootStra(EstructuraCamposGabinete,
                                                                                              ClassStruRowGabineteGeneric.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Return Result
            End If
            Dim gabinete As String = ""
            Dim Class_system1 As New Class_system1
            '---/// Solicita el nombre del gabinete por número de identificación de gabinete
            Result = Class_system1.SolicitaNombreGabinetePorId(IdGabinete,
                                                               gabinete)
            If Result <> "YES" Then
                Return Result
            End If
            Dim SqlConsulta As String = ""
            ' --------- /// Solicita el comando SQL para realizar la consulta
            Result = Me.SolicitasqlConsultaGabinete(ClassConfigGeneralService,
                                                    TipoConsulta,
                                                    ValorConsulta,
                                                    gabinete,
                                                    ClassStruRowGabineteGeneric.Obj_ilist_fileds_generic,
                                                    SqlConsulta)
            If Result <> "YES" Then
                Return Result
            End If
            Result = SolicitaEstructuraConsultaGabinete(SqlConsulta,
                                                        ClassStruRowGabineteGeneric.Obj_ilist_row_generic)
            Return Result
        Catch ex As Exception
            Return "Inconsistecia general funcion ConsultaGabinete " & ex.Message
        End Try
    End Function
    Function Consulta_gabinete_migracion(ByVal tipo_consulta As Object,
                                         ByVal valor_consulta As String,
                                         ByVal id_gabinete As Object,
                                         ByVal Class_config_general_service As List(Of Class_config_general_service),
                                         ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '----------------------------------------------------------------------------------
        'Funcion : Servicio web Solicita la consulta sobre gabinetes de migración
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'Class_config_general_service : Representa la estructura del indice extraidos
        '                               de la interface
        'tipo_consulta                : Tipo de consulta de gabinete migracion 1 - consulta
        '                               campos  2- Tipo de consulta general todos los campos
        'valor_consulta               : Valor de consulta para tipo de consulta 2
        'id_gabinete                  : Representa identificacion del gabiete
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_stru_date_Gabinete_Generic : Retorna la estructura de datos de la consulta
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try

            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim Class_system1 As New Class_system1
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            '------- /// Solicita la estructura de campos o field de la tabla y de la consulta sql
            Result = Class_DETALLE_GABIENETE.Solicita_estructura_campos_dynamic_migracion(id_gabinete,
                                                                                          class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                Consulta_gabinete_migracion = Result
                Exit Function
            End If
            Dim gabinete As String = ""
            '---/// Solicita el nombre del gabinete por número de identificación de gabinete
            Result = Class_system1.SolicitaNombreGabinetePorId(id_gabinete,
                                                               gabinete)
            If Result <> "YES" Then
                Consulta_gabinete_migracion = Result
                Exit Function
            End If
            Dim sql_consulta As String = ""
            ' --------- /// Solicita el comando SQL para realizar la consulta
            Result = ClassDaGabinete.Solicita_Sql_Consulta_gabinete_migracion(Class_config_general_service,
                                                                              tipo_consulta,
                                                                              valor_consulta,
                                                                              gabinete,
                                                                              class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic,
                                                                              sql_consulta)
            If Result <> "YES" Then
                Consulta_gabinete_migracion = Result
                Exit Function
            End If
            '--------/// Solicita la estructura de la consulta para dibujar en la interfaz
            Result = ClassDaGabinete.Solicita_structura_consulta_gabinete_migracion(sql_consulta,
                                                                                    class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                Consulta_gabinete_migracion = Result
                Exit Function
            End If
            Consulta_gabinete_migracion = "YES"
        Catch ex As Exception
            Consulta_gabinete_migracion = "Inconsistencia general funcion Consulta_gabinete_migracion " & ex.Message
        End Try
    End Function
    Function Veri_existe_regitro(ByVal country As Object,
                                 ByVal valor As String,
                                 ByRef estado_exist As String) As String
        Try
            Veri_existe_regitro = "NO"
            For i As Integer = 0 To country.Count - 1
                If Trim(country(i).ToString) = Trim(valor) Then
                    estado_exist = "YES"
                    Veri_existe_regitro = "YES"
                    Exit Function
                End If
            Next
            Veri_existe_regitro = "YES"
        Catch ex As Exception
            Veri_existe_regitro = "Inconcistencia general función Veri_existe_regitro " & ex.Message
        End Try
    End Function
    Function SolicitaAutoCompleteCampoGabinete(ByVal request As AutoCompleteRequest,
                                               ByRef country As List(Of String)) As String
        Try
            country = New List(Of String)()
            Dim conexDbase As Object = If(request.NameDbsAuto = "WF",
                                      New conect.Dbase_Conction_Mysql(),
                                      New conect.Dbase_Conction_Mysql_RA())
            Dim selectSql As String = $" SELECT distinct {request.NameCampoAuto} "
            Dim whereSql As String = $" WHERE  {request.NameCampoAuto}  LIKE '%{request.Value}%'"
            ' Armar consulta final
            Dim sqlFrom As String = $" FROM {request.NameTableAuto} AS da"
            Dim sqlConsulta As String = $"{selectSql} {sqlFrom} {whereSql} AND (DBT <= 1) ORDER BY {request.NameCampoAuto}  DESC LIMIT 50"
            Dim dataset As New DataSet("DAT_ADIC")
            Dim result = conexDbase.SELECTION_SELECT_FIELD(sqlConsulta, dataset)
            If result <> "YES" Then Return result
            ' Vimalidar resultados
            If dataset.Tables(0).Rows.Count = 0 Then
                country = Nothing
                Return "YES"
            End If
            ' Procesar resultados y evitar duplicados
            For Each row As DataRow In dataset.Tables(0).Rows
                For Each col As DataColumn In dataset.Tables(0).Columns
                    If Not row.IsNull(col) Then
                        Dim valor As String = If(TypeOf row(col) Is DateTime,
                                             DirectCast(row(col), DateTime).ToString("yyyy-MM-dd"),
                                             row(col).ToString())

                        If Not country.Contains(valor) Then
                            country.Add(valor)
                        End If
                    End If
                Next
            Next
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitaAutoCompleteCampoGabinete " & ex.Message
        End Try
    End Function
    Function SolicitaDatosAutoCompleteConsultaGabinete(ByVal request As AutoCompleteRequest,
                                                       ByRef country As List(Of String)) As String
        '---------------------------------------------------------------------------
        'Funcion : Servicio que solicita la estructura con los registro de auto AutoCompleteRequest
        '          de auto complete de una gabinete para la cosulta de migración
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'request              : Representa la estructura contenedora de los parametros
        '                       nombre de tabla name_table_auto y el dbms de conuslta
        '                       name_dbs_auto.
        'value                : Representa el valor de consulta sobre la tabla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'country  : Retorna la estructura con los registtros
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-08-27
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            country = New List(Of String)()
            Dim classCampos As List(Of class_campos_table_bostra_table) = Nothing
            Dim estructuraCampos() As estructura_gabinete = Nothing

            ' Obtener estructura
            Dim detalleGabinete As New Class_DETALLE_GABIENETE
            Dim result As String = detalleGabinete.SolicitaEstructuraCamposGabinetePorId(request.IdTable, estructuraCampos)
            If result <> "YES" Then Return result

            result = detalleGabinete.SolicitaEstructuraCamposConsultaGabineteBootStra(estructuraCampos, classCampos)
            If result <> "YES" Then Return result

            ' Obtener campos visibles
            Dim camposVisibles = classCampos.Where(Function(c) c.visible_like_sql = 1).Select(Function(c) c.field).ToList()
            If Not camposVisibles.Any() Then Return "No hay campos visibles configurados"

            ' Construcción del SELECT dinámico
            Dim selectSql As String = "SELECT " & String.Join(",", camposVisibles)

            ' Construcción del WHERE con condiciones LIKE
            Dim condiciones = camposVisibles.Select(Function(f) $"{f} LIKE '%{request.Value}%'").ToList()
            Dim whereSql As String = " WHERE " & String.Join(" OR ", condiciones)

            ' Selección de la conexión DB
            Dim conexDbase As Object = If(request.NameDbsAuto = "WF",
                                      New conect.Dbase_Conction_Mysql(),
                                      New conect.Dbase_Conction_Mysql_RA())

            ' Construcción de la consulta final
            Dim sqlFrom As String = $" FROM {request.NameTableAuto} AS da"
            Dim sqlConsulta As String = $"{selectSql} {sqlFrom} {whereSql} AND DBT <= 1 ORDER BY ID DESC LIMIT 500"

            ' Ejecutar consulta
            Dim dataset As New DataSet("DAT_ADIC")
            result = conexDbase.SELECTION_SELECT_FIELD(sqlConsulta, dataset)
            If result <> "YES" Then Return result

            ' Validar que haya datos
            If dataset.Tables(0).Rows.Count = 0 Then
                country = Nothing
                Return "YES"
            End If
            ' Usar un HashSet para evitar duplicados de manera eficiente
            Dim uniqueValues As New HashSet(Of String)()
            ' Procesar resultados
            For Each row As DataRow In dataset.Tables(0).Rows
                For Each col As DataColumn In dataset.Tables(0).Columns
                    If Not row.IsNull(col) Then
                        ' Convertir los valores de fecha a formato 'yyyy-MM-dd' si es necesario
                        Dim valor As String = If(TypeOf row(col) Is DateTime,
                                             DirectCast(row(col), DateTime).ToString("yyyy-MM-dd"),
                                             row(col).ToString())

                        ' Agregar solo si no existe (HashSet evita duplicados automáticamente)
                        If valor.IndexOf(request.Value, StringComparison.OrdinalIgnoreCase) >= 0 Then
                            ' Agregar solo si no existe (HashSet evita duplicados automáticamente)
                            If uniqueValues.Add(valor) Then
                                country.Add(valor)
                            End If
                        End If
                    End If
                Next
            Next
            If Not country Is Nothing Then
                country.Sort()
            End If
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia en función SolicitaDatosAutoCompleteConsultaGabinete: " & ex.Message
        End Try
    End Function

    'Function SolicitaDatosAutoCompleteConsultaGabinete(ByVal request As AutoCompleteRequest,
    '                                                   ByRef country As List(Of String)) As String
    '    '---------------------------------------------------------------------------
    '    'Funcion : Servicio que solicita la estructura con los registro de auto AutoCompleteRequest
    '    '          de auto complete de una gabinete para la cosulta de migración
    '    '         
    '    '---------------------------------------------------------------------------
    '    '                           PARAMETROS  
    '    '---------------------------------------------------------------------------
    '    'request              : Representa la estructura contenedora de los parametros
    '    '                       nombre de tabla name_table_auto y el dbms de conuslta
    '    '                       name_dbs_auto.
    '    'value                : Representa el valor de consulta sobre la tabla
    '    '---------------------------------------------------------------------------
    '    '                           RETORNO
    '    '---------------------------------------------------------------------------
    '    'country  : Retorna la estructura con los registtros
    '    '---------------------------------------------------------------------------
    '    '                         CARACTERIZACIÓN
    '    '---------------------------------------------------------------------------
    '    'Fecha                 : 2025-08-27
    '    'Elabora               : Miguel Angel Urueta Miranda
    '    '----------------------------------------------------------------------------
    '    Try
    '        country = New List(Of String)()
    '        Dim classCampos As List(Of class_campos_table_bostra_table) = Nothing
    '        Dim estructuraCampos() As estructura_gabinete = Nothing
    '        ' Obtener estructura
    '        Dim detalleGabinete As New Class_DETALLE_GABIENETE
    '        Dim result As String = detalleGabinete.SolicitaEstructuraCamposGabinetePorId(request.IdTable, estructuraCampos)
    '        If result <> "YES" Then Return result

    '        result = detalleGabinete.SolicitaEstructuraCamposConsultaGabineteBootStra(estructuraCampos, classCampos)
    '        If result <> "YES" Then Return result

    '        ' Construcción del SELECT dinámico
    '        Dim camposVisibles = classCampos.Where(Function(c) c.visible_like_sql = 1).Select(Function(c) c.field).ToList()
    '        If Not camposVisibles.Any() Then Return "No hay campos visibles configurados"

    '        Dim selectSql As String = "SELECT " & String.Join(",", camposVisibles)

    '        ' Construcción del WHERE con condiciones LIKE
    '        Dim condiciones = camposVisibles.Select(Function(f) $"{f} LIKE '%{request.Value}%'")
    '        Dim whereSql As String = " WHERE " & String.Join(" OR ", condiciones)

    '        ' Conexión según DB
    '        Dim conexDbase As Object = If(request.NameDbsAuto = "WF",
    '                                  New conect.Dbase_Conction_Mysql(),
    '                                  New conect.Dbase_Conction_Mysql_RA())

    '        ' Armar consulta final
    '        Dim sqlFrom As String = $" FROM {request.NameTableAuto} AS da"
    '        Dim sqlConsulta As String = $"{selectSql} {sqlFrom} {whereSql} AND DBT <= 1 ORDER BY ID DESC LIMIT 50"

    '        ' Ejecutar consulta
    '        Dim dataset As New DataSet("DAT_ADIC")
    '        result = conexDbase.SELECTION_SELECT_FIELD(sqlConsulta, dataset)
    '        If result <> "YES" Then Return result

    '        ' Validar resultados
    '        If dataset.Tables(0).Rows.Count = 0 Then
    '            country = Nothing
    '            Return "YES"
    '        End If
    '        ' Procesar resultados y evitar duplicados
    '        For Each row As DataRow In dataset.Tables(0).Rows
    '            For Each col As DataColumn In dataset.Tables(0).Columns
    '                If Not row.IsNull(col) Then
    '                    Dim valor As String = If(TypeOf row(col) Is DateTime,
    '                                         DirectCast(row(col), DateTime).ToString("yyyy-MM-dd"),
    '                                         row(col).ToString())

    '                    If Not country.Contains(valor) Then
    '                        country.Add(valor)
    '                    End If
    '                End If
    '            Next
    '        Next
    '        Return "YES"
    '    Catch ex As Exception
    '        Return "Inconsistencia en función SolicitaDatosAutoCompleteConsultaGabinete: " & ex.Message
    '    End Try
    'End Function

    'Function SolicitaDatosAutoCompleteConsultaGabinete(ByVal AutoCompleteRequest As AutoCompleteRequest,
    '                                                   ByRef country As List(Of String)) As String
    '    Try
    '        Dim class_campos_table_bostra_table As List(Of class_campos_table_bostra_table) = Nothing
    '        Dim Result As String = ""
    '        Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
    '        Dim ConexDbase As Object
    '        Dim SqlConsulta As String = ""
    '        country = New List(Of String)()
    '        Dim EstructuraCampoGabinete() As estructura_gabinete = Nothing
    '        Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinetePorId(AutoCompleteRequest.IdTable,
    '                                                                               EstructuraCampoGabinete)
    '        If Result <> "YES" Then
    '            Return Result
    '        End If
    '        Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposConsultaGabineteBootStra(EstructuraCampoGabinete,
    '                                                                                          class_campos_table_bostra_table)
    '        If Result <> "YES" Then
    '            Return Result
    '        End If
    '        Dim seleccampos As String = "Select "
    '        Dim campo_clase_documento As String = ""
    '        Dim campo_expediente As String = ""
    '        For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
    '            If class_campos_table_bostra_table.Item(i).visible_like_sql = 1 Then
    '                If seleccampos = "Select " Then
    '                    seleccampos = seleccampos & class_campos_table_bostra_table(i).field
    '                Else
    '                    seleccampos = seleccampos & "," & class_campos_table_bostra_table(i).field
    '                End If
    '            End If
    '        Next
    '        Dim condicionsql As String = " where "
    '        Dim likeigual As String = " Like"
    '        For i As Integer = 0 To class_campos_table_bostra_table.Count - 1
    '            If class_campos_table_bostra_table.Item(i).visible_like_sql = 1 Then
    '                If condicionsql = " where " Then
    '                    condicionsql = condicionsql & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & AutoCompleteRequest.Value & "%'"
    '                Else
    '                    condicionsql = condicionsql & " or " & class_campos_table_bostra_table.Item(i).field & likeigual & "'%" & AutoCompleteRequest.Value & "%'"
    '                End If
    '            End If
    '        Next

    '        If AutoCompleteRequest.NameDbsAuto = "WF" Then
    '            ConexDbase = New conect.Dbase_Conction_Mysql
    '        Else
    '            ConexDbase = New conect.Dbase_Conction_Mysql_RA
    '        End If
    '        Dim order_colum As String = "DESC"
    '        Dim colum_order_name As String = "ID"
    '        Dim sqlfrom As String = " From " & AutoCompleteRequest.NameTableAuto & " as da"
    '        SqlConsulta = seleccampos & " " & sqlfrom & " " & condicionsql & " AND DBT <=1 Order by " & colum_order_name & " " & order_colum & " LIMIT 50"
    '        Dim Datset As DataSet = New DataSet("DAT_ADIC")
    '        Result = ConexDbase.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
    '        If Result <> "YES" Then
    '            Return Result
    '        End If
    '        If Datset.Tables(0).Rows.Count = 0 Then
    '            country = Nothing
    '            Return "YES"
    '        Else
    '            If Datset.Tables(0).Rows.Count > 0 Then
    '                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
    '                    For z As Integer = 0 To Datset.Tables(0).Columns.Count - 1
    '                        If Datset.Tables(0).Rows(i).IsNull(z) = False Then
    '                            Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(z).GetType.ToString
    '                            Dim estado_exit As String = "NO"
    '                            If obsgetipe = "System.DateTime" Then
    '                                Dim subtrin As String = Datset.Tables(0).Rows(i).Item(z).ToString()
    '                                Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
    '                                Dim tempo_fecha As String = splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0)
    '                                Veri_existe_regitro(country,
    '                                          tempo_fecha,
    '                                          estado_exit)
    '                                If estado_exit = "NO" Then
    '                                    country.Add(tempo_fecha)
    '                                End If
    '                            Else
    '                                Veri_existe_regitro(country,
    '                                         Datset.Tables(0).Rows(i).Item(z).ToString(),
    '                                         estado_exit)
    '                                If estado_exit = "NO" Then
    '                                    country.Add(Datset.Tables(0).Rows(i).Item(z).ToString())
    '                                End If

    '                            End If
    '                        End If
    '                    Next
    '                Next
    '            End If
    '            Return "YES"
    '        End If
    '    Catch ex As Exception
    '        Return "Inconsistencia funcion SolicitaDatosAutoCompleteConsultaGabinete"
    '    End Try
    'End Function
    Function Solicita_datos_auto_complete_gabinete_migracion(ByVal name_dbs_auto As String,
                                                             ByVal name_table_auto As String,
                                                             ByVal value_auto As String,
                                                             ByRef country As List(Of String)) As String
        '--------------------------------------------------------------------------------
        'Funcion : Solicita la estructura con los registro de auto 
        '          de auto complete de una gabinete para la cosulta de migración
        '         
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'name_dbs_auto        : Representa el nombre del dbs de conexion a basde de datos
        'name_table_auto      : Representa el nombre de la tabla de consulta                              
        'value                : Representa el valor de consulta sobre la tabla
        '--------------------------------------------------------------------------------
        '                           RETORNO
        '--------------------------------------------------------------------------------
        'country              : Retorna la estructura con los registros
        '--------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '--------------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Try
            Dim ref As Object
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Class_system1 As New Class_system1
            Dim id_gabinete As Integer = 0
            country = New List(Of String)()
            Result = Class_system1.SolicitaIdGabineteDocuarchi(name_table_auto,
                                                               id_gabinete)
            If Result <> "YES" Then
                Solicita_datos_auto_complete_gabinete_migracion = Result
                Exit Function
            End If
            Dim class_campos_table_bostra_table As List(Of class_campos_table_bostra_table) = Nothing
            Result = Class_DETALLE_GABIENETE.Solicita_estructura_campos_dynamic_migracion(id_gabinete,
                                                                                          class_campos_table_bostra_table)
            If Result <> "YES" Then
                Solicita_datos_auto_complete_gabinete_migracion = Result
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
            Dim colum_order_name As String = "ID"
            Dim sqlfrom As String = " From " & name_table_auto & " as da"
            Sql_consulta = seleccampos & " " & sqlfrom & " " & condicionsql & " AND DBT <=1 Order by " & colum_order_name & " " & order_colum & " LIMIT 50"
            Dim Datset As DataSet = New DataSet("DAT_ADIC")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_auto_complete_gabinete_migracion = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                country = Nothing
                Solicita_datos_auto_complete_gabinete_migracion = "YES"
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
                                    Veri_existe_regitro(country,
                                              tempo_fecha,
                                              estado_exit)
                                    If estado_exit = "NO" Then
                                        country.Add(tempo_fecha)
                                    End If
                                Else
                                    Veri_existe_regitro(country,
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

                Solicita_datos_auto_complete_gabinete_migracion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_auto_complete_gabinete_migracion = "Inconsistencia general funcion Solicita_datos_auto_complete_gabinete_migracion " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraConsultaGabinete(ByVal SqlConsulta As String,
                                                ByRef StruRowGabineteGeneric As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '          del gabinete
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'consulta               : Representa la consulta en comando SQL
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '  : Retorna la estructura de datos de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(SqlConsulta, Datset)
            If Result <> "YES" Then
                Return " Inconsistencia funcion  SolicitaEstructuraConsultaGabinete " & Result
            End If
            StruRowGabineteGeneric = JsonConvert.SerializeObject(Datset.Tables(0))
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general fucnion SolicitaEstructuraConsultaGabinete " & ex.Message
        End Try
    End Function
    Function Solicita_structura_consulta_gabinete_migracion(ByVal consulta As String,
                                                            ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         del de documentos para migracion
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'consulta               : Representa la consulta en comando SQL
        '                        
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '  : Retorna la estructura de datos de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-06-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                Solicita_structura_consulta_gabinete_migracion = "Funcion  Solicita_structura_consulta_gabinete_migracion " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            Solicita_structura_consulta_gabinete_migracion = "YES"
        Catch ex As Exception
            Solicita_structura_consulta_gabinete_migracion = "Inconsistencia general fucnion Solicita_structura_consulta_gabinete_migracion " & ex.Message
        End Try
    End Function
    Function Solicita_Sql_Consulta_gabinete_migracion(ByVal Class_config_general_service As List(Of Class_config_general_service),
                                                      ByVal tipo_consulta As Integer,
                                                      ByVal valor_consulta As String,
                                                      ByVal gabinete As String,
                                                      ByVal class_campos_table_bostra_table As List(Of class_campos_table_bostra_table),
                                                      ByRef consulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta del gabinete de migracion
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
        'Fecha                 : 2024-06-04
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
            Dim Sql_carte_extension As String = " LEFT JOIN da_extension as de on (dbt=de.ESTADO_NORMAL or dbt=de.ESTADO_ADJUNTO)  "
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
                            If condicionsql = " where " Then
                                condicionsql = condicionsql & campo_plantilla & " between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
                            Else
                                condicionsql = condicionsql & " and " & campo_plantilla & "  between '" & Class_config_general_service.Item(i).value_campo & "' and '" & Class_config_general_service.Item(i).value_campo_beetwen & "'"
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
            Dim colum_order_name As String = "ID"
            Dim sqlfrom As String = " From " & gabinete & " as da"
            consulta = seleccampos & " " & sqlfrom & " " & Sql_carte_extension & " " & condicionsql & " AND DBT <=1 Order by " & colum_order_name & " " & order_colum & " LIMIT 5000"
            Solicita_Sql_Consulta_gabinete_migracion = "YES"
        Catch ex As Exception
            Solicita_Sql_Consulta_gabinete_migracion = "Inconsistencia general funcion Consulta_gabinete_migracion " & ex.Message
        End Try
    End Function
    Function SolicitasqlConsultaGabinete(ByVal ClassConfigGeneralService As List(Of Class_config_general_service),
                                         ByVal TipoConsulta As Integer,
                                         ByVal ValorConsulta As String,
                                         ByVal Gabinete As String,
                                         ByVal ClassCamposTableBostraTable As List(Of class_campos_table_bostra_table),
                                         ByRef SqlConsulta As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el sql de consulta del gabinete
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'ClassConfigGeneralService     : Representa la clase generica con los campoa
        'TipoConsulta                  : Representa el tipo de consulta
        'ValorConsulta                 : Representa el valor para la consulta tipo like
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'SqlConsulta  : Retorna comando sql de consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-08-26
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim condicionsql As String = " where "
            Dim seleccampos As String = "Select "
            Dim campo_clase_documento As String = ""
            Dim campo_expediente As String = ""
            For i As Integer = 0 To ClassCamposTableBostraTable.Count - 1
                If ClassCamposTableBostraTable(i).viisble_sql = "1" Then
                    If seleccampos = "Select " Then
                        seleccampos = seleccampos & ClassCamposTableBostraTable(i).field
                    Else
                        seleccampos = seleccampos & "," & ClassCamposTableBostraTable(i).field
                    End If
                End If
            Next
            'Dim Sql_carte_extension As String = " LEFT JOIN da_extension as de on (dbt=de.ESTADO_NORMAL or dbt=de.ESTADO_ADJUNTO)  "
            Dim Sql_carte_extension As String = ""
            If TipoConsulta = 2 Then
                Dim likeigual As String = " Like"
                For i As Integer = 0 To ClassCamposTableBostraTable.Count - 1
                    If ClassCamposTableBostraTable.Item(i).visible_like_sql = 1 Then
                        If condicionsql = " where " Then
                            condicionsql = condicionsql & ClassCamposTableBostraTable.Item(i).field & likeigual & "'%" & ValorConsulta & "%'"
                        Else
                            condicionsql = condicionsql & " or " & ClassCamposTableBostraTable.Item(i).field & likeigual & "'%" & ValorConsulta & "%'"
                        End If
                    End If
                Next
            End If
            If TipoConsulta = 1 Then
                For i As Integer = 0 To ClassConfigGeneralService.Count - 1
                    Dim campo_plantilla As String = "da." & ClassConfigGeneralService.Item(i).name_campo
                    If ClassConfigGeneralService.Item(i).tipo_campo = "DATE" Or ClassConfigGeneralService.Item(i).tipo_campo = "INT" Then
                        'caso between
                        If ClassConfigGeneralService.Item(i).value_campo <> "" And ClassConfigGeneralService.Item(i).value_campo_beetwen <> "" Then
                            If condicionsql = " where " Then
                                condicionsql = condicionsql & campo_plantilla & " between '" & ClassConfigGeneralService.Item(i).value_campo & "' and '" & ClassConfigGeneralService.Item(i).value_campo_beetwen & "'"
                            Else
                                condicionsql = condicionsql & " and " & campo_plantilla & "  between '" & ClassConfigGeneralService.Item(i).value_campo & "' and '" & ClassConfigGeneralService.Item(i).value_campo_beetwen & "'"
                            End If
                        Else
                            'Caso primer campo
                            If ClassConfigGeneralService.Item(i).value_campo <> "" Then
                                If condicionsql = " where " Then
                                    condicionsql = condicionsql & campo_plantilla & "='" & ClassConfigGeneralService.Item(i).value_campo & "'"
                                Else
                                    condicionsql = condicionsql & " and " & campo_plantilla & "='" & ClassConfigGeneralService.Item(i).value_campo & "'"
                                End If
                            End If
                        End If
                    Else
                        'Caso primer campo
                        If ClassConfigGeneralService.Item(i).value_campo <> "" Then
                            If condicionsql = " where " Then
                                condicionsql = condicionsql & campo_plantilla & "='" & ClassConfigGeneralService.Item(i).value_campo & "'"
                            Else
                                condicionsql = condicionsql & " and " & campo_plantilla & "='" & ClassConfigGeneralService.Item(i).value_campo & "'"
                            End If
                        End If
                    End If
                Next
            End If
            Dim order_colum As String = "ASC"
            Dim colum_order_name As String = "ID"
            Dim sqlfrom As String = " From " & Gabinete & " as da"
            If condicionsql = " where " Then
                Return "Por favor, ingrese un criterio de búsqueda"
            End If
            SqlConsulta = seleccampos & " " & sqlfrom & " " & Sql_carte_extension & " " & condicionsql & " AND DBT <=1 Order by " & colum_order_name & " " & order_colum & " LIMIT 5000"
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitasqlConsultaGabinete " & ex.Message
        End Try
    End Function
    Function SolicitaEstructurainterfaceBusquedaGabinete(ByVal IdGabinete As Integer,
                                                         ByVal ActivaCampoDateAlmacena As Integer,
                                                         ByVal AplicaCampoIdAlmacena As Integer,
                                                         ByRef ClassConfigGeneralService As List(Of Class_config_general_service)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de los campos de busqueda de un gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdGabinete                 : Representa la identificación del gabiente
        'ActivaCampoDateAlmacena    : Activa presentar campo fecha almacenamiento
        'ActivaCampoDateAlmacena    : Representa la opción de mostrar el campo de identiifcación de al
        '                             macenamiento
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'ClassConfigGeneralService  : Retorna la estructura de los campos 
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim Gabinete As String = ""
            Dim class_system1 As New Class_system1
            Result = class_system1.SolicitaNombreGabinetePorId(IdGabinete,
                                                               Gabinete)
            If Result <> "YES" Then
                SolicitaEstructurainterfaceBusquedaGabinete = Result
                Exit Function
            End If
            Dim name_espace_form_control As String = Gabinete & "_search_gabinet_" & IdGabinete
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinetePorId(IdGabinete,
                                                                                   estructura_gabinete)
            If Result <> "YES" Then
                SolicitaEstructurainterfaceBusquedaGabinete = Result
                Exit Function
            End If
            Dim i = estructura_gabinete.Length
            If ActivaCampoDateAlmacena = 1 Then
                i += 1
                ReDim Preserve estructura_gabinete(i)
                estructura_gabinete(i).IDENTI = i
                estructura_gabinete(i).TIPO = "DATE"
                estructura_gabinete(i).CAMPO = "DATE1"
                estructura_gabinete(i).VISIBLE = 1
                estructura_gabinete(i).SISTEMA = 0
                estructura_gabinete(i).ESTADO = 0
                estructura_gabinete(i).INFOCAMPO = "Fecha almacenamiento"
                estructura_gabinete(i).CAMPOPUBLICO = 0
                estructura_gabinete(i).CAMPOUNICO = 0
                estructura_gabinete(i).CAMPO_RADICADO = 0
                estructura_gabinete(i).ALEAS_CAMPO = "FECHA ALMACENA"
                estructura_gabinete(i).CAMPO_ENABLE_DISABLE = 1
            End If
            If AplicaCampoIdAlmacena = 1 Then
                i += 1
                ReDim Preserve estructura_gabinete(i)
                estructura_gabinete(i).IDENTI = i
                estructura_gabinete(i).TIPO = "INT"
                estructura_gabinete(i).CAMPO = "ID"
                estructura_gabinete(i).VISIBLE = 1
                estructura_gabinete(i).SISTEMA = 0
                estructura_gabinete(i).ESTADO = 0
                estructura_gabinete(i).INFOCAMPO = "Identificación documento"
                estructura_gabinete(i).CAMPOPUBLICO = 0
                estructura_gabinete(i).CAMPOUNICO = 0
                estructura_gabinete(i).CAMPO_RADICADO = 0
                estructura_gabinete(i).ALEAS_CAMPO = "ID ALMACENA"
                estructura_gabinete(i).CAMPO_ENABLE_DISABLE = 1
                i = i + 1
            End If
            Result = SolicitaEstructuraInterfaceBusquedaGabinete(estructura_gabinete,
                                                                     name_espace_form_control,
                                                                     Gabinete,
                                                                     ClassConfigGeneralService)
            If Result <> "YES" Then
                SolicitaEstructurainterfaceBusquedaGabinete = Result
                Exit Function
            End If
            SolicitaEstructurainterfaceBusquedaGabinete = "YES"
        Catch ex As Exception
            SolicitaEstructurainterfaceBusquedaGabinete = "Inconsistencia general funcion SolicitaEstructurainterfaceBusquedaGabinete " & ex.Message
        End Try
    End Function
    Function Eliminar_documento_relacionado_enlace_radicado(ByVal gabinete As String,
                                                            ByVal id_imagen As Integer,
                                                            ByVal idex As Integer,
                                                            ByVal option_verfica_propietario As Integer,
                                                            ByVal master_eliminacion As Integer,
                                                            ByVal id_tarea As Long,
                                                            ByVal radicado As String) As String
        '--------------------------------------------------------------------------------
        'Funcion : Elimina documento relacionado a tareas en modulo radicado 
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'gabinete                   : Representa el nombre del gabinete al que pertenece
        '                             la imagen a eliminar
        'id_imagen                  : Representa la imagen a eliminar
        'idex                       : Representa el idex de la imagen
        'option_verfica_propietario : Representa si valida la opción de propietario
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2023-06-27
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim ClassEliminarDocListResult As New ClassEliminarDocListResult
            Result = ClassEliminarDocListResult.EliminarDocumentosGabinete(id_imagen,
                                                                                 idex,
                                                                                 gabinete,
                                                                                 0,
                                                                                 1,
                                                                                 master_eliminacion,
                                                                                 id_tarea,
                                                                                 "ENLACERADICADO")
            If Result <> "YES" Then
                Eliminar_documento_relacionado_enlace_radicado = Result
                Exit Function
            End If
            Dim id_documento_remplazo As Integer = 0
            Dim Ref_class_dagabinete As New ClassDaGabinete
            Dim Estado_relacion As String = ""
            If id_tarea <> 0 Then
                Result = Class_DAT_ADIC_TAR.Verifica_relacion_imagen_workflow(id_imagen,
                                                                              HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                              id_tarea,
                                                                              Estado_relacion)
                If Result <> "YES" Then
                    Eliminar_documento_relacionado_enlace_radicado = Result
                    Exit Function
                End If

                If Estado_relacion = "YES" Then
                    Result = Ref_class_dagabinete.Solicita_id_documento_gabinete(gabinete,
                                                                                 radicado,
                                                                                 id_imagen,
                                                                                 id_documento_remplazo)
                    If Result <> "YES" Then
                        Eliminar_documento_relacionado_enlace_radicado = Result
                        Exit Function
                    End If

                End If
            End If
            Dim Ref_id_documento_remplazo As Object
            If Estado_relacion = "YES" Then
                If id_documento_remplazo = 0 Then
                    Ref_id_documento_remplazo = "Null"
                Else
                    Ref_id_documento_remplazo = id_documento_remplazo
                End If
                Result = Class_DAT_ADIC_TAR.ActualizaIdImagenTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                           id_tarea,
                                                                           Ref_id_documento_remplazo)
                If Result <> "YES" Then
                    Eliminar_documento_relacionado_enlace_radicado = Result
                    Exit Function
                End If

            End If
            Eliminar_documento_relacionado_enlace_radicado = "YES"
            Exit Function
        Catch ex As Exception
            Eliminar_documento_relacionado_enlace_radicado = "Inconsistencia general fuction Eliminar_documento_relacionado_enlace_radicado : " & ex.Message
        End Try
    End Function
    Function Eliminar_documento_relcionado_workflow(ByVal gabinete As String,
                                                    ByVal id_imagen As Integer,
                                                    ByVal idex As Integer,
                                                    ByVal option_verfica_propietario As Integer,
                                                    ByVal master_eliminacion As Integer,
                                                    ByVal id_tarea As Long) As String
        '--------------------------------------------------------------------------------
        'Funcion : Elimina documento relacionado a tareas workflow
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'gabinete                   : Representa el nombre del gabinete al que pertenece
        '                             la imagen a eliminar
        'id_imagen                  : Representa la imagen a eliminar
        'idex                       : Representa el idex de la imagen
        'option_verfica_propietario : 
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2023-06-24
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim RefclasEliminadoc As New ClassEliminarDocListResult
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim id_documento_remplazo As Integer = 0
            Dim Ref_class_dagabinete As New ClassDaGabinete
            Dim Estado_relacion As String = ""
            If id_tarea <> 0 Or id_tarea <> -1 Then
                Result = Class_DAT_ADIC_TAR.Verifica_relacion_imagen_workflow_null(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                  id_tarea,
                                                                                  Estado_relacion)
                If Result <> "YES" Then
                    Eliminar_documento_relcionado_workflow = Result
                    Exit Function
                End If
                '----------------------------------------------------
                'Valida estado relacion documento en la ruta workflow
                '----------------------------------------------------
                If Estado_relacion = "YES" Then
                    Dim radicado_tarea As String = ""
                    Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea,
                                                                                        radicado_tarea)
                    If Result <> "YES" Then
                        Eliminar_documento_relcionado_workflow = Result
                        Exit Function
                    End If
                    Result = Ref_class_dagabinete.Solicita_id_documento_gabinete(gabinete,
                                                                                 radicado_tarea,
                                                                                 id_imagen,
                                                                                 id_documento_remplazo)
                    If Result <> "YES" Then
                        Eliminar_documento_relcionado_workflow = Result
                        Exit Function
                    End If
                    If id_documento_remplazo = 0 Then
                        Eliminar_documento_relcionado_workflow = "Imposible eliminar el documento por ser el único documento relacionado a la tarea. Para eliminar el documento actual debe relacionar otro documento a la tarea"
                        Exit Function
                    End If
                End If
                '--------------------------------------------------
                'Elimina el documento del gabinete
                '--------------------------------------------------
                Result = RefclasEliminadoc.EliminarDocumentosGabinete(id_imagen,
                                                                            idex,
                                                                            gabinete,
                                                                            0,
                                                                            option_verfica_propietario,
                                                                            master_eliminacion,
                                                                            id_tarea,
                                                                            "WORKFLOW")
                If Result <> "YES" Then
                    Eliminar_documento_relcionado_workflow = Result
                    Exit Function
                End If
                '-------------------------------------------------
                'Actualiza con el nuevo documento principaal
                'en workflow
                '------------------------------------------------
                If Estado_relacion = "YES" Then
                    Result = Class_DAT_ADIC_TAR.ActualizaIdImagenTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                                id_tarea,
                                                                                id_documento_remplazo)
                    If Result <> "YES" Then
                        Eliminar_documento_relcionado_workflow = Result
                        Exit Function
                    End If
                End If
                Eliminar_documento_relcionado_workflow = "YES"
                Exit Function
            Else
                Eliminar_documento_relcionado_workflow = "El sistema no detecta tarea seleccionda para eliminar el documento"
                Exit Function
            End If
        Catch ex As Exception
            Eliminar_documento_relcionado_workflow = "Inconsistencia general funcion Eliminar_documento_relcionado_workflow " & ex.Message
        End Try
    End Function
    Function Eliminar_documento_relacionado_consulta_radicado(ByVal gabinete As String,
                                                              ByVal id_imagen As Integer,
                                                              ByVal idex As Integer,
                                                              ByVal option_verfica_propietario As Integer,
                                                              ByVal master_eliminacion As Integer,
                                                              ByVal id_tarea As Long) As String
        '--------------------------------------------------------------------------------
        'Funcion : Elimina documento relacionado en la consulta de radicados
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------
        'gabinete                   : Representa el nombre del gabinete al que pertenece
        '                             la imagen a eliminar
        'id_imagen                  : Representa la imagen a eliminar
        'idex                       : Representa el idex de la imagen
        'option_verfica_propietario : 
        '---------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2023-06-28
        'Modifica              : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim id_documento_remplazo As Integer = 0
            Dim Ref_class_dagabinete As New ClassDaGabinete
            Dim ClassEliminarDocListResult As New ClassEliminarDocListResult
            Dim Estado_relacion As String = ""
            If id_tarea <> 0 Or id_tarea <> -1 Then
                Result = Class_DAT_ADIC_TAR.Verifica_relacion_imagen_workflow_null(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                   id_tarea,
                                                                                   Estado_relacion)
                If Result <> "YES" Then
                    Eliminar_documento_relacionado_consulta_radicado = Result
                    Exit Function
                End If
                If Estado_relacion = "YES" Then
                    Dim radicado_tarea As String = ""
                    Result = Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(id_tarea,
                                                                                        radicado_tarea)
                    If Result <> "YES" Then
                        Eliminar_documento_relacionado_consulta_radicado = Result
                        Exit Function
                    End If
                    Result = Ref_class_dagabinete.Solicita_id_documento_gabinete(gabinete,
                                                                                 radicado_tarea,
                                                                                 id_imagen,
                                                                                 id_documento_remplazo)
                    If Result <> "YES" Then
                        Eliminar_documento_relacionado_consulta_radicado = Result
                        Exit Function
                    End If
                    If id_documento_remplazo = 0 Then
                        Eliminar_documento_relacionado_consulta_radicado = "Imposible eliminar el documento por ser el único documento relacionado a la tarea. Para eliminar el documento actual debe relacionar otro documento a la tarea"
                        Exit Function
                    End If
                End If
            End If

            Result = ClassEliminarDocListResult.EliminarDocumentosGabinete(id_imagen,
                                                                                 idex,
                                                                                 gabinete,
                                                                                 0,
                                                                                 option_verfica_propietario,
                                                                                 master_eliminacion,
                                                                                 id_tarea, "CONSULTARADICADO")

            If Result <> "YES" Then
                Eliminar_documento_relacionado_consulta_radicado = Result
                Exit Function
            End If
            Dim Ref_id_documento_remplazo As Object
            If Estado_relacion = "YES" Then
                If id_documento_remplazo = 0 Then
                    Ref_id_documento_remplazo = "Null"
                Else
                    Ref_id_documento_remplazo = id_documento_remplazo
                End If
                Result = Class_DAT_ADIC_TAR.ActualizaIdImagenTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                            id_tarea,
                                                                            Ref_id_documento_remplazo)
                If Result <> "YES" Then
                    Eliminar_documento_relacionado_consulta_radicado = Result
                    Exit Function
                End If
            End If
            Eliminar_documento_relacionado_consulta_radicado = "YES"
            Exit Function
        Catch ex As Exception
            Eliminar_documento_relacionado_consulta_radicado = "Inconsistencia general funcion Eliminar_documento_relacionado_consulta_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_usuario_propietario_imagen_gabinete(ByVal nombre_gabinete As String, ByVal id_imagen As Integer,
                                                          ByRef user_propietario As String) As String
        Try
            Dim Parametro_Consulta = "select USER " &
            " from " & nombre_gabinete & " where ID=" & id_imagen
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_usuario_propietario_imagen_gabinete = "Funcion  Solicita_verificar_imagen_relacionada_a_documento_compartido dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_usuario_propietario_imagen_gabinete = "Imposible encontrar el propietario del documento (" & id_imagen & ") en el gabiente (" & nombre_gabinete & ")"
                Exit Function
            Else
                user_propietario = Datset.Tables(0).Rows(0).Item(0)
                Solicita_usuario_propietario_imagen_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_usuario_propietario_imagen_gabinete = "Inconsistencia general función Solicita_usuario_propietario_imagen_gabinete " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraValoresCamposIndice(ByVal IdDocumento As Integer,
                                                   ByVal NombreGabinete As String,
                                                   ByVal EnabledRadicadoEnlace As Integer,
                                                   ByVal NameEspaceControl As String,
                                                   ByRef EstructuraGabinete() As estructura_gabinete,
                                                   ByRef ClassConfigGeneralService As List(Of Class_config_general_service)) As String
        '-------------------------------------------------------------------------------------
        'Funcion : Asigna datos del indice del documento de un gabinete
        '          
        '-------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------------
        'id_documento          : Representa la Identificación de la imagen en el gabinente
        'nombre_gabinete       : Representa el nombre del gabinete
        '-------------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------------
        'estructura_gabinete   : Retorna la estructura del gabinete con los valores asignados
        'Class_config_general_service : Retorna la para campos dinamicos para la interface
        '-------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------------
        'Fecha                 : 2023-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            EstructuraGabinete = Nothing
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(NombreGabinete,
                                                                              EstructuraGabinete)
            If Result <> "YES" Then
                Return Result
            End If
            Result = AsignaDatosIndiceDocumento(IdDocumento,
                                                NombreGabinete,
                                                EstructuraGabinete)
            If Result <> "YES" Then
                Return Result
            End If
            Result = AsignaDatosEstructuraGeneralIndice(EstructuraGabinete,
                                                        NameEspaceControl,
                                                        NombreGabinete,
                                                        IdDocumento,
                                                        EnabledRadicadoEnlace,
                                                        ClassConfigGeneralService)
            Return Result
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitaEstructuraValoresCamposIndice " & ex.Message
        End Try
    End Function

    Function AsignaDatosIndiceDocumento(ByVal IdDocumento As Integer,
                                        ByVal NombreGabinete As String,
                                        ByRef EstructuraGabinete() As estructura_gabinete) As String
        '-------------------------------------------------------------------------------------
        'Funcion : Asigna datos de una estructura de un gabinete con la identificacion
        '          de la imagen expecifica
        '-------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------------
        'id_documento          : Representa la Identificación de la imagen en el gabinente
        'nombre_gabinete       : Representa el nombre del gabinete
        '-------------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------------
        'estructura_gabinete   : Retorna la estructura del gabinete con los valores asignados
        '-------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------------
        'Fecha                 : 2023-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------------
        Try
            Dim SqlConsulta As String = ""
            Dim SqlCampos As String = ""
            Dim Result As String = ""
            Dim ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To EstructuraGabinete.Length - 1
                If SqlCampos = "" Then
                    SqlCampos = "Select " & EstructuraGabinete(i).CAMPO
                Else
                    SqlCampos = SqlCampos & "," & EstructuraGabinete(i).CAMPO
                End If
            Next
            SqlConsulta = SqlCampos & " from " & NombreGabinete & " where ID=" & IdDocumento
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(NombreGabinete)
            Result = ref.SELECTION_SELECT_FIELDA(SqlConsulta, Datset)
            If Result <> "YES" Then
                Return "Funcion AsignaDatosIndiceDocumento dice : (" & Result & ")"
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "Imposible encontrar datos del documento : (" & IdDocumento & ") del gabinete (" & NombreGabinete & ")"
            Else
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    If Datset.Tables(0).Rows(0).IsNull(i) = True Then
                        If EstructuraGabinete(i).TIPO = "INT" Or EstructuraGabinete(i).TIPO = "LONG" Then
                            EstructuraGabinete(i).VALORCAMPO = ""
                        Else
                            EstructuraGabinete(i).VALORCAMPO = ""
                        End If
                    Else
                        Select Case EstructuraGabinete(i).TIPO
                            Case "DATE"
                                ClassGestionFechas.Formatea_fecha_time_base_mysql(Datset.Tables(0).Rows(0).Item(i).ToString,
                                                                                       EstructuraGabinete(i).VALORCAMPO)
                                EstructuraGabinete(i).VALORCAMPO = Left(EstructuraGabinete(i).VALORCAMPO, "10")

                            Case Else
                                EstructuraGabinete(i).VALORCAMPO = Datset.Tables(0).Rows(0).Item(i)

                        End Select
                    End If
                Next
                Return "YES"
            End If
        Catch ex As Exception
            AsignaDatosIndiceDocumento = "Inconsistencia general funcion AsignaDatosIndiceDocumento " & ex.Message
        End Try
    End Function
    Function SolicitaEstructuraInterfaceBusquedaGabinete(ByVal EstructuraGabinete() As estructura_gabinete,
                                                         ByVal NameEspaceFormControl As String,
                                                         ByVal NombreGabinete As String,
                                                         ByRef ClassConfigGeneralService As List(Of Class_config_general_service)) As String
        '-------------------------------------------------------------------------------------
        'Funcion : Asigna los datos de la estructura del gabinete a la estructura general
        '          de formulario
        '-------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------------
        'EstructuraGabinete   : Representa la estructura del gabinete con los datos
        '
        '-------------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------------
        'ClassConfigGeneralService   : Retorna la estructura general del gabinete
        '-------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------------
        'Fecha                 : 2024-05-21
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------------
        Try
            For i As Integer = 0 To EstructuraGabinete.Length - 1
                If EstructuraGabinete(i).VISIBLE = 1 And EstructuraGabinete(i).SISTEMA = 0 And EstructuraGabinete(i).ESTADO = 0 Then
                    Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
                    If EstructuraGabinete(i).ALEAS_CAMPO <> "0" And EstructuraGabinete(i).ALEAS_CAMPO <> "" Then
                        parameter_gestion.aleas_campo = EstructuraGabinete(i).ALEAS_CAMPO
                    Else
                        parameter_gestion.aleas_campo = EstructuraGabinete(i).CAMPO
                    End If
                    parameter_gestion.name_campo = EstructuraGabinete(i).CAMPO
                    parameter_gestion.alow_null = 0
                    parameter_gestion.alow_tipo_value = 1
                    parameter_gestion.campo_tip = 1
                    parameter_gestion.value_campo = ""
                    parameter_gestion.disable_campo = 1
                    parameter_gestion.control_tip_correo = 0
                    If EstructuraGabinete(i).CAMPO_RADICADO = 1 Then
                        parameter_gestion.disable_campo = 1
                    End If
                    If EstructuraGabinete(i).CAMPO = "ENLASE" Then
                        parameter_gestion.aleas_campo = "ENLACE"
                        parameter_gestion.disable_campo = 1
                    End If
                    Dim tipo_campo As String = ""
                    Dim leng_campo As Integer = 0
                    If InStr(EstructuraGabinete(i).TIPO, "VARCHAR") > 0 Then
                        Dim temp_campo As String = EstructuraGabinete(i).TIPO.Replace("VARCHAR", "")
                        temp_campo = temp_campo.Replace("(", "")
                        temp_campo = temp_campo.Replace(")", "")
                        leng_campo = Val(temp_campo)
                        tipo_campo = "VARCHAR"
                    Else
                        Select Case EstructuraGabinete(i).TIPO
                            Case "INT"
                                leng_campo = 9
                                tipo_campo = EstructuraGabinete(i).TIPO
                            Case "DATE"
                                leng_campo = 10
                                tipo_campo = EstructuraGabinete(i).TIPO
                            Case Else
                                leng_campo = 100
                                tipo_campo = EstructuraGabinete(i).TIPO
                        End Select
                    End If
                    parameter_gestion.tipo_campo = tipo_campo
                    parameter_gestion.max_leng_campo = leng_campo
                    parameter_gestion.name_space_campo = NameEspaceFormControl
                    parameter_gestion.dbms_control = "DA"
                    parameter_gestion.dms_id_registro = -1
                    parameter_gestion.tbl_control = NombreGabinete
                    parameter_gestion.clas_service_control = "WebService_control_general.asmx"
                    parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
                    parameter_gestion.error_gestion = "YES"
                    ClassConfigGeneralService.Add(parameter_gestion)
                End If
            Next
            SolicitaEstructuraInterfaceBusquedaGabinete = "YES"
        Catch ex As Exception
            SolicitaEstructuraInterfaceBusquedaGabinete = "Inconsistencia general funcion Solicita_estructura_interface_busqueda_gabinete " & ex.Message
        End Try
    End Function
    Function AsignaDatosEstructuraGeneralIndice(ByVal EstructuraGabinete() As estructura_gabinete,
                                                ByVal NameEspaceFormControl As String,
                                                ByVal NombreGabinete As String,
                                                ByVal IdDocumento As Integer,
                                                ByVal EnabledRadicadoEnlace As Integer,
                                                ByRef ClassConfigGeneralService As List(Of Class_config_general_service)) As String
        '-------------------------------------------------------------------------------------
        'Funcion : Asigna los datos de la estructura del gabinete a la estructura general
        '          de formularios y asigna los datos 
        '-------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-------------------------------------------------------------------------------------
        'estructura_gabinete   : Representa la estructura del gabinete con los datos
        '
        '-------------------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------------------
        'Class_config_general_service   : Retorna la estructura general del gabinete para campos
        '                               : dinamicos
        '---------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------------
        'Fecha                 : 2024-06-15
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------------------
        Try
            For i As Integer = 0 To EstructuraGabinete.Length - 1
                If EstructuraGabinete(i).VISIBLE = 1 And EstructuraGabinete(i).SISTEMA = 0 And EstructuraGabinete(i).ESTADO = 0 Then
                    Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
                    If EstructuraGabinete(i).ALEAS_CAMPO <> "0" And EstructuraGabinete(i).ALEAS_CAMPO <> "" Then
                        parameter_gestion.aleas_campo = EstructuraGabinete(i).ALEAS_CAMPO
                    Else
                        parameter_gestion.aleas_campo = EstructuraGabinete(i).CAMPO
                    End If
                    parameter_gestion.name_campo = EstructuraGabinete(i).CAMPO
                    parameter_gestion.alow_null = 0
                    parameter_gestion.alow_tipo_value = 1
                    parameter_gestion.campo_tip = 1
                    parameter_gestion.value_campo = EstructuraGabinete(i).VALORCAMPO
                    parameter_gestion.disable_campo = EstructuraGabinete(i).CAMPO_ENABLE_DISABLE
                    parameter_gestion.control_tip_correo = 0
                    If EstructuraGabinete(i).CAMPO_RADICADO = 1 Then
                        parameter_gestion.disable_campo = 0
                    End If
                    If EstructuraGabinete(i).CAMPO = "ENLASE" And EnabledRadicadoEnlace = 1 Then
                        parameter_gestion.aleas_campo = "ENLACE"
                        parameter_gestion.disable_campo = 0
                    End If
                    Dim tipo_campo As String = ""
                    Dim leng_campo As Integer = 0
                    If InStr(EstructuraGabinete(i).TIPO, "VARCHAR") > 0 Then
                        Dim temp_campo As String = EstructuraGabinete(i).TIPO.Replace("VARCHAR", "")
                        temp_campo = temp_campo.Replace("(", "")
                        temp_campo = temp_campo.Replace(")", "")
                        leng_campo = Val(temp_campo)
                        tipo_campo = "VARCHAR"
                    Else
                        Select Case EstructuraGabinete(i).TIPO
                            Case "INT"
                                leng_campo = 9
                                tipo_campo = EstructuraGabinete(i).TIPO
                            Case "DATE"
                                leng_campo = 10
                                tipo_campo = EstructuraGabinete(i).TIPO
                            Case Else
                                leng_campo = 100
                                tipo_campo = EstructuraGabinete(i).TIPO
                        End Select
                    End If
                    parameter_gestion.tipo_campo = tipo_campo
                    parameter_gestion.max_leng_campo = leng_campo
                    parameter_gestion.name_space_campo = NameEspaceFormControl
                    parameter_gestion.dbms_control = "DA"
                    parameter_gestion.dms_id_registro = IdDocumento
                    parameter_gestion.tbl_control = NombreGabinete
                    parameter_gestion.clas_service_control = "WebService_control_general.asmx"
                    parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_campos_form_control"
                    parameter_gestion.error_gestion = "YES"
                    ClassConfigGeneralService.Add(parameter_gestion)
                End If
            Next
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general funcion AsignaDatosEstructuraGeneralIndice " & ex.Message
        End Try
    End Function
    Function Solicita_structura_imagen_gabinete_indice_expediente(ByVal Nombre_Tabla As String,
                                                                  ByVal Id_Imagen As Long,
                                                                  ByRef stru_paramter_image As stru_paramter_image,
                                                                  Optional ByVal option_trd As Integer = 0) As String
        Try
            Dim Sql_consulta = ""
            Dim order As String = ""
            Dim tipo_imagen As String = " "
            Dim id_tipo_imagen As String = ""
            If option_trd <> 0 Then
                tipo_imagen = ",TIPODOCUMENTO "
                id_tipo_imagen = ",ID_TIPODOCUMENTO"
                order = " order by ID_TIPODOCUMENTO"
            End If
            Sql_consulta = "SELECT ID,DISC,PAG,IDEX,DBT" & tipo_imagen & id_tipo_imagen &
            " FROM " & Nombre_Tabla &
            " WHERE ID=" & Id_Imagen & order
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_structura_imagen_gabinete_indice_expediente = "Funcion Solicita_structura_imagen_gabinete_indice_expediente dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_structura_imagen_gabinete_indice_expediente = "Imposible encontrar la estructura de la imagen (" & Id_Imagen & ") del gabinete  (" & Nombre_Tabla & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru_paramter_image.ID = 0
                Else
                    stru_paramter_image.ID = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru_paramter_image.DISC = 0
                Else
                    stru_paramter_image.DISC = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru_paramter_image.PAG = 0
                Else
                    stru_paramter_image.PAG = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru_paramter_image.IDEX = 0
                Else
                    stru_paramter_image.IDEX = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru_paramter_image.DBT_TIPO_IMAGEN = 0
                Else
                    stru_paramter_image.DBT_TIPO_IMAGEN = Datset.Tables(0).Rows(0).Item(4)
                End If
                If option_trd <> 0 Then
                    If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                        stru_paramter_image.TIPODOCUMENTO = ""
                    Else
                        stru_paramter_image.TIPODOCUMENTO = Trim(Datset.Tables(0).Rows(0).Item(5))
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                        stru_paramter_image.ID_TIPODOCUMENTO = 0
                    Else
                        stru_paramter_image.ID_TIPODOCUMENTO = Datset.Tables(0).Rows(0).Item(6)
                    End If
                End If
                Solicita_structura_imagen_gabinete_indice_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_structura_imagen_gabinete_indice_expediente = "Inconsistencia general funcion Solicita_structura_imagen_gabinete_indice_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_structura_imagen_gabinete_SII(ByVal Nombre_gabinete As String,
                                                    ByVal Recibo As String,
                                                    ByVal codigo_barras As String,
                                                    ByRef id_imagen As Integer,
                                                    ByRef auxiliar As String,
                                                    ByRef acto As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita estructura de campos SII de una gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Nombre_gabinete     : Representa el nombre del gabinete
        'Recibo              : Representa el recibo del DII
        'codigo_barras       : Rpresenta codigo barraas SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_imagen  : Retorna la idnetificación de la imagen
        'auxiliar   : Retorna el campo auxiliar del sistema SII
        'acto       : Retorna el campo acto del sistema SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ID,AUXILIAR,ACTOWF" &
            " FROM " & Nombre_gabinete &
             " Where RECIBOCAJA='" &
            Recibo & "'" & " And  CODBARRAS='" & codigo_barras & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_structura_imagen_gabinete_SII = "Funcion Solicita_structura_imagen_gabinete_SII dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_structura_imagen_gabinete_SII = "Imposible encontrar la estructura de la imagen de recibo (" & Recibo & ") y del codigo de barras (" & codigo_barras & ") del gabinete  (" & Nombre_gabinete & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_imagen = 0
                Else
                    id_imagen = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    auxiliar = ""
                Else
                    auxiliar = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    acto = ""
                Else
                    acto = Datset.Tables(0).Rows(0).Item(2)
                End If
                Solicita_structura_imagen_gabinete_SII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_structura_imagen_gabinete_SII = "Inconsistencia general funcion Solicita_structura_imagen_gabinete_SII " & ex.Message
        End Try
    End Function
    Function Solicita_structura_imagen_gabinete_producion(ByVal Nombre_Tabla As String,
                                                          ByVal Id_Imagen As Long,
                                                          ByRef stru_paramter_image As stru_paramter_image,
                                                          Optional ByVal option_trd As Integer = 0) As String
        Try
            Dim Sql_consulta = ""
            Dim order As String = ""
            Dim tipo_imagen As String = " "
            Dim id_tipo_imagen As String = ""
            If option_trd <> 0 Then
                tipo_imagen = ",TIPODOCUMENTO "
                id_tipo_imagen = ",ID_TIPODOCUMENTO"
                order = " order by ID_TIPODOCUMENTO"
            End If
            Sql_consulta = "SELECT ID,DISC,PAG,IDEX,ENLASE,DBT" & tipo_imagen & id_tipo_imagen & ",rp.ID_REGISTRO_PRODUCION_DOCUMENTAL " &
            " FROM " & Nombre_Tabla &
            " inner join registro_producion_documental as rp on (rp.ID_DOCUMENTO_DOCUARCHI_ALMACEN=ID and rp.NOMBRE_GABINETE='" & Nombre_Tabla & "')" &
            " WHERE ID=" & Id_Imagen & order
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_structura_imagen_gabinete_producion = "Funcion Solicita_structura_imagen_gabinete_producion dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_structura_imagen_gabinete_producion = "Imposible encontrar la estructura de la imagen (" & Id_Imagen & ") del gabinete  (" & Nombre_Tabla & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru_paramter_image.ID = 0
                Else
                    stru_paramter_image.ID = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru_paramter_image.DISC = 0
                Else
                    stru_paramter_image.DISC = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru_paramter_image.PAG = 0
                Else
                    stru_paramter_image.PAG = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru_paramter_image.IDEX = 0
                Else
                    stru_paramter_image.IDEX = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru_paramter_image.ENLACE = ""
                Else
                    stru_paramter_image.ENLACE = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru_paramter_image.DBT_TIPO_IMAGEN = 0
                Else
                    stru_paramter_image.DBT_TIPO_IMAGEN = Datset.Tables(0).Rows(0).Item(5)
                End If
                If option_trd <> 0 Then
                    If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                        stru_paramter_image.TIPODOCUMENTO = ""
                    Else
                        stru_paramter_image.TIPODOCUMENTO = Trim(Datset.Tables(0).Rows(0).Item(6))
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                        stru_paramter_image.ID_TIPODOCUMENTO = 0
                    Else
                        stru_paramter_image.ID_TIPODOCUMENTO = Datset.Tables(0).Rows(0).Item(7)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                        stru_paramter_image.ID_PRODUCCION = 0
                    Else
                        stru_paramter_image.ID_PRODUCCION = Datset.Tables(0).Rows(0).Item(8)
                    End If
                Else
                    If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                        stru_paramter_image.ID_PRODUCCION = 0
                    Else
                        stru_paramter_image.ID_PRODUCCION = Datset.Tables(0).Rows(0).Item(6)
                    End If
                End If
                Solicita_structura_imagen_gabinete_producion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_structura_imagen_gabinete_producion = "Inconsistencia general funcion Solicita_structura_imagen_gabinete_producion " & ex.Message
            Exit Function
        End Try
    End Function
    Function SolicitaListaImagensGabineteRelacionTareaWorkflow(ByVal IdTareaWorkflow As Long,
                                                               ByRef StruImagenGabineteWorkflow As stru_imagen_gabinete_workflow()) As String
        Try
            Dim Result As String = ""
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                       IdTareaWorkflow,
                                                                                       structure_datos_tarea_workflow)
            If Result <> "YES" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = Result
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_DAT = 0 Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = "#255  Imposible encontrar id de la tarea en la tabla dat_adic_tar  (" & IdTareaWorkflow & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = "#266 tarea sin id gabinete asignado (" & IdTareaWorkflow & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = "#277  La imagen de esta tarea fue cambiada o eliminada  tarea sin imagen adjunta  (" & IdTareaWorkflow & ")"
                Exit Function
            End If
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                              structure_gabinete_workflow)
            If Result <> "YES" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                     structure_datos_tarea_workflow.ID_IMAGEN,
                                                                     stru_paramter_image,
                                                                     0)
            If Result <> "YES" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = Result
                Exit Function
            End If
            ReDim Preserve StruImagenGabineteWorkflow(0)
            StruImagenGabineteWorkflow(0).id_image = structure_datos_tarea_workflow.ID_IMAGEN
            StruImagenGabineteWorkflow(0).gabinete = structure_gabinete_workflow.NOMBRE_GABINETE
            Result = ClassDaGabinete.SolicitaListaImagenesGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                   stru_paramter_image.ENLACE,
                                                                   StruImagenGabineteWorkflow,
                                                                   structure_datos_tarea_workflow.ID_IMAGEN)
            If Result <> "YES" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = Result
                Exit Function
            End If
            If stru_paramter_image.ENLACE = "" Then
                SolicitaListaImagensGabineteRelacionTareaWorkflow = "La imagen principal (" & structure_datos_tarea_workflow.ID_IMAGEN & ") del gabinete (" &
                    structure_gabinete_workflow.NOMBRE_GABINETE & ") no relaciona datos campo ENLACE, tarea workflow (" & IdTareaWorkflow & ") por favor contace a su administrador de sistema"
                Exit Function
            End If
            SolicitaListaImagensGabineteRelacionTareaWorkflow = "YES"
        Catch ex As Exception
            SolicitaListaImagensGabineteRelacionTareaWorkflow = "Inconsistencia general funcion Solicita_lista_id_imagen_gabinete_relacion_tarea_workflow " & ex.Message
        End Try
    End Function
    Function SolicitaEtructuraImagenGabinete(ByVal NombreTabla As String,
                                             ByVal IdImagen As Long,
                                             ByRef stru_paramter_image As stru_paramter_image,
                                             Optional ByVal option_trd As Integer = 0,
                                             Optional ByVal valida_exist_imagen As Integer = 0,
                                             Optional ByVal option_reg_migracion As Integer = 0,
                                             Optional ByVal option_firma_digital As Integer = 0,
                                             Optional ByVal option_produccion_documental As Integer = 0) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de la imagen de un gabinete
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Nombre_Tabla        : Representa el nombre del gabinete y de la tabla
        'Id_Imagen           : Representa la identificación de la imagen
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_paramter_image  : Retorna la estructura de la imagen
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Dim order As String = ""
            Dim tipo_imagen As String = " "
            Dim id_tipo_imagen As String = ""
            Dim header_reg_mig As String = ""
            Dim header_firma_digital As String = ""
            Dim header_produc_documental As String = ""
            If option_trd <> 0 Then
                tipo_imagen = ",TIPODOCUMENTO "
                id_tipo_imagen = ",ID_TIPODOCUMENTO"
                order = " order by ID_TIPODOCUMENTO"
            End If
            If option_reg_migracion <> 0 Then
                header_reg_mig = ",DA_MIG,CTRL_ACES,ID_PROP,ID_REGISTRO_VERSION,ID_VERSION_DOC "
            End If
            If option_firma_digital <> 0 Then
                header_firma_digital = ",ESTADO_FIRMA_DIGITAL "
            End If
            If option_produccion_documental <> 0 Then
                header_produc_documental = ",ID_INVENTARIO_DOCUMENTAL "
            End If
            Sql_consulta = "SELECT ID,DISC,PAG,IDEX,ENLASE,DBT,USER,DATE1,TIME1" & tipo_imagen & id_tipo_imagen & header_reg_mig & header_firma_digital & header_produc_documental &
            " FROM " & NombreTabla &
            " WHERE ID=" & IdImagen & order
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaEtructuraImagenGabinete = "Funcion Solicita_structura_imagen_gabinete dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If valida_exist_imagen = 0 Then
                    SolicitaEtructuraImagenGabinete = "Imposible encontrar la estructura de la imagen (" & IdImagen & ") del gabinete  (" & NombreTabla & ")"
                    Exit Function
                Else
                    stru_paramter_image.ID = 0
                    stru_paramter_image.DISC = 0
                    stru_paramter_image.PAG = 0
                    stru_paramter_image.IDEX = 0
                    stru_paramter_image.ENLACE = ""
                    stru_paramter_image.DBT_TIPO_IMAGEN = 0
                    stru_paramter_image.TIPODOCUMENTO = ""
                    stru_paramter_image.ID_TIPODOCUMENTO = 0
                    stru_paramter_image.CTRL_ACES = 0
                    stru_paramter_image.ID_PROP = 0
                    stru_paramter_image.ID_REGISTRO_VERSION = 0
                    stru_paramter_image.ID_VERSION_DOC = 0
                    stru_paramter_image.DA_MIG = 0
                    stru_paramter_image.ESTADO_FIRMA_DIGITAL = 0
                    stru_paramter_image.ID_PRODUCCION = 0
                    SolicitaEtructuraImagenGabinete = "YES"
                    Exit Function
                End If
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru_paramter_image.ID = 0
                Else
                    stru_paramter_image.ID = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru_paramter_image.DISC = 0
                Else
                    stru_paramter_image.DISC = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru_paramter_image.PAG = 0
                Else
                    stru_paramter_image.PAG = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru_paramter_image.IDEX = 0
                Else
                    stru_paramter_image.IDEX = Datset.Tables(0).Rows(0).Item(3)
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru_paramter_image.ENLACE = ""
                Else
                    stru_paramter_image.ENLACE = Datset.Tables(0).Rows(0).Item(4)
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru_paramter_image.DBT_TIPO_IMAGEN = 0
                Else
                    stru_paramter_image.DBT_TIPO_IMAGEN = Datset.Tables(0).Rows(0).Item(5)
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    stru_paramter_image.USER = ""
                Else
                    stru_paramter_image.USER = Datset.Tables(0).Rows(0).Item(6)
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    stru_paramter_image.DATE1 = ""
                Else
                    stru_paramter_image.DATE1 = Datset.Tables(0).Rows(0).Item(7)
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    stru_paramter_image.TIME1 = ""
                Else
                    stru_paramter_image.TIME1 = Datset.Tables(0).Rows(0).Item(8)
                End If
                If option_trd <> 0 Then
                    If Datset.Tables(0).Rows(0).IsNull("TIPODOCUMENTO") = True Then
                        stru_paramter_image.TIPODOCUMENTO = ""
                    Else
                        stru_paramter_image.TIPODOCUMENTO = Trim(Datset.Tables(0).Rows(0).Item("TIPODOCUMENTO"))
                    End If
                    If Datset.Tables(0).Rows(0).IsNull("ID_TIPODOCUMENTO") = True Then
                        stru_paramter_image.ID_TIPODOCUMENTO = 0
                    Else
                        stru_paramter_image.ID_TIPODOCUMENTO = Datset.Tables(0).Rows(0).Item("ID_TIPODOCUMENTO")
                    End If
                End If
                If option_reg_migracion <> 0 Then
                    If Datset.Tables(0).Rows(0).IsNull("DA_MIG") = True Then
                        stru_paramter_image.DA_MIG = 0
                    Else
                        stru_paramter_image.DA_MIG = Datset.Tables(0).Rows(0).Item("DA_MIG")
                    End If
                    stru_paramter_image.CTRL_ACES = Datset.Tables(0).Rows(0).Item("CTRL_ACES")
                    stru_paramter_image.ID_PROP = Datset.Tables(0).Rows(0).Item("ID_PROP")
                    stru_paramter_image.ID_REGISTRO_VERSION = Datset.Tables(0).Rows(0).Item("ID_REGISTRO_VERSION")
                    stru_paramter_image.ID_VERSION_DOC = Datset.Tables(0).Rows(0).Item("ID_VERSION_DOC")
                End If
                If option_firma_digital <> 0 Then
                    stru_paramter_image.ESTADO_FIRMA_DIGITAL = Datset.Tables(0).Rows(0).Item("ESTADO_FIRMA_DIGITAL")
                End If
                If option_produccion_documental <> 0 Then
                    stru_paramter_image.ID_PRODUCCION = Datset.Tables(0).Rows(0).Item("ID_INVENTARIO_DOCUMENTAL")
                End If
                SolicitaEtructuraImagenGabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEtructuraImagenGabinete = "Inconsistencia general funcion SolicitaEtructuraImagenGabinete " & ex.Message
        End Try
    End Function
    Function Solicita_imagenes_enlazadas_gabinete_produccion(ByVal Nombre_Tabla As String,
                                                             ByVal enlace As String,
                                                             ByRef stru_paramter_image() As stru_paramter_image,
                                                             ByVal Id_Imagen As Long,
                                                             ByVal id_tarea_wf As Long,
                                                             Optional ByVal option_trd As Integer = 0) As String
        Try

            Dim iConta As Integer = stru_paramter_image.Length
            Dim Sql_consulta = ""
            Dim tipo_imagen As String = " "
            If option_trd <> 0 Then
                tipo_imagen = ",TIPODOCUMENTO,ID_TIPODOCUMENTO"
            End If
            Dim Result As String = ""
            Sql_consulta = "SELECT ID,DISC,PAG,IDEX,ENLASE,DBT" & tipo_imagen & ",rp.ID_REGISTRO_PRODUCION_DOCUMENTAL" &
            " FROM " & Nombre_Tabla &
            " inner join registro_producion_documental as rp on (rp.ID_DOCUMENTO_DOCUARCHI_ALMACEN=ID and rp.NOMBRE_GABINETE='" & Nombre_Tabla & "') " &
            " WHERE ENLASE='" & enlace & "' AND ID <> " & Id_Imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_imagenes_enlazadas_gabinete_produccion = "Funcion Solicita_imagenes_enlazadas_gabinete_produccion dice : (" & Result & ")"
                Exit Function
            End If
            'iConta = iConta + 1
            For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                ReDim Preserve stru_paramter_image(iConta)
                If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                    stru_paramter_image(iConta).ID = 0
                Else
                    stru_paramter_image(iConta).ID = Datset.Tables(0).Rows(i).Item(0)
                End If
                If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                    stru_paramter_image(iConta).DISC = 0
                Else
                    stru_paramter_image(iConta).DISC = Datset.Tables(0).Rows(i).Item(1)
                End If
                If Datset.Tables(0).Rows(i).IsNull(2) = True Then
                    stru_paramter_image(iConta).PAG = 0
                Else
                    stru_paramter_image(iConta).PAG = Datset.Tables(0).Rows(i).Item(2)
                End If
                If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                    stru_paramter_image(iConta).IDEX = 0
                Else
                    stru_paramter_image(iConta).IDEX = Datset.Tables(0).Rows(i).Item(3)
                End If
                If Datset.Tables(0).Rows(i).IsNull(4) = True Then
                    stru_paramter_image(iConta).ENLACE = ""
                Else
                    stru_paramter_image(iConta).ENLACE = Datset.Tables(0).Rows(i).Item(4)
                End If
                If Datset.Tables(0).Rows(i).IsNull(5) = True Then
                    stru_paramter_image(iConta).DBT_TIPO_IMAGEN = 0
                Else
                    stru_paramter_image(iConta).DBT_TIPO_IMAGEN = Datset.Tables(0).Rows(i).Item(5)
                End If
                If option_trd <> 0 Then
                    If Datset.Tables(0).Rows(i).IsNull(6) = True Then
                        stru_paramter_image(iConta).TIPODOCUMENTO = ""
                    Else
                        stru_paramter_image(iConta).TIPODOCUMENTO = Trim(Datset.Tables(0).Rows(i).Item(6))
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(7) = True Then
                        stru_paramter_image(iConta).ID_TIPODOCUMENTO = 0
                    Else
                        stru_paramter_image(iConta).ID_TIPODOCUMENTO = Datset.Tables(0).Rows(i).Item(7)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(8) = True Then
                        stru_paramter_image(iConta).ID_PRODUCCION = 0
                    Else
                        stru_paramter_image(iConta).ID_PRODUCCION = Datset.Tables(0).Rows(i).Item(8)
                    End If
                Else
                    If Datset.Tables(0).Rows(i).IsNull(6) = True Then
                        stru_paramter_image(iConta).ID_PRODUCCION = 0
                    Else
                        stru_paramter_image(iConta).ID_PRODUCCION = Datset.Tables(0).Rows(i).Item(6)
                    End If
                End If
                iConta = iConta + 1
            Next
            Solicita_imagenes_enlazadas_gabinete_produccion = "YES"
        Catch ex As Exception
            Solicita_imagenes_enlazadas_gabinete_produccion = "Inconsistencia general funcion Solicita_imagenes_enlazadas_gabinete_produccion " & ex.Message
        End Try
    End Function
    Function Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion(ByVal Nombre_Tabla As String,
                                                                                              ByVal enlace As String,
                                                                                              ByRef imagenes_sin_expediente As Integer,
                                                                                              ByRef imagenes_sin_firma As Integer) As String
        Try
            Dim Result As String
            Dim Sql_consulta = "SELECT rp.ESTADO_FIRMA_DIGITAL,rp.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE" &
            " FROM " & Nombre_Tabla &
            " inner join registro_producion_documental as rp on (rp.ID_DOCUMENTO_DOCUARCHI_ALMACEN=ID and rp.NOMBRE_GABINETE='" & Nombre_Tabla & "') " &
            " WHERE ENLASE='" & enlace & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(Nombre_Tabla)
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion = "Funcion Solicita_imagenes_enlazadas_gabinete_produccion dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion = "Imposible contrar imagenes enlazadas para evaluar el estado de expediente y firmado digital"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).Item(0) = 0 Then
                        imagenes_sin_firma += 1
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        imagenes_sin_expediente += 1
                    End If
                Next
                Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion = "Inconsistencia general funcion Solicita_estados_expediente_firma_digital_imagenes_enlazadas_gabinete_produccion " & ex.Message
        End Try
    End Function
    Function SolicitaListaImagenesGabineteEnlace(ByVal NombreTabla As String,
                                                 ByVal EnlaceWorkflow As String,
                                                 ByRef stru_imagen_gabinete_workflow() As stru_imagen_gabinete_workflow) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita estructura imagenes relacionadas a un enlace
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreTabla         : Representa el nombre de la tabla workflow
        'EnlaceWorkflow      : Representa el nombre del enlace workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'stru_imagen_gabinete_workflow  : Retorna la estructura de imagenes relacionadas
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-23
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String
            Dim Sql_consulta = "SELECT ID,ID_TIPODOCUMENTO" &
            " FROM " & NombreTabla &
            " WHERE ENLASE='" & EnlaceWorkflow & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaListaImagenesGabineteEnlace = "Funcion SolicitaListaImagenesGabineteEnlace dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaListaImagenesGabineteEnlace = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_imagen_gabinete_workflow(i)
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                        stru_imagen_gabinete_workflow(i).id_image = 0
                    Else
                        stru_imagen_gabinete_workflow(i).id_image = Datset.Tables(0).Rows(i).Item(0)
                    End If
                    stru_imagen_gabinete_workflow(i).ID_TIPODOCUMENTO = Datset.Tables(0).Rows(i).Item(1)
                    stru_imagen_gabinete_workflow(i).gabinete = NombreTabla
                Next
                SolicitaListaImagenesGabineteEnlace = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaListaImagenesGabineteEnlace = "Inconsistencia general funcion SolicitaListaImagenesGabineteEnlace " & ex.Message
        End Try
    End Function
    Function SolicitaListaImagenesGabinete(ByVal Nombre_Tabla As String,
                                           ByVal enlace As String,
                                           ByRef stru_imagen_gabinete_workflow() As stru_imagen_gabinete_workflow,
                                           ByVal Id_Imagen As Long) As String
        Try
            Dim iConta As Integer = stru_imagen_gabinete_workflow.Length
            Dim Result As String
            Dim Sql_consulta = "SELECT ID,ID_TIPODOCUMENTO" &
            " FROM " & Nombre_Tabla &
            " WHERE ENLASE='" & enlace & "' AND ID <> " & Id_Imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaListaImagenesGabinete = "Funcion Solicita_lista_imagenes_gabinete dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaListaImagenesGabinete = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_imagen_gabinete_workflow(iConta)
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                        stru_imagen_gabinete_workflow(iConta).id_image = 0
                    Else
                        stru_imagen_gabinete_workflow(iConta).id_image = Datset.Tables(0).Rows(i).Item(0)
                    End If
                    stru_imagen_gabinete_workflow(iConta).ID_TIPODOCUMENTO = Datset.Tables(0).Rows(i).Item(1)
                    stru_imagen_gabinete_workflow(iConta).gabinete = Nombre_Tabla
                    iConta = iConta + 1
                Next
                SolicitaListaImagenesGabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaListaImagenesGabinete = "Inconsistencia general funcion Solicita_lista_imagenes_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_lista_rutas_imagenes_enlazadas_gabinete(ByVal nombre_gabinete As String,
                                                              ByVal enlace As String,
                                                              ByVal sub_directory_descarga As String,
                                                              ByVal option_trd As Integer,
                                                              ByVal option_convierte_pdf As Integer,
                                                              ByVal ruta_server As String,
                                                              ByRef stru_paramter_image() As stru_paramter_image) As String
        Try
            stru_paramter_image = Nothing
            Dim Sql_consulta = ""
            Dim tipo_imagen As String = " "
            If option_trd <> 0 Then
                tipo_imagen = ",TIPODOCUMENTO,ID_TIPODOCUMENTO"
            End If
            Dim Result As String
            Sql_consulta = "SELECT ID,DISC,PAG,IDEX,ENLASE,DBT" & tipo_imagen &
            " FROM " & nombre_gabinete &
            " WHERE ENLASE='" & enlace & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_rutas_imagenes_enlazadas_gabinete = "Funcion Solicita_lista_rutas_imagenes_enlazadas_gabinete dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_rutas_imagenes_enlazadas_gabinete = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_paramter_image(i)
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                        stru_paramter_image(i).ID = 0
                    Else
                        stru_paramter_image(i).ID = Datset.Tables(0).Rows(i).Item(0)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        stru_paramter_image(i).DISC = 0
                    Else
                        stru_paramter_image(i).DISC = Datset.Tables(0).Rows(i).Item(1)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(2) = True Then
                        stru_paramter_image(i).PAG = 0
                    Else
                        stru_paramter_image(i).PAG = Datset.Tables(0).Rows(i).Item(2)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        stru_paramter_image(i).IDEX = 0
                    Else
                        stru_paramter_image(i).IDEX = Datset.Tables(0).Rows(i).Item(3)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(4) = True Then
                        stru_paramter_image(i).ENLACE = ""
                    Else
                        stru_paramter_image(i).ENLACE = Datset.Tables(0).Rows(i).Item(4)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(5) = True Then
                        stru_paramter_image(i).DBT_TIPO_IMAGEN = 0
                    Else
                        stru_paramter_image(i).DBT_TIPO_IMAGEN = Datset.Tables(0).Rows(i).Item(5)
                    End If
                    If option_trd <> 0 Then
                        If Datset.Tables(0).Rows(i).IsNull(6) = True Then
                            stru_paramter_image(i).TIPODOCUMENTO = ""
                        Else
                            stru_paramter_image(i).TIPODOCUMENTO = Trim(Datset.Tables(0).Rows(i).Item(6))
                        End If
                        If Datset.Tables(0).Rows(i).IsNull(7) = True Then
                            stru_paramter_image(i).ID_TIPODOCUMENTO = 0
                        Else
                            stru_paramter_image(i).ID_TIPODOCUMENTO = Datset.Tables(0).Rows(i).Item(7)
                        End If
                    End If
                Next
                Dim refclas_visualiza As New ClassVisualisaDocumento
                Dim matri_documento() As String = Nothing
                Dim correo_copia As String = ""
                For i As Integer = 0 To stru_paramter_image.Length - 1
                    matri_documento = Nothing
                    Result = refclas_visualiza.Genera_Matris_Documentos_Almacenados(stru_paramter_image(i).ID,
                                                                                    nombre_gabinete,
                                                                                    matri_documento)
                    If Result <> "YES" Then
                        Solicita_lista_rutas_imagenes_enlazadas_gabinete = "Funcion  Solicita_lista_rutas_imagenes_enlazadas_gabinete dice " & Result
                        Exit Function
                    End If
                    If Not matri_documento Is Nothing Then
                        Dim file_ As New FileInfo(matri_documento(1))
                        If file_.Exists = False Then
                            Solicita_lista_rutas_imagenes_enlazadas_gabinete = "Imposible contrar la ruta del documento ( " & matri_documento(1) & ")"
                            Exit Function
                        End If
                        Result = Me.Solicta_documento_gabinete_service(matri_documento,
                                                                       ruta_server,
                                                                       sub_directory_descarga,
                                                                       stru_paramter_image(i).RUTA_IMAGEN_FISICA,
                                                                       stru_paramter_image(i).RUTA_UNC_ORIGINAL,
                                                                       stru_paramter_image(i).RUTA_IMAGEN_URL)
                        If Result <> "YES" Then
                            Solicita_lista_rutas_imagenes_enlazadas_gabinete = "Funcion  Solicita_lista_rutas_imagenes_enlazadas_gabinete dice " & Result
                            Exit Function
                        End If
                    End If
                Next
                Solicita_lista_rutas_imagenes_enlazadas_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_rutas_imagenes_enlazadas_gabinete = "Inconsistencia general funcion Solicita_lista_rutas_imagenes_enlazadas_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_lista_imagenes_enlzadas_gabinete(ByVal Nombre_Tabla As String,
                                                       ByVal enlace As String,
                                                       ByRef stru_paramter_image() As stru_paramter_image,
                                                       ByVal Id_Imagen As Long,
                                                       Optional ByVal option_trd As Integer = 0) As String
        Try
            stru_paramter_image = Nothing
            Dim Sql_consulta = ""
            Dim tipo_imagen As String = " "
            If option_trd <> 0 Then
                tipo_imagen = ",TIPODOCUMENTO,ID_TIPODOCUMENTO"
            End If
            Dim Result As String
            Sql_consulta = "SELECT ID,DISC,PAG,IDEX,ENLASE,DBT" & tipo_imagen &
            " FROM " & Nombre_Tabla &
            " WHERE ENLASE='" & enlace & "' AND ID <> " & Id_Imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_imagenes_enlzadas_gabinete = "Funcion Solicita_lista_imagenes_enlzadas_gabinete dice : (" & Result & ")"
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_imagenes_enlzadas_gabinete = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_paramter_image(i)
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                        stru_paramter_image(i).ID = 0
                    Else
                        stru_paramter_image(i).ID = Datset.Tables(0).Rows(i).Item(0)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        stru_paramter_image(i).DISC = 0
                    Else
                        stru_paramter_image(i).DISC = Datset.Tables(0).Rows(i).Item(1)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(2) = True Then
                        stru_paramter_image(i).PAG = 0
                    Else
                        stru_paramter_image(i).PAG = Datset.Tables(0).Rows(i).Item(2)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(3) = True Then
                        stru_paramter_image(i).IDEX = 0
                    Else
                        stru_paramter_image(i).IDEX = Datset.Tables(0).Rows(i).Item(3)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(4) = True Then
                        stru_paramter_image(i).ENLACE = ""
                    Else
                        stru_paramter_image(i).ENLACE = Datset.Tables(0).Rows(i).Item(4)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(5) = True Then
                        stru_paramter_image(i).DBT_TIPO_IMAGEN = 0
                    Else
                        stru_paramter_image(i).DBT_TIPO_IMAGEN = Datset.Tables(0).Rows(i).Item(5)
                    End If
                    If option_trd <> 0 Then
                        If Datset.Tables(0).Rows(i).IsNull(6) = True Then
                            stru_paramter_image(i).TIPODOCUMENTO = ""
                        Else
                            stru_paramter_image(i).TIPODOCUMENTO = Trim(Datset.Tables(0).Rows(i).Item(6))
                        End If
                        If Datset.Tables(0).Rows(i).IsNull(7) = True Then
                            stru_paramter_image(i).ID_TIPODOCUMENTO = 0
                        Else
                            stru_paramter_image(i).ID_TIPODOCUMENTO = Datset.Tables(0).Rows(i).Item(7)
                        End If
                    End If
                Next
                Solicita_lista_imagenes_enlzadas_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_imagenes_enlzadas_gabinete = "Inconsistencia general funcion Solicita_lista_imagenes_enlzadas_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_valor_campo_gebinete(ByVal id_imagen As Integer,
                                           ByVal nombre_gabinete As String,
                                           ByVal nombre_campo As String,
                                           ByRef valor_Campo As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el valor de un campo expecifico en un gabinete expecifico
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Respresenta la identificación de la imagen en el 
        '                        gabinete
        'nombre_gabinete       : Representa el nombre del gabinete
        'nombre_campo          : Representa el nombre del campo en el gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'valor_Campo           : Retorna el valor que contiene el campo en el gabinete
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            Sql_consulta = "select " & nombre_campo & "  from " & nombre_gabinete & "  where ID = " & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_valor_campo_gebinete = "Error función Solicita_valor_campo_gebinete " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    valor_Campo = ""
                Else
                    valor_Campo = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_valor_campo_gebinete = "YES"
                Exit Function
            Else
                Solicita_valor_campo_gebinete = "Imposible encontar el valor del campo (" & nombre_campo & ") del documento  (" & id_imagen & ") en el gabinete (" & nombre_gabinete & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_valor_campo_gebinete = "Inconsistencia general funcion Solicita_valor_campo_gebinete " & ex.Message
        End Try
    End Function
    Function Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado(ByVal Campo_Radicado As String,
                                                                                  ByVal Gabinete As String,
                                                                                  ByVal Radicado As String,
                                                                                  ByVal aplica_trd As Integer,
                                                                                  ByRef stru_paramter_image() As stru_paramter_image) As String
        Try
            Erase stru_paramter_image
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            If aplica_trd = 0 Then
                Sql_consulta = "select ID,DISC,PAG,DBT,IDEX," & Campo_Radicado & "  from " & Gabinete & "  where ENLASE = '" & Radicado & "'" &
               " order by ID "
            Else
                Sql_consulta = "select ID,DISC,PAG,DBT,IDEX," & Campo_Radicado & ",TIPODOCUMENTO,ID_TIPODOCUMENTO" & "  from " & Gabinete & "  where ENLASE = '" & Radicado & "'" &
              " order by ID "
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(Gabinete)
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado = "Error función Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_paramter_image(i)
                    stru_paramter_image(i).ID = Datset.Tables(0).Rows(i).Item(0)
                    stru_paramter_image(i).DISC = Datset.Tables(0).Rows(i).Item(1)
                    stru_paramter_image(i).PAG = Datset.Tables(0).Rows(i).Item(2)
                    stru_paramter_image(i).DBT_TIPO_IMAGEN = Datset.Tables(0).Rows(i).Item(3)
                    stru_paramter_image(i).IDEX = Datset.Tables(0).Rows(i).Item(4)
                    If Datset.Tables(0).Rows(i).IsNull(5) = True Then
                        stru_paramter_image(i).RADICADO = ""
                    Else
                        stru_paramter_image(i).RADICADO = Datset.Tables(0).Rows(i).Item(5)
                    End If
                    If aplica_trd = 1 Then
                        If Datset.Tables(0).Rows(i).IsNull(6) = True Then
                            stru_paramter_image(i).TIPODOCUMENTO = ""
                        Else
                            stru_paramter_image(i).TIPODOCUMENTO = Datset.Tables(0).Rows(i).Item(6)
                        End If
                        If Datset.Tables(0).Rows(i).IsNull(7) = True Then
                            stru_paramter_image(i).ID_TIPODOCUMENTO = 0
                        Else
                            stru_paramter_image(i).ID_TIPODOCUMENTO = Datset.Tables(0).Rows(i).Item(7)
                        End If
                    Else
                        stru_paramter_image(i).TIPODOCUMENTO = ""
                        stru_paramter_image(i).ID_TIPODOCUMENTO = 0
                    End If
                Next
                Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado = "YES"
                Exit Function
            Else
                Erase stru_paramter_image
                Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado = "Inconsistencia general función Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_id_documento_gabinete(ByVal Gabinete As String,
                                            ByVal Radicado As String,
                                            ByVal id_imagen As Integer,
                                            ByRef id_documento As Integer) As String
        Try
            Dim Sql_consulta As String = ""
            Dim Result As String = ""
            Sql_consulta = "select ID from " & Gabinete & "  where ENLASE = '" & Radicado & "'" &
            " and ID <> " & id_imagen &
            " order by ID"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(Gabinete)
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_documento_gabinete = "Error función Solicita_id_documento_gabinete " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_documento = 0
                Solicita_id_documento_gabinete = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    id_documento = 0
                Else
                    id_documento = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_documento_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_documento_gabinete = "Inconsistencia general funcion Solicita_id_documento_gabinete " & ex.Message
        End Try
    End Function
    Function Lista_campos_documentos_relacionados(ByVal Campo_Radicado As String,
                                                  ByVal aplica_trd As Integer,
                                                  ByRef class_campos_table_bostra_table As List(Of class_campos_table_bostra_table)) As String
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la estructura de campos tipo BOOTSTRAF para la lista de
        '          de documentos relacionados
        '         
        '----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        '
        '                             
        '
        ' 
        '
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'class_campos_table_bostra_table : Retorna la estructura de campos
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2024-10-29
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            class_campos_table_bostra_table = New List(Of class_campos_table_bostra_table)
            Dim item As New class_campos_table_bostra_table
            item = New class_campos_table_bostra_table
            item.field = "Campo_Radicado"
            item.title = "DOCUMENTO"
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
            item.field = "ID"
            item.title = "ID"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "DBT"
            item.title = "DBT"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            item = New class_campos_table_bostra_table
            item.field = "PAG"
            item.title = "PAG"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            If aplica_trd = 0 Then

            Else
                item = New class_campos_table_bostra_table
                item.field = "TIPODOCUMENTO"
                item.title = "DOCUMENTO"
                item.checkbox = False
                item.visible = False
                item.viisble_sql = 1
                item.clickToSelect = False
                item.visible_like_sql = 1
                class_campos_table_bostra_table.Add(item)
                item = New class_campos_table_bostra_table
                item.field = Campo_Radicado
                item.title = Campo_Radicado
                item.checkbox = False
                item.visible = False
                item.viisble_sql = 1
                item.clickToSelect = False
                item.visible_like_sql = 1
                class_campos_table_bostra_table.Add(item)
            End If
            item = New class_campos_table_bostra_table
            item.field = "ESTADO_FIRMA_DIGITAL"
            item.title = "ESTADO_FIRMA_DIGITAL"
            item.checkbox = False
            item.visible = False
            item.viisble_sql = 1
            item.clickToSelect = False
            item.visible_like_sql = 0
            class_campos_table_bostra_table.Add(item)
            Lista_campos_documentos_relacionados = "YES"
        Catch ex As Exception
            Lista_campos_documentos_relacionados = "Inconsistencia general funcion Lista_campos_documentos_relacionados " & ex.Message
        End Try
    End Function
    Function SolicitaDocumentosRelacionadosRadicadoEnlace(ByVal NombreCampoRadicado As String,
                                                          ByVal Gabinete As String,
                                                          ByVal Radicado As String,
                                                          ByVal AplicaTrd As Integer,
                                                          ByRef class_stru_Row_Gabinete_Generic As class_stru_Row_Gabinete_Generic) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita documentos relacionados al radicado en enlace radicado
        '          
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Campo_Radicado                : Representa el nombre del campo radicado
        'Gabinete                      : Representa el nombre del gabinete
        'Radicado                      : Representa el numero radicado
        'aplica_trd                    : Representa si aplica trd
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'class_stru_Row_Gabinete_Generic  : Retorna la estructura con los campos 
        ' y los registros de la consulta
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try

            Dim Result As String = ""
            ' --------- /// Solicita la estructura de los campos
            Result = Lista_campos_documentos_relacionados(NombreCampoRadicado,
                                                          AplicaTrd,
                                                          class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic)
            If Result <> "YES" Then
                SolicitaDocumentosRelacionadosRadicadoEnlace = Result
                Exit Function
            End If
            ' --------- /// Solicita el SQL para realizar la consulta
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim sql_consulta As String = ""
            Dim condicionsql As String = ""
            Dim seleccampos As String = "Select "
            For i As Integer = 0 To class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic.Count - 1
                If class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic(i).viisble_sql = "1" Then
                    If seleccampos = "Select " Then
                        seleccampos = seleccampos & "  " & class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic(i).field
                    Else
                        seleccampos = seleccampos & "," & class_stru_Row_Gabinete_Generic.Obj_ilist_fileds_generic(i).field
                    End If
                End If
            Next
            condicionsql = "  where ENLASE='" & Radicado & "' "
            Dim order_colum As String = "ASC"
            Dim colum_order_name As String = "ID"
            Dim sqlfrom As String = " From " & Gabinete & " as da "
            sql_consulta = seleccampos & " " & sqlfrom & " " & condicionsql & "  order by ID  " & " LIMIT 5000"
            ' --------- /// Ejecuta la consulta  y retorna los row
            Result = ClassDaGabinete.SolicitaRowDocumentosRelacionadosRadicadoEnlaceTableBoot(sql_consulta,
                                                                                              class_stru_Row_Gabinete_Generic.Obj_ilist_row_generic)
            If Result <> "YES" Then
                SolicitaDocumentosRelacionadosRadicadoEnlace = Result
                Exit Function
            End If
            SolicitaDocumentosRelacionadosRadicadoEnlace = "YES"
            Exit Function
        Catch ex As Exception
            SolicitaDocumentosRelacionadosRadicadoEnlace = "Inconsistencia general funcion SolicitaDocumentosRelacionadosRadicadoEnlace " & ex.Message
        End Try
    End Function
    Function SolicitaRowDocumentosRelacionadosRadicadoEnlaceTableBoot(ByVal consulta As String,
                                                                      ByRef stru_row_gabinete_generic As Object) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura generica con los datos de la consulta
        '         de registros de documentos relacionados al radicado
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
        'Fecha                 : 2024-10-29
        'Elabora               : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Class_ConverDataTable As New Class_ConverDataTable
            Dim Datset As DataSet = New DataSet("gabinete")
            Result = ref.SELECTION_SELECT_FIELDA(consulta, Datset)
            If Result <> "YES" Then
                SolicitaRowDocumentosRelacionadosRadicadoEnlaceTableBoot = "Funcion  SolicitaRowDocumentosRelacionadosRadicadoEnlaceTableBoot " & Result
                Exit Function
            End If
            stru_row_gabinete_generic = JsonConvert.SerializeObject(Datset.Tables(0))
            SolicitaRowDocumentosRelacionadosRadicadoEnlaceTableBoot = "YES"
        Catch ex As Exception
            SolicitaRowDocumentosRelacionadosRadicadoEnlaceTableBoot = "Inconsistencia general fucnion SolicitaRowDocumentosRelacionadosRadicadoEnlaceTableBoot " & ex.Message
        End Try
    End Function
    Function Lista_documentos_relacionados_a_radicado_enlace(ByVal Campo_Radicado As String,
                                                             ByVal Gabinete As String,
                                                             ByVal Radicado As String,
                                                             ByVal aplica_trd As Integer,
                                                             ByVal id_tarea As Long,
                                                             ByVal option_versionado As Integer,
                                                             ByRef scripma As GridView,
                                                             ByRef labetitle As Label,
                                                             ByRef hideselecion As HtmlInputHidden,
                                                             ByRef updat As UpdatePanel,
                                                             ByRef update_label As UpdatePanel,
                                                             ByRef numero_documentos As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista los documentos relacionados a una tarea workflow cuando la
        '          tarea esta en estado de enlace
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Campo_Radicado      : Representa el nombre del campo radicado
        'Gabinete            : Representa el nombre del gabinete
        'Radicado            : Representa el radicado de la tarea
        'aplica_trd          : Representa si se muestra la tipologia
        'IdTareaWorkflow         : Representa la identificación de la tarea
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'scripma             : Retorna la estructura con la lista de documentos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            If aplica_trd = 0 Then
                Sql_consulta = "select ID,DBT,PAG," & Campo_Radicado & " AS DOCUMENTO,ESTADO_FIRMA_DIGITAL  from " & Gabinete & "  where ENLASE = '" & Radicado & "'" &
               " order by ID"
            Else
                Sql_consulta = "select ID,DBT," & Campo_Radicado & ",TIPODOCUMENTO AS DOCUMENTO,ESTADO_FIRMA_DIGITAL" & "  from " & Gabinete & "  where ENLASE = '" & Radicado & "'" &
              " order by ID"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(Gabinete)
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_relacionados_a_radicado_enlace = "Error función Lista_documentos_relacionados_a_radicado_enlace " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = "Documentos " & 0
                numero_documentos = 0
                Datset.Tables(0).Rows.Add(Datset.Tables(0).NewRow)
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                scripma.Rows(0).Visible = False
                updat.Update()
                update_label.Update()
                Lista_documentos_relacionados_a_radicado_enlace = "YES"
                Exit Function
            Else
                labetitle.Text = "Documentos " & Datset.Tables(0).Rows.Count
                numero_documentos = Datset.Tables(0).Rows.Count
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                update_label.Update()
                updat.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    '---/////Agrega parametro principal
                    scripma.Rows(i).Attributes.Add("id_rad", scripma.Rows(i).Cells(1).Text)
                    Dim icono As String = ""
                    '----////Agrega icono awe
                    Me.SolicitaIconoImageFownt(scripma.Rows(i).Cells(2).Text.ToString,
                                                icono)
                    '----////Agrega icono firma awe
                    If Val(scripma.Rows(i).Cells(5).Text) = 1 Then
                        icono = "fal fa-file-certificate"
                    End If
                    Dim text As String = WebUtility.HtmlDecode(scripma.Rows(i).Cells(4).Text.ToString).Trim()
                    Dim value_documento As String = ""
                    Result = RemoveDiacritics(text,
                                             value_documento)
                    scripma.Rows(i).Attributes.Add("idd_rad", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea & "|0|fa-file")

                    'Agrego el div de la celda general
                    Dim divhtml_Celda As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda.Attributes.Add("class", "row pl-1 w-100")

                    Dim imput_check As New HtmlControls.HtmlGenericControl("INPUT")
                    imput_check.Attributes.Add("type", "checkbox")
                    imput_check.Attributes.Add("Class", "ml-0 chek_selecion_list_rad")
                    imput_check.Attributes.Add("chek_id", scripma.Rows(i).Cells(1).Text)

                    Dim divhtml_div_check As New HtmlControls.HtmlGenericControl("div")
                    divhtml_div_check.Attributes.Add("class", "pl-0 pt-2")
                    divhtml_div_check.Controls.Add(imput_check)
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml_div_check)


                    '//////-----Zona visualiza icono visualiza documento---//////////////

                    '-------//Agrega la celda contenedora de los atributos de visualizacion de documentos
                    Dim divhtml_Celda_icono_title As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda_icono_title.Attributes.Add("class", "w-100 col-10 pl-2 row")
                    divhtml_Celda_icono_title.Style.Add("margin-right", "1px")
                    divhtml_Celda_icono_title.Attributes.Add("onclick", "prevent(event,this);")
                    divhtml_Celda_icono_title.Attributes.Add("title", "Ver documento")
                    divhtml_Celda_icono_title.Attributes.Add("id_rad", scripma.Rows(i).Cells(1).Text)
                    divhtml_Celda_icono_title.Attributes.Add("idd_rad", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea & "|0|fa-file")
                    divhtml_Celda_icono_title.Attributes.Add("tip_event", "vis_doc_selecion_rad")

                    '--//Inicializa la celda del icono del tipo documento
                    Dim divhtml_Celda_icono As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda_icono.Attributes.Add("class", "col-2 pt-2 ")
                    '---//Inicializa la (A) contenedora del icono del documento
                    Dim ahtml_icono_principal As New HtmlControls.HtmlGenericControl("a")
                    '---Agrega clase al icono para udentificarlo y actuaizarlo para firma digital
                    Dim clase_icono As String = "r_d_v_d_" & scripma.Rows(i).Cells(1).Text & i
                    ahtml_icono_principal.Attributes.Add("class", "font-weight-light " & clase_icono)
                    'ahtml_icono_principal.Attributes.Add("class", " font-weight-light")
                    ahtml_icono_principal.ID = "d_v_d_" & scripma.Rows(i).Cells(1).Text
                    '--//Inicializa el (I) DEL icono del documento
                    Dim ihtml_icono As New HtmlControls.HtmlGenericControl("i")
                    ihtml_icono.Attributes.Add("class", icono)
                    ihtml_icono.Style.Add("color", "#0062cc")
                    '--//////////Agrega la I del icono  a la celda del icono
                    ahtml_icono_principal.Controls.Add(ihtml_icono)
                    '---////////Agrega el nombre del documento si no trae tipologia
                    If text = "" Then
                        text = "D-" & scripma.Rows(i).Cells(1).Text
                    End If
                    '---////////Agrega el A la celda del icono
                    divhtml_Celda_icono.Controls.Add(ahtml_icono_principal)
                    '---////////Agrega la celda icono a la celda contenedora del icono y el spam del tuitulo
                    divhtml_Celda_icono_title.Controls.Add(divhtml_Celda_icono)

                    '----//Inicializa la celda del spam
                    Dim divhtml_Celda_span As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda_span.Attributes.Add("class", "col-10 pl-1 pt-1")
                    '----//Inicializa el spam del titulo
                    Dim spamHtmlTitle As New HtmlControls.HtmlGenericControl("SPAM")
                    spamHtmlTitle.InnerText = text
                    spamHtmlTitle.Attributes.Add("class", "pl-0 GridviewSpanOverFlow")
                    spamHtmlTitle.Style.Add("color", "black")
                    '----////////Agrega el SPAN a la celda
                    divhtml_Celda_span.Controls.Add(spamHtmlTitle)
                    '----////////Agrega la celda span a la celda icono titulo
                    divhtml_Celda_icono_title.Controls.Add(divhtml_Celda_span)
                    divhtml_Celda.Controls.Add(divhtml_Celda_icono_title)

                    '//////-----Zona menu toogle---//////////////
                    '---//Incializa celda toogle del menu
                    Dim divhtml_Celda_toogle As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda_toogle.Attributes.Add("class", "col-2 p-0 nav-item dropdown active")
                    '--//Incializa la (A) para el toogle del menu
                    Dim ahtml_toogle As New HtmlControls.HtmlGenericControl("a")
                    ahtml_toogle.Attributes.Add("class", "nav-link dropdown-toggle justify-content-start btn-lg mt-1")
                    ahtml_toogle.Attributes.Add("data-toggle", "dropdown")
                    ahtml_toogle.Attributes.Add("aria-haspopup", "true")
                    ahtml_toogle.Attributes.Add("aria-expanded", "false")
                    ahtml_toogle.Attributes.Add("href", "#")
                    '-----//////////Agrega la (A) del toogle a la celda toogle
                    divhtml_Celda_toogle.Controls.Add(ahtml_toogle)
                    '-----/////////Agrega la celda toogle a la celda general
                    divhtml_Celda.Controls.Add(divhtml_Celda_toogle)
                    '--//Incializa el div del drowp menu
                    Dim divhtmldrowp As New HtmlControls.HtmlGenericControl("div")
                    divhtmldrowp.Attributes.Add("class", "dropdown-menu")
                    divhtmldrowp.Attributes.Add("aria-labelledby", "navbarDropdownMenuLink")

                    '--//Incializa la (A) de la opción ELIMINAR DOCUMENTO
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Eliminar documento")
                    ahtml.Attributes.Add("id_rad", scripma.Rows(i).Cells(1).Text)
                    ahtml.Attributes.Add("idd_rad", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea & "|0|fa-file")
                    ahtml.Attributes.Add("tip_event", "elim_doc_selecion_rad")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.ID = "d_e_d_r_" & scripma.Rows(i).Cells(1).Text
                    '--//Inicializa la (I) del icono 
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Attributes.Add("class", "fal fa-trash-alt")
                    ihtml.Style.Add("color", "#0062cc")
                    '---------/////////Agrega (i) a la (a) opcion eliminar
                    ahtml.Controls.Add(ihtml)
                    '--//Inicializa el spam del titulo
                    Dim spamHtml As New HtmlControls.HtmlGenericControl("SPAM")
                    spamHtml.InnerText = "Eliminar documento"
                    spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                    ahtml.Controls.Add(spamHtml)
                    '---------/////////Agrega la (a) al div drowp
                    divhtmldrowp.Controls.Add(ahtml)

                    '--//Incializa la (A) de la opción CAMBIAR TIPOLOGIA
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Attributes.Add("class", "fal fa-file-edit")
                    ihtml.Style.Add("color", "#0062cc")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                    ahtml.Attributes.Add("title", "Cambiar tipología documental")
                    ahtml.Attributes.Add("id_rad", scripma.Rows(i).Cells(1).Text)
                    ahtml.Attributes.Add("idd_rad", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea & "|0|fa-file")
                    ahtml.Attributes.Add("tip_event", "cambia_doc_selecion_rad")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.ID = "d_ch_t_d_" & scripma.Rows(i).Cells(1).Text
                    '--/////////////Agrega la (i) a la (a) opcion cambiar tipologia
                    ahtml.Controls.Add(ihtml)
                    '--//Inicializa el spam del titulo del documento
                    spamHtml = New HtmlControls.HtmlGenericControl("SPAM")
                    spamHtml.InnerText = "Cambiar tipología"
                    spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                    '--///////////Agrega el span a la (A)
                    ahtml.Controls.Add(spamHtml)
                    '---------/////////Agrega la (a) al div drowp
                    divhtmldrowp.Controls.Add(ahtml)

                    '--//Incializa la (A) de la opción FRIMA DIGITAL
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")

                    If Val(scripma.Rows(i).Cells(5).Text) = 0 Then
                        ahtml.Attributes.Add("title", "Firmar y agerar meta dato")
                    End If
                    If Val(scripma.Rows(i).Cells(5).Text) = 1 Then
                        ahtml.Attributes.Add("title", "Documento con firma digital y meta datos")
                    End If
                    If Val(scripma.Rows(i).Cells(5).Text) = 2 Then
                        ahtml.Attributes.Add("title", "Documento con meta datos")
                    End If
                    ahtml.Attributes.Add("id_rad", scripma.Rows(i).Cells(1).Text)
                    ahtml.Attributes.Add("idd_rad", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea & "|0|fa-file|" & clase_icono)
                    ahtml.Attributes.Add("tip_event", "firma_doc_selecion_rad")
                    ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.ID = "d_s_d_f_" & scripma.Rows(i).Cells(1).Text
                    '--//Inicializa (i) a la (a) opcion firma digital
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    If Val(scripma.Rows(i).Cells(5).Text) = 0 Then
                        ihtml.Attributes.Add("class", " fal fa-file-signature")
                    End If
                    If Val(scripma.Rows(i).Cells(5).Text) = 1 Then
                        ihtml.Attributes.Add("class", "fal fa-lock-alt")
                    End If
                    If Val(scripma.Rows(i).Cells(5).Text) = 2 Then
                        ihtml.Attributes.Add("class", "fal fa-file-invoice")
                    End If
                    ihtml.Style.Add("color", "#0062cc")
                    '--////////////Agrega la (i) a la (a) opcion firma digital
                    ahtml.Controls.Add(ihtml)
                    spamHtml = New HtmlControls.HtmlGenericControl("SPAM")
                    spamHtml.InnerText = "Firma digital"
                    spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                    ahtml.Controls.Add(spamHtml)
                    '--////////////Agrega la (a) al div drowp
                    divhtmldrowp.Controls.Add(ahtml)
                    divhtml_Celda_toogle.Controls.Add(divhtmldrowp)
                    divhtml_Celda.Style.Add("display", "inline-flex")
                    If option_versionado = 1 Then
                        '--//Incializa el (I) de la opción VERSIONES DEl DOCUMENTO
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Attributes.Add("class", "fal fa-folder-open")
                        ihtml.Style.Add("color", "#0062cc")
                        '--//Incializa el (a) 
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                        ahtml.Attributes.Add("title", "Versiones del documento")
                        ahtml.Attributes.Add("id_rad", scripma.Rows(i).Cells(1).Text)
                        ahtml.Attributes.Add("idd_rad", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea & "|0|fa-file|" & clase_icono)
                        ahtml.Attributes.Add("tip_event", "lista_ver_doc_selecion_rad")
                        ahtml.Style.Add("margin-left", "1px")
                        ahtml.ID = "d_l_v_d_" & scripma.Rows(i).Cells(1).Text
                        '--////////////Agrega (i) a la (a) opcion versiones del documento
                        ahtml.Controls.Add(ihtml)
                        '--//Incializa el spam del titulo
                        spamHtml = New HtmlControls.HtmlGenericControl("SPAM")
                        spamHtml.InnerText = "Versiones del documento"
                        spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                        ahtml.Controls.Add(spamHtml)
                        '--////////////Agrega la (a) al div drowp
                        divhtmldrowp.Controls.Add(ahtml)

                        '--//Incializa el (I) de la opción REMPLAZA VERSION
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Attributes.Add("class", "fal fa-clone")
                        ihtml.Style.Add("color", "#0062cc")
                        '--//Incializa el (A)
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                        ahtml.Attributes.Add("title", "Remplazar documento")
                        ahtml.Attributes.Add("id_rad", scripma.Rows(i).Cells(1).Text)
                        ahtml.Attributes.Add("idd_rad", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea & "|0|fa-file|" & clase_icono)
                        ahtml.Attributes.Add("tip_event", "remplaza_ver_doc_selecion_rad")
                        ahtml.Style.Add("margin-left", "1px")
                        ahtml.ID = "d_r_v_d_" & scripma.Rows(i).Cells(1).Text
                        '--////////////Agrega (i) a la (a) opcion rmplazar
                        ahtml.Controls.Add(ihtml)
                        '--//Incializa el spam del titulo
                        spamHtml = New HtmlControls.HtmlGenericControl("SPAM")
                        spamHtml.InnerText = "Remplazar documento"
                        spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                        ahtml.Controls.Add(spamHtml)
                        '--////////////Agrega la (a) al div drowp
                        divhtmldrowp.Controls.Add(ahtml)
                        divhtml_Celda_toogle.Controls.Add(divhtmldrowp)
                        divhtml_Celda.Style.Add("display", "inline-flex")
                    End If
                    scripma.Rows(i).Cells(4).Controls.Add(divhtml_Celda)
                    scripma.Rows(i).Style.Add("cursor", "pointer")

                Next
                Lista_documentos_relacionados_a_radicado_enlace = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_relacionados_a_radicado_enlace = "Inconistencia general función Lista_documentos_relacionados_a_radicado_enlace " & ex.Message
        End Try
    End Function
    Function Lista_documentos_relacionados_a_tarea_workflow(ByVal Campo_Radicado As String,
                                                            ByVal Gabinete As String,
                                                            ByVal Radicado As String,
                                                            ByVal aplica_trd As Integer,
                                                            ByVal id_tarea_wf As Long,
                                                            ByVal option_versionado As Integer,
                                                            ByRef scripma As GridView,
                                                            ByRef labetitle As Label,
                                                            ByRef hideselecion As HtmlInputHidden,
                                                            ByRef updat As UpdatePanel,
                                                            ByRef update_label As UpdatePanel,
                                                            ByRef numero_documentos As Integer,
                                                            Optional ByVal modernDocumentCountFormat As Boolean = False) As String
        '---------------------------------------------------------------------------
        'Funcion : Lista los documentos relacionados a una tarea workflow cuando la
        '          tarea esta asignada
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'Campo_Radicado      : Representa el nombre del campo radicado
        'Gabinete            : Representa el nombre del gabinete
        'Radicado            : Representa el radicado de la tarea
        'aplica_trd          : Representa si se muestra la tipologia
        'IdTareaWorkflow         : Representa la identificación de la tarea
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'scripma             : Retorna la estructura con la lista de documentos
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = ""
            HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_CHAECHE") = Gabinete
            If aplica_trd = 0 Then
                HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_CAMPOS_CHAECHE") = "select ID,DBT,PAG," & Campo_Radicado & " AS DOCUMENTO,ESTADO_FIRMA_DIGITAL"
                Sql_consulta = "select ID,DBT,PAG," & Campo_Radicado & " AS DOCUMENTO,ESTADO_FIRMA_DIGITAL  from " & Gabinete & "  where ENLASE = '" & Radicado & "'" &
               " order by ID"
            Else
                HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_CAMPOS_CHAECHE") = "select ID,DBT," & Campo_Radicado & ",TIPODOCUMENTO AS DOCUMENTO,ESTADO_FIRMA_DIGITAL"
                Sql_consulta = "select ID,DBT," & Campo_Radicado & ",TIPODOCUMENTO AS DOCUMENTO,ESTADO_FIRMA_DIGITAL" & "  from " & Gabinete &
                     "  where ENLASE = '" & Radicado & "'" &
              " order by ID"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(Gabinete)
            Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_documentos_relacionados_a_tarea_workflow = "Error función Lista_documentos_relacionados_a_tarea_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                labetitle.Text = If(modernDocumentCountFormat, "Documentos (0)", "Documentos 0")
                numero_documentos = 0
                Datset.Tables(0).Rows.Add(Datset.Tables(0).NewRow)
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                scripma.Rows(0).Visible = False
                updat.Update()
                update_label.Update()
                Lista_documentos_relacionados_a_tarea_workflow = "YES"
                Exit Function
            Else
                labetitle.Text = If(modernDocumentCountFormat,
                                    "Documentos (" & Datset.Tables(0).Rows.Count & ")",
                                    "Documentos " & Datset.Tables(0).Rows.Count)
                numero_documentos = Datset.Tables(0).Rows.Count
                scripma.DataSource = Datset
                hideselecion.Value = "-1"
                scripma.DataBind()
                update_label.Update()
                updat.Update()
                For i As Integer = 0 To scripma.Rows.Count - 1
                    '---/////Agrega parametro principal
                    scripma.Rows(i).Attributes.Add("id_wf", scripma.Rows(i).Cells(1).Text)
                    Dim icono As String = ""
                    '----////Agrega icono awe
                    Me.SolicitaIconoImageFownt(scripma.Rows(i).Cells(2).Text.ToString,
                                                icono)
                    '----////Agrega icono firma awe
                    If Val(scripma.Rows(i).Cells(5).Text) = 1 Then
                        icono = "fal fa-file-certificate"
                    End If
                    Dim text As String = WebUtility.HtmlDecode(scripma.Rows(i).Cells(4).Text.ToString).Trim()
                    Dim value_documento As String = ""
                    Result = RemoveDiacritics(text,
                                             value_documento)
                    scripma.Rows(i).Attributes.Add("idd_wf", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea_wf & "|0|fa-file")
                    'Agrego el div de la celda general
                    Dim divhtml_Celda As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda.Attributes.Add("class", "row pl-1 w-100")

                    Dim imput_check As New HtmlControls.HtmlGenericControl("INPUT")
                    imput_check.Attributes.Add("type", "checkbox")
                    imput_check.Attributes.Add("Class", "ml-0 chek_selecion_list_wf")
                    imput_check.Attributes.Add("chek_id", scripma.Rows(i).Cells(1).Text)

                    Dim divhtml_div_check As New HtmlControls.HtmlGenericControl("div")
                    divhtml_div_check.Attributes.Add("class", "pl-0 pt-2")
                    divhtml_div_check.Controls.Add(imput_check)
                    scripma.Rows(i).Cells(0).Controls.Add(divhtml_div_check)


                    '//////-----Zona visualiza icono visualiza documento---//////////////

                    '-------//Agrega la celda contenedora de los atributos de visualizacion de documentos
                    Dim divhtml_Celda_icono_title As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda_icono_title.Attributes.Add("class", "w-100 col-10 pl-2 row")
                    divhtml_Celda_icono_title.Style.Add("margin-right", "1px")
                    divhtml_Celda_icono_title.Attributes.Add("onclick", "prevent(event,this);")
                    divhtml_Celda_icono_title.Attributes.Add("title", "Ver documento")
                    divhtml_Celda_icono_title.Attributes.Add("id_wf", scripma.Rows(i).Cells(1).Text)
                    divhtml_Celda_icono_title.Attributes.Add("idd_wf", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea_wf & "|0|fa-file")
                    divhtml_Celda_icono_title.Attributes.Add("tip_event", "vis_doc_selecion_wf")

                    '--//Inicializa la celda del icono del tipo documento
                    Dim divhtml_Celda_icono As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda_icono.Attributes.Add("class", "col-2 pt-2 ")
                    '---//Inicializa la (A) contenedora del icono del documento
                    Dim ahtml_icono_principal As New HtmlControls.HtmlGenericControl("a")
                    '---Agrega clase al icono para identificarlo y actuaizarlo para firma digital
                    Dim clase_icono As String = "f_d_v_d_a_" & scripma.Rows(i).Cells(1).Text & i
                    ahtml_icono_principal.Attributes.Add("class", "font-weight-light " & clase_icono)
                    ahtml_icono_principal.ID = "d_v_i_d_a_" & scripma.Rows(i).Cells(1).Text
                    '--//Inicializa el (I) DEL icono del documento
                    Dim ihtml_icono As New HtmlControls.HtmlGenericControl("i")
                    ihtml_icono.Attributes.Add("class", icono)
                    ihtml_icono.Style.Add("color", "#0062cc")
                    '--//////////Agrega la I del icono  a la celda del icono
                    ahtml_icono_principal.Controls.Add(ihtml_icono)
                    '---////////Agrega el nombre del documento si no trae tipologia
                    If text = "" Then
                        text = "D-" & scripma.Rows(i).Cells(1).Text
                    End If
                    '---////////Agrega el A la celda del icono
                    divhtml_Celda_icono.Controls.Add(ahtml_icono_principal)
                    '---////////Agrega la celda icono a la celda contenedora del icono y el spam del tuitulo
                    divhtml_Celda_icono_title.Controls.Add(divhtml_Celda_icono)

                    '----//Inicializa la celda del spam
                    Dim divhtml_Celda_span As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda_span.Attributes.Add("class", "col-10 pl-1 pt-1")
                    '----//Inicializa el spam del titulo
                    Dim spamHtmlTitle As New HtmlControls.HtmlGenericControl("SPAM")
                    spamHtmlTitle.InnerText = text
                    spamHtmlTitle.Attributes.Add("class", "pl-0 GridviewSpanOverFlow")
                    spamHtmlTitle.Style.Add("color", "black")
                    '----////////Agrega el SPAN a la celda
                    divhtml_Celda_span.Controls.Add(spamHtmlTitle)
                    '----////////Agrega la celda span a la celda icono titulo
                    divhtml_Celda_icono_title.Controls.Add(divhtml_Celda_span)
                    divhtml_Celda.Controls.Add(divhtml_Celda_icono_title)

                    '//////-----Zona menu toogle---//////////////
                    '---//Incializa celda toogle del menu
                    Dim divhtml_Celda_toogle As New HtmlControls.HtmlGenericControl("div")
                    divhtml_Celda_toogle.Attributes.Add("class", "col-2 p-0 nav-item dropdown active")
                    '--//Incializa la (A) para el toogle del menu
                    Dim ahtml_toogle As New HtmlControls.HtmlGenericControl("a")
                    ahtml_toogle.Attributes.Add("class", "nav-link dropdown-toggle justify-content-start btn-lg mt-1")
                    ahtml_toogle.Attributes.Add("data-toggle", "dropdown")
                    ahtml_toogle.Attributes.Add("aria-haspopup", "true")
                    ahtml_toogle.Attributes.Add("aria-expanded", "false")
                    ahtml_toogle.Attributes.Add("href", "#")
                    '-----//////////Agrega la (A) del toogle a la celda toogle
                    divhtml_Celda_toogle.Controls.Add(ahtml_toogle)
                    '-----/////////Agrega la celda toogle a la celda general
                    divhtml_Celda.Controls.Add(divhtml_Celda_toogle)
                    '--//Incializa el div del drowp menu
                    Dim divhtmldrowp As New HtmlControls.HtmlGenericControl("div")
                    divhtmldrowp.Attributes.Add("class", "dropdown-menu")
                    divhtmldrowp.Attributes.Add("aria-labelledby", "navbarDropdownMenuLink")

                    '--//Incializa la (A) de la opción ELIMINAR DOCUMENTO
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Eliminar documento")
                    ahtml.Attributes.Add("id_wf", scripma.Rows(i).Cells(1).Text)
                    ahtml.Attributes.Add("idd_wf", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea_wf & "|0|fa-file")
                    ahtml.Attributes.Add("tip_event", "elim_doc_selecion_wf")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.ID = "d_e_d_a_" & scripma.Rows(i).Cells(1).Text
                    '--//Inicializa la (I) del icono 
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Attributes.Add("class", "fal fa-trash-alt")
                    ihtml.Style.Add("color", "#0062cc")
                    '---------/////////Agrega (i) a la (a) opcion eliminar
                    ahtml.Controls.Add(ihtml)
                    '--//Inicializa el spam del titulo
                    Dim spamHtml As New HtmlControls.HtmlGenericControl("SPAM")
                    spamHtml.InnerText = "Eliminar documento"
                    spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                    ahtml.Controls.Add(spamHtml)
                    '---------/////////Agrega la (a) al div drowp
                    divhtmldrowp.Controls.Add(ahtml)

                    '--//Incializa la (A) de la opción CAMBIAR TIPOLOGIA
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Attributes.Add("class", "fal fa-file-edit")
                    ihtml.Style.Add("color", "#0062cc")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                    ahtml.Attributes.Add("title", "Cambiar tipología documental")
                    ahtml.Attributes.Add("id_wf", scripma.Rows(i).Cells(1).Text)
                    ahtml.Attributes.Add("idd_wf", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea_wf & "|0|fa-file")
                    ahtml.Attributes.Add("tip_event", "cambia_doc_selecion_wf")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.ID = "d_ch_t_d_a_" & scripma.Rows(i).Cells(1).Text
                    '--/////////////Agrega la (i) a la (a) opcion cambiar tipologia
                    ahtml.Controls.Add(ihtml)
                    '--//Inicializa el spam del titulo del documento
                    spamHtml = New HtmlControls.HtmlGenericControl("SPAM")
                    spamHtml.InnerText = "Cambiar tipología"
                    spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                    '--///////////Agrega el span a la (A)
                    ahtml.Controls.Add(spamHtml)
                    '---------/////////Agrega la (a) al div drowp
                    divhtmldrowp.Controls.Add(ahtml)


                    '--//Incializa la (A) de la opción FRIMA DIGITAL
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")

                    If Val(scripma.Rows(i).Cells(5).Text) = 0 Then
                        ahtml.Attributes.Add("title", "Firmar y agerar meta dato")
                    End If
                    If Val(scripma.Rows(i).Cells(5).Text) = 1 Then
                        ahtml.Attributes.Add("title", "Documento con firma digital y meta datos")
                    End If
                    If Val(scripma.Rows(i).Cells(5).Text) = 2 Then
                        ahtml.Attributes.Add("title", "Documento con meta datos")
                    End If
                    ahtml.Attributes.Add("id_wf", scripma.Rows(i).Cells(1).Text)
                    ahtml.Attributes.Add("idd_wf", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea_wf & "|0|fa-file" & "|" & clase_icono)
                    ahtml.Attributes.Add("tip_event", "firma_doc_selecion_wf")
                    ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.ID = "d_s_d_f_a_" & scripma.Rows(i).Cells(1).Text
                    '--//Inicializa (i) a la (a) opcion firma digital
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    If Val(scripma.Rows(i).Cells(5).Text) = 0 Then
                        ihtml.Attributes.Add("class", " fal fa-file-signature")
                    End If
                    If Val(scripma.Rows(i).Cells(5).Text) = 1 Then
                        ihtml.Attributes.Add("class", "fal fa-lock-alt")
                    End If
                    If Val(scripma.Rows(i).Cells(5).Text) = 2 Then
                        ihtml.Attributes.Add("class", "fal fa-file-invoice")
                    End If
                    ihtml.Style.Add("color", "#0062cc")
                    '--////////////Agrega la (i) a la (a) opcion firma digital
                    ahtml.Controls.Add(ihtml)
                    spamHtml = New HtmlControls.HtmlGenericControl("SPAM")
                    spamHtml.InnerText = "Firma digital"
                    spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                    ahtml.Controls.Add(spamHtml)
                    '--////////////Agrega la (a) al div drowp
                    divhtmldrowp.Controls.Add(ahtml)

                    If option_versionado = 1 Then
                        '--//Incializa el (I) de la opción VERSIONES DEl DOCUMENTO
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Attributes.Add("class", "fal fa-folder-open")
                        ihtml.Style.Add("color", "#0062cc")
                        '--//Incializa el (a) 
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                        ahtml.Attributes.Add("title", "Versiones del documento")
                        ahtml.Attributes.Add("id_wf", scripma.Rows(i).Cells(1).Text)
                        ahtml.Attributes.Add("idd_wf", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea_wf & "|0|fa-file|" & clase_icono)
                        ahtml.Attributes.Add("tip_event", "lista_ver_doc_selecion_wf")
                        ahtml.Style.Add("margin-left", "1px")
                        ahtml.ID = "d_l_v_d_a_" & scripma.Rows(i).Cells(1).Text
                        '--////////////Agrega (i) a la (a) opcion versiones del documento
                        ahtml.Controls.Add(ihtml)
                        '--//Incializa el spam del titulo
                        spamHtml = New HtmlControls.HtmlGenericControl("SPAM")
                        spamHtml.InnerText = "Versiones del documento"
                        spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                        ahtml.Controls.Add(spamHtml)
                        '--////////////Agrega la (a) al div drowp
                        divhtmldrowp.Controls.Add(ahtml)

                        '--//Incializa el (I) de la opción REMPLAZA VERSION
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Attributes.Add("class", "fal fa-clone")
                        ihtml.Style.Add("color", "#0062cc")
                        '--//Incializa el (A)
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("Class", "dropdown-item font-weight-light")
                        ahtml.Attributes.Add("title", "Remplazar documento")
                        ahtml.Attributes.Add("id_wf", scripma.Rows(i).Cells(1).Text)

                        ahtml.Attributes.Add("idd_wf", Gabinete & "|" & scripma.Rows(i).Cells(1).Text & "|" & Radicado & "|" & scripma.Rows(i).Cells(2).Text.ToString() & "|" & value_documento & "|" & id_tarea_wf & "|0|fa-file" & "|" & clase_icono)
                        ahtml.Attributes.Add("tip_event", "remplaza_ver_doc_selecion_wf")
                        ahtml.Style.Add("margin-left", "1px")
                        ahtml.ID = "d_r_v_d_a_" & scripma.Rows(i).Cells(1).Text
                        '--////////////Agrega (i) a la (a) opcion eliminar
                        ahtml.Controls.Add(ihtml)
                        '--//Incializa el spam del titulo
                        spamHtml = New HtmlControls.HtmlGenericControl("SPAM")
                        spamHtml.InnerText = "Remplazar documento"
                        spamHtml.Attributes.Add("class", "pl-1 font-weight-light")
                        ahtml.Controls.Add(spamHtml)
                        '--////////////Agrega la (a) al div drowp
                        divhtmldrowp.Controls.Add(ahtml)

                    End If
                    divhtml_Celda_toogle.Controls.Add(divhtmldrowp)
                    divhtml_Celda.Style.Add("display", "inline-flex")
                    scripma.Rows(i).Cells(4).Controls.Add(divhtml_Celda)
                    scripma.Rows(i).Style.Add("cursor", "pointer")

                Next
                Lista_documentos_relacionados_a_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_documentos_relacionados_a_tarea_workflow = "Inconistencia general función Lista_documentos_relacionados_a_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Inicializa_documentos_seleccion_workflow(ByRef pag As Page) As String
        Try
            Dim GridView_list_documento_relacion_wf As GridView = pag.FindControl("GridView_list_documento_relacion_wf")
            If GridView_list_documento_relacion_wf Is Nothing Then
                Inicializa_documentos_seleccion_workflow = "Imposible encontrar el control (GridView_list_documento_relacion_wf)"
                Exit Function
            End If
            Dim Label_docu_relacionado_wf As Label = pag.FindControl("Label_docu_relacionado_wf")
            If Label_docu_relacionado_wf Is Nothing Then
                Inicializa_documentos_seleccion_workflow = "Imposible encontrar el control (Label_docu_relacionado_wf)"
                Exit Function
            End If
            Dim hiden_seleccion_documento_wf As HtmlInputHidden = pag.FindControl("hiden_seleccion_documento_wf")
            If hiden_seleccion_documento_wf Is Nothing Then
                Inicializa_documentos_seleccion_workflow = "Imposible encontrar el control (hiden_seleccion_documento_wf)"
                Exit Function
            End If
            Dim hiden_seleccion_documento_id_wf As HtmlInputHidden = pag.FindControl("hiden_seleccion_documento_id_wf")
            If hiden_seleccion_documento_id_wf Is Nothing Then
                Inicializa_documentos_seleccion_workflow = "Imposible encontrar el control (hiden_seleccion_documento_id_wf)"
                Exit Function
            End If
            Dim Hidden_numero_doc_rel_wf As HtmlInputHidden = pag.FindControl("Hidden_numero_doc_rel_wf")
            If Hidden_numero_doc_rel_wf Is Nothing Then
                Inicializa_documentos_seleccion_workflow = "Imposible encontrar el control (Hidden_numero_doc_rel_wf)"
                Exit Function
            End If
            Dim UpdatePanel_label_seleccion As UpdatePanel = pag.FindControl("UpdatePanel_label_seleccion")
            If UpdatePanel_label_seleccion Is Nothing Then
                Inicializa_documentos_seleccion_workflow = "Imposible encontrar el control (UpdatePanel_label_seleccion)"
                Exit Function
            End If
            Dim UpdatePanelseleccion As UpdatePanel = pag.FindControl("UpdatePanelseleccion")
            If UpdatePanelseleccion Is Nothing Then
                Inicializa_documentos_seleccion_workflow = "Imposible encontrar el control (UpdatePanelseleccion)"
                Exit Function
            End If

            Dim Sql_consulta As String = ""
            Sql_consulta = HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_CAMPOS_CHAECHE") & " FROM " & HttpContext.Current.Session.Item("WF_GABINETE_SELECCIONADO_CHAECHE") & "  where ENLASE = '" & "----------------*" & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("MAUM")
            Dim Result = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            Label_docu_relacionado_wf.Text = "Documentos (" & 0 & ")"
            Datset.Tables(0).Rows.Add(Datset.Tables(0).NewRow)
            GridView_list_documento_relacion_wf.DataSource = Datset
            hiden_seleccion_documento_wf.Value = "-1"
            hiden_seleccion_documento_id_wf.Value = ""
            Hidden_numero_doc_rel_wf.Value = "0"
            GridView_list_documento_relacion_wf.DataBind()
            GridView_list_documento_relacion_wf.Rows(0).Visible = False
            UpdatePanel_label_seleccion.Update()
            UpdatePanelseleccion.Update()
            Inicializa_documentos_seleccion_workflow = "YES"
            Exit Function
        Catch ex As Exception
            Inicializa_documentos_seleccion_workflow = "Inconsistencia general funcion Inicializa_documentos_seleccion_workflow " & ex.Message
        End Try
    End Function
    Function Actualiza_lista_documentos_relacionados_a_radicado_enlace(ByVal id_registro_estado As Long,
                                                                       ByRef scripma As GridView,
                                                                       ByRef labetitle As Label,
                                                                       ByRef hideselecion As HtmlInputHidden,
                                                                       ByRef updat As UpdatePanel,
                                                                       ByRef update_label As UpdatePanel,
                                                                       ByRef numero_documentos As Integer,
                                                                       ByVal id_tarea As Long) As String
        Try
            Dim Result As String = ""
            Dim stru_registro_estado As stru_registro_estado = Nothing
            Dim Class_ra_rad_estados_modulo_radicacion As New Class_ra_rad_estados_modulo_radicacion
            Result = Class_ra_rad_estados_modulo_radicacion.SolicitaDatosEstructuraEstadoRadicado(id_registro_estado,
                                                                                                     stru_registro_estado)
            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_radicado_enlace = Result
                Exit Function
            End If
            Dim Ref_class_system1 As New Class_system1
            Dim inventario_documental As Integer
            Dim aplica_trd As Integer
            Dim asigna_unidad As Integer
            Result = Ref_class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_radicado_enlace = Result
                Exit Function
            End If
            Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
            Dim nombre_campo_radicado_gabinete As String = ""
            Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                nombre_campo_radicado_gabinete)
            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_radicado_enlace = Result
                Exit Function
            End If
            Dim stru_parameter_image() As stru_paramter_image = Nothing
            Result = Me.Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado(nombre_campo_radicado_gabinete,
                                                                                             HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                             stru_registro_estado.consecutivo_radicado,
                                                                                             aplica_trd,
                                                                                             stru_parameter_image)
            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_radicado_enlace = Result
                Exit Function
            End If


            Dim ref_class_da_gabinete As New ClassDaGabinete
            Result = ref_class_da_gabinete.Lista_documentos_relacionados_a_radicado_enlace(nombre_campo_radicado_gabinete,
                                                                                          HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                          stru_registro_estado.consecutivo_radicado,
                                                                                          aplica_trd,
                                                                                          id_tarea,
                                                                                          0,
                                                                                          scripma,
                                                                                          labetitle,
                                                                                          hideselecion,
                                                                                          updat,
                                                                                          update_label,
                                                                                          numero_documentos)

            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_radicado_enlace = Result
                Exit Function
            End If
            Actualiza_lista_documentos_relacionados_a_radicado_enlace = "YES"
            Exit Function
        Catch ex As Exception
            Actualiza_lista_documentos_relacionados_a_radicado_enlace = "Inconissistencia general funcion Actualiza_lista_documentos_relacionados_a_radicado_enlace " & ex.Message
        End Try
    End Function
    Function Actualiza_lista_documentos_relacionados_a_enlace(ByRef scripma As GridView,
                                                              ByRef labetitle As Label,
                                                              ByRef hideselecion As HtmlInputHidden,
                                                              ByRef updat As UpdatePanel,
                                                              ByRef update_label As UpdatePanel,
                                                              ByRef numero_documentos As Integer, ByVal id_tarea As Long) As String
        Try
            Dim Result As String = ""
            Dim Ref_class_system1 As New Class_system1
            Dim inventario_documental As Integer
            Dim aplica_trd As Integer
            Dim asigna_unidad As Integer
            Result = Ref_class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_enlace = Result
                Exit Function
            End If
            Dim Ref_class_detalle_gabinete As New Class_DETALLE_GABIENETE
            Dim nombre_campo_radicado_gabinete As String = ""
            Result = Ref_class_detalle_gabinete.SolicitaNombreCampoRadicadoGabinete(HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                nombre_campo_radicado_gabinete)
            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_enlace = Result
                Exit Function
            End If
            Dim stru_parameter_image() As stru_paramter_image = Nothing
            Result = Me.Solicita_lista_parametros_documentos_gabinete_relacionado_a_radicado(nombre_campo_radicado_gabinete,
                                                                                             HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                             HttpContext.Current.Session("DG_RADICADO"),
                                                                                             aplica_trd,
                                                                                             stru_parameter_image)
            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_enlace = Result
                Exit Function
            End If


            Dim ref_class_da_gabinete As New ClassDaGabinete
            Result = ref_class_da_gabinete.Lista_documentos_relacionados_a_radicado_enlace(nombre_campo_radicado_gabinete,
                                                                                         HttpContext.Current.Session.Item("DG_NOMBRE_GABINETE"),
                                                                                         HttpContext.Current.Session("DG_RADICADO"),
                                                                                         aplica_trd,
                                                                                         id_tarea,
                                                                                         0,
                                                                                         scripma,
                                                                                         labetitle,
                                                                                         hideselecion,
                                                                                         updat,
                                                                                         update_label,
                                                                                         numero_documentos)

            If Result <> "YES" Then
                Actualiza_lista_documentos_relacionados_a_enlace = Result
                Exit Function
            End If
            Actualiza_lista_documentos_relacionados_a_enlace = "YES"
            Exit Function
        Catch ex As Exception
            Actualiza_lista_documentos_relacionados_a_enlace = "Inconissistencia general funcion Actualiza_lista_documentos_relacionados_a_enlace " & ex.Message
        End Try
    End Function
    Function RemoveDiacritics(ByVal text As String,
                              ByRef return_ As String) As String
        Try
            Dim normalizedString = text.Normalize(NormalizationForm.FormD)
            Dim stringBuilder = New StringBuilder()
            For Each c In normalizedString
                Dim unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
                If unicodeCategory <> UnicodeCategory.NonSpacingMark Then
                    stringBuilder.Append(c)
                End If
            Next
            return_ = stringBuilder.ToString().Normalize(NormalizationForm.FormD)
            RemoveDiacritics = "YES"
        Catch ex As Exception
            RemoveDiacritics = "Inconsistencia funcion RemoveDiacritics " & ex.Message
        End Try

    End Function
    Function SolicitaIconoImageFownt(ByVal FileExtensionDocuarchi As String,
                                     ByRef IconoFontAwsome As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el icno awsome para la representación de la imagem
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'FileExtensionDocuarchi : Representa la identificación del tipo de archivo Docuarchi
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IconoFont  : Retorna la identificación del icono awasome
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Select Case FileExtensionDocuarchi
                Case "-1"
                    IconoFontAwsome = "fal fa-file"
                Case "-10"
                    IconoFontAwsome = "fal fa-file"
                Case "-11"
                    IconoFontAwsome = "fal fa-file"
                Case "-3"
                    IconoFontAwsome = "fal fa-file"
                Case "-30"
                    IconoFontAwsome = "fal fa-file"
                Case "-33"
                    IconoFontAwsome = "fal fa-file"
                Case "-4"
                    IconoFontAwsome = "fal fa-file"
                Case "-40"
                    IconoFontAwsome = "fal fa-file"
                Case "-44"
                    IconoFontAwsome = "fal fa-file"
                Case "-2"
                    IconoFontAwsome = "fal fa-file-pdf"
                Case "-20"
                    IconoFontAwsome = "fal fa-file-pdf"
                Case "-22"
                    IconoFontAwsome = "fal fa-file-pdf"
                Case "-5"
                    IconoFontAwsome = "fal fa-file-word"
                Case "-50"
                    IconoFontAwsome = "fal fa-file-word"
                Case "-55"
                    IconoFontAwsome = "fal fa-file-word"
                Case "-51"
                    IconoFontAwsome = "fal fa-file-word"
                Case "-510"
                    IconoFontAwsome = "fal fa-file-word"
                Case "-561"
                    IconoFontAwsome = "fal fa-file-word"
                Case "-52"
                    IconoFontAwsome = "fal fa-file-excel"
                Case "-520"
                    IconoFontAwsome = "fal fa-file-excel"
                Case "-572"
                    IconoFontAwsome = "fal fa-file-excel"
                Case "-53"
                    IconoFontAwsome = "fal fa-file-excel"
                Case "-530"
                    IconoFontAwsome = "fal fa-file-excel"
                Case "-583"
                    IconoFontAwsome = "fal fa-file-excel"
                Case "-54"
                    IconoFontAwsome = "fal fa-file-powerpoint"
                Case "-540"
                    IconoFontAwsome = "fal fa-file-powerpoint"
                Case "-594"
                    IconoFontAwsome = "fal fa-file-powerpoint"
                Case "-15"
                    IconoFontAwsome = "fal fa-file-powerpoint"
                Case "-550"
                    IconoFontAwsome = "fal fa-file-powerpoint"
                Case "-605"
                    IconoFontAwsome = "fal fa-file-powerpoint"
                Case Else
                    IconoFontAwsome = "fal fa-file-exclamation"
            End Select
            SolicitaIconoImageFownt = "YES"
        Catch ex As Exception
            SolicitaIconoImageFownt = "Inconsistencia general función SolicitaIconoImageFownt " & ex.Message
        End Try
    End Function
    Function Agrega_icono_image_fownt_java(ByVal file_extension As String,
                                           ByRef icono_font As String) As String
        Try
            Select Case file_extension
                Case "-1"
                    icono_font = "fa-file"
                Case "-10"
                    icono_font = "fa-file"
                Case "-11"
                    icono_font = "fa-file"
                Case "-3"
                    icono_font = "fa-file"
                Case "-30"
                    icono_font = "fa-file"
                Case "-33"
                    icono_font = "fa-file"
                Case "-4"
                    icono_font = "fa-file"
                Case "-40"
                    icono_font = "fa-file"
                Case "-44"
                    icono_font = "fa-file"
                Case "-2"
                    icono_font = "fa-file-pdf"
                Case "-20"
                    icono_font = "fa-file-pdf"
                Case "-22"
                    icono_font = "fa-file-pdf"
                Case "-5"
                    icono_font = "fa-file-word"
                Case "-50"
                    icono_font = "fa-file-word"
                Case "-55"
                    icono_font = "fa-file-word"
                Case "-51"
                    icono_font = "fa-file-word"
                Case "-510"
                    icono_font = "fa-file-word"
                Case "-561"
                    icono_font = "fa-file-word"
                Case "-52"
                    icono_font = "fa-file-excel"
                Case "-520"
                    icono_font = "fa-file-excel"
                Case "-572"
                    icono_font = "fa-file-excel"
                Case "-53"
                    icono_font = "fa-file-excel"
                Case "-530"
                    icono_font = "fa-file-excel"
                Case "-583"
                    icono_font = "fa-file-excel"
                Case "-54"
                    icono_font = "fa-file-powerpoint"
                Case "-540"
                    icono_font = "fa-file-powerpoint"
                Case "-594"
                    icono_font = "fal fa-file-powerpoint"
                Case "-15"
                    icono_font = "fal fa-file-powerpoint"
                Case "-550"
                    icono_font = "fa-file-powerpoint"
                Case "-605"
                    icono_font = "fa-file-powerpoint"
                Case Else
                    icono_font = "fa-file-exclamation"
            End Select
            Agrega_icono_image_fownt_java = "YES"
        Catch ex As Exception
            Agrega_icono_image_fownt_java = "Inconsistencia general función Agrega_icono_image_fownt_java " & ex.Message
        End Try
    End Function
    Function Agrega_icono_image_fownt_extension(ByVal file_extension As String,
                                                ByRef icono_font As String) As String
        Try
            Select Case file_extension
                Case ".TIF"
                    icono_font = "fal fa-file"
                Case ".JPG"
                    icono_font = "fal fa-file"
                Case ".BMP"
                    icono_font = "fal fa-file"
                Case ".PDF"
                    icono_font = "fal fa-file-pdf"
                Case ".DOC"
                    icono_font = "fal fa-file-word"
                Case ".DOCX"
                    icono_font = "fal fa-file-word"
                Case ".XLS"
                    icono_font = "fal fa-file-excel"
                Case ".XLSX"
                    icono_font = "fal fa-file-excel"
                Case ".PPT"
                    icono_font = "fal fa-file-powerpoint"
                Case ".PPTX"
                    icono_font = "fal fa-file-powerpoint"
                Case "-594"
                    icono_font = "fal fa-file-powerpoint"
                Case Else
                    icono_font = "fal fa-file-exclamation"
            End Select
            Agrega_icono_image_fownt_extension = "YES"
        Catch ex As Exception
            Agrega_icono_image_fownt_extension = "Inconsistencia general función Agrega_icono_image_fownt " & ex.Message
        End Try
    End Function
    Function Agrega_icono_image_fownt_extension_cort(ByVal file_extension As String,
                                                     ByRef icono_font As String) As String
        Try
            Select Case file_extension
                Case ".TIF"
                    icono_font = "fa-file"
                Case ".JPG"
                    icono_font = "fa-file"
                Case ".BMP"
                    icono_font = "fa-file"
                Case ".PDF"
                    icono_font = "fa-file-pdf"
                Case ".DOC"
                    icono_font = "fa-file-word"
                Case ".DOCX"
                    icono_font = "fa-file-word"
                Case ".XLS"
                    icono_font = "fa-file-excel"
                Case ".XLSX"
                    icono_font = "fa-file-excel"
                Case ".PPT"
                    icono_font = "fa-file-powerpoint"
                Case ".PPTX"
                    icono_font = "fa-file-powerpoint"
                Case "-594"
                    icono_font = "fa-file-powerpoint"
                Case Else
                    icono_font = "fa-file-exclamation"
            End Select
            Agrega_icono_image_fownt_extension_cort = "YES"
        Catch ex As Exception
            Agrega_icono_image_fownt_extension_cort = "Inconsistencia general función Agrega_icono_image_fownt_extension_cort " & ex.Message
        End Try
    End Function
    Function Solicita_url_documento_soporte_documental_rad_simple(ByVal id_imagen As Integer,
                                                                  ByVal gabinete As String,
                                                                  ByVal id_tarea_workflow As Long,
                                                                  ByVal rad_dicado As String,
                                                                  ByRef url As String,
                                                                  ByRef Tipo_documento As String) As String
        '------------------------------------------------------------------------------------
        'Funcion : Solicita la url de vizualización del archivo a visualizar 
        '          para el modulo de radicación simple
        '
        '         
        '------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '------------------------------------------------------------------------------------
        'id_imagen                    : Representa la idneitifcación de la imagen
        'gabinete                     : Rpresenta el nombre del gabinete                             
        'id_tarea_workflow            : Representa la identificación de la tarea
        'rad_dicado                   : Representa el radicado
        '------------------------------------------------------------------------------------
        '                           RETORNO
        '------------------------------------------------------------------------------------
        'url                          : Retorna la url de visualización
        '------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-01
        'Elabora               : Miguel Angel Urueta Miranda
        '-------------------------------------------------------------------------------------
        Try
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Dim id_tipo_imagen As Integer = 0
            Dim extension_imagen As String = ""
            Dim Class_system1 As New Class_system1
            Dim option_trd As Integer = 0
            Dim option_inventario As Integer = 0
            Dim option_unidad As Integer = 0
            Dim Result As String = ""
            Result = Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(gabinete,
                                                                                                     option_inventario,
                                                                                                     option_trd,
                                                                                                     option_unidad)
            If Result <> "YES" Then
                Solicita_url_documento_soporte_documental_rad_simple = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(gabinete,
                                                                     id_imagen,
                                                                     stru_paramter_image,
                                                                     option_trd)

            If Result <> "YES" Then
                Solicita_url_documento_soporte_documental_rad_simple = Result
                Exit Function
            End If
            Tipo_documento = stru_paramter_image.TIPODOCUMENTO
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                                 extension_imagen)
            If Result <> "YES" Then
                Solicita_url_documento_soporte_documental_rad_simple = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DA_IMAGEN") = id_imagen
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = gabinete
            HttpContext.Current.Session.Item("WF_MATRI_IMAGE_EMERGENTE") = ""
            If extension_imagen = ".TIF" Or extension_imagen = ".JPG" Or extension_imagen = ".BMP" Then
                url = "../Docuarchi/WebFormDaVisorDocuarchi.aspx"
                Class_logdocuarchi.Registra_log_procesing_image(id_imagen, gabinete, "WORKFLOW", "Visualiza", id_tarea_workflow, rad_dicado, "")
                Solicita_url_documento_soporte_documental_rad_simple = "YES"
                Exit Function
            Else
                url = "../Docuarchi/WebFormDaVisorExterno.aspx"
                Class_logdocuarchi.Registra_log_procesing_image(id_imagen, gabinete, "WORKFLOW", "Visualiza", id_tarea_workflow, rad_dicado, "")
                Solicita_url_documento_soporte_documental_rad_simple = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_url_documento_soporte_documental_rad_simple = "Inconsistencia general funcion Solicita_url_documento_soporte_documental_rad_simple " & ex.Message
        End Try
    End Function
    Function Prevent_visualiza_documento_seleccion_envio_radicado(ByVal id_seleccion As String,
                                                                  ByVal id_tarea_workflow As Integer,
                                                                  ByRef IframeVisor As Object,
                                                                  ByRef UpdatePanelIframevisor As UpdatePanel) As String

        Try
            Dim Result As String = ""
            If id_seleccion = "" Then
                Prevent_visualiza_documento_seleccion_envio_radicado = "Por favor seleccione el documento para visualizar"
                Exit Function
            End If
            Dim Class_logdocuarchi As New Class_logdocuarchi
            Dim ValueItem As String = id_seleccion
            Dim spligabi() As String = ValueItem.Split("|")
            Dim id_tipo_imagen As Integer = 0
            Dim extension_imagen As String = ""
            Dim Refclas As New ClassDaGabinete
            Result = Refclas.SolicitaIdTipoImagen(spligabi(1),
                                                    spligabi(0),
                                                    id_tipo_imagen)
            If Result <> "YES" Then
                Prevent_visualiza_documento_seleccion_envio_radicado = Result
                Exit Function
            End If
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(id_tipo_imagen,
                                                                                 extension_imagen)
            If Result <> "YES" Then
                Prevent_visualiza_documento_seleccion_envio_radicado = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DA_IMAGEN") = spligabi(1)
            HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA") = spligabi(0)
            Dim refcla As New ClassWorflowVisor
            Dim Resutl As String = ""
            HttpContext.Current.Session.Item("WF_MATRI_IMAGE_EMERGENTE") = ""
            If extension_imagen = ".TIF" Or extension_imagen = ".JPG" Or extension_imagen = ".BMP" Then
                IframeVisor.Attributes("src") = "../Docuarchi/WebFormDaVisorDocuarchi.aspx"
                IframeVisor.Attributes("Width") = "100%"
                IframeVisor.Attributes("Heith") = "100%"
                UpdatePanelIframevisor.Update()
                Class_logdocuarchi.Registra_log_procesing_image(Val(spligabi(1)), spligabi(0), "WORKFLOW", "Visualiza", id_tarea_workflow, "", "")
                Prevent_visualiza_documento_seleccion_envio_radicado = "YES"
                Exit Function
            Else
                IframeVisor.Attributes("src") = "../Docuarchi/WebFormDaVisorExterno.aspx"
                IframeVisor.Attributes("Width") = "100%"
                IframeVisor.Attributes("Heith") = "100%"
                UpdatePanelIframevisor.Update()
                Class_logdocuarchi.Registra_log_procesing_image(Val(spligabi(1)), spligabi(0), "WORKFLOW", "Visualiza", id_tarea_workflow, "", "")
                Prevent_visualiza_documento_seleccion_envio_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Prevent_visualiza_documento_seleccion_envio_radicado = "Inconsistencia general Prevent_visualiza_documento_seleccion_envio_radicado " & ex.Message
        End Try
    End Function
    Function Generando_Matriz_Imagenes_Sleccionadas(ByRef Matri_Doc_Visual() As String,
                                                    ByVal text_rangos As String,
                                                    ByRef Matri_Selec_Pag() As String) As String
        '*******************************************************
        'Funcion : Generando_Matriz_Imagenes_Sleccionadas
        'Fecha : 2011-04-14
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Genera matriz documentos seleccionados
        'para realizar las transformaciones
        '*******************************************************
        Try
            '****************************************
            'Seleccion pagina
            '****************************************

            '*********************************
            'Validacion text vacio
            '*********************************
            If text_rangos = "" Then
                Generando_Matriz_Imagenes_Sleccionadas = "Debe digitar los rangos de pagina "
                Exit Function
            End If

            '*********************************
            'Rango paginas
            '*********************************
            If InStr(text_rangos, "-") <> 0 Then
                Dim splitRnago() As String = text_rangos.Split("-")
                '*****************************************
                'Determina que los rangos no esten vacios
                '*****************************************
                If splitRnago(0) = "" Or splitRnago(1) = "" Then
                    Generando_Matriz_Imagenes_Sleccionadas = "Rango de Seleccion Vacio "
                    Exit Function
                End If
                '*****************************************
                'Determina que el rango incial no se menor 
                'o igual al rango final
                '*****************************************
                If Val(splitRnago(0)) >= Val(splitRnago(1)) Then
                    Generando_Matriz_Imagenes_Sleccionadas = "El rango incial no puede ser igual o mayor al rango final "
                    Exit Function
                End If
                '*****************************************
                'Determina que el rango inicial  sea mayor
                'que cero
                '*****************************************
                If Val(splitRnago(1)) = 0 Then
                    Generando_Matriz_Imagenes_Sleccionadas = "El rango incial no puede ser igual a cero "
                    Exit Function
                End If
                '*****************************************
                'Determina que el rango final no sea mayor
                'que el tamaño de la matriz
                '*****************************************
                If Val(splitRnago(1)) > Matri_Doc_Visual.Length - 1 Then
                    splitRnago(1) = Matri_Doc_Visual.Length
                End If
                '******************************************
                'Genera el rango de numero paginas
                '******************************************
                Erase Matri_Selec_Pag
                'Dim Contador_Rango As Integer = 0
                'Dim Rango_Inicial As Integer = Val(splitRnago(0))
                'Dim Rango_Final As Integer = Val(splitRnago(1))
                'While Rango_Inicial <= Rango_Final
                '    ReDim Preserve Matri_Selec_Pag(Contador_Rango)
                '    Matri_Selec_Pag(Contador_Rango) = Rango_Inicial - 1
                '    Rango_Inicial = Rango_Inicial + 1
                '    Contador_Rango = Contador_Rango + 1
                'End While
                Dim i2 As Integer = 0
                For i As Integer = Val(splitRnago(0)) To Val(splitRnago(1))
                    ReDim Preserve Matri_Selec_Pag(i2)
                    Matri_Selec_Pag(i2) = Matri_Doc_Visual(i - 1)
                    i2 = i2 + 1
                Next
            Else

            End If
            '****************************************
            'Genera matriz paginas contiguas
            '****************************************
            If InStr(text_rangos, ",") <> 0 Then
                Dim SplitMatri() As String = text_rangos.Split(",")
                '********************************************
                'Determinar elemento matriz vacio
                '********************************************
                For i As Integer = 0 To UBound(SplitMatri)
                    If SplitMatri(i) = "" Then
                        Generando_Matriz_Imagenes_Sleccionadas = "No pueden haber comas continuas o  el final de la selección no puede terminar en coma Ejemplo 1(,,)2,3,4 o 1,3(,)"
                        Exit Function
                    End If
                    If SplitMatri(i) = "0" Then
                        Generando_Matriz_Imagenes_Sleccionadas = "El valor cero (0) no es permitido para rangos "
                        Exit Function
                    End If
                Next
                '*********************************************
                'Determina que no este un numero mayor que la
                'matriz en la coleccion
                '*********************************************
                For i As Integer = 0 To UBound(SplitMatri)
                    If Val(SplitMatri(i)) > Matri_Doc_Visual.Length Then
                        Generando_Matriz_Imagenes_Sleccionadas = "El numero " & SplitMatri(i) & " Es mayor que numero de documentos a convertir que es " & Matri_Doc_Visual.Length - 1
                        Exit Function
                    End If
                Next
                '*********************************************
                'Determina que no esten numeros repetidos
                '*********************************************
                Dim Marcador As Integer = 0
                For i As Integer = 0 To UBound(SplitMatri)
                    For z As Integer = 0 To UBound(SplitMatri)
                        If SplitMatri(i) = SplitMatri(z) Then
                            Marcador = Marcador + 1
                            If Marcador > 1 Then
                                Generando_Matriz_Imagenes_Sleccionadas = "El numero " & SplitMatri(i) & " se encuentra repetido en la seleccion "
                                Exit Function
                            End If
                        End If

                    Next
                    Marcador = 0
                Next
                '*****************************************
                'Genera la matriz de documentos
                '*****************************************
                'Erase Matri_Selec_Pag
                'For zi As Integer = 0 To UBound(SplitMatri)
                '    ReDim Preserve Matri_Selec_Pag(zi)
                '    Matri_Selec_Pag(zi) = SplitMatri(zi)
                'Next
                Dim i2 As Integer = 0
                For i As Integer = 0 To UBound(SplitMatri)
                    ReDim Preserve Matri_Selec_Pag(i2)
                    Matri_Selec_Pag(i2) = Matri_Doc_Visual(Val(SplitMatri(i)) - 1)
                    i2 = i2 + 1
                Next
            End If
            If InStr(text_rangos, ",") = 0 And
               InStr(text_rangos, "-") = 0 Then
                If Val(text_rangos) > Matri_Doc_Visual.Length Then
                    Erase Matri_Selec_Pag
                    ReDim Preserve Matri_Selec_Pag(0)
                    Matri_Selec_Pag(0) = Matri_Doc_Visual((Matri_Doc_Visual.Length - 1))
                Else
                    Erase Matri_Selec_Pag
                    ReDim Preserve Matri_Selec_Pag(0)
                    Matri_Selec_Pag(0) = Matri_Doc_Visual(Val(text_rangos) - 1)
                End If

            End If
            '********************************************
            'Determina que no este ningun iten en ceros
            '********************************************
            If Not Matri_Selec_Pag Is Nothing Then
                For kz As Integer = 1 To UBound(Matri_Selec_Pag)
                    If Matri_Selec_Pag(kz) = "0" Then
                        Generando_Matriz_Imagenes_Sleccionadas = "En la seleccion no puede haber ceros (0)"
                        Exit Function

                    End If

                Next
            End If

            Generando_Matriz_Imagenes_Sleccionadas = "YES"
        Catch ex As Exception
            Generando_Matriz_Imagenes_Sleccionadas = "Error General Funcion Generando_Matriz_Imagenes_Sleccionadas Cod_Error : " & ex.Message
        End Try
    End Function
    Function SolicitaListaGabinetesPermitidos(ByVal IdGrupoDocuarchi As Integer,
                                              ByVal IdUsuarioDocuarchi As Integer,
                                              ByRef CDGabinetesPermitidos As List(Of CDGabinetesPermitidos)) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la lista de gabientes per mitidos por usuario 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_script           : Representa la identificación del script de validación
        'campo_radicacion    : Representa el nombre del campo de radicación destino
        'id_plantilla_radicacion : 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_usuario_radicador  : Retorna la identificación del usuario radicador
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim UsuarioCDGabinetesPermitidos As New List(Of CDGabinetesPermitidos)
            Dim GrupoCDGabinetesPermitidos As New List(Of CDGabinetesPermitidos)
            Dim class_permisos_grupo_gabinete As New Class_permisos_grupos_gabinetes
            Dim clas_permisos_usarios_gabinete As New Class_permisos_usuarios_gabinetes
            Result = class_permisos_grupo_gabinete.SolicitaGabinetesPermitidosGrupo(IdGrupoDocuarchi,
                                                                                    GrupoCDGabinetesPermitidos)
            If Result <> "YES" Then
                Return Result
            End If
            Result = clas_permisos_usarios_gabinete.SolicitagabinetesPermitidosUsuario(IdUsuarioDocuarchi,
                                                                                       UsuarioCDGabinetesPermitidos)
            If Result <> "YES" Then
                Return Result
            End If
            If GrupoCDGabinetesPermitidos Is Nothing Then
                GrupoCDGabinetesPermitidos = New List(Of CDGabinetesPermitidos)
            End If
            If UsuarioCDGabinetesPermitidos Is Nothing Then
                UsuarioCDGabinetesPermitidos = New List(Of CDGabinetesPermitidos)
            End If
            CDGabinetesPermitidos = GrupoCDGabinetesPermitidos.Concat(UsuarioCDGabinetesPermitidos).
                                    GroupBy(Function(g) g.IdGabinete).
                                    Select(Function(g) g.First()).
                                    OrderBy(Function(g) g.NombreGabinete).
                                   ToList()
            Return "YES"
        Catch ex As Exception
            Return "Inconsistencia general funcion SolicitaListaGabinetesPermitidos " & ex.Message
        End Try
    End Function
    Function Retorna_gabinetes_permitidos(ByVal Id_grupo As Integer,
                                          ByVal Id_usuario As Integer,
                                          ByRef refdrowlist As DropDownList) As String
        '--------------------------------------------------------------
        'Funcion : Solicita todos los gabinetes permitidos
        '
        'Fecha : 2015-09-09
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Refclas_permisos_usarios_gabinete As New Class_permisos_usuarios_gabinetes
            Dim Ref_class_permisos_grupo_gabinete As New Class_permisos_grupos_gabinetes
            Dim Result As String = ""
            Dim matri_gabinetes_grupo() As String
            Erase matri_gabinetes_grupo
            Dim matri_gabinetes_usuario() As String
            Erase matri_gabinetes_usuario
            Dim matri_general() As String
            Erase matri_general
            refdrowlist.Items.Clear()
            If Id_grupo <> 0 Then
                Result = Ref_class_permisos_grupo_gabinete.SolicitaGabinetesPermitidosGrupo(Id_grupo,
                                                                                               matri_gabinetes_grupo)
                If Result <> "YES" Then
                    Retorna_gabinetes_permitidos = Result
                    Exit Function
                End If
            End If
            If Id_usuario <> 0 Then
                Result = Refclas_permisos_usarios_gabinete.SolicitagabinetesPermitidosUsuario(Id_usuario,
                                                                                                matri_gabinetes_usuario)
                If Result <> "YES" Then
                    Retorna_gabinetes_permitidos = Result
                    Exit Function
                End If
            End If
            Dim icontador As Integer = 0
            If Not matri_gabinetes_grupo Is Nothing Then
                For i As Integer = 0 To matri_gabinetes_grupo.Length - 1
                    ReDim Preserve matri_general(icontador)
                    matri_general(icontador) = matri_gabinetes_grupo(i)
                    icontador = icontador + 1
                Next
            End If
            If Not matri_gabinetes_usuario Is Nothing Then
                For i As Integer = 0 To matri_gabinetes_usuario.Length - 1
                    '-------------------------------------------
                    'Verifica la existencia del gabinete
                    '-------------------------------------------
                    If Not matri_gabinetes_grupo Is Nothing Then
                        Dim estado_exist As String = ""
                        For z As Integer = 0 To matri_gabinetes_grupo.Length - 1
                            If UCase(matri_gabinetes_usuario(i)) = UCase(matri_gabinetes_grupo(z)) Then
                                estado_exist = "YES"
                                Exit For
                            End If
                        Next
                        If estado_exist = "" Then
                            ReDim Preserve matri_general(icontador)
                            matri_general(icontador) = matri_gabinetes_usuario(i)
                            icontador = icontador + 1
                        End If

                    Else
                        ReDim Preserve matri_general(icontador)
                        matri_general(icontador) = matri_gabinetes_usuario(i)
                        icontador = icontador + 1
                    End If
                Next

            End If
            If Not matri_general Is Nothing Then
                For i As Integer = 0 To matri_general.Length - 1
                    refdrowlist.Items.Add(matri_general(i))
                Next
            End If
            Retorna_gabinetes_permitidos = "YES"
        Catch ex As Exception
            Retorna_gabinetes_permitidos = "Inconsistencia general función Retorna_gabinetes_permitidos " & ex.Message
        End Try
    End Function
    Function Retorna_gabinetes_permitidos_almacenamiento(ByVal Id_grupo As Integer,
                                                         ByVal Id_usuario As Integer,
                                                         ByRef refdrowlist As DropDownList) As String
        '--------------------------------------------------------------
        'Funcion : Solicita todos los gabinetes permitidos
        '
        'Fecha : 2015-09-09
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Refclas_permisos_usarios_gabinete As New Class_permisos_usuarios_gabinetes
            Dim Ref_class_permisos_grupo_gabinete As New Class_permisos_grupos_gabinetes
            Dim Result As String = ""
            Dim matri_gabinetes_grupo() As String
            Erase matri_gabinetes_grupo
            Dim matri_gabinetes_usuario() As String
            Erase matri_gabinetes_usuario
            Dim matri_general() As String
            Erase matri_general
            refdrowlist.Items.Clear()
            If Id_grupo <> 0 Then
                Result = Ref_class_permisos_grupo_gabinete.Retorna_gabinetes_permitidos_grupos_almacenaminento(Id_grupo,
                                                                                                               matri_gabinetes_grupo)
                If Result <> "YES" Then
                    Retorna_gabinetes_permitidos_almacenamiento = Result
                    Exit Function
                End If
            End If
            If Id_usuario <> 0 Then
                Result = Refclas_permisos_usarios_gabinete.Retorna_gabinetes_permitidos_usuario_almacenamiento(Id_usuario,
                                                                                                matri_gabinetes_usuario)
                If Result <> "YES" Then
                    Retorna_gabinetes_permitidos_almacenamiento = Result
                    Exit Function
                End If
            End If
            Dim icontador As Integer = 0
            If Not matri_gabinetes_grupo Is Nothing Then
                For i As Integer = 0 To matri_gabinetes_grupo.Length - 1
                    ReDim Preserve matri_general(icontador)
                    matri_general(icontador) = matri_gabinetes_grupo(i)
                    icontador = icontador + 1
                Next
            End If
            If Not matri_gabinetes_usuario Is Nothing Then
                For i As Integer = 0 To matri_gabinetes_usuario.Length - 1
                    '-------------------------------------------
                    'Verifica la existencia del gabinete
                    '-------------------------------------------
                    If Not matri_gabinetes_grupo Is Nothing Then
                        Dim estado_exist As String = ""
                        For z As Integer = 0 To matri_gabinetes_grupo.Length - 1
                            If UCase(matri_gabinetes_usuario(i)) = UCase(matri_gabinetes_grupo(z)) Then
                                estado_exist = "YES"
                                Exit For
                            End If
                        Next
                        If estado_exist = "" Then
                            ReDim Preserve matri_general(icontador)
                            matri_general(icontador) = matri_gabinetes_usuario(i)
                            icontador = icontador + 1
                        End If

                    Else
                        ReDim Preserve matri_general(icontador)
                        matri_general(icontador) = matri_gabinetes_usuario(i)
                        icontador = icontador + 1
                    End If
                Next

            End If
            If Not matri_general Is Nothing Then
                For i As Integer = 0 To matri_general.Length - 1
                    refdrowlist.Items.Add(matri_general(i))
                Next
            End If
            Retorna_gabinetes_permitidos_almacenamiento = "YES"
        Catch ex As Exception
            Retorna_gabinetes_permitidos_almacenamiento = "Inconsistencia general función Retorna_gabinetes_permitidos_almacenamiento " & ex.Message
        End Try
    End Function
    Function SolicitaIdTipoImagen(ByVal IdImagenGabinete As Integer,
                                  ByVal NombreGabinete As String,
                                  ByRef IdTipoImagen As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita tipo de imagen de gabinete con los parametros nombre de gabinete y identifi
        '          cación de imagen
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdImagenGabinete    : Representa la identificación del gabinete
        'NombreGabinete      : Representa el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdTipoImagen        : Retorna e tipo de imagen
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-08-28
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SqlConsulta As String = "select  dbt  from   " & NombreGabinete &
                   " where ID=" & IdImagenGabinete
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(NombreGabinete)
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(SqlConsulta, Datset)
            If Result <> "YES" Then
                Return " La función SolicitaIdTipoImagen dice : " & Result
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Return "Imposible encontrar el id de la imagen " & IdImagenGabinete & " en el gabinete " & NombreGabinete
            Else
                IdTipoImagen = Datset.Tables(0).Rows(0).Item(0)
                Return "YES"
            End If
        Catch ex As Exception
            Return "Inconsistencia función  SolicitaIdTipoImagen " & ex.Message
        End Try
    End Function
    Function Solicita_extension_documento_docuarchi_segun_id_tipo_archivo(ByVal tipo_archivo As Integer,
                                                                          ByRef extension As String) As String
        Try
            Dim Parametro_Consulta = "select ESTENSION " &
            " from da_extension Where ESTADO_NORMAL=" & tipo_archivo & " or ESTADO_ADJUNTO=" & tipo_archivo &
            " or ESTADO_LINK=" & tipo_archivo
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("da_extension")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_extension_documento_docuarchi_segun_id_tipo_archivo = "Funcion  Solicita_extension_documento_docuarchi_segun_id_tipo_archivo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_extension_documento_docuarchi_segun_id_tipo_archivo = "Imposible encontrar la extensión del archivo según el tipo de documento (" & tipo_archivo & ")"
                Exit Function
            Else
                extension = Datset.Tables(0).Rows(0).Item(0)
                Solicita_extension_documento_docuarchi_segun_id_tipo_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_extension_documento_docuarchi_segun_id_tipo_archivo = "Inconsistencia función Solicita_extension_documento_docuarchi_segun_id_tipo_archivo " & ex.Message
        End Try
    End Function




    Function Asigna_gabinetes_disponibles_interface_droplist(ByVal matri_gabinetes() As String,
                                                             ByRef ref_droplist As DropDownList,
                                                             ByRef ref_update As UpdatePanel) As String
        '-----------------------------------------------------------
        'Función : Agrega a la interface items con los gabinetes
        'disponibles
        'Fecha : 2017-07-19
        'Ing Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            ref_droplist.Items.Clear()
            ref_droplist.Items.Add("")
            If Not matri_gabinetes Is Nothing Then
                For i As Integer = 0 To matri_gabinetes.Length - 1
                    ref_droplist.Items.Add(matri_gabinetes(i))
                Next
            End If
            ref_update.Update()
            Asigna_gabinetes_disponibles_interface_droplist = "YES"
        Catch ex As Exception
            Asigna_gabinetes_disponibles_interface_droplist = "Inconsistencia general función Asigna_gabinetes_disponibles_interface_droplist " & ex.Message
        End Try
    End Function
    Function Lista_datos_configuracion_gabinete_seleccionado(ByVal nombre_gabinete As String,
                                                             ByRef ruta_fisica As String,
                                                             ByRef ruta_busqueda As String,
                                                             ByRef ruta_almacena As String,
                                                             ByRef ref_DropDownList_base_datos_gabinete_agrega As DropDownList,
                                                             ByRef DropDownList_dbms_gabinete_agrega As DropDownList,
                                                             ByRef ref_update As UpdatePanel) As String
        '--------------------------------------------------------------
        'Funcion : Solicita los datos de configuración del gabinete
        '
        'Fecha : 2017-07-20
        'Ingeniero : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------
        ref_DropDownList_base_datos_gabinete_agrega.Items.Clear()
        ref_DropDownList_base_datos_gabinete_agrega.Items.Add("DOCUARCHI")
        DropDownList_dbms_gabinete_agrega.Items.Clear()
        DropDownList_dbms_gabinete_agrega.Items.Add("MYSQL")
        Try
            Dim Parametro_Consulta As String = "select  RUTBUSCA,RUTALMA from  system1 " &
                  " where NOMBRE='" & nombre_gabinete & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("system1")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_datos_configuracion_gabinete_seleccionado = " La funcion Lista_datos_configuracion_gabinete_seleccionado dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_datos_configuracion_gabinete_seleccionado = "Imposible encontrar datos de configuración del gabinete " & nombre_gabinete
                Exit Function
            Else
                ruta_busqueda = Datset.Tables(0).Rows(0).Item(0)
                ruta_busqueda = ruta_busqueda.Replace("/", "\")
                ruta_almacena = Datset.Tables(0).Rows(0).Item(1)
                ruta_almacena = ruta_almacena.Replace("/", "\")
                Lista_datos_configuracion_gabinete_seleccionado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_datos_configuracion_gabinete_seleccionado = "Inconsistencia función Lista_datos_configuracion_gabinete_seleccionado " & ex.Message
        Finally
            ref_update.Update()
        End Try
    End Function

    Function SolicitaPermisosGeneralesGabinete(ByVal NombreGabinete As String,
                                               ByVal IdusuarioLogueado As Integer,
                                               ByVal IdGrupoDocuaerchi As Integer,
                                               ByRef StruPermisoGabinete As stru_permiso_gabinete) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los persmisos general de una gabinete respecto al grupo y al usuario
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete      : Representa el nombre del gabinete
        'IdusuarioLogueado   : Representa la identifcación del usuario docuarchi logueado
        'IdGrupoDocuaerchi   : Representa la identificació del grupo
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'StruPermisoGabinete  : Retorna la estructura con los respectivos permisos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha Modifica        : 2025-08-25
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Class_permisos_usuarios_gabinetes As New Class_permisos_usuarios_gabinetes
            Dim Class_permisos_grupos_gabinetes As New Class_permisos_grupos_gabinetes
            Dim estado_permiso_grupo As String = ""
            If IdGrupoDocuaerchi <> 0 Then
                Result = Class_permisos_grupos_gabinetes.SolicitaPermisosGabineteGrupo(NombreGabinete,
                                                                                       IdGrupoDocuaerchi,
                                                                                       StruPermisoGabinete,
                                                                                       estado_permiso_grupo)
                If Result <> "YES" Then
                    SolicitaPermisosGeneralesGabinete = Result
                    Exit Function

                End If
                If estado_permiso_grupo = "NO" Then
                    Result = Class_permisos_usuarios_gabinetes.SolicitaPermisosGabineteUsuario(NombreGabinete,
                                                                                               IdusuarioLogueado,
                                                                                               StruPermisoGabinete)
                    If Result <> "YES" Then
                        SolicitaPermisosGeneralesGabinete = Result
                        Exit Function
                    End If
                End If
            Else
                Result = Class_permisos_usuarios_gabinetes.SolicitaPermisosGabineteUsuario(NombreGabinete,
                                                                                           IdusuarioLogueado,
                                                                                           StruPermisoGabinete)
                If Result <> "YES" Then
                    SolicitaPermisosGeneralesGabinete = Result
                    Exit Function
                End If
            End If
            SolicitaPermisosGeneralesGabinete = "YES"
        Catch ex As Exception
            SolicitaPermisosGeneralesGabinete = "Inconsistencia general función SolicitaPermisosGeneralesGabinete " & ex.Message
        End Try
    End Function
    Function Retorna_Datos_Auditoria_Gabinete(ByVal Id_imagen As Integer,
                                              ByVal nombre_gabinete As String,
                                              ByRef datos_auditoria As String) As String
        '**********************************************************
        'Funcion : Retorna los datos para el registro de auditoria
        'del sistema con los parametros nombre del gabinete y el
        'id de la imagen 
        'Fecha : 2014-05-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '**********************************************************
        Try
            Dim sqlconsulta As String = "Select * from " & nombre_gabinete & " where id=" & Id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_Datos_Auditoria_Gabinete = " La funcion Retorna_Datos_Auditoria_Gabinete dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Datos_Auditoria_Gabinete = " Retorna_Datos_Auditoria_Gabinete dice Imposible encontrar datos de la imagen"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    Dim valor As String = ""
                    Dim ob As String = Datset.Tables(0).Columns(i).ColumnName
                    If Datset.Tables(0).Rows(0).IsNull(i) = True Then
                        valor = ""
                    Else
                        valor = Datset.Tables(0).Rows(0).Item(i).ToString
                    End If
                    If i = 0 Then
                        datos_auditoria = ob.ToString & "=" & valor
                    Else
                        datos_auditoria = datos_auditoria & "¬" & ob.ToString & "=" & valor
                    End If
                Next
                Retorna_Datos_Auditoria_Gabinete = "YES"
            End If
        Catch ex As Exception
            Retorna_Datos_Auditoria_Gabinete = "Inconsistencia general funcion Retorna_Datos_Auditoria_Gabinete " & ex.Message
        End Try
    End Function
    Function Registra_Auditoria_Eventos(ByVal Nombre_Gabinete As String,
                                        ByVal Rutadocument As String,
                                        ByVal Id_Documento As Integer,
                                        ByVal datos_campo As String,
                                        ByVal TIPO_AUDITORIA As String) As String
        '***********************************************************
        'Funcion : Registra auditoria sobre los eventos realizados 
        'que afecten las imagenes 
        'Fecha : 2014-05-08
        'Ing : Migeuel Angel Urueta Miranda
        '***********************************************************
        Try
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Now
            Dim Result As String = ""
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Registra_Auditoria_Eventos = "Error formatenado fecha  log para el control de auditoria"
                Exit Function
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim hor As New System.DateTime
            hor = Date.Now
            If datos_campo <> "" Then
                datos_campo = datos_campo.Replace("'", "")
            End If
            Dim hora As String = hor.Hour.ToString & ":" & hor.Minute.ToString & ":" & hor.Second.ToString
            Rutadocument = Rutadocument.Replace("\", "/")
            Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
            & "RUT_DOCU,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO) VALUES ( "
            SqlTransac = SqlTransac & "'" & Id_Documento & "',"
            SqlTransac = SqlTransac & "'" & TIPO_AUDITORIA & "',"
            SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("DA_Login_Usuario") & "',"
            SqlTransac = SqlTransac & "'" & date1al & "',"
            SqlTransac = SqlTransac & "'" & Rutadocument & "',"
            SqlTransac = SqlTransac & "'" & Nombre_Gabinete & "',"
            SqlTransac = SqlTransac & "'" & datos_campo & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "')"
            Result = ref.SELECTION_INSERT_COMMAND(SqlTransac)
            If Result <> "YES" Then
                Registra_Auditoria_Eventos = "Imposible registrar el control de auditoria " & Result
                Exit Function
            Else
                Registra_Auditoria_Eventos = "YES"
            End If

        Catch ex As Exception
            Registra_Auditoria_Eventos = "Inconsistencia general registro de auditoria funcion Registra_Auditoria_Eventos " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_imagen_gabinete(ByVal id_imagen As Integer,
                                                 ByVal nombre_gabinete As String,
                                                 ByRef existencia_imagen As String) As String
        Try
            Dim sqlconsulta As String = "Select * from " & nombre_gabinete & " where id=" & id_imagen
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_imagen_gabinete = " La funcion Solicita_existencia_imagen_gabinete dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                existencia_imagen = "NO"
                Solicita_existencia_imagen_gabinete = "YES"
                Exit Function
            Else
                existencia_imagen = "YES"
                Solicita_existencia_imagen_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_imagen_gabinete = "Inconsistencia general función Solicita_existencia_imagen_gabinete " & ex.Message
        End Try
    End Function
    Function Inicializa_interface_exporta_archivo_gabinete(ByVal id_imagen As Integer,
                                                           ByVal nombre_gabinete As String,
                                                           ByVal id_usuario_docuarchi As Integer,
                                                           ByRef ifimpre_post As Object,
                                                           ByRef ModalPopupExtenderimpre_post As AjaxControlToolkit.ModalPopupExtender,
                                                           ByRef UpdatePaneliframe_post As UpdatePanel,
                                                           Optional ByVal option_valida_permisos As Integer = 1) As String
        Try
            Dim stru_permiso As stru_permiso_gabinete
            Dim refgabinete As New ClassDaGabinete
            Dim Result As String = ""
            If option_valida_permisos = 1 Then
                Result = refgabinete.SolicitaPermisosGeneralesGabinete(nombre_gabinete,
                                                                      id_usuario_docuarchi,
                                                                      HttpContext.Current.Session.Item("DA_gruposusu"),
                                                                      stru_permiso)
                If Result <> "YES" Then
                    Inicializa_interface_exporta_archivo_gabinete = Result
                    Exit Function
                End If
                If stru_permiso.EXPORT_IMAGE_FYLESYSTEM = 0 Then
                    Inicializa_interface_exporta_archivo_gabinete = "El usuario no tiene permisos para descargar en el gabinete "
                    Exit Function
                End If
            End If
            If id_imagen = -1 Then
                Inicializa_interface_exporta_archivo_gabinete = "Debe seleccionar el registro o la imagen a descargar "
                Exit Function
            End If
            Dim Refclas As New ClassWorflowVisor
            Dim Matri_Doc_Visual() As String
            Erase Matri_Doc_Visual
            HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL") = ""
            HttpContext.Current.Session.Item("DA_MATRI_IMAGE_EMERGENTE") = ""
            Result = Refclas.Visualizacion_Documentos_da(Matri_Doc_Visual,
                                                         id_imagen,
                                                         nombre_gabinete)
            If Result = "YES" Then
                If Not Matri_Doc_Visual Is Nothing Then
                    For i As Integer = 1 To Matri_Doc_Visual.Length - 1
                        If i = 1 Then
                            HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL") = Matri_Doc_Visual(i)
                        Else
                            HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL") = HttpContext.Current.Session.Item("RA_RUTA_IMPRESION_FINAL") & "," & Matri_Doc_Visual(i)
                        End If

                    Next
                End If
            Else
                Inicializa_interface_exporta_archivo_gabinete = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("DA_IMAGEN") = id_imagen
            HttpContext.Current.Session.Item("DA_GABINETE_IMPRESION") = nombre_gabinete
            HttpContext.Current.Session.Item("DA_ID_IMAGEN_IMPRESION") = id_imagen
            ifimpre_post.Attributes.Add("src", "../Docuarchi/WebFormDaExportArchivo.aspx")
            ModalPopupExtenderimpre_post.Show()
            UpdatePaneliframe_post.Update()
            Inicializa_interface_exporta_archivo_gabinete = "YES"
        Catch ex As Exception
            Inicializa_interface_exporta_archivo_gabinete = "Inconsistencia general función Inicializa_interface_exporta_archivo_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_url_descarga_anexo_respuesta(ByVal id_anexo As Integer,
                                                   ByRef url_anexo_virtual As String,
                                                   ByRef name_file As String) As String
        Try
            Dim Refclas_anexo As New Class_ra_anexos_respuesta
            Dim Ref_clas_gabinete As New ClassDaGabinete
            Dim Result As String = ""
            Dim stru_anex As stru_anexos = Nothing
            Result = Refclas_anexo.Solicita_datos_estructura_anexo(id_anexo,
                                                                   stru_anex)
            If Result <> "YES" Then
                Solicita_url_descarga_anexo_respuesta = Result
                Exit Function
            End If
            Dim Refclas As New ClassWorflowVisor
            Dim Matri_Doc_Visual() As String
            Erase Matri_Doc_Visual
            Result = Refclas.Visualizacion_Documentos_da(Matri_Doc_Visual,
                                                        stru_anex.id_imagen_gabinete,
                                                        stru_anex.nombre_gabinete)
            If Result <> "YES" Then
                Solicita_url_descarga_anexo_respuesta = Result
                Exit Function
            End If
            If Matri_Doc_Visual Is Nothing Then
                Solicita_url_descarga_anexo_respuesta = "Imposible encontrar la matriz de documentos de la imagen (" & stru_anex.id_imagen_gabinete & "), en el gabinete (" & stru_anex.nombre_gabinete & ")"
                Exit Function
            End If
            Dim file_ As New FileInfo(Matri_Doc_Visual(1))
            Dim ruta_virtual As String = "../Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_descarga As String = "/Temp_Image/" & HttpContext.Current.Session.Item("Id_Usuario_Workflow").ToString & "/DESCARGA/"
            Dim ruta_fisica As String = HttpContext.Current.Server.MapPath(ruta_virtual)
            If Directory.Exists(ruta_fisica) = False Then
                Directory.CreateDirectory(ruta_fisica)
            End If
            If UCase(file_.Extension) = ".TIF" Or UCase(file_.Extension) = ".JPJ" Or UCase(file_.Extension) = ".BMP" Then
                Using zip As New ZipFile()
                    For i As Integer = 1 To Matri_Doc_Visual.Length - 1
                        If i = 1 Then
                            zip.AddFile(Matri_Doc_Visual(i), "FilesDocuarchi")
                        Else
                            zip.AddFile(Matri_Doc_Visual(i), "FilesDocuarchi")
                        End If
                    Next
                    Dim zipName As String = [String].Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
                    Dim archivo_salida_zip As String = ruta_fisica & zipName
                    If File.Exists(archivo_salida_zip) Then
                        Kill(archivo_salida_zip)
                    End If
                    zip.Save(archivo_salida_zip)
                    name_file = zipName
                    url_anexo_virtual = HttpContext.Current.Request.Url.Scheme & System.Uri.SchemeDelimiter & HttpContext.Current.Request.Url.Host & HttpContext.Current.Request.ApplicationPath & ruta_descarga & zipName
                    Solicita_url_descarga_anexo_respuesta = "YES"
                    Exit Function
                End Using
            Else
                Dim archivo_dowload As String = ruta_fisica & file_.Name
                If File.Exists(archivo_dowload) Then
                    Kill(archivo_dowload)
                End If
                FileCopy(Matri_Doc_Visual(1), archivo_dowload)
                name_file = file_.Name
                url_anexo_virtual = HttpContext.Current.Request.Url.Scheme & System.Uri.SchemeDelimiter & HttpContext.Current.Request.Url.Host & HttpContext.Current.Request.ApplicationPath & ruta_descarga & file_.Name
                Solicita_url_descarga_anexo_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_url_descarga_anexo_respuesta = "Inconsistencia general funcion Solicita_url_descarga_anexo_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_Datos_Gabinete(ByVal Nombre_Gabinete As String,
                                            ByVal id As Integer,
                                            ByRef Numero_Paginas As Integer,
                                            ByRef Tipo_Doc As Integer,
                                            Optional ByRef disc As Integer = -1,
                                            Optional ByRef idex As Integer = -1) As String
        '*************************************************************
        'Funcion:Solicita_Datos_Gabinete
        'Fecha 2010-10-06
        'Descripcion:Retorna el numero de paginas y el tipo documento
        'quetiene
        'el documento en la base de datos
        'Modificado 2013-08-09 Ingeniero Miguel Angel Urueta Miranda
        'Para cumplir con los requerimientos de la nueva conexion
        'para el sistema web de workflow
        '*************************************************************
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Fecha As String = ""
            Dim Sql_consulta = "select PAG,DBT,DISC,IDEX  from " & Nombre_Gabinete &
                " where id='" & id & "'"
            Dim Datset As DataSet = New DataSet("CONSULTA_GABINETE")
            Dim result As String = ref.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If result <> "YES" Then

                Solicita_Datos_Gabinete = "Error Consultando en tabla  " & Nombre_Gabinete & " " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_Datos_Gabinete = "Imposible encontrar imagen " & id & " en el gabinete " & Nombre_Gabinete
                Exit Function
            Else
                Numero_Paginas = Trim(Datset.Tables(0).Rows(0).Item(0).ToString)
                Tipo_Doc = Trim(Datset.Tables(0).Rows(0).Item(1).ToString)
                disc = Trim(Datset.Tables(0).Rows(0).Item(2).ToString)
                idex = Trim(Datset.Tables(0).Rows(0).Item(3).ToString)
                Solicita_Datos_Gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_Datos_Gabinete = "Inconsistencia general función  Solicita_Datos_Gabinete " & ex.Message
        End Try
    End Function
    Function Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado(ByVal Datos As String,
                                                                              ByRef ID_ALMACEN As Integer,
                                                                              ByVal Tipo_Amacen As Integer,
                                                                              ByRef datos_image As stru_datos_image_lista,
                                                                              ByVal tipo_digitalizacion As String,
                                                                              ByVal id_tarea_workflow As Long,
                                                                              ByVal gabinete_radicado As String,
                                                                              ByVal registro_radicado As String,
                                                                              ByVal evalua_flujo As Integer,
                                                                              Optional tipo_almacenamiento As Integer = 0,
                                                                              Optional tipo_java As Integer = 1) As String
        '-----------------------------------------------------------
        'Funcion : Almacena los documentos digitalizados
        'Fecha : 2014-02-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim RefclasDigitaliza As New ClassWorkflowDigitalizacion
            Dim Matri_Documentos_Final() As String
            Erase Matri_Documentos_Final
            If tipo_almacenamiento = 0 Then
                '--------------------------------------------------------
                'Retorna los documentos almacenados en el file system
                '--------------------------------------------------------
                Result = RefclasDigitaliza.SolicitaMatrizDocumentosDigitalizados(id_tarea_workflow,
                                                                                 HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ESCANER"),
                                                                                 Matri_Documentos_Final)
                If Result <> "YES" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                    Exit Function
                End If

            End If
            If tipo_almacenamiento = 1 Then
                '-------------------------------------------------------
                'Retorma matriz de documentos almacenados adjuntos
                '-------------------------------------------------------
                Dim Refclas_almacenamiento As New ClassAlmacenamiento
                Result = Refclas_almacenamiento.Retorna_matriz_documentos_adjuntos_workflow(Matri_Documentos_Final)
                If Result <> "YES" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                    Exit Function
                End If
            End If
            Dim refclas_ClassNeodynamic As New ClassNeodynamic
            If tipo_almacenamiento = 2 Then
                Dim file As New FileInfo(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                If UCase(file.Extension) = ".TIF" Then
                    Result = refclas_ClassNeodynamic.Extraer_Documento_de_Multitif_fisico(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"),
                                                                                          Matri_Documentos_Final,
                                                                                          HttpContext.Current.Session.Item("WF_RUTA_TEMPO_FINAL") & "\")
                    If Result <> "YES" Then
                        Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "function Almacenamiento_documentos_load_enlace dice (Imposible extraer documento Multi tif " & Result & ")"
                        Exit Function
                    End If
                    If Matri_Documentos_Final Is Nothing Then
                        Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "function Almacenamiento_documentos_load_enlace dice (La matriz de multi tif es nothing) "
                        Exit Function
                    End If
                Else
                    ReDim Preserve Matri_Documentos_Final(0)
                    Matri_Documentos_Final(0) = HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")
                End If
            End If

            '----------------------------------------------------
            'Obtiene los datos de almacenamiento
            '----------------------------------------------------
            Dim Matri_estructura_gabinete() As Datos_Almacenamiento
            Dim Gabinete As String = ""
            Dim Radicado As String = ""
            Dim Campo_Radicado As String = ""
            If Datos <> "" Then
                Dim datos_enlace As String = Trim(Datos)
                If datos_enlace = "" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "Enlace sin datos imposible almacenar"
                    Exit Function
                End If
                If InStr(datos_enlace, "POSITIVOQL_") < 1 Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "Enlace sin datos correctos"
                    Exit Function
                End If
                Dim dat As String = datos_enlace.Replace("POSITIVOQL_", "<!#>")
                Dim Splipositvol() As String = dat.Split("<!#>")
                Dim SpliDATOS() As String = Splipositvol(1).Split("|")
                Dim SpliCAMPOS() As String = Splipositvol(2).Split("|")
                Gabinete = SpliDATOS(1)
                Radicado = SpliDATOS(2)
                Campo_Radicado = SpliCAMPOS(2)
                '---------------------------------------------
                'Remplaza las R000 del numero de radicado
                '---------------------------------------------
                Dim RadicTemporal As String = ""
                RadicTemporal = Radicado.Replace("R", "")
                Radicado = Val(RadicTemporal)
                ReDim Preserve Matri_estructura_gabinete(0)
                Matri_estructura_gabinete(0).nombre_campo = Campo_Radicado
                If Tipo_Amacen = 1 Then
                    Matri_estructura_gabinete(0).valor_campo = Radicado
                Else
                    Matri_estructura_gabinete(0).valor_campo = ""
                End If
                ReDim Preserve Matri_estructura_gabinete(1)
                Matri_estructura_gabinete(1).nombre_campo = "ENLASE"
                Matri_estructura_gabinete(1).valor_campo = Radicado
            Else
                Gabinete = gabinete_radicado
                Radicado = registro_radicado
                ReDim Preserve Matri_estructura_gabinete(0)
                Dim nombre_campo_radicado_gabinete As String = ""
                Dim Ref_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
                Result = Ref_Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(Gabinete,
                                                                                         nombre_campo_radicado_gabinete)
                If Result <> "YES" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                    Exit Function
                End If
                Matri_estructura_gabinete(0).nombre_campo = nombre_campo_radicado_gabinete
                Matri_estructura_gabinete(0).valor_campo = Radicado
                ReDim Preserve Matri_estructura_gabinete(1)
                Matri_estructura_gabinete(1).nombre_campo = "ENLASE"
                Matri_estructura_gabinete(1).valor_campo = Radicado
            End If
            '-----------------------------------------------
            'Retorna nombre ruta tarea
            '-----------------------------------------------
            'Dim Nombre_ruta As String = ""
            Dim Refclas_workflow As New ClassWorkflow
            Dim Ref_class_ruta As New Class_worflow_rutas

            '------------------------------------------------
            'Retorna el nombre de la tabla de radicación
            'si el flujo se genero internamente desde
            'desde el radicador
            '-----------------------------------------------
            Dim Refclasalmacena As New ClassAlmacenamiento
            Dim Nombre_plantilla_radicado As String = ""
            Dim nombre_tipo_documento As String = ""
            Dim id_clase_documento As Integer = 0
            Dim fecha_elaboracion As String = ""
            Dim Refclas_radicado As New ClassRadicador
            Dim reflcas_tipo_documento As New ClassGaTipoDocumental
            Dim Ref_Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
            Result = Ref_Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                              Nombre_plantilla_radicado)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If

            Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                          id_clase_documento)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            Else
                nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
            End If
            Dim date1al As String = Date.Today
            Result = ""
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "Error formateando fecha almacenamiento Funcion: Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado " & Result
                Exit Function
            Else
                fecha_elaboracion = date1al

            End If
            '----------------------------------------------
            'Configura la radicacion tipo tramite
            '----------------------------------------------
            Dim id_tipo_documento As Integer = 0
            Dim id_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Dim descripcion_tipo_documento As String = ""
            Dim nombre_area As String = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim Ref_class_workflow As New ClassWorkflowDigitalizacion
            If HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO") <> -1 Then
                Dim stru As stru_tipo_lista_chequeo = Nothing
                Dim Ref_Class_ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
                Result = Ref_Class_ra_dig_tipos_docum_lista_chequeo.SolicitaDatosTipoDocumentalListaChequeo(HttpContext.Current.Session.Item("DG_LISTA_CHEQUEO"),
                                                                                                                 stru)
                If Result <> "YES" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                    Exit Function
                End If
                If stru.subseries_documentales_Id_SubSeries <> 0 Then
                    id_tipo_documento = stru.tipos_doc_subseries_Id_Tipos_Doc_SubSerie
                Else
                    id_tipo_documento = stru.tipo_doc_series_Id_Tipo_Doc_Series
                End If
                '-----------------------------------------------
                'Retorna serie y sub serie tipo documento
                '-----------------------------------------------
                Dim stru_tipo As stru_tipo_documental = Nothing
                Dim ref_clas_trd As New ClassTrdDocumental
                Result = ref_clas_trd.Solicita_datos_estructura_tipo_documento(id_tipo_documento,
                                                                               stru_tipo)
                If Result <> "YES" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                    Exit Function
                End If
                id_serie = stru_tipo.Series_Documentales_Id_Series
                id_sub_serie = stru_tipo.sub_serie_id_serie
                Dim ref_Class_series_documentales As New Class_series_documentales
                Result = ref_Class_series_documentales.Retorna_id_area_serie_documental(id_serie,
                                                                                        id_area)
                If Result <> "YES" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                    Exit Function
                End If
                Dim ref_Class_ra_tipo_doc_series As New Class_ra_tipo_doc_series
                If id_tipo_documento <> 0 Then
                    Result = ref_Class_ra_tipo_doc_series.Retorna_nombre_tipo_documental(id_serie,
                                                                                         id_sub_serie,
                                                                                         id_tipo_documento,
                                                                                         descripcion_tipo_documento)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                        Exit Function
                    End If
                End If
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                  id_clase_documento)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
                Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
                If id_area <> 0 Then
                    Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area,
                                                                                               nombre_area)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                        Exit Function
                    End If
                End If

                If id_serie <> 0 Then
                    Result = ref_Class_series_documentales.Retorna_nombre_serie_id_serie(id_serie,
                                                                                         nombre_serie)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                        Exit Function
                    End If
                End If
                Dim Class_subseries_documentales As New Class_subseries_documentales
                If id_sub_serie <> 0 Then
                    Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                                    nombre_sub_serie)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                        Exit Function
                    End If
                End If

            End If
            Dim Ref_producion As New ClassGaProducionDocumental
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(Gabinete,
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If
            If aplica_trd = 1 Then
                If nombre_tipo_documento = "" Then
                    Result = reflcas_tipo_documento.SolicitaIdTipoFormatoDocumento("DOCUMENTO DIGITALIZADO",
                                                                                   id_clase_documento)
                    If Result <> "YES" Then
                        Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                        Exit Function
                    Else
                        nombre_tipo_documento = "DOCUMENTO DIGITALIZADO"
                    End If
                End If
            End If
            Dim Refclas_digitalizacion As New Classselecciotarea
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim id_imagen As Long = 0
            If evalua_flujo = 1 Then
                Result = Class_DAT_ADIC_TAR.SolicitaIdImagenRelacionadaTareaworkflowIdRuta(HttpContext.Current.Session.Item("Id_Ruta_Workflow"),
                                                                                           id_tarea_workflow,
                                                                                           id_imagen)
                If Result <> "YES" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                    Exit Function
                End If
            End If
            '-------------------------------------------------------
            'Solicita relación campos radicado plantilla gabinete
            '-------------------------------------------------------
            Dim id_plantilla_radicado As Integer = 0
            Dim Ref_system_plantilla As New Class_system_plantilla_radicado
            Result = Ref_system_plantilla.SolicitaIdPlantillaRadicado(id_plantilla_radicado,
                                                                        Nombre_plantilla_radicado)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If
            Dim id_gabinete As Integer = 0
            Dim ref_system1 As New Class_system1
            Result = Ref_Class_system1.SolicitaIdGabineteDocuarchi(Gabinete,
                                                                  id_gabinete)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If
            Dim stru_campos_plantilla_gabinete() As csfc_structure_relacion_campos_plantilla_ruta = Nothing
            Dim ref_Class_ra_relacion_plantilla_gabinete As New Class_ra_relacion_plantilla_gabinete
            Result = ref_Class_ra_relacion_plantilla_gabinete.SolicitaCamposRelacionPlantillaGabinete(id_plantilla_radicado,
                                                                                                      id_gabinete,
                                                                                                      stru_campos_plantilla_gabinete)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If
            '------------------------------------------------------
            'Asigna los datos a la estructura de relación plantilla
            'gabinete
            '-------------------------------------------------------
            Dim ref_Class_plantillas_radicacion As New Class_plantillas_radicacion
            Result = ref_Class_plantillas_radicacion.AsignaDatosCamposPlantillaRadicadoGabinete(stru_campos_plantilla_gabinete,
                                                                                                Radicado,
                                                                                                Nombre_plantilla_radicado)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'Formatea campos tipo date  y date time
            '--------------------------------------------------------
            Dim refclas_ClassGestionFechas As New ClassGestionFechas
            For i As Integer = 0 To stru_campos_plantilla_gabinete.Length - 1
                If stru_campos_plantilla_gabinete(i).tipo_campo_plantilla = "DATE" Then
                    If Not stru_campos_plantilla_gabinete(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_gabinete(i).dato_campo_plantilla <> "" Then
                        Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Time_MYSQL_Fecha_Inicio(stru_campos_plantilla_gabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                            Exit Function
                        End If
                    End If

                End If
                If stru_campos_plantilla_gabinete(i).tipo_campo_plantilla = "DATETIME" Then
                    If Not stru_campos_plantilla_gabinete(i).dato_campo_plantilla Is Nothing And stru_campos_plantilla_gabinete(i).dato_campo_plantilla <> "" Then
                        Result = refclas_ClassGestionFechas.csfc_Formatea_Fecha_Almacenamiento_Time_bsd(stru_campos_plantilla_gabinete(i).dato_campo_plantilla)
                        If Result <> "YES" Then
                            Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                            Exit Function
                        End If
                    End If

                End If
            Next
            '-------------------------------------------
            'Asigna datos gestion
            '-------------------------------------------
            Dim matri_gestion As estructure_gestion = Nothing     'cambia por EstructuraGestionAlmacenamiento
            '-------------------------------------------------------
            'Asigna valores del expediente a relacionar el documento
            '-------------------------------------------------------
            Dim ClassGaExpediente As New ClassGaExpediente
            Result = ClassGaExpediente.SolicitaEstructuraExpedienteDocumentoVinculante(matri_gestion,
                                                                                        gabinete_radicado,
                                                                                        id_imagen,
                                                                                        id_tarea_workflow,
                                                                                        registro_radicado,
                                                                                        HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                                        HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                                        "")
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If
            matri_gestion.CLASE_DOCUMENTO = nombre_tipo_documento
            matri_gestion.ID_AREA = id_area
            matri_gestion.ID_CLASE_DOCUMENTO = id_clase_documento
            matri_gestion.ID_SERIE = id_serie
            matri_gestion.ID_SUB_SERIE = id_sub_serie
            matri_gestion.ID_TIPO_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_TIPODOCUMENTO = id_tipo_documento
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            matri_gestion.FECHA_ELABORACION = fecha_elaboracion
            matri_gestion.TIPODOCUMENTO = descripcion_tipo_documento
            matri_gestion.NOMBRE_SERIE = nombre_serie
            matri_gestion.NOMBRE_SUB_SERIE = nombre_sub_serie
            ReDim Preserve Matri_estructura_gabinete(2)
            Matri_estructura_gabinete(2).nombre_campo = "EXPEDIENTE"
            Matri_estructura_gabinete(2).valor_campo = matri_gestion.EXPEDIENTE
            ReDim Preserve Matri_estructura_gabinete(3)
            Matri_estructura_gabinete(3).nombre_campo = "CLASEDOCUMENTO"
            Matri_estructura_gabinete(3).valor_campo = nombre_tipo_documento
            ReDim Preserve Matri_estructura_gabinete(4)
            Matri_estructura_gabinete(4).nombre_campo = "FECHAELABORACION"
            Matri_estructura_gabinete(4).valor_campo = fecha_elaboracion
            ReDim Preserve Matri_estructura_gabinete(5)
            Matri_estructura_gabinete(5).nombre_campo = "TIPODOCUMENTO"
            Matri_estructura_gabinete(5).valor_campo = descripcion_tipo_documento
            ReDim Preserve Matri_estructura_gabinete(6)
            Matri_estructura_gabinete(6).nombre_campo = "NOMBRESERIE"
            Matri_estructura_gabinete(6).valor_campo = nombre_serie
            ReDim Preserve Matri_estructura_gabinete(7)
            Matri_estructura_gabinete(7).nombre_campo = "NOMBRESUBSERIE"
            Matri_estructura_gabinete(7).valor_campo = nombre_sub_serie
            '----------------------------------------------
            'Genera la matriz de datos de almacenamiento
            '----------------------------------------------
            Dim Refclas_Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim Matri_Datos_Almacenamineto() As String
            Erase Matri_Datos_Almacenamineto
            Result = Refclas_Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(Gabinete,
                                                                                         Matri_Datos_Almacenamineto,
                                                                                         Matri_estructura_gabinete)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "Imposible general mariz datos almacenamiento "
                Exit Function
            End If
            If Matri_Datos_Almacenamineto Is Nothing Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "Matriz de datos de almacenamiento es nothing "
                Exit Function
            End If
            Dim iconta As Integer = 0
            For i As Integer = 8 To Matri_estructura_gabinete.Length - 1
                For z As Integer = 0 To stru_campos_plantilla_gabinete.Length - 1
                    If Matri_estructura_gabinete(i).nombre_campo = stru_campos_plantilla_gabinete(z).nombre_campo_ruta Then
                        Matri_Datos_Almacenamineto(iconta) = stru_campos_plantilla_gabinete(z).dato_campo_plantilla
                        Matri_estructura_gabinete(i).valor_campo = stru_campos_plantilla_gabinete(z).dato_campo_plantilla
                    End If
                Next
                iconta = iconta + 1
            Next
            '----------------------------------------------
            'Obtiene el tipo documento 
            '----------------------------------------------
            Dim Tipo_Documento As Integer = -1
            Dim Refclasvisor As New Classactualizacionvisor
            Result = ""
            Dim filinf As New FileInfo(Matri_Documentos_Final(0))
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(filinf.Extension,
                                                                          Tipo_Documento)
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If
            Dim icono As String = ""
            If tipo_java = 1 Then
                Me.Agrega_icono_image_fownt_java(Tipo_Documento.ToString,
                                                 icono)
            Else
                Me.SolicitaIconoImageFownt(Tipo_Documento.ToString,
                                                 icono)
            End If
            '-----------------------------------------------
            'Almacena documento en la base docuarchi.net
            '-----------------------------------------------
            Dim UserWf As String = "WF-" & HttpContext.Current.Session.Item("Login_Usuario_Workfow")
            Result = ""
            Dim estado_firma_digital As Integer = 0
            Result = Refclasalmacena.Almacenamiento("", "", Gabinete, 0, Matri_Datos_Almacenamineto, 2,
            Matri_Documentos_Final.Length, Tipo_Documento, Matri_Documentos_Final, 0, ID_ALMACEN,
            Tipo_Documento, HttpContext.Current.Session.Item("GA_IDEMPRESA"),
            HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), matri_gestion.ID_AREA,
            matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, Radicado, "RAD-" & Radicado, "", 0, estado_firma_digital, id_tarea_workflow,
            HttpContext.Current.Session.Item("Id_Ruta_Workflow"))
            If Result <> "YES" Then
                Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                Exit Function
            End If
            datos_image.nombre_gabinete = Gabinete
            datos_image.id_imagen = ID_ALMACEN
            datos_image.radicado = Radicado
            datos_image.tipodocumental = descripcion_tipo_documento
            datos_image.notipodocumento = descripcion_tipo_documento
            datos_image.extension = UCase(filinf.Extension)
            datos_image.icono_icono_awe_some = icono
            datos_image.estado_firma_digital = estado_firma_digital
            datos_image.DBT = Tipo_Documento
            'Actualiza documento en el flujo
            Dim ref_class_dat_adic As New Class_DAT_ADIC_TAR
            If evalua_flujo = 1 And id_imagen = 0 Then
                Result = ref_class_dat_adic.ActualizaIdImagenTareaWorkflow(HttpContext.Current.Session.Item("WF_RUTAWORKFLOW"),
                                                                            id_tarea_workflow,
                                                                            ID_ALMACEN)
                If Result <> "YES" Then
                    Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = Result
                    Exit Function
                End If
            End If
            Dim val_documento As String = matri_gestion.TIPODOCUMENTO
            If tipo_almacenamiento = 2 Then
                If val_documento = "" Then
                    val_documento = ""
                End If
            Else
                If val_documento = "" Then
                    val_documento = "Documento"
                End If
            End If
            '-----------------------------------------------
            'Elimina los documentos almacenados
            '-----------------------------------------------
            For k As Integer = 0 To Matri_Documentos_Final.Length - 1
                If File.Exists(Matri_Documentos_Final(k)) Then
                    File.Delete(Matri_Documentos_Final(k))
                End If
            Next
            If tipo_almacenamiento <> 0 Then
                If File.Exists(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA")) = True Then
                    Kill(HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA"))
                End If
                HttpContext.Current.Session.Item("WF_RUTA_TEMPO_ADJUNTA") = ""
                HttpContext.Current.Session.Item("WF_ERROR_RESPUESTA") = ""
            End If
            Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "YES"
            Exit Function
        Catch ex As Exception
            Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado = "Funcion Almacenamiento_documentos_adjuntos_digitalizados_modulo_radicado : " & ex.Message
        End Try
    End Function

    Function Retorna_Matriz_imagenes_relacionadas_a_tarea(ByVal Id_tarea As Long) As String
        '----------------------------------------------------------------
        'Funcion : Lista documentos relacionados a una tarea especifica
        'Fecha : 2017-03-14
        'Ing : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Nombre_Ruta As String = ""
            Dim Datos_Tarea As String = ""
            Dim Conta_Doc As Integer = 0
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID-23 SELECCIONA-WF Imposible Encontrar nombre de Ruta " + Result
                Exit Function
            End If
            '--------------------------------------
            'Obtener datos de la tarea
            '--------------------------------------
            'Matri_Datos_Tarea Informacion
            'Matri_Datos_Tarea(0)=ID DATOS TAREA
            'Matri_Datos_Tarea(1)=ID GABIENTE
            'Matri_Datos_Tarea(2)=ID_IMAGEN
            Result = ""
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Nombre_Ruta,
                                                                                            Id_tarea,
                                                                                            structure_datos_tarea_workflow)

            'Result = ref_Class_DAT_ADIC_TAR.Obtener_Datos_Tarea(Nombre_Ruta, _
            '                                                    Id_tarea, _
            '                                                    Datos_Tarea)
            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = Result
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_DAT = 0 Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID#25  Imposible encontrar id de la tarea en la tabla dat_adic_tar  (" & Id_tarea & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID#26 tarea sin id gabinete asignado (" & Id_tarea & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID#27  La imagen de esta tarea fue cambiada o eliminada  tarea sin imagen adjunta  (" & Id_tarea & ")"
                Exit Function
            End If
            '----------------------------------------
            'Obtener datos del gabinete
            '----------------------------------------
            Result = ""
            Dim Datos_Gabientes As String = ""
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                structure_gabinete_workflow)

            'Result = ref_Class_configuracion_gabinete.Obtener_Datos_Gabinete(Matri_Datos_Tarea(1), _
            '                                                                 Datos_Gabientes)
            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = Result
                Exit Function
            End If
            Dim Refclas_trd As New ClassTrdDocumental
            Dim option_aplica_trd As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                               structure_gabinete_workflow.NOMBRE_GABINETE)
            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "#31 SELECCIONA-WF Imposible encontrar opción aplicar trd gabinete (" + structure_gabinete_workflow.NOMBRE_GABINETE + ")"
                Exit Function
            End If

            Dim ref_ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                         structure_datos_tarea_workflow.ID_IMAGEN,
                                                                         stru_paramter_image,
                                                                         option_aplica_trd)

            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID-31 SELECCIONA-WF Obteniendo datos de la imagen " & structure_datos_tarea_workflow.ID_IMAGEN & " en el gabinete" + structure_gabinete_workflow.NOMBRE_GABINETE + " Por favor verifique la existencia del documento en el gabinete"
                Exit Function
            End If
            '***********************************************
            'Determina la extension de la imagen
            '***********************************************
            Dim Cod_Visor As String = ""
            Dim Extension As String = ""
            Dim Estado_Documento As String = ""
            Dim Refclasvis As New Classactualizacionvisor
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                          Cod_Visor,
                                                                          Extension,
                                                                          Estado_Documento)
            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = Result
                Exit Function
            End If
            If Estado_Documento = "LINK" Then
                'Dim Tempo As String = Datos_Imagen_Matri(1).Replace("-", "")
                'Result = ""
                'Result = ref_ClassDaGabinete.Obtener_Datos_Imagen(Conection_conectro_C, _
                '                                                  Datos_Gabientes_Matri(0), _
                '                                                  Tempo, _
                '                                                  Datos_Imagen)
                'If Result <> "YES" Then
                '    Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID-37 SELECCIONA-WF Obteniendo datos de la imagen=" + Datos_Gabientes_Matri(2) + " " + Result
                '    Exit Function
                'End If
                'Erase Datos_Imagen_Matri
                'Datos_Imagen_Matri = Split(Datos_Imagen, "|")
                'Matri_Datos_Tarea(2) = Datos_Imagen_Matri(0)

            End If
            Result = ""
            Dim Valor_Ceros_Imagen As String = "DIG"
            Dim Valor_Ceros_Carpeta_Imagen As String = ""
            Dim Valor_Disco_Imagen As String = ""
            Dim Ruta_Imagen As String = ""
            '----------------------------------------------
            'Obteniendo la identidad de la imagen 
            '----------------------------------------------
            Result = Obtener_Ceros_Imagen(structure_datos_tarea_workflow.ID_IMAGEN.ToString,
                                          Valor_Ceros_Imagen)
            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = Result
                Exit Function
            End If
            Valor_Ceros_Imagen = Valor_Ceros_Imagen & Extension
            Result = ""
            '--------------------------------------------------
            'obtener la identidad de la carpeta
            '--------------------------------------------------
            Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image.IDEX,
                                                  Valor_Ceros_Carpeta_Imagen)
            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID-39 SELECCIONA-WF Error en la funcion obtener ceros de la carpeta =" + stru_paramter_image.DISC + Result
                Exit Function
            End If
            '---------------------------------------------------
            'Obtener carpeta cntenedora imagen enlace
            '---------------------------------------------------
            Valor_Disco_Imagen = structure_gabinete_workflow.NOMBRE_GABINETE & stru_paramter_image.DISC
            '---------------------------------------------------
            'Obtener ruta completa de iamgen enlace
            'asignarla a la matris general
            '---------------------------------------------------
            Dim Matri_Documentos_Tareas() As String
            Conta_Doc = Conta_Doc + 1
            Ruta_Imagen = structure_gabinete_workflow.RUTA_BUSQUEDA_IMAGEN & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen
            ReDim Preserve Matri_Documentos_Tareas(Conta_Doc)
            Matri_Documentos_Tareas(Conta_Doc) = Ruta_Imagen & "|" & stru_paramter_image.PAG & "|" & stru_paramter_image.ID & "|" & Extension & "|" & Estado_Documento
            '-----------------------------------------
            'Consulta los id de la imagenes enlazadas
            'al documento
            '-----------------------------------------
            Dim stru_paramter_image_enlace() As stru_paramter_image = Nothing
            Result = ref_ClassDaGabinete.Solicita_lista_imagenes_enlzadas_gabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                   stru_paramter_image.ENLACE,
                                                                                   stru_paramter_image_enlace,
                                                                                   stru_paramter_image.ID,
                                                                                   option_aplica_trd)
            If Result <> "YES" Then
                Retorna_Matriz_imagenes_relacionadas_a_tarea = Result
                Exit Function
            End If
            If Not stru_paramter_image_enlace Is Nothing Then
                For Iconta = 0 To stru_paramter_image_enlace.Length - 1
                    '----------------------------------------------
                    Valor_Ceros_Imagen = "DIG"
                    Valor_Ceros_Carpeta_Imagen = ""
                    Valor_Disco_Imagen = ""
                    Ruta_Imagen = ""
                    '***********************************************
                    'Determina la extension de la imagen
                    '***********************************************
                    Cod_Visor = ""
                    Extension = ""
                    Estado_Documento = ""
                    Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image_enlace(Iconta).DBT_TIPO_IMAGEN,
                                                                                Cod_Visor,
                                                                                Extension,
                                                                                Estado_Documento)
                    If Result <> "YES" Then
                        Retorna_Matriz_imagenes_relacionadas_a_tarea = Result
                        Exit Function
                    End If
                    If Estado_Documento = "LINK" Then
                        'Dim Tempo As String = Datos_Imagen_Matri(1).Replace("-", "")
                        'Result = ""
                        'Result = ref_ClassDaGabinete.Obtener_Datos_Imagen(Conection_conectro_C, _
                        '                                                  Datos_Gabientes_Matri(0), _
                        '                                                  Tempo, _
                        '                                                  Datos_Imagen)
                        'If Result <> "YES" Then
                        '    Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID-42 SELECCIONA-WF Obteniendo datos de la imagen=" + Datos_Gabientes_Matri(2) + " " + Result
                        '    Exit Function
                        'End If
                        'Erase Datos_Imagen_Matri
                        'Datos_Imagen_Matri = Split(Datos_Imagen, "|")
                    End If

                    'Obteniendo la identidad de la imagen enlace
                    Result = Obtener_Ceros_Imagen(stru_paramter_image_enlace(Iconta).ID,
                                                  Valor_Ceros_Imagen)
                    If Result <> "YES" Then
                        Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID-43 SELECCIONA-WF =" + stru_paramter_image_enlace(Iconta).ID + " " + Result
                        Exit Function
                    End If
                    Valor_Ceros_Imagen = Valor_Ceros_Imagen & Extension
                    Result = ""
                    '--------------------------------------------------
                    'Obtener la identidad de la carpeta enlace
                    '--------------------------------------------------
                    Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image_enlace(Iconta).IDEX,
                                                          Valor_Ceros_Carpeta_Imagen)
                    If Result <> "YES" Then
                        Retorna_Matriz_imagenes_relacionadas_a_tarea = "RMIEID-44 SELECCIONA-WF =" + stru_paramter_image_enlace(Iconta).DISC + Result
                        Exit Function
                    End If
                    '---------------------------------------------------
                    'Obtener carpeta cntenedora imagen enlace
                    '---------------------------------------------------
                    Valor_Disco_Imagen = structure_gabinete_workflow.NOMBRE_GABINETE & stru_paramter_image_enlace(Iconta).DISC
                    '---------------------------------------------------
                    'Obtener ruta completa de imagen enlace
                    'asignarla a la matris general
                    '---------------------------------------------------
                    Conta_Doc = Conta_Doc + 1
                    Ruta_Imagen = structure_gabinete_workflow.RUTA_BUSQUEDA_IMAGEN & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen
                    ReDim Preserve Matri_Documentos_Tareas(Conta_Doc)
                    Matri_Documentos_Tareas(Conta_Doc) = Ruta_Imagen & "|" & stru_paramter_image_enlace(Iconta).PAG & "|" & stru_paramter_image_enlace(Iconta).ID & "|" & Extension & "|" & Estado_Documento
                Next

            End If
            Dim stru_documento_compartido() As stru_documentos_compartidos
            If Not Matri_Documentos_Tareas Is Nothing Then
                For i As Integer = 1 To Matri_Documentos_Tareas.Length - 1
                    Dim split_documento() As String = Matri_Documentos_Tareas(i).Split("|")
                    ReDim Preserve stru_documento_compartido(i - 1)
                    stru_documento_compartido(i - 1).id_imagen = split_documento(2)
                    stru_documento_compartido(i - 1).tipo_documento = split_documento(3)
                    stru_documento_compartido(i - 1).nombre_gabinete = structure_gabinete_workflow.NOMBRE_GABINETE
                    stru_documento_compartido(i - 1).ruta_documento = split_documento(0)
                    stru_documento_compartido(i - 1).identificador = i - 1
                Next
                HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_COMPARTIDO") = stru_documento_compartido
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("GA_STRU_DOCUMENTO_COMPARTIDO") = stru_documento_compartido
                Retorna_Matriz_imagenes_relacionadas_a_tarea = "Imposible encontrar el documento relacionado a la tarea"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Matriz_imagenes_relacionadas_a_tarea = "Inconsistencia general función Retorna_Matriz_imagenes_relacionadas_a_tarea " & ex.Message
        End Try
    End Function
    Function Lista_documentos_visor_workflow(ByVal Id_tarea As Integer,
                                             ByRef Tre_v2 As Object,
                                             ByVal Swuitch As Integer,
                                             ByVal Id_Actividad As Integer,
                                             ByVal Deter_Pendiente As Integer,
                                             ByRef Matri_Documentos_Tareas() As String,
                                             Optional ByVal id_pendiente As Integer = -1,
                                             Optional ByRef value_dafault As String = "") As String


        '----------------------------------------------------------
        'Funcion : Seleccion_Documentos_Wf
        'Funcion que selecicona y lista documentos relacionados
        'con una tarea si se envia la variable switch en 0 cambia
        'de estado la tarea y la lista en un listview
        'Fecha 2012-12-28 
        'Ingeniero : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try

            Dim Nombre_Ruta As String = ""
            Dim Result As String = ""
            Dim Datos_Tarea As String = ""
            Dim Conta_Doc As Integer = 0
            'Dim Matri_Documentos_Tareas() As String
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = "#23 SELECCIONA-WF Imposible Encontrar nombre de Ruta " + Result
                Exit Function
            End If
            '--------------------------------------
            'Obtener datos de la tarea
            '--------------------------------------
            Dim structure_datos_tarea_workflow As structure_datos_tarea_workflow = Nothing
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaDatosEstructuraBasicaTareaWorkflow(Nombre_Ruta,
                                                                                            Id_tarea,
                                                                                            structure_datos_tarea_workflow)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = Result
                Exit Function
            End If

            If structure_datos_tarea_workflow.ID_DAT = 0 Then
                Lista_documentos_visor_workflow = "#25  Imposible encontrar id de la tarea en la tabla dat_adic_tar  (" & Id_tarea & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_GABINETE = 0 Then
                Lista_documentos_visor_workflow = "#26 tarea sin id gabinete asignado (" & Id_tarea & ")"
                Exit Function
            End If
            If structure_datos_tarea_workflow.ID_IMAGEN = 0 Then
                Lista_documentos_visor_workflow = "#27  La imagen de esta tarea fue cambiada o eliminada  tarea sin imagen adjunta  (" & Id_tarea & ")"
                Exit Function
            End If
            Result = ""
            Dim ref_Class_configuracion_gabinete As New Class_configuracion_gabinete
            Dim structure_gabinete_workflow As structure_gabinete_workflow = Nothing
            Result = ref_Class_configuracion_gabinete.SolicitaDatosEstructuraGabineteWorkflow(structure_datos_tarea_workflow.ID_GABINETE,
                                                                                                  structure_gabinete_workflow)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = "#28 SELECCIONA-WF " & Result
                Exit Function
            End If

            Dim Refclas_trd As New ClassTrdDocumental
            Dim option_aplica_trd As Integer = 0
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                               structure_gabinete_workflow.NOMBRE_GABINETE)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = "#31 SELECCIONA-WF Imposible encontrar opción aplicar trd gabinete (" + structure_gabinete_workflow.NOMBRE_GABINETE + ")"
                Exit Function
            End If

            Result = ""
            Dim ref_ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ref_ClassDaGabinete.SolicitaEtructuraImagenGabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                         structure_datos_tarea_workflow.ID_IMAGEN,
                                                                         stru_paramter_image,
                                                                         option_aplica_trd)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = Result
                Exit Function
            End If

            Dim matri_relacion_tipos() As String
            Erase matri_relacion_tipos

            '***********************************************
            'Determina la extension de la imagen
            '***********************************************
            Dim Cod_Visor As String = ""
            Dim Extension As String = ""
            Dim Estado_Documento As String = ""
            Dim Refclasvis As New Classactualizacionvisor
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                          Cod_Visor,
                                                                          Extension,
                                                                          Estado_Documento)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = Result
                Exit Function
            End If
            If Estado_Documento = "LINK" Then
                'Dim Tempo As String = Datos_Imagen_Matri(1).Replace("-", "")
                'Result = ""

                'Result = ref_ClassDaGabinete.Obtener_Datos_Imagen(Conection_conectro_C, _
                '                                                  Datos_Gabientes_Matri(0), _
                '                                                  Tempo, _
                '                                                  Datos_Imagen, _
                '                                                  option_aplica_trd)
                'If Result <> "YES" Then
                '    Lista_documentos_visor_workflow = "#37 SELECCIONA-WF Obteniendo datos de la imagen=" + Datos_Gabientes_Matri(2) + " " + Result
                '    Exit Function
                'End If
                'Erase Datos_Imagen_Matri
                'Datos_Imagen_Matri = Split(Datos_Imagen, "|")
                'Matri_Datos_Tarea(2) = Datos_Imagen_Matri(0)

            End If
            Result = ""
            Dim Valor_Ceros_Imagen As String = "DIG"
            Dim Valor_Ceros_Carpeta_Imagen As String = ""
            Dim Valor_Disco_Imagen As String = ""
            Dim Ruta_Imagen As String = ""
            '----------------------------------------------
            'Obteniendo la identidad de la imagen 
            '----------------------------------------------
            Result = Obtener_Ceros_Imagen(structure_datos_tarea_workflow.ID_IMAGEN.ToString,
                                          Valor_Ceros_Imagen)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = "#38 SELECCIONA-WF Error En la funcion Obtener ceros para la imagen=" + structure_datos_tarea_workflow.ID_IMAGEN + "(" + Result + ")"
                Exit Function
            End If
            Valor_Ceros_Imagen = Valor_Ceros_Imagen & Extension
            Result = ""
            '--------------------------------------------------
            'obtener la identidad de la carpeta
            '--------------------------------------------------
            Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image.IDEX,
                                                  Valor_Ceros_Carpeta_Imagen)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = "#39 SELECCIONA-WF Error en la funcion obtener ceros de la carpeta =" + stru_paramter_image.DISC + "(" + Result + ")"
                Exit Function
            End If
            '---------------------------------------------------
            'Obtener carpeta cntenedora imagen enlace
            '---------------------------------------------------
            Valor_Disco_Imagen = structure_gabinete_workflow.NOMBRE_GABINETE & stru_paramter_image.DISC
            '---------------------------------------------------
            'Obtener ruta completa de iamgen enlace
            'asignarla a la matris general
            '---------------------------------------------------
            Conta_Doc = Conta_Doc + 1
            Ruta_Imagen = structure_gabinete_workflow.RUTA_BUSQUEDA_IMAGEN & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen
            ReDim Preserve Matri_Documentos_Tareas(Conta_Doc)
            Matri_Documentos_Tareas(Conta_Doc) = Ruta_Imagen & "|" & stru_paramter_image.PAG & "|" & stru_paramter_image.ID & "|" & Extension & "|" & Estado_Documento
            If option_aplica_trd <> 0 Then
                ReDim Preserve matri_relacion_tipos(Conta_Doc)
                matri_relacion_tipos(Conta_Doc) = stru_paramter_image.TIPODOCUMENTO
            End If
            '-----------------------------------------
            'Consulta los id de la imagenes enlazadas
            'al documento
            '-----------------------------------------
            Result = ""
            Dim stru_paramter_image_enlace() As stru_paramter_image = Nothing
            Result = ref_ClassDaGabinete.Solicita_lista_imagenes_enlzadas_gabinete(structure_gabinete_workflow.NOMBRE_GABINETE,
                                                                                   stru_paramter_image.ENLACE,
                                                                                   stru_paramter_image_enlace,
                                                                                   stru_paramter_image.ID,
                                                                                   option_aplica_trd)
            If Result <> "YES" Then
                Lista_documentos_visor_workflow = Result
                Exit Function
            End If

            If Not stru_paramter_image_enlace Is Nothing Then
                For Iconta = 0 To stru_paramter_image_enlace.Length - 1
                    '----------------------------------------------
                    Valor_Ceros_Imagen = "DIG"
                    Valor_Ceros_Carpeta_Imagen = ""
                    Valor_Disco_Imagen = ""
                    Ruta_Imagen = ""
                    '***********************************************
                    'Determina la extension de la imagen
                    '***********************************************
                    Cod_Visor = ""
                    Extension = ""
                    Estado_Documento = ""
                    Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image_enlace(Iconta).DBT_TIPO_IMAGEN,
                                                                                  Cod_Visor,
                                                                                  Extension,
                                                                                  Estado_Documento)
                    If Result <> "YES" Then
                        Lista_documentos_visor_workflow = Result
                        Exit Function
                    End If

                    If Estado_Documento = "LINK" Then
                        'Dim Tempo As String = Datos_Imagen_Matri(1).Replace("-", "")
                        'Result = ""
                        'Result = ref_ClassDaGabinete.Obtener_Datos_Imagen(Conection_conectro_C, _
                        '                                                  Datos_Gabientes_Matri(0), _
                        '                                                  Tempo, _
                        '                                                  Datos_Imagen, _
                        '                                                  option_aplica_trd)
                        'If Result <> "YES" Then
                        '    Lista_documentos_visor_workflow = "#42 SELECCIONA-WF Obteniendo datos de la imagen=" + Datos_Gabientes_Matri(2) + " " + Result
                        '    Exit Function
                        'End If
                        'Erase Datos_Imagen_Matri
                        'Datos_Imagen_Matri = Split(Datos_Imagen, "|")
                    End If

                    'Obteniendo la identidad de la imagen enlace
                    Result = Obtener_Ceros_Imagen(stru_paramter_image_enlace(Iconta).ID,
                                                  Valor_Ceros_Imagen)
                    If Result <> "YES" Then
                        Lista_documentos_visor_workflow = "#43 SELECCIONA-WF =" + stru_paramter_image_enlace(Iconta).ID + Result
                        Exit Function
                    End If

                    Valor_Ceros_Imagen = Valor_Ceros_Imagen & Extension
                    Result = ""
                    '--------------------------------------------------
                    'obtener la identidad de la carpeta enlase
                    Result = Obtener_Ceros_Carpeta_Imagen(stru_paramter_image_enlace(Iconta).IDEX,
                                                          Valor_Ceros_Carpeta_Imagen)
                    If Result <> "YES" Then
                        Lista_documentos_visor_workflow = "#44 SELECCIONA-WF =" + stru_paramter_image_enlace(Iconta).DISC + Result
                        Exit Function
                    End If
                    '---------------------------------------------------
                    'Obtener carpeta cntenedora imagen enlace
                    '---------------------------------------------------
                    Valor_Disco_Imagen = structure_gabinete_workflow.NOMBRE_GABINETE & stru_paramter_image_enlace(Iconta).DISC
                    '---------------------------------------------------
                    'Obtener ruta completa de iamgen enlace
                    'asignarla a la matris general
                    '---------------------------------------------------
                    Conta_Doc = Conta_Doc + 1
                    Ruta_Imagen = structure_gabinete_workflow.RUTA_BUSQUEDA_IMAGEN & Valor_Disco_Imagen & "\" & Valor_Ceros_Carpeta_Imagen & "\" & Valor_Ceros_Imagen
                    ReDim Preserve Matri_Documentos_Tareas(Conta_Doc)
                    Matri_Documentos_Tareas(Conta_Doc) = Ruta_Imagen & "|" & stru_paramter_image_enlace(Iconta).PAG & "|" & stru_paramter_image_enlace(Iconta).ID & "|" & Extension & "|" & Estado_Documento
                    If option_aplica_trd <> 0 Then
                        ReDim Preserve matri_relacion_tipos(Conta_Doc)
                        matri_relacion_tipos(Conta_Doc) = stru_paramter_image_enlace(Iconta).TIPODOCUMENTO
                    End If
                Next
            End If
            Tre_v2.Nodes.Clear()
            Dim Tre_v As New TreeNode
            Tre_v.Text = ""
            Tre_v.ExpandAll()
            For i As Integer = 1 To Matri_Documentos_Tareas.Length - 1
                Dim attrNodeGru1 As New TreeNode
                If i = 1 Then
                    value_dafault = Matri_Documentos_Tareas(i) & "|" & structure_gabinete_workflow.NOMBRE_GABINETE
                End If
                attrNodeGru1.Value = Matri_Documentos_Tareas(i) & "|" & structure_gabinete_workflow.NOMBRE_GABINETE
                attrNodeGru1.ToolTip = Matri_Documentos_Tareas(i) & "|" & structure_gabinete_workflow.NOMBRE_GABINETE
                attrNodeGru1.PopulateOnDemand = True
                If option_aplica_trd <> 0 Then
                    If Trim(matri_relacion_tipos(i)) <> "" And Trim(matri_relacion_tipos(i)) <> " " Then
                        attrNodeGru1.Text = matri_relacion_tipos(i)
                    Else
                        attrNodeGru1.Text = "Documento(" & i & ")"
                    End If
                Else
                    attrNodeGru1.Text = "Documento(" & i & ")"
                End If
                Dim ref_clas_seleccion As New Classselecciotarea
                Dim spli_archivo() As String = Matri_Documentos_Tareas(i).Split("|")
                Result = ref_clas_seleccion.Agrega_icono_image_tre_view(spli_archivo(0),
                                                                      attrNodeGru1)
                Tre_v2.Nodes.Add(attrNodeGru1)
            Next
            Tre_v2.EnableViewState = True
            Tre_v2.ExpandAll()
            Lista_documentos_visor_workflow = "YES"
        Catch ex As Exception
            Lista_documentos_visor_workflow = ex.Message
        End Try
    End Function
    Function Solicta_documento_gabinete_service(ByVal matri_documentos_migracion() As String,
                                                ByVal ruta_server As String,
                                                ByVal clasifiquer As String,
                                                ByRef documento_ruta_fisica As String,
                                                ByRef documento_original_fisica As String,
                                                ByRef documento_url_service As String) As String
        Try
            documento_original_fisica = matri_documentos_migracion(1)
            Dim Ref_class_reportes As New Class_ItexShare
            Dim fileinff As New IO.FileInfo(matri_documentos_migracion(1))
            Dim Result As String = ""
            Dim ruta_migracion As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("DA_TEMPO_MIGRACION"))
            If Directory.Exists(ruta_migracion) = False Then
                Directory.CreateDirectory(ruta_migracion)
            End If
            Dim url_clasifque As String = ""
            If clasifiquer <> "" Then
                ruta_migracion = ruta_migracion & clasifiquer & "\"
                If Directory.Exists(ruta_migracion) = False Then
                    Directory.CreateDirectory(ruta_migracion)
                End If
                url_clasifque = clasifiquer & "/"
            End If
            Dim fil_archivo As String = ""
            Dim mtri_redimencion() As String = Nothing
            For i As Integer = 1 To matri_documentos_migracion.Length - 1
                ReDim Preserve mtri_redimencion(i - 1)
                mtri_redimencion(i - 1) = matri_documentos_migracion(i)
            Next
            If UCase(fileinff.Extension) = ".TIF" Or UCase(fileinff.Extension) = ".BMP" Or UCase(fileinff.Extension) = ".JPG" Then
                Dim file_name As String = fileinff.Name.Replace(fileinff.Extension, ".PDF")
                fil_archivo = ruta_migracion & file_name
                If File.Exists(fil_archivo) Then
                    File.Delete(fil_archivo)
                End If
                Result = Ref_class_reportes.Convertir_tif_pdf_gabinete(mtri_redimencion,
                                                                       fil_archivo,
                                                                       "YES")
                If Result <> "YES" Then
                    Solicta_documento_gabinete_service = Result
                    Exit Function
                End If
                documento_ruta_fisica = fil_archivo
                If ruta_server <> "" Then
                    documento_url_service = ruta_server & "/Temp_Radicacion/migracion/" & url_clasifque & file_name
                End If
                Solicta_documento_gabinete_service = "YES"
            Else
                Dim file_archivo_fuente As String = matri_documentos_migracion(1)
                fil_archivo = ruta_migracion & fileinff.Name
                If File.Exists(fil_archivo) Then
                    File.Delete(fil_archivo)
                End If
                If File.Exists(file_archivo_fuente) Then
                    File.Copy(file_archivo_fuente, fil_archivo)
                End If
                documento_ruta_fisica = fil_archivo
                If ruta_server <> "" Then
                    documento_url_service = ruta_server & "/Temp_Radicacion/migracion/" & url_clasifque & fileinff.Name
                End If
                Solicta_documento_gabinete_service = "YES"
            End If
        Catch ex As Exception
            Solicta_documento_gabinete_service = "Inconsistencia general funcion Conver_documento_gabinete_pdf " & ex.Message
        End Try
    End Function
    Function soliCitarMigracion(ByVal matri_documentos_migracion() As String,
                                ByRef documento_migracion As String,
                                ByRef documento_migracion_xml As String) As String
        Try
            Dim ruta_server As String = ""
            Dim tipo_notificacion As Integer = 1
            Dim correo_copia As String = ""
            Dim Refclas_ra_config As New Class_ra_config_notifica_correo
            Dim Ref_class_reportes As New Class_ItexShare
            Dim Result As String = ""
            Result = Refclas_ra_config.Solicita_estado_ruta_notificacion(ruta_server,
                                                                         tipo_notificacion,
                                                                         correo_copia)
            If Result <> "YES" Then
                soliCitarMigracion = Result
                Exit Function
            End If
            Dim fileinff As New IO.FileInfo(matri_documentos_migracion(1))
            Dim ruta_migracion As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("DA_TEMPO_MIGRACION"))
            If Directory.Exists(ruta_migracion) = False Then
                Directory.CreateDirectory(ruta_migracion)
            End If
            Dim fil_archivo As String = ""
            Dim file_xml_nombre = fileinff.Name.Replace(fileinff.Extension, ".XML")
            file_xml_nombre = file_xml_nombre.Replace("DIG", "FXL")
            Dim fil_archivo_xml As String = ruta_migracion & file_xml_nombre
            If File.Exists(fil_archivo_xml) Then
                File.Delete(fil_archivo_xml)
            End If
            Dim file_xml As String = matri_documentos_migracion(1).ToString.Replace(fileinff.Name,
                                                                                    file_xml_nombre)
            If fileinff.Extension <> ".TIF" And fileinff.Extension <> ".BMP" And fileinff.Extension <> ".JPG" Then
                fil_archivo = ruta_migracion & fileinff.Name
                If File.Exists(fil_archivo) Then
                    File.Delete(fil_archivo)
                End If
                Dim file_archivo_fuente As String = matri_documentos_migracion(1)
                If File.Exists(file_archivo_fuente) Then
                    File.Copy(file_archivo_fuente, fil_archivo)
                    documento_migracion = ruta_server & "/Temp_Radicacion/migracion/" & fileinff.Name
                End If
                If File.Exists(file_xml) Then
                    File.Copy(file_xml, fil_archivo_xml)
                    documento_migracion_xml = ruta_server & "/Temp_Radicacion/migracion/" & file_xml_nombre
                End If
                soliCitarMigracion = "YES"
                Exit Function
            Else
                Dim file_name As String = fileinff.Name.Replace(fileinff.Extension, ".PDF")
                fil_archivo = ruta_migracion & file_name
                If File.Exists(fil_archivo) Then
                    File.Delete(fil_archivo)
                End If

                Dim mtri_redimencion() As String = Nothing
                For i As Integer = 1 To matri_documentos_migracion.Length - 1
                    ReDim Preserve mtri_redimencion(i - 1)
                    mtri_redimencion(i - 1) = matri_documentos_migracion(i)
                Next
                Result = Ref_class_reportes.Convertir_tif_pdf_gabinete(mtri_redimencion,
                                                                       fil_archivo,
                                                                       "YES")
                If Result <> "YES" Then
                    soliCitarMigracion = Result
                    Exit Function
                End If
                If File.Exists(file_xml) Then
                    File.Copy(file_xml, fil_archivo_xml)
                    documento_migracion_xml = ruta_server & "/Temp_Radicacion/migracion/" & file_xml_nombre
                End If
                documento_migracion = ruta_server & "/Temp_Radicacion/migracion/" & file_name
                soliCitarMigracion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            soliCitarMigracion = "Inconsistencia general funcion soliCitarMigracion " & ex.Message
        End Try
    End Function
    Function Copia_archivo_para_firma(ByVal ruta_web_server As String,
                                      ByVal file As String,
                                      ByRef url_file_send_firma As String,
                                      ByRef file_archivo_copia As String) As String
        '----------------------------------------------------------
        'Funcion : Solicita ruta url archivo para firmado digital
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-14
        '----------------------------------------------------------
        Try
            Dim ruta_migracion As String = HttpContext.Current.Server.MapPath(HttpContext.Current.Session.Item("DA_TEMPO_MIGRACION"))
            If Directory.Exists(ruta_migracion) = False Then
                Directory.CreateDirectory(ruta_migracion)
            End If
            Dim fileinff As New IO.FileInfo(file)
            file_archivo_copia = ruta_migracion & fileinff.Name
            If IO.File.Exists(file_archivo_copia) Then
                IO.File.Delete(file_archivo_copia)
            End If
            IO.File.Copy(file, file_archivo_copia)
            url_file_send_firma = ruta_web_server & "/Temp_Radicacion/migracion/" & fileinff.Name
            Copia_archivo_para_firma = "YES"
        Catch ex As Exception
            Copia_archivo_para_firma = "Inconsistencia general funcion Copia_archivo_para_firma " & ex.Message
        End Try
    End Function
    Function Copia_archivo_remplaza_archivo_firmado(ByVal file_firmado As String,
                                                    ByVal file_original As String,
                                                    ByVal id_imagen As Integer,
                                                    ByVal gabinete As String,
                                                    ByRef archivo_salida_remplaza As String) As String
        '------------------------------------------------------------------
        'Función : Copia archivo en el ultimo disco, si esta en una carpeta
        'anterior
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-15
        '------------------------------------------------------------------
        Try

            Dim Result As String = ""
            Dim disco As Integer = 0
            Dim carpeta As Integer = 0
            Dim Class_system1 As New Class_system1
            '--------///solicita disco actual y carpeta del gabinete
            Result = Class_system1.Solicita_disco_carpeta_gabinete(gabinete,
                                                                   disco,
                                                                   carpeta)
            If Result <> "YES" Then
                Copia_archivo_remplaza_archivo_firmado = Result
                Exit Function
            End If
            '------///Solicita estructura imagen a actualizar
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = Me.Solicita_structura_imagen_gabinete_indice_expediente(gabinete,
                                                                             id_imagen,
                                                                             stru_paramter_image, 0)
            If Result <> "YES" Then
                Copia_archivo_remplaza_archivo_firmado = Result
                Exit Function
            End If
            Dim Ruta_copia_archivo As String = ""
            Dim _Ruta_Almacenamiento As String = ""
            Dim file_inf As New FileInfo(file_original)
            If stru_paramter_image.IDEX <> carpeta Then
                Dim ref_Class_SYSTEM1RUT As New Class_SYSTEM1RUT
                Result = ref_Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(_Ruta_Almacenamiento,
                                                                           gabinete)
                If Result <> "YES" Then
                    Copia_archivo_remplaza_archivo_firmado = Result
                    Exit Function
                End If
                Dim cerros_carpeta As String = ""
                Result = Ceros_Imagen_Carpeta(carpeta,
                                              cerros_carpeta)
                If Result <> "YES" Then
                    Copia_archivo_remplaza_archivo_firmado = Result
                    Exit Function
                End If
                '-------///Copia archivo contendor de la imagen
                Dim Disco_Documento As String = gabinete & disco
                Ruta_copia_archivo = _Ruta_Almacenamiento & Disco_Documento & "\" & cerros_carpeta & carpeta & "\" & file_inf.Name
                archivo_salida_remplaza = Ruta_copia_archivo
                Dim ruta_copia_archivos_xml As String = _Ruta_Almacenamiento & Disco_Documento & "\" & cerros_carpeta & carpeta & "\"
                File.Copy(file_firmado, Ruta_copia_archivo, True)
                '---------////Copia archivo indice y meda tados xml
                Dim Ceros_Cuerpo_Imag As String = ""
                Result = Ceros_Imagen_Carpeta(stru_paramter_image.IDEX,
                                              cerros_carpeta)
                If Result <> "YES" Then
                    Copia_archivo_remplaza_archivo_firmado = Result
                    Exit Function
                End If
                Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, id_imagen)
                If Result <> "YES" Then
                    Copia_archivo_remplaza_archivo_firmado = Result
                    Exit Function
                End If
                Disco_Documento = gabinete & stru_paramter_image.DISC
                Dim nombre_archivo_xml_meta_dato As String = _Ruta_Almacenamiento & Disco_Documento & "\" & cerros_carpeta & stru_paramter_image.IDEX & "\" & "DIG" & Ceros_Cuerpo_Imag & id_imagen & ".xml"
                Dim nombre_archivo_xml_index As String = _Ruta_Almacenamiento & Disco_Documento & "\" & cerros_carpeta & stru_paramter_image.IDEX & "\" & "FXL" & Ceros_Cuerpo_Imag & id_imagen & ".xml"
                If File.Exists(nombre_archivo_xml_meta_dato) Then
                    File.Copy(nombre_archivo_xml_meta_dato, ruta_copia_archivos_xml & "DIG" & Ceros_Cuerpo_Imag & id_imagen & ".xml", True)
                End If
                If File.Exists(nombre_archivo_xml_index) Then
                    File.Copy(nombre_archivo_xml_index, ruta_copia_archivos_xml & "FXL" & Ceros_Cuerpo_Imag & id_imagen & ".xml", True)
                End If
                '------//Actualiza el disco y la carpeta del documento
                Result = Me.Actualiza_disco_carpeta_gabinete(gabinete,
                                                             id_imagen,
                                                             disco,
                                                             carpeta)
                If Result <> "YES" Then
                    Copia_archivo_remplaza_archivo_firmado = Result
                    Exit Function
                End If
                '----//Elimina archivos antiguos
                File.Delete(file_original)
                If File.Exists(nombre_archivo_xml_meta_dato) Then
                    File.Delete(nombre_archivo_xml_meta_dato)
                End If
                If File.Exists(nombre_archivo_xml_index) Then
                    File.Delete(nombre_archivo_xml_index)
                End If
                Copia_archivo_remplaza_archivo_firmado = "YES"
            Else
                archivo_salida_remplaza = file_original
                File.Copy(file_firmado, file_original, True)
                Copia_archivo_remplaza_archivo_firmado = "YES"
            End If
        Catch ex As Exception
            Copia_archivo_remplaza_archivo_firmado = "Inconsistencia general funcion Copia_archivo_remplaza_archivo_firmado " & ex.Message
        End Try
    End Function
    Function Actualiza_disco_carpeta_gabinete(ByVal gabinete As String,
                                              ByVal id_imagen As Integer,
                                              ByVal disco As Integer,
                                              ByVal carpeta As Integer) As String
        '-------------------------------------------------------------
        'Funcion : Actualiza el disco y la carpeta de la imagen en el
        'gabinete cuando se remplaza
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-15
        '-------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Sql_update As String = "Update " & gabinete & " set DISC=" & disco & ",IDEX=" & carpeta & " where ID=" & id_imagen
            Dim Result As String = ref.SELECTION_INSERT_COMMAND(Sql_update)
            If Result <> "YES" Then
                Actualiza_disco_carpeta_gabinete = "Error Funcion  Actualiza_disco_carpeta_gabinete " & Result
                Exit Function
            Else
                Actualiza_disco_carpeta_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_disco_carpeta_gabinete = "Inconsistencia general funcion Actualiza_disco_carpeta_gabinete " & ex.Message
        End Try
    End Function

    Function Solicita_datos_imagen_gabinete(ByVal gabinete As String,
                                            ByVal id_imagen As Integer,
                                            ByRef datos_imagen As String) As String
        Try
            Dim Sql_consulta = "SELECT * FROM " &
               gabinete &
               " WHERE ID=" & id_imagen
            Dim ref2 As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_GABINETE")
            Dim Resulta As String = ref2.SELECTION_SELECT_FIELDA(Sql_consulta, Datset)
            If Resulta <> "YES" Then
                Solicita_datos_imagen_gabinete = "Funcion  Solicita_datos_imagen_gabinete " & Resulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_imagen_gabinete = "Imposible encontrar los datos de la imagen : " & id_imagen & " en el gabinete (" & gabinete & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    If Datset.Tables(0).Rows(0).IsNull(i) = True Then
                    Else
                        datos_imagen = datos_imagen & vbCrLf & Datset.Tables(0).Rows(0).Item(i)
                    End If
                Next
                Solicita_datos_imagen_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_imagen_gabinete = "Inconsistencia general funcion Solicita_datos_imagen_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_matriz_documentos_almacenados_gabinete(ByVal Id_Documento As Integer,
                                                             ByVal Nombre_Gabinete As String,
                                                             ByRef Matri_Doc_a_Visualizar() As String) As String
        '***********************************************
        'Funcion : Genera_Matris_Documentos_Almacenados
        'Fecha : 2011-02-16
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Genera matriz de documentos 
        'almacenados incluyendo los documentos
        'añanidos
        '************************************************
        Dim Result As String = ""
        Dim Matri_Dat_Principal() As Datos_Registro
        Erase Matri_Dat_Principal
        '*******************************
        'Consulta datos de sistema
        'del documento seleccionado
        '*******************************
        Try
            Result = Solicita_Datos_Documentos(Id_Documento,
                                               Nombre_Gabinete,
                                               Matri_Dat_Principal)
            If Result <> "YES" Then
                Solicita_matriz_documentos_almacenados_gabinete = "Imposible encontrar datos del documento Generando matriz de documentos"
                Exit Function
            End If
            If Matri_Dat_Principal Is Nothing Then
                Solicita_matriz_documentos_almacenados_gabinete = "Matriz datos de sistema de documento principal es nula"
                Exit Function
            End If
            '***************************************
            'Consulta numero paginas documentos 
            'añadidos
            '***************************************
            Dim Ruta_Busqueda As String = ""
            Dim Numero_Doc_Añadidos As Integer = 0
            Dim Numero_Doc_Principal As Integer = 0
            Dim Matri_Documentos() As String
            Erase Matri_Documentos
            If Matri_Dat_Principal(0).dbt = 1 Or
            Matri_Dat_Principal(0).dbt = -10 Or
            Matri_Dat_Principal(0).dbt = -20 Or
            Matri_Dat_Principal(0).dbt = -30 Or
            Matri_Dat_Principal(0).dbt = -40 _
            Then
                Result = Suma_Numero_Documentos_Añadidos(Id_Documento,
                                                         Nombre_Gabinete,
                                                         Numero_Doc_Añadidos)
                If Result <> "YES" Then
                    Solicita_matriz_documentos_almacenados_gabinete = Result
                    Exit Function
                End If
                Numero_Doc_Principal = Val(Matri_Dat_Principal(0).Pag)
            Else
                Numero_Doc_Principal = Val(Matri_Dat_Principal(0).Pag)
            End If
            '****************************************
            'Consulta ruta busqueda de documentos  
            '****************************************
            '****************************************
            'Marcado Cambio visualiza documentos
            'Se agrega una funcion que obtenga ruta
            'de almacenamiento WEB SERIVE
            '****************************************
            Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Result = ""
            Result = Class_SYSTEM1RUT.Consulta_Ruta_Busqueda_Webservice(Ruta_Busqueda,
                                                                        Nombre_Gabinete)
            If Result <> "YES" Then
                Solicita_matriz_documentos_almacenados_gabinete = Result
                Exit Function
            End If
            '******************************************
            'Genera Matriz documentos del documento
            'principal
            '******************************************
            Erase Matri_Doc_a_Visualizar
            Result = ""
            Result = Genera_Matriz_Documentos(Matri_Doc_a_Visualizar,
                                              Id_Documento,
                                              Matri_Dat_Principal(0).dbt,
                                              Ruta_Busqueda,
                                              Matri_Dat_Principal,
                                              Numero_Doc_Principal,
                                              Nombre_Gabinete,
                                              0)
            If Result <> "YES" Then
                Solicita_matriz_documentos_almacenados_gabinete = "Error generando matris documento principal " & Result
            End If
            '*******************************************
            'Determina si tiene documento añadidos
            '*******************************************
            If Numero_Doc_Añadidos = 0 Then
                Solicita_matriz_documentos_almacenados_gabinete = "YES"
                Exit Function
            End If
            '*******************************************
            'Genera matriz datos doc añadidos
            '*******************************************
            Dim Matri_Dat_Añadidos() As Datos_Registro
            Erase Matri_Dat_Añadidos
            If Matri_Dat_Principal(0).dbt = 1 Or
            Matri_Dat_Principal(0).dbt = -10 Or
            Matri_Dat_Principal(0).dbt = -20 Or
            Matri_Dat_Principal(0).dbt = -30 Or
            Matri_Dat_Principal(0).dbt = -40 _
            Then
                Result = ""
                Result = Consulta_Documentos_Añadidos(Id_Documento,
                                                      Nombre_Gabinete,
                                                      Matri_Dat_Añadidos)
                If Result <> "YES" Then
                    Solicita_matriz_documentos_almacenados_gabinete = "Error buscando documentos añadidos " & Result
                    Exit Function
                End If
                If Matri_Dat_Añadidos Is Nothing Then
                    Solicita_matriz_documentos_almacenados_gabinete = "YES"
                    Exit Function
                End If
                '*************************************************
                'Genera matriz documentos añadidos
                '*************************************************
                For z As Integer = 0 To UBound(Matri_Dat_Añadidos)
                    Result = ""
                    Result = Genera_Matriz_Documentos(Matri_Doc_a_Visualizar,
                                                      Matri_Dat_Añadidos(z).Id,
                                                      Matri_Dat_Principal(0).dbt,
                                                      Ruta_Busqueda,
                                                      Matri_Dat_Añadidos,
                                                      Matri_Dat_Añadidos(z).Pag,
                                                      Nombre_Gabinete,
                                                      z)
                    If Result <> "YES" Then
                        Solicita_matriz_documentos_almacenados_gabinete = "Error Generando matriz doc añadidos " & Result
                        Exit Function
                    End If
                Next
            End If
            Solicita_matriz_documentos_almacenados_gabinete = "YES"
        Catch ex As Exception
            Solicita_matriz_documentos_almacenados_gabinete = "Error General function Solicita_matriz_documentos_almacenados_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_Datos_Documentos(ByVal Id_Documento As Integer,
                                       ByVal Nombre_Gabinete As String,
                                       ByRef Matri_Datos() As Datos_Registro) As String
        '***********************************************************
        'Funcion : Funcion que solicita datos de los documentos
        'con id de la imagen y el nombre del gabinete
        'Ingeniero Miguel Angel Urueta Miranda
        'Fecha 2013-05-20
        '***********************************************************
        Try
            Dim Parametro_Consulta As String = "select ID,DISC,PAG,IDEX,DBT  from " & Nombre_Gabinete &
                   " where id='" & Id_Documento & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_TAREA")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_Datos_Documentos = "Error Funcion  Solicita_Datos_Documentos WF-01 Mensaje DBMS" & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_Datos_Documentos = "Funcion Solicita_Datos_Documentos WF-02 imposible encontrar datos pata el documento " & Id_Documento & " del gabiente " & Nombre_Gabinete
                Exit Function
            End If
            ReDim Preserve Matri_Datos(0)
            Matri_Datos(0).Id = (Trim(Datset.Tables(0).Rows(0).Item(0).ToString))
            Matri_Datos(0).Disc = (Trim(Datset.Tables(0).Rows(0).Item(1).ToString))
            Matri_Datos(0).Pag = (Trim(Datset.Tables(0).Rows(0).Item(2).ToString))
            Matri_Datos(0).idex = (Datset.Tables(0).Rows(0).Item(3).ToString)
            Matri_Datos(0).dbt = (Trim(Datset.Tables(0).Rows(0).Item(4).ToString))
            Solicita_Datos_Documentos = "YES"
            Exit Function
            Solicita_Datos_Documentos = "YES"
        Catch ex As Exception
            Solicita_Datos_Documentos = ex.ToString
        End Try
    End Function
    Function Suma_Numero_Documentos_Añadidos(ByVal Id_Documento As String,
                                            ByVal Nombre_Gabinete As String,
                                            ByRef Numero_Doc As Integer) As String
        '*******************************************
        'Funcion : Suma_Numero_Documentos_Añadidos
        'Fecha : 2011-02-16
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Consulta numero documentos
        'añadidos para la imagen
        'Modificado para aplicacion web 2013-05-20
        'Ingeniero Miguel Angel Urueta Miranda
        '*******************************************
        Try
            Dim Parametro_Consulta As String = "select sum(pag) as numero_Paginas  from " & Nombre_Gabinete &
                       " where dbt='" & Id_Documento & "'"
            'Dim Ref_Car_Conec As New Conect.vb.Dbase_Conction_Mysql
            'Dim Dat_reader As MySqlDataReader
            'Dat_reader = Ref_Car_Conec.C_Dareader_Mysql(Conection_conectro_DA, Parametro_Consulta)
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("DATOS_TAREA")
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Suma_Numero_Documentos_Añadidos = "Error Funcion  Suma_Numero_Documentos_Añadidos WF-01 Mensaje DBMS" & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                Suma_Numero_Documentos_Añadidos = "Funcion Suma_Numero_Documentos_Añadidos WF-02 Imposible Encontrar numero paginas añadidas para el id: " & Id_Documento & " del gabiente " & Nombre_Gabinete
                Exit Function
            End If
            Dim Tempvalor As Object = Datset.Tables(0).Rows(0).Item(0)
            If IsDBNull(Tempvalor) Then
                Numero_Doc = 0
            Else
                Numero_Doc = (Trim(Datset.Tables(0).Rows(0).Item(0).ToString))
            End If
            Suma_Numero_Documentos_Añadidos = "YES"
        Catch ex As Exception
            Suma_Numero_Documentos_Añadidos = "Error general Funcion " & vbCrLf &
            "Suma_Numero_Documentos_Añadidos Decri Error : " & ex.Message
        End Try
    End Function
    Function Genera_Matriz_Documentos(ByRef Matri_Documentos() As String,
                                      ByVal id_documento As Integer,
                                      ByVal tipo_doc As Integer,
                                      ByVal Ruta_Documento As String,
                                      ByVal Matri_Datos_Imagen() As Datos_Registro,
                                      ByVal Numero_Documentos As Integer,
                                      ByVal Nombre_Gabinete As String,
                                      ByVal Indice_Matri_Datos As Integer) As String
        '*******************************************
        'Funcion : Genera_Matriz_Documentos
        'Fecha : 2011-06-16
        'Ing : Miguel Angel Urueta Miranda
        'Descripcion : Genera matris documentos
        'sobre una estructura de la imagen
        'y agrega la matriz a una matriz existente
        '*******************************************
        Dim Ceros_Cuerpo_Imag As String = ""
        Dim Result As String = ""
        Dim i As Integer = 0
        Dim i2 As Integer = 0
        Dim Ceros_Ext As String = ""
        Dim Imagen_Principal As String = ""
        Dim Carpeta_Documento As String = ""
        Dim Disco_Documento As String = ""
        Dim Ceros_Carpeta As String = ""
        Dim Icremento_Matri As Integer = 0
        Dim Cuerpo_Doc As String = ""
        Dim Ruta_Documento_Completa As String = ""
        Try
            '*****************************************
            'Crea disco imagen
            '*****************************************
            Disco_Documento = Nombre_Gabinete & Matri_Datos_Imagen(Indice_Matri_Datos).Disc
            '*****************************************
            'Crea Carpeta almacenamiento
            '****************************************
            Result = Ceros_Imagen_Carpeta(Matri_Datos_Imagen(Indice_Matri_Datos).idex.ToString,
                                          Ceros_Carpeta)
            If Result <> "YES" Then
                Genera_Matriz_Documentos = "Error generando ceros carpeta " & Result & Ceros_Carpeta
                Exit Function
            End If
            '*****************************************
            'Crea la ruta del documento
            '*****************************************
            Ruta_Documento_Completa = Ruta_Documento & Disco_Documento & "\" & Ceros_Carpeta &
            Matri_Datos_Imagen(Indice_Matri_Datos).idex.ToString & "\"

            '*****************************************
            'Crea el cuerpo de la imagen
            '*****************************************
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag,
                                             id_documento)
            If Result <> "YES" Then
                Genera_Matriz_Documentos = "Error generando ceros de imagen "
                Exit Function
            End If
            Dim ref_clas As New Classactualizacionvisor
            Dim visor As String = ""
            Dim ext As String = ""
            Dim Est_doc As String = ""
            Result = ""
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(tipo_doc,
                                                                          visor,
                                                                          ext,
                                                                          Est_doc)
            If Result <> "YES" Then
                Genera_Matriz_Documentos = "Error generando imagen añadida " & Result
                Exit Function
            End If
            '***************************************
            'Agrega el documento principal
            'a la matriz de documentos
            '***************************************
            Cuerpo_Doc = "DIG" & Ceros_Cuerpo_Imag & id_documento
            If Matri_Documentos Is Nothing Then
                Icremento_Matri = 1
                ReDim Preserve Matri_Documentos(1)
            Else
                Icremento_Matri = UBound(Matri_Documentos) + 1
                ReDim Preserve Matri_Documentos(Icremento_Matri)
            End If
            Matri_Documentos(Icremento_Matri) = Ruta_Documento_Completa & Cuerpo_Doc & ext
            '*******************************************
            'Agrega los documentos del tif
            '*******************************************
            If UCase(ext) = ".TIF" Or UCase(ext) = ".BMP" Or UCase(ext) = ".JPG" Then
                If Numero_Documentos > 1 Then
                    For i3 As Integer = 0 To Numero_Documentos - 2
                        Result = ""
                        Ceros_Ext = ""
                        Result = Ceros_Imagen_Alamacenada_ext(i3,
                                                              Ceros_Ext)
                        If Result <> "YES" Then
                            Genera_Matriz_Documentos = "Error generando ceros extension imagen "
                            Exit Function
                        End If
                        Icremento_Matri = Icremento_Matri + 1
                        ReDim Preserve Matri_Documentos(Icremento_Matri)
                        Matri_Documentos(Icremento_Matri) = Ruta_Documento_Completa & Cuerpo_Doc & "." & Ceros_Ext & i3
                    Next
                End If
            End If
            Genera_Matriz_Documentos = "YES"
        Catch ex As Exception
            Genera_Matriz_Documentos = ex.ToString
        End Try
    End Function
    Function Consulta_Documentos_Añadidos(ByVal Id_Documento As Integer,
                                          ByVal Nombre_Gabinete As String,
                                          ByRef Matri_Datos() As Datos_Registro) As String
        Try
            Dim Parametro_Consulta As String = "select ID,DISC,PAG,IDEX,DBT  from " & Nombre_Gabinete &
                       " where dbt='" & Id_Documento & "' order by id"
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet(Nombre_Gabinete)
            Dim Result As String = ref.SELECTION_SELECT_FIELDA(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Consulta_Documentos_Añadidos = "Funcion  Consulta_Documentos_Añadidos WF-01 Mensaje DBMS" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Consulta_Documentos_Añadidos = "YES"
                Exit Function
            Else
                For z As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Datos(z)
                    Matri_Datos(z).Id = Datset.Tables(0).Rows(z).Item(0).ToString
                    Matri_Datos(z).Disc = Datset.Tables(0).Rows(z).Item(1).ToString
                    Matri_Datos(z).Pag = Datset.Tables(0).Rows(z).Item(2).ToString
                    Matri_Datos(z).idex = Datset.Tables(0).Rows(z).Item(3).ToString
                    Matri_Datos(z).dbt = Datset.Tables(0).Rows(z).Item(4).ToString
                Next
                Consulta_Documentos_Añadidos = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Consulta_Documentos_Añadidos = "Inconsistencia general funcion Consulta_Documentos_Añadidos " & ex.ToString
        End Try
    End Function
    Function Solicita_ruta_documento_gabinete(ByVal id_imagen As Integer,
                                              ByVal nombre_gabinete As String,
                                              ByRef ruta_archivo_gabinete As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita ruta documento gabinete
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_imagen             : Representa la identificacón de la imagen
        'nombre_gabinete       : Representa el nombre del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ruta_archivo_gabinete : Retorna ruta documento gabinete
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Dim Result As String = ""
            Dim Ruote_cabinet As String = ""
            Result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruote_cabinet,
                                                                   nombre_gabinete)
            If Result <> "YES" Then
                Solicita_ruta_documento_gabinete = Result
                Exit Function
            End If
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim class_zerro_fill_ As New Class_zero_fill
            Dim expediente_zero_fil As String = id_imagen.ToString
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(nombre_gabinete,
                                                                                          id_imagen,
                                                                                          stru_paramter_image,
                                                                                          0)
            If Result <> "YES" Then
                Solicita_ruta_documento_gabinete = Result
                Exit Function
            End If
            Dim cerros_carpeta As String = ""
            Result = Ceros_Imagen_Carpeta(stru_paramter_image.IDEX,
                                          cerros_carpeta)
            If Result <> "YES" Then
                Solicita_ruta_documento_gabinete = Result
                Exit Function
            End If
            Dim Ceros_Cuerpo_Imag As String = ""
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag,
                                             id_imagen)
            If Result <> "YES" Then
                Solicita_ruta_documento_gabinete = Result
                Exit Function
            End If
            Dim ref_clas As New Classactualizacionvisor
            Dim visor As String = ""
            Dim ext As String = ""
            Dim Est_doc As String = ""
            Result = ""
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                          visor,
                                                                          ext,
                                                                          Est_doc)
            If Result <> "YES" Then
                Solicita_ruta_documento_gabinete = Result
                Exit Function
            End If
            Dim Disco_Documento As String = nombre_gabinete & stru_paramter_image.DISC
            ruta_archivo_gabinete = Ruote_cabinet & Disco_Documento & "\" & cerros_carpeta & stru_paramter_image.IDEX & "\" & "DIG" & Ceros_Cuerpo_Imag & id_imagen & ext
            Solicita_ruta_documento_gabinete = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_ruta_documento_gabinete = "Inconsistencia general funcion Solicita_ruta_documento_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_ruta_achivo_gabinete(ByVal id_imagen As String,
                                           ByVal nombre_gabinete As String,
                                           ByVal ruta_almacenamiento As String,
                                           ByRef ruta_archivo_gabinete As String) As String
        '------------------------------------------------------------
        'Funcion : Solicita ruta del archivo especifico del gabinete
        'con el parametro de la ruta y la identificacion del gabinete
        'Fecha : 2022-02-17
        'Ing .Miguel Angel Urueta Miranda
        '------------------------------------------------------------
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim class_zerro_fill_ As New Class_zero_fill
            Dim expediente_zero_fil As String = id_imagen.ToString
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Dim Result As String = ""
            Result = ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(nombre_gabinete,
                                                                                          id_imagen,
                                                                                          stru_paramter_image,
                                                                                          0)
            If Result <> "YES" Then
                Solicita_ruta_achivo_gabinete = Result
                Exit Function
            End If
            Dim cerros_carpeta As String = ""
            Result = Ceros_Imagen_Carpeta(stru_paramter_image.IDEX,
                                          cerros_carpeta)
            If Result <> "YES" Then
                Solicita_ruta_achivo_gabinete = Result
                Exit Function
            End If
            Dim Ceros_Cuerpo_Imag As String = ""
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag,
                                             id_imagen)
            If Result <> "YES" Then
                Solicita_ruta_achivo_gabinete = Result
                Exit Function
            End If
            Dim ref_clas As New Classactualizacionvisor
            Dim visor As String = ""
            Dim ext As String = ""
            Dim Est_doc As String = ""
            Result = ""
            Dim ref_Class_da_extension As New Class_da_extension
            Result = ref_Class_da_extension.Determina_tipo_documento_list(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                          visor,
                                                                          ext,
                                                                          Est_doc)
            If Result <> "YES" Then
                Solicita_ruta_achivo_gabinete = Result
                Exit Function
            End If
            Dim Disco_Documento As String = nombre_gabinete & stru_paramter_image.DISC
            ruta_archivo_gabinete = ruta_almacenamiento & Disco_Documento & "\" & cerros_carpeta & stru_paramter_image.IDEX & "\" & "DIG" & Ceros_Cuerpo_Imag & id_imagen & ext
            Solicita_ruta_achivo_gabinete = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_ruta_achivo_gabinete = "Incosistencia general función Solicita_ruta_achivo_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_datos_expediente_relacion_gabinete(ByVal id_imagen As Integer,
                                                         ByVal nombre_gabinete As String,
                                                         ByRef matri_gestion As estructure_gestion) As String
        Try
            Dim Parametro_Consulta = "SELECT ID_EXPEDIENTE,ID_TIPO_EXPEDIENTE,EXPEDIENTE  FROM " & nombre_gabinete &
               " where ID=" & id_imagen
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_gabinete)
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_expediente_relacion_gabinete = "Funcion Solicita_datos_expediente_relacion_gabinete dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_expediente_relacion_gabinete = "Imposible encontrar el id (" & id_imagen & ") de la imagen en la tabla (" & nombre_gabinete & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    matri_gestion.ID_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(0)
                Else
                    matri_gestion.ID_EXPEDIENTE = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    matri_gestion.ID_TIPO_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(1)
                Else
                    matri_gestion.ID_TIPO_EXPEDIENTE = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = False Then
                    matri_gestion.EXPEDIENTE = Datset.Tables(0).Rows(0).Item(2)
                Else
                    matri_gestion.EXPEDIENTE = ""
                End If
                Solicita_datos_expediente_relacion_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_expediente_relacion_gabinete = "Inconsistencia general función Solicita_datos_expediente_relacion_gabinete " & ex.Message
        End Try
    End Function
    Function Expotar_de_gabinete_a_gabinete(ByVal nombre_gabinete As String,
                                            ByVal nombre_gabinete_destino As String,
                                            ByVal id_imagen As Integer,
                                            ByRef id_imagen_copia As Integer) As String
        Try
            Dim Result As String = ""
            Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
            Dim estructura_gabinete() As estructura_gabinete = Nothing
            Dim estructura_gabinete_destino() As estructura_gabinete = Nothing
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete,
                                                                                 estructura_gabinete)
            If Result <> "YES" Then
                Expotar_de_gabinete_a_gabinete = Result
                Exit Function
            End If
            Result = Class_DETALLE_GABIENETE.SolicitaEstructuraCamposGabinete(nombre_gabinete_destino,
                                                                                estructura_gabinete_destino)
            If Result <> "YES" Then
                Expotar_de_gabinete_a_gabinete = Result
                Exit Function
            End If
            If estructura_gabinete.Length > estructura_gabinete_destino.Length Then
                Expotar_de_gabinete_a_gabinete = "El gabinete (" & nombre_gabinete & ") fuente del documento a exportar tiene mayor numero de campos de almacenamiento que el gabinete destino (" & nombre_gabinete_destino & ")"
                Exit Function
            End If
            If estructura_gabinete.Length < estructura_gabinete_destino.Length Then
                Expotar_de_gabinete_a_gabinete = "El gabinete (" & nombre_gabinete & ") fuente del documento a exportar tiene menor numero de campos de almacenamiento que el gabinete destino (" & nombre_gabinete_destino & ")"
                Exit Function
            End If
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Result = ClassAlmacenamiento.Retorna_parametros_almacenamiento_documento_relacionado(id_imagen,
                                                                                                 matri_datos_almacen,
                                                                                                 matri_gestion,
                                                                                                 nombre_gabinete)
            If Result <> "YES" Then
                Expotar_de_gabinete_a_gabinete = Result
                Exit Function
            End If
            matri_gestion.ID_UNIDAD_CONSERVACION = 0
            matri_gestion.ID_USUARIO_GESTION = HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION")
            matri_gestion.TIPO_UNIDAD_DOCUMENTAL = 0
            matri_gestion.UNIDAD_CONSERVACION = ""
            Dim matri_documentos() As String = Nothing
            Dim matri_documentos_() As String = Nothing
            Result = Me.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                        nombre_gabinete,
                                                                        matri_documentos_)

            If Result <> "YES" Then
                Expotar_de_gabinete_a_gabinete = Result
                Exit Function
            End If
            Dim incre As Integer = 0
            For i As Integer = 1 To matri_documentos_.Length - 1
                ReDim Preserve matri_documentos(incre)
                matri_documentos(incre) = matri_documentos_(i)
                incre += 1
            Next
            Dim Tipo_Doc_int As Integer = -1
            Dim Filein As New FileInfo(matri_documentos(0))
            Dim Refalmacena As New ClassAlmacenamiento
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.SolicitaTipoArchivoDocuarchiExtension(UCase(Filein.Extension),
                                                                          Tipo_Doc_int)
            If Result <> "YES" Then
                Expotar_de_gabinete_a_gabinete = "Imposible determinar el tipo de documento " & Result
                Exit Function
            End If

            Dim estado_firma_digital As Integer = 0
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete_destino, 0, matri_datos_almacen,
             2, matri_documentos.Length, Tipo_Doc_int, matri_documentos, 0, id_imagen_copia, Tipo_Doc_int,
             HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
             matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION)
            If Result <> "YES" Then
                Expotar_de_gabinete_a_gabinete = Result
                Exit Function
            End If
            Expotar_de_gabinete_a_gabinete = "YES"
        Catch ex As Exception
            Expotar_de_gabinete_a_gabinete = "Inconsistencia general funcion Expotar_de_gabinete_a_gabinete " & ex.Message
        End Try
    End Function
    Function Retorna_url_documento(ByRef url As String) As String
        Try
            Dim Refclas As New ClassWorflowVisor
            Dim Result As String = ""
            Dim Matri_Doc_Visual() As String = Nothing
            Result = Refclas.Visualizacion_Documentos_da(Matri_Doc_Visual,
                                                             HttpContext.Current.Session.Item("DA_IMAGEN"),
                                                             HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"))
            If Result <> "YES" Then
                Retorna_url_documento = Result
                Exit Function
            Else

                url = Matri_Doc_Visual(1)
            End If
            Dim refgabinete As New ClassDaGabinete
            Dim datos_log As String = ""
            Result = refgabinete.Retorna_Datos_Auditoria_Gabinete(HttpContext.Current.Session.Item("DA_IMAGEN"),
                                                                  HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"),
                                                                  datos_log)
            If Result <> "YES" Then
                Retorna_url_documento = Result
                Exit Function
            End If
            Dim selecion As String = ""
            Result = refgabinete.Registra_Auditoria_Eventos(HttpContext.Current.Session.Item("DA_GABINETE_CONSULTA"),
                                                            selecion & " Imagen Principal " & Matri_Doc_Visual(0),
                                                            HttpContext.Current.Session.Item("DA_IMAGEN"),
                                                            datos_log,
                                                            "Visualiza")
            If Result <> "YES" Then
                Retorna_url_documento = Result
                Exit Function
            End If
            Retorna_url_documento = "YES"
        Catch ex As Exception
            Retorna_url_documento = "Inconsistencia general funcion Retorna_url_documento " & ex.Message
        End Try
    End Function
    Function Valida_cambio_valor_campo_indice(ByVal nombre_campo As String,
                                              ByVal valor_campo As String,
                                              ByVal estructura_gabinete() As estructura_gabinete,
                                              ByRef Exitencia_cambio_valor As String) As String
        Try
            For i As Integer = 0 To estructura_gabinete.Length - 1
                If UCase(nombre_campo) = UCase(estructura_gabinete(i).CAMPO) Then
                    If valor_campo <> estructura_gabinete(i).VALORCAMPO Then
                        Exitencia_cambio_valor = "YES"
                        Exit For
                    End If
                End If
            Next
            Valida_cambio_valor_campo_indice = "YES"
            Exit Function
        Catch ex As Exception
            Valida_cambio_valor_campo_indice = "Inconsistencia general funcion Valida_cambio_valor_campo_indice " & ex.Message
        End Try
    End Function
    Function Solicita_valor_campo_indice(ByVal nombre_campo As String,
                                         ByVal estructura_gabinete_old() As estructura_gabinete,
                                         ByRef valor_campo As String) As String
        Try
            For i As Integer = 0 To estructura_gabinete_old.Length - 1
                If UCase(nombre_campo) = UCase(estructura_gabinete_old(i).CAMPO) Then
                    valor_campo = estructura_gabinete_old(i).VALORCAMPO
                    Exit For
                End If
            Next
            Solicita_valor_campo_indice = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_valor_campo_indice = "Inconsistencia general funcion Solicita_valor_campo_indice " & ex.Message
        End Try
    End Function

End Class
