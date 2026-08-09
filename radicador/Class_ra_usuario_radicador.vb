Public Class Class_ra_usuario_radicador
    Function Solicita_id_usuario_gestion_relacion_usuario_radicador(ByVal id_usuario_radicador As Integer,
                                                                    ByRef id_usuario_gestion As Integer) As String
        '---------------------------------------------------------------------------
        'Funcion : Solicita la relación del usuario radicador con el usuario de
        '          gestión
        '---------------------------------------------------------------------------
        '                           PARAMETROS  
        '---------------------------------------------------------------------------
        'id_usuario_radicador  : Respresenta la identificación del usuario radicador
        '
        '---------------------------------------------------------------------------
        '                           RETORNO
        '---------------------------------------------------------------------------
        'id_usuario_gestion    : Retorna el usuario de gestión relacionado al usuario
        '                        de gestión
        '---------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '---------------------------------------------------------------------------
        'Fecha                 : 2023-04-01
        'Elabora               : Miguel Angel Urueta Miranda
        '----------------------------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = "Select  Relacion_Gestion " &
               " from usuario_radicador  where id_usuario=" & id_usuario_radicador
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Dat_reader As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Dat_reader)
            If Result <> "YES" Then
                Solicita_id_usuario_gestion_relacion_usuario_radicador = " Función Solicita_id_usuario_gestion_relacion_usuario_radicador dice   " & Result
                Exit Function
            End If
            If Dat_reader.Tables(0).Rows.Count = 0 Then
                Solicita_id_usuario_gestion_relacion_usuario_radicador = "Imposible encontrar  el usuario de gestión relacionado con el usuario radicador (" & id_usuario_radicador & ")"
                Exit Function
            Else
                id_usuario_gestion = Dat_reader.Tables(0).Rows(0).Item(0)
                Solicita_id_usuario_gestion_relacion_usuario_radicador = "YES"
                Exit Function
            End If

        Catch ex As Exception
            Solicita_id_usuario_gestion_relacion_usuario_radicador = "Inconsistencia general funcion Solicita_id_usuario_gestion_relacion_usuario_radicador " & ex.Message
        End Try
    End Function
End Class
