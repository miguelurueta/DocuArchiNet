Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.IO.IsolatedStorage
Imports System.Xml
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Public Structure rotulo_unidad_conservacion
    Dim tipo_unidad_conservacion As Integer
    Dim nombre_unidad As String
    Dim x As Integer
    Dim y As Integer
    Dim chekmarco As Boolean
    Dim numero_columnas_datos As Integer
    Dim image_empresa As Boolean
    Dim nit_empresa As Boolean
    Dim nombre_empresa As Boolean
    Dim DATOS_UNIDAD_CONSERVACION As Boolean
    Dim Codigo_unico As Boolean
    Dim Tema_unidad As Boolean
    Dim Fechas_Extremas As Boolean
    Dim Observacion As Boolean
    Dim Rangos_Extremos As Boolean
    Dim Descripcion_unidad As Boolean
    Dim TRD_UNIDAD_CONSERVACION As Boolean
    Dim Nombre_Area As Boolean
    Dim Codigo_Area As Boolean
    Dim Nombre_Serie As Boolean
    Dim Codigo_Serie As Boolean
    Dim Nombre_sub_Serie As Boolean
    Dim Codigo_sub_Serie As Boolean
    Dim Edificio As Boolean
    Dim Piso As Boolean
    Dim Area As Boolean
    Dim Estante As Boolean
    Dim Modulo As Boolean
    Dim Estrepaño As Boolean
    Dim UBICACION_UNIDAD_CONSERVACION As Boolean
    Dim TAM_LETRA_TITULO As Integer
    Dim TAM_LETRA_DATOS_UNIDAD As Integer
    Dim TAM_LETRA_DATOS_TRD As Integer
    Dim TAM_LETRA_UBICACION As Integer
    Dim nombre_plantilla As String
    Dim TAM_LETRA_UNI_ANIDADO As Integer
    Dim UNIDADES_ANIDADAS As Integer
    Dim TAM_LETRA_UNIDADDES_CONTENIDA As Integer
    Dim numero_folio As Boolean
    Dim numero_volumen As Boolean
    Dim nombre_propietario As Boolean
    Dim identificacion_propietario As Boolean
    Dim nombre_responsable As Boolean
    Dim identificacion_responsable As Boolean
    Dim nombre_fondo As Boolean
    Dim version_trd As Boolean
    Dim campo_orden_expediente As String
End Structure
Public Structure expediente_conservacion
    Dim ID_EXPEDIENTE As Integer
    Dim ID_UNIDAD_CONSERVACION As Integer
    Dim CONSECUTIVO_UNIDAD_CONSERVACION As Integer
    Dim CONSECUTIVO_EXPEDIENTE As Integer
    Dim CONSECUTIVO_DOCUMENTO As Integer
    Dim CODIGO_LARGO As String
    Dim CODIGO_UNICO As String
    Dim TIPO_UNIDAD_CONSERVACION As Integer
    Dim NUMERO_FOLIO_UNIDAD_CONSERVACION As Integer
    Dim ID_USUARIO_GESTION As Integer
    Dim FECHA_CREACION As String
    Dim CODIGO_AREA_TRD As String
    Dim NOMBRE_AREA As String
    Dim CODIGO_SERIE As String
    Dim NOMBRE_SERIE As String
    Dim CODIGO_SUBSERIE As String
    Dim NOMBRE_SUBSERIE As String
    Dim ESTADO_EXPEDIENTE As Integer
    Dim ESTADO_ARCHIVO_EXPEDIENTE As Integer
    Dim FECHA_EXTREMA_INICIAL As String
    Dim FECHA_EXTREMA_FINAL As String
    Dim RANGO_EXTREMO_INICIAL As String
    Dim RANGO_EXTREMO_FINAL As String
    Dim TEMA_EXPEDIENTE As String
    Dim DESCRIPCION_UNIDAD_CONSERVACION As String
    Dim CODIGO_BARRAS_UNIDAD As String
    Dim ESTADO_UNIDAD_CONSERVACION As Integer
    Dim ESTADO_ARCHIVO_INIDAD As Integer
    Dim TEMA_UNIDAD_CONSERVACION As String
    Dim ENTRE_PAÑO_ID_ENTREPAÑO As Integer
    Dim ID_EMPRESA_GESTION As Integer
    Dim VOLUMEN_EXPEIDENTE As Integer
    Dim NOMBRE_TIPO_EXPEDIENTE As String
    Dim NUMERO_ELECTRONICO_CONTENIDO As Integer
    Dim NUMERO_DIGITALIZADO_CONTENIDO As Integer
    Dim ASUNTO_EXPEDIENTE As String
    Dim OBSERVACION_EXPEDIENTE As String
    Dim TIPO_UNIDAD_ID_TIPO As Integer
    Dim ID_TIPO_UNIDAD_DOCUMENTAL As Integer
    Dim NOMBRE_TIPO_UNIDAD_DOCUMENTAL As String
    Dim ID_SUB_AREA As Integer
    Dim NOMBRE_SUB_AREA As String
    Dim ID_FONDO As Integer
    Dim NOMBRE_FONDO As String
    Dim Id_tipos_ciclo_archivo As Integer
    Dim NOMBRE_CICLO_ARCHIVO As String
    Dim NOMBRE_PERSONA_EXPEDIENTE As String
    Dim IDENTIFICACION_PERSONA_EXPEDIENTE As String
    Dim NOMBRE_RESPONSABLE_EXPEDIENTE As String
    Dim IDENFICACION_RESPONSABLE_EXPEDIENTE As String
    Dim Estado_Publico_Sub_Expediente As Integer
    Dim ALEAS_EXPEDIENTE As String
    Dim CONSECUTIVO_EXPEDIENTE_2 As Integer
    Dim EXPEDIENTE_PADRE As Integer
    Dim fecha_ret_central As String
    Dim fecha_ret_gestion As String
    Dim id_instrumento As Integer
    Dim GABINETE_PRODUCION As String
    Dim Id_registro_procedimiento As Integer
    Dim Id_registro_proceso As Integer
    Dim ID_DISCO As Integer
    Dim ESTADO_FIRMA As Integer
    Dim FECHA_FIRMA As String
    Dim estado_expediente_electronico As Integer
    Dim ESTADO_CODIGO_UNICO As Integer
    Dim ra_auto_registro_expediente_id_auto_registro As Integer
End Structure
Public Structure RA_ALEAS_CAMPOS_ROTULO_EXPEDIENTE
    Dim nombre_Campo As String
    Dim aleas_campo As String
End Structure
Public Class ClassExpedienteVincula
    Public Property error_gestion As String
    Public Property id_expediente As Integer
    Public Property gabinete As String
    Public Property list_image As List(Of list_imagen_expediente_service)
    Public Property ClsssStructureVinculaDocumento As List(Of ClsssStructureVinculaDocumento)
    Public Property radicado As String
    Public Property id_flujo As Long
    Public Property tipo_copia As Integer
    Public Property id_imagen_copia As Integer
    Public Property valor_campos As String
    Public Property nombre_expediente As String
    Public Property nombre_expediente_rlacionado As String
    Public Property Matricula As String
End Class
Public Class list_imagen_expediente_service
    Public Property id_imagen As Integer
    Public Property gabinete As String
End Class
Public Structure str_expediente_service
    Dim CODIGO_UNICO As String
    Dim ESTADO_CODIGO_UNICO As String
    Dim ID_EMPRESA_GESTION As String
    Dim FECHA_EXTREMA_INICIAL As String
    Dim FECHA_EXTREMA_FINAL As String
    Dim RANGO_EXTREMO_INICIAL As String
    Dim RANGO_EXTREMO_FINAL As String
    Dim TEMA_EXPEDIENTE As String
    Dim REGISTRO_ORGANIGRAMA As String
    Dim NOMBRE_AREA As String
    Dim NOMBRE_SERIE As String
    Dim NOMBRE_SUBSERIE As String
    Dim TIPO_EXPEDIENTE As String
    Dim NUMERO_DIGITALIZADO_CONTENIDO As String
    Dim NUMERO_FOLIO_UNIDAD_CONSERVACION As String
    Dim NUMERO_ELECTRONICO_CONTENIDO As String
    Dim ASUNTO_EXPEDIENTE As String
    Dim NOMBRE_TIPO_UNIDAD_DOCUMENTAL As String
    Dim OBSERVACION_EXPEDIENTE As String
    Dim NOMBRE_SUB_AREA As String
    Dim NOMBRE_CICLO_ARCHIVO As String
    Dim NOMBRE_FONDO As String
    Dim NOMBRE_PERSONA_EXPEDIENTE As String
    Dim IDENTIFICACION_PERSONA_EXPEDIENTE As String
    Dim NOMBRE_RESPONSABLE_EXPEDIENTE As String
    Dim IDENFICACION_RESPONSABLE_EXPEDIENTE As String
    Dim ALEAS_EXPEDIENTE As String
    Dim EXPEDIENTE_PADRE As String
    Dim ID_INSTRUMENTO As String
    Dim GABINETE_PRODUCION As String
    Dim ID_NIVEL_PADRE As String
    Dim ID_REGISTRO_RELACION As String
End Structure
Public Class ClsssStructureVinculaDocumento
    Public IdExpedienteWeb As Object
    Public IdImagen As Integer
    Public Gabinete As String
    Public Radicado As String
    Public IdFlujoTarea As Long
End Class
Public Class ClassGaExpediente
    Function SolicitaEstructuraExpedienteDocumentoVinculante(ByRef EstructuraGestion As estructure_gestion,
                                                             ByVal NombreGabinete As String,
                                                             ByVal IdImagen As Integer,
                                                             ByVal IdTareaWorkflow As Long,
                                                             ByVal Radicado As String,
                                                             ByVal NombreRutaWorkflow As String,
                                                             ByVal IdRutaWorkflow As Integer,
                                                             ByVal MatriculaSII As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura del expediente vinculante de una tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete      : Representa el nombre del gabinete
        'IdImagen            : Representa la identificación de la imagen en el gabinete
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'Radicado            : Representa el consecutivo del radicado del tramite
        'NombreRutaWorkflow  : Representa el nombre de la ruta general 
        'IdRutaWorkflow      : Representa la identificación de la ruta workflow
        'MatriculaSII        : Representa la identificación de la matricula para el caso integración SII
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstructuraGestion   : Retorna la estructura del expediente vinculante
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2022-06-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            EstructuraGestion.ID_EXPEDIENTE = 0
            EstructuraGestion.ID_TIPO_EXPEDIENTE = 0
            EstructuraGestion.EXPEDIENTE = ""
            Dim ClassWorkflow As New ClassWorkflow
            Dim Result As String = ""
            '------------------------------------------------
            'Retorna si el tipo de tarea workflow es externa
            'Valores 1. Tarea interna    2. Tarea externa
            '------------------------------------------------
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim IdTipoTarea As Integer = 0
            If IdTareaWorkflow <> 0 Then
                Result = Class_DAT_ADIC_TAR.SolicitaTipoFujoExternoInterno(IdTareaWorkflow,
                                                                           IdTipoTarea,
                                                                           NombreRutaWorkflow)
                If Result <> "YES" Then
                    SolicitaEstructuraExpedienteDocumentoVinculante = Result
                    Exit Function
                End If
            End If
            Dim Nombre_plantilla_radicado As String = ""
            Dim Refclas_radicado As New ClassRadicador
            Dim IdExpedientePlantillaRadicado As Integer = 0
            Dim NombreExpedientePlantillaRadicado As String = ""
            Dim IdTipoExpedientePlantillaRadicado As Integer = 0
            Dim IdExpediente As Integer = 0
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim ClassRaRelacionRadicadoExternoExpediente As New ClassRaRelacionRadicadoExternoExpediente
            Dim Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
            '---------------------------------------------------------------
            'Solicita el expdiente relacionado para el flujo interno  en
            'la plantilla de radicación con el consecutivo radicado
            'si el tipo de tarea es interna de workflow
            '---------------------------------------------------------------
            If IdTipoTarea = 1 And Radicado <> "" Then
                Dim Ref_Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
                Result = Ref_Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(Radicado,
                                                                                                  Nombre_plantilla_radicado)
                If Result <> "YES" Then
                    SolicitaEstructuraExpedienteDocumentoVinculante = Result
                    Exit Function
                End If
                '---------------------------------------
                'Retorna expediente y id expediente
                '---------------------------------------
                If Nombre_plantilla_radicado <> "" Then
                    Result = Refclas_radicado.Retorna_nombre_expediente_id_expediente_radicado(Radicado,
                                                                                               Nombre_plantilla_radicado,
                                                                                               IdExpedientePlantillaRadicado,
                                                                                               NombreExpedientePlantillaRadicado,
                                                                                               IdTipoExpedientePlantillaRadicado)
                    If Result <> "YES" Then
                        SolicitaEstructuraExpedienteDocumentoVinculante = Result
                        Exit Function
                    End If
                End If
                If IdExpedientePlantillaRadicado <> 0 Then
                    EstructuraGestion.ID_EXPEDIENTE = IdExpedientePlantillaRadicado
                    EstructuraGestion.ID_TIPO_EXPEDIENTE = IdTipoExpedientePlantillaRadicado
                    EstructuraGestion.EXPEDIENTE = NombreExpedientePlantillaRadicado
                Else
                    If IdTareaWorkflow <> 0 And IdRutaWorkflow <> 0 Then
                        Result = Class_ra_rel_copia_wf_produccion.SolicitaUltimaRelacionExpedienteIdTareaWorkflow(IdTareaWorkflow,
                                                                                                                  IdRutaWorkflow,
                                                                                                                  IdExpediente)
                        If Result <> "YES" Then
                            SolicitaEstructuraExpedienteDocumentoVinculante = Result
                            Exit Function
                        End If
                        If IdExpediente <> 0 Then
                            Result = ClassGaExpediente.Solicita_datos_expediente_relacion(IdExpediente,
                                                                                          EstructuraGestion)
                            If Result <> "YES" Then
                                SolicitaEstructuraExpedienteDocumentoVinculante = Result
                                Exit Function
                            End If
                        End If
                    End If
                End If
            End If
            '--------------------------------------------------------------------
            'Solicita el expdiente relacionado para el flujo externo tipo 2  en
            'la ultima relación del ultimo documento vinculado al expediente
            '--------------------------------------------------------------------- 
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            Dim CStruSiiCahcheExpediente As New CStruSiiCahcheExpediente
            CStruSiiCahcheExpediente.IdExpediente = 0
            If IdTipoTarea = 2 And IdTareaWorkflow <> 0 And IdRutaWorkflow <> 0 Then
                '---//Solicita expediente por matricula------////
                If MatriculaSII <> "" Then
                    Result = ClassRaSIiCacheExpediente.SolicitaCacheCreacionExpedienteSII(MatriculaSII,
                                                                                          NombreGabinete,
                                                                                          CStruSiiCahcheExpediente)
                    If Result <> "YES" Then
                        SolicitaEstructuraExpedienteDocumentoVinculante = Result
                        Exit Function
                    End If
                    IdExpediente = CStruSiiCahcheExpediente.IdExpediente
                End If
                '----///Solicita expediente por relación de radicado SII-------////
                If IdExpediente = 0 Then
                    Result = ClassRaRelacionRadicadoExternoExpediente.SolicitaExpedienteRadicadoExterno(Radicado,
                                                                                                        IdExpediente)
                    If Result <> "YES" Then
                        SolicitaEstructuraExpedienteDocumentoVinculante = Result
                        Exit Function
                    End If
                End If
                '----///Solicita expediente de la ultimo expediente relacionado a la tarea workflow-----/////
                If IdExpediente = 0 Then
                    Result = Class_ra_rel_copia_wf_produccion.SolicitaUltimaRelacionExpedienteIdTareaWorkflow(IdTareaWorkflow,
                                                                                                              IdRutaWorkflow,
                                                                                                              IdExpediente)
                    If Result <> "YES" Then
                        SolicitaEstructuraExpedienteDocumentoVinculante = Result
                        Exit Function
                    End If
                End If
                If IdExpediente <> 0 Then
                    Result = Me.Solicita_datos_expediente_relacion(IdExpediente,
                                                                   EstructuraGestion)
                    If Result <> "YES" Then
                        SolicitaEstructuraExpedienteDocumentoVinculante = Result
                        Exit Function
                    End If
                End If
            End If
            '--------------------------------------------------
            'Asigna el expdiente relacionado al documento
            'principal de la tarea si no hay una relación
            'de vinculación a un expediente o si la tarea
            'pertence a un tramite interno pero la plantilla
            'de radicación no tiene un tramite relacionado
            '--------------------------------------------------
            Dim ClassDaGabinete As New ClassDaGabinete
            If EstructuraGestion.ID_EXPEDIENTE = 0 And IdImagen <> 0 And NombreGabinete <> "" Then
                Result = ClassDaGabinete.Solicita_datos_expediente_relacion_gabinete(IdImagen,
                                                                                     NombreGabinete,
                                                                                     EstructuraGestion)
                If Result <> "YES" Then
                    SolicitaEstructuraExpedienteDocumentoVinculante = Result
                    Exit Function
                End If
            End If
            SolicitaEstructuraExpedienteDocumentoVinculante = "YES"
        Catch ex As Exception
            SolicitaEstructuraExpedienteDocumentoVinculante = "Inconsistencia función Solicita_datos_expediente_estructura_base_datos " & ex.Message
        End Try
    End Function
    Function Listar_datos_Expediente_Conservacion_estructura_entrepano(ByVal id_entrepaño As Integer,
                                                                       ByRef estru_unidad_conservacion() As expediente_conservacion) As String
        '************************************************************
        'Funcion Listar estrucutura expediente con el
        'parametro id entrepaño
        'Fecha 2015-01-29
        'Ing : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Erase estru_unidad_conservacion
            Dim campos_seleccion As String = "UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION,ID_EXPEDIENTE,CONSECUTIVO_DOCUMENTO," &
            "CONSECUTIVO_EXPEDIENTE_2,CODIGO_LARGO,CODIGO_UNICO,NUMERO_FOLIOS_CONTENIDOS," &
            "ID_USUARIO_GESTION,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD,CODIGO_SERIE_TRD,NOMBRE_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_SUBSERIE_TRD," &
            "ESTADO_EXPEDIENTE,ESTADO_ARCHIVO_EXPEDIENTE,FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," &
            "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_EXPEDIENTE,ENTRE_PAÑO_ID_ENTREPAÑO,ASUNTO_EXPEDIENTE,ID_EMPRESA_EXPEDIENTE,VOLUMEN_EXPEDIENTE,rte.NOMBRE_TIPO_EXPEDIENTE,ALEAS_EXPEDIENTE "
            Dim SqlConsulta As String = "select " & campos_seleccion & " from  expediente_archivo " &
            " inner join ra_tipo_expediente as rte on (rte.ID_TIPO_EXPEDIENTE=RA_TIP_EXPE_ID_TIPO_EXPEDIENTE)" &
                                              " where ENTRE_PAÑO_ID_ENTREPAÑO=" & id_entrepaño
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Listar_datos_Expediente_Conservacion_estructura_entrepano = " Error solicitando estrucutura expediente " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_unidad_conservacion(i)
                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION")
                    End If
                    estru_unidad_conservacion(i).ID_EXPEDIENTE = Datset.Tables(0).Rows(i).Item(1)
                    estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_DOCUMENTO")
                    'estru_unidad_conservacion(i).CONSECUTIVO_EXPEDIENTE = Dat_reader.Item("CONSECUTIVO_EXPEDIENTE")
                    'estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Dat_reader.Item("CONSECUTIVO_DOCUMENTO")
                    estru_unidad_conservacion(i).CODIGO_LARGO = Datset.Tables(0).Rows(i).Item("CODIGO_LARGO")
                    estru_unidad_conservacion(i).CODIGO_UNICO = Datset.Tables(0).Rows(i).Item("CODIGO_UNICO")
                    estru_unidad_conservacion(i).TIPO_UNIDAD_CONSERVACION = 1
                    estru_unidad_conservacion(i).NUMERO_FOLIO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("NUMERO_FOLIOS_CONTENIDOS")
                    estru_unidad_conservacion(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(i).Item("ID_USUARIO_GESTION")

                    If Datset.Tables(0).Rows(i).IsNull(8) = True Then
                        estru_unidad_conservacion(i).FECHA_CREACION = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_CREACION = Datset.Tables(0).Rows(i).Item("FECHA_CREACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(9) = True Then
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = Datset.Tables(0).Rows(i).Item("CODIGO_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(10) = True Then
                        estru_unidad_conservacion(i).NOMBRE_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_AREA = Datset.Tables(0).Rows(i).Item("NOMBRE_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(11) = True Then
                        estru_unidad_conservacion(i).CODIGO_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(12) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(13) = True Then
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SUB_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(14) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SUBSERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(15) = True Then
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ESTADO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(16) = True Then
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = Datset.Tables(0).Rows(i).Item("ESTADO_ARCHIVO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(20) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(21) = True Then
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TEMA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(22) = True Then
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = 0
                    Else
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = Datset.Tables(0).Rows(i).Item(22)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(23) = True Then
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ASUNTO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(24) = True Then
                        estru_unidad_conservacion(i).ID_EMPRESA_GESTION = 0
                    Else
                        estru_unidad_conservacion(i).ID_EMPRESA_GESTION = Datset.Tables(0).Rows(i).Item("ID_EMPRESA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(25) = True Then
                        estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE = 0
                    Else
                        estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE = Datset.Tables(0).Rows(i).Item("VOLUMEN_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(26) = True Then
                        estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("NOMBRE_TIPO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(27) = True Then
                        estru_unidad_conservacion(i).ALEAS_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).ALEAS_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("ALEAS_EXPEDIENTE")
                    End If
                Next
                Listar_datos_Expediente_Conservacion_estructura_entrepano = "YES"
                Exit Function
            Else
                Listar_datos_Expediente_Conservacion_estructura_entrepano = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_datos_Expediente_Conservacion_estructura_entrepano = "Inconsistencia general funcion Listar_datos_Expediente_Conservacion_estructura_entrepano " & ex.Message
        End Try
    End Function
    Function Listar_expediente_treview(ByRef Tree As TreeView,
                                       ByVal estru_unidad_conservacion() As expediente_conservacion _
                                       , ByRef trenode As TreeNode,
                                       Optional ByVal limpia_nodo As Integer = 0) As String
        Try
            If limpia_nodo = 0 Then
                Tree.Nodes.Clear()
            End If

            If estru_unidad_conservacion Is Nothing Then
                Listar_expediente_treview = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_unidad_conservacion.Length - 1
                Dim NodeTree As New TreeNode
                Dim ref_fecha_extrema_ini As String = ""
                Dim ref_fecha_extrema_fin As String = ""
                If estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL <> "" Then
                    ref_fecha_extrema_ini = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL, 10)
                Else
                    ref_fecha_extrema_ini = estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL
                End If
                If estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL <> "" Then
                    ref_fecha_extrema_fin = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL, 10)
                Else
                    ref_fecha_extrema_fin = estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL
                End If
                Dim tipo_unidad As String = ""
                If estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE Is Nothing Then
                Else
                    tipo_unidad = "TIPO UNIDAD :(" & estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD_DOCUMENTAL & ")" & " CLASE UNIDAD : (" & estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE & ")"
                End If
                NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion(i).CODIGO_UNICO & " TEMA: " &
                estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION &
                " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " &
                ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL & " HASTA " &
                estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE
                NodeTree.Value = estru_unidad_conservacion(i).ID_EXPEDIENTE
                NodeTree.ToolTip = "TreeViewunidad"
                NodeTree.ImageUrl = "../Gestion/imagenes/carpeta_dos_exp.png"
                If estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE > 1 Then
                    'NodeTree.ForeColor = Color.Blue
                End If
                trenode.ChildNodes.Add(NodeTree)
            Next
            Listar_expediente_treview = "YES"
        Catch ex As Exception
            Listar_expediente_treview = "Inconsistencia funcion Listar_expediente_treview " & ex.Message
        End Try
    End Function
    Function Listar_expediente_treview_node(ByVal estru_unidad_conservacion() As expediente_conservacion _
                                            , ByRef trenode As TreeNode,
                                            Optional ByVal limpia_nodo As Integer = 0) As String
        Try
            If limpia_nodo = 0 Then
                trenode.ChildNodes.Clear()
            End If
            If estru_unidad_conservacion Is Nothing Then
                Listar_expediente_treview_node = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_unidad_conservacion.Length - 1
                Dim NodeTree As New TreeNode
                Dim ref_fecha_extrema_ini As String = ""
                Dim ref_fecha_extrema_fin As String = ""
                If estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL <> "" Then
                    ref_fecha_extrema_ini = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL, 10)
                Else
                    ref_fecha_extrema_ini = estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL
                End If
                If estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL <> "" Then
                    ref_fecha_extrema_fin = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL, 10)
                Else
                    ref_fecha_extrema_fin = estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL
                End If
                Dim tipo_unidad As String = ""
                If estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE Is Nothing Then
                Else
                    tipo_unidad = "TIPO UNIDAD :(" & estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD_DOCUMENTAL & ")" & " CLASE UNIDAD : (" & estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE & ")"
                End If
                NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion(i).CODIGO_UNICO & " TEMA: " &
                estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION &
                " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " &
                ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL & " HASTA " &
                estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE
                NodeTree.Value = estru_unidad_conservacion(i).ID_EXPEDIENTE
                NodeTree.ToolTip = "Expediente"
                NodeTree.ImageUrl = "../Gestion/imagenes/carpeta_dos_exp.png"
                If estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE > 1 Then
                    'NodeTree.ForeColor = Color.Blue
                End If
                trenode.ChildNodes.Add(NodeTree)
            Next
            Listar_expediente_treview_node = "YES"
        Catch ex As Exception
            Listar_expediente_treview_node = "Inconsistencia funcion Listar_expediente_treview_node " & ex.Message
        End Try
    End Function
    Function Listar_expediente_treview_ubicacion(ByRef Tree As TreeView,
                                                 ByVal estru_unidad_conservacion() As expediente_conservacion _
                                                 , ByRef trenode As TreeNode,
                                                 Optional ByVal limpia_nodo As Integer = 0) As String
        Try
            If limpia_nodo = 0 Then
                Tree.Nodes.Clear()
            End If

            If estru_unidad_conservacion Is Nothing Then
                Listar_expediente_treview_ubicacion = "YES"
                Exit Function
            End If
            For i As Integer = 0 To estru_unidad_conservacion.Length - 1
                Dim NodeTree As New TreeNode
                Dim ref_fecha_extrema_ini As String = ""
                Dim ref_fecha_extrema_fin As String = ""
                If estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL <> "" Then
                    ref_fecha_extrema_ini = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL, 10)
                Else
                    ref_fecha_extrema_ini = estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL
                End If
                If estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL <> "" Then
                    ref_fecha_extrema_fin = Left(estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL, 10)
                Else
                    ref_fecha_extrema_fin = estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL
                End If
                Dim tipo_unidad As String = ""
                If estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE Is Nothing Then
                Else
                    tipo_unidad = "TIPO UNIDAD :(" & estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD_DOCUMENTAL & ")" & " CLASE UNIDAD : (" & estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE & ")"
                End If
                NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion(i).CODIGO_UNICO & " TEMA: " &
                estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION &
                " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " &
                ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL & " HASTA " &
                estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE
                NodeTree.Value = estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION & "|" & estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE & "|" & estru_unidad_conservacion(i).CODIGO_UNICO
                NodeTree.ToolTip = "Expediente"
                NodeTree.ImageUrl = "../Gestion/imagenes/carpeta_dos_exp.png"
                If estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE > 1 Then
                    'NodeTree.ForeColor = Color.Blue
                End If
                trenode.ChildNodes.Add(NodeTree)
            Next
            Listar_expediente_treview_ubicacion = "YES"
        Catch ex As Exception
            Listar_expediente_treview_ubicacion = "Inconsistencia funcion Listar_expediente_treview_ubicacion " & ex.Message
        End Try
    End Function
    Function Agregar_expediente_anidad_en_entrepano_treview_ubicacion(ByRef ref_treview_fuente As TreeView,
   ByVal estru_unidad_conservacion As expediente_conservacion, ByRef ref_treview_destino As TreeView,
   Optional ByVal opcion_eliminar_previo As Integer = 0, Optional ByVal opcion_agrega_nodo As Integer = 0) As String
        '*******************************************************
        'Función : Agrega un nodo al treview
        'datos del expeidente
        'Fecha : 2015-01-30
        'Ingeniero : Miguel Angel Urueta Miranda
        '********************************************************
        Try
            Dim NodeTree As New TreeNode
            Dim ref_fecha_extrema_ini As String = ""
            Dim ref_fecha_extrema_fin As String = ""
            If estru_unidad_conservacion.FECHA_EXTREMA_INICIAL <> "" Then
                ref_fecha_extrema_ini = Left(estru_unidad_conservacion.FECHA_EXTREMA_INICIAL, 10)
            Else
                ref_fecha_extrema_ini = estru_unidad_conservacion.FECHA_EXTREMA_INICIAL
            End If
            If estru_unidad_conservacion.FECHA_EXTREMA_FINAL <> "" Then
                ref_fecha_extrema_fin = Left(estru_unidad_conservacion.FECHA_EXTREMA_FINAL, 10)
            Else
                ref_fecha_extrema_fin = estru_unidad_conservacion.FECHA_EXTREMA_FINAL
            End If
            Dim tipo_unidad As String = ""
            If estru_unidad_conservacion.NOMBRE_TIPO_EXPEDIENTE Is Nothing Then
            Else
                tipo_unidad = "TIPO EXPEDIENTE : (" & estru_unidad_conservacion.NOMBRE_TIPO_EXPEDIENTE & ")  "
            End If
            NodeTree.Text = tipo_unidad & "CODIGO UNICO: " & estru_unidad_conservacion.CODIGO_UNICO & " TEMA: " &
            estru_unidad_conservacion.TEMA_UNIDAD_CONSERVACION &
            " FECHAS EXTREMAS: " & ref_fecha_extrema_ini & " HASTA " &
            ref_fecha_extrema_fin & " RANGO EXTREMOS:" & estru_unidad_conservacion.RANGO_EXTREMO_INICIAL & " HASTA " &
            estru_unidad_conservacion.RANGO_EXTREMO_FINAL & "-" & estru_unidad_conservacion.VOLUMEN_EXPEIDENTE
            NodeTree.Value = estru_unidad_conservacion.ID_UNIDAD_CONSERVACION & "|" & estru_unidad_conservacion.NOMBRE_TIPO_EXPEDIENTE & "|" & estru_unidad_conservacion.CODIGO_UNICO
            If opcion_eliminar_previo = 0 Then
                Dim Refclas_unidad As New ClassUnidadConservacion
                Dim result As String = Refclas_unidad.Rescursive_node_tree_elimina(ref_treview_destino, NodeTree.Value)
                If result <> "YES" Then
                    Agregar_expediente_anidad_en_entrepano_treview_ubicacion = result
                    Exit Function
                End If
            End If
            If estru_unidad_conservacion.VOLUMEN_EXPEIDENTE > 1 Then
                'NodeTree.ForeColor = Color.Blue
            End If
            If opcion_agrega_nodo = 0 Then
                ref_treview_destino.SelectedNode.ChildNodes.Add(NodeTree)
            End If

            Agregar_expediente_anidad_en_entrepano_treview_ubicacion = "YES"
        Catch ex As Exception
            Agregar_expediente_anidad_en_entrepano_treview_ubicacion = "Inconsistencia función Agregar_expediente_anidad_en_entrepano_treview  " &
            ex.Message
        End Try
    End Function
    Function Archiva_expediente_en_entrepano_archivado(ByVal id_entrepaño As Integer,
                                                       ByVal id_expediente As Integer,
                                                       ByVal id_usuario_gestion As Integer,
                                                       ByVal user_Gestion As String,
                                                       ByVal iptrans As String) As String
        '**************************************************************
        'Función : Archiva unidad de conservacion tipo 2 en entrepaño
        'Fecha : 2015-01-23
        'Ing : Miguel Angel Urueta Miranda
        '**************************************************************
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim Result As String = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Archiva_expediente_en_entrepano_archivado = Result
            Exit Function
        End If
        Dim id_tipo_expediente As Integer = 0
        Result = Me.Retorna_tipo_id_expediente_por_id(id_tipo_expediente,
                                                    id_expediente)
        If Result <> "YES" Then
            Archiva_expediente_en_entrepano_archivado = Result
            Exit Function
        End If
        Dim estado_archiva As Integer = 0
        Dim nombre_tipo_expediente As String = ""
        Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
        Result = ref_Class_ra_tipo_expediente.Retorna_tipo_expediente_requiere_unidad_conservacion(id_tipo_expediente,
                                                                                                   estado_archiva)
        If Result <> "YES" Then
            Archiva_expediente_en_entrepano_archivado = Result
            Exit Function
        End If
        If estado_archiva = 0 Then
            Result = ref_Class_ra_tipo_expediente.Retorna_nombre_tipo_expediente_por_id_expediente(id_expediente,
                                                                                                   nombre_tipo_expediente)
            If Result <> "YES" Then
                Archiva_expediente_en_entrepano_archivado = Result
                Exit Function
            End If
            Archiva_expediente_en_entrepano_archivado = "El tipo de expediente (" & nombre_tipo_expediente & ") no se puede archivar "
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update expediente_archivo set ENTRE_PAÑO_ID_ENTREPAÑO=" & id_entrepaño &
            ",ESTADO_ARCHIVO_EXPEDIENTE=1, UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=null  where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Archiva_expediente_en_entrepano_archivado = "Imposible archivar expediente  : " & sqlinsertcion
                errorM = "Imposible archivar expediente  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If

            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'ARCHIVA EXPEDIENTE','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           id_expediente & ",'" & iptrans & "','" & hor & "','DOCUARCHI-WEB','ARCHIVADO EN UNIDAD CONSERVACION" & id_entrepaño.ToString & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()
            Archiva_expediente_en_entrepano_archivado = errorM
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Archiva_expediente_en_entrepano_archivado = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Archiva_expediente_en_entrepano_archivado = errorM

        End Try
    End Function
    Function Archiva_expediente_en_entrepano(ByVal id_entrepaño As Integer, ByVal idex_row As Integer,
       ByVal id_expediente As Integer, ByVal id_usuario_gestion As Integer,
       ByVal user_Gestion As String, ByVal iptrans As String,
       ByRef estru As expediente_conservacion, ByRef reftre_destino As TreeView,
       ByRef reftre_fuente As TreeView, ByRef up_date_trevie_fuente As UpdatePanel) As String
        '**************************************************************
        'Función : Archiva unidad de conservacion tipo 2 en entrepaño
        'Fecha : 2015-01-23
        'Ing : Miguel Angel Urueta Miranda
        '**************************************************************
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim Result As String = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Archiva_expediente_en_entrepano = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update expediente_archivo set ENTRE_PAÑO_ID_ENTREPAÑO=" & id_entrepaño &
            ",ESTADO_ARCHIVO_EXPEDIENTE=1, UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=null  where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Archiva_expediente_en_entrepano = "Imposible archivar expediente  : " & sqlinsertcion
                errorM = "Imposible archivar expediente  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If

            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'ARCHIVA EXPEDIENTE','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           id_expediente & ",'" & iptrans & "','" & hor & "','DOCUARCHI-WEB','ARCHIVADO EN UNIDAD CONSERVACION" & id_entrepaño.ToString & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim trnode_copia_agregar As TreeNode = reftre_fuente.SelectedNode
            Dim tree_slected_node_fuente As TreeNode = Nothing
            Dim tree_slected_node_destino As TreeNode = Nothing
            '-----------------------------------------------------
            'Elimina el registro para agregarlo en otra unidad
            '-----------------------------------------------------
            If Not reftre_fuente.SelectedNode Is Nothing Then
                reftre_fuente.Nodes.Remove(reftre_fuente.SelectedNode)
                Dim sNodo As TreeNode = reftre_fuente.SelectedNode
                Dim pNodo As TreeNode = sNodo.Parent
                pNodo.ChildNodes.Remove(sNodo)
            End If
            '--------------------------------------------
            'Busca el nodo seleccionado en el el destino
            'en el nodo fuente
            '---------------------------------------------
            Result = Me.NodoChild_add_node(reftre_fuente, tree_slected_node_fuente, reftre_destino.SelectedNode.Text)
            If Result <> "YES" Then
                errorM = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            Else
                '----------------------------------------------
                'Agrega el nodo en el destino si existe
                '----------------------------------------------
                If Not tree_slected_node_fuente Is Nothing Then
                    tree_slected_node_fuente.ChildNodes.Add(trnode_copia_agregar)
                    trnode_copia_agregar.Selected = True
                    up_date_trevie_fuente.Update()
                End If

            End If
            myTrans.Commit()
            myConnection.Close()
            Archiva_expediente_en_entrepano = errorM
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Archiva_expediente_en_entrepano = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Archiva_expediente_en_entrepano = errorM

        End Try
    End Function
    Function Archiva_expediente_unidad_contenedora(ByVal id_unidad_conservacion As Integer, ByVal idex_row As Integer,
                                                   ByVal id_expediente As Integer, ByVal id_usuario_gestion As Integer,
                                                   ByVal user_Gestion As String, ByVal iptrans As String, ByRef nod As TreeNode,
                                                   ByRef estru As expediente_conservacion, ByRef reftreview_destino As TreeView,
                                                   ByRef reftre_fuente As TreeView, ByRef up_date_trevie_fuente As UpdatePanel) As String
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim Result As String = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Archiva_expediente_unidad_contenedora = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update expediente_archivo set UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion &
            ",ESTADO_ARCHIVO_EXPEDIENTE=1 , ENTRE_PAÑO_ID_ENTREPAÑO=null  where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Archiva_expediente_unidad_contenedora = "Imposible archivar expediente  : " & sqlinsertcion
                errorM = "Imposible archivar expediente  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If

            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'ARCHIVA EXPEDIENTE','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           id_expediente & ",'" & iptrans & "','" & hor & "','DOCUARCHI-WEB','ARCHIVADO EN UNIDAD CONSERVACION" & id_unidad_conservacion.ToString & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim trnode_copia_agregar As TreeNode = reftre_fuente.SelectedNode
            Dim tree_slected_node_fuente As TreeNode = Nothing
            Dim tree_slected_node_destino As TreeNode = Nothing
            '-----------------------------------------------------
            'Elimina el registro para agregarlo en otra unidad
            '-----------------------------------------------------
            If Not reftre_fuente.SelectedNode Is Nothing Then
                reftre_fuente.Nodes.Remove(reftre_fuente.SelectedNode)
                Dim sNodo As TreeNode = reftre_fuente.SelectedNode
                Dim pNodo As TreeNode = sNodo.Parent
                pNodo.ChildNodes.Remove(sNodo)
            End If
            '--------------------------------------------
            'Busca el nodo seleccionado en el el destino
            'en el nodo fuente
            '---------------------------------------------
            Result = Me.NodoChild_add_node(reftre_fuente, tree_slected_node_fuente, reftreview_destino.SelectedNode.Text)
            If Result <> "YES" Then
                errorM = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            Else
                If Not tree_slected_node_fuente Is Nothing Then
                    tree_slected_node_fuente.ChildNodes.Add(trnode_copia_agregar)
                    trnode_copia_agregar.Selected = True
                    up_date_trevie_fuente.Update()
                End If

            End If
            '------------------------------------------------------
            myTrans.Commit()
            myConnection.Close()
            Archiva_expediente_unidad_contenedora = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Archiva_expediente_unidad_contenedora = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Archiva_expediente_unidad_contenedora = errorM

        End Try
    End Function
    Function NodoChild_add_node(ByRef Tre_vie As TreeView, ByRef tree_node As TreeNode, ByVal texto_nodo As String) As String
        Try
            tree_node = Nothing
            Dim Result As String = ""
            If Tre_vie.Nodes.Count > 0 Then
                Dim i As Integer = 0
                For i = 0 To Tre_vie.Nodes.Count - 1
                    Dim t = Tre_vie.Nodes(i).Text
                    Result = Nod_CHILD_busqueda(Tre_vie.Nodes(i), tree_node, texto_nodo)
                    If Not tree_node Is Nothing Then
                        NodoChild_add_node = "YES"
                        Return NodoChild_add_node
                    End If
                Next
            End If
            NodoChild_add_node = "YES"
        Catch ex As Exception
            NodoChild_add_node = ex.Message
        End Try
    End Function
    Function Nod_CHILD_busqueda(ByVal NodeC As TreeNode, ByRef tre_node As TreeNode, ByVal texto_value As String) As String
        Try
            Dim i As Integer = 0
            If NodeC.Text = texto_value Then
                tre_node = NodeC
                Nod_CHILD_busqueda = "YES"
                Return Nod_CHILD_busqueda
            End If
            For i = 0 To NodeC.ChildNodes.Count - 1
                Dim t = NodeC.ChildNodes(i).Text
                If NodeC.ChildNodes(i).Text = texto_value Then
                    tre_node = NodeC.ChildNodes(i)
                    'NodeC.ChildNodes(i).Parent.ExpandAll()
                    'node_expand_recursive(NodeC.ChildNodes(i))
                    'NodeC.ChildNodes(i).Selected = True
                    Nod_CHILD_busqueda = "YES"
                    Return Nod_CHILD_busqueda
                End If
                Nod_CHILD_busqueda(NodeC.ChildNodes(i), tre_node, texto_value)
            Next
            Nod_CHILD_busqueda = "YES"
        Catch ex As Exception
            Nod_CHILD_busqueda = ex.Message
        End Try

    End Function
    Function Archiva_expediente_unidad_contenedora_Archivado(ByVal id_unidad_conservacion As Integer,
                                                             ByVal id_expediente As Integer,
                                                             ByVal id_usuario_gestion As Integer,
                                                             ByVal user_Gestion As String,
                                                             ByVal iptrans As String) As String
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim Result As String = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Archiva_expediente_unidad_contenedora_Archivado = Result
            Exit Function
        End If
        Dim id_tipo_expediente As Integer = 0
        Result = Me.Retorna_tipo_id_expediente_por_id(id_tipo_expediente,
                                                    id_expediente)
        If Result <> "YES" Then
            Archiva_expediente_unidad_contenedora_Archivado = Result
            Exit Function
        End If
        Dim estado_archiva As Integer = 0
        Dim nombre_tipo_expediente As String = ""
        Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
        Result = ref_Class_ra_tipo_expediente.Retorna_tipo_expediente_requiere_unidad_conservacion(id_tipo_expediente,
                                                                                                   estado_archiva)
        If Result <> "YES" Then
            Archiva_expediente_unidad_contenedora_Archivado = Result
            Exit Function
        End If
        If estado_archiva = 0 Then
            Result = ref_Class_ra_tipo_expediente.Retorna_nombre_tipo_expediente_por_id_expediente(id_expediente,
                                                                                                   nombre_tipo_expediente)
            If Result <> "YES" Then
                Archiva_expediente_unidad_contenedora_Archivado = Result
                Exit Function
            End If
            Archiva_expediente_unidad_contenedora_Archivado = "El tipo de expediente (" & nombre_tipo_expediente & ") no se puede archivar "
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update expediente_archivo set UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion &
            ",ESTADO_ARCHIVO_EXPEDIENTE=1 , ENTRE_PAÑO_ID_ENTREPAÑO=null  where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Archiva_expediente_unidad_contenedora_Archivado = "Imposible archivar expediente  : " & sqlinsertcion
                errorM = "Imposible archivar expediente  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If

            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'ARCHIVA EXPEDIENTE','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           id_expediente & ",'" & iptrans & "','" & hor & "','DOCUARCHI-WEB','ARCHIVADO EN UNIDAD CONSERVACION" & id_unidad_conservacion.ToString & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Archiva_expediente_unidad_contenedora_Archivado = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Archiva_expediente_unidad_contenedora_Archivado = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Archiva_expediente_unidad_contenedora_Archivado = errorM

        End Try
    End Function
    Function Retorna_Ubicacion_expediente_por_codigo_unico(ByVal id_expediente As Integer,
                                                           ByVal tree As TreeView,
                                                           ByVal codigo_unico As String) As String
        '**************************************************************************
        'Funcion Retorna la ubicacion del expediente dentro de la ubicacion fisica
        'Fecha 2014-10-09
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************************************
        Try
            Dim refclas As New ClassGestionArchivo
            Dim Result As String = ""
            Dim refclasunidad As New ClassUnidadConservacion
            '---------------------------------------------------
            'Retorna datos  del expediente o unidad conservación
            'simple
            '---------------------------------------------------
            Dim estru_expediente() As expediente_conservacion
            Erase estru_expediente
            Result = Me.SolicitaDatosEstructuraExpediente(id_expediente, estru_expediente)
            If Result <> "YES" Then
                Retorna_Ubicacion_expediente_por_codigo_unico = Result
                Exit Function
            End If
            If estru_expediente(0).ESTADO_ARCHIVO_INIDAD = 0 Then
                Retorna_Ubicacion_expediente_por_codigo_unico = "El expediente no esta archivado"
                Exit Function
            End If
            Dim id_estante As Integer = 0
            Dim id_entrepaño As Integer = 0
            Dim id_entrapaño_idex As Integer = 0
            Dim id_modulo As Integer = 0
            Dim id_area As Integer = 0
            Dim id_piso As Integer = 0
            Dim id_empresa As Integer = 0
            Dim id_edificio As Integer = 0
            Dim Unidad_Conservacion_contenedora As String = ""
            Dim nombre_tipo_unidad As String = ""
            Dim struentrepaño() As ClassGestionArchivo.Entrapño_archivo
            Erase struentrepaño
            If estru_expediente(0).ID_UNIDAD_CONSERVACION <> 0 Then
                '----------------------------------------------------
                'Solicita los datos de la unidad de conservación
                'del expediente
                '----------------------------------------------------
                Dim estru_unidad_conservacion() As unidad_conservacion
                Erase estru_unidad_conservacion
                Result = refclasunidad.Listar_datos_Unidad_Conservacion_estructura(
                estru_expediente(0).ID_UNIDAD_CONSERVACION, estru_unidad_conservacion)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '-----------------------------------------------------
                'Retorna el nombre del tipo de unidad de conservación
                '-----------------------------------------------------
                Dim nombre_tipo_unidad_conservacion As String = ""
                Result = refclasunidad.Retorna_nombre_tipo_unidad_conservacion_por_id_unidad_conservacion(
                estru_expediente(0).ID_UNIDAD_CONSERVACION, nombre_tipo_unidad_conservacion)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                Unidad_Conservacion_contenedora = "(" & nombre_tipo_unidad_conservacion & ") " & estru_unidad_conservacion(0).CODIGO_UNICO
                '--------------------------------------------------------
                'Retorna el entrepaño de la unidad de conservación o caja
                '--------------------------------------------------------
                Result = refclas.Retorna_id_Entrepaño_id_unidad_conservacion(
                estru_unidad_conservacion(0).ID_UNIDAD_CONSERVACION, id_entrepaño)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '----------------------------------------------------------
                'Retorna el estante del entrepaño
                '----------------------------------------------------------
                Result = refclas.Retorna_Id_Estante_por_entrepaño(id_entrepaño, id_estante)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = "Imposible el id del estante " & Result
                    Exit Function
                End If
                '---------------------------------------------------------
                'Retorna id modulo
                '---------------------------------------------------------
                Result = refclas.Retorna_id_modulo_estante_archivo(id_estante, id_modulo)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '---------------------------------------------------------
                'Retorna id area 
                '---------------------------------------------------------
                Result = refclas.Retorna_id_area_archivo_por_id_modulo(id_modulo, id_area)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '----------------------------------------------------------
                'Retorna id piso
                '----------------------------------------------------------
                Result = refclas.Retorna_id_piso_archivo_por_id_area(id_area, id_piso)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '----------------------
                'Retorna id edificio
                '----------------------
                Result = refclas.Retorna_id_edifio_archivo_por_id_piso(id_piso, id_edificio)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If

            Else
                '----------------------------------------------------
                'solicita el id del entrapaño por el expediente 
                '----------------------------------------------------
                Result = refclas.Retorna_id_Entrepaño_id_expediente(
                estru_expediente(0).ID_EXPEDIENTE, id_entrepaño)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '---------------------------------------------------
                'Retorna id entrepaño
                '---------------------------------------------------
                Result = refclas.Retorna_Id_Estante_por_entrepaño(id_entrepaño, id_estante)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = "Imposible listar la identificación del estante " & Result
                    Exit Function
                End If
                '----------------------------------------------------------
                'Retorna el estante del entrepaño
                '----------------------------------------------------------
                Result = refclas.Retorna_Id_Estante_por_entrepaño(id_entrepaño, id_estante)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = "Imposible el id del estante " & Result
                    Exit Function
                End If
                '---------------------------------------------------------
                'Retorna id modulo
                '---------------------------------------------------------
                Result = refclas.Retorna_id_modulo_estante_archivo(id_estante, id_modulo)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '---------------------------------------------------------
                'Retorna id area 
                '---------------------------------------------------------
                Result = refclas.Retorna_id_area_archivo_por_id_modulo(id_modulo, id_area)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '----------------------------------------------------------
                'Retorna id piso
                '----------------------------------------------------------
                Result = refclas.Retorna_id_piso_archivo_por_id_area(id_area, id_piso)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If
                '----------------------
                'Retorna id edificio
                '----------------------
                Result = refclas.Retorna_id_edifio_archivo_por_id_piso(id_piso, id_edificio)
                If Result <> "YES" Then
                    Retorna_Ubicacion_expediente_por_codigo_unico = Result
                    Exit Function
                End If

            End If
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            Result = ref_Class_ra_tipo_expediente.Retorna_nombre_tipo_expediente_por_id_expediente(
                                                                                                   id_expediente, nombre_tipo_unidad)
            If Result <> "YES" Then
                Retorna_Ubicacion_expediente_por_codigo_unico = Result
                Exit Function
            End If
            Dim nombre_edificio As String = ""
            Dim nombre_piso As String = ""
            Dim nombre_area As String = ""
            Dim tipo_archivo As String = ""
            Dim nombre_modulo As String = ""
            Dim nombre_estante As String = ""
            Dim nombre_entrepaño As String = ""
            Result = refclas.Retorna_nombre_edificio_por_id(id_edificio, nombre_edificio)
            If Result <> "YES" Then
                Retorna_Ubicacion_expediente_por_codigo_unico = Result
                Exit Function
            End If
            Result = refclas.Retorna_nombre_piso_por_id_piso(id_piso, nombre_piso)
            If Result <> "YES" Then
                Retorna_Ubicacion_expediente_por_codigo_unico = Result
                Exit Function
            End If
            Result = refclas.Retorna_nombre_modulo_por_id_modulo(id_modulo, nombre_modulo)
            If Result <> "YES" Then
                Retorna_Ubicacion_expediente_por_codigo_unico = Result
                Exit Function
            End If
            Result = refclas.Retorna_nombre_estante_por_id_estante(id_estante, nombre_estante)
            If Result <> "YES" Then
                Retorna_Ubicacion_expediente_por_codigo_unico = Result
                Exit Function
            End If
            Result = refclas.Retorna_nombre_entrepaño_por_id_entrepaño(id_entrepaño, nombre_entrepaño)
            If Result <> "YES" Then
                Retorna_Ubicacion_expediente_por_codigo_unico = Result
                Exit Function
            End If
            Result = refclas.Retorna_nombre_area_por_id_area(id_area, nombre_area, tipo_archivo)
            If Result <> "YES" Then
                Retorna_Ubicacion_expediente_por_codigo_unico = Result
                Exit Function
            End If
            tree.Nodes.Clear()
            Dim trenod As New TreeNode
            trenod.Text = "Edificio : " & nombre_edificio
            Dim trenode_piso As New TreeNode
            trenode_piso.Text = "Piso : " & nombre_piso
            trenod.ChildNodes.Add(trenode_piso)
            Dim trenode_area As New TreeNode
            trenode_area.Text = "Area : " & nombre_area & " TIPO : (" & tipo_archivo & ")"
            trenode_piso.ChildNodes.Add(trenode_area)
            Dim trenode_modulo As New TreeNode
            trenode_modulo.Text = "Modulo : " & nombre_modulo
            trenode_area.ChildNodes.Add(trenode_modulo)
            Dim trenode_estante As New TreeNode
            trenode_estante.Text = "Estante : " & nombre_estante
            trenode_modulo.ChildNodes.Add(trenode_estante)
            Dim trenode_entrepaño As New TreeNode
            trenode_entrepaño.Text = "Entrepaño : " & nombre_entrepaño
            trenode_estante.ChildNodes.Add(trenode_entrepaño)
            Dim trenode_caja As New TreeNode
            trenode_caja.Text = "Unidad de conservación Contenedora : " & Unidad_Conservacion_contenedora & " ID " & estru_expediente(0).ID_UNIDAD_CONSERVACION
            If Unidad_Conservacion_contenedora <> "" Then
                trenode_entrepaño.ChildNodes.Add(trenode_caja)
            End If
            Dim treenode_expediente As New TreeNode
            treenode_expediente.Text = " Tipo unidad : " & estru_expediente(0).NOMBRE_TIPO_UNIDAD_DOCUMENTAL & " Clase unidad documental(Contenido) : (" & nombre_tipo_unidad & ") " & codigo_unico & " ID " & id_expediente
            If Unidad_Conservacion_contenedora = "" Then
                trenode_entrepaño.ChildNodes.Add(treenode_expediente)
            Else
                trenode_caja.ChildNodes.Add(treenode_expediente)
            End If
            Retorna_Ubicacion_expediente_por_codigo_unico = "YES"
            tree.Nodes.Add(trenod)
            tree.ExpandAll()

        Catch ex As Exception
            Retorna_Ubicacion_expediente_por_codigo_unico = "Inconsistencia funcion Retorna_Ubicacion_expediente_por_codigo_unico " & ex.Message
        End Try
    End Function
    Function Listar_datos_Expediente_estructura_por_tipo_expediente(ByVal id_expediente As Integer,
                                                                    ByRef estru_unidad_conservacion() As expediente_conservacion) As String
        '****************************************************************
        'Funcion Listar estrucutura expediente con el
        'parametro id del expediente y cartesianidad con el tipo de exp
        'Fecha 2015-01-30
        'Ing : Miguel Angel Urueta Miranda
        '*****************************************************************
        Try
            Erase estru_unidad_conservacion
            Dim campos_seleccion As String = "UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION,ID_EXPEDIENTE,CONSECUTIVO_DOCUMENTO," &
            "CONSECUTIVO_EXPEDIENTE_2,CODIGO_LARGO,CODIGO_UNICO,NUMERO_FOLIOS_CONTENIDOS," &
            "ID_USUARIO_GESTION,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD,CODIGO_SERIE_TRD,NOMBRE_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_SUBSERIE_TRD," &
            "ESTADO_EXPEDIENTE,ESTADO_ARCHIVO_EXPEDIENTE,FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," &
            "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_EXPEDIENTE,ENTRE_PAÑO_ID_ENTREPAÑO,ASUNTO_EXPEDIENTE,ID_EMPRESA_EXPEDIENTE,VOLUMEN_EXPEDIENTE,rte.NOMBRE_TIPO_EXPEDIENTE "
            Dim SqlConsulta As String = "select " & campos_seleccion & " from  expediente_archivo " &
            " inner join ra_tipo_expediente as rte on (rte.ID_TIPO_EXPEDIENTE=RA_TIP_EXPE_ID_TIPO_EXPEDIENTE)" &
                                              " where ID_EXPEDIENTE=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim result As String = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If result <> "YES" Then
                Listar_datos_Expediente_estructura_por_tipo_expediente = " Error solicitando estrucutura expediente función Listar_datos_Expediente_estructura_por_tipo_expediente " & result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then

                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_unidad_conservacion(i)

                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION")
                    End If
                    estru_unidad_conservacion(i).ID_EXPEDIENTE = Datset.Tables(0).Rows(i).Item(1)
                    estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_DOCUMENTO")
                    'estru_unidad_conservacion(i).CONSECUTIVO_EXPEDIENTE = Dat_reader.Item("CONSECUTIVO_EXPEDIENTE")
                    'estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Dat_reader.Item("CONSECUTIVO_DOCUMENTO")
                    estru_unidad_conservacion(i).CODIGO_LARGO = Datset.Tables(0).Rows(i).Item("CODIGO_LARGO")
                    estru_unidad_conservacion(i).CODIGO_UNICO = Datset.Tables(0).Rows(i).Item("CODIGO_UNICO")
                    estru_unidad_conservacion(i).TIPO_UNIDAD_CONSERVACION = 1
                    estru_unidad_conservacion(i).NUMERO_FOLIO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("NUMERO_FOLIOS_CONTENIDOS")
                    estru_unidad_conservacion(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(i).Item("ID_USUARIO_GESTION")

                    If Datset.Tables(0).Rows(i).IsNull(8) = True Then
                        estru_unidad_conservacion(i).FECHA_CREACION = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_CREACION = Datset.Tables(0).Rows(i).Item("FECHA_CREACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(9) = True Then
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = Datset.Tables(0).Rows(i).Item("CODIGO_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(10) = True Then
                        estru_unidad_conservacion(i).NOMBRE_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_AREA = Datset.Tables(0).Rows(i).Item("NOMBRE_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(11) = True Then
                        estru_unidad_conservacion(i).CODIGO_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(12) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(13) = True Then
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SUB_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(14) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SUBSERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(15) = True Then
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ESTADO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(16) = True Then
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = Datset.Tables(0).Rows(i).Item("ESTADO_ARCHIVO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(20) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(21) = True Then
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TEMA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(22) = True Then
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = 0
                    Else
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = Datset.Tables(0).Rows(i).Item(22)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(23) = True Then
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ASUNTO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(24) = True Then
                        estru_unidad_conservacion(i).ID_EMPRESA_GESTION = 0
                    Else
                        estru_unidad_conservacion(i).ID_EMPRESA_GESTION = Datset.Tables(0).Rows(i).Item("ID_EMPRESA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(25) = True Then
                        estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE = 0
                    Else
                        estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE = Datset.Tables(0).Rows(i).Item("VOLUMEN_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(26) = True Then
                        estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("NOMBRE_TIPO_EXPEDIENTE")
                    End If

                Next
                Listar_datos_Expediente_estructura_por_tipo_expediente = "YES"
                Exit Function
            Else
                Listar_datos_Expediente_estructura_por_tipo_expediente = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_datos_Expediente_estructura_por_tipo_expediente = "Inconsistencia general función Listar_datos_Expediente_estructura_por_tipo_expediente " & ex.Message
        End Try
    End Function
    Function Actualiza_Expediente_Conservacion(ByVal id_usuario_gestion As Integer,
                                               ByVal codigo_unico As String,
                                               ByVal estado_codigo_unico As Integer,
                                               ByVal id_empresa As Integer,
                                               ByVal fecha_extrema_incial As String,
                                               ByVal fecha_extrema_final As String,
                                               ByVal rango_extremo_inicial As String,
                                               ByVal rango_extremo_final As String,
                                               ByVal tema_unidad_conservacion As String,
                                               ByVal nombre_organigrama As String,
                                               ByVal nombre_area As String,
                                               ByVal nombre_serie As String,
                                               ByVal nombre_sub_serie As String,
                                               ByVal id_expediente As Integer,
                                               ByVal user_gestion As String,
                                               ByVal ip_transaccion As String,
                                               ByVal index_row As Integer,
                                               ByVal id_tipo_expediente As Integer,
                                               ByVal numero_documento_digitalizado As Integer,
                                               ByVal numero_folios_fisicos As Integer,
                                               ByVal numero_documentos_electronicos As Integer,
                                               ByVal asunto_expediente As String,
                                               ByVal volumen_expediente As Integer,
                                               ByVal nombre_tipo_unidad_conservacion As String,
                                               ByVal tipo_unidad_conservacion As String,
                                               ByVal observacion As String,
                                               ByVal nombre_tipo_unidad_documental As String,
                                               ByVal nombre_sub_seccion As String,
                                               ByVal nombre_ciclo_documental As String,
                                               ByVal nombre_fondo_documental As String,
                                               ByVal nombre_persona_expediente As String,
                                               ByVal indentificacion_persona_expediente As String,
                                               ByVal nombre_responsable As String,
                                               ByVal identificacion_responsable As String,
                                               ByVal id_instrumento As Object,
                                               ByVal requiere_unidad_fisica As Integer,
                                               ByVal tipo_actualizacion As String) As String
        Dim Result As String = "YES"
        Dim Refclas As New ClassGestionDocumental
        Dim id_organigrama As Integer = 0
        Dim stru_expediente() As expediente_conservacion = Nothing
        Result = Me.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                    stru_expediente)
        If Result <> "YES" Then
            Actualiza_Expediente_Conservacion = "Función Actualiza_Expediente_Conservacion dice " & Result
            Exit Function
        End If
        '-----------------------------------------------
        'Valida expediente auto creación
        '-----------------------------------------------
        If stru_expediente(0).ra_auto_registro_expediente_id_auto_registro <> 0 Then
            Actualiza_Expediente_Conservacion = "El expediente fue creado por auto registro del sistema imposible actualizar "
            Exit Function
        End If
        '------------------------------------------------
        'Verifica expediente produción documental 
        'no se elimine desde el gestor de expedientes
        '------------------------------------------------
        Dim estado_expediente As Integer = 0
        Dim estado_publico As Integer = 0
        Result = Retorna_estado_expediente(id_expediente,
                                           estado_expediente,
                                           estado_publico)
        If Result <> "YES" Then
            Actualiza_Expediente_Conservacion = Result
            Exit Function
        End If
        If tipo_actualizacion = "EDITAR" Then
            If estado_publico = 2 And stru_expediente(0).ID_USUARIO_GESTION <> id_usuario_gestion Then
                Actualiza_Expediente_Conservacion = "Imposible editar el expediente, debido a que pertenece a la producción documental de otro usuario "
                Exit Function
            End If
        End If
        If fecha_extrema_incial = "" Then
            Actualiza_Expediente_Conservacion = "Por favor informe la fecha extrema inicial de la unidad documental "
            Exit Function
        End If
        If nombre_tipo_unidad_documental = "" Then
            Actualiza_Expediente_Conservacion = "Por favor seleccione el tipo de expediente"
            Exit Function
        End If
        If nombre_organigrama = "" Then
            Actualiza_Expediente_Conservacion = "Por favor seleccione el organigrama"
            Exit Function
        End If
        If nombre_area = "" Then
            Actualiza_Expediente_Conservacion = "Por favor seleccione el area"
            Exit Function
        End If
        If id_tipo_expediente = 0 Then
            Actualiza_Expediente_Conservacion = "Por favor seleccione el tipo de expediente"
            Exit Function
        End If
        If id_instrumento <> 0 Then
            If nombre_ciclo_documental = "" Then
                Actualiza_Expediente_Conservacion = "Por favor seleccione el nombre ciclo de archivo"
                Exit Function
            End If
            If nombre_serie = "" And nombre_sub_serie = "" Then
                Actualiza_Expediente_Conservacion = "Por favor seleccione la serie o la sub serie del expediente"
                Exit Function
            End If
        End If
        If requiere_unidad_fisica = 1 Then
            If nombre_tipo_unidad_conservacion = "" Then
                Actualiza_Expediente_Conservacion = "Debe informar el tipo de unidad conservación"
                Exit Function
            End If
        End If
        '-------------------------------------------------
        'Validación longitud de caracteres  
        '-------------------------------------------------
        If Len(codigo_unico) > 45 Then
            Actualiza_Expediente_Conservacion = "El campo (consecutivo unidad) supera el número de caracteres permitidos (75 caracteres)"
            Exit Function
        End If
        'If Len(tema_unidad_conservacion) > 120 Then
        '    Actualiza_Expediente_Conservacion = "El campo (tema expediente) supera el número de caracteres permitidos (120 caracteres)"
        '    Exit Function
        'End If
        If Len(indentificacion_persona_expediente) > 60 Then
            Actualiza_Expediente_Conservacion = "El campo (identificacion solicitante) supera el número de caracteres permitidos (60 caracteres)"
            Exit Function
        End If
        If Len(identificacion_responsable) > 60 Then
            Actualiza_Expediente_Conservacion = "El campo (identificacion responsable) supera el número de caracteres permitidos (60 caracteres)"
            Exit Function
        End If
        If Len(rango_extremo_inicial) > 45 Then
            Actualiza_Expediente_Conservacion = "El campo (rango extremo inicial) supera el número de caracteres permitidos (45 caracteres)"
            Exit Function
        End If
        If Len(rango_extremo_final) > 45 Then
            Actualiza_Expediente_Conservacion = "El campo (rango extremo final) supera el número de caracteres permitidos (45 caracteres)"
            Exit Function
        End If
        'If Len(nombre_persona_expediente) > 60 Then
        '    Actualiza_Expediente_Conservacion = "El campo (nombre solicitante) supera el número de caracteres permitidos (60 caracteres)"
        '    Exit Function
        'End If
        'If Len(nombre_responsable) > 60 Then
        '    Actualiza_Expediente_Conservacion = "El campo (nombre responsable) supera el número de caracteres permitidos (60 caracteres)"
        '    Exit Function
        'End If
        Dim id_ciclo_documental As Integer = 0
        Dim id_fondo_documental As Integer = 0
        Dim ref_Class_ra_de_fondo_documental As New Class_ra_de_fondo_documental
        If nombre_fondo_documental <> "" Then
            Result = ref_Class_ra_de_fondo_documental.Retorna_id_fondo_documental_nombre(nombre_fondo_documental,
                                                                                         id_fondo_documental)
            If Result <> "YES" Then
                Actualiza_Expediente_Conservacion = Result
                Exit Function
            End If
        End If
        Dim ref_Class_ra_de_tipos_ciclos_archivo As New Class_ra_de_tipos_ciclos_archivo
        If nombre_ciclo_documental <> "" Then
            Result = ref_Class_ra_de_tipos_ciclos_archivo.Retorna_id_ciclo_archivo_nombre(nombre_ciclo_documental,
                                                                                          id_ciclo_documental)
            If Result <> "YES" Then
                Actualiza_Expediente_Conservacion = Result
                Exit Function
            End If
        End If
        '-----------------------------------------------------
        'Especifica el ciclo de archivo y el fondo documental
        '-----------------------------------------------------
        Dim ref_id_ciclo_documental As Object = Nothing
        Dim ref_id_fondo_documental As Object = Nothing
        Dim ref_nombre_fondo_documental As String = ""
        Dim ref_nombre_ciclo_documental As String = ""
        If id_ciclo_documental = 0 Then
            ref_id_ciclo_documental = "Null"
        Else
            ref_id_ciclo_documental = id_ciclo_documental
        End If
        If id_fondo_documental = 0 Then
            ref_id_fondo_documental = "Null"
        Else
            ref_id_fondo_documental = id_fondo_documental
        End If
        If nombre_ciclo_documental <> "" Then
            ref_nombre_ciclo_documental = "'" & nombre_ciclo_documental & "'"
        Else
            ref_nombre_ciclo_documental = "Null"
        End If
        If nombre_fondo_documental <> "" Then
            ref_nombre_fondo_documental = "'" & nombre_fondo_documental & "'"
        Else
            ref_nombre_fondo_documental = "Null"
        End If
        Dim ref_nombre_persona_expediente As String = ""
        Dim ref_indentificacion_persona_expediente As String = ""
        If nombre_persona_expediente = "" Then
            ref_nombre_persona_expediente = "Null"
        Else
            ref_nombre_persona_expediente = "'" & nombre_persona_expediente & "'"
        End If
        If indentificacion_persona_expediente = "" Then
            ref_indentificacion_persona_expediente = "Null"
        Else
            ref_indentificacion_persona_expediente = "'" & indentificacion_persona_expediente & "'"
        End If
        Dim ref_nombre_responsable As String = ""
        If nombre_responsable = "" Then
            ref_nombre_responsable = "Null"
        Else
            ref_nombre_responsable = "'" & nombre_responsable & "'"
        End If
        Dim ref_identificacion_responsable As String = ""
        If identificacion_responsable = "" Then
            ref_identificacion_responsable = "Null"
        Else
            ref_identificacion_responsable = "'" & identificacion_responsable & "'"
        End If
        Dim Reclas_registro_organigrama As New Class_registro_organigrama
        Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama, id_empresa, id_organigrama)
        If Result <> "YES" Then
            Actualiza_Expediente_Conservacion = Result
            Exit Function
        End If

        Dim codigo_area As Integer = 0
        Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
        Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama,
                                                                                 codigo_area,
                                                                                 nombre_area)
        If Result <> "YES" Then
            Actualiza_Expediente_Conservacion = Result
            Exit Function
        End If

        Dim id_serie As Object = 0
        Dim consecutivo_serie As Integer = 0
        Dim consecutivo_Sub_serie As Integer = 0
        Dim ref_Class_series_documentales As New Class_series_documentales
        If nombre_serie <> "" Then
            Result = ref_Class_series_documentales.Retorna_Id_serie_instrumento_Documental(codigo_area.ToString,
                                                                                           nombre_serie,
                                                                                           id_instrumento,
                                                                                           id_serie,
                                                                                           consecutivo_serie,
                                                                                           consecutivo_Sub_serie)
            If Result <> "YES" Then
                Actualiza_Expediente_Conservacion = Result
                Exit Function
            End If
        End If
        Dim id_consecutivo_doc As Integer = 0
        Dim id_sub_serie As Object = 0
        Dim ref_Class_subseries_documentales As New Class_subseries_documentales
        If nombre_sub_serie <> "" Then
            Result = ref_Class_subseries_documentales.Retorna_Id_Subserie_Consecutivo_TipDoc(nombre_sub_serie,
                                                                                             id_serie,
                                                                                             id_sub_serie,
                                                                                             id_consecutivo_doc)
            If Result <> "YES" Then
                Actualiza_Expediente_Conservacion = Result
                Exit Function
            End If
        End If
        '------------------------------------------
        'Actualiza los tiempos de retención si hay
        'cambios de fecha de apertura de expediente
        'y cambio de tipo instrumento
        '-------------------------------------------
        Dim spli_fecha() As String = Nothing
        Dim fecha_ret_gestion As String = "Null"
        Dim fecha_ret_central As String = "Null"
        Dim id_tipo_instrumento As Integer = 0
        If stru_expediente(0).fecha_ret_gestion <> "" Then
            spli_fecha = stru_expediente(0).fecha_ret_gestion.Split("/")
            fecha_ret_gestion = "'" & spli_fecha(2) & "-" & spli_fecha(1) & "-" & spli_fecha(0) & "'"
        End If
        If stru_expediente(0).fecha_ret_central <> "" Then
            spli_fecha = stru_expediente(0).fecha_ret_central.Split("/")
            fecha_ret_central = "'" & spli_fecha(2) & "-" & spli_fecha(1) & "-" & spli_fecha(0) & "'"
        End If
        Dim ref_fecha_extrema As String = stru_expediente(0).FECHA_EXTREMA_INICIAL
        If ref_fecha_extrema <> "" Then
            spli_fecha = ref_fecha_extrema.Split("/")
            ref_fecha_extrema = spli_fecha(2) & "-" & spli_fecha(1) & "-" & spli_fecha(0)
        End If
        '-----------------------------------------------------------------
        'Detecta cambios de instrumento y fecha extrema
        '-----------------------------------------------------------------
        Dim Refclas_gagestioninstrumento As New ClassGaGestionInstrumento
        Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
        If stru_expediente(0).id_instrumento <> id_instrumento Or ref_fecha_extrema <> fecha_extrema_incial Or stru_expediente(0).CODIGO_SERIE <>
            id_serie Or stru_expediente(0).CODIGO_SUBSERIE <> id_sub_serie Then
            If id_instrumento <> 0 Then
                Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(id_instrumento,
                                                                                   id_tipo_instrumento)
                If Result <> "YES" Then
                    Actualiza_Expediente_Conservacion = Result
                    Exit Function
                End If
                '----------------------------------------------
                'Determina existecnia del tipo instrumento
                '----------------------------------------------
                If id_tipo_instrumento = 0 Then
                    Actualiza_Expediente_Conservacion = "Imposible determinar el tipo de instrumento "
                    Exit Function
                End If
                '----------------------------------------------
                'Tiempos de retención tablas de retención
                '----------------------------------------------
                If id_tipo_instrumento = 1 Then
                    Result = Me.Retorna_tiempos_de_retencion_tablas_retencion(id_serie, id_sub_serie, fecha_extrema_incial,
                                                                              fecha_ret_gestion, fecha_ret_central)
                    If Result <> "YES" Then
                        Actualiza_Expediente_Conservacion = Result
                        Exit Function
                    End If
                    If fecha_ret_gestion = "" Then
                        Actualiza_Expediente_Conservacion = "La serie o sub serie no registran tiempos de retención "
                        Exit Function
                    End If
                    If fecha_ret_central = "" Then
                        fecha_ret_central = "Null"
                    Else
                        fecha_ret_central = "'" & fecha_ret_central & "'"
                    End If
                    fecha_ret_gestion = "'" & fecha_ret_gestion & "'"
                End If
                '----------------------------------------------
                'Tiempos de retención tablas de valoración
                '----------------------------------------------
                If id_tipo_instrumento = 2 Then
                    Result = Me.Retorna_tiempos_de_retencion_tablas_de_valoracion(id_serie, id_sub_serie, fecha_extrema_incial,
                                                                                  fecha_ret_central)
                    If Result <> "YES" Then
                        Actualiza_Expediente_Conservacion = Result
                        Exit Function
                    End If
                    If fecha_ret_central = "" Then
                        Actualiza_Expediente_Conservacion = "La serie o sub serie no registran tiempos de retención "
                        Exit Function
                    End If
                    fecha_ret_central = "'" & fecha_ret_central & "'"
                    fecha_ret_gestion = "Null"
                End If
            Else
                fecha_ret_gestion = "Null"
                fecha_ret_central = "Null"
            End If
        End If
        '------------------------------------------
        'Retorna id tipo unidad documental
        '------------------------------------------
        Dim id_tipo_unidad_documental As Integer = 0
        Dim refclas_unidad_conservacion As New ClassUnidadConservacion
        Dim ref_Class_tipo_unidad_documental As New Class_tipo_unidad_documental
        Result = ref_Class_tipo_unidad_documental.Retorna_id_tipo_unidad_documental_por_nombre(nombre_tipo_unidad_documental,
                                                                                               id_tipo_unidad_documental)
        If Result <> "YES" Then
            Actualiza_Expediente_Conservacion = Result
            Exit Function
        End If
        '---------------------------------------------
        'Retorna codigo subseccion
        '---------------------------------------------
        Dim id_sub_seccion As Object = 0
        'If nombre_sub_seccion <> "" Then
        '    Result = Refclas.Retorna_codigo_sub_area_departamento_radicacion(codigo_area, nombre_sub_seccion, id_sub_seccion)
        '    If Result <> "YES" Then
        '        Actualiza_Expediente_Conservacion = Result
        '        Exit Function
        '    End If
        'End If
        Dim re_nombre_sub_seccion As String = ""
        If nombre_sub_seccion = "" Then
            re_nombre_sub_seccion = "null"
        Else
            re_nombre_sub_seccion = "'" & nombre_sub_seccion & "'"
        End If
        If id_sub_seccion = 0 Then
            id_sub_seccion = "null"
        End If
        '---------------------------------------------
        'Reterna el tipo de unidad de conservación
        'ejemplo carpeta etc
        '---------------------------------------------
        Dim id_tipo_unidad_conservacion As Integer = 0
        Dim ref_id_tipo_unidad_conservacion As Object = "null"
        Dim ref_Class_tipo_unidad_conservacion As New Class_tipo_unidad_conservacion
        If nombre_tipo_unidad_conservacion <> "" Then
            Result = ref_Class_tipo_unidad_conservacion.Retorna_id_tipo_unidad_conservacion_expediente(nombre_tipo_unidad_conservacion,
                                                                                                       id_tipo_unidad_conservacion,
                                                                                                       2)
            If Result <> "YES" Then
                Actualiza_Expediente_Conservacion = Result
                Exit Function
            End If
            If id_tipo_unidad_conservacion = 0 Then
                Actualiza_Expediente_Conservacion = "Imposible encontrar el tipo de unidad de conservación del expediente"
                Exit Function
            End If
        End If

        If id_tipo_unidad_conservacion = -1 Or id_tipo_unidad_conservacion = 0 Then
            ref_id_tipo_unidad_conservacion = "null"
        Else
            ref_id_tipo_unidad_conservacion = id_tipo_unidad_conservacion
        End If
        Dim ref_nombre_tipo_unidad_conservacion As String = "null"
        If nombre_tipo_unidad_conservacion = "" Then
            ref_nombre_tipo_unidad_conservacion = "null"
        Else
            ref_nombre_tipo_unidad_conservacion = "'" & nombre_tipo_unidad_conservacion & "'"
        End If
        Dim ref_nombre_serie As String = "null"
        If nombre_serie = "" Then
            ref_nombre_serie = "null"
        Else
            ref_nombre_serie = "'" & nombre_serie & "'"
        End If
        Dim ref_nombre_sub_serie As String = "null"
        If nombre_sub_serie = "" Then
            ref_nombre_sub_serie = "null"
        Else
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        If id_serie = 0 Then
            id_serie = "null"
        End If
        If id_sub_serie = 0 Then
            id_sub_serie = "null"
        End If

        ''-------------------------------------------------
        ''Verfica el codigo es manual
        ''-------------------------------------------------
        'If estado_codigo_unico = 1 Then
        '    If codigo_unico = "" Then
        '        Actualiza_Expediente_Conservacion = "Debe informar el código"
        '        Exit Function
        '    End If
        '    Result = Verfica_Existencia_Codigo_Unico_Expediente(codigo_unico, id_empresa, volumen_expediente, codigo_area)
        '    If Result <> "YES" Then
        '        Actualiza_Expediente_Conservacion = Result
        '        Exit Function
        '    End If
        'End If
        '---------------------------------------
        'Vefica formato fechas extremas
        '---------------------------------------
        Dim re_fecha_extrema_incial As String = ""
        Dim re_fecha_extrema_final As String = ""
        If fecha_extrema_incial <> "" Then
            'Dim splifecha() As String = fecha_extrema_incial.Split("/")
            re_fecha_extrema_incial = "'" & fecha_extrema_incial & "'"
        Else
            re_fecha_extrema_incial = "null"
        End If
        If fecha_extrema_final <> "" Then
            'Dim splifecha() As String = fecha_extrema_final.Split("/")
            re_fecha_extrema_final = "'" & fecha_extrema_final & "'"
        Else
            re_fecha_extrema_final = "null"
        End If

        If rango_extremo_inicial = "" Then
            rango_extremo_inicial = "null"
        Else
            rango_extremo_inicial = "'" & rango_extremo_inicial & "'"
        End If

        If rango_extremo_final = "" Then
            rango_extremo_final = "null"
        Else
            rango_extremo_final = "'" & rango_extremo_final & "'"
        End If

        If tema_unidad_conservacion = "" Then
            tema_unidad_conservacion = "null"
        Else
            tema_unidad_conservacion = "'" & tema_unidad_conservacion & "'"
        End If
        Dim re_fasunto_expediente As String = asunto_expediente
        If asunto_expediente = "" Then
            asunto_expediente = "'" & asunto_expediente & "'"
        Else
            asunto_expediente = "'" & asunto_expediente & "'"
        End If
        Dim ref_observacion As String = ""
        If observacion = "" Then
            ref_observacion = "null"
        Else
            ref_observacion = "'" & observacion & "'"
        End If
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date1al)
        If Result <> "YES" Then
            Actualiza_Expediente_Conservacion = Result
            Exit Function
        End If
        If id_instrumento = 0 Then
            id_instrumento = "null"
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad As String = 0
        Dim errorM As String = "YES"
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
            Dim sqlinsertcion As String = "update expediente_archivo set CODIGO_UNICO='" & codigo_unico & "'," &
            "FECHA_EXTREMA_INICIAL=" & re_fecha_extrema_incial & ",FECHA_EXTREMA_FINAL=" & re_fecha_extrema_final &
            ",RANGO_EXTREMO_INICIAL=" & rango_extremo_inicial & ",RANGO_EXTREMO_FINAL=" & rango_extremo_final &
            ",TEMA_EXPEDIENTE=" & tema_unidad_conservacion & ",NOMBRE_AREA_TRD='" & nombre_area & "'" &
            ",CODIGO_AREA_TRD=" & codigo_area & ",NOMBRE_SERIE_TRD=" & ref_nombre_serie & ",CODIGO_SERIE_TRD=" &
            id_serie & ",NOMBRE_SUBSERIE_TRD=" & ref_nombre_sub_serie & ",CODIGO_SUB_SERIE_TRD=" & id_sub_serie &
            ",ASUNTO_EXPEDIENTE=" & asunto_expediente & ",RA_TIP_EXPE_ID_TIPO_EXPEDIENTE=" & id_tipo_expediente &
            ",TIPO_EXPEDIENTE=" & id_tipo_expediente &
            ",NUMERO_DIGITALIZADO_CONTENIDO=" & numero_documento_digitalizado & ",NUMERO_FOLIOS_CONTENIDOS=" & numero_folios_fisicos &
            ",NUMERO_ELECTRONICO_CONTENIDO=" & numero_documentos_electronicos &
            ",TIPO_UNIDAD_ID_TIPO=" & ref_id_tipo_unidad_conservacion &
            ",TIPO_UNIDAD_CONSERVACION=" & ref_nombre_tipo_unidad_conservacion &
            ",OBSERVACION_EXPEDIENTE=" & ref_observacion &
            ",ID_TIPO_UNIDAD_DOCUMENTAL=" & id_tipo_unidad_documental &
            ",NOMBRE_TIPO_UNIDAD_DOCUMENTAL='" & nombre_tipo_unidad_documental & "'" &
            ",ID_SUB_AREA=" & id_sub_seccion &
            ",NOMBRE_SUB_AREA=" & re_nombre_sub_seccion &
            ",ID_FONDO=" & ref_id_fondo_documental &
            ",NOMBRE_FONDO=" & ref_nombre_fondo_documental &
            ",Id_tipos_ciclo_archivo=" & ref_id_ciclo_documental &
            ",NOMBRE_CICLO_ARCHIVO=" & ref_nombre_ciclo_documental &
            ",NOMBRE_PERSONA_EXPEDIENTE=" & ref_nombre_persona_expediente &
            ",IDENTIFICACION_PERSONA_EXPEDIENTE=" & ref_indentificacion_persona_expediente &
             ",NOMBRE_RESPONSABLE_EXPEDIENTE=" & ref_nombre_responsable &
            ",IDENFICACION_RESPONSABLE_EXPEDIENTE=" & ref_identificacion_responsable &
            ",fecha_ret_central=" & fecha_ret_central &
            ",fecha_ret_gestion=" & fecha_ret_gestion &
            ",id_instrumento=" & id_instrumento &
            " where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_Expediente_Conservacion = "Imposible registrar expediente  : " & sqlinsertcion
                'myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar unidad de conservacion  : " & sqlinsertcion
                Exit Function
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) values (" &
            "'EDITA UNIDAD','" & user_gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
            id_expediente & ",'" & ip_transaccion & "','" & hor & "','GESTOR WEB')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()

            Actualiza_Expediente_Conservacion = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Actualiza_Expediente_Conservacion = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_Expediente_Conservacion = errorM

        End Try
    End Function
    Function Actualiza_expediente_produccion(ByVal id_usuario_gestion As Integer,
                                             ByVal nombre_expediente As String,
                                             ByVal TextBox_nombre_persona_expediente_actualizar As String,
                                             ByVal TextBox_identificacion_persona_expediente_actualizar As String,
                                             ByVal TextBox_asunto_expediente_actualizar As String,
                                             ByVal TextBox_tema_expediente_actualizar As String,
                                             ByVal TextBox_observacion_expediente_actualizar As String,
                                             ByVal nombre_serie As String,
                                             ByRef nombre_sub_serie As String,
                                             ByRef drow_list_fondo As DropDownList,
                                             ByVal id_expediente As Integer,
                                             ByRef ref_trenode As TreeNode,
                                             ByRef ref_update As UpdatePanel,
                                             ByVal user_gestion As String,
                                             ByVal ip_transaccion As String,
                                             ByVal id_serie As Integer,
                                             ByVal id_sub_serie As Integer,
                                             ByVal nombre_gabinete_producion As String,
                                             ByVal id_instrumento_archivistico As Integer) As String

        Dim refclasproduccion As New ClassGaProducionDocumental
        Dim Result As String = ""
        '----------------------------------------------------
        'Solicita opciones de aplicacion tablas de retención
        '----------------------------------------------------
        Dim stru_config As STRU_CONFIG_PRODUCION
        Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
        Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
        If Result <> "YES" Then
            Actualiza_expediente_produccion = Result
            Exit Function
        End If
        '----------------------------------------------------
        'Verifica existencia nombre carpeta
        '----------------------------------------------------
        If nombre_expediente = "" Then
            Actualiza_expediente_produccion = "Debe informar el nombre del expediente o carpeta "
            Exit Function
        End If

        '----------------------------------------------------
        'Solicita identificacion area usuarios gestion
        '----------------------------------------------------
        Dim id_area_usuario_gestion As Integer = 0
        '-------------------------------------------------------------------
        'Verifica existencia clasificación y aplicación tablas de retención
        '-------------------------------------------------------------------
        If id_instrumento_archivistico = 0 And stru_config.ACTIVA_OBLIGA_TRD = 1 Then
            Actualiza_expediente_produccion = "Debe seleccionar el intrumento al que pertenece la carpeta o expediente "
            Exit Function
        End If
        If nombre_serie = "" And stru_config.ACTIVA_OBLIGA_TRD = 1 Then
            Actualiza_expediente_produccion = "Debe seleccionar el nombre de la serie o sub serie a la que pertenece la carpeta o expediente "
            Exit Function
        End If
        '----------------------------------------------
        'Verifica existencia selección fondo documental 
        '----------------------------------------------
        If drow_list_fondo.Text = "" And stru_config.ACTIVA_OBLIGA_TRD = 1 Then
            Actualiza_expediente_produccion = "Debe seleccionar el fondo documenta al que pertenecera el expediente o carpeta "
            Exit Function
        End If
        Dim nombre_serie_ As String = nombre_serie
        Dim Ref_serie As New Class_series_documentales
        If id_serie <> 0 Then
            Result = Ref_serie.Solicita_nombre_serie_documental(id_serie,
                                                                nombre_serie_)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
        End If
        '--------------------------------------------------------
        'Determina el tipo instrumento de la serie
        '--------------------------------------------------------
        Dim Id_tipo_instrumento As Integer = 0
        Dim RefclassGestionInstrumento As New ClassGaGestionInstrumento
        Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
        If id_instrumento_archivistico <> 0 Then
            Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(id_instrumento_archivistico,
                                                                                Id_tipo_instrumento)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
        End If
        '-------------------------------------------------------------------
        'Verfica tipos documentales relacionados a al serie o  sub serie
        '-------------------------------------------------------------------
        Dim numero_tipos_relacionados As Integer = 0
        If nombre_sub_serie <> "" Then
            Result = refclasproduccion.Solicita_numero_tipos_de_documentos_relacionados_con_la_sub_serie(id_sub_serie,
                                                                                                         numero_tipos_relacionados)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
            '--------------------------------------------------------
            'Numero tipos documentales relacionados  a la sub serie
            '--------------------------------------------------------
            If numero_tipos_relacionados = 0 And Id_tipo_instrumento = 1 Then
                Actualiza_expediente_produccion = "La sub serie ( " & id_sub_serie & " ) no tiene tipos documentales relacionados, imposible crear la carpeta expediente"
                Exit Function
            End If
        Else
            numero_tipos_relacionados = 0
            '---------------------------------------------------
            'Verifica tipos documentales relacionados a la serie
            '---------------------------------------------------
            If nombre_serie <> "" Then
                Result = refclasproduccion.Solicita_numero_tipos_de_documentos_relacionados_con_la_serie(id_serie,
                                                                                                         numero_tipos_relacionados)
                If Result <> "YES" Then
                    Actualiza_expediente_produccion = Result
                    Exit Function
                End If
                '--------------------------------------------------
                'Numero tipos documentales relacionados  a la serie
                '--------------------------------------------------
                If numero_tipos_relacionados = 0 And Id_tipo_instrumento = 1 Then
                    Actualiza_expediente_produccion = "La serie ( " & id_serie & " ) no tiene tipos documentales relacionados, imposible crear la carpeta expediente"
                    Exit Function
                End If
            End If
        End If

        '---------------------------------------------------------------
        'Solicita organigrama relacionado a  instrumento archivístico
        '---------------------------------------------------------------
        Dim id_organigrama As Integer = 0
        If id_instrumento_archivistico <> 0 Then
            Result = RefclassGestionInstrumento.Solicita_id_organigrama_instrumento(id_instrumento_archivistico,
                                                                                    id_organigrama)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
            '----------------------------------------------------------------
            'Existencia organigrama relacionado al instrumento archivístico
            '----------------------------------------------------------------
            If id_organigrama = 0 Then
                Actualiza_expediente_produccion = "El instrumento archivístico ( " & id_instrumento_archivistico & " )  no esta relaciona a un organigrama"
                Exit Function
            End If
        End If
        '----------------------------------------------
        'Solicita id area del expediente y nombre area
        '----------------------------------------------
        Dim id_area_departamento As Integer = 0
        Dim nombre_area As String = ""
        Dim Ref_GAEexpediente As New ClassGaExpediente
        Dim stru_expediente() As expediente_conservacion = Nothing
        Result = Ref_GAEexpediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                   stru_expediente)
        If Result <> "YES" Then
            Actualiza_expediente_produccion = "Función Actualiza_expediente_produccion dice " & Result
            Exit Function
        End If
        If stru_expediente Is Nothing Then
            Actualiza_expediente_produccion = "Imposible encontrar la estructura del expediente (" & id_expediente & ")"
            Exit Function
        End If
        '-----------------------------------------------
        'Valida expediente auto creación
        '-----------------------------------------------
        If stru_expediente(0).ra_auto_registro_expediente_id_auto_registro <> 0 Then
            Actualiza_expediente_produccion = "El expediente fue creado por auto registro del sistema imposible actualizar "
            Exit Function
        End If
        'id_area_departamento = stru_expediente(0).CODIGO_AREA_TRD
        'nombre_area = stru_expediente(0).NOMBRE_AREA
        Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
        '--------------------------------------------------------------
        'Solicita nombre area relacionada a la producción documental
        '--------------------------------------------------------------
        If id_serie <> 0 Then
            Result = RefclassGestionInstrumento.Solicita_id_area_serie_documental(id_serie,
                                                                                  id_area_departamento)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
            If id_area_departamento = 0 Then
                Actualiza_expediente_produccion = "La serie ( " & id_serie & " ) no esta relacionda a un área o departamento "
                Exit Function
            End If
            '------------------------------
            'Solicita nombre area
            '------------------------------
            Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area_departamento,
                                                                                       nombre_area)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
            If nombre_area = "" Then
                Actualiza_expediente_produccion = "Imposible encontrar el nombre del área con la identificación (" & id_area_departamento & ")"
                Exit Function
            End If

        Else
            '-------------------------------------------------------------------
            'Solicita nombre serie por el área relacionada al usuario de gestión
            '-------------------------------------------------------------------
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(id_usuario_gestion,
                                                                                           id_area_departamento)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
            If id_area_departamento <> 0 Then
                '------------------------------------------------------
                'Solicita el nombre del área del usuario de gestión  
                '------------------------------------------------------
                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area_departamento,
                                                                                           nombre_area)
                If Result <> "YES" Then
                    Actualiza_expediente_produccion = Result
                    Exit Function
                End If
                If nombre_area = "" Then
                    Actualiza_expediente_produccion = "Imposible encontrar el nombre del área con la identificación (" & id_area_departamento & ")"
                    Exit Function
                End If
            Else
                Actualiza_expediente_produccion = "El usuario de gestión no  se encuentra relacionado al área "
                Exit Function
            End If
        End If

        '---------------------------
        'Solicita nombre organigrama
        '---------------------------
        Dim nombre_organigrama As String = ""
        If id_organigrama <> 0 Then
            Result = RefclassGestionInstrumento.Solicita_nombre_organigrama_por_identidad_organigrama(id_organigrama,
                                                                                                      nombre_organigrama)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
            '----------------------------------
            'Existencia nombre organigrama
            '----------------------------------
            If nombre_organigrama = "" Then
                Actualiza_expediente_produccion = "Imposible encontrar el nombre del organigrama relacionado al organigrama (" & id_organigrama & " )"
                Exit Function
            End If
        End If

        '------------------------------------------------------------------------------------------
        'Solicitud elementos basicos de clasificación sin clasificación del expediente o carpeta
        '------------------------------------------------------------------------------------------
        Dim id_empresa_usuario_gestion As Integer = HttpContext.Current.Session.Item("GA_IDEMPRESA")
        Dim Refclastrd As New ClassTrdDocumental
        Dim Ref_class_registro_organigrama As New Class_registro_organigrama
        Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
        If id_organigrama = 0 Then
            Result = Ref_class_registro_organigrama.Solicita_datos_caracterizacion_organigrama_activo(id_empresa_usuario_gestion,
                                                                                                       id_organigrama,
                                                                                                       nombre_organigrama)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(id_usuario_gestion,
                                                                                           id_area_usuario_gestion)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
            If id_area_usuario_gestion <> 0 Then
                '------------------------------------------------------
                'Solicita el nombre del área del usuario de gestión  
                '------------------------------------------------------
                Result = ref_Class_areas_depart_radicacion.Retorna_nombre_area_por_id_area(id_area_usuario_gestion,
                                                                                           "")
                If Result <> "YES" Then
                    Actualiza_expediente_produccion = Result
                    Exit Function
                End If
            Else
                Actualiza_expediente_produccion = "El usuario de gestión no  se encuentra relacionado al área "
                Exit Function
            End If

        End If
        '------------------------------------------
        'Actualiza los tiempos de retención si hay
        'cambios de fecha de apertura de expediente
        'y cambio de tipo instrumento
        '-------------------------------------------
        Dim spli_fecha() As String = Nothing
        Dim fecha_ret_gestion As String = "Null"
        Dim fecha_ret_central As String = "Null"
        If stru_expediente(0).fecha_ret_gestion <> "" Then
            spli_fecha = stru_expediente(0).fecha_ret_gestion.Split("/")
            fecha_ret_gestion = "'" & spli_fecha(2) & "-" & spli_fecha(1) & "-" & spli_fecha(0) & "'"
        End If
        If stru_expediente(0).fecha_ret_central <> "" Then
            spli_fecha = stru_expediente(0).fecha_ret_central.Split("/")
            fecha_ret_central = "'" & spli_fecha(2) & "-" & spli_fecha(1) & "-" & spli_fecha(0) & "'"
        End If
        Dim ref_fecha_extrema As String = stru_expediente(0).FECHA_EXTREMA_INICIAL
        If ref_fecha_extrema <> "" Then
            spli_fecha = ref_fecha_extrema.Split("/")
            ref_fecha_extrema = spli_fecha(2) & "-" & spli_fecha(1) & "-" & spli_fecha(0)
        End If
        Dim fecha_extrema_incial As String = ref_fecha_extrema
        '-----------------------------------------------------------------
        'Detecta cambios de instrumento, serie y sub serie
        '-----------------------------------------------------------------
        Dim Refclas_gagestioninstrumento As New ClassGaGestionInstrumento
        If stru_expediente(0).id_instrumento <> id_instrumento_archivistico _
            Or stru_expediente(0).CODIGO_SERIE <>
            id_serie Or
            stru_expediente(0).CODIGO_SUBSERIE <> id_sub_serie Then
            If id_instrumento_archivistico <> 0 Then

                '----------------------------------------------
                'Determina existecnia del tipo instrumento
                '----------------------------------------------
                If Id_tipo_instrumento = 0 Then
                    Actualiza_expediente_produccion = "Imposible determinar el tipo de instrumento "
                    Exit Function
                End If
                '----------------------------------------------
                'Tiempos de retención tablas de retención
                '----------------------------------------------
                If Id_tipo_instrumento = 1 Then
                    Result = Me.Retorna_tiempos_de_retencion_tablas_retencion(id_serie,
                                                                              id_sub_serie,
                                                                              fecha_extrema_incial,
                                                                              fecha_ret_gestion,
                                                                              fecha_ret_central)
                    If Result <> "YES" Then
                        Actualiza_expediente_produccion = Result
                        Exit Function
                    End If
                    If fecha_ret_gestion = "" Then
                        Actualiza_expediente_produccion = "La serie o sub serie no registran tiempos de retención "
                        Exit Function
                    End If
                    If fecha_ret_central = "" Then
                        fecha_ret_central = "Null"
                    Else
                        fecha_ret_central = "'" & fecha_ret_central & "'"
                    End If
                    fecha_ret_gestion = "'" & fecha_ret_gestion & "'"
                End If
                '----------------------------------------------
                'Tiempos de retención tablas de valoración
                '----------------------------------------------
                If Id_tipo_instrumento = 2 Then
                    Result = Me.Retorna_tiempos_de_retencion_tablas_de_valoracion(id_serie, id_sub_serie,
                                                                                  fecha_extrema_incial,
                                                                                  fecha_ret_central)
                    If Result <> "YES" Then
                        Actualiza_expediente_produccion = Result
                        Exit Function
                    End If
                    If fecha_ret_central = "" Then
                        Actualiza_expediente_produccion = "La serie o sub serie no registran tiempos de retención "
                        Exit Function
                    End If
                    fecha_ret_central = "'" & fecha_ret_central & "'"
                    fecha_ret_gestion = "Null"
                End If
            Else
                fecha_ret_gestion = "Null"
                fecha_ret_central = "Null"
            End If
        End If
        '-----------------------------------------------------
        'Solicita id fondo documental
        '-----------------------------------------------------
        Dim id_fondo_documental As Object = 0
        Dim ref_Class_ra_de_fondo_documental As New Class_ra_de_fondo_documental
        If drow_list_fondo.Text <> "" Then
            Result = ref_Class_ra_de_fondo_documental.Retorna_id_fondo_documental_nombre(drow_list_fondo.Text,
                                                                                         id_fondo_documental)
            If Result <> "YES" Then
                Actualiza_expediente_produccion = Result
                Exit Function
            End If
        End If
        Dim ref_id_fondo As Object = "Null"
        If id_fondo_documental <> 0 Then
            ref_id_fondo = id_fondo_documental
        End If
        Dim ref_nombre_fondo As String = ""
        If drow_list_fondo.Text = "" Then
            ref_nombre_fondo = "Null"
        Else
            ref_nombre_fondo = "'" & drow_list_fondo.Text & "'"
        End If
        Dim ref_nombre_serie As String = ""
        If nombre_serie_ = "" Then
            ref_nombre_serie = "null"
        Else
            ref_nombre_serie = "'" & nombre_serie_ & "'"
        End If
        Dim ref_nombre_sub_serie As String = ""
        If nombre_sub_serie = "" Then
            ref_nombre_sub_serie = "null"
        Else
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        Dim ref_id_instrumento_archivistico As Object = Nothing
        If id_instrumento_archivistico = 0 Then
            ref_id_instrumento_archivistico = "null"
        Else
            ref_id_instrumento_archivistico = id_instrumento_archivistico
        End If
        Dim ref_nombre_persona As String = ""
        If TextBox_nombre_persona_expediente_actualizar = "" Then
            ref_nombre_persona = "null"
        Else
            ref_nombre_persona = "'" & TextBox_nombre_persona_expediente_actualizar & "'"
        End If
        Dim ref_identificacion_persona As String = ""
        If TextBox_identificacion_persona_expediente_actualizar = "" Then
            ref_identificacion_persona = "null"
        Else
            ref_identificacion_persona = "'" & TextBox_identificacion_persona_expediente_actualizar & "'"
        End If
        Dim ref_asunto As String = ""
        If TextBox_asunto_expediente_actualizar = "" Then
            ref_asunto = "null"
        Else
            ref_asunto = "'" & TextBox_asunto_expediente_actualizar & "'"
        End If
        Dim ref_tema As String = ""
        If TextBox_tema_expediente_actualizar = "" Then
            ref_tema = "null"
        Else
            ref_tema = "'" & TextBox_tema_expediente_actualizar & "'"
        End If
        Dim ref_observacion As String = ""
        If TextBox_observacion_expediente_actualizar = "" Then
            ref_observacion = "null"
        Else
            ref_observacion = "'" & TextBox_observacion_expediente_actualizar & "'"
        End If
        Dim ref_nombre_area As String = ""
        If nombre_area = "" Then
            ref_nombre_area = "null"
        Else
            ref_nombre_area = "'" & nombre_area & "'"
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Actualiza_expediente_produccion = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad As String = 0
        Dim errorM As String = "YES"
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update expediente_archivo set " &
             "NOMBRE_SERIE_TRD=" & ref_nombre_serie & ",CODIGO_SERIE_TRD=" &
            id_serie & ",NOMBRE_SUBSERIE_TRD=" &
            ref_nombre_sub_serie &
            ",CODIGO_SUB_SERIE_TRD=" & id_sub_serie &
            ",ID_FONDO=" & ref_id_fondo &
            ",NOMBRE_FONDO=" & ref_nombre_fondo &
            ",ALEAS_EXPEDIENTE='" & nombre_expediente & "'" &
            ",CODIGO_UNICO='" & nombre_expediente & "'" &
            ",id_instrumento=" & ref_id_instrumento_archivistico &
            ",fecha_ret_gestion=" & fecha_ret_gestion &
            ",fecha_ret_central=" & fecha_ret_central &
            ",GABINETE_PRODUCION='" & nombre_gabinete_producion & "'" &
            ",NOMBRE_PERSONA_EXPEDIENTE=" & ref_nombre_persona &
            ",IDENTIFICACION_PERSONA_EXPEDIENTE=" & ref_identificacion_persona &
            ",ASUNTO_EXPEDIENTE=" & ref_asunto &
            ",OBSERVACION_EXPEDIENTE=" & ref_observacion &
            ",TEMA_EXPEDIENTE=" & ref_tema &
            ",CODIGO_AREA_TRD=" & id_area_departamento &
            ",NOMBRE_AREA_TRD=" & ref_nombre_area &
            " where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Actualiza_expediente_produccion = "Imposible registrar expediente  : " & sqlinsertcion
                'myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar unidad de conservacion  : " & sqlinsertcion
                Exit Function
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
            ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) values (" &
            "'EDITA UNIDAD','" & user_gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
            id_expediente & ",'" & ip_transaccion & "','" & hor & "','GESTOR WEB')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            Actualiza_expediente_produccion = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Actualiza_expediente_produccion = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Actualiza_expediente_produccion = errorM

        End Try
    End Function

    Function Asigna_datos_interface_expediente(ByRef update As UpdatePanel,
                                               ByRef ref_hdnEmailID As Object,
                                               ByRef ref_Hidden_id_empresa As Object) As String
        '***************************************************************
        'Función : Asigna datos expediente desde la interface
        'Fecha : 2015-04-20
        'Ing: Miguel Angel Urueta Miranda
        '***************************************************************
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassAdmonEmpresa
            Dim Resfclas As New ClassGaTipoDocumental
            Dim id_empresa As Integer = 0
            Dim refclas_rad As New ClassRadicador
            Dim refclascexpediente As New ClassGaExpediente
            Dim refclasGestionInstrumento As New ClassGaGestionInstrumento
            Dim refclastrdDocumental As New ClassTrdDocumental
            Dim ref_DropDownListorganigrama As DropDownList = Nothing
            Dim ref_TextBoxNUMERO_FOLIOS_CONTENIDOS As TextBox = Nothing
            Dim ref_TextBoxNUMERO_DIGITALIZADO_CONTENIDO As TextBox = Nothing
            Dim ref_TextBoxNUMERO_ELECTRONICO_CONTENIDO As TextBox = Nothing
            Dim ref_TextBoxASUNTO_EXPEDIENTE As TextBox = Nothing
            Dim ref_TextBoxCodigoManual As TextBox = Nothing
            Dim ref_TextBoxRANGO_EXTREMO_INICIAL As TextBox = Nothing
            Dim ref_TextBoxRANGO_EXTREMO_FINAL As TextBox = Nothing
            Dim ref_TextBoxFECHA_EXTREMA_INICIAL As TextBox = Nothing
            Dim ref_TextBoxFECHA_EXTREMA_FINAL As TextBox = Nothing
            Dim ref_TextBoxayuda As TextBox = Nothing
            Dim ref_TextBoxTEMA_EXPEDIENTE As TextBox = Nothing
            Dim ref_TextBoxOBSERVACION_EXPEDIENTE As TextBox = Nothing
            Dim ref_DropDownListArea As DropDownList = Nothing
            Dim ref_DropDownListSerie As DropDownList = Nothing
            Dim ref_DropDownListSubserie As DropDownList = Nothing
            Dim ref_DropDownListBoxtipoexpediente As DropDownList = Nothing
            Dim ref_DropDownList_tipo_unidad_conservacion As DropDownList = Nothing
            Dim ref_DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL As DropDownList = Nothing
            Dim ref_DropDownListNOMBRE_FONDO As DropDownList = Nothing
            Dim ref_DropDownListNOMBRE_CICLO_ARCHIVO As DropDownList = Nothing
            Dim ref_DropDownListsub_seccion As DropDownList = Nothing
            Dim ref_TextBox_id_archivo As TextBox = Nothing
            Dim ref_TextBoxNOMBRE_PERSONA_EXPEDIENTE As TextBox = Nothing
            Dim ref_TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE As TextBox = Nothing
            Dim ref_Hidden_tipo_unidad As Object = Nothing
            Dim ref_Button_activa_archivar_unidad As Button = Nothing
            Dim ref_Button_des_archivar As Button = Nothing
            Dim ref_TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE As TextBox = Nothing
            Dim ref_TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE As TextBox = Nothing
            Dim ref_DropDownList_instrumento As DropDownList = Nothing
            Dim matri_nombre_controles() As String = {"DropDownListorganigrama", "TextBoxNUMERO_FOLIOS_CONTENIDOS",
               "TextBoxNUMERO_DIGITALIZADO_CONTENIDO", "TextBoxNUMERO_ELECTRONICO_CONTENIDO", "TextBoxASUNTO_EXPEDIENTE",
               "TextBoxCodigoManual", "TextBoxRANGO_EXTREMO_INICIAL", "TextBoxRANGO_EXTREMO_FINAL", "TextBoxFECHA_EXTREMA_INICIAL",
               "TextBoxFECHA_EXTREMA_FINAL", "TextBoxayuda", "TextBoxTEMA_EXPEDIENTE", "DropDownListArea", "DropDownListSerie",
               "DropDownListSubserie", "DropDownListBoxtipoexpediente", "DropDownList_tipo_unidad_conservacion", "TextBoxOBSERVACION_EXPEDIENTE",
               "DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL", "DropDownListsub_seccion", "TextBox_id_archivo", "Hidden_tipo_unidad",
                "Button_activa_archivar_unidad", "Button_des_archivar", "DropDownListNOMBRE_FONDO", "DropDownListNOMBRE_CICLO_ARCHIVO",
                                                      "TextBoxNOMBRE_PERSONA_EXPEDIENTE", "TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE",
                                                      "TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE", "TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE", "DropDownList_instrumento"}
            Dim total_controles As Integer = matri_nombre_controles.Length
            For i As Integer = 0 To total_controles - 1
                Dim control_ob As Object = Nothing
                control_ob = update.FindControl(matri_nombre_controles(i))
                If control_ob Is Nothing Then
                    Asigna_datos_interface_expediente = "Imposible encontrar el control " & matri_nombre_controles(i)
                    Exit Function
                End If
                Select Case matri_nombre_controles(i)
                    Case "DropDownListorganigrama"
                        ref_DropDownListorganigrama = control_ob
                    Case "TextBoxNUMERO_FOLIOS_CONTENIDOS"
                        ref_TextBoxNUMERO_FOLIOS_CONTENIDOS = control_ob
                    Case "TextBoxNUMERO_DIGITALIZADO_CONTENIDO"
                        ref_TextBoxNUMERO_DIGITALIZADO_CONTENIDO = control_ob
                    Case "TextBoxNUMERO_ELECTRONICO_CONTENIDO"
                        ref_TextBoxNUMERO_ELECTRONICO_CONTENIDO = control_ob
                    Case "TextBoxASUNTO_EXPEDIENTE"
                        ref_TextBoxASUNTO_EXPEDIENTE = control_ob
                    Case "TextBoxOBSERVACION_EXPEDIENTE"
                        ref_TextBoxOBSERVACION_EXPEDIENTE = control_ob
                    Case "TextBoxCodigoManual"
                        ref_TextBoxCodigoManual = control_ob
                    Case "TextBoxRANGO_EXTREMO_INICIAL"
                        ref_TextBoxRANGO_EXTREMO_INICIAL = control_ob
                    Case "TextBoxRANGO_EXTREMO_FINAL"
                        ref_TextBoxRANGO_EXTREMO_FINAL = control_ob
                    Case "TextBoxFECHA_EXTREMA_INICIAL"
                        ref_TextBoxFECHA_EXTREMA_INICIAL = control_ob
                    Case "TextBoxFECHA_EXTREMA_FINAL"
                        ref_TextBoxFECHA_EXTREMA_FINAL = control_ob
                    Case "TextBoxayuda"
                        ref_TextBoxayuda = control_ob
                    Case "TextBoxTEMA_EXPEDIENTE"
                        ref_TextBoxTEMA_EXPEDIENTE = control_ob
                    Case "DropDownListArea"
                        ref_DropDownListArea = control_ob
                    Case "DropDownListSerie"
                        ref_DropDownListSerie = control_ob
                    Case "DropDownListSubserie"
                        ref_DropDownListSubserie = control_ob
                    Case "DropDownListBoxtipoexpediente"
                        ref_DropDownListBoxtipoexpediente = control_ob
                    Case "DropDownList_tipo_unidad_conservacion"
                        ref_DropDownList_tipo_unidad_conservacion = control_ob
                    Case "DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL"
                        ref_DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL = control_ob
                    Case "DropDownListsub_seccion"
                        ref_DropDownListsub_seccion = control_ob
                    Case "DropDownListNOMBRE_FONDO"
                        ref_DropDownListNOMBRE_FONDO = control_ob
                    Case "DropDownListNOMBRE_CICLO_ARCHIVO"
                        ref_DropDownListNOMBRE_CICLO_ARCHIVO = control_ob
                    Case "TextBoxNOMBRE_PERSONA_EXPEDIENTE"
                        ref_TextBoxNOMBRE_PERSONA_EXPEDIENTE = control_ob
                    Case "TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE"
                        ref_TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE = control_ob
                    Case "TextBox_id_archivo"
                        ref_TextBox_id_archivo = control_ob
                    Case "Hidden_tipo_unidad"
                        ref_Hidden_tipo_unidad = control_ob
                    Case "Button_activa_archivar_unidad"
                        ref_Button_activa_archivar_unidad = control_ob
                    Case "Button_des_archivar"
                        ref_Button_des_archivar = control_ob
                    Case "TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE"
                        ref_TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE = control_ob
                    Case "TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE"
                        ref_TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE = control_ob
                    Case "DropDownList_instrumento"
                        ref_DropDownList_instrumento = control_ob
                End Select
            Next
            Dim split() As String = (HttpContext.Current.Session.Item("SESIONITERCAMBIOEXPEDIENTE").ToString.Split("|"))
            If split.Length > 1 Then
                Result = Refclas.Retorna_Id_Emprea(split(0),
                                                   id_empresa)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice 10" & Result
                    Exit Function
                End If
                ref_Hidden_id_empresa.value = id_empresa
                ref_hdnEmailID.Value = split(1)
                If split.Length > 2 Then
                    If split(2) = "VOLUMEN" Then
                        ref_Button_activa_archivar_unidad.Enabled = True
                        ref_Button_des_archivar.Enabled = True
                    Else
                        ref_Button_activa_archivar_unidad.Enabled = False
                        ref_Button_des_archivar.Enabled = False
                    End If
                End If
            Else
                id_empresa = ref_Hidden_id_empresa.value
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                ref_DropDownListorganigrama.Enabled = False
            Else
                ref_DropDownListorganigrama.Enabled = True
            End If
            Dim stru_expediente() As expediente_conservacion = Nothing
            Result = refclascexpediente.SolicitaDatosEstructuraExpediente(ref_hdnEmailID.Value,
                                                                                        stru_expediente)
            If Result <> "YES" Then
                Asigna_datos_interface_expediente = Result
                Exit Function
            End If
            '---------------------------------------------
            'Lista los datos del organigrama relacionado
            'relacionado al expediente
            '---------------------------------------------
            Dim id_organigrama As Integer = 0
            Dim nombre_organigrama As String = ""
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Result = Class_areas_depart_radicacion.Lista_datos_organigrama_por_codigo_area(stru_expediente(0).CODIGO_AREA_TRD,
                                                                                           id_organigrama,
                                                                                           nombre_organigrama)
            If Result <> "YES" Then
                Asigna_datos_interface_expediente = Result
                Exit Function
            End If
            Dim Refclasunidad As New ClassUnidadConservacion
            Dim Refclas_organigrama As New Class_registro_organigrama
            Result = Refclas_organigrama.Listar_Organigramas_Empresa_Combo_Default(id_empresa,
                                                                                   nombre_organigrama,
                                                                                   ref_DropDownListorganigrama,
                                                                                   update)
            If Result <> "YES" Then
                Asigna_datos_interface_expediente = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Lista o asigna el tipo de unidad documental
            'disponible
            '-----------------------------------------------
            If stru_expediente(0).ID_TIPO_UNIDAD_DOCUMENTAL <> 0 Then
                Result = Refclasunidad.Lista_asigna_tipos_unidad_documental(ref_DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL,
                                                                            stru_expediente(0).ID_TIPO_UNIDAD_DOCUMENTAL)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = Result
                    Exit Function
                End If
            Else
                Result = Refclasunidad.lista_tipos_unidades_documentales(ref_DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Lista_asigna_tipos_unidad_documental dice 14 " & Result
                    Exit Function
                End If
            End If
            '------------------------------------------
            'Asigna datos archivamento
            '------------------------------------------
            Dim refclas_unidad_conservacion_ As New ClassUnidadConservacion
            If stru_expediente(0).ESTADO_ARCHIVO_INIDAD <> 0 Then
                If stru_expediente(0).ENTRE_PAÑO_ID_ENTREPAÑO <> 0 Then
                    Dim codigo_unidad_contenedora As String = ""
                    Result = refclas_unidad_conservacion_.Ratorna_codigo_corto_entrepaño_id_entrepaño(stru_expediente(0).ENTRE_PAÑO_ID_ENTREPAÑO,
                                                                                                      codigo_unidad_contenedora)
                    If Result <> "YES" Then
                        Asigna_datos_interface_expediente = "Función Lista_asigna_tipos_unidad_documental dice 15" & Result
                        Exit Function
                    End If
                    ref_Hidden_tipo_unidad.value = "Entrepaño"
                    ref_TextBox_id_archivo.Text = stru_expediente(0).ENTRE_PAÑO_ID_ENTREPAÑO & "|" & "ENTREPAÑO" & "|" & codigo_unidad_contenedora
                End If
                If stru_expediente(0).ID_UNIDAD_CONSERVACION <> 0 Then
                    Dim nombre_tipo_unidad As String = ""
                    Dim codigo_unico As String = ""
                    Result = refclas_unidad_conservacion_.Retorna_datos_unidad_contenedora_por_id(stru_expediente(0).ID_UNIDAD_CONSERVACION,
                                                                                                  nombre_tipo_unidad,
                                                                                                  codigo_unico)
                    If Result <> "YES" Then
                        Asigna_datos_interface_expediente = "Función Lista_asigna_tipos_unidad_documental dice 16 " & Result
                        Exit Function
                    End If
                    ref_Hidden_tipo_unidad.value = "UNIDAD CONTENEDORA EXPEDIENTE"
                    ref_TextBox_id_archivo.Text = stru_expediente(0).ID_UNIDAD_CONSERVACION & "|" & nombre_tipo_unidad & "|" & codigo_unico
                End If

            End If
            ref_TextBoxNUMERO_FOLIOS_CONTENIDOS.Text = stru_expediente(0).NUMERO_FOLIO_UNIDAD_CONSERVACION
            ref_TextBoxNUMERO_DIGITALIZADO_CONTENIDO.Text = stru_expediente(0).NUMERO_DIGITALIZADO_CONTENIDO
            ref_TextBoxNUMERO_ELECTRONICO_CONTENIDO.Text = stru_expediente(0).NUMERO_ELECTRONICO_CONTENIDO
            ref_TextBoxASUNTO_EXPEDIENTE.Text = stru_expediente(0).ASUNTO_EXPEDIENTE
            ref_TextBoxCodigoManual.Text = stru_expediente(0).CODIGO_UNICO
            ref_TextBoxOBSERVACION_EXPEDIENTE.Text = stru_expediente(0).OBSERVACION_EXPEDIENTE
            If stru_expediente(0).FECHA_EXTREMA_INICIAL = "" Then
                ref_TextBoxFECHA_EXTREMA_INICIAL.Text = ""
            Else
                Dim splifecha() As String = Left(stru_expediente(0).FECHA_EXTREMA_INICIAL, 10).Split("/")
                ref_TextBoxFECHA_EXTREMA_INICIAL.Text = splifecha(2) & "-" & splifecha(1) & "-" & splifecha(0)
            End If
            If stru_expediente(0).FECHA_EXTREMA_FINAL = "" Then
                ref_TextBoxFECHA_EXTREMA_FINAL.Text = ""
            Else
                Dim splifecha() As String = Left(stru_expediente(0).FECHA_EXTREMA_FINAL, 10).Split("/")
                ref_TextBoxFECHA_EXTREMA_FINAL.Text = splifecha(2) & "-" & splifecha(1) & "-" & splifecha(0)
            End If
            ref_TextBoxRANGO_EXTREMO_INICIAL.Text = stru_expediente(0).RANGO_EXTREMO_INICIAL
            ref_TextBoxRANGO_EXTREMO_FINAL.Text = stru_expediente(0).RANGO_EXTREMO_FINAL
            ref_TextBoxTEMA_EXPEDIENTE.Text = stru_expediente(0).TEMA_EXPEDIENTE
            ref_Hidden_id_empresa.Value = id_empresa
            Dim Refclas_dos As New ClassGestionDocumental
            ref_DropDownListArea.Items.Clear()
            ref_DropDownListSerie.Items.Clear()
            ref_DropDownListSubserie.Items.Clear()
            ref_DropDownList_instrumento.Items.Clear()
            '----------------------------------------------------
            'Lista o asigna areas relacionadas
            '----------------------------------------------------
            Dim nombre_area As String = ""
            If stru_expediente(0).CODIGO_AREA_TRD <> 0 Then
                Result = Refclas_dos.Retorna_nombre_area_por_id(stru_expediente(0).CODIGO_AREA_TRD,
                                                                nombre_area)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = Result
                    Exit Function
                End If
            End If

            If nombre_area <> "" Then
                If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                    'Lista todas las areas disponibles en el organigrama- solo para usuarios manager produccion
                    Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default(id_organigrama,
                                                                                                     nombre_area,
                                                                                                     ref_DropDownListArea)
                    If Result <> "YES" Then
                        Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                        Exit Function
                    End If
                Else
                    Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default_por_id_area(id_organigrama,
                                                                                                                 nombre_area,
                                                                                                                 stru_expediente(0).CODIGO_AREA_TRD,
                                                                                                                 ref_DropDownListArea)
                    If Result <> "YES" Then
                        Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                        Exit Function
                    End If
                    'Result = Refclas_dos.lista_areas_permitidas_usuario_gestion_organigrama_default(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                    '                                                                                id_organigrama, _
                    '                                                                                nombre_area, _
                    '                                                                                ref_DropDownListArea)
                    'If Result <> "YES" Then
                    '    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    '    Exit Function
                    'End If
                End If
            Else


                If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                    'Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series(id_organigrama, _
                    '                                                                         ref_DropDownListArea)
                    'If Result <> "YES" Then
                    '    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    '    Exit Function
                    'End If
                    'Lista todas las areas disponibles en el organigrama- solo para usuarios manager produccion
                    Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default(id_organigrama,
                                                                                                     nombre_area,
                                                                                                     ref_DropDownListArea)
                    If Result <> "YES" Then
                        Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                        Exit Function
                    End If
                Else
                    Dim id_area_usuario_gestion As Integer = 0
                    Dim Class_remit_dest_interno As New Class_remit_dest_interno
                    Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                   id_area_usuario_gestion)
                    If Result <> "YES" Then
                        Asigna_datos_interface_expediente = Result
                        Exit Function
                    End If
                    Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default_por_id_area(id_organigrama,
                                                                                                                nombre_area,
                                                                                                                id_area_usuario_gestion,
                                                                                                                ref_DropDownListArea)
                    If Result <> "YES" Then
                        Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                        Exit Function
                    End If
                    'Result = Refclas_dos.lista_areas_permitidas_usuario_gestion_organigrama(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
                    '                                                                        id_organigrama, _
                    '                                                                        ref_DropDownListArea)
                    'If Result <> "YES" Then
                    '    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    '    Exit Function
                    'End If
                End If
            End If
            '---------------------------------------------
            'Asigna y lista instrumentos archivisticos
            '---------------------------------------------
            If stru_expediente(0).id_instrumento <> 0 Then
                Result = refclasGestionInstrumento.Lista_instrumentos_archivisticos(id_organigrama,
                                                                                    stru_expediente(0).id_instrumento,
                                                                                    ref_DropDownList_instrumento,
                                                                                    update)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    Exit Function
                End If
            Else
                Result = refclasGestionInstrumento.Lista_instrumentos_archivisticos_activos_default(id_organigrama,
                                                                                                    stru_expediente(0).id_instrumento,
                                                                                                    ref_DropDownList_instrumento,
                                                                                                    update)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    Exit Function
                End If
            End If
            '----------------------------------------------------
            'Lista y asigna series documentales
            '----------------------------------------------------
            Dim Ref_class_series As New Class_series_documentales
            If stru_expediente(0).id_instrumento <> 0 Then

                Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area_default(stru_expediente(0).CODIGO_AREA_TRD,
                                                                                                stru_expediente(0).id_instrumento,
                                                                                                stru_expediente(0).CODIGO_SERIE,
                                                                                                ref_DropDownListSerie,
                                                                                                update)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    Exit Function
                End If
            Else
                'Result = refclastrdDocumental.Lista_series_relacionadas_id_area(stru_expediente(0).CODIGO_AREA_TRD, _
                '                                                                stru_expediente(0).CODIGO_SERIE, _
                '                                                                ref_DropDownListSerie, update)

                'If Result <> "YES" Then
                '    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                '    Exit Function
                'End If

            End If
            '----------------------------------------------------
            'Lista y asigna sub series documentales
            '----------------------------------------------------
            Dim nombre_sub_serie As String = ""
            If stru_expediente(0).CODIGO_SUBSERIE <> 0 Then
                Result = Refclas_dos.Retorna_nombre_sub_Serie_por_id(stru_expediente(0).CODIGO_SUBSERIE,
                                                                     nombre_sub_serie)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    Exit Function
                End If
                Result = Refclas_dos.Listar_SubSeries_Documentales_default(stru_expediente(0).CODIGO_SERIE, nombre_sub_serie, ref_DropDownListSubserie)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice" & Result
                    Exit Function
                End If
            Else
                Result = Refclas_dos.Listar_SubSeries_Documentales_default(stru_expediente(0).CODIGO_SERIE,
                                                                           nombre_sub_serie,
                                                                           ref_DropDownListSubserie)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    Exit Function
                End If
            End If
            '---------------------------------------------
            'Asigna datos ciclo archivo 
            '---------------------------------------------
            Dim id_tipo_instrumento As Integer = 0
            Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
            If stru_expediente(0).id_instrumento <> 0 Then
                Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(stru_expediente(0).id_instrumento,
                                                                                   id_tipo_instrumento)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    Exit Function
                End If
                Result = Me.Lista_ciclo_archivo_instrumento_default(id_tipo_instrumento, id_tipo_instrumento, ref_DropDownListNOMBRE_CICLO_ARCHIVO, update)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    Exit Function
                End If
            Else
                Result = Me.Listar_ciclos_archivo(ref_DropDownListNOMBRE_CICLO_ARCHIVO, stru_expediente(0).NOMBRE_CICLO_ARCHIVO, "")
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                    Exit Function
                End If
            End If
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            Result = ref_Class_ra_tipo_expediente.lista_tipos_expedientes_Combo(ref_DropDownListBoxtipoexpediente,
                                                                                update,
                                                                                0)
            If Result <> "YES" Then
                Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice" & Result
                Exit Function
            End If
            Dim nombre_tipo_expediente As String = ""
            Result = ref_Class_ra_tipo_expediente.Retorna_nombre_tipo_expediente_por_id_expediente(ref_hdnEmailID.Value,
                                                                                                   nombre_tipo_expediente)
            If Result <> "YES" Then
                Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                Exit Function
            End If
            For i As Integer = 0 To ref_DropDownListBoxtipoexpediente.Items.Count - 1
                If ref_DropDownListBoxtipoexpediente.Items(i).Text = nombre_tipo_expediente Then
                    ref_DropDownListBoxtipoexpediente.Text = nombre_tipo_expediente
                    Exit For
                End If
            Next
            If ref_DropDownListBoxtipoexpediente.Text <> "" Then
                Result = ref_Class_ra_tipo_expediente.Retorna_ayuda_clase_expediente(ref_DropDownListBoxtipoexpediente.Text,
                                                                                     ref_TextBoxayuda.Text)
                If Result <> "YES" Then
                    Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice" & Result
                    Exit Function
                End If
                ref_TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = True
                ref_TextBoxNUMERO_DIGITALIZADO_CONTENIDO.ReadOnly = True
                ref_TextBoxNUMERO_ELECTRONICO_CONTENIDO.ReadOnly = True
                ref_TextBoxNUMERO_FOLIOS_CONTENIDOS.BackColor = Drawing.Color.Gray
                ref_TextBoxNUMERO_DIGITALIZADO_CONTENIDO.BackColor = Drawing.Color.Gray
                ref_TextBoxNUMERO_ELECTRONICO_CONTENIDO.BackColor = Drawing.Color.Gray
                If ref_DropDownListBoxtipoexpediente.Text = "FISICO" Then
                    ref_TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                End If
                'EXPEDIENTE HIBRIDO
                If ref_DropDownListBoxtipoexpediente.Text = "HIBRIDO" Then
                    ref_TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                End If
                'EXPEDIENTE MIXTO
                If ref_DropDownListBoxtipoexpediente.Text = "MIXTO" Then
                    ref_TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                    ref_TextBoxNUMERO_ELECTRONICO_CONTENIDO.ReadOnly = False
                End If
            End If

            '-------------------------------------------
            'Asigna tipo unidad conservacion expediente
            '-------------------------------------------
            Dim Refclas_unidad_conservacion As New ClassUnidadConservacion
            Result = Refclas_unidad_conservacion.Lista_asigna_tipos_unidad_expedientes(ref_DropDownList_tipo_unidad_conservacion,
                                                                                       stru_expediente(0).TIPO_UNIDAD_ID_TIPO)
            If Result <> "YES" Then
                Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                Exit Function
            End If

            '---------------------------------------------
            'Asigna datos fondo documental
            '---------------------------------------------
            Result = Me.Listar_fodos_documentales(ref_DropDownListNOMBRE_FONDO,
                                                  stru_expediente(0).NOMBRE_FONDO)
            If Result <> "YES" Then
                Asigna_datos_interface_expediente = "Función Asigna_datos_interface_expediente dice " & Result
                Exit Function
            End If
            ref_TextBoxNOMBRE_PERSONA_EXPEDIENTE.Text = stru_expediente(0).NOMBRE_PERSONA_EXPEDIENTE
            ref_TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE.Text = stru_expediente(0).IDENTIFICACION_PERSONA_EXPEDIENTE
            ref_TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE.Text = stru_expediente(0).NOMBRE_RESPONSABLE_EXPEDIENTE
            ref_TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE.Text = stru_expediente(0).IDENFICACION_RESPONSABLE_EXPEDIENTE
            Asigna_datos_interface_expediente = "YES"
        Catch ex As Exception
            Asigna_datos_interface_expediente = "Inconsistencia general función Asigna_datos_interface_expediente " & ex.Message
        End Try
    End Function
    Function cambia_estado_abierto_serrado_expediente(ByVal id_expediente As Integer,
                                                      ByVal id_estado_expediente As Integer,
                                                      ByVal id_usuario_gestion As Integer,
                                                      ByVal user_Gestion As String,
                                                      ByVal iptrans As String,
                                                      ByVal motivo_cambio_estado As String) As String

        Dim Result As String = ""
        '------------------------------------------------
        'Verifica expediente produción documental 
        'no se elimine desde el gestor de expedientes
        '------------------------------------------------
        Dim estado_expediente As Integer = 0
        Dim estado_publico As Integer = 0
        Result = Retorna_estado_expediente(id_expediente,
                                           estado_expediente,
                                           estado_publico)
        If Result <> "YES" Then
            cambia_estado_abierto_serrado_expediente = Result
            Exit Function
        End If
        'If estado_publico = 2 Then
        '    cambia_estado_abierto_serrado_expediente = "Imposible cambiar el estado del expediente, debido a que pertenece a la producción documental de otro usuario "
        '    Exit Function
        'End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            cambia_estado_abierto_serrado_expediente = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad As String = 0
        Dim detalle_transacion As String = ""
        If id_estado_expediente = 1 Then
            detalle_transacion = "ABRIENDO EXPEDIENTE"
        Else
            detalle_transacion = "CERRANDO EXPEDIENTE"
        End If
        Dim errorM As String = "YES"
        Try
            Dim sqlforupdate As String = "update expediente_archivo set ESTADO_EXPEDIENTE=" & id_estado_expediente &
            " where ID_EXPEDIENTE=" & id_expediente
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            Dim hor As String = Now
            If Switc = 0 Then
                cambia_estado_abierto_serrado_expediente = "Imposible Cerrar expediente   "
                'myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible cambiar estado expediente   "
                Exit Function
            End If
            Dim sqlforupdate_ As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'" & detalle_transacion & "','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           id_expediente & ",'" & iptrans & "','" & hor & "','GESTOR DOCUMENTAL','" & motivo_cambio_estado & "')"
            myCommand.CommandText = sqlforupdate_
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualizar los expediente  : " & sqlforupdate_
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            myTrans.Commit()
            myConnection.Close()
            cambia_estado_abierto_serrado_expediente = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                cambia_estado_abierto_serrado_expediente = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            cambia_estado_abierto_serrado_expediente = errorM

        End Try
    End Function
    Function Listar_datos_Expediente_estructura_unidad_conservacion(ByVal id_unidad_conservacion As Integer,
                                                                    ByRef estru_unidad_conservacion() As expediente_conservacion,
                                                                    ByVal campo_order As String) As String
        '************************************************************
        'Funcion Listar estrucutura expediente con el
        'parametro id unidad conservacion a la que pertenece
        'Fecha 2015-01-29
        'Ing : Miguel Angel Urueta Miranda
        '************************************************************
        Try
            Erase estru_unidad_conservacion
            Dim campos_seleccion As String = "UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION,ID_EXPEDIENTE,CONSECUTIVO_DOCUMENTO," &
            "CONSECUTIVO_EXPEDIENTE_2,CODIGO_LARGO,CODIGO_UNICO,NUMERO_FOLIOS_CONTENIDOS," &
            "ID_USUARIO_GESTION,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD,CODIGO_SERIE_TRD,NOMBRE_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_SUBSERIE_TRD," &
            "ESTADO_EXPEDIENTE,ESTADO_ARCHIVO_EXPEDIENTE,FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," &
            "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_EXPEDIENTE,ENTRE_PAÑO_ID_ENTREPAÑO,ASUNTO_EXPEDIENTE,ID_EMPRESA_EXPEDIENTE,VOLUMEN_EXPEDIENTE,rte.NOMBRE_TIPO_EXPEDIENTE "
            Dim SqlConsulta As String = "select " & campos_seleccion & " from  expediente_archivo " &
            " inner join ra_tipo_expediente as rte on (rte.ID_TIPO_EXPEDIENTE=RA_TIP_EXPE_ID_TIPO_EXPEDIENTE)" &
                                              " where UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=" & id_unidad_conservacion & " order by " & campo_order & ",ID_EXPEDIENTE"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Listar_datos_Expediente_estructura_unidad_conservacion = " Error solicitando estrucutura expediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_unidad_conservacion(i)

                    If Datset.Tables(0).Rows(i).IsNull(0) = True Then
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION")
                    End If
                    estru_unidad_conservacion(i).ID_EXPEDIENTE = Datset.Tables(0).Rows(i).Item(1)
                    estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(i).Item("CONSECUTIVO_DOCUMENTO")
                    'estru_unidad_conservacion(i).CONSECUTIVO_EXPEDIENTE = Dat_reader.Item("CONSECUTIVO_EXPEDIENTE")
                    'estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Dat_reader.Item("CONSECUTIVO_DOCUMENTO")
                    estru_unidad_conservacion(i).CODIGO_LARGO = Datset.Tables(0).Rows(i).Item("CODIGO_LARGO")
                    estru_unidad_conservacion(i).CODIGO_UNICO = Datset.Tables(0).Rows(i).Item("CODIGO_UNICO")
                    estru_unidad_conservacion(i).TIPO_UNIDAD_CONSERVACION = 1
                    estru_unidad_conservacion(i).NUMERO_FOLIO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("NUMERO_FOLIOS_CONTENIDOS")
                    estru_unidad_conservacion(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(i).Item("ID_USUARIO_GESTION")

                    If Datset.Tables(0).Rows(i).IsNull(8) = True Then
                        estru_unidad_conservacion(i).FECHA_CREACION = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_CREACION = Datset.Tables(0).Rows(i).Item("FECHA_CREACION")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(9) = True Then
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = Datset.Tables(0).Rows(i).Item("CODIGO_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(10) = True Then
                        estru_unidad_conservacion(i).NOMBRE_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_AREA = Datset.Tables(0).Rows(i).Item("NOMBRE_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(11) = True Then
                        estru_unidad_conservacion(i).CODIGO_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(12) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(13) = True Then
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = Datset.Tables(0).Rows(i).Item("CODIGO_SUB_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(14) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = Datset.Tables(0).Rows(i).Item("NOMBRE_SUBSERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(15) = True Then
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ESTADO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(16) = True Then
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = Datset.Tables(0).Rows(i).Item("ESTADO_ARCHIVO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(17) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(18) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = Datset.Tables(0).Rows(i).Item("FECHA_EXTREMA_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(19) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(20) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(i).Item("RANGO_EXTREMO_FINAL")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(21) = True Then
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("TEMA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(21) = True Then
                        estru_unidad_conservacion(i).TEMA_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("TEMA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(22) = True Then
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = 0
                    Else
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = Datset.Tables(0).Rows(i).Item(22)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(23) = True Then
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = ""
                    Else
                        estru_unidad_conservacion(i).DESCRIPCION_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(i).Item("ASUNTO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(23) = True Then
                        estru_unidad_conservacion(i).ASUNTO_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).ASUNTO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("ASUNTO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(24) = True Then
                        estru_unidad_conservacion(i).ID_EMPRESA_GESTION = 0
                    Else
                        estru_unidad_conservacion(i).ID_EMPRESA_GESTION = Datset.Tables(0).Rows(i).Item("ID_EMPRESA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(25) = True Then
                        estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE = 0
                    Else
                        estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE = Datset.Tables(0).Rows(i).Item("VOLUMEN_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(26) = True Then
                        estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_TIPO_EXPEDIENTE = Datset.Tables(0).Rows(i).Item("NOMBRE_TIPO_EXPEDIENTE")
                    End If

                Next
                Listar_datos_Expediente_estructura_unidad_conservacion = "YES"
                Exit Function
            Else
                Listar_datos_Expediente_estructura_unidad_conservacion = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_datos_Expediente_estructura_unidad_conservacion = "Inconsistencia general función Listar_datos_Expediente_estructura_unidad_conservacion " & ex.Message
        End Try
    End Function
    Function SolicitaDatosEstructuraExpediente(ByVal IdExpediente As Integer,
                                               ByRef estru_unidad_conservacion() As expediente_conservacion) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura del expediente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdExpediente        : Representa la identificación del expediente
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'estru_unidad_conservacion  : Retorna estructura expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2014-09-25
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Erase estru_unidad_conservacion
            Dim campos_seleccion As String = "UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION,ID_EXPEDIENTE,CONSECUTIVO_DOCUMENTO," &
            "CONSECUTIVO_EXPEDIENTE_2,CODIGO_LARGO,CODIGO_UNICO,NUMERO_FOLIOS_CONTENIDOS," &
            "ID_USUARIO_GESTION,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD,CODIGO_SERIE_TRD,NOMBRE_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_SUBSERIE_TRD," &
            "ESTADO_EXPEDIENTE,ESTADO_ARCHIVO_EXPEDIENTE,FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," &
            "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_EXPEDIENTE,ENTRE_PAÑO_ID_ENTREPAÑO,ASUNTO_EXPEDIENTE,ID_EMPRESA_EXPEDIENTE,VOLUMEN_EXPEDIENTE," _
            & "NUMERO_ELECTRONICO_CONTENIDO,NUMERO_DIGITALIZADO_CONTENIDO,TIPO_UNIDAD_ID_TIPO,OBSERVACION_EXPEDIENTE,ID_TIPO_UNIDAD_DOCUMENTAL," _
            & "NOMBRE_TIPO_UNIDAD_DOCUMENTAL,ID_SUB_AREA,NOMBRE_SUB_AREA," &
             "ID_FONDO,NOMBRE_FONDO,Id_tipos_ciclo_archivo,NOMBRE_CICLO_ARCHIVO,NOMBRE_PERSONA_EXPEDIENTE,IDENTIFICACION_PERSONA_EXPEDIENTE," _
             & "Estado_Publico_Sub_Expediente,NOMBRE_RESPONSABLE_EXPEDIENTE,IDENFICACION_RESPONSABLE_EXPEDIENTE,ALEAS_EXPEDIENTE,EXPEDIENTE_PADRE, " &
             "fecha_ret_central,fecha_ret_gestion,id_instrumento,GABINETE_PRODUCION,Id_registro_procedimiento,Id_registro_proceso,ID_DISCO,ESTADO_FIRMA," &
             "FECHA_FIRMA,estado_expediente_electronico,ra_auto_registro_expediente_id_auto_registro "
            Dim sql_consulta As String = "select " & campos_seleccion & " from  expediente_archivo " &
                                              " where ID_EXPEDIENTE=" & IdExpediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosEstructuraExpediente = "Error en la función SolicitaDatosEstructuraExpediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve estru_unidad_conservacion(i)
                    If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = 0
                    Else
                        estru_unidad_conservacion(i).ID_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item("UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION")
                    End If
                    estru_unidad_conservacion(i).ID_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(1)
                    estru_unidad_conservacion(i).CONSECUTIVO_DOCUMENTO = Datset.Tables(0).Rows(0).Item("CONSECUTIVO_DOCUMENTO")
                    estru_unidad_conservacion(i).CODIGO_LARGO = Datset.Tables(0).Rows(0).Item("CODIGO_LARGO")
                    estru_unidad_conservacion(i).CODIGO_UNICO = Datset.Tables(0).Rows(0).Item("CODIGO_UNICO")
                    estru_unidad_conservacion(i).TIPO_UNIDAD_CONSERVACION = 1
                    estru_unidad_conservacion(i).NUMERO_FOLIO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item("NUMERO_FOLIOS_CONTENIDOS")
                    estru_unidad_conservacion(i).ID_USUARIO_GESTION = Datset.Tables(0).Rows(0).Item("ID_USUARIO_GESTION")
                    estru_unidad_conservacion(i).CONSECUTIVO_EXPEDIENTE_2 = Datset.Tables(0).Rows(0).Item("CONSECUTIVO_EXPEDIENTE_2")
                    If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                        estru_unidad_conservacion(i).FECHA_CREACION = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_CREACION = Datset.Tables(0).Rows(0).Item("FECHA_CREACION")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = ""
                    Else
                        estru_unidad_conservacion(i).CODIGO_AREA_TRD = Datset.Tables(0).Rows(0).Item("CODIGO_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(10) = True Then
                        estru_unidad_conservacion(i).NOMBRE_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_AREA = Datset.Tables(0).Rows(0).Item("NOMBRE_AREA_TRD")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(11) = True Then
                        estru_unidad_conservacion(i).CODIGO_SERIE = 0
                    Else
                        estru_unidad_conservacion(i).CODIGO_SERIE = Datset.Tables(0).Rows(0).Item("CODIGO_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(12) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SERIE = Datset.Tables(0).Rows(0).Item("NOMBRE_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(13) = True Then
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = 0
                    Else
                        estru_unidad_conservacion(i).CODIGO_SUBSERIE = Datset.Tables(0).Rows(0).Item("CODIGO_SUB_SERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(14) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUBSERIE = Datset.Tables(0).Rows(0).Item("NOMBRE_SUBSERIE_TRD")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(15) = True Then
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_UNIDAD_CONSERVACION = Datset.Tables(0).Rows(0).Item("ESTADO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(15) = True Then
                        estru_unidad_conservacion(i).ESTADO_EXPEDIENTE = 1
                    Else
                        estru_unidad_conservacion(i).ESTADO_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("ESTADO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(16) = True Then
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_ARCHIVO_INIDAD = Datset.Tables(0).Rows(0).Item("ESTADO_ARCHIVO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(17) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_INICIAL = Datset.Tables(0).Rows(0).Item("FECHA_EXTREMA_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(18) = True Then
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_EXTREMA_FINAL = Datset.Tables(0).Rows(0).Item("FECHA_EXTREMA_FINAL")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(19) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(0).Item("RANGO_EXTREMO_INICIAL")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(20) = True Then
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = ""
                    Else
                        estru_unidad_conservacion(i).RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(0).Item("RANGO_EXTREMO_FINAL")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(21) = True Then
                        estru_unidad_conservacion(i).TEMA_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).TEMA_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("TEMA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(22) = True Then
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = 0
                    Else
                        estru_unidad_conservacion(i).ENTRE_PAÑO_ID_ENTREPAÑO = Datset.Tables(0).Rows(0).Item(22)
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(23) = True Then
                        estru_unidad_conservacion(i).ASUNTO_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).ASUNTO_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("ASUNTO_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(24) = True Then
                        estru_unidad_conservacion(i).ID_EMPRESA_GESTION = 0
                    Else
                        estru_unidad_conservacion(i).ID_EMPRESA_GESTION = Datset.Tables(0).Rows(0).Item("ID_EMPRESA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(25) = True Then
                        estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE = 0
                    Else
                        estru_unidad_conservacion(i).VOLUMEN_EXPEIDENTE = Datset.Tables(0).Rows(0).Item("VOLUMEN_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(26) = True Then
                        estru_unidad_conservacion(i).NUMERO_ELECTRONICO_CONTENIDO = 0
                    Else
                        estru_unidad_conservacion(i).NUMERO_ELECTRONICO_CONTENIDO = Datset.Tables(0).Rows(0).Item("NUMERO_ELECTRONICO_CONTENIDO")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(27) = True Then
                        estru_unidad_conservacion(i).NUMERO_DIGITALIZADO_CONTENIDO = 0
                    Else
                        estru_unidad_conservacion(i).NUMERO_DIGITALIZADO_CONTENIDO = Datset.Tables(0).Rows(0).Item("NUMERO_DIGITALIZADO_CONTENIDO")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(28) = True Then
                        estru_unidad_conservacion(i).TIPO_UNIDAD_ID_TIPO = 0
                    Else
                        estru_unidad_conservacion(i).TIPO_UNIDAD_ID_TIPO = Datset.Tables(0).Rows(0).Item("TIPO_UNIDAD_ID_TIPO")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(29) = True Then
                        estru_unidad_conservacion(i).OBSERVACION_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).OBSERVACION_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("OBSERVACION_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(30) = True Then
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_DOCUMENTAL = 0
                    Else
                        estru_unidad_conservacion(i).ID_TIPO_UNIDAD_DOCUMENTAL = Datset.Tables(0).Rows(0).Item("ID_TIPO_UNIDAD_DOCUMENTAL")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(31) = True Then
                        estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD_DOCUMENTAL = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_TIPO_UNIDAD_DOCUMENTAL = Datset.Tables(0).Rows(0).Item("NOMBRE_TIPO_UNIDAD_DOCUMENTAL")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(32) = True Then
                        estru_unidad_conservacion(i).ID_SUB_AREA = 0
                    Else
                        estru_unidad_conservacion(i).ID_SUB_AREA = Datset.Tables(0).Rows(0).Item("ID_SUB_AREA")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(33) = True Then
                        estru_unidad_conservacion(i).NOMBRE_SUB_AREA = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_SUB_AREA = Datset.Tables(0).Rows(0).Item("NOMBRE_SUB_AREA")
                    End If

                    If Datset.Tables(0).Rows(0).IsNull(34) = True Then
                        estru_unidad_conservacion(i).ID_FONDO = 0
                    Else
                        estru_unidad_conservacion(i).ID_FONDO = Datset.Tables(0).Rows(0).Item("ID_FONDO")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(35) = True Then
                        estru_unidad_conservacion(i).NOMBRE_FONDO = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_FONDO = Datset.Tables(0).Rows(0).Item("NOMBRE_FONDO")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(36) = True Then
                        estru_unidad_conservacion(i).Id_tipos_ciclo_archivo = 0
                    Else
                        estru_unidad_conservacion(i).Id_tipos_ciclo_archivo = Datset.Tables(0).Rows(0).Item("Id_tipos_ciclo_archivo")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(37) = True Then
                        estru_unidad_conservacion(i).NOMBRE_CICLO_ARCHIVO = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_CICLO_ARCHIVO = Datset.Tables(0).Rows(0).Item("NOMBRE_CICLO_ARCHIVO")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(38) = True Then
                        estru_unidad_conservacion(i).NOMBRE_PERSONA_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_PERSONA_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("NOMBRE_PERSONA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(39) = True Then
                        estru_unidad_conservacion(i).IDENTIFICACION_PERSONA_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).IDENTIFICACION_PERSONA_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("IDENTIFICACION_PERSONA_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(40) = True Then
                        estru_unidad_conservacion(i).Estado_Publico_Sub_Expediente = 0
                    Else
                        estru_unidad_conservacion(i).Estado_Publico_Sub_Expediente = Datset.Tables(0).Rows(0).Item("Estado_Publico_Sub_Expediente")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(41) = True Then
                        estru_unidad_conservacion(i).NOMBRE_RESPONSABLE_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).NOMBRE_RESPONSABLE_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("NOMBRE_RESPONSABLE_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(42) = True Then
                        estru_unidad_conservacion(i).IDENFICACION_RESPONSABLE_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).IDENFICACION_RESPONSABLE_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("IDENFICACION_RESPONSABLE_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(43) = True Then
                        estru_unidad_conservacion(i).ALEAS_EXPEDIENTE = ""
                    Else
                        estru_unidad_conservacion(i).ALEAS_EXPEDIENTE = Datset.Tables(0).Rows(0).Item("ALEAS_EXPEDIENTE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(44) = True Then
                        estru_unidad_conservacion(i).EXPEDIENTE_PADRE = 0
                    Else
                        estru_unidad_conservacion(i).EXPEDIENTE_PADRE = Datset.Tables(0).Rows(0).Item("EXPEDIENTE_PADRE")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(45) = True Then
                        estru_unidad_conservacion(i).fecha_ret_central = ""
                    Else
                        estru_unidad_conservacion(i).fecha_ret_central = Datset.Tables(0).Rows(0).Item("fecha_ret_central")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(46) = True Then
                        estru_unidad_conservacion(i).fecha_ret_gestion = ""
                    Else
                        estru_unidad_conservacion(i).fecha_ret_gestion = Datset.Tables(0).Rows(0).Item("fecha_ret_gestion")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(47) = True Then
                        estru_unidad_conservacion(i).id_instrumento = 0
                    Else
                        estru_unidad_conservacion(i).id_instrumento = Datset.Tables(0).Rows(0).Item("id_instrumento")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(48) = True Then
                        estru_unidad_conservacion(i).GABINETE_PRODUCION = ""
                    Else
                        estru_unidad_conservacion(i).GABINETE_PRODUCION = Datset.Tables(0).Rows(0).Item("GABINETE_PRODUCION")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(49) = True Then
                        estru_unidad_conservacion(i).Id_registro_procedimiento = 0
                    Else
                        estru_unidad_conservacion(i).Id_registro_procedimiento = Datset.Tables(0).Rows(0).Item("Id_registro_procedimiento")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(50) = True Then
                        estru_unidad_conservacion(i).Id_registro_proceso = 0
                    Else
                        estru_unidad_conservacion(i).Id_registro_proceso = Datset.Tables(0).Rows(0).Item("Id_registro_proceso")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(51) = True Then
                        estru_unidad_conservacion(i).ID_DISCO = 0
                    Else
                        estru_unidad_conservacion(i).ID_DISCO = Datset.Tables(0).Rows(0).Item("ID_DISCO")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(52) = True Then
                        estru_unidad_conservacion(i).ESTADO_FIRMA = 0
                    Else
                        estru_unidad_conservacion(i).ESTADO_FIRMA = Datset.Tables(0).Rows(0).Item("ESTADO_FIRMA")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(53) = True Then
                        estru_unidad_conservacion(i).FECHA_FIRMA = ""
                    Else
                        estru_unidad_conservacion(i).FECHA_FIRMA = Datset.Tables(0).Rows(0).Item("FECHA_FIRMA")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(54) = True Then
                        estru_unidad_conservacion(i).estado_expediente_electronico = 0
                    Else
                        estru_unidad_conservacion(i).estado_expediente_electronico = Datset.Tables(0).Rows(0).Item("estado_expediente_electronico")
                    End If
                    If Datset.Tables(0).Rows(0).IsNull(55) = True Then
                        estru_unidad_conservacion(i).ra_auto_registro_expediente_id_auto_registro = 0
                    Else
                        estru_unidad_conservacion(i).ra_auto_registro_expediente_id_auto_registro = Datset.Tables(0).Rows(0).Item("ra_auto_registro_expediente_id_auto_registro")
                    End If
                Next
                SolicitaDatosEstructuraExpediente = "YES"
                Exit Function
            Else
                SolicitaDatosEstructuraExpediente = "Imposible encontrar la estructura del expediente ( " & IdExpediente & " )"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosEstructuraExpediente = "Inconsistencia general funcion SolicitaDatosEstructuraExpediente " & ex.Message
        End Try
    End Function
    Function Limpia_campos_agregar_expediente(ByRef table_ref As Table) As String
        Try
            Dim tipo As String = ""
            For Each ob_TableRow As TableRow In table_ref.Rows
                For Each ob_TableCell As TableCell In ob_TableRow.Cells
                    For Each obcontrol As Object In ob_TableCell.Controls
                        Dim obcel As String = obcontrol.GetType().ToString
                        If obcel = "System.Web.UI.WebControls.TextBox" Then
                            If obcontrol.ReadOnly = False Then
                                obcontrol.text = ""
                            End If
                        End If
                    Next
                Next

            Next
            Limpia_campos_agregar_expediente = "YES"
        Catch ex As Exception
            Limpia_campos_agregar_expediente = "Inconsistencia función " & ex.Message
        End Try
    End Function
    Function Limpia_campos_agregar_expediente(ByRef table_ref As Table,
                                              ByRef update As UpdatePanel) As String
        Try
            Dim tipo As String = ""
            For Each ob_TableRow As TableRow In table_ref.Rows
                For Each ob_TableCell As TableCell In ob_TableRow.Cells
                    For Each obcontrol As Object In ob_TableCell.Controls
                        Dim obcel As String = obcontrol.GetType().ToString
                        If obcel = "System.Web.UI.WebControls.TextBox" Then
                            If obcontrol.ReadOnly = False Then
                                obcontrol.text = ""
                            End If
                        End If
                    Next
                Next

            Next
            update.Update()
            Limpia_campos_agregar_expediente = "YES"
        Catch ex As Exception
            Limpia_campos_agregar_expediente = "Inconsistencia función " & ex.Message
        End Try
    End Function
    Function Limpia_campos_consulta_expediente(ByRef panel_ref As Panel,
                                               ByRef update As UpdatePanel) As String
        Try
            Dim ref_DropDownListEstado_Expediente As DropDownList = panel_ref.FindControl("DropDownListEstado_Expediente")
            If Not ref_DropDownListEstado_Expediente Is Nothing Then
                ref_DropDownListEstado_Expediente.Text = "Todas"
            End If
            Dim ref_DropDownListUusuariocreador As DropDownList = panel_ref.FindControl("DropDownListUusuariocreador")
            If Not ref_DropDownListUusuariocreador Is Nothing Then
                ref_DropDownListUusuariocreador.Text = "Todas"
            End If
            Dim ref_DropDownListtipoexpediente As DropDownList = panel_ref.FindControl("DropDownListtipoexpediente")
            If Not ref_DropDownListtipoexpediente Is Nothing Then
                ref_DropDownListtipoexpediente.Text = "Todas"
            End If
            Dim ref_DropDownListEstadoExpedienteSierre As DropDownList = panel_ref.FindControl("DropDownListEstadoExpedienteSierre")
            If Not ref_DropDownListEstadoExpedienteSierre Is Nothing Then
                ref_DropDownListEstadoExpedienteSierre.Text = "Todas"
            End If
            Dim ref_DropDownListNOMBRE_FONDO As DropDownList = panel_ref.FindControl("DropDownListNOMBRE_FONDO")
            If Not ref_DropDownListNOMBRE_FONDO Is Nothing Then
                If ref_DropDownListNOMBRE_FONDO.Items.Count > 0 Then
                    For i As Integer = 0 To ref_DropDownListNOMBRE_FONDO.Items.Count - 1
                        ref_DropDownListNOMBRE_FONDO.Items(i).Text = "Todas"
                        Exit For
                    Next
                End If

            End If
            Dim ref_DropDownListNOMBRE_CICLO_ARCHIVO As DropDownList = panel_ref.FindControl("DropDownListNOMBRE_CICLO_ARCHIVO")
            If Not ref_DropDownListNOMBRE_CICLO_ARCHIVO Is Nothing Then
                If ref_DropDownListNOMBRE_CICLO_ARCHIVO.Items.Count > 0 Then
                    For i As Integer = 0 To ref_DropDownListNOMBRE_CICLO_ARCHIVO.Items.Count - 1
                        ref_DropDownListNOMBRE_CICLO_ARCHIVO.Items(i).Text = "Todas"
                        Exit For
                    Next
                End If
            End If
            Dim tipo As String = ""
            For Each ob As Object In panel_ref.Controls
                Dim g = ob.GetType().ToString
                tipo = tipo & ob.GetType().ToString & vbCrLf
                If ob.GetType().ToString = "System.Web.UI.WebControls.TextBox" Then
                    ob.text = ""
                End If
            Next
            update.Update()
            Limpia_campos_consulta_expediente = "YES"
        Catch ex As Exception
            Limpia_campos_consulta_expediente = "Inconsistencia función " & ex.Message
        End Try
    End Function
    Class MyStruct
        Public Property Name As String
        Public Property Adres As String

        Public Sub New(ByVal name As String, ByVal adress As String)
            name = name
            Adres = adress
        End Sub
    End Class
    Function Consulta_Expedientes(
      ByVal codigo_unico As String, ByVal fecha_creacion_ini As String, ByVal fecha_creacion_fin As String,
      ByVal tema_expediente As String, ByVal nombre_area As String,
      ByVal nombre_serie As String, ByVal nombre_sub_serie As String,
      ByVal fecha_extrema_inicial_inicial As String, ByVal fecha_extrema_inicial_final As String,
      ByVal fecha_extrema_final_inicial As String, ByVal fecha_extrema_final_final As String,
      ByVal rango_extremo_inicial_inicial As String, ByVal rango_extremo_inicial_final As String,
      ByVal rango_extremo_final_inicial As String, ByVal rango_extremo_final_final As String,
      ByVal usuario_gestion As DropDownList, ByVal estado_expediente As String,
      ByRef grediview As GridView,
      ByRef reflabel As Label, ByVal nombre_tipo_expediente As String,
      ByVal estado_expediente_serrado As String, ByVal asunto_expediente As String,
      ByVal id_empresa As Integer, ByRef update As UpdatePanel, ByRef hideselecion As Object,
      ByRef HiddenEmailconsulta As Object, ByVal option_expeidente_propio As Boolean,
      ByVal option_asunto As Boolean, ByVal option_observacion As Boolean, ByVal observacion As String,
      ByVal id_expediente As String, ByVal sub_seccion As String, ByVal tipo_unidad_documental As String,
      ByVal nombre_ciclo_documental As String, ByVal nombre_fondo_documental As String,
      ByVal nombre_persona_expediente As String, ByVal indentificacion_persona_expediente As String,
      ByVal nombre_responsable As String, identificacion_responsable As String,
      ByRef gred_view_documento As GridView, ByRef label_documento As Label,
      ByRef up_date_documento As UpdatePanel,
      ByVal tipo_consulta As Integer,
      ByRef colum_order_name As String,
      ByRef order_colum As String,
      ByVal valor_consulta As String,
      ByRef limit As String,
      ByVal estado_gestion_expedinete As Integer,
      ByRef UpdatePanel_general_titulo As UpdatePanel,
      ByRef cantidad_row As Object) As String
        Try
            gred_view_documento.DataSource = Nothing
            gred_view_documento.DataBind()
            label_documento.Text = "Se encontraron 0 registro(s) "
            up_date_documento.Update()
            Dim activaand As Integer = -1
            Dim sql_condicion As String = ""
            If tipo_consulta = 1 Then
                If codigo_unico <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where CODIGO_UNICO='" & codigo_unico & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND CODIGO_UNICO='" & codigo_unico & "'"
                    End If

                End If
                If id_expediente <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ID_EXPEDIENTE='" & id_expediente & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ID_EXPEDIENTE='" & id_expediente & "'"
                    End If

                End If
                If asunto_expediente <> "" Then
                    Dim likeigual As String = "="
                    If option_asunto = True Then
                        If InStr(asunto_expediente, "%") <= 0 Then
                            asunto_expediente = "%" & asunto_expediente & "%"
                        End If
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where ASUNTO_EXPEDIENTE like '" & asunto_expediente & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND ASUNTO_EXPEDIENTE like '" & asunto_expediente & "'"
                        End If
                    Else
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where ASUNTO_EXPEDIENTE='" & asunto_expediente & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND ASUNTO_EXPEDIENTE='" & asunto_expediente & "'"
                        End If
                    End If


                End If
                If observacion <> "" Then
                    Dim likeigual As String = "="
                    If option_observacion = True Then
                        If InStr(observacion, "%") <= 0 Then
                            observacion = "%" & observacion & "%"
                        End If
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where OBSERVACION_EXPEDIENTE like '" & observacion & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND OBSERVACION_EXPEDIENTE like '" & observacion & "'"
                        End If
                    Else
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where OBSERVACION_EXPEDIENTE='" & observacion & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND OBSERVACION_EXPEDIENTE='" & observacion & "'"
                        End If
                    End If


                End If
                If fecha_creacion_ini <> "" And fecha_creacion_fin <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where FECHA_CREACION BETWEEN '" & fecha_creacion_ini & "' AND '" &
                        fecha_creacion_fin & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND FECHA_CREACION BETWEEN '" & fecha_creacion_ini & "' AND '" &
                       fecha_creacion_fin & "'"
                    End If
                Else
                    If fecha_creacion_ini <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where FECHA_CREACION='" & fecha_creacion_ini & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND FECHA_CREACION='" & fecha_creacion_ini & "'"
                        End If
                    End If
                    If fecha_creacion_fin <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where FECHA_CREACION='" & fecha_creacion_fin & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND FECHA_CREACION='" & fecha_creacion_fin & "'"
                        End If
                    End If
                End If
                If tema_expediente <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where TEMA_EXPEDIENTE='" & tema_expediente & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND TEMA_EXPEDIENTE='" & tema_expediente & "'"
                    End If

                End If
                'nombre_area
                If nombre_area <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_AREA_TRD='" & nombre_area & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_AREA_TRD='" & nombre_area & "'"
                    End If

                End If
                'nombre_serie
                If nombre_serie <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_SERIE_TRD='" & nombre_serie & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_SERIE_TRD='" & nombre_serie & "'"
                    End If

                End If
                'nombre_sub_serie
                If nombre_sub_serie <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_SUBSERIE_TRD='" & nombre_sub_serie & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_SUBSERIE_TRD='" & nombre_sub_serie & "'"
                    End If

                End If

                'Rango fecha inicial
                If fecha_extrema_inicial_final <> "" And fecha_extrema_inicial_inicial <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where FECHA_EXTREMA_INICIAL between '" & fecha_extrema_inicial_inicial & "' and '" & fecha_extrema_inicial_final & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " and FECHA_EXTREMA_INICIAL between '" & fecha_extrema_inicial_inicial & "' and '" & fecha_extrema_inicial_final & "'"
                    End If
                Else
                    'fecha_extrema_incial incial
                    If fecha_extrema_inicial_inicial <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where FECHA_EXTREMA_INICIAL='" & fecha_extrema_inicial_inicial & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND FECHA_EXTREMA_INICIAL='" & fecha_extrema_inicial_inicial & "'"
                        End If

                    End If
                    'fecha_extrema_ inicial final
                    If fecha_extrema_inicial_final <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where FECHA_EXTREMA_INICIAL='" & fecha_extrema_inicial_final & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND FECHA_EXTREMA_INICIAL='" & fecha_extrema_inicial_final & "'"
                        End If

                    End If


                End If
                'Rango fecha final
                If fecha_extrema_final_inicial <> "" And fecha_extrema_final_final <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where FECHA_EXTREMA_FINAL between '" & fecha_extrema_final_inicial & "' and '" & fecha_extrema_final_final & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " and FECHA_EXTREMA_FINAL between '" & fecha_extrema_final_inicial & "' and '" & fecha_extrema_final_final & "'"
                    End If
                Else
                    'fecha_extrema_incial incial
                    If fecha_extrema_final_inicial <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where FECHA_EXTREMA_FINAL='" & fecha_extrema_final_inicial & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND FECHA_EXTREMA_FINAL='" & fecha_extrema_final_inicial & "'"
                        End If

                    End If
                    'fecha_extrema_ final final
                    If fecha_extrema_final_final <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where FECHA_EXTREMA_FINAL='" & fecha_extrema_final_final & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND FECHA_EXTREMA_FINAL='" & fecha_extrema_final_final & "'"
                        End If

                    End If
                End If

                'Rangos extremos iniciales 
                If rango_extremo_inicial_inicial <> "" And rango_extremo_inicial_final <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where RANGO_EXTREMO_INICIAL between '" & rango_extremo_inicial_inicial & "' and '" & rango_extremo_inicial_final & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " and RANGO_EXTREMO_INICIAL between '" & rango_extremo_inicial_inicial & "' and '" & rango_extremo_inicial_final & "'"
                    End If
                Else
                    'rango_extremo_inicial_inicial
                    If rango_extremo_inicial_inicial <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where RANGO_EXTREMO_INICIAL='" & rango_extremo_inicial_inicial & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND RANGO_EXTREMO_INICIAL='" & rango_extremo_inicial_inicial & "'"
                        End If

                    End If
                    ''rango_extremo_inicial_final
                    If rango_extremo_inicial_final <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where RANGO_EXTREMO_INICIAL='" & rango_extremo_inicial_final & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND RANGO_EXTREMO_INICIAL='" & rango_extremo_inicial_final & "'"
                        End If
                    End If
                End If

                'Rangos extremos finales
                If rango_extremo_final_inicial <> "" And rango_extremo_final_final <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where RANGO_EXTREMO_FINAL between '" & rango_extremo_final_inicial & "' and '" & rango_extremo_final_final & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " and RANGO_EXTREMO_FINAL between '" & rango_extremo_final_inicial & "' and '" & rango_extremo_final_final & "'"
                    End If
                Else
                    'Rango extremo final inicial
                    If rango_extremo_final_inicial <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where RANGO_EXTREMO_FINAL='" & rango_extremo_final_inicial & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND RANGO_EXTREMO_FINAL='" & rango_extremo_final_inicial & "'"
                        End If
                    End If
                    'Rango extremo final final
                    If rango_extremo_final_final <> "" Then
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where RANGO_EXTREMO_FINAL='" & rango_extremo_final_final & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND RANGO_EXTREMO_FINAL='" & rango_extremo_final_final & "'"
                        End If
                    End If
                End If
                If estado_expediente <> "" And estado_expediente <> "Todas" Then
                    Dim tipo_estado As Integer = 0
                    If estado_expediente = "Archivados" Then
                        tipo_estado = 1
                    End If
                    If estado_expediente = "Sin archivar" Then
                        tipo_estado = 0
                    End If
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ESTADO_ARCHIVO_EXPEDIENTE='" & tipo_estado & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ESTADO_ARCHIVO_EXPEDIENTE='" & tipo_estado & "'"
                    End If
                End If
                If usuario_gestion.Items.Count > 0 Then
                    For z As Integer = 0 To usuario_gestion.Items.Count - 1
                        Dim id_usuario_gestion As Integer = -1
                        If activaand = -1 Then
                            sql_condicion = sql_condicion & " where ID_USUARIO_GESTION='" & id_usuario_gestion & "'"
                            activaand = 1
                        Else
                            sql_condicion = sql_condicion & " AND ID_USUARIO_GESTION='" & id_usuario_gestion & "'"
                        End If
                    Next
                End If
                If option_expeidente_propio = True Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ID_USUARIO_GESTION='" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ID_USUARIO_GESTION='" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "'"
                    End If
                End If
                Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
                If nombre_tipo_expediente <> "Todas" And nombre_tipo_expediente <> "" Then
                    Dim id_tipo_expediente As Integer = 0
                    Dim Resulta = ref_Class_ra_tipo_expediente.Retorna_tipo_id_expediente(id_tipo_expediente,
                                                                                          nombre_tipo_expediente)
                    If Resulta <> "YES" Then
                        Consulta_Expedientes = Resulta
                        Exit Function
                    End If
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where RA_TIP_EXPE_ID_TIPO_EXPEDIENTE='" & id_tipo_expediente & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND RA_TIP_EXPE_ID_TIPO_EXPEDIENTE='" & id_tipo_expediente & "'"
                    End If
                End If
                Dim id_estado_expediente As Integer = 1
                If estado_expediente_serrado <> "Todas" And estado_expediente_serrado <> "" Then
                    If estado_expediente_serrado = "Cerrado" Then
                        id_estado_expediente = 0
                    End If
                    If estado_expediente_serrado = "Abierto" Then
                        id_estado_expediente = 1
                    End If
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where ESTADO_EXPEDIENTE='" & id_estado_expediente & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND ESTADO_EXPEDIENTE='" & id_estado_expediente & "'"
                    End If
                End If
                If activaand = -1 Then
                    sql_condicion = sql_condicion & " where ID_EMPRESA_EXPEDIENTE='" & id_empresa & "'"
                    activaand = 1
                Else
                    sql_condicion = sql_condicion & " AND ID_EMPRESA_EXPEDIENTE='" & id_empresa & "'"
                End If
                If sub_seccion <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_SUB_AREA='" & sub_seccion & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_SUB_AREA='" & sub_seccion & "'"
                    End If
                End If
                If tipo_unidad_documental <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_TIPO_UNIDAD_DOCUMENTAL='" & tipo_unidad_documental & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_TIPO_UNIDAD_DOCUMENTAL='" & tipo_unidad_documental & "'"
                    End If
                End If
                '------------------------------
                'nombre ciclo archivo
                '------------------------------
                If nombre_ciclo_documental <> "Todas" And nombre_ciclo_documental <> "" Then

                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_CICLO_ARCHIVO='" & nombre_ciclo_documental & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_CICLO_ARCHIVO='" & nombre_ciclo_documental & "'"
                    End If
                End If
                '-------------------------------
                'Nombre fondo documental
                '-------------------------------
                If nombre_fondo_documental <> "Todas" And nombre_fondo_documental <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_FONDO='" & nombre_fondo_documental & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_FONDO='" & nombre_fondo_documental & "'"
                    End If
                End If
                '-------------------------------
                'Nombre persona
                '-------------------------------
                If nombre_persona_expediente <> "Todas" And nombre_persona_expediente <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_PERSONA_EXPEDIENTE='" & nombre_persona_expediente & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_PERSONA_EXPEDIENTE='" & nombre_persona_expediente & "'"
                    End If
                End If
                '-------------------------------
                'Indentificación persona
                '-------------------------------
                If indentificacion_persona_expediente <> "Todas" And indentificacion_persona_expediente <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where IDENTIFICACION_PERSONA_EXPEDIENTE='" & indentificacion_persona_expediente & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND IDENTIFICACION_PERSONA_EXPEDIENTE='" & indentificacion_persona_expediente & "'"
                    End If
                End If
                '--------------------------------
                'Nombre responsable
                '--------------------------------
                If nombre_responsable <> "Todas" And nombre_responsable <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where NOMBRE_RESPONSABLE_EXPEDIENTE='" & nombre_responsable & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND NOMBRE_RESPONSABLE_EXPEDIENTE='" & nombre_responsable & "'"
                    End If
                End If
                '--------------------------------
                'Indentificacion responsable
                '-------------------------------
                If identificacion_responsable <> "Todas" And identificacion_responsable <> "" Then
                    If activaand = -1 Then
                        sql_condicion = sql_condicion & " where IDENFICACION_RESPONSABLE_EXPEDIENTE='" & identificacion_responsable & "'"
                        activaand = 1
                    Else
                        sql_condicion = sql_condicion & " AND IDENFICACION_RESPONSABLE_EXPEDIENTE='" & identificacion_responsable & "'"
                    End If
                End If
            End If
            If tipo_consulta = 2 Then
                sql_condicion = " where  CONSECUTIVO_EXPEDIENTE_2 like '%" & valor_consulta & "%'" &
                    "  or VOLUMEN_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or EXPEDIENTE_PADRE like '%" & valor_consulta & "%'" &
                    " or ID_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or CODIGO_UNICO like '%" & valor_consulta & "%'" &
                    " or NOMBRE_SERIE_TRD like '%" & valor_consulta & "%'" &
                    " or NOMBRE_SUBSERIE_TRD like '%" & valor_consulta & "%'" &
                    " or TEMA_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or ASUNTO_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or FECHA_CREACION like '%" & valor_consulta & "%'" &
                    " or CODIGO_AREA_TRD like '%" & valor_consulta & "%'" &
                    " or NOMBRE_AREA_TRD like '%" & valor_consulta & "%'" &
                    " or CODIGO_SERIE_TRD like '%" & valor_consulta & "%'" &
                    " or CODIGO_SUB_SERIE_TRD like '%" & valor_consulta & "%'" &
                    " or NOMBRE_TIPO_UNIDAD_DOCUMENTAL like '%" & valor_consulta & "%'" &
                    " or COMPOSICION_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or FECHA_EXTREMA_INICIAL like '%" & valor_consulta & "%'" &
                    " or FECHA_EXTREMA_FINAL like '%" & valor_consulta & "%'" &
                    " or RANGO_EXTREMO_INICIAL like '%" & valor_consulta & "%'" &
                    " or RANGO_EXTREMO_FINAL like '%" & valor_consulta & "%'" &
                    " or ESTADO_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or NOMBRE_PERSONA_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or IDENTIFICACION_PERSONA_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or NOMBRE_RESPONSABLE_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or IDENFICACION_RESPONSABLE_EXPEDIENTE like '%" & valor_consulta & "%'" &
                    " or NOMBRE_FONDO like '%" & valor_consulta & "%'" &
                    " or NOMBRE_CICLO_ARCHIVO like '%" & valor_consulta & "%'" &
                    " or NUMERO_FOLIOS_CONTENIDOS like '%" & valor_consulta & "%'" &
                    " or NUMERO_ELECTRONICO_CONTENIDO like '%" & valor_consulta & "%'" &
                    " or NUMERO_DIGITALIZADO_CONTENIDO like '%" & valor_consulta & "%'"
            End If
            Dim sql_consulta As String = "SELECT CONSECUTIVO_EXPEDIENTE_2,VOLUMEN_EXPEDIENTE,EXPEDIENTE_PADRE,ID_EXPEDIENTE AS CODIGO_UNICO," &
                "CODIGO_UNICO AS CONSECUTIVO,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD,TEMA_EXPEDIENTE AS TEMA, " &
                "ASUNTO_EXPEDIENTE AS ASUNTO,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD," &
                "CODIGO_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS TIPO_UNIDAD,COMPOSICION_EXPEDIENTE," &
                "FECHA_EXTREMA_INICIAL AS FECHA_INICIAL_EXPEDICION,FECHA_EXTREMA_FINAL AS FECHA_FINAL_TERMINACION," &
                "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,ESTADO_EXPEDIENTE,NOMBRE_PERSONA_EXPEDIENTE" &
                " AS NOMBRE_SOLICITANTE,IDENTIFICACION_PERSONA_EXPEDIENTE AS IDENTIFICACION_SOLICITANTE," _
                & "NOMBRE_RESPONSABLE_EXPEDIENTE AS RESPONSABLE_EXPEDIENTE,IDENFICACION_RESPONSABLE_EXPEDIENTE" &
                " AS IDENFICACION_RESPONSABLE,NOMBRE_FONDO,NOMBRE_CICLO_ARCHIVO,NUMERO_FOLIOS_CONTENIDOS as FOLIO_FISICO," &
                "NUMERO_ELECTRONICO_CONTENIDO AS FOLIO_ELECTRONICO,NUMERO_DIGITALIZADO_CONTENIDO AS FOLIO_DIGITALIZADO from expediente_archivo "
            Dim sqltotal As String = sql_consulta & sql_condicion & " order by " & colum_order_name & " " & order_colum & " " & limit
            HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = sql_consulta
            HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_CONDICION") = sql_condicion
            HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_ADD_PAGINACION") = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqltotal, Datset)
            If Result <> "YES" Then
                Consulta_Expedientes = "Error listando datos " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                cantidad_row = 0
                HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = ""
                reflabel.Text = "0 registro(s) de expediente (s)"
                Datset.Tables(0).Rows.Add(Datset.Tables(0).NewRow)
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                grediview.Rows(0).Visible = False
                update.Update()
                UpdatePanel_general_titulo.Update()
                Consulta_Expedientes = "YES"
                Exit Function
            Else
                cantidad_row = Datset.Tables(0).Rows.Count
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro(s) de expediente (s)"
                grediview.DataSource = Nothing
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                UpdatePanel_general_titulo.Update()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ihtml.Style.Add("color", "white")
                    If grediview.Rows(i).Cells(29).Text > "0" Or grediview.Rows(i).Cells(30).Text > "0" Then
                        ihtml.Attributes.Add("class", "fas fa-folder-open fa-lg")
                    Else
                        ihtml.Attributes.Add("class", "fad fa-folder-open fa-lg")
                    End If
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos relacionados")
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc_col")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ihtml.Style.Add("color", "white")
                    If Val(grediview.Rows(i).Cells(1).Text) > 1 Then
                        ihtml.Attributes.Add("class", "fal fa-database fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente padre, click para listar expedientes relacionados")
                        ahtml.Attributes.Add("id_list_rel_", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "display_exp")
                    End If
                    If Val(grediview.Rows(i).Cells(2).Text) > 1 Then
                        ihtml.Attributes.Add("class", "fal fa-coins fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente volumen del expediente (" & grediview.Rows(i).Cells(3).Text.ToString() & "), presione click para desvincular")
                        ahtml.Attributes.Add("idd_image_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("idd_expediente_rel_padre", grediview.Rows(i).Cells(3).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "elimina_rel_exp")

                    End If
                    If Val(grediview.Rows(i).Cells(1).Text) = 1 And Val(grediview.Rows(i).Cells(2).Text) = 1 Then
                        ihtml.Attributes.Add("class", "fal fa-folder-plus fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente, click para relacionar como voulmen ")
                        ahtml.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "activa_rel_exp")

                    End If
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    If estado_gestion_expedinete = 1 Then
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-paste fa-lg")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Copiar documentos al expediente")
                        ahtml.Attributes.Add("id_expediente_", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "copia_documento_expediente")
                        ahtml.Attributes.Add("Class", "btn btn-dark btn-sm")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If

                    If estado_gestion_expedinete = 2 Then
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-folder-download fa-lg")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-dark btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Vincular documentos al expediente")
                        ahtml.Attributes.Add("id_expediente_", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "vincula_documento_expediente")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    If estado_gestion_expedinete = 3 Then
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-folder-upload fa-lg")
                        ahtml.Attributes.Add("Class", "btn btn-dark btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Asigna expediente al nuevo radicado")
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "asig_exp")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If

                    Next
                Next
                HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion") = {"OPCIONES", "CONSECUTIVO_EXPEDIENTE_2",
                                                                           "VOLUMEN_EXPEDIENTE", "EXPEDIENTE_PADRE",
                                                                           "CODIGO_UNICO", "CONSECUTIVO", "NOMBRE_SERIE_TRD", "NOMBRE_SUBSERIE_TRD", "TEMA",
                                                                           "ASUNTO", "FECHA_CREACION", "CODIGO_AREA_TRD", "NOMBRE_AREA_TRD",
                                                                            "CODIGO_SERIE_TRD", "CODIGO_SUB_SERIE_TRD", "TIPO_UNIDAD",
                                                                            "COMPOSICION_EXPEDIENTE", "FECHA_INICIAL_EXPEDICION",
                                                                            "FECHA_FINAL_TERMINACION", "RANGO_EXTREMO_INICIAL",
                                                                            "RANGO_EXTREMO_FINAL", "ESTADO_EXPEDIENTE",
                                                                            "NOMBRE_SOLICITANTE", "IDENTIFICACION_PERSONA_EXPEDIENTE",
                                                                            "NOMBRE_RESPONSABLE_EXPEDIENTE", "IDENTIFICACION_SOLICITANTE",
                                                                            "NOMBRE_FONDO", "NOMBRE_CICLO_ARCHIVO", "FOLIO_FISICO",
                                                                            "FOLIO_ELECTRONICO", "FOLIO_DIGITALIZADO"}
                HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = colum_order_name
                HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = order_colum
                HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion") = tipo_consulta
                HttpContext.Current.Session.Item("GA_DATO_CONSULTA_expe_clasificacion") = sql_consulta
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion"),
                                                            order_colum,
                                                            grediview)
                If Result <> "YES" Then
                    Consulta_Expedientes = "Error add clase funcion  Lista_solictudes_compartidos_de_un_usuario " & Result
                    Exit Function
                End If
                Consulta_Expedientes = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Consulta_Expedientes = "Inconsistencia general funcion Consulta_Expedientes " & ex.Message
        End Try
    End Function
    Function Consulta_Expedientes_post(ByRef update As UpdatePanel,
                                       ByRef hideselecion As Object,
                                       ByRef HiddenEmailconsulta As Object,
                                       ByRef grediview As GridView,
                                       ByRef reflabel As Object,
                                       ByVal tipo_consulta As Integer,
                                       ByRef colum_order_name As String,
                                       ByRef order_colum As String,
                                       ByVal valor_consulta As String,
                                       ByRef limit As String,
                                       ByVal estado_gestion_expedinete As Integer,
                                       ByRef UpdatePanel_general_titulo As UpdatePanel) As String
        Try
            If HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = "" Then
                Consulta_Expedientes_post = "YES"
                Exit Function
            End If
            Dim sql_consulta As String = HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") & HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_CONDICION") &
             HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_ADD_PAGINACION") & " order by " & colum_order_name & " " & order_colum & " " & limit
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("radicado")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_Expedientes_post = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = ""
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro(s) de expediente (s) " &
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                UpdatePanel_general_titulo.Update()
                update.Update()
                Consulta_Expedientes_post = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro(s) de expediente (s)"
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                UpdatePanel_general_titulo.Update()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ihtml.Style.Add("color", "white")
                    If grediview.Rows(i).Cells(28).Text > 0 Or grediview.Rows(i).Cells(29).Text > 0 Then
                        ihtml.Attributes.Add("class", "fas fa-folder-open fa-lg")
                    Else
                        ihtml.Attributes.Add("class", "fad fa-folder-open fa-lg")
                    End If
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos relacionados")
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc_col")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ihtml.Style.Add("color", "white")
                    If Val(grediview.Rows(i).Cells(1).Text) > 1 Then
                        ihtml.Attributes.Add("class", "fal fa-database fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente padre, click para listar expedientes relacionados")
                        ahtml.Attributes.Add("id_list_rel_", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "display_exp")
                    End If
                    If Val(grediview.Rows(i).Cells(2).Text) > 1 Then
                        ihtml.Attributes.Add("class", "fal fa-coins fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente volumen del expediente (" & grediview.Rows(i).Cells(3).Text.ToString() & "), presione click para desvincular")
                        ahtml.Attributes.Add("idd_image_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("idd_expediente_rel_padre", grediview.Rows(i).Cells(3).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "elimina_rel_exp")

                    End If
                    If Val(grediview.Rows(i).Cells(1).Text) = 1 And Val(grediview.Rows(i).Cells(2).Text) = 1 Then
                        ihtml.Attributes.Add("class", "fal fa-folder-plus fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente, click para relacionar como voulmen ")
                        ahtml.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "activa_rel_exp")
                    End If
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    If estado_gestion_expedinete = 1 Then
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-paste fa-lg")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Copiar documentos al expediente")
                        ahtml.Attributes.Add("id_expediente_", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "copia_documento_expediente")
                        ahtml.Attributes.Add("Class", "btn btn-dark btn-sm")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    If estado_gestion_expedinete = 2 Then
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-folder-download fa-lg")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-dark btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Vincular documentos al expediente")
                        ahtml.Attributes.Add("id_expediente_", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "vincula_documento_expediente")
                        ahtml.Style.Add("margin-left", "1px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    If estado_gestion_expedinete = 3 Then
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-folder-upload fa-lg")
                        ahtml.Attributes.Add("Class", "btn btn-dark btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Asigna expediente al nuevo radicado")
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "asig_exp")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If

                    Next
                Next
                'For i As Integer = 0 To grediview.Rows.Count - 1
                '    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                '    grediview.HeaderRow.Cells(i).Attributes.Add("Class", "GridviewScrollHeader_line_blanco_cort_leter")
                '    Dim imaga_buton As New HtmlInputImage
                '    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                '    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                '    imaga_buton.Attributes.Add("tip_event", "asig_exp")
                '    imaga_buton.Attributes.Add("title", "Asigna expediente al nuevo radicado")
                '    imaga_buton.Src = "../gestion/imagenes/layer-plus-light.png"
                '    imaga_buton.Attributes.Add("idd_image", grediview.Rows(i).Cells(4).Text.ToString())
                '    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                '    imaga_buton = New HtmlInputImage
                '    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                '    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                '    If Val(grediview.Rows(i).Cells(1).Text) > 1 Then
                '        imaga_buton.Src = "../gestion/imagenes/folder-plus-light.png"
                '        imaga_buton.Attributes.Add("title", "Expediente padre, click para listar expedientes relacionados")
                '        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                '        imaga_buton.Attributes.Add("tip_event", "display_exp")
                '    End If
                '    If Val(grediview.Rows(i).Cells(2).Text) > 1 Then
                '        imaga_buton.Src = "../gestion/imagenes/folder-light-vol.png"
                '        imaga_buton.Attributes.Add("title", "Expediente volumen de expediente " & grediview.Rows(i).Cells(3).Text.ToString())
                '        imaga_buton.Attributes.Add("tip_event", "elimina_rel_exp")
                '        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                '    End If
                '    If Val(grediview.Rows(i).Cells(1).Text) = 1 And Val(grediview.Rows(i).Cells(2).Text) = 1 Then
                '        imaga_buton.Src = "../gestion/imagenes/folder-light.png"
                '        imaga_buton.Attributes.Add("tip_event", "activa_rel_exp")
                '        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                '        imaga_buton.Attributes.Add("title", "Expediente, click para relacionar como voulmen")
                '    End If
                '    imaga_buton.Attributes.Add("idd_image", grediview.Rows(i).Cells(4).Text.ToString())
                '    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                '    imaga_buton = New HtmlInputImage
                '    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                '    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                '    imaga_buton.Attributes.Add("title", "Ver documentos relacionados")
                '    If grediview.Rows(i).Cells(28).Text > 0 Or grediview.Rows(i).Cells(29).Text > 0 Then
                '        imaga_buton.Src = "../workflow/imageneswf/lista_sub_serie.png"
                '    Else
                '        imaga_buton.Src = "../workflow/imageneswf/folder-open-light.png"
                '    End If
                '    imaga_buton.Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                '    imaga_buton.Attributes.Add("tip_event", "ver_doc_col")
                '    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                '    imaga_buton = New HtmlInputImage
                '    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                '    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                '    imaga_buton.Attributes.Add("title", "Copiar documentos al expediente")
                '    imaga_buton.Src = "../workflow/imageneswf/page_white_principal.png"
                '    imaga_buton.Attributes.Add("id_expediente_", grediview.Rows(i).Cells(4).Text.ToString())
                '    imaga_buton.Attributes.Add("tip_event", "copia_documento_expediente")
                '    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                '    imaga_buton = New HtmlInputImage
                '    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                '    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                '    imaga_buton.Attributes.Add("title", "Vincular documentos al expediente")
                '    imaga_buton.Src = "../workflow/imageneswf/page_white.png"
                '    imaga_buton.Attributes.Add("id_expediente_", grediview.Rows(i).Cells(4).Text.ToString())
                '    imaga_buton.Attributes.Add("tip_event", "vincula_documento_expediente")
                '    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                '    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                '        grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_corte_tr")
                '        grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                '    Next
                'Next
                HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion") = {"OPCIONES", "CONSECUTIVO_EXPEDIENTE_2",
                                                                           "VOLUMEN_EXPEDIENTE", "EXPEDIENTE_PADRE",
                                                                           "CODIGO_UNICO", "CONSECUTIVO", "NOMBRE_SERIE_TRD", "NOMBRE_SUBSERIE_TRD", "TEMA",
                                                                           "ASUNTO", "FECHA_CREACION", "CODIGO_AREA_TRD", "NOMBRE_AREA_TRD",
                                                                            "CODIGO_SERIE_TRD", "CODIGO_SUB_SERIE_TRD", "TIPO_UNIDAD",
                                                                            "COMPOSICION_EXPEDIENTE", "FECHA_INICIAL_EXPEDICION",
                                                                            "FECHA_FINAL_TERMINACION", "RANGO_EXTREMO_INICIAL",
                                                                            "RANGO_EXTREMO_FINAL", "ESTADO_EXPEDIENTE",
                                                                            "NOMBRE_SOLICITANTE", "IDENTIFICACION_PERSONA_EXPEDIENTE",
                                                                            "NOMBRE_RESPONSABLE_EXPEDIENTE", "IDENTIFICACION_SOLICITANTE",
                                                                            "NOMBRE_FONDO", "NOMBRE_CICLO_ARCHIVO", "FOLIO_FISICO",
                                                                            "FOLIO_ELECTRONICO", "FOLIO_DIGITALIZADO"}
                HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = colum_order_name
                HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = order_colum
                HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion") = tipo_consulta
                HttpContext.Current.Session.Item("GA_DATO_CONSULTA_expe_clasificacion") = sql_consulta
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion"),
                                                            order_colum,
                                                            grediview)
                If Result <> "YES" Then
                    Consulta_Expedientes_post = "Error add clase funcion  Lista_solictudes_compartidos_de_un_usuario " & Result
                    Exit Function
                End If
                Consulta_Expedientes_post = "YES"
                Exit Function
            End If
            Consulta_Expedientes_post = "YES"
        Catch ex As Exception
            Consulta_Expedientes_post = "Inconsistencia funcion Consulta_Expedientes_post " & ex.Message
        End Try
    End Function
    Function Listar_expedientes_agregados(ByVal id_usuario_gestion As Integer,
                                          ByRef grediview As GridView,
                                          ByRef reflabel As Label,
                                          ByRef HiddenEmailconsulta As Object,
                                          ByRef hideselecion As Object,
                                          ByRef update As UpdatePanel,
                                          ByVal tipo_consulta As Integer,
                                          ByRef colum_order_name As String,
                                          ByRef order_colum As String,
                                          ByVal valor_consulta As String,
                                          ByRef limit As String) As String
        Try
            Dim sqlcampos As String = "SELECT CONSECUTIVO_EXPEDIENTE_2,VOLUMEN_EXPEDIENTE,EXPEDIENTE_PADRE,ID_EXPEDIENTE AS CODIGO_UNICO," &
                "CODIGO_UNICO AS CONSECUTIVO,TEMA_EXPEDIENTE AS TEMA,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS TIPO_UNIDAD," &
                "ASUNTO_EXPEDIENTE AS ASUNTO,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD," &
                "CODIGO_SERIE_TRD,NOMBRE_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_SUBSERIE_TRD,COMPOSICION_EXPEDIENTE," &
                "FECHA_EXTREMA_INICIAL AS FECHA_INICIAL_EXPEDICION,FECHA_EXTREMA_FINAL AS FECHA_FINAL_TERMINACION," &
                "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,ESTADO_EXPEDIENTE,NOMBRE_PERSONA_EXPEDIENTE" &
                " AS NOMBRE_SOLICITANTE,IDENTIFICACION_PERSONA_EXPEDIENTE AS IDENTIFICACION_SOLICITANTE," _
                & "NOMBRE_RESPONSABLE_EXPEDIENTE AS RESPONSABLE_EXPEDIENTE,IDENFICACION_RESPONSABLE_EXPEDIENTE" &
                " AS IDENFICACION_RESPONSABLE,NOMBRE_FONDO,NOMBRE_CICLO_ARCHIVO,NUMERO_FOLIOS_CONTENIDOS as FOLIO_FISICO," &
                "NUMERO_ELECTRONICO_CONTENIDO AS FOLIO_ELECTRONICO,NUMERO_DIGITALIZADO_CONTENIDO AS FOLIO_DIGITALIZADO "
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Today
            Dim Result As String = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Listar_expedientes_agregados = Result
                Exit Function
            End If
            Dim sql_condicion As String = " from expediente_archivo where FECHA_CREACION='" & date1al & "' and ID_USUARIO_GESTION=" & id_usuario_gestion &
            " "
            colum_order_name = "ID_EXPEDIENTE"
            order_colum = "DESC"
            Dim sql_consulta As String = sqlcampos & sql_condicion & " ORDER BY " & colum_order_name & " " & order_colum & " " & limit
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_expedientes_agregados = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = sqlcampos & sql_condicion
                reflabel.Text = "Se encontro 0 registro(s) de expediente "
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                Listar_expedientes_agregados = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = sqlcampos & sql_condicion
                reflabel.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) de expediente "
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                    grediview.HeaderRow.Cells(i).Attributes.Add("Class", "GridviewScrollHeader_line_blanco_cort_leter")
                    Dim imaga_buton As New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    imaga_buton.Attributes.Add("tip_event", "asig_exp")
                    imaga_buton.Attributes.Add("title", "Agrega documento a expediente")
                    imaga_buton.Src = "../gestion/imagenes/layer-plus-light.png"
                    imaga_buton.Attributes.Add("idd_image", grediview.Rows(i).Cells(4).Text.ToString())
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    imaga_buton = New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    If Val(grediview.Rows(i).Cells(1).Text) > 1 Then
                        imaga_buton.Src = "../gestion/imagenes/folder-plus-light.png"
                        imaga_buton.Attributes.Add("title", "Expediente padre, click para listar expedientes relacionados")
                        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        imaga_buton.Attributes.Add("tip_event", "display_exp")
                    End If
                    If Val(grediview.Rows(i).Cells(2).Text) > 1 Then
                        imaga_buton.Src = "../gestion/imagenes/folder-light-vol.png"
                        imaga_buton.Attributes.Add("title", "Expediente volumen de expediente " & grediview.Rows(i).Cells(3).Text.ToString())
                        imaga_buton.Attributes.Add("tip_event", "elimina_rel_exp")
                        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                    End If
                    If Val(grediview.Rows(i).Cells(1).Text) = 1 And Val(grediview.Rows(i).Cells(2).Text) = 1 Then
                        imaga_buton.Src = "../gestion/imagenes/folder-light.png"
                        imaga_buton.Attributes.Add("tip_event", "activa_rel_exp")
                        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        imaga_buton.Attributes.Add("title", "Expediente, click para relacionar como voulmen")
                    End If
                    imaga_buton.Attributes.Add("idd_image", grediview.Rows(i).Cells(4).Text.ToString())
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    imaga_buton = New HtmlInputImage
                    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                    imaga_buton.Attributes.Add("title", "Ver documentos relacionados")
                    If grediview.Rows(i).Cells(28).Text > 0 Or grediview.Rows(i).Cells(29).Text > 0 Then
                        imaga_buton.Src = "../workflow/imageneswf/lista_sub_serie.png"
                    Else
                        imaga_buton.Src = "../workflow/imageneswf/folder-open-light.png"
                    End If
                    imaga_buton.Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                    imaga_buton.Attributes.Add("tip_event", "ver_doc_col")
                    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_corte_tr")
                        grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                    Next
                Next
                HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion") = {"OPCIONES", "CONSECUTIVO_EXPEDIENTE_2",
                                                                          "VOLUMEN_EXPEDIENTE", "EXPEDIENTE_PADRE",
                                                                          "CODIGO_UNICO", "CONSECUTIVO", "TEMA", "TIPO_UNIDAD",
                                                                          "ASUNTO", "FECHA_CREACION", "CODIGO_AREA_TRD", "NOMBRE_AREA_TRD",
                                                                           "CODIGO_SERIE_TRD", "NOMBRE_SERIE_TRD", "CODIGO_SUB_SERIE_TRD",
                                                                           "NOMBRE_SUBSERIE_TRD", "COMPOSICION_EXPEDIENTE", "FECHA_INICIAL_EXPEDICION",
                                                                           "FECHA_FINAL_TERMINACION", "RANGO_EXTREMO_INICIAL",
                                                                           "RANGO_EXTREMO_FINAL", "ESTADO_EXPEDIENTE",
                                                                           "NOMBRE_SOLICITANTE", "IDENTIFICACION_PERSONA_EXPEDIENTE",
                                                                           "NOMBRE_RESPONSABLE_EXPEDIENTE", "IDENTIFICACION_SOLICITANTE",
                                                                           "NOMBRE_FONDO", "NOMBRE_CICLO_ARCHIVO", "FOLIO_FISICO",
                                                                           "FOLIO_ELECTRONICO", "FOLIO_DIGITALIZADO"}
                HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = colum_order_name
                HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = order_colum
                HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion") = tipo_consulta
                HttpContext.Current.Session.Item("GA_DATO_CONSULTA_expe_clasificacion") = sql_consulta
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion"),
                                                            order_colum,
                                                            grediview)
                If Result <> "YES" Then
                    Listar_expedientes_agregados = "Error add clase funcion  Lista_solictudes_compartidos_de_un_usuario " & Result
                    Exit Function
                End If
                Listar_expedientes_agregados = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Listar_expedientes_agregados = "Inconsistencia funcion Listar_expedientes_agregados " & ex.Message
        End Try
    End Function

    Function Listar_expedientes_agregados_volumen(ByVal id_usuario_gestion As Integer,
                                                  ByRef grediview As GridView,
                                                  ByRef reflabel As Label,
                                                  ByRef HiddenEmailconsulta As Object,
                                                  ByRef hideselecion As Object,
                                                  ByRef update As UpdatePanel,
                                                  ByVal id_expediente_padre As Integer,
                                                  ByVal tipo_consulta As Integer,
                                                  ByRef colum_order_name As String,
                                                  ByRef order_colum As String,
                                                  ByVal valor_consulta As String,
                                                  ByRef limit As String,
                                                  ByVal estado_gestion_expedinete As Integer,
                                                  ByRef UpdatePanel_general_titulo As UpdatePanel) As String
        Try
            Dim sqlcampos As String = "SELECT CONSECUTIVO_EXPEDIENTE_2,VOLUMEN_EXPEDIENTE,EXPEDIENTE_PADRE,ID_EXPEDIENTE AS CODIGO_UNICO," &
                "CODIGO_UNICO AS CONSECUTIVO,TEMA_EXPEDIENTE AS TEMA,NOMBRE_TIPO_UNIDAD_DOCUMENTAL AS TIPO_UNIDAD," &
                "ASUNTO_EXPEDIENTE AS ASUNTO,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD," &
                "CODIGO_SERIE_TRD,NOMBRE_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_SUBSERIE_TRD,COMPOSICION_EXPEDIENTE," &
                "FECHA_EXTREMA_INICIAL AS FECHA_INICIAL_EXPEDICION,FECHA_EXTREMA_FINAL AS FECHA_FINAL_TERMINACION," &
                "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,ESTADO_EXPEDIENTE,NOMBRE_PERSONA_EXPEDIENTE" &
                " AS NOMBRE_SOLICITANTE,IDENTIFICACION_PERSONA_EXPEDIENTE AS IDENTIFICACION_SOLICITANTE," _
                & "NOMBRE_RESPONSABLE_EXPEDIENTE AS RESPONSABLE_EXPEDIENTE,IDENFICACION_RESPONSABLE_EXPEDIENTE" &
                " AS IDENFICACION_RESPONSABLE,NOMBRE_FONDO,NOMBRE_CICLO_ARCHIVO,NUMERO_FOLIOS_CONTENIDOS as FOLIO_FISICO," &
                "NUMERO_ELECTRONICO_CONTENIDO AS FOLIO_ELECTRONICO,NUMERO_DIGITALIZADO_CONTENIDO AS FOLIO_DIGITALIZADO "
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Today
            Dim Result As String = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Listar_expedientes_agregados_volumen = Result
                Exit Function
            End If
            Dim sql_condicion As String = " from expediente_archivo where ID_EXPEDIENTE='" & id_expediente_padre & "' or EXPEDIENTE_PADRE=" & id_expediente_padre &
            " "
            colum_order_name = "ID_EXPEDIENTE"
            order_colum = "DESC"
            Dim sql_consulta As String = sqlcampos & sql_condicion & " ORDER BY " & colum_order_name & " " & order_colum & " " & limit
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_expedientes_agregados_volumen = "Error listando datos " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = sqlcampos & sql_condicion

                'HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_AGREGADOS") = ""
                reflabel.Text = "Se encontro 0 registro(s) de expediente "
                grediview.DataSource = Nothing
                'grediview.DataKeyNames = DataKey
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                UpdatePanel_general_titulo.Update()
                Listar_expedientes_agregados_volumen = "YES"
                Exit Function
            Else
                HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES") = sqlcampos & sql_condicion
                'HttpContext.Current.Session.Item("GA_CACHE_CONSULTA_EXPEDIENTES_AGREGADOS") = sql_consulta
                reflabel.Text = "Se encontro " & Datset.Tables(0).Rows.Count & " registro(s) de expediente "
                'grediview.DataKeyNames = DataKey
                grediview.DataSource = Datset
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                UpdatePanel_general_titulo.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ihtml.Style.Add("color", "white")
                    If grediview.Rows(i).Cells(28).Text > 0 Or grediview.Rows(i).Cells(29).Text > 0 Then
                        ihtml.Attributes.Add("class", "fas fa-folder-open fa-lg")
                    Else
                        ihtml.Attributes.Add("class", "fad fa-folder-open fa-lg")
                    End If
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Ver documentos relacionados")
                    ahtml.Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "ver_doc_col")
                    ahtml.Style.Add("margin-left", "1px")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ihtml.Style.Add("color", "white")
                    If Val(grediview.Rows(i).Cells(1).Text) > 1 Then
                        ihtml.Attributes.Add("class", "fal fa-folder fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente padre, click para listar expedientes relacionados")
                        ahtml.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "display_exp")
                    End If
                    If Val(grediview.Rows(i).Cells(2).Text) > 1 Then
                        ihtml.Attributes.Add("class", "fal fa-folders fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente volumen de expediente " & grediview.Rows(i).Cells(3).Text.ToString())
                        ahtml.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "elimina_rel_exp")
                    End If
                    If Val(grediview.Rows(i).Cells(1).Text) = 1 And Val(grediview.Rows(i).Cells(2).Text) = 1 Then
                        ihtml.Attributes.Add("class", "fal fa-folder-plus fa-lg")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Expediente, click para relacionar como voulmen ")
                        ahtml.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "activa_rel_exp")
                    End If
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    If estado_gestion_expedinete = 1 Then
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-paste fa-lg")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Copiar documentos al expediente")
                        ahtml.Attributes.Add("id_expediente_", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "copia_documento_expediente")
                        ahtml.Attributes.Add("Class", "btn btn-dark btn-sm")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If

                    If estado_gestion_expedinete = 2 Then
                        ihtml = New HtmlControls.HtmlGenericControl("i")
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-folder-download fa-lg")
                        ahtml = New HtmlControls.HtmlGenericControl("a")
                        ahtml.Attributes.Add("Class", "btn btn-dark btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Vincular documentos al expediente")
                        ahtml.Attributes.Add("id_expediente_", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "vincula_documento_expediente")
                        ahtml.Style.Add("margin-left", "1px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    If estado_gestion_expedinete = 3 Then
                        ihtml.Style.Add("color", "white")
                        ihtml.Attributes.Add("class", "fal fa-folder-upload fa-lg")
                        ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                        ahtml.Attributes.Add("onclick", "prevent(event,this);")
                        ahtml.Attributes.Add("title", "Asigna expediente al nuevo radicado")
                        ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(4).Text.ToString())
                        ahtml.Attributes.Add("tip_event", "asig_exp")
                        ahtml.Style.Add("margin-left", "3px")
                        ahtml.Controls.Add(ihtml)
                        divhtml.Controls.Add(ahtml)
                    End If
                    divhtml.Style.Add("display", "inline-flex")
                    grediview.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If

                    Next
                Next
                'For i As Integer = 0 To grediview.Rows.Count - 1
                '    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                '    grediview.HeaderRow.Cells(i).Attributes.Add("Class", "GridviewScrollHeader_line_blanco_cort_leter")
                '    Dim imaga_buton As New HtmlInputImage
                '    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                '    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                '    imaga_buton.Attributes.Add("tip_event", "asig_exp")
                '    imaga_buton.Attributes.Add("title", "Agrega documento a expediente")
                '    imaga_buton.Src = "../gestion/imagenes/layer-plus-light.png"
                '    imaga_buton.Attributes.Add("idd_image", grediview.Rows(i).Cells(4).Text.ToString())
                '    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                '    imaga_buton = New HtmlInputImage
                '    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                '    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                '    If Val(grediview.Rows(i).Cells(1).Text) > 1 Then
                '        imaga_buton.Src = "../gestion/imagenes/folder-plus-light.png"
                '        imaga_buton.Attributes.Add("title", "Expediente padre, click para listar expedientes relacionados")
                '        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                '        imaga_buton.Attributes.Add("tip_event", "display_exp")
                '    End If
                '    If Val(grediview.Rows(i).Cells(2).Text) > 1 Then
                '        imaga_buton.Src = "../gestion/imagenes/folder-light-vol.png"
                '        imaga_buton.Attributes.Add("title", "Expediente volumen de expediente " & grediview.Rows(i).Cells(3).Text.ToString())
                '        imaga_buton.Attributes.Add("tip_event", "elimina_rel_exp")
                '        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                '    End If
                '    If Val(grediview.Rows(i).Cells(1).Text) = 1 And Val(grediview.Rows(i).Cells(2).Text) = 1 Then
                '        imaga_buton.Src = "../gestion/imagenes/folder-light.png"
                '        imaga_buton.Attributes.Add("tip_event", "activa_rel_exp")
                '        imaga_buton.Attributes.Add("id_list_rel", grediview.Rows(i).Cells(4).Text.ToString())
                '        imaga_buton.Attributes.Add("title", "Expediente, click para relacionar como voulmen")
                '    End If
                '    imaga_buton.Attributes.Add("idd_image", grediview.Rows(i).Cells(4).Text.ToString())
                '    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                '    imaga_buton = New HtmlInputImage
                '    imaga_buton.Attributes.Add("Class", "image_buton_clik_image_")
                '    imaga_buton.Attributes.Add("onclick", "prevent(event,this);")
                '    imaga_buton.Attributes.Add("title", "Ver documentos relacionados")
                '    If grediview.Rows(i).Cells(28).Text > 0 Or grediview.Rows(i).Cells(29).Text > 0 Then
                '        imaga_buton.Src = "../workflow/imageneswf/lista_sub_serie.png"
                '    Else
                '        imaga_buton.Src = "../workflow/imageneswf/folder-open-light.png"
                '    End If
                '    imaga_buton.Attributes.Add("id", grediview.Rows(i).Cells(4).Text.ToString())
                '    imaga_buton.Attributes.Add("tip_event", "ver_doc_col")
                '    grediview.Rows(i).Cells(0).Controls.Add(imaga_buton)
                '    For z As Integer = 0 To grediview.Rows(i).Cells.Count - 1
                '        grediview.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_corte_tr")
                '        grediview.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                '    Next
                'Next
                HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion") = {"OPCIONES", "CONSECUTIVO_EXPEDIENTE_2",
                                                                         "VOLUMEN_EXPEDIENTE", "EXPEDIENTE_PADRE",
                                                                         "CODIGO_UNICO", "CONSECUTIVO", "TEMA", "TIPO_UNIDAD",
                                                                         "ASUNTO", "FECHA_CREACION", "CODIGO_AREA_TRD", "NOMBRE_AREA_TRD",
                                                                          "CODIGO_SERIE_TRD", "NOMBRE_SERIE_TRD", "CODIGO_SUB_SERIE_TRD",
                                                                          "NOMBRE_SUBSERIE_TRD", "COMPOSICION_EXPEDIENTE", "FECHA_INICIAL_EXPEDICION",
                                                                          "FECHA_FINAL_TERMINACION", "RANGO_EXTREMO_INICIAL",
                                                                          "RANGO_EXTREMO_FINAL", "ESTADO_EXPEDIENTE",
                                                                          "NOMBRE_SOLICITANTE", "IDENTIFICACION_PERSONA_EXPEDIENTE",
                                                                          "NOMBRE_RESPONSABLE_EXPEDIENTE", "IDENTIFICACION_SOLICITANTE",
                                                                          "NOMBRE_FONDO", "NOMBRE_CICLO_ARCHIVO", "FOLIO_FISICO",
                                                                          "FOLIO_ELECTRONICO", "FOLIO_DIGITALIZADO"}
                HttpContext.Current.Session.Item("SortExpression_expe_clasificacion") = colum_order_name
                HttpContext.Current.Session.Item("SortDirection_expe_clasificacion") = order_colum
                HttpContext.Current.Session.Item("GA_TIPO_CONSULTA_expe_clasificacion") = tipo_consulta
                HttpContext.Current.Session.Item("GA_DATO_CONSULTA_expe_clasificacion") = sql_consulta
                Dim Refclas As New ClassGredview
                Result = Refclas.add_clase_acender_decender(colum_order_name,
                                                            HttpContext.Current.Session.Item("Sort_matri_colum_expe_clasificacion"),
                                                            order_colum,
                                                            grediview)
                If Result <> "YES" Then
                    Listar_expedientes_agregados_volumen = "Error add clase funcion  Lista_solictudes_compartidos_de_un_usuario " & Result
                    Exit Function
                End If
                Listar_expedientes_agregados_volumen = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_expedientes_agregados_volumen = "Inconsistencia funcion Listar_expedientes_agregados " & ex.Message
        End Try
    End Function
    Function Solicita_datos_expediente_service_web(ByVal id_expediente As Integer,
                                                   ByRef stru_result As WebServiceGaExpediente.stru_result_expediente) As String
        Try
            Dim sql_consulta As String = "SELECT CONSECUTIVO_EXPEDIENTE_2,VOLUMEN_EXPEDIENTE,EXPEDIENTE_PADRE,ID_EXPEDIENTE," &
               "CODIGO_UNICO,NOMBRE_SERIE_TRD,NOMBRE_SUBSERIE_TRD,TEMA_EXPEDIENTE, " &
               "ASUNTO_EXPEDIENTE,FECHA_CREACION,CODIGO_AREA_TRD,NOMBRE_AREA_TRD," &
               "CODIGO_SERIE_TRD,CODIGO_SUB_SERIE_TRD,NOMBRE_TIPO_UNIDAD_DOCUMENTAL,COMPOSICION_EXPEDIENTE," &
               "FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL," &
               "RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,ESTADO_EXPEDIENTE,NOMBRE_PERSONA_EXPEDIENTE" &
               ",IDENTIFICACION_PERSONA_EXPEDIENTE," _
               & "NOMBRE_RESPONSABLE_EXPEDIENTE,IDENFICACION_RESPONSABLE_EXPEDIENTE" &
               ",NOMBRE_FONDO,NOMBRE_CICLO_ARCHIVO,NUMERO_FOLIOS_CONTENIDOS," &
               "NUMERO_ELECTRONICO_CONTENIDO,NUMERO_DIGITALIZADO_CONTENIDO from expediente_archivo " &
               " where ID_EXPEDIENTE=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(sql_consulta,
                                                              Datset)
            If Result <> "YES" Then
                Solicita_datos_expediente_service_web = "Error funcion Solicita_datos_expediente_service_web  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_expediente_service_web = "No se pudo encontrar el expediente  (" & id_expediente & ")"
                Exit Function
            Else

                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    stru_result.CONSECUTIVO_EXPEDIENTE_2 = ""
                Else
                    stru_result.CONSECUTIVO_EXPEDIENTE_2 = Datset.Tables(0).Rows(0).Item(0).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    stru_result.VOLUMEN_EXPEDIENTE = ""
                Else
                    stru_result.VOLUMEN_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(1).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    stru_result.EXPEDIENTE_PADRE = ""
                Else
                    stru_result.EXPEDIENTE_PADRE = Datset.Tables(0).Rows(0).Item(2).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    stru_result.ID_EXPEDIENTE = ""
                Else
                    stru_result.ID_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(3).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(4) = True Then
                    stru_result.CODIGO_UNICO = ""
                Else
                    stru_result.CODIGO_UNICO = Datset.Tables(0).Rows(0).Item(4).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(5) = True Then
                    stru_result.NOMBRE_SERIE_TRD = ""
                Else
                    stru_result.NOMBRE_SERIE_TRD = Datset.Tables(0).Rows(0).Item(5).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(6) = True Then
                    stru_result.NOMBRE_SUBSERIE_TRD = ""
                Else
                    stru_result.NOMBRE_SUBSERIE_TRD = Datset.Tables(0).Rows(0).Item(6).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(7) = True Then
                    stru_result.TEMA = ""
                Else
                    stru_result.TEMA = Datset.Tables(0).Rows(0).Item(7).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(8) = True Then
                    stru_result.ASUNTO = ""
                Else
                    stru_result.ASUNTO = Datset.Tables(0).Rows(0).Item(8).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(9) = True Then
                    stru_result.FECHA_CREACION = ""
                Else
                    stru_result.FECHA_CREACION = Datset.Tables(0).Rows(0).Item(9).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(10) = True Then
                    stru_result.CODIGO_AREA_TRD = ""
                Else
                    stru_result.CODIGO_AREA_TRD = Datset.Tables(0).Rows(0).Item(10).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(11) = True Then
                    stru_result.NOMBRE_AREA_TRD = ""
                Else
                    stru_result.NOMBRE_AREA_TRD = Datset.Tables(0).Rows(0).Item(11).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(12) = True Then
                    stru_result.CODIGO_SERIE_TRD = ""
                Else
                    stru_result.CODIGO_SERIE_TRD = Datset.Tables(0).Rows(0).Item(12).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(13) = True Then
                    stru_result.CODIGO_SUB_SERIE_TRD = ""
                Else
                    stru_result.CODIGO_SUB_SERIE_TRD = Datset.Tables(0).Rows(0).Item(13).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(14) = True Then
                    stru_result.TIPO_UNIDAD_DOCUMENTAL = ""
                Else
                    stru_result.TIPO_UNIDAD_DOCUMENTAL = Datset.Tables(0).Rows(0).Item(14).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(15) = True Then
                    stru_result.COMPOSICION_EXPEDIENTE = ""
                Else
                    stru_result.COMPOSICION_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(15).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(16) = True Then
                    stru_result.FECHA_INICIAL_EXPEDICION = ""
                Else
                    stru_result.FECHA_INICIAL_EXPEDICION = Datset.Tables(0).Rows(0).Item(16).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(17) = True Then
                    stru_result.FECHA_FINAL_TERMINACION = ""
                Else
                    stru_result.FECHA_FINAL_TERMINACION = Datset.Tables(0).Rows(0).Item(17).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(18) = True Then
                    stru_result.RANGO_EXTREMO_INICIAL = ""
                Else
                    stru_result.RANGO_EXTREMO_INICIAL = Datset.Tables(0).Rows(0).Item(18).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(19) = True Then
                    stru_result.RANGO_EXTREMO_FINAL = ""
                Else
                    stru_result.RANGO_EXTREMO_FINAL = Datset.Tables(0).Rows(0).Item(19).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(20) = True Then
                    stru_result.ESTADO_EXPEDIENTE = ""
                Else
                    stru_result.ESTADO_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(20).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(21) = True Then
                    stru_result.NOMBRE_SOLICITANTE = ""
                Else
                    stru_result.NOMBRE_SOLICITANTE = Datset.Tables(0).Rows(0).Item(21).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(22) = True Then
                    stru_result.IDENTIFICACION_SOLICITANTE = ""
                Else
                    stru_result.IDENTIFICACION_SOLICITANTE = Datset.Tables(0).Rows(0).Item(22).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(23) = True Then
                    stru_result.RESPONSABLE_EXPEDIENTE = ""
                Else
                    stru_result.RESPONSABLE_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(23).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(24) = True Then
                    stru_result.IDENFICACION_RESPONSABLE = ""
                Else
                    stru_result.IDENFICACION_RESPONSABLE = Datset.Tables(0).Rows(0).Item(24).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(25) = True Then
                    stru_result.NOMBRE_FONDO = ""
                Else
                    stru_result.NOMBRE_FONDO = Datset.Tables(0).Rows(0).Item(25).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(26) = True Then
                    stru_result.NOMBRE_CICLO_ARCHIVO = ""
                Else
                    stru_result.NOMBRE_CICLO_ARCHIVO = Datset.Tables(0).Rows(0).Item(26).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(27) = True Then
                    stru_result.FOLIO_FISICO = "0"
                Else
                    stru_result.FOLIO_FISICO = Datset.Tables(0).Rows(0).Item(27).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(28) = True Then
                    stru_result.FOLIO_ELECTRONICO = "0"
                Else
                    stru_result.FOLIO_ELECTRONICO = Datset.Tables(0).Rows(0).Item(28).ToString
                End If
                If Datset.Tables(0).Rows(0).IsNull(29) = True Then
                    stru_result.FOLIO_DIGITALIZADO = "0"
                Else
                    stru_result.FOLIO_DIGITALIZADO = Datset.Tables(0).Rows(0).Item(29).ToString
                End If
                Solicita_datos_expediente_service_web = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_expediente_service_web = "Inconsistencia general funcion Solicita_datos_expediente_service_web " & ex.Message
        End Try

    End Function
    Function Eliminar_Expediente(ByVal id_expediente As Integer,
                                 ByVal id_usuario_gestion As Integer,
                                 ByVal user_Gestion As String,
                                 ByVal iptrans As String,
                                 ByVal valida_estado_publico_producion As Integer,
                                 ByVal id_nivel_padre As Integer) As String
        Dim Result As String = ""
        Result = Verifica_existencia_expediente_control_produccion(id_expediente)
        If Result <> "YES" Then
            Eliminar_Expediente = Result
            Exit Function
        End If
        Dim Ref_class_relacion_exp As New Class_ra_relacion_expediente
        Dim existencia As String = ""
        Result = Ref_class_relacion_exp.Verfica_existencia_expediente_padre_volumen(id_expediente,
                                                                                    existencia)
        If Result <> "YES" Then
            Eliminar_Expediente = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Eliminar_Expediente = "El expediente tiene volúmenes relacionados imposible eliminar el expediente"
            Exit Function
        End If
        Result = Me.Verifica_existencia_relacion_expediente_producion(id_expediente,
                                                                      existencia)
        If Result <> "YES" Then
            Eliminar_Expediente = Result
            Exit Function
        End If
        If existencia = "YES" Then
            Eliminar_Expediente = "Imposible eliminar el expediente, tiene relación con otros expedientes en la producción documental"
            Exit Function
        End If
        '------------------------------------------------
        'Verifica expediente produción documental 
        'no se elimine desde el gestor de expedientes
        '------------------------------------------------
        Dim estado_expediente As Integer = 0
        Dim estado_publico As Integer = 0
        Result = Retorna_estado_expediente(id_expediente,
                                           estado_expediente,
                                           estado_publico)
        If Result <> "YES" Then
            Eliminar_Expediente = Result
            Exit Function
        End If
        If valida_estado_publico_producion = 1 Then
            If estado_publico = 2 Then
                Eliminar_Expediente = "Imposible eliminar el expediente, debido a que pertenece a la producción documental de otro usuario "
                Exit Function
            End If
        End If

        Dim existencia_relacion_hijo As String = "NO"
        Result = Me.Verifica_existencia_relacion_expediente_producion_hijo(id_expediente, existencia_relacion_hijo)
        If Result <> "YES" Then
            Eliminar_Expediente = Result
            Exit Function
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Eliminar_Expediente = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim consecutivo_exp_nivel As Integer = 0
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            Dim dat_reader As MySqlDataReader
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            If id_nivel_padre <> 0 Then
                Dim sqlforupdate_ = "Select conta_expediente  from ra_pro_niveles where id_nivel=" & id_nivel_padre & " for update "
                myCommand.CommandText = sqlforupdate_
                dat_reader = myCommand.ExecuteReader()
                If dat_reader Is Nothing Then
                    Eliminar_Expediente = "Imposible Encontrar el nivel del expediente error de conexión"
                    errorM = "Imposible Encontrar consecutivo de expedientes del nivel"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If dat_reader.HasRows = False Then
                    Eliminar_Expediente = "IImposible Encontrar consecutivo de expedientes del nivel"
                    errorM = "Imposible Encontrar consecutivo de expedientes del nivel"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If dat_reader.HasRows = True Then
                    dat_reader.Read()
                    consecutivo_exp_nivel = dat_reader.Item(0)
                    consecutivo_exp_nivel = consecutivo_exp_nivel - 1
                    dat_reader.Close()
                End If
            End If
            Dim Switc As Integer
            If id_nivel_padre <> 0 Then
                Dim sql_consecutivo_nivel As String = "Update ra_pro_niveles set conta_expediente=" & consecutivo_exp_nivel & "  where id_nivel=" & id_nivel_padre
                myCommand.CommandText = sql_consecutivo_nivel
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    errorM = "Imposible actualizar el consecutivo del nivel de expedientes "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim sql_insert_relacion As String = "delete from ra_pro_niveles_has_expediente_archivo where ra_pro_niveles_id_nivel=" & id_nivel_padre &
                    " and expediente_archivo_ID_EXPEDIENTE=" & id_expediente
                myCommand.CommandText = sql_insert_relacion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    errorM = "Imposible eliminar la relación con el nivel "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If

            End If
            Dim sqlinsertcion As String = "Delete from expediente_archivo where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Eliminar_Expediente = "Imposible eliminar expediente  : " & sqlinsertcion
                errorM = "Imposible eliminar expediente  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If
            '----------------------------------------------
            'Elimina relación nodo hijo
            '----------------------------------------------
            Dim Sql_delete As String = "Delete from ra_pro_relacion_exp_produccion where ID_EXPEDIENTE_HIJO=" & id_expediente
            If existencia_relacion_hijo = "YES" Then
                myCommand.CommandText = Sql_delete
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    errorM = "Imposible eliminar relación produción expediente  : " & Sql_delete
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO) values (" &
           "'ELIMINA EXPEDIENTE','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           id_expediente & ",'" & iptrans & "','" & hor & "','GESTOR DOCUMENTAL')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()
            Eliminar_Expediente = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Eliminar_Expediente = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Eliminar_Expediente = errorM

        End Try
    End Function
    Function Verifica_existencia_relacion_expediente_producion(ByVal id_expediente As Integer,
                                                               ByRef estado_relacion As String) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select ID_RA_PRO_RELACION_EXP_PRODUCCION from ra_pro_relacion_exp_produccion where ID_EXPEDIENTE_PADRE=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_relacion_exp_produccion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_relacion_expediente_producion = "Función Verifica_existencia_relacion_expediente_producion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_relacion = "NO"
                Verifica_existencia_relacion_expediente_producion = "YES"
                Exit Function
            Else
                estado_relacion = "YES"
                Verifica_existencia_relacion_expediente_producion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_relacion_expediente_producion = "Inconsistencia general función Verifica_existencia_relacion_expediente_producion " & ex.Message
        End Try
    End Function
    Function Verifica_existencia_relacion_expediente_producion_hijo(ByVal id_expediente As Integer,
                                                                    ByRef estado_relacion As String) As String
        Try
            Dim Result As String = ""
            Dim sql_consulta As String = "Select ID_RA_PRO_RELACION_EXP_PRODUCCION from ra_pro_relacion_exp_produccion where ID_EXPEDIENTE_HIJO=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_pro_relacion_exp_produccion")
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_relacion_expediente_producion_hijo = "Función Verifica_existencia_relacion_expediente_producion_hijo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_relacion = "NO"
                Verifica_existencia_relacion_expediente_producion_hijo = "YES"
                Exit Function
            Else
                estado_relacion = "YES"
                Verifica_existencia_relacion_expediente_producion_hijo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_existencia_relacion_expediente_producion_hijo = "Inconsistencia general función Verifica_existencia_relacion_expediente_producion_hijo " & ex.Message
        End Try
    End Function

    Function Genera_Codigo_corto_Expediente(ByRef codigo_unida_conservacion As String,
                                            ByVal id_empresa As Integer,
                                            ByVal id_usuario_gestion As Integer,
                                            ByVal consecutivo_unidad As Integer,
                                            ByVal año As String) As String

        Try
            codigo_unida_conservacion = año & "-" & id_empresa & "-" & id_usuario_gestion & "-" & consecutivo_unidad
            Genera_Codigo_corto_Expediente = "YES"
        Catch ex As Exception
            Genera_Codigo_corto_Expediente = "Iconsistencia general funcion " & ex.Message
        End Try
    End Function
    Function Genera_Codigo_largo_Unidad_Conservacion(ByRef codigo_unida_conservacion As String,
                                                     ByVal id_empresa As Integer,
                                                     ByVal consecutivo_unidad As Integer,
                                                     ByVal año As String,
                                                     ByVal disper As Integer) As String
        Try

            Dim refclas As New ClassUnidadConservacion
            Dim ref_id_empresa As String = id_empresa
            Dim Result As String = ""
            Result = refclas.zero_fill(ref_id_empresa, 2, "0")
            If Result <> "YES" Then
                Genera_Codigo_largo_Unidad_Conservacion = Result
                Exit Function
            End If
            Dim ref_consecutivo_unidad As String = consecutivo_unidad
            Result = refclas.zero_fill(ref_consecutivo_unidad, 8, "0")
            If Result <> "YES" Then
                Genera_Codigo_largo_Unidad_Conservacion = Result
                Exit Function
            End If
            codigo_unida_conservacion = año & ref_consecutivo_unidad
            Genera_Codigo_largo_Unidad_Conservacion = "YES"
        Catch ex As Exception
            Genera_Codigo_largo_Unidad_Conservacion = "Iconsistencia general funcion Genera_Codigo_largo_Unidad_Conservacion " & ex.Message
        End Try
    End Function
    Function Lista_ayuda_aplicacion(ByVal nombre_ayuda As String) As String
        '**************************************************
        'Funcion : Retorna ayuda sistema
        'Fecha : 2017-08-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '*************************************************
        Try
            Dim sqlconsulta As String = "Select CONTENIDO_AYUDA from  ra_ayuda_sistema  " &
                          " where NOMBRE_AYUDA='" & nombre_ayuda & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_de_tipos_ciclos_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Lista_ayuda_aplicacion = "Función Lista_ayuda_aplicacion Error de  conexión " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Lista_ayuda_aplicacion = Datset.Tables(0).Rows(0).Item(0)
                Exit Function
            Else
                Lista_ayuda_aplicacion = "Ayuda no encontrada"
                Exit Function
            End If
        Catch ex As Exception
            Lista_ayuda_aplicacion = "Inconsistencia función Lista_ayuda_aplicacion " & ex.Message
        End Try
    End Function


    Function Listar_ciclos_archivo(ByRef refcombo As DropDownList,
                                   ByVal nombre_ciclo As String,
                                   ByVal valor_inicial As String) As String
        '**************************************************
        'Funcion : Lista ciclos de archivo
        'Fecha : 2017-02-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************
        Try
            Dim sqlconsulta As String = "Select Nombre_Tipo_ciclo_archivo from ra_de_tipos_ciclos_archivo  "

            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_de_tipos_ciclos_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Listar_ciclos_archivo = "Función Listar_ciclos_archivo Error de  conexión " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Clear()
                refcombo.Items.Add(valor_inicial)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                If nombre_ciclo <> "" Then
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        If UCase(nombre_ciclo) = UCase(Datset.Tables(0).Rows(i).Item(0)) Then
                            refcombo.Text = Datset.Tables(0).Rows(i).Item(0)
                            Exit For
                        End If
                    Next
                End If
                Listar_ciclos_archivo = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                Listar_ciclos_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_ciclos_archivo = "Inconsistencia general función Listar_ciclos_archivo " & ex.Message
        End Try
    End Function
    Function Listar_ciclos_archivo(ByRef refcombo As DropDownList,
                                   ByVal nombre_ciclo As String) As String
        '**************************************************
        'Funcion : Lista ciclos de archivo
        'Fecha : 2017-02-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************
        Try
            Dim sqlconsulta As String = "Select Nombre_Tipo_ciclo_archivo from ra_de_tipos_ciclos_archivo  "

            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_de_tipos_ciclos_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Listar_ciclos_archivo = "Función Listar_ciclos_archivo Error de  conexión " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Clear()
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                If nombre_ciclo <> "" Then
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        If UCase(nombre_ciclo) = UCase(Datset.Tables(0).Rows(i).Item(0)) Then
                            refcombo.Text = Datset.Tables(0).Rows(i).Item(0)
                            Exit For
                        End If
                    Next
                End If
                Listar_ciclos_archivo = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                Listar_ciclos_archivo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_ciclos_archivo = "Inconsistencia general función Listar_ciclos_archivo " & ex.Message
        End Try
    End Function
    Function Listar_fodos_documentales(ByRef refcombo As DropDownList,
                                       ByVal nombre_fondo As String) As String
        '**************************************************
        'Funcion : Lista los fondos documentales
        'Fecha : 2017-02-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************
        Try
            Dim sqlconsulta As String = "Select Nombre_fondo_documental from ra_de_fondo_documental  " &
                        " where estado_ciclo_archivo=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_de_fondo_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Listar_fodos_documentales = "Función Listar_ciclos_archivo Error de  conexión " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Clear()
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                If nombre_fondo <> "" Then
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        If UCase(nombre_fondo) = UCase(Datset.Tables(0).Rows(i).Item(0)) Then
                            refcombo.Text = Datset.Tables(0).Rows(i).Item(0)
                            Exit For
                        End If
                    Next
                End If
                Listar_fodos_documentales = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                Listar_fodos_documentales = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_fodos_documentales = "Inconsistencia general función Listar_ciclos_archivo " & ex.Message
        End Try
    End Function
    Function Listar_fodos_documentales(ByRef refcombo As DropDownList,
                                       ByVal nombre_fondo As String,
                                       ByVal Valor_inicial As String) As String
        '**************************************************
        'Funcion : Lista los fondos documentales
        'Fecha : 2017-02-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '**************************************************
        Try
            Dim sqlconsulta As String = "Select Nombre_fondo_documental from ra_de_fondo_documental  " &
                        " where estado_ciclo_archivo=1"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_de_fondo_documental")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Listar_fodos_documentales = "Función Listar_ciclos_archivo Error de  conexión " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                refcombo.Items.Clear()
                refcombo.Items.Add(Valor_inicial)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    refcombo.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                If nombre_fondo <> "" Then
                    For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                        If UCase(nombre_fondo) = UCase(Datset.Tables(0).Rows(i).Item(0)) Then
                            refcombo.Text = Datset.Tables(0).Rows(i).Item(0)
                            Exit For
                        End If
                    Next
                End If
                Listar_fodos_documentales = "YES"
                Exit Function
            Else
                refcombo.Items.Clear()
                Listar_fodos_documentales = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Listar_fodos_documentales = "Inconsistencia general función Listar_ciclos_archivo " & ex.Message
        End Try
    End Function

    Function Retorna_estado_expediente(ByVal id_expediente As Integer,
                                       ByRef estado_expediente As Integer,
                                       ByRef estado_publico As Integer) As String
        '**********************************************************
        'Funcion : Retorna estado expediente, con el parametro
        'id expediente
        'Fecha :  2015-01-08
        'Ing : Miguel Angel Urueta Miranda
        'Modificación función conector base de datos para web
        'fecha 2015-04-21 ingeniero Miguel Angel Urueta
        '**********************************************************
        Try
            Dim SqlConsulta As String = "select ESTADO_EXPEDIENTE,Estado_Publico_Sub_Expediente from  expediente_archivo " &
                                      " where ID_EXPEDIENTE=" & id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_expediente = "Fución Retorna_estado_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                estado_expediente = Datset.Tables(0).Rows(0).Item(0)
                estado_publico = Datset.Tables(0).Rows(0).Item(1)
                Retorna_estado_expediente = "YES"
                Exit Function
            Else
                Retorna_estado_expediente = "Función Retorna_estado_expediente dice imposible encontrar el estado del expediente "
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estado_expediente = "Inconsistencia función Retorna_estado_expediente " & ex.Message
        End Try
    End Function

    Function Retorna_tipo_id_expediente_por_id(ByRef id_tipo_expediente As Integer,
                                                ByVal id_expediente As String) As String
        '******************************************************
        'Funcion : Retorna el id del tipo expediente enviando
        'como parametro el nombre del expediente
        'Fecha : 2015-06-24
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************
        Try
            Dim sqlconsulta As String = "Select RA_TIP_EXPE_ID_TIPO_EXPEDIENTE from expediente_archivo where " &
               "  ID_EXPEDIENTE='" & id_expediente & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_tipo_id_expediente_por_id = "Error listando datos " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                id_tipo_expediente = Datset.Tables(0).Rows(0).Item(0)
                Retorna_tipo_id_expediente_por_id = "YES"
                Exit Function
            Else
                Retorna_tipo_id_expediente_por_id = "Imposible encontrar la identificacion del tipo de expediente"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_tipo_id_expediente_por_id = "Inconsistencia funcion Retorna_tipo_id_expediente_por_id " & ex.Message
        End Try
    End Function
    Function Registrar_Expediente_Volumen(ByVal id_usuario_gestion As Integer,
         ByVal codigo_unico As String, ByVal estado_codigo_unico As Integer, ByVal id_empresa As Integer,
         ByVal fecha_extrema_incial As String, ByVal fecha_extrema_final As String,
         ByVal rango_extremo_inicial As String, ByVal rango_extremo_final As String,
         ByVal tema_unidad_conservacion As String,
         ByVal nombre_organigrama As String, ByVal nombre_area As String,
         ByVal nombre_serie As String, ByVal nombre_sub_serie As String, ByVal id_tipo_expediente As Integer,
         ByVal numero_documento_digitalizado As Integer, ByVal numero_folios_fisicos As Integer,
         ByVal numero_documentos_electronicos As Integer, ByVal asunto_expediente As String,
         ByVal id_expediente_padre As Integer,
         ByVal codigo_corto_unidad As String, ByVal nombre_tipo_unidad_conservacion As String, ByRef id_expediente As Integer, ByRef observacion As String,
         ByVal nombre_tipo_unidad_documental As String, ByVal nombre_sub_seccion As String, ByVal opcio_requiere_unidad_contenedora As Integer,
         ByVal tipo_unidad_contendora As String, ByVal id_unidad_contendora As Integer, ByVal requiere_unidad_fisica As Integer,
         ByVal nombre_ciclo_documental As String, ByVal nombre_fondo_documental As String, ByVal nombre_persona_expediente As String,
         ByVal indentificacion_persona_expediente As String, ByVal nombre_responsable As String, ByVal identificacion_responsable As String,
         ByVal id_instrumento As Object) As String
        Dim Result As String = "YES"
        '------------------------------------------------
        'Verifica expediente produción documental 
        'no se elimine desde el gestor de expedientes
        '------------------------------------------------
        numero_documento_digitalizado = 0
        numero_documentos_electronicos = 0
        Dim estado_expediente As Integer = 0
        Dim estado_publico As Integer = 0
        Result = Retorna_estado_expediente(id_expediente_padre,
                                           estado_expediente,
                                           estado_publico)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        If estado_publico = 2 Then
            Registrar_Expediente_Volumen = "Imposible registrar volumen del expediente, debido a que pertenece a la producción documental de otro usuario "
            Exit Function
        End If

        If fecha_extrema_incial = "" Then
            Registrar_Expediente_Volumen = "Por favor seleccione la fecha inicial de la unidad documental"
            Exit Function
        End If
        If nombre_tipo_unidad_documental = "" Then
            Registrar_Expediente_Volumen = "Por favor seleccione el tipo de unidad documental"
            Exit Function
        End If
        If nombre_organigrama = "" Then
            Registrar_Expediente_Volumen = "Por favor seleccione el organigrama"
            Exit Function
        End If
        If nombre_area = "" Then
            Registrar_Expediente_Volumen = "Por favor seleccione el área"
            Exit Function
        End If
        If id_tipo_expediente = 0 Then
            Registrar_Expediente_Volumen = "Por favor seleccione el tipo de expediente"
            Exit Function
        End If
        If id_instrumento <> 0 Then
            If nombre_ciclo_documental = "" Then
                Registrar_Expediente_Volumen = "Por favor seleccione el nombre ciclo de archivo"
                Exit Function
            End If
            If nombre_serie = "" And nombre_sub_serie = "" Then
                Registrar_Expediente_Volumen = "Por favor seleccione la serie o la sub serie del expediente"
                Exit Function
            End If
        End If
        If requiere_unidad_fisica = 1 Then
            If nombre_tipo_unidad_conservacion = "" Then
                Registrar_Expediente_Volumen = "Debe informar el tipo de unidad conservación"
                Exit Function
            End If
        End If
        If opcio_requiere_unidad_contenedora = 1 Then
            If id_unidad_contendora = 0 Then
                Registrar_Expediente_Volumen = "Debe seleccionar una unidad contendora para la unidad documental"
                Exit Function
            End If
            If tipo_unidad_contendora = "" Then
                Registrar_Expediente_Volumen = "Debe seleccionar el tipo de unidad contendora para la unidad documental"
                Exit Function
            End If
        End If
        '-------------------------------------------------
        'Validación longitud de caracteres  
        '-------------------------------------------------
        If Len(codigo_unico) > 45 Then
            Registrar_Expediente_Volumen = "El campo (consecutivo unidad) supera el número de caracteres permitidos (45 caracteres)"
            Exit Function
        End If
        'If Len(tema_unidad_conservacion) > 120 Then
        '    Registrar_Expediente_Volumen = "El campo (tema expediente) supera el número de caracteres permitidos (120 caracteres)"
        '    Exit Function
        'End If
        If Len(indentificacion_persona_expediente) > 60 Then
            Registrar_Expediente_Volumen = "El campo (identificacion solicitante) supera el número de caracteres permitidos (60 caracteres)"
            Exit Function
        End If
        If Len(identificacion_responsable) > 60 Then
            Registrar_Expediente_Volumen = "El campo (identificacion responsable) supera el número de caracteres permitidos (60 caracteres)"
            Exit Function
        End If
        If Len(rango_extremo_inicial) > 45 Then
            Registrar_Expediente_Volumen = "El campo (rango extremo inicial) supera el número de caracteres permitidos (45 caracteres)"
            Exit Function
        End If
        If Len(rango_extremo_final) > 45 Then
            Registrar_Expediente_Volumen = "El campo (rango extremo final) supera el número de caracteres permitidos (45 caracteres)"
            Exit Function
        End If
        'If Len(nombre_persona_expediente) > 60 Then
        '    Registrar_Expediente_Volumen = "El campo (nombre solicitante) supera el número de caracteres permitidos (60 caracteres)"
        '    Exit Function
        'End If
        'If Len(nombre_responsable) > 60 Then
        '    Registrar_Expediente_Volumen = "El campo (nombre responsable) supera el número de caracteres permitidos (60 caracteres)"
        '    Exit Function
        'End If

        Dim re_nombre_sub_seccion As String = "Null"
        Dim id_ciclo_documental As Integer = 0
        Dim id_fondo_documental As Integer = 0
        Dim ref_Class_ra_de_fondo_documental As New Class_ra_de_fondo_documental
        If nombre_fondo_documental <> "" Then
            Result = ref_Class_ra_de_fondo_documental.Retorna_id_fondo_documental_nombre(nombre_fondo_documental,
                                                                                         id_fondo_documental)
            If Result <> "YES" Then
                Registrar_Expediente_Volumen = Result
                Exit Function
            End If
        End If
        Dim ref_Class_ra_de_tipos_ciclos_archivo As New Class_ra_de_tipos_ciclos_archivo
        If nombre_ciclo_documental <> "" Then
            Result = ref_Class_ra_de_tipos_ciclos_archivo.Retorna_id_ciclo_archivo_nombre(nombre_ciclo_documental,
                                                                                          id_ciclo_documental)
            If Result <> "YES" Then
                Registrar_Expediente_Volumen = Result
                Exit Function
            End If
        End If
        '-----------------------------------------------------
        'Especifica el ciclo de archivo y el fondo documental
        '-----------------------------------------------------
        Dim ref_id_ciclo_documental As Object = Nothing
        Dim ref_id_fondo_documental As Object = Nothing
        Dim ref_nombre_fondo_documental As String = ""
        Dim ref_nombre_ciclo_documental As String = ""
        If id_ciclo_documental = 0 Then
            ref_id_ciclo_documental = "Null"
        Else
            ref_id_ciclo_documental = id_ciclo_documental
        End If
        If id_fondo_documental = 0 Then
            ref_id_fondo_documental = "Null"
        Else
            ref_id_fondo_documental = id_fondo_documental
        End If
        If nombre_ciclo_documental <> "" Then
            ref_nombre_ciclo_documental = "'" & nombre_ciclo_documental & "'"
        Else
            ref_nombre_ciclo_documental = "Null"
        End If
        If nombre_fondo_documental <> "" Then
            ref_nombre_fondo_documental = "'" & nombre_fondo_documental & "'"
        Else
            ref_nombre_fondo_documental = "Null"
        End If
        Dim ref_nombre_persona_expediente As String = ""
        Dim ref_indentificacion_persona_expediente As String = ""
        If nombre_persona_expediente = "" Then
            ref_nombre_persona_expediente = "Null"
        Else
            ref_nombre_persona_expediente = "'" & nombre_persona_expediente & "'"
        End If
        If indentificacion_persona_expediente = "" Then
            ref_indentificacion_persona_expediente = "Null"
        Else
            ref_indentificacion_persona_expediente = "'" & indentificacion_persona_expediente & "'"
        End If
        Dim ref_nombre_responsable As String = ""
        If nombre_responsable = "" Then
            ref_nombre_responsable = "Null"
        Else
            ref_nombre_responsable = "'" & nombre_responsable & "'"
        End If
        Dim ref_identificacion_responsable As String = ""
        If identificacion_responsable = "" Then
            ref_identificacion_responsable = "Null"
        Else
            ref_identificacion_responsable = "'" & identificacion_responsable & "'"
        End If
        Dim Refclas As New ClassGestionDocumental
        Dim id_organigrama As Integer = 0
        Dim Reclas_registro_organigrama As New Class_registro_organigrama
        Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama,
                                                                   id_empresa,
                                                                   id_organigrama)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Dim codigo_area As Integer = 0
        Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
        Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama,
                                                                                 codigo_area,
                                                                                 nombre_area)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        '------------------------------------------
        'Retorna id tipo unidad documental
        '------------------------------------------
        Dim id_tipo_unidad_documental As Integer = 0
        Dim refclas_unidad_conservacion As New ClassUnidadConservacion
        Dim ref_Class_tipo_unidad_documental As New Class_tipo_unidad_documental
        Result = ref_Class_tipo_unidad_documental.Retorna_id_tipo_unidad_documental_por_nombre(nombre_tipo_unidad_documental,
                                                                                               id_tipo_unidad_documental)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        '---------------------------------------------
        'Retorna codigo subseccion
        '---------------------------------------------
        Dim id_sub_seccion As Object = 0
        If id_sub_seccion = 0 Then
            id_sub_seccion = "null"
        End If
        Dim id_serie As Object = 0
        Dim consecutivo_serie As Integer = 0
        Dim consecutivo_Sub_serie As Integer = 0
        Dim ref_Class_series_documentales As New Class_series_documentales
        If nombre_serie <> "" Then
            Result = ref_Class_series_documentales.Retorna_Id_serie_instrumento_Documental(codigo_area.ToString,
                                                                                           nombre_serie,
                                                                                           id_instrumento,
                                                                                           id_serie,
                                                                                           consecutivo_serie,
                                                                                           consecutivo_Sub_serie)
            If Result <> "YES" Then
                Registrar_Expediente_Volumen = Result
                Exit Function
            End If
        End If
        Dim id_consecutivo_doc As Integer = 0
        Dim id_sub_serie As Object = 0
        Dim ref_Class_subseries_documentales As New Class_subseries_documentales
        If nombre_sub_serie <> "" Then
            Result = ref_Class_subseries_documentales.Retorna_Id_Subserie_Consecutivo_TipDoc(nombre_sub_serie,
                                                                                             id_serie,
                                                                                             id_sub_serie,
                                                                                             id_consecutivo_doc)
            If Result <> "YES" Then
                Registrar_Expediente_Volumen = Result
                Exit Function
            End If
        End If
        Dim id_tipo_unidad_conservacion As Integer = 0
        Dim ref_Class_tipo_unidad_conservacion As New Class_tipo_unidad_conservacion
        Result = ref_Class_tipo_unidad_conservacion.Retorna_id_tipo_unidad_conservacion_expediente(nombre_tipo_unidad_conservacion,
                                                                                                   id_tipo_unidad_conservacion,
                                                                                                   2)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Dim ref_id_tipo_unidad_conservacion As Object = "null"
        If id_tipo_unidad_conservacion = -1 Or id_tipo_unidad_conservacion = 0 Then
            ref_id_tipo_unidad_conservacion = "null"
        Else
            ref_id_tipo_unidad_conservacion = id_tipo_unidad_conservacion
        End If
        Dim ref_nombre_tipo_unidad_conservacion As String = "null"
        If nombre_tipo_unidad_conservacion = "" Then
            ref_nombre_tipo_unidad_conservacion = "null"
        Else
            ref_nombre_tipo_unidad_conservacion = "'" & nombre_tipo_unidad_conservacion & "'"
        End If
        Dim ref_nombre_serie As String = "null"
        If nombre_serie = "" Then
            ref_nombre_serie = "null"
        Else
            ref_nombre_serie = "'" & nombre_serie & "'"
        End If
        Dim ref_nombre_sub_serie As String = "null"
        If nombre_sub_serie = "" Then
            ref_nombre_sub_serie = "null"
        Else
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        Dim fecha_ret_gestion As String = ""
        Dim fecha_ret_central As String = ""
        Dim id_tipo_instrumento As Integer = 0
        Dim Refclas_gagestioninstrumento As New ClassGaGestionInstrumento
        Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
        If id_instrumento <> 0 Then
            Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(id_instrumento,
                                                                               id_tipo_instrumento)
            If Result <> "YES" Then
                Registrar_Expediente_Volumen = Result
                Exit Function
            End If
            If id_tipo_instrumento = 1 Then
                Result = Me.Retorna_tiempos_de_retencion_tablas_retencion(id_serie, id_sub_serie, fecha_extrema_incial,
                                                                          fecha_ret_gestion, fecha_ret_central)
                If Result <> "YES" Then
                    Registrar_Expediente_Volumen = Result
                    Exit Function
                End If
                If fecha_ret_gestion = "" Then
                    Registrar_Expediente_Volumen = "La serie o sub serie no registran tiempos de retención "
                    Exit Function
                End If
                If fecha_ret_central = "" Then
                    fecha_ret_central = "Null"
                Else
                    fecha_ret_central = "'" & fecha_ret_central & "'"
                End If
                fecha_ret_gestion = "'" & fecha_ret_gestion & "'"
            End If
            If id_tipo_instrumento = 2 Then
                Result = Me.Retorna_tiempos_de_retencion_tablas_de_valoracion(id_serie, id_sub_serie, fecha_extrema_incial,
                                                                              fecha_ret_central)
                If Result <> "YES" Then
                    Registrar_Expediente_Volumen = Result
                    Exit Function
                End If
                If fecha_ret_central = "" Then
                    Registrar_Expediente_Volumen = "La serie o sub serie no registran tiempos de retención "
                    Exit Function
                End If
                fecha_ret_central = "'" & fecha_ret_central & "'"
                fecha_ret_gestion = "Null"
            End If
        Else
            fecha_ret_gestion = "Null"
            fecha_ret_central = "Null"
        End If
        If id_serie = 0 Then
            id_serie = "null"
        End If
        If id_sub_serie = 0 Then
            id_sub_serie = "null"
        End If


        '---------------------------------------
        'Vefica formato fechas extremas
        '---------------------------------------
        Dim re_fecha_extrema_incial As String = ""
        Dim re_fecha_extrema_final As String = ""
        If fecha_extrema_incial <> "" Then
            'Dim splifecha() As String = fecha_extrema_incial.Split("/")
            re_fecha_extrema_incial = "'" & fecha_extrema_incial & "'"
        Else
            re_fecha_extrema_incial = "null"
        End If
        If fecha_extrema_final <> "" Then
            'Dim splifecha() As String = fecha_extrema_final.Split("/")
            re_fecha_extrema_final = "'" & fecha_extrema_final & "'"
        Else
            re_fecha_extrema_final = "null"
        End If

        If rango_extremo_inicial = "" Then
            rango_extremo_inicial = "null"
        Else
            rango_extremo_inicial = "'" & rango_extremo_inicial & "'"
        End If

        If rango_extremo_final = "" Then
            rango_extremo_final = "null"
        Else
            rango_extremo_final = "'" & rango_extremo_final & "'"
        End If

        If tema_unidad_conservacion = "" Then
            tema_unidad_conservacion = "null"
        Else
            tema_unidad_conservacion = "'" & tema_unidad_conservacion & "'"
        End If
        Dim ref_asunto_expediente As String = ""
        If asunto_expediente = "" Then
            ref_asunto_expediente = "null"
        Else
            ref_asunto_expediente = "'" & asunto_expediente & "'"
        End If
        Dim ref_observacion As String = ""
        If observacion = "" Then
            ref_observacion = "null"
        Else
            ref_observacion = "'" & observacion & "'"
        End If
        Dim ref_id_unidad_contendora As Object = "null"
        Dim ref_id_entrepaño As Object = "null"
        Dim ref_estado_archivado_expediente As Integer = 0
        If id_unidad_contendora <> 0 Then
            If requiere_unidad_fisica <> 1 Then
                Registrar_Expediente_Volumen = "La unidad documental no requiere de unidad contenedora física"
                Exit Function
            End If
            If tipo_unidad_contendora = "Entrepaño" Then
                ref_id_entrepaño = id_unidad_contendora
            Else
                ref_id_unidad_contendora = id_unidad_contendora
            End If
            ref_estado_archivado_expediente = 1
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Dim date_registro As String = Date.Today
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_registro)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        If id_instrumento = 0 Then
            id_instrumento = "Null"
        End If
        '---------------------------------------
        'Actualización expediente electrónico
        '---------------------------------------
        Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
        Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
        Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Dim disco_carpeta As String = stru_ruta_expediente_.DISCO
        Dim class_zerro_fill As New Class_zero_fill
        Result = class_zerro_fill.zero_fill(disco_carpeta, 9, "0")
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
        If Directory.Exists(Ruta_expediente) = False Then
            Registrar_Expediente_Volumen = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
            Exit Function
        End If
        Ruta_expediente = Ruta_expediente & disco_carpeta
        If Directory.Exists(Ruta_expediente) = False Then
            Directory.CreateDirectory(Ruta_expediente)
        End If
        Dim id_sistema_meta_dato As Integer = 0
        Dim tipo_meta_dato As Integer = 2
        Dim stru_detalle_item_meta_dato() As stru_detalle_sis_meta_dato = Nothing
        Dim Ref_clas_ra_m_sistema_meta_datos As New Class_ra_m_sistema_meta_datos
        Dim Ref_ra_m_detalle_sis_meta_datos As New Class_ra_m_detalle_sis_meta_datos
        Result = Ref_clas_ra_m_sistema_meta_datos.Solicita_identificacion_sistema_meta_dato_default_archivo(tipo_meta_dato,
                                                                                                            id_sistema_meta_dato)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Result = Ref_ra_m_detalle_sis_meta_datos.Solicita_estructura_meta_dato_sistema_stru(id_sistema_meta_dato,
                                                                                            stru_detalle_item_meta_dato)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Dim Ref_class_remit_dest_interno As New Class_remit_dest_interno
        Dim cargo_usuario_gestion As String = ""
        Dim nombre_usuario_gestion As String = ""
        Dim correo_electronico As String = ""
        Result = Ref_class_remit_dest_interno.Retorna_datos_caracterizacion_usuario_gestion(id_usuario_gestion,
                                                                                            nombre_usuario_gestion,
                                                                                            cargo_usuario_gestion,
                                                                                            correo_electronico)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Dim Nombre_tipo_expediente As String = ""
        Dim Ref_class_tipo_expdiente As New Class_ra_tipo_expediente
        Result = Ref_class_tipo_expdiente.Solicita_nombre_tipo_expediente(id_tipo_expediente,
                                                                          Nombre_tipo_expediente)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        Dim nombre_empresa As String = ""
        Dim Ref_class_empresa As New Class_empresa_gestion_documental
        Result = Ref_class_empresa.Solicita_nombre_identificacion_empresa("",
                                                                         nombre_empresa)
        If Result <> "YES" Then
            Registrar_Expediente_Volumen = Result
            Exit Function
        End If
        For i As Integer = 0 To stru_detalle_item_meta_dato.Length - 1
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Fecha_" Then
                stru_detalle_item_meta_dato(i).value = date1al.Replace("/", "-")
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Autor_" Then
                stru_detalle_item_meta_dato(i).value = nombre_usuario_gestion & "(" & cargo_usuario_gestion & ") (" & nombre_empresa & ")"
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "asunto_" Then
                stru_detalle_item_meta_dato(i).value = asunto_expediente
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "nombre_serie" Then
                stru_detalle_item_meta_dato(i).value = nombre_serie
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "codigo_serie" Then
                stru_detalle_item_meta_dato(i).value = id_serie
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "nombre_sub_serie" Then
                stru_detalle_item_meta_dato(i).value = nombre_sub_serie
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "codigo_sub_serie" Then
                stru_detalle_item_meta_dato(i).value = id_sub_serie
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Fecha_apertura" Then
                stru_detalle_item_meta_dato(i).value = date_registro
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "codigo_area_" Then
                stru_detalle_item_meta_dato(i).value = codigo_area
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Nombre_area_" Then
                stru_detalle_item_meta_dato(i).value = nombre_area
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Nombre_tipo_expediente" Then
                stru_detalle_item_meta_dato(i).value = Nombre_tipo_expediente
            End If
        Next
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad_volumen As String = 0
        Dim errorM As String = "YES"
        Try

            Dim sqlforupdate As String = "Select  CONSECUTIVO_EXPEDIENTE_2 from expediente_archivo  where ID_EXPEDIENTE=" &
            id_expediente_padre & " for update "
            'myConnection.Open()
            Dim dat_reader As MySqlDataReader
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            dat_reader = myCommand.ExecuteReader()
            If dat_reader Is Nothing Then
                Registrar_Expediente_Volumen = "Imposible Encontrar consecutivo expediente error de conexion"
                errorM = "Imposible Encontrar consecutivo expediente error de conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = False Then
                Registrar_Expediente_Volumen = "Imposible Encontrar consecutivo expediente"
                errorM = "Imposible Encontrar consecutivo expediente"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = True Then
                dat_reader.Read()
                consecutivo_unidad_volumen = dat_reader.Item(0)
                dat_reader.Close()
            End If
            consecutivo_unidad_volumen = consecutivo_unidad_volumen + 1
            Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
            '--------------------------------------------
            'Agregar valores insertcion
            '--------------------------------------------
            Dim sqlcampos_insert As String = "Insert into expediente_archivo (CONSECUTIVO_EXPEDIENTE," &
            "CODIGO_LARGO,CODIGO_UNICO,ID_USUARIO_GESTION,FECHA_CREACION," &
            "FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL,RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_EXPEDIENTE," &
            "TIPO_EXPEDIENTE,MODULO_PRODUCTOR,ID_EMPRESA_EXPEDIENTE,NUMERO_FOLIOS_CONTENIDOS,NOMBRE_AREA_TRD," &
            "CODIGO_AREA_TRD,NOMBRE_SERIE_TRD,CODIGO_SERIE_TRD,NOMBRE_SUBSERIE_TRD,CODIGO_SUB_SERIE_TRD" &
            ",ASUNTO_EXPEDIENTE,RA_TIP_EXPE_ID_TIPO_EXPEDIENTE,NUMERO_DIGITALIZADO_CONTENIDO," &
            "NUMERO_ELECTRONICO_CONTENIDO,EXPEDIENTE_PADRE,VOLUMEN_EXPEDIENTE,TIPO_UNIDAD_ID_TIPO,TIPO_UNIDAD_CONSERVACION,OBSERVACION_EXPEDIENTE,ID_SUB_AREA,NOMBRE_SUB_AREA" _
            & ",ID_TIPO_UNIDAD_DOCUMENTAL,NOMBRE_TIPO_UNIDAD_DOCUMENTAL,UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION,ENTRE_PAÑO_ID_ENTREPAÑO,ESTADO_ARCHIVO_EXPEDIENTE," _
            & "ID_FONDO,NOMBRE_FONDO,Id_tipos_ciclo_archivo,NOMBRE_CICLO_ARCHIVO,NOMBRE_PERSONA_EXPEDIENTE,IDENTIFICACION_PERSONA_EXPEDIENTE,NOMBRE_RESPONSABLE_EXPEDIENTE,IDENFICACION_RESPONSABLE_EXPEDIENTE" &
            ",fecha_ret_central,fecha_ret_gestion,id_instrumento,fecha_registro,estado_expediente_electronico,ID_DISCO) values "
            Dim sqlinsert_campos As String = "( 0" & ",'" & codigo_corto_unidad & "','" & codigo_unico & "'," &
             id_usuario_gestion & ",'" & date1al & "'," &
             re_fecha_extrema_incial & "," & re_fecha_extrema_final & "," & rango_extremo_inicial & "," & rango_extremo_final &
            "," & tema_unidad_conservacion & ",1," & "'DOCUARCHI-WEB'" & "," & id_empresa & "," & numero_folios_fisicos & ",'" & nombre_area &
            "'," & codigo_area & "," & ref_nombre_serie & "," & id_serie & "," & ref_nombre_sub_serie & "," & id_sub_serie &
             "," & ref_asunto_expediente & "," & id_tipo_expediente & "," & numero_documento_digitalizado & "," & numero_documentos_electronicos & "," &
             id_expediente_padre & "," & consecutivo_unidad_volumen & "," & ref_id_tipo_unidad_conservacion & "," & ref_nombre_tipo_unidad_conservacion & "," & ref_observacion & "," & id_sub_seccion & "," & re_nombre_sub_seccion &
            "," & id_tipo_unidad_documental & ",'" & nombre_tipo_unidad_documental & "'," & ref_id_unidad_contendora & "," & ref_id_entrepaño & "," & ref_estado_archivado_expediente & "," & ref_id_fondo_documental &
            "," & ref_nombre_fondo_documental & "," & ref_id_ciclo_documental & "," & ref_nombre_ciclo_documental & "," & ref_nombre_persona_expediente & "," _
            & ref_indentificacion_persona_expediente & "," & ref_nombre_responsable & "," & ref_identificacion_responsable &
            "," & fecha_ret_central & "," & fecha_ret_gestion & "," & id_instrumento & ",'" & date_registro & "'," & 1 & "," & stru_ruta_expediente_.DISCO & ")"
            Dim sqlinsertcion As String = sqlcampos_insert & sqlinsert_campos
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            Dim objet As Object = myCommand.LastInsertedId
            If Switc = 0 Then
                Registrar_Expediente_Volumen = "Imposible registrar expediente  : " & sqlinsertcion
                'myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar unidad de conservación  : " & sqlinsertcion
                Exit Function
            End If
            '--------------------------------------------------
            'Actualiza el consecutivo expediente padre
            '--------------------------------------------------
            Dim updatconsecutivo As String = "UPDATE expediente_archivo SET CONSECUTIVO_EXPEDIENTE_2=" &
            consecutivo_unidad_volumen & " where ID_EXPEDIENTE=" &
            id_expediente_padre
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '////Actualización expediente elctronico
            For i As Integer = 0 To stru_detalle_item_meta_dato.Length - 1
                If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Titulo_" Then
                    stru_detalle_item_meta_dato(i).value = codigo_unico
                End If
                If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Identicador_exp" Then
                    stru_detalle_item_meta_dato(i).value = id_expediente
                End If
                If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Titulo_exp" Then
                    stru_detalle_item_meta_dato(i).value = codigo_corto_unidad
                End If
                If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Palabra_clave_expediente" Then
                    stru_detalle_item_meta_dato(i).value = codigo_unico
                End If
            Next
            Dim expediente_zero_fil As String = id_expediente.ToString
            Result = class_zerro_fill.zero_fill(expediente_zero_fil, 9, "0")
            If Result <> "YES" Then
                errorM = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim Ruta_archivo_xml As String = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
            Result = Me.Crea_archivo_indice_xml_expediente(Ruta_archivo_xml,
                                                           id_expediente,
                                                           date1al.Replace("/", "-"),
                                                           nombre_usuario_gestion,
                                                           nombre_empresa,
                                                           Nombre_tipo_expediente)
            If Result <> "YES" Then
                errorM = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            '-------------------------------------------------
            'Ingresa la relación de expedientes
            '-------------------------------------------------
            Dim isertrelacion As String = "Insert Into ra_relacion_expediente (ID_EXPEDIENTE_PADRE,ID_EXPDIENTE_HIJO) values " &
            "(" & id_expediente_padre & "," & objet & ")"
            myCommand.CommandText = isertrelacion
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible crear relación  : " & isertrelacion
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            id_expediente = objet
            myTrans.Commit()
            myConnection.Close()
            Registrar_Expediente_Volumen = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Registrar_Expediente_Volumen = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Registrar_Expediente_Volumen = errorM

        End Try
    End Function
    'Function Registrar_Expediente_Volumen(ByVal id_usuario_gestion As Integer, _
    '    ByVal codigo_unico As String, ByVal estado_codigo_unico As Integer, ByVal id_empresa As Integer, _
    '    ByVal fecha_extrema_incial As String, ByVal fecha_extrema_final As String, _
    '    ByVal rango_extremo_inicial As String, ByVal rango_extremo_final As String, _
    '      ByVal tema_unidad_conservacion As String, _
    '      ByVal nombre_organigrama As String, ByVal nombre_area As String, _
    '     ByVal nombre_serie As String, ByVal nombre_sub_serie As String, ByVal id_tipo_expediente As Integer, _
    '     ByVal numero_documento_digitalizado As Integer, ByVal numero_folios_fisicos As Integer, _
    '     ByVal numero_documentos_electronicos As Integer, ByVal asunto_expediente As String, _
    '     ByVal volumen_expediente As Integer, ByVal id_expediente_padre As Integer, _
    '     ByVal codigo_corto_unidad As String, ByVal nombre_tipo_unidad_conservacion As String, ByRef id_expediente As Integer, ByRef observacion As String, _
    '     ByVal nombre_tipo_unidad_documental As String, ByVal nombre_sub_seccion As String, ByVal opcio_requiere_unidad_contenedora As Integer, _
    '     ByVal tipo_unidad_contendora As String, ByVal id_unidad_contendora As Integer, ByVal requiere_unidad_fisica As Integer, _
    '     ByVal nombre_ciclo_documental As String, ByVal nombre_fondo_documental As String, ByVal nombre_persona_expediente As String, _
    '     ByVal indentificacion_persona_expediente As String, ByVal nombre_responsable As String, ByVal identificacion_responsable As String) As String
    '    Dim Result As String = "YES"
    '    '------------------------------------------------
    '    'Verifica expediente produción documental 
    '    'no se elimine desde el gestor de expedientes
    '    '------------------------------------------------
    '    'Dim estado_expediente As Integer = 0
    '    'Dim estado_publico As Integer = 0
    '    'Result = Retorna_estado_expediente(id_expediente_padre, estado_expediente, estado_publico)
    '    'If Result <> "YES" Then
    '    '    Registrar_Expediente_Volumen = Result
    '    '    Exit Function
    '    'End If

    '    'If estado_publico = 2 Then
    '    '    Registrar_Expediente_Volumen = "Imposible agregar volumen al expediente, debido a que pertenece a la producción documental de otro usuario "
    '    '    Exit Function
    '    'End If
    '    If opcio_requiere_unidad_contenedora = 1 Then
    '        If id_unidad_contendora = 0 Then
    '            Registrar_Expediente_Volumen = "Debe seleccionar una unidad contendora para la unidad documental"
    '            Exit Function
    '        End If
    '        If tipo_unidad_contendora = "" Then
    '            Registrar_Expediente_Volumen = "Debe seleccionar el tipo de unidad contendora para la unidad documental"
    '            Exit Function
    '        End If
    '    End If
    '    If nombre_tipo_unidad_documental = "" Then
    '        Registrar_Expediente_Volumen = "Por favor seleccione el tipo de unidad documental"
    '        Exit Function
    '    End If
    '    If id_tipo_expediente = 0 Then
    '        Registrar_Expediente_Volumen = "Por favor seleccione el tipo de expediente"
    '        Exit Function
    '    End If
    '    If nombre_organigrama = "" Then
    '        Registrar_Expediente_Volumen = "Por favor seleccione el organigrama"
    '        Exit Function
    '    End If
    '    If fecha_extrema_incial = "" Then
    '        Registrar_Expediente_Volumen = "Por favor seleccione la fecha inicial de la unidad documental"
    '        Exit Function
    '    End If
    '    If nombre_area = "" Then
    '        Registrar_Expediente_Volumen = "Por favor seleccione el área"
    '        Exit Function
    '    End If
    '    If nombre_ciclo_documental = "" Then
    '        Registrar_Expediente_Volumen = "Por favor seleccione el nombre ciclo de archivo"
    '        Exit Function
    '    End If
    '    If fecha_extrema_incial = "" Then
    '        Registrar_Expediente_Volumen = "Por favor informe la fecha inicial"
    '        Exit Function
    '    End If

    '    Dim id_ciclo_documental As Integer = 0
    '    Dim id_fondo_documental As Integer = 0
    '    If nombre_fondo_documental <> "" Then
    '        Result = Me.Retorna_id_fondo_documental_nombre(nombre_fondo_documental, id_fondo_documental)
    '        If Result <> "YES" Then
    '            Registrar_Expediente_Volumen = Result
    '            Exit Function
    '        End If
    '    End If
    '    If nombre_ciclo_documental <> "" Then
    '        Result = Me.Retorna_id_ciclo_archivo_nombre(nombre_ciclo_documental, id_ciclo_documental)
    '        If Result <> "YES" Then
    '            Registrar_Expediente_Volumen = Result
    '            Exit Function
    '        End If
    '    End If
    '    '-----------------------------------------------------
    '    'Especifica el ciclo de archivo y el fondo documental
    '    '-----------------------------------------------------
    '    Dim ref_id_ciclo_documental As Object = Nothing
    '    Dim ref_id_fondo_documental As Object = Nothing
    '    Dim ref_nombre_fondo_documental As String = ""
    '    Dim ref_nombre_ciclo_documental As String = ""
    '    If id_ciclo_documental = 0 Then
    '        ref_id_ciclo_documental = "Null"
    '    Else
    '        ref_id_ciclo_documental = id_ciclo_documental
    '    End If
    '    If id_fondo_documental = 0 Then
    '        ref_id_fondo_documental = "Null"
    '    Else
    '        ref_id_fondo_documental = id_fondo_documental
    '    End If
    '    If nombre_ciclo_documental <> "" Then
    '        ref_nombre_ciclo_documental = "'" & nombre_ciclo_documental & "'"
    '    Else
    '        ref_nombre_ciclo_documental = "Null"
    '    End If
    '    If nombre_fondo_documental <> "" Then
    '        ref_nombre_fondo_documental = "'" & nombre_fondo_documental & "'"
    '    Else
    '        ref_nombre_fondo_documental = "Null"
    '    End If
    '    Dim ref_nombre_persona_expediente As String = ""
    '    Dim ref_indentificacion_persona_expediente As String = ""
    '    If nombre_persona_expediente = "" Then
    '        ref_nombre_persona_expediente = "Null"
    '    Else
    '        ref_nombre_persona_expediente = "'" & nombre_persona_expediente & "'"
    '    End If
    '    If indentificacion_persona_expediente = "" Then
    '        ref_indentificacion_persona_expediente = "Null"
    '    Else
    '        ref_indentificacion_persona_expediente = "'" & indentificacion_persona_expediente & "'"
    '    End If
    '    Dim ref_nombre_responsable As String = ""
    '    If nombre_responsable = "" Then
    '        ref_nombre_responsable = "Null"
    '    Else
    '        ref_nombre_responsable = "'" & nombre_responsable & "'"
    '    End If
    '    Dim ref_identificacion_responsable As String = ""
    '    If identificacion_responsable = "" Then
    '        ref_identificacion_responsable = "Null"
    '    Else
    '        ref_identificacion_responsable = "'" & identificacion_responsable & "'"
    '    End If
    '    Dim Refclas As New ClassGestionDocumental
    '    Dim id_organigrama As Integer = 0
    '    Result = Refclas.Retorna_Id_Organigrama_activo_empresa(id_empresa, nombre_organigrama, id_organigrama)
    '    If Result <> "YES" Then
    '        Registrar_Expediente_Volumen = Result
    '        Exit Function
    '    End If
    '    Dim codigo_area As Integer = 0
    '    Result = Refclas.Retorna_cod_Area_Departamento(id_organigrama, codigo_area, nombre_area)
    '    If Result <> "YES" Then
    '        Registrar_Expediente_Volumen = Result
    '        Exit Function
    '    End If
    '    '------------------------------------------
    '    'Retorna id tipo unidad documental
    '    '------------------------------------------
    '    Dim id_tipo_unidad_documental As Integer = 0
    '    Dim refclas_unidad_conservacion As New ClassUnidadConservacion
    '    Result = refclas_unidad_conservacion.Retorna_id_tipo_unidad_documental_por_nombre(nombre_tipo_unidad_documental, id_tipo_unidad_documental)
    '    If Result <> "YES" Then
    '        Registrar_Expediente_Volumen = Result
    '        Exit Function
    '    End If
    '    '---------------------------------------------
    '    'Retorna codigo subseccion
    '    '---------------------------------------------
    '    Dim id_sub_seccion As Object = 0
    '    If nombre_sub_seccion <> "" Then
    '        Result = Refclas.Retorna_codigo_sub_area_departamento_radicacion(codigo_area, nombre_sub_seccion, id_sub_seccion)
    '        If Result <> "YES" Then
    '            Registrar_Expediente_Volumen = Result
    '            Exit Function
    '        End If
    '    End If
    '    Dim re_nombre_sub_seccion As String = ""
    '    If nombre_sub_seccion = "" Then
    '        re_nombre_sub_seccion = "null"
    '    Else
    '        re_nombre_sub_seccion = "'" & nombre_sub_seccion & "'"
    '    End If
    '    If id_sub_seccion = 0 Then
    '        id_sub_seccion = "null"
    '    End If
    '    Dim id_serie As Object = 0
    '    Dim consecutivo_serie As Integer = 0
    '    Dim consecutivo_Sub_serie As Integer = 0
    '    If nombre_serie <> "" Then
    '        Result = Refclas.Retorna_Id_serie_Documental(codigo_area.ToString, nombre_serie, id_serie, consecutivo_serie, consecutivo_Sub_serie)
    '        If Result <> "YES" Then
    '            Registrar_Expediente_Volumen = Result
    '            Exit Function
    '        End If
    '    End If
    '    Dim id_consecutivo_doc As Integer = 0
    '    Dim id_sub_serie As Object = 0
    '    If nombre_sub_serie <> "" Then
    '        Result = Refclas.Retorna_Id_Subserie_Consecutivo_TipDoc(nombre_sub_serie, id_serie, id_sub_serie, id_consecutivo_doc)
    '        If Result <> "YES" Then
    '            Registrar_Expediente_Volumen = Result
    '            Exit Function
    '        End If
    '    End If
    '    Dim id_tipo_unidad_conservacion As Integer = 0
    '    Result = refclas_unidad_conservacion.Retorna_id_tipo_unidad_conservacion_expediente(nombre_tipo_unidad_conservacion, id_tipo_unidad_conservacion, 2)
    '    If Result <> "YES" Then
    '        Registrar_Expediente_Volumen = Result
    '        Exit Function
    '    End If
    '    Dim ref_id_tipo_unidad_conservacion As Object = "null"
    '    If id_tipo_unidad_conservacion = -1 Or id_tipo_unidad_conservacion = 0 Then
    '        ref_id_tipo_unidad_conservacion = "null"
    '    Else
    '        ref_id_tipo_unidad_conservacion = id_tipo_unidad_conservacion
    '    End If
    '    Dim ref_nombre_tipo_unidad_conservacion As String = "null"
    '    If nombre_tipo_unidad_conservacion = "" Then
    '        ref_nombre_tipo_unidad_conservacion = "null"
    '    Else
    '        ref_nombre_tipo_unidad_conservacion = "'" & nombre_tipo_unidad_conservacion & "'"
    '    End If
    '    Dim ref_nombre_serie As String = "null"
    '    If nombre_serie = "" Then
    '        ref_nombre_serie = "null"
    '    Else
    '        ref_nombre_serie = "'" & nombre_serie & "'"
    '    End If
    '    Dim ref_nombre_sub_serie As String = "null"
    '    If nombre_sub_serie = "" Then
    '        ref_nombre_sub_serie = "null"
    '    Else
    '        ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
    '    End If
    '    If id_serie = 0 Then
    '        id_serie = "null"
    '    End If
    '    If id_sub_serie = 0 Then
    '        id_sub_serie = "null"
    '    End If


    '    '---------------------------------------
    '    'Vefica formato fechas extremas
    '    '---------------------------------------
    '    Dim re_fecha_extrema_incial As String = ""
    '    Dim re_fecha_extrema_final As String = ""
    '    If fecha_extrema_incial <> "" Then
    '        'Dim splifecha() As String = fecha_extrema_incial.Split("/")
    '        re_fecha_extrema_incial = "'" & fecha_extrema_incial & "'"
    '    Else
    '        re_fecha_extrema_incial = "null"
    '    End If
    '    If fecha_extrema_final <> "" Then
    '        'Dim splifecha() As String = fecha_extrema_final.Split("/")
    '        re_fecha_extrema_final = "'" & fecha_extrema_final & "'"
    '    Else
    '        re_fecha_extrema_final = "null"
    '    End If

    '    If rango_extremo_inicial = "" Then
    '        rango_extremo_inicial = "null"
    '    Else
    '        rango_extremo_inicial = "'" & rango_extremo_inicial & "'"
    '    End If

    '    If rango_extremo_final = "" Then
    '        rango_extremo_final = "null"
    '    Else
    '        rango_extremo_final = "'" & rango_extremo_final & "'"
    '    End If

    '    If tema_unidad_conservacion = "" Then
    '        tema_unidad_conservacion = "null"
    '    Else
    '        tema_unidad_conservacion = "'" & tema_unidad_conservacion & "'"
    '    End If
    '    Dim ref_asunto_expediente As String = ""
    '    If asunto_expediente = "" Then
    '        ref_asunto_expediente = "null"
    '    Else
    '        ref_asunto_expediente = "'" & asunto_expediente & "'"
    '    End If
    '    Dim ref_observacion As String = ""
    '    If observacion = "" Then
    '        ref_observacion = "null"
    '    Else
    '        ref_observacion = "'" & observacion & "'"
    '    End If
    '    Dim ref_id_unidad_contendora As Object = "null"
    '    Dim ref_id_entrepaño As Object = "null"
    '    Dim ref_estado_archivado_expediente As Integer = 0
    '    If id_unidad_contendora <> 0 Then
    '        If requiere_unidad_fisica <> 1 Then
    '            Registrar_Expediente_Volumen = "La unidad documental no requiere de unidad contenedora física"
    '            Exit Function
    '        End If
    '        If tipo_unidad_contendora = "Entrepaño" Then
    '            ref_id_entrepaño = id_unidad_contendora
    '        Else
    '            ref_id_unidad_contendora = id_unidad_contendora
    '        End If
    '        ref_estado_archivado_expediente = 1
    '    End If
    '    Dim Refclasradic As New ClassAlmacenamiento
    '    Dim date1al As String = Date.Today
    '    Result = Refclasradic.Formatea_Fecha_Almacenamiento(date1al)
    '    If Result <> "YES" Then
    '        Registrar_Expediente_Volumen = Result
    '        Exit Function
    '    End If
    '    Dim myConnection As New MySqlConnection
    '    Dim ref As New conect.Dbase_Conction_Mysql_RA
    '    ref.Returna_Conexion_Mysql(myConnection)
    '    Dim myTrans As MySqlTransaction
    '    Dim consecutivo_unidad_volumen As String = 0
    '    Dim errorM As String = "YES"
    '    Try

    '        Dim sqlforupdate As String = "Select  CONSECUTIVO_EXPEDIENTE_2 from expediente_archivo  where ID_EXPEDIENTE=" & _
    '        id_expediente_padre & " for update "
    '        'myConnection.Open()
    '        Dim dat_reader As MySqlDataReader
    '        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
    '        myTrans = myConnection.BeginTransaction()
    '        myCommand.Connection = myConnection
    '        myCommand.Transaction = myTrans
    '        myCommand.CommandText = sqlforupdate
    '        dat_reader = myCommand.ExecuteReader()
    '        If dat_reader Is Nothing Then
    '            Registrar_Expediente_Volumen = "Imposible Encontrar consecutivo expediente error de conexion"
    '            errorM = "Imposible Encontrar consecutivo expediente error de conexion"
    '            'myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        If dat_reader.HasRows = False Then
    '            Registrar_Expediente_Volumen = "Imposible Encontrar consecutivo expediente"
    '            errorM = "Imposible Encontrar consecutivo expediente"
    '            'myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        If dat_reader.HasRows = True Then
    '            dat_reader.Read()
    '            consecutivo_unidad_volumen = dat_reader.Item(0)
    '            dat_reader.Close()
    '        End If
    '        consecutivo_unidad_volumen = consecutivo_unidad_volumen + 1
    '        Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
    '        '--------------------------------------------
    '        'Agregar valores insertcion
    '        '--------------------------------------------
    '        Dim sqlcampos_insert As String = "Insert into expediente_archivo (CONSECUTIVO_EXPEDIENTE," & _
    '        "CODIGO_LARGO,CODIGO_UNICO,ID_USUARIO_GESTION,FECHA_CREACION," & _
    '        "FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL,RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_EXPEDIENTE," & _
    '        "TIPO_EXPEDIENTE,MODULO_PRODUCTOR,ID_EMPRESA_EXPEDIENTE,NUMERO_FOLIOS_CONTENIDOS,NOMBRE_AREA_TRD," & _
    '        "CODIGO_AREA_TRD,NOMBRE_SERIE_TRD,CODIGO_SERIE_TRD,NOMBRE_SUBSERIE_TRD,CODIGO_SUB_SERIE_TRD" & _
    '        ",ASUNTO_EXPEDIENTE,RA_TIP_EXPE_ID_TIPO_EXPEDIENTE,NUMERO_DIGITALIZADO_CONTENIDO," & _
    '        "NUMERO_ELECTRONICO_CONTENIDO,EXPEDIENTE_PADRE,VOLUMEN_EXPEDIENTE,TIPO_UNIDAD_ID_TIPO,TIPO_UNIDAD_CONSERVACION,OBSERVACION_EXPEDIENTE,ID_SUB_AREA,NOMBRE_SUB_AREA" _
    '        & ",ID_TIPO_UNIDAD_DOCUMENTAL,NOMBRE_TIPO_UNIDAD_DOCUMENTAL,UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION,ENTRE_PAÑO_ID_ENTREPAÑO,ESTADO_ARCHIVO_EXPEDIENTE," _
    '        & "ID_FONDO,NOMBRE_FONDO,Id_tipos_ciclo_archivo,NOMBRE_CICLO_ARCHIVO,NOMBRE_PERSONA_EXPEDIENTE,IDENTIFICACION_PERSONA_EXPEDIENTE,NOMBRE_RESPONSABLE_EXPEDIENTE,IDENFICACION_RESPONSABLE_EXPEDIENTE) values "
    '        Dim sqlinsert_campos As String = "( 0" & ",'" & codigo_corto_unidad & "','" & codigo_unico & "'," & _
    '         id_usuario_gestion & ",'" & date1al & "'," & _
    '         re_fecha_extrema_incial & "," & re_fecha_extrema_final & "," & rango_extremo_inicial & "," & rango_extremo_final & _
    '        "," & tema_unidad_conservacion & ",1," & "'DOCUARCHI'" & "," & id_empresa & "," & numero_folios_fisicos & ",'" & nombre_area & _
    '        "'," & codigo_area & "," & ref_nombre_serie & "," & id_serie & "," & ref_nombre_sub_serie & "," & id_sub_serie & _
    '         ",'" & asunto_expediente & "'," & id_tipo_expediente & "," & numero_documento_digitalizado & "," & numero_documentos_electronicos & "," & _
    '         id_expediente_padre & "," & consecutivo_unidad_volumen & "," & ref_id_tipo_unidad_conservacion & "," & ref_nombre_tipo_unidad_conservacion & "," & ref_observacion & "," & id_sub_seccion & "," & re_nombre_sub_seccion & _
    '        "," & id_tipo_unidad_documental & ",'" & nombre_tipo_unidad_documental & "'," & ref_id_unidad_contendora & "," & ref_id_entrepaño & "," & ref_estado_archivado_expediente & "," & ref_id_fondo_documental & _
    '        "," & ref_nombre_fondo_documental & "," & ref_id_ciclo_documental & "," & ref_nombre_ciclo_documental & "," & ref_nombre_persona_expediente & "," & ref_indentificacion_persona_expediente & "," & ref_nombre_responsable & "," & ref_identificacion_responsable & ")"
    '        Dim sqlinsertcion As String = sqlcampos_insert & sqlinsert_campos
    '        myCommand.CommandText = sqlinsertcion
    '        Dim Switc As Integer = myCommand.ExecuteNonQuery()
    '        Dim objet As Object = myCommand.LastInsertedId
    '        If Switc = 0 Then
    '            Registrar_Expediente_Volumen = "Imposible registrar expediente  : " & sqlinsertcion
    '            'myTrans.Rollback()
    '            myConnection.Close()
    '            errorM = "Imposible registrar unidad de conservación  : " & sqlinsertcion
    '            Exit Function
    '        End If
    '        '--------------------------------------------------
    '        'Actualiza el consecutivo expediente padre
    '        '--------------------------------------------------
    '        Dim updatconsecutivo As String = "UPDATE expediente_archivo SET CONSECUTIVO_EXPEDIENTE_2=" & _
    '        consecutivo_unidad_volumen & " where ID_EXPEDIENTE=" & _
    '        id_expediente_padre
    '        myCommand.CommandText = updatconsecutivo
    '        Switc = myCommand.ExecuteNonQuery()
    '        If Switc = 0 Then
    '            errorM = "Imposible actualiza consecutivo tipo unidad  : " & updatconsecutivo
    '            myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        '-------------------------------------------------
    '        'Ingresa la relación de expedientes
    '        '-------------------------------------------------
    '        Dim isertrelacion As String = "Insert Into ra_relacion_expediente (ID_EXPEDIENTE_PADRE,ID_EXPDIENTE_HIJO) values " & _
    '        "(" & id_expediente_padre & "," & objet & ")"
    '        myCommand.CommandText = isertrelacion
    '        Switc = myCommand.ExecuteNonQuery()
    '        If Switc = 0 Then
    '            errorM = "Imposible crear relación  : " & isertrelacion
    '            myTrans.Rollback()
    '            myConnection.Close()
    '            Exit Function
    '        End If
    '        id_expediente = objet
    '        myTrans.Commit()
    '        myConnection.Close()
    '        Registrar_Expediente_Volumen = "YES"
    '    Catch ex As MySqlException
    '        If Not myTrans.Connection Is Nothing Then
    '            'myTrans.Rollback()
    '            myConnection.Close()
    '            Registrar_Expediente_Volumen = "An exception of type " + ex.GetType().ToString() + _
    '                              " was encountered while attempting to roll back the transaction."
    '            errorM = "An exception of type " + ex.GetType().ToString() + _
    '                              " was encountered while attempting to roll back the transaction."
    '            Exit Function
    '        End If
    '    Finally

    '        If Not myConnection Is Nothing Then
    '            myConnection.Close()
    '        End If
    '        Registrar_Expediente_Volumen = errorM

    '    End Try
    'End Function
    Function Des_Archiva_expediente(ByVal id_expediente As Integer,
                                    ByVal id_usuario_gestion As Integer,
                                    ByVal user_Gestion As String,
                                    ByVal iptrans As String _
                                    , ByRef node As TreeNode,
                                    ByRef trevi As TreeView) As String
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Des_Archiva_expediente = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update expediente_archivo set UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=null" &
            ",ESTADO_ARCHIVO_EXPEDIENTE=0, ENTRE_PAÑO_ID_ENTREPAÑO=null  where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Des_Archiva_expediente = "Imposible desarchivar expediente  : " & sqlinsertcion
                errorM = "Imposible desarchivar expediente  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'DESARCHIVA EXPEDIENTE','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           "null" & ",'" & iptrans & "','" & hor & "','DOCUARCHI','DESARCHIVADO EN EXPEDIENTE " & "null" & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza el estado de archivo  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            trevi.Nodes.Remove(trevi.SelectedNode)
            Dim sNodo As TreeNode = trevi.SelectedNode
            Dim pNodo As TreeNode = sNodo.Parent
            pNodo.ChildNodes.Remove(sNodo)
            myTrans.Commit()
            myConnection.Close()
            Des_Archiva_expediente = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Des_Archiva_expediente = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Des_Archiva_expediente = errorM

        End Try
    End Function
    Function Des_Archiva_expediente(ByVal id_expediente As Integer,
                                    ByVal id_usuario_gestion As Integer,
                                    ByVal user_Gestion As String,
                                    ByVal iptrans As String) As String
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Des_Archiva_expediente = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update expediente_archivo set UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=null" &
            ",ESTADO_ARCHIVO_EXPEDIENTE=0, ENTRE_PAÑO_ID_ENTREPAÑO=null  where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Des_Archiva_expediente = "Imposible desarchivar expediente  : " & sqlinsertcion
                errorM = "Imposible desarchivar expediente  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If
            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'DESARCHIVA EXPEDIENTE','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           "null" & ",'" & iptrans & "','" & hor & "','DOCUARCHI','DESARCHIVADO EN EXPEDIENTE " & "null" & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza el estado de archivo  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If

            myTrans.Commit()
            myConnection.Close()
            Des_Archiva_expediente = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Des_Archiva_expediente = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Des_Archiva_expediente = errorM

        End Try
    End Function
    Function Archiva_expediente_en_entrepano(ByVal id_entrepaño As Integer,
                                             ByVal idex_row As Integer,
                                             ByVal id_expediente As Integer,
                                             ByVal id_usuario_gestion As Integer,
                                             ByVal user_Gestion As String,
                                             ByVal iptrans As String,
                                             ByRef nod As TreeNode,
                                             ByRef estru As expediente_conservacion,
                                             ByRef reftreview As TreeView) As String
        '**************************************************************
        'Función : Archiva unidad de conservacion tipo 2 en entrepaño
        '
        'Fecha : 2015-01-23
        'Ing : Miguel Angel Urueta Miranda
        '**************************************************************
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Dim Result As String = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Archiva_expediente_en_entrepano = Result
            Exit Function
        End If
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim errorM As String = "YES"
        Dim myCommand As MySqlCommand = myConnection.CreateCommand()
        Try
            'myConnection.Open()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinsertcion As String = "update expediente_archivo set ENTRE_PAÑO_ID_ENTREPAÑO=" & id_entrepaño &
            ",ESTADO_ARCHIVO_EXPEDIENTE=1, UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION=null  where ID_EXPEDIENTE=" & id_expediente
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Archiva_expediente_en_entrepano = "Imposible archivar expediente  : " & sqlinsertcion
                errorM = "Imposible archivar expediente  : " & sqlinsertcion
                myConnection.Close()
                Exit Function
            End If

            Dim hor As String = Now
            Dim sqlforupdate As String = "INSERT INTO ra_log_expediente (desc_op,USER_OPER,ID_USER,DATE_TRANS,ID_UNIDAD" &
           ",IP_TRANS,HORA_REGISTRO,MODULO_REGISTRO,CAMPOS) values (" &
           "'ARCHIVA EXPEDIENTE','" & user_Gestion & "','" & id_usuario_gestion & "','" & date1al & "'," &
           id_expediente & ",'" & iptrans & "','" & hor & "','DOCUARCHI-WEB','ARCHIVADO EN UNIDAD CONSERVACION" & id_entrepaño.ToString & "')"
            myCommand.CommandText = sqlforupdate
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & sqlforupdate
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Result = Me.Agregar_expediente_anidad_en_entrepano_treview(nod, estru, reftreview)
            'If Result <> "YES" Then
            '    errorM = Result
            '    myTrans.Rollback()
            '    myConnection.Close()
            '    Exit Function
            'End If
            'data_gred.Selected = False
            myTrans.Commit()
            myConnection.Close()
            Archiva_expediente_en_entrepano = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                myTrans.Rollback()
                myConnection.Close()
                Archiva_expediente_en_entrepano = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                errorM = "An exception of type " + ex.GetType().ToString() +
                                  " was encountered while attempting to roll back the transaction."
                Exit Function
            End If
        Finally

            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Archiva_expediente_en_entrepano = errorM

        End Try
    End Function
    Function Registrar_Expediente_Conservacion(ByVal id_usuario_gestion As Integer,
        ByVal codigo_unico As String, ByVal estado_codigo_unico As Integer, ByVal id_empresa As Integer,
        ByVal fecha_extrema_incial As String, ByVal fecha_extrema_final As String,
        ByVal rango_extremo_inicial As String, ByVal rango_extremo_final As String,
        ByVal tema_unidad_conservacion As String,
        ByVal nombre_organigrama As String, ByVal nombre_area As String,
        ByVal nombre_serie As String, ByVal nombre_sub_serie As String, ByVal id_tipo_expediente As Integer,
        ByVal numero_documento_digitalizado As Integer, ByVal numero_folios_fisicos As Integer,
        ByVal numero_documentos_electronicos As Integer, ByVal asunto_expediente As String,
        ByVal volumen_expediente As Integer, ByVal nombre_tipo_unidad_conservacion As String, ByRef id_expediente As Integer, ByVal observacion As String,
        ByVal nombre_tipo_unidad_documental As String, ByVal nombre_sub_seccion As String, ByVal opcio_requiere_unidad_contenedora As Integer,
        ByVal tipo_unidad_contendora As String, ByVal id_unidad_contendora As Integer, ByVal requiere_unidad_fisica As Integer,
        ByVal nombre_ciclo_documental As String, ByVal nombre_fondo_documental As String, ByVal nombre_persona_expediente As String,
        ByVal indentificacion_persona_expediente As String, ByVal nombre_responsable As String, ByVal identificacion_responsable As String,
        ByVal aleas_expediente As String, ByVal expediente_relacion_producion As Integer, ByVal Estado_Publico_Sub_Expediente As Integer,
        ByVal id_instrumento As Object, ByVal nombre_gabinete As String,
        ByVal id_nivel_padre As Integer, ByRef id_registro_relacion As Integer,
                                               ByVal id_auto_registro As Integer) As String
        If fecha_extrema_incial = "" Then
            Registrar_Expediente_Conservacion = "Por favor seleccione la fecha inicial de la unidad documental (expediente)"
            Exit Function
        End If
        'Nombre tipounidad documental (simple-compuesta)
        If nombre_tipo_unidad_documental = "" Then
            Registrar_Expediente_Conservacion = "Por favor seleccione el tipo de unidad documental (Expediente)"
            Exit Function
        End If
        If nombre_organigrama = "" Then
            Registrar_Expediente_Conservacion = "Por favor seleccione el organigrama al que pertenece la unidad documental (Expediente)"
            Exit Function
        End If
        If nombre_area = "" Then
            Registrar_Expediente_Conservacion = "Por favor seleccione el área/departamento al que pertenece la unidad documental (Expediente)"
            Exit Function
        End If
        If id_tipo_expediente = 0 Then
            Registrar_Expediente_Conservacion = "Por favor seleccione el tipo de (expediente)"
            Exit Function
        End If
        If id_instrumento <> 0 Then
            If nombre_ciclo_documental = "" Then
                Registrar_Expediente_Conservacion = "Por favor seleccione el nombre ciclo de archivo al que pertenece la unidad documental (Expediente)"
                Exit Function
            End If
            If nombre_serie = "" And nombre_sub_serie = "" Then
                Registrar_Expediente_Conservacion = "Por favor seleccione la serie o la sub serie del expediente al que pertenece la unidad documental (Expediente)"
                Exit Function
            End If
        End If
        If requiere_unidad_fisica = 1 Then
            If nombre_tipo_unidad_conservacion = "" Then
                Registrar_Expediente_Conservacion = "Debe informar el tipo de unidad conservación al que pertenecerá la unidad documental (Expediente)"
                Exit Function
            End If
        End If
        If opcio_requiere_unidad_contenedora = 1 Then
            If id_unidad_contendora = 0 Then
                Registrar_Expediente_Conservacion = "Debe seleccionar una unidad contendora para la unidad documental (Expediente)"
                Exit Function
            End If
            If tipo_unidad_contendora = "" Then
                Registrar_Expediente_Conservacion = "Debe seleccionar el tipo de unidad contendora para la unidad documental (Expediente)"
                Exit Function
            End If
        End If
        Dim ref_estado_archivado_expediente As Integer = 0
        Dim ref_id_unidad_contendora As Object = "null"
        Dim ref_id_entrepaño As Object = "null"
        'Verifica si la unidad documental requiere de unidad de almacenamiento fisico
        If id_unidad_contendora <> 0 Then
            If requiere_unidad_fisica <> 1 Then
                Registrar_Expediente_Conservacion = "La unidad documental (Expediente) no requiere de unidad contenedora física"
                Exit Function
            End If
            If tipo_unidad_contendora = "Entrepaño" Then
                ref_id_entrepaño = id_unidad_contendora
            Else
                ref_id_unidad_contendora = id_unidad_contendora
            End If
            ref_estado_archivado_expediente = 1
        End If
        '-------------------------------------------------
        'Validación longitud de caracteres  
        '-------------------------------------------------
        If Len(codigo_unico) > 70 Then
            Registrar_Expediente_Conservacion = "El campo (consecutivo unidad) supera el número de caracteres permitidos (45 caracteres)"
            Exit Function
        End If
        'If Len(tema_unidad_conservacion) > 120 Then
        '    Registrar_Expediente_Conservacion = "El campo (tema expediente) supera el número de caracteres permitidos (120 caracteres)"
        '    Exit Function
        'End If
        If Len(indentificacion_persona_expediente) > 60 Then
            Registrar_Expediente_Conservacion = "El campo (identificacion solicitante) supera el número de caracteres permitidos (60 caracteres)"
            Exit Function
        End If
        If Len(identificacion_responsable) > 60 Then
            Registrar_Expediente_Conservacion = "El campo (identificacion responsable) supera el número de caracteres permitidos (60 caracteres)"
            Exit Function
        End If
        If Len(rango_extremo_inicial) > 45 Then
            Registrar_Expediente_Conservacion = "El campo (rango extremo inicial) supera el número de caracteres permitidos (45 caracteres)"
            Exit Function
        End If
        If Len(rango_extremo_final) > 45 Then
            Registrar_Expediente_Conservacion = "El campo (rango extremo final) supera el número de caracteres permitidos (45 caracteres)"
            Exit Function
        End If
        'If Len(nombre_persona_expediente) > 60 Then
        '    Registrar_Expediente_Conservacion = "El campo (nombre solicitante) supera el número de caracteres permitidos (60 caracteres)"
        '    Exit Function
        'End If
        'If Len(nombre_responsable) > 60 Then
        '    Registrar_Expediente_Conservacion = "El campo (nombre responsable) supera el número de caracteres permitidos (60 caracteres)"
        '    Exit Function
        'End If
        If Len(aleas_expediente) > 120 Then
            Registrar_Expediente_Conservacion = "El campo (aleas expediente) supera el número de caracteres permitidos (120 caracteres)"
            Exit Function
        End If
        Dim Result As String = ""
        Dim id_ciclo_documental As Integer = 0
        Dim id_fondo_documental As Integer = 0
        Dim ref_Class_ra_de_fondo_documental As New Class_ra_de_fondo_documental
        'Retorna identificación del fondo documental
        If nombre_fondo_documental <> "" Then
            Result = ref_Class_ra_de_fondo_documental.Retorna_id_fondo_documental_nombre(nombre_fondo_documental,
                                                                                         id_fondo_documental)
            If Result <> "YES" Then
                Registrar_Expediente_Conservacion = Result
                Exit Function
            End If
        End If
        Dim ref_Class_ra_de_tipos_ciclos_archivo As New Class_ra_de_tipos_ciclos_archivo
        If nombre_ciclo_documental <> "" Then
            Result = ref_Class_ra_de_tipos_ciclos_archivo.Retorna_id_ciclo_archivo_nombre(nombre_ciclo_documental,
                                                                                          id_ciclo_documental)
            If Result <> "YES" Then
                Registrar_Expediente_Conservacion = Result
                Exit Function
            End If
        End If
        '-----------------------------------------------------
        'Especifica el ciclo de archivo y el fondo documental
        '-----------------------------------------------------
        Dim ref_id_auto_registro As Object = Nothing
        Dim ref_id_ciclo_documental As Object = Nothing
        Dim ref_id_fondo_documental As Object = Nothing
        Dim ref_nombre_fondo_documental As String = ""
        Dim ref_nombre_ciclo_documental As String = ""
        If id_auto_registro = 0 Then
            ref_id_auto_registro = "Null"
        Else
            ref_id_auto_registro = id_auto_registro
        End If
        If id_ciclo_documental = 0 Then
            ref_id_ciclo_documental = "Null"
        Else
            ref_id_ciclo_documental = id_ciclo_documental
        End If
        If id_fondo_documental = 0 Then
            ref_id_fondo_documental = "Null"
        Else
            ref_id_fondo_documental = id_fondo_documental
        End If
        If nombre_ciclo_documental <> "" Then
            ref_nombre_ciclo_documental = "'" & nombre_ciclo_documental & "'"
        Else
            ref_nombre_ciclo_documental = "Null"
        End If
        If nombre_fondo_documental <> "" Then
            ref_nombre_fondo_documental = "'" & nombre_fondo_documental & "'"
        Else
            ref_nombre_fondo_documental = "Null"
        End If
        Dim ref_nombre_persona_expediente As String = ""
        Dim ref_indentificacion_persona_expediente As String = ""
        If nombre_persona_expediente = "" Then
            ref_nombre_persona_expediente = "Null"
        Else
            ref_nombre_persona_expediente = "'" & nombre_persona_expediente & "'"
        End If
        If indentificacion_persona_expediente = "" Then
            ref_indentificacion_persona_expediente = "Null"
        Else
            ref_indentificacion_persona_expediente = "'" & indentificacion_persona_expediente & "'"
        End If
        Dim ref_nombre_responsable As String = ""
        If nombre_responsable = "" Then
            ref_nombre_responsable = "Null"
        Else
            ref_nombre_responsable = "'" & nombre_responsable & "'"
        End If
        Dim ref_identificacion_responsable As String = ""
        If identificacion_responsable = "" Then
            ref_identificacion_responsable = "Null"
        Else
            ref_identificacion_responsable = "'" & identificacion_responsable & "'"
        End If
        Dim Refclas As New ClassGestionDocumental
        Dim id_organigrama As Integer = 0
        Dim Reclas_registro_organigrama As New Class_registro_organigrama
        Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama,
                                                                    id_empresa,
                                                                    id_organigrama)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        Dim codigo_area As Integer = 0
        Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
        Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama,
                                                                                 codigo_area,
                                                                                 nombre_area)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        '-----------------------------------------------------
        'Retorna id tipo unidad documental (SIMPLE-COMPUESTA)
        '-----------------------------------------------------
        Dim id_tipo_unidad_documental As Integer = 0
        Dim refclas_unidad_conservacion As New ClassUnidadConservacion
        Dim ref_Class_tipo_unidad_documental As New Class_tipo_unidad_documental
        Result = ref_Class_tipo_unidad_documental.Retorna_id_tipo_unidad_documental_por_nombre(nombre_tipo_unidad_documental,
                                                                                               id_tipo_unidad_documental)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        '---------------------------------------------
        'Retorna codigo sub seccion
        '---------------------------------------------
        Dim id_sub_seccion As Object = "Null"
        Dim re_nombre_sub_seccion As String = "Null"
        Dim id_tipo_unidad_conservacion As Integer = 0
        Dim ref_Class_tipo_unidad_conservacion As New Class_tipo_unidad_conservacion
        If nombre_tipo_unidad_conservacion <> "" Then
            Result = ref_Class_tipo_unidad_conservacion.Retorna_id_tipo_unidad_conservacion_expediente(nombre_tipo_unidad_conservacion,
                                                                                                       id_tipo_unidad_conservacion,
                                                                                                       2)
            If Result <> "YES" Then
                Registrar_Expediente_Conservacion = Result
                Exit Function
            End If

            If id_tipo_unidad_conservacion = 0 Then
                Registrar_Expediente_Conservacion = "Imposible encontrar el identificador del tipo de unidad conservación (" & nombre_tipo_unidad_conservacion & ")"
                Exit Function
            End If
        End If

        Dim id_serie As Object = 0
        Dim consecutivo_serie As Integer = 0
        Dim consecutivo_Sub_serie As Integer = 0
        Dim ref_Class_series_documentales As New Class_series_documentales
        If nombre_serie <> "" Then
            Result = ref_Class_series_documentales.Retorna_Id_serie_instrumento_Documental(codigo_area.ToString,
                                                                                          nombre_serie,
                                                                                          id_instrumento,
                                                                                          id_serie,
                                                                                          consecutivo_serie,
                                                                                          consecutivo_Sub_serie)
            If Result <> "YES" Then
                Registrar_Expediente_Conservacion = Result
                Exit Function
            End If
        End If
        Dim id_consecutivo_doc As Integer = 0
        Dim id_sub_serie As Object = 0
        Dim ref_Class_subseries_documentales As New Class_subseries_documentales
        If nombre_sub_serie <> "" Then
            Result = ref_Class_subseries_documentales.Retorna_Id_Subserie_Consecutivo_TipDoc(nombre_sub_serie,
                                                                                             id_serie,
                                                                                             id_sub_serie,
                                                                                             id_consecutivo_doc)
            If Result <> "YES" Then
                Registrar_Expediente_Conservacion = Result
                Exit Function
            End If
        End If
        Dim ref_id_tipo_unidad_conservacion As Object = "null"
        If id_tipo_unidad_conservacion = -1 Or id_tipo_unidad_conservacion = 0 Then
            ref_id_tipo_unidad_conservacion = "null"
        Else
            ref_id_tipo_unidad_conservacion = id_tipo_unidad_conservacion
        End If
        Dim ref_nombre_tipo_unidad_conservacion As String = "null"
        If nombre_tipo_unidad_conservacion = "" Then
            ref_nombre_tipo_unidad_conservacion = "null"
        Else
            ref_nombre_tipo_unidad_conservacion = "'" & nombre_tipo_unidad_conservacion & "'"
        End If
        Dim ref_nombre_serie As String = "null"
        If nombre_serie = "" Then
            ref_nombre_serie = "null"
        Else
            ref_nombre_serie = "'" & nombre_serie & "'"
        End If
        Dim ref_nombre_sub_serie As String = "null"
        If nombre_sub_serie = "" Then
            ref_nombre_sub_serie = "null"
        Else
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        Dim fecha_ret_gestion As String = ""
        Dim fecha_ret_central As String = ""
        Dim id_tipo_instrumento As Integer = 0
        Dim Refclas_gagestioninstrumento As New ClassGaGestionInstrumento
        Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
        If id_instrumento <> 0 Then
            Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(id_instrumento,
                                                                               id_tipo_instrumento)
            If Result <> "YES" Then
                Registrar_Expediente_Conservacion = Result
                Exit Function
            End If

            If id_tipo_instrumento = 1 Then
                Result = Me.Retorna_tiempos_de_retencion_tablas_retencion(id_serie,
                                                                          id_sub_serie,
                                                                          fecha_extrema_incial,
                                                                          fecha_ret_gestion,
                                                                          fecha_ret_central)
                If Result <> "YES" Then
                    Registrar_Expediente_Conservacion = Result
                    Exit Function
                End If
                If fecha_ret_gestion = "" Then
                    Registrar_Expediente_Conservacion = "La serie o sub serie no registran tiempos de retención "
                    Exit Function
                End If
                If fecha_ret_central = "" Then
                    fecha_ret_central = "Null"
                Else
                    fecha_ret_central = "'" & fecha_ret_central & "'"
                End If
                fecha_ret_gestion = "'" & fecha_ret_gestion & "'"
            End If
            If id_tipo_instrumento = 2 Then
                Result = Me.Retorna_tiempos_de_retencion_tablas_de_valoracion(id_serie,
                                                                              id_sub_serie,
                                                                              fecha_extrema_incial,
                                                                              fecha_ret_central)
                If Result <> "YES" Then
                    Registrar_Expediente_Conservacion = Result
                    Exit Function
                End If
                If fecha_ret_central = "" Then
                    Registrar_Expediente_Conservacion = "La serie o sub serie no registran tiempos de retención "
                    Exit Function
                End If
                fecha_ret_central = "'" & fecha_ret_central & "'"
                fecha_ret_gestion = "Null"
            End If
        Else
            fecha_ret_gestion = "Null"
            fecha_ret_central = "Null"
        End If
        If id_serie = 0 Then
            id_serie = "null"
        End If
        If id_sub_serie = 0 Then
            id_sub_serie = "null"
        End If
        ''-------------------------------------------------
        ''Verfica el codigo es manual
        ''-------------------------------------------------
        'If estado_codigo_unico = 1 Then
        '    If codigo_unico = "" Then
        '        Registrar_Expediente_Conservacion = "Debe informar el código"
        '        Exit Function
        '    End If
        '    Result = Verfica_Existencia_Codigo_Unico_Expediente(codigo_unico, id_empresa, volumen_expediente, codigo_area)
        '    If Result <> "YES" Then
        '        Registrar_Expediente_Conservacion = Result
        '        Exit Function
        '    End If
        'End If
        '---------------------------------------
        'Vefica formato fechas extremas
        '---------------------------------------
        Dim re_fecha_extrema_incial As String = ""
        Dim re_fecha_extrema_final As String = ""
        If fecha_extrema_incial <> "" Then
            re_fecha_extrema_incial = "'" & fecha_extrema_incial & "'"
        Else
            re_fecha_extrema_incial = "null"
        End If
        If fecha_extrema_final <> "" Then
            re_fecha_extrema_final = "'" & fecha_extrema_final & "'"
        Else
            re_fecha_extrema_final = "null"
        End If

        If rango_extremo_inicial = "" Then
            rango_extremo_inicial = "null"
        Else
            rango_extremo_inicial = "'" & rango_extremo_inicial & "'"
        End If

        If rango_extremo_final = "" Then
            rango_extremo_final = "null"
        Else
            rango_extremo_final = "'" & rango_extremo_final & "'"
        End If

        If tema_unidad_conservacion = "" Then
            tema_unidad_conservacion = "null"
        Else
            tema_unidad_conservacion = "'" & tema_unidad_conservacion & "'"
        End If
        Dim ref_asunto_expediente As String = ""
        If asunto_expediente = "" Then
            ref_asunto_expediente = "null"
        Else
            ref_asunto_expediente = "'" & asunto_expediente & "'"
        End If
        Dim ref_observacion As String = ""
        If observacion = "" Then
            ref_observacion = "null"
        Else
            ref_observacion = "'" & observacion & "'"
        End If
        Dim ref_aleas_expediente As String = ""
        If aleas_expediente = "" Then
            ref_aleas_expediente = "null"
        Else
            ref_aleas_expediente = "'" & aleas_expediente & "'"
        End If
        If id_instrumento = 0 Then
            id_instrumento = "Null"
        End If
        If nombre_gabinete = "" Then
            nombre_gabinete = "PRODUCIONDOC"
        End If
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Dim date1al As String = Date.Today
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        Dim date_registro As String = Date.Today
        Result = ref_ClassGestionFechas.Retorna_fecha_registro(date_registro)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        '---------------------------------------
        'Actualización expediente electrónico
        '---------------------------------------
        Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
        Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
        Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        Dim disco_carpeta As String = stru_ruta_expediente_.DISCO
        Dim class_zerro_fill As New Class_zero_fill
        Result = class_zerro_fill.zero_fill(disco_carpeta, 9, "0")
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
        If Directory.Exists(Ruta_expediente) = False Then
            Registrar_Expediente_Conservacion = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
            Exit Function
        End If
        Ruta_expediente = Ruta_expediente & disco_carpeta
        If Directory.Exists(Ruta_expediente) = False Then
            Directory.CreateDirectory(Ruta_expediente)
        End If
        Dim id_sistema_meta_dato As Integer = 0
        Dim tipo_meta_dato As Integer = 2
        Dim stru_detalle_item_meta_dato() As stru_detalle_sis_meta_dato = Nothing
        Dim Ref_clas_ra_m_sistema_meta_datos As New Class_ra_m_sistema_meta_datos
        Dim Ref_ra_m_detalle_sis_meta_datos As New Class_ra_m_detalle_sis_meta_datos
        Result = Ref_clas_ra_m_sistema_meta_datos.Solicita_identificacion_sistema_meta_dato_default_archivo(tipo_meta_dato,
                                                                                                            id_sistema_meta_dato)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        Result = Ref_ra_m_detalle_sis_meta_datos.Solicita_estructura_meta_dato_sistema_stru(id_sistema_meta_dato,
                                                                                            stru_detalle_item_meta_dato)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        Dim Ref_class_remit_dest_interno As New Class_remit_dest_interno
        Dim cargo_usuario_gestion As String = ""
        Dim nombre_usuario_gestion As String = ""
        Dim correo_electronico As String = ""
        Result = Ref_class_remit_dest_interno.Retorna_datos_caracterizacion_usuario_gestion(id_usuario_gestion,
                                                                                            nombre_usuario_gestion,
                                                                                            cargo_usuario_gestion,
                                                                                            correo_electronico)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        Dim Nombre_tipo_expediente As String = ""
        Dim Ref_class_tipo_expdiente As New Class_ra_tipo_expediente
        Result = Ref_class_tipo_expdiente.Solicita_nombre_tipo_expediente(id_tipo_expediente,
                                                                          Nombre_tipo_expediente)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        Dim nombre_empresa As String = ""
        Dim Ref_class_empresa As New Class_empresa_gestion_documental
        Result = Ref_class_empresa.Solicita_nombre_identificacion_empresa("",
                                                                         nombre_empresa)
        If Result <> "YES" Then
            Registrar_Expediente_Conservacion = Result
            Exit Function
        End If
        For i As Integer = 0 To stru_detalle_item_meta_dato.Length - 1
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Fecha_" Then
                stru_detalle_item_meta_dato(i).value = date1al.Replace("/", "-")
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Autor_" Then
                stru_detalle_item_meta_dato(i).value = nombre_usuario_gestion & "(" & cargo_usuario_gestion & ") (" & nombre_empresa & ")"
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "asunto_" Then
                stru_detalle_item_meta_dato(i).value = asunto_expediente
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "nombre_serie" Then
                stru_detalle_item_meta_dato(i).value = nombre_serie
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "codigo_serie" Then
                stru_detalle_item_meta_dato(i).value = id_serie
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "nombre_sub_serie" Then
                stru_detalle_item_meta_dato(i).value = nombre_sub_serie
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "codigo_sub_serie" Then
                stru_detalle_item_meta_dato(i).value = id_sub_serie
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Fecha_apertura" Then
                stru_detalle_item_meta_dato(i).value = date_registro
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "codigo_area_" Then
                stru_detalle_item_meta_dato(i).value = codigo_area
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Nombre_area_" Then
                stru_detalle_item_meta_dato(i).value = nombre_area
            End If
            If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Nombre_tipo_expediente" Then
                stru_detalle_item_meta_dato(i).value = Nombre_tipo_expediente
            End If
        Next
        Dim sqlinsertcion As String = ""
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_RA
        ref.Returna_Conexion_Mysql(myConnection)
        Dim myTrans As MySqlTransaction
        Dim consecutivo_unidad As String = 0
        Dim consecutivo_exp_nivel As Integer = 1
        Dim errorM As String = "YES"
        Try
            Dim sqlforupdate As String = "Select CONSECUTIVO_EXPEDIENTE  from ra_consecutivo_expediente_archivo for update "
            'myConnection.Open()
            Dim dat_reader As MySqlDataReader
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            myCommand.CommandText = sqlforupdate
            dat_reader = myCommand.ExecuteReader()
            If dat_reader Is Nothing Then
                Registrar_Expediente_Conservacion = "Imposible Encontrar consecutivo expediente error de conexión"
                errorM = "Imposible Encontrar consecutivo expediente error de conexion"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = False Then
                Registrar_Expediente_Conservacion = "Imposible Encontrar consecutivo expediente"
                errorM = "Imposible Encontrar consecutivo expediente"
                'myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If dat_reader.HasRows = True Then
                dat_reader.Read()
                consecutivo_unidad = dat_reader.Item(0)
                dat_reader.Close()
            End If
            consecutivo_unidad = consecutivo_unidad + 1
            Dim año_radic As String = Now.Year.ToString.Substring(2, 2)
            If id_nivel_padre <> 0 Then
                sqlforupdate = "Select conta_expediente  from ra_pro_niveles where id_nivel=" & id_nivel_padre & " for update "
                myCommand.CommandText = sqlforupdate
                dat_reader = myCommand.ExecuteReader()
                If dat_reader Is Nothing Then
                    Registrar_Expediente_Conservacion = "Imposible Encontrar el nivel del expediente error de conexión"
                    errorM = "Imposible Encontrar consecutivo de expedientes del nivel"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If dat_reader.HasRows = False Then
                    Registrar_Expediente_Conservacion = "Imposible Encontrar consecutivo de expedientes del nivel"
                    errorM = "Imposible Encontrar consecutivo de expedientes del nivel"
                    'myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If dat_reader.HasRows = True Then
                    dat_reader.Read()
                    consecutivo_exp_nivel = dat_reader.Item(0)
                    consecutivo_exp_nivel = consecutivo_exp_nivel + 1
                    dat_reader.Close()
                End If
            End If
            '-------------------------------------------
            'Asigna codigo corto unidad
            '-------------------------------------------
            Dim codigo_corto_unidad As String = ""
            Result = Genera_Codigo_corto_Expediente(codigo_corto_unidad,
                                                    id_empresa,
                                                    id_usuario_gestion,
                                                    consecutivo_unidad,
                                                    año_radic)
            If Result <> "YES" Then
                Registrar_Expediente_Conservacion = Result
                errorM = Result
                Exit Function
            End If
            '-------------------------------------------
            'Asigna codigo unico
            '-------------------------------------------
            If estado_codigo_unico = 0 Then
                Result = Genera_Codigo_largo_Unidad_Conservacion(codigo_unico,
                                                                 id_empresa,
                                                                 consecutivo_unidad,
                                                                 año_radic,
                                                                 1)
                If Result <> "YES" Then
                    Registrar_Expediente_Conservacion = Result
                    errorM = Result
                    Exit Function
                End If
            End If

            '--------------------------------------------
            'Agregar valores insertcion
            '--------------------------------------------
            Dim sqlcampos_insert As String = "Insert into expediente_archivo (CONSECUTIVO_EXPEDIENTE," &
            "CODIGO_LARGO,CODIGO_UNICO,ID_USUARIO_GESTION,FECHA_CREACION," &
            "FECHA_EXTREMA_INICIAL,FECHA_EXTREMA_FINAL,RANGO_EXTREMO_INICIAL,RANGO_EXTREMO_FINAL,TEMA_EXPEDIENTE," &
            "TIPO_EXPEDIENTE,MODULO_PRODUCTOR,ID_EMPRESA_EXPEDIENTE,NUMERO_FOLIOS_CONTENIDOS,NOMBRE_AREA_TRD," &
            "CODIGO_AREA_TRD,NOMBRE_SERIE_TRD,CODIGO_SERIE_TRD,NOMBRE_SUBSERIE_TRD,CODIGO_SUB_SERIE_TRD" &
            ",ASUNTO_EXPEDIENTE,RA_TIP_EXPE_ID_TIPO_EXPEDIENTE,NUMERO_DIGITALIZADO_CONTENIDO," &
            "NUMERO_ELECTRONICO_CONTENIDO,TIPO_UNIDAD_ID_TIPO,TIPO_UNIDAD_CONSERVACION,OBSERVACION_EXPEDIENTE,ID_SUB_AREA,NOMBRE_SUB_AREA" _
            & ",ID_TIPO_UNIDAD_DOCUMENTAL,NOMBRE_TIPO_UNIDAD_DOCUMENTAL,UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION,ENTRE_PAÑO_ID_ENTREPAÑO,ESTADO_ARCHIVO_EXPEDIENTE,ID_FONDO," _
            & "NOMBRE_FONDO,Id_tipos_ciclo_archivo,NOMBRE_CICLO_ARCHIVO,NOMBRE_PERSONA_EXPEDIENTE,IDENTIFICACION_PERSONA_EXPEDIENTE,NOMBRE_RESPONSABLE_EXPEDIENTE," &
            "IDENFICACION_RESPONSABLE_EXPEDIENTE,ALEAS_EXPEDIENTE,Estado_Publico_Sub_Expediente,expediente_relacion_producion," &
            "fecha_ret_central,fecha_ret_gestion,id_instrumento,fecha_registro,GABINETE_PRODUCION,estado_expediente_electronico,ID_DISCO,ra_auto_registro_expediente_id_auto_registro) values "
            Dim sqlinsert_campos As String = "( 0" & ",'" & codigo_corto_unidad & "','" & codigo_unico & "'," &
             id_usuario_gestion & ",'" & date1al & "'," &
             re_fecha_extrema_incial & "," & re_fecha_extrema_final & "," & rango_extremo_inicial & "," & rango_extremo_final &
            "," & tema_unidad_conservacion & "," & id_tipo_expediente & "," & "'DOCUARCHI_WEB'" & "," & id_empresa & "," & numero_folios_fisicos & ",'" & nombre_area &
            "'," & codigo_area & "," & ref_nombre_serie & "," & id_serie & "," & ref_nombre_sub_serie & "," & id_sub_serie &
             "," & ref_asunto_expediente & "," & id_tipo_expediente & "," & numero_documento_digitalizado & "," & numero_documentos_electronicos & "," _
             & ref_id_tipo_unidad_conservacion & "," & ref_nombre_tipo_unidad_conservacion & "," & ref_observacion & "," & id_sub_seccion & "," & re_nombre_sub_seccion &
            "," & id_tipo_unidad_documental & ",'" & nombre_tipo_unidad_documental & "'," & ref_id_unidad_contendora & "," & ref_id_entrepaño & "," & ref_estado_archivado_expediente & "," & ref_id_fondo_documental &
            "," & ref_nombre_fondo_documental & "," & ref_id_ciclo_documental & "," & ref_nombre_ciclo_documental & "," & ref_nombre_persona_expediente & "," & ref_indentificacion_persona_expediente & "," &
            ref_nombre_responsable & "," & ref_identificacion_responsable & "," & ref_aleas_expediente & "," & Estado_Publico_Sub_Expediente & "," & expediente_relacion_producion &
           "," & fecha_ret_central & "," & fecha_ret_gestion & "," & id_instrumento & ",'" & date_registro & "','" & nombre_gabinete & "'," & 2 & "," & stru_ruta_expediente_.DISCO & "," & ref_id_auto_registro & ")"
            sqlinsertcion = sqlcampos_insert & sqlinsert_campos
            myCommand.CommandText = sqlinsertcion
            Dim Switc As Integer = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                Registrar_Expediente_Conservacion = "Imposible registrar expediente  : " & sqlinsertcion
                'myTrans.Rollback()
                myConnection.Close()
                errorM = "Imposible registrar unidad de conservacion  : " & sqlinsertcion
                Exit Function
            End If
            id_expediente = myCommand.LastInsertedId
            '////Actualización expediente elctronico
            For i As Integer = 0 To stru_detalle_item_meta_dato.Length - 1
                If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Titulo_" Then
                    stru_detalle_item_meta_dato(i).value = codigo_unico
                End If
                If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Identicador_exp" Then
                    stru_detalle_item_meta_dato(i).value = id_expediente
                End If
                If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Titulo_exp" Then
                    stru_detalle_item_meta_dato(i).value = codigo_corto_unidad
                End If
                If stru_detalle_item_meta_dato(i).nombre_meta_dato = "Palabra_clave_expediente" Then
                    stru_detalle_item_meta_dato(i).value = codigo_unico
                End If
            Next
            Dim expediente_zero_fil As String = id_expediente.ToString
            Result = class_zerro_fill.zero_fill(expediente_zero_fil, 9, "0")
            If Result <> "YES" Then
                errorM = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            Dim Ruta_archivo_xml As String = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
            Result = Me.Crea_archivo_indice_xml_expediente(Ruta_archivo_xml,
                                                           id_expediente,
                                                           date1al.Replace("/", "-"),
                                                           nombre_usuario_gestion,
                                                           nombre_empresa,
                                                           Nombre_tipo_expediente)
            If Result <> "YES" Then
                errorM = Result
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            'Crear archivo pdf expediente
            Dim updatconsecutivo As String = "UPDATE ra_consecutivo_expediente_archivo SET CONSECUTIVO_EXPEDIENTE=" &
            consecutivo_unidad
            myCommand.CommandText = updatconsecutivo
            Switc = myCommand.ExecuteNonQuery()
            If Switc = 0 Then
                errorM = "Imposible actualiza consecutivo tipo unidad  : " & updatconsecutivo
                myTrans.Rollback()
                myConnection.Close()
                Exit Function
            End If
            If expediente_relacion_producion <> 0 Then
                Dim insert_relacion_expediente_producion As String = "insert into ra_pro_relacion_exp_produccion (ID_EXPEDIENTE_PADRE,ID_EXPEDIENTE_HIJO) values " &
                    "(" & expediente_relacion_producion & "," & id_expediente & ")"
                myCommand.CommandText = insert_relacion_expediente_producion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    errorM = "Imposible registrar la relación de expediente "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            If id_nivel_padre <> 0 Then
                Dim sql_consecutivo_nivel As String = "Update ra_pro_niveles set conta_expediente=" & consecutivo_exp_nivel & "  where id_nivel=" & id_nivel_padre
                myCommand.CommandText = sql_consecutivo_nivel
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    errorM = "Imposible actualizar el consecutivo del nivel de expedientes "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim sql_insert_relacion As String = "Insert into ra_pro_niveles_has_expediente_archivo (ra_pro_niveles_id_nivel," &
                    "expediente_archivo_ID_EXPEDIENTE) values (" & id_nivel_padre & "," & id_expediente & ")"
                myCommand.CommandText = sql_insert_relacion
                Switc = myCommand.ExecuteNonQuery()
                If Switc = 0 Then
                    errorM = "Imposible registrar la relación con el nivel "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                id_registro_relacion = myCommand.LastInsertedId
            End If
            myTrans.Commit()
            myConnection.Close()
            errorM = "YES"
            Registrar_Expediente_Conservacion = "YES"
        Catch ex As MySqlException
            If Not myTrans.Connection Is Nothing Then
                'myTrans.Rollback()
                myConnection.Close()
                Registrar_Expediente_Conservacion = "An exception of type " + ex.GetType().ToString() +
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
                Registrar_Expediente_Conservacion = errorM + sqlinsertcion
            Else
                Registrar_Expediente_Conservacion = errorM
            End If

        End Try
    End Function
    Public Structure stru_values_cambio_indice
        Dim clave_index As String
        Dim value_index As String
    End Structure
    Function Actualiza_campos_indice_expediente_xml_expediente(ByVal ruta_archivo As String,
                                                               ByVal id_indice As Long,
                                                               ByVal stru_values_cambio_indice() As stru_values_cambio_indice) As String
        Try
            Dim xmlNodoList As XmlNodeList
            If File.Exists(ruta_archivo) = False Then
                Actualiza_campos_indice_expediente_xml_expediente = "Imposible encontrar el archivo (" & ruta_archivo & ") para actualizar el indice del expediente"
                Exit Function
            End If
            Dim xmlArchivo As XmlDocument = New XmlDocument
            xmlArchivo.Load(ruta_archivo)
            xmlNodoList = xmlArchivo.GetElementsByTagName("DocumentoIndizado")
            If xmlNodoList.Count > 1 Then
                For i As Integer = 0 To xmlNodoList.Count - 1
                    If xmlNodoList.Item(i).HasChildNodes Then
                        If xmlNodoList.Item(i).FirstChild.InnerText = id_indice Then
                            For k As Integer = 0 To stru_values_cambio_indice.Length - 1
                                xmlNodoList.Item(i).Item(stru_values_cambio_indice(k).clave_index).InnerText = stru_values_cambio_indice(k).value_index
                            Next
                            Exit For
                        End If
                    End If
                Next
            End If
            xmlArchivo.Save(ruta_archivo)
            Actualiza_campos_indice_expediente_xml_expediente = "YES"
        Catch ex As Exception
            Actualiza_campos_indice_expediente_xml_expediente = "Inconsistencia general funcion Actualiza_campos_indice_expediente_xml_expediente (" & ex.Message & ")"
        End Try
    End Function
    Function Actualiza_indice_tipo_documental_xml_expediente(ByVal ruta_archivo As String,
                                                             ByVal id_indice As Long,
                                                             ByVal descripcion_documento As String,
                                                             ByRef xmlArchivo As XmlDocument) As String
        Dim xmlNodoList As XmlNodeList
        Try
            If File.Exists(ruta_archivo) = False Then
                Actualiza_indice_tipo_documental_xml_expediente = "Imposible encontrar el archivo (" & ruta_archivo & ") para actualizar el indice del expediente"
                Exit Function
            End If
            xmlArchivo.Load(ruta_archivo)
            xmlNodoList = xmlArchivo.GetElementsByTagName("DocumentoIndizado")
            If xmlNodoList.Count > 1 Then
                For i As Integer = 0 To xmlNodoList.Count - 1
                    If xmlNodoList.Item(i).HasChildNodes Then
                        If xmlNodoList.Item(i).FirstChild.InnerText = id_indice Then
                            xmlNodoList.Item(i).Item("Tipologia_Documental").InnerText = descripcion_documento
                            Exit For
                        End If
                    End If
                Next
            End If

            Actualiza_indice_tipo_documental_xml_expediente = "YES"
            Exit Function
        Catch ex As Exception
            Actualiza_indice_tipo_documental_xml_expediente = "Inconsistencia genera funcion Actualiza_indice_tipo_documental_xml_expediente " & ex.Message
        End Try
    End Function
    Function Elimina_indice_archivo_xml_expediente(ByVal ruta_archivo As String,
                                                   ByVal id_indice As Long,
                                                   ByRef xmlArchivo As XmlDocument) As String
        Dim xmlNodoList As XmlNodeList
        Dim xmlNodoList_prent As XmlNode
        Try
            If File.Exists(ruta_archivo) = False Then
                Elimina_indice_archivo_xml_expediente = "Imposible encontrar el archivo (" & ruta_archivo & ") para actualizar el indice del expediente"
                Exit Function
            End If
            xmlArchivo.Load(ruta_archivo)
            xmlNodoList = xmlArchivo.GetElementsByTagName("DocumentoIndizado")
            Dim indixe As Integer = 0
            Dim pagina_inicial_anterior As Integer = 0
            Dim pagina_inicial_cache As Integer = 0
            If xmlNodoList.Count > 1 Then
                For i As Integer = 0 To xmlNodoList.Count - 1
                    If xmlNodoList.Item(i).HasChildNodes Then
                        If xmlNodoList.Item(i).FirstChild.InnerText = id_indice Then
                            indixe = i
                            Exit For
                        End If
                    End If
                Next
                Dim idndice_copia As Integer = indixe
                idndice_copia = idndice_copia + 1
                If idndice_copia = xmlNodoList.Count Then
                    xmlNodoList.Item(indixe).RemoveAll()
                    xmlNodoList_prent = xmlNodoList.Item(indixe).ParentNode
                    xmlNodoList_prent.RemoveChild(xmlNodoList.Item(indixe))
                    Elimina_indice_archivo_xml_expediente = "YES"
                    Exit Function
                End If
                If idndice_copia <> xmlNodoList.Count Then
                    For i As Integer = idndice_copia To xmlNodoList.Count - 1
                        Dim orden_expediente As Integer = 0
                        orden_expediente = Val(xmlNodoList.Item(i).Item("Orden_Documento_Expediente").InnerText)
                        orden_expediente = orden_expediente - 1
                        xmlNodoList.Item(i).Item("Orden_Documento_Expediente").InnerText = orden_expediente.ToString
                        pagina_inicial_anterior = Val(xmlNodoList.Item(i - 1).Item("Pagina_Inicio").InnerText)
                        If pagina_inicial_cache = 0 Then
                            Dim numero_paginas = (Val(xmlNodoList.Item(i).Item("Pagina_Fin").InnerText) - Val(xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText)) + 1
                            pagina_inicial_cache = Val(xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText)
                            xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText = pagina_inicial_anterior
                            If numero_paginas = 1 Then
                                xmlNodoList.Item(i).Item("Pagina_Fin").InnerText = xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText
                            Else
                                xmlNodoList.Item(i).Item("Pagina_Fin").InnerText = (Val(xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText) + numero_paginas) - 1
                            End If
                        Else
                            Dim numero_paginas = (Val(xmlNodoList.Item(i).Item("Pagina_Fin").InnerText) - Val(xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText)) + 1
                            Dim pagina_final_anterior = Val(xmlNodoList.Item(i - 1).Item("Pagina_Fin").InnerText)
                            xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText = pagina_final_anterior + 1
                            If numero_paginas = 1 Then
                                xmlNodoList.Item(i).Item("Pagina_Fin").InnerText = xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText
                            Else
                                xmlNodoList.Item(i).Item("Pagina_Fin").InnerText = (Val(xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText) + numero_paginas) - 1
                            End If
                        End If
                        'mat = mat & xmlNodoList.Item(i).Item("Orden_Documento_Expediente").InnerText & "-" & xmlNodoList.Item(i).Item("Pagina_Inicio").InnerText & "-" & xmlNodoList.Item(i).Item("Pagina_Fin").InnerText & vbCrLf
                    Next
                    xmlNodoList.Item(indixe).RemoveAll()
                    xmlNodoList_prent = xmlNodoList.Item(indixe).ParentNode
                    xmlNodoList_prent.RemoveChild(xmlNodoList.Item(indixe))
                    Elimina_indice_archivo_xml_expediente = "YES"
                    Exit Function
                End If
            End If
            If xmlNodoList.Count = 1 Then
                xmlNodoList.Item(0).RemoveAll()
                xmlNodoList_prent = xmlNodoList.Item(0).ParentNode
                xmlNodoList_prent.RemoveChild(xmlNodoList.Item(0))
                Elimina_indice_archivo_xml_expediente = "YES"
                Exit Function
            End If
            Elimina_indice_archivo_xml_expediente = "YES"
            Exit Function
        Catch ex As Exception
            Elimina_indice_archivo_xml_expediente = "Inconistencia general función Elimina_indice_archivo_xml_expediente " & ex.Message
        End Try
    End Function
    Function Actualiza_archivo_xml_indice_expediente(ByVal ruta_archivo As String,
                                                     ByVal stru_produccion_indice As stru_produccion_indice,
                                                     ByRef xmlArchivo As XmlDocument) As String
        Dim xmlNodoList As XmlNodeList
        Dim xmlNodo As XmlElement
        Dim xmlNodo_ As XmlElement
        Dim classGestionFechas As New ClassGestionFechas
        Dim Result As String = ""
        Try
            If File.Exists(ruta_archivo) = False Then
                Actualiza_archivo_xml_indice_expediente = "Imposible encontrar el archivo (" & ruta_archivo & ") para actualizar el indice del expediente"
                Exit Function
            End If
            xmlArchivo.Load(ruta_archivo)
            Dim ns As String = xmlArchivo.DocumentElement.NamespaceURI
            xmlNodoList = xmlArchivo.GetElementsByTagName("tipodocumentoFoliado")
            xmlNodo = xmlArchivo.CreateElement("DocumentoIndizado", ns)
            xmlNodo.InnerText = ""
            xmlNodo_ = xmlArchivo.CreateElement("Id", ns)
            xmlNodo_.InnerText = stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Nombre_Documento", ns)
            xmlNodo_.InnerText = stru_produccion_indice.NOMBRE_DOCUARCHI
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Tipologia_Documental", ns)
            xmlNodo_.InnerText = stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO
            xmlNodo.AppendChild(xmlNodo_)
            Dim fecha_tempo As String = stru_produccion_indice.FECHA_DOCUMENTO
            Result = classGestionFechas.Formatea_fecha_time_xmls_indice(fecha_tempo)
            xmlNodo_ = xmlArchivo.CreateElement("Fecha_Creacion_Documento", ns)
            xmlNodo_.InnerText = fecha_tempo
            fecha_tempo = stru_produccion_indice.FECHA_ELABORACION
            Result = classGestionFechas.Formatea_fecha_time_xmls_indice(fecha_tempo)
            xmlNodo_ = xmlArchivo.CreateElement("Fecha_Incorporacion_Expediente", ns)
            xmlNodo_.InnerText = fecha_tempo
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Valor_Huella", ns)
            xmlNodo_.InnerText = stru_produccion_indice.VALOR_HUELLA
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Funcion_Resumen", ns)
            xmlNodo_.InnerText = stru_produccion_indice.FUCION_RESUMEN
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Orden_Documento_Expediente", ns)
            xmlNodo_.InnerText = stru_produccion_indice.ORDEN_EN_EXPEDIENTE
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Pagina_Inicio", ns)
            xmlNodo_.InnerText = stru_produccion_indice.PAGINA_INICIO
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Pagina_Fin", ns)
            xmlNodo_.InnerText = stru_produccion_indice.PAGINA_FINAL
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Formato", ns)
            xmlNodo_.InnerText = stru_produccion_indice.FORMATO
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodo_ = xmlArchivo.CreateElement("Tamano", ns)
            xmlNodo_.InnerText = stru_produccion_indice.TAMANO
            xmlNodo.AppendChild(xmlNodo_)
            xmlNodoList.Item(0).AppendChild(xmlNodo)
            'xmlArchivo.Save(ruta_archivo)
            Actualiza_archivo_xml_indice_expediente = "YES"
        Catch ex As Exception
            Actualiza_archivo_xml_indice_expediente = "Inconistencia general función Actualiza_archivo_xml_indice_expediente " & ex.Message
        End Try
    End Function
    Function Registra_archivo_xml_indice_expediente(ByVal ruta_archivo As String,
                                                    ByVal id_expediente As Long,
                                                    ByVal fecha_expedente As String,
                                                    ByVal autor_expediente As String,
                                                    ByVal entidad_expediente As String,
                                                    ByVal nombre_tipo_expediente As String,
                                                    ByRef stru_produccion_indice() As stru_produccion_indice) As String
        Try
            If File.Exists(ruta_archivo) = True Then
                Kill(ruta_archivo)
            End If
        Catch ex As Exception

        End Try
        Dim myXmlTextWriter As XmlTextWriter = New XmlTextWriter(ruta_archivo,
                                                                  System.Text.Encoding.UTF8)
        Try
            Dim classGestionFechas As New ClassGestionFechas
            Dim Result As String = ""
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented
            myXmlTextWriter.WriteStartDocument(False)
            myXmlTextWriter.WriteStartElement("tipoIndiceContenido")
            myXmlTextWriter.WriteElementString("tipofechaIndiceElectronico", fecha_expedente)
            myXmlTextWriter.WriteElementString("tipoExpedienteFoliado", nombre_tipo_expediente)
            myXmlTextWriter.WriteStartElement("detalleindice")
            myXmlTextWriter.WriteElementString("identicacionexpediente", id_expediente)
            myXmlTextWriter.WriteElementString("autorexpediente", autor_expediente)
            myXmlTextWriter.WriteElementString("entidadexpediente", entidad_expediente)
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteStartElement("tipodocumentoFoliado", "http://www.w3.org/2001/XMLSchema-instance")
            If Not stru_produccion_indice Is Nothing Then
                For i As Integer = 0 To stru_produccion_indice.Length - 1
                    myXmlTextWriter.WriteStartElement("DocumentoIndizado")
                    myXmlTextWriter.WriteElementString("Id", stru_produccion_indice(i).ID_REGISTRO_PRODUCION_DOCUMENTAL)
                    myXmlTextWriter.WriteElementString("Nombre_Documento", stru_produccion_indice(i).NOMBRE_DOCUARCHI)
                    myXmlTextWriter.WriteElementString("Tipologia_Documental", stru_produccion_indice(i).DESCRIPCION_TIPO_DOCUMENTO)
                    Dim fecha_tempo As String = stru_produccion_indice(i).FECHA_DOCUMENTO
                    Result = classGestionFechas.Formatea_fecha_time_xmls_indice(fecha_tempo)
                    myXmlTextWriter.WriteElementString("Fecha_Creacion_Documento", fecha_tempo)
                    fecha_tempo = stru_produccion_indice(i).FECHA_ELABORACION
                    Result = classGestionFechas.Formatea_fecha_time_xmls_indice(fecha_tempo)
                    myXmlTextWriter.WriteElementString("Fecha_Incorporacion_Expediente", fecha_tempo)
                    myXmlTextWriter.WriteElementString("Valor_Huella", stru_produccion_indice(i).VALOR_HUELLA)
                    myXmlTextWriter.WriteElementString("Funcion_Resumen", stru_produccion_indice(i).FUCION_RESUMEN)
                    myXmlTextWriter.WriteElementString("Orden_Documento_Expediente", stru_produccion_indice(i).ORDEN_EN_EXPEDIENTE)
                    myXmlTextWriter.WriteElementString("Pagina_Inicio", stru_produccion_indice(i).PAGINA_INICIO)
                    myXmlTextWriter.WriteElementString("Pagina_Fin", stru_produccion_indice(i).PAGINA_FINAL)
                    myXmlTextWriter.WriteElementString("Formato", stru_produccion_indice(i).FORMATO)
                    myXmlTextWriter.WriteElementString("Tamano", stru_produccion_indice(i).TAMANO)
                    myXmlTextWriter.WriteEndElement()
                Next
            End If
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.Flush()
            myXmlTextWriter.Close()
            Registra_archivo_xml_indice_expediente = "YES"
        Catch ex As Exception
            If Not myXmlTextWriter Is Nothing Then
                myXmlTextWriter.Close()
            End If
            Registra_archivo_xml_indice_expediente = "Error general Registra_archivo_xml_indiece_expediente " & ex.Message
        End Try
    End Function
    Function Crea_archivo_indice_xml_expediente(ByVal ruta_archivo As String,
                                                ByVal id_expediente As Long,
                                                ByVal fecha_expedente As String,
                                                ByVal autor_expediente As String,
                                                ByVal entidad_expediente As String,
                                                ByVal nombre_tipo_expediente As String) As String
        Try
            If File.Exists(ruta_archivo) = True Then
                Kill(ruta_archivo)
            End If
        Catch ex As Exception

        End Try

        Dim myXmlTextWriter As XmlTextWriter = New XmlTextWriter(ruta_archivo,
                                                                  System.Text.Encoding.UTF8)
        Try
            myXmlTextWriter.Formatting = System.Xml.Formatting.Indented
            myXmlTextWriter.WriteStartDocument(False)
            myXmlTextWriter.WriteStartElement("tipoIndiceContenido")
            myXmlTextWriter.WriteElementString("tipofechaIndiceElectronico", fecha_expedente)
            myXmlTextWriter.WriteElementString("tipoExpedienteFoliado", nombre_tipo_expediente)
            myXmlTextWriter.WriteStartElement("detalleindice")
            myXmlTextWriter.WriteElementString("identicacionexpediente", id_expediente)
            myXmlTextWriter.WriteElementString("autorexpediente", autor_expediente)
            myXmlTextWriter.WriteElementString("entidadexpediente", entidad_expediente)
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteStartElement("tipodocumentoFoliado", "http://www.w3.org/2001/XMLSchema-instance")
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.WriteEndElement()
            myXmlTextWriter.Flush()
            myXmlTextWriter.Close()
            Crea_archivo_indice_xml_expediente = "YES"
        Catch ex As Exception
            If Not myXmlTextWriter Is Nothing Then
                myXmlTextWriter.Close()
            End If
            Crea_archivo_indice_xml_expediente = "Error general Crea_archivo_indice_xml_expediente " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_expediente_indice_electronico(ByVal id_expediente As Integer,
                                                            ByVal orden_inidice As Integer,
                                                            ByVal ultima_pagina_indice As Integer) As String
        Try
            Dim sql_update As String = "update expediente_archivo set estado_expediente_electronico=2" &
                ",ORDEN_INDICE=" & orden_inidice & ",ULTIMA_PAGINA_INDICE=" & ultima_pagina_indice &
                " where ID_EXPEDIENTE=" & id_expediente
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Actualiza_estado_expediente_indice_electronico = "Error function  Actualiza_estado_expediente_indice_electronico " & Result
                Exit Function
            Else
                Actualiza_estado_expediente_indice_electronico = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_expediente_indice_electronico = "Inconsistencia general funcion Actualiza_estado_expediente_indice_electronico " & ex.Message
        End Try
    End Function
    Function Solicita_estado_expediente_indice(ByVal id_expediente As Integer,
                                               ByVal estado_expediente_electronico As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select estado_expediente_electronico from  expediente_archivo " &
          " where ID_EXPEDIENTE=" & id_expediente
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_expediente_indice = "Función Solicita_estado_expediente_indice  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_expediente_indice = "Imposible encontrar el registro del expediente (" & id_expediente & ")"
                Exit Function
            Else
                estado_expediente_electronico = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_expediente_indice = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_expediente_indice = "Inconistencia general funcion Solicita_estado_expediente_indice " & ex.Message
        End Try
    End Function
    Function Retorna_estado_expediente_volumen_anexo(ByVal id_expediente As Integer) As String
        '***************************************************************
        'Funcion : Retorna si un expediente pertenece a un expediente
        'padre como volumen de composición
        'Fecha : 2015-01-12
        'Ingeniero : Miguel Angel Urueta Miranda
        'Modificación : Se actualiza la función para adaptarla al modo
        'de conexión del modulo web fecha 2015-04-21 Ing Miguel Angel
        'Urueta Miranda
        '***************************************************************
        Try
            Dim sqlconsulta As String = "Select EXPEDIENTE_PADRE from expediente_archivo where ID_EXPEDIENTE=" &
               id_expediente
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_estado_expediente_volumen_anexo = "Función Retorna_estado_expediente_volumen_anexo dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then

                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Retorna_estado_expediente_volumen_anexo = "YES"
                    Exit Function
                Else
                    Retorna_estado_expediente_volumen_anexo = "Este expediente es un volumen compuesto, no se puede relacionar como volumen"
                    Exit Function
                End If
            Else
                Retorna_estado_expediente_volumen_anexo = "Imposible encontrar el expediente " & id_expediente & ", es posible que lo aya eliminado  otro usuario"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_estado_expediente_volumen_anexo = "Inconsistencia funcion Retorna_estado_expediente_volumen_anexo " & ex.Message
        End Try
    End Function
    Function Verfica_Existencia_Codigo_Unico_Expediente(ByVal codigo_unico As String,
                                                        ByVal id_empresa As Integer,
                                                        ByVal volumen_expediente As Integer,
                                                        ByVal id_area As Integer) As String
        '**********************************************************************
        'Funcion verifica la existencia del codigo unico del expediente
        'con el parametro codigo
        'Fecha 2014-10-01
        'Ing : Miguel Angel Urueta Miranda
        '***********************************************************************
        Try
            Dim Parametro_Consulta As String = "select CODIGO_UNICO from  expediente_archivo " &
            " where CODIGO_UNICO='" & codigo_unico & "' and ID_EMPRESA_EXPEDIENTE=" & id_empresa &
            " and VOLUMEN_EXPEDIENTE=" & volumen_expediente & " and CODIGO_AREA_TRD=" & id_area
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verfica_Existencia_Codigo_Unico_Expediente = "Función Verfica_Existencia_Codigo_Unico_Expediente  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verfica_Existencia_Codigo_Unico_Expediente = "El codigo informado ya existe"
            Else
                Verfica_Existencia_Codigo_Unico_Expediente = "YES"
            End If
        Catch ex As Exception
            Verfica_Existencia_Codigo_Unico_Expediente = "Inconsistencia funcion Verfica_Existencia_Codigo_Unico_Expediente " & ex.Message
        End Try

    End Function
    Function Verifica_existencia_expediente_control_produccion(ByVal id_expediente As Integer) As String
        '******************************************************************
        'Funcion : Verifica la existencia documento producidos en el expe
        'diente
        'Fecha : 2014-10-03
        'Ingeniero: Miguel Angel Urueta Miranda
        'Modificado para la versión web 2015-21-04, se cambia el modo de
        'conexión a la base de datos
        '******************************************************************
        Try
            Dim Parametro_Consulta As String = "Select * from registro_producion_documental where EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE='" &
            id_expediente & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Verifica_existencia_expediente_control_produccion = "Función Verifica_existencia_expediente_control_produccion  Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verifica_existencia_expediente_control_produccion = "El expediente tiene documentos asignados"
            Else
                Verifica_existencia_expediente_control_produccion = "YES"
            End If
        Catch ex As Exception
            Verifica_existencia_expediente_control_produccion = "Inconsistencia función El expediente tiene documentos asignados " & ex.Message
        End Try

    End Function

    Function Verifica_propiedad_usuario_expediente(ByVal id_unidad_conservacion As Integer,
                                                    ByVal id_usuario_gstion As Integer) As String
        '********************************************************
        'Funcion : Verfica si el usuario si es propiestario de
        'expediente
        'Fecha 2014-09-27
        'Ing Migeuel Angel Urueta Miranda
        '********************************************************
        Try
            Dim SqlConsulta As String = "select * from  expediente_archivo " &
            " where ID_EXPEDIENTE=" & id_unidad_conservacion & " and ID_USUARIO_GESTION=" & id_usuario_gstion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                Verifica_propiedad_usuario_expediente = "Función Verifica_propiedad_usuario_expediente Error dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Verifica_propiedad_usuario_expediente = "YES"
                Exit Function
            Else
                Verifica_propiedad_usuario_expediente = "Usted no es propietario del expediente no puede interactuar con el expediente (" & id_unidad_conservacion & ")"
                Exit Function
            End If

        Catch ex As Exception
            Verifica_propiedad_usuario_expediente = "Inconsistencia funcion Verifica_propiedad_usuario_expediente " & ex.Message
        End Try
    End Function
    Function Asigna_datos_expediente_estructura(ByVal stru_campos_docuarchi() As stru_campos_docuarchi,
                                                ByRef matri_gestion As estructure_gestion,
                                                ByVal nombre_gabinete As String) As String
        '*********************************************************
        'Funcion : Asigna datos expediente de la 
        'interface de almacenamiento a la estructura
        'Fecha : 2015-01-10
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try

            '********************************************
            'Consulta opción aplica expediente Retorna_tipo_id_expediente_por_id
            '*******************************************
            Dim ref_Class_system1 As New Class_system1
            Dim Result As String = ""
            Dim opt_expediente As Integer = 0
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(opt_expediente,
                                                                       nombre_gabinete)
            If Result <> "YES" Then
                Asigna_datos_expediente_estructura = Result
                Exit Function
            End If
            If opt_expediente = 0 Then
                Asigna_datos_expediente_estructura = "YES"
                Exit Function
            End If
            Dim ref_ClassWorkflowIndiceDA As New ClassWorkflowIndiceDA
            Dim valor_campo As String = ""
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi,
                                                                                        "EXPEDIENTE",
                                                                                         valor_campo)
            If Result <> "YES" Then
                Asigna_datos_expediente_estructura = Result
                Exit Function
            End If
            matri_gestion.EXPEDIENTE = valor_campo
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi,
                                                                                         "Hidden_id_expediente",
                                                                                         valor_campo)
            If Result <> "YES" Then
                Asigna_datos_expediente_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_EXPEDIENTE = valor_campo
            Result = ref_ClassWorkflowIndiceDA.Solicita_valor_campo_estructura_docuarchi(stru_campos_docuarchi,
                                                                                        "Hidden_id_tipo_expediente",
                                                                                        valor_campo)
            If Result <> "YES" Then
                Asigna_datos_expediente_estructura = Result
                Exit Function
            End If
            matri_gestion.ID_TIPO_EXPEDIENTE = valor_campo
            If matri_gestion.ID_EXPEDIENTE <> 0 Then
                Result = Me.Retorna_tipo_id_expediente_por_id(matri_gestion.ID_TIPO_EXPEDIENTE,
                                                              matri_gestion.ID_EXPEDIENTE)
                If Result <> "YES" Then
                    Asigna_datos_expediente_estructura = "Función Asigna_datos_expediente_estructura dice : " & Result
                    Exit Function
                End If

            End If
            Asigna_datos_expediente_estructura = "YES"
        Catch ex As Exception
            Asigna_datos_expediente_estructura = "Inconsistencia general función Asigna_datos_expediente_estructura " & ex.Message
        End Try
    End Function
    Function Asigna_datos_expediente_estructura(ByRef page1 As Page,
                                                ByRef matri_gestion As estructure_gestion,
                                                ByVal nombre_gabinete As String) As String
        '*********************************************************
        'Funcion : Asigna datos expediente de la 
        'interface de almacenamiento a la estructura
        'Fecha : 2015-01-10
        'Ing : Miguel Angel Urueta Miranda
        '*********************************************************
        Try

            '********************************************
            'Consulta opción aplica expediente Retorna_tipo_id_expediente_por_id
            '*******************************************
            Dim ref_Class_system1 As New Class_system1
            Dim Result As String = ""
            Dim opt_expediente As Integer = 0
            Result = ref_Class_system1.Verfica_opcion_seleccion_unidad(opt_expediente,
                                                                       nombre_gabinete)
            If Result <> "YES" Then
                Asigna_datos_expediente_estructura = Result
                Exit Function
            End If
            If opt_expediente = 0 Then
                Asigna_datos_expediente_estructura = "YES"
                Exit Function
            End If
            Dim EXPEDIENTE As Object = Nothing
            Dim Hidden_id_expediente As Object = Nothing
            Dim Hidden_id_tipo_expediente As Object = Nothing
            EXPEDIENTE = page1.FindControl("EXPEDIENTE")
            If EXPEDIENTE Is Nothing Then
                Asigna_datos_expediente_estructura = "Función Asigna_datos_expediente_estructura dice : imposible encontrar el control EXPEDIENTE"
                Exit Function
            End If
            Hidden_id_expediente = page1.FindControl("Hidden_id_expediente")
            If Hidden_id_expediente Is Nothing Then
                Asigna_datos_expediente_estructura = "Función Asigna_datos_expediente_estructura dice : imposible encontrar el control Hidden_id_expediente"
                Exit Function
            End If
            Hidden_id_tipo_expediente = page1.FindControl("Hidden_id_tipo_expediente")
            If Hidden_id_tipo_expediente Is Nothing Then
                Asigna_datos_expediente_estructura = "Función Asigna_datos_expediente_estructura dice : imposible encontrar el control Hidden_id_tipo_expediente"
                Exit Function
            End If
            If Hidden_id_expediente.value <> 0 Then
                Result = Me.Retorna_tipo_id_expediente_por_id(Hidden_id_tipo_expediente.value,
                                                              Hidden_id_expediente.value)
                If Result <> "YES" Then
                    Asigna_datos_expediente_estructura = "Función Asigna_datos_expediente_estructura dice : " & Result
                    Exit Function
                End If

            End If
            matri_gestion.ID_TIPO_EXPEDIENTE = Hidden_id_tipo_expediente.value
            matri_gestion.ID_EXPEDIENTE = Hidden_id_expediente.value
            matri_gestion.EXPEDIENTE = EXPEDIENTE.text
            Asigna_datos_expediente_estructura = "YES"
        Catch ex As Exception
            Asigna_datos_expediente_estructura = "Inconsistencia general función Asigna_datos_expediente_estructura " & ex.Message
        End Try
    End Function
    Function Retorna_id_configuracion_rotulo_por_nombre_plantilla(ByVal nombre_plantilla_configuracion As String,
                                                                  ByRef id_configuracion_rotulo As Integer) As String
        '******************************************************************
        'Función : Retorna el id configuracion del rotulo, con el id del
        ' expediente y 
        'el id de usaurio de gestión
        'Fecha 2015-01-27 Modificado para el modulo web 2016-08-30 
        'Ingeniero : Miguel Angel Urueta Miranda
        '*******************************************************************
        Try
            Dim sqlconsulta As String = "Select ID_ROTULO_EXPEDIENTE  from ra_configuracion_rotulo_expediente  " &
            " where nombre_plantilla='" & nombre_plantilla_configuracion & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_configuracion_rotulo_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_configuracion_rotulo_por_nombre_plantilla = "Función Retorna_id_configuracion_rotulo_por_nombre_plantilla Error dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_configuracion_rotulo_por_nombre_plantilla = "Imposible encontrar configuración rotulo"
                Exit Function
            Else
                id_configuracion_rotulo = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_configuracion_rotulo_por_nombre_plantilla = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_id_configuracion_rotulo_por_nombre_plantilla = "Incosistencia  Función " & vbCrLf &
            " Retorna_id_configuracion_rotulo_expediente_por_id_expediente " & ex.Message
        End Try

    End Function
    Function Retorna_id_unidad_conservacion_contenedora_expediente(ByVal id_expediente As Integer,
                                                                   ByRef unidad_contenedora As String) As String
        '------------------------------------------------------
        'Función : Reotorna unidad de conservacion contenedora
        'del expediente 
        'Fecha 2016-09-15
        'Ing :Miguel Angel Urueta Miranda
        '------------------------------------------------------
        Try
            Dim sqlconsulta As String = "select uc.CODIGO_UNICO, uc.ID_UNIDAD_CONSERVACION,tu.NOMBRE_TIPO_UNIDAD " &
            " FROM  expediente_archivo as ea " &
            " INNER JOIN  unidad_conservacion AS uc on (uc.ID_UNIDAD_CONSERVACION=ea.UNIDAD_CONSERVACION_ID_UNIDAD_CONSERVACION) " &
            " INNER JOIN  tipo_unidad_conservacion AS tu on (uc.ID_TIPO_UNIDAD_CONSERVACION=tu.ID_TIPO_UNIDAD) " &
            " where ID_EXPEDIENTE=" & id_expediente
            unidad_contenedora = "No asignado"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_id_unidad_conservacion_contenedora_expediente = "Función Retorna_id_unidad_conservacion_contenedora_expediente Error dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                unidad_contenedora = Datset.Tables(0).Rows(0).Item(2) & " = " & Datset.Tables(0).Rows(0).Item(0) & " ID " & Datset.Tables(0).Rows(0).Item(1)
                Retorna_id_unidad_conservacion_contenedora_expediente = "YES"
            Else
                Retorna_id_unidad_conservacion_contenedora_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_unidad_conservacion_contenedora_expediente = "Inconsistencia general función Retorna_id_unidad_conservacion_contenedora_expediente " & ex.Message
        End Try
    End Function
    Function Retorna_estructura_aleas_campo_rotulo_expediente(ByVal id_configuracion_rotulo As Integer,
                                                              ByRef matri_config() As RA_ALEAS_CAMPOS_ROTULO_EXPEDIENTE) As String
        '----------------------------------------------------------
        'Funcion : Retorna de los campos aleas del los eninciados
        'del rotulo
        'Fecha : 2017-07-30
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select NOMBRE_CAMPO,ALEAS_CAMPO from RA_ALEAS_CAMPOS_ROTULO_EXPEDIENTE " &
           " where RA_CONF_ROT_EXP_ID_ROTULO_EXPEDIENTE=" & id_configuracion_rotulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("RA_ALEAS_CAMPOS_ROTULO_EXPEDIENTE")
            Dim Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Retorna_estructura_aleas_campo_rotulo_expediente = "Función Retorna_estructura_aleas_campo_rotulo_expediente Error dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve matri_config(i)
                    matri_config(i).nombre_Campo = Datset.Tables(0).Rows(i).Item(0)
                    matri_config(i).aleas_campo = Datset.Tables(0).Rows(i).Item(1)
                Next
                Retorna_estructura_aleas_campo_rotulo_expediente = "YES"
                Exit Function
            Else
                matri_config = Nothing
                Retorna_estructura_aleas_campo_rotulo_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_estructura_aleas_campo_rotulo_expediente = "Inconsistencia general función Retorna_estructura_aleas_campo_rotulo_expediente " & ex.Message
        End Try

    End Function
    Function Retorna_nombre_aleas_campo_rotulo(ByVal nombre_campo As String,
                                               ByVal matri_config() As RA_ALEAS_CAMPOS_ROTULO_EXPEDIENTE,
                                               ByRef nombre_aleas_campo As String) As String
        Try
            If Not matri_config Is Nothing Then
                For i As Integer = 0 To matri_config.Length - 1
                    If nombre_campo = matri_config(i).nombre_Campo Then
                        nombre_aleas_campo = matri_config(i).aleas_campo
                        Exit For
                    End If
                Next
            End If
            Retorna_nombre_aleas_campo_rotulo = "YES"
        Catch ex As Exception
            Retorna_nombre_aleas_campo_rotulo = "Inconsistencia general función Retorna_nombre_aleas_campo_rotulo " & ex.Message
        End Try

    End Function
    Function Genera_rotulo_Eexpediente_pdf(ByVal id_expediente As Integer,
                                           ByVal id_empresa As Integer,
                                           ByVal nombre_plantilla_impresion As String,
                                           ByRef ruta_archivo_rotulo As String) As String
        '*********************************************************Retorna_estructura_aleas_campo_rotulo_expediente******
        'Función : Genera rotulo expediente
        'Ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2015-01-25
        '***************************************************************
        Dim doc As New Document
        Dim writer As PdfWriter = Nothing
        Try
            Dim Result As String = ""
            '-------------------------------------------------
            'Retorna id configuracion unidad de conservación
            '-------------------------------------------------
            Dim id_configuracion_rotulo As Integer = 0
            Result = Me.Retorna_id_configuracion_rotulo_por_nombre_plantilla(nombre_plantilla_impresion,
            id_configuracion_rotulo)
            If Result <> "YES" Then
                Genera_rotulo_Eexpediente_pdf = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Retorna datos al rotulo
            '---------------------------------------------------
            Dim estru_unidad_conservacion() As expediente_conservacion
            Erase estru_unidad_conservacion
            Result = Me.SolicitaDatosEstructuraExpediente(id_expediente, estru_unidad_conservacion)
            If Result <> "YES" Then
                Genera_rotulo_Eexpediente_pdf = Result
                Exit Function
            End If
            '-----------------------------------------------------
            'Retorna estructura rotulo
            '-----------------------------------------------------
            Dim estru As rotulo_unidad_conservacion = Nothing
            Result = Me.Asigna_Datos_DB_Configuracion_rotulo_expediente_estructura(estru, id_configuracion_rotulo)
            If Result <> "YES" Then
                Genera_rotulo_Eexpediente_pdf = Result
                Exit Function
            End If
            '----------------------------------------------------
            'Retorna datos instrumento archivístico
            '----------------------------------------------------
            Dim nombre_instrumento As String = ""
            Dim version_instrumento As String = ""
            Dim id_tipo_instrumento As Integer = 0
            Dim Refclas_GestionInstrumento As New ClassGaGestionInstrumento
            If estru_unidad_conservacion(0).id_instrumento <> 0 Then
                Result = Refclas_GestionInstrumento.Solicita_datos_instrumento_rotulo(estru_unidad_conservacion(0).id_instrumento,
                                                                                      id_tipo_instrumento,
                                                                                      nombre_instrumento,
                                                                                      version_instrumento)
                If Result <> "YES" Then
                    Genera_rotulo_Eexpediente_pdf = Result
                    Exit Function
                End If
            End If
            '-----------------------------------------------------
            'Retorna estructura campos aleas del rotulo
            '-----------------------------------------------------
            Dim nombre_aleas As String = ""
            Dim Stru_aleas_rotulo() As RA_ALEAS_CAMPOS_ROTULO_EXPEDIENTE = Nothing
            Result = Me.Retorna_estructura_aleas_campo_rotulo_expediente(id_configuracion_rotulo,
                                                                         Stru_aleas_rotulo)
            If Result <> "YES" Then
                Genera_rotulo_Eexpediente_pdf = Result
                Exit Function
            End If
            Dim nombre_empresa As String = ""
            Dim nit_empresa As String = ""
            Dim refclasunidad As New ClassUnidadConservacion
            Result = refclasunidad.Retorna_Datos_Empresa(id_empresa,
                                                         nombre_empresa,
                                                         nit_empresa)
            If Result <> "YES" Then
                Genera_rotulo_Eexpediente_pdf = Result
                Exit Function
            End If
            Dim struentrepaño() As ClassGestionArchivo.Entrapño_archivo
            Erase struentrepaño
            Dim id_entrapaño_idex As Integer = 0
            If estru_unidad_conservacion(0).ESTADO_ARCHIVO_INIDAD = 1 Then
                Dim id_estante As Integer = 0
                Dim refclas As New ClassGestionArchivo
                If estru_unidad_conservacion(0).ENTRE_PAÑO_ID_ENTREPAÑO <> 0 Then
                    Result = refclas.Retorna_Id_Estante_por_entrepaño(estru_unidad_conservacion(0).ENTRE_PAÑO_ID_ENTREPAÑO, id_estante)
                    If Result <> "YES" Then
                        Genera_rotulo_Eexpediente_pdf = "Imposible listar entrepaños " & Result
                        Exit Function
                    End If
                Else
                    '----------------------------------------------------
                    'solicita el id del entrapaño por la unidad de 
                    'conservacion
                    '----------------------------------------------------
                    Dim id_entrepaño As Integer = 0
                    Result = refclas.Retorna_id_Entrepaño_id_unidad_conservacion(
                    estru_unidad_conservacion(0).ID_UNIDAD_CONSERVACION, id_entrepaño)
                    If Result <> "YES" Then
                        Genera_rotulo_Eexpediente_pdf = Result
                        Exit Function
                    End If
                    Result = refclas.Retorna_Id_Estante_por_entrepaño(id_entrepaño, id_estante)
                    If Result <> "YES" Then
                        Genera_rotulo_Eexpediente_pdf = "Imposible listar entrepaños " & Result
                        Exit Function
                    End If
                End If
                Result = refclas.Listar_Entrepaño_Archivo(id_empresa, struentrepaño, id_estante)
                If Result <> "YES" Then
                    Genera_rotulo_Eexpediente_pdf = "Imposible listar entrepaños " & Result
                    Exit Function
                End If

                For i As Integer = 0 To struentrepaño.Length - 1
                    If estru_unidad_conservacion(0).ENTRE_PAÑO_ID_ENTREPAÑO = struentrepaño(i).id_entreapaño Then
                        id_entrapaño_idex = i
                        Exit For
                    End If
                Next
            End If
            '--------------------------------------------------
            'Retorna unidad contenedora expediente
            '--------------------------------------------------
            Dim unidad_contenedora As String = ""
            Result = Me.Retorna_id_unidad_conservacion_contenedora_expediente(estru_unidad_conservacion(0).ID_EXPEDIENTE, unidad_contenedora)
            If Result <> "YES" Then
                Genera_rotulo_Eexpediente_pdf = Result
                Exit Function
            End If
            Dim Rutatemp As String = HttpContext.Current.Session.Item("GA_RUTA_TEMPO_IMPRESION") & "\"
            Rutatemp = HttpContext.Current.Session.Item("GA_RUTA_TEMPO_IMPRESION") & "\"
            If Directory.Exists(Rutatemp) = False Then
                Directory.CreateDirectory(Rutatemp)
            End If
            Dim archivo_pdf As String = Rutatemp & "temp_" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ".pdf"
            If File.Exists(archivo_pdf) = True Then
                Kill(archivo_pdf)
            End If
            ruta_archivo_rotulo = archivo_pdf
            doc = New Document(New Rectangle(estru.x, estru.y))
            doc.SetMargins(2.0F, 1.0F, 0.0F, 0.0F)
            writer = PdfWriter.GetInstance(doc,
                                New FileStream(archivo_pdf, FileMode.Create))
            writer.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE)
            doc.Open()
            Dim ruta_image As String = HttpContext.Current.Server.MapPath("../imagera/" & "logo_trd.png")
            If estru.image_empresa = True Then
                If File.Exists(ruta_image) = False And estru.image_empresa = True Then
                    Genera_rotulo_Eexpediente_pdf = "El sistema no tiene registrado el icono para rotulo en la ruta " & vbCrLf &
                         ruta_image
                    MsgBox(Genera_rotulo_Eexpediente_pdf)
                    'doc.Close()
                    'writer.Close()
                    'Exit Function
                End If
                Dim imagen As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(ruta_image)
                imagen.BorderWidth = 0
                imagen.Alignment = Element.ALIGN_CENTER
                Dim percentage As Object = 0.0F
                percentage = 100 / imagen.Width
                imagen.ScalePercent(percentage * 50)
                'Insertamos la imagen en el documento
                doc.Add(imagen)
            End If
            Dim _standardFont As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
            estru.TAM_LETRA_TITULO, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim _standardFont_datos_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
            estru.TAM_LETRA_DATOS_UNIDAD, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            Dim paragraf As New Paragraph
            paragraf = New Paragraph(nombre_empresa, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            If estru.nombre_empresa = True Then
                doc.Add(paragraf)
            End If
            paragraf = New Paragraph(nit_empresa, _standardFont)
            paragraf.Alignment = Element.ALIGN_CENTER
            If estru.nit_empresa = True Then
                doc.Add(paragraf)
            End If
            '---------------------------
            'Agrega version instrumento
            '---------------------------
            If estru.version_trd = True Then
                Dim datos_instrumento As String = ""
                If id_tipo_instrumento <> 0 Then
                    If id_tipo_instrumento = 1 Then
                        datos_instrumento = "  TRD - " & nombre_instrumento & " Vesrión " & version_instrumento
                    End If
                    If id_tipo_instrumento = 2 Then
                        datos_instrumento = "  TVD - " & nombre_instrumento & " Vesrión " & version_instrumento
                    End If
                End If
                paragraf = New Paragraph("instrumento : " & datos_instrumento, _standardFont)
                paragraf.Alignment = Element.ALIGN_CENTER
                If estru.nit_empresa = True Then
                    doc.Add(paragraf)
                End If
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("DATOS UNIDAD DOCUMENTAL", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "DATOS UNIDAD DOCUMENTAL "
            End If
            paragraf = New Paragraph(nombre_aleas & estru_unidad_conservacion(0).ID_EXPEDIENTE, _standardFont_datos_unidad_conservacion)
            paragraf.Alignment = Element.ALIGN_CENTER
            If estru.DATOS_UNIDAD_CONSERVACION = True Then
                doc.Add(paragraf)
                paragraf = New Paragraph(".", _standardFont_datos_unidad_conservacion)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
            End If
            doc.Add(New Paragraph(3, vbCrLf))
            Dim tblrdatos As PdfPTable = New PdfPTable(estru.numero_columnas_datos)
            tblrdatos.WidthPercentage = 100
            Dim Descripcion_Tipo As String = ""
            '-------------------------------------------------------------
            'Asigna id entrepaño-id tipo unidad-id unidad -descripcion
            '-------------------------------------------------------------
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Tipo unidad documental - Clase unidad documental:", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Tipo unidad documental - Clase unidad documental:"
            End If
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            ref_Class_ra_tipo_expediente.Retorna_nombre_tipo_expediente_por_id_expediente(estru_unidad_conservacion(0).ID_EXPEDIENTE, Descripcion_Tipo)
            Dim cltipounidad As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
            cltipounidad.BorderWidth = 1
            Dim cltipounidad_valor As PdfPCell = New PdfPCell(New Phrase(Descripcion_Tipo & " - " & estru_unidad_conservacion(0).NOMBRE_TIPO_UNIDAD_DOCUMENTAL, _standardFont_datos_unidad_conservacion))
            cltipounidad_valor.BorderWidth = 1
            tblrdatos.AddCell(cltipounidad)
            tblrdatos.AddCell(cltipounidad_valor)
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Código único", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Código único  :"
            End If
            Dim clCodigounico As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
            clCodigounico.BorderWidth = 1
            Dim clCodigounico_valor As PdfPCell = New PdfPCell(New Phrase(id_expediente, _standardFont_datos_unidad_conservacion))
            clCodigounico_valor.BorderWidth = 1
            If estru.Codigo_unico = True Then
                tblrdatos.AddCell(clCodigounico)
                tblrdatos.AddCell(clCodigounico_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Consecutivo unidad", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Consecutivo unidad :"
            End If
            Dim clCodigounico_exp As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
            clCodigounico.BorderWidth = 1
            Dim clCodigounico_exp_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).CODIGO_UNICO, _standardFont_datos_unidad_conservacion))
            clCodigounico_valor.BorderWidth = 1
            tblrdatos.AddCell(clCodigounico_exp)
            tblrdatos.AddCell(clCodigounico_exp_valor)
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Tema unidad documental", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Tema unidad documental :"
            End If
            Dim clTema As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
            clTema.BorderWidth = 1
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Tema unidad documental", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Tema unidad documental : "
            End If
            Dim clTema_valor As PdfPCell = New PdfPCell(New Phrase(nombre_aleas & estru_unidad_conservacion(0).TEMA_EXPEDIENTE, _standardFont_datos_unidad_conservacion))
            clTema_valor.BorderWidth = 1
            If estru.Tema_unidad = True Then
                'tblrdatos.AddCell(clTema)
                clTema_valor.Colspan = 2
                tblrdatos.AddCell(clTema_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Fechas", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Fechas  :"
            End If
            Dim hasta As String = " Hasta "
            Dim clrangosfechas As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
            clrangosfechas.BorderWidth = 1
            If estru_unidad_conservacion(0).FECHA_EXTREMA_FINAL = "" Then
                hasta = "  "
            Else
                hasta = " Hasta "
            End If
            Dim clrangosfechas_valor As PdfPCell = New PdfPCell(New Phrase _
            (estru_unidad_conservacion(0).FECHA_EXTREMA_INICIAL & hasta & estru_unidad_conservacion(0).FECHA_EXTREMA_FINAL, _standardFont_datos_unidad_conservacion))
            clrangosfechas_valor.BorderWidth = 1
            If estru.Fechas_Extremas = True Then
                tblrdatos.AddCell(clrangosfechas)
                tblrdatos.AddCell(clrangosfechas_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Rangos", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Rangos  :"
            End If
            Dim clrangosextremos As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
            clrangosextremos.BorderWidth = 1
            If estru_unidad_conservacion(0).RANGO_EXTREMO_FINAL = "" Then
                hasta = "  "
            Else
                hasta = " Hasta "
            End If
            Dim clrangosextremos_valor As PdfPCell = New PdfPCell(New Phrase _
            (estru_unidad_conservacion(0).RANGO_EXTREMO_INICIAL & hasta & estru_unidad_conservacion(0).RANGO_EXTREMO_FINAL, _standardFont_datos_unidad_conservacion))
            clrangosextremos_valor.BorderWidth = 1
            If estru.Rangos_Extremos = True Then
                tblrdatos.AddCell(clrangosextremos)
                tblrdatos.AddCell(clrangosextremos_valor)
            End If
            '----------------------------------------
            'Numero volumen
            '----------------------------------------
            If estru.numero_volumen = True Then
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Volumen", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Volumen : "
                End If
                Dim numero_volumnes As Integer = 1
                Dim indice As Integer = 0
                Dim texto_volumen As String = ""
                If estru_unidad_conservacion(0).EXPEDIENTE_PADRE = 0 Then
                    Result = Me.Solicita_numero_volumenes_relacionados(estru_unidad_conservacion(0).ID_EXPEDIENTE, estru_unidad_conservacion(0).ID_EXPEDIENTE, numero_volumnes, indice)
                    texto_volumen = estru_unidad_conservacion(0).VOLUMEN_EXPEIDENTE & " de " & numero_volumnes
                Else
                    Result = Me.Solicita_numero_volumenes_relacionados(estru_unidad_conservacion(0).EXPEDIENTE_PADRE, estru_unidad_conservacion(0).ID_EXPEDIENTE, numero_volumnes, indice)
                    texto_volumen = indice & " de " & numero_volumnes
                End If

                Dim clr_numero_volumen As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
                clr_numero_volumen.BorderWidth = 1
                Dim clr_numero_volumen_valor As PdfPCell = New PdfPCell(New Phrase _
                (texto_volumen, _standardFont_datos_unidad_conservacion))
                clr_numero_volumen_valor.BorderWidth = 1
                tblrdatos.AddCell(clr_numero_volumen)
                tblrdatos.AddCell(clr_numero_volumen_valor)
            End If
            '-----------------------------------------
            'Numero folio
            '----------------------------------------
            If estru.numero_folio = True Then
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Folios", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Folios : "
                End If
                Dim clr_numero_folio As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
                clr_numero_folio.BorderWidth = 1
                Dim clr_numero_folio_valor As PdfPCell = New PdfPCell(New Phrase _
                (estru_unidad_conservacion(0).NUMERO_FOLIO_UNIDAD_CONSERVACION, _standardFont_datos_unidad_conservacion))
                clr_numero_folio_valor.BorderWidth = 1
                tblrdatos.AddCell(clr_numero_folio)
                tblrdatos.AddCell(clr_numero_folio_valor)
            End If
            '-----------------------------------------
            'Nombre fondo documental
            '-----------------------------------------
            If estru.nombre_fondo = True Then
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Fondo", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Fondo : "
                End If
                Dim clr_nombre_fondo As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
                clr_nombre_fondo.BorderWidth = 1
                Dim clr_nombre_fondo_valor As PdfPCell = New PdfPCell(New Phrase _
                (estru_unidad_conservacion(0).NOMBRE_FONDO, _standardFont_datos_unidad_conservacion))
                clr_nombre_fondo_valor.BorderWidth = 1
                tblrdatos.AddCell(clr_nombre_fondo)
                tblrdatos.AddCell(clr_nombre_fondo_valor)
            End If
            If estru.nombre_propietario = True Then
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Nombre", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Nombre : "
                End If
                Dim clr_nombre_propietario As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
                clr_nombre_propietario.BorderWidth = 1
                Dim clr_nombre_propietario_valor As PdfPCell = New PdfPCell(New Phrase _
                (estru_unidad_conservacion(0).NOMBRE_PERSONA_EXPEDIENTE, _standardFont_datos_unidad_conservacion))
                clr_nombre_propietario_valor.BorderWidth = 1
                tblrdatos.AddCell(clr_nombre_propietario)
                tblrdatos.AddCell(clr_nombre_propietario_valor)
            End If
            If estru.identificacion_propietario = True Then
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Indetificación", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Indetificación : "
                End If
                Dim clr_identificacion_propietario As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
                clr_identificacion_propietario.BorderWidth = 1
                Dim clr_identificacion_propietario_valor As PdfPCell = New PdfPCell(New Phrase _
                (estru_unidad_conservacion(0).IDENTIFICACION_PERSONA_EXPEDIENTE, _standardFont_datos_unidad_conservacion))
                clr_identificacion_propietario_valor.BorderWidth = 1
                tblrdatos.AddCell(clr_identificacion_propietario)
                tblrdatos.AddCell(clr_identificacion_propietario_valor)
            End If
            If estru.nombre_responsable = True Then
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Responsable", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Responsable : "
                End If
                Dim clr_nombre_propietario As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
                clr_nombre_propietario.BorderWidth = 1
                Dim clr_nombre_propietario_valor As PdfPCell = New PdfPCell(New Phrase _
                (estru_unidad_conservacion(0).NOMBRE_RESPONSABLE_EXPEDIENTE, _standardFont_datos_unidad_conservacion))
                clr_nombre_propietario_valor.BorderWidth = 1
                tblrdatos.AddCell(clr_nombre_propietario)
                tblrdatos.AddCell(clr_nombre_propietario_valor)
            End If
            If estru.identificacion_responsable = True Then
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Identificación responsable", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Identificación responsable : "
                End If
                Dim clr_identificacion_propietario As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
                clr_identificacion_propietario.BorderWidth = 1
                Dim clr_identificacion_propietario_valor As PdfPCell = New PdfPCell(New Phrase _
                (estru_unidad_conservacion(0).IDENFICACION_RESPONSABLE_EXPEDIENTE, _standardFont_datos_unidad_conservacion))
                clr_identificacion_propietario_valor.BorderWidth = 1
                tblrdatos.AddCell(clr_identificacion_propietario)
                tblrdatos.AddCell(clr_identificacion_propietario_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Asunto", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Asunto :"
            End If
            Dim cldescripcion As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
            cldescripcion.BorderWidth = 1
            Dim cldescripcion_valor As PdfPCell = New PdfPCell(New Phrase _
            (nombre_aleas & estru_unidad_conservacion(0).ASUNTO_EXPEDIENTE, _standardFont_datos_unidad_conservacion))
            cldescripcion_valor.BorderWidth = 1
            If estru.Descripcion_unidad = True Then
                'tblrdatos.AddCell(cldescripcion)
                cldescripcion_valor.Colspan = 2
                tblrdatos.AddCell(cldescripcion_valor)
            End If
            '-----------------------------------------
            'Observacion
            '-----------------------------------------
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Observación", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Observación : "
            End If
            Dim cldobservacion As PdfPCell = New PdfPCell(New Phrase(nombre_aleas & estru_unidad_conservacion(0).OBSERVACION_EXPEDIENTE, _standardFont_datos_unidad_conservacion))
            cldobservacion.BorderWidth = 1
            If estru.Observacion = True Then
                cldobservacion.Colspan = 2
                tblrdatos.AddCell(cldobservacion)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Unidad contenedora", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Unidad contenedora :"
            End If
            Dim cldescunidadcontendora As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_datos_unidad_conservacion))
            cldescunidadcontendora.BorderWidth = 1
            Dim cldesunidadcontendora_valor As PdfPCell = New PdfPCell(New Phrase _
            (unidad_contenedora, _standardFont_datos_unidad_conservacion))
            cldesunidadcontendora_valor.BorderWidth = 1
            tblrdatos.AddCell(cldescunidadcontendora)
            tblrdatos.AddCell(cldesunidadcontendora_valor)
            doc.Add(tblrdatos)
            Dim _standardFont_trd_unidad_conservacion As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
           estru.TAM_LETRA_DATOS_TRD, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            If estru.TRD_UNIDAD_CONSERVACION = True Then
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("TABLA DE RETENCION Y CLASIFICACION", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "TABLA DE RETENCION Y CLASIFICACION"
                End If
                paragraf = New Paragraph(nombre_aleas, _standardFont_trd_unidad_conservacion)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
                paragraf = New Paragraph(".", _standardFont_datos_unidad_conservacion)
                paragraf.Alignment = Element.ALIGN_CENTER
                doc.Add(paragraf)
            End If
            '****************************************************
            'Tabla trd
            '****************************************************

            Dim tblPrueba As PdfPTable = New PdfPTable(estru.numero_columnas_datos)
            tblPrueba.WidthPercentage = 100
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Nombre Area (Sección)", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Nombre Area (Sección):"
            End If
            Dim clNombre As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_trd_unidad_conservacion))
            clNombre.BorderWidth = 1
            Dim clNombre_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).NOMBRE_AREA, _standardFont_trd_unidad_conservacion))
            clNombre_valor.BorderWidth = 1
            If estru.Nombre_Area = True Then
                tblPrueba.AddCell(clNombre)
                tblPrueba.AddCell(clNombre_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Nombre sub Area (Sub sebcción)", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Nombre sub Area (Sub sebcción):"
            End If
            Dim clCodigoArea As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_trd_unidad_conservacion))
            clCodigoArea.BorderWidth = 1
            Dim clCodigoArea_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).NOMBRE_SUB_AREA, _standardFont_trd_unidad_conservacion))
            clCodigoArea_valor.BorderWidth = 1
            If estru.Codigo_Area = True Then
                tblPrueba.AddCell(clCodigoArea)
                tblPrueba.AddCell(clCodigoArea_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Nombre Serie", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Nombre Serie :"
            End If
            Dim clSerie As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_trd_unidad_conservacion))
            clSerie.BorderWidth = 1
            Dim clSerie_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).NOMBRE_SERIE, _standardFont_trd_unidad_conservacion))
            clSerie_valor.BorderWidth = 1
            If estru.Nombre_Serie = True Then
                tblPrueba.AddCell(clSerie)
                tblPrueba.AddCell(clSerie_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Código Serie", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Código Serie :"
            End If
            Dim clCodigoSerie As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_trd_unidad_conservacion))
            clCodigoSerie.BorderWidth = 1
            Dim clCodigoSerie_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).CODIGO_SERIE, _standardFont_trd_unidad_conservacion))
            clCodigoSerie_valor.BorderWidth = 1
            If estru.Codigo_Serie = True Then
                tblPrueba.AddCell(clCodigoSerie)
                tblPrueba.AddCell(clCodigoSerie_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Nombre sub Serie", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Nombre sub Serie :"
            End If
            Dim clnombresubSerie As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_trd_unidad_conservacion))
            clnombresubSerie.BorderWidth = 1
            Dim clnombresubSerie_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).NOMBRE_SUBSERIE, _standardFont_trd_unidad_conservacion))
            clnombresubSerie_valor.BorderWidth = 1
            If estru.Nombre_sub_Serie = True Then
                tblPrueba.AddCell(clnombresubSerie)
                tblPrueba.AddCell(clnombresubSerie_valor)
            End If
            nombre_aleas = ""
            Result = Me.Retorna_nombre_aleas_campo_rotulo("Código sub Serie", Stru_aleas_rotulo, nombre_aleas)
            If nombre_aleas = "" Then
                nombre_aleas = "Código sub Serie :"
            End If
            Dim clcodigosubSerie As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_trd_unidad_conservacion))
            clcodigosubSerie.BorderWidth = 1
            Dim clcodigosubSerie_valor As PdfPCell = New PdfPCell(New Phrase(estru_unidad_conservacion(0).CODIGO_SUBSERIE, _standardFont_trd_unidad_conservacion))
            clcodigosubSerie_valor.BorderWidth = 1
            If estru.Codigo_sub_Serie = True Then
                tblPrueba.AddCell(clcodigosubSerie)
                tblPrueba.AddCell(clcodigosubSerie_valor)
            End If
            doc.Add(tblPrueba)
            '***********************************************
            'Tabla ubicacion
            '***********************************************
            Dim _standardFont_ubicacion_unidad As iTextSharp.text.Font = New iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
            estru.TAM_LETRA_UBICACION, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
            If estru_unidad_conservacion(0).ESTADO_ARCHIVO_INIDAD = 1 Then
                Dim tblrubicacion As PdfPTable = New PdfPTable(estru.numero_columnas_datos)
                tblrubicacion.WidthPercentage = 100
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Edificio Archivo", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Edificio Archivo :"
                End If
                Dim cledificio As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_ubicacion_unidad))
                cledificio.BorderWidth = 1
                Dim cledificio_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).edificio_contenedor, _standardFont_ubicacion_unidad))
                cledificio_valor.BorderWidth = 1
                If estru.Edificio = True Then
                    tblrubicacion.AddCell(cledificio)
                    tblrubicacion.AddCell(cledificio_valor)
                End If
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Piso Archivo", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Piso Archivo :"
                End If
                Dim clpiso As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_ubicacion_unidad))
                clpiso.BorderWidth = 1
                Dim clpiso_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).piso_contenedor, _standardFont_ubicacion_unidad))
                clpiso_valor.BorderWidth = 1
                If estru.Piso = True Then
                    tblrubicacion.AddCell(clpiso)
                    tblrubicacion.AddCell(clpiso_valor)
                End If
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Area Archivo", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Area Archivo :"
                End If
                Dim clarea As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_ubicacion_unidad))
                clarea.BorderWidth = 1
                Dim clarea_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).area_contenedor, _standardFont_ubicacion_unidad))
                clarea_valor.BorderWidth = 1
                If estru.Area = True Then
                    tblrubicacion.AddCell(clarea)
                    tblrubicacion.AddCell(clarea_valor)
                End If
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Módulo Archivo", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Módulo Archivo :"
                End If
                Dim clmodulo As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_ubicacion_unidad))
                clmodulo.BorderWidth = 1
                Dim clmodulo_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).modulo_contendor, _standardFont_ubicacion_unidad))
                clmodulo_valor.BorderWidth = 1
                If estru.Modulo = True Then
                    tblrubicacion.AddCell(clmodulo)
                    tblrubicacion.AddCell(clmodulo_valor)
                End If
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Estante Archivo", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Estante Archivo :"
                End If
                Dim clestante As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_ubicacion_unidad))
                clestante.BorderWidth = 1
                Dim clestante_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).estante_contenedor, _standardFont_ubicacion_unidad))
                clestante_valor.BorderWidth = 1
                If estru.Estante = True Then
                    tblrubicacion.AddCell(clestante)
                    tblrubicacion.AddCell(clestante_valor)
                End If
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("Estrepaño Archivo", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "Estrepaño Archivo :"
                End If
                Dim clentrepaño As PdfPCell = New PdfPCell(New Phrase(nombre_aleas, _standardFont_ubicacion_unidad))
                clentrepaño.BorderWidth = 1
                Dim clentrepaño_valor As PdfPCell = New PdfPCell(New Phrase(struentrepaño(id_entrapaño_idex).codigo_corto, _standardFont_ubicacion_unidad))
                clentrepaño_valor.BorderWidth = 1
                If estru.Estrepaño = True Then
                    tblrubicacion.AddCell(clentrepaño)
                    tblrubicacion.AddCell(clentrepaño_valor)
                End If
                nombre_aleas = ""
                Result = Me.Retorna_nombre_aleas_campo_rotulo("UBICACION TOPONIMICA EXPEDIENTE", Stru_aleas_rotulo, nombre_aleas)
                If nombre_aleas = "" Then
                    nombre_aleas = "UBICACION TOPONIMICA EXPEDIENTE"
                End If
                paragraf = New Paragraph(nombre_aleas, _standardFont_ubicacion_unidad)
                paragraf.Alignment = Element.ALIGN_CENTER
                If estru.UBICACION_UNIDAD_CONSERVACION = True Then
                    doc.Add(paragraf)
                    paragraf = New Paragraph(".", _standardFont_datos_unidad_conservacion)
                    paragraf.Alignment = Element.ALIGN_CENTER
                    doc.Add(paragraf)
                End If
                doc.Add(tblrubicacion)
            End If
            If estru.chekmarco = True Then
                Dim pageRect As Rectangle = doc.PageSize
                Dim content = writer.DirectContent
                Dim pageBorderRect = New Rectangle(doc.PageSize)
                pageBorderRect.Left += doc.LeftMargin
                pageBorderRect.Right -= doc.RightMargin
                pageBorderRect.Top -= doc.TopMargin
                pageBorderRect.Bottom += doc.BottomMargin
                content.SetColorStroke(BaseColor.BLACK)
                content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height)
                content.Stroke()
            End If

            Genera_rotulo_Eexpediente_pdf = "YES"
        Catch ex As Exception
            Genera_rotulo_Eexpediente_pdf = "Inconsistencia función : Genera_rotulo_Eexpediente_pdf " & ex.Message
        Finally
            If Not doc Is Nothing Then
                doc.Close()
            End If
            If Not writer Is Nothing Then
                writer.Close()
            End If
        End Try
    End Function

    Function Asigna_Datos_DB_Configuracion_rotulo_expediente_estructura(ByRef estru As rotulo_unidad_conservacion,
                                                                        ByVal id_configiuracion_rotulo As Integer) As String
        '***********************************************************************
        'Función : Asigna datos configuración de rotulo expediente
        ' a la estructura
        'Fecha : 2015-01-27 Modificado para web 2016-08-30
        'Img :Miguel Angel Urueta Miranda
        '***********************************************************************
        Try

            Dim Parametro_consulta As String = "select EJE_X,EJE_Y,chekmarco,numero_columnas_datos,image_empresa" &
            ",nit_empresa,nombre_empresa,DATOS_UNIDAD_CONSERVACION,Codigo_unico,Tema_unidad,Fechas_Extremas,Rangos_Extremos" &
            ",Descripcion_unidad,TRD_UNIDAD_CONSERVACION,Nombre_Area,Codigo_Area,Nombre_Serie,Codigo_Serie,Nombre_sub_Serie," &
            "Codigo_sub_Serie,Edificio,Piso,Area,Estante,Modulo,Estrepano,UBICACION_UNIDAD_CONSERVACION," &
            "TAM_LETRA_TITULO,TAM_LETRA_DATOS_UNIDAD,TAM_LETRA_DATOS_TRD,TAM_LETRA_UBICACION,nombre_plantilla,Observacion, " &
            "numero_folio,numero_volumen,nombre_propietario,identificacion_propietario,nombre_fondo,nombre_responsable,identificacion_responsable,version_trd " &
            " from ra_configuracion_rotulo_expediente where ID_ROTULO_EXPEDIENTE=" & id_configiuracion_rotulo
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_configuracion_rotulo_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_consulta, Datset)
            If Result <> "YES" Then
                Asigna_Datos_DB_Configuracion_rotulo_expediente_estructura = "Función Retorna_id_configuracion_rotulo_por_nombre_plantilla Error dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Asigna_Datos_DB_Configuracion_rotulo_expediente_estructura = "Imposible encontrar configuración rotulo "
                Exit Function
            Else

                estru.x = Datset.Tables(0).Rows(0).Item(0)
                estru.y = Datset.Tables(0).Rows(0).Item(1)
                If Datset.Tables(0).Rows(0).Item(2) = 0 Then
                    estru.chekmarco = False
                Else
                    estru.chekmarco = True
                End If
                estru.numero_columnas_datos = Datset.Tables(0).Rows(0).Item(3)
                If Datset.Tables(0).Rows(0).Item(4) = 0 Then
                    estru.image_empresa = False
                Else
                    estru.image_empresa = True
                End If
                If Datset.Tables(0).Rows(0).Item(5) = 0 Then
                    estru.nit_empresa = False
                Else
                    estru.nit_empresa = True
                End If
                If Datset.Tables(0).Rows(0).Item(6) = 0 Then
                    estru.nombre_empresa = False
                Else
                    estru.nombre_empresa = True
                End If
                If Datset.Tables(0).Rows(0).Item(7) = 0 Then
                    estru.DATOS_UNIDAD_CONSERVACION = False
                Else
                    estru.DATOS_UNIDAD_CONSERVACION = True
                End If
                If Datset.Tables(0).Rows(0).Item(8) = 0 Then
                    estru.Codigo_unico = False
                Else
                    estru.Codigo_unico = True
                End If
                If Datset.Tables(0).Rows(0).Item(9) = 0 Then
                    estru.Tema_unidad = False
                Else
                    estru.Tema_unidad = True
                End If
                If Datset.Tables(0).Rows(0).Item(10) = 0 Then
                    estru.Fechas_Extremas = False
                Else
                    estru.Fechas_Extremas = True
                End If
                If Datset.Tables(0).Rows(0).Item(11) = 0 Then
                    estru.Rangos_Extremos = False
                Else
                    estru.Rangos_Extremos = True
                End If
                If Datset.Tables(0).Rows(0).Item(12) = 0 Then
                    estru.Descripcion_unidad = False
                Else
                    estru.Descripcion_unidad = True
                End If
                If Datset.Tables(0).Rows(0).Item(13) = 0 Then
                    estru.TRD_UNIDAD_CONSERVACION = False
                Else
                    estru.TRD_UNIDAD_CONSERVACION = True
                End If
                If Datset.Tables(0).Rows(0).Item(14) = 0 Then
                    estru.Nombre_Area = False
                Else
                    estru.Nombre_Area = True
                End If
                If Datset.Tables(0).Rows(0).Item(15) = 0 Then
                    estru.Codigo_Area = False
                Else
                    estru.Codigo_Area = True
                End If
                If Datset.Tables(0).Rows(0).Item(16) = 0 Then
                    estru.Nombre_Serie = False
                Else
                    estru.Nombre_Serie = True
                End If
                If Datset.Tables(0).Rows(0).Item(17) = 0 Then
                    estru.Codigo_Serie = False
                Else
                    estru.Codigo_Serie = True
                End If
                If Datset.Tables(0).Rows(0).Item(18) = 0 Then
                    estru.Nombre_sub_Serie = False
                Else
                    estru.Nombre_sub_Serie = True
                End If
                If Datset.Tables(0).Rows(0).Item(19) = 0 Then
                    estru.Codigo_sub_Serie = False
                Else
                    estru.Codigo_sub_Serie = True
                End If
                If Datset.Tables(0).Rows(0).Item(20) = 0 Then
                    estru.Edificio = False
                Else
                    estru.Edificio = True
                End If
                If Datset.Tables(0).Rows(0).Item(21) = 0 Then
                    estru.Piso = False
                Else
                    estru.Piso = True
                End If
                If Datset.Tables(0).Rows(0).Item(22) = 0 Then
                    estru.Area = False
                Else
                    estru.Area = True
                End If
                If Datset.Tables(0).Rows(0).Item(23) = 0 Then
                    estru.Estante = False
                Else
                    estru.Estante = True
                End If
                If Datset.Tables(0).Rows(0).Item(24) = 0 Then
                    estru.Modulo = False
                Else
                    estru.Modulo = True
                End If
                If Datset.Tables(0).Rows(0).Item(25) = 0 Then
                    estru.Estrepaño = False
                Else
                    estru.Estrepaño = True
                End If
                If Datset.Tables(0).Rows(0).Item(26) = 0 Then
                    estru.UBICACION_UNIDAD_CONSERVACION = False
                Else
                    estru.UBICACION_UNIDAD_CONSERVACION = True
                End If
                estru.TAM_LETRA_TITULO = Datset.Tables(0).Rows(0).Item(27)
                estru.TAM_LETRA_DATOS_UNIDAD = Datset.Tables(0).Rows(0).Item(28)
                estru.TAM_LETRA_DATOS_TRD = Datset.Tables(0).Rows(0).Item(29)
                estru.TAM_LETRA_UBICACION = Datset.Tables(0).Rows(0).Item(30)
                estru.nombre_plantilla = Datset.Tables(0).Rows(0).Item(31)
                If Datset.Tables(0).Rows(0).Item(32) = 0 Then
                    estru.Observacion = False
                Else
                    estru.Observacion = True
                End If
                If Datset.Tables(0).Rows(0).Item(33) = 0 Then
                    estru.numero_folio = False
                Else
                    estru.numero_folio = True
                End If
                If Datset.Tables(0).Rows(0).Item(34) = 0 Then
                    estru.numero_volumen = False
                Else
                    estru.numero_volumen = True
                End If
                If Datset.Tables(0).Rows(0).Item(35) = 0 Then
                    estru.nombre_propietario = False
                Else
                    estru.nombre_propietario = True
                End If
                If Datset.Tables(0).Rows(0).Item(36) = 0 Then
                    estru.identificacion_propietario = False
                Else
                    estru.identificacion_propietario = True
                End If
                If Datset.Tables(0).Rows(0).Item(37) = 0 Then
                    estru.nombre_fondo = False
                Else
                    estru.nombre_fondo = True
                End If
                If Datset.Tables(0).Rows(0).Item(38) = 0 Then
                    estru.nombre_responsable = False
                Else
                    estru.nombre_responsable = True
                End If
                If Datset.Tables(0).Rows(0).Item(39) = 0 Then
                    estru.identificacion_responsable = False
                Else
                    estru.identificacion_responsable = True
                End If
                If Datset.Tables(0).Rows(0).Item(40) = 0 Then
                    estru.version_trd = False
                Else
                    estru.version_trd = True
                End If
                Asigna_Datos_DB_Configuracion_rotulo_expediente_estructura = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Asigna_Datos_DB_Configuracion_rotulo_expediente_estructura = "Inconsistencia función " & vbCrLf &
            " Asigna_Datos_DB_Configuracion_rotulo_expediente  " & vbCrLf & ex.Message
        End Try

    End Function
    Function Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                                              ByRef id_configiracion_rotulo As Integer,
                                                                              ByRef nombre_configuracion_rotulo As String) As String
        '------------------------------------------------------------------
        'Funcion : Retorna el nombre de la configuración de la plantilla
        'para imprimir el rotulo del expediente relacionado al usuario
        'de gestión
        'ingeniero : Miguel Angel Urueta Miranda
        'Fecha : 2017-07-29
        '------------------------------------------------------------------
        Try
            Dim Parametro_consulta As String = "select rpr.RA_CONF_ROT_EXP_ID_ROTULO_EXPEDIENTE,rcr.nombre_plantilla" &
           " from RA_PLANTILLA_ROTULO_USUARIO_EXPEDIENTE as rpr " &
           "inner join ra_configuracion_rotulo_expediente as rcr on (rcr.ID_ROTULO_EXPEDIENTE=rpr.RA_CONF_ROT_EXP_ID_ROTULO_EXPEDIENTE)" &
           " where REMIT_DEST_INTERNO_ID_REMIT_DEST_INT=" & id_usuario_gestion
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("RA_PLANTILLA_ROTULO_USUARIO_EXPEDIENTE")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_consulta, Datset)
            If Result <> "YES" Then
                Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion = "Función Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_configiracion_rotulo = 0
                nombre_configuracion_rotulo = ""
                Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion = "YES"
                Exit Function
            Else
                id_configiracion_rotulo = Datset.Tables(0).Rows(0).Item(0)
                nombre_configuracion_rotulo = Datset.Tables(0).Rows(0).Item(1)
                Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion = "Inconsistencia general función Retorna_nombre_configuracion_rotulo_seleccionado_usuario_gestion " & ex.Message
        End Try

    End Function
    Function Retorna_id_nombre_configuracion_rotulo_expediente(ByVal nombre_rotulo_expediente As String,
                                                               ByRef id_configuracion_rotulo_expediente As Integer) As String
        '-----------------------------------------------------------
        'Función : Retorna el id de la configuración de un nombre
        'Fecha : 2017-07-29
        'Ingeniero : Miguel Angel Urueta Miranda
        '------------------------------------------------------------
        Try
            Dim Parametro_consulta As String = "select ID_ROTULO_EXPEDIENTE" &
          " from ra_configuracion_rotulo_expediente " &
          " where nombre_plantilla='" & nombre_rotulo_expediente & "'"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_configuracion_rotulo_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_nombre_configuracion_rotulo_expediente = "Función Retorna_id_nombre_configuracion_rotulo_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_nombre_configuracion_rotulo_expediente = "Imposible encontrar el id la configración del rotulo"
                Exit Function
            Else
                id_configuracion_rotulo_expediente = Datset.Tables(0).Rows(0).Item(0)
                Retorna_id_nombre_configuracion_rotulo_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_nombre_configuracion_rotulo_expediente = "Inconsistencia general función Retorna_id_nombre_configuracion_rotulo_expediente " & ex.Message
        End Try
    End Function
    Function Retorna_listado_configuracion_rotulo_expediente(ByVal nombre_configuracion As String,
                                                             ByRef ref_droplist As DropDownList,
                                                             ByRef ref_update As UpdatePanel) As String

        '---------------------------------------------------------------
        'Función : Retorna el listado de las configuraciones plantillas
        'Fecha : 2017-07-29
        'Ingeniero : Miguel Angel Urueta Miranda
        '---------------------------------------------------------------
        Try
            Dim Parametro_consulta As String = "select nombre_plantilla" &
          " from ra_configuracion_rotulo_expediente "
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_configuracion_rotulo_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_consulta, Datset)
            If Result <> "YES" Then
                Retorna_listado_configuracion_rotulo_expediente = "Función Retorna_listado_configuracion_rotulo_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ref_droplist.Items.Clear()
                ref_update.Update()
                Retorna_listado_configuracion_rotulo_expediente = "YES"
                Exit Function
            Else
                ref_droplist.Items.Clear()
                ref_droplist.Items.Add("")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ref_droplist.Items.Add(Datset.Tables(0).Rows(i).Item(0))
                Next
                For i As Integer = 0 To ref_droplist.Items.Count - 1
                    If nombre_configuracion = ref_droplist.Items(i).Text Then
                        ref_droplist.Items(i).Text = nombre_configuracion
                        ref_droplist.SelectedIndex = i
                        Exit For
                    End If
                Next
                ref_update.Update()
                Retorna_listado_configuracion_rotulo_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_listado_configuracion_rotulo_expediente = "Inconsistencia general función Retorna_listado_configuracion_rotulo_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_numero_volumenes_relacionados(ByVal id_expediente_padre As Integer,
                                                    ByVal id_expediente_hijo As Integer,
                                                    ByRef numero_volumenes As Integer,
                                                    ByRef indice_hijo As Integer) As String
        '-----------------------------------------------------
        'Función : Solicita numero de volumenes relacionados
        ' a un expediente padre
        'Ing Miguel Angel Urueta Miranda
        'Fecha : 2017-12-07
        '-----------------------------------------------------
        Try
            Dim Parametro_consulta As String = "select ID_EXPEDIENTE_PADRE,ID_EXPDIENTE_HIJO" &
            " from ra_relacion_expediente where ID_EXPEDIENTE_PADRE=" & id_expediente_padre
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_relacion_expediente")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_consulta, Datset)
            If Result <> "YES" Then
                Solicita_numero_volumenes_relacionados = "Función Solicita_numero_volumenes_relacionados dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                numero_volumenes = 1
                Solicita_numero_volumenes_relacionados = "YES"
                Exit Function
            Else
                numero_volumenes = Datset.Tables(0).Rows.Count + 1
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).Item(1) = id_expediente_hijo Then
                        indice_hijo = i + 2
                        Exit For
                    End If
                Next
                Solicita_numero_volumenes_relacionados = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_numero_volumenes_relacionados = "Inconsistencia función Solicita_numero_volumenes_relacionados " & ex.Message
        End Try
    End Function
    Function Registra_configuracion_rotulo_expediente_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                                      ByVal id_rotulo_impresion As Integer) As String
        '-----------------------------------------------
        'Función : Registra la plantilla de impresión
        'de rotulo para el usuario de gestión
        'Fecha : 2017-07-29
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------
        Try
            Dim sql_insert As String = "insert into RA_PLANTILLA_ROTULO_USUARIO_EXPEDIENTE (REMIT_DEST_INTERNO_ID_REMIT_DEST_INT,RA_CONF_ROT_EXP_ID_ROTULO_EXPEDIENTE) values " &
           "(" & id_usuario_gestion & "," & id_rotulo_impresion & ")"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Registra_configuracion_rotulo_expediente_usuario_gestion = "Funcion Registra_configuracion_rotulo_expediente_usuario_gestion dice " & Result
                Exit Function
            Else
                Registra_configuracion_rotulo_expediente_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_configuracion_rotulo_expediente_usuario_gestion = "Inconsistencia general función Registra_configuracion_rotulo_expediente_usuario_gestion " & ex.Message
        End Try

    End Function
    Function Actualiza_configuracion_rotulo_expediente_usuario_gestion(ByVal id_usuario_gestion As Integer,
                                                                       ByVal id_rotulo_impresion As Integer) As String
        '-----------------------------------------------
        'Función : Actualiza la plantilla de impresión
        'de rotulo para el usuario de gestión
        'Fecha : 2017-07-29
        'Ing : Miguel Angel Urueta Miranda
        '-----------------------------------------------
        Try
            Dim sql_insert As String = "Update RA_PLANTILLA_ROTULO_USUARIO_EXPEDIENTE set RA_CONF_ROT_EXP_ID_ROTULO_EXPEDIENTE=" &
           "'" & id_rotulo_impresion & "' where REMIT_DEST_INTERNO_ID_REMIT_DEST_INT=" & id_usuario_gestion
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_INSERT_COMMAND(sql_insert)
            If Result <> "YES" Then
                Actualiza_configuracion_rotulo_expediente_usuario_gestion = "Funcion Actualiza_configuracion_rotulo_expediente_usuario_gestion dice " & Result
                Exit Function
            Else
                Actualiza_configuracion_rotulo_expediente_usuario_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_configuracion_rotulo_expediente_usuario_gestion = "Inconsistencia general función Actualiza_configuracion_rotulo_expediente_usuario_gestion " & ex.Message
        End Try
    End Function
    Function Activa_registrar_expediente_conservacion(ByRef pag As Page) As String

        Try
            Dim update_panel_controles As UpdatePanel = pag.FindControl("update_panel_controles")
            Dim Hiddenname_empresagestion As Object = pag.FindControl("Hiddenname_empresagestion")
            Dim DropDownListorganigrama As DropDownList = pag.FindControl("DropDownListorganigrama")
            Dim DropDownList_tipo_unidad_conservacion As DropDownList = pag.FindControl("DropDownList_tipo_unidad_conservacion")
            Dim DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL As DropDownList = pag.FindControl("DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL")
            Dim DropDownListBoxtipoexpediente As DropDownList = pag.FindControl("DropDownListBoxtipoexpediente")
            Dim id_empresa As Integer = 0
            Dim Result As String = ""
            Dim Refclas As New ClassAdmonEmpresa
            Dim Resfclas As New ClassGaTipoDocumental
            id_empresa = HttpContext.Current.Session.Item("GA_IDEMPRESA")
            If HttpContext.Current.Session.Item("SESIONITERCAMBIOEXPEDIENTE") <> "" Then
                Result = Refclas.Retorna_Id_Emprea(HttpContext.Current.Session.Item("SESIONITERCAMBIOEXPEDIENTE"),
                                                   id_empresa)
                If Result <> "YES" Then
                    Activa_registrar_expediente_conservacion = Result
                    Exit Function
                End If
            Else
                Result = Refclas.Retorna_nombre_empresa_usuario_gestion(HttpContext.Current.Session.Item("SESIONITERCAMBIOEXPEDIENTE"),
                                                                        HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    Activa_registrar_expediente_conservacion = Result
                    Exit Function
                Else
                    Hiddenname_empresagestion.Value = HttpContext.Current.Session.Item("SESIONITERCAMBIOEXPEDIENTE")
                End If
            End If
            Dim id_area_usuario_gestion As Integer = 0
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                id_area_usuario_gestion)
            If Result <> "YES" Then
                Activa_registrar_expediente_conservacion = Result
                Exit Function
            End If
            Dim Nombre_area As String = ""
            Result = Class_areas_depart_radicacion.Solicita_nombre_area_departamento(id_area_usuario_gestion,
                                                                                     Nombre_area)
            If Result <> "YES" Then
                Activa_registrar_expediente_conservacion = Result
                Exit Function
            End If
            Dim id_organigrama As Integer = 0
            Dim nombre_organigrama As String = ""
            Result = Class_areas_depart_radicacion.Lista_datos_organigrama_por_codigo_area(id_area_usuario_gestion,
                                                                                           id_organigrama,
                                                                                           nombre_organigrama)
            If Result <> "YES" Then
                Activa_registrar_expediente_conservacion = Result
                Exit Function
            End If
            Dim Refclasunidad As New ClassUnidadConservacion
            Dim Refclas_organigrama As New Class_registro_organigrama
            Result = Refclas_organigrama.Listar_Organigramas_Empresa_Combo_Default(id_empresa,
                                                                                   nombre_organigrama,
                                                                                   DropDownListorganigrama,
                                                                                   update_panel_controles)
            If Result <> "YES" Then
                Activa_registrar_expediente_conservacion = Result
                Exit Function
            End If

            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") = 0 Then
                DropDownListorganigrama.Enabled = False
            Else
                DropDownListorganigrama.Enabled = True
            End If
            '-----------------------------------------
            'Lista tipos de unidades de conservación
            '------------------------------------------
            Result = Refclasunidad.Lista_tipos_unidades_conservacion_expedientes(DropDownList_tipo_unidad_conservacion)
            If Result <> "YES" Then
                Activa_registrar_expediente_conservacion = Result
                Exit Function
            End If
            '----------------------------------------
            'Lista tipos unidades documentales
            '----------------------------------------
            Result = Refclasunidad.lista_tipos_unidades_documentales(DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL)
            If Result <> "YES" Then
                Activa_registrar_expediente_conservacion = Result
                Exit Function
            End If
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            Result = ref_Class_ra_tipo_expediente.lista_tipos_expedientes_Combo(DropDownListBoxtipoexpediente,
                                                                                update_panel_controles,
                                                                                0)
            If Result <> "YES" Then
                Activa_registrar_expediente_conservacion = Result
                Exit Function
            End If
            Dim TextBoxayuda As TextBox = pag.FindControl("TextBoxayuda")
            Dim TextBoxNUMERO_DIGITALIZADO_CONTENIDO As TextBox = pag.FindControl("TextBoxNUMERO_DIGITALIZADO_CONTENIDO")
            Dim TextBoxNUMERO_FOLIOS_CONTENIDOS As TextBox = pag.FindControl("TextBoxNUMERO_FOLIOS_CONTENIDOS")
            Dim TextBoxNUMERO_ELECTRONICO_CONTENIDO As TextBox = pag.FindControl("TextBoxNUMERO_ELECTRONICO_CONTENIDO")
            If DropDownListBoxtipoexpediente.Text <> "" Then
                Result = ref_Class_ra_tipo_expediente.Retorna_ayuda_clase_expediente(DropDownListBoxtipoexpediente.Text,
                                                                                     TextBoxayuda.Text)
                If Result <> "YES" Then
                    Activa_registrar_expediente_conservacion = Result
                    Exit Function
                End If
                TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = True
                TextBoxNUMERO_DIGITALIZADO_CONTENIDO.ReadOnly = True
                TextBoxNUMERO_ELECTRONICO_CONTENIDO.ReadOnly = True
                TextBoxNUMERO_FOLIOS_CONTENIDOS.BackColor = Drawing.Color.Gray
                TextBoxNUMERO_DIGITALIZADO_CONTENIDO.BackColor = Drawing.Color.Gray
                TextBoxNUMERO_ELECTRONICO_CONTENIDO.BackColor = Drawing.Color.Gray
                If DropDownListBoxtipoexpediente.Text = "FISICO" Then
                    TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                End If
                'EXPEDIENTE HIBRIDO
                If DropDownListBoxtipoexpediente.Text = "HIBRIDO" Then
                    TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                End If
                'EXPEDIENTE MIXTO
                If DropDownListBoxtipoexpediente.Text = "MIXTO" Then
                    TextBoxNUMERO_FOLIOS_CONTENIDOS.ReadOnly = False
                    TextBoxNUMERO_ELECTRONICO_CONTENIDO.ReadOnly = False
                End If
            End If
            Dim Hidden_id_empresa As Object = pag.FindControl("Hidden_id_empresa")
            Dim DropDownListArea As DropDownList = pag.FindControl("DropDownListArea")
            Dim DropDownListSerie As DropDownList = pag.FindControl("DropDownListSerie")
            Dim DropDownListSubserie As DropDownList = pag.FindControl("DropDownListSubserie")
            Dim DropDownList_instrumento As DropDownList = pag.FindControl("DropDownList_instrumento")
            Dim DropDownListNOMBRE_CICLO_ARCHIVO As DropDownList = pag.FindControl("DropDownListNOMBRE_CICLO_ARCHIVO")
            Dim Labelresultado As Label = pag.FindControl("Labelresultado")
            Hidden_id_empresa.Value = id_empresa
            Dim Refclas_dos As New ClassGestionDocumental
            DropDownListArea.Items.Clear()
            DropDownListSerie.Items.Clear()
            DropDownListSubserie.Items.Clear()
            DropDownList_instrumento.Items.Clear()
            If DropDownListorganigrama.Text <> "" Then
                '************************************************
                'Consulta el id de la empresa de gestion
                '************************************************
                Dim Reclas_registro_organigrama As New Class_registro_organigrama
                Result = Reclas_registro_organigrama.Retorna_id_organigrama(DropDownListorganigrama.Text,
                                                                            id_empresa,
                                                                            id_organigrama)
                If Result <> "YES" Then
                    Activa_registrar_expediente_conservacion = Result
                    Exit Function
                End If
                If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                    Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default(id_organigrama,
                                                                                                     Nombre_area,
                                                                                                     DropDownListArea)
                    If Result <> "YES" Then
                        Activa_registrar_expediente_conservacion = Result
                        Exit Function
                    End If
                Else
                    Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default_por_id_area(id_organigrama,
                                                                                                                 Nombre_area,
                                                                                                                 id_area_usuario_gestion,
                                                                                                                 DropDownListArea)
                    If Result <> "YES" Then
                        Activa_registrar_expediente_conservacion = Result
                        Exit Function
                    End If

                End If
                Dim RefclasGestionInstrumento As New ClassGaGestionInstrumento
                Result = RefclasGestionInstrumento.Lista_instrumentos_archivisticos_activos(id_organigrama,
                                                                                            DropDownList_instrumento,
                                                                                            update_panel_controles)
                If Result <> "YES" Then
                    Activa_registrar_expediente_conservacion = Result
                    Exit Function
                End If
                Dim id_tipo_instrumento As Integer = 0
                Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
                If Not DropDownList_instrumento.SelectedItem Is Nothing Then
                    If DropDownList_instrumento.SelectedValue <> 0 Then
                        Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(DropDownList_instrumento.SelectedValue,
                                                                                           id_tipo_instrumento)
                        If Result <> "YES" Then
                            Activa_registrar_expediente_conservacion = Result
                            Exit Function
                        End If
                        Result = Me.Lista_ciclo_archivo_instrumento(id_tipo_instrumento,
                                                                    DropDownListNOMBRE_CICLO_ARCHIVO,
                                                                    update_panel_controles)
                        If Result <> "YES" Then
                            Activa_registrar_expediente_conservacion = Result
                            Exit Function
                        End If
                        Result = Me.Seleccion_area_departamento(DropDownListorganigrama.Text,
                                                                DropDownListArea.SelectedItem.Text,
                                                                Val(DropDownList_instrumento.SelectedValue),
                                                                DropDownListSerie,
                                                                DropDownListSubserie,
                                                                update_panel_controles)
                        If Result <> "YES" Then
                            Activa_registrar_expediente_conservacion = Result
                            Exit Function
                        End If
                    End If

                End If
            End If
            '--------------------------------------------------
            'LISTA FONDOS DOCUMENTALES
            '--------------------------------------------------
            Dim DropDownListNOMBRE_FONDO As DropDownList = pag.FindControl("DropDownListNOMBRE_FONDO")
            Result = Me.Listar_fodos_documentales(DropDownListNOMBRE_FONDO, "")
            If Result <> "YES" Then
                Activa_registrar_expediente_conservacion = Result
                Exit Function
            End If
            Activa_registrar_expediente_conservacion = "YES"
        Catch ex As Exception
            Activa_registrar_expediente_conservacion = "Inconsistencia general función Activa_registrar_expediente_conservacion " & ex.Message
        End Try
    End Function
    Function Seleccion_area_departamento(ByVal nombre_organigrama As String,
                                         ByVal nombre_area_departamento As String,
                                         ByVal id_instrumento As Integer,
                                         ByRef DropDownListSerie As DropDownList,
                                         ByRef DropDownListSubserie As DropDownList,
                                         ByRef update_panel_controles As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim id_empresa As Integer = 0
            Dim id_organigrama As Integer = 0
            Dim id_area_departamento As Integer = 0
            Dim Refclas As New ClassAdmonEmpresa
            Dim Refclas_dos As New ClassGestionDocumental
            Dim Ref_class_series As New Class_series_documentales
            DropDownListSerie.Items.Clear()
            DropDownListSubserie.Items.Clear()
            If nombre_organigrama = "" Or nombre_organigrama = "" Then
                Seleccion_area_departamento = "YES"
                Exit Function
            End If
            Result = Refclas.Retorna_id_empresa_usuario_gestion(id_empresa, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Seleccion_area_departamento = Result
                Exit Function
            End If
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama, id_empresa, id_organigrama)
            If Result <> "YES" Then
                Seleccion_area_departamento = Result
                Exit Function
            End If
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If nombre_area_departamento <> "" Then
                Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama,
                                                                                         id_area_departamento,
                                                                                         nombre_area_departamento)
                If Result <> "YES" Then
                    Seleccion_area_departamento = Result
                    Exit Function
                End If
                Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area(id_area_departamento,
                                                                                        id_instrumento,
                                                                                        DropDownListSerie,
                                                                                        update_panel_controles)
                If Result <> "YES" Then
                    Seleccion_area_departamento = Result
                    Exit Function
                End If
            End If
            Seleccion_area_departamento = "YES"
        Catch ex As Exception
            Seleccion_area_departamento = "Inconsistencia general función Seleccion_area_departamento " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function
    Function Seleccion_instrumento(ByVal nombre_organigrama As String,
                                   ByRef nombre_area_departamento As String,
                                   ByVal id_instrumento As Integer,
                                   ByRef DropDownListSerie As DropDownList,
                                   ByRef DropDownListSubserie As DropDownList,
                                   ByRef DropDownListNOMBRE_CICLO_ARCHIVO As DropDownList,
                                   ByRef update_panel_controles As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim id_empresa As Integer = 0
            Dim id_organigrama As Integer = 0
            Dim id_area_departamento As Integer = 0
            Dim Refclas As New ClassAdmonEmpresa
            Dim Refclas_dos As New ClassGestionDocumental
            Dim Ref_class_series As New Class_series_documentales
            DropDownListSerie.Items.Clear()
            DropDownListSubserie.Items.Clear()
            DropDownListNOMBRE_CICLO_ARCHIVO.Items.Clear()
            Result = Refclas.Retorna_id_empresa_usuario_gestion(id_empresa,
                                                                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Seleccion_instrumento = Result
                Exit Function
            End If
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama,
                                                                        id_empresa,
                                                                        id_organigrama)
            If Result <> "YES" Then
                Seleccion_instrumento = Result
                Exit Function
            End If

            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If nombre_area_departamento <> "" Then
                Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama,
                                                                                         id_area_departamento,
                                                                                         nombre_area_departamento)
                If Result <> "YES" Then
                    Seleccion_instrumento = Result
                    Exit Function
                End If
                Result = Ref_class_series.Lista_series_relacionadas_instrumento_id_area(id_area_departamento,
                                                                                        id_instrumento,
                                                                                        DropDownListSerie,
                                                                                        update_panel_controles)
                If Result <> "YES" Then
                    Seleccion_instrumento = Result
                    Exit Function
                End If
            End If

            Dim id_tipo_instrumento As Integer = 0
            Dim RefclasGestionInstrumento As New ClassGaGestionInstrumento
            Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
            If id_instrumento <> 0 Then
                Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(id_instrumento,
                                                                                   id_tipo_instrumento)
                If Result <> "YES" Then
                    Seleccion_instrumento = Result
                    Exit Function
                End If
                Result = Me.Lista_ciclo_archivo_instrumento(id_tipo_instrumento, DropDownListNOMBRE_CICLO_ARCHIVO, update_panel_controles)
                If Result <> "YES" Then
                    Seleccion_instrumento = Result
                    Exit Function
                End If
            End If
            Seleccion_instrumento = "YES"
        Catch ex As Exception
            Seleccion_instrumento = "Inconsistencia general función Seleccion_instrumento " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function
    Function Seleccion_organigrama_editar(ByVal nombre_organigrama As String,
                                          ByVal id_empresa As Integer,
                                          ByRef DropDownListArea As DropDownList,
                                          ByRef DropDownListSerie As DropDownList,
                                          ByRef DropDownListSubserie As DropDownList,
                                          ByRef DropDownList_instrumento As DropDownList,
                                          ByRef DropDownListNOMBRE_CICLO_ARCHIVO As DropDownList,
                                          ByRef Labelresultado As Label,
                                          ByRef update_panel_controles As UpdatePanel,
                                          ByVal id_expediente As Integer) As String
        Try
            Dim Refclas As New ClassAdmonEmpresa
            Dim Resfclas As New ClassGaTipoDocumental
            Dim Refclas_dos As New ClassGestionDocumental
            Dim Result As String = ""
            Dim id_organigrama As Integer = 0
            DropDownListArea.Items.Clear()
            DropDownListSerie.Items.Clear()
            DropDownListSubserie.Items.Clear()
            DropDownList_instrumento.Items.Clear()
            DropDownListNOMBRE_CICLO_ARCHIVO.Items.Clear()
            If nombre_organigrama = "" Then
                Seleccion_organigrama_editar = "YES"
                Exit Function
            End If
            Dim refclascexpediente As New ClassGaExpediente
            Dim stru_expediente() As expediente_conservacion = Nothing
            Result = refclascexpediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                        stru_expediente)
            If Result <> "YES" Then
                Seleccion_organigrama_editar = "Función Asigna_datos_interface_expediente dice 11" & Result
                Exit Function
            End If
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama,
                                                                        id_empresa,
                                                                        id_organigrama)
            If Result <> "YES" Then
                Seleccion_organigrama_editar = Result
                Exit Function
            End If
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                DropDownListArea.Items.Clear()
                Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default(id_organigrama,
                                                                                                 "",
                                                                                                 DropDownListArea)
                If Result <> "YES" Then
                    Seleccion_organigrama_editar = Result
                    Exit Function
                End If
            Else
                DropDownListArea.Items.Clear()
                Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default_por_id_area(id_organigrama,
                                                                                                             "",
                                                                                                             stru_expediente(0).CODIGO_AREA_TRD,
                                                                                                             DropDownListArea)
                If Result <> "YES" Then
                    Seleccion_organigrama_editar = Result
                    Exit Function
                End If

            End If
            Dim RefclasGestionInstrumento As New ClassGaGestionInstrumento
            Result = RefclasGestionInstrumento.Lista_instrumentos_archivisticos_activos(id_organigrama,
                                                                                        DropDownList_instrumento,
                                                                                        update_panel_controles)
            If Result <> "YES" Then
                Seleccion_organigrama_editar = Result
                Exit Function
            End If
            Dim id_tipo_instrumento As Integer = 0
            Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
            If Not DropDownList_instrumento.SelectedItem Is Nothing Then
                If DropDownList_instrumento.SelectedValue <> 0 Then
                    Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(DropDownList_instrumento.SelectedValue,
                                                                                       id_tipo_instrumento)
                    If Result <> "YES" Then
                        Seleccion_organigrama_editar = Result
                        Exit Function
                    End If

                    Result = Me.Lista_ciclo_archivo_instrumento(id_tipo_instrumento,
                                                                DropDownListNOMBRE_CICLO_ARCHIVO,
                                                                update_panel_controles)
                    If Result <> "YES" Then
                        Seleccion_organigrama_editar = Result
                        Exit Function
                    End If
                End If

            End If
            Seleccion_organigrama_editar = "YES"
            Exit Function
        Catch ex As Exception
            Seleccion_organigrama_editar = "Inconsistencia general fución Seleccion_organigrama_editar " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function
    Function Seleccion_organigrama(ByVal nombre_organigrama As String,
                                  ByVal id_empresa As Integer,
                                  ByRef DropDownListArea As DropDownList,
                                  ByRef DropDownListSerie As DropDownList,
                                  ByRef DropDownListSubserie As DropDownList,
                                  ByRef DropDownList_instrumento As DropDownList,
                                  ByRef DropDownListNOMBRE_CICLO_ARCHIVO As DropDownList,
                                  ByRef Labelresultado As Label,
                                  ByRef update_panel_controles As UpdatePanel) As String
        Try
            Dim Refclas As New ClassAdmonEmpresa
            Dim Resfclas As New ClassGaTipoDocumental
            Dim Refclas_dos As New ClassGestionDocumental
            Dim Result As String = ""
            Dim id_organigrama As Integer = 0
            DropDownListArea.Items.Clear()
            DropDownListSerie.Items.Clear()
            DropDownListSubserie.Items.Clear()
            DropDownList_instrumento.Items.Clear()
            DropDownListNOMBRE_CICLO_ARCHIVO.Items.Clear()
            If nombre_organigrama = "" Then
                Seleccion_organigrama = "YES"
                Exit Function
            End If
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama,
                                                                        id_empresa,
                                                                        id_organigrama)
            If Result <> "YES" Then
                Seleccion_organigrama = Result
                Exit Function
            End If
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            Dim Class_remit_dest_interno As New Class_remit_dest_interno
            Dim id_area_usuario_gestion As Integer = 0
            Result = Class_remit_dest_interno.Solicita_identificacion_area_usuario_gestion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                                                id_area_usuario_gestion)
            If Result <> "YES" Then
                Seleccion_organigrama = Result
                Exit Function
            End If
            Dim Nombre_area As String = ""
            Result = Class_areas_depart_radicacion.Solicita_nombre_area_departamento(id_area_usuario_gestion,
                                                                                     Nombre_area)
            If Result <> "YES" Then
                Seleccion_organigrama = Result
                Exit Function
            End If
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
                DropDownListArea.Items.Clear()
                Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default(id_organigrama,
                                                                                                 "",
                                                                                                 DropDownListArea)
                If Result <> "YES" Then
                    Seleccion_organigrama = Result
                    Exit Function
                End If
            Else
                DropDownListArea.Items.Clear()
                Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series_Default_por_id_area(id_organigrama,
                                                                                                             "",
                                                                                                             id_area_usuario_gestion,
                                                                                                             DropDownListArea)
                If Result <> "YES" Then
                    Seleccion_organigrama = Result
                    Exit Function
                End If

            End If
            'Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            'If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 0 Then
            '    DropDownListArea.Items.Clear()
            '    Result = Class_areas_depart_radicacion.Lista_AreasDep_Organigrama_Series(id_organigrama, _
            '                                                                             DropDownListArea)
            '    If Result <> "YES" Then
            '        Seleccion_organigrama = Result
            '        Exit Function
            '    End If
            'Else

            '    Result = Refclas_dos.lista_areas_permitidas_usuario_gestion_organigrama(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"), _
            '                                                                            id_organigrama, _
            '                                                                            DropDownListArea)
            '    If Result <> "YES" Then
            '        Seleccion_organigrama = Result
            '        Exit Function
            '    Else
            '        If DropDownListArea.Items.Count = 0 Then
            '            Labelresultado.Text = "El usuario no tiene areas permitidas para clasificar "
            '        End If
            '    End If
            'End If
            Dim RefclasGestionInstrumento As New ClassGaGestionInstrumento
            Result = RefclasGestionInstrumento.Lista_instrumentos_archivisticos_activos(id_organigrama,
                                                                                        DropDownList_instrumento,
                                                                                        update_panel_controles)
            If Result <> "YES" Then
                Seleccion_organigrama = Result
                Exit Function
            End If
            Dim id_tipo_instrumento As Integer = 0
            Dim Ref_class_gestion_instrumento As New Class_ra_registro_instrumento_archivistico
            If Not DropDownList_instrumento.SelectedItem Is Nothing Then
                If DropDownList_instrumento.SelectedValue <> 0 Then
                    Result = Ref_class_gestion_instrumento.Retorna_id_tipo_instrumento(DropDownList_instrumento.SelectedValue,
                                                                                       id_tipo_instrumento)
                    If Result <> "YES" Then
                        Seleccion_organigrama = Result
                        Exit Function
                    End If

                    Result = Me.Lista_ciclo_archivo_instrumento(id_tipo_instrumento,
                                                                DropDownListNOMBRE_CICLO_ARCHIVO,
                                                                update_panel_controles)
                    If Result <> "YES" Then
                        Seleccion_organigrama = Result
                        Exit Function
                    End If
                End If

            End If
            Seleccion_organigrama = "YES"
            Exit Function
        Catch ex As Exception
            Seleccion_organigrama = "Inconsistencia general fución Seleccion_organigrama " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function
    Function Seleccion_serie_documental(ByVal nombre_organigrama As String,
                                        ByVal nombre_area_departamento As String,
                                        ByVal id_instrumento As Integer,
                                        ByVal nombre_serie As String,
                                        ByRef DropDownListSubserie As DropDownList,
                                        ByRef update_panel_controles As UpdatePanel) As String
        Try
            Dim Result As String = ""
            Dim id_empresa As Integer = 0
            Dim id_organigrama As Integer = 0
            Dim id_area_departamento As Integer = 0
            Dim Refclas As New ClassAdmonEmpresa
            Dim Refclas_dos As New ClassGestionDocumental
            Dim Refclas_Trd_documental As New ClassTrdDocumental
            DropDownListSubserie.Items.Clear()
            Result = Refclas.Retorna_id_empresa_usuario_gestion(id_empresa, HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
            If Result <> "YES" Then
                Seleccion_serie_documental = Result
                Exit Function
            End If
            Dim Reclas_registro_organigrama As New Class_registro_organigrama
            Result = Reclas_registro_organigrama.Retorna_id_organigrama(nombre_organigrama, id_empresa, id_organigrama)
            If Result <> "YES" Then
                Seleccion_serie_documental = Result
                Exit Function
            End If
            Dim ref_Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If nombre_area_departamento <> "" Then
                Result = ref_Class_areas_depart_radicacion.Retorna_cod_Area_Departamento(id_organigrama,
                                                                                         id_area_departamento,
                                                                                         nombre_area_departamento)
                If Result <> "YES" Then
                    Seleccion_serie_documental = Result
                    Exit Function
                End If
                Dim id_serie As Integer = 0
                Dim consecutivo_serie As Integer = 0
                Dim consecutivo_Sub_serie As Integer = 0
                Dim ref_Class_series_documentales As New Class_series_documentales
                If nombre_serie <> "" Then
                    Result = ref_Class_series_documentales.Retorna_Id_serie_instrumento_Documental(id_area_departamento,
                                                                                                   nombre_serie,
                                                                                                   id_instrumento,
                                                                                                   id_serie,
                                                                                                   consecutivo_serie,
                                                                                                   consecutivo_Sub_serie)
                    If Result <> "YES" Then
                        Seleccion_serie_documental = Result
                        Exit Function
                    End If
                    Result = Refclas_dos.Listar_SubSeries_Documentales(id_serie,
                                                                       DropDownListSubserie)
                    If Result <> "YES" Then
                        Seleccion_serie_documental = Result
                        Exit Function
                    End If
                End If
            End If
            Seleccion_serie_documental = "YES"
        Catch ex As Exception
            Seleccion_serie_documental = "Inconsistencia general función Seleccion_serie_documental " & ex.Message
        Finally
            update_panel_controles.Update()
        End Try
    End Function
    Function Lista_ciclo_archivo_instrumento(ByVal id_tipo_instrumento As Integer,
                                            ByVal drop_list As DropDownList,
                                            ByRef update As UpdatePanel) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_de_tipos_ciclos_archivo")
            drop_list.Items.Clear()
            Dim sql_consulta As String = "Select Id_tipos_ciclo_archivo,Nombre_Tipo_ciclo_archivo from ra_de_tipos_ciclos_archivo " &
                " where estado_ciclo_archivo=" & id_tipo_instrumento
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_ciclo_archivo_instrumento = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_ciclo_archivo_instrumento = "YES"
                Exit Function
            Else
                Dim ilis_ As System.Web.UI.WebControls.ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New System.Web.UI.WebControls.ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                ilis_ = New System.Web.UI.WebControls.ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                Lista_ciclo_archivo_instrumento = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_ciclo_archivo_instrumento = "Inconsistencia general función Lista_ciclo_archivo_instrumento " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Lista_ciclo_archivo_instrumento_default(ByVal id_tipo_instrumento As Integer,
                                                     ByVal id_tipo_ciclo As Integer,
                                                     ByVal drop_list As DropDownList,
                                                     ByRef update As UpdatePanel) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("ra_de_tipos_ciclos_archivo")
            drop_list.Items.Clear()
            Dim sql_consulta As String = "Select Id_tipos_ciclo_archivo,Nombre_Tipo_ciclo_archivo from ra_de_tipos_ciclos_archivo " &
                " where estado_ciclo_archivo=" & id_tipo_instrumento
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Lista_ciclo_archivo_instrumento_default = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                drop_list.Items.Clear()
                Lista_ciclo_archivo_instrumento_default = "YES"
                Exit Function
            Else
                Dim ilis_ As System.Web.UI.WebControls.ListItem
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilis_ = New System.Web.UI.WebControls.ListItem
                    ilis_.Text = Datset.Tables(0).Rows(i).Item(1)
                    ilis_.Value = Datset.Tables(0).Rows(i).Item(0)
                    drop_list.Items.Add(ilis_)
                Next
                ilis_ = New System.Web.UI.WebControls.ListItem
                ilis_.Text = ""
                ilis_.Value = 0
                drop_list.Items.Add(ilis_)
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Value = id_tipo_ciclo Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_ciclo_archivo_instrumento_default = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Lista_ciclo_archivo_instrumento_default = "Inconsistencia general función Lista_ciclo_archivo_instrumento_default " & ex.Message
        Finally
            update.Update()
        End Try
    End Function
    Function Retorna_tiempos_de_retencion_tablas_retencion(ByVal id_serie As Integer,
                                                           ByVal id_sub_serie As Integer,
                                                           ByVal fecha_expediente As String,
                                                           ByRef fecha_ret_gestion As String,
                                                           ByRef fecha_ret_central As String)
        Try
            Dim Result As String = ""
            Dim tiempo_ret_gestion As Integer = 0
            Dim tiempo_ret_central As Integer = 0
            If id_sub_serie <> 0 Then
                Result = Me.Solicita_tiempos_de_retencion_sub_serie_documental(id_sub_serie,
                                                                              tiempo_ret_gestion,
                                                                              tiempo_ret_central)
                If Result <> "YES" Then
                    Retorna_tiempos_de_retencion_tablas_retencion = Result
                    Exit Function
                End If
                If tiempo_ret_gestion = 0 And tiempo_ret_central = 0 Then
                    Retorna_tiempos_de_retencion_tablas_retencion = "La sub serie documental no tiene tiempos de retención, consulte con su administrador"
                    Exit Function
                End If
                If tiempo_ret_gestion = 0 Then
                    Retorna_tiempos_de_retencion_tablas_retencion = "La sub serie documental no tiene tiempo de retención en el archivo de gestión, consulte con su administrador"
                    Exit Function
                End If
                Result = Me.Suma_anualidad(fecha_expediente,
                                           tiempo_ret_gestion,
                                           fecha_ret_gestion)
                If Result <> "YES" Then
                    Retorna_tiempos_de_retencion_tablas_retencion = Result
                    Exit Function
                End If
                If tiempo_ret_central <> 0 Then
                    Result = Me.Suma_anualidad(fecha_ret_gestion,
                                               tiempo_ret_central,
                                               fecha_ret_central)
                    If Result <> "YES" Then
                        Retorna_tiempos_de_retencion_tablas_retencion = Result
                        Exit Function
                    End If
                End If
            Else
                Result = Me.Solicita_tiempos_de_retencion_serie_documental(id_serie,
                                                                            tiempo_ret_gestion,
                                                                            tiempo_ret_central)
                If Result <> "YES" Then
                    Retorna_tiempos_de_retencion_tablas_retencion = Result
                    Exit Function
                End If
                If tiempo_ret_gestion = 0 And tiempo_ret_central = 0 Then
                    Retorna_tiempos_de_retencion_tablas_retencion = "La serie documental no tiene tiempos de retención, consulte con su administrador"
                    Exit Function
                End If
                If tiempo_ret_gestion = 0 Then
                    Retorna_tiempos_de_retencion_tablas_retencion = "La serie documental no tiene tiempo de retención en el archivo de gestión, consulte con su administrador"
                    Exit Function
                End If
                Result = Me.Suma_anualidad(fecha_expediente,
                                           tiempo_ret_gestion,
                                           fecha_ret_gestion)
                If Result <> "YES" Then
                    Retorna_tiempos_de_retencion_tablas_retencion = Result
                    Exit Function
                End If
                If tiempo_ret_central <> 0 Then
                    Result = Me.Suma_anualidad(fecha_ret_gestion,
                                               tiempo_ret_central,
                                               fecha_ret_central)
                    If Result <> "YES" Then
                        Retorna_tiempos_de_retencion_tablas_retencion = Result
                        Exit Function
                    End If
                End If
            End If
            Retorna_tiempos_de_retencion_tablas_retencion = "YES"
        Catch ex As Exception
            Retorna_tiempos_de_retencion_tablas_retencion = "Inconsistencia general función Retorna_tiempos_de_retencion_tablas_retencion " & ex.Message
        End Try
    End Function
    Function Solicita_tiempos_de_retencion_sub_serie_documental(ByVal id_sub_serie As Integer,
                                                                ByRef tiempo_ret_gestion As Integer,
                                                                ByRef tiempo_ret_central As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("subseries_documentales")
            Dim sql_consulta As String = "Select TIEMPO_RET_ARCH_GESTION,TIEMPO_RET_ARCH_CENTRAL from subseries_documentales " &
                " where Id_SubSeries=" & id_sub_serie
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_tiempos_de_retencion_sub_serie_documental = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                tiempo_ret_gestion = 0
                tiempo_ret_central = 0
                Solicita_tiempos_de_retencion_sub_serie_documental = "YES"
                Exit Function
            Else
                tiempo_ret_gestion = Datset.Tables(0).Rows(0).Item(0)
                tiempo_ret_central = Datset.Tables(0).Rows(0).Item(1)
                Solicita_tiempos_de_retencion_sub_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tiempos_de_retencion_sub_serie_documental = "Inconsistencia general función Solicita_tiempos_de_retencion_sub_serie_documental " & ex.Message
        End Try
    End Function
    Function Solicita_tiempos_de_retencion_serie_documental(ByVal id_serie As Integer,
                                                                ByRef tiempo_ret_gestion As Integer,
                                                                ByRef tiempo_ret_central As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("series_documentales")
            Dim sql_consulta As String = "Select Tiempo_Ret_Arch_Gestion,Tiempo_Ret_Arch_Central from series_documentales " &
                " where Id_Series=" & id_serie
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_tiempos_de_retencion_serie_documental = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                tiempo_ret_gestion = 0
                tiempo_ret_central = 0
                Solicita_tiempos_de_retencion_serie_documental = "YES"
                Exit Function
            Else
                tiempo_ret_gestion = Datset.Tables(0).Rows(0).Item(0)
                tiempo_ret_central = Datset.Tables(0).Rows(0).Item(1)
                Solicita_tiempos_de_retencion_serie_documental = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tiempos_de_retencion_serie_documental = "Inconsistencia general función Solicita_tiempos_de_retencion_serie_documental " & ex.Message
        End Try
    End Function
    Function Suma_anualidad(ByVal fecha_ As String,
                            ByVal tiempo As Integer,
                            ByRef fecha_retencion As String) As String
        Try
            Dim Fecha As Date = fecha_
            Dim fecha_finish As String = CStr(Fecha.AddYears(tiempo).ToString("yyyy'-'MM'-'dd HH':'mm':'ss"))
            fecha_finish = Microsoft.VisualBasic.Left(fecha_finish, 10)
            fecha_retencion = fecha_finish
            Suma_anualidad = "YES"
        Catch ex As Exception
            Suma_anualidad = "Inconsistencia función Suma_anualidad " & ex.Message
        End Try
    End Function
    Function Retorna_tiempos_de_retencion_tablas_de_valoracion(ByVal id_serie As Integer,
                                                         ByVal id_sub_serie As Integer,
                                                         ByVal fecha_expediente As String,
                                                         ByRef fecha_ret_central As String)
        Try
            Dim Result As String = ""
            Dim tiempo_ret_gestion As Integer = 0
            Dim tiempo_ret_central As Integer = 0
            If id_sub_serie <> 0 Then
                Result = Me.Solicita_tiempos_de_retencion_sub_serie_documental(id_sub_serie,
                                                                             tiempo_ret_gestion,
                                                                             tiempo_ret_central)
                If Result <> "YES" Then
                    Retorna_tiempos_de_retencion_tablas_de_valoracion = Result
                    Exit Function
                End If
                If tiempo_ret_central = 0 Then
                    Retorna_tiempos_de_retencion_tablas_de_valoracion = "La sub serie documental no tiene tiempos de retención, consulte con su administrador"
                    Exit Function
                End If

                Result = Me.Suma_anualidad(fecha_expediente, tiempo_ret_central, fecha_ret_central)
                If Result <> "YES" Then
                    Retorna_tiempos_de_retencion_tablas_de_valoracion = Result
                    Exit Function
                End If

            Else
                Result = Me.Solicita_tiempos_de_retencion_serie_documental(id_serie,
                                                                            tiempo_ret_gestion,
                                                                            tiempo_ret_central)
                If Result <> "YES" Then
                    Retorna_tiempos_de_retencion_tablas_de_valoracion = Result
                    Exit Function
                End If
                If tiempo_ret_central = 0 Then
                    Retorna_tiempos_de_retencion_tablas_de_valoracion = "La serie documental no tiene tiempos de retención, consulte con su administrador"
                    Exit Function
                End If
                Result = Me.Suma_anualidad(fecha_expediente, tiempo_ret_central, fecha_ret_central)
                If Result <> "YES" Then
                    Retorna_tiempos_de_retencion_tablas_de_valoracion = Result
                    Exit Function
                End If

            End If
            Retorna_tiempos_de_retencion_tablas_de_valoracion = "YES"
        Catch ex As Exception
            Retorna_tiempos_de_retencion_tablas_de_valoracion = "Inconsistencia general función Retorna_tiempos_de_retencion_tablas_retencion " & ex.Message
        End Try
    End Function
    Function Verifica_propietario_expediente(ByVal id_expeiente As Long,
                                             ByRef estado_propietario As Integer) As String
        Try
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("GA_MANAGER_GESTION") <> 1 Then
                Result = Me.Verifica_propiedad_usuario_expediente(id_expeiente,
                                                                  HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"))
                If Result <> "YES" Then
                    estado_propietario = 0
                    Verifica_propietario_expediente = Result
                    Exit Function
                Else
                    estado_propietario = 1
                    Verifica_propietario_expediente = "YES"
                End If
            Else
                estado_propietario = 1
                Verifica_propietario_expediente = "YES"
            End If
        Catch ex As Exception
            Verifica_propietario_expediente = "Inconistencia general función Verifica_propietario_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_expedientes_para_relacionar_volumenes(ByVal valor_consulta As String,
                                                            ByRef reflabel As Label,
                                                            ByRef grediview As GridView,
                                                            ByRef hideselecion As Object,
                                                            ByRef update As UpdatePanel,
                                                            ByRef update_title As UpdatePanel,
                                                            ByVal option_solo_palablra_completa As Integer) As String
        Try
            Dim sql_consulta As String = ""
            If option_solo_palablra_completa = 0 Then
                sql_consulta = "Select ID_EXPEDIENTE as CODIGO_UNICO, CODIGO_UNICO AS CONSECUTIVO,NOMBRE_SERIE_TRD AS SERIE,NOMBRE_SUBSERIE_TRD AS SUBSERIE, FECHA_CREACION AS FECHA_REGISTRO from " &
             " expediente_archivo as rdi " &
             " WHERE " & "(  ID_EXPEDIENTE like '%" & valor_consulta & "%'" &
               " or rdi.CODIGO_UNICO like '%" & valor_consulta & "%'" &
               " or rdi.FECHA_CREACION like '%" & valor_consulta & "%'" &
               " or TEMA_EXPEDIENTE like '%" & valor_consulta & "%'" &
               " or rdi.ASUNTO_EXPEDIENTE like '%" & valor_consulta & "%')" &
               " and rdi.EXPEDIENTE_PADRE is " & "null" &
               " order by CODIGO_UNICO,ID_EXPEDIENTE"
            Else
                sql_consulta = "Select ID_EXPEDIENTE as CODIGO_UNICO, CODIGO_UNICO AS CONSECUTIVO,NOMBRE_SERIE_TRD AS SERIE,NOMBRE_SUBSERIE_TRD AS SUBSERIE, FECHA_CREACION AS FECHA_REGISTRO from " &
          " expediente_archivo as rdi " &
          " WHERE " & "(  ID_EXPEDIENTE = '" & valor_consulta & "'" &
            " or rdi.CODIGO_UNICO = '" & valor_consulta & "'" &
            " or rdi.FECHA_CREACION = '" & valor_consulta & "'" &
            " or TEMA_EXPEDIENTE = '" & valor_consulta & "'" &
            " or rdi.ASUNTO_EXPEDIENTE = '" & valor_consulta & "')" &
            " and rdi.EXPEDIENTE_PADRE is " & "null" &
            " order by CODIGO_UNICO,ID_EXPEDIENTE"
            End If

            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_expedientes_para_relacionar_volumenes = "Error función Solicita_expedientes_para_relacionar_volumenes  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Solicita_expedientes_para_relacionar_volumenes = "YES"
                Exit Function
            Else
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
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
                    ihtml.Attributes.Add("class", "fal fa-folder-plus fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Relaciona expediente")
                    ahtml.Attributes.Add("idd_image_rel_", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "relacion_rel_exp_")
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
                Solicita_expedientes_para_relacionar_volumenes = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_expedientes_para_relacionar_volumenes = "Inconsistencia general función Solicita_expedientes_para_relacionar_volumenes " & ex.Message
        End Try
    End Function
    Function Solicita_expedientes_para_relacionar_expdiente_padre(ByVal valor_consulta As String,
                                                                  ByRef reflabel As Label,
                                                                  ByRef grediview As GridView,
                                                                  ByRef hideselecion As Object,
                                                                  ByRef update As UpdatePanel,
                                                                  ByRef update_title As UpdatePanel,
                                                                  ByVal option_solo_palablra_completa As Integer) As String
        Try
            Dim sql_consulta As String = ""
            If option_solo_palablra_completa = 0 Then
                sql_consulta = "Select ID_EXPEDIENTE as CODIGO_UNICO, CODIGO_UNICO AS CONSECUTIVO,NOMBRE_SERIE_TRD AS SERIE,NOMBRE_SUBSERIE_TRD AS SUBSERIE, FECHA_CREACION AS FECHA_REGISTRO from " &
             " expediente_archivo as rdi " &
             " WHERE " & "(  ID_EXPEDIENTE like '%" & valor_consulta & "%'" &
               " or rdi.CODIGO_UNICO like '%" & valor_consulta & "%'" &
               " or rdi.FECHA_CREACION like '%" & valor_consulta & "%'" &
               " or TEMA_EXPEDIENTE like '%" & valor_consulta & "%'" &
               " or rdi.ASUNTO_EXPEDIENTE like '%" & valor_consulta & "%')" &
               " and rdi.EXPEDIENTE_PADRE is " & "null" &
               " order by CODIGO_UNICO,ID_EXPEDIENTE"
            Else
                sql_consulta = "Select ID_EXPEDIENTE as CODIGO_UNICO, CODIGO_UNICO AS CONSECUTIVO,NOMBRE_SERIE_TRD AS SERIE,NOMBRE_SUBSERIE_TRD AS SUBSERIE, FECHA_CREACION AS FECHA_REGISTRO from " &
          " expediente_archivo as rdi " &
          " WHERE " & "(  ID_EXPEDIENTE = '" & valor_consulta & "'" &
            " or rdi.CODIGO_UNICO = '" & valor_consulta & "'" &
            " or rdi.FECHA_CREACION = '" & valor_consulta & "'" &
            " or TEMA_EXPEDIENTE = '" & valor_consulta & "'" &
            " or rdi.ASUNTO_EXPEDIENTE = '" & valor_consulta & "')" &
            " and rdi.EXPEDIENTE_PADRE is " & "null" &
            " order by CODIGO_UNICO,ID_EXPEDIENTE"
            End If

            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_expedientes_para_relacionar_expdiente_padre = "Error función Solicita_expedientes_para_relacionar_expdiente_padre  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "Se encontraron 0 registro(s) "
                grediview.DataSource = Nothing
                hideselecion.value = "-1"
                grediview.DataBind()
                update.Update()
                update_title.Update()
                Solicita_expedientes_para_relacionar_expdiente_padre = "YES"
                Exit Function
            Else
                reflabel.Text = "Se encontraron " & Datset.Tables(0).Rows.Count & " registro(s) "
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
                    ihtml.Attributes.Add("class", "fal fa-folder-plus fa-lg")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-primary btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Relaciona expediente")
                    ahtml.Attributes.Add("idd_image_rel_padre", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "relacion_rel_exp_padre")
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
                Solicita_expedientes_para_relacionar_expdiente_padre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_expedientes_para_relacionar_expdiente_padre = "Inconsistencia general función Solicita_expedientes_para_relacionar_expdiente_padre " & ex.Message
        End Try
    End Function
    Function SolicitaGabineteProducionExpediente(ByVal IdExpediente As Integer,
                                                 ByRef NombreGabinete As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el nombre del gabinete relacionado al expediente de produccion
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdExpediente        : Representa la identificación del expediente
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete     : Retorna el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim sql_consulta As String = "Select GABINETE_PRODUCION from expediente_archivo " &
                " where ID_EXPEDIENTE=" & IdExpediente
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaGabineteProducionExpediente = "Función SolicitaGabineteProducionExpediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaGabineteProducionExpediente = "Imposible encontrar el nombre del gabinete de producción del expediente (" & IdExpediente & ")"
                Exit Function
            Else
                NombreGabinete = Datset.Tables(0).Rows(0).Item(0)
                SolicitaGabineteProducionExpediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaGabineteProducionExpediente = "Inconsistencia general función Solicita_gabinete_producion_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_ruta_xml_expediente(ByVal id_expediente As Integer,
                                          ByRef Ruta_archivo_xml As String) As String
        Try
            Dim Result As String = ""
            Dim stru_produccion_indice() As stru_produccion_indice = Nothing
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Result = ClassGaProducionDocumental.Solicita_estructura_registro_relacion_expediente_indice(id_expediente,
                                                                                                        stru_produccion_indice)
            If Result <> "YES" Then
                Solicita_ruta_xml_expediente = Result
                Exit Function
            End If
            Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
            Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
            Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
            If Result <> "YES" Then
                Solicita_ruta_xml_expediente = Result
                Exit Function
            End If
            Dim disco_carpeta As String = stru_ruta_expediente_.DISCO
            Dim class_zerro_fill As New Class_zero_fill
            Result = class_zerro_fill.zero_fill(disco_carpeta, 9, "0")
            If Result <> "YES" Then
                Solicita_ruta_xml_expediente = Result
                Exit Function
            End If
            Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
            If Directory.Exists(Ruta_expediente) = False Then
                Solicita_ruta_xml_expediente = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
                Exit Function
            End If
            Ruta_expediente = Ruta_expediente & disco_carpeta
            If Directory.Exists(Ruta_expediente) = False Then
                Directory.CreateDirectory(Ruta_expediente)
            End If

            Dim expediente_zero_fil As String = id_expediente.ToString
            Result = class_zerro_fill.zero_fill(expediente_zero_fil, 9, "0")
            If Result <> "YES" Then
                Solicita_ruta_xml_expediente = Result
                Exit Function
            End If
            Ruta_archivo_xml = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
            Solicita_ruta_xml_expediente = "YES"
        Catch ex As Exception
            Solicita_ruta_xml_expediente = "Inconsistencia general funcion Solicita_ruta_xml_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_archivo_indice_expediente(ByVal id_expediente As Integer,
                                                ByRef Ruta_archivo_xml As String) As String
        Try
            Dim Result As String = ""
            Dim stru_ruta_expediente_ As stru_ruta_expediente = Nothing
            Dim ref_ra_ruta_expediente As New Class_ra_ruta_expediente
            Dim stru_produccion_indice As stru_produccion_indice = Nothing
            Result = ref_ra_ruta_expediente.Solicita_datos_estructura_ruta_expediente(stru_ruta_expediente_)
            If Result <> "YES" Then
                Solicita_archivo_indice_expediente = Result
                Exit Function
            End If
            Dim disco_carpeta_ As String = stru_ruta_expediente_.DISCO
            Dim class_zerro_fill_ As New Class_zero_fill
            Result = class_zerro_fill_.zero_fill(disco_carpeta_, 9, "0")
            If Result <> "YES" Then
                Solicita_archivo_indice_expediente = Result
                Exit Function
            End If
            Dim Ruta_expediente As String = stru_ruta_expediente_.RUTA.Replace("/", "\")
            If Directory.Exists(Ruta_expediente) = False Then
                Solicita_archivo_indice_expediente = "Por favor crea la siguiente ruta en el servidor " & Ruta_expediente
                Exit Function
            End If
            Ruta_expediente = Ruta_expediente & disco_carpeta_
            If Directory.Exists(Ruta_expediente) = False Then
                Directory.CreateDirectory(Ruta_expediente)
            End If
            Dim expediente_zero_fil As String = id_expediente.ToString
            Result = class_zerro_fill_.zero_fill(expediente_zero_fil, 9, "0")
            If Result <> "YES" Then
                Solicita_archivo_indice_expediente = Result
                Exit Function
            End If
            Ruta_archivo_xml = Ruta_expediente & "\" & expediente_zero_fil & ".xml"
            If File.Exists(Ruta_archivo_xml) = False Then
                Dim expediente_conservacion() As expediente_conservacion = Nothing
                Dim classGaExpediente As New ClassGaExpediente
                Result = classGaExpediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                           expediente_conservacion)
                If Result <> "YES" Then
                    Solicita_archivo_indice_expediente = Result
                    Exit Function
                End If
                Dim Ref_class_remit_dest_interno As New Class_remit_dest_interno
                Dim cargo_usuario_gestion As String = ""
                Dim nombre_usuario_gestion As String = ""
                Dim correo_electronico As String = ""
                Result = Ref_class_remit_dest_interno.Retorna_datos_caracterizacion_usuario_gestion(expediente_conservacion(0).ID_USUARIO_GESTION,
                                                                                                    nombre_usuario_gestion,
                                                                                                    cargo_usuario_gestion,
                                                                                                    correo_electronico)
                If Result <> "YES" Then
                    Solicita_archivo_indice_expediente = Result
                    Exit Function
                End If
                Dim nombre_empresa As String = ""
                Dim Ref_class_empresa As New Class_empresa_gestion_documental
                Result = Ref_class_empresa.Solicita_nombre_identificacion_empresa("",
                                                                                 nombre_empresa)
                If Result <> "YES" Then
                    Solicita_archivo_indice_expediente = Result
                    Exit Function
                End If
                Result = classGaExpediente.Registra_archivo_xml_indice_expediente(Ruta_archivo_xml,
                                                                                 id_expediente,
                                                                                 expediente_conservacion(0).FECHA_CREACION,
                                                                                 nombre_usuario_gestion,
                                                                                 nombre_empresa,
                                                                                 expediente_conservacion(0).DESCRIPCION_UNIDAD_CONSERVACION,
                                                                                 Nothing)
                If Result <> "YES" Then
                    Solicita_archivo_indice_expediente = Result
                    Exit Function
                End If
            End If
            Solicita_archivo_indice_expediente = "YES"
        Catch ex As Exception
            Solicita_archivo_indice_expediente = "Inconsistencia general funcion Solicita_archivo_indice_expediente " & ex.Message
        End Try
    End Function
    Function Vincula_documento_gabinete_expediente_migracion(ByVal id_expediente As Integer,
                                                             ByVal id_imagen As Integer,
                                                             ByVal gabinete As String,
                                                             ByRef existencia_vinculacion As String,
                                                             ByRef nombre_expediente_relacion As String) As String

        '--------------------------------------------------------------------------------
        'Funcion : Vinculación de un documento a un expediente
        '          desde un gabinete
        '--------------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------------
        'id_imagen             : Representa la identificación de la imagen en un gabinete
        'id_expediente         : Representa la identificación de un expediente
        'gabinete              : Representa la identificación del gabinete al que
        '                        pertenece el documento
        'nombre_expediente_relacion : Representa el nombre del expediente relacionado
        '----------------------------------------------------------------------------------
        '                           RETORNO
        '----------------------------------------------------------------------------------
        'existencia_vinculacion : Retorna la exitencia de una vinculación previa del 
        '                         documento con un expediente
        '----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------------
        'Fecha                 : 2024-08-16
        'Elabora               : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------
        Dim Result As String = ""
        Dim expediente_conservacion() As expediente_conservacion = Nothing
        Result = Me.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                    expediente_conservacion)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Dim id_produccion As Long = 0
        Dim estado_existencia_produccion As String = ""
        Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
        Dim id_expediente_relacion As Integer = 0
        Result = ClassGaProducionDocumental.Solicita_existencia_produccion_documental(id_imagen,
                                                                                      gabinete,
                                                                                      estado_existencia_produccion,
                                                                                      id_produccion,
                                                                                      id_expediente_relacion,
                                                                                      nombre_expediente_relacion)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        '-----------------------------------------------------------------------------------------------
        '-----------------Valida el estado relación documento a expediente en la producción documental
        '-----------------------------------------------------------------------------------------------
        If id_expediente_relacion <> 0 Then
            existencia_vinculacion = "Documento  relacionado al expediente (" & nombre_expediente_relacion & ") de código único (" & id_expediente_relacion & ")"
            Vincula_documento_gabinete_expediente_migracion = "YES"
            Exit Function
        End If
        '----------------------------------------------------------------------------------------------------------------------
        '-----------------Asigna el retorno del nombre del expediente asignado al documento para  actualizar interface
        '-----------------------------------------------------------------------------------------------------------------------
        nombre_expediente_relacion = expediente_conservacion(0).CODIGO_UNICO
        Dim Ruta_archivo_indice_expediente As String = ""
        '-----------------------------------------------------------------------------------------------
        '-----------------Solicita la ruta de archivo de expediente electrónico
        '-----------------------------------------------------------------------------------------------
        Result = Me.Solicita_archivo_indice_expediente(id_expediente,
                                                       Ruta_archivo_indice_expediente)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Dim Option_aplicar_trd As Integer = 0
        Dim Option_unidad_conservacion As Integer = 0
        Dim Class_sytem1_ As New Class_system1
        Result = Class_sytem1_.VerificaOpcionAplicarTablaRetencion(Option_aplicar_trd,
                                                                       gabinete)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Result = Class_sytem1_.VerificaOpcionAplicarInventarioDocumental(Option_unidad_conservacion,
                                                                             gabinete)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Dim date1al As String = Date.Today
        Dim ref_ClassGestionFechas As New ClassGestionFechas
        Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
            Exit Function
        End If
        Dim Class_DETALLE_GABIENETE As New Class_DETALLE_GABIENETE
        Dim campo_radicado As String = ""
        Result = Class_DETALLE_GABIENETE.SolicitaNombreCampoRadicadoGabinete(gabinete,
                                                                         campo_radicado)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Dim ref_ClassDaGabinete As New ClassDaGabinete
        Dim valor_campo_radicado As String = ""
        Result = ref_ClassDaGabinete.Solicita_valor_campo_gebinete(id_imagen,
                                                                   gabinete,
                                                                   campo_radicado,
                                                                   valor_campo_radicado)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Dim stru_paramter_image As stru_paramter_image = Nothing
        Dim numero_paginas As Integer = 0
        Dim tipo_doc As Integer = 0
        Result = ref_ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(gabinete,
                                                                                          id_imagen,
                                                                                          stru_paramter_image,
                                                                                          Option_aplicar_trd)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Dim datos_imagen_gabinete As String = ""
        If estado_existencia_produccion = "NO" Then
            Result = ref_ClassDaGabinete.Solicita_datos_imagen_gabinete(gabinete,
                                                                        id_imagen,
                                                                        datos_imagen_gabinete)
            If Result <> "YES" Then
                Vincula_documento_gabinete_expediente_migracion = Result
                Exit Function
            End If
        End If
        Dim extenssion As String = ""
        Dim ClassDaExtension As New Class_da_extension
        Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                              extenssion)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        extenssion = extenssion.Replace(".", "")
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
        Dim ref_radicado As String = "null"
        Dim sugundo_nombre_documento As String = ""
        If valor_campo_radicado <> "" Then
            ref_radicado = "'" & valor_campo_radicado & "'"
        End If
        Dim ref_sugundo_nombre_documento As String = ""
        Dim nombre_docuarchi As String = ""
        If sugundo_nombre_documento <> "" Then
            ref_sugundo_nombre_documento = "'" & sugundo_nombre_documento & "'"
            nombre_docuarchi = sugundo_nombre_documento
        Else
            Dim Ceros_Cuerpo_Imag As String = "DIG"
            Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, id_imagen)
            ref_sugundo_nombre_documento = "'DIG" & Ceros_Cuerpo_Imag & id_imagen & "." & extenssion & "'"
            nombre_docuarchi = "DIG" & Ceros_Cuerpo_Imag & id_imagen & "." & extenssion
        End If
        Dim matri_doc() As String = Nothing
        Dim tamano As String = ""
        Result = ref_ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     gabinete,
                                                                                     matri_doc)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Dim tam_archivo As Object = 1024
        For i As Integer = 1 To matri_doc.Length - 1
            Dim fi As New FileInfo(matri_doc(i))
            If fi.Exists Then
                tam_archivo = tam_archivo + fi.Length
            End If

        Next
        If (tam_archivo / 1024) > 1024 Then
            tamano = Math.Round(((tam_archivo / 1024) / 1024), 2).ToString() & " Mb"
        Else
            tamano = Math.Round((tam_archivo / 1024), 2).ToString() & " Kb"
        End If

        '-------------------------------------------------
        'Detecta el numero de paaginas cundo el documento
        'es diferente a TIF, BMP, JPG
        '-------------------------------------------------
        Dim pagi As Integer = matri_doc.Length - 1
        Dim numero_pagina As Integer = -1
        Dim Class_ItexShare As New Class_ItexShare
        Result = Class_ItexShare.Retorna_numero_paginas_documentos_unificados(matri_doc(1),
                                                                                  numero_pagina)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        If numero_pagina <> -1 Then
            pagi = numero_pagina
        End If
        Dim id_clase_documento As Integer = stru_paramter_image.ID_TIPODOCUMENTO
        Dim tipo_documento As String = stru_paramter_image.TIPODOCUMENTO
        Dim id_tipo_unidad_documental As Integer = expediente_conservacion(0).ID_TIPO_UNIDAD_DOCUMENTAL
        Dim id_tipo_expediente As Integer = expediente_conservacion(0).ID_TIPO_UNIDAD_DOCUMENTAL
        Dim id_tipo_unidad_conservacion As Integer = expediente_conservacion(0).TIPO_UNIDAD_ID_TIPO
        Dim id_sub_serie As Integer = expediente_conservacion(0).CODIGO_SUBSERIE
        Dim nombre_area As String = expediente_conservacion(0).NOMBRE_AREA
        Dim id_serie As Integer = expediente_conservacion(0).CODIGO_SERIE
        Dim id_area As Integer = expediente_conservacion(0).CODIGO_AREA_TRD
        Dim id_unidad_conservacion = expediente_conservacion(0).ID_UNIDAD_CONSERVACION
        Dim expediente As String = expediente_conservacion(0).CODIGO_UNICO
        Dim nombre_serie As String = expediente_conservacion(0).NOMBRE_SERIE
        Dim nombre_sub_serie As String = expediente_conservacion(0).NOMBRE_SUBSERIE
        Dim unidad_conserva As String = ""
        Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
        Dim fecha_elaboracion As String = date1al
        Dim estado_archivo As Integer = 0
        Dim tipo_archivo_producion As Integer = 0
        If id_clase_documento <> 0 Then
            ref_id_clase_documento = id_clase_documento
        End If
        If id_tipo_unidad_documental <> 0 Then
            ref_id_tipo_unidad_documental = id_tipo_unidad_documental
        End If
        If id_tipo_expediente <> 0 Then
            ref_id_tipo_expediente = id_tipo_expediente
        End If
        If id_sub_serie <> 0 Then
            ref_id_sub_serie = id_sub_serie
        End If
        If nombre_area <> "" Then
            ref_nombre_area = "'" & nombre_area & "'"
        End If
        If id_tipo_unidad_conservacion <> 0 Then
            ref_id_tipo_unidad_conservacion = id_tipo_unidad_conservacion
        End If
        If id_serie <> 0 Then
            ref_id_serie = id_serie
        End If
        If id_area <> 0 Then
            ref_id_area = id_area
        End If
        If id_expediente <> 0 Then
            ref_id_expediente = id_expediente
        End If
        If id_unidad_conservacion <> 0 Then
            ref_id_unidad_conservacion = id_unidad_conservacion
        End If
        If expediente <> "" Then
            ref_expediente = "'" & expediente & "'"
        End If
        If nombre_serie <> "" Then
            ref_nombre_serie = "'" & nombre_serie & "'"
        End If
        If nombre_sub_serie <> "" Then
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        If nombre_sub_serie <> "" Then
            ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
        End If
        If tipo_documento <> "" Then
            ref_tipo_documento = "'" & tipo_documento & "'"
        End If
        If unidad_conserva <> "" Then
            ref_unidad_conserva = "'" & unidad_conserva & "'"
        End If
        If clase_documento <> "" Then
            ref_clase_documento = "'" & clase_documento & "'"
        End If
        If fecha_elaboracion <> "" Then
            ref_fecha_elaboracion = "'" & fecha_elaboracion & "'"
        End If
        If id_expediente <> 0 Or id_unidad_conservacion <> 0 Then
            estado_archivo = 0
        End If
        Dim mySqldatReader As MySqlDataReader
        Dim myConnection As New MySqlConnection
        Dim ref As New conect.Dbase_Conction_Mysql_DA
        Result = ref.Returna_Conexion_Mysql(myConnection)
        If Result <> "YES" Then
            Vincula_documento_gabinete_expediente_migracion = Result
            Exit Function
        End If
        Dim stru_produccion_indice As stru_produccion_indice = Nothing
        Dim myTrans As MySqlTransaction
        Try
            Dim myCommand As MySqlCommand = myConnection.CreateCommand()
            myTrans = myConnection.BeginTransaction()
            myCommand.Connection = myConnection
            myCommand.Transaction = myTrans
            Dim sqlinventario As String = ""
            Dim datos_insert_inventario As String = ""
            Dim datos_actualiza_producion As String = ""
            Dim datos_actualiza_gabinete_gestion As String = ""
            Dim Switc2 As Integer = 0
            '-----------------------------------------------
            'Registro produccion documental
            '-----------------------------------------------
            If estado_existencia_produccion = "NO" Then
                sqlinventario = "insert into registro_producion_documental (remit_dest_interno_idremit_dest_interno," &
                    "ID_USUARIO_GESTION,FECHA_DOCUMENTO,ID_AREA_DEPARTAMENTO,ID_SERIE_DOCUMENTO,SERIE_DOCUMENTO," &
                    "ID_SUBSERIE_DOCUMENTO,SUBSERIE_DOCUMENTO,ID_TIPO_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO,FULTEXT_DOCUMENTO," &
                    "ID_DOCUMENTO_DOCUARCHI_ALMACEN,ESTADO_DOCUMENTO_ARCHIVO,NOMBRE_GABINETE,NUMERO_FOLIOS," &
                    "EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,EXPEDIENTE,ID_TIPO_EXPEDIENTE,ID_TIPO_UNIDAD_CONSERVACION," &
                    "ID_UNIDAD_CONSERVACION,ID_CLASE_DOCUMENTO,CLASEDOCUMENTO," &
                    "FECHA_ELABORACION,UNIDADCONSERVA,NOMBRE_AREA_DEPARTAMENTO,ID_TIPO_UNIDAD_DOCUMENTAL,ID_EMPRESA_DOCUMENTO," &
                    "RADICADO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO,DOCUMENTO_PRODUCION_DOCUMENTAL,TAMANO,FORMATO) values "
                datos_insert_inventario = "(" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "," & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ",'" & date1al & "'," &
                    ref_id_area & "," & ref_id_serie & "," & ref_nombre_serie & "," & ref_id_sub_serie & "," & ref_nombre_sub_serie &
                    "," & ref_id_tipo_documento & "," & ref_tipo_documento & ",'" & datos_imagen_gabinete & "'," & id_imagen & "," &
                    estado_archivo & ",'" & gabinete & "'," & pagi & "," & ref_id_expediente & "," & ref_expediente & "," & ref_id_tipo_expediente &
                    "," & ref_id_tipo_unidad_conservacion & "," & ref_id_unidad_conservacion & "," & ref_id_clase_documento & "," &
                    ref_clase_documento & "," & ref_fecha_elaboracion & "," & ref_unidad_conserva & "," & ref_nombre_area & "," & ref_id_tipo_unidad_documental &
                    "," & HttpContext.Current.Session.Item("GA_IDEMPRESA") & "," & ref_radicado & "," & ref_sugundo_nombre_documento & "," & tipo_archivo_producion & ",'" & tamano & "','" & extenssion & "')"
                sqlinventario = sqlinventario & datos_insert_inventario
                '-----------------------------------------------
                'Registra inventario documental
                '-----------------------------------------------
                myCommand.CommandText = sqlinventario
                Switc2 = myCommand.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible agregar registro de inventario documental  "
                    myConnection.Close()
                    Exit Function
                End If
                id_produccion = myCommand.LastInsertedId
            Else
                datos_actualiza_producion = "UPDATE registro_producion_documental SET ID_AREA_DEPARTAMENTO=" & ref_id_area &
                        ",ID_SERIE_DOCUMENTO=" & ref_id_serie & " , SERIE_DOCUMENTO=" & ref_nombre_serie & " , ID_SUBSERIE_DOCUMENTO=" &
                        ref_id_sub_serie & " , SUBSERIE_DOCUMENTO=" & ref_nombre_sub_serie & " , EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" &
                        ref_id_expediente & " , EXPEDIENTE=" & ref_expediente & " , ID_TIPO_EXPEDIENTE=" & ref_id_tipo_expediente &
                        " , ID_TIPO_UNIDAD_CONSERVACION=" & ref_id_tipo_unidad_conservacion & " , ID_UNIDAD_CONSERVACION=" & ref_id_unidad_conservacion &
                        " , NOMBRE_AREA_DEPARTAMENTO=" & ref_nombre_area & " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_produccion
                myCommand.CommandText = datos_actualiza_producion
                Switc2 = myCommand.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible actualizar registro inventario de produccion documental  "
                    myConnection.Close()
                    Exit Function
                End If

            End If
            If Option_aplicar_trd = 1 Then
                datos_actualiza_gabinete_gestion = "UPDATE " & gabinete & " set ID_INVENTARIO_DOCUMENTAL=" & id_produccion &
                            " , ID_AREA=" & ref_id_area & " , ID_SERIE=" & ref_id_serie & " , ID_SUB_SERIE=" & ref_id_sub_serie &
                            " , NOMBRESERIE=" & ref_nombre_serie & " , NOMBRESUBSERIE=" & ref_nombre_sub_serie & " , ID_EXPEDIENTE=" &
                            ref_id_expediente & " , ID_TIPO_EXPEDIENTE=" & ref_id_tipo_expediente & " , EXPEDIENTE=" & ref_expediente &
                            " where ID=" & id_imagen
                myCommand.CommandText = datos_actualiza_gabinete_gestion
                Switc2 = myCommand.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible actualizar la gestion en el gabinete  "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            Dim Numero_Digitalizado_contenido As Integer = 0
            Dim Numero_Electronico_contenido As Integer = 0
            '-------------------------------------------------------------
            'Actualiza el numero  de documentos contenido en el expediente
            '-------------------------------------------------------------
            If id_expediente <> 0 Then
                Dim Parametro_Select_System1 As String = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                   & "'" & id_expediente & "' " & "for update"
                myCommand.CommandText = Parametro_Select_System1
                mySqldatReader = myCommand.ExecuteReader()
                If mySqldatReader Is Nothing Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible encontrar la identificación del expediente "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If mySqldatReader.HasRows = False Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible Encontrar el registro del expediente"
                    mySqldatReader.Close()
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                Else
                    mySqldatReader.Read()
                    Numero_Digitalizado_contenido = mySqldatReader.Item(0)
                    Numero_Electronico_contenido = mySqldatReader.Item(1)
                    mySqldatReader.Close()
                End If
                Dim update_sql As String = ""
                Numero_Electronico_contenido = Numero_Electronico_contenido + pagi
                update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido &
                     ",NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Electronico_contenido & " where ID_EXPEDIENTE = " & "'" & id_expediente & "' "
                myCommand.CommandText = update_sql
                Switc2 = myCommand.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible Actualizar numero de folios del expediente "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
            End If
            '------------------------------------------------------
            'Registra indice del documento en el expediente
            '------------------------------------------------------
            If id_expediente <> 0 And expediente_conservacion(0).estado_expediente_electronico = 2 Then
                Dim Extension As String = ""
                Dim visor As String = ""
                Dim Estado_doc As String = ""
                stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = ref_sugundo_nombre_documento
                stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL = id_produccion
                stru_produccion_indice.NOMBRE_DOCUARCHI = nombre_docuarchi
                Dim valor_ingreso_hueya As String = stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL
                encriptacion.encript_md5(valor_ingreso_hueya,
                                                  "7894561230!",
                                                   stru_produccion_indice.VALOR_HUELLA)
                stru_produccion_indice.FUCION_RESUMEN = "MD5"
                Dim ClassGestionFechas As New ClassGestionFechas
                Dim fecha_incorporacion As String = ""
                Result = ClassGestionFechas.Formatea_Fecha_Almacenamiento_guion(fecha_incorporacion)
                fecha_incorporacion = Left(fecha_incorporacion, 10)
                stru_produccion_indice.FECHA_ELABORACION = fecha_incorporacion
                stru_produccion_indice.FECHA_DOCUMENTO = fecha_incorporacion
                stru_produccion_indice.FORMATO = extenssion
                stru_produccion_indice.TAMANO = tamano
                stru_produccion_indice.CLASEDOCUMENTO = ref_clase_documento
                stru_produccion_indice.RUTA_ARCHIVO = matri_doc(1)
                stru_produccion_indice.RUTA_ARCHIVO = stru_produccion_indice.RUTA_ARCHIVO.Replace("/", "\")
                stru_produccion_indice.NUMERO_FOLIOS = pagi
                If tipo_documento = "" Then
                    stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO = "NA"
                Else
                    stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO = tipo_documento
                End If
                stru_produccion_indice.CLASEDOCUMENTO = ref_clase_documento
                Dim ORDEN_INDICE As Integer = 0
                Dim ULTIMA_PAGINA_INDICE As Integer = 0
                Dim Parametro_orden_indice As String = " SELECT ORDEN_INDICE,ULTIMA_PAGINA_INDICE" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                     & id_expediente & " " & " for update"
                myCommand.CommandText = Parametro_orden_indice
                mySqldatReader = myCommand.ExecuteReader()
                If mySqldatReader Is Nothing Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible encontrar el expediente. "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                If mySqldatReader.HasRows = False Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible encontrar el expediente"
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
                ORDEN_INDICE = ORDEN_INDICE + 1
                Dim PAGINA_INICIAL As Integer = ULTIMA_PAGINA_INDICE + 1
                ULTIMA_PAGINA_INDICE = ULTIMA_PAGINA_INDICE + pagi
                stru_produccion_indice.ORDEN_EN_EXPEDIENTE = ORDEN_INDICE
                stru_produccion_indice.PAGINA_INICIO = PAGINA_INICIAL
                stru_produccion_indice.PAGINA_FINAL = ULTIMA_PAGINA_INDICE
                Dim sql_insert As String = "insert into  ra_cert_indice_expediente (registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL," &
                    "expediente_archivo_ID_EXPEDIENTE,Nombre_documento,Tipologia_documental,fecha_declaracion_documento,fecha_incorporacion_documento," &
                    "valor_huella,Funcion_resumen,orden_documento_expedicion,pagina_inicial,pagina_final,formato,dimension_kb,origen,ruta_documento,numero_folios, segundo_nombre) values (" &
                    stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL & "," & id_expediente & ",'" & stru_produccion_indice.NOMBRE_DOCUARCHI & "','" &
                    stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO & "','" & stru_produccion_indice.FECHA_DOCUMENTO & "','" & stru_produccion_indice.FECHA_ELABORACION & "','" &
                    stru_produccion_indice.VALOR_HUELLA & "','" & stru_produccion_indice.FUCION_RESUMEN & "'," & ORDEN_INDICE & "," & PAGINA_INICIAL &
                    "," & ULTIMA_PAGINA_INDICE & ",'" & stru_produccion_indice.FORMATO & "','" & stru_produccion_indice.TAMANO & "'," & stru_produccion_indice.CLASEDOCUMENTO & ",'" &
                    stru_produccion_indice.RUTA_ARCHIVO & "'," & stru_produccion_indice.NUMERO_FOLIOS & "," & stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO & ")"
                myCommand.CommandText = sql_insert
                Switc2 = myCommand.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible crear indice documento en el expediente "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim update_orden_ultima_pagina As String = " UPDATE expediente_archivo " &
                        " SET ORDEN_INDICE=" & ORDEN_INDICE & " , ULTIMA_PAGINA_INDICE=" & ULTIMA_PAGINA_INDICE &
                        " , estado_expediente_electronico=" & expediente_conservacion(0).estado_expediente_electronico &
                        "  where ID_EXPEDIENTE = " & id_expediente
                myCommand.CommandText = update_orden_ultima_pagina
                Switc2 = myCommand.ExecuteNonQuery()
                If Switc2 = 0 Then
                    Vincula_documento_gabinete_expediente_migracion = "Imposible actualizar el orden del indice en el expediente "
                    myTrans.Rollback()
                    myConnection.Close()
                    Exit Function
                End If
                Dim xmlArchivo As New XmlDocument
                stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO.Replace("'", "")
                stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO.Replace("'", "")
                Result = Me.Actualiza_archivo_xml_indice_expediente(Ruta_archivo_indice_expediente,
                                                                        stru_produccion_indice,
                                                                        xmlArchivo)
                If Result <> "YES" Then
                    myTrans.Rollback()
                    If Not myConnection Is Nothing Then
                        myConnection.Close()
                    End If
                    Vincula_documento_gabinete_expediente_migracion = "Error actualizando archivo xml indice " & Result
                    Exit Function
                Else
                    xmlArchivo.Save(Ruta_archivo_indice_expediente)
                End If
            End If
            myTrans.Commit()
            myConnection.Close()
            Vincula_documento_gabinete_expediente_migracion = "YES"
            Exit Function
        Catch e As Exception
            Try

            Catch ex As MySqlException
                If Not myTrans.Connection Is Nothing Then
                    myTrans.Rollback()
                    myConnection.Close()
                    Vincula_documento_gabinete_expediente_migracion = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                    Exit Function
                End If
            End Try
            If Not myTrans Is Nothing Then
                myTrans.Rollback()
            End If
            If Not myConnection Is Nothing Then
                myConnection.Close()
            End If
            Vincula_documento_gabinete_expediente_migracion = "Error General " & e.Message
            Exit Function
        End Try

    End Function
    Function VinculaDocumentoExpediente(ByVal id_expediente As Integer,
                                        ByVal id_imagen As Integer,
                                        ByVal gabinete As String,
                                        ByVal radicado As String,
                                        ByVal id_tarea_wf As Long,
                                        ByRef valor_campo As String,
                                        ByRef nombre_expediente_relacion As String) As String
        Try
            Dim Result As String = ""
            Dim expediente_conservacion() As expediente_conservacion = Nothing
            Result = Me.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                        expediente_conservacion)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            Dim id_produccion As Long = 0
            Dim estado_existencia_produccion As String = ""
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            nombre_expediente_relacion = ""
            Dim id_expediente_relacion As Integer = 0
            Result = ClassGaProducionDocumental.Solicita_existencia_produccion_documental(id_imagen,
                                                                                          gabinete,
                                                                                          estado_existencia_produccion,
                                                                                          id_produccion,
                                                                                          id_expediente_relacion,
                                                                                          nombre_expediente_relacion)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            '-----------------------------------------------------------------------------
            'Verifica estado relación documento a expediente en la producción documental
            '-----------------------------------------------------------------------------
            If id_expediente_relacion <> 0 Then
                VinculaDocumentoExpediente = "YES"
                Exit Function
            End If
            Dim Nombre_ruta As String = ""
            Dim Refclas_workflow As New ClassWorkflow
            Dim Ref_class_ruta As New Class_worflow_rutas
            Nombre_ruta = HttpContext.Current.Session.Item("WF_RUTAWORKFLOW")
            '------------------------------------------------
            'Retorna si el tipo de tarea workflow es externa
            'Valores 1. Tarea interna    2. Tarea externa
            '------------------------------------------------
            Dim Ref_dat_adic As New Class_DAT_ADIC_TAR
            Dim id_tipo_tarea As Integer = 0
            Result = Ref_dat_adic.SolicitaTipoFujoExternoInterno(id_tarea_wf,
                                                                    id_tipo_tarea,
                                                                    Nombre_ruta)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            Dim Nombre_plantilla_radicado As String = ""
            Dim Refclas_radicado As New ClassRadicador
            Dim id_expediente_plantilla_radicado As Integer = 0
            Dim nombre_expediente_plantilla_radicado As String = ""
            Dim id_tipo_expediente_plantilla_radicado As Integer = 0
            '------------------------------------------------------------------
            'Para el caso de las tareas workflow de flujos internos, es decir
            'flujos que se inician directamente desde la plantilla radicación
            'del gestor documental, se solicita el nombre de la plantilla
            ', expediente relacionado a la plantilla, nombre expediente y 
            'tipo  expediente
            '------------------------------------------------------------------
            If id_tipo_tarea = 1 Then
                Dim Ref_Class_ra_registro_general_radicacion As New Class_ra_registro_general_radicacion
                Result = Ref_Class_ra_registro_general_radicacion.SolicitaNombrePlantillaRadicado(radicado,
                                                                                                  Nombre_plantilla_radicado)
                If Result <> "YES" Then
                    VinculaDocumentoExpediente = Result
                    Exit Function
                End If
                '---------------------------------------
                'Retorna expediente y id expediente
                '---------------------------------------
                If Nombre_plantilla_radicado <> "" Then
                    Result = Refclas_radicado.Retorna_nombre_expediente_id_expediente_radicado(radicado,
                                                                                               Nombre_plantilla_radicado,
                                                                                               id_expediente_plantilla_radicado,
                                                                                               nombre_expediente_plantilla_radicado,
                                                                                               id_tipo_expediente_plantilla_radicado)
                    If Result <> "YES" Then
                        VinculaDocumentoExpediente = Result
                        Exit Function
                    End If
                End If
                '-------------------------------------------
                'Valida que el expediente vinculante no sea
                'diferente al expediente relacionado al 
                'expediente relacionado al radicado
                '-------------------------------------------
                If id_expediente_plantilla_radicado <> 0 And id_expediente_plantilla_radicado <> id_expediente Then
                    VinculaDocumentoExpediente = "Esta tratando de vincualar el documento a un expediente diferente al expediente (" &
                        nombre_expediente_plantilla_radicado & ") relacionado en la plantilla de radicacion (" & Nombre_plantilla_radicado & ")"
                    Exit Function
                End If
            End If
            'Solicita archivo indice expediente
            Dim Ruta_archivo_indice_expediente As String = ""
            Result = Me.Solicita_archivo_indice_expediente(id_expediente,
                                                           Ruta_archivo_indice_expediente)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            '----------------------------------------------------------------
            'Asigna estado expediente con indice electronico
            '----------------------------------------------------------------
            If expediente_conservacion(0).estado_expediente_electronico = 0 Or expediente_conservacion(0).estado_expediente_electronico = 1 Then
                expediente_conservacion(0).estado_expediente_electronico = 2
            End If
            Dim Option_aplicar_trd As Integer = 0
            Dim Option_unidad_conservacion As Integer = 0
            Dim Class_sytem1_ As New Class_system1
            Result = Class_sytem1_.VerificaOpcionAplicarTablaRetencion(Option_aplicar_trd,
                                                                       gabinete)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            Result = Class_sytem1_.VerificaOpcionAplicarInventarioDocumental(Option_unidad_conservacion,
                                                                             gabinete)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            Dim ref_ClassDaGabinete As New ClassDaGabinete
            Dim stru_paramter_image As stru_paramter_image = Nothing
            Dim numero_paginas As Integer = 0
            Dim tipo_doc As Integer = 0
            Result = ref_ClassDaGabinete.Solicita_structura_imagen_gabinete_indice_expediente(gabinete,
                                                                                              id_imagen,
                                                                                              stru_paramter_image,
                                                                                              Option_aplicar_trd)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            Dim datos_imagen_gabinete As String = ""
            If estado_existencia_produccion = "NO" Then
                Result = ref_ClassDaGabinete.Solicita_datos_imagen_gabinete(gabinete,
                                                                            id_imagen,
                                                                            datos_imagen_gabinete)
                If Result <> "YES" Then
                    VinculaDocumentoExpediente = Result
                    Exit Function
                End If
            End If
            Dim extenssion As String = ""
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                  extenssion)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            extenssion = extenssion.Replace(".", "")
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
            Dim ref_radicado As String = "null"
            Dim sugundo_nombre_documento As String = ""
            If radicado <> "" Then
                ref_radicado = "'" & radicado & "'"
            End If
            Dim ref_sugundo_nombre_documento As String = ""
            Dim nombre_docuarchi As String = ""
            If sugundo_nombre_documento <> "" Then
                ref_sugundo_nombre_documento = "'" & sugundo_nombre_documento & "'"
                nombre_docuarchi = sugundo_nombre_documento
            Else
                Dim Ceros_Cuerpo_Imag As String = "DIG"
                Result = Ceros_Imagen_Almacenada(Ceros_Cuerpo_Imag, id_imagen)
                ref_sugundo_nombre_documento = "'DIG" & Ceros_Cuerpo_Imag & id_imagen & "." & extenssion & "'"
                nombre_docuarchi = "DIG" & Ceros_Cuerpo_Imag & id_imagen & "." & extenssion
            End If
            Dim matri_doc() As String = Nothing
            Dim tamano As String = ""
            Result = ref_ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                         gabinete,
                                                                                         matri_doc)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            Dim tam_archivo As Object = 1024
            For i As Integer = 1 To matri_doc.Length - 1
                Dim fi As New FileInfo(matri_doc(i))
                If fi.Exists Then
                    tam_archivo = tam_archivo + fi.Length
                End If

            Next
            If (tam_archivo / 1024) > 1024 Then
                tamano = Math.Round(((tam_archivo / 1024) / 1024), 2).ToString() & " Mb"
            Else
                tamano = Math.Round((tam_archivo / 1024), 2).ToString() & " Kb"
            End If

            '-------------------------------------------------
            'Detecta el numero de paaginas cundo el documento
            'es diferente a TIF, BMP, JPG
            '-------------------------------------------------
            Dim pagi As Integer = matri_doc.Length - 1
            Dim numero_pagina As Integer = -1
            'Dim ref_ClassAlmacenamiento As New ClassAlmacenamiento
            Dim Class_ItexShare As New Class_ItexShare
            Result = Class_ItexShare.Retorna_numero_paginas_documentos_unificados(matri_doc(1),
                                                                                  numero_pagina)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            If numero_pagina <> -1 Then
                pagi = numero_pagina
            End If
            Dim id_clase_documento As Integer = stru_paramter_image.ID_TIPODOCUMENTO
            Dim tipo_documento As String = stru_paramter_image.TIPODOCUMENTO
            Dim id_tipo_unidad_documental As Integer = expediente_conservacion(0).ID_TIPO_UNIDAD_DOCUMENTAL
            Dim id_tipo_expediente As Integer = expediente_conservacion(0).ID_TIPO_UNIDAD_DOCUMENTAL
            Dim id_tipo_unidad_conservacion As Integer = expediente_conservacion(0).TIPO_UNIDAD_ID_TIPO
            Dim id_sub_serie As Integer = expediente_conservacion(0).CODIGO_SUBSERIE
            Dim nombre_area As String = expediente_conservacion(0).NOMBRE_AREA
            Dim id_serie As Integer = expediente_conservacion(0).CODIGO_SERIE
            Dim id_area As Integer = expediente_conservacion(0).CODIGO_AREA_TRD
            Dim id_unidad_conservacion = expediente_conservacion(0).ID_UNIDAD_CONSERVACION
            Dim expediente As String = expediente_conservacion(0).CODIGO_UNICO
            Dim nombre_serie As String = expediente_conservacion(0).NOMBRE_SERIE
            Dim nombre_sub_serie As String = expediente_conservacion(0).NOMBRE_SUBSERIE
            Dim unidad_conserva As String = ""
            Dim clase_documento As String = "DOCUMENTO ELECTRONICO"
            Dim fecha_elaboracion As String = date1al
            Dim estado_archivo As Integer = 0
            Dim tipo_archivo_producion As Integer = 0
            If id_clase_documento <> 0 Then
                ref_id_clase_documento = id_clase_documento
            End If
            If id_tipo_unidad_documental <> 0 Then
                ref_id_tipo_unidad_documental = id_tipo_unidad_documental
            End If
            If id_tipo_expediente <> 0 Then
                ref_id_tipo_expediente = id_tipo_expediente
            End If
            If id_sub_serie <> 0 Then
                ref_id_sub_serie = id_sub_serie
            End If
            If nombre_area <> "" Then
                ref_nombre_area = "'" & nombre_area & "'"
            End If
            If id_tipo_unidad_conservacion <> 0 Then
                ref_id_tipo_unidad_conservacion = id_tipo_unidad_conservacion
            End If
            If id_serie <> 0 Then
                ref_id_serie = id_serie
            End If
            If id_area <> 0 Then
                ref_id_area = id_area
            End If
            If id_expediente <> 0 Then
                ref_id_expediente = id_expediente
            End If
            If id_unidad_conservacion <> 0 Then
                ref_id_unidad_conservacion = id_unidad_conservacion
            End If
            If expediente <> "" Then
                ref_expediente = "'" & expediente & "'"
            End If
            If nombre_serie <> "" Then
                ref_nombre_serie = "'" & nombre_serie & "'"
            End If
            If nombre_sub_serie <> "" Then
                ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
            End If
            If nombre_sub_serie <> "" Then
                ref_nombre_sub_serie = "'" & nombre_sub_serie & "'"
            End If
            If tipo_documento <> "" Then
                ref_tipo_documento = "'" & tipo_documento & "'"
            End If
            If unidad_conserva <> "" Then
                ref_unidad_conserva = "'" & unidad_conserva & "'"
            End If
            If clase_documento <> "" Then
                ref_clase_documento = "'" & clase_documento & "'"
            End If
            If fecha_elaboracion <> "" Then
                ref_fecha_elaboracion = "'" & fecha_elaboracion & "'"
            End If
            If id_expediente <> 0 Or id_unidad_conservacion <> 0 Then
                estado_archivo = 0
            End If
            Dim mySqldatReader As MySqlDataReader
            Dim myConnection As New MySqlConnection
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Result = ref.Returna_Conexion_Mysql(myConnection)
            If Result <> "YES" Then
                VinculaDocumentoExpediente = Result
                Exit Function
            End If
            Dim stru_produccion_indice As stru_produccion_indice = Nothing
            Dim myTrans As MySqlTransaction
            Try
                Dim myCommand As MySqlCommand = myConnection.CreateCommand()
                myTrans = myConnection.BeginTransaction()
                myCommand.Connection = myConnection
                myCommand.Transaction = myTrans
                Dim sqlinventario As String = ""
                Dim datos_insert_inventario As String = ""
                Dim datos_actualiza_producion As String = ""
                Dim datos_actualiza_gabinete_gestion As String = ""
                Dim Switc2 As Integer = 0
                '-----------------------------------------------
                'Registro produccion documental
                '-----------------------------------------------
                If estado_existencia_produccion = "NO" Then
                    sqlinventario = "insert into registro_producion_documental (remit_dest_interno_idremit_dest_interno," &
                    "ID_USUARIO_GESTION,FECHA_DOCUMENTO,ID_AREA_DEPARTAMENTO,ID_SERIE_DOCUMENTO,SERIE_DOCUMENTO," &
                    "ID_SUBSERIE_DOCUMENTO,SUBSERIE_DOCUMENTO,ID_TIPO_DOCUMENTO,DESCRIPCION_TIPO_DOCUMENTO,FULTEXT_DOCUMENTO," &
                    "ID_DOCUMENTO_DOCUARCHI_ALMACEN,ESTADO_DOCUMENTO_ARCHIVO,NOMBRE_GABINETE,NUMERO_FOLIOS," &
                    "EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE,EXPEDIENTE,ID_TIPO_EXPEDIENTE,ID_TIPO_UNIDAD_CONSERVACION," &
                    "ID_UNIDAD_CONSERVACION,ID_CLASE_DOCUMENTO,CLASEDOCUMENTO," &
                    "FECHA_ELABORACION,UNIDADCONSERVA,NOMBRE_AREA_DEPARTAMENTO,ID_TIPO_UNIDAD_DOCUMENTAL,ID_EMPRESA_DOCUMENTO," &
                    "RADICADO_DOCUMENTO,SEGUNDO_NOMBRE_DOCUMENTO,DOCUMENTO_PRODUCION_DOCUMENTAL,TAMANO,FORMATO) values "
                    datos_insert_inventario = "(" & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & "," & HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") & ",'" & date1al & "'," &
                    ref_id_area & "," & ref_id_serie & "," & ref_nombre_serie & "," & ref_id_sub_serie & "," & ref_nombre_sub_serie &
                    "," & ref_id_tipo_documento & "," & ref_tipo_documento & ",'" & datos_imagen_gabinete & "'," & id_imagen & "," &
                    estado_archivo & ",'" & gabinete & "'," & pagi & "," & ref_id_expediente & "," & ref_expediente & "," & ref_id_tipo_expediente &
                    "," & ref_id_tipo_unidad_conservacion & "," & ref_id_unidad_conservacion & "," & ref_id_clase_documento & "," &
                    ref_clase_documento & "," & ref_fecha_elaboracion & "," & ref_unidad_conserva & "," & ref_nombre_area & "," & ref_id_tipo_unidad_documental &
                    "," & HttpContext.Current.Session.Item("GA_IDEMPRESA") & "," & ref_radicado & "," & ref_sugundo_nombre_documento & "," & tipo_archivo_producion & ",'" & tamano & "','" & extenssion & "')"
                    sqlinventario = sqlinventario & datos_insert_inventario
                    '-----------------------------------------------
                    'Registra inventario documental
                    '-----------------------------------------------
                    myCommand.CommandText = sqlinventario
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpediente = "Imposible agregar registro de inventario documental  "
                        myConnection.Close()
                        Exit Function
                    End If
                    id_produccion = myCommand.LastInsertedId
                Else
                    datos_actualiza_producion = "UPDATE registro_producion_documental SET ID_AREA_DEPARTAMENTO=" & ref_id_area &
                        ",ID_SERIE_DOCUMENTO=" & ref_id_serie & " , SERIE_DOCUMENTO=" & ref_nombre_serie & " , ID_SUBSERIE_DOCUMENTO=" &
                        ref_id_sub_serie & " , SUBSERIE_DOCUMENTO=" & ref_nombre_sub_serie & " , EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=" &
                        ref_id_expediente & " , EXPEDIENTE=" & ref_expediente & " , ID_TIPO_EXPEDIENTE=" & ref_id_tipo_expediente &
                        " , ID_TIPO_UNIDAD_CONSERVACION=" & ref_id_tipo_unidad_conservacion & " , ID_UNIDAD_CONSERVACION=" & ref_id_unidad_conservacion &
                        " , NOMBRE_AREA_DEPARTAMENTO=" & ref_nombre_area & " where ID_REGISTRO_PRODUCION_DOCUMENTAL=" & id_produccion
                    myCommand.CommandText = datos_actualiza_producion
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpediente = "Imposible actualizar registro inventario de produccion documental  "
                        myConnection.Close()
                        Exit Function
                    End If

                End If
                If Option_aplicar_trd = 1 Then
                    datos_actualiza_gabinete_gestion = "UPDATE " & gabinete & " set ID_INVENTARIO_DOCUMENTAL=" & id_produccion &
                            " , ID_AREA=" & ref_id_area & " , ID_SERIE=" & ref_id_serie & " , ID_SUB_SERIE=" & ref_id_sub_serie &
                            " , NOMBRESERIE=" & ref_nombre_serie & " , NOMBRESUBSERIE=" & ref_nombre_sub_serie & " , ID_EXPEDIENTE=" &
                            ref_id_expediente & " , ID_TIPO_EXPEDIENTE=" & ref_id_tipo_expediente & " , EXPEDIENTE=" & ref_expediente &
                            " where ID=" & id_imagen
                    myCommand.CommandText = datos_actualiza_gabinete_gestion
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpediente = "Imposible actualizar la gestion en el gabinete  "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                '-----------------------------------------------
                'Registra la vinculación del documento con el 
                'expediente para el caso de workflow tipo 2
                'documento relacionado
                '-----------------------------------------------
                If id_expediente <> 0 And id_tarea_wf <> 0 Then
                    Dim sql_insert = "insert into   ra_rel_copia_wf_produccion " &
                     "(ID_REGISTRO_PRODUCION_DOCUMENTAL,id_tarea_wf,id_usuario_wf,id_imagen_da,nombre_gabinete,id_producion_wf,id_expediente_destino,id_ruta_wf,estado_copia_vincula) values " &
                    "(" & id_produccion & "," & id_tarea_wf & "," & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & "," &
                    id_imagen & ",'" & gabinete & "'," & -1 & "," & id_expediente & "," & HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",2)"
                    myCommand.CommandText = sql_insert
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpediente = "Imposible registrar relación incoporacion workflow "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                '------------------------------------------------------
                'Registra la relación del expediente  con consecutivo 
                'radicado, si el radicado no tiene un expediente
                'previo relacionado con el radicado y si es tipo
                '1 radicado interno y si no tiene expediente 
                'relacionado con el radicado
                '------------------------------------------------------
                If id_tipo_tarea = 1 And id_expediente_plantilla_radicado <> 0 Then
                    Dim sql_actualiza As String = "update " & Nombre_plantilla_radicado & " set id_Expediente=" & id_expediente & ",Expediente='" & expediente & "'" &
                        " where Consecutivo_Rad='" & radicado & "'"
                    myCommand.CommandText = sql_actualiza
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpediente = "Imposible registrar relación del expediente  "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                Dim Numero_Digitalizado_contenido As Integer = 0
                Dim Numero_Electronico_contenido As Integer = 0
                '-------------------------------------------------------------
                'Actualiza el numero  de documentos contenido en el expediente
                '-------------------------------------------------------------
                If id_expediente <> 0 Then
                    Dim Parametro_Select_System1 As String = " SELECT NUMERO_DIGITALIZADO_CONTENIDO,NUMERO_ELECTRONICO_CONTENIDO" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                   & "'" & id_expediente & "' " & "for update"
                    myCommand.CommandText = Parametro_Select_System1
                    mySqldatReader = myCommand.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        VinculaDocumentoExpediente = "Imposible encontrar la identificación del expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        VinculaDocumentoExpediente = "Imposible Encontrar el registro del expediente"
                        mySqldatReader.Close()
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    Else
                        mySqldatReader.Read()
                        Numero_Digitalizado_contenido = mySqldatReader.Item(0)
                        Numero_Electronico_contenido = mySqldatReader.Item(1)
                        mySqldatReader.Close()
                    End If
                    Dim update_sql As String = ""
                    Numero_Electronico_contenido = Numero_Electronico_contenido + pagi
                    update_sql = "update expediente_archivo Set NUMERO_ELECTRONICO_CONTENIDO=" & Numero_Electronico_contenido &
                     ",NUMERO_DIGITALIZADO_CONTENIDO=" & Numero_Electronico_contenido & " where ID_EXPEDIENTE = " & "'" & id_expediente & "' "
                    myCommand.CommandText = update_sql
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpediente = "Imposible Actualizar numero de folios del expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                End If
                '------------------------------------------------------
                'Registra indice del documento en el expediente
                '------------------------------------------------------
                If id_expediente <> 0 And expediente_conservacion(0).estado_expediente_electronico = 2 Then
                    Dim Extension As String = ""
                    Dim visor As String = ""
                    Dim Estado_doc As String = ""
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = ref_sugundo_nombre_documento
                    stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL = id_produccion
                    stru_produccion_indice.NOMBRE_DOCUARCHI = nombre_docuarchi
                    Dim valor_ingreso_hueya As String = stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL
                    encriptacion.encript_md5(valor_ingreso_hueya,
                                                  "7894561230!",
                                                   stru_produccion_indice.VALOR_HUELLA)
                    stru_produccion_indice.FUCION_RESUMEN = "MD5"
                    Dim ClassGestionFechas As New ClassGestionFechas
                    Dim fecha_incorporacion As String = ""
                    Result = ClassGestionFechas.Formatea_Fecha_Almacenamiento_guion(fecha_incorporacion)
                    fecha_incorporacion = Left(fecha_incorporacion, 10)
                    stru_produccion_indice.FECHA_ELABORACION = fecha_incorporacion
                    stru_produccion_indice.FECHA_DOCUMENTO = fecha_incorporacion
                    stru_produccion_indice.FORMATO = extenssion
                    stru_produccion_indice.TAMANO = tamano
                    stru_produccion_indice.CLASEDOCUMENTO = ref_clase_documento
                    stru_produccion_indice.RUTA_ARCHIVO = matri_doc(1)
                    stru_produccion_indice.RUTA_ARCHIVO = stru_produccion_indice.RUTA_ARCHIVO.Replace("/", "\")
                    stru_produccion_indice.NUMERO_FOLIOS = pagi
                    If tipo_documento = "" Then
                        stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO = "NA"
                    Else
                        stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO = tipo_documento
                    End If
                    stru_produccion_indice.CLASEDOCUMENTO = ref_clase_documento
                    Dim ORDEN_INDICE As Integer = 0
                    Dim ULTIMA_PAGINA_INDICE As Integer = 0
                    Dim Parametro_orden_indice As String = " SELECT ORDEN_INDICE,ULTIMA_PAGINA_INDICE" &
                    " FROM expediente_archivo where ID_EXPEDIENTE = " _
                     & id_expediente & " " & " for update"
                    myCommand.CommandText = Parametro_orden_indice
                    mySqldatReader = myCommand.ExecuteReader()
                    If mySqldatReader Is Nothing Then
                        VinculaDocumentoExpediente = "Imposible encontrar el expediente. "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        VinculaDocumentoExpediente = "Imposible encontrar el expediente"
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
                    ORDEN_INDICE = ORDEN_INDICE + 1
                    Dim PAGINA_INICIAL As Integer = ULTIMA_PAGINA_INDICE + 1
                    ULTIMA_PAGINA_INDICE = ULTIMA_PAGINA_INDICE + pagi
                    stru_produccion_indice.ORDEN_EN_EXPEDIENTE = ORDEN_INDICE
                    stru_produccion_indice.PAGINA_INICIO = PAGINA_INICIAL
                    stru_produccion_indice.PAGINA_FINAL = ULTIMA_PAGINA_INDICE
                    Dim sql_insert As String = "insert into  ra_cert_indice_expediente (registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL," &
                    "expediente_archivo_ID_EXPEDIENTE,Nombre_documento,Tipologia_documental,fecha_declaracion_documento,fecha_incorporacion_documento," &
                    "valor_huella,Funcion_resumen,orden_documento_expedicion,pagina_inicial,pagina_final,formato,dimension_kb,origen,ruta_documento,numero_folios, segundo_nombre) values (" &
                    stru_produccion_indice.ID_REGISTRO_PRODUCION_DOCUMENTAL & "," & id_expediente & ",'" & stru_produccion_indice.NOMBRE_DOCUARCHI & "','" &
                    stru_produccion_indice.DESCRIPCION_TIPO_DOCUMENTO & "','" & stru_produccion_indice.FECHA_DOCUMENTO & "','" & stru_produccion_indice.FECHA_ELABORACION & "','" &
                    stru_produccion_indice.VALOR_HUELLA & "','" & stru_produccion_indice.FUCION_RESUMEN & "'," & ORDEN_INDICE & "," & PAGINA_INICIAL &
                    "," & ULTIMA_PAGINA_INDICE & ",'" & stru_produccion_indice.FORMATO & "','" & stru_produccion_indice.TAMANO & "'," & stru_produccion_indice.CLASEDOCUMENTO & ",'" &
                    stru_produccion_indice.RUTA_ARCHIVO & "'," & stru_produccion_indice.NUMERO_FOLIOS & "," & stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO & ")"
                    myCommand.CommandText = sql_insert
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpediente = "Imposible crear indice documento en el expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    Dim update_orden_ultima_pagina As String = " UPDATE expediente_archivo " &
                        " SET ORDEN_INDICE=" & ORDEN_INDICE & " , ULTIMA_PAGINA_INDICE=" & ULTIMA_PAGINA_INDICE &
                        " , estado_expediente_electronico=" & expediente_conservacion(0).estado_expediente_electronico &
                        "  where ID_EXPEDIENTE = " & id_expediente
                    myCommand.CommandText = update_orden_ultima_pagina
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpediente = "Imposible actualizar el orden del indice en el expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    Dim xmlArchivo As New XmlDocument
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO.Replace("'", "")
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO.Replace("'", "")
                    Result = Me.Actualiza_archivo_xml_indice_expediente(Ruta_archivo_indice_expediente,
                                                                        stru_produccion_indice,
                                                                        xmlArchivo)
                    If Result <> "YES" Then
                        myTrans.Rollback()
                        If Not myConnection Is Nothing Then
                            myConnection.Close()
                        End If
                        VinculaDocumentoExpediente = "Error actualizando archivo xml indice " & Result
                        Exit Function
                    Else
                        xmlArchivo.Save(Ruta_archivo_indice_expediente)
                    End If
                End If
                myTrans.Commit()
                myConnection.Close()
                VinculaDocumentoExpediente = "YES"
                Exit Function
            Catch e As Exception
                Try

                Catch ex As MySqlException
                    If Not myTrans.Connection Is Nothing Then
                        myTrans.Rollback()
                        myConnection.Close()
                        VinculaDocumentoExpediente = "An exception of type " + ex.GetType().ToString() +
                                      " was encountered while attempting to roll back the transaction."
                        Exit Function
                    End If
                End Try
                If Not myTrans Is Nothing Then
                    myTrans.Rollback()
                End If
                If Not myConnection Is Nothing Then
                    myConnection.Close()
                End If
                VinculaDocumentoExpediente = "Error General " & e.Message
                Exit Function
            End Try
        Catch ex As Exception
            VinculaDocumentoExpediente = "Inconsistencia general funcion VinculaDocumentoExpediente " & ex.Message
        End Try
    End Function
    Function Copia_documento_expediente(ByVal id_usuario_gestion As Integer,
                                        ByRef id_documento_producion As Long,
                                        ByVal id_expediente As Integer,
                                        ByRef campos_valores As String,
                                        ByVal tipo_copia As Integer,
                                        ByVal id_tarea_wf As Long,
                                        ByVal radicado_wf As String,
                                        ByVal obliga_viculo_expe_gabinete As Integer,
                                        ByVal actuactualiza_expediente_gabinete As Integer) As String
        Try
            Dim Result As String = ""
            Dim nombre_archivo As String = ""
            Dim nombre_tipo_documental As String = ""
            Dim id_clase_documento As Integer = 0
            Dim id_expediente_ As Integer = 0
            Dim id_imagen As Integer = 0
            Dim nombre_gabinete As String = ""
            Dim Fecha_documento As String = ""
            Dim numero_folios As Integer = 0
            Dim nombre_documento_radicado As String = ""
            Dim registro_radicado As String = ""
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            Result = ClassGaProducionDocumental.Solicita_documento_radicado_produccion(id_documento_producion,
                                                                                       nombre_documento_radicado,
                                                                                       registro_radicado)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            If tipo_copia = 0 Then
                If nombre_documento_radicado <> "" Then
                    Copia_documento_expediente = "El documento (" & nombre_documento_radicado & ") se encuentra relacionado radicado(" & registro_radicado & ") , imposible copiar "
                    Exit Function
                End If
            End If
            Result = ClassGaProducionDocumental.Solicita_datos_caracterizacion_archivo_produccion(id_documento_producion,
                                                                                                  nombre_archivo,
                                                                                                  nombre_tipo_documental,
                                                                                                  id_clase_documento,
                                                                                                  id_expediente_,
                                                                                                  id_imagen,
                                                                                                  nombre_gabinete,
                                                                                                  Fecha_documento,
                                                                                                  numero_folios)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            '----------------------------------------------------
            'Verifica la existencia de copias archivo workflow
            '----------------------------------------------------
            Dim ref_Class_ra_rel_copia_wf_produccion As New Class_ra_rel_copia_wf_produccion
            Dim exitencia_copia_wf As String = ""
            If tipo_copia = 1 Then
                Result = ref_Class_ra_rel_copia_wf_produccion.Solicita_existencia_copia_estructura_expediente_workflow(id_imagen,
                                                                                                                       nombre_gabinete,
                                                                                                                       id_tarea_wf,
                                                                                                                       id_expediente,
                                                                                                                       exitencia_copia_wf)
                If Result <> "YES" Then
                    Copia_documento_expediente = Result
                    Exit Function
                End If
                If exitencia_copia_wf = "YES" Then
                    campos_valores = ""
                    Copia_documento_expediente = "YES"
                    Exit Function
                End If
            End If
            Dim refclas_expediente As New ClassGaExpediente
            '----------------------------------------------
            'Retorna el estado del expediente carpeta
            '----------------------------------------------
            Dim estado_expediente As Integer = 1
            Dim estado_publico As Integer = 1
            Result = refclas_expediente.Retorna_estado_expediente(id_expediente,
                                                                  estado_expediente,
                                                                  estado_publico)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            If estado_expediente <> 1 Then
                Copia_documento_expediente = "No se puede copiar el documento a la carpeta o expediente por que está cerrado"
                Exit Function
            End If
            '---------------------------------------------
            'Valida la configuración del gabinete
            '---------------------------------------------
            Dim inventario_documental As Integer = 0
            Dim aplica_trd As Integer = 0
            Dim asigna_unidad As Integer = 0
            Dim nombre_gabinete_destino As String = nombre_gabinete
            Result = refclas_expediente.SolicitaGabineteProducionExpediente(id_expediente,
                                                                               nombre_gabinete_destino)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            Dim Ref_Class_system1 As New Class_system1
            Result = Ref_Class_system1.Verifica_opcion_aplicar_trd_inventario_unidad_documental_gabinete(nombre_gabinete_destino,
                                                                                                         inventario_documental,
                                                                                                         aplica_trd,
                                                                                                         asigna_unidad)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            If inventario_documental = 0 Then
                Copia_documento_expediente = "El gabinete " & nombre_gabinete_destino & "  no tiene activa la opción inventario documental"
                Exit Function
            End If
            If aplica_trd = 0 Then
                Copia_documento_expediente = "El gabinete " & nombre_gabinete_destino & "  no tiene activa la opción aplicar tabla de retención"
                Exit Function
            End If
            If asigna_unidad = 0 Then
                Copia_documento_expediente = "El gabinete " & nombre_gabinete_destino & "  no tiene activa la opción asignar unidad documental"
                Exit Function
            End If
            '---------------------------------------------
            'Solicita las opciones de produción documental
            '---------------------------------------------
            Dim stru_config As STRU_CONFIG_PRODUCION
            Dim Class_ra_pro_config_producion_documental As New Class_ra_pro_config_producion_documental
            Result = Class_ra_pro_config_producion_documental.Solicita_obligatoriedad_aplica_trd_producion_documental(stru_config)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            If stru_config.ACTIVA_OBLIGA_TRD = 1 Then
                If nombre_tipo_documental = "" Then
                    Copia_documento_expediente = "Debe seleccionar el tipo documental del archivo que desea copiar"
                    Exit Function
                End If
            End If
            Dim matri_documentos_almacenados() As String = Nothing
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(id_imagen,
                                                                                     nombre_gabinete,
                                                                                     matri_documentos_almacenados)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Solicita el tipo de archivo según extensión
            '-----------------------------------------------
            Dim file_inf As New FileInfo(matri_documentos_almacenados(1))
            Dim id_tipo_archivo As Integer = 0
            Dim Class_da_extension As New Class_da_extension
            Result = Class_da_extension.Solicita_el_tipo_documento_docuarchi_segun_extension_de_archivo(file_inf.Extension,
                                                                                                        id_tipo_archivo)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            '-----------------------------------------------
            'Retorna identificación empresa gestión
            '-----------------------------------------------
            Dim refclas_admon_empresa As New ClassAdmonEmpresa
            Dim id_empresa As Integer = 0
            Result = refclas_admon_empresa.Retorna_id_empresa_usuario_gestion(id_empresa,
                                                                              id_usuario_gestion)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Result = refclas_expediente.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                                        estru_unidad_conservacion)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            '-----------------Solicita estructura tarea
            Dim Class_estados_tarea_workflow As New Class_estados_tarea_workflow
            Dim stru_estado As stru_estado = Nothing
            Result = Class_estados_tarea_workflow.Solicita_datos_estructura_tareas_seleccionada(id_tarea_wf,
                                                                                                stru_estado)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            Dim matri_datos_almacen() As String = Nothing
            Dim matri_gestion As estructure_gestion = Nothing
            Dim matri_documentos() As String = Nothing
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Copia_documento_expediente = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
                Exit Function
            End If
            Dim date_trans As Object = ""
            ref_ClassGestionFechas.Formatea_Fecha_Almacenamiento_Time(date_trans)
            Result = ClassGaProducionDocumental.Retorna_parametros_almacenamiento_documento_adjunto(id_expediente,
                                                                                                    matri_datos_almacen,
                                                                                                    matri_gestion,
                                                                                                    matri_documentos,
                                                                                                    nombre_gabinete_destino,
                                                                                                    nombre_archivo,
                                                                                                    estru_unidad_conservacion,
                                                                                                    "",
                                                                                                    nombre_tipo_documental,
                                                                                                    id_clase_documento,
                                                                                                    1,
                                                                                                    obliga_viculo_expe_gabinete,
                                                                                                    actuactualiza_expediente_gabinete)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            If tipo_copia = 1 Then
                If nombre_documento_radicado = "" Then
                    nombre_archivo = radicado_wf
                End If
            End If
            Erase matri_documentos
            For i As Integer = 1 To matri_documentos_almacenados.Length - 1
                ReDim Preserve matri_documentos(i - 1)
                matri_documentos(i - 1) = matri_documentos_almacenados(i)
            Next
            Dim Refalmacena As New ClassAlmacenamiento
            Dim id_imagen_ As Integer = 0
            Dim radicado As String = ""
            Dim id_registro As Integer = 0
            Result = Refalmacena.Almacenamiento("", "", nombre_gabinete_destino, 0, matri_datos_almacen,
            2, matri_documentos.Length, id_tipo_archivo, matri_documentos, 0, id_imagen_, id_tipo_archivo,
            HttpContext.Current.Session.Item("GA_IDEMPRESA"), HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
            matri_gestion.ID_AREA, matri_gestion.ID_SERIE, matri_gestion.ID_SUB_SERIE,
            matri_gestion.ID_TIPODOCUMENTO, matri_gestion.ID_EXPEDIENTE, matri_gestion.ID_TIPO_EXPEDIENTE,
            matri_gestion.ID_UNIDAD_CONSERVACION, matri_gestion.ID_TIPO_UNIDAD_CONSERVACION,
            matri_gestion.ID_CLASE_DOCUMENTO, matri_gestion.EXPEDIENTE, matri_gestion.NOMBRE_SERIE,
            matri_gestion.NOMBRE_SUB_SERIE, matri_gestion.TIPODOCUMENTO, matri_gestion.UNIDAD_CONSERVACION,
            matri_gestion.CLASE_DOCUMENTO, matri_gestion.FECHA_ELABORACION, radicado, nombre_archivo, id_registro, 1)
            If Result <> "YES" Then
                Copia_documento_expediente = Result
                Exit Function
            End If
            Dim fecha_tempo As String = ""
            ref_ClassGestionFechas.FormateaFechaTimeDbDefault(Fecha_documento,
                                                                  fecha_tempo)
            fecha_tempo = Left(fecha_tempo, 10)
            fecha_tempo = fecha_tempo.Replace("-", "/")
            campos_valores = id_registro & "|" & nombre_archivo.Replace("|", "") & "|" & fecha_tempo & "|" _
                & nombre_tipo_documental.Replace("|", "") & "|" & nombre_gabinete_destino & "|" & estru_unidad_conservacion(0).CODIGO_UNICO
            '-------------------------------------------------------------------------------------------------
            'Inserta registro copia documentos workflow
            ' Campo estado_copia_vincula 3- Copia a expediente producion documental  1- Copia a expediente
            '--------------------------------------------------------------------------------------------------
            If tipo_copia = 1 Then
                Dim sql_insert = "insert into ra_rel_copia_wf_produccion " &
               "(ID_REGISTRO_PRODUCION_DOCUMENTAL,id_tarea_wf,id_usuario_wf,id_imagen_da,nombre_gabinete,id_producion_wf,id_expediente_destino," &
               "id_ruta_wf,estado_copia_vincula,date_registro_trans,id_estado_tarea,ID_FLUJO_TRABAJO,Id_Actividad,Id_actividad_flujo) values " &
               "(" & id_registro & "," & id_tarea_wf & "," & HttpContext.Current.Session.Item("Id_Usuario_Workflow") & "," &
               id_imagen & ",'" & nombre_gabinete & "'," & id_documento_producion & "," & id_expediente & "," &
               HttpContext.Current.Session.Item("Id_Ruta_Workflow") & ",1,'" & date_trans & "'," & stru_estado.id_Estado & "," &
               stru_estado.ID_FLUJO_TRABAJO & "," & stru_estado.Id_Actividad & "," & stru_estado.ID_ACTIVIDAD_FLUJO_TRABAJO & ")"
                Dim ref2 As New conect.Dbase_Conction_Mysql_RA
                Result = ref2.SELECTION_INSERT_COMMAND(sql_insert)
                If Result <> "YES" Then
                    Copia_documento_expediente = "Se copio el archivo pero no se registro la relación con la tarea workflow " & Result
                    Exit Function
                End If
            End If
            id_documento_producion = 0
            Copia_documento_expediente = "YES"
            Exit Function
        Catch ex As Exception
            Copia_documento_expediente = "Inconsistencia general funcion Copia_documento_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_id_serie_documental_expediente(ByVal id_expediente As Integer,
                                                     ByRef id_serie_dcoumental As Integer) As String
        '------------------------------------------------
        'Function : Solicita la serie documental a la 
        'que pertenece el expediente
        'Fecha : 2022-02-08
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------
        Try
            Dim Parametro_Consulta = "select CODIGO_SERIE_TRD " &
          " from expediente_archivo where ID_EXPEDIENTE=" & id_expediente
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_serie_documental_expediente = "Funcion  Solicita_id_serie_documental_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_serie_dcoumental = 0
                Solicita_id_serie_documental_expediente = "Imposible encontrar el expediente (" & id_expediente & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_expediente = 0
                Else
                    id_expediente = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_id_serie_documental_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_serie_documental_expediente = "Inconsistencia gneral Solicita_id_serie_documental_expediente " & ex.Message
        End Try
    End Function
    Function Asigna_meta_dato_archivo_expediente_gestion(ByVal id_expediente As Integer,
                                                         ByRef stru_detalle_sis_meta_dato() As Class_ra_m_detalle_sis_meta_datos_) As String
        '--------------------------------------------------------------
        'Funcion : Asigna los meta datos de auto poblado heredados
        'del expediente en la estructura de meta datos
        'Fecha : 2022-02-17
        'Ing. Migue angel Urueta Miranda
        '--------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim estru_unidad_conservacion() As expediente_conservacion = Nothing
            Result = Me.SolicitaDatosEstructuraExpediente(id_expediente,
                                                          estru_unidad_conservacion)
            If Result <> "YES" Then
                Asigna_meta_dato_archivo_expediente_gestion = Result
                Exit Function
            End If
            Dim Proceso As String = ""
            Dim Class_ra_registro_proceso As New Class_ra_registro_proceso
            If estru_unidad_conservacion(0).Id_registro_proceso <> 0 Then
                Result = Class_ra_registro_proceso.Solicita_nombre_proceso(estru_unidad_conservacion(0).Id_registro_proceso,
                                                                           Proceso)
                If Result <> "YES" Then
                    Asigna_meta_dato_archivo_expediente_gestion = Result
                    Exit Function
                End If
            End If
            For i As Integer = 0 To stru_detalle_sis_meta_dato.Length - 1
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "Expediente_relacion" Then
                    stru_detalle_sis_meta_dato(i).value = id_expediente
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "nombre_serie" Then
                    stru_detalle_sis_meta_dato(i).value = estru_unidad_conservacion(0).NOMBRE_SERIE
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "nombre_sub_serie" Then
                    stru_detalle_sis_meta_dato(i).value = estru_unidad_conservacion(0).NOMBRE_SUBSERIE
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "nombre_sub_serie" Then
                    stru_detalle_sis_meta_dato(i).value = estru_unidad_conservacion(0).NOMBRE_SUBSERIE
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "codigo_serie" Then
                    stru_detalle_sis_meta_dato(i).value = estru_unidad_conservacion(0).CODIGO_SERIE
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "codigo_sub_serie" Then
                    stru_detalle_sis_meta_dato(i).value = estru_unidad_conservacion(0).CODIGO_SUBSERIE
                End If
                If stru_detalle_sis_meta_dato(i).nombre_meta_dato = "proceso administrativo" Then
                    stru_detalle_sis_meta_dato(i).value = Proceso
                End If
            Next
            Asigna_meta_dato_archivo_expediente_gestion = "YES"
            Exit Function
        Catch ex As Exception
            Asigna_meta_dato_archivo_expediente_gestion = "Inconsistencia general funcion Asigna_meta_dato_archivo_expediente_gestion " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_tipo_expediente(ByVal id_expediente As Integer,
                                             ByRef nombre_expediente As String,
                                             ByRef id_tipo_expediente As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT RA_TIP_EXPE_ID_TIPO_EXPEDIENTE,CODIGO_UNICO " &
            "FROM expediente_archivo " &
            "where ID_EXPEDIENTE=" & id_expediente
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_nombre_tipo_expediente = " Error funcion Solicita_nombre_tipo_expediente  " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_tipo_expediente = "YES"
                Exit Function
            Else
                If Dat_reader.Tables(0).Rows(0).IsNull(0) = False Then
                    nombre_expediente = Dat_reader.Tables(0).Rows(0).Item(0)
                Else
                    nombre_expediente = ""
                End If
                If Dat_reader.Tables(0).Rows(0).IsNull(1) = False Then
                    id_tipo_expediente = Dat_reader.Tables(0).Rows(0).Item(1)
                Else
                    id_tipo_expediente = 0
                End If
                Solicita_nombre_tipo_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_tipo_expediente = "Inconsisencia general funcion Solicita_nombre_tipo_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_datos_expediente_relacion(ByVal id_expediente As Integer,
                                                ByRef matri_gestion As estructure_gestion) As String

        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT RA_TIP_EXPE_ID_TIPO_EXPEDIENTE,CODIGO_UNICO " &
            "FROM expediente_archivo " &
            "where ID_EXPEDIENTE=" & id_expediente
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_expediente_relacion = " Error funcion Solicita_datos_expediente_relacion  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_datos_expediente_relacion = "Imposible encontrar el registro del expediente (" & id_expediente & "), para la asignación de datos de vinculación "
                Exit Function
            Else
                matri_gestion.ID_EXPEDIENTE = id_expediente
                If Datset.Tables(0).Rows(0).IsNull(0) = False Then
                    matri_gestion.ID_TIPO_EXPEDIENTE = Datset.Tables(0).Rows(0).Item(0)
                Else
                    matri_gestion.ID_TIPO_EXPEDIENTE = 0
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = False Then
                    matri_gestion.EXPEDIENTE = Datset.Tables(0).Rows(0).Item(1)
                Else
                    matri_gestion.EXPEDIENTE = ""
                End If
                Solicita_datos_expediente_relacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_expediente_relacion = "Inconsisencia general funcion Solicita_datos_expediente_relacion " & ex.Message
        End Try
    End Function
    Function Auto_registra_gabinete_expediente(ByVal id_gabinete As Integer,
                                               ByVal gabinete As String,
                                               ByVal id_auto_registro As Integer,
                                               ByVal id_imagen As Integer,
                                               ByRef id_expediente As Integer,
                                               ByRef nombre_expediente As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Auto registra expediente con datos del gabinete 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_gabinete           : Representa la identificación del gabinete
        'nombre_gabinete       : Representa el consecutivo de radicado
        'id_imagen             : Representa la identificación de la imagen
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_expediente        : Retorna la idnetificación del expediente
        'nombre_expediente    : Retorna el nombre del expediente
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-13
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            '----------------------------------------------------------------------------------------------------
            '--------------Solicita la relación de campos de gabinete y expediente
            '----------------------------------------------------------------------------------------------------
            Dim Class_ra_auto_rel_campos_gabinete_expediente As New Class_ra_auto_rel_campos_gabinete_expediente
            Dim Ra_auto_rel_campos_gabinete_expediente() As ra_auto_rel_campos_gabinete_expediente = Nothing
            Result = Class_ra_auto_rel_campos_gabinete_expediente.Solicita_estructura_relacion_auto_registro_gabinete_expediente(id_auto_registro,
                                                                                                                                 Ra_auto_rel_campos_gabinete_expediente)
            If Result <> "YES" Then
                Auto_registra_gabinete_expediente = Result
                Exit Function
            End If
            '----------------------------------------------------------------------------------------------------
            '--------------Asigna los valores a la relación de campos de gabinete y expediente desde el gabinete
            '----------------------------------------------------------------------------------------------------
            Dim ClassDaGabinete As New ClassDaGabinete
            Result = ClassDaGabinete.Asigna_valores_campos_gabinete_auto_relacion_gabinete_expediente(id_imagen,
                                                                                                      gabinete,
                                                                                                      Ra_auto_rel_campos_gabinete_expediente)
            If Result <> "YES" Then
                Auto_registra_gabinete_expediente = Result
                Exit Function
            End If
            '--------------------------------------------------------------------------------------------------------------
            '-----------------Solicita los datos de gestión documental para el expediente con la relacion de auto registro
            '--------------------------------------------------------------------------------------------------------------
            Dim Class_ra_auto_campos_gestion_expediente As New Class_ra_auto_campos_gestion_expediente
            Dim id_fondo As Integer = 0
            Dim id_instrumento As Integer = 0
            Dim d_area As Integer = 0
            Dim id_serie As Integer = 0
            Dim id_sub_serie As Integer = 0
            Result = Class_ra_auto_campos_gestion_expediente.SolicitaDatosGestionCamposAutoRegistro(id_auto_registro,
                                                                                                    id_fondo,
                                                                                                    id_instrumento,
                                                                                                    d_area,
                                                                                                    id_serie,
                                                                                                    id_sub_serie)
            If Result <> "YES" Then
                Auto_registra_gabinete_expediente = Result
                Exit Function
            End If
            '----------------------------------------------------------------------------------
            '----------------------Solicita la estructura de campos unicos del auto registro
            '----------------------------------------------------------------------------------
            Dim Class_ra_auto_campo_unico_expediente As New Class_ra_auto_campo_unico_expediente
            Dim stru_campos_expediente() As stru_campos_expediente = Nothing
            Result = Class_ra_auto_campo_unico_expediente.SolicitaCamposUnicosAutoRegistroExpediente(id_auto_registro,
                                                                                                          stru_campos_expediente)
            If Result <> "YES" Then
                Auto_registra_gabinete_expediente = Result
                Exit Function
            End If
            '----------------------------------------------------------------------------------------------------------
            '----------------------Asigna los datos del gabinete desde la relación de campos de gabinete y expediente
            '----------------------------------------------------------------------------------------------------------
            For i As Integer = 0 To Ra_auto_rel_campos_gabinete_expediente.Length - 1
                For z As Integer = 0 To stru_campos_expediente.Length - 1
                    If UCase(stru_campos_expediente(z).campo_expediente) = UCase(Ra_auto_rel_campos_gabinete_expediente(i).nombre_campo) Then
                        stru_campos_expediente(z).valor_campo_expediente = Ra_auto_rel_campos_gabinete_expediente(i).value_campo_expediente
                    End If
                Next
            Next
            '--------------------------------------------------------------------------------
            '------------------------Asigna datos campo gestión
            '--------------------------------------------------------------------------------
            Dim nombre_fondo As Object = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim nombre_area As String = ""
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If d_area <> 0 Then
                Result = Class_areas_depart_radicacion.Solicita_nombre_area_departamento(d_area,
                                                                                         nombre_area)
                If Result <> "YES" Then
                    Auto_registra_gabinete_expediente = Result
                    Exit Function
                End If
            End If
            Dim Class_ra_de_fondo_documental As New Class_ra_de_fondo_documental
            If id_fondo <> 0 Then
                Result = Class_ra_de_fondo_documental.Retorna_nombre_fondo_documental(id_fondo,
                                                                                      nombre_fondo)
                If Result <> "YES" Then
                    Auto_registra_gabinete_expediente = Result
                    Exit Function
                End If
            End If
            Dim Class_series_documentales As New Class_series_documentales
            If id_serie <> 0 Then
                Result = Class_series_documentales.Solicita_nombre_serie_documental(id_serie,
                                                                                    nombre_serie)
                If Result <> "YES" Then
                    Auto_registra_gabinete_expediente = Result
                    Exit Function
                End If
            End If
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If id_sub_serie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(id_sub_serie,
                                                                               nombre_sub_serie)
                If Result <> "YES" Then
                    Auto_registra_gabinete_expediente = Result
                    Exit Function
                End If
            End If
            Dim RefclassGestionInstrumento As New ClassGaGestionInstrumento
            Dim id_organigrama As Integer = 0
            If id_instrumento <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_id_organigrama_instrumento(id_instrumento,
                                                                                        id_organigrama)
                If Result <> "YES" Then
                    Auto_registra_gabinete_expediente = Result
                    Exit Function
                End If
            End If
            Dim nombre_organigrama As String = ""
            If id_organigrama <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_nombre_organigrama_por_identidad_organigrama(id_organigrama,
                                                                                                         nombre_organigrama)
                If Result <> "YES" Then
                    Auto_registra_gabinete_expediente = Result
                    Exit Function
                End If
            End If
            Dim Class_tipo_unidad_conservacion As New Class_tipo_unidad_conservacion
            Dim id_tipo_unidad_conservacion As Integer = 0
            Dim nombre_tipo_unidad_conservacion As String = "CARPETA CUATRO ALETAS"
            Result = Class_tipo_unidad_conservacion.Retorna_id_tipo_unidad_conservacion_expediente(nombre_tipo_unidad_conservacion,
                                                                                                   id_tipo_unidad_conservacion,
                                                                                                   2)
            If Result <> "YES" Then
                Auto_registra_gabinete_expediente = Result
                Exit Function
            End If
            Dim id_tipo_expediente_carpeta As Integer = 0
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            Result = ref_Class_ra_tipo_expediente.Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido(id_tipo_expediente_carpeta,
                                                                                                                    0)
            If Result <> "YES" Then
                Auto_registra_gabinete_expediente = Result
                Exit Function
            End If

            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Today
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                Auto_registra_gabinete_expediente = Result
                Exit Function
            End If
            '--------------------------------------------
            'Asigna datos de gestión para el expediente
            '--------------------------------------------
            For i As Integer = 0 To stru_campos_expediente.Length - 1
                Select Case stru_campos_expediente(i).campo_expediente
                    Case "CODIGO_AREA_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = d_area
                    Case "NOMBRE_AREA_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_area
                    Case "CODIGO_SERIE_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = id_serie
                    Case "NOMBRE_SERIE_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_serie
                    Case "CODIGO_SUB_SERIE_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = id_sub_serie
                    Case "NOMBRE_SUBSERIE_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_sub_serie
                    Case "NOMBRE_TIPO_UNIDAD_DOCUMENTAL"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_tipo_unidad_conservacion
                    Case "ID_TIPO_UNIDAD_DOCUMENTAL"
                        stru_campos_expediente(i).valor_campo_expediente = id_tipo_unidad_conservacion
                    Case "ID_FONDO"
                        stru_campos_expediente(i).valor_campo_expediente = id_fondo
                    Case "NOMBRE_FONDO"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_fondo
                    Case "id_instrumento"
                        stru_campos_expediente(i).valor_campo_expediente = id_instrumento
                    Case "FECHA_CREACION"
                        stru_campos_expediente(i).valor_campo_expediente = date1al
                End Select
            Next

            Dim codigo_unico As String = ""
            Dim fecha_creacion As String = ""
            Dim tema_expediente As String = ""
            Dim asunto_expediente As String = ""
            Dim observacion_expediente As String = ""
            Dim nombre_persona_expediente As String = ""
            Dim identificacion_persona_expediente As String = ""
            Dim nombre_responsable_expediente As String = ""
            Dim idenficacion_responsable_expediente As String = ""
            '-----------------------------------
            'Valida campos obligatorios
            '-----------------------------------
            For i As Integer = 0 To stru_campos_expediente.Length - 1
                If stru_campos_expediente(i).estado_obligatorio = 1 And stru_campos_expediente(i).valor_campo_expediente = "" Then
                    Auto_registra_gabinete_expediente = "El campo (" & stru_campos_expediente(i).campo_expediente & ") debe ser informado en el gabinete (" & gabinete & ")"
                    Exit Function
                End If
            Next
            '---------------------------------------------------------------------
            '---------------------------------Asigna datos fijos del expediente
            '----------------------------------------------------------------------
            For i As Integer = 0 To stru_campos_expediente.Length - 1
                Select Case stru_campos_expediente(i).campo_expediente
                    Case "CODIGO_UNICO"
                        codigo_unico = stru_campos_expediente(i).valor_campo_expediente
                    Case "TEMA_EXPEDIENTE"
                        tema_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "ASUNTO_EXPEDIENTE"
                        asunto_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "OBSERVACION_EXPEDIENTE"
                        observacion_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "NOMBRE_PERSONA_EXPEDIENTE"
                        nombre_persona_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "IDENTIFICACION_PERSONA_EXPEDIENTE"
                        identificacion_persona_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "NOMBRE_RESPONSABLE_EXPEDIENTE"
                        nombre_responsable_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "IDENFICACION_RESPONSABLE_EXPEDIENTE"
                        idenficacion_responsable_expediente = stru_campos_expediente(i).valor_campo_expediente
                End Select
            Next
            Result = Me.Valida_existencia_expediente_auto_registro(stru_campos_expediente,
                                                                   id_expediente)
            If Result <> "YES" Then
                Auto_registra_gabinete_expediente = Result
                Exit Function
            End If
            nombre_expediente = codigo_unico
            Dim Refclas As New ClassGaExpediente
            Dim estado_codigo_unico As Integer = 1
            Dim requiere_unida_conservacion_fisica As Integer = 0
            Dim option_obliga_archivo_unidad As Integer = 0
            Dim id_registro_relacion As Integer = 0
            If id_expediente = 0 Then
                Result = Refclas.Registrar_Expediente_Conservacion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                   codigo_unico,
                                                                   estado_codigo_unico,
                                                                   HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                                   date1al,
                                                                   "",
                                                                   "",
                                                                   "",
                                                                   tema_expediente,
                                                                   nombre_organigrama,
                                                                   nombre_area,
                                                                   nombre_serie,
                                                                   nombre_sub_serie,
                                                                   id_tipo_expediente_carpeta,
                                                                   "0",
                                                                   "0",
                                                                   "0",
                                                                    asunto_expediente,
                                                                   1,
                                                                   "",
                                                                   id_expediente,
                                                                   observacion_expediente,
                                                                   "COMPUESTA(EXPEDIENTE)",
                                                                   "",
                                                                   option_obliga_archivo_unidad,
                                                                   "0",
                                                                   "0",
                                                                   requiere_unida_conservacion_fisica,
                                                                   "Archivo Gestión",
                                                                   nombre_fondo,
                                                                   nombre_persona_expediente,
                                                                   identificacion_persona_expediente,
                                                                   nombre_responsable_expediente,
                                                                   idenficacion_responsable_expediente,
                                                                   codigo_unico,
                                                                   0,
                                                                   5,
                                                                   id_instrumento,
                                                                   gabinete,
                                                                   0,
                                                                   id_registro_relacion,
                                                                   id_auto_registro)
                If Result <> "YES" Then
                    Auto_registra_gabinete_expediente = Result
                    Exit Function
                Else
                    Auto_registra_gabinete_expediente = Result
                    Exit Function
                End If
            Else
                Auto_registra_gabinete_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Auto_registra_gabinete_expediente = "Inconsistencia general funcion Auto_registra_gabinete_expediente " & ex.Message
        End Try
    End Function
    Function AutoRegistraExpedienteTramite(ByVal IdTipoDocEntrante As Integer,
                                           ByVal Parametro As Object,
                                           ByVal IdTareaWorkflow As Long,
                                           ByVal IdNivelPadre As Integer,
                                           ByRef IdExpediente As Integer,
                                           ByRef NombreExpediente As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Auto registra expeidente con al opción de registrarlo a un nivel
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoDocEntrante    : Representa la identificación del tramite de auto vinculación
        'radicado             : Representa el radicado que lo auto vincula
        'id_tarea_workflow    : Representa la idneitifcación de la tarea workflow que lo vincula
        'id_nivel_padre       : Representa la identificación del nivel padre
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdExpediente            : Retorna la idnetificación del expediente
        'NombreExpediente        : Retorna el codigo unico o nombre del expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim IdAutoRegistro As Integer = 0
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            '//-----------Solicita la reación del tramite y la plantilla de uto registro--------////
            Result = Class_tipo_doc_entrante.SolicitaidAutoRegistroExpediente(IdTipoDocEntrante,
                                                                              IdAutoRegistro)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            If IdAutoRegistro = 0 Then
                AutoRegistraExpedienteTramite = "El  tramite (" & IdTipoDocEntrante & ")  no tiene relacionado el auto registro"
                Exit Function
            End If
            Dim Class_ra_auto_registro_expediente As New Class_ra_auto_registro_expediente
            Dim NombreAutoRegistro As String = ""
            Dim FuncionServicioDatos As String = ""
            '//------Solicita los datos de la función de auto registro----////
            Result = Class_ra_auto_registro_expediente.SolicitaDatosAutoRegistro(IdAutoRegistro,
                                                                                 NombreAutoRegistro,
                                                                                 FuncionServicioDatos)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            Dim Class_ra_auto_campos_gestion_expediente As New Class_ra_auto_campos_gestion_expediente
            Dim IdFondo As Integer = 0
            Dim IdInstrumento As Integer = 0
            Dim IdArea As Integer = 0
            Dim IdSerie As Integer = 0
            Dim IdSubSerie As Integer = 0
            '//------------Solicita datos de gestión documental-----------///
            Result = Class_ra_auto_campos_gestion_expediente.SolicitaDatosGestionCamposAutoRegistro(IdAutoRegistro,
                                                                                                    IdFondo,
                                                                                                    IdInstrumento,
                                                                                                    IdArea,
                                                                                                    IdSerie,
                                                                                                    IdSubSerie)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            Dim Class_ra_auto_campo_unico_expediente As New Class_ra_auto_campo_unico_expediente
            Dim stru_campos_expediente() As stru_campos_expediente = Nothing
            '//-----Solicita la estructura de campos del expediente-------//////
            Result = Class_ra_auto_campo_unico_expediente.SolicitaCamposUnicosAutoRegistroExpediente(IdAutoRegistro,
                                                                                                     stru_campos_expediente)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            '//-----Asigna los datos de auto registro-----/////
            Result = Class_ra_auto_registro_expediente.SolicitaDatosFuncionAutoRegistro(FuncionServicioDatos,
                                                                                        Parametro,
                                                                                        IdAutoRegistro,
                                                                                        stru_campos_expediente)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            '------------------------------------------
            'Campos expediente
            '------------------------------------------
            'CODIGO_UNICO
            'FECHA_CREACION
            'CODIGO_AREA_TRD
            'NOMBRE_AREA_TRD
            'CODIGO_SERIE_TRD
            'NOMBRE_SERIE_TRD
            'CODIGO_SUB_SERIE_TRD
            'NOMBRE_SUBSERIE_TRD
            'TEMA_EXPEDIENTE
            'ASUNTO_EXPEDIENTE
            'OBSERVACION_EXPEDIENTE
            'ID_TIPO_UNIDAD_DOCUMENTAL
            'NOMBRE_TIPO_UNIDAD_DOCUMENTAL
            'ID_SUB_AREA
            'NOMBRE_SUB_AREA
            'NOMBRE_PERSONA_EXPEDIENTE
            'IDENTIFICACION_PERSONA_EXPEDIENTE
            'ID_FONDO
            'NOMBRE_FONDO
            'NOMBRE_RESPONSABLE_EXPEDIENTE
            'IDENFICACION_RESPONSABLE_EXPEDIENTE
            'id_instrumento
            '------------------------------------------
            '------------------------------------------
            'Asigna datos campo gestión
            '------------------------------------------
            Dim nombre_fondo As Object = ""
            Dim nombre_serie As String = ""
            Dim nombre_sub_serie As String = ""
            Dim nombre_area As String = ""
            Dim Class_areas_depart_radicacion As New Class_areas_depart_radicacion
            If IdArea <> 0 Then
                Result = Class_areas_depart_radicacion.Solicita_nombre_area_departamento(IdArea,
                                                                                         nombre_area)
                If Result <> "YES" Then
                    AutoRegistraExpedienteTramite = Result
                    Exit Function
                End If
            End If
            Dim Class_ra_de_fondo_documental As New Class_ra_de_fondo_documental
            If IdFondo <> 0 Then
                Result = Class_ra_de_fondo_documental.Retorna_nombre_fondo_documental(IdFondo,
                                                                                      nombre_fondo)
                If Result <> "YES" Then
                    AutoRegistraExpedienteTramite = Result
                    Exit Function
                End If
            End If
            Dim Class_series_documentales As New Class_series_documentales
            If IdSerie <> 0 Then
                Result = Class_series_documentales.Solicita_nombre_serie_documental(IdSerie,
                                                                                    nombre_serie)
                If Result <> "YES" Then
                    AutoRegistraExpedienteTramite = Result
                    Exit Function
                End If
            End If
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If IdSubSerie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(IdSubSerie,
                                                                               nombre_sub_serie)
                If Result <> "YES" Then
                    AutoRegistraExpedienteTramite = Result
                    Exit Function
                End If
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Dim id_imagen As Integer = 0
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(IdTareaWorkflow,
                                                                 nombre_gabinete,
                                                                 id_imagen)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            Dim RefclassGestionInstrumento As New ClassGaGestionInstrumento
            Dim id_organigrama As Integer = 0
            If IdInstrumento <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_id_organigrama_instrumento(IdInstrumento,
                                                                                        id_organigrama)
                If Result <> "YES" Then
                    AutoRegistraExpedienteTramite = Result
                    Exit Function
                End If
            End If
            Dim nombre_organigrama As String = ""
            If id_organigrama <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_nombre_organigrama_por_identidad_organigrama(id_organigrama,
                                                                                                         nombre_organigrama)
                If Result <> "YES" Then
                    AutoRegistraExpedienteTramite = Result
                    Exit Function
                End If
            End If
            Dim Class_tipo_unidad_conservacion As New Class_tipo_unidad_conservacion
            Dim id_tipo_unidad_conservacion As Integer = 0
            Dim nombre_tipo_unidad_conservacion As String = "CARPETA CUATRO ALETAS"
            Result = Class_tipo_unidad_conservacion.Retorna_id_tipo_unidad_conservacion_expediente(nombre_tipo_unidad_conservacion,
                                                                                                   id_tipo_unidad_conservacion,
                                                                                                   2)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            '---------------------------------------------------
            'Retorn tipo de expediente electrónico 
            '---------------------------------------------------
            Dim id_tipo_expediente_carpeta As Integer = 0
            Dim ref_Class_ra_tipo_expediente As New Class_ra_tipo_expediente
            Result = ref_Class_ra_tipo_expediente.Solicita_la_identificacion_del_tipo_de_expediente_carpeta_hibrido(id_tipo_expediente_carpeta,
                                                                                                                    0)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If


            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Today
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            '--------------------------------------------
            'Asigna datos de gestión para el expediente
            '--------------------------------------------
            For i As Integer = 0 To stru_campos_expediente.Length - 1
                Select Case stru_campos_expediente(i).campo_expediente
                    Case "CODIGO_AREA_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = IdArea
                    Case "NOMBRE_AREA_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_area
                    Case "CODIGO_SERIE_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = IdSerie
                    Case "NOMBRE_SERIE_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_serie
                    Case "CODIGO_SUB_SERIE_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = IdSubSerie
                    Case "NOMBRE_SUBSERIE_TRD"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_sub_serie
                    Case "NOMBRE_TIPO_UNIDAD_DOCUMENTAL"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_tipo_unidad_conservacion
                    Case "ID_TIPO_UNIDAD_DOCUMENTAL"
                        stru_campos_expediente(i).valor_campo_expediente = id_tipo_unidad_conservacion
                    Case "ID_FONDO"
                        stru_campos_expediente(i).valor_campo_expediente = IdFondo
                    Case "NOMBRE_FONDO"
                        stru_campos_expediente(i).valor_campo_expediente = nombre_fondo
                    Case "id_instrumento"
                        stru_campos_expediente(i).valor_campo_expediente = IdInstrumento
                    Case "FECHA_CREACION"
                        stru_campos_expediente(i).valor_campo_expediente = date1al
                End Select
            Next
            Dim codigo_unico As String = ""
            Dim fecha_creacion As String = ""
            Dim tema_expediente As String = ""
            Dim asunto_expediente As String = ""
            Dim observacion_expediente As String = ""
            Dim nombre_persona_expediente As String = ""
            Dim identificacion_persona_expediente As String = ""
            Dim nombre_responsable_expediente As String = ""
            Dim idenficacion_responsable_expediente As String = ""
            '-----------------------------------
            'Valida campos obligatorios
            '-----------------------------------
            For i As Integer = 0 To stru_campos_expediente.Length - 1
                If stru_campos_expediente(i).estado_obligatorio = 1 And stru_campos_expediente(i).valor_campo_expediente = "" Then
                    AutoRegistraExpedienteTramite = "El campo (" & stru_campos_expediente(i).campo_expediente & ") debe ser informado"
                    Exit Function
                End If
            Next
            '----------------------------------
            'Asigna datos fijos del expediente
            '----------------------------------
            For i As Integer = 0 To stru_campos_expediente.Length - 1
                Select Case stru_campos_expediente(i).campo_expediente
                    Case "CODIGO_UNICO"
                        codigo_unico = stru_campos_expediente(i).valor_campo_expediente
                    Case "TEMA_EXPEDIENTE"
                        tema_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "ASUNTO_EXPEDIENTE"
                        asunto_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "OBSERVACION_EXPEDIENTE"
                        observacion_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "NOMBRE_PERSONA_EXPEDIENTE"
                        nombre_persona_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "IDENTIFICACION_PERSONA_EXPEDIENTE"
                        identificacion_persona_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "NOMBRE_RESPONSABLE_EXPEDIENTE"
                        nombre_responsable_expediente = stru_campos_expediente(i).valor_campo_expediente
                    Case "IDENFICACION_RESPONSABLE_EXPEDIENTE"
                        idenficacion_responsable_expediente = stru_campos_expediente(i).valor_campo_expediente
                End Select
            Next
            Result = Me.Valida_existencia_expediente_auto_registro(stru_campos_expediente,
                                                                   IdExpediente)
            If Result <> "YES" Then
                AutoRegistraExpedienteTramite = Result
                Exit Function
            End If
            Dim Refclas As New ClassGaExpediente
            Dim estado_codigo_unico As Integer = 1
            Dim requiere_unida_conservacion_fisica As Integer = 0
            Dim option_obliga_archivo_unidad As Integer = 0
            Dim id_registro_relacion As Integer = 0
            NombreExpediente = codigo_unico
            If IdExpediente = 0 Then
                Result = Refclas.Registrar_Expediente_Conservacion(HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION"),
                                                                   codigo_unico,
                                                                   estado_codigo_unico,
                                                                   HttpContext.Current.Session.Item("GA_IDEMPRESA"),
                                                                   date1al,
                                                                   "",
                                                                   "",
                                                                   "",
                                                                   tema_expediente,
                                                                   nombre_organigrama,
                                                                   nombre_area,
                                                                   nombre_serie,
                                                                   nombre_sub_serie,
                                                                   id_tipo_expediente_carpeta,
                                                                   "0",
                                                                   "0",
                                                                   "0",
                                                                    asunto_expediente,
                                                                   1,
                                                                   "",
                                                                   IdExpediente,
                                                                   observacion_expediente,
                                                                   "COMPUESTA(EXPEDIENTE)",
                                                                   "",
                                                                   option_obliga_archivo_unidad,
                                                                   "0",
                                                                   "0",
                                                                   requiere_unida_conservacion_fisica,
                                                                   "Archivo Gestión",
                                                                   nombre_fondo,
                                                                   nombre_persona_expediente,
                                                                   identificacion_persona_expediente,
                                                                   nombre_responsable_expediente,
                                                                   idenficacion_responsable_expediente,
                                                                   codigo_unico,
                                                                   0,
                                                                   5,
                                                                   IdInstrumento,
                                                                   nombre_gabinete,
                                                                   IdNivelPadre,
                                                                   id_registro_relacion,
                                                                   IdAutoRegistro)
                If Result <> "YES" Then
                    AutoRegistraExpedienteTramite = Result
                    Exit Function
                Else
                    AutoRegistraExpedienteTramite = Result
                    Exit Function
                End If
            Else
                AutoRegistraExpedienteTramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            AutoRegistraExpedienteTramite = "Inconsistencia general funcion Auto_registra_expediente_tramite" & ex.Message
        End Try
    End Function
    Function Valida_existencia_expediente_auto_registro(ByVal stru_campos_expediente() As stru_campos_expediente,
                                                        ByRef id_expediente As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la identificación del expediente con la estructura
        '          de campos del expediente 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'stru_campos_expediente: Representa la estructuctura con los campos y los 
        '                        valores para realizar la consulta de la existerncia
        '                        del expediente según los criterios
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_expediente  : Retorna la idnetificación del expediente
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------

        Try
            Dim sql_condicion As String = ""
            For i As Integer = 0 To stru_campos_expediente.Length - 1
                If stru_campos_expediente(i).estado_unico = 1 Then
                    If sql_condicion = "" Then
                        sql_condicion = " where " & stru_campos_expediente(i).campo_expediente & "='" & stru_campos_expediente(i).valor_campo_expediente & "'"
                    Else
                        sql_condicion = sql_condicion & " and " & stru_campos_expediente(i).campo_expediente & "='" & stru_campos_expediente(i).valor_campo_expediente & "'"
                    End If
                End If
            Next
            If sql_condicion = "" Then
                Valida_existencia_expediente_auto_registro = "La función de auto registro de expediente, no registra campos unicos de comparación"
                Exit Function
            End If
            Dim sql_consulta As String = "Select ID_EXPEDIENTE from expediente_archivo " & sql_condicion
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Valida_existencia_expediente_auto_registro = "Funcion  Valida_existencia_expediente_auto_registro dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_expediente = 0
                Valida_existencia_expediente_auto_registro = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_expediente = 0
                Else
                    id_expediente = Datset.Tables(0).Rows(0).Item(0)
                End If
                Valida_existencia_expediente_auto_registro = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Valida_existencia_expediente_auto_registro = "Inconsistencia general función Valida_existencia_expediente_auto_registro " & ex.Message
        End Try
    End Function
    Function Activa_vincula_documento_expediente(ByVal item_list As List(Of class_item_element)) As String
        '-----------------------------------------------------------------------------
        'Funcion : Activa la vinculación de los documentos seleccionados al expediente 
        '
        '  
        '------------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'item_list             : Representa la estructura de identificación de los
        '                        documentos a vincular en el expediente
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-06-08
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Classselecciotarea As New Classselecciotarea
            Dim Result As String = ""
            If HttpContext.Current.Session.Item("RELACIONA_EXPEDIENTE") = 0 Then
                Activa_vincula_documento_expediente = "Usuario sin permmisos para vincular documentos  a expediente"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = 0 Then
                Activa_vincula_documento_expediente = "Usuario sin tarea seleccionada, imposible vincular documentos "
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                Activa_vincula_documento_expediente = Result
                Exit Function
            End If
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            For i As Integer = 0 To item_list.Count - 1
                ReDim Preserve HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA")(i)
                HttpContext.Current.Session.Item("WF_MATRI_VINCULA_ESTRUCTURA")(i) = item_list(i).id_item & "|" & nombre_gabinete
            Next
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Activa_vincula_documento_expediente = Result
                Exit Function
            End If
            HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 2
            Activa_vincula_documento_expediente = "YES"
        Catch ex As Exception
            Activa_vincula_documento_expediente = "Inconsistencia general funcion Activa_vincula_documento_expediente " & ex.Message
        End Try
    End Function
    Function Activa_copia_documento_a_expediente_produccion(ByVal item_list As List(Of class_item_element)) As String
        '---------------------------------------------------------------------------
        'Funcion : Activa la copia de documentos al expediente produccion,
        'asignado los id
        '          de la producción documental a la variable general
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'item_list             : Representa la estructura de identificación de los
        '                        documentos a copiar en el expediente
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-06-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            If HttpContext.Current.Session.Item("COPIA_ESTRUCTURA_PRODUCION") = 0 Then
                Activa_copia_documento_a_expediente_produccion = "Usuario sin permmisos para copiar documentos a estructutura de produción documental"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Activa_copia_documento_a_expediente_produccion = "Usuario sin tarea seleccionada imposible copiar documentos"
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                Activa_copia_documento_a_expediente_produccion = Result
                Exit Function
            End If
            For i As Integer = 0 To item_list.Count - 1
                ReDim Preserve HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i)
                Dim id_registro_producion As Long = 0
                Result = ClassGaProducionDocumental.Solicita_id_registro_producion_documental(item_list(i).id_item,
                                                                                              nombre_gabinete,
                                                                                              id_registro_producion)
                If Result <> "YES" Then
                    Activa_copia_documento_a_expediente_produccion = Result
                    Exit Function
                End If
                If id_registro_producion = 0 Then
                    Result = ClassGaProducionDocumental.Registra_documento_inventario_documental(item_list(i).id_item,
                                                                                                 nombre_gabinete,
                                                                                                 id_registro_producion)
                    If Result <> "YES" Then
                        Activa_copia_documento_a_expediente_produccion = Result
                        Exit Function
                    End If
                End If
                HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i) = id_registro_producion
            Next
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Activa_copia_documento_a_expediente_produccion = Result
                Exit Function
            Else
                'HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 1
                Activa_copia_documento_a_expediente_produccion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Activa_copia_documento_a_expediente_produccion = "Inconsistencia general funcion Activa_copia_documento_a_expediente_produccion " & ex.Message
        End Try
    End Function
    Function Activa_copia_documento_a_expediente(ByVal item_list As List(Of class_item_element)) As String
        '---------------------------------------------------------------------------
        'Funcion : Activa la copia de documentos al expediente, asignado los id
        '          de la producción documental a la variable general
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'item_list             : Representa la estructura de identificación de los
        '                        documentos a copiar en el expediente
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        '
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-05-29
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassGaProducionDocumental As New ClassGaProducionDocumental
            If HttpContext.Current.Session.Item("COPIA_DOCUMENTO_EXPEDIENTE") = 0 Then
                Activa_copia_documento_a_expediente = "Usuario sin permmisos para copiar documentos a expediente"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Activa_copia_documento_a_expediente = "Usuario sin tarea seleccionada imposible copiar documentos"
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                Activa_copia_documento_a_expediente = Result
                Exit Function
            End If
            For i As Integer = 0 To item_list.Count - 1
                ReDim Preserve HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i)
                Dim id_registro_producion As Long = 0
                Result = ClassGaProducionDocumental.Solicita_id_registro_producion_documental(item_list(i).id_item,
                                                                                              nombre_gabinete,
                                                                                              id_registro_producion)
                If Result <> "YES" Then
                    Activa_copia_documento_a_expediente = Result
                    Exit Function
                End If
                If id_registro_producion = 0 Then
                    Result = ClassGaProducionDocumental.Registra_documento_inventario_documental(item_list(i).id_item,
                                                                                                 nombre_gabinete,
                                                                                                 id_registro_producion)
                    If Result <> "YES" Then
                        Activa_copia_documento_a_expediente = Result
                        Exit Function
                    End If
                End If
                HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA")(i) = id_registro_producion
            Next
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.Solicita_radicado_id_tarea_seleccionada(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                    HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Activa_copia_documento_a_expediente = Result
                Exit Function
            Else
                HttpContext.Current.Session.Item("WF_ESTADO_GESTION_EXPEDIENTE") = 1
                Activa_copia_documento_a_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Activa_copia_documento_a_expediente = "Inconsistencia general funcion Activa_copia_documento_a_expediente " & ex.Message
        End Try
    End Function
    Function SolicitaValoresCampoGabineteCampoExpediente(ByVal IdExpediente As Integer,
                                                         ByRef StruRelExpGabinete() As stru_rel_exp_gabinete) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Solicita valor campos gabinetes expedientes 
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'id_relacion_expediente_gabinete   : Representa la identificación del expediente 
        '                                  : 
        '
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------
        'stru_rel_exp_gabinete      : Retorna el valor de de campos y
        '                             gabinetes recuperados desde el expediente
        '-----------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------
        'Fecha                 : 2023-06-05
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim sql_campos As String = ""
            For i As Integer = 0 To StruRelExpGabinete.Length - 1
                If sql_campos = "" Then
                    sql_campos = StruRelExpGabinete(i).nombre_campo
                Else
                    sql_campos = sql_campos & "," & StruRelExpGabinete(i).nombre_campo
                End If
            Next
            Dim sql_consulta As String = "Select " & sql_campos & " from  expediente_archivo where ID_EXPEDIENTE=" & IdExpediente
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim ClassGestionFechas As New ClassGestionFechas
            Dim Datset As DataSet = New DataSet("expediente_archivo")
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaValoresCampoGabineteCampoExpediente = "Error funcion SolicitaValoresCampoGabineteCampoExpediente " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaValoresCampoGabineteCampoExpediente = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    If Datset.Tables(0).Rows(0).IsNull(i) = True Then
                        If StruRelExpGabinete(i).tipo_campo = "INT" Or StruRelExpGabinete(i).tipo_campo = "LONG" Then
                            StruRelExpGabinete(i).valor_campo_expediente = 0
                            StruRelExpGabinete(i).valor_campo_gabinete = 0
                        Else
                            StruRelExpGabinete(i).valor_campo_expediente = ""
                            StruRelExpGabinete(i).valor_campo_gabinete = ""
                        End If
                    Else
                        Select Case StruRelExpGabinete(i).tipo_campo
                            Case "DATE"
                                If StruRelExpGabinete(i).TIPO <> "DATE" Then
                                    SolicitaValoresCampoGabineteCampoExpediente = "Incosistencia en la relación de campos expediente a gabinete, diferencia en los tipos de datos (" &
                                   StruRelExpGabinete(i).CAMPO & "-" & StruRelExpGabinete(i).TIPO & ") (" & StruRelExpGabinete(i).nombre_campo & "-" &
                                   StruRelExpGabinete(i).tipo_campo & ")"
                                    Exit Function
                                End If
                                ClassGestionFechas.Formatea_fecha_time_base_mysql(Datset.Tables(0).Rows(0).Item(i).ToString,
                                                                                 StruRelExpGabinete(i).valor_campo_gabinete)
                                StruRelExpGabinete(i).valor_campo_gabinete = Left(StruRelExpGabinete(i).valor_campo_gabinete, "10")
                                StruRelExpGabinete(i).valor_campo_expediente = StruRelExpGabinete(i).valor_campo_gabinete
                            Case "DATETIME"
                                If StruRelExpGabinete(i).TIPO <> "DATETIME" Then
                                    SolicitaValoresCampoGabineteCampoExpediente = "Incosistencia en la relación de campos expediente a gabinete,  diferencia en los tipos de datos (" &
                                   StruRelExpGabinete(i).CAMPO & "-" & StruRelExpGabinete(i).TIPO & ") (" & StruRelExpGabinete(i).nombre_campo & "-" &
                                   StruRelExpGabinete(i).tipo_campo & ")"
                                    Exit Function
                                End If
                                ClassGestionFechas.Formatea_fecha_time_base_mysql(Datset.Tables(0).Rows(0).Item(i).ToString,
                                                                                  StruRelExpGabinete(i).valor_campo_gabinete)
                                StruRelExpGabinete(i).valor_campo_expediente = StruRelExpGabinete(i).valor_campo_gabinete
                            Case "INT"
                                If StruRelExpGabinete(i).TIPO <> "INT" Then
                                    SolicitaValoresCampoGabineteCampoExpediente = "Incosistencia en la relación de campos expediente a gabinete,  diferencia en los tipos de datos (" &
                                   StruRelExpGabinete(i).CAMPO & "-" & StruRelExpGabinete(i).TIPO & ") (" & StruRelExpGabinete(i).nombre_campo & "-" &
                                   StruRelExpGabinete(i).tipo_campo & ")"
                                    Exit Function
                                End If
                                StruRelExpGabinete(i).valor_campo_gabinete = Datset.Tables(0).Rows(0).Item(i)
                                StruRelExpGabinete(i).valor_campo_expediente = StruRelExpGabinete(i).valor_campo_gabinete

                            Case "LONG"
                                If StruRelExpGabinete(i).TIPO <> "INT" Then
                                    SolicitaValoresCampoGabineteCampoExpediente = "Incosistencia en la relación de campos expediente a gabinete, hay diferencia en los tipos de datos (" &
                                   StruRelExpGabinete(i).CAMPO & "-" & StruRelExpGabinete(i).TIPO & ") (" & StruRelExpGabinete(i).nombre_campo & "-" &
                                   StruRelExpGabinete(i).tipo_campo & ")"
                                    Exit Function
                                End If
                                StruRelExpGabinete(i).valor_campo_gabinete = Datset.Tables(0).Rows(0).Item(i)
                                StruRelExpGabinete(i).valor_campo_expediente = StruRelExpGabinete(i).valor_campo_gabinete
                            Case "VARCHAR"
                                Dim valor_replace As String = ""
                                If InStr(StruRelExpGabinete(i).TIPO, "VARCHAR") > 0 Then
                                    valor_replace = StruRelExpGabinete(i).TIPO.Replace("VARCHAR", "")
                                    valor_replace = valor_replace.Replace("(", "")
                                    valor_replace = valor_replace.Replace(")", "")
                                    StruRelExpGabinete(i).valor_campo_gabinete = Left(Datset.Tables(0).Rows(0).Item(i), Val(valor_replace))
                                    StruRelExpGabinete(i).valor_campo_expediente = StruRelExpGabinete(i).valor_campo_gabinete
                                Else
                                    SolicitaValoresCampoGabineteCampoExpediente = "Incosistencia en la relación de campos expediente a gabinete, hay diferencia en los tipos de datos (" &
                                    StruRelExpGabinete(i).CAMPO & "-" & StruRelExpGabinete(i).TIPO & ") (" & StruRelExpGabinete(i).nombre_campo & "-" &
                                    StruRelExpGabinete(i).tipo_campo & ")"
                                    Exit Function
                                End If
                            Case Else
                                StruRelExpGabinete(i).valor_campo_gabinete = Datset.Tables(0).Rows(0).Item(i)
                                StruRelExpGabinete(i).valor_campo_expediente = StruRelExpGabinete(i).valor_campo_gabinete
                        End Select
                    End If
                Next
                SolicitaValoresCampoGabineteCampoExpediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaValoresCampoGabineteCampoExpediente = "Inconsistencia general funcion SolicitaValoresCampoGabineteCampoExpediente " & ex.Message
        End Try
    End Function
    Function Auto_vincula_documentos_a_expediente(ByRef parameter_gestion As ClassExpedienteVincula) As String
        '---------------------------------------------------------------------------
        'Funcion : Crea expediente autoamticamente y vincula documentos a expediente
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ID_FLUJO          : Retorna el identificador de la tarea worfkflow
        'TIPO_COPIA        : Retorna el tipo copia del expediente
        'RADICADO          : Retorna el radicado relacionado al expediente
        'id_expediente     : Retorna la identificación del expediente
        'nombre_expediente : Retorna el nombre del expediente
        'list_image        : Retorna la lista de imagenes (gabinete, id_imagen)
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-06-11
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim id_expediente As Integer = 0
            Dim nombre_expediente As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            If HttpContext.Current.Session.Item("RELACIONA_EXPEDIENTE") = 0 Then
                Auto_vincula_documentos_a_expediente = "Usuario sin permmisos para vincular archivos a expediente"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Auto_vincula_documentos_a_expediente = "Usuario sin tarea seleccionada imposible vincular documentos"
                Exit Function
            End If
            Dim stru_paramter_image_final As stru_imagen_gabinete_workflow() = Nothing
            Result = ClassDaGabinete.SolicitaListaImagensGabineteRelacionTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                       stru_paramter_image_final)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente = Result
                Exit Function
            End If
            If stru_paramter_image_final Is Nothing Then
                Auto_vincula_documentos_a_expediente = "Imposible encontrar documentos para relacionar al expediente de la tarea (" &
                    HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") & ")"
                Exit Function
            End If
            HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA") = Nothing
            Dim resultList_ = New List(Of list_imagen_expediente_service)()
            For i As Integer = 0 To stru_paramter_image_final.Length - 1
                Dim parameter_gestion_ As list_imagen_expediente_service = New list_imagen_expediente_service()
                parameter_gestion_.gabinete = stru_paramter_image_final(i).gabinete
                parameter_gestion_.id_imagen = stru_paramter_image_final(i).id_image
                resultList_.Add(parameter_gestion_)
            Next
            parameter_gestion.list_image = resultList_
            parameter_gestion.id_flujo = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaCodigoBarrasIdTareaWorflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                         HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente = Result
                Exit Function
            End If
            parameter_gestion.radicado = HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA")
            parameter_gestion.tipo_copia = 2
            Dim nombre_campo_tramite As String = ""
            Dim tramite As String = ""
            Dim id_tipo_doc_entrante As Integer = 0
            Dim class_config_list_ruta As New Class_configuracion_listado_ruta
            Result = class_config_list_ruta.SolicitaNombreCampoTramiteRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                           nombre_campo_tramite)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente = Result
                Exit Function
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaTramiteFlujoWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                            HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                            nombre_campo_tramite,
                                                                            HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                            tramite,
                                                                            0)

            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(tramite,
                                                                               id_tipo_doc_entrante)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente = Result
                Exit Function
            End If
            If id_tipo_doc_entrante = 0 Then
                Auto_vincula_documentos_a_expediente = "Imposible econtrar el registro del tramite (" & tramite & "), por favor registre el tramite y relacione con el auto registro"
                Exit Function
            End If
            Result = ClassGaExpediente.AutoRegistraExpedienteTramite(id_tipo_doc_entrante,
                                                                        HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"),
                                                                        HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                        0,
                                                                        id_expediente,
                                                                        nombre_expediente)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente = Result
                Exit Function
            Else
                parameter_gestion.nombre_expediente = nombre_expediente
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = id_expediente
                Auto_vincula_documentos_a_expediente = Result
                Exit Function
            End If
        Catch ex As Exception
            Auto_vincula_documentos_a_expediente = "Inconsistencia general funcion Auto_vincula_documentos_a_expediente " & ex.Message
        End Try
    End Function
    Function CreaExpedienteIntegracionSII(ByVal IdTramite As Integer,
                                          ByVal NombreRuta As String,
                                          ByVal IdRutaWorkflow As Integer,
                                          ByVal IdTareaWorkflow As Long,
                                          ByVal CIncripcionSII As List(Of CIncripcionSII),
                                          ByRef ClassExpedienteVincula As ClassExpedienteVincula) As String
        '---------------------------------------------------------------------------
        'Funcion : Crea expediente autoamticamente con los parametros de integración
        '          con el sistema SII
        '         
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ID_FLUJO          : Retorna el identificador de la tarea worfkflow
        'TIPO_COPIA        : Retorna el tipo copia del expediente
        'RADICADO          : Retorna el radicado relacionado al expediente
        'id_expediente     : Retorna la identificación del expediente
        'nombre_expediente : Retorna el nombre del expediente
        'list_image        : Retorna la lista de imagenes (gabinete, id_imagen)
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2025-04-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Dim CTipoDocEntrante As New CTipoDocEntrante
            Result = Class_tipo_doc_entrante.SolicitaEstructuraTramite(IdTramite,
                                                                       CTipoDocEntrante)
            If Result <> "YES" Then
                CreaExpedienteIntegracionSII = Result
                Exit Function
            End If
            Dim StruImageGabineteWorfkflow As stru_imagen_gabinete_workflow() = Nothing
            '///--------------------Solicita las imagenes relacionadas al flujo de trabajo---------------////
            Result = ClassDaGabinete.SolicitaListaImagenesGabineteEnlace(CTipoDocEntrante.nombre_gabinete_workflow,
                                                                         CIncripcionSII(0).RADICADO_SII,
                                                                         StruImageGabineteWorfkflow)

            If StruImageGabineteWorfkflow Is Nothing Then
                CreaExpedienteIntegracionSII = "No fue posible encontrar documentos para relacionar con el expediente asociado a la tarea (" &
                    IdTareaWorkflow & ")"
                Exit Function
            End If
            Dim ReciboSII As String = CIncripcionSII(0).RADICADO_SII
            Dim CodigoBarrasSII As String = CIncripcionSII(0).COD_BARRA_SII
            If ReciboSII = "" Then
                CreaExpedienteIntegracionSII = "El consecutivo de recibo SII relacionado con la tarea de workflow se encuentra vacío."
                Exit Function
            End If
            If CodigoBarrasSII = "" Then
                CreaExpedienteIntegracionSII = "El consecutivo de codigo de barras del SII relacionado con la tarea de workflow se encuentra vacío."
                Exit Function
            End If
            ClassExpedienteVincula.id_flujo = IdTareaWorkflow
            ClassExpedienteVincula.radicado = CodigoBarrasSII
            ClassExpedienteVincula.gabinete = CTipoDocEntrante.nombre_gabinete_workflow
            Dim ra_dig_tipos_docum_lista_chequeo As New ra_dig_tipos_docum_lista_chequeo
            Dim StruTiposExpedienteSegundarioSII() As StruTiposExpedienteSegundarioSII = Nothing
            Dim ClassConsultaExpedienteSII As New ClassConsultaExpedienteSII
            Dim StruSiiCahcheInscripcion As StruSiiCahcheInscripcion = Nothing
            Dim ClassRaSIiCacheExpediente As New ClassRaSIiCacheExpediente
            Dim Matricula As String = ""
            Dim Proponente As String = ""
            ClassExpedienteVincula.ClsssStructureVinculaDocumento = New List(Of ClsssStructureVinculaDocumento)
            '//----------------Caso transaccion que afecta más de un expediente-------//
            If CTipoDocEntrante.util_Estado_Multiple_expedienteSII = 1 Then
                '//--------Solicita tipologias de expedientes segundarios------//
                Result = ra_dig_tipos_docum_lista_chequeo.SolicitaListaTiposExpedienteSegundarioSii(IdTramite,
                                                                                                    StruTiposExpedienteSegundarioSII)
                If Result <> "YES" Then
                    CreaExpedienteIntegracionSII = Result
                    Exit Function
                End If
                '//----Busca las inscripciones para crear el expediente primario y el segundario---/////
                For i As Integer = 0 To CIncripcionSII.Count - 1
                    Matricula = CIncripcionSII(i).MATRICULA_SII
                    Proponente = CIncripcionSII(i).PROPONENTE_SII
                    Result = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(Matricula,
                                                                                        Proponente,
                                                                                        CTipoDocEntrante.nombre_gabinete_workflow,
                                                                                        StruSiiCahcheInscripcion)
                    If Result <> "YES" Then
                        CreaExpedienteIntegracionSII = Result
                        Exit Function
                    End If
                    '//----------Crea expediente primario----------////
                    If StruSiiCahcheInscripcion.MatriculaPropietario = "" Then
                        Result = ClassGaExpediente.AutoRegistraExpedienteTramite(IdTramite,
                                                                                 CIncripcionSII(i),
                                                                                 IdTareaWorkflow,
                                                                                 0,
                                                                                 ClassExpedienteVincula.id_expediente,
                                                                                 ClassExpedienteVincula.nombre_expediente)
                        If Result <> "YES" Then
                            CreaExpedienteIntegracionSII = Result
                            Exit Function
                        End If
                        '//-----------------Registra cache expediente primario---------///
                        Dim CStruSiiCahcheExpediente As New CStruSiiCahcheExpediente
                        CStruSiiCahcheExpediente.CodBarras = CodigoBarrasSII
                        CStruSiiCahcheExpediente.RadicadoSII = ReciboSII
                        CStruSiiCahcheExpediente.Matricula = StruSiiCahcheInscripcion.Matricula
                        CStruSiiCahcheExpediente.NitIdentificacion = StruSiiCahcheInscripcion.NitIdentificacion
                        CStruSiiCahcheExpediente.Rsocial = StruSiiCahcheInscripcion.Rsocial
                        CStruSiiCahcheExpediente.IdExpediente = ClassExpedienteVincula.id_expediente
                        CStruSiiCahcheExpediente.NombreGabinete = CTipoDocEntrante.nombre_gabinete_workflow
                        ClassExpedienteVincula.Matricula = StruSiiCahcheInscripcion.Matricula
                        CStruSiiCahcheExpediente.EstadoPadre = 1
                        Result = ClassRaSIiCacheExpediente.RegistraCacheCreacionExpedienteSII(CStruSiiCahcheExpediente, 0)
                        If Result <> "YES" Then
                            CreaExpedienteIntegracionSII = Result
                            Exit Function
                        End If
                        '//-------------Agrega la estructura de documentos a vincular del expediente primario---------////
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
                                    ClsssStructureVinculaDocumento.Gabinete = CTipoDocEntrante.nombre_gabinete_workflow
                                    ClsssStructureVinculaDocumento.IdExpedienteWeb = CStruSiiCahcheExpediente.IdExpediente
                                    ClsssStructureVinculaDocumento.IdImagen = StruImageGabineteWorfkflow(z).id_image
                                    ClsssStructureVinculaDocumento.IdFlujoTarea = ClassExpedienteVincula.id_flujo
                                    ClsssStructureVinculaDocumento.Radicado = CStruSiiCahcheExpediente.CodBarras
                                    ClassExpedienteVincula.ClsssStructureVinculaDocumento.Add(ClsssStructureVinculaDocumento)
                                End If
                            Next
                        Else
                            For z As Integer = 0 To StruImageGabineteWorfkflow.Length - 1
                                Dim ClsssStructureVinculaDocumento As New ClsssStructureVinculaDocumento
                                ClsssStructureVinculaDocumento.Gabinete = CTipoDocEntrante.nombre_gabinete_workflow
                                ClsssStructureVinculaDocumento.IdExpedienteWeb = CStruSiiCahcheExpediente.IdExpediente
                                ClsssStructureVinculaDocumento.IdImagen = StruImageGabineteWorfkflow(z).id_image
                                ClsssStructureVinculaDocumento.IdFlujoTarea = ClassExpedienteVincula.id_flujo
                                ClsssStructureVinculaDocumento.Radicado = CStruSiiCahcheExpediente.CodBarras
                                ClassExpedienteVincula.ClsssStructureVinculaDocumento.Add(ClsssStructureVinculaDocumento)
                            Next
                        End If
                    End If
                    '//----------Crea expediente segundario----------////
                    If StruSiiCahcheInscripcion.MatriculaPropietario <> "" Then
                        Dim idExpediente As Integer = 0
                        Result = ClassGaExpediente.AutoRegistraExpedienteTramite(IdTramite,
                                                                                CIncripcionSII(i),
                                                                                IdTareaWorkflow,
                                                                                0,
                                                                                idExpediente,
                                                                                "")
                        If Result <> "YES" Then
                            CreaExpedienteIntegracionSII = Result
                            Exit Function
                        End If
                        '//-----------------Registra cache expediente segundario---------///
                        Dim CStruSiiCahcheExpediente As New CStruSiiCahcheExpediente
                        CStruSiiCahcheExpediente.CodBarras = CodigoBarrasSII
                        CStruSiiCahcheExpediente.RadicadoSII = ReciboSII
                        CStruSiiCahcheExpediente.Matricula = StruSiiCahcheInscripcion.Matricula
                        CStruSiiCahcheExpediente.NitIdentificacion = StruSiiCahcheInscripcion.NitIdentificacion
                        CStruSiiCahcheExpediente.Rsocial = StruSiiCahcheInscripcion.Rsocial
                        CStruSiiCahcheExpediente.IdExpediente = idExpediente
                        CStruSiiCahcheExpediente.EstadoPadre = 2
                        CStruSiiCahcheExpediente.NombreGabinete = CTipoDocEntrante.nombre_gabinete_workflow
                        Result = ClassRaSIiCacheExpediente.RegistraCacheCreacionExpedienteSII(CStruSiiCahcheExpediente, 0)
                        If Result <> "YES" Then
                            CreaExpedienteIntegracionSII = Result
                            Exit Function
                        End If
                        '//-------------Agrega la estructura de documentos a vincular del expediente segundario---------////
                        If StruTiposExpedienteSegundarioSII IsNot Nothing Then
                            For z As Integer = 0 To StruImageGabineteWorfkflow.Length - 1
                                For k As Integer = 0 To StruTiposExpedienteSegundarioSII.Length - 1
                                    If StruImageGabineteWorfkflow(z).ID_TIPODOCUMENTO = StruTiposExpedienteSegundarioSII(k).IdTipo Then
                                        Dim ClsssStructureVinculaDocumento As New ClsssStructureVinculaDocumento
                                        ClsssStructureVinculaDocumento.Gabinete = CTipoDocEntrante.nombre_gabinete_workflow
                                        ClsssStructureVinculaDocumento.IdExpedienteWeb = CStruSiiCahcheExpediente.IdExpediente
                                        ClsssStructureVinculaDocumento.IdImagen = StruImageGabineteWorfkflow(z).id_image
                                        ClsssStructureVinculaDocumento.IdFlujoTarea = ClassExpedienteVincula.id_flujo
                                        ClsssStructureVinculaDocumento.Radicado = CStruSiiCahcheExpediente.CodBarras
                                        ClassExpedienteVincula.ClsssStructureVinculaDocumento.Add(ClsssStructureVinculaDocumento)
                                    End If
                                Next
                            Next
                        End If
                    End If
                Next
            End If
            '//----------------Caso transaccion que afecta un solo expediente-------///
            If CTipoDocEntrante.util_Estado_Multiple_expedienteSII <> 1 Then
                Matricula = CIncripcionSII(0).MATRICULA_SII
                Proponente = CIncripcionSII(0).PROPONENTE_SII
                Result = ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(Matricula,
                                                                                    Proponente,
                                                                                    CTipoDocEntrante.nombre_gabinete_workflow,
                                                                                    StruSiiCahcheInscripcion)
                If Result <> "YES" Then
                    CreaExpedienteIntegracionSII = Result
                    Exit Function
                End If
                Result = ClassGaExpediente.AutoRegistraExpedienteTramite(IdTramite,
                                                                         CIncripcionSII(0),
                                                                         HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                         0,
                                                                         ClassExpedienteVincula.id_expediente,
                                                                         ClassExpedienteVincula.nombre_expediente)
                If Result <> "YES" Then
                    CreaExpedienteIntegracionSII = Result
                    Exit Function
                End If
                Dim MatriculaSII As String = ""
                Dim NitIdentiicacion As String = ""
                Dim Rsocial As String = ""
                If StruSiiCahcheInscripcion.MatriculaPropietario = "" Then
                    MatriculaSII = StruSiiCahcheInscripcion.Matricula
                    NitIdentiicacion = StruSiiCahcheInscripcion.NitIdentificacion
                    Rsocial = StruSiiCahcheInscripcion.Rsocial
                Else
                    MatriculaSII = StruSiiCahcheInscripcion.MatriculaPropietario
                    NitIdentiicacion = StruSiiCahcheInscripcion.Identificacionpro
                    Rsocial = StruSiiCahcheInscripcion.NombrePropietario
                End If
                '//-----------------Registra cache expediente primario---------///
                Dim CStruSiiCahcheExpediente As New CStruSiiCahcheExpediente
                CStruSiiCahcheExpediente.CodBarras = CodigoBarrasSII
                CStruSiiCahcheExpediente.RadicadoSII = ReciboSII
                CStruSiiCahcheExpediente.Matricula = MatriculaSII
                CStruSiiCahcheExpediente.NitIdentificacion = NitIdentiicacion
                CStruSiiCahcheExpediente.Rsocial = Rsocial
                CStruSiiCahcheExpediente.IdExpediente = ClassExpedienteVincula.id_expediente
                CStruSiiCahcheExpediente.EstadoPadre = 1
                CStruSiiCahcheExpediente.NombreGabinete = CTipoDocEntrante.nombre_gabinete_workflow
                ClassExpedienteVincula.Matricula = MatriculaSII
                Result = ClassRaSIiCacheExpediente.RegistraCacheCreacionExpedienteSII(CStruSiiCahcheExpediente, 0)
                If Result <> "YES" Then
                    CreaExpedienteIntegracionSII = Result
                    Exit Function
                End If
                For z As Integer = 0 To StruImageGabineteWorfkflow.Length - 1
                    Dim ClsssStructureVinculaDocumento As New ClsssStructureVinculaDocumento
                    ClsssStructureVinculaDocumento.Gabinete = CTipoDocEntrante.nombre_gabinete_workflow
                    ClsssStructureVinculaDocumento.IdExpedienteWeb = CStruSiiCahcheExpediente.IdExpediente
                    ClsssStructureVinculaDocumento.IdImagen = StruImageGabineteWorfkflow(z).id_image
                    ClsssStructureVinculaDocumento.IdFlujoTarea = ClassExpedienteVincula.id_flujo
                    ClsssStructureVinculaDocumento.Radicado = CStruSiiCahcheExpediente.CodBarras
                    ClassExpedienteVincula.ClsssStructureVinculaDocumento.Add(ClsssStructureVinculaDocumento)
                Next
            End If
            CreaExpedienteIntegracionSII = "YES"
            Exit Function
        Catch ex As Exception
            CreaExpedienteIntegracionSII = "Inconsistencia general funcion CreaExpedienteIntegracionSII " & ex.Message
        End Try
    End Function

    Function Auto_vincula_documentos_a_expediente_estructura(ByVal id_tarea_selecionada As Long,
                                                             ByRef parameter_gestion As ClassExpedienteVincula) As String
        '---------------------------------------------------------------------------
        'Funcion : Crea expediente autoamticamente y vincula documentos a expediente
        '          y vincula el expediente a una estructurua 
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ID_FLUJO          : Retorna el identificador de la tarea worfkflow
        'TIPO_COPIA        : Retorna el tipo copia del expediente
        'RADICADO          : Retorna el radicado relacionado al expediente
        'id_expediente     : Retorna la identificación del expediente
        'nombre_expediente : Retorna el nombre del expediente
        'list_image        : Retorna la lista de imagenes (gabinete, id_imagen)
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-11-11
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim id_expediente As Integer = 0
            Dim nombre_expediente As String = ""
            Dim classDaGabinete As New ClassDaGabinete
            If id_tarea_selecionada = 0 Then
                Auto_vincula_documentos_a_expediente_estructura = "Usuario sin tarea seleccionada imposible vincular documentos"
                Exit Function
            End If
            Dim stru_paramter_image_final As stru_imagen_gabinete_workflow() = Nothing
            Result = classDaGabinete.SolicitaListaImagensGabineteRelacionTareaWorkflow(id_tarea_selecionada,
                                                                                       stru_paramter_image_final)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente_estructura = Result
                Exit Function
            End If
            If stru_paramter_image_final Is Nothing Then
                Auto_vincula_documentos_a_expediente_estructura = "Imposible encontrar documentos para relacionar al expediente de la tarea (" &
                    id_tarea_selecionada & ")"
                Exit Function
            End If
            HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA") = Nothing
            Dim resultList_ = New List(Of list_imagen_expediente_service)()
            For i As Integer = 0 To stru_paramter_image_final.Length - 1
                Dim parameter_gestion_ As list_imagen_expediente_service = New list_imagen_expediente_service()
                parameter_gestion_.gabinete = stru_paramter_image_final(i).gabinete
                parameter_gestion_.id_imagen = stru_paramter_image_final(i).id_image
                resultList_.Add(parameter_gestion_)
            Next
            parameter_gestion.list_image = resultList_
            parameter_gestion.id_flujo = id_tarea_selecionada
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaCodigoBarrasIdTareaWorflow(id_tarea_selecionada,
                                                                                         HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente_estructura = Result
                Exit Function
            End If
            parameter_gestion.radicado = HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA")
            parameter_gestion.tipo_copia = 2
            Dim nombre_campo_tramite As String = ""
            Dim tramite As String = ""
            Dim id_tipo_doc_entrante As Integer = 0
            Dim class_config_list_ruta As New Class_configuracion_listado_ruta
            Result = class_config_list_ruta.SolicitaNombreCampoTramiteRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                           nombre_campo_tramite)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente_estructura = Result
                Exit Function
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaTramiteFlujoWorkflow(id_tarea_selecionada,
                                                                         HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                         nombre_campo_tramite,
                                                                         HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                         tramite,
                                                                         0)

            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente_estructura = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(tramite,
                                                                               id_tipo_doc_entrante)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente_estructura = Result
                Exit Function
            End If
            If id_tipo_doc_entrante = 0 Then
                Auto_vincula_documentos_a_expediente_estructura = "Imposible econtrar el registro del tramite (" & tramite & "), por favor registre el tramite y relacione con el auto registro"
                Exit Function
            End If
            Dim id_nivel_padre_auto_vincula As Integer = 0
            Class_tipo_doc_entrante.Solicita_nivel_padre_vinculacion(id_tipo_doc_entrante,
                                                                     id_nivel_padre_auto_vincula)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente_estructura = Result
                Exit Function
            End If
            Result = ClassGaExpediente.AutoRegistraExpedienteTramite(id_tipo_doc_entrante,
                                                                        HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"),
                                                                        id_tarea_selecionada,
                                                                        id_nivel_padre_auto_vincula,
                                                                        id_expediente,
                                                                        nombre_expediente)
            If Result <> "YES" Then
                Auto_vincula_documentos_a_expediente_estructura = Result
                Exit Function
            Else
                parameter_gestion.nombre_expediente = nombre_expediente
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = id_expediente
                Auto_vincula_documentos_a_expediente_estructura = Result
                Exit Function
            End If
        Catch ex As Exception
            Auto_vincula_documentos_a_expediente_estructura = "Inconsistencia general funcion Auto_vincula_documentos_a_expediente_estructura " & ex.Message
        End Try
    End Function

    Function Auto_vincula_documentos_seleccionado_a_expediente(ByVal item_list As List(Of class_item_element),
                                                               ByRef parameter_gestion As ClassExpedienteVincula) As String
        '---------------------------------------------------------------------------
        'Funcion : Crea expediente autoamticamente para documentos seleccionados
        '          en el modulo workflow
        '          
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ID_FLUJO          : Retorna el identificador de la tarea worfkflow
        'TIPO_COPIA        : Retorna el tipo copia del expediente
        'RADICADO          : Retorna el radicado relacionado al expediente
        'id_expediente     : Retorna la identificación del expediente
        'nombre_expediente : Retorna el nombre del expediente
        'list_image        : Retorna la lista de imagenes (gabinete, id_imagen)
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-06-11
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ClassGaExpediente As New ClassGaExpediente
            Dim id_expediente As Integer = 0
            Dim nombre_expediente As String = ""
            Dim refclas As New Classselecciotarea
            If HttpContext.Current.Session.Item("RELACIONA_EXPEDIENTE") = 0 Then
                Auto_vincula_documentos_seleccionado_a_expediente = "Usuario sin permmisos para vincular archivos a expediente"
                Exit Function
            End If
            If HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA") = "0" Then
                Auto_vincula_documentos_seleccionado_a_expediente = "Usuario sin tarea seleccionada imposible vincular documentos"
                Exit Function
            End If
            Dim Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Dim nombre_gabinete As String = ""
            Result = Class_DAT_ADIC_TAR.SolicitaNombreGabineteImagenTareaWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                 nombre_gabinete,
                                                                 0)
            If Result <> "YES" Then
                Auto_vincula_documentos_seleccionado_a_expediente = Result
                Exit Function
            End If
            Dim resultList_ = New List(Of list_imagen_expediente_service)()
            For i As Integer = 0 To item_list.Count - 1
                Dim parameter_gestion_ As list_imagen_expediente_service = New list_imagen_expediente_service()
                parameter_gestion_.gabinete = nombre_gabinete
                parameter_gestion_.id_imagen = item_list(i).id_item
                resultList_.Add(parameter_gestion_)
                HttpContext.Current.Session.Item("WF_MATRI_COPIA_ESTRUCTURA") = Nothing
            Next
            parameter_gestion.list_image = resultList_
            parameter_gestion.id_flujo = HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA")
            parameter_gestion.gabinete = nombre_gabinete
            Dim ref_Class_DAT_ADIC_TAR As New Class_DAT_ADIC_TAR
            Result = ref_Class_DAT_ADIC_TAR.SolicitaCodigoBarrasIdTareaWorflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                                         HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"))
            If Result <> "YES" Then
                Auto_vincula_documentos_seleccionado_a_expediente = Result
                Exit Function
            End If
            parameter_gestion.radicado = HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA")
            parameter_gestion.tipo_copia = 2
            Dim nombre_campo_tramite As String = ""
            Dim tramite As String = ""
            Dim id_tipo_doc_entrante As Integer = 0
            Dim class_config_list_ruta As New Class_configuracion_listado_ruta
            Result = class_config_list_ruta.SolicitaNombreCampoTramiteRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                           nombre_campo_tramite)
            If Result <> "YES" Then
                Auto_vincula_documentos_seleccionado_a_expediente = Result
                Exit Function
            End If
            Result = ref_Class_DAT_ADIC_TAR.SolicitaTramiteFlujoWorkflow(HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                         HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                         nombre_campo_tramite,
                                                                         HttpContext.Current.Session("WF_RUTAWORKFLOW"),
                                                                         tramite,
                                                                         0)

            If Result <> "YES" Then
                Auto_vincula_documentos_seleccionado_a_expediente = Result
                Exit Function
            End If
            Dim Class_tipo_doc_entrante As New Class_tipo_doc_entrante
            Result = Class_tipo_doc_entrante.RetornaIdTipoTramitePorNombreTipo(tramite,
                                                                               id_tipo_doc_entrante)
            If Result <> "YES" Then
                Auto_vincula_documentos_seleccionado_a_expediente = Result
                Exit Function
            End If
            If id_tipo_doc_entrante = 0 Then
                Auto_vincula_documentos_seleccionado_a_expediente = "Imposible econtrar el registro del tramite (" & tramite & "), por favor registre el tramite y relacione con el auto registro"
                Exit Function
            End If
            Dim id_nivel_padre_auto_vincula As Integer = 0
            Class_tipo_doc_entrante.Solicita_nivel_padre_vinculacion(id_tipo_doc_entrante,
                                                                     id_nivel_padre_auto_vincula)
            If Result <> "YES" Then
                Auto_vincula_documentos_seleccionado_a_expediente = Result
                Exit Function
            End If
            Result = ClassGaExpediente.AutoRegistraExpedienteTramite(id_tipo_doc_entrante,
                                                                        HttpContext.Current.Session.Item("WF_RADICADO_COPIA_ESTRUCTURA"),
                                                                        HttpContext.Current.Session.Item("ID_TAREA_SELECCIONDA"),
                                                                        id_nivel_padre_auto_vincula,
                                                                        id_expediente,
                                                                        nombre_expediente)
            If Result <> "YES" Then
                Auto_vincula_documentos_seleccionado_a_expediente = Result
                Exit Function
            Else
                parameter_gestion.nombre_expediente = nombre_expediente
                parameter_gestion.error_gestion = Result
                parameter_gestion.id_expediente = id_expediente
                Auto_vincula_documentos_seleccionado_a_expediente = Result
                Exit Function
            End If
        Catch ex As Exception
            Auto_vincula_documentos_seleccionado_a_expediente = "Inconsistencia general funcion Auto_vincula_documentos_seleccionado_a_expediente " & ex.Message
        End Try
    End Function

End Class
