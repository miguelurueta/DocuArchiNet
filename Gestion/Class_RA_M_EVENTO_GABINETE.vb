Public Class Class_RA_M_EVENTO_GABINETE
    Function Solicita_nombre_campo_gabinete(ByVal ID_M_AUTO_TIP_EVENT As Integer,
                                            ByVal ra_m_auto_evento_ID_M_AUTO_EVENTO As Integer,
                                            ByVal nombre_gabinete As String,
                                            ByRef nombre_campo As String) As String
        Try
            Dim Sql_consulta As String = "Select NOMBRE_GABINETE,NOMBRE_CAMPO From ra_m_evento_gabinete where RA_M_AUTO_TIP_EVENTO_ID_M_AUTO_TIP_EVENT=" &
               ID_M_AUTO_TIP_EVENT & " AND NOMBRE_GABINETE='" & nombre_gabinete & "' and ra_m_auto_evento_ID_M_AUTO_EVENTO=" & ra_m_auto_evento_ID_M_AUTO_EVENTO
            Dim Ref_Car_Conec_ra As New conect.Dbase_Conction_Mysql_RA
            Dim Result As String = ""
            Dim Datset As DataSet = New DataSet("ra_m_auto_tip_evento")
            Result = Ref_Car_Conec_ra.SELECTION_SELECT_FIELD(Sql_consulta, Datset)

            If Result <> "YES" Then
                Solicita_nombre_campo_gabinete = "Functión Solicita_nombre_campo_gabinete dice  " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                nombre_campo = Datset.Tables(0).Rows(0).Item(1)
                Solicita_nombre_campo_gabinete = "YES"
                Exit Function
            Else
                nombre_campo = ""
                Solicita_nombre_campo_gabinete = "YES"
                'Solicita_nombre_campo_gabinete = "Imposible encontrar el nombre  del campo de retorno  del evento (" & ID_M_AUTO_TIP_EVENT & ") para el gabinete (" & nombre_gabinete & "), relacione en (ra_m_evento_gabinete) "
                Exit Function
            End If
        Catch ex As Exception
            Solicita_nombre_campo_gabinete = "Inconsistencia general funcion Solicita_nombre_campo_gabinete " & ex.Message
        End Try
    End Function
End Class
