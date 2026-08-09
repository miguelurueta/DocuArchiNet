Public Class Class_ra_cd_usuarios_solicitudes_aprobacion
    Function Solicita_lista_usuarios_firmas_solicitudes_aprobacion(ByVal id_solicitud_aprobacion As Integer, _
                                                                   ByRef stru_usu_firmas_autoriza() As stru_usu_firmas_autorizadas) As String
        Try
            Erase stru_usu_firmas_autoriza
            Dim Parametro_Consulta As String = "Select rrr.Remit_Dest_Interno_id_remit_dest_Int, " & _
                                              "rdi.Nombre_Remitente, rdi.Cargo_Remite" & _
                                              " from ra_cd_usuarios_solicitudes_aprobacion as rrr " & _
                                              " left outer join remit_dest_interno as rdi on " & _
                                              " (rdi.id_Remit_Dest_Int=rrr.Remit_Dest_Interno_id_remit_dest_Int) " & _
                                               " where rrr.RA_CD_SOLICITUDES_APROBACION_ID_SOLICITUDES_APROBACION = " & _
                                               id_solicitud_aprobacion & " AND ESTADO_AUTORIZACION_FIRMA=1" & _
                                               " order by rdi.Nombre_Remitente"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_ra_realcion_firmas_autorizadas")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_usuarios_firmas_solicitudes_aprobacion = "Funcion  Solicita_lista_usuarios_firmas_solicitudes_aprobacion dice " & Result
                Exit Function
            End If
            Erase stru_usu_firmas_autoriza
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_usuarios_firmas_solicitudes_aprobacion = "YES"
                Exit Function
            Else
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_usu_firmas_autoriza(i)
                    stru_usu_firmas_autoriza(i).id_usuario_autorizado = Datset.Tables(0).Rows(i).Item(0)
                    If Datset.Tables(0).Rows(i).IsNull(1) = True Then
                        stru_usu_firmas_autoriza(i).nombre_usuario_autorizado = ""
                    Else
                        stru_usu_firmas_autoriza(i).nombre_usuario_autorizado = Datset.Tables(0).Rows(i).Item(1)
                    End If
                    If Datset.Tables(0).Rows(i).IsNull(2) = True Then
                        stru_usu_firmas_autoriza(i).nombre_cargo_autorizado = ""
                    Else
                        stru_usu_firmas_autoriza(i).nombre_cargo_autorizado = Datset.Tables(0).Rows(i).Item(2)
                    End If
                Next
                Solicita_lista_usuarios_firmas_solicitudes_aprobacion = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_usuarios_firmas_solicitudes_aprobacion = "Inconsistencia general función Solicita_lista_usuarios_firmas_solicitudes_aprobacion"
        End Try
    End Function
    
End Class
