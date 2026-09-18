Imports System
Imports System.Data
Imports System.IO
Imports System.Xml
Imports MySql.Data.MySqlClient

' Mutadores modernos aislados; reciben autoridad explícita y preservan los originales.
Public NotInheritable Class ModernExpedientMutationBridge
    Public Function RegistrarExpedienteTramiteConContexto(ByVal IdTipoDocEntrante As Integer,
                                           ByVal Parametro As Object,
                                           ByVal IdTareaWorkflow As Long,
                                           ByVal IdNivelPadre As Integer,
                                           ByVal IdUsuarioGestion As Integer,
                                           ByVal IdEmpresaGestion As Integer,
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
                RegistrarExpedienteTramiteConContexto = Result
                Exit Function
            End If
            If IdAutoRegistro = 0 Then
                RegistrarExpedienteTramiteConContexto = "El  tramite (" & IdTipoDocEntrante & ")  no tiene relacionado el auto registro"
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
                RegistrarExpedienteTramiteConContexto = Result
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
                RegistrarExpedienteTramiteConContexto = Result
                Exit Function
            End If
            Dim Class_ra_auto_campo_unico_expediente As New Class_ra_auto_campo_unico_expediente
            Dim stru_campos_expediente() As stru_campos_expediente = Nothing
            '//-----Solicita la estructura de campos del expediente-------//////
            Result = Class_ra_auto_campo_unico_expediente.SolicitaCamposUnicosAutoRegistroExpediente(IdAutoRegistro,
                                                                                                     stru_campos_expediente)
            If Result <> "YES" Then
                RegistrarExpedienteTramiteConContexto = Result
                Exit Function
            End If
            '//-----Asigna los datos de auto registro-----/////
            Result = Class_ra_auto_registro_expediente.SolicitaDatosFuncionAutoRegistro(FuncionServicioDatos,
                                                                                        Parametro,
                                                                                        IdAutoRegistro,
                                                                                        stru_campos_expediente)
            If Result <> "YES" Then
                RegistrarExpedienteTramiteConContexto = Result
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
                    RegistrarExpedienteTramiteConContexto = Result
                    Exit Function
                End If
            End If
            Dim Class_ra_de_fondo_documental As New Class_ra_de_fondo_documental
            If IdFondo <> 0 Then
                Result = Class_ra_de_fondo_documental.Retorna_nombre_fondo_documental(IdFondo,
                                                                                      nombre_fondo)
                If Result <> "YES" Then
                    RegistrarExpedienteTramiteConContexto = Result
                    Exit Function
                End If
            End If
            Dim Class_series_documentales As New Class_series_documentales
            If IdSerie <> 0 Then
                Result = Class_series_documentales.Solicita_nombre_serie_documental(IdSerie,
                                                                                    nombre_serie)
                If Result <> "YES" Then
                    RegistrarExpedienteTramiteConContexto = Result
                    Exit Function
                End If
            End If
            Dim Class_subseries_documentales As New Class_subseries_documentales
            If IdSubSerie <> 0 Then
                Result = Class_subseries_documentales.Retorna_nombre_sub_serie(IdSubSerie,
                                                                               nombre_sub_serie)
                If Result <> "YES" Then
                    RegistrarExpedienteTramiteConContexto = Result
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
                RegistrarExpedienteTramiteConContexto = Result
                Exit Function
            End If
            Dim RefclassGestionInstrumento As New ClassGaGestionInstrumento
            Dim id_organigrama As Integer = 0
            If IdInstrumento <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_id_organigrama_instrumento(IdInstrumento,
                                                                                        id_organigrama)
                If Result <> "YES" Then
                    RegistrarExpedienteTramiteConContexto = Result
                    Exit Function
                End If
            End If
            Dim nombre_organigrama As String = ""
            If id_organigrama <> 0 Then
                Result = RefclassGestionInstrumento.Solicita_nombre_organigrama_por_identidad_organigrama(id_organigrama,
                                                                                                         nombre_organigrama)
                If Result <> "YES" Then
                    RegistrarExpedienteTramiteConContexto = Result
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
                RegistrarExpedienteTramiteConContexto = Result
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
                RegistrarExpedienteTramiteConContexto = Result
                Exit Function
            End If


            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Dim date1al As String = Date.Today
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                RegistrarExpedienteTramiteConContexto = Result
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
                    RegistrarExpedienteTramiteConContexto = "El campo (" & stru_campos_expediente(i).campo_expediente & ") debe ser informado"
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
            Dim Refclas As New ClassGaExpediente
            Dim estado_codigo_unico As Integer = 1
            Dim requiere_unida_conservacion_fisica As Integer = 0
            Dim option_obliga_archivo_unidad As Integer = 0
            Dim id_registro_relacion As Integer = 0
            NombreExpediente = codigo_unico
            If IdExpediente = 0 Then
                Result = Refclas.Registrar_Expediente_Conservacion(IdUsuarioGestion,
                                                                   codigo_unico,
                                                                   estado_codigo_unico,
                                                                   IdEmpresaGestion,
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
                    RegistrarExpedienteTramiteConContexto = Result
                    Exit Function
                Else
                    RegistrarExpedienteTramiteConContexto = Result
                    Exit Function
                End If
            Else
                RegistrarExpedienteTramiteConContexto = "YES"
                Exit Function
            End If
        Catch ex As Exception
            RegistrarExpedienteTramiteConContexto = "Inconsistencia general funcion Auto_registra_expediente_tramite" & ex.Message
        End Try
    End Function

    Public Function VinculaDocumentoExpedienteConContexto(ByVal id_expediente As Integer,
                                        ByVal id_imagen As Integer,
                                        ByVal gabinete As String,
                                        ByVal radicado As String,
                                        ByVal id_tarea_wf As Long,
                                        ByRef valor_campo As String,
                                        ByRef nombre_expediente_relacion As String,
                                        ByVal NombreRutaWorkflow As String,
                                        ByVal IdUsuarioGestion As Integer,
                                        ByVal IdEmpresaGestion As Integer,
                                        ByVal IdUsuarioWorkflow As Integer,
                                        ByVal IdRutaWorkflow As Integer) As String
        Try
            Dim legacy As New ClassGaExpediente()
            Dim Result As String = ""
            Dim expediente_conservacion() As expediente_conservacion = Nothing
            Result = legacy.SolicitaDatosEstructuraExpediente(id_expediente,
                                                                        expediente_conservacion)
            If Result <> "YES" Then
                VinculaDocumentoExpedienteConContexto = Result
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
                VinculaDocumentoExpedienteConContexto = Result
                Exit Function
            End If
            '-----------------------------------------------------------------------------
            'Verifica estado relación documento a expediente en la producción documental
            '-----------------------------------------------------------------------------
            If id_expediente_relacion <> 0 Then
                VinculaDocumentoExpedienteConContexto = "YES"
                Exit Function
            End If
            Dim Nombre_ruta As String = ""
            Dim Refclas_workflow As New ClassWorkflow
            Dim Ref_class_ruta As New Class_worflow_rutas
            Nombre_ruta = NombreRutaWorkflow
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
                VinculaDocumentoExpedienteConContexto = Result
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
                    VinculaDocumentoExpedienteConContexto = Result
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
                        VinculaDocumentoExpedienteConContexto = Result
                        Exit Function
                    End If
                End If
                '-------------------------------------------
                'Valida que el expediente vinculante no sea
                'diferente al expediente relacionado al 
                'expediente relacionado al radicado
                '-------------------------------------------
                If id_expediente_plantilla_radicado <> 0 And id_expediente_plantilla_radicado <> id_expediente Then
                    VinculaDocumentoExpedienteConContexto = "Esta tratando de vincualar el documento a un expediente diferente al expediente (" &
                        nombre_expediente_plantilla_radicado & ") relacionado en la plantilla de radicacion (" & Nombre_plantilla_radicado & ")"
                    Exit Function
                End If
            End If
            'Solicita archivo indice expediente
            Dim Ruta_archivo_indice_expediente As String = ""
            Result = legacy.Solicita_archivo_indice_expediente(id_expediente,
                                                           Ruta_archivo_indice_expediente)
            If Result <> "YES" Then
                VinculaDocumentoExpedienteConContexto = Result
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
                VinculaDocumentoExpedienteConContexto = Result
                Exit Function
            End If
            Result = Class_sytem1_.VerificaOpcionAplicarInventarioDocumental(Option_unidad_conservacion,
                                                                             gabinete)
            If Result <> "YES" Then
                VinculaDocumentoExpedienteConContexto = Result
                Exit Function
            End If
            Dim date1al As String = Date.Today
            Dim ref_ClassGestionFechas As New ClassGestionFechas
            Result = ref_ClassGestionFechas.FormateaFechaAlmacenamiento(date1al)
            If Result <> "YES" Then
                VinculaDocumentoExpedienteConContexto = "Error formateando fecha almacenamiento Funcion: Formatea_Fecha_Almacenamiento " & Result
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
                VinculaDocumentoExpedienteConContexto = Result
                Exit Function
            End If
            Dim datos_imagen_gabinete As String = ""
            If estado_existencia_produccion = "NO" Then
                Result = ref_ClassDaGabinete.Solicita_datos_imagen_gabinete(gabinete,
                                                                            id_imagen,
                                                                            datos_imagen_gabinete)
                If Result <> "YES" Then
                    VinculaDocumentoExpedienteConContexto = Result
                    Exit Function
                End If
            End If
            Dim extenssion As String = ""
            Dim ClassDaExtension As New Class_da_extension
            Result = ClassDaExtension.SolicitaExtensionArchivoGabineteTipoImagen(stru_paramter_image.DBT_TIPO_IMAGEN,
                                                                  extenssion)
            If Result <> "YES" Then
                VinculaDocumentoExpedienteConContexto = Result
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
                VinculaDocumentoExpedienteConContexto = Result
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
                VinculaDocumentoExpedienteConContexto = Result
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
                VinculaDocumentoExpedienteConContexto = Result
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
                    datos_insert_inventario = "(" & IdUsuarioGestion & "," & IdUsuarioGestion & ",'" & date1al & "'," &
                    ref_id_area & "," & ref_id_serie & "," & ref_nombre_serie & "," & ref_id_sub_serie & "," & ref_nombre_sub_serie &
                    "," & ref_id_tipo_documento & "," & ref_tipo_documento & ",'" & datos_imagen_gabinete & "'," & id_imagen & "," &
                    estado_archivo & ",'" & gabinete & "'," & pagi & "," & ref_id_expediente & "," & ref_expediente & "," & ref_id_tipo_expediente &
                    "," & ref_id_tipo_unidad_conservacion & "," & ref_id_unidad_conservacion & "," & ref_id_clase_documento & "," &
                    ref_clase_documento & "," & ref_fecha_elaboracion & "," & ref_unidad_conserva & "," & ref_nombre_area & "," & ref_id_tipo_unidad_documental &
                    "," & IdEmpresaGestion & "," & ref_radicado & "," & ref_sugundo_nombre_documento & "," & tipo_archivo_producion & ",'" & tamano & "','" & extenssion & "')"
                    sqlinventario = sqlinventario & datos_insert_inventario
                    '-----------------------------------------------
                    'Registra inventario documental
                    '-----------------------------------------------
                    myCommand.CommandText = sqlinventario
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpedienteConContexto = "Imposible agregar registro de inventario documental  "
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
                        VinculaDocumentoExpedienteConContexto = "Imposible actualizar registro inventario de produccion documental  "
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
                        VinculaDocumentoExpedienteConContexto = "Imposible actualizar la gestion en el gabinete  "
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
                    "(" & id_produccion & "," & id_tarea_wf & "," & IdUsuarioWorkflow & "," &
                    id_imagen & ",'" & gabinete & "'," & -1 & "," & id_expediente & "," & IdRutaWorkflow & ",2)"
                    myCommand.CommandText = sql_insert
                    Switc2 = myCommand.ExecuteNonQuery()
                    If Switc2 = 0 Then
                        VinculaDocumentoExpedienteConContexto = "Imposible registrar relación incoporacion workflow "
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
                        VinculaDocumentoExpedienteConContexto = "Imposible registrar relación del expediente  "
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
                        VinculaDocumentoExpedienteConContexto = "Imposible encontrar la identificación del expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        VinculaDocumentoExpedienteConContexto = "Imposible Encontrar el registro del expediente"
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
                        VinculaDocumentoExpedienteConContexto = "Imposible Actualizar numero de folios del expediente "
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
                        VinculaDocumentoExpedienteConContexto = "Imposible encontrar el expediente. "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    If mySqldatReader.HasRows = False Then
                        VinculaDocumentoExpedienteConContexto = "Imposible encontrar el expediente"
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
                        VinculaDocumentoExpedienteConContexto = "Imposible crear indice documento en el expediente "
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
                        VinculaDocumentoExpedienteConContexto = "Imposible actualizar el orden del indice en el expediente "
                        myTrans.Rollback()
                        myConnection.Close()
                        Exit Function
                    End If
                    Dim xmlArchivo As New XmlDocument
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO.Replace("'", "")
                    stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO = stru_produccion_indice.SEGUNDO_NOMBRE_DOCUMENTO.Replace("'", "")
                    Result = legacy.Actualiza_archivo_xml_indice_expediente(Ruta_archivo_indice_expediente,
                                                                        stru_produccion_indice,
                                                                        xmlArchivo)
                    If Result <> "YES" Then
                        myTrans.Rollback()
                        If Not myConnection Is Nothing Then
                            myConnection.Close()
                        End If
                        VinculaDocumentoExpedienteConContexto = "Error actualizando archivo xml indice " & Result
                        Exit Function
                    Else
                        xmlArchivo.Save(Ruta_archivo_indice_expediente)
                    End If
                End If
                myTrans.Commit()
                myConnection.Close()
                VinculaDocumentoExpedienteConContexto = "YES"
                Exit Function
            Catch e As Exception
                Try

                Catch ex As MySqlException
                    If Not myTrans.Connection Is Nothing Then
                        myTrans.Rollback()
                        myConnection.Close()
                        VinculaDocumentoExpedienteConContexto = "An exception of type " + ex.GetType().ToString() +
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
                VinculaDocumentoExpedienteConContexto = "Error General " & e.Message
                Exit Function
            End Try
        Catch ex As Exception
            VinculaDocumentoExpedienteConContexto = "Inconsistencia general funcion VinculaDocumentoExpedienteConContexto " & ex.Message
        End Try
    End Function

End Class
