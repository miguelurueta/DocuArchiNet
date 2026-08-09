Public Class Class_permisos_usuarios_workflow_service
    Property Error_gestion As String
    Property permisos_int_sii As class_permisos_integracion_sii
End Class
Public Class class_permisos_integracion_sii
    Property util_sii_registro_tarea_ruta As Object
    Property util_sii_registro_tarea_flujo As Object
    Property util_sii_gestion_tarea_rue As Object
    Property util_sii_gestion_tarea_virtual As Object
    Property util_sii_getion_tarea As Object
    Property util_reasigna_tarea_workflow_sii As Object
    Property util_gestion_reasing_user As Object
End Class
Public Class Class_permisos_usuarios_workflow
    Function Solicita_permisos_usuario_workflow_intgracion_sii(ByVal id_usuario_workflow As Integer,
                                                               ByRef class_permisos_interacion_sii As class_permisos_integracion_sii) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita permisos del usuario workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_usuario_workflow         : Representa la identificación del usuario workflow
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'class_permisos_interacion_sii  : Retorna la matriz de permisos integración SII
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-01-05
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("PERMISOS_USUARIO_WORKFLOW")
            Dim SQL_consulta As String = "SELECT UTIL_SII_REGISTRO_TAREA_RUTA,UTIL_SII_REGISTRO_TAREA_FLUJO," &
                "UTIL_SII_GESTION_TAREA_RUE,UTIL_SII_GESTION_TAREA_VIRTUAL,UTIL_SII_GETION_TAREA,REASIGNA_TAREA_WORKFLOW_SII,UTIL_GESTION_REASING_USER " &
                " from PERMISOS_USUARIO_WORKFLOW WHERE Usuario_Workflow_idU_suario =" & id_usuario_workflow
            Result = ref.SELECTION_SELECT_FIELD(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_permisos_usuario_workflow_intgracion_sii = "Error funcion Solicita_permisos_usuario_workflow_intgracion_sii " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_permisos_usuario_workflow_intgracion_sii = "Usuario sin permisos de  integración sii  "
                Exit Function
            Else
                class_permisos_interacion_sii.util_sii_registro_tarea_ruta = Datset.Tables(0).Rows(0).Item("UTIL_SII_REGISTRO_TAREA_RUTA")
                class_permisos_interacion_sii.util_sii_registro_tarea_flujo = Datset.Tables(0).Rows(0).Item("UTIL_SII_REGISTRO_TAREA_FLUJO")
                class_permisos_interacion_sii.util_sii_gestion_tarea_rue = Datset.Tables(0).Rows(0).Item("UTIL_SII_GESTION_TAREA_RUE")
                class_permisos_interacion_sii.util_sii_gestion_tarea_virtual = Datset.Tables(0).Rows(0).Item("UTIL_SII_GESTION_TAREA_VIRTUAL")
                class_permisos_interacion_sii.util_sii_getion_tarea = Datset.Tables(0).Rows(0).Item("UTIL_SII_GETION_TAREA")
                class_permisos_interacion_sii.util_reasigna_tarea_workflow_sii = Datset.Tables(0).Rows(0).Item("REASIGNA_TAREA_WORKFLOW_SII")
                class_permisos_interacion_sii.util_gestion_reasing_user = Datset.Tables(0).Rows(0).Item("UTIL_GESTION_REASING_USER")
                Solicita_permisos_usuario_workflow_intgracion_sii = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_permisos_usuario_workflow_intgracion_sii = "Inonistencia gneral funcion Solicita_permisos_usuario_workflow_intgracion_sii " & ex.Message
        End Try
    End Function
    Function SolicitaPermisosUsuarioWorkflow(ByVal IdUsuarioWorkflow As Integer,
                                            ByRef matri() As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita permisos del usuario workflow
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'Id_Usuario           : Representa la identificación del usuario
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'matri  : Retorna la matriz de permisos
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2009-07-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "SELECT  PUW.SELECCION_MANUAL,PUW.SELECCION_AUTOMATICO,PUW.ACTUALIZAR_IMAGEN," _
           & "PUW.DATOS_EXTERNOS,PUW.INTERACTUAR_APLICACIONES,PUW.INTERACTUAR_MENSAJERIA" _
           & ",PUW.INTERACTUAR_ALERTAS,PUW.EDITAR_INDICE_IMAGEN,PUW.CAMBIO_RUTA," _
           & "PUW.INTERACTUAR_ANOTACIONES,PUW.INTERACTUAR_PENDIENTE,PUW.AGREGAR_FIRMA,PUW.AGREGAR_STAMP" _
           & ",PUW.ADJUNTAR_IMAGENES_USUARIO,PUW.ADJUNTAR_IMAGENES_PREDETERMINADA,ADJUNTAR_SELLO" _
           & " ,PUW.IMPRIMIR_IMAGENES,PUW.EJECUTAR_CODIGO_DEFAULT,PUW.CAMBIO_USUARIO," &
           "PUW.RECUPERAR_TAREA, PUW.UNIR_TAREA,PUW.DUPLICAR_DOCUMENTO,PUW.AGREGAR_DOCUMENTO_LIBRE," &
           "PUW.AGREGAR_DOCUMENTO_TRD,PUW.EDITAR_INDICE_WORKFLOW,PUW.SELECIONA_ACTIVIDAD_AREA_WORKFLOW," &
           "PUW.SELECIONA_ACTIVIDAD_USUARIO_WORKFLOW,REASIGNA_TAREA,RESPUESTA_LIBRE," &
           "COMPARTE_USUARIO_INTERNO,COMPARTE_CORREO_ELECTRONICO,ESTADO_PENDIENTE_APROBACION,LISTA_ESTADO_PENDIENTE_APROBACION," &
           "RESPUESTA_TRAMITE,REASIGNA_RESPUESTA_TRAMITE,CAMBIA_FLUJO_TRABAJO,GESTION_FLUJOS_TRABAJO,REVERSA_RESPUESTA," &
           "UTIL_PAGINACION,COPIA_ESTRUCTURA_PRODUCION,RELACIONA_EXPEDIENTE,UTIL_ITER_PENDIENTE,FIRMA_DIGITAL_DOCUMENTO_WF,DEVOLVER_TAREA_WORKFLOW," &
           "EXPORTA_GABINETE_WORKFLOW,MASTER_ELIMINA_GABINETE_WORKFLOW,REASIGNA_TAREA_WORKFLOW,REASIGNA_TAREA_WORKFLOW_SII,COPIA_DOCUMENTO_EXPEDIENTE," &
           "WF_ACTUALIZA_INDICE_BATCH_WF,UTIL_SAVE_DOCUMENT,UTIL_VISOR_EXPRESS,UTIL_GESTION_REASING_USER,UTIL_VER_WF_RESTAURA_VERSION_DOCUMENTO_GABINETE," &
           "UTIL_VER_WF_ELIMINA_VERSION_DOCUMENTO,UTIL_VER_WF_REMPLAZA_VERSION_DOCUMENTO,UTIL_SII_REGISTRO_TAREA_RUTA,UTIL_SII_REGISTRO_TAREA_FLUJO," &
           "UTIL_SII_GESTION_TAREA_RUE,UTIL_SII_GESTION_TAREA_VIRTUAL,UTIL_SII_GETION_TAREA,UTIL_VER_WF_MASTER_REMPLAZA_VERSION_DOCUMENTO" &
           " from PERMISOS_USUARIO_WORKFLOW as PUW" &
            " WHERE Usuario_Workflow_idU_suario = " & IdUsuarioWorkflow
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("PERMISOS_USUARIO_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaPermisosUsuarioWorkflow = " Error Verificando Permisos   " & Result
                Return SolicitaPermisosUsuarioWorkflow
                Exit Function
            Else
                If Datset.Tables(0).Rows.Count = 0 Then
                    SolicitaPermisosUsuarioWorkflow = " Usuario sin permisos de iteracción    "
                    Return SolicitaPermisosUsuarioWorkflow
                    Exit Function
                Else
                    Dim i As Integer = 0
                    Dim icontador As Integer = 0
                    Erase matri
                    For i = 0 To Datset.Tables(0).Columns.Count - 1
                        ReDim Preserve matri(icontador)
                        matri(icontador) = Datset.Tables(0).Rows(0).Item(i).ToString
                        icontador = icontador + 1
                    Next
                    SolicitaPermisosUsuarioWorkflow = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            SolicitaPermisosUsuarioWorkflow = "Inconsistencia general función  SolicitaPermisosUsuarioWorkflow " & ex.Message
        End Try
    End Function
End Class
