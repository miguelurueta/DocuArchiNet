Public Class Class_ra_cert_certificado_has_remit_dest_interno
    Function Solicita_identificacion_certificado_usuario(ByVal id_usuario_gestion As Integer,
                                                         ByRef id_certificado As Integer) As String
        '-----------------------------------------------------
        'Funcion : Retorna el certificado asociaso al usuario
        'de gestión
        'Ing . Miguel Angel Urueta Miranda
        'Fecha : 2022-03-11
        '------------------------------------------------------
        Try
            Dim Parametro_Consulta As String = " SELECT  ra_cert_certificado_id_certificado " &
             " from ra_cert_certificado_has_remit_dest_interno where remit_dest_interno_id_Remit_Dest_Int=" &
              id_usuario_gestion
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cert_certificado_has_remit_dest_interno")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_identificacion_certificado_usuario = "Función Solicita_identificacion_certificado_usuario dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                id_certificado = 0
                Solicita_identificacion_certificado_usuario = "YES"
                Exit Function
            Else
                id_certificado = Datset.Tables(0).Rows(0).Item(0)
                Solicita_identificacion_certificado_usuario = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_identificacion_certificado_usuario = "Inconsistencia general función Solicita_identificacion_certificado_usuario " & ex.Message
        End Try
    End Function
End Class
