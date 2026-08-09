Public Structure stru_usu_firmas_autorizadas
    Dim id_usuario_autorizado As Integer
    Dim nombre_usuario_autorizado As String
    Dim nombre_cargo_autorizado As String
End Structure
Public Class Class_relacion_firmas_autorizadas
    Function Solicita_lista_usuarios_permitidos_firma(ByVal id_usuario_autorizado As Integer, _
                                                ByRef stru_usu_firmas_autoriza() As stru_usu_firmas_autorizadas) As String
        Try
            Dim Parametro_Consulta As String = "Select rrr.remit_dest_interno_id_Remit_Dest_Int, " & _
                                               "rdi.Nombre_Remitente, rdi.Cargo_Remite" & _
                                               " from ra_ra_realcion_firmas_autorizadas as rrr " & _
                                               " left outer join remit_dest_interno as rdi on " & _
                                               " (rdi.id_Remit_Dest_Int=rrr.remit_dest_interno_id_Remit_Dest_Int) " & _
                                                " where rrr.id_usuario_autorizado = " & id_usuario_autorizado & _
                                                " order by rdi.Nombre_Remitente"
            Dim ref2 As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_ra_realcion_firmas_autorizadas")
            Dim Result = ref2.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_lista_usuarios_permitidos_firma = "Funcion  Solicita_lista_usuarios_permitidos_firma dice " & Result
                Exit Function
            End If
            Erase stru_usu_firmas_autoriza
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_lista_usuarios_permitidos_firma = "YES"
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
                Solicita_lista_usuarios_permitidos_firma = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_lista_usuarios_permitidos_firma = "Inconsistencia general función Solicita_lista_usuarios_permitidas " & ex.Message
        End Try
    End Function

End Class
