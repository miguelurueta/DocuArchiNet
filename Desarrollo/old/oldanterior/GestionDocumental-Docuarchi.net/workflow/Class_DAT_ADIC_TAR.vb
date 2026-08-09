Imports Dynamsoft.DotNet.TWAIN.Barcode

Public Structure structure_table_detalle_flujo
    Dim columname As String
    Dim columtext As String
End Structure
Public Structure structure_datos_tarea_workflow
    Dim ID_DAT As Long
    Dim ID_GABINETE As Integer
    Dim ID_IMAGEN As Integer
    Dim FLUJO_INTERNO_WF As Integer
End Structure
Public Class CDParmeterValoresCamposGabineteDatAdicTar
    Property IdTareaWorkflow As Long
    Property IdRutaWorkflow As Integer
    Property NombreRutaWorkflow As String
    Property Gabinete As String
End Class
Public Class Class_DAT_ADIC_TAR
    Function SolicitaDatosCamposIndiceGabineteFlujoExterno(ByVal CDParmeterValoresCamposGabineteDatAdicTar As CDParmeterValoresCamposGabineteDatAdicTar,
                                                           ByRef Radicado As String,
                                                           ByRef CDcamposAsignaAlmacenamiento As List(Of CDcamposAsignaAlmacenamiento)) As String
        '--------------------------------------------------------------------------------------------------
        'Funcion : Asgina campos y datos de alamacenamiento para indice de gabinete para flujos externos
        '          no identiifcados, el sistema solo solicta el valor del campo radicado desde la tabla
        '          de registro de descripcion de tareas DAT_ADIC_TAR
        '          
        '--------------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '--------------------------------------------------------------------------------------------------
        'CDParmeterValoresCamposGabineteDatAdicTar  : Representa la estructura con los parmetros para estructura
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
            Dim ReciboSII As String = ""
            Dim NombreCampoRadicado As String = ""
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(CDParmeterValoresCamposGabineteDatAdicTar.IdRutaWorkflow,
                                                                                      NombreCampoRadicado)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabineteFlujoExterno = Result
                Exit Function
            End If
            Result = SolicitaRadicadoTareaWorkflow(NombreCampoRadicado,
                                                   CDParmeterValoresCamposGabineteDatAdicTar.NombreRutaWorkflow,
                                                   CDParmeterValoresCamposGabineteDatAdicTar.IdTareaWorkflow,
                                                   Radicado)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabineteFlujoExterno = Result
                Exit Function
            End If
            Dim NombreCampoCodigoBarras As String = ""
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoCodigoBarrasRuta(CDParmeterValoresCamposGabineteDatAdicTar.IdRutaWorkflow,
                                                                                          0,
                                                                                          NombreCampoCodigoBarras)
            If Result <> "YES" Then
                SolicitaDatosCamposIndiceGabineteFlujoExterno = Result
                Exit Function
            End If
            Dim CodigoBarras As String = ""
            If NombreCampoCodigoBarras <> "" Then
                Result = SolicitaCodigoBarrasIdTareaWorflow(CDParmeterValoresCamposGabineteDatAdicTar.IdTareaWorkflow,
                                                            CDParmeterValoresCamposGabineteDatAdicTar.NombreRutaWorkflow,
                                                            NombreCampoCodigoBarras,
                                                            0,
                                                            CodigoBarras)
                If Result <> "YES" Then
                    SolicitaDatosCamposIndiceGabineteFlujoExterno = Result
                    Exit Function
                End If
                Dim IlistCDcamposAsignaAlmacenamiento As New CDcamposAsignaAlmacenamiento
                IlistCDcamposAsignaAlmacenamiento = New CDcamposAsignaAlmacenamiento
                IlistCDcamposAsignaAlmacenamiento.NombreCampoGabinete = "CODBARRAS"
                IlistCDcamposAsignaAlmacenamiento.ValorCampoGabinete = CodigoBarras
                CDcamposAsignaAlmacenamiento.Add(IlistCDcamposAsignaAlmacenamiento)
            End If
            SolicitaDatosCamposIndiceGabineteFlujoExterno = "YES"
        Catch ex As Exception
            SolicitaDatosCamposIndiceGabineteFlujoExterno = "Inconsistencia general funcion SolicitaDatosCamposIndiceGabineteFlujoExterno " & ex.Message
        End Try
    End Function
    Function AcualizaIdImagenTareaWorkflow(ByVal IdTareaworkflow As Long,
                                           ByVal IdGabinteWorkflow As Integer,
                                           ByVal NombreRutaWorkflow As String,
                                           ByVal IdImagenDocuarchi As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza imagen tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaworkflow     : Representa la identificación de la tarea workflow
        'IdGabinteWorkflow   : Representa la identiifcación del gabinete workflow
        'NombreRutaWorkflow  : Representa el nombre de la ruta workflow
        'IdImagenDocuarchi   : Representa la identificación de la imagen del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-30
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Update As String = "Update  dat_adic_tar" & NombreRutaWorkflow & "  set ID_GABINETE=" &
            IdGabinteWorkflow & ", ID_IMAGEN=" & IdImagenDocuarchi & " where   INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaworkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(Update)
            If Result <> "YES" Then
                AcualizaIdImagenTareaWorkflow = "Error función AcualizaIdImagenTareaWorkflow " & Result
                Exit Function
            Else
                AcualizaIdImagenTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            AcualizaIdImagenTareaWorkflow = "Inconsistencia general función Acualiza_id_imagen_relacion_workflow " & ex.Message
        End Try
    End Function
    Function AcualizaIdImagenTareaWorkflowIdRUta(ByVal IdRutaWorkflow As Integer,
                                                 ByVal IdTareaworkflow As Long,
                                                 ByVal IdImagenDocuarchi As Integer,
                                                 ByVal NombreGabinete As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Actualiza imagen tarea workflow con la identiicación de la ruta
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaworkflow     : Representa la identificación de la tarea workflow
        'IdGabinteWorkflow   : Representa la identiifcación del gabinete workflow
        'IdRutaWorkflow      : Representa la identiifcación de la ruta workflow
        'IdImagenDocuarchi   : Representa la identificación de la imagen del gabinete
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-30
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result = Ref_clas_rutas.Retorna_nombre_ruta_por_id_ruta(IdRutaWorkflow.ToString,
                                                                    nombre_ruta)
            If Result <> "YES" Then
                Result = Ref_clas_rutas.Retorna_nombre_ruta_workflow(nombre_ruta)
                If Result <> "YES" Then
                    AcualizaIdImagenTareaWorkflowIdRUta = Result
                    Exit Function
                End If
            End If
            Dim id_gabinete As Integer = 0
            Dim Class_configuracion_gabinete As New Class_configuracion_gabinete
            Result = Class_configuracion_gabinete.SolicitaIdGabineteWorkflowPorNombre(NombreGabinete,
                                                                                      id_gabinete)
            If Result <> "YES" Then
                AcualizaIdImagenTareaWorkflowIdRUta = Result
                Exit Function
            End If
            Dim update As String = "Update  dat_adic_tar" & nombre_ruta & "  set ID_GABINETE=" &
            id_gabinete & ", ID_IMAGEN=" & IdImagenDocuarchi & " where   INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaworkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Result = ref.SELECTION_INSERT_COMMAND(update)
            If Result <> "YES" Then
                AcualizaIdImagenTareaWorkflowIdRUta = "Error función AcualizaIdImagenTareaWorkflowIdRUta " & Result
                Exit Function
            Else
                AcualizaIdImagenTareaWorkflowIdRUta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            AcualizaIdImagenTareaWorkflowIdRUta = "Inconsistencia general función AcualizaIdImagenTareaWorkflowIdRUta " & ex.Message
        End Try
    End Function
    Function SolicitaidimagenrelacionadaTareaworkflowRuta(ByVal NombreRutaWorkflow As String,
                                                          ByVal IdTareaWorkflow As Long,
                                                          ByRef IdImagen As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificacion de la imagen de una ruta relacionada a una tarea
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreRutaWorkflow  : Representa el nombre de la ruta worklow
        'IdTareaWorkflow     : Representa la identiicción de la tarea
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagen            : Retorna la identficacion de la imagen del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Ref_clas_rutas As New Class_worflow_rutas

            Dim sqlconsulta As String = "Select ID_IMAGEN from dat_adic_tar" & NombreRutaWorkflow & " where " &
                " INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & NombreRutaWorkflow)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaidimagenrelacionadaTareaworkflowRuta = "Error función SolicitaidimagenrelacionadaTareaworkflowRuta Consultando en tabla " & "dat_adic_tar" & NombreRutaWorkflow & " " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                IdImagen = 0
                SolicitaidimagenrelacionadaTareaworkflowRuta = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    IdImagen = 0
                Else
                    IdImagen = Datset.Tables(0).Rows(0).Item(0)
                End If
                SolicitaidimagenrelacionadaTareaworkflowRuta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaidimagenrelacionadaTareaworkflowRuta = "Inconsistencia general función SolicitaidimagenrelacionadaTareaworkflowRuta " & ex.Message
        End Try
    End Function
    Function SolicitaIdImagenRelacionadaTareaworkflowIdRuta(ByVal IdRutaWorkflow As Integer,
                                                            ByVal IdTareaworklflow As Long,
                                                            ByRef idImagen As Long) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificacion de la imagen de una ruta relacionada a una tarea por la 
        'id ruta
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdRutaWorkflow      : Representa la identifcación de la ruta
        'IdTareaWorkflow     : Representa la identifcación de la tarea
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagen            : Retorna la identficacion de la imagen del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result = Ref_clas_rutas.Retorna_nombre_ruta_por_id_ruta(IdRutaWorkflow.ToString,
                                                                    nombre_ruta)
            If Result <> "YES" Then
                Result = Ref_clas_rutas.Retorna_nombre_ruta_workflow(nombre_ruta)
                If Result <> "YES" Then
                    SolicitaIdImagenRelacionadaTareaworkflowIdRuta = Result
                    Exit Function
                End If
            End If
            Dim sqlconsulta As String = "Select ID_IMAGEN from dat_adic_tar" & nombre_ruta & " where " &
                " INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaworklflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaIdImagenRelacionadaTareaworkflowIdRuta = "Error función SolicitaIdImagenRelacionadaTareaworkflowIdRuta consultando en tabla " & "dat_adic_tar" & nombre_ruta & " " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                idImagen = 0
                SolicitaIdImagenRelacionadaTareaworkflowIdRuta = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    idImagen = 0
                Else
                    idImagen = Datset.Tables(0).Rows(0).Item(0)
                End If
                SolicitaIdImagenRelacionadaTareaworkflowIdRuta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdImagenRelacionadaTareaworkflowIdRuta = "Inconsistencia general función Solicita_id_imagen_relacionada_workflow " & ex.Message
        End Try
    End Function
    Function SolicitaIdGabineteWorkflowRuta(ByVal NombreRutaWorkflow As String,
                                            ByVal IdTareaWorkflow As Long,
                                            ByRef IdGabineteWorkflow As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificación del gabinete en la ruta workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreRutaWorkflow  : Representa el nombre de la ruta
        'IdTareaWorkflow     : Representa la identiifcación de la tarea
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdGabineteWorkflow  : Retorna la identificación del gabiinete en la ruta workflow
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha actualiza       : 2025-05-29
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select ID_GABINETE from dat_adic_tar" & NombreRutaWorkflow &
                " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & NombreRutaWorkflow)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaIdGabineteWorkflowRuta = "Error función SolicitaIdGabineteWorkflowRuta  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdGabineteWorkflowRuta = "Imposible encontrar la identificación del gabinete de la tarea (" & IdTareaWorkflow & ")  en la ruta dat_adic_tar" & NombreRutaWorkflow
                Exit Function
            Else
                IdGabineteWorkflow = Datset.Tables(0).Rows(0).Item(0)
                SolicitaIdGabineteWorkflowRuta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdGabineteWorkflowRuta = "Inconsistencia general función SolicitaIdGabineteWorkflowRuta " & ex.Message
        End Try
    End Function
    Public Shared Function Solicita_radicado_id_tarea_workflow(ByVal id_tarea As Long,
                                                               ByRef radicado_sii As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el radicado de la tarea
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_Actividad          : Representa el nombre de plantilla del radicado
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'radicado_sii            : Retorna el radicado sii del tramite
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-11-04
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ws_estado_ruta_documentos_sii")
            Dim Sql_consulta As String = ""
            Sql_consulta = "Select CODIGO_BARRAS,AUXILIAR from DAT_ADIC_TARREGISTROPUBLICO  " &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_radicado_id_tarea_workflow = "Función Solicita_radicado_id_tarea_workflow dice : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_radicado_id_tarea_workflow = "Imposible encontrar el registro de la tarea (" & id_tarea & ") en la ruta workflow"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    radicado_sii = ""
                Else
                    radicado_sii = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_radicado_id_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_radicado_id_tarea_workflow = "Inconsistencia general función Solicita_radicado_id_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_imagen_id_tarea_relacionada_flujo_workflow(ByRef existencia As String,
                                                                 ByVal nombre_ruta As String,
                                                                 ByVal campo_radicado As String,
                                                                 ByVal RADICADO As String,
                                                                 ByRef id_imagen As Object,
                                                                 ByRef id_tarea_workflow As Object) As String
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Dim Parametro_Consulta As String = "Select ID_IMAGEN,INICIO_TAREAS_WORKFLOW_ID_TAREA FROM dat_adic_tar" & nombre_ruta & " WHERE " & campo_radicado & "='" & RADICADO & "'"
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_imagen_id_tarea_relacionada_flujo_workflow = "Inconsistencia general función Solicita_imagen_id_tarea_relacionada_flujo_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_imagen = 0
                id_tarea_workflow = 0
                existencia = "NO"
                Solicita_imagen_id_tarea_relacionada_flujo_workflow = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    id_imagen = 0
                Else
                    id_imagen = Datset.Tables(0).Rows(0).Item(0)
                End If
                id_tarea_workflow = Datset.Tables(0).Rows(0).Item(1)
                existencia = "YES"
                Solicita_imagen_id_tarea_relacionada_flujo_workflow = "YES"
            End If

        Catch ex As Exception
            Solicita_imagen_id_tarea_relacionada_flujo_workflow = "Inconsistencia función Solicita_imagen_id_tarea_relacionada_flujo_workflow " & ex.Message
        End Try
    End Function
    Function SolicitaIdFlujoTrabajoIdTareaRutaWorkflow(ByVal NombreRuta As String,
                                                       ByVal IdTareaWorkflow As Long,
                                                       ByRef IdFlujoTrabajo As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identificación del flujo de trabajo en la ruta
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreRuta          : Representa el nombre de la ruta
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdFlujoTrabajo     : Retorna la idnetificación del flujo de trabajo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-09
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select FLUJO_TRABAJO_WF from dat_adic_tar" & NombreRuta &
                                       " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("estados_tarea_workflow")
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaIdFlujoTrabajoIdTareaRutaWorkflow = "Se presentó una inconsistencia en la función SolicitaIdFlujoTrabajoIdTareaRutaWorkflow : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                IdFlujoTrabajo = 0
                SolicitaIdFlujoTrabajoIdTareaRutaWorkflow = "Imposible encontrar el registro de la tarea (" & IdTareaWorkflow & ") en la ruta (" & NombreRuta & ")"
                Exit Function
            Else
                IdFlujoTrabajo = Datset.Tables(0).Rows(0).Item(0)
                SolicitaIdFlujoTrabajoIdTareaRutaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdFlujoTrabajoIdTareaRutaWorkflow = "Inconsistencia general función Solicita_id_flujo_trabajo_id_tarea_ruta_workflow " & ex.Message
        End Try
    End Function
    Function Verifica_relacion_imagen_workflow_null(ByVal id_ruta As Integer,
                                                    ByVal id_tarea As Long,
                                                    ByRef estado_relacion As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result = Ref_clas_rutas.Retorna_nombre_ruta_por_id_ruta(id_ruta.ToString,
                                                                    nombre_ruta)
            If Result <> "YES" Then
                Result = Ref_clas_rutas.Retorna_nombre_ruta_workflow(nombre_ruta)
                If Result <> "YES" Then
                    Verifica_relacion_imagen_workflow_null = Result
                    Exit Function
                End If
            End If
            Dim sqlconsulta As String = "Select ID_IMAGEN from dat_adic_tar" & nombre_ruta & " where  INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Verifica_relacion_imagen_workflow_null = "Error función Verifica_relacion_imagen_workflow_null Consultando en tabla " & "dat_adic_tar" & nombre_ruta & " " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_relacion = "NO"
                Verifica_relacion_imagen_workflow_null = "Imposoble encontrar el id de la tarea (" & id_tarea & ") en la ruta (" & nombre_ruta & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estado_relacion = "NO"
                Else
                    estado_relacion = "YES"
                End If
                Verifica_relacion_imagen_workflow_null = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_relacion_imagen_workflow_null = "Inconsistencia general función Verifica_relacion_imagen_workflow " & ex.Message
        End Try
    End Function
    Function Verifica_relacion_imagen_workflow(ByVal id_imagen As Long,
                                               ByVal id_ruta As Integer,
                                               ByVal id_tarea As Long,
                                               ByRef estado_relacion As String) As String
        Try
            Dim Result As String = ""
            Dim Refclas As New ClassWorkflow
            Dim Ref_clas_rutas As New Class_worflow_rutas
            Dim nombre_ruta As String = ""
            Result = Ref_clas_rutas.Retorna_nombre_ruta_por_id_ruta(id_ruta.ToString,
                                                                    nombre_ruta)
            If Result <> "YES" Then
                Result = Ref_clas_rutas.Retorna_nombre_ruta_workflow(nombre_ruta)
                If Result <> "YES" Then
                    Verifica_relacion_imagen_workflow = Result
                    Exit Function
                End If
            End If
            Dim sqlconsulta As String = "Select ID_DAT from dat_adic_tar" & nombre_ruta & " where ID_IMAGEN=" & id_imagen & " and " &
                " INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Verifica_relacion_imagen_workflow = "Error función Verifica_relacion_imagen_workflow Consultando en tabla " & "dat_adic_tar" & nombre_ruta & " " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_relacion = "NO"
                Verifica_relacion_imagen_workflow = "YES"
                Exit Function
            Else
                estado_relacion = "YES"
                Verifica_relacion_imagen_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Verifica_relacion_imagen_workflow = "Inconsistencia general función Verifica_relacion_imagen_workflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_imagen_gabinete_seleccionada(ByVal RADICADO As String,
                                                      ByVal id_ruta_workflow As Integer,
                                                      ByVal campo_radicado As String,
                                                      ByVal nombre_ruta As String,
                                                      ByRef id_imagen As Integer,
                                                      ByRef nombre_gabinete As String) As String
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = "Select dat.ID_IMAGEN,cg.Nombre_Gabinete From DAT_ADIC_TAR" & nombre_ruta &
              " as dat INNER JOIN configuracion_gabinete AS cg on (dat.ID_GABINETE=cg.ID_GABINETE)" &
              " WHERE dat." & campo_radicado & "='" & RADICADO & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_imagen_gabinete_seleccionada = "Función Solicita_id_imagen_gabinete_seleccionada dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_imagen_gabinete_seleccionada = "Imposible encontrar el nombre del gabiente en la tabla configuración"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_imagen = 0
                Else
                    id_imagen = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    nombre_gabinete = ""
                Else
                    nombre_gabinete = Datset.Tables(0).Rows(0).Item(1)
                End If
                Solicita_id_imagen_gabinete_seleccionada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_imagen_gabinete_seleccionada = "Inconsistencia general función Solicita_id_imagen_gabinete_seleccionada " & ex.Message
        End Try
    End Function
    Function SolicitaidImagenTareaworkflow(ByVal IdTareaWorkflow As Long,
                                           ByVal NombreRutaWorkflow As String,
                                           ByRef IdImagenGabiente As Long) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identiifcación de la imagen relacionada  a la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'NombreRutaWorkflow  : Representa el nombre de la ruta workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdImagenGabiente  : Retorna la identificación de el documento en el gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = "Select ID_IMAGEN From DAT_ADIC_TAR" & NombreRutaWorkflow &
              " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA =" & IdTareaWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaidImagenTareaworkflow = "Función SolicitaidImagenTareaworkflow dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaidImagenTareaworkflow = "Imposible encontrar con ID (" & IdTareaWorkflow & ") en la ruta ( " & NombreRutaWorkflow & ") Verifique que la tarea exista y que la ruta especificada sea correcta."
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    IdImagenGabiente = 0
                Else
                    IdImagenGabiente = Datset.Tables(0).Rows(0).Item(0)
                End If
                SolicitaidImagenTareaworkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaidImagenTareaworkflow = "Inconsistencia general función SolicitaidImagenTareaworkflow " & ex.Message
        End Try
    End Function
    Function SolicitaTramiteFlujoWorkflow(ByVal id_tarea_workflow As Long,
                                          ByVal id_ruta As Integer,
                                          ByVal nombre_campo_tramite As String,
                                          ByVal nombre_ruta As String,
                                          ByRef tramite As String,
                                          ByRef estado_flujo As Integer) As String
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select " & nombre_campo_tramite & ",FLUJO_TRABAJO_WF from dat_adic_tar" & nombre_ruta &
                " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea_workflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaTramiteFlujoWorkflow = "Error Consultando en tabla " & " dat_adic_tar" & nombre_ruta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaTramiteFlujoWorkflow = "Imposible encontrar el tipo tramite de la tarea (" & id_tarea_workflow & ")  en la ruta dat_adic_tar" & nombre_ruta
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) Then
                    tramite = ""
                Else
                    tramite = Datset.Tables(0).Rows(0).Item(0)
                End If
                estado_flujo = Datset.Tables(0).Rows(0).Item(1)
                SolicitaTramiteFlujoWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaTramiteFlujoWorkflow = "Inconsistencia general función SolicitaTramiteFlujoWorkflow " & ex.Message
        End Try
    End Function
    Function Solicita_estado_tramite_tarea_workflow(ByVal nombre_ruta As String,
                                                    ByVal id_tarea As Long,
                                                    ByRef estado_tramite As String) As String
        Try
            Dim Result As String = ""
            Dim sqlconsulta As String = "Select estado_tramite from dat_adic_tar" & nombre_ruta &
                " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_tramite_tarea_workflow = "Error consultando en tabla " & " dat_adic_tar" & nombre_ruta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                estado_tramite = ""
                Solicita_estado_tramite_tarea_workflow = "Imposible encontrar el estado tramite de la tarea (" & id_tarea & ")  en la ruta dat_adic_tar" & nombre_ruta
                Exit Function
            Else
                estado_tramite = Datset.Tables(0).Rows(0).Item(0)
                Solicita_estado_tramite_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_tramite_tarea_workflow = "Inconsistencia general función Solicita_estado_tramite_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Actualiza_estado_tramite_tarea_workflow(ByVal nombre_ruta As String,
                                                     ByVal id_tarea As Long,
                                                     ByVal estado_tramite As String) As String
        Try
            Dim Result As String = ""
            Dim sql_update As String = "Update   dat_adic_tar" & nombre_ruta &
                " set estado_tramite='" & estado_tramite & "'" &
                " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Actualiza_estado_tramite_tarea_workflow = "Error function  Actualiza_estado_tramite_tarea_workflow " & Result
                Exit Function
            Else
                Actualiza_estado_tramite_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_estado_tramite_tarea_workflow = "Incosistencia general función Actualiza_estado_tramite_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Actualiza_codigo_corto_sii_tarea_workflow(ByVal nombre_ruta As String,
                                                       ByVal id_tarea As Long,
                                                       ByVal codigo_corto_sii As String) As String
        Try
            Dim Result As String = ""
            Dim sql_update As String = "Update   dat_adic_tar" & nombre_ruta &
                " set AUXILIAR='" & codigo_corto_sii & "'" &
                " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Result = ref.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                Actualiza_codigo_corto_sii_tarea_workflow = "Error function  Actualiza_codigo_corto_sii_tarea_workflow " & Result
                Exit Function
            Else
                Actualiza_codigo_corto_sii_tarea_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_codigo_corto_sii_tarea_workflow = "Incosistencia general función Actualiza_codigo_corto_sii_tarea_workflow " & ex.Message
        End Try
    End Function
    Function ActualizaIdImagenTareaWorkflow(ByVal NombreRutaWorkflow As String,
                                            ByVal IdTareaWorkflow As Long,
                                            ByVal IdImagenGabinete As Object) As String
        Try
            Dim Result As String = ""
            Dim sql_update As String = "Update   dat_adic_tar" & NombreRutaWorkflow &
                " set ID_IMAGEN=" & IdImagenGabinete &
                " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & NombreRutaWorkflow)
            Result = ref.SELECTION_INSERT_COMMAND(sql_update)
            If Result <> "YES" Then
                ActualizaIdImagenTareaWorkflow = "Error function  ActualizaIdImagenTareaWorkflow " & Result
                Exit Function
            Else
                ActualizaIdImagenTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            ActualizaIdImagenTareaWorkflow = "Incosistencia general función ActualizaIdImagenTareaWorkflow " & ex.Message
        End Try
    End Function
    Function Solicita_id_tarea_radicado(ByVal RADICADO As String,
                                        ByVal nombre_ruta As String,
                                        ByVal campo_radicado As String,
                                        ByRef id_tarea As Long,
                                        ByVal confir As Integer) As String
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = "Select INICIO_TAREAS_WORKFLOW_ID_TAREA  From DAT_ADIC_TAR" & nombre_ruta &
              " WHERE " & campo_radicado & "='" & RADICADO & "'"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_tarea_radicado = "Función Solicita_id_tarea_radicado dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If confir = 0 Then
                    id_tarea = 0
                    Solicita_id_tarea_radicado = "Imposible encontrar el id tarea del radicado (" & RADICADO & ")"
                    Exit Function
                Else
                    id_tarea = 0
                    Solicita_id_tarea_radicado = "YES"
                    Exit Function
                End If
            Else
                id_tarea = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_tarea_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_tarea_radicado = "Incosnsitencia general función Solicita_id_tarea_radicado " & ex.Message
        End Try
    End Function
    Function SolicitaTipoTareaPadreWorkflow(ByVal IdTareaWorkflow As Long,
                                            ByVal NombreRuta As String,
                                            ByRef TipoTareaPadre As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el tipo de tarea workflow si una tarea padre o una tarea hija
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'NombreRuta          : Representa el nombre de la ruta workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'TipoTareaPadre  : Retorna el tipo tarea si es padre o hija de otra tarea
        '                  0- Tarea corriente 1-Tarea padre 2- Tarea hija
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SQLconsulta As String = "Select TipoTareaPadre from DAT_ADIC_TAR" & NombreRuta &
                " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim dat_base As New conect.Dbase_Conction_Mysql
            Dim DataSet As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = dat_base.SELECTION_SELECT_FIELD(SQLconsulta, DataSet)
            If Result <> "YES" Then
                SolicitaTipoTareaPadreWorkflow = "Función SolicitaTipoTareaPadreWorkflow dice " & Result
                Exit Function
            End If
            If DataSet.Tables(0).Rows.Count = 0 Then
                SolicitaTipoTareaPadreWorkflow = "Imposible encontrar la tarea workflow (" & IdTareaWorkflow & ") para determinar el estado de tarea padre."
                Exit Function
            Else
                TipoTareaPadre = DataSet.Tables(0).Rows(0).Item(0)
                SolicitaTipoTareaPadreWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaTipoTareaPadreWorkflow = "Inconsistencia funcion SolicitaTipoTareaPadreWorkflow " & ex.Message
        End Try
    End Function
    Function SolicitaIdTareaRutaRadicado(ByVal NombreRuta As String,
                                         ByVal NombreCampoRadicado As String,
                                         ByVal RadicadoTramite As String,
                                         ByRef IdTareaWorkflow As Long) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita identificación de la tarea relacionada al radicado
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreRuta          : Representa el nombre de la ruta workflow
        'NombreCampoRadicado : Representa el nombre del campo radicado
        'RadicadoTramite     : Representa el consucutivo del radicado
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow  : Retorna la indentificación de la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-05-17
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SQLconsulta As String = "Select INICIO_TAREAS_WORKFLOW_ID_TAREA from DAT_ADIC_TAR" & NombreRuta &
                " where " & NombreCampoRadicado & "='" & RadicadoTramite & "'"
            Dim dat_base As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = dat_base.SELECTION_SELECT_FIELD(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaIdTareaRutaRadicado = "Función SolicitaIdTareaRutaRadicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdTareaRutaRadicado = "Imposible encontrar la tarea workflow  relacionada al radicado (" & RadicadoTramite & ")."
                Exit Function
            End If
            If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                IdTareaWorkflow = 0
            Else
                IdTareaWorkflow = Datset.Tables(0).Rows(0).Item(0)
            End If
            SolicitaIdTareaRutaRadicado = "YES"
        Catch ex As Exception
            SolicitaIdTareaRutaRadicado = "Inconsistencia general funcion SolicitaIdTareaRutaRadicado " & ex.Message
        End Try
    End Function
    Function SolicitaRadicadoTareaWorkflow(ByVal NombreCampoRadicado As String,
                                           ByVal NombreRutaWorkflow As String,
                                           ByVal IdTareaWorkflow As Integer,
                                           ByRef RadicadoTarea As String) As String
        Try
            Dim Result As String = ""
            Dim SqlConsulta As String = "Select " & NombreCampoRadicado & " From DAT_ADIC_TAR" & NombreRutaWorkflow &
             " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim dat_base As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = dat_base.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                SolicitaRadicadoTareaWorkflow = "Función SolicitaRadicadoTareaWorkflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                RadicadoTarea = ""
                SolicitaRadicadoTareaWorkflow = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    RadicadoTarea = ""
                Else
                    RadicadoTarea = Datset.Tables(0).Rows(0).Item(0)
                End If
                SolicitaRadicadoTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaRadicadoTareaWorkflow = "Incosistencia general funcion SolicitaRadicadoTareaWorkflow " & ex.Message
        End Try
    End Function
    Function Solicita_radicado_id_tarea_seleccionada(ByVal id_tarea_seleccionada As Long,
                                                     ByRef Radicado As String) As String
        '**************************************************
        'Funcion : Solicita_radicado_id_tarea_seleccionada
        'con el paramentro id tarea
        'Fecha : 2015-03-07
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************
        Try
            Dim Result As String = ""
            Dim nombre_ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     nombre_ruta)
            If Result <> "YES" Then
                Solicita_radicado_id_tarea_seleccionada = "Error listando Ruta " + Result
                Exit Function
            End If
            If nombre_ruta = "" Then
                Solicita_radicado_id_tarea_seleccionada = "Imposible Econtrar Nombre de la ruta " + Result
                Exit Function
            End If
            Dim nombre_campo_radicado As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Result = Ref_class_config_listado.SolicitaNombreCampoRadicadoRuta(HttpContext.Current.Session("Id_Ruta_Workflow"),
                                                                              nombre_campo_radicado)
            If Result <> "YES" Then
                Solicita_radicado_id_tarea_seleccionada = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "Select " & nombre_campo_radicado & " From DAT_ADIC_TAR" & nombre_ruta &
                " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea_seleccionada
            Dim dat_base As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = dat_base.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_radicado_id_tarea_seleccionada = "Función Solicita_radicado_id_tarea_seleccionada dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_radicado_id_tarea_seleccionada = "Función Solicita_radicado_id_tarea_seleccionada dice Imposible encontrar radicado del id tarea " & id_tarea_seleccionada
                Exit Function
            End If
            If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                Radicado = ""
            Else
                Radicado = Datset.Tables(0).Rows(0).Item(0)
            End If
            Solicita_radicado_id_tarea_seleccionada = "YES"
        Catch ex As Exception
            Solicita_radicado_id_tarea_seleccionada = "Inconsistencia función Solicita_radicado_id_tarea_seleccionada " & ex.Message
        End Try
    End Function
    Function Solicita_radicado_id_tarea_seleccionada(ByVal id_tarea_seleccionada As Integer,
                                                     ByVal nombre_campo_radicado As String,
                                                     ByVal nombre_ruta As String,
                                                     ByRef Radicado As String) As String
        '**************************************************
        'Funcion : Solicita_radicado_id_tarea_seleccionada
        'con el paramentro id tarea
        'Fecha : 2015-03-07
        'Ing :Miguel Angel Urueta Miranda
        '*************************************************
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = "Select " & nombre_campo_radicado & " From DAT_ADIC_TAR" & nombre_ruta &
                " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea_seleccionada
            Dim dat_base As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = dat_base.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_radicado_id_tarea_seleccionada = "Función Solicita_radicado_id_tarea_seleccionada dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_radicado_id_tarea_seleccionada = "Función Solicita_radicado_id_tarea_seleccionada dice Imposible encontrar radicado del id tarea " & id_tarea_seleccionada
                Exit Function
            End If
            If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                Radicado = ""
            Else
                Radicado = Datset.Tables(0).Rows(0).Item(0)
            End If
            Solicita_radicado_id_tarea_seleccionada = "YES"
        Catch ex As Exception
            Solicita_radicado_id_tarea_seleccionada = "Inconsistencia función Solicita_radicado_id_tarea_seleccionada " & ex.Message
        End Try
    End Function

    Function SolicitaReciboCodigoBarrasSII(ByVal IdTareaWorkflow As Long,
                                           ByVal NombreRuta As String,
                                           ByVal IdRutaWorkflow As Integer,
                                           ByRef ReciboSII As String,
                                           ByRef CodigoBarrasSII As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el consecutivo de radicado SII y el codigo de barras SII
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workflow
        'NombreRuta          : Representa el nombre de la ruta de la tarea workflow
        'IdRutaWorkflow      : Reprenta la identificación de la ruta workflow
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'ReciboSII           : Retorna el consecutivo radicado SII
        'CodigoBarrasSII     : Retorna el consecutivo de codigo de barras SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-21
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim NombreCampoRadicado As String = ""
            Dim NombreCampoCodigoBarras As String = ""
            Dim Class_configuracion_listado_ruta As New Class_configuracion_listado_ruta
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoRadicadoRuta(IdRutaWorkflow,
                                                                                      NombreCampoRadicado)
            If Result <> "YES" Then
                SolicitaReciboCodigoBarrasSII = Result
                Exit Function
            End If
            Result = Class_configuracion_listado_ruta.SolicitaNombreCampoCodigoBarrasRuta(IdRutaWorkflow,
                                                                                          1,
                                                                                          NombreCampoCodigoBarras)
            If Result <> "YES" Then
                SolicitaReciboCodigoBarrasSII = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "Select " & NombreCampoCodigoBarras & " , " & NombreCampoRadicado & " From DAT_ADIC_TAR" & NombreRuta &
               " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim DataBase As New conect.Dbase_Conction_Mysql
            Dim DataSet As DataSet = New DataSet("DAT_ADIC_TAR")
            Result = DataBase.SELECTION_SELECT_FIELD(Sql_consulta, DataSet)
            If Result <> "YES" Then
                SolicitaReciboCodigoBarrasSII = "Función SolicitaReciboCodigoBarrasSII dice " & Result
                Exit Function
            End If
            If DataSet.Tables(0).Rows.Count = 0 Then
                SolicitaReciboCodigoBarrasSII = "No fue posible encontrar el consecutivo, el código de barras ni el consecutivo de radicado SII asociado a la identificación de la tarea (" & IdTareaWorkflow & ")"
                Exit Function
            Else
                If DataSet.Tables(0).Rows(0).IsNull(0) = True Then
                    CodigoBarrasSII = ""
                Else
                    CodigoBarrasSII = DataSet.Tables(0).Rows(0).Item(0)
                End If
                If DataSet.Tables(0).Rows(0).IsNull(1) = True Then
                    ReciboSII = ""
                Else
                    ReciboSII = DataSet.Tables(0).Rows(0).Item(1)
                End If
                SolicitaReciboCodigoBarrasSII = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaReciboCodigoBarrasSII = "Inconsistencia general funcion SolicitaReciboCodigoBarrasSII " & ex.Message
        End Try

    End Function
    Function SolicitaCodigoBarrasIdTareaWorflow(ByVal IdTareaWorkflow As Long,
                                                ByVal NombreRuta As String,
                                                ByVal NombreCampoCodigoBarras As String,
                                                ByVal ValidaExistencia As Integer,
                                                ByRef CodigoBarras As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita codigo de barras relacionado a la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorflow      : Representa la identificación del script de validación
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CodigoBarras  : Retorna el código de barras
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 :  2015-03-07
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Sql_consulta As String = "Select " & NombreCampoCodigoBarras & " From DAT_ADIC_TAR" & NombreRuta &
                " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim dat_base As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = dat_base.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaCodigoBarrasIdTareaWorflow = "Función SolicitaCodigoBarrasIdTareaWorflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If ValidaExistencia = 1 Then
                    SolicitaCodigoBarrasIdTareaWorflow = "No fue posible localizar el código de barras asociado a la tarea de workflow (" & IdTareaWorkflow & ")."
                    Exit Function
                Else
                    CodigoBarras = ""
                    SolicitaCodigoBarrasIdTareaWorflow = "YES"
                    Exit Function
                End If
            End If
            If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                CodigoBarras = ""
            Else
                CodigoBarras = Datset.Tables(0).Rows(0).Item(0)
            End If
            SolicitaCodigoBarrasIdTareaWorflow = "YES"
        Catch ex As Exception
            SolicitaCodigoBarrasIdTareaWorflow = "Inconsistencia función SolicitaCodigoBarrasIdTareaWorflow " & ex.Message
        End Try
    End Function
    Function SolicitaCodigoBarrasIdTareaWorflow(ByVal IdTareaWorkflow As Long,
                                                ByRef CodigoBarras As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita codigo de barras relacionado a la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorflow      : Representa la identificación del script de validación
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CodigoBarras  : Retorna el código de barras
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 :  2015-03-07
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim NombreRuta As String = ""
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Result = Class_worflow_rutas.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                       NombreRuta)
            If Result <> "YES" Then
                SolicitaCodigoBarrasIdTareaWorflow = "Error listando Ruta " + Result
                Exit Function
            End If
            If NombreRuta = "" Then
                SolicitaCodigoBarrasIdTareaWorflow = "Imposible Econtrar Nombre de la ruta " + Result
                Exit Function
            End If
            Dim NombreCampoCodigoBarras As String = ""
            Dim Ref_class_config_listado As New Class_configuracion_listado_ruta
            Result = Ref_class_config_listado.SolicitaNombreCampoCodigoBarrasRuta(HttpContext.Current.Session("Id_Ruta_Workflow"), 1,
                                                                                  NombreCampoCodigoBarras)
            If Result <> "YES" Then
                SolicitaCodigoBarrasIdTareaWorflow = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "Select " & NombreCampoCodigoBarras & " From DAT_ADIC_TAR" & NombreRuta &
                " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim dat_base As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = dat_base.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaCodigoBarrasIdTareaWorflow = "Función SolicitaCodigoBarrasIdTareaWorflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaCodigoBarrasIdTareaWorflow = "No fue posible localizar el código de barras asociado a la tarea de workflow (" & IdTareaWorkflow & ")."
                Exit Function
            End If
            If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                CodigoBarras = ""
            Else
                CodigoBarras = Datset.Tables(0).Rows(0).Item(0)
            End If
            SolicitaCodigoBarrasIdTareaWorflow = "YES"
        Catch ex As Exception
            SolicitaCodigoBarrasIdTareaWorflow = "Inconsistencia función SolicitaCodigoBarrasIdTareaWorflow " & ex.Message
        End Try
    End Function
    Function SolicitaBeneficiarioTareaWorkflow(ByVal NombreRuta As String,
                                               ByVal NombreCampoBenficiario As String,
                                               ByVal IdTarea As Long,
                                               ByRef ValorCampoBenficiario As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el valor del campo beneficiario de una tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreRuta                : Representa el nombre de la ruta
        'NombreCampoBenficiario    : Representa el nombre del campo beneficiario
        'IdTarea                   : Representa la identificación de la tarea
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'ValorCampoBenficiario     : Retorna el valor del campo beneficiario
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                     : 2025-04-01
        'Elabora                   : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim SqlConsulta As String = "Select " & NombreCampoBenficiario & " from dat_adic_tar" & NombreRuta &
               " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & NombreRuta)
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                SolicitaBeneficiarioTareaWorkflow = "Error consultando en tabla " & " dat_adic_tar" & NombreRuta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                ValorCampoBenficiario = ""
                SolicitaBeneficiarioTareaWorkflow = "Imposible encontrar el nombre benficiario de la tarea (" & IdTarea & ")  en la ruta dat_adic_tar" & NombreRuta
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    ValorCampoBenficiario = ""
                Else
                    ValorCampoBenficiario = Datset.Tables(0).Rows(0).Item(0)
                End If
                SolicitaBeneficiarioTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaBeneficiarioTareaWorkflow = "Inconsistencia general función SolicitaBeneficiarioTareaWorkflow " & ex.Message
        End Try
    End Function
    Function SolicitaFlujoTareaWorkflow(ByVal NombreRutaworkflow As String,
                                        ByVal IdTareaworkflow As Long,
                                        ByRef IdFlujoTarea As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identiicación del flujo de una tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreRutaworkflow  : Representa el nombre de la ruta workflow
        'IdTareaworkflow     : Representa la identificación de la tarea workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdFlujoTarea  : Retorna la idnetificación flujo de la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim sqlconsulta As String = "Select FLUJO_TRABAJO_WF from dat_adic_tar" & NombreRutaworkflow &
              " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaworkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & NombreRutaworkflow)
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                SolicitaFlujoTareaWorkflow = "Error función SolicitaFlujoTareaWorkflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                IdFlujoTarea = 0
                SolicitaFlujoTareaWorkflow = "Imposible encontrar la identificación del flujo de la tarea (" & IdTareaworkflow & ") "
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    IdFlujoTarea = 0
                Else
                    IdFlujoTarea = Datset.Tables(0).Rows(0).Item(0)
                End If
                SolicitaFlujoTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaFlujoTareaWorkflow = "Inconsistencia general función Solicita_flujo_tarea_workflow " & ex.Message
        End Try
    End Function
    Function SolicitaIdTipoFlujoTareaWorkflow(ByVal IdTareaWorkflow As Long,
                                              ByVal NombreRuta As String,
                                              ByRef IdTipoFlujoTarea As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion :  Retorna tipo flujo de trabajo interno o externo
        '           argumentos id tarea workflow, nombre ruta y id de la ruta
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea
        'NombreRuta          : Representa el nombre de la ruta
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdTipoFlujoTarea  : Retorna el tipo flujo tarea valores 1-Interno  2-Integración
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2018-03-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "Select FLUJO_INTERNO_WF from dat_adic_tar" & NombreRuta & " where " &
            "  INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet(NombreRuta)
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaIdTipoFlujoTareaWorkflow = "Función SolicitaIdTipoFlujoTareaWorkflow dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdTipoFlujoTareaWorkflow = "Imposible encontrar el tipo de tarea relacionada al código de la tarea (" & IdTareaWorkflow & ") de " &
                    " la ruta (" & NombreRuta & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    IdTipoFlujoTarea = 0
                Else
                    IdTipoFlujoTarea = Datset.Tables(0).Rows(0).Item(0)
                End If
                SolicitaIdTipoFlujoTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaIdTipoFlujoTareaWorkflow = "Inconsistencia general función SolicitaIdTipoFlujoTareaWorkflow " & ex.Message
        End Try
    End Function
    Function Solicita_valor_campo_dinamico_ruta(ByVal nombre_ruta As String,
                                                ByVal nombre_campo As String,
                                                ByVal id_tarea As Long,
                                                ByRef valor_valor_campo As String) As String
        Try
            Dim sqlconsulta As String = "Select " & nombre_campo & " from dat_adic_tar" & nombre_ruta &
               " where INICIO_TAREAS_WORKFLOW_ID_TAREA=" & id_tarea
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("dat_adic_tar" & nombre_ruta)
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sqlconsulta, Datset)
            If Result <> "YES" Then
                Solicita_valor_campo_dinamico_ruta = "Error Solicita_valor_campo_dinamico_ruta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                valor_valor_campo = ""
                Solicita_valor_campo_dinamico_ruta = "Imposible encontrar el " & nombre_campo & " de la tarea (" & id_tarea & ")  en la ruta dat_adic_tar" & nombre_ruta
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    valor_valor_campo = ""
                Else
                    valor_valor_campo = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_valor_campo_dinamico_ruta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_valor_campo_dinamico_ruta = "Inconsistencia general función Solicita_valor_campo_dinamico_ruta " & ex.Message
        End Try
    End Function
    Function SolicitaTipoFujoExternoInterno(ByVal IdTareaWorkflow As Long,
                                            ByRef EstadoFlujoDocumental As Integer,
                                            ByVal NombreRuta As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Retrona tipo de tarea interna o externa 
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow          : Representa la identificación de la tarea frente a workflow
        'EstadoFlujoDocumental    : Representa el nombre del campo de radicación destino
        'NombreRuta               : Representa el nombre de la ruta general
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstadoFlujoDocumental  : Retorna estado flujo 1- interno -2 Externo
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2017-04-19
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "select  FLUJO_INTERNO_WF  from dat_adic_tar" & NombreRuta & "  where INICIO_TAREAS_WORKFLOW_ID_TAREA =" & IdTareaWorkflow
            Dim Numero_Imagenesl As Integer = 0
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("nombre_ruta")
            Dim result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If result <> "YES" Then
                SolicitaTipoFujoExternoInterno = "Error Consultando en tabla " & " dat_adic_tar" & NombreRuta & Sql_consulta
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaTipoFujoExternoInterno = "Imposible encontrar la tarea (" & IdTareaWorkflow & ") en la ruta " & "(" & NombreRuta & ") para determinar si pertenece a un radicado interno o externo"
                Exit Function
            Else
                EstadoFlujoDocumental = Datset.Tables(0).Rows(0).Item(0)
                SolicitaTipoFujoExternoInterno = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaTipoFujoExternoInterno = "Inconsistencia general función SolicitaTipoFujoExternoInterno " & ex.Message
        End Try
    End Function
    Function Listar_datos_tarea_workflow(ByRef structure_colum() As structure_table_detalle_flujo) As String
        Try
            structure_colum = Nothing
            Dim ref As New ClassListandoTareas
            Dim Result As String = ""
            Dim I2 As Integer = 0
            Dim Sql_consulta As String = "Select * From DAT_ADIC_TAR" & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & HttpContext.Current.Session("ID_TAREA_SELECCIONDA")
            Dim ref2 As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DATOS")
            Result = ref2.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Listar_datos_tarea_workflow = " Error función Listar_datos_tarea_workflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Listar_datos_tarea_workflow = "Imposible encontrar la estructura de la tarea (" & HttpContext.Current.Session("ID_TAREA_SELECCIONDA") & ")"
                Exit Function
            Else
                For z2 As Integer = 0 To Datset.Tables(0).Columns.Count - 1
                    ReDim Preserve structure_colum(z2)
                    structure_colum(z2).columname = UCase(Datset.Tables(0).Columns(z2).ColumnName)
                    structure_colum(z2).columname.Replace("_", " ")
                    If Datset.Tables(0).Rows(0).IsNull(z2) = True Then
                        structure_colum(z2).columtext = ""
                    Else
                        structure_colum(z2).columtext = Datset.Tables(0).Rows(0).Item(z2).ToString
                    End If
                Next
                Listar_datos_tarea_workflow = "YES"
            End If
        Catch ex As Exception
            Listar_datos_tarea_workflow = "Inconsistencia general función  Listar_datos_tarea_workflow" & ex.Message
        End Try
    End Function
    Function Genera_interface_detalle_tarea_workflow(ByRef structure_colum() As structure_table_detalle_flujo,
                                                     ByRef table As Object,
                                                     ByRef update As UpdatePanel) As String
        Try
            Dim _LabelboxIco As Label() = {}
            Dim objRow As TableRow
            Dim objCell As TableCell
            If structure_colum Is Nothing Then
                Genera_interface_detalle_tarea_workflow = "YES"
                Exit Function
            End If
            Dim conta_celda As Integer = 0
            If structure_colum.Length > 0 Then
                For i As Integer = 0 To structure_colum.Length - 1
                    conta_celda = conta_celda + 1
                    ReDim Preserve _LabelboxIco(conta_celda)
                    objRow = New TableRow
                    objCell = New TableCell
                    _LabelboxIco(conta_celda) = New Label
                    _LabelboxIco(conta_celda).Text = UCase(structure_colum(i).columname)
                    objCell.Controls.Add(_LabelboxIco(conta_celda))
                    objRow.Cells.Add(objCell)
                    objCell = New TableCell
                    conta_celda = conta_celda + 1
                    ReDim Preserve _LabelboxIco(conta_celda)
                    _LabelboxIco(conta_celda) = New Label
                    _LabelboxIco(conta_celda).Text = structure_colum(i).columtext
                    objCell.Controls.Add(_LabelboxIco(conta_celda))
                    objRow.Cells.Add(objCell)
                    table.Rows.Add(objRow)
                Next
            End If
            update.Update()
            Genera_interface_detalle_tarea_workflow = "YES"
            Exit Function
        Catch ex As Exception
            Genera_interface_detalle_tarea_workflow = "Inconsistenca general función Genera_interface_detalle_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Genera_consulta_recuperar_tarea_workflow(ByRef Refpage As Page) As String
        Try
            Dim Matri_Campos_Lista() As String
            Erase Matri_Campos_Lista
            Dim SqlConsula As String = ""
            Dim SqlConsultaG As String = ""
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO,TIPO_CAMPO from " &
                " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                " AND LISTA_TAREA=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Genera_consulta_recuperar_tarea_workflow = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then

            Else
                Erase Matri_Campos_Lista
                Dim i As Integer = 0
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Campos_Lista(i)
                    Matri_Campos_Lista(i) = Datset.Tables(0).Rows(i).Item(0).ToString
                Next
            End If
            SqlConsula = "SELECT ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA,ETW.ID_ACTIVIDAD,LAW.NOMBRE_ACTIVIDAD,ETW.ID_USUARIO "
            If Not Matri_Campos_Lista Is Nothing Then
                Dim i As Integer = 0
                For i = 0 To UBound(Matri_Campos_Lista)
                    SqlConsula = SqlConsula & "," & Matri_Campos_Lista(i)
                Next
            End If
            SqlConsula = SqlConsula & ",UW.Nombre_Usuario AS NOMBRE_USUARIO,UW.Cargo_Usuario AS CARGO_USUARIO "
            Dim Nombre_Ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Genera_consulta_recuperar_tarea_workflow = Result
                Exit Function
            End If
            Dim SqlConsulTabla As String = " FROM DAT_ADIC_TAR" & Nombre_Ruta & " DATW "
            Dim SqlConsultaCuerpo As String = "INNER JOIN ESTADOS_TAREA_WORKFLOW ETW ON " &
              "(ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA= " &
              "DATW.INICIO_TAREAS_WORKFLOW_ID_TAREA AND ETW.FECHA_FIN IS NULL AND " &
              "ETW.ESTADO_TAREA=0) " &
              "INNER JOIN LISTADO_ACTIVIDADES_WORKFLOW LAW ON " &
              "(LAW.ID_ACTIVIDAD=ETW.ID_ACTIVIDAD)  " &
              " Left outer join usuario_workflow  as uw on (uw.idU_suario=ETW.Id_Usuario) "
            Dim Nombre_actividad As String = ""
            Dim drownousca As DropDownList = Refpage.FindControl("Drownobusca")
            If drownousca Is Nothing Then
                Genera_consulta_recuperar_tarea_workflow = "Imposible encontrar control drownobusca en la pagina"
                Exit Function
            Else
                Nombre_actividad = drownousca.Text
            End If
            Dim Id_tar As Integer = -1
            Dim Ref_class_list_actividades As New Class_Listado_Actividades_workflow
            Result = Ref_class_list_actividades.Obtener_Id_Actividad(Id_tar,
                                                                     Nombre_actividad)
            If Result <> "YES" Then
                Genera_consulta_recuperar_tarea_workflow = Result
                Exit Function
            End If
            Dim Objet As New Object
            Dim MatriTipo() As String
            Dim Tipo_Campos As String = ""
            Erase MatriTipo
            Dim Reftable As Table = Refpage.FindControl("TableControles")
            If Reftable Is Nothing Then
                Genera_consulta_recuperar_tarea_workflow = "Imposible encontrar control table en la pagina"
                Exit Function

            End If
            If Id_tar <> -1 Then
                SqlConsultaCuerpo = "INNER JOIN ESTADOS_TAREA_WORKFLOW ETW ON " &
                                   "(ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA= " &
                                   "DATW.INICIO_TAREAS_WORKFLOW_ID_TAREA AND ETW.FECHA_FIN IS NULL AND " &
                                   " ID_ACTIVIDAD <> " & Id_tar & " ) " &
                                   "INNER JOIN LISTADO_ACTIVIDADES_WORKFLOW LAW ON " &
                                   "(LAW.ID_ACTIVIDAD=ETW.ID_ACTIVIDAD)  " &
                                   " Left outer join usuario_workflow  as uw on (uw.idU_suario=ETW.Id_Usuario) "
            End If
            For i As Integer = 0 To Reftable.Controls.Count - 1
                Dim tabler As TableRow = Reftable.Controls(i)
                For Each Objet In tabler.Controls
                    For Each Objet1 In Objet.Controls
                        If Objet1.GetType().Name = "TextBox" Then
                            If Objet1.Text <> "" Then
                                SqlConsultaCuerpo = "INNER JOIN ESTADOS_TAREA_WORKFLOW ETW ON " &
                                "(ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA= " &
                                "DATW.INICIO_TAREAS_WORKFLOW_ID_TAREA AND ETW.FECHA_FIN IS NULL AND " &
                                " ID_ACTIVIDAD <> " & Id_tar & " ) " &
                                " Left outer join usuario_workflow  as uw on (uw.idU_suario=ETW.Id_Usuario) " &
                                "INNER JOIN LISTADO_ACTIVIDADES_WORKFLOW LAW ON " &
                                "(LAW.ID_ACTIVIDAD=ETW.ID_ACTIVIDAD) WHERE "
                                Exit For
                            End If
                        End If
                    Next
                Next
            Next
            SqlConsultaG = SqlConsula & SqlConsulTabla & SqlConsultaCuerpo
            Dim valorCuerpo As Integer = SqlConsultaG.Length
            For i As Integer = 0 To Reftable.Controls.Count - 1
                Dim tabler As TableRow = Reftable.Controls(i)
                For Each Objet In tabler.Controls
                    For Each Objet1 In Objet.Controls
                        If Objet1.GetType().Name = "TextBox" Then
                            If Objet1.Text <> "" Then
                                'valorCuerpo = SqlConsultaG.Length
                                If valorCuerpo = SqlConsultaG.Length Then
                                    SqlConsultaG = SqlConsultaG & Objet1.ID & "='" & Objet1.text & "' "
                                Else
                                    SqlConsultaG = SqlConsultaG & " AND " & Objet1.ID & "='" & Objet1.text & "' "
                                End If
                            End If
                        End If
                    Next
                Next
            Next
            SqlConsultaG = SqlConsultaG & " LIMIT 2000"
            Dim refgrid As GridView = Refpage.FindControl("GridViewlista")
            Dim updat As UpdatePanel = Refpage.FindControl("UpdateGeneral")
            Dim updatelabel As UpdatePanel = Refpage.FindControl("UpdatePanel_labelresultado")
            Dim label_resultado As Label = Refpage.FindControl("Label_resultado")
            If refgrid Is Nothing Then
                Genera_consulta_recuperar_tarea_workflow = "Imposible encontrar campo GridViewlista"
                Exit Function
            End If
            ref = New conect.Dbase_Conction_Mysql
            Datset = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(SqlConsultaG, Datset)
            If Result <> "YES" Then
                Genera_consulta_recuperar_tarea_workflow = " Error # Imposible encontrar datos de la tarea" & SqlConsultaG
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                label_resultado.Text = "(0) registro(s)"
                updatelabel.Update()
            Else
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                For i As Integer = 0 To refgrid.Rows.Count - 1
                    refgrid.Rows(i).Attributes.Add("id", refgrid.Rows(i).Cells(1).Text.ToString() & "-" & refgrid.Rows(i).Cells(2).Text)
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-folder-open")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("id", refgrid.Rows(i).Cells(1).Text.ToString() & "-" & refgrid.Rows(i).Cells(2).Text)
                    ahtml.Attributes.Add("title", "Documentos")
                    ahtml.Attributes.Add("tip_event", "ver_docu")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)

                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-arrow-down")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("id", refgrid.Rows(i).Cells(1).Text.ToString() & "-" & refgrid.Rows(i).Cells(2).Text)
                    ahtml.Attributes.Add("title", "Recuperar")
                    ahtml.Attributes.Add("tip_event", "asig_flujo")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    refgrid.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To refgrid.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            refgrid.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            refgrid.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                label_resultado.Text = refgrid.Rows.Count & " registro (s)"
                updatelabel.Update()
            End If
            Genera_consulta_recuperar_tarea_workflow = "YES"
        Catch ex As Exception
            Genera_consulta_recuperar_tarea_workflow = "Inconsistencia general  funcion Genera_consulta_recuperar_tarea_workflow " & ex.Message
        End Try
    End Function
    Function Consulta_Datos_Tareas_estudiadas(ByRef Refpage As Page) As String
        Try
            Dim Matri_Campos_Lista() As String
            Erase Matri_Campos_Lista
            Dim SqlConsula As String = ""
            Dim SqlConsultaG As String = ""
            Dim Ref_TextBoxFECHA_ENVIO_INI As TextBox = Refpage.FindControl("TextBoxFECHA_ENVIO_INI")
            Dim Ref_TextBoxFECHA_ENVIO_FIN As TextBox = Refpage.FindControl("TextBoxFECHA_ENVIO_FIN")
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO,TIPO_CAMPO from " &
                " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                " AND LISTA_TAREA=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Consulta_Datos_Tareas_estudiadas = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA" & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then

            Else
                Erase Matri_Campos_Lista
                Dim i As Integer = 0
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Campos_Lista(i)
                    Matri_Campos_Lista(i) = Datset.Tables(0).Rows(i).Item(0).ToString
                Next
            End If
            SqlConsula = "SELECT DISTINCT ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA,ETW.ID_ACTIVIDAD,LAW.NOMBRE_ACTIVIDAD,ETW.ID_USUARIO,CAST(ETW.FECHA_FIN AS DATE) AS FECHA_TAREA_TERMINADA "
            If Not Matri_Campos_Lista Is Nothing Then
                Dim i As Integer = 0
                For i = 0 To UBound(Matri_Campos_Lista)
                    SqlConsula = SqlConsula & "," & Matri_Campos_Lista(i)
                Next
            End If
            SqlConsula = SqlConsula & ",UW.Nombre_Usuario AS NOMBRE_USUARIO,UW.Cargo_Usuario AS CARGO_USUARIO "
            Dim Nombre_Ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Consulta_Datos_Tareas_estudiadas = Result
                Exit Function
            End If
            Dim SqlConsulTabla As String = " FROM ESTADOS_TAREA_WORKFLOW ETW "
            Dim SqlConsultaCuerpo As String = " INNER JOIN DAT_ADIC_TAR" & Nombre_Ruta & " DATW  ON " &
              "(ETW.INICIO_TAREAS_WORKFLOW_ID_TAREA= " &
              "DATW.INICIO_TAREAS_WORKFLOW_ID_TAREA ) " &
               " Left outer join usuario_workflow  as uw on (uw.idU_suario=ETW.Id_Usuario) " &
               " INNER JOIN LISTADO_ACTIVIDADES_WORKFLOW LAW ON " &
              "(LAW.ID_ACTIVIDAD=ETW.ID_ACTIVIDAD) WHERE ETW.FECHA_FIN IS NOT NULL AND " &
              " ETW.Id_Usuario=" & HttpContext.Current.Session("Id_Usuario_Workflow") & " and " &
              " ETW.Inicio_Tareas_Workflow_Rutas_Workflow_id_Ruta=" & HttpContext.Current.Session("Id_Ruta_Workflow")
            Dim sql_condicion As String = ""
            If Ref_TextBoxFECHA_ENVIO_INI.Text <> "" And Ref_TextBoxFECHA_ENVIO_FIN.Text <> "" Then
                sql_condicion = " AND CAST(ETW.FECHA_FIN AS DATE) BETWEEN '" & Ref_TextBoxFECHA_ENVIO_INI.Text & "' AND '" &
                Ref_TextBoxFECHA_ENVIO_FIN.Text & "'"
            Else
                If Ref_TextBoxFECHA_ENVIO_INI.Text <> "" Then
                    sql_condicion = sql_condicion & " AND CAST(ETW.FECHA_FIN AS DATE)='" & Ref_TextBoxFECHA_ENVIO_INI.Text & "'"
                End If
                If Ref_TextBoxFECHA_ENVIO_FIN.Text <> "" Then
                    sql_condicion = sql_condicion & " AND CAST(ETW.FECHA_FIN AS DATE)='" & Ref_TextBoxFECHA_ENVIO_FIN.Text & "'"
                End If
            End If
            SqlConsultaG = SqlConsula & SqlConsulTabla & SqlConsultaCuerpo & sql_condicion
            SqlConsultaG = SqlConsultaG & " order by  FECHA_FIN desc LIMIT 2000"
            Dim refgrid As GridView = Refpage.FindControl("GridViewlista")
            Dim updat As UpdatePanel = Refpage.FindControl("UpdateGeneral")
            Dim updatelabel As UpdatePanel = Refpage.FindControl("UpdatePanel_labelresultado")
            Dim label_resultado As Label = Refpage.FindControl("Label_resultado")
            If refgrid Is Nothing Then
                Consulta_Datos_Tareas_estudiadas = "Imposible encontrar campo GridViewlista"
                Exit Function
            End If
            ref = New conect.Dbase_Conction_Mysql
            Datset = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(SqlConsultaG, Datset)
            If Result <> "YES" Then
                Consulta_Datos_Tareas_estudiadas = " Error # Imposible encontrar datos de la tarea" & SqlConsultaG
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                label_resultado.Text = "(0) registro(s)"
                updatelabel.Update()
                Consulta_Datos_Tareas_estudiadas = "YES"
                Exit Function
            Else
                refgrid.DataSource = Datset
                refgrid.DataBind()
                updat.Update()
                For i As Integer = 0 To refgrid.Rows.Count - 1
                    refgrid.Rows(i).Attributes.Add("id", refgrid.Rows(i).Cells(1).Text.ToString() & "-" & refgrid.Rows(i).Cells(2).Text)
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-folder-open")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("id", refgrid.Rows(i).Cells(1).Text.ToString() & "-" & refgrid.Rows(i).Cells(2).Text)
                    ahtml.Attributes.Add("title", "Documentos")
                    ahtml.Attributes.Add("tip_event", "ver_docu")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    ihtml = New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-arrow-down")
                    ihtml.Style.Add("color", "white")
                    ahtml = New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-danger btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("id", refgrid.Rows(i).Cells(1).Text.ToString() & "-" & refgrid.Rows(i).Cells(2).Text)
                    ahtml.Attributes.Add("title", "Recuperar")
                    ahtml.Attributes.Add("tip_event", "asig_flujo")
                    ahtml.Style.Add("margin-left", "3px")
                    ahtml.Controls.Add(ihtml)
                    divhtml.Controls.Add(ahtml)
                    divhtml.Style.Add("display", "inline-flex")
                    refgrid.Rows(i).Cells(0).Controls.Add(divhtml)
                    For z As Integer = 0 To refgrid.Rows(i).Cells.Count - 1
                        If z > 0 Then
                            refgrid.Rows(i).Cells(z).Attributes.Add("Class", "GridviewScrollItem_line_cort_tr_flex")
                            refgrid.Rows(i).Cells(z).Attributes.Add("onclick", "prevent_scrol(event,this);")
                        End If
                    Next
                Next
                'For i As Integer = 0 To refgrid.Rows.Count - 1
                '    Dim tex As String = refgrid.Rows(i).Cells(1).Text & "-" & refgrid.Rows(i).Cells(2).Text
                '    refgrid.Rows(i).Attributes.Add("id", tex)
                'Next
                label_resultado.Text = "Se encontraron (" & refgrid.Rows.Count & ") registros limite 2000 registros"
                updatelabel.Update()
                Consulta_Datos_Tareas_estudiadas = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Consulta_Datos_Tareas_estudiadas = "Error General  funcion Consulta datos tarea" & ex.Message

        End Try
    End Function
    Function Solicita_datos_auto_complete_tareas_workflow(ByVal name_dbs_auto As String,
                                                          ByVal name_table_auto As String,
                                                          ByVal name_campo_auto As String,
                                                          ByVal value_auto As String,
                                                          ByRef country As List(Of String)) As String
        Try
            Dim ref As Object
            Dim valor_consulta As String = name_campo_auto & " like '%" & value_auto & "%'"
            Dim Sql_consulta As String = "Select distinct (DAT." & name_campo_auto & ")  from " &
                            " estados_tarea_workflow etw " &
                            " inner join dat_adic_tar" & HttpContext.Current.Session.Item("WF_RUTAWORKFLOW") & " as  DAT on " &
                            " (etw.Inicio_Tareas_Workflow_id_Tarea=DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA " & " ) " &
                            " Left outer join wf_flujos_trabajo as wf_fl on (wf_fl.ID_WF_FLUJOS_TRABAJO=etw.ID_FLUJO_TRABAJO)" &
                            " where (" & valor_consulta & ") " &
                            " and ((etw.id_actividad=" & HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD") & " and etw.fecha_fin is null " & "and etw.id_usuario=" & HttpContext.Current.Session("Id_Usuario_Workflow") & ")" &
                            "  or (etw.id_actividad=" & HttpContext.Current.Session.Item("WF_ID_ACTIVIDAD") & " And etw.fecha_fin Is null   And etw.id_usuario Is null)) limit 50"

            If name_dbs_auto = "WF" Then
                ref = New conect.Dbase_Conction_Mysql
            Else
                ref = New conect.Dbase_Conction_Mysql_RA
            End If
            Dim Datset As DataSet = New DataSet("DAT_ADIC")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_auto_complete_tareas_workflow = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                country = Nothing
                Solicita_datos_auto_complete_tareas_workflow = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).IsNull(0) = False Then
                        Dim obsgetipe As Object = Datset.Tables(0).Rows(i).Item(0).GetType.ToString
                        If obsgetipe = "System.DateTime" Then
                            Dim subtrin As String = Datset.Tables(0).Rows(i).Item(0).ToString()
                            Dim splitsubtrin() As String = subtrin.Substring(0, 10).Split("/")
                            country.Add(splitsubtrin(2) & "-" & splitsubtrin(1) & "-" & splitsubtrin(0))
                        Else
                            country.Add(Datset.Tables(0).Rows(i).Item(0).ToString())
                        End If
                    End If
                Next
                Solicita_datos_auto_complete_tareas_workflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_datos_auto_complete_tareas_workflow = "Inconsistencia general funcion Solicita_datos_auto_complete_tareas_workflow " & ex.Message
        End Try
    End Function
    Function solicita_structucre_consulta_ruta(ByVal name_espace_form_control As String,
                                               ByRef Class_config_general_service As List(Of Class_config_general_service)) As String
        Try
            Dim Result As String = ""
            Dim Nombre_Ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                solicita_structucre_consulta_ruta = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO,TIPO_CAMPO from " &
                " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                " AND LISTA_TAREA=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("DAT_ADIC")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                solicita_structucre_consulta_ruta = "Error funcion Solicita_lista_campos_ruta_service " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                solicita_structucre_consulta_ruta = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA"
                Exit Function
            End If
            For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                Dim parameter_gestion As Class_config_general_service = New Class_config_general_service()
                parameter_gestion.aleas_campo = Datset.Tables(0).Rows(i).Item(0)
                parameter_gestion.name_campo = Datset.Tables(0).Rows(i).Item(0)
                parameter_gestion.alow_null = 1
                parameter_gestion.alow_tipo_value = 1
                parameter_gestion.campo_tip = 1
                parameter_gestion.max_leng_campo = 0
                parameter_gestion.name_space_campo = name_espace_form_control
                parameter_gestion.dbms_control = "WF"
                parameter_gestion.tbl_control = "dat_adic_tar" & Nombre_Ruta
                parameter_gestion.clas_service_control = "WebServiceWorkflow.asmx"
                parameter_gestion.service_control = "Service_Solicita_datos_auto_complete_tareas_workflow"
                parameter_gestion.error_gestion = "YES"
                Class_config_general_service.Add(parameter_gestion)
            Next
            solicita_structucre_consulta_ruta = "YES"
        Catch ex As Exception
            solicita_structucre_consulta_ruta = "Inconsistencia general fucion Solicita_lista_campos_ruta_service " & ex.Message
        End Try
    End Function
    Function Genera_interface_recuperar_rarea_workflow(ByRef Page1 As Page) As String
        Try
            Dim Update As New UpdatePanel
            Update.ID = "Recupera"
            Update.UpdateMode = UpdatePanelUpdateMode.Conditional
            Dim Matri_Campos_Lista() As String
            Dim Result As String = ""
            Dim Nombre_Ruta As String = ""
            Dim Ref_calss_wf_ruta As New Class_worflow_rutas
            Result = Ref_calss_wf_ruta.Solicita_nombre_ruta_workflow(HttpContext.Current.Session("Id_Ruta_Workflow").ToString,
                                                                     Nombre_Ruta)
            If Result <> "YES" Then
                Genera_interface_recuperar_rarea_workflow = Result
                Exit Function
            End If
            Dim Sql_consulta As String = "Select NOMBRE_CAMPO,TIPO_CAMPO from " &
                " CONFIGURACION_LISTADO_RUTA WHERE RUTAS_WORKFLOW_ID_RUTA =" & HttpContext.Current.Session("Id_Ruta_Workflow") &
                " AND LISTA_TAREA=1"
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("CONFIGURACION_GABINETE")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Genera_interface_recuperar_rarea_workflow = "Error Consultando Configuracion listado ruta " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Genera_interface_recuperar_rarea_workflow = " Error # 01 Imposible Encontrar campos en la tabla CONFIGURACION_LISTADO_RUTA" & Sql_consulta
                Exit Function
            End If
            Dim Ref_classworkflow As New ClassWorkflow
            If Datset.Tables(0).Rows.Count > 0 Then
                Erase Matri_Campos_Lista
                Dim i As Integer = 0
                For i = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve Matri_Campos_Lista(i)
                    Matri_Campos_Lista(i) = Datset.Tables(0).Rows(i).Item(0).ToString & "|" & Datset.Tables(0).Rows(i).Item(1).ToString
                Next
                Dim Table As Table = Page1.FindControl("TableControles")
                Dim objRow As TableRow
                Dim objCell As TableCell
                Dim Icontr As Integer = 0
                Dim Z As Integer = 0
                Dim m_TextBoxes() As TextBox = {}
                'Dim LabelBox() As Label = {}
                Dim Matri_CampoES() As String
                Erase Matri_CampoES
                Dim pane As Panel = Page1.FindControl("Panel1")
                Dim spanhtml As New HtmlControls.HtmlGenericControl("span")
                For Z = 0 To UBound(Matri_Campos_Lista)
                    ReDim Preserve m_TextBoxes(Z)
                    spanhtml = New HtmlControls.HtmlGenericControl("span")
                    m_TextBoxes(Z) = New TextBox
                    Erase Matri_CampoES
                    Matri_CampoES = Matri_Campos_Lista(Z).Split("|")
                    If Matri_CampoES(0) = "" Then
                        spanhtml.InnerText = "SIN CAMPO"
                        m_TextBoxes(Z).Text = "SIN CAMPO"
                        m_TextBoxes(Z).ID = "SIN CAMPO-Z"
                    Else
                        spanhtml.InnerText = Matri_CampoES(0)
                        m_TextBoxes(Z).Text = ""
                        m_TextBoxes(Z).ID = Matri_CampoES(0).ToString
                        m_TextBoxes(Z).CssClass = "form-control"
                        m_TextBoxes(Z).Attributes.Add("onDblClick", "presionBoton('" + Matri_CampoES(0).ToString + "-" _
                       + Matri_CampoES(1).ToString + "')")
                    End If
                    objRow = New TableRow()
                    objRow.CssClass = "m-2"
                    objCell = New TableCell
                    objCell.Controls.Add(spanhtml)
                    objRow.Cells.Add(objCell)
                    objCell = New TableCell
                    objCell.Controls.Add(m_TextBoxes(Z))
                    objRow.Cells.Add(objCell)
                    Table.Rows.Add(objRow)
                    Dim trigText As New AsyncPostBackTrigger()
                    trigText.ControlID = m_TextBoxes(Z).ID
                    Update.Triggers.Add(trigText)
                    Result = Ref_classworkflow.agregar_auto_complete_workflow(m_TextBoxes(Z).ID, pane, "GetPosiblesDatos", "DAT_ADIC_TAR" & Nombre_Ruta, m_TextBoxes(Z).ID)
                    If Result <> "YES" Then
                        Genera_interface_recuperar_rarea_workflow = Result
                        Exit Function

                    End If
                Next
                pane.Controls.Add(Table)
                Dim Table1 As New Table
                Dim Consec As Integer = UBound(Matri_Campos_Lista) + 1
                spanhtml = New HtmlControls.HtmlGenericControl("span")
                Dim Refcomb As New DropDownList
                Refcomb.ID = "Drownobusca"
                spanhtml.InnerText = "Actividad que no busca el sistema"
                objRow = New TableRow()
                objRow.CssClass = "m-2"
                objCell = New TableCell
                objCell.Controls.Add(spanhtml)
                objRow.Cells.Add(objCell)
                objCell = New TableCell
                objCell.Controls.Add(Refcomb)
                objRow.Cells.Add(objCell)
                Table.Rows.Add(objRow)
                '-------------------------------------
                Dim Comandb As New Button
                Dim labelGABI As New Label
                Comandb.Text = "Consultar"
                Comandb.Height = 35
                Comandb.ID = "Consultar"
                Comandb.CssClass = "btn btn-success"
                AddHandler Comandb.Click, AddressOf _
                comman_clik
                objRow = New TableRow()
                objRow.CssClass = "m-2"
                objCell = New TableCell
                objCell.Text = "               "
                objRow.Cells.Add(objCell)
                Table.Rows.Add(objRow)
                objCell = New TableCell
                objCell.Controls.Add(Comandb)
                objRow.Cells.Add(objCell)
                Table.Rows.Add(objRow)
                Dim btn4 As Object = Page1.FindControl("btnOkay")
                Dim trigModal As New AsyncPostBackTrigger()
                trigModal.ControlID = btn4.ID
                trigModal.EventName = "Click"
                Update.Triggers.Add(trigModal)
                Dim Drowlis As GridView = Page1.FindControl("GridViewlista")
                Dim trigForBtns As New AsyncPostBackTrigger()
                trigForBtns.ControlID = Drowlis.ID
                Dim tribOTON1 As New AsyncPostBackTrigger()
                tribOTON1.ControlID = Comandb.ID
                Update.Triggers.Add(tribOTON1)
                Page1.Form.Controls.Add(Update)
                Result = ""
                Dim Ref_listado As New Class_Listado_Actividades_workflow
                Result = Ref_listado.Lista_Actividades_Combo_Duplex(Refcomb)
                If Result <> "YES" Then
                    Genera_interface_recuperar_rarea_workflow = "Imposible listar actividades " & Result
                    Exit Function
                End If

            End If
            Genera_interface_recuperar_rarea_workflow = "YES"
        Catch ex As Exception
            Genera_interface_recuperar_rarea_workflow = "Error general funcion Genera_interface_recuperar_rarea_workflow " & ex.Message
        End Try
    End Function
    Private Sub comman_clik(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim Ref As New Classscrripjava
            Dim Pag As Page = sender.Page
            Dim Matri_Sender() As String
            Dim Result As String = ""
            Erase Matri_Sender
            Dim hiden As Object = Pag.FindControl("HiddenFiltro")
            hiden.value = ""
            Dim updat As UpdatePanel = Pag.FindControl("Recupera")
            Matri_Sender = Split(sender.id, "|")
            Dim Ref_class_dat_adic As New Class_DAT_ADIC_TAR
            Result = Ref_class_dat_adic.Genera_consulta_recuperar_tarea_workflow(Pag)
            If Result <> "YES" Then
                Ref.Showscripman(Result, updat)
                Exit Sub
            End If
        Catch ex As Exception

        End Try
    End Sub
    Function SolicitaNombreGabneteTareaWokflow(ByVal NombreRutaWorkflow As String,
                                               ByVal IdTareaWorflow As Long,
                                               ByRef NombreGabinete As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el nombre del gabinete de la tarea workflow relacionada
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreRutaWorkflow  : Representa el nombre de la ruta de la tarea workflow
        'IdTareaWorflow      : Representa la identificaicón de la tarea workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete    : Retorna el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-06
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT cg.NOMBRE_GABINETE FROM dat_adic_tar" & NombreRutaWorkflow &
            " dat INNER JOIN configuracion_gabinete cg on (cg.id_Gabinete=dat.ID_GABINETE) " &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Dim Result As String = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreGabneteTareaWokflow = "Inconsistencia detectada en la función SolicitaNombreGabneteTareaWokflow " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreGabneteTareaWokflow = "No se localizaron datos vinculados a la tarea de workflow para identificar el gabinete correspondiente."
                Exit Function
            Else
                NombreGabinete = Datset.Tables(0).Rows(0).Item(0)
                SolicitaNombreGabneteTareaWokflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreGabneteTareaWokflow = "Inconsistencia general funcion SolicitaNombreGabneteTareaWokflow " & ex.Message
        End Try
    End Function
    Function SolicitaNombreGabineteImagenTareaWorkflow(ByVal IdTareaWorkflow As Long,
                                                       ByRef NombreGabinete As String,
                                                       ByRef IdImagen As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita nombre de gabinete y identiifcación de imagen de una tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaWorkflow     : Representa la identificación de la tarea workfflow
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'NombreGabinete  : Retorna el nombre del gabinete
        'IdImagen        : Retorna la identificación de la imagen relacionada a la tarea
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2017-01-27
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ClassListandoTareas As New ClassListandoTareas
            Dim Result As String = ""
            Dim NombreRuta As String = ""
            Dim Class_worflow_rutas As New Class_worflow_rutas
            Result = Class_worflow_rutas.Solicita_nombre_ruta_workflow(HttpContext.Current.Session.Item("Id_Ruta_Workflow").ToString,
                                                                       NombreRuta)
            If Result <> "YES" Then
                SolicitaNombreGabineteImagenTareaWorkflow = "Error #23 Imposible Encontrar nombre de Ruta " + Result
                Exit Function
            End If
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT cg.NOMBRE_GABINETE,dat.ID_IMAGEN FROM dat_adic_tar" & NombreRuta &
            " dat INNER JOIN configuracion_gabinete cg on (cg.id_Gabinete=dat.ID_GABINETE) " &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("configuracion_gabinete")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                SolicitaNombreGabineteImagenTareaWorkflow = "Error de conexión función SolicitaNombreGabineteImagenTareaWorkflow " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaNombreGabineteImagenTareaWorkflow = "No se encuentran datos del gabinete en la tabla de configuración para la indentificación de la tarea (" & IdTareaWorkflow & ")"
                Exit Function
            Else
                NombreGabinete = Datset.Tables(0).Rows(0).Item(0)
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    IdImagen = -1
                Else
                    IdImagen = Datset.Tables(0).Rows(0).Item(1)
                End If
                SolicitaNombreGabineteImagenTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaNombreGabineteImagenTareaWorkflow = "Inconsistencia general funcion ConsultaNombreGabinete  " & ex.Message
        End Try
    End Function
    Function SolicitaDatosEstructuraBasicaTareaWorkflow(ByVal NombreRuta As String,
                                                        ByVal IdTareaWorkflow As Long,
                                                        ByRef structure_datos_tarea_workflow As structure_datos_tarea_workflow) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita los datos de la estructura basica de una tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombreRuta          : Representa el nombre de la ruta workflow
        'IdTareaWorkflow     : Representa la identificación de la tarea
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'structure_datos_tarea_workflow  : Solicita la estructura de la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-03
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim SqlConsulta As String = ""
            SqlConsulta = "SELECT ID_DAT,ID_GABINETE,ID_IMAGEN,FLUJO_INTERNO_WF " &
            " FROM DAT_ADIC_TAR" & NombreRuta &
            " WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA=" & IdTareaWorkflow
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("ESTADOS_TAREA_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(SqlConsulta, Datset)
            If Result <> "YES" Then
                SolicitaDatosEstructuraBasicaTareaWorkflow = "Funcion Solicita_datos_estructura_basica_tarea dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaDatosEstructuraBasicaTareaWorkflow = "Imposible encontrar los datos basicos de la tarea workflow (" & IdTareaWorkflow & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    structure_datos_tarea_workflow.ID_DAT = 0
                Else
                    structure_datos_tarea_workflow.ID_DAT = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    structure_datos_tarea_workflow.ID_GABINETE = 0
                Else
                    structure_datos_tarea_workflow.ID_GABINETE = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    structure_datos_tarea_workflow.ID_IMAGEN = 0
                Else
                    structure_datos_tarea_workflow.ID_IMAGEN = Datset.Tables(0).Rows(0).Item(2)
                End If
                If Datset.Tables(0).Rows(0).IsNull(3) = True Then
                    structure_datos_tarea_workflow.FLUJO_INTERNO_WF = 0
                Else
                    structure_datos_tarea_workflow.FLUJO_INTERNO_WF = Datset.Tables(0).Rows(0).Item(3)
                End If
                SolicitaDatosEstructuraBasicaTareaWorkflow = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaDatosEstructuraBasicaTareaWorkflow = "Inconsistencia general funcion SolicitaDatosEstructuraBasicaTareaWorkflow " & ex.Message
        End Try
    End Function

End Class
