Public Class Class_usuario_radicador
    Function Retorna_Id_Nombre_sede_Empresa(ByVal id_usuario_radicador As Integer, _
                                            ByRef id_sede As Integer, _
                                            ByRef nombre_sede As String) As String
        '*****************************************************************************************
        'Funcion : retorna id y nombre de sede usuario radicador
        'Fecha 2014-09-02
        'Ingeniero : Miguel Angel Urueta Miranda
        '******************************************************************************************
        Try
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Parametro_Consulta As String = "SELECT se.ID_SEDES_EMPRESA,se.NOMBRE_SEDE FROM  usuario_radicador ur " & _
            " inner join sedes_empresa as se on (se.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=ur.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA and  se.ID_SEDES_EMPRESA=ur.SEDES_EMPRESA_ID_SEDES_EMPRESA) " & _
            " where id_usuario=" & id_usuario_radicador
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Retorna_Id_Nombre_sede_Empresa = " Error listado id sede destinatario funcion Retorna_Id_Nombre_sede_Empresa   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Retorna_Id_Nombre_sede_Empresa = "Imposible encontrar id sede nombre sede del id usuario radicador (" & id_usuario_radicador & ")"
                Exit Function
            Else
                id_sede = Datset.Tables(0).Rows(0).Item(0)
                nombre_sede = Datset.Tables(0).Rows(0).Item(1)
                Retorna_Id_Nombre_sede_Empresa = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Retorna_Id_Nombre_sede_Empresa = "Inconsistencia "
        End Try
    End Function
    Function Solicita_caraterizacion_usuario_radicador_gestion(ByVal id_usuario_radicador As Integer, _
                                                               ByRef nombre_usuario_radicador As String, _
                                                               ByRef cargo_usuario_radicador As String, _
                                                               ByRef sede_empresa As String) As String
        Try
            Dim Parametro_Consulta As String = "SELECT rdi.Nombre_Remitente, rdi.Cargo_Remite, se.NOMBRE_SEDE FROM  usuario_radicador ur " & _
          " inner join sedes_empresa as se on (se.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=ur.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA and  se.ID_SEDES_EMPRESA=ur.SEDES_EMPRESA_ID_SEDES_EMPRESA) " & _
          " left outer join remit_dest_interno as rdi on (rdi.id_Remit_Dest_Int=ur.Relacion_Gestion) " & _
          " where id_usuario=" & id_usuario_radicador
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As New DataSet
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_caraterizacion_usuario_radicador_gestion = " Error funcion Solicita_caraterizacion_usuario_radicador_gestion   " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_caraterizacion_usuario_radicador_gestion = "Imposible encontrar id sede usuario radicador (" & id_usuario_radicador & ")"
                Exit Function
            Else
                If Datset.Tables(0).Rows(0).IsNull(0) = True Then
                    nombre_usuario_radicador = ""
                Else
                    nombre_usuario_radicador = Datset.Tables(0).Rows(0).Item(0)
                End If
                If Datset.Tables(0).Rows(0).IsNull(1) = True Then
                    cargo_usuario_radicador = ""
                Else
                    cargo_usuario_radicador = Datset.Tables(0).Rows(0).Item(1)
                End If
                If Datset.Tables(0).Rows(0).IsNull(2) = True Then
                    sede_empresa = ""
                Else
                    sede_empresa = Datset.Tables(0).Rows(0).Item(2)
                End If
                Solicita_caraterizacion_usuario_radicador_gestion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_caraterizacion_usuario_radicador_gestion = "Inconsistencia general funcion Solicita_caraterizacion_usuario_radicador_gestion " & ex.Message
        End Try
    End Function
End Class
