Public Class Class_remit_dest_interno_perfil_produccion
    Function AsignaPermisosPerfilUsuarioGestion(ByVal IdUsuarioGestion As Integer) As String
        '**********************************************************************
        'Funcion : asigna permisos perfil usuario gestion documental
        'Ing : Miguel Angel Urueta Miranda
        'Fecha : 2014-09-24
        '**********************************************************************
        Try
            Dim Parametro_Consulta As String = "select * from remit_dest_interno_perfil_produccion where" &
            " remit_dest_interno_idremit_dest_interno =" & IdUsuarioGestion
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Dat_reader As DataSet = New DataSet("remit_dest_interno_perfil_produccion")
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Dat_reader Is Nothing Then
                AsignaPermisosPerfilUsuarioGestion = " Imposible conectar la base de datos del gestor documental " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                HttpContext.Current.Session.Item("GA_IDUSUARIOGESTION") = IdUsuarioGestion
                HttpContext.Current.Session("GA_Manager_Produccion") = Dat_reader.Tables(0).Rows(0).Item("Manager_Produccion")
                HttpContext.Current.Session("GA_Generar_Documento") = Dat_reader.Tables(0).Rows(0).Item("Generar_Documento")
                HttpContext.Current.Session("GA_Anular_documento") = Dat_reader.Tables(0).Rows(0).Item("Anular_documento")
                HttpContext.Current.Session("GA_Eliminar_documento") = Dat_reader.Tables(0).Rows(0).Item("Eliminar_documento")
                HttpContext.Current.Session("GA_Almacenar_Documento") = Dat_reader.Tables(0).Rows(0).Item("Almacenar_Documento")
                HttpContext.Current.Session("GA_Radicar_enviar_documento") = Dat_reader.Tables(0).Rows(0).Item("Radicar_enviar_documento")
                HttpContext.Current.Session("GA_MANAGER_CONFIGURACION") = Dat_reader.Tables(0).Rows(0).Item("MANAGER_CONFIGURACION")
                HttpContext.Current.Session("GA_MANAGER_GESTION") = Dat_reader.Tables(0).Rows(0).Item("MANAGER_GESTION")
                HttpContext.Current.Session("GA_REGISTRA_UNIDAD_CONSERVACION") = Dat_reader.Tables(0).Rows(0).Item("REGISTRA_UNIDAD_CONSERVACION")
                HttpContext.Current.Session("GA_EDITA_UNIDAD_CONSERVACION") = Dat_reader.Tables(0).Rows(0).Item("EDITA_UNIDAD_CONSERVACION")
                HttpContext.Current.Session("GA_ELIMINA_UNIDAD_CONSERVACION") = Dat_reader.Tables(0).Rows(0).Item("ELIMINA_UNIDAD_CONSERVACION")
                HttpContext.Current.Session("GA_ARCHIVA_UNIDAD_CONSERVACION") = Dat_reader.Tables(0).Rows(0).Item("ARCHIVA_UNIDAD_CONSERVACION")
                HttpContext.Current.Session("GA_APLICATRD_UNIDAD_CONSERVACION") = Dat_reader.Tables(0).Rows(0).Item("APLICATRD_UNIDAD_CONSERVACION")
                HttpContext.Current.Session("GA_TRANSLADO_UNIDAD_CONSERVACION") = Dat_reader.Tables(0).Rows(0).Item("TRANSLADO_UNIDAD_CONSERVACION")
                HttpContext.Current.Session("GA_REGISTRA_EXPEDIENTES") = Dat_reader.Tables(0).Rows(0).Item("REGISTRA_EXPEDIENTES")
                HttpContext.Current.Session("GA_EDITA_EXPEDIENTES") = Dat_reader.Tables(0).Rows(0).Item("EDITA_EXPEDIENTES")
                HttpContext.Current.Session("GA_ELIMINA_EXPEDIENTES") = Dat_reader.Tables(0).Rows(0).Item("ELIMINA_EXPEDIENTES")
                HttpContext.Current.Session("GA_ARCHIVA_EXPEDIENTES") = Dat_reader.Tables(0).Rows(0).Item("ARCHIVA_EXPEDIENTES")
                HttpContext.Current.Session("GA_APLICATRD_EXPEDIENTES") = Dat_reader.Tables(0).Rows(0).Item("APLICATRD_EXPEDIENTES")
                HttpContext.Current.Session("GA_TRANSLADO_EXPEDIENTES") = Dat_reader.Tables(0).Rows(0).Item("TRANSLADO_EXPEDIENTES")
                HttpContext.Current.Session("GA_REGISTRA_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("REGISTRA_DOCUMENTOS")
                HttpContext.Current.Session("GA_EDITA_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("EDITA_DOCUMENTOS")
                HttpContext.Current.Session("GA_ELIMINA_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("ELIMINA_DOCUMENTOS")
                HttpContext.Current.Session("GA_ARCHIVA_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("ARCHIVA_DOCUMENTOS")
                HttpContext.Current.Session("GA_APLICATRD_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("APLICATRD_DOCUMENTOS")
                HttpContext.Current.Session("GA_TRANSLADO_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("TRANSLADO_DOCUMENTOS")
                HttpContext.Current.Session("GA_PRESTAMO_ARCHIVO") = Dat_reader.Tables(0).Rows(0).Item("PRESTAMO_ARCHIVO")
                HttpContext.Current.Session("GA_CLASIFICA_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("CLASIFICA_DOCUMENTOS")
                HttpContext.Current.Session("GA_ASIGNA_UNIDAD_CONSERVACION_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("ASIGNA_UNIDAD_CONSERVACION_DOCUMENTOS")
                HttpContext.Current.Session("GA_ASIGNA_EXPEDIENTE_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("ASIGNA_EXPEDIENTE_DOCUMENTOS")
                HttpContext.Current.Session("GA_SELECCIONA_CLASE_DOCUMENTOS") = Dat_reader.Tables(0).Rows(0).Item("SELECCIONA_CLASE_DOCUMENTOS")
                HttpContext.Current.Session("GA_CLASIFICA_UNIDAD_CONSERVACION") = Dat_reader.Tables(0).Rows(0).Item("CLASIFICA_UNIDAD_CONSERVACION")
                HttpContext.Current.Session("GA_CLASIFICA_EXPEDIENTES") = Dat_reader.Tables(0).Rows(0).Item("CLASIFICA_EXPEDIENTES")
                HttpContext.Current.Session("GA_ADMINISTRACION_ORGANICA") = Dat_reader.Tables(0).Rows(0).Item("ADMINISTRACION_ORGANICA")
                HttpContext.Current.Session("GA_ADMINISTRACION_TRD") = Dat_reader.Tables(0).Rows(0).Item("ADMINISTRACION_TRD")
                HttpContext.Current.Session("GA_ADMINISTRACION_TVD") = Dat_reader.Tables(0).Rows(0).Item("ADMINISTRACION_TVD")
                HttpContext.Current.Session("GA_ADMINISTRACION_CCD") = Dat_reader.Tables(0).Rows(0).Item("ADMINISTRACION_CCD")
                HttpContext.Current.Session("GA_ADMINISTRACION_ESTRUCTURA_ARCHIVO") = Dat_reader.Tables(0).Rows(0).Item("ADMINISTRACION_ESTRUCTURA_ARCHIVO")
                HttpContext.Current.Session("PRODUCCION_MANAGER") = Dat_reader.Tables(0).Rows(0).Item("PRODUCCION_MANAGER")
                HttpContext.Current.Session("GESTION_EXPEDIENTE") = Dat_reader.Tables(0).Rows(0).Item("GESTION_EXPEDIENTE")
                HttpContext.Current.Session("GESTION_FISICA") = Dat_reader.Tables(0).Rows(0).Item("GESTION_FISICA")
                HttpContext.Current.Session("GESTION_UNIDAD_CONSERVACION") = Dat_reader.Tables(0).Rows(0).Item("GESTION_UNIDAD_CONSERVACION")
                HttpContext.Current.Session("CONSULTA_EXPEDIENTE") = Dat_reader.Tables(0).Rows(0).Item("CONSULTA_EXPEDIENTE")
                HttpContext.Current.Session("GA_ADMINISTRACION_INSTRUMENTO") = Dat_reader.Tables(0).Rows(0).Item("ADMINISTRACION_INSTRUMENTO")
                HttpContext.Current.Session("GA_CONSULTA_TABLA_RETENCION") = Dat_reader.Tables(0).Rows(0).Item("CONSULTA_TABLA_RETENCION")
                HttpContext.Current.Session("GA_CONSULTA_CUADRO_CLASIFICACION") = Dat_reader.Tables(0).Rows(0).Item("CONSULTA_CUADRO_CLASIFICACION")
                HttpContext.Current.Session("FIRMA_DIGITAL_DOCUMENTO_GD") = Dat_reader.Tables(0).Rows(0).Item("FIRMA_DIGITAL_DOCUMENTO_GD")
                HttpContext.Current.Session("Radicar_enviar_documento_master_interno") = Dat_reader.Tables(0).Rows(0).Item("Radicar_enviar_documento_master_interno")
                HttpContext.Current.Session("UTIL_VISOR_EXPRESS_PRODUCION") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VISOR_EXPRESS_PRODUCION")
                HttpContext.Current.Session("UTIL_VISOR_EXPRESS_APROBACION") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VISOR_EXPRESS_APROBACION")
                HttpContext.Current.Session("UTIL_VISOR_EXPRESS_EXPEDIENTE") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VISOR_EXPRESS_EXPEDIENTE")
                HttpContext.Current.Session("UTIL_VISOR_EXPRESS_CONSULTAS") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VISOR_EXPRESS_CONSULTAS")
                HttpContext.Current.Session("UTIL_VISOR_EXPRESS_DOCUARCHI") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VISOR_EXPRESS_DOCUARCHI")
                HttpContext.Current.Session("UTIL_MODULO_CONSULTA_MIGRA_FORMATO_ARCHIVO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_MODULO_CONSULTA_MIGRA_FORMATO_ARCHIVO")
                HttpContext.Current.Session("UTIL_MODULO_MIGRA_FORMATO_ARCHIVO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_MODULO_MIGRA_FORMATO_ARCHIVO")
                HttpContext.Current.Session("UTIL_MIGRA_FORMATO_ARCHIVO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_MIGRA_FORMATO_ARCHIVO")
                HttpContext.Current.Session("UTIL_MIGRA_LOAD_FORMATO_ARCHIVO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_MIGRA_LOAD_FORMATO_ARCHIVO")
                HttpContext.Current.Session("UTIL_MIGRA_REMPLAZA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_MIGRA_REMPLAZA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_MIG_RESTAURA_VERSION_DOCUMENTO_GABINETE") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_MIG_RESTAURA_VERSION_DOCUMENTO_GABINETE")
                HttpContext.Current.Session("UTIL_VER_PR_RESTAURA_VERSION_DOCUMENTO_GABINETE") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_PR_RESTAURA_VERSION_DOCUMENTO_GABINETE")
                HttpContext.Current.Session("UTIL_VER_DA_RESTAURA_VERSION_DOCUMENTO_GABINETE") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_DA_RESTAURA_VERSION_DOCUMENTO_GABINETE")
                HttpContext.Current.Session("UTIL_VER_RA_RESTAURA_VERSION_DOCUMENTO_GABINETE") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_RA_RESTAURA_VERSION_DOCUMENTO_GABINETE")
                HttpContext.Current.Session("UTIL_VER_COR_RESTAURA_VERSION_DOCUMENTO_GABINETE") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_COR_RESTAURA_VERSION_DOCUMENTO_GABINETE")
                HttpContext.Current.Session("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_MASTER_RESTAURA_VERSION_DOCUMENTO_GABINETE")
                HttpContext.Current.Session("UTIL_VER_MIG_ELIMINA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_MIG_ELIMINA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_PR_ELIMINA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_PR_ELIMINA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_DA_ELIMINA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_DA_ELIMINA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_RA_ELIMINA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_RA_ELIMINA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_COR_ELIMINA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_COR_ELIMINA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_CON_MIGRA_ELIMINA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_CON_MIGRA_ELIMINA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_MASTER_ELIMINA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_MIG_REMPLAZA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_MIG_REMPLAZA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_PR_REMPLAZA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_PR_REMPLAZA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_DA_REMPLAZA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_DA_REMPLAZA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_RA_REMPLAZA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_RA_REMPLAZA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_COR_REMPLAZA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_COR_REMPLAZA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO") = Dat_reader.Tables(0).Rows(0).Item("UTIL_VER_MASTER_REMPLAZA_VERSION_DOCUMENTO")
                HttpContext.Current.Session("UTIL_MIG_AUTO_VINCULA_DOC_EXPEDIENTE") = Dat_reader.Tables(0).Rows(0).Item("UTIL_MIG_AUTO_VINCULA_DOC_EXPEDIENTE")
                HttpContext.Current.Session("UTIL_MIGRA_UPDATE_TIPOLOGIA") = Dat_reader.Tables(0).Rows(0).Item("UTIL_MIGRA_UPDATE_TIPOLOGIA")
                HttpContext.Current.Session("UTIL_MIGRA_UPDATE_INDICE_BATCH") = Dat_reader.Tables(0).Rows(0).Item("UTIL_MIGRA_UPDATE_INDICE_BATCH")
                HttpContext.Current.Session("UTILGCOROptionHCarchivaTramite") = Dat_reader.Tables(0).Rows(0).Item("UTILGCOROptionHCarchivaTramite")
                For i As Integer = 0 To Dat_reader.Tables(0).Columns.Count - 1
                    HttpContext.Current.Session.Item("DETALLE_SESION") = HttpContext.Current.Session.Item("DETALLE_SESION") & Dat_reader.Tables(0).Columns(i).ColumnName & "|" & Dat_reader.Tables(0).Rows(0).Item(i) & "||"
                Next
                AsignaPermisosPerfilUsuarioGestion = "YES"
                Exit Function
            Else
                AsignaPermisosPerfilUsuarioGestion = "Imposible asignar el perfil al usario de gestión por favor perfile el usuario " & IdUsuarioGestion.ToString
                Exit Function
            End If
        Catch ex As Exception
            AsignaPermisosPerfilUsuarioGestion = "Inconsistencia funcion  AsignaPermisosPerfilUsuarioGestion = " & ex.Message
        End Try
    End Function
End Class
