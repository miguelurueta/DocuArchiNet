Public Class interface_config_digitaliza
    Public Property id_config_digitalizacion As Integer
    Public Property error_gestion As String
    Public Property zoon_visor As String
    Public Property tumbail_visor As Integer
    Public Property vista_configuracion_escaner As Integer
    Public Property duplex_configuracion As Integer
    Public Property desc_pag_blanco_configuracion As Integer
    Public Property detect_borde_configuracion As Integer
    Public Property desk_configuracion As Integer
    Public Property adf_configuracion As Integer
    Public Property controlador_propio_configuracion As Integer
    '--Zona restrictiva
    Public Property Id_Ra_Config As Integer
    Public Property Obliga_Lista_Chequeo As Integer
    Public Property Tipo_Digitalizacion As Integer
    Public Property Tipo_Archivo_Digitaliza As String
    Public Property Activa_Ocr As Integer
    Public Property Activa_Compresion As Integer
    Public Property Resolucion As Integer
    Public Property Tonalidad_Digitalizacion As Integer ' 1-black y negro  2-Grises  3-Color
    Public Property Tonalidad_Digitalizacion_black As Integer
    Public Property Tonalidad_Digitalizacion_gray As Integer
    Public Property Tonalidad_Digitalizacion_color As Integer
    Public Property Extension_Archivo As String
    Public Property Estado_formato_pdf As Integer
    Public Property Estado_formato_pdf_a As Integer
    Public Property Estado_formato_tif As Integer
    Public Property Estado_tonalidad_black As Integer
    Public Property Estado_tonalidad_gray As Integer
    Public Property Estado_tonalidad_color As Integer
End Class
Public Class Class_ra_dig_config_digitalizacion
    Function Solicita_id_configuracion_digitalizacion(ByVal id_tipo_doc_tramite As Integer,
                                                     ByVal tipo_plantilla_tramite As String,
                                                     ByRef id_config_digitalizacion As Integer,
                                                     Optional ByVal notificacion_existencia As Integer = 1) As String
        Try
            Dim Campo As String = ""
            Dim Result As String = ""
            If tipo_plantilla_tramite = "RADICACION ENTRANTE" Or tipo_plantilla_tramite = "" Then
                tipo_plantilla_tramite = "tipo_doc_entrante_id_Tipo_Doc_Entrante"
            Else
                tipo_plantilla_tramite = "tipo_doc_saliente_id_Tip_Doc_Saliente"
            End If
            Dim Parametro_Consulta As String = " SELECT  ID_RA_CONFIG from RA_DIG_CONFIG_DIGITALIZACION where  " & tipo_plantilla_tramite &
                "=" & id_tipo_doc_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("RA_DIG_CONFIG_DIGITALIZACION")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_configuracion_digitalizacion = "Función Solicita_id_configuracion_digitalizacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_config_digitalizacion = -1
                If notificacion_existencia = 1 Then
                    Solicita_id_configuracion_digitalizacion = "El tipo de tramite (" & id_tipo_doc_tramite & ") no tiene configuración con una lista de chequeo"
                    Exit Function
                Else
                    Solicita_id_configuracion_digitalizacion = "YES"
                    Exit Function
                End If

            Else
                id_config_digitalizacion = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_configuracion_digitalizacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_id_configuracion_digitalizacion = "Inconsistencia general función Solicita_id_configuracion_digitalizacion " & ex.Message
        End Try
    End Function
    Function Solicita_datos_estructura_config_tramite(ByVal id_tramite As Integer,
                                                      ByRef parameter_gestion As interface_config_digitaliza) As String
        '----------------------------------------------------------------------------------
        '----------------------------------------------------------------------------------
        'Funcion : Solicita la configuración de la interface
        'de digitalización heredada del tipo tramite, esta función retorna los parametros 
        'restricitivos de configuración de cada tramite a digitalizar o un tramite
        'general rectrictivo  "default_digitalizacion"
        '
        '---------------------------------------------------------------------------------------------------------------------------------
        'Restorno 
        '----------------------------------------------------------------------------------------------------------------------------------
        '------------------------------
        'Obliga_Lista_Chequeo        : Gauarda la configuración de la obligatoriedad de palicar una lista de chequeo 1-0 valores retornados
        '----------------------------
        'Tipo_Digitalizacion         : Gauarda la configuración de los tipos de digitalizacón del perfil de digitalización 1-Probatoria 
        '------------------------------ 2- Certificada   3- con fines probatorio
        '----------------------------
        '-----------------------------
        'Tipo_Archivo_Digitaliza     : Gauarda la configuración del tipo de archivo que se digitaliza 1- tiff 2-pdf 3-pdf/a
        '-----------------------------
        'Activa_Compresion           : Gauarda la configuración si esta activo o no activo la compresión de archivos pdf 1-0 valores retornados
        '-----------------------------
        'Activa_Ocr                  : Gauarda la configuración si esta activo o no activo el ocr para documentos digitalizados 1-0 valores retornados
        '-----------------------------
        'Resolucion                  : Gauarda la configuración de la resolución de la digializacion de documentoss  100-1000 valores retornados
        '-----------------------------
        'Extension_Archivo           : Gauarda la configuración con las extensiones de archivo permitidas para la carga de documentos
        '-----------------------------
        '-----------------------------
        'Tonalidad_Digitalizacion    : Guarda la configuración de la tonalidad de la digitalización 1- Negro 2-grises 3- Color
        '-----------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        'Fecha     : 2022-08-16
        '-----------
        '----------------------------------------------------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select ID_RA_CONFIG,OBLIGA_LISTA_CHEQUEO,TIPO_DIGITALIZACION," &
          "TIPO_ARCHIVO_DIGITALIZA,ACTIVA_OCR,ACTIVA_COMPRESION," &
          "RESOLUCION,TONALIDAD_DIGITALIZACION,EXTENSION_ARCHIVO" &
          ",TONALIDAD_DIGITALIZACION_BLACK,TONALIDAD_DIGITALIZACION_GRAY,TONALIDAD_DIGITALIZACION_COLOR" &
          ",ESTADO_FORMATO_PDF,ESTADO_FORMATO_PDF_A,ESTADO_FORMATO_TIF" &
          ",ESTADO_TONALIDAD_BLACK,ESTADO_TONALIDAD_GRAY,ESTADO_TONALIDAD_COLOR" &
          " from ra_dig_config_digitalizacion where tipo_doc_entrante_id_Tipo_Doc_Entrante =" & id_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_set As DataSet = New DataSet("ra_dig_config_digitalizacion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_set)
            If Result <> "YES" Then
                Solicita_datos_estructura_config_tramite = " Error funcion Solicita_existencia_configuracion_interface_digitalizacion " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count = 0 Then
                parameter_gestion.Id_Ra_Config = -1
                Solicita_datos_estructura_config_tramite = "YES"
                Exit Function
            Else
                parameter_gestion.Id_Ra_Config = Dat_set.Tables(0).Rows(0).Item(0)
                parameter_gestion.Obliga_Lista_Chequeo = Dat_set.Tables(0).Rows(0).Item(1)
                parameter_gestion.Tipo_Digitalizacion = Dat_set.Tables(0).Rows(0).Item(2)
                parameter_gestion.Tipo_Archivo_Digitaliza = Dat_set.Tables(0).Rows(0).Item(3)
                parameter_gestion.Activa_Ocr = Dat_set.Tables(0).Rows(0).Item(4)
                parameter_gestion.Activa_Compresion = Dat_set.Tables(0).Rows(0).Item(5)
                parameter_gestion.Resolucion = Dat_set.Tables(0).Rows(0).Item(6)
                parameter_gestion.Tonalidad_Digitalizacion = Dat_set.Tables(0).Rows(0).Item(7)
                parameter_gestion.Extension_Archivo = Dat_set.Tables(0).Rows(0).Item(8)
                parameter_gestion.Tonalidad_Digitalizacion_black = Dat_set.Tables(0).Rows(0).Item(9)
                parameter_gestion.Tonalidad_Digitalizacion_gray = Dat_set.Tables(0).Rows(0).Item(10)
                parameter_gestion.Tonalidad_Digitalizacion_color = Dat_set.Tables(0).Rows(0).Item(11)
                parameter_gestion.Estado_formato_pdf = Dat_set.Tables(0).Rows(0).Item("ESTADO_FORMATO_PDF")
                parameter_gestion.Estado_formato_pdf_a = Dat_set.Tables(0).Rows(0).Item("ESTADO_FORMATO_PDF_A")
                parameter_gestion.Estado_formato_tif = Dat_set.Tables(0).Rows(0).Item("ESTADO_FORMATO_TIF")
                parameter_gestion.Estado_tonalidad_black = Dat_set.Tables(0).Rows(0).Item(15)
                parameter_gestion.Estado_tonalidad_gray = Dat_set.Tables(0).Rows(0).Item(16)
                parameter_gestion.Estado_tonalidad_color = Dat_set.Tables(0).Rows(0).Item(17)
                Solicita_datos_estructura_config_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_estructura_config_tramite = "Inconsistencia general funcion Solicita_datos_estructura_config_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_datos_configuracion_digitalizacion(ByVal id_configuracion As Integer,
                                                         ByRef stru_config As Stru_config_digitalizacion) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  ID_RA_CONFIG,tipo_doc_entrante_id_Tipo_Doc_Entrante," &
                "tipo_doc_saliente_id_Tip_Doc_Saliente,OBLIGA_LISTA_CHEQUEO" &
                ",TIPO_DIGITALIZACION,TIPO_ARCHIVO_DIGITALIZA,ACTIVA_OCR" &
                " from RA_DIG_CONFIG_DIGITALIZACION where ID_RA_CONFIG= " &
                 id_configuracion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("RA_DIG_CONFIG_DIGITALIZACION")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_configuracion_digitalizacion = "Función Solicita_datos_configuracion_digitalizacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_configuracion_digitalizacion = "Imposible encontrar datos de configuración para el identificador (" & id_configuracion & ")"
                Exit Function
            Else
                stru_config.ID_RA_CONFIG = Datset.Tables(0).Rows(0).Item(0)
                stru_config.tipo_doc_entrante_id_Tipo_Doc_Entrante = Datset.Tables(0).Rows(0).Item(1)
                stru_config.tipo_doc_saliente_id_Tip_Doc_Saliente = Datset.Tables(0).Rows(0).Item(2)
                stru_config.OBLIGA_LISTA_CHEQUEO = Datset.Tables(0).Rows(0).Item(3)
                stru_config.TIPO_DIGITALIZACION = Datset.Tables(0).Rows(0).Item(4)
                stru_config.TIPO_ARCHIVO_DIGITALIZA = Datset.Tables(0).Rows(0).Item(5)
                stru_config.ACTIVA_OCR = Datset.Tables(0).Rows(0).Item(6)
                Solicita_datos_configuracion_digitalizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_configuracion_digitalizacion = "Inconsistencia general función lista_datos_configuracion_digitalizacion " & ex.Message
        End Try
    End Function
    Function SolicitaDatosConfiguracionDigitalizacionPorTramite(ByVal IdTipoTramite As Integer,
                                                                ByRef StruConfigDigitalizacion As Stru_config_digitalizacion) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  ID_RA_CONFIG,tipo_doc_entrante_id_Tipo_Doc_Entrante," &
                "tipo_doc_saliente_id_Tip_Doc_Saliente,OBLIGA_LISTA_CHEQUEO" &
                ",TIPO_DIGITALIZACION,TIPO_ARCHIVO_DIGITALIZA,ACTIVA_OCR" &
                " from RA_DIG_CONFIG_DIGITALIZACION where tipo_doc_entrante_id_Tipo_Doc_Entrante= " &
                 IdTipoTramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("RA_DIG_CONFIG_DIGITALIZACION")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosConfiguracionDigitalizacionPorTramite = "Función SolicitaDatosConfiguracionDigitalizacionPorTramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                StruConfigDigitalizacion.ID_RA_CONFIG = 0
                StruConfigDigitalizacion.tipo_doc_entrante_id_Tipo_Doc_Entrante = 0
                StruConfigDigitalizacion.tipo_doc_saliente_id_Tip_Doc_Saliente = 0
                StruConfigDigitalizacion.OBLIGA_LISTA_CHEQUEO = 0
                StruConfigDigitalizacion.TIPO_DIGITALIZACION = 0
                StruConfigDigitalizacion.TIPO_ARCHIVO_DIGITALIZA = ""
                StruConfigDigitalizacion.ACTIVA_OCR = 0
                SolicitaDatosConfiguracionDigitalizacionPorTramite = "YES"
                Exit Function
            Else
                StruConfigDigitalizacion.ID_RA_CONFIG = Datset.Tables(0).Rows(0).Item(0)
                StruConfigDigitalizacion.tipo_doc_entrante_id_Tipo_Doc_Entrante = Datset.Tables(0).Rows(0).Item(1)
                StruConfigDigitalizacion.tipo_doc_saliente_id_Tip_Doc_Saliente = Datset.Tables(0).Rows(0).Item(2)
                StruConfigDigitalizacion.OBLIGA_LISTA_CHEQUEO = Datset.Tables(0).Rows(0).Item(3)
                StruConfigDigitalizacion.TIPO_DIGITALIZACION = Datset.Tables(0).Rows(0).Item(4)
                StruConfigDigitalizacion.TIPO_ARCHIVO_DIGITALIZA = Datset.Tables(0).Rows(0).Item(5)
                StruConfigDigitalizacion.ACTIVA_OCR = Datset.Tables(0).Rows(0).Item(6)
                SolicitaDatosConfiguracionDigitalizacionPorTramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosConfiguracionDigitalizacionPorTramite = "Inconsistencia general función SolicitaDatosConfiguracionDigitalizacionPorTramite " & ex.Message
        End Try
    End Function
End Class
