Public Class CTipoDocEntrante
    Property id_Tipo_Doc_Entrante As Integer
    Property Descripcion_Doc As String
    Property system_plantilla_radicado_id_plantilla As Integer
    Property numero_dias_vence As Integer
    Property estado_tipo_documento As Integer
    Property flow_tipo As Integer
    Property requiere_respuesta As Integer
    Property codigo_gabinete_workflow As Integer
    Property nombre_gabinete_workflow As String
    Property tipo_activo_pqr As Integer
    Property obliga_respuesta_radicado_tramite As Integer
    Property resp_correo_fisico_electronico As Integer
    Property id_ruta As Integer
    Property ID_RA_WF_GRUPOS_WORKFLOW_TRAMITE As Integer
    Property ID_WF_FLUJOS_TRABAJO As Integer
    Property tipo_activo_rad_interno As Integer
    Property nombre_tramite As String
    Property tipo_registro As String
    Property descripcion_tramite As String
    Property tipo_tramite As Integer
    Property estado_ruta_open_close As Integer
    Property obliga_exp_radicado As Integer
    Property activo_modulo_respuesta As Integer
    Property util_tipo_modulo_envio As Integer
    Property util_producion_documental As Integer
    Property tipo_tramite_entrante_saliente As Integer
    Property ra_auto_registro_expediente_id_auto_registro As Integer
    Property wf_copia_doc_expediente_actualiza_exped_gabinete As Integer
    Property wf_auto_vincula_doc_expediente_actualiza_exped_gabinete As Integer
    Property wf_copia_doc_expediente_produc_actualiza_exped_gabinete As Integer
    Property util_auto_vincula_migracion As Integer
    Property id_gabinete As Integer
    Property util_radicacion_simple As Integer
    Property util_nivel_padre_auto_vincula As Integer
    Property util_opcion_auto_vincula As Integer
    Property util_Estado_Crea_ExpedienteSII As Integer
    Property util_Estado_Multiple_expedienteSII As Integer
    Property Id_ser_servicioIntegracion As Integer
    Property RaSerServicioInteracion As New RaSerServicioInteracion
End Class
Public Class Class_tipo_doc_entrante
    Function SolicitaEstructuraTramite(ByVal IdTramite As Integer,
                                       ByRef CTipoDocEntrante As CTipoDocEntrante) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la estructura del tramite
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTramite           : Representa la identificación del tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'CTipoDocEntrante      : Retorna la estructura del tipo documento
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-10
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim SQLconsulta As String = "Select id_Tipo_Doc_Entrante,Descripcion_Doc,system_plantilla_radicado_id_plantilla,numero_dias_vence," &
                "estado_tipo_documento,flow_tipo,requiere_respuesta,codigo_gabinete_workflow,nombre_gabinete_workflow,tipo_activo_pqr," &
                "obliga_respuesta_radicado_tramite,resp_correo_fisico_electronico,id_ruta,ID_RA_WF_GRUPOS_WORKFLOW_TRAMITE,ID_WF_FLUJOS_TRABAJO," &
                "tipo_activo_rad_interno,nombre_tramite,tipo_registro,descripcion_tramite,tipo_tramite,estado_ruta_open_close,obliga_exp_radicado," &
                "activo_modulo_respuesta,util_tipo_modulo_envio,util_producion_documental,tipo_tramite_entrante_saliente," &
                "ra_auto_registro_expediente_id_auto_registro,wf_copia_doc_expediente_actualiza_exped_gabinete," &
                "wf_auto_vincula_doc_expediente_actualiza_exped_gabinete,wf_copia_doc_expediente_produc_actualiza_exped_gabinete," &
                "util_auto_vincula_migracion,id_gabinete,util_radicacion_simple,util_nivel_padre_auto_vincula,util_opcion_auto_vincula," &
                "util_Estado_Crea_ExpedienteSII,util_Estado_Multiple_expedienteSII,Id_ser_servicioIntegracion " &
                " from tipo_doc_entrante where id_Tipo_Doc_Entrante=" & IdTramite
            Dim ConexDB As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = ConexDB.SELECTION_SELECT_FIELDA(SQLconsulta, Datset)
            If Result <> "YES" Then
                SolicitaEstructuraTramite = "Error funcion  SolicitaEstructuraTramite " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaEstructuraTramite = "El sistema no pudo encontrar la estructura del trámite con el número de identificación proporcionado : (" & IdTramite & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull("id_Tipo_Doc_Entrante") = True Then
                    CTipoDocEntrante.id_Tipo_Doc_Entrante = 0
                Else
                    CTipoDocEntrante.id_Tipo_Doc_Entrante = Datset.Tables(0).Rows(0).Item("id_Tipo_Doc_Entrante")
                End If
                If Datset.Tables(0).Rows(0).IsNull("Descripcion_Doc") = True Then
                    CTipoDocEntrante.Descripcion_Doc = ""
                Else
                    CTipoDocEntrante.Descripcion_Doc = Datset.Tables(0).Rows(0).Item("Descripcion_Doc")
                End If
                If Datset.Tables(0).Rows(0).IsNull("system_plantilla_radicado_id_plantilla") = True Then
                    CTipoDocEntrante.system_plantilla_radicado_id_plantilla = 0
                Else
                    CTipoDocEntrante.system_plantilla_radicado_id_plantilla = Datset.Tables(0).Rows(0).Item("system_plantilla_radicado_id_plantilla")
                End If
                If Datset.Tables(0).Rows(0).IsNull("numero_dias_vence") = True Then
                    CTipoDocEntrante.numero_dias_vence = 0
                Else
                    CTipoDocEntrante.numero_dias_vence = Datset.Tables(0).Rows(0).Item("numero_dias_vence")
                End If
                If Datset.Tables(0).Rows(0).IsNull("estado_tipo_documento") = True Then
                    CTipoDocEntrante.estado_tipo_documento = 0
                Else
                    CTipoDocEntrante.estado_tipo_documento = Datset.Tables(0).Rows(0).Item("estado_tipo_documento")
                End If
                If Datset.Tables(0).Rows(0).IsNull("flow_tipo") = True Then
                    CTipoDocEntrante.flow_tipo = 0
                Else
                    CTipoDocEntrante.flow_tipo = Datset.Tables(0).Rows(0).Item("flow_tipo")
                End If
                If Datset.Tables(0).Rows(0).IsNull("requiere_respuesta") = True Then
                    CTipoDocEntrante.requiere_respuesta = 0
                Else
                    CTipoDocEntrante.requiere_respuesta = Datset.Tables(0).Rows(0).Item("requiere_respuesta")
                End If
                If Datset.Tables(0).Rows(0).IsNull("codigo_gabinete_workflow") = True Then
                    CTipoDocEntrante.codigo_gabinete_workflow = 0
                Else
                    CTipoDocEntrante.codigo_gabinete_workflow = Datset.Tables(0).Rows(0).Item("codigo_gabinete_workflow")
                End If
                If Datset.Tables(0).Rows(0).IsNull("nombre_gabinete_workflow") = True Then
                    CTipoDocEntrante.nombre_gabinete_workflow = ""
                Else
                    CTipoDocEntrante.nombre_gabinete_workflow = Datset.Tables(0).Rows(0).Item("nombre_gabinete_workflow")
                End If
                If Datset.Tables(0).Rows(0).IsNull("tipo_activo_pqr") = True Then
                    CTipoDocEntrante.tipo_activo_pqr = 0
                Else
                    CTipoDocEntrante.tipo_activo_pqr = Datset.Tables(0).Rows(0).Item("tipo_activo_pqr")
                End If
                If Datset.Tables(0).Rows(0).IsNull("obliga_respuesta_radicado_tramite") = True Then
                    CTipoDocEntrante.obliga_respuesta_radicado_tramite = 0
                Else
                    CTipoDocEntrante.obliga_respuesta_radicado_tramite = Datset.Tables(0).Rows(0).Item("obliga_respuesta_radicado_tramite")
                End If
                If Datset.Tables(0).Rows(0).IsNull("resp_correo_fisico_electronico") = True Then
                    CTipoDocEntrante.resp_correo_fisico_electronico = 0
                Else
                    CTipoDocEntrante.resp_correo_fisico_electronico = Datset.Tables(0).Rows(0).Item("resp_correo_fisico_electronico")
                End If
                If Datset.Tables(0).Rows(0).IsNull("id_ruta") = True Then
                    CTipoDocEntrante.id_ruta = 0
                Else
                    CTipoDocEntrante.id_ruta = Datset.Tables(0).Rows(0).Item("id_ruta")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_RA_WF_GRUPOS_WORKFLOW_TRAMITE") = True Then
                    CTipoDocEntrante.ID_RA_WF_GRUPOS_WORKFLOW_TRAMITE = 0
                Else
                    CTipoDocEntrante.ID_RA_WF_GRUPOS_WORKFLOW_TRAMITE = Datset.Tables(0).Rows(0).Item("ID_RA_WF_GRUPOS_WORKFLOW_TRAMITE")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ID_WF_FLUJOS_TRABAJO") = True Then
                    CTipoDocEntrante.ID_WF_FLUJOS_TRABAJO = 0
                Else
                    CTipoDocEntrante.ID_WF_FLUJOS_TRABAJO = Datset.Tables(0).Rows(0).Item("ID_WF_FLUJOS_TRABAJO")
                End If
                If Datset.Tables(0).Rows(0).IsNull("tipo_activo_rad_interno") = True Then
                    CTipoDocEntrante.tipo_activo_rad_interno = 0
                Else
                    CTipoDocEntrante.tipo_activo_rad_interno = Datset.Tables(0).Rows(0).Item("tipo_activo_rad_interno")
                End If
                If Datset.Tables(0).Rows(0).IsNull("nombre_tramite") = True Then
                    CTipoDocEntrante.nombre_tramite = ""
                Else
                    CTipoDocEntrante.nombre_tramite = Datset.Tables(0).Rows(0).Item("nombre_tramite")
                End If
                If Datset.Tables(0).Rows(0).IsNull("tipo_registro") = True Then
                    CTipoDocEntrante.tipo_registro = ""
                Else
                    CTipoDocEntrante.tipo_registro = Datset.Tables(0).Rows(0).Item("tipo_registro")
                End If
                If Datset.Tables(0).Rows(0).IsNull("descripcion_tramite") = True Then
                    CTipoDocEntrante.descripcion_tramite = ""
                Else
                    CTipoDocEntrante.descripcion_tramite = Datset.Tables(0).Rows(0).Item("descripcion_tramite")
                End If
                If Datset.Tables(0).Rows(0).IsNull("tipo_tramite") = True Then
                    CTipoDocEntrante.tipo_tramite = 0
                Else
                    CTipoDocEntrante.tipo_tramite = Datset.Tables(0).Rows(0).Item("tipo_tramite")
                End If
                If Datset.Tables(0).Rows(0).IsNull("estado_ruta_open_close") = True Then
                    CTipoDocEntrante.estado_ruta_open_close = 0
                Else
                    CTipoDocEntrante.estado_ruta_open_close = Datset.Tables(0).Rows(0).Item("estado_ruta_open_close")
                End If
                If Datset.Tables(0).Rows(0).IsNull("obliga_exp_radicado") = True Then
                    CTipoDocEntrante.obliga_exp_radicado = 0
                Else
                    CTipoDocEntrante.obliga_exp_radicado = Datset.Tables(0).Rows(0).Item("obliga_exp_radicado")
                End If
                If Datset.Tables(0).Rows(0).IsNull("activo_modulo_respuesta") = True Then
                    CTipoDocEntrante.activo_modulo_respuesta = 0
                Else
                    CTipoDocEntrante.activo_modulo_respuesta = Datset.Tables(0).Rows(0).Item("activo_modulo_respuesta")
                End If
                If Datset.Tables(0).Rows(0).IsNull("util_tipo_modulo_envio") = True Then
                    CTipoDocEntrante.util_tipo_modulo_envio = 0
                Else
                    CTipoDocEntrante.util_tipo_modulo_envio = Datset.Tables(0).Rows(0).Item("util_tipo_modulo_envio")
                End If
                If Datset.Tables(0).Rows(0).IsNull("util_producion_documental") = True Then
                    CTipoDocEntrante.util_producion_documental = 0
                Else
                    CTipoDocEntrante.util_producion_documental = Datset.Tables(0).Rows(0).Item("util_producion_documental")
                End If
                If Datset.Tables(0).Rows(0).IsNull("tipo_tramite_entrante_saliente") = True Then
                    CTipoDocEntrante.tipo_tramite_entrante_saliente = 0
                Else
                    CTipoDocEntrante.tipo_tramite_entrante_saliente = Datset.Tables(0).Rows(0).Item("tipo_tramite_entrante_saliente")
                End If
                If Datset.Tables(0).Rows(0).IsNull("ra_auto_registro_expediente_id_auto_registro") = True Then
                    CTipoDocEntrante.ra_auto_registro_expediente_id_auto_registro = 0
                Else
                    CTipoDocEntrante.ra_auto_registro_expediente_id_auto_registro = Datset.Tables(0).Rows(0).Item("ra_auto_registro_expediente_id_auto_registro")
                End If
                If Datset.Tables(0).Rows(0).IsNull("wf_copia_doc_expediente_actualiza_exped_gabinete") = True Then
                    CTipoDocEntrante.wf_copia_doc_expediente_actualiza_exped_gabinete = 0
                Else
                    CTipoDocEntrante.wf_copia_doc_expediente_actualiza_exped_gabinete = Datset.Tables(0).Rows(0).Item("wf_copia_doc_expediente_actualiza_exped_gabinete")
                End If
                If Datset.Tables(0).Rows(0).IsNull("wf_auto_vincula_doc_expediente_actualiza_exped_gabinete") = True Then
                    CTipoDocEntrante.wf_auto_vincula_doc_expediente_actualiza_exped_gabinete = 0
                Else
                    CTipoDocEntrante.wf_auto_vincula_doc_expediente_actualiza_exped_gabinete = Datset.Tables(0).Rows(0).Item("wf_auto_vincula_doc_expediente_actualiza_exped_gabinete")
                End If
                If Datset.Tables(0).Rows(0).IsNull("wf_copia_doc_expediente_produc_actualiza_exped_gabinete") = True Then
                    CTipoDocEntrante.wf_copia_doc_expediente_produc_actualiza_exped_gabinete = 0
                Else
                    CTipoDocEntrante.wf_copia_doc_expediente_produc_actualiza_exped_gabinete = Datset.Tables(0).Rows(0).Item("wf_copia_doc_expediente_produc_actualiza_exped_gabinete")
                End If
                If Datset.Tables(0).Rows(0).IsNull("util_auto_vincula_migracion") = True Then
                    CTipoDocEntrante.util_auto_vincula_migracion = 0
                Else
                    CTipoDocEntrante.util_auto_vincula_migracion = Datset.Tables(0).Rows(0).Item("util_auto_vincula_migracion")
                End If
                If Datset.Tables(0).Rows(0).IsNull("util_radicacion_simple") = True Then
                    CTipoDocEntrante.util_radicacion_simple = 0
                Else
                    CTipoDocEntrante.util_radicacion_simple = Datset.Tables(0).Rows(0).Item("util_radicacion_simple")
                End If
                If Datset.Tables(0).Rows(0).IsNull("util_nivel_padre_auto_vincula") = True Then
                    CTipoDocEntrante.util_nivel_padre_auto_vincula = 0
                Else
                    CTipoDocEntrante.util_nivel_padre_auto_vincula = Datset.Tables(0).Rows(0).Item("util_nivel_padre_auto_vincula")
                End If
                If Datset.Tables(0).Rows(0).IsNull("util_opcion_auto_vincula") = True Then
                    CTipoDocEntrante.util_opcion_auto_vincula = 0
                Else
                    CTipoDocEntrante.util_opcion_auto_vincula = Datset.Tables(0).Rows(0).Item("util_opcion_auto_vincula")
                End If
                If Datset.Tables(0).Rows(0).IsNull("util_Estado_Crea_ExpedienteSII") = True Then
                    CTipoDocEntrante.util_Estado_Crea_ExpedienteSII = 0
                Else
                    CTipoDocEntrante.util_Estado_Crea_ExpedienteSII = Datset.Tables(0).Rows(0).Item("util_Estado_Crea_ExpedienteSII")
                End If
                If Datset.Tables(0).Rows(0).IsNull("util_Estado_Multiple_expedienteSII") = True Then
                    CTipoDocEntrante.util_Estado_Multiple_expedienteSII = 0
                Else
                    CTipoDocEntrante.util_Estado_Multiple_expedienteSII = Datset.Tables(0).Rows(0).Item("util_Estado_Multiple_expedienteSII")
                End If
                If Datset.Tables(0).Rows(0).IsNull("Id_ser_servicioIntegracion") = True Then
                    CTipoDocEntrante.Id_ser_servicioIntegracion = 0
                Else
                    CTipoDocEntrante.Id_ser_servicioIntegracion = Datset.Tables(0).Rows(0).Item("Id_ser_servicioIntegracion")
                End If
                SolicitaEstructuraTramite = "YES"
            End If
        Catch ex As Exception
            SolicitaEstructuraTramite = "Inconsistencia general funcion SolicitaEstructuraTramite " & ex.Message
        End Try
    End Function
    Function Solicita_opcion_auto_vinculacion(ByVal id_tipo_tramite As Integer,
                                              ByRef util_opcion_auto_vincula As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el estado de la opción de auto vinculación
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_tipo_tramite     : Representa la identificación del tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'util_opcion_auto_vincula  : Retorna el estado de auto vinculcion
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "select util_opcion_auto_vincula from  tipo_doc_entrante " &
                                               " where id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_opcion_auto_vinculacion = "Función Solicita_nivel_padre_vinculacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_opcion_auto_vinculacion = "Imposible encontrar el tramite (" & id_tipo_tramite & ") para ubicar la opción de auto vinculación"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    util_opcion_auto_vincula = 0
                Else
                    util_opcion_auto_vincula = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_opcion_auto_vinculacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_opcion_auto_vinculacion = "Inconsistencia función Solicita_opcion_auto_vinculacion " & ex.Message
        End Try
    End Function
    Function Solicita_nivel_padre_vinculacion(ByVal id_tipo_tramite As Integer,
                                              ByRef id_nivel_padre_auto_vincula As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita le nivel padre de vinculación del expediente
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_tipo_tramite     : Representa la identificación del tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_nivel_padre_auto_vincula  : Retorna la identificación del nivel paadre del expediente
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-11-11
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = "select util_nivel_padre_auto_vincula from  tipo_doc_entrante " &
                                               " where id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nivel_padre_vinculacion = "Función Solicita_nivel_padre_vinculacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nivel_padre_vinculacion = "Imposible encontrar el tramite (" & id_tipo_tramite & ") para ubicar el nivel padre de auto vinculación"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_nivel_padre_auto_vincula = 0
                Else
                    id_nivel_padre_auto_vincula = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_nivel_padre_vinculacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nivel_padre_vinculacion = "Inconsistencia función Solicita_nivel_padre_vinculacion " & ex.Message
        End Try
    End Function
    Function Solicita_lista_tipo_tramite_simple(ByVal id_plantilla As Integer,
                                                ByRef Class_service_ilist_drowlist As List(Of Class_config_general_service.Class_service_ilist_drowlist)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de tramites de radicación simple
        '
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_plantilla               : Representa la identificación de la plantilla
        '                          
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'Class_service_ilist_drowlist : Retorna la lista de gabinetes 
        '                     value: identificación del tipo documento
        '                      text: Nombre del tipo documento
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-10-15
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  id_Tipo_Doc_Entrante,Descripcion_Doc  from  tipo_doc_entrante " &
           " where util_radicacion_simple=1 And system_plantilla_radicado_id_plantilla=" & id_plantilla &
            " order by Descripcion_Doc"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_tipo_tramite_simple = " Función Solicita_lista_tipo_tramite_simple dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Dim item As Class_config_general_service.Class_service_ilist_drowlist = New Class_config_general_service.Class_service_ilist_drowlist()
                item.id_value = "-1"
                item.value_campo = ""
                Class_service_ilist_drowlist.Add(item)
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New Class_config_general_service.Class_service_ilist_drowlist()
                    item.id_value = Datset.Tables(0).Rows(i).Item(0)
                    item.value_campo = Datset.Tables(0).Rows(i).Item(1)
                    Class_service_ilist_drowlist.Add(item)
                Next
                Solicita_lista_tipo_tramite_simple = "YES"
                Exit Function
            Else
                Dim item As Class_config_general_service.Class_service_ilist_drowlist = New Class_config_general_service.Class_service_ilist_drowlist()
                item.id_value = "-1"
                item.value_campo = ""
                Class_service_ilist_drowlist.Add(item)
                Solicita_lista_tipo_tramite_simple = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_tipo_tramite_simple = "Inconistencia general función Solicita_lista_tipo_tramite_simple " & ex.Message
        End Try
    End Function
    Function Solicita_lista_tramite_auto_vinculacion_gabinete(ByVal id_gabinente As Integer,
                                                              ByRef control_drow_lista As List(Of control_drow_lista)) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita lista de tramites de auto vinculación de
        'de documento a expediente relacionada el gabinete seleccionado
        '       
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_gabinente               : Representa la identificación del gabinete
        '---------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------
        'control_drow_lista        : Retorna la lista de gabinetes 
        '                     value: identificación del tipo documento
        '                      text: Nombre del tipo documento
        'error_sistema             : Retorna el resultado del consumo del servicio
        '-----------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------
        'Fecha                 : 2024-08-12
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "select  ra_auto_registro_expediente_id_auto_registro,Descripcion_Doc  from  tipo_doc_entrante " &
            " where util_auto_vincula_migracion=1 And id_gabinete=" & id_gabinente &
             " order by Descripcion_Doc"
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_tramite_auto_vinculacion_gabinete = " Función Solicita_lista_tramite_auto_vinculacion_gabinete dice " & Result
                Exit Function
            End If
            Dim item As control_drow_lista
            If Datset.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    item = New control_drow_lista
                    item.value = Datset.Tables(0).Rows(i).Item(0)
                    item.text = Datset.Tables(0).Rows(i).Item(1)
                    control_drow_lista.Add(item)
                Next
                Solicita_lista_tramite_auto_vinculacion_gabinete = "YES"
                Exit Function
            Else
                Solicita_lista_tramite_auto_vinculacion_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_tramite_auto_vinculacion_gabinete = "Inconsistencia general función Solicita_lista_tramite_auto_vinculacion_gabinete " & ex.Message
        End Try
    End Function
    Function Solicita_tipo_tramite_respuesta(ByVal id_plantilla As Integer,
                                             ByVal tipo_tramite_entrante As String,
                                             ByRef tipo_tramite_homologado_saliente As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Descripcion_Doc from tipo_doc_entrante where system_plantilla_radicado_id_plantilla=" &
                id_plantilla & " And tipo_activo_pqr <> 1 And estado_tipo_documento=1 And tipo_activo_rad_interno <> 1 And tipo_tramite=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            tipo_tramite_homologado_saliente = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipo_tramite_respuesta = "Función Solicita_tipo_tramite_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipo_tramite_respuesta = "Imposible encontrar tramites disponibles de radicación en la plantilla de radicacion (" & id_plantilla & ")"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    If Datset.Tables(0).Rows(i).Item(0) = tipo_tramite_entrante Then
                        tipo_tramite_homologado_saliente = tipo_tramite_entrante
                        Exit For
                    End If
                Next
                If tipo_tramite_homologado_saliente = "" Then
                    tipo_tramite_homologado_saliente = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_tipo_tramite_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipo_tramite_respuesta = "Inconsistencia general función  Solicita_tipo_tramite_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_tipo_envio_respuesta(ByVal id_tipo_tramite As Integer,
                                           ByRef id_tipo_envio_respuesta As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select resp_correo_fisico_electronico from tipo_doc_entrante " &
                " where id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipo_envio_respuesta = "Función Solicita_tipo_envio_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipo_envio_respuesta = "Imposible el tipo de envío del tipo tramite (" & id_tipo_tramite & ")"
                Exit Function
            Else
                id_tipo_envio_respuesta = Datset.Tables(0).Rows(0).Item(0)
                Solicita_tipo_envio_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipo_envio_respuesta = "Inconsistencia general función Solicita_tipo_envio_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_estado_obligatorio_expediente(ByVal id_tipo_tramite As Integer,
                                                    ByRef estado_obliga_expediente As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select obliga_exp_radicado from  tipo_doc_entrante " &
                                               " where id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_obligatorio_expediente = "Función Solicita_estado_obligatorio_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_obligatorio_expediente = "Imposible encontrar el estado del tipo tramite " & id_tipo_tramite
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estado_obliga_expediente = 0
                Else
                    estado_obliga_expediente = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_estado_obligatorio_expediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_obligatorio_expediente = "Inconsistencia función Solicita_estado_obligatorio_expediente " & ex.Message
        End Try
    End Function
    Function Solicita_estado_obligatoria_respuesta_tramite(ByVal id_tipo_tramite As Integer,
                                                           ByRef estado_obliga_resp_radicado As Integer) As String
        Try
            Dim Parametro_Consulta As String = "select obliga_respuesta_radicado_tramite from  tipo_doc_entrante " &
                                               " where id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estado_obligatoria_respuesta_tramite = "Función Retorna_detallle_tramite_obligatoria_respuesta dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estado_obligatoria_respuesta_tramite = "Imposible encontrar el estado del tipo tramite " & id_tipo_tramite
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    estado_obliga_resp_radicado = 0
                Else
                    estado_obliga_resp_radicado = Datset.Tables(0).Rows(0).Item(0)
                End If
                Solicita_estado_obligatoria_respuesta_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estado_obligatoria_respuesta_tramite = "Inconsistencia función Retorna_detallle_tramite_obligatoria_respuesta " & ex.Message
        End Try
    End Function
    Function RetornaIdTipoTramitePorNombreTipo(ByVal nombre_tipo_tramite As String,
                                               ByRef id_tipo_tramite As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita identificación del tramite, por nombre tipo tramite
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'nombre_tipo_tramite : Representa el nombre del tipo tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'id_tipo_tramite  : Retorna la idnetificación del tipo tramite 
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = "Select id_Tipo_Doc_Entrante from tipo_doc_entrante " &
                " where Descripcion_Doc='" & nombre_tipo_tramite & "'"
            Dim Datset_consulta As DataSet = New DataSet("tipo_doc_entrante")
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset_consulta)
            If Result <> "YES" Then
                RetornaIdTipoTramitePorNombreTipo = "Error RetornaIdTipoTramitePorNombreTipo " & Result
                Exit Function
            End If
            If Datset_consulta.Tables(0).Rows.Count = 0 Then
                RetornaIdTipoTramitePorNombreTipo = "YES"
                Exit Function
            Else
                If Datset_consulta.Tables(0).Rows(0).IsNull(0) Then
                    id_tipo_tramite = 0
                Else
                    id_tipo_tramite = Datset_consulta.Tables(0).Rows(0).Item(0)
                End If
                RetornaIdTipoTramitePorNombreTipo = "YES"
                Exit Function
            End If
        Catch ex As Exception
            RetornaIdTipoTramitePorNombreTipo = "Inconsistencia general función RetornaIdTipoTramitePorNombreTipo " & ex.Message
        End Try
    End Function

    Function SolicitaEstadoTramiteRutaAbiertaCerrada(ByVal DescripcionTramite As String,
                                                     ByRef EstadoRutaAbiertaCerrada As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el estado de una tramite si pertence a la ruta abierta o cerrada
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'DescripcionTramite  : Representa la decripcion del tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'EstadoRutaAbiertaCerrada  : Retorna el estado de la ruta
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  estado_ruta_open_close " &
                " from tipo_doc_entrante where Descripcion_Doc='" & DescripcionTramite & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaEstadoTramiteRutaAbiertaCerrada = "Se presentó una inconsistencia en la función SolicitaEstadoTramiteRutaAbiertaCerrada : " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                EstadoRutaAbiertaCerrada = 0
                SolicitaEstadoTramiteRutaAbiertaCerrada = "YES"
                Exit Function
            Else
                EstadoRutaAbiertaCerrada = Datset.Tables(0).Rows(0).Item(0)
                SolicitaEstadoTramiteRutaAbiertaCerrada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaEstadoTramiteRutaAbiertaCerrada = "Inconistencia general función SolicitaEstadoTramiteRutaAbiertaCerrada " & ex.Message
        End Try
    End Function
    Function Solicita_lista_tramite(ByVal id_ruta As Integer,
                                    ByVal tipo_consulta As Integer,
                                    ByVal valor_consulta As String,
                                    ByRef colum_order_name As String,
                                    ByRef order_colum As String,
                                    ByRef grediview As GridView,
                                    ByRef reflabel As Label,
                                    ByRef hideselecion As Object,
                                    ByRef update As UpdatePanel) As String
        Try
            Dim Sql_consulta As String = ""
            If tipo_consulta = 1 Then
                Sql_consulta = "Select law.id_Tipo_Doc_Entrante,law.Descripcion_Doc As NOMBRE_TRAMITE,spr.Nombre_Plantilla_Radicado as NOMBRE_PLANTILLA from tipo_doc_entrante as law " &
                " inner join system_plantilla_radicado as spr on (spr.id_Plantilla=law.system_plantilla_radicado_id_plantilla and spr.Tipo_Plantilla='RADICACION ENTRANTE') " &
                " where   flow_tipo=1 order by   law.Descripcion_Doc, spr.Nombre_Plantilla_Radicado"
            Else
                Sql_consulta = "Select law.id_Tipo_Doc_Entrante,law.Descripcion_Doc As NOMBRE_TRAMITE,spr.Nombre_Plantilla_Radicado as NOMBRE_PLANTILLA from tipo_doc_entrante as law " &
               " inner join system_plantilla_radicado as spr on (spr.id_Plantilla=law.system_plantilla_radicado_id_plantilla and spr.Tipo_Plantilla='RADICACION ENTRANTE') " &
               " where  ( " &
               "  law.Descripcion_Doc like '%" & valor_consulta & "%'" &
               " ) and flow_tipo=1 order by   law.Descripcion_Doc, spr.Nombre_Plantilla_Radicado"
            End If
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_tramite = "Error Solicita_lista_tramite  " & Result
                Exit Function
            End If

            If Datset.Tables(0).Rows.Count = 0 Then
                reflabel.Text = "0 registro (s)"
                grediview.DataSource = Nothing
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                Solicita_lista_tramite = "YES"
                Exit Function
            Else
                reflabel.Text = Datset.Tables(0).Rows.Count & " registro(s) "
                grediview.DataSource = Datset
                hideselecion.value = ""
                grediview.DataBind()
                update.Update()
                For i As Integer = 0 To grediview.Rows.Count - 1
                    grediview.Rows(i).Attributes.Add("id", grediview.Rows(i).Cells(1).Text.ToString())
                    Dim divhtml As New HtmlControls.HtmlGenericControl("div")
                    Dim ihtml As New HtmlControls.HtmlGenericControl("i")
                    ihtml.Style.Add("color", "white")
                    ihtml.Attributes.Add("class", "fad fa-arrow-to-bottom")
                    ihtml.Style.Add("color", "white")
                    Dim ahtml As New HtmlControls.HtmlGenericControl("a")
                    ahtml.Attributes.Add("Class", "btn btn-success btn-sm")
                    ahtml.Attributes.Add("onclick", "prevent(event,this);")
                    ahtml.Attributes.Add("title", "Asignar")
                    ahtml.Attributes.Add("idd", grediview.Rows(i).Cells(1).Text.ToString())
                    ahtml.Attributes.Add("tip_event", "a_s_r_p_333")
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
                Solicita_lista_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_tramite = "Inconsistencia general función Solicita_lista_tramite " & ex.Message
        End Try

    End Function
    Function Solicita_nombre_tipo_tramite_por_id_tramite(ByVal id_tipo_tramite As Integer,
                                                         ByRef nombre_tipo_tramite As String) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim sql_consulta As String = "Select Descripcion_Doc from tipo_doc_entrante " &
                " where id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_tipo_tramite_por_id_tramite = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_tipo_tramite_por_id_tramite = "Imposible encontrar nombre tipo trámite con el id " & id_tipo_tramite
                Exit Function
            Else
                nombre_tipo_tramite = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_tipo_tramite_por_id_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_tipo_tramite_por_id_tramite = "Inconsistencia general función Solicita_nombre_tipo_tramite_por_id_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_tipo_tramite_por_id_tramite(ByVal id_tipo_tramite As Integer,
                                                  ByRef tipo_tramite As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim sql_consulta As String = "Select tipo_tramite_entrante_saliente from tipo_doc_entrante " &
                " where id_Tipo_Doc_Entrante=" & id_tipo_tramite
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_tipo_tramite_por_id_tramite = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                tipo_tramite = 0
                Solicita_tipo_tramite_por_id_tramite = "Imposible encontrar nombre tipo trámite con el id " & id_tipo_tramite
                Exit Function
            Else
                tipo_tramite = Datset.Tables(0).Rows(0).Item(0)
                Solicita_tipo_tramite_por_id_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipo_tramite_por_id_tramite = "Inconsistencia general función Solicita_tipo_tramite_por_id_tramite " & ex.Message
        End Try
    End Function
    Function Determina_gestion_modulo_pqr_Tipo_Tramite(ByVal codigo_plantilla As Integer,
                                                       ByVal tramite As String,
                                                       ByRef estado_modulo_correspo As Integer) As String
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Sql_consulta As String = "Select activo_modulo_respuesta  from " & "tipo_doc_entrante" & " " &
              " WHERE system_plantilla_radicado_id_plantilla = " & codigo_plantilla & " and Descripcion_Doc='" & tramite & "'"
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Determina_gestion_modulo_pqr_Tipo_Tramite = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Determina_gestion_modulo_pqr_Tipo_Tramite = "Imposible detectar el estado modulo respuesta del tipo tramite " & tramite
                Exit Function
            Else
                estado_modulo_correspo = Datset.Tables(0).Rows(0).Item(0)
                Determina_gestion_modulo_pqr_Tipo_Tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Determina_gestion_modulo_pqr_Tipo_Tramite = "Inconsistencia general función Determina_gestion_modulo_pqr_Tipo_Tramite " & ex.Message
        End Try
    End Function
    Function Determina_gestion_modulo_pqr_id_Tipo_Tramite(ByVal codigo_plantilla As Integer,
                                                          ByVal id_tipo_doc As Integer,
                                                          ByRef estado_modulo_correspo As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el estado de gestión para modulo de correspondencia para el tipo de tramite
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'codigo_plantilla    : Representa el codigo de la plantilla de radicación
        'id_tipo_doc         : Representa la identiifcación del tipo de tramite
        'id_plantilla_radicacion : 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'estado_modulo_correspo  : Retorna el estado asignación pqr
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2023-11-15
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim ref As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Sql_consulta As String = "Select activo_modulo_respuesta  from " & "tipo_doc_entrante" & " " &
              " WHERE system_plantilla_radicado_id_plantilla = " & codigo_plantilla & " and id_Tipo_Doc_Entrante=" & id_tipo_doc
            Dim Result As String = ""
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Determina_gestion_modulo_pqr_id_Tipo_Tramite = Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Determina_gestion_modulo_pqr_id_Tipo_Tramite = "Imposible detectar el estado modulo respuesta del id tipo tramite " & id_tipo_doc
                Exit Function
            Else
                estado_modulo_correspo = Datset.Tables(0).Rows(0).Item(0)
                Determina_gestion_modulo_pqr_id_Tipo_Tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Determina_gestion_modulo_pqr_id_Tipo_Tramite = "Inconsistencia general función Determina_gestion_modulo_pqr_id_Tipo_Tramite " & ex.Message
        End Try
    End Function
    Function Lista_tipos_documentales_de_radicacion_interna(ByRef RefCombo As DropDownList,
                                                            ByVal id_plantilla As Integer,
                                                            ByVal descripcion_tramite As String) As String
        Try

            RefCombo.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select * from tipo_doc_entrante where system_plantilla_radicado_id_plantilla=" &
                id_plantilla & " and tipo_activo_pqr <> 1 and estado_tipo_documento=1 and tipo_activo_rad_interno=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_tipos_documentales_de_radicacion_interna = " Error Listando tipos documentales   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_tipos_documentales_de_radicacion_interna = "YES"
                Exit Function
            Else
                RefCombo.Items.Add("SELECCIONE")
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    RefCombo.Items.Add(Datset.Tables(0).Rows(i).Item(1).ToString)
                Next
                For i As Integer = 0 To RefCombo.Items.Count - 1
                    If UCase(descripcion_tramite) = UCase(RefCombo.Items.Item(i).Text) Then
                        RefCombo.Items.Item(i).Text = descripcion_tramite
                        Exit For
                    End If
                Next
                Lista_tipos_documentales_de_radicacion_interna = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_tipos_documentales_de_radicacion_interna = ex.Message
        End Try
    End Function
    Function Lista_tipos_documentales_de_radicacion_interna_item(ByRef drop_list As DropDownList,
                                                                 ByVal id_plantilla As Integer,
                                                                 ByVal descripcion_tramite As String) As String
        Try

            drop_list.Items.Clear()
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select id_Tipo_Doc_Entrante,Descripcion_Doc from tipo_doc_entrante where system_plantilla_radicado_id_plantilla=" &
                id_plantilla & " and tipo_activo_pqr <> 1 and estado_tipo_documento=1 and tipo_activo_rad_interno=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Lista_tipos_documentales_de_radicacion_interna_item = " Error función Lista_tipos_documentales_de_radicacion_interna_item   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Lista_tipos_documentales_de_radicacion_interna_item = "YES"
                Exit Function
            Else
                drop_list.Items.Clear()
                Dim ilist As New ListItem
                ilist.Value = 0
                ilist.Text = ""
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ilist = New ListItem
                    ilist.Value = Datset.Tables(0).Rows(i).Item(0)
                    ilist.Text = Datset.Tables(0).Rows(i).Item(1)
                    drop_list.Items.Add(ilist)
                Next
                For i As Integer = 0 To drop_list.Items.Count - 1
                    If drop_list.Items(i).Text = descripcion_tramite Then
                        drop_list.Items(i).Selected = True
                        Exit For
                    End If
                Next
                Lista_tipos_documentales_de_radicacion_interna_item = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Lista_tipos_documentales_de_radicacion_interna_item = "Inconsistencia general funcion Lista_tipos_documentales_de_radicacion_interna_item " & ex.Message
        End Try
    End Function
    Function retorna_id_tipo_tramite_radicado(ByVal id_plantilla_radicado As Integer,
                                              ByVal descripcion_tramite As String,
                                              ByRef id_tipo_tramite As Integer) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  id_Tipo_Doc_Entrante " &
                " from tipo_doc_entrante where system_plantilla_radicado_id_plantilla=" &
                 id_plantilla_radicado & " and Descripcion_Doc='" & descripcion_tramite & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                retorna_id_tipo_tramite_radicado = "Función retorna_id_tipo_tramite_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                retorna_id_tipo_tramite_radicado = "Imposible encontrar el identificador del tipo trámite (" & descripcion_tramite & ") con " &
                    " el identificador de plantilla (" & id_plantilla_radicado & ")"
                Exit Function
            Else
                id_tipo_tramite = Datset.Tables(0).Rows(0).Item(0)
                retorna_id_tipo_tramite_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            retorna_id_tipo_tramite_radicado = "Inconsistencia general función retorna_id_tipo_tramite_radicado " & ex.Message
        End Try
    End Function
    Function Retorna_id_Flujo_workflow_tramite_id_tramite(ByVal id_tipo_doc_entrante As Integer,
                                                          ByVal option_notifica As Integer,
                                                          ByRef id_flujo_workflow_tramite As Integer) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  ID_WF_FLUJOS_TRABAJO " &
                " from tipo_doc_entrante where id_Tipo_Doc_Entrante=" &
                 id_tipo_doc_entrante
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_Flujo_workflow_tramite_id_tramite = "Función Retorna_id_Flujo_workflow_tramite_id_tramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If option_notifica = 1 Then
                    Retorna_id_Flujo_workflow_tramite_id_tramite = "Imposible encontrar el flujo de trabajo del codigo tramite (" & id_tipo_doc_entrante & ") "
                    Exit Function
                Else
                    id_flujo_workflow_tramite = 0
                    Retorna_id_Flujo_workflow_tramite_id_tramite = "YES"
                    Exit Function
                End If

            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_flujo_workflow_tramite = 0
                    Retorna_id_Flujo_workflow_tramite_id_tramite = "YES"
                    Exit Function
                Else
                    id_flujo_workflow_tramite = Datset.Tables(0).Rows(0).Item(0)
                    Retorna_id_Flujo_workflow_tramite_id_tramite = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Retorna_id_Flujo_workflow_tramite_id_tramite = "Inconsistencia general función Retorna_id_Flujo_workflow_tramite_id_tramite " & ex.Message
        End Try
    End Function
    Function Retorna_tipo_respuesta_tramite_radicado(ByVal id_plantilla_radicado As String,
                                                     ByVal tipo_tramite As String,
                                                     ByRef id_estado As Integer) As String
        Try
            id_estado = 0
            Dim Result As String = ""
            Dim conext As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim Parametro_Consulta As String = "Select requiere_respuesta from tipo_doc_entrante where " &
                " system_plantilla_radicado_id_plantilla=" & id_plantilla_radicado & " and Descripcion_Doc='" &
                tipo_tramite & "'"
            Result = conext.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Retorna_tipo_respuesta_tramite_radicado = "Inconsistencia tratando de determinar el estado de respeuesta " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Retorna_tipo_respuesta_tramite_radicado = "Imposible encontrar el tipo tramite de la plantilla"
                Exit Function
            Else
                id_estado = datset.Tables(0).Rows(0).Item(0)
                Retorna_tipo_respuesta_tramite_radicado = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_tipo_respuesta_tramite_radicado = "Inconsistencia funcion Retorna_tipo_respuesta_tramite_radicado " & ex.Message
        End Try
    End Function
    Function Retorna_estado_respuesta_obligatoria(ByVal id_plantilla_radicado As String,
                                                 ByVal tipo_tramite As String,
                                                 ByRef id_estado As String) As String
        Try
            id_estado = 0
            Dim Result As String = ""
            Dim conext As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim Parametro_Consulta As String = "Select requiere_respuesta from tipo_doc_entrante where " &
                " system_plantilla_radicado_id_plantilla=" & id_plantilla_radicado & " and Descripcion_Doc='" &
                tipo_tramite & "'"
            Result = conext.SELECTION_SELECT_FIELD(Parametro_Consulta, datset)
            If Result <> "YES" Then
                Retorna_estado_respuesta_obligatoria = "Inconsistencia tratando de determinar el estado de respeuesta " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Retorna_estado_respuesta_obligatoria = "YES"
                Exit Function
            Else
                id_estado = datset.Tables(0).Rows(0).Item(0)
                Retorna_estado_respuesta_obligatoria = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_estado_respuesta_obligatoria = "Inconsistencia funcion Retorna_estado_respuesta_obligatoria " & ex.Message
        End Try
    End Function
    Function Determina_Suberadicado_Tipo_Tramite(ByVal codigo_plantilla As Integer,
                                                 ByVal tramite As String,
                                                 ByRef estado_sube_radicado As Integer) As String
        '---------------------------------------------------------------
        'Función : Determina si el documento genera flujo 
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2017-08-16
        '----------------------------------------------------------------
        Try
            Dim Sql_consulta As String = "Select flow_tipo  from " & "tipo_doc_entrante" & " " &
              " WHERE system_plantilla_radicado_id_plantilla = " & codigo_plantilla & " and Descripcion_Doc='" & tramite & "'"
            Dim Result As String = ""
            Dim conext As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Result = conext.SELECTION_SELECT_FIELD(Sql_consulta, datset)
            If Result <> "YES" Then
                Determina_Suberadicado_Tipo_Tramite = "Función  Determina_Suberadicado_Tipo_Tramite dice" & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                estado_sube_radicado = 0
                Determina_Suberadicado_Tipo_Tramite = "YES"
                Exit Function
            Else
                estado_sube_radicado = datset.Tables(0).Rows(0).Item(0)
                Determina_Suberadicado_Tipo_Tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Determina_Suberadicado_Tipo_Tramite = "Inconsistencia general función Determina_Suberadicado_Tipo_Tramite " & ex.Message
        End Try
    End Function
    Function Retorna_id_nombre_gabinete_tipo_tramite(ByVal id_plantilla_radicado As Integer,
                                                     ByVal descripcion_tramite As String,
                                                     ByRef id_gabinete As Integer,
                                                     ByRef nombre_gabinete As String) As String
        Try
            Dim Result As String = ""
            Dim Parametro_Consulta As String = " SELECT  codigo_gabinete_workflow,nombre_gabinete_workflow " &
                " from tipo_doc_entrante where system_plantilla_radicado_id_plantilla=" &
                 id_plantilla_radicado & " and Descripcion_Doc='" & descripcion_tramite & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_id_nombre_gabinete_tipo_tramite = "Función Retorna_id_nombre_gabinete_tipo_tramite dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_id_nombre_gabinete_tipo_tramite = "Imposible encontrar gabinete workflow relacionado al tramite trámite (" & descripcion_tramite & ") con " &
                    " el identificador de plantilla (" & id_plantilla_radicado & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_gabinete = 0
                Else
                    id_gabinete = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    nombre_gabinete = ""
                Else
                    nombre_gabinete = Datset.Tables(0).Rows(0).Item(1)
                End If
                Retorna_id_nombre_gabinete_tipo_tramite = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_id_nombre_gabinete_tipo_tramite = "Inconsistencia general función Retorna_id_nombre_gabinete_tipo_tramite " & ex.Message
        End Try
    End Function
    Function Solicita_tipo_modulo_soporte_documental_envio(ByVal id_tipo_tramite As Integer,
                                                           ByRef tipo_modulo_gestion_envio As Integer) As String
        Try
            Dim Sql_consulta As String = "Select util_tipo_modulo_envio  from " & "tipo_doc_entrante" & " " &
              " WHERE id_Tipo_Doc_Entrante = " & id_tipo_tramite
            Dim Result As String = ""
            Dim conext As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Result = conext.SELECTION_SELECT_FIELD(Sql_consulta, datset)
            If Result <> "YES" Then
                Solicita_tipo_modulo_soporte_documental_envio = "Función  Solicita_tipo_modulo_soporte_documental_envio dice" & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                Solicita_tipo_modulo_soporte_documental_envio = "Imposoble enocontrar el detalle del id tipo tramite (" & id_tipo_tramite & ")"
                Exit Function
            Else
                tipo_modulo_gestion_envio = datset.Tables(0).Rows(0).Item(0)
                Solicita_tipo_modulo_soporte_documental_envio = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_tipo_modulo_soporte_documental_envio = "Inconistencia general funcion Solicita_tipo_modulo_soporte_documental_envio " & ex.Message
        End Try
    End Function
    Function Solicita_identificacion_tipo_documento_entrante_externo_nombre(ByVal nombre_tramite As String,
                                                                            ByRef id_Tipo_Doc_Entrante As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la identificación del tipo documental con el nombre
        '          del tramite y la condicion de tramite externo
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'nombre_tramite        : Representa el nombre del tramite
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_Tipo_Doc_Entrante  : Retorna la idnetificación del tipo de tramite
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-08-05
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim SQL_consulta As String = " SELECT  id_Tipo_Doc_Entrante " &
               " from tipo_doc_entrante where nombre_tramite='" &
                nombre_tramite & "' and tipo_tramite=2"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(SQL_consulta, Datset)
            If Result <> "YES" Then
                Solicita_identificacion_tipo_documento_entrante_externo_nombre = "Función Solicita_identificacion_tipo_documento_entrante_externo_nombre dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_identificacion_tipo_documento_entrante_externo_nombre = "Imposible encontrar la identificación del  tramite (" & nombre_tramite & ") SII en registro de tramites, por favor contacte a su administrador para crear el trámite"
                Exit Function
            Else
                id_Tipo_Doc_Entrante = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_tipo_documento_entrante_externo_nombre = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_tipo_documento_entrante_externo_nombre = "Inconsistencia general funcion Solicita_identificacion_tipo_documento_entrante_externo_nombre " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_gabinete_sii(ByVal nombre_tramite As String,
                                          ByRef gabinete As String) As String
        Try
            Dim Parametro_Consulta As String = " SELECT  nombre_gabinete_workflow " &
               " from tipo_doc_entrante where nombre_tramite='" &
                nombre_tramite & "' and tipo_tramite=2"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_gabinete_sii = "Función Solicita_nombre_gabinete_sii dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_gabinete_sii = "Imposible encontrar el gabinete del  tramite (" & nombre_tramite & ") , por favor registre el tramite"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Solicita_nombre_gabinete_sii = "El  tramite (" & nombre_tramite & ")  no tiene asignado el gabinete"
                    Exit Function
                Else
                    gabinete = Datset.Tables(0).Rows(0).Item(0)
                    Solicita_nombre_gabinete_sii = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_nombre_gabinete_sii = "Inconsistencia general funcion Solicita_nombre_gabinete_sii " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_gabinete_sii(ByVal id_Tipo_Doc_Entrante As Integer,
                                          ByRef gabinete As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el nombre del gabinete workflow solo para tramites externos
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'id_Tipo_Doc_Entrante  : Representa la identificación del tramite
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'gabinete  : Retorna el nombre del gabinete
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2024-12-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Parametro_Consulta As String = " SELECT  nombre_gabinete_workflow " &
               " from tipo_doc_entrante where id_Tipo_Doc_Entrante=" &
                id_Tipo_Doc_Entrante & " and tipo_tramite=2"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_gabinete_sii = "Función Solicita_nombre_gabinete_sii dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_gabinete_sii = "Imposible encontrar el gabinete del  tramite (" & id_Tipo_Doc_Entrante & ") , por favor registre el tramite"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Solicita_nombre_gabinete_sii = "El  tramite (" & id_Tipo_Doc_Entrante & ")  no tiene asignado el gabinete"
                    Exit Function
                Else
                    gabinete = Datset.Tables(0).Rows(0).Item(0)
                    Solicita_nombre_gabinete_sii = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_nombre_gabinete_sii = "Inconsistencia general funcion Solicita_nombre_gabinete_sii " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_gabinete_tramite(ByVal nombre_tramite As String,
                                              ByVal id_plantilla_radicado As Integer,
                                              ByRef gabinete As String) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita el gabinete relacionado a un tipo de tramite de una
        '          plantilla de radicación
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'nombre_tramite        : Representa el nombre del tramite
        'id_plantilla_radicado : Representa la identificación de la plantilla
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'gabinete              : Retorna el nombre del gabinete
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-07-21
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " SELECT  nombre_gabinete_workflow " &
               " from tipo_doc_entrante where Descripcion_Doc='" &
                nombre_tramite & "' and system_plantilla_radicado_id_plantilla=" & id_plantilla_radicado
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_gabinete_tramite = "Función Solicita_nombre_gabinete_sii dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_gabinete_tramite = "Imposible encontrar el gabinete del  tramite (" & nombre_tramite & ") , por favor registre el tramite"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    Solicita_nombre_gabinete_tramite = "El  tramite (" & nombre_tramite & ")  no tiene asignado el gabinete"
                    Exit Function
                Else
                    gabinete = Datset.Tables(0).Rows(0).Item(0)
                    Solicita_nombre_gabinete_tramite = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_nombre_gabinete_tramite = "Inconsistencia general funcion Solicita_nombre_gabinete_tramite " & ex.Message
        End Try
    End Function
    Function SolicitaidAutoRegistroExpediente(ByVal IdTipoDocumento As Integer,
                                              ByRef IdAutoRegistro As Integer) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita el codigo de auto registro de expediente relacionado al tramite
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTipoDocumento     : Representa la identificación del tipo tramite
        'campo_radicacion    : Representa el nombre del campo de radicación destino
        'id_plantilla_radicacion : 
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdAutoRegistro  : Retorna la identificación del auto registro
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2022-06-13
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " SELECT  ra_auto_registro_expediente_id_auto_registro " &
             " from tipo_doc_entrante where id_Tipo_Doc_Entrante=" &
             IdTipoDocumento
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaidAutoRegistroExpediente = "Función Solicita_id_auto_registro_expediente dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaidAutoRegistroExpediente = "Imposible encontrar  registro del tipo tramite (" & IdTipoDocumento & ") , por favor revice el codigo del tramite"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    IdAutoRegistro = 0
                Else
                    IdAutoRegistro = Datset.Tables(0).Rows(0).Item(0)
                End If
                SolicitaidAutoRegistroExpediente = "YES"
                Exit Function
            End If
        Catch ex As Exception
            SolicitaidAutoRegistroExpediente = "Inconsistencia general función SolicitaidAutoRegistroExpediente " & ex.Message
        End Try
    End Function
    Function Solicita_tramite_default_digitalizacion(ByRef id_tramite As Integer) As String
        '----------------------------------------------------------------------------------
        '----------------------------------------------------------------------------------
        'Funcion : Solicita el tramite con el perfil default para digitalización de 
        'documentos
        '---------------------------------------------------------------------------------------------------------------------------------
        'Restorno 
        '----------------------------------------------------------------------------------------------------------------------------------
        '-------------------
        'id_tramite        : Gauarda la identificación del tramite defaul
        '-------------------
        '---------------------------------------------------------------------------------------------------------------------------------
        'Restorno 
        '----------------------------------------------------------------------------------------------------------------------------------
        '-------------------
        'Parametro         :El tramite default siempre tiene este nombre    "default_digitalizacion"
        '-------------------
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------
        'Fecha     : 2022-08-16
        '-----------
        '---------------------------------------------------------------------------------------------------------------------------------
        Try

            Dim Parametro_Consulta As String = " SELECT  id_Tipo_Doc_Entrante " &
             " from tipo_doc_entrante where Descripcion_Doc='" &
             "default_digitalizacion" & "'"
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_tramite_default_digitalizacion = "Función Solicita_tramite_default_digitalizacion dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_tramite = -1
                Solicita_tramite_default_digitalizacion = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    id_tramite = -1
                    Solicita_tramite_default_digitalizacion = "YES"
                    Exit Function
                Else
                    id_tramite = Datset.Tables(0).Rows(0).Item(0)
                    Solicita_tramite_default_digitalizacion = "YES"
                    Exit Function
                End If
            End If
        Catch ex As Exception
            Solicita_tramite_default_digitalizacion = "Inconsistencia general funcion Solicita_tramite_default_digitalizacion " & ex.Message
        End Try
    End Function
    Function Solicita_permiso_actualiza_expediente_indice_gabinete(ByVal nombre_tramite As String,
                                                                   ByRef wf_copia_doc_expediente_actualiza_exped_gabinete As Integer,
                                                                   ByRef wf_auto_vincula_doc_expediente_actualiza_exped_gabinete As Integer,
                                                                   ByRef wf_copia_doc_expediente_produc_actualiza_exped_gabinete As Integer) As String
        '-----------------------------------------------------------------------------------
        'Funcion : Solicita permisos del tramite para la actualización del expediente a 
        '          indice de documento en el gabbinete
        '-----------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------
        'nombre_tramite             : Representa el nombre del tramite
        '-----------------------------------------------------------------------------------
        '                           RETORNO
        '-------------------------------------------------------------------------------------
        'wf_copia_doc_expediente         : Permiso actualiza indice gabinete expediente 
        '                                  copia documento a expediente desde workflow
        'wf_auto_vincula_doc_expediente  : Permiso actualiza indice gabinete expediente 
        '                                  en la auto vinculacion a expediente desde workflow
        'wf_copia_doc_expediente_produc  : Permiso actualiza indice gabinete expediente
        '                                  en la copia de documento a expdiente produccion
        '-------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-------------------------------------------------------------------------------------
        'Fecha                 : 2023-06-06
        'Elabora               : Miguel Angel Urueta Miranda
        '-----------------------------------------------------------------------------------
        Try
            Dim sql_consulta As String = "Select wf_copia_doc_expediente_actualiza_exped_gabinete," &
                "wf_auto_vincula_doc_expediente_actualiza_exped_gabinete,wf_copia_doc_expediente_produc_actualiza_exped_gabinete " &
                " from tipo_doc_entrante where Descripcion_Doc ='" & nombre_tramite & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql_DA
            Dim Datset As DataSet = New DataSet("tipo_doc_entrante")
            Result = ref.SELECTION_SELECT_FIELDA(sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_permiso_actualiza_expediente_indice_gabinete = "Error funcion Solicita_permiso_actualiza_expediente_indice_gabinete  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_permiso_actualiza_expediente_indice_gabinete = "Imposible encontrar los permisos de actualizacion de indice expediente gabinete del tipo tramite (" & nombre_tramite & ")"
                Exit Function
            Else
                wf_copia_doc_expediente_actualiza_exped_gabinete = Datset.Tables(0).Rows(0).Item(0)
                wf_auto_vincula_doc_expediente_actualiza_exped_gabinete = Datset.Tables(0).Rows(0).Item(1)
                wf_copia_doc_expediente_produc_actualiza_exped_gabinete = Datset.Tables(0).Rows(0).Item(2)
                Solicita_permiso_actualiza_expediente_indice_gabinete = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_permiso_actualiza_expediente_indice_gabinete = "Incosistencia general funcion Solicita_permiso_actualiza_expediente_indice_gabinete " & ex.Message
        End Try
    End Function
End Class
