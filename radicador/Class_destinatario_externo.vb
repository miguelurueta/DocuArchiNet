Public Class Class_destinatario_externo
    Function Solicita_datos_caracterizacion_solicitante_respuesta_externo(ByVal id_destinatrio As Integer, _
                                                                          ByVal nombre_plantilla As String, _
                                                                          ByRef nombre_destinatario_externo As String, _
                                                                          ByRef direccion_destinatario As String, _
                                                                          ByRef tel_destinatario As String) As String
        Try
            Dim Parametro_Consulta As String = "Select Nombre_Remitente,Direccion_Dest,Telefono from destinatario_externo where id_Dest_Ext='" & id_destinatrio & "'"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("destinatario_externo")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_datos_caracterizacion_solicitante_respuesta_externo = "Funcion  Solicita_datos_caracterizacion_solicitante_respuesta_externo dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                nombre_destinatario_externo = "NA"
                direccion_destinatario = "NA"
                tel_destinatario = "NA"
                Solicita_datos_caracterizacion_solicitante_respuesta_externo = "YES"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_destinatario_externo = "No informado"
                Else
                    nombre_destinatario_externo = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    direccion_destinatario = "No informado"    
                Else
                    direccion_destinatario = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    tel_destinatario = "No informado"                 
                Else
                    tel_destinatario = Datset.Tables(0).Rows(0).Item(2)
                End If
                Solicita_datos_caracterizacion_solicitante_respuesta_externo = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_datos_caracterizacion_solicitante_respuesta_externo = "Inconsistencia función Solicita_datos_caracterizacion_solicitante_respuesta_externo " & ex.Message
        End Try
    End Function
    Function verifica_existencia_destinatario_externo(ByVal Nombre_dest_externo As String, _
                                                      ByRef Cod_dest As Integer, _
                                                      ByVal nombre_plantilla As String, _
                                                      ByVal nombre_campo As String) As String
        '--------------------------------------------------------------------------------------
        'Funcion : Verifica la existencia del destinatario
        'Fecha : 2014-04-08
        'ing : Miguel Angel Urueta Miranda
        '--------------------------------------------------------------------------------------
        Try
            Dim Result As String = ""
            Dim conext As New conect.Dbase_Conction_Mysql_RA
            Dim datset As New DataSet
            Dim sqltext As String = "Select id_dest_ext from " & nombre_plantilla & " where " & nombre_campo & "='" & Nombre_dest_externo & "'"
            Result = conext.SELECTION_SELECT_FIELD(sqltext, datset)
            If Result <> "YES" Then
                verifica_existencia_destinatario_externo = "Inconsistencia tratando de determinar la existencia de nombre destinatario " & Result
                Exit Function
            End If
            If datset.Tables(0).Rows.Count = 0 Then
                verifica_existencia_destinatario_externo = "El destinatario o remitente  " & Nombre_dest_externo & " no se ecuentra registrado"
                Exit Function
            Else
                Cod_dest = datset.Tables(0).Rows(0).Item(0)
                verifica_existencia_destinatario_externo = "YES"
                Exit Function
            End If

        Catch ex As Exception
            verifica_existencia_destinatario_externo = "Inconsistencia general función verifica_existencia_destinatario_externo " & ex.Message
        End Try
    End Function
End Class
