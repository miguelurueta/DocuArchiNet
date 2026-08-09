Public Structure stru_opciones_tiempo_autorizacion
    Dim util_evalua_periodo_general As Integer
    Dim utill_evalua_periodo_pqr As Integer
    Dim util_evalua_periodo_interno As Integer
    Dim util_evalua_festivo As Integer
End Structure
Public Class Class_system_plantilla_radicado_opciones
    Property util_fecha_vencimiento As Integer
    Property util_cita_respuesta As Integer
    Property Util_Radic_General As Integer
    Property Valida_dest_Ext As Integer
    Property util_codigo_corto_radicado As Integer
    Property Util_activo_plantilla_codigo_simple As Integer
    Property util_estado_pendiente_rad As Integer
    Property Error_result As String
End Class
Public Class class_system_plantilla_defaul_simplificada
    Public Property id_Plantilla As Integer
    Public Property Nombre_Plantilla_Radicado As String
    Public Property Tipo_Plantilla As String
    Public Property id_tipo_plantilla As Integer
    Public Property util_estado_pendiente_rad As Integer
    Public Property error_gestion As String
End Class
Public Class Class_system_plantilla_radicado
    Function Solicita_estructura_plantilla_radicacion_default_simplificada(ByRef Class_system_plantilla_defaul_simplificada As class_system_plantilla_defaul_simplificada) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la estructura de la plantilla radicación default
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        '
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'Class_system_plantilla_defaul_simplificada  : Retorna la estructura de la
        'plantilla de radicación por default simplificada
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2024-10-13
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select id_Plantilla,Nombre_Plantilla_Radicado,Tipo_Plantilla,util_estado_pendiente_rad from system_plantilla_radicado where util_default_simple=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_estructura_plantilla_radicacion_default_simplificada = " Error Solicita_estructura_plnatilla_radicacion_default_simplificada  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_estructura_plantilla_radicacion_default_simplificada = "Imposible encontrar el nombre de la plantilla default para radicación simplificada"
                Exit Function
            Else
                Class_system_plantilla_defaul_simplificada.id_Plantilla = Datset.Tables(0).Rows(0).Item(0)
                Class_system_plantilla_defaul_simplificada.Nombre_Plantilla_Radicado = Datset.Tables(0).Rows(0).Item(1).ToString
                Class_system_plantilla_defaul_simplificada.Tipo_Plantilla = Datset.Tables(0).Rows(0).Item(2).ToString
                Class_system_plantilla_defaul_simplificada.util_estado_pendiente_rad = Datset.Tables(0).Rows(0).Item(3)
                If Class_system_plantilla_defaul_simplificada.Tipo_Plantilla = "RADICACION ENTRANTE" Then
                    Class_system_plantilla_defaul_simplificada.id_tipo_plantilla = 1
                Else
                    Class_system_plantilla_defaul_simplificada.id_tipo_plantilla = 2
                End If
                Solicita_estructura_plantilla_radicacion_default_simplificada = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_estructura_plantilla_radicacion_default_simplificada = "Inconistencia general función  Solicita_estructura_plantilla_radicacion_default_simplificada " & ex.Message
        End Try
    End Function
    Function Solicita_plantilla_default_respuesta(ByRef IdPlantilla As Integer,
                                                  ByRef NombrePlantilla As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select id_Plantilla,Nombre_Plantilla_Radicado from system_plantilla_radicado where util_default_respuesta=1"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_plantilla_default_respuesta = " Error Solicita_plantilla_default_respuesta  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_plantilla_default_respuesta = "Imposible encontrar el nombre de la plantilla default respuesta"
                Exit Function
            Else
                IdPlantilla = Datset.Tables(0).Rows(0).Item(0)
                NombrePlantilla = Datset.Tables(0).Rows(0).Item(1).ToString
                Solicita_plantilla_default_respuesta = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_plantilla_default_respuesta = "Función Solicita_plantilla_default_respuesta " & ex.Message
        End Try
    End Function
    Function Solicita_nombre_plantilla_radicado(ByVal id_plantilla As Integer,
                                                ByRef nombre_plantilla As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select Nombre_Plantilla_Radicado from system_plantilla_radicado where id_Plantilla=" & id_plantilla
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_plantilla_radicado = " Error funcion Solicita_nombre_plantilla_radicado  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_nombre_plantilla_radicado = "Imposible encontrar el nombre de la plantilla con el id (" & id_plantilla & ")"
                Exit Function
            Else
                nombre_plantilla = Datset.Tables(0).Rows(0).Item(0)
                Solicita_nombre_plantilla_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_plantilla_radicado = "Inconsistencia general función Solicita_nombre_plantilla_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_id_tipo_tramite_plantilla_radicado(ByVal radicado As String,
                                                         ByVal nombre_plantilla As String,
                                                         ByRef id_tipo_tramite As Integer) As String
        Try
            Dim Parametro_Consulta As String = "SELECT rrr.tipo_doc_entrante_id_tipo_doc_entrante " &
                   " FROM " & nombre_plantilla & " as rrr " &
                    " where  Consecutivo_Rad='" & radicado & "'"
            Dim Result As String = ""
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet(nombre_plantilla)
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_id_tipo_tramite_plantilla_radicado = "Función Solicita_id_tipo_tramite_plantilla_radicado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_id_tipo_tramite_plantilla_radicado = "Imposible econtrar el estado del tipo documento radicado  " & radicado & " de la plantilla " & nombre_plantilla
                Exit Function
            Else
                id_tipo_tramite = Datset.Tables(0).Rows(0).Item(0)
                Solicita_id_tipo_tramite_plantilla_radicado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_id_tipo_tramite_plantilla_radicado = "Inconsistencia general función Solicita_id_tipo_tramite_plantilla_radicado " & ex.Message
        End Try
    End Function
    Function Solicita_opciones_tiempos_radicacion(ByVal id_plantilla As Integer,
                                                  ByRef stru_opciones As stru_opciones_tiempo_autorizacion) As String
        '---------------------------------------------------------
        'Función : Lista las opciones de evaluación de los tiempos
        'permitidos en las plantillas de radicación
        'Fecha : 2018-09-18
        'Ingeniero : Miguel Angel Urueta Miranda
        '----------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select util_evalua_periodo_general,utill_evalua_periodo_pqr,util_evalua_periodo_interno," &
                "util_evalua_festivo from system_plantilla_radicado where id_Plantilla=" & id_plantilla
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_opciones_tiempos_radicacion = " Error Listando opciones tiempos de radicación   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_opciones_tiempos_radicacion = "YES"
                Exit Function
            Else
                stru_opciones.util_evalua_periodo_general = Datset.Tables(0).Rows(0).Item(0)
                stru_opciones.utill_evalua_periodo_pqr = Datset.Tables(0).Rows(0).Item(1)
                stru_opciones.util_evalua_periodo_interno = Datset.Tables(0).Rows(0).Item(2)
                stru_opciones.util_evalua_festivo = Datset.Tables(0).Rows(0).Item(3)
                Solicita_opciones_tiempos_radicacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_opciones_tiempos_radicacion = "Inconsistencia general función Solicita_opciones_tiempos_radicacion " & ex.Message
        End Try
    End Function
    Function Lista_Opcion_Plantilla_Radicacion(ByVal codigo_plantilla_radicacion As Integer,
                                              ByRef Estado_opcion_fecha As Integer,
                                              ByRef Estado_opcion_cita_respuesta As Integer,
                                              ByRef Estado_opcion_radicado_general As Integer,
                                              Optional valida_externo As Integer = -1,
                                              Optional ByRef codigo_corto As Integer = 0,
                                              Optional ByRef Util_activo_plantilla_codigo_simple As Integer = 0) As String
        '*****************************************************************
        'Funcion : Lista las opcion de la plantilla de radicacion
        'Ingeniero : Miguel angel Urueta Miranda
        'Fecha : 2014-08-09
        '*****************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select util_fecha_vencimiento,util_cita_respuesta,Util_Radic_General,Valida_dest_Ext," &
            "util_codigo_corto_radicado,Util_activo_plantilla_codigo_simple from system_plantilla_radicado where " &
            " id_Plantilla=" & codigo_plantilla_radicacion
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Lista_Opcion_Plantilla_Radicacion = " Error Listando opciones plantilla radicacion   " & result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                Estado_opcion_fecha = Dat_reader.Tables(0).Rows(0).Item(0)
                Estado_opcion_cita_respuesta = Dat_reader.Tables(0).Rows(0).Item(1)
                Estado_opcion_radicado_general = Dat_reader.Tables(0).Rows(0).Item(2)
                valida_externo = Dat_reader.Tables(0).Rows(0).Item(3)
                codigo_corto = Dat_reader.Tables(0).Rows(0).Item(4)
                Util_activo_plantilla_codigo_simple = Dat_reader.Tables(0).Rows(0).Item(5)
                Lista_Opcion_Plantilla_Radicacion = "YES"
                Exit Function
            Else
                Lista_Opcion_Plantilla_Radicacion = "Imposible encontrar las opciones de lA plantilla de radicación  de codigo (" & codigo_plantilla_radicacion & ")"
                Exit Function
            End If
        Catch ex As Exception
            Lista_Opcion_Plantilla_Radicacion = "Inconsistencia general funcion Lista_Opcion_Fecha_Vence_Plantilla_Radicacion " & ex.Message
        End Try
    End Function
    Function Solicita_Opcion_Plantilla_Radicacion(ByVal codigo_plantilla_radicacion As Integer,
                                                  ByRef ilist_opciones_plantilla As Class_system_plantilla_radicado_opciones) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita las opciones de la plantilla de radicación
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'codigo_plantilla_radicacion : Representa el id de plantilla del radicado
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'ilist_opciones_plantilla    : Retorna opciones plantilla
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-09-07
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "Select util_fecha_vencimiento,util_cita_respuesta,Util_Radic_General,Valida_dest_Ext," &
            "util_codigo_corto_radicado,Util_activo_plantilla_codigo_simple,util_estado_pendiente_rad from system_plantilla_radicado where " &
            " id_Plantilla=" & codigo_plantilla_radicacion
            Dim Dat_reader As New DataSet
            Dim result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If result <> "YES" Then
                Solicita_Opcion_Plantilla_Radicacion = " Error Solicita_Opcion_Plantilla_Radicacion   " & result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count > 0 Then
                ilist_opciones_plantilla.util_fecha_vencimiento = Dat_reader.Tables(0).Rows(0).Item(0)
                ilist_opciones_plantilla.util_cita_respuesta = Dat_reader.Tables(0).Rows(0).Item(1)
                ilist_opciones_plantilla.Util_Radic_General = Dat_reader.Tables(0).Rows(0).Item(2)
                ilist_opciones_plantilla.Valida_dest_Ext = Dat_reader.Tables(0).Rows(0).Item(3)
                ilist_opciones_plantilla.util_codigo_corto_radicado = Dat_reader.Tables(0).Rows(0).Item(4)
                ilist_opciones_plantilla.Util_activo_plantilla_codigo_simple = Dat_reader.Tables(0).Rows(0).Item(5)
                ilist_opciones_plantilla.util_estado_pendiente_rad = Dat_reader.Tables(0).Rows(0).Item(6)
                Solicita_Opcion_Plantilla_Radicacion = "YES"
                Exit Function
            Else
                Solicita_Opcion_Plantilla_Radicacion = "Imposible encontrar las opciones de la plantilla de radicación  de codigo (" & codigo_plantilla_radicacion & ")"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_Opcion_Plantilla_Radicacion = "Inconsistencia general funcion Solicita_Opcion_Plantilla_Radicacion " & ex.Message
        End Try
    End Function
    Function Retorna_Tipo_Plantilla(ByVal Id_Plantilla As String,
                                    ByRef Tipo_Plantilla As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select tipo_plantilla from system_plantilla_radicado where id_Plantilla=" &
            Id_Plantilla
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Tipo_Plantilla = " Funcion Retorna_Tipo_Plantilla COD-1   " & Result
                Return Retorna_Tipo_Plantilla
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Tipo_Plantilla = "Imposible encontrar tipo plantilla"
                Exit Function
            Else
                Tipo_Plantilla = Datset.Tables(0).Rows(0).Item(0).ToString
                Retorna_Tipo_Plantilla = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Tipo_Plantilla = "Inconsistencia general función Retorna_Tipo_Plantilla " & ex.Message
        End Try
    End Function
    Function Retorna_Tipo_Plantilla_nombre(ByVal nombre_plantilla As String,
                                           ByRef Tipo_Plantilla As String) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select tipo_plantilla from system_plantilla_radicado where nombre_plantilla_radicado='" &
            nombre_plantilla & "'"
            Dim Dat_set As New DataSet
            Dim Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_set)
            If Result <> "YES" Then
                Retorna_Tipo_Plantilla_nombre = "Función Retorna_Tipo_Plantilla_nombre dice  " & Result
                Exit Function
            End If
            If Dat_set.Tables(0).Rows.Count = 0 Then
                Retorna_Tipo_Plantilla_nombre = "Imposible encontrar tipo plantilla para la plantilla (" & nombre_plantilla & ")"
                Exit Function
            Else
                Tipo_Plantilla = Dat_set.Tables(0).Rows(0).Item(0).ToString
                Retorna_Tipo_Plantilla_nombre = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Retorna_Tipo_Plantilla_nombre = "Inconsistencia función Retorna_Tipo_Plantilla_nombre " & ex.Message
        End Try
    End Function
    Function SolicitaIdPlantillaRadicado(ByRef IdPlantillaRadicado As Integer,
                                         ByVal NombrePlantillaRadicado As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Solicita la identiicación de la plantilla con el nombre de la plantilla
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'NombrePlantillaRadicado     : Representa el nombre de la plantilla
        '
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        'IdPlantillaRadicado         : Retorna la idnetificación de la plantilla de radicación
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select * from system_plantilla_radicado where Nombre_Plantilla_Radicado='" &
            NombrePlantillaRadicado & "'"
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                SolicitaIdPlantillaRadicado = " Error Listando id plantilla   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                SolicitaIdPlantillaRadicado = "Imposible encontrar el nombre de la plantilla"
                Exit Function
            Else
                IdPlantillaRadicado = Datset.Tables(0).Rows(0).Item(0)
                SolicitaIdPlantillaRadicado = "YES"
                Exit Function
            End If
            SolicitaIdPlantillaRadicado = "YES"
        Catch ex As Exception
            SolicitaIdPlantillaRadicado = ex.Message
        End Try
    End Function
    Function Solicita_nombre_id_plantilla_radicación_interna_default(ByRef nombre_plantilla As String,
                                                                     ByRef id_plantilla_radicado As Integer,
                                                                     ByVal confirma_existencia As Integer) As String
        '--------------------------------------------------------------
        'Función : Retorna el nombre y la identifiación de la plantilla
        'default para radicación interna
        'Ing : Miguel Angel Urueta Miranda
        'Fecha 2017-11-30
        '---------------------------------------------------------------
        Try
            Dim Parametro_Consulta = "select Nombre_Plantilla_Radicado,id_Plantilla " &
          " from system_plantilla_radicado where Util_activo_plantilla_default_rad_interno=1"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("system_plantilla_radicado")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_nombre_id_plantilla_radicación_interna_default = "Funcion  Solicita_nombre_id_plantilla_radicación_interna_default dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                If confirma_existencia = 1 Then
                    Solicita_nombre_id_plantilla_radicación_interna_default = "Imposible encontrar el nombre de la plantilla predeterminada para radicación interna, contacte a su admistrador para que active la plantilla "
                    Exit Function
                Else
                    Solicita_nombre_id_plantilla_radicación_interna_default = "YES"
                    Exit Function
                End If

            Else
                nombre_plantilla = Datset.Tables(0).Rows(0).Item(0)
                id_plantilla_radicado = Datset.Tables(0).Rows(0).Item(1)
                Solicita_nombre_id_plantilla_radicación_interna_default = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_nombre_id_plantilla_radicación_interna_default = "Inconsistencia general función Solicita_nombre_id_plantilla_radicación_interna_default " & ex.Message
        End Try
    End Function
End Class
