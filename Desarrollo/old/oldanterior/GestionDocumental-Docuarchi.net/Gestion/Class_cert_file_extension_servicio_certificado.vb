Public Class Class_cert_file_extension_servicio_certificado
    Function Solicita_extensiones_archivo_servicio_certificado(ByVal id_servicio_certificado As Integer,
                                                               ByRef stru_file_extensiones() As String) As String
        Try
            Dim Parametro_Consulta As String = " SELECT  file_extesion " &
             " from ra_cert_file_extension_servicio_certificado where ra_cert_servicio_certificado_id_cert_sevcio_certificado=" &
              id_servicio_certificado
            Dim Ref_Car_Conec As New conect.Dbase_Conction_Mysql_RA
            Dim Datset As DataSet = New DataSet("ra_cert_file_extension_servicio_certificado")
            Dim Result As String = ""
            Result = Ref_Car_Conec.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Solicita_extensiones_archivo_servicio_certificado = "Función Solicita_extensiones_archivo_servicio_certificado dice " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count = 0 Then
                Solicita_extensiones_archivo_servicio_certificado = "No existen extensiones de archivos para certificación digital"
                Exit Function
            Else
                Erase stru_file_extensiones
                For i As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                    ReDim Preserve stru_file_extensiones(i)
                    stru_file_extensiones(i) = Datset.Tables(0).Rows(0).Item(0)
                Next
                Solicita_extensiones_archivo_servicio_certificado = "YES"
                Exit Function
            End If
        Catch ex As Exception
            Solicita_extensiones_archivo_servicio_certificado = "Inconsistencia general funcion Solicita_extensiones_archivo_servicio_certificado " & ex.Message
        End Try
    End Function
End Class
