Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Public Class class_ra_m_meta_archivo_
    Public ra_m_id As Integer
    Public Meta_dato As String
    Public Valor_meta_dato As String
    Public Estado_obligatorio As String
    Public Tipo As String
    Public Estandar_meta_dato As String
    Public descripcion As String
    Public ERROR_SERVICE As String
    Public CONTEXTO As String
End Class
Public Class Class_ra_m_registro_meta_dato_archivo
    Function Solicita_existencia_registro_sistema_meta_dato_Archivo(ByVal id_registro_produccion As Long,
                                                                    ByRef id_registro_meta_archivo_meta_dato As Long,
                                                                    ByRef id_sistema_meta_dato As Integer) As String
        '-----------------------------------------------
        'Function : Solicita la existencia del registro
        'de meta datos de un archivo de producción
        'documental
        'Fecha : 2022-02-08
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select id_registro_meta_dato_archivo,ra_m_sistema_meta_datos_id_sistema_meta_datos " &
                "from ra_m_registro_meta_dato_arhivo where registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_produccion
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_registro_sistema_meta_dato_Archivo = "Functión Solicita_existencia_registro_sistema_meta_dato_Archivo dice   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_registro_meta_archivo_meta_dato = 0
                id_sistema_meta_dato = 0
                Solicita_existencia_registro_sistema_meta_dato_Archivo = "YES"
                Exit Function
            Else
                id_registro_meta_archivo_meta_dato = Datset.Tables(0).Rows(0).Item(0)
                id_sistema_meta_dato = Datset.Tables(0).Rows(0).Item(1)
                Solicita_existencia_registro_sistema_meta_dato_Archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_registro_sistema_meta_dato_Archivo = "Inconsistencia general funcion Solicita_existencia_registro_sistema_meta_dato_Archivo " & ex.Message
        End Try
    End Function
    Function Solicita_existencia_meta_dato_archivo(ByVal id_registro_produccion As Long,
                                                   ByRef id_sistema_meta_datos As Integer) As String
        Try
            Dim Parametro_Consulta As String = "Select ra_m_sistema_meta_datos_id_sistema_meta_datos  from ra_m_registro_meta_dato_arhivo " &
                " where registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_produccion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_m_sistema_meta_datos")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_existencia_meta_dato_archivo = "Función Solicita_existencia_meta_dato_archivo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_sistema_meta_datos = Datset.Tables(0).Rows(0).Item(0)
                Solicita_existencia_meta_dato_archivo = "YES"
                Exit Function
            Else
                id_sistema_meta_datos = 0
                Solicita_existencia_meta_dato_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_existencia_meta_dato_archivo = "Inconsistencia general función Solicita_existencia_meta_dato_archivo " & ex.Message
        End Try
    End Function

    Function Solicita_nombre_archivo_xml_meta_dato_archivo(ByVal id_imagen As Integer,
                                                           ByVal nombre_gabinete As String,
                                                           ByVal ruta_almacenamiento As String,
                                                           ByRef nombre_archivo_xml_meta_dato As String) As String
        Try
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            Dim class_zerro_fill_ As New Class_zero_fill
            Dim expediente_zero_fil As String = id_imagen.ToString
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Dim Result As String = ""
            Result = ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(nombre_gabinete,
                                                                                          id_imagen,
                                                                                          stru_paramter_image, 0)
            If Result <> "YES" Then
                Solicita_nombre_archivo_xml_meta_dato_archivo = Result
                Exit Function
            End If
            Dim cerros_carpeta As String = ""
            Result = Ceros_Imagen_Carpeta(stru_paramter_image.IDEX,
                                          cerros_carpeta)
            If Result <> "YES" Then
                Solicita_nombre_archivo_xml_meta_dato_archivo = Result
                Exit Function
            End If
            Dim Ceros_Cuerpo_Imag As String = ""
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, id_imagen)
            If Result <> "YES" Then
                Solicita_nombre_archivo_xml_meta_dato_archivo = Result
                Exit Function
            End If
            Dim Disco_Documento As String = nombre_gabinete & stru_paramter_image.DISC
            nombre_archivo_xml_meta_dato = ruta_almacenamiento & Disco_Documento & "\" & cerros_carpeta & stru_paramter_image.IDEX & "\" & "DIG" & Ceros_Cuerpo_Imag & id_imagen & ".xml"
            Solicita_nombre_archivo_xml_meta_dato_archivo = "YES"
            Exit Function
        Catch ex As Exception
            Solicita_nombre_archivo_xml_meta_dato_archivo = "Inconsistencia general funcion Solicita_nombre_archivo_xml_meta_dato_archivo " & ex.Message
        End Try
    End Function
    Function Agrega_meta_dato_documento(ByVal id_imagen As Integer,
                                        ByVal gabinete As String,
                                        ByVal radicado As String,
                                        ByVal id_tarea As Object,
                                        ByVal id_registro_version As Integer,
                                        ByVal modulo_funcion As Integer,
                                        ByVal valida_firma_digital As Integer,
                                        ByVal valida_meta_dato_obligatorio As Integer,
                                        ByVal valida_expediente_obligatorio As Integer,
                                        ByRef stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_) As String
        '--------------------------------------------------------------
        'Funcion : Agrega meta datos al archivo con la opción de firmar
        'digital mente el documento, la identifcación de los diferentes
        'modulo es las siguinete 1.Workflow 2. Porduccion documental
        '3. Docuarchi.net
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-16
        '---------------------------------------------------------------
        Try
            Dim Ref_ClassGaProducionDocumental As New ClassGaProducionDocumental
            Dim Class_ra_cert_registro_certificado_archivo As New Class_ra_cert_registro_certificado_archivo
            Dim Result As String = ""
            Dim id_registro_produccion As Long = 0
            Dim id_certificado_archivo_registro As Integer = 0
            Result = Ref_ClassGaProducionDocumental.Solicita_id_registro_producion_documental(id_imagen,
                                                                                              gabinete,
                                                                                              id_registro_produccion)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            If id_registro_produccion = 0 Then
                Agrega_meta_dato_documento = "El documento (" & id_imagen & ") no esta relacionado en el registro de produución documental gabinete (" & gabinete & ")"
                Exit Function
            End If
            Dim id_expediente As Integer = 0
            Result = Ref_ClassGaProducionDocumental.Solicita_id_expediente_registro_produccion(id_registro_produccion,
                                                                                               id_expediente,
                                                                                               valida_expediente_obligatorio)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            If valida_firma_digital = 1 Then
                '------//Valida permiso firma workflow
                If modulo_funcion = 1 Then
                    If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_WF") = 0 Then
                        Agrega_meta_dato_documento = "El usuario workflow no tiene permiso para agregar firma digital"
                        Exit Function
                    End If
                End If
                '------//Valida permiso produccion
                If modulo_funcion = 2 Then
                    If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_GD") = 0 Then
                        Agrega_meta_dato_documento = "El usuario de gestión no tiene permiso para agregar firma digital"
                        Exit Function
                    End If
                End If
                '------//Valida permiso docuarchi   
                If modulo_funcion = 3 Then
                    If HttpContext.Current.Session.Item("FIRMA_DIGITAL_DOCUMENTO_DA") = 0 Then
                        Agrega_meta_dato_documento = "El usuario docuarchi no tiene permiso para agregar firma digital"
                        Exit Function
                    End If
                End If
                Result = Class_ra_cert_registro_certificado_archivo.Solicita_registro_certificado_archivo(id_registro_produccion,
                                                                                                          id_registro_version,
                                                                                                          id_certificado_archivo_registro)
                If Result <> "YES" Then
                    Agrega_meta_dato_documento = Result
                    Exit Function
                End If
                If id_certificado_archivo_registro <> 0 Then
                    Agrega_meta_dato_documento = "El archivo se encuentra firmado digitalmente"
                    Exit Function
                End If
            End If
            '-----------------Asigna informacion dinamica de auto poblado (gabinete, ruta workflow, radicado, expediente)
            Dim Class_ra_m_auto_evento As New Class_ra_m_auto_evento
            Dim Class_ra_m_auto_tip_evento As New Class_ra_m_auto_tip_evento
            Dim Class_ra_m_auto_tip_evento_ As Class_ra_m_auto_tip_evento = Nothing
            Dim conexion_db As String = ""
            For i As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                '---------------Solicita detalle evento del meta dato "id tipo evento, nombre tabla evento, nombre campo parametro, nombre campo retorno "
                Dim Class_ra_m_auto_evento_ As Class_ra_m_auto_evento = Nothing
                Result = Class_ra_m_auto_evento.Solicita_detalle_evento_auto_poblado(stru_detalle_sis_meta_dato(i).id_detalle_sistema_meta_datos,
                                                                                     Class_ra_m_auto_evento_)
                If Result <> "YES" Then
                    Agrega_meta_dato_documento = Result
                    Exit Function
                End If
                If Not Class_ra_m_auto_evento_ Is Nothing Then
                    Dim valor_patametro As Object = Nothing
                    Dim valor_auto_pobaldo As String = ""
                    '-----------Solicita estructura tipo evento para saber donde buscar si en : "gabinete, ruta workflow, radicado, expediente" 
                    Result = Class_ra_m_auto_tip_evento.Solicita_estructura_tipo_evento(Class_ra_m_auto_evento_.ID_M_AUTO_TIP_EVENT,
                                                                                        Class_ra_m_auto_tip_evento_)
                    If Result <> "YES" Then
                        Agrega_meta_dato_documento = Result
                        Exit Function
                    End If
                    'Parametro gabinete "nombre campo gabinete"
                    Dim Class_RA_M_EVENTO_GABINETE As New Class_RA_M_EVENTO_GABINETE
                    If Class_ra_m_auto_tip_evento_.TIP_AUTO_TIP_EVENTO = 1 Then
                        stru_detalle_sis_meta_dato(i).descripcion_error_campo_meta_dato = " en el Gabinete " & gabinete
                        Result = Class_RA_M_EVENTO_GABINETE.Solicita_nombre_campo_gabinete(Class_ra_m_auto_tip_evento_.ID_M_AUTO_TIP_EVENT,
                                                                                           Class_ra_m_auto_evento_.ID_M_AUTO_EVENTO,
                                                                                           gabinete,
                                                                                           Class_ra_m_auto_evento_.NOMBRE_CAMPO_RETORNO)
                        If Result <> "YES" Then
                            Agrega_meta_dato_documento = Result & ", meta dato de auto poblado (" & stru_detalle_sis_meta_dato(i).nombre_meta_dato & ")"
                            Exit Function
                        Else
                            If Class_ra_m_auto_evento_.NOMBRE_CAMPO_RETORNO <> "" Then
                                Class_ra_m_auto_evento_.NOMBRE_TABLA_EVENTO = gabinete
                                Class_ra_m_auto_evento_.NOMBRE_CAMPO_PARAMETRO = "ID"
                                valor_patametro = id_imagen
                            End If
                        End If
                    End If
                    'Parametro workflow
                    If Class_ra_m_auto_tip_evento_.TIP_AUTO_TIP_EVENTO = 2 Then
                        stru_detalle_sis_meta_dato(i).descripcion_error_campo_meta_dato = " en la ruta Workflow  "
                        valor_patametro = id_tarea
                    End If
                    'Parametro radicado
                    If Class_ra_m_auto_tip_evento_.TIP_AUTO_TIP_EVENTO = 3 Then
                        stru_detalle_sis_meta_dato(i).descripcion_error_campo_meta_dato = " en el radicado en la plantilla "
                        valor_patametro = radicado
                    End If
                    'Parametro expediente
                    If Class_ra_m_auto_tip_evento_.TIP_AUTO_TIP_EVENTO = 4 Then
                        stru_detalle_sis_meta_dato(i).descripcion_error_campo_meta_dato = " en el expediente "
                        valor_patametro = id_expediente
                    End If

                    If valor_patametro IsNot Nothing Then
                        Result = Class_ra_m_auto_evento.Solicita_valor_meta_dato_auto_poblado(Class_ra_m_auto_tip_evento_,
                                                                                              Class_ra_m_auto_evento_,
                                                                                              valor_patametro,
                                                                                              valor_auto_pobaldo)
                        If Result <> "YES" Then
                            Agrega_meta_dato_documento = Result
                            Exit Function
                        End If
                        stru_detalle_sis_meta_dato(i).value = valor_auto_pobaldo
                        stru_detalle_sis_meta_dato(i).nombre_campo_retorno_meta_dato = Class_ra_m_auto_evento_.NOMBRE_CAMPO_RETORNO
                    End If

                End If
            Next
            '-------------Asigna datos heredados del expediente
            Dim ClassGaExpediente As New ClassGaExpediente
            If id_expediente <> 0 Then
                Result = ClassGaExpediente.Asigna_meta_dato_archivo_expediente_gestion(id_expediente,
                                                                                       stru_detalle_sis_meta_dato)
                If Result <> "YES" Then
                    Agrega_meta_dato_documento = Result
                    Exit Function
                End If
            End If
            Dim class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Dim Ruta_almacenamiento As String = ""
            Result = class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruta_almacenamiento,
                                                                   gabinete)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            Dim Class_ra_m_detalle_sis_meta_datos As New Class_ra_m_detalle_sis_meta_datos
            Dim id_sistema_meta_dato As Integer = 0
            Result = Class_ra_m_detalle_sis_meta_datos.Solicita_id_sistema_meta_dato_en_detalle_meta_dato(Val(stru_detalle_sis_meta_dato(0).id_detalle_sistema_meta_datos),
                                                                                                          id_sistema_meta_dato)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            Dim nombre_sistema_meta_dato As String = ""
            Dim Class_ra_m_sistema_meta_datos As New Class_ra_m_sistema_meta_datos
            Result = Class_ra_m_sistema_meta_datos.Solicita_nombre_sistema_meta_dato(id_sistema_meta_dato,
                                                                                     nombre_sistema_meta_dato)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            Dim Nombre_archivo_xml As String = ""
            Result = Me.Solicita_nombre_archivo_xml_meta_dato_archivo(id_imagen,
                                                                      gabinete,
                                                                      Ruta_almacenamiento,
                                                                      Nombre_archivo_xml)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            '-------Solicita nombre empresa meta datos estaticos---------------
            Dim formato_ As String = ""
            Dim ruta_archivo_gabinete As String = ""
            Result = Me.Asigna_meta_datos_estaticos(id_registro_produccion,
                                                    id_imagen,
                                                    gabinete,
                                                    Ruta_almacenamiento,
                                                    id_sistema_meta_dato,
                                                    formato_,
                                                    nombre_sistema_meta_dato,
                                                    stru_detalle_sis_meta_dato,
                                                    ruta_archivo_gabinete)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            If valida_meta_dato_obligatorio = 1 Then
                For i As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                    If stru_detalle_sis_meta_dato(i).estado_obliga_torio = "O" And stru_detalle_sis_meta_dato(i).value = "" Then

                        Agrega_meta_dato_documento = "El meta dato (" & stru_detalle_sis_meta_dato(i).nombre_meta_dato & ") debe ser informado, " &
                              "por favor  revisar el valor " & stru_detalle_sis_meta_dato(i).nombre_campo_retorno_meta_dato &
                            stru_detalle_sis_meta_dato(i).descripcion_error_campo_meta_dato

                        Exit Function
                    End If
                Next
            End If
            '--------Escribe en el archivo el sistema de meta datos
            'Dim Class_ra_m_extension_permitida_meta_dato As New Class_ra_m_extension_permitida_meta_dato
            'Dim matri_extension_meta_dato As String() = Nothing
            'Result = Class_ra_m_extension_permitida_meta_dato.Solicita_extension_activa_meta_dato(id_sistema_meta_dato,
            '                                                                                      matri_extension_meta_dato)
            'If Result <> "YES" Then
            '    Agrega_meta_dato_documento = Result
            '    Exit Function
            'End If
            'Dim estado_extension_archivo_permitido As String = ""
            'If Not matri_extension_meta_dato Is Nothing Then
            '    Dim format_repalce As String = formato_.Replace(".", "")
            '    For i As Integer = 0 To matri_extension_meta_dato.Length - 1
            '        If UCase(format_repalce) = UCase(matri_extension_meta_dato(i)) Then
            '            estado_extension_archivo_permitido = "YES"
            '        End If
            '    Next
            'End If
            'If estado_extension_archivo_permitido = "YES" Then

            'End If
            Result = Registra_meta_dato_archivo_xml(Nombre_archivo_xml,
                                                    stru_detalle_sis_meta_dato)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            Dim estado_formato_permitido As String = "NO"
            Dim id_certificado As Integer = 0
            Dim id_registro_firma_digital As Long = 0
            If valida_firma_digital = 1 Then
                'Result = Class_ra_cert_registro_certificado_archivo.firma_digital_documento(id_imagen,
                '                                                                            gabinete,
                '                                                                            ruta_archivo_gabinete,
                '                                                                            id_registro_produccion,
                '                                                                            1,
                '                                                                            0,
                '                                                                            estado_formato_permitido,
                '                                                                            id_certificado,
                '                                                                            id_registro_firma_digital)
                'If Result <> "YES" Then
                '    Agrega_meta_dato_documento = Result
                '    Exit Function
                'End If
            End If
            For i As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                stru_detalle_sis_meta_dato(i).ESTADO_FIRMA_DIGITAL = estado_formato_permitido
            Next
            Dim datos_meta_dato As String = ""
            Dim strcutura_xml_meta_dato As String = ""
            For i As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                If stru_detalle_sis_meta_dato(i).ESTADO_VISIBLE_METADATO = 1 And stru_detalle_sis_meta_dato(i).value <> "" Then
                    datos_meta_dato = datos_meta_dato & stru_detalle_sis_meta_dato(i).value & vbCrLf
                    Dim valor_meta_dato As String = ""
                    If stru_detalle_sis_meta_dato(i).value = "" Then
                        valor_meta_dato = "NA"
                    Else
                        valor_meta_dato = stru_detalle_sis_meta_dato(i).value
                    End If
                    Dim nombre_meta As String = stru_detalle_sis_meta_dato(i).nombre_meta_dato.ToString.Replace(" ", "_")
                    strcutura_xml_meta_dato = strcutura_xml_meta_dato & "id : " & stru_detalle_sis_meta_dato(i).id_detalle_sistema_meta_datos & vbCrLf &
                    nombre_meta & ":" & valor_meta_dato & vbCrLf &
                    "Descripcion_meta_dato :" & stru_detalle_sis_meta_dato(i).descripcion_meta_dato & vbCrLf &
                    "Estado_obligatorio :" & stru_detalle_sis_meta_dato(i).estado_obliga_torio & vbCrLf &
                    "Tipo_meta_dato :" & stru_detalle_sis_meta_dato(i).tipo_datos_meta_datos & vbCrLf &
                    "Estandar_meta_dato :" & stru_detalle_sis_meta_dato(i).ESTANDAR & vbCrLf & "|"
                End If
            Next
            datos_meta_dato = datos_meta_dato.Replace("'", "")
            strcutura_xml_meta_dato = strcutura_xml_meta_dato.Replace("'", "")
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim fech As String = ""
            Result = ref_ClassGestionFechas.Formatea_fecha_time_framework(Now.Date, fech)
            If Result <> "YES" Then
                Agrega_meta_dato_documento = Result
                Exit Function
            End If
            Dim estado_firma_digital As Integer = 0
            Dim Fecha_firma As Object = "null"
            If estado_formato_permitido = "YES" Then
                Fecha_firma = "'" & fech & "'"
                estado_firma_digital = 1
            Else
                estado_firma_digital = 2
                id_certificado = 0
            End If
            Dim sqlinsertcion As String = "insert into ra_m_registro_meta_dato_arhivo (ra_m_sistema_meta_datos_id_sistema_meta_datos," &
                "registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL,fecha_registro,meta_dato_registro,id_usuario_gestion) values (" &
                id_sistema_meta_dato & "," & id_registro_produccion & ",'" & fech & "','" & strcutura_xml_meta_dato & "'," & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ")"
            Dim sql_update As String = ""
            sql_update = "update registro_producion_documental set TEXT_META_DATO='" & datos_meta_dato & "'" &
              ",id_sistema_meta_datos=" & id_sistema_meta_dato &
              " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_registro_produccion
            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            ref.Returna_Conexion_Mysql(myConnection)
            Dim myTrans As MySqlTransaction
            Dim errorM As String = "YES"
            Try
                Dim myCommand As MySqlCommand = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand.Connection = myConnection
                myCommand.Transaction = myTrans
                myCommand.CommandText = sqlinsertcion
                Dim Switc As Integer = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Agrega_meta_dato_documento = "Imposible registrar el registro del meta dato : " & sqlinsertcion
                    myConnection.Close()
                    errorM = "Imposible registrar el registro del meta dato  : " & sqlinsertcion
                    Exit Function
                End If
                myCommand.CommandText = sql_update
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    Agrega_meta_dato_documento = "Imposible actualizar el registro del meta dato en la produccion documental  : " & sql_update
                    myTrans.Rollback()
                    myConnection.Close()
                    errorM = "Imposible actualizar el registro del meta dato en la produccion documental  : " & sql_update
                    Exit Function
                End If
                myTrans.Commit()
                myConnection.Close()
                Agrega_meta_dato_documento = "YES"
            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myConnection.Close()
                    Agrega_meta_dato_documento = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                    errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            Finally
                If Not myConnection Is Nothing Then
                    myConnection.Close()
                End If
                If errorM <> "YES" Then
                    Agrega_meta_dato_documento = errorM + sqlinsertcion
                Else
                    Agrega_meta_dato_documento = errorM
                End If
            End Try

        Catch ex As Exception
            Agrega_meta_dato_documento = "Inconsistencia general funcion Agrega_meta_dato_documento " & ex.Message
        End Try
    End Function
    Function Asigna_meta_datos_estaticos(ByVal id_registro_produccion As Long,
                                         ByVal id_imagen As Integer,
                                         ByVal gabinete As String,
                                         ByVal Ruta_almacenamiento As String,
                                         ByVal id_sistema_meta_dato As Integer,
                                         ByRef formato_ As String,
                                         ByVal nombre_sistema_meta_dato As String,
                                         ByRef stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_,
                                         ByRef ruta_archivo_gabinete As String) As String
        Try
            Dim Class_empresa_gestion_documental As New Class_empresa_gestion_documental
            Dim nombre_empresa As String = ""
            Dim identificacion As String = ""
            Dim Result As String = ""
            Result = Class_empresa_gestion_documental.Solicita_nombre_identificacion_empresa(identificacion,
                                                                                             nombre_empresa)
            If Result <> "YES" Then
                Asigna_meta_datos_estaticos = Result
                Exit Function
            End If
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Dim nombre_usuario_gestion As String = ""
            Dim cargo_usuario_gestion As String = ""
            Dim correo_electronico As String = ""
            Result = Class_remit_dest_interno.Retorna_datos_caracterizacion_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                            nombre_usuario_gestion,
                                                                                            cargo_usuario_gestion,
                                                                                            correo_electronico)
            If Result <> "YES" Then
                Asigna_meta_datos_estaticos = Result
                Exit Function
            End If
            Dim fecha_incorpora As String = ""
            Dim Ref_ClassGaProducionDocumental As New ClassGaProducionDocumental
            Result = Ref_ClassGaProducionDocumental.Solicita_fecha_incorpora_documento(id_registro_produccion,
                                                                                       fecha_incorpora)
            If Result <> "YES" Then
                Asigna_meta_datos_estaticos = Result
                Exit Function
            End If

            Dim ClassDaGabinete As New ClassDaGabinete
            'Dim ruta_archivo_gabinete As String = ""
            Result = ClassDaGabinete.Solicita_ruta_achivo_gabinete(id_imagen,
                                                                   gabinete,
                                                                   Ruta_almacenamiento,
                                                                   ruta_archivo_gabinete)
            If Result <> "YES" Then
                Asigna_meta_datos_estaticos = Result
                Exit Function
            End If
            Dim Class_system1 As New Class_system1
            Dim opt_tabla_retencion As Integer = 0
            Result = Class_system1.VerificaOpcionAplicarTablaRetencion(opt_tabla_retencion,
                                                                           gabinete)
            If Result <> "YES" Then
                Asigna_meta_datos_estaticos = Result
                Exit Function
            End If
            Dim Nombre_tipo_trd As String = ""
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Result = ClassDaGabinete.SolicitaEtructuraImagenGabinete(gabinete,
                                                                     id_imagen,
                                                                     stru_paramter_image,
                                                                     opt_tabla_retencion)
            If Result <> "YES" Then
                Asigna_meta_datos_estaticos = Result
                Exit Function
            End If
            Nombre_tipo_trd = stru_paramter_image.TIPODOCUMENTO
            Dim tam_archivo As Object = 1024
            Dim tamano As Object = 1024
            Dim nombre_imagen As String = ""
            Dim fecha_create_documento As String = ""
            Dim numero_folios_pagina As Integer = 1
            'Dim formato_ As String = ""
            Dim ClassAlmacenamiento As New ClassAlmacenamiento
            If File.Exists(ruta_archivo_gabinete) Then
                Dim fil As New FileInfo(ruta_archivo_gabinete)
                tam_archivo = fil.Length
                nombre_imagen = fil.Name
                If (tam_archivo / 1024) > 1024 Then
                    tamano = Math.Round(((tam_archivo / 1024) / 1024), 2).ToString() & " Mb"
                Else
                    tamano = Math.Round((tam_archivo / 1024), 2).ToString() & " Kb"
                End If
                fecha_create_documento = fil.CreationTime
                formato_ = fil.Extension
                Dim Class_ItexShare As New Class_ItexShare
                If fil.Extension = ".PDF" Then
                    Result = Class_ItexShare.Retorna_numero_paginas_documentos_pdf(ruta_archivo_gabinete,
                                                                                   numero_folios_pagina)
                    If Result <> "YES" Then
                        Asigna_meta_datos_estaticos = Result
                        Exit Function
                    End If
                End If
            End If

            '---------Asigna meta datos estaticos-----------------------
            For i As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Peso_byte_documento" Then
                    stru_detalle_sis_meta_dato(i).value = tam_archivo
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Id_sistema_meta_dato" Then
                    stru_detalle_sis_meta_dato(i).value = id_sistema_meta_dato
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Nombre_sistema_meta_dato" Then
                    stru_detalle_sis_meta_dato(i).value = nombre_sistema_meta_dato
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Nombre_documento" Then
                    stru_detalle_sis_meta_dato(i).value = nombre_imagen
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Fecha_creación_documento" Then
                    stru_detalle_sis_meta_dato(i).value = fecha_create_documento
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Numero_paginas_documento" Then
                    stru_detalle_sis_meta_dato(i).value = fecha_create_documento
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Numero_paginas_documento" Then
                    stru_detalle_sis_meta_dato(i).value = numero_folios_pagina
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Numero_paginas_documento" Then
                    stru_detalle_sis_meta_dato(i).value = numero_folios_pagina
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "formato" Then
                    stru_detalle_sis_meta_dato(i).value = formato_
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Ubicación" Then
                    stru_detalle_sis_meta_dato(i).value = ruta_archivo_gabinete
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Fecha_declaración" Then
                    stru_detalle_sis_meta_dato(i).value = fecha_incorpora
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Entidad_productora_documento" Then
                    stru_detalle_sis_meta_dato(i).value = nombre_empresa
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Nombre_productor_documento" Then
                    stru_detalle_sis_meta_dato(i).value = nombre_usuario_gestion
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Nombre_productor_documento" Then
                    stru_detalle_sis_meta_dato(i).value = nombre_usuario_gestion
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Cargo_productor_documento" Then
                    stru_detalle_sis_meta_dato(i).value = cargo_usuario_gestion
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Nivel_de_acceso_documento" Then
                    stru_detalle_sis_meta_dato(i).value = "na"
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Autor_" Then
                    stru_detalle_sis_meta_dato(i).value = nombre_usuario_gestion
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Fecha_" Then
                    stru_detalle_sis_meta_dato(i).value = fecha_create_documento
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Tipo documental trd" Then
                    stru_detalle_sis_meta_dato(i).value = Nombre_tipo_trd
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Numero folio" Then
                    stru_detalle_sis_meta_dato(i).value = numero_folios_pagina
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "codigo_unico" Then
                    stru_detalle_sis_meta_dato(i).value = id_registro_produccion
                End If
            Next
            Asigna_meta_datos_estaticos = "YES"
        Catch ex As Exception
            Asigna_meta_datos_estaticos = "Inconsistencia general funcion Asigna_meta_datos_estaticos " & ex.Message
        End Try
    End Function
    Function Registra_meta_dato_archivo_xml(ByVal Nombre_archivo_xml As String,
                                            ByVal stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_) As String
        Try
            If File.Exists(Nombre_archivo_xml) = True Then
                Kill(Nombre_archivo_xml)
            End If
        Catch ex As Exception

        End Try
        Dim myXmlTextWriter As XmlTextWriter = New XmlTextWriter(Nombre_archivo_xml,
                                                                  System.Text.Encoding.UTF8)
        Try
            Dim Result As String = ""
            Dim time1al As String = Date.Now.ToString
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented
            myXmlTextWriter.WriteStartDocument(False)
            myXmlTextWriter.WriteStartElement("Contenido")
            myXmlTextWriter.WriteElementString("fecha_meta_dato", time1al)
            myXmlTextWriter.WriteStartElement("meta_datos")
            If Not stru_detalle_sis_meta_dato Is Nothing Then
                For i As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                    If stru_detalle_sis_meta_dato(i).ESTADO_VISIBLE_METADATO = 1 Then
                        Dim valor_meta_dato As String = ""
                        If stru_detalle_sis_meta_dato(i).value = "" Then
                            valor_meta_dato = "NA"
                        Else
                            valor_meta_dato = stru_detalle_sis_meta_dato(i).value
                        End If
                        myXmlTextWriter.WriteStartElement("meta_dato")
                        myXmlTextWriter.WriteElementString("id", stru_detalle_sis_meta_dato(i).id_detalle_sistema_meta_datos)
                        Dim nombre_meta As String = stru_detalle_sis_meta_dato(i).nombre_meta_dato.ToString.Replace(" ", "_")
                        myXmlTextWriter.WriteElementString(nombre_meta, valor_meta_dato)
                        myXmlTextWriter.WriteElementString("Descripcion_meta_dato", stru_detalle_sis_meta_dato(i).descripcion_meta_dato)
                        myXmlTextWriter.WriteElementString("Estado_obligatorio", stru_detalle_sis_meta_dato(i).estado_obliga_torio)
                        myXmlTextWriter.WriteElementString("Tipo_meta_dato", stru_detalle_sis_meta_dato(i).tipo_meta_dato)
                        myXmlTextWriter.WriteElementString("Estandar_meta_dato", stru_detalle_sis_meta_dato(i).ESTANDAR)
                        myXmlTextWriter.WriteEndElement()
                    End If
                Next
            End If
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.Flush()
            myXmlTextWriter.Close()
            Registra_meta_dato_archivo_xml = "YES"
        Catch ex As Exception
            If Not myXmlTextWriter Is Nothing Then
                myXmlTextWriter.Close()
            End If
            Registra_meta_dato_archivo_xml = "Inconsistencia general funcion Registra_meta_dato_archivo_xml " & ex.Message
        End Try
    End Function
    Function Solicita_listar_meta_datos_Archivo(ByVal id_imagen As Integer,
                                                ByVal gabinete As String,
                                                ByRef class_ra_m_meta_archivo_ As List(Of class_ra_m_meta_archivo_)) As String
        Try
            Dim Class_SYSTEM1RUT As New Class_SYSTEM1RUT
            Dim Result As String = ""
            Dim Ruta_almacenamiento As String = ""
            Result = Class_SYSTEM1RUT.Consulta_Ruta_Almacenamiento(Ruta_almacenamiento,
                                                                   gabinete)
            If Result <> "YES" Then
                Solicita_listar_meta_datos_Archivo = Result
                Exit Function
            End If
            Dim nombre_archivo_xml As String = ""
            Result = Me.Solicita_nombre_archivo_xml_meta_dato_archivo(id_imagen,
                                                                      gabinete,
                                                                      Ruta_almacenamiento,
                                                                      nombre_archivo_xml)
            If Result <> "YES" Then
                Solicita_listar_meta_datos_Archivo = Result
                Exit Function
            End If
            If File.Exists(nombre_archivo_xml) = False Then
                Solicita_listar_meta_datos_Archivo = "Imposible encontrar el archivo xml (" & nombre_archivo_xml & ")"
                Exit Function
            End If
            Result = Me.Solicita_archivo_meta_dato_archivo(nombre_archivo_xml,
                                                           class_ra_m_meta_archivo_)
            If Result <> "YES" Then
                Solicita_listar_meta_datos_Archivo = Result
                Exit Function
            End If
            If class_ra_m_meta_archivo_.Count = 0 Then
                Solicita_listar_meta_datos_Archivo = "Imposible encontrar meta datos en el archivo (" & nombre_archivo_xml & ")"
                Exit Function
            End If
            Solicita_listar_meta_datos_Archivo = "YES"
        Catch ex As Exception
            Solicita_listar_meta_datos_Archivo = "Inconsistencia general funcion Solicita_listar_meta_datos_Archivo " & ex.Message
        End Try
    End Function
    Function Solicita_archivo_meta_dato_archivo(ByVal nombre_archivo_xml As String,
                                                ByRef class_ra_m_meta_archivo_ As List(Of class_ra_m_meta_archivo_)) As String
        Try
            Dim xmlArchivo As New XmlDocument
            Dim xmlNodoList As XmlNodeList
            xmlArchivo.Load(nombre_archivo_xml)
            xmlNodoList = xmlArchivo.GetElementsByTagName("meta_dato")
            If xmlNodoList.Count > 1 Then
                For i As Integer = 0 To xmlNodoList.Count - 1
                    If xmlNodoList.Item(i).HasChildNodes Then
                        Dim item As New class_ra_m_meta_archivo_
                        item.ra_m_id = xmlNodoList.Item(i).Item("id").InnerText
                        item.Valor_meta_dato = xmlNodoList.Item(i).Item(xmlNodoList.Item(i).ChildNodes(1).Name).InnerText
                        item.descripcion = xmlNodoList.Item(i).Item("Descripcion_meta_dato").InnerText
                        item.Estado_obligatorio = xmlNodoList.Item(i).Item("Estado_obligatorio").InnerText
                        item.Tipo = xmlNodoList.Item(i).Item("Tipo_meta_dato").InnerText
                        item.Estandar_meta_dato = xmlNodoList.Item(i).Item("Estandar_meta_dato").InnerText
                        item.Meta_dato = xmlNodoList.Item(i).ChildNodes(1).Name
                        item.ERROR_SERVICE = "YES"
                        class_ra_m_meta_archivo_.Add(item)
                    End If
                Next
            End If
            Solicita_archivo_meta_dato_archivo = "YES"
        Catch ex As Exception
            Solicita_archivo_meta_dato_archivo = "Incosistencia general funcion Solicita_archivo_meta_dato_archivo " & ex.Message
        End Try
    End Function

End Class
