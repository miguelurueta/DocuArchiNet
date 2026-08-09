Public Class Class_wf_configura_autoriza_tarea
    Function Solicita_configuracion_autorizacion(ByVal nombre_configuracion As String, _
                                                 ByRef ruta_archivo As String, _
                                                 ByVal estado_obliga_firma_archivo As Integer) As String
        '-----------------------------------------------------------------
        'Función : Retorna la configuración de las aprobaciones de
        'workflow la ruta donde ae guardan los archivo xmls
        'nombre_configuracion : Nombre de la configuracion valores
        'AUTORIZA_TAREA_WORKFLOW  : configuración para las aprobaciones
        'de las tareas
        'ruta_archivo: Retorna la ruta del archivo xml unc
        'estado_obliga_firma_archivo : Retorna el estado de obligatoriedad
        'de firmar el archivo XML
        'Fecha : 2019-10-25
        'Ing : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------
        Try
            Dim Sql_consulta = ""
            Sql_consulta = "SELECT ruta_archivo_xml,estado_obliga_firma_digital FROM wf_configura_autoriza_tarea" & _
            " WHERE  nombre_configuracion='" & nombre_configuracion & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("wf_autoriza_tarea")
            Result = ref.SELECTION_SELECT_FIELD(Sql_consulta, Datset)
            If Result <> "YES" Then
                Solicita_configuracion_autorizacion = "Funcion  Solicita_configuracion_autorizacion " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_configuracion_autorizacion = "Imposible encontrar los datos de la confguración  (" & nombre_configuracion & ")"
                Exit Function
            Else
                ruta_archivo = Datset.Tables(0).Rows(0).Item(0)
                estado_obliga_firma_archivo = Datset.Tables(0).Rows(0).Item(1)
                Solicita_configuracion_autorizacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_configuracion_autorizacion = "Inconsistencia general función Solicita_configuracion_autorizacion " & ex.Message
        End Try
    End Function
End Class
