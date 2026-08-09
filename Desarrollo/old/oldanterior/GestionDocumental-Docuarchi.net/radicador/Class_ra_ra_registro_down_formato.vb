Public Class Class_ra_ra_registro_down_formato
    Function Registra_descarga_formato_respuesta_solicitud(ByVal id_usuario_gestion As Integer, _
                                                           ByVal id_respuesta_radicado As Integer, _
                                                           ByVal id_usuario_firma As Integer, _
                                                           ByVal fecha_regitro_descarga As String) As String
        Try
            Dim sqlinsert As String = "Insert into ra_ra_registro_down_formato (remit_dest_interno_id_Remit_Dest_Int," & _
                                      "ra_respuesta_radicado_ID_RESPUESTA_RADICADO,id_usuario_firma, fecha_registro_descarga) values (" & _
                                      id_usuario_gestion & "," & id_respuesta_radicado & "," & id_usuario_firma & "," & _
                                      "'" & fecha_regitro_descarga & "')"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Result = ref2.SELECTION_INSERT_COMMAND(sqlinsert)
            If Result <> "YES" Then
                Registra_descarga_formato_respuesta_solicitud = "Función Registra_descarga_formato_respuesta_solicitud dice " & Result
                Exit Function
            Else
                Registra_descarga_formato_respuesta_solicitud = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Registra_descarga_formato_respuesta_solicitud = "Inconsistencia general función Registra_descarga_formato_respuesta_solicitud " & ex.Message
        End Try
    End Function
    Function Solicita_utltimo_usuario_firma_formato_descarga(ByVal id_respuesta As Integer, _
                                                             ByRef id_usuario_firma As Integer) As String
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "select id_usuario_firma from ra_ra_registro_down_formato " & _
                                               " Where ra_respuesta_radicado_ID_RESPUESTA_RADICADO=" & id_respuesta & _
                                               "  order by id_registro_dow desc  LIMIT 1 "
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_utltimo_usuario_firma_formato_descarga = "Función Solicita_utltimo_usuario_firma_formato_descarga dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_usuario_firma = 0
                Solicita_utltimo_usuario_firma_formato_descarga = "YES"
                Exit Function
            Else
                id_usuario_firma = Datset.Tables(0).Rows(0).Item(0)
                Solicita_utltimo_usuario_firma_formato_descarga = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_utltimo_usuario_firma_formato_descarga = "Inconsistencia general función Solicita_utltimo_usuario_firma_formato_descarga " & ex.Message
        End Try
    End Function
End Class
