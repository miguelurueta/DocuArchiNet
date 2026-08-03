Imports System.IO
Imports MySql.Data.MySqlClient
Imports System.Xml

Public Class ClassEliminarDocListResult
    Function EliminarDocumentosGabinete(ByVal Id_Documento As Integer,
                                              ByVal Id_Index As Integer,
                                              ByVal Nombre_Gabinete As String,
                                              ByVal option_verfica_radicado As Integer,
                                              ByVal option_verifca_propietario As Integer,
                                              ByVal master_eliminacion As Integer,
                                              ByVal id_tarea_wf As Long,
                                              ByVal modulo_log As String) As String
        Dim ClassVisualisaDocumento As New ClassVisualisaDocumento
        Dim Result As String = ""
        Dim Matri_Dat_Sistema() As String
        Dim Matri_Doc_Eliminar() As String
        Erase Matri_Dat_Sistema
        Dim Matri_Documentos_Eliminados() As String
        Erase Matri_Documentos_Eliminados
        Dim opcion_inventario As Integer = 0
        Dim option_aplica_trd As Integer = 0
        Dim id_tipo_unidad_documental As Integer = 0
        Dim option_unidad_conservacion As Integer = 0
        Dim id_inventario As Integer = -1
        Dim radicado As String = ""
        Dim pagi As Integer = 0
        '----------------------------------------------------------------
        'Lista matriz de documentos a eliminar 
        '----------------------------------------------------------------
        Result = ClassVisualisaDocumento.Genera_Matris_Documentos_Almacenados(Id_Documento,
                                                                              Nombre_Gabinete,
                                                                              Matri_Documentos_Eliminados)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = Result
            Exit Function
        End If
        pagi = Matri_Documentos_Eliminados.Length
        '----------------------------------------
        'Verfifica la existencia de la imagenes
        '----------------------------------------
        If Matri_Documentos_Eliminados Is Nothing Then
            EliminarDocumentosGabinete = "Imposible econtrar la identificación de la imagen (" & Id_Documento & ") en el gabinete (" & Nombre_Gabinete & ")"
            Exit Function
        End If
        '-------------------------------------------------------------------------------
        'Restricción propietario si el usuario no está activo como master eliminación
        '--------------------------------------------------------------------------------
        Dim ClassDaGabinete As New ClassDaGabinete
        If master_eliminacion = 0 Then
            If option_verifca_propietario = 1 Then
                If HttpContext.Current.Session.Item("DA_Login_Usuario") = "" Then
                    EliminarDocumentosGabinete = "Debe relacionar el usuario en el contenedor DocuArchi  "
                    Exit Function
                End If
                Dim usuario_propietario As String = ""
                Result = ClassDaGabinete.Solicita_usuario_propietario_imagen_gabinete(Nombre_Gabinete,
                                                                                      Id_Documento,
                                                                                      usuario_propietario)
                If Result <> "YES" Then
                    EliminarDocumentosGabinete = Result
                    Exit Function
                End If
                If UCase(HttpContext.Current.Session.Item("DA_Login_Usuario")) <> UCase(usuario_propietario) Then
                    EliminarDocumentosGabinete = "No puede eliminar el documento  pertenece al usuario (" & UCase(usuario_propietario) & ")"
                    Exit Function
                End If
            End If
        End If
        Dim Ref_class_producion As New ClassGaProducionDocumental
        Dim id_registro_producion_documental As Long = 0
        Result = Ref_class_producion.Solicita_id_registro_producion_documental(Id_Documento,
                                                                               Nombre_Gabinete,
                                                                               id_registro_producion_documental)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = Result
            Exit Function
        End If
        '-----------------------------------------------------------
        'Restrición eliminar documento compartido
        '-----------------------------------------------------------
        Dim Class_ra_Cd_Documentos_Compartidos As New Class_ra_Cd_Documentos_Compartidos
        Dim id_registro_documento_compartido As Integer = 0
        Result = Class_ra_Cd_Documentos_Compartidos.Restricion_eliminar_documento_compartido(Id_Documento,
                                                                                             Nombre_Gabinete,
                                                                                             id_registro_documento_compartido)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = Result
            Exit Function
        End If
        If id_registro_documento_compartido <> 0 Then
            Result = Ref_class_producion.Elimina_archivo_producion_restriccion(id_registro_producion_documental)
            If Result <> "YES" Then
                EliminarDocumentosGabinete = Result
                Exit Function
            End If
        End If
        '-----------------------------------------------------------
        'Restriccion_eliminar_documento_producción_radicado
        '-----------------------------------------------------------
        Dim radicado_relacionado_producion_documental As String = ""
        Result = Me.Restriccion_eliminar_documento_producción_radicado(Id_Documento,
                                                                       Nombre_Gabinete,
                                                                       radicado_relacionado_producion_documental)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = Result
            Exit Function
        End If
        If radicado_relacionado_producion_documental <> "" Then
            Result = Ref_class_producion.Elimina_archivo_producion_restriccion(id_registro_producion_documental)
            If Result <> "YES" Then
                EliminarDocumentosGabinete = Result
                Exit Function
            End If
        End If
        '-------------------------------------------------------------------
        'Detecta archivo firmado digital y cancela la elminación
        '------------------------------------------------------------------
        Dim estado_firma_digital As Integer = 0
        If id_registro_producion_documental <> 0 Then
            Result = Ref_class_producion.Solicita_estado_firma_digital(id_registro_producion_documental,
                                                                       estado_firma_digital)
            If Result <> "YES" Then
                EliminarDocumentosGabinete = Result
                Exit Function
            End If
            If estado_firma_digital = 1 And HttpContext.Current.Session.Item("UTIL_FIR_MASTER_ELIMINA_DOCUMENTO") = 0 Then
                EliminarDocumentosGabinete = "El documento tiene una firma digital que no se puede eliminar. A continuación, se detalla la información del documento.  Identificador único  (" &
                id_registro_producion_documental & ") Gabinete de almacenamiento (" & Nombre_Gabinete & ") Identificador de Gabinete  (" & Val(Id_Documento) & ")"
                Exit Function
            End If
        End If
        '---------------------------------------------------------------
        'Detecta archivo de versiones firmado digitalmente
        '---------------------------------------------------------------
        Dim ClassRaCertRegistroCertificadoArchivo As New Class_ra_cert_registro_certificado_archivo
        Dim CountCertificados As Integer = 0
        Result = ClassRaCertRegistroCertificadoArchivo.Solicita_registro_certificado_registro_producion(id_registro_producion_documental,
                                                                                                        CountCertificados)
        If CountCertificados > 0 And HttpContext.Current.Session.Item("UTIL_FIR_MASTER_ELIMINA_DOCUMENTO") = 0 Then
            EliminarDocumentosGabinete = "El documento contiene varias versiones firmadas digitalmente, las cuales no pueden ser eliminadas. A continuación, se detalla la información del documento.  Identificador único  (" &
                id_registro_producion_documental & ") Gabinete de almacenamiento (" & Nombre_Gabinete & ") Identificador de Gabinete  (" & Val(Id_Documento) & ")"
            Exit Function
        End If
        Dim ClassSystem As New Class_system1
        Dim IdGabinete As Integer = 0
        Result = ClassSystem.SolicitaIdGabineteDocuarchi(Nombre_Gabinete,
                                                         IdGabinete)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = Result
            Exit Function
        End If
        '--------------------------------------------------------------
        'Solicita versiones del documento para eliminación
        '--------------------------------------------------------------
        Dim Class_ra_ver_version_documento As New Class_ra_ver_version_documento
        Dim Stru_registro_version_documento() As Stru_registro_version_documento = Nothing
        Result = Class_ra_ver_version_documento.SolicitaEstructurasVersionesDocumentoPorIdGabinete(Id_Documento,
                                                                                                   IdGabinete,
                                                                                                   0,
                                                                                                   Stru_registro_version_documento)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = Result
            Exit Function
        End If
        '-------------------------------------------------------------
        'Solicita los documentos relacionados a las versiones
        '-------------------------------------------------------------
        If Not Stru_registro_version_documento Is Nothing Then
            For i As Integer = 0 To Stru_registro_version_documento.Length - 1
                Result = Class_ra_ver_version_documento.Solicita_matriz_documentos_version(Stru_registro_version_documento(i),
                                                                                           Stru_registro_version_documento(i).EstruDocumentosRelacionados)
                If Result <> "YES" Then
                    EliminarDocumentosGabinete = Result
                    Exit Function
                End If
                '--//Establecer atributo normales para permitir la eliminación de documentos de versiones---///
                For z As Integer = 0 To Stru_registro_version_documento(i).EstruDocumentosRelacionados.Length - 1
                    Dim infoReader As System.IO.FileInfo
                    infoReader = My.Computer.FileSystem.GetFileInfo(Stru_registro_version_documento(i).EstruDocumentosRelacionados(z))
                    Dim attributeReader As System.IO.FileAttributes
                    attributeReader = infoReader.Attributes
                    If attributeReader <> FileAttributes.Normal Then
                        infoReader.Attributes = FileAttributes.Normal
                    End If
                Next
            Next
        End If
        Dim matri_gestion_antigua As estructure_gestion = Nothing
        Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
        Dim id_registro_indice As Long = 0
        Dim Ruta_archivo_xml As String = ""
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
        Try
            Erase Matri_Doc_Eliminar
            Dim Contador As Integer = 0
            For i As Integer = 1 To UBound(Matri_Documentos_Eliminados)
                '-------------------------------------------------------
                'Establecer atributos normal de archivos
                '-------------------------------------------------------
                If HttpContext.Current.Session.Item("ESTADOFILESERVER") = "1" Then
                    Dim infoReader As System.IO.FileInfo
                    infoReader = My.Computer.FileSystem.GetFileInfo(Matri_Documentos_Eliminados(i).ToString)
                    Dim attributeReader As System.IO.FileAttributes
                    attributeReader = infoReader.Attributes
                    If attributeReader <> FileAttributes.Normal Then
                        infoReader.Attributes = FileAttributes.Normal
                    End If

                End If
                ReDim Preserve Matri_Doc_Eliminar(Contador)
                Matri_Doc_Eliminar(Contador) = Matri_Documentos_Eliminados(i)
                Contador = Contador + 1
            Next
            '------------------------------------
            'Consulta Datos imagen Seleccionada
            '------------------------------------
            Dim RefClasConsultGabi As New ClassConsultaGabinete
            Result = RefClasConsultGabi.Datos_Sitema_de_Imagen(Id_Documento,
                                                               Nombre_Gabinete,
                                                               Matri_Dat_Sistema)
            If Result <> "YES" Then
                EliminarDocumentosGabinete = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Adiciona las validaciones para gestiòn documental
            'Verfica la opcion aplicar inventario documental
            '---------------------------------------------------
            Dim refclastrd As New ClassTrdDocumental
            Dim ref_Class_system1 As New Class_system1
            Result = ref_Class_system1.VerificaOpcionAplicarInventarioDocumental(opcion_inventario,
                                                                                     Nombre_Gabinete)
            If Result <> "YES" Then
                EliminarDocumentosGabinete = "Inconsistencia verficando opción registrar inventario documental codigo:  " & Result
                Exit Function
            End If
            '--------------------------------------------------------------------
            'Verfica opción aplica tabla de retencion documental
            '--------------------------------------------------------------------
            Result = ref_Class_system1.VerificaOpcionAplicarTablaRetencion(option_aplica_trd,
                                                                               Nombre_Gabinete)
            If Result <> "YES" Then
                EliminarDocumentosGabinete = "Inconsistencia verficando opción asignación trd codigo:  " & Result
                Exit Function
            End If
            '--------------------------------------------------------------------
            'Verfica la opcion aplica unidad de conservación
            '--------------------------------------------------------------------
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(option_unidad_conservacion,
                                                                       Nombre_Gabinete)
            If Result <> "YES" Then
                EliminarDocumentosGabinete = "Inconsistencia verficando opción asignación unidad y expediente codigo:  " & Result
                Exit Function
            End If
            '-------------------------------------------------------------
            'Asigna datos a la estructura desde la base de datos
            '-------------------------------------------------------------
            Dim refclas2 As New ClassAlmacenamiento
            Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
            Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
            Dim stru_produccion_indice As stru_produccion_indice = Nothing
            Dim Class_ra_cert_indice_expediente As New Class_ra_cert_indice_expediente
            If option_unidad_conservacion <> 0 Then
                Result = refclas2.Solicita_datos_unidad_conservacion_estructura_base_datos(matri_gestion_antigua,
                                                                                           Nombre_Gabinete,
                                                                                           Id_Documento)
                If Result <> "YES" Then
                    EliminarDocumentosGabinete = Result
                    Exit Function
                End If
                Result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(Id_Documento,
                                                                                     Nombre_Gabinete,
                                                                                     matri_gestion_antigua)
                If Result <> "YES" Then
                    EliminarDocumentosGabinete = Result
                    Exit Function
                End If
                If matri_gestion_antigua.ID_EXPEDIENTE <> 0 Then
                    Dim ref_clas_expediente As New ClassGaExpediente
                    Result = ref_clas_expediente.SolicitaDatosEstructuraExpediente(matri_gestion_antigua.ID_EXPEDIENTE,
                                                                                   estru_unidad_conservacion)
                    If Result <> "YES" Then
                        EliminarDocumentosGabinete = "Inconsistencia verficando propiedades del expediente  " & Result
                        Exit Function
                    End If
                    If estru_unidad_conservacion(0).ESTADO_EXPEDIENTE <> 1 Then
                        EliminarDocumentosGabinete = "No se puede eliminar el documento debido a que puede estar cerrado"
                        Exit Function
                    End If
                    If estru_unidad_conservacion(0).ESTADO_EXPEDIENTE <> 0 And estru_unidad_conservacion(0).estado_expediente_electronico = 2 Then
                        Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
                        If Result <> "YES" Then
                            EliminarDocumentosGabinete = Result
                            Exit Function
                        End If
                        Dim disco_carpeta_ As String = stru_ruta_expediente_.DISCO
                        Dim class_zerro_fill_ As New Class_zero_fill
                        Result = class_zerro_fill_.zero_fill(disco_carpeta_, 9, "0")
                        If Result <> "YES" Then
                            EliminarDocumentosGabinete = Result
                            Exit Function
                        End If
                        Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
                        If Directory.Exists(Ruta_expediente) = False Then
                            EliminarDocumentosGabinete = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
                            Exit Function
                        End If
                        Ruta_expediente = Ruta_expediente & disco_carpeta_
                        If Directory.Exists(Ruta_expediente) = False Then
                            Directory.CreateDirectory(Ruta_expediente)
                        End If
                        Dim expediente_zero_fil As String = estru_unidad_conservacion(0).ID_EXPEDIENTE.ToString
                        Result = class_zerro_fill_.zero_fill(expediente_zero_fil, 9, "0")
                        If Result <> "YES" Then
                            EliminarDocumentosGabinete = Result
                            Exit Function
                        End If
                        Ruta_archivo_xml = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
                    End If
                End If
            End If
            If option_aplica_trd <> 0 Then
                Result = refclas2.Solicita_datos_gestion_estructura_base_datos(matri_gestion_antigua,
                                                                               Nombre_Gabinete,
                                                                               Id_Documento)
                If Result <> "YES" Then
                    EliminarDocumentosGabinete = Result
                    Exit Function
                End If
            End If
            '----------------------------------------------------
            'Solicita id inventario
            '----------------------------------------------------
            If opcion_inventario = 1 Then
                If HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = 0 Then
                    EliminarDocumentosGabinete = "El usuario docuArchi debe estar asociado a un usuario de gestión  "
                    Exit Function
                End If
                '-----------------------------------------------------
                'Retorna el id del inventario del documento
                '-----------------------------------------------------
                Dim refclasconsulta As New ClassConsultaGabinete
                Result = refclasconsulta.verifica_exitencia_valor_invnetario_gabinete(Nombre_Gabinete,
                                                                                      Id_Documento,
                                                                                      id_inventario)
                If Result <> "YES" Then
                    EliminarDocumentosGabinete = Result
                    Exit Function
                End If
                '---------------------------------------------------
                'Retorna numero radicado
                '---------------------------------------------------
                Dim refclasgestion As New ClassGestionDocumental
                Result = refclasgestion.Retorna_numero_radicado_inventario(id_inventario,
                                                                               radicado)
                If Result <> "YES" Then
                    EliminarDocumentosGabinete = Result
                    Exit Function
                End If
                If option_verfica_radicado <> 0 And master_eliminacion = 0 Then
                    If radicado <> "" Then
                        EliminarDocumentosGabinete = "El documento esta relacionado con el radicado " & radicado &
                        " es posible que pertenezca a un flujo documental " &
                        " el documento no se puede eliminar"
                        Exit Function
                    End If
                End If
                Result = Class_ra_cert_indice_expediente.Solicita_existencia_indice_produccion(id_inventario,
                                             id_registro_indice)
                If Result <> "YES" Then
                    EliminarDocumentosGabinete = Result
                    Exit Function
                End If
            End If
        Catch ex As Exception
            EliminarDocumentosGabinete = ex.Message
            Exit Function
        End Try
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim mySqldatReader As MySqlDataReader
        Dim xmlArchivo As New XmlDocument
        Dim Datset As DataSet = New DataSet("ra_cert_indice_expediente")
        Dim estado_eliminacion_indice As Integer = 0
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim time1al As String = Date.Now.ToString
        Result = ref_ClassGestionFechas.Formatea_fecha_time_framework(Date.Now,
                                                                     time1al)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = Result
            Exit Function
        End If
        Dim TempoFecha As String = Left(time1al, 10)
        time1al = Trim(time1al.Replace(TempoFecha, ""))
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = "Error formatenado fecha  log"
            Exit Function
        End If

        Dim stru_paramter_image As stru_paramter_image = Nothing
        Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(Nombre_Gabinete,
                                                                 Id_Documento,
                                                                 stru_paramter_image,
                                                                 option_aplica_trd,
                                                                 1)
        If Result <> "YES" Then
            EliminarDocumentosGabinete = Result
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
        Try
            Dim myCommand2 As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand2.Connection = myConnection
            myCommand2.Transaction = myTrans
            '--------------------------------------
            'Actualiza numero imagenes en la tabla
            'disco detalle
            '--------------------------------------
            Dim Parametro_A As String = "select NUMERO_IMAGENES  from disco_detalle  where disco = '" & Matri_Dat_Sistema(1) & "'" &
            " and gabinete ='" & Nombre_Gabinete & "'" & " for update "
            myCommand2.CommandText = Parametro_A
            mySqldatReader = myCommand2.ExecuteReader()
            If mySqldatReader Is Nothing Then
                EliminarDocumentosGabinete = "Error sql para encontrar disco commando " & Parametro_A
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If mySqldatReader.HasRows = False Then
                EliminarDocumentosGabinete = "Imposible Encontrar registro en la tabla disco detalle"
                mySqldatReader.Close()
                myConnection.Close()
                Exit Function
            End If
            mySqldatReader.Read()
            If mySqldatReader.IsDBNull(0) = True Then
                EliminarDocumentosGabinete = "El disco " & Matri_Dat_Sistema(1) & " no esta sincronizado para alamcenar contacte a su administrador estado null"
                mySqldatReader.Close()
                myConnection.Close()
            End If
            Dim IncreNumPage As Integer = mySqldatReader.Item(0)
            If IncreNumPage > Val(Matri_Dat_Sistema(2)) Then
                IncreNumPage = IncreNumPage - Val(Matri_Dat_Sistema(2))
            End If
            mySqldatReader.Close()
            '-----------------------------
            'Elimina registro de las 
            'imagenes en la base de datos
            '-----------------------------
            Dim SqlDelete As String = "Delete From " & Nombre_Gabinete & " where id =" & Id_Documento & " or dbt=" & Id_Documento
            myCommand2.CommandText = SqlDelete
            Dim Swicth As Integer = 0
            Swicth = myCommand2.ExecuteNonQuery()
            '------------------------------------
            'Determina si se inserto el registro
            'el nuevo id de la base de datos
            '------------------------------------
            If Swicth = 0 Then
                myTrans.Rollback()
                myConnection.Close()
                EliminarDocumentosGabinete = "Imposible eliminar registro  : " & SqlDelete
                Exit Function
            End If
            '----------------------------------------
            'Actualiza la base de datos con el nuevo
            'numero de imagenes tabla disco detalle
            '----------------------------------------
            Dim SqlActualiza As String = "Update disco_detalle set NUMERO_IMAGENES=" & IncreNumPage & " where disco = '" & Matri_Dat_Sistema(1) & "'" &
                        " and gabinete ='" & Nombre_Gabinete & "'"
            myCommand2.CommandText = SqlActualiza
            Swicth = myCommand2.ExecuteNonQuery()
            If Swicth = 0 Then
                EliminarDocumentosGabinete = "Imposible actualizar disco detalle  : " & SqlActualiza
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------------
            'Registra log de la transsacion  
            '----------------------------------------
            Dim Campos As String = ""
            For k As Integer = 8 To UBound(Matri_Dat_Sistema)
                If Campos.Length = 250 Then
                    Exit For
                End If
                Campos = Campos & "¬" & Matri_Dat_Sistema(k)
            Next
            Dim hor As New System.DateTime
            hor = Date.Now
            Dim Rut_docu As String = Matri_Documentos_Eliminados(1).Replace("\", "/")
            Dim hora As String = hor.Hour.ToString & ":" & hor.Minute.ToString & ":" & hor.Second.ToString
            Dim SqlTransac As String = " INSERT INTO logdocuarchi (id_tran,desc_op,USER_OPER,DATE_TRANS," _
            & "RUT_DOCU,MODULO_REGISTRO,GABINETE,CAMPOS,IP_TRANS,HORA_REGISTRO,RADICADO,ID_TAREA_WF,ID_RUTA_WF,USER_PROPIETARIO,TIPOLOGIA_DOCUMENTAL) VALUES ( "
            SqlTransac = SqlTransac & "'" & Id_Documento & "',"
            SqlTransac = SqlTransac & "'" & "Eliminar" & "',"
            SqlTransac = SqlTransac & "'" & HttpContext.Current.Session.Item("DA_Login_Usuario") & "',"
            SqlTransac = SqlTransac & "'" & date1al & "',"
            SqlTransac = SqlTransac & "'" & Rut_docu & "',"
            SqlTransac = SqlTransac & "'" & modulo_log & "',"
            SqlTransac = SqlTransac & "'" & Nombre_Gabinete & "',"
            SqlTransac = SqlTransac & "'" & Campos & "','" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hora & "','" & radicado & "'," &
                id_tarea_wf & "," & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & "," & ref_user & "," & ref_Tipologia & ")"
            myCommand2.CommandText = SqlTransac
            Swicth = myCommand2.ExecuteNonQuery()
            If Swicth = 0 Then
                EliminarDocumentosGabinete = "Imposible actualiza tabla log  : " & SqlTransac
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            '****************************************
            'Elimina inventario documental
            '****************************************
            If opcion_inventario <> 0 And id_inventario <> 0 Then
                SqlTransac = "delete from registro_producion_documental where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_inventario
                myCommand2.CommandText = SqlTransac
                Swicth = myCommand2.ExecuteNonQuery()
                If Swicth = 0 Then
                    EliminarDocumentosGabinete = "Imposible eliminar el registro del inventario documental : " & SqlTransac
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                '*************************************
                'Registra log invnetario
                '*************************************
                Dim detalle_transacion As String = "ELIMINA INVENTARIO " &
                                         id_inventario & " de la imagen " & Id_Documento & " del gabinete " & Nombre_Gabinete
                SqlTransac = "INSERT INTO ra_log_inventario (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_REGISTRO_PRODUCCION" &
                                  ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values " &
                                     "('" & "ELIMINA" & "','" & HttpContext.Current.Session.Item("GA_LOGINUSUARIOGESTION") & "','" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "','" & date1al & "'," &
                                         id_inventario & ",'" & HttpContext.Current.Session.Item("ip_host_name") & "','" & hor & "','DOCUARCHI','" & detalle_transacion & "')"
                myCommand2.CommandText = SqlTransac
                Swicth = myCommand2.ExecuteNonQuery()
                If Swicth = 0 Then
                    EliminarDocumentosGabinete = "Imposible eliminar el registro del inventario documental : " & SqlTransac
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim update_indice As String = ""
                Dim pagina_final_indice_expediente = 0
                If matri_gestion_antigua.ID_EXPEDIENTE <> 0 Then
                    If estru_unidad_conservacion(0).ESTADO_EXPEDIENTE <> 0 And estru_unidad_conservacion(0).estado_expediente_electronico = 2 Then
                        Dim ORDEN_INDICE As Integer = 0
                        Dim ULTIMA_PAGINA_INDICE As Integer = 0
                        Dim Parametro_orden_indice As String = " SELECT ORDEN_INDICE,ULTIMA_PAGINA_INDICE" &
                        " FROM expediente_archivo where ID_EXPEDIENTE = " _
                         & estru_unidad_conservacion(0).ID_EXPEDIENTE & " " & " for update"
                        myCommand2.CommandText = Parametro_orden_indice
                        mySqldatReader = myCommand2.ExecuteReader()
                        If mySqldatReader Is Nothing Then
                            EliminarDocumentosGabinete = "Imposible encontrar el indice del documento "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        If mySqldatReader.HasRows = False Then
                            EliminarDocumentosGabinete = "Imposible Encontrar el registro de la unidad de conservación"
                            mySqldatReader.Close()
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else
                            mySqldatReader.Read()
                            ORDEN_INDICE = mySqldatReader.Item(0)
                            ULTIMA_PAGINA_INDICE = mySqldatReader.Item(1)
                            mySqldatReader.Close()
                        End If
                        '------------------------------------------------------
                        'Actualiza foliado de expediente y orden de documentos
                        'en la tabla indice
                        '------------------------------------------------------
                        Dim Parametro_Consulta = "select * " &
                        " from ra_cert_indice_expediente  where expediente_archivo_ID_EXPEDIENTE=" & estru_unidad_conservacion(0).ID_EXPEDIENTE
                        Dim ref2 As New conect.Dbase_Conction_Mysql_RA
                        Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
                        If Result <> "YES" Then
                            myTrans.Rollback()
                            myConnection.Close()
                            EliminarDocumentosGabinete = "Funcion  EliminarDocumentosGabinete dice " & Result
                            Exit Function
                        End If
                        If Datset.Tables(0).Rows.Count > 1 Then
                            Dim index As Integer = 0
                            For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                                If Datset.Tables(0).Rows(i).Item("id_cert_indice_expediente") = id_registro_indice Then
                                    index = i
                                    Exit For
                                End If
                            Next
                            Dim mat As String = ""
                            index = index + 1
                            Dim pagina_inicial_cache As Integer = 0
                            Dim pagina_inicial_cache_max As Integer = 0
                            '------------------------------------------------------------
                            'Caso Ultimo indice
                            '------------------------------------------------------------
                            If index = Datset.Tables(0).Rows.Count Then
                                pagina_final_indice_expediente = Datset.Tables(0).Rows(index - 2).Item("pagina_final")
                            End If
                            '-------------------------------------------------------------
                            'Caso indice intermedio
                            '-------------------------------------------------------------
                            If index <> Datset.Tables(0).Rows.Count Then
                                For i As Integer = index To Datset.Tables(0).Rows.Count - 1
                                    Dim orden_anterior = Datset.Tables(0).Rows(i).Item("orden_documento_expedicion")
                                    Datset.Tables(0).Rows(i).Item("orden_documento_expedicion") = orden_anterior - 1
                                    Dim pagina_inicial_anterior = Datset.Tables(0).Rows(i - 1).Item("pagina_inicial")
                                    If pagina_inicial_cache = 0 Then
                                        pagina_inicial_cache = Datset.Tables(0).Rows(i).Item("pagina_inicial")
                                        Datset.Tables(0).Rows(i).Item("pagina_inicial") = pagina_inicial_anterior
                                        Dim numero_paginas = Datset.Tables(0).Rows(i).Item("numero_folios")
                                        If numero_paginas = 1 Then
                                            Datset.Tables(0).Rows(i).Item("pagina_final") = Datset.Tables(0).Rows(i).Item("pagina_inicial")
                                        Else
                                            Datset.Tables(0).Rows(i).Item("pagina_final") = (Datset.Tables(0).Rows(i).Item("pagina_inicial") + numero_paginas) - 1
                                        End If
                                        pagina_final_indice_expediente = Datset.Tables(0).Rows(i).Item("pagina_final")
                                    Else
                                        Dim pagina_final_anterior = Datset.Tables(0).Rows(i - 1).Item("pagina_final")
                                        Datset.Tables(0).Rows(i).Item("pagina_inicial") = pagina_final_anterior + 1
                                        Dim numero_paginas = Datset.Tables(0).Rows(i).Item("numero_folios")
                                        If numero_paginas = 1 Then
                                            Datset.Tables(0).Rows(i).Item("pagina_final") = Datset.Tables(0).Rows(i).Item("pagina_inicial")
                                        Else
                                            Datset.Tables(0).Rows(i).Item("pagina_final") = (Datset.Tables(0).Rows(i).Item("pagina_inicial") + numero_paginas) - 1
                                        End If
                                        pagina_final_indice_expediente = Datset.Tables(0).Rows(i).Item("pagina_final")
                                    End If
                                    If update_indice = "" Then
                                        update_indice = "update ra_cert_indice_expediente set orden_documento_expedicion=" & Datset.Tables(0).Rows(i).Item("orden_documento_expedicion") &
                                            ",pagina_inicial=" & Datset.Tables(0).Rows(i).Item("pagina_inicial") & ", pagina_final=" & Datset.Tables(0).Rows(i).Item("pagina_final") &
                                            " where id_cert_indice_expediente=" & Datset.Tables(0).Rows(i).Item("id_cert_indice_expediente")
                                    Else
                                        update_indice = update_indice & "; update ra_cert_indice_expediente set orden_documento_expedicion=" & Datset.Tables(0).Rows(i).Item("orden_documento_expedicion") &
                                           ",pagina_inicial=" & Datset.Tables(0).Rows(i).Item("pagina_inicial") & ", pagina_final=" & Datset.Tables(0).Rows(i).Item("pagina_final") &
                                            " where id_cert_indice_expediente=" & Datset.Tables(0).Rows(i).Item("id_cert_indice_expediente")
                                    End If
                                Next
                            End If
                        End If
                        If Datset.Tables(0).Rows.Count = 1 Then
                            pagina_final_indice_expediente = 0
                        End If
                        '-------------------------------------------------------------------------
                        'Elimina indice documento en la tabla indice
                        '-------------------------------------------------------------------------
                        'Dim sql_delete As String = "delete from ra_cert_indice_expediente where id_cert_indice_expediente=" & id_registro_indice
                        If update_indice <> "" Then
                            myCommand2.CommandText = update_indice
                            Swicth = myCommand2.ExecuteNonQuery()
                            If Swicth = 0 Then
                                EliminarDocumentosGabinete = "Imposible actualizar el indice del dexpediente   : " & update_indice
                                myTrans.Rollback()
                                myConnection.Close()
                                Exit Function
                            End If
                        End If
                        '--------------------------------------------------------------------------
                        'Actualiza el orden y la utima pagina en el expediente
                        '--------------------------------------------------------------------------
                        ORDEN_INDICE = ORDEN_INDICE - 1
                        Dim update_orden_ultima_pagina As String = " UPDATE expediente_archivo " &
                          " SET ORDEN_INDICE=" & ORDEN_INDICE & " , ULTIMA_PAGINA_INDICE=" & pagina_final_indice_expediente &
                          "  where ID_EXPEDIENTE = " _
                          & estru_unidad_conservacion(0).ID_EXPEDIENTE
                        myCommand2.CommandText = update_orden_ultima_pagina
                        Swicth = myCommand2.ExecuteNonQuery()
                        If Swicth = 0 Then
                            EliminarDocumentosGabinete = "Imposible actualizar el orden del indice en el expediente " & update_orden_ultima_pagina
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        End If
                        '----------------------------------------------------------------------------
                        'Actualiza indice archivo expediente
                        '-----------------------------------------------------------------------------
                        Dim classgaexpediente As New ClassGaExpediente
                        Result = classgaexpediente.Elimina_indice_archivo_xml_expediente(Ruta_archivo_xml,
                                                                                         id_inventario,
                                                                                         xmlArchivo)
                        If Result <> "YES" Then
                            EliminarDocumentosGabinete = "Funcion  EliminarDocumentosGabinete dice " & Result
                            Exit Function
                        End If
                        estado_eliminacion_indice = 1
                    End If
                End If
            End If
            '//---------Elmina el registro de las versiones relacionadas-----//
            If Not Stru_registro_version_documento Is Nothing Then
                SqlTransac = "Delete from ra_ver_version_documento where id=" & Id_Documento & " and system1_id_gabinete=" & IdGabinete
                myCommand2.CommandText = SqlTransac
                Swicth = myCommand2.ExecuteNonQuery()
                If Swicth = 0 Then
                    EliminarDocumentosGabinete = "Imposible eliminar el registro de versiones : " & SqlTransac
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                SqlTransac = "Delete from ra_ver_control_version_documento where id_imagen=" & Id_Documento & " and system1_id_gabinete=" & IdGabinete
                myCommand2.CommandText = SqlTransac
                Swicth = myCommand2.ExecuteNonQuery()
                If Swicth = 0 Then
                    EliminarDocumentosGabinete = "Imposible eliminar el registro de control de versiones : " & SqlTransac
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '----------------------------------------
            'Decrementa unidad de conservacion
            '----------------------------------------
            Dim sqlconuslta As String = ""
            Dim Numero_Digitalizado_contenido_antiguo As Integer = 0
            Dim Numero_Electronico_contenido_antiguo As Integer = 0
            If option_unidad_conservacion <> 0 Then
                If matri_gestion_antigua.ID_EXPEDIENTE <> 0 _
                Or matri_gestion_antigua.ID_UNIDAD_CONSERVACION <> 0 Then
                    Dim refclastrd As New ClassTrdDocumental
                    Dim unidad_conserva_tipo_antiguo As String = ""
                    Result = refclastrd.Retorna_unidad_conserva_tipo_documento(matri_gestion_antigua.ID_CLASE_DOCUMENTO,
                                                                               unidad_conserva_tipo_antiguo)
                    If Result <> "YES" Then
                        myConnection.Close()
                        myTrans.Rollback()
                        EliminarDocumentosGabinete = Result
                        Exit Function
                    End If
                    If matri_gestion_antigua.ID_EXPEDIENTE <> 0 Then
                        sqlconuslta = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                        " FROM expediente_archivo where ID_EXPEDIENTE = " _
                       & "'" & matri_gestion_antigua.ID_EXPEDIENTE & "' " & "for update"
                    Else
                        sqlconuslta = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                                    " FROM unidad_conservacion where ID_UNIDAD_CONSERVACION = " _
                                    & "'" & matri_gestion_antigua.ID_UNIDAD_CONSERVACION & "' " & "for update"
                    End If
                    myCommand2.CommandText = sqlconuslta
                    mySqldatReader = myCommand2.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        EliminarDocumentosGabinete = "Imposible encontrar la identificación del expediente por conexión caso 1 decrementar"
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        EliminarDocumentosGabinete = "Imposible Encontrar el registro del expediente caso 1 decrementar"
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
                    '-------------------------------------------
                    'Arma update del conteo del expediente 
                    '-------------------------------------------
                    Dim update_sql As String = ""
                    If matri_gestion_antigua.ID_EXPEDIENTE <> 0 Then
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
                    Else
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
                    End If
                    If update_sql <> "" Then
                        myCommand2.CommandText = update_sql
                        Swicth = myCommand2.ExecuteNonQuery()
                        If Swicth = 0 Then
                            EliminarDocumentosGabinete = "Imposible Actualizar numero de folios de la unidad de conservación "
                            myTrans.Rollback()
                            myConnection.Close()
                            Exit Function
                        Else

                        End If
                    End If
                End If
            End If
            '----------------------------------------
            'Salva archivo xml
            '----------------------------------------
            If estado_eliminacion_indice = 1 Then
                xmlArchivo.Save(Ruta_archivo_xml)
            End If
            myTrans.Commit()
            myConnection.Close()
            '*****************************************
            'Elimina archivos
            '*****************************************
            Dim RefclasAlmacen As New ClassAlmacenamiento
            If HttpContext.Current.Session.Item("ESTADOFILESERVER") = "1" Then
                For i As Integer = 0 To UBound(Matri_Doc_Eliminar)
                    If File.Exists(Matri_Doc_Eliminar(i)) = True Then
                        Dim RefFileInf As New FileInfo(Matri_Doc_Eliminar(i))
                        Dim Ruta_Doc_Elimimina As String = RefFileInf.FullName.Replace(RefFileInf.Extension, "*")
                        Kill(Ruta_Doc_Elimimina)
                        Dim Doc_Xml As String = ""
                        Result = RefclasAlmacen.Arma_Nombre_Archivo_Preindex(Matri_Doc_Eliminar(i), Doc_Xml)
                        If Result = "YES" Then
                            Dim Rutaxml As String = RefFileInf.FullName.Replace(RefFileInf.Name, Doc_Xml)
                            If File.Exists(Rutaxml) = True Then
                                Kill(Rutaxml)
                            End If
                        End If
                    End If
                Next
            End If
            '//----------Elimina archivos de versiones------///
            If Not Stru_registro_version_documento Is Nothing Then
                For i As Integer = 0 To Stru_registro_version_documento.Length - 1
                    For k As Integer = 0 To Stru_registro_version_documento(i).EstruDocumentosRelacionados.Length - 1
                        If File.Exists(Stru_registro_version_documento(i).EstruDocumentosRelacionados(k)) = True Then
                            Kill(Stru_registro_version_documento(i).EstruDocumentosRelacionados(k))
                        End If
                    Next
                Next
            End If
            EliminarDocumentosGabinete = "YES"
            Exit Function
        Catch e As Exception
            Try
                myConnection.Close()
                If Not mySqldatReader Is Nothing Then
                    mySqldatReader.Close()
                End If
                myTrans.Rollback()
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    EliminarDocumentosGabinete = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            EliminarDocumentosGabinete = "Error General " & e.Message
            Exit Function
        End Try

    End Function





    Function Restriccion_eliminar_documento_producción_radicado(ByVal id_imagen As Integer,
                                                                ByVal nombre_gabinete As String,
                                                                ByRef radicado As String) As String
        Try
            Dim Result As String = ""
            Dim Refclass As New ClassGaProducionDocumental
            '-----------------------------------------------
            'Solicita el registro de producción documental 
            'de un documento o imagen
            '---------------------------------------------
            Dim id_registro_produccion_documental As Long = 0
            Result = Refclass.Solicita_id_registro_producion_documental(id_imagen,
                                                                        nombre_gabinete,
                                                                        id_registro_produccion_documental)
            If Result <> "YES" Then
                Restriccion_eliminar_documento_producción_radicado = Result
                Exit Function
            End If
            If id_registro_produccion_documental = 0 Then
                radicado = ""
                Restriccion_eliminar_documento_producción_radicado = "YES"
                Exit Function
            End If
            '-----------------------------------------------
            'Solicita el radicado relacionado al registro
            'de producción documental
            '-----------------------------------------------
            Result = Refclass.Solicita_relacion_registro_produccion_documental_con_radicado_interno(id_registro_produccion_documental,
                                                                                                    radicado)
            If Result <> "YES" Then
                Restriccion_eliminar_documento_producción_radicado = Result
                Exit Function
            Else
                Restriccion_eliminar_documento_producción_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Restriccion_eliminar_documento_producción_radicado = "Inconsistencia general función Restriccion_eliminar_documento_producción_radicado " & ex.Message
        End Try
    End Function
End Class
